using Autodesk.Navisworks.Api;
using Holoone.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HolooneNavis.Models
{
    public class Marker : BaseModel
    {
        [XmlIgnore]
        public string Id { get; set; }
        /// <summary>
        /// The 3D model that is used for metadata.
        /// </summary>
        public ModelItem ModelItem { get; set; }
        public long MarkerNumber { get; set; }
        public string MarkerName { get; set; }

        public double MarkerPosX { get; set; }
        public double MarkerPosY { get; set; }
        public double MarkerPosZ { get; set; }

        public string SavedViewPointName { get; set; }
    }
}
