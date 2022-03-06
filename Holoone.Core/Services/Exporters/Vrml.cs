using HolooneNavis.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using media = System.Windows.Media;

namespace HolooneNavis.Services.Exporters
{
    /// <summary>
    /// Class for generating Vrml-Files (.wrl).
    /// </summary>
    internal class Vrml
    {

        /// <summary>
        /// Get a string in vrml syntax containing the selected markers as sphere shapes.
        /// </summary>
        /// <param name="markers">The markers to write</param>
        /// <param name="radius">The sphere radius</param>
        /// <param name="getMarkerUrl">A function for generating the marker url. 
        /// A url is added to each marker that can be used to connect a wrl file with an xml file containing metadata.</param>
        /// <returns></returns>
        public string GetVrml(Marker marker, Func<Marker, string> getMarkerUrl = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Prefix);

            string url = getMarkerUrl != null ? getMarkerUrl(marker) : null;
            stringBuilder.Append(Sphere(marker.Id, marker.X, marker.Y, marker.Z, marker.Radious, "Active", url));
            // stringBuilder.Append(Tag(marker));

            stringBuilder.Append(Suffix);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Prefix or header; the start of the .wrl file.
        /// </summary>
        private string Prefix
        {
            get
            {
                //return "#VRML V2.0 utf8\n\nTransform {\n\tchildren [\n\n";
                return "#VRML V2.0 utf8\n";
            }
        }

        /// <summary>
        /// The end of the .wrl file.
        /// </summary>
        private string Suffix
        {
            get
            {
                // return "\t]\n}\n\n";
                return "";
            }
        }

        /// <summary>
        /// Adds a sphere to the .wrl file.
        /// </summary>
        /// <param name="X">x-position</param>
        /// <param name="Y">y-position</param>
        /// <param name="Z">z-position</param>
        /// <param name="radius">Sphere radius.</param>
        /// <returns></returns>
        private string Sphere(string markerId, double X, double Y, double Z, float radius, string markerState, string url = null)
        {
            media.Color markerColor = (media.Color)media.ColorConverter.ConvertFromString("#0000ff");

            StringBuilder sb = new StringBuilder();
            if (url != null)
            {
                sb.AppendLine("Anchor {");
                sb.AppendLine($"\turl \"{url}\"");
                sb.AppendLine("\tchildren");
            }
            sb.AppendLine($"DEF Marker_{markerId} Transform {{");
            sb.AppendLine($"\t\ttranslation {X} {Y} {Z}");
            sb.AppendLine("\t\t\tchildren [");
            sb.AppendLine("\t\t\t\tShape {");

            sb.AppendLine("\t\t\t\t\t\tgeometry Text {");
            sb.AppendLine($"\t\t\t\t\t\t string \"{markerId} \"");
            sb.AppendLine("\t\t\t\t\t\t\tfontStyle FontStyle {");
            sb.AppendLine("\t\t\t\t\t\t\t\tsize 2}}");

            sb.AppendLine($"\t\t\t\t\tgeometry Sphere {{ radius {radius} }}");
            sb.AppendLine("\t\t\t\t\tappearance Appearance {");
            sb.AppendLine("\t\t\t\t\t\tmaterial Material {");
            sb.AppendLine($"\t\t\t\t\t\t\tdiffuseColor {markerColor.R / 255f} {markerColor.G / 255f} {markerColor.B / 255f}");
            sb.AppendLine("\t\t\t\t\t\t\ttransparency 0.5");
            sb.AppendLine("\t\t\t\t\t\t}");
            sb.AppendLine("\t\t\t\t\t}");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t]");
            sb.AppendLine("\t\t}");
            if (url != null)
            {
                sb.AppendLine("\t}");
            }
            return sb.ToString();
        }

        //private Color GetMarkerColor(MarkerState markerState)
        //{
        //    switch (markerState)
        //    {
        //        case MarkerState.Free:
        //            return (Color)ColorConverter.ConvertFromString("#0000ff"); // blue
        //        case MarkerState.Open:
        //            return (Color)ColorConverter.ConvertFromString("#e30000"); // red
        //        case MarkerState.InProcessing:
        //            return (Color)ColorConverter.ConvertFromString("#ffbc00"); // yellow
        //        case MarkerState.Back:
        //            return (Color)ColorConverter.ConvertFromString("#c00000"); // reddark
        //        case MarkerState.PostProcessing:
        //            return (Color)ColorConverter.ConvertFromString("#e30000");  // redlight
        //        case MarkerState.Done:
        //            return (Color)ColorConverter.ConvertFromString("#5dcb41"); green
        //        case MarkerState.Proof:
        //            return (Color)ColorConverter.ConvertFromString("#7daadc"); blugrey
        //        case MarkerState.Checked:
        //            return (Color)ColorConverter.ConvertFromString("#006c7c"); // bluedark
        //    }
        //    return Colors.Red;
        //}
    }
}