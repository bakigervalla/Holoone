using Flurl.Http;
using Flurl.Http.Configuration;
using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Services
{
    public class Folder
    {
        public string Name { get; set; }
        public IList<OFile> ListofFiles { get; set; }
    }


    public class OFile
    {

        public string Name { get; set; }
        public string ext { get; set; }
        public string path { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ExportService : IExportService
    {
        private readonly IFlurlClient _flurlClient;

        public ExportService(IFlurlClientFactory flurlClientFac)
        {
            _flurlClient = flurlClientFac.Get(RequestConstants.BaseUrl);
        }
        

        public async Task<IFlurlResponse> ExportModelAsync(UserLogin user, MediaItem mediaItem, MediaItem modelItem)
        {
            _flurlClient.BaseUrl = RequestConstants.BaseUrl;

            return await _flurlClient.Request("media/add/file/")
                   .WithBasicAuth(user.Username, user.Password)
                   .PostJsonAsync(modelItem)
                   .ReceiveJson<IFlurlResponse>();

            //return await _flurlClient.Request("media/add/file/")
            //            .WithBasicAuth(user.Username, user.Password)
            //            .WithHeader("Content-Type", "application/x-www-form-urlencoded")
            //            .PostUrlEncodedAsync(content)
            //            .ReceiveJson<IFlurlResponse>();
        }



        //******/
        //public async Task<IApiResponse> CreateRepository(string user, string repository)
        //{
        //    var repo = new Repository
        //    {
        //        Name = repository,
        //        FullName = $"{user}/{repository}",
        //        Description = "Generic description",
        //        Private = false
        //    };

        //    var result = await RequestConstants.BaseUrl
        //        .AppendPathSegments("user", "repos")
        //        .WithHeader(RequestConstants.UserAgent, RequestConstants.UserAgentValue)
        //        .WithOAuthBearerToken(_githubToken)
        //        .PostJsonAsync(repo)
        //        .ReceiveJson<Repository>();

        //    //var post = await "http://jsonplaceholder.typicode.com"
        //    //       .AppendPathSegment("posts")
        //    //       .PostJsonAsync(new { userId = 123, title = "test", body = "test" })
        //    //       .ReceiveJson<Post>();

        //    return result;
        //}

        //public async Task<Repository> EditRepository(string user, string repository)
        //{
        //    var repo = new Repository
        //    {
        //        Name = repository,
        //        FullName = $"{user}/{repository}",
        //        Description = "Modified repository",
        //        Private = false
        //    };

        //    var result = await RequestConstants.BaseUrl
        //        .AppendPathSegments("repos", user, repository)
        //        .WithHeader(RequestConstants.UserAgent, RequestConstants.UserAgentValue)
        //        .WithOAuthBearerToken(_githubToken)
        //        .PatchJsonAsync(repo)
        //        .ReceiveJson<Repository>();

        //    return result;
        //}

        //public async Task<HttpResponseMessage> DeleteRepository(string user, string repository)
        //{
        //    var result = await RequestConstants.BaseUrl
        //        .AppendPathSegments("repos", user, repository)
        //        .WithHeader(RequestConstants.UserAgent, RequestConstants.UserAgentValue)
        //        .WithOAuthBearerToken(_githubToken)
        //        .DeleteAsync();

        //    return result;
        //}
    }
}
