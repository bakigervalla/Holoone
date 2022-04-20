using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Holoone.Api.Models
{
    public class Anchor : BaseModel
    {
        [Required(ErrorMessage = "Anchor Name is required")]
        public string Name { get; set; }

        private string _fullName;
        public string FullName { get { return string.IsNullOrEmpty(_fullName) ? $"sphere_anchor_{Name}" : _fullName ; } set { _fullName = value; } }

        public string ParentDocument { get; set; }
        [JsonIgnore]
        public bool IsDirty { get; set; }
    }
}
