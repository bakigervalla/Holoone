using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Models
{
    public class MediaItem
    {
        public MediaItem() {}

        [JsonProperty("mode")]
        public string Mode { get; set; } = "formdata";

        [JsonProperty("formdata")]
        public List<FormData> FormData { get; set; }

        [JsonIgnore]
        public RequestProcessingParams RequestProcessingParams { get; set; }
    }

    public class FormData
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }

}
