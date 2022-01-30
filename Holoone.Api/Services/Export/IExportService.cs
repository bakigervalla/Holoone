using Flurl.Http;
using Holoone.Api.Models;
using Holoone.Api.Services.MicrosoftGraph;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Services.Interfaces
{
    public interface IExportService
    {

        Task<string> ExportModelFormCompositionAsync(UserLogin user, NameValueCollection values, NameValueCollection files, ProcessingParams processingParams, string urlPath, string formDataName);

        Task<IList<MediaFile>> GetCompanyMediaFolderContent(UserLogin user, int folderId = 0);

        // OBSOLETE
        Task<IFlurlResponse> ExportModelAsync(UserLogin user, MediaItem mediaItem, string filePath);
    }
}
