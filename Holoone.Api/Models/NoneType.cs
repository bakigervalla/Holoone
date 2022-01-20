using Holoone.Api.Helpers.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Models
{
    public class RequestNoneType
    {
        [JsonProperty("nonetype")]
        public NoneType NoneType { get; set; }
    }

    public class NoneType
    {
        [JsonProperty("model_type")]
        public string ModelType { get; set; } = "bim";

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty("is_primary")]
        public bool IsPrimary { get; set; } = true;
    }
}
