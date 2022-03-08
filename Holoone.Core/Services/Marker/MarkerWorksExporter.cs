using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Interop.ComApi;
using HolooneNavis.Models;
using HolooneNavis.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;

namespace HolooneNavis.Services
{
    /// <summary>
    /// Exports the markers of the selected review as a markerworks zip-archive that can be imported in VRresult.
    /// </summary>
    internal class MarkerWorksExporter : IExporter
    {
        public string FileExtension
        {
            get
            {
                return "zip";
            }
        }

        public string FileName
        {
            get
            {
                return "CMS_Data";
            }
        }

        private IEnumerable<Marker> _markers;
        private string _projectName;
        private SavedItemCollection _reviewItems;

        public MarkerWorksExporter(IEnumerable<Marker> markers, string projectName, SavedItemCollection reviewItems)
        {
            _markers = markers;
            _projectName = projectName;
            _reviewItems = reviewItems;
        }

        /// <summary>
        /// Exports the markers of the selected review as an archive containing a .mw2 file and two .jpg images per marker.
        /// </summary>
        /// <param name="path">Path of the new .mw2 file.</param>
        /// <param name="review">Review to export.</param>
        /// <param name="organizer">Review organizer, currently not in use.</param>
        /// <param name="markers">Markers to export.</param>
        /// <param name="projectName">Name of the project; used in the file header.</param>
        /// <param name="reviewItems">A collection of Navisworks SavedViewpoint objects; used to create viewpoint screenshots.</param>
        public void Export(string path)
        {
            Document document = Application.MainDocument;
            var viewpoint = Application.MainDocument.CurrentViewpoint.CreateCopy();

            List<string> imageNamesToDelete = new List<string>();

            using (FileStream zipToOpen = new FileStream(path, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    foreach (SavedViewpoint markerViewpoint in _reviewItems)
                    {
                        string viewpointName = markerViewpoint.DisplayName;
                        string markerNumber = int.Parse(viewpointName.Split(' ')[1]).ToString("D3");

                        // set the current viewpoint
                        document.SavedViewpoints.CurrentSavedViewpoint = markerViewpoint;
                        // the image name
                        string imageName = _projectName + "_all_" + DateTime.Now.ToString("yyyyMMdd") + "_" + markerNumber;
                        // Add image to zip in two resolutions.
                        AddActiveViewAsImageToZip(archive, imageName, 800, 600);
                        AddActiveViewAsImageToZip(archive, imageName + "_u", 512, 512);
                        // Add image names to list; these files will be deleted when the export is done.
                        imageNamesToDelete.Add(imageName);
                        imageNamesToDelete.Add(imageName + "_u");
                    }

                    string data = ToMarkerWorksFile(new MarkerWorksData
                    {
                        ProjectName = _projectName,
                        Organizer = "Holo-one",
                        Format = "NWP",
                        DataDate = DateTime.Now,
                        ReviewDate = DateTime.Now,
                        Markers = _markers
                    });

                    ZipArchiveEntry mw2Entry = archive.CreateEntry("cmsimport.mw2");
                    using (StreamWriter writer = new StreamWriter(mw2Entry.Open()))
                    {
                        writer.Write(data);
                    }
                }
            }

            Application.MainDocument.CurrentViewpoint.CopyFrom(viewpoint);

            // Delete the temporary images after 1 second to prevent IOException.
            Task.Delay(1000).ContinueWith(task => DeleteImages(imageNamesToDelete));
        }

        private void AddActiveViewAsImageToZip(ZipArchive archive, string imageName, int width, int height)
        {
            InwOpState10 oState = ComBridge.State;

            InwOaPropertyVec options = oState.GetIOPluginOptions("lcodpimage");
            foreach (InwOaProperty property in options.Properties())
            {
                if (property.name == "export.image.width")
                {
                    property.value = width;
                }
                else if (property.name == "export.image.height")
                {
                    property.value = height;
                }
            }

            oState.DriveIOPlugin("lcodpimage", imageName, options);

            Bitmap viewpointBitmap = new Bitmap(imageName);

            // Create two entries both containing the same image.
            ZipArchiveEntry entry = archive.CreateEntry(imageName + ".jpg");

            // Save the images to the zip archive.
            viewpointBitmap.Save(entry.Open(), System.Drawing.Imaging.ImageFormat.Jpeg);
        }


        private void DeleteImages(List<string> images)
        {
            foreach (var imageName in images)
            {
                if (File.Exists(imageName))
                {
                    File.Delete(imageName);
                }
            }
        }

        /// <summary>
        /// Returns a markerworks .mw2 data structure as a string for serialization.
        /// </summary>
        private string ToMarkerWorksFile(MarkerWorksData data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# Holo-one Navisworks Plugin Import File");
            sb.AppendLine("# Version: 1.0");
            sb.AppendLine($"Projekt:{data.ProjectName}");
            sb.AppendLine($"Teilprojekt:t1");
            sb.AppendLine($"Format:{data.Format}");
            sb.AppendLine($"Datenstand:{data.DataDate.ToString("yyyy-MM-dd")}");
            sb.AppendLine($"Reviewdatum:{data.ReviewDate.ToString("yyyy-MM-dd")}");
            //sb.AppendLine($"Organisator:{data.Organizer}"); // Currently not part of the .mw2 file.
            sb.AppendLine("Datenspalten:Review_Marker_Nr|k_x|k_y|k_z|Bild|Bild_Uebersicht|View_x|View_y|View_z|VDP_UUID");
            sb.AppendLine("NWD:");
            foreach (var marker in data.Markers)
            {
                sb.AppendLine(GetMarkerRow(marker));
                sb.AppendLine("<comment>");
                sb.AppendLine("Holo-one anchors");
                sb.AppendLine("</comment>");
            }

            return sb.ToString().TrimEnd('\r', '\n');
        }

        /// <summary>
        /// Returns a data row created from the marker.
        /// </summary>
        private string GetMarkerRow(Marker marker)
        {
            //string overviewPicture = Path.GetFileNameWithoutExtension(marker.DetailPicture) + "_u" + ".jpg";
            //object[] markerProperties = { marker.ReviewMarkerNumber, marker.MarkerPosX, marker.MarkerPosY, marker.MarkerPosZ,
            //    marker.DetailPicture, overviewPicture, marker.ViewX, marker.ViewY, marker.ViewZ, marker.Guid };

            //return string.Join("|", markerProperties);
            return "";
        }


    }

    /// <summary>
    /// Datastructure of a .mw2 markerworks file.
    /// </summary>
    internal class MarkerWorksData
    {
        public string ProjectName { get; set; }

        public string Organizer { get; set; }

        public string Format { get; set; }

        public DateTime DataDate { get; set; }
        public DateTime ReviewDate { get; set; }

        public IEnumerable<Marker> Markers { get; set; }
    }
}