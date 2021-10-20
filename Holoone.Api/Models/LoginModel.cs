using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Models
{

    public class LoginSphereCredentials
    {
        [JsonProperty("mode")]
        public string Mode { get; set; } = "raw";

        [JsonProperty("raw")]
        public LoginCredentials Raw { get; set; }

        [JsonProperty("options")]
        public LoginOptions Options { get; set; }
    }

    public class LoginOptions
    {
        [JsonProperty("raw")]
        public LoginLanguage Raw { get; set; }
    }
    public class LoginLanguage
    {
        [JsonProperty("language")]
        public string Language { get; set; } = "json";
    }

    //{
    //	"mode": "raw",
    //	"raw": "{\r\n    \"username\" : \"\",\r\n    \"password\" : \"\"\r\n\r\n}",
    //	"options": {
    //		"raw": {
    //			"language": "json"
    //		}
    //	}
    //}

    public class LoginCredentials
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; set; }
    }

    public class LoginCredentialsGraph
    {
        [JsonProperty("license_type")]
        public string LicenseType { get; set; }

        [JsonProperty("graph_token")]
        public string Token { get; set; }

        [JsonProperty("expires_at")]
        public long ExpiresAt { get; set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; set; }
    }

    public class EmployeeDisplay
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        public string FullName { get { return FirstName + " " + LastName; } }

        [JsonProperty("profile_image")]
        public string RelativeProfileImageUrl { get; set; }

        public string ProfileImageURL { get { return RelativeProfileImageUrl; } }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("job_title")]
        public string JobTitle { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        //[JsonProperty("employee_groups", NullValueHandling = NullValueHandling.Ignore)]
        //public List<Group> Groups { get; set; } = new List<Group>();

        //[JsonProperty("languages")]
        //public List<Language> Languages { get; set; } = new List<Language>();

        [JsonProperty("mobile_linked")]
        public bool MobileLinked { get; set; }

        [JsonProperty("microsoft_graph", NullValueHandling = NullValueHandling.Ignore)]
        public bool HasMicrosoftGraph { get; set; }

        [JsonProperty("permissions", NullValueHandling = NullValueHandling.Ignore)]
        public UserPermissions Permissions { get; set; }

        [JsonProperty("company_white_labeling_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool CompanyWhiteLabelingEnabled { get; set; }

        [JsonProperty("socket_server_url", NullValueHandling = NullValueHandling.Ignore)]
        public string SocketServerIP { get; set; }

        [JsonProperty("use_https", NullValueHandling = NullValueHandling.Ignore)]
        public bool UseHTTPS { get; set; }

        [JsonProperty("eula_signed")]
        public bool EulaSigned { get; set; }

        //public string[] GetLanguageCodes()
        //{
        //    string[] codes = new string[Languages.Count];
        //    for (int i = 0; i < Languages.Count; i++)
        //    {
        //        codes[i] = Languages[i].Code;
        //    }
        //    return codes;
        //}

        public string GetServerProtocol()
        {
            if (UseHTTPS)
            {
                return "https";
            }
            return "http";
        }

        public override string ToString()
        {
            return $"Employee {Id} {FullName}";
        }
    }
}
