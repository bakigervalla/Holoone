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
        [JsonProperty("up_vector_definition")]
        public string UpVectorDefinition { get; set; } = "Yes";

        [JsonProperty("coordinate_system_orientation")]
        public string CoordinateSystem { get; set; } = "Left-handed";

        [JsonProperty("model_size")]
        public string LifeOrTableTopSize { get; set; } = "Tabletop size";

        [JsonProperty("hierarchy_cutoff")]
        public int LevelOfHeirarchy { get; set; } = 1;

        [JsonProperty("model_overlay")]
        public bool Overlay { get; set; }

        [JsonProperty("optimize_model")]
        public bool DecimateModel { get; set; } = true;

        [JsonProperty("remove_hidden_geometry")]
        public bool RemoveHiddenObjects { get; set; }

        [JsonProperty("merge_geometry")]
        public bool MergeGeometry { get; set; } = true;

        [JsonProperty("blocking_collider")]
        public bool BlockingCollider { get; set; }


        [JsonIgnore]
        // [JsonProperty("model_type")]
        public string ModelType { get; set; }

        [JsonIgnore]
        // [JsonProperty("extract_metadata")]
        public short ExtractMetadata { get; set; }

        [JsonIgnore]
        // [JsonProperty("is_primary")]
        public bool IsPrimary { get; set; }
    }
}
