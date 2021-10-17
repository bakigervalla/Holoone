using Caliburn.Micro;
using Holoone.Api.Models;
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

        public LoginCredentials LoginCredentials { get; set; }

        public EmployeeDisplay EmployeeDisplay { get; set; }

        public string UserFullName { get =>
                LoginCredentials != null
                ? $"Welcome {LoginCredentials.Username}"
                    : EmployeeDisplay != null
                      ? $"Welcome {EmployeeDisplay.FirstName} {EmployeeDisplay.LastName}"
                      : "Welcome";
        }

    }
}
