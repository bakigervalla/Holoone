using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.ComApi;
using HolooneNavis.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis.Services.Exporters
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
            string vrml = new Vrml().GetVrml(marker);

            // Save the file to disk.
            File.WriteAllText(vrmlPath, vrml);

            // Append the file to the navisworks document, adding the marker spheres to the 3D-environment.
            Application.ActiveDocument.AppendFile(vrmlPath);
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

    }
}
