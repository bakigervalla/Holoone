using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis.Services.Interfaces
{
    /// <summary>
    /// Interface for exporting markers or viewpoints.
    /// </summary>
    public interface IExporter
    {
        string FileExtension { get; }

        string FileName { get; }

        /// <summary>
        /// Implementing classes should write a file to disk using the specified path.
        /// </summary>
        /// <param name="path">The full path of the new file</param>
        void Export(string path);
    }
}
