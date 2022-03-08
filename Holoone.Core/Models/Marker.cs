using Autodesk.Navisworks.Api;
using Holoone.Api.Models;

namespace HolooneNavis.Models
{
    public class Marker : BaseModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public float Radious { get; set; } = .20f;

        public Point3D Point3D { get; set; }

        public Point3D ExactPoint3D { get; set; }

        public double X { get => Point3D.X; } 
        public double Y { get => Point3D.Y; }
        public double Z { get => Point3D.Z; }

        public ModelItem ModelItem { get; set; }
    }
}
