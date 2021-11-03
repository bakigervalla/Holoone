using Caliburn.Micro;
using Holoone.Api.Models;
using Holoone.Api.Services;
using Holoone.Api.Services.Interfaces;
using Holoone.Core.Services.Interfaces;
using Holoone.Core.ViewModels.Home;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Holoone.Core.ViewModels.Export.Default
{
    public class ExportDefaultViewModel : BaseViewModel
    {
        private readonly IExportService _exportService;
        private readonly IEventAggregator _eventAggregator;

        public ExportDefaultViewModel(
            IHoloNavigationService navigationService,
            IExportService exportService,
            IEventAggregator eventAggregator
            )
        {
            _exportService = exportService;
            _eventAggregator = eventAggregator;

            try
            {
                TraverseTreeParallelForEach(@"e:\Users\BGERVALLA\Downloads\Autodesk\Navisworks");
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

        #endregion

        #region navigation

        public string _state = "Selection";
        public string State { get => _state; set { _state = value; NotifyOfPropertyChange("State"); } }

        public void NavigateToFoldersPage()
        {
            State = "FolderSelection";

            Task.Run(async () => await GetFoldersAsync());
        }

        public void NavigateToExportPage()
        {
            State = "ExportData";
        }

        #endregion

        public async Task GetFoldersAsync()
        {
            try
            {
                await _eventAggregator.PublishOnUIThreadAsync(true);

                var response = await _exportService.GetCompanyMediaFolderContent(Instance.UserLogin, 0);

                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    MediaFiles = await response.GetJsonAsync<IList<MediaFile>>();
                    // add children to show arrow
                   // MediaFiles.ToList().ForEach(x => x.SubFolders = new List<MediaFile> { new MediaFile() });
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
                await _eventAggregator.PublishOnUIThreadAsync(true);

                //List<MediaFile> _folders = MediaFiles.ToList();
                //var _subFolders = _folders.Where(x => x.MediaFiles != null && x.MediaFiles.Count > 0).SelectMany(x => x.MediaFiles);

                //if (_subFolders != null)
                //    _folders = _folders.Concat(_subFolders).ToList();

                //var folder = _folders.Single(x => x.Id == mediaFile.Id);

                var response = await _exportService.GetCompanyMediaFolderContent(Instance.UserLogin, mediaFile.Id);

                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = await response.GetJsonAsync<IList<MediaFile>>();
                    mediaFile.SubFolders = result.Where(x => x.MediaFileType == "folder").ToList();
                    // add children to show arrow
                    // mediaFile.SubFolders.ToList().ForEach(x => x.SubFolders = new List<MediaFile> { new MediaFile() });
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
                await _eventAggregator.PublishOnUIThreadAsync(true);

                var requestParams = new RequestProcessingParams
                {
                    ProcessingParams = ProcessingParams
                };

                string processingArgs = JsonConvert.SerializeObject(requestParams);

                var files = GetSelectedFiles();

                foreach (var file in files)
                {
                    var valParts = new NameValueCollection
                    {
                        { "display_name", Path.GetFileNameWithoutExtension(file.path) },
                        { "parent_folder", "5820" }, // Shared=5820, Test=5735
                        { "file_extension", Path.GetExtension(file.path).Replace(".", "") },
                    };

                    var valColl = new NameValueCollection
                    {
                        { file.path, "" }
                    };

                    await _exportService.ExportModelFormCompositionAsync(Instance.UserLogin, valParts, valColl, ProcessingParams);

                    MessageBox.Show("Uploaded successfully.");

                    await NavigationService.GoTo<HomeViewModel>();


                    #region "flurAPI not in use"
                    /*
                        var requestParams = new RequestProcessingParams
                        {
                            ProcessingParams = ProcessingParams
                        };

                        string processingArgs = JsonConvert.SerializeObject(requestParams);

                        var mediaItem = new MediaItem
                        {
                            Mode = "formdata",
                            RequestProcessingParams = requestParams,
                            FormData = new List<FormData>
                        {
                            // new FormData { Key = "file", Src = file.path, Type = "file" },
                            new FormData { Key = "display_name", Value = "Baki File", Type = "text" },
                            new FormData { Key = "parent_folder", Value="5820", Type = "text" },
                            new FormData { Key = "file_extension", Value = System.IO.Path.GetExtension(file.path), Type = "text" },
                            //* new FormData { Key = "processing_params", Value = processingArgs, Type = "text" },
                        }
                        };

                        //VERSION 1:
                        var result = await _exportService.ExportModelAsync(Instance.UserLogin, mediaItem, file.path);
                    */
                    #endregion

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
