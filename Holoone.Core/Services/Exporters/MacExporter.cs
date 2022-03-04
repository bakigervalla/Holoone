using HolooneNavis.Models;
using HolooneNavis.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis.Services.Exporters
{
    /// <summary>
    /// Exports markers as mac file.
    /// </summary>
    internal class MacExporter : IExporter
    {
        private IEnumerable<Marker> _markers;

        public string FileExtension
        {
            get
            {
                return "mac";
            }
        }

        public string FileName
        {
            get
            {
                return "sphere_anchor_" + DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        public MacExporter(IEnumerable<Marker> markers)
        {
            _markers = markers;
        }

        /// <summary>
        /// Exports markers as mac file.
        /// </summary>
        public void Export(string path)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$* Created by: Holo-one Navis");
            sb.AppendLine($"$* Created at: {DateTime.Now}");
            sb.AppendLine();
            sb.AppendLine("var !implantihole repre hole");
            sb.AppendLine("repre hole off");
            sb.AppendLine("var !implantiunits units");
            sb.AppendLine("var !implantiunit  split |$!implantiunits|");
            sb.AppendLine("mm distance");
            sb.AppendLine($"NEW ZONE /{Path.GetFileNameWithoutExtension(path)}");

            foreach (var marker in _markers)
            {
                sb.AppendLine($"\tNEW EQUI /Marker_{DateTime.Now.ToString("yyyy-MM-dd")}_{marker.Name}");
                sb.AppendLine("\t\tNEW CYLI");
                sb.AppendLine($"\t\t\tAT E{marker.X * 1000} N{marker.Y * 1000} U{marker.Z * 1000}");
                sb.AppendLine("\t\t\tDIAM 100.0000 HEIG 10.0000");
            }

            sb.AppendLine("repr hole $!implantihole");
            sb.AppendLine("$!implantiunit[3] distance");
            sb.AppendLine("$.");
            sb.AppendLine();

            File.WriteAllText(path, sb.ToString());
        }
    }
}