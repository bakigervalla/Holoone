using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Services.MicrosoftGraph
{
    public enum SignInMethods
    {
        WAM,
        Dialog,
        AzureAD

    }

    public class MicrosoftGraphResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public UserLogin User { get; set; }
    }

    public interface IMicrosoftGraphService
    {
        Task<AuthenticationResult> CallGraph(SignInMethods signInMethod);
    }

    public class MicrosoftGraphService : IMicrosoftGraphService
    {
        public async Task<AuthenticationResult> CallGraph(SignInMethods signInMethod)
        {
            AuthenticationResult authResult = null;
            var app = RequestConstants.PublicClientApp;

            IAccount firstAccount;

            switch (signInMethod)
            {
                // 0: Use account used to signed-in in Windows (WAM)
                case SignInMethods.WAM:
                    // WAM will always get an account in the cache. So if we want
                    // to have a chance to select the accounts interactively, we need to
                    // force the non-account
                    firstAccount = PublicClientApplication.OperatingSystemAccount;
                    break;

                //  1: Use one of the Accounts known by Windows(WAM)
                case SignInMethods.Dialog:
                    // We force WAM to display the dialog with the accounts
                    firstAccount = null;
                    break;

                //  Use any account(Azure AD). It's not using WAM
                default:
                    var accounts = await app.GetAccountsAsync();
                    firstAccount = accounts.FirstOrDefault();
                    break;
            }

            try
            {
                authResult = await app.AcquireTokenSilent(RequestConstants.Scopes, firstAccount)
                    .ExecuteAsync();

                return authResult;
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent. 
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await app.AcquireTokenInteractive(RequestConstants.Scopes)
                        .WithAccount(firstAccount)
                        //.WithParentActivityOrWindow(new WindowInteropHelper(this).Handle) // optional, used to center the browser on the window
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();

                    return authResult;
                }
                catch (MsalException msalex)
                {
                    throw new Exception($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
            }

            if (authResult != null)
            {
                await GetHttpContentWithToken(RequestConstants.GraphAPIEndpoint, authResult.AccessToken);
                return authResult;
                // DisplayBasicTokenInfo(authResult);
            }
        }

        /// <summary>
        /// Perform an HTTP GET request to a URL using an HTTP Authorization header
        /// </summary>
        /// <param name="url">The URL</param>
        /// <param name="token">The token</param>
        /// <returns>String containing the results of the GET operation</returns>
        private async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;
            try
            {
                var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Display basic information contained in the token
        /// </summary>
        private void DisplayBasicTokenInfo(AuthenticationResult authResult)
        {
            if (authResult != null)
            {
                var userName = $"Username: {authResult.Account.Username}" + Environment.NewLine;
                var tokenExpires = $"Token Expires: {authResult.ExpiresOn.ToLocalTime()}" + Environment.NewLine;
            }
        }

    }
}