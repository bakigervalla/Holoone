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
using Holoone.Api.Services.MicrosoftGraph;
using Microsoft.Identity.Client;

namespace Holoone.Api.Services
{
    public class LoginService : ILoginService
    {

        private readonly IFlurlClient _flurlClient;
        private readonly IMicrosoftGraphService _microsoftGraph;


        private static string _usUrl;
        private static string _euUrl;
        private static string _chUrl;

        public LoginService(
            IFlurlClientFactory flurlClientFac,
            IMicrosoftGraphService microsoftGraph
            )
        {
            _usUrl = Environment.GetEnvironmentVariable("US_URL");
            _euUrl = Environment.GetEnvironmentVariable("EU_URL");
            _chUrl = Environment.GetEnvironmentVariable("CH_URL");

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

        public async Task<MicrosoftGraphResponse> LoginWithMicrosoftAsync()
        {
            try
            {
                CreateApplication(true);

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

        private void CreateApplication(bool useWam)
        {
            var builder = PublicClientApplicationBuilder.Create(RequestConstants.ClientId)
                .WithAuthority($"{RequestConstants.Instance}{RequestConstants.Tenant}")
                .WithDefaultRedirectUri();

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

        public Task<UserPermissions> LoginWithThinkReality(string id)
        {
            throw new NotImplementedException();
        }
    }
}