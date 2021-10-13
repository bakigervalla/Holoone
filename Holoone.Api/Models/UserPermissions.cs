using Holoone.Api.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Models
{
    public class UserPermissions : IResponse
    {
        // users

        [JsonProperty("manage_employees")]
        public bool ManageEmployees { get; set; }

        [JsonProperty("view_employees")]
        public bool ViewEmployees { get; set; }

        [JsonProperty("view_skills")]
        public bool ViewSkills { get; set; }

        // files

        [JsonProperty("manage_files")]
        public bool ManageFiles { get; set; }

        [JsonProperty("view_allfiles")]
        public bool ViewAllFiles { get; set; }

        [JsonProperty("view_files")]
        public bool ViewFiles { get; set; }

        // workflows

        [JsonProperty("manage_workflows")]
        public bool ManageWorkflows { get; set; }

        [JsonProperty("view_all_workflows")]
        public bool ViewAllWorkflows { get; set; }

        [JsonProperty("view_workflows")]
        public bool ViewWorkflows { get; set; }

        // equipment

        [JsonProperty("view_all_equipments")]
        public bool ViewAllEquipments { get; set; }

        [JsonProperty("view_equipments")]
        public bool ViewEquipments { get; set; }

        // other

        [JsonProperty("share_content")]
        public bool ShareContent { get; set; }

        [JsonProperty("view_livepanels")]
        public bool ViewLivePanels { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        [JsonIgnore]
        public string Content { get; set; }

        [JsonIgnore]
        public byte[] RawContent { get; set; }
    }
}
