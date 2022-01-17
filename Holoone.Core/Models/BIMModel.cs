using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis.Models
{
    public class BIMModel
    {
        public BIMModel()
        {
            BIMLayers = new ObservableCollection<BIMLayer>();
        }
        private string _modelName;
        public string ModelName { get; set; }
        private ObservableCollection<BIMLayer> _bimLayers;
        public ObservableCollection<BIMLayer> BIMLayers { get; set; }
    }

    public class BIMLayer
    {
        public string Name { get; set; }

        public ModelItem ModelItem { get; set; }

        public bool Select { get; set; }
        public bool IsSet { get => ModelItem != null; }
    }
}
