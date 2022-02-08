using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Models
{
    public class ExistingBIM3D
    {
        [JsonProperty("model_name")]
        public string ModelName { get; set; }

        [JsonProperty("primary_layer_id")]
        public string PrimaryLayerId { get; set; }

        [JsonProperty("previous_primary_layer_id")]
        public string PreviousPrimaryLayerId { get; set; }

        [JsonProperty("layers_to_delete")]
        public List<string> LayersToDelete { get; set; }

        [JsonProperty("layers_to_update")]
        public List<string> LayersToUpdate { get; set; }
        //public IEnumerable<LayerToUpdate> LayersToUpdate { get; set; }

        [JsonProperty("layers[]")]
        public IEnumerable<LayerFile> Layers { get; set; }

        // parent_folder

    }

    //[JsonArray]
    public class LayerFile
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("file")]
        public byte[] File { get; set; }
    }

    //[JsonArray]
    public class LayerToUpdate
    {
        public int index { get; set; }
        public int value { get; set; }
    }

}
