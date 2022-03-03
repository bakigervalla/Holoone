using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Models
{
    public class Anchor
    {
        public string Name { get; set; }
        public string FullName { get { return $"sphere_anchor_{Name}"; } }
        public bool IsDeleted { get; set; }
    }
}
