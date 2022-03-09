using Autodesk.Navisworks.Api;
using HolooneNavis.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HolooneNavis.Services
{
    /// <summary>
    /// Creates spheres to represent the markers of the current review or of the project in navisworks. 
    /// </summary>
    public class MarkerSphereCreator
    {
        /// <summary>
        /// Create Spheres for all loaded markers
        /// </summary>
        /// <param name="path"></param>
        /// <param name="marker"></param>
        public static void CreateMarkerSphere(string vrmlPath, Marker marker)
        {
            //* SaveViewFromExistingData(marker);

            string vrml = new Vrml().GetVrml(marker);

            // Save the file to disk.
            if (File.Exists(vrmlPath))
                File.Delete(vrmlPath);

            File.WriteAllText(vrmlPath, vrml);

            // Append the file to the navisworks document, adding the marker spheres to the 3D-environment.
            Application.ActiveDocument.MergeFile(vrmlPath);
        }

        //public bool SphereExists()
        //{
        //    string path = Path.Combine(Path_Plugin, $"Name.wrl");
        //    return File.Exists(path);
        //}

        //private bool SphereExists(int totalMarkers)
        //{
        //    var document = Application.ActiveDocument;
        //    var modelItem = document.Models.FirstOrDefault(model => model.FileName == _vrmlPath);
        //    return modelItem != null && modelItem.RootItem.Children.Count() == totalMarkers;
        //}

        /// <summary>
        /// Generate and load SavedView for given Marker (executes when a Review is selected from the list to generate markers)
        /// </summary>
        /// <param name="item">Each marker from selecte Review</param>
        /// <param name="review">Marker's review</param>
        private static void SaveViewFromExistingData(Marker item)
        {
            try
            {
                var doc = Application.MainDocument;
                Viewpoint viewpoint = new Viewpoint();
                SavedViewpoint savedViewpoint = null;

                doc.CurrentViewpoint.CopyFrom(viewpoint);

                savedViewpoint = new SavedViewpoint(doc.CurrentViewpoint.ToViewpoint());
                savedViewpoint.Viewpoint.Position = new Point3D(item.X, item.Y, item.Z);
                savedViewpoint.Viewpoint.AlignDirection(new Vector3D(0, 0, -1)); // align direction top
                savedViewpoint.Viewpoint.Rotation = new Rotation3D(0, 0, 0, 1); // align direction north
                savedViewpoint.Viewpoint.Projection = ViewpointProjection.Orthographic;
                savedViewpoint.Viewpoint.HeightField = 50;


                savedViewpoint.DisplayName = "Marker " + item.Id;
                string viewPointComment = $"Creator: Holoone | {DateTime.Now.ToShortDateString()}\r\nAnchor";
                savedViewpoint.Comments.Add(new Comment(viewPointComment, CommentStatus.New, "Holoone"));

                FolderItem folder = doc.SavedViewpoints.RootItem;
                doc.SavedViewpoints.AddCopy(folder, savedViewpoint);
            }
            catch
            {
                throw;
            }
        }
    }
}
