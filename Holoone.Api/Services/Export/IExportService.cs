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
        Task<ExportService> EnsureTokenAsync(UserLogin userLogin);
        Task<string> ExportDefaultModelAndNewBIMAsync(UserLogin user, NameValueCollection values, NameValueCollection files, ProcessingParams processingParams, string urlPath, string exportType);
        Task<string> ExportExistingBIMAsync(UserLogin user, int mediaId, Dictionary<string, dynamic> payload, NameValueCollection files);
        Task<IList<MediaFile>> GetCompanyMediaFolderContent(UserLogin user, int folderId = 0);
        Task<IList<MediaFile>> GetCompany3DModels(UserLogin user);
        Task<IEnumerable<BIM3DLayer>> Get3DModelById(UserLogin user, int mediaFileId);
    }
}
