using Autodesk.Navisworks.Api;
using Caliburn.Micro;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using HolooneNavis.Models;
using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels.Home;
using HolooneNavis.Views.Export.BIM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HolooneNavis.ViewModels.Export.BIM.New
{
    public class ExportBIMNewViewModel : BaseViewModel
    {
        private readonly IExportService _exportService;
        private readonly INavisService _navisService;
        private readonly IEventAggregator _eventAggregator;

        public ExportBIMNewViewModel(
            IHoloNavigationService navigationService,
            IExportService exportService,
            INavisService navisService,
            IEventAggregator eventAggregator
            )
        {
            _exportService = exportService;
            _navisService = navisService;
            _eventAggregator = eventAggregator;

            QueryNavisModel().AsResult();
        }

        #region navigation

        public string _state = "AddLayer";
        public string State { get => _state; set { _state = value; NotifyOfPropertyChange("State"); } }

        public void NavigateToSelectModelPage()
        {
            State = "SelectModel";

            SelectedFiles = new List<string>();
        }

        public void NavigateToDestinationPage()
        {
            if (string.IsNullOrEmpty(BIMModel.ModelName))
            {
                MessageBox.Show("Model is empty. Please add a model name and layers.");
                return;
            }

            State = "SelectDestination";

            GetFoldersAsync().AsResult();
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

        private BIMModel _bimModel = new();
        public BIMModel BIMModel { get => _bimModel; set { _bimModel = value; NotifyOfPropertyChange(nameof(BIMModel)); } }

        private ObservableCollection<BIMLayer> _bimLayers = new ObservableCollection<BIMLayer>();
        public ObservableCollection<BIMLayer> BIMLayers { get => _bimLayers; set { _bimLayers = value; NotifyOfPropertyChange(nameof(BIMLayers)); } }

        public ModelItem SelectedModelItem { get; set; }

        #endregion

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
            ModelSelectionWindow window = new ModelSelectionWindow(true) { DataContext = this };

            if (window.ShowDialog() ?? true)
            {
                bimLayer.ModelItem = SelectedModelItem;
                bimLayer.IsSet = true;
                bimLayer.Name = getLayerName(SelectedModelItem);

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

        public void SaveBIMModelAndLayers()
        {
            // save new model
            BIMModel.BIMLayers = BIMLayers;
            // navigate to destination folders
            NavigateToDestinationPage();
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

        public async Task GetFoldersAsync()
        {
            try
            {
                await _eventAggregator.PublishOnUIThreadAsync(true);

                MediaFiles = await (await _exportService.EnsureTokenAsync(Instance.UserLogin)).GetCompanyMediaFolderContent(Instance.UserLogin, 0);

                if (MediaFiles != null)
                {
                    MediaFiles = MediaFiles.Where(x => x.MediaFileType == "folder").ToList();
                    MediaFiles = MediaFiles.Prepend(new MediaFile { Id = 0, DisplayName = "Root Folder" });
                }
                else
                    MessageBox.Show("Could not retrieve folders");

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

        public async Task GetSubfoldersAsync(MediaFile mediaFile)
        {
            try
            {
                if (mediaFile.Id == 0)
                {
                    SelectedFolder = mediaFile;
                    return;
                }

                await _eventAggregator.PublishOnUIThreadAsync(true);

                var result = await (await _exportService.EnsureTokenAsync(Instance.UserLogin)).GetCompanyMediaFolderContent(UserLogin, mediaFile.Id);

                if (result != null)
                {
                    mediaFile.SubFolders = result.Where(x => x.MediaFileType == "folder").ToList();
                    SelectedFolder = mediaFile;
                }
                else
                    MessageBox.Show("Could not retrieve folders");

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

                await (await _exportService.EnsureTokenAsync(Instance.UserLogin)).ExportModelFormCompositionAsync(Instance.UserLogin, valParts, valColl, null, "media/bim/add/", "layers");

                foreach (var layer in BIMLayers)
                    File.Delete(layer.FilePath);

                await _eventAggregator.PublishOnUIThreadAsync(false);

                MessageBox.Show("Uploaded successfully.");

                await NavigationService.GoTo<HomeViewModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
