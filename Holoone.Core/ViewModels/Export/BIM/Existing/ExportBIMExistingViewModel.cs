using Autodesk.Navisworks.Api;
using Caliburn.Micro;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using HolooneNavis.Helpers;
using HolooneNavis.Models;
using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels.Home;
using HolooneNavis.Views.Export.BIM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HolooneNavis.ViewModels.Export.BIM.Existing
{
    public class ExportBIMExistingViewModel : BaseViewModel
    {
        private readonly IExportService _exportService;
        private readonly INavisService _navisService;
        private readonly IEventAggregator _eventAggregator;

        public ExportBIMExistingViewModel(
            IHoloNavigationService navigationService,
            IExportService exportService,
            INavisService navisService,
            IEventAggregator eventAggregator
            )
        {
            _exportService = exportService;
            _navisService = navisService;
            _eventAggregator = eventAggregator;

            Get3DModels().AsResult();

            QueryNavisModel().AsResult();
        }

        #region navigation

        public string _state = "SelectLayer";
        public string State { get => _state; set { _state = value; NotifyOfPropertyChange("State"); } }

        public void NavigateToSelectModelPage()
        {
            State = "SelectModel";

            QueryNavisModel().AsResult();

            SelectedFiles = new List<string>();
        }

        public void NavigateToAddLayerlPage()
        {
            State = "AddLayer";
        }

        #endregion

        #region properties

        private ModelItemCollection _navisItems;
        public ModelItemCollection NavisItems { get => _navisItems; set { _navisItems = value; NotifyOfPropertyChange(nameof(NavisItems)); } }

        public IList<string> SelectedFiles { get; set; }

        private MediaFile _selectedFolder;
        public MediaFile SelectedFolder { get => _selectedFolder; set { _selectedFolder = value; NotifyOfPropertyChange(nameof(SelectedFolder)); } }

        private IEnumerable<MediaFile> _mediaFiles;
        public IEnumerable<MediaFile> MediaFiles { get => _mediaFiles; set { _mediaFiles = value; NotifyOfPropertyChange(nameof(MediaFiles)); } }

        private MediaFile _selectedMediaFile;
        public MediaFile SelectedMediaFile { get => _selectedMediaFile; set { _selectedMediaFile = value; NotifyOfPropertyChange(nameof(SelectedMediaFile)); } }

        private BIMModel _bimModel = new();
        public BIMModel BIMModel { get => _bimModel; set { _bimModel = value; NotifyOfPropertyChange(nameof(BIMModel)); } }

        private ObservableCollection<BIMLayer> _bimLayers = new ObservableCollection<BIMLayer>();
        public ObservableCollection<BIMLayer> BIMLayers { get => _bimLayers; set { _bimLayers = value; NotifyOfPropertyChange(nameof(BIMLayers)); } }

        //
        public ModelItem SelectedModelItem { get; set; }
        public IEnumerable<BIM3DLayer> BIM3DLayers { get; set; }
        private int ParentFolderId { get; set; }


        // 
        private int? OriginalPrimaryLayerId { get; set; }
        private List<int> LayersToDelete = new();

        #endregion

        private async Task Get3DModels()
        {
            try
            {
                await _eventAggregator.PublishOnUIThreadAsync(true);

                MediaFiles = await (await _exportService.EnsureTokenAsync(Instance.UserLogin)).GetCompany3DModels(UserLogin);
                MediaFiles = MediaFiles.Where(x => x.MediaSubtype.Equals("bim", StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                await _eventAggregator.PublishOnUIThreadAsync(false);
            }
        }

        public async Task Get3DModelAsync()
        {
            try
            {
                await _eventAggregator.PublishOnUIThreadAsync(true);

                if (SelectedMediaFile == null)
                {
                    MessageBox.Show("Please selecte a model to continue");
                    return;
                }

                BIMLayers.Clear();

                BIM3DLayers = await (await _exportService.EnsureTokenAsync(Instance.UserLogin)).Get3DModelById(UserLogin, SelectedMediaFile.Id);

                BIMModel.ModelName = SelectedMediaFile.DisplayName;
                ParentFolderId = SelectedMediaFile.ParentFolderId;

                foreach (var item in BIM3DLayers)
                    BIMLayers.Add(new BIMLayer
                    {
                        Name = Path.GetFileNameWithoutExtension(item.Name),
                        OriginalName = Path.GetFileNameWithoutExtension(item.Name),
                        FilePath = item.Name,
                        IsDefault = item.IsPrimary,
                        IsSet = true,
                        Id = item.CadModelId
                    });

                // get original primary layer Id
                OriginalPrimaryLayerId = BIMLayers.FirstOrDefault(x => x.IsDefault)?.Id;

                State = "AddLayer";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                await _eventAggregator.PublishOnUIThreadAsync(false);
            }
        }

        private async Task QueryNavisModel()
        {
            try
            {
                await _eventAggregator.PublishOnUIThreadAsync(true);

                NavisItems = await _navisService.GetModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                await _eventAggregator.PublishOnUIThreadAsync(false);
            }
        }

        public void GetSelectedModelItemAsync(ModelItem model)
        {
            if (model.Model == null || string.IsNullOrEmpty(model.Model.SourceFileName) || !File.Exists(model.Model.SourceFileName))
                SelectedFiles.Add(Autodesk.Navisworks.Api.Application.ActiveDocument.FileName);
            else
                SelectedFiles.Add(model.Model.SourceFileName);
        }

        public void AddNewLayer()
        {
            if (BIMLayers.Count == 0)
                BIMLayers.Add(new BIMLayer { Name = "", IsDefault = true });
            else
                BIMLayers.Add(new BIMLayer { Name = "", IsDefault = false });
        }

        public void RemoveModelItem(BIMLayer bimLayer)
        {
            BIMLayers.Remove(bimLayer);

            if (bimLayer.IsDefault)
            {
                var layer = BIMLayers.FirstOrDefault();
                if (layer != null)
                    layer.IsDefault = true;
            }

            if (bimLayer.Id > 0)
                LayersToDelete.Add(bimLayer.Id);
        }

        public void AttachModelItem(BIMLayer bimLayer)
        {
            ModelSelectionWindow window = new ModelSelectionWindow(false) { DataContext = this };

            if (window.ShowDialog() ?? true)
            {
                bimLayer.ModelItem = SelectedModelItem;
                bimLayer.IsSet = true;
                // bimLayer.Name = getLayerName(SelectedModelItem);
            }
        }

        private string getLayerName(ModelItem model)
        {
            if (!string.IsNullOrEmpty(model.DisplayName))
                return model.DisplayName;
            else if (string.IsNullOrEmpty(model.Descendants.FirstOrDefault()?.DisplayName))
                return model.Descendants.FirstOrDefault()?.DisplayName;
            else
                return model.ClassDisplayName;
        }

        public async Task ExportAsync()
        {
            try
            {
                if (BIMLayers.FirstOrDefault(x => x.IsDefault) == null)
                {
                    MessageBox.Show("Please choose a default layer");
                    return;
                }

                await _eventAggregator.PublishOnUIThreadAsync(true);

                BIMLayers = new ObservableCollection<BIMLayer>(_navisService.ExportToNWD(BIMLayers));

                var layersToUpdate = BIMLayers.Where(x => x.ModelItem != null && x.Id > 0).Select((s, i) => new { s, i }).ToDictionary(x => x.i.ToString(), x => x.s.Id);

                var layers_to_update = new StringBuilder("{");
                string separator = "";

                foreach (var itm in layersToUpdate)
                {
                    layers_to_update.Append(separator);
                    layers_to_update.Append("\"" + itm.Key + "\"");
                    layers_to_update.Append(":");
                    layers_to_update.Append(itm.Value);
                    separator = ",";
                }
                layers_to_update.Append("}");

                var layers_to_delete = new StringBuilder("[");

                foreach (var key in LayersToDelete)
                {
                    layers_to_delete.Append(key);
                }
                layers_to_delete.Append("]");

                var payload = new Dictionary<string, dynamic>
                    {
                        { "model_name", BIMModel.ModelName },
                        { "primary_layer_id",  BIMLayers.Single(x => x.IsDefault).Id.ToString() },
                        { "previous_primary_layer_id", OriginalPrimaryLayerId.HasValue ? OriginalPrimaryLayerId.ToString() : BIMLayers.Single(x => x.IsDefault).Id.ToString() },
                        { "parent_folder", ParentFolderId.ToString() },
                        { "layers_to_update", layers_to_update.ToString() },
                        { "layers_to_delete", layers_to_delete.ToString()},
                        { "model_type", null },
                    };

                NameValueCollection files = new NameValueCollection();

                foreach (var layer in BIMLayers)
                    files.Add(layer.FilePath, layer.Name);

                await (await _exportService.EnsureTokenAsync(Instance.UserLogin))
                                            .ExportExistingBIMAsync(Instance.UserLogin, SelectedMediaFile.Id, payload, files);

                foreach (var layer in BIMLayers)
                    File.Delete(layer.FilePath);

                await _eventAggregator.PublishOnUIThreadAsync(false);

                MessageBox.Show("Project exported successfully.");

                if (Util.Anchors != null)
                    Util.Anchors.Clear();

                await NavigationService.GoTo<HomeViewModel>();

            }
            catch (Exception ex)
            {
                await _eventAggregator.PublishOnUIThreadAsync(false);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
