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
        }


        #region navigation

        public string _state = "SelectLayer";
        public string State { get => _state; set { _state = value; NotifyOfPropertyChange("State"); } }

        public void NavigateToAddLayerlPage()
        {
            State = "AddLayer";
        }

        public void NavigateToSelectModelPage()
        {
            State = "SelectModel";

            QueryNavisModel().AsResult();

            SelectedFiles = new List<string>();
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

        public ModelItem SelectedModelItem { get; set; }

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

                SelectedMediaFile = MediaFiles.FirstOrDefault(x => x.IsSelected);

                if(SelectedMediaFile == null)
                {
                    MessageBox.Show("Please selecte a model to continue");
                    return;
                }

                var result = await _apiService.Get3DModel(UserLogin, SelectedMediaFile);
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
        }

        public void AttachModelItem(BIMLayer bimLayer)
        {
            ModelSelectionWindow window = new ModelSelectionWindow() { DataContext = this };

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
                if (SelectedFolder == null)
                {
                    MessageBox.Show("Please, select a destination folder");
                    return;
                }

                await _eventAggregator.PublishOnUIThreadAsync(true);

                //* BIMLayers = new ObservableCollection<BIMLayer>(_navisService.ExportToFBX(BIMLayers));

                BIMLayers = new ObservableCollection<BIMLayer>(_navisService.ExportToNWD(BIMLayers));

                var valParts = new NameValueCollection
                    {
                        { "model_name", BIMModel.ModelName },
                        { "primary_layer_index", BIMLayers.ToList().FindIndex(x=> x.IsDefault).ToString() },
                        { "parent_folder", SelectedFolder.Id == 0 ? "null" : SelectedFolder.Id.ToString() },
                    };

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
