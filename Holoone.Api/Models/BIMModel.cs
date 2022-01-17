using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Models
{
    public class BIMModel_Suspended : BaseModel
    {
        public BIMModel_Suspended()
        {
            BIMLayers = new ObservableCollection<BIMLayer_Suspended>();
        }
        private string _modelName;
        public string ModelName { get => _modelName; set { _modelName = value; RaisePropertyChanged(); } }
        private ObservableCollection<BIMLayer_Suspended> _bimLayers;
        public ObservableCollection<BIMLayer_Suspended> BIMLayers { get => _bimLayers; set { _bimLayers = value; RaisePropertyChanged(); } }
    }

    public class BIMLayer_Suspended : BaseModel
    {
        private string _name;
        public string Name { get => _name; set { _name = value;  RaisePropertyChanged(); } }
        private bool _select;
        public bool Select { get => _select; set { _select = value; RaisePropertyChanged(); } }
    }
}
