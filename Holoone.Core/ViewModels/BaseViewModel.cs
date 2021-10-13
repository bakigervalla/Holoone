using Caliburn.Micro;
using Holoone.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Core.ViewModels
{
    public abstract class BaseViewModel : Conductor<object>
    {
        
        public IHoloNavigationService NavigationService { get; private set; }

        public BaseViewModel(IHoloNavigationService navigationService)
        {
            NavigationService = navigationService;
        }


    }
}
