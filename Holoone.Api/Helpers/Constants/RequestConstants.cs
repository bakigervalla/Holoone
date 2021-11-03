using Holoone.Api.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Helpers.Constants
{
    public static class RequestConstants
    {
        public const string RootUrl = "https://dev.holo-one.com/";
        public const string BaseUrl = "https://dev.holo-one.com/core/";
        
        public static IDictionary<string, string> SphereBaseUrls = new Dictionary<string, string> {
            { "USA", "https://dev.holo-one.com/core/"},
            { "Europe", "https://dev.holo-one.com/core/"},
            { "China", "https://core.holo-one.cn/core/"}
        };

        public static IDictionary<string, string> LenovoBaseUrls = new Dictionary<string, string> {
            { "USA", "https://core.naea1.holo-one.lenovo.com"},
            { "Europe", "https://core.euwe1.holo-one.lenovo.com"},
            { "China", "https://core.cnno1.holo-one.lenovo.com"}
        };

        public static IDictionary<string, string> LCPHOSTBaseUrls = new Dictionary<string, string> {
            { "USA", "naea1.uds.lenovo.com"},
            { "Europe", "euwe1.uds.lenovo.com"},
            { "China", "cnno1.uds.lenovo.com"}
        };

        public const string UserAgent = "User-Agent";
        public const string UserAgentValue = "Flurl";

        // Microsoft Graph
        //Set the API Endpoint to Graph 'me' endpoint. 
        // To change from Microsoft public cloud to a national cloud, use another value of graphAPIEndpoint.
        // Reference with Graph endpoints here: https://docs.microsoft.com/graph/deployments#microsoft-graph-and-graph-explorer-service-root-endpoints
        public static string GraphAPIEndpoint = "https://graph.microsoft.com/v1.0/me";

        //Set the scope for API call to user.read
        public static string[] Scopes = new string[] { "user.read" };

        // Below are the clientId (Application Id) of your app registration and the tenant information. 
        // You have to replace:
        // - the content of ClientID with the Application Id for your app registration
        // - The content of Tenant by the information about the accounts allowed to sign-in in your application:
        //   - For Work or School account in your org, use your tenant ID, or domain
        //   - for any Work or School accounts, use organizations
        //   - for any Work or School accounts, or Microsoft personal account, use common
        //   - for Microsoft Personal account, use consumers

        // Holo one AG ClientId: 863ced6e-1061-4c26-9911-e6eef5f64418 (shows Holo one AG on the web dialog that opens for login)
        // Just another ClientId that shows unverified on web dialog: "4a1aa1d5-c567-49d0-ad0b-cd957a47f842";
        public static string ClientId = "863ced6e-1061-4c26-9911-e6eef5f64418";

        // Note: Tenant is important for the quickstart.
        // "common": allows anyone with microsoft account to login
        // Client ID: "f0afb69b-5fee-45d0-bf59-a332c9b695e7";
        // Tenant ID: 0d4ed3fc-659e-476e-b1e1-8a1f041eb6bc
        public static string Tenant = "common";
        public static string Instance = "https://login.microsoftonline.com/"; 
        public static IPublicClientApplication _clientApp;

        public static IPublicClientApplication PublicClientApp { get { return _clientApp; } }
    }
}
