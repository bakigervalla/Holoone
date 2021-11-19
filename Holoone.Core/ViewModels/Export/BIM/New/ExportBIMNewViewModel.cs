using Autodesk.Navisworks.Api;
using Caliburn.Micro;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels.Home;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        }

        #region navigation

        public string _state = "AddLayer";
        public string State { get => _state; set { _state = value; NotifyOfPropertyChange("State"); } }

        public void NavigateToSelectModelPage()
        {
            State = "SelectModel";

            QueryNavisModel().AsResult();

            SelectedFiles = new List<string>();
        }

        public void NavigateToDestinationPage()
        {
            if (SelectedFiles.Count == 0)
            {
                MessageBox.Show("Please, select a model");
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

        #endregion

        public async Task AddLayerAsync()
        {

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


        public async Task GetFoldersAsync()
        {
            try
            {
                await _eventAggregator.PublishOnUIThreadAsync(true);

                var response = await _exportService.GetCompanyMediaFolderContent(UserLogin, 0);

                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    MediaFiles = await response.GetJsonAsync<IList<MediaFile>>();
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

                var response = await _exportService.GetCompanyMediaFolderContent(UserLogin, mediaFile.Id);

                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = await response.GetJsonAsync<IList<MediaFile>>();
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

    
                foreach (var file in SelectedFiles)
                {
                    var valParts = new NameValueCollection
                    {
                        { "display_name", Path.GetFileNameWithoutExtension(file) },
                        { "parent_folder", SelectedFolder.Id == 0 ? "null" : SelectedFolder.Id.ToString() },
                        { "file_extension", Path.GetExtension(file).Replace(".", "") },
                    };

                    var valColl = new NameValueCollection
                    {
                        { file, "" }
                    };

                    await _exportService.ExportModelFormCompositionAsync(Instance.UserLogin, valParts, valColl, null);

                    MessageBox.Show("Uploaded successfully.");

                    await NavigationService.GoTo<HomeViewModel>();
                }
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

    }
}
