using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Internal.ApiImplementation;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis
{
    public class Item : PropertyChangedBase
    {

        //public Item()
        //{
        //    Children = new List<Item>();
        //}

        private string _displayName { get; set; }
        public string DisplayName { get => _displayName; set { _displayName = value; NotifyOfPropertyChange(nameof(DisplayName)); } }

        public string FilePath { get; set; }
        public bool _isSelected { get; set; }
        public bool IsSelected { get => _isSelected; set { _isSelected = value; NotifyOfPropertyChange(nameof(IsSelected)); } }

        private IList<Item> _children;
        public IList<Item> Children { get => _children; set { _children = value; NotifyOfPropertyChange(nameof(Children)); } }

    }
}
