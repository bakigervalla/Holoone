using Holoone.Api.Helpers.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Models
{
    public class RequestProcessingParams
    {
        [JsonProperty("processing_params")]
        public ProcessingParams ProcessingParams { get; set; }
    }

    public class ProcessingParams
    {
        [JsonProperty("model_type")]
        public string ModelType { get; set; } = "default";

        [JsonProperty("up_vector_definition")]
        public string UpVectorDefinition { get; set; } = "y";

        [JsonProperty("coordinate_system_orientation")]
        public string CoordinateSystem { get; set; } = "left_hand";

        [JsonProperty("model_size")]
        public string LifeOrTableTopSize { get; set; } = "tabletop";

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty("model_overlay")]
        public bool Overlay { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty("optimize_model")]
        public bool DecimateModel { get; set; } = true;

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty("remove_hidden_geometry")]
        public bool RemoveHiddenObjects { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty("merge_geometry")]
        public bool MergeGeometry { get; set; } = true;

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty("hierarchy_cutoff")]
        public bool LevelOfHeirarchy { get; set; } = true;

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty("extract_metadata")]
        public bool ExtractMetadata { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        [JsonProperty("blocking_collider")]
        public bool BlockingCollider { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        [JsonIgnore]
        // [JsonProperty("is_primary")]
        public bool IsPrimary { get; set; }
    }
}
