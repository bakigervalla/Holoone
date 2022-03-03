using HolooneNavis.Models;
using HolooneNavis.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HolooneNavis.Services.Exporters
{
    internal class VdpTagExporter : IExporter
    {

        private string _projectName;
        private IEnumerable<Marker> _markers;

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
                return "tags";
            }
        }

        public VdpTagExporter(string projectName, IEnumerable<Marker> markers)
        {
            _projectName = projectName;
            _markers = markers;
        }

        /// <summary>
        /// Exports markers as a zip archive containing a wrl file and a zip file with metadata.
        /// </summary>
        /// <param name="path">The path of the zip file</param>
        /// <param name="projectName">The project name</param>
        /// <param name="review">The selected review</param>
        /// <param name="markers">The review markers</param>
        public void Export(string path)
        {

            // Create a new xml document for the xml file.
            var doc = new XmlDocument();
            var rootElement = doc.CreateElement("root");

            // Define a function to create the url for a single marker. This is used in the xml and also
            // passed to Vrml().GetVrmlWithUrl() to make sure the url is the same in both wrl and xml.
            Func<Marker, string> getMarkerUrl =
                new Func<Marker, string>(m => DateTime.Now.ToString("yyyy-MM-dd") + " " + m.Id);

            // Create the xml metadata for each marker.
            foreach (var marker in _markers)
            {
                var markerElement = doc.CreateElement("label");
                var attr = doc.CreateAttribute("name");

                attr.Value = getMarkerUrl(marker);

                markerElement.Attributes.Append(attr);

                markerElement.AppendChild(doc.CreateTextNode($"\n\t\tReview: { DateTime.Now.ToString("yyyy-MM-dd")}\n"));
                markerElement.AppendChild(doc.CreateTextNode($"\t\tMarker: {marker.Id}\n"));
                markerElement.AppendChild(doc.CreateTextNode($"\t\tProjekt: {_projectName}\n"));

                rootElement.AppendChild(markerElement);
            }
            doc.AppendChild(rootElement);
            // Preserve whitespace to improve readability.
            doc.PreserveWhitespace = true;

            // Create a zip archive and add entries for xml and wrl files.
            using (FileStream zipToOpen = new FileStream(path, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {

                    string xmlFileName = Path.GetFileName(Path.ChangeExtension(path, "xml"));
                    ZipArchiveEntry xmlEntry = archive.CreateEntry(xmlFileName);

                    using (XmlTextWriter xmlWriter = new XmlTextWriter(xmlEntry.Open(), Encoding.UTF8))
                    {
                        // Save the xml with indentation to improve readability.
                        xmlWriter.Formatting = Formatting.Indented;
                        doc.Save(xmlWriter);
                    }

                    string vrml = new Vrml().GetVrml(_markers, 0.2f, getMarkerUrl);

                    string wrlFileName = Path.GetFileName(Path.ChangeExtension(path, "wrl"));
                    ZipArchiveEntry wrlEntry = archive.CreateEntry(wrlFileName);
                    using (StreamWriter writer = new StreamWriter(wrlEntry.Open()))
                    {
                        writer.Write(vrml);
                    }
                }
            }
        }
    }
}