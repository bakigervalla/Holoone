using Autodesk.Navisworks.Api;
using Caliburn.Micro;
using Holoone.Api.Models;
using Holoone.Api.Services;
using Holoone.Api.Services.Export;
using Holoone.Api.Services.Interfaces;
using HolooneNavis.Helpers;
using HolooneNavis.Models;
using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels.Home;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HolooneNavis.ViewModels.Export.Default
{
    public class ExportDefaultViewModel : BaseViewModel
    {
        private readonly IExportService _exportService;
        private readonly INavisService _navisService;
        private readonly IEventAggregator _eventAggregator;

        public ExportDefaultViewModel(
            IHoloNavigationService navigationService,
            IExportService exportService,
            INavisService navisService,
            IEventAggregator eventAggregator
            )
        {
            _exportService = exportService;
            _navisService = navisService;
            _eventAggregator = eventAggregator;

            try
            {
                // TraverseTreeParallelForEach(@"e:\Users\BGERVALLA\Downloads\Autodesk\Navisworks");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        #region properties

        private ProcessingParams _processingParams = new ProcessingParams();
        public ProcessingParams ProcessingParams { get => _processingParams; set { _processingParams = value; NotifyOfPropertyChange(nameof(ProcessingParams)); } }

        private IEnumerable<MediaFile> _mediaFiles;
        public IEnumerable<MediaFile> MediaFiles { get => _mediaFiles; set { _mediaFiles = value; NotifyOfPropertyChange(nameof(MediaFiles)); } }

        private ModelItemCollection _navisItems;
        public ModelItemCollection NavisItems { get => _navisItems; set { _navisItems = value; NotifyOfPropertyChange(nameof(NavisItems)); } }

        private MediaFile _selectedFolder;
        public MediaFile SelectedFolder { get => _selectedFolder; set { _selectedFolder = value; NotifyOfPropertyChange(nameof(SelectedFolder)); } }

        private ObservableCollection<BIMLayer> _bimLayers = new ObservableCollection<BIMLayer>();
        public ObservableCollection<BIMLayer> BIMLayers { get => _bimLayers; set { _bimLayers = value; NotifyOfPropertyChange(nameof(BIMLayers)); } }

        #endregion

        #region navigation

        public string _state = "Selection";
        public string State { get => _state; set { _state = value; NotifyOfPropertyChange("State"); } }

        public void NavigateToNavisSelectionPage()
        {
            State = "NavisSelection";

            _ = QueryNavisModel().AsResult();
        }

        public void NavigateToFoldersPage()
        {
            if (BIMLayers.Count == 0)
            {
                _ = MessageBox.Show("Please, select a model");
                return;
            }

            State = "FolderSelection";

            _ = GetFoldersAsync().AsResult();
        }

        #endregion

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
            BIMLayers.Clear();
            BIMLayers.Add(new BIMLayer { Name = getLayerName(model), ModelItem = model });
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

                var result = await (await _exportService.EnsureTokenAsync(Instance.UserLogin)).GetCompanyMediaFolderContent(Instance.UserLogin, mediaFile.Id);

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

                BIMLayers = new ObservableCollection<BIMLayer>(_navisService.ExportToNWD(BIMLayers));

                foreach (var layer in BIMLayers)
                {
                    var valParts = new NameValueCollection
                    {
                        { "display_name", Path.GetFileNameWithoutExtension(layer.FilePath) },
                        { "parent_folder", SelectedFolder.Id == 0 ? "null" : SelectedFolder.Id.ToString() },
                        { "file_extension", Path.GetExtension(layer.FilePath).Replace(".", "") },
                        { "processing_params", JsonConvert.SerializeObject(ProcessingParams) },
                    };

                    NameValueCollection valColl = new NameValueCollection
                    {
                        { layer.FilePath, "" }
                    };

                   await (await _exportService.EnsureTokenAsync(Instance.UserLogin)).ExportDefaultModelAndNewBIMAsync(Instance.UserLogin, valParts, valColl, ProcessingParams, "media/add/file/", "file");
                }

                foreach (BIMLayer layer in BIMLayers)
                    File.Delete(layer.FilePath);

                await _eventAggregator.PublishOnUIThreadAsync(false);

                MessageBox.Show("Uploaded successfully.");

                if (Util.Anchors != null)
                    Util.Anchors.Clear();

                await NavigationService.GoTo<HomeViewModel>();

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

        public List<Folder> Folders { get; set; } = new List<Folder>();
        private IEnumerable<OFile> GetSelectedFiles()
        {
            foreach (var file in Folders.SelectMany(x => x.ListofFiles.Where(f => f.IsSelected)))
                yield return file;
        }

        private void TraverseTreeParallelForEach(string root)
        {
            var rootDirName = System.IO.Path.GetFileNameWithoutExtension(root);
            if (!Directory.Exists(root))
            {
                throw new ArgumentException();
            }

            Folder rootFolder = new Folder { Name = rootDirName, ListofFiles = new List<OFile>() };

            // root folder
            foreach (var file in Directory.GetFiles(root))
            {
                FileInfo oFileInfo = new FileInfo(file);
                rootFolder.ListofFiles.Add(new OFile { Name = oFileInfo.Name, ext = oFileInfo.Extension, path = file });
            }

            Folders.Add(rootFolder);

            // subfolders
            foreach (var dir in Directory.GetDirectories(root))
            {
                var dirName = System.IO.Path.GetFileNameWithoutExtension(dir);
                Folder newFolder = new Folder { Name = dirName, ListofFiles = new List<OFile>() };

                foreach (var file in Directory.GetFiles(dir))
                {
                    FileInfo oFileInfo = new FileInfo(file);
                    newFolder.ListofFiles.Add(new OFile { Name = oFileInfo.Name, ext = oFileInfo.Extension, path = file });
                }

                Folders.Add(newFolder);
            }
        }

    }

}
