using Autodesk.Navisworks.Api;
using Caliburn.Micro;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
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
using System.Threading.Tasks;
using System.Windows;

namespace HolooneNavis.ViewModels.Export.BIM.Existing
{
    public class ExportBIMExistingViewModel : BaseViewModel
    {
        private readonly IExportService _apiService;
        private readonly INavisService _navisService;
        private readonly IEventAggregator _eventAggregator;

        public ExportBIMExistingViewModel(
            IHoloNavigationService navigationService,
            IExportService apiService,
            INavisService navisService,
            IEventAggregator eventAggregator
            )
        {
            _apiService = apiService;
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
        public IEnumerable<BIM3DModel> BIM3DModels { get; set; }

        // 
        private int? OriginalPrimaryLayerId { get; set; }
        public List<int> DeletedLayers;

        #endregion

        private async Task Get3DModels()
        {
            try
            {
                await _eventAggregator.PublishOnUIThreadAsync(true);

                MediaFiles = await _apiService.GetCompany3DModels(UserLogin);
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

                BIM3DModels = await _apiService.Get3DModelById(UserLogin, SelectedMediaFile.Id);

                BIMModel.ModelName = SelectedMediaFile.DisplayName;
                foreach (var item in BIM3DModels)
                    BIMLayers.Add(new BIMLayer
                    {
                        Name = Path.GetFileNameWithoutExtension(item.Name),
                        FilePath = item.Name,
                        IsDefault = item.IsPrimary,
                        IsSet = true
                    });

                // get original primary layer Id
                OriginalPrimaryLayerId = _bimLayers.First(x => x.IsDefault)?.Id;

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
                DeletedLayers.Add(bimLayer.Id);
        }

        public void AttachModelItem(BIMLayer bimLayer)
        {
            ModelSelectionWindow window = new ModelSelectionWindow(false) { DataContext = this };

            if (window.ShowDialog() ?? true)
            {
                bimLayer.ModelItem = SelectedModelItem;
                bimLayer.IsSet = true;
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

        public async Task ExportAsync()
        {
            try
            {
                await _eventAggregator.PublishOnUIThreadAsync(true);

                BIMLayers = new ObservableCollection<BIMLayer>(_navisService.ExportToNWD(BIMLayers));

                // prepare input
                int? primaryLayerId = BIMLayers.First(x => x.IsDefault)?.Id;

                var valParts = new NameValueCollection
                    {
                        { "model_name", BIMModel.ModelName },
                        { "primary_layer_id",  primaryLayerId.ToString()},
                        { "previous_primary_layer_id", primaryLayerId.Equals(OriginalPrimaryLayerId) ? "" : OriginalPrimaryLayerId.ToString() },
                        { "layers_to_delete", BIMLayers.ToList().FindIndex(x=> x.IsDefault).ToString() },
                        { "layers_to_update", BIMLayers.ToList().FindIndex(x=> x.IsDefault).ToString() },
                    };

                //var valParts = new NameValueCollection
                //    {
                //        { "model_name", BIMModel.ModelName },
                //        { "parent_folder", SelectedFolder.Id == 0 ? "null" : SelectedFolder.Id.ToString() },
                //        { "primary_layer_id", BIMLayers.ToList().FindIndex(x=> x.IsDefault).ToString() },
                //        { "previous_primary_layer_id", BIMLayers.ToList().FindIndex(x=> x.IsDefault).ToString() },
                //        { "layers_to_delete", BIMLayers.ToList().FindIndex(x=> x.IsDefault).ToString() },
                //        { "layers_to_update", BIMLayers.ToList().FindIndex(x=> x.IsDefault).ToString() },
                //    };

                NameValueCollection valColl = new NameValueCollection();

                foreach (var layer in BIMLayers)
                {
                    valColl.Add(layer.FilePath, "");
                }

                await _apiService.ExportModelFormCompositionAsync(Instance.UserLogin, valParts, valColl, null, "media/bim/add/", "layers");

                MessageBox.Show("Uploaded successfully.");

                await NavigationService.GoTo<HomeViewModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                try
                {
                    foreach (var layer in BIMLayers)
                        File.Delete(layer.FilePath);
                }
                catch { }

                await _eventAggregator.PublishOnUIThreadAsync(false);
            }
        }

    }
}
