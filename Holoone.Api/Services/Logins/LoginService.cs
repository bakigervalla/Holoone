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
using Holoone.Api.Services.MicrosoftGraph;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Holoone.Api.Helpers.Extensions;
using Hanssens.Net;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace Holoone.Api.Services
{
    public class LoginService : ILoginService
    {

        private readonly IFlurlClient _flurlClient;
        private readonly IMicrosoftGraphService _microsoftGraph;
        private readonly ILocalStorage _localeStorage;

        public LoginService(
            IFlurlClientFactory flurlClientFac,
            IMicrosoftGraphService microsoftGraph,
            ILocalStorage localeStorage
            )
        {
            _flurlClient = flurlClientFac.Get(RequestConstants.BaseUrl);
            _microsoftGraph = microsoftGraph;
            _localeStorage = localeStorage;
        }

        public async Task<IFlurlResponse> LoginSphereAsync(LoginCredentials loginCredentials)
        {
            string hostKey = loginCredentials.Hosts.Single(x => x.IsChecked).Text;
            _flurlClient.BaseUrl = Utility.GetBaseUrl("Sphere", hostKey);

            return await _flurlClient.Request("api-token-auth/")
                    .WithHeader(RequestConstants.UserAgent, RequestConstants.UserAgentValue)
                    .PostJsonAsync(loginCredentials);
        }

        public async Task RefreshLoginToken(UserLogin userLogin)
        {
            // get login data
            var isLoggedIn = _localeStorage.Exists("user_login");

            if (!isLoggedIn)
                return;

            var client = new HttpClient();

            //var userLogin = _localeStorage.Get<UserLogin>("user_login");

            if (userLogin.LoginType.Type == "LCP")
            {
                // check is token valid, otherwise refresh token
                var checkUrl = Utility.GetBaseUrl("LCP", userLogin.LoginType.Region);

                using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{checkUrl}media/?type=generic_3d_model"))
                {
                    requestMessage.Headers.Add("Bearer", userLogin.Token);

                    var result = await client.SendAsync(requestMessage);

                    if (result.StatusCode != HttpStatusCode.Forbidden)
                        return;
                }

                // refresh token
                var host = Utility.GetHostUrl("LCP", userLogin.LoginType.Region);

                var refreshTokenUrl = $"{userLogin.RegionUrl}integration/thinkreality/refresh/?client_id={userLogin.OrganizationId}&host={host}&refresh_token={userLogin.RefreshToken}";

                using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, refreshTokenUrl))
                {
                    var response = await client.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject(responseContent);

                        userLogin.Token = result.access_token;
                        userLogin.RefreshToken = result.refresh_token;
                        _localeStorage.Store("user_login", userLogin);
                    }
                }
            }
        }

        public async Task<MicrosoftGraphResponse> LoginWithMicrosoftAsync(LoginCredentials loginCredentials)
        {
            string hostKey = loginCredentials.Hosts.Single(x => x.IsChecked).Text;
            _flurlClient.BaseUrl = Utility.GetBaseUrl("Sphere", hostKey);

            string response = await _flurlClient.Request("integration/microsoft-graph/authorize/?request_type=sign_up&origin=app")
                                    // .WithHeaders(new { Content_Type = "application/x-www-form-urlencoded" })
                                    .WithHeader(RequestConstants.UserAgent, RequestConstants.UserAgentValue)
                                    .GetStringAsync();

            var url = response.GetTextBetween("redirect_uri=", "\",").HtmlDecode().DecodeUrlString();
            var urlMsaSignUp = response.GetTextBetween("urlMsaSignUp\":\"", "\",").HtmlDecode().DecodeUrlString();

            return await CallMicrosoftGraph(urlMsaSignUp);
        }

        private async Task<MicrosoftGraphResponse> CallMicrosoftGraph(string urlMsaSignUp)
        {
            try
            {
                CreateApplication(true, urlMsaSignUp);

                var authResult = await _microsoftGraph.CallGraph(SignInMethods.Dialog);

                if (authResult == null)
                    return new MicrosoftGraphResponse { HttpStatusCode = System.Net.HttpStatusCode.BadRequest };

                return new MicrosoftGraphResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK,
                    User = new UserLogin
                    {
                        IsLoggedIn = true,
                        Token = authResult.AccessToken,
                        UserFullName = authResult.Account.Username,
                        TokenExpires = authResult.ExpiresOn.ToLocalTime()
                    }
                };
            }
            catch
            {
                throw;
            }
        }

        private void CreateApplication(bool useWam, string Url)
        {
            var builder = PublicClientApplicationBuilder.Create(RequestConstants.ClientId)
                .WithAuthority($"{RequestConstants.Instance}{RequestConstants.Tenant}")
                .WithDefaultRedirectUri()
                ;

            if (useWam)
            {
                builder.WithExperimentalFeatures();
                // builder.WithWindowsBrokerOptions(true);  // Requires redirect URI "ms-appx-web://microsoft.aad.brokerplugin/{client_id}" in app registration
            }
            RequestConstants._clientApp = builder.Build();
        }

        public async Task<IFlurlResponse> UpdateMicrosoftGraphTokenAsync(string graphToken, DateTimeOffset expiryOn)
        {
            throw new NotImplementedException();

            //    var url = "integration/microsoft-graph/token/update/";
            //    LoginCredentialsGraph credentialsGraph = new LoginCredentialsGraph
            //    {
            //        Token = graphToken,
            //        ExpiresAt = expiryOn.ToUnixTimeMilliseconds()
            //    };

            //    var request = await Post(url, credentialsGraph);
            //    return request.Response.IsSuccess;
        }


        // PCP Login
        public async Task<string> LCPLoginGenerateAuthCode(string regionUrl, string deviceId)
        {
            try
            {
                _flurlClient.BaseUrl = regionUrl;

                dynamic request = await _flurlClient.Request("integration/thinkreality/generate-auth-code/")
                                 .SetQueryParam("device_id", deviceId)
                                 .GetJsonAsync();

                return request.temp_auth_token;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<LCPLogin> LCPLoginPolling(string regionUrl, string deviceId, string authCode)
        {
            try
            {
                _flurlClient.BaseUrl = regionUrl;

                var request = await _flurlClient.Request("integration/thinkreality/check-login-state/")
                                        .PostJsonAsync(new
                                        {
                                            auth_code = authCode,
                                            device_id = deviceId
                                        })
                                        .ReceiveJson<LCPLogin>();

                return request;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}