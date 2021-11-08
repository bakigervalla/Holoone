using Caliburn.Micro;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis.ViewModels.Login
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel(IHoloNavigationService navigationService)
{ }
            
        public void ShowLoginSpherePage() => NavigationService.GoTo<LoginSphereAndMicrosoftViewModel>();

        public void ShowLoginThinkRealityPage() => NavigationService.GoTo<LoginThinkRealityViewModel>();

    }
}