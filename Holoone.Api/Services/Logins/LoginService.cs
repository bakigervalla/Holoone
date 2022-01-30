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

namespace Holoone.Api.Services
{
    public class LoginService : ILoginService
    {

        private readonly IFlurlClient _flurlClient;
        private readonly IMicrosoftGraphService _microsoftGraph;

        public LoginService(
            IFlurlClientFactory flurlClientFac,
            IMicrosoftGraphService microsoftGraph
            )
        {
            _flurlClient = flurlClientFac.Get(RequestConstants.BaseUrl);
            _microsoftGraph = microsoftGraph;
        }

        public async Task<IFlurlResponse> LoginSphereAsync(LoginCredentials loginCredentials)
        {
            string hostKey = loginCredentials.Hosts.Single(x => x.IsChecked).Text;
            _flurlClient.BaseUrl = RequestConstants.SphereBaseUrls[hostKey];

            return await _flurlClient.Request("api-token-auth/")
                    .WithHeader(RequestConstants.UserAgent, RequestConstants.UserAgentValue)
                    .PostJsonAsync(loginCredentials);
        }

        public async Task<MicrosoftGraphResponse> LoginWithMicrosoftAsync(LoginCredentials loginCredentials)
        {
            string hostKey = loginCredentials.Hosts.Single(x => x.IsChecked).Text;
            _flurlClient.BaseUrl = RequestConstants.SphereBaseUrls[hostKey];

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

                dynamic request = await _flurlClient.Request("core/integration/thinkreality/generate-auth-code/")
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

                var request = await _flurlClient.Request("core/integration/thinkreality/check-login-state/")
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