using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Services
{
    public class LoginService : ILoginService
    {

        private readonly IFlurlClient _flurlClient;

        private static string _githubUsername;
        private static string _githubPassword;
        private static string _githubToken;

        public LoginService(IFlurlClientFactory flurlClientFac)
        {
            _githubUsername = Environment.GetEnvironmentVariable("GITHUB_USERNAME");
            _githubPassword = Environment.GetEnvironmentVariable("GITHUB_PASS");
            _githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

            _flurlClient = flurlClientFac.Get(RequestConstants.BaseUrl);
        }

        public async Task<IEnumerable<UserPermissions>> GetLoginAsync()
        {
            try
            {
                var result = await _flurlClient.Request("users")
                        // .AppendPathSegments("user", "repos")
                        .AppendPathSegment("authenticate")
                        .WithHeader(RequestConstants.UserAgent, RequestConstants.UserAgentValue)
                        .SetQueryParams(new { userId = 1 })
                        //.WithBasicAuth(_githubUsername, _githubPassword) //alternative way of logging in (basic auth)
                        .GetJsonAsync<IEnumerable<UserPermissions>>();

                return result;
            }
            catch (FlurlHttpException ex)
            {
                throw ex;
            }
        }

        public async Task<UserPermissions> GetLoginAsync(string id)
        {
            return await _flurlClient.Request("things", id).GetJsonAsync<UserPermissions>();
        }

        public async Task AddLoginAsync(UserPermissions userPermission)
        {
            var result = await _flurlClient.Request("users")
                    // .AppendPathSegments("user", "repos")
                    .AppendPathSegment("authenticate")
                    .WithHeader(RequestConstants.UserAgent, RequestConstants.UserAgentValue)
                    .SetQueryParams(new { userId = 1 })
                    //.WithBasicAuth(_githubUsername, _githubPassword) //alternative way of logging in (basic auth)
                    .WithOAuthBearerToken(_githubToken)
                    .GetJsonAsync<IEnumerable<UserPermissions>>();
        }

        public async Task UpdateLoginAsync(UserPermissions userPermission)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteLoginAsync(UserPermissions userPermission)
        {
            try
            {
                await RequestConstants.BaseUrl
                        .AppendPathSegment("user")
                        .SetQueryParam("id", 1)
                        .DeleteAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
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
