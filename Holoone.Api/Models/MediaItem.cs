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
        public MediaItem(string filePath)
        {
            byte[] fileStream = System.IO.File.ReadAllBytes(filePath);

            FormData = new List<Item>
            {
                new Item
                {
                    Key = "file",
                    Type = "byte",
                    Src = fileStream
                },
                new Item
                {
                    Key = "display_name",
                    Value = System.IO.Path.GetFileNameWithoutExtension(filePath),
                    Type = "text"
                },
                new Item
                {
                    Key = "parent_folder",
                    Type = "text"
                },
                new Item
                {
                    Key = "file_extension",
                    Value = System.IO.Path.GetExtension(filePath),
                    Type = "text"
                },
                new Item
                {
                    Key = "processing_params",
                    Value = "{\"processing_params\": {\"model_type\": \"default\", \"up_vector_definition\": \"y\", \"coordinate_system_orientation\": \"left_hand\", \"model_size\": \"tabletop\", \"model_overlay\": 0, \"optimize_model\": 1, \"remove_hidden_geometry\": 0, \"merge_geometry\": 1, \"hierarchy_cutoff\": 1, \"extract_metadata\": 0, \"blocking_collider\" : 0}, \"is_primary\": false}",
                    Type = "text"
                },
            };
        }

        [JsonProperty("mode")]
        public string Mode { get; set; } = "formdata";

        [JsonProperty("formdata")]
        public List<Item> FormData { get; set; }
    }

    public class Item
    {

        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        public byte[] Src { get; set; }
    }

    //[JsonProperty("file")]
    //public StreamContent File { get; set; }

    //[JsonProperty("display_name")]
    //public string DisplayName { get; set; }

    //[JsonProperty("parent_folder")]
    //public string ParentFolder { get; set; }

    //[JsonProperty("file_extension")]
    //public string FileExtension { get; set; }

    //[JsonProperty("processing_params")]
    //public ProcessingParams ProcessingParams { get; set; }
    //}

    public class ProcessingParams
    {
        [JsonProperty("model_type")]
        public string ModelType { get; set; }

        [JsonProperty("up_vector_definition")]
        public string UpVectorDefinition { get; set; }

        [JsonProperty("coordinate_system_orientation")]
        public string CoordinateSystemOrientation { get; set; }

        [JsonProperty("model_size")]
        public string ModelSize { get; set; }

        [JsonProperty("model_overlay")]
        public short ModelOverlay { get; set; }

        [JsonProperty("optimize_model")]
        public short OptimizeModel { get; set; }

        [JsonProperty("remove_hidden_geometry")]
        public short RemoveHiddenGeometry { get; set; }

        [JsonProperty("merge_geometry")]
        public short MergeGeometry { get; set; }

        [JsonProperty("hierarchy_cutoff")]
        public short HierarchyCutoff { get; set; }

        [JsonProperty("extract_metadata")]
        public short ExtractMetadata { get; set; }

        [JsonProperty("blocking_collider")]
        public short BlockingCollider { get; set; }

        [JsonProperty("is_primary")]
        public bool IsPrimary { get; set; }

    }
}
