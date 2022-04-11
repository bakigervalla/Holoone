using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Holoone.Api.Models
{
    public class Anchor : BaseModel
    {
        [Required(ErrorMessage = "Anchor Name is required")]
        public string Name { get; set; }
        public string FullName { get { return $"sphere_anchor_{Name}"; } }

        public string ParentDocument { get; set; }
        [JsonIgnore]
        public bool IsDirty { get; set; }
    }
}
