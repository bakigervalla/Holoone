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
using Holoone.Api.Helpers.Extensions;

namespace Holoone.Api.Services
{
    public class LoginService : ILoginService
    {

        private readonly IFlurlClient _flurlClient;

        private static string _usUrl;
        private static string _euUrl;
        private static string _chUrl;

        public LoginService(IFlurlClientFactory flurlClientFac)
        {
            _usUrl = Environment.GetEnvironmentVariable("US_URL");
            _euUrl = Environment.GetEnvironmentVariable("EU_URL");
            _chUrl = Environment.GetEnvironmentVariable("CH_URL");

            _flurlClient = flurlClientFac.Get(RequestConstants.BaseUrl);
        }

        public async Task<IFlurlResponse> LoginSphereAsync(LoginCredentials loginCredentials)
        {
            string hostKey = loginCredentials.Hosts.Single(x => x.IsChecked).Text;
            _flurlClient.BaseUrl = RequestConstants.SphereBaseUrls[hostKey];

            return await _flurlClient.Request("api-token-auth/")
                    .WithHeader(RequestConstants.UserAgent, RequestConstants.UserAgentValue)
                    .PostJsonAsync(loginCredentials);
        }

        public async Task<IFlurlResponse> LoginWithMicrosoftAsync(LoginCredentialsGraph loginCredentialsGraph)
        {
            string hostKey = loginCredentialsGraph.Hosts.Single(x => x.IsChecked).Text;
            _flurlClient.BaseUrl = RequestConstants.SphereBaseUrls[hostKey];

            return await _flurlClient.Request("/integration/microsoft-graph/authorize/native/")
                    .WithHeader(RequestConstants.UserAgent, RequestConstants.UserAgentValue)
                    .PostJsonAsync(loginCredentialsGraph);
        }


        //public async Task<int> LoginWithGraphToken(string graphToken, DateTimeOffset expiryOn, bool isMR)
        //{
        //    var url = "/integration/microsoft-graph/authorize/native/";
        //    string licenseType = isMR ? "mr" : "lite";

        //    LoginCredentialsGraph credentialsGraph = new LoginCredentialsGraph
        //    {
        //        LicenseType = licenseType,
        //        Token = graphToken,
        //        ExpiresAt = expiryOn.ToUnixTimeMilliseconds(),
        //        DeviceId = SystemInfo.deviceUniqueIdentifier
        //    };

        //    var request = await Post(url, credentialsGraph);
        //    if (request.Response.IsSuccess)
        //    {
        //        var jsonToken = JObject.Parse(request.Response.DataAsText);
        //        PlayerPrefConstants.AuthToken.Value = ((string)jsonToken["token"]);
        //        PlayerPrefs.Save();
        //        return request.Response.StatusCode;
        //    }

        //    return request.Response.StatusCode;
        //}

        //public async Task<bool> UpdateGraphToken(string graphToken, DateTimeOffset expiryOn)
        //{
        //    var url = "integration/microsoft-graph/token/update/";
        //    LoginCredentialsGraph credentialsGraph = new LoginCredentialsGraph
        //    {
        //        Token = graphToken,
        //        ExpiresAt = expiryOn.ToUnixTimeMilliseconds()
        //    };

        //    var request = await Post(url, credentialsGraph);
        //    return request.Response.IsSuccess;
        //}

        //public async Task<bool> GetJWTToken(string guestDisplayName = null, string callId = null)
        //{
        //    string url = "api-jwt-auth/";

        //    if (!string.IsNullOrEmpty(guestDisplayName) && !string.IsNullOrEmpty(callId))
        //    {
        //        url += $"guest/?username={guestDisplayName}&call_id={callId}";
        //    }

        //    var request = await Get(url);
        //    if (request.Response != null && request.Response.IsSuccess)
        //    {
        //        var jwtToken = request.Response.DataAsText;

        //        PlayerPrefConstants.JwtToken.Value = jwtToken.Replace("\"", "");
        //        PlayerPrefs.Save();
        //        return true;
        //    }
        //    else
        //    {
        //        Debug.LogError(request.Exception);
        //    }

        //    return false;
        //}



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
