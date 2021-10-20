using Holoone.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Helpers.Constants
{
    public static class RequestConstants
    {
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

        public static UserLogin UserLogin { get; set; } = new UserLogin();


    }
}
