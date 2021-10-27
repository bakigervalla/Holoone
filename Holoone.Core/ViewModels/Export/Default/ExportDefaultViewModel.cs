using Holoone.Api.Models;
using Holoone.Api.Services;
using Holoone.Api.Services.Interfaces;
using Holoone.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Holoone.Core.ViewModels.Export.Default
{
    public class ExportDefaultViewModel : BaseViewModel
    {
        private readonly IExportService _exportService;

        public ExportDefaultViewModel(
            IHoloNavigationService navigationService,
            IExportService exportService
            )
        {
            _exportService = exportService;

            try
            {
                TraverseTreeParallelForEach(@"e:\Users\BGERVALLA\Downloads\Autodesk\Navisworks");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private ProcessingParams _processingParams = new ProcessingParams();
        public ProcessingParams ProcessingParams { get => _processingParams; set { _processingParams = value; NotifyOfPropertyChange(nameof(ProcessingParams)); } }

        #region navigation

        public string _state = "Selection";
        public string State { get => _state; set { _state = value; NotifyOfPropertyChange("State"); } }

        public void NavigateToExportPage()
        {
            State = "ExportData";
        }

        #endregion

        public List<Folder> Folders { get; set; } = new List<Folder>();

        public async Task ExportAsync()
        {
            try
            {

                IsBusy = true;

                var files = GetSelectedFiles();

                foreach (var file in files)
                {


                    MediaItem model = new MediaItem(file.path);

                    // Version: 1
                    //{
                    //DisplayName = fileName,
                    //FileExtension = System.IO.Path.GetExtension(file.path),
                    //File = new StreamContent(filestream),
                    //ParentFolder = null,
                    //ProcessingParams = new ProcessingParams
                    //{
                    //    ModelType = "default",
                    //    UpVectorDefinition = "y",
                    //    CoordinateSystemOrientation = "left_hand",
                    //    ModelSize = "tabletop",
                    //    ModelOverlay = 0,
                    //    OptimizeModel = 1,
                    //    RemoveHiddenGeometry = 0,
                    //    MergeGeometry = 1,
                    //    HierarchyCutoff = 1,
                    //    ExtractMetadata = 0,
                    //    BlockingCollider = 0,
                    //    IsPrimary = false
                    //}

                    //};

                    // Version: 2
                    //MultipartFormDataContent content = new MultipartFormDataContent();
                    //FileStream filestream = new FileStream(file.path, FileMode.Open);
                    //string fileName = System.IO.Path.GetFileNameWithoutExtension(file.path);
                    //content.Add(new StreamContent(filestream), "file", fileName);
                    //content.Add(new StringContent("", UTF8Encoding.UTF8));

                    // Version: 3
                    //IList<KeyValuePair<string, string>> postData = new List<KeyValuePair<string,string>> {
                    //    new KeyValuePair<string, string>("file", @"e:\Users\BGERVALLA\Downloads\Autodesk\Navisworks\Navis 'Drawings\holo_one_logo.png"),
                    //    new KeyValuePair<string, string>("display_name", "Baki test api"),
                    //    new KeyValuePair<string, string>("parent_folder", null),
                    //    new KeyValuePair<string, string>("file_extension", "png"),
                    //    new KeyValuePair<string, string>("processing_params", "{\"processing_params\": {\"model_type\": \"default\", \"up_vector_definition\": \"y\", \"coordinate_system_orientation\": \"left_hand\", \"model_size\": \"tabletop\", \"model_overlay\": 0, \"optimize_model\": 1, \"remove_hidden_geometry\": 0, \"merge_geometry\": 1, \"hierarchy_cutoff\": 1, \"extract_metadata\": 0, \"blocking_collider\" : 0}, \"is_primary\": false}")
                    //};

                    //var content = new FormUrlEncodedContent(postData);

                    var result = await _exportService.ExportModelAsync(Instance.UserLogin, null, model);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

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
