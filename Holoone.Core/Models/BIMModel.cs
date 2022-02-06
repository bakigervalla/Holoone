using Autodesk.Navisworks.Api;
using Holoone.Api.Models;
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
        public string ModelName { get; set; }
        public ObservableCollection<BIMLayer> BIMLayers { get; set; }
    }

    public class BIMLayer : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private ModelItem _modelItem { get; set; }
        public ModelItem ModelItem { get => _modelItem; set { _modelItem = value; RaisePropertyChanged(nameof(ModelItem)); } }

        private string _filePath { get; set; }
        public string FilePath { get => _filePath; set { _filePath = value; RaisePropertyChanged(nameof(FilePath)); } }

        private bool _isDefault;
        public bool IsDefault { get => _isDefault; set { _isDefault = value; RaisePropertyChanged(nameof(IsDefault)); } }

        private bool _isSet;
        public bool IsSet { get => _isSet; set { _isSet = value; RaisePropertyChanged(nameof(IsSet)); } }
    }
}
