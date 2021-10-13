﻿using Caliburn.Micro;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using Holoone.Core.Services.Interfaces;
using Holoone.Core.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Core.ViewModels.Login
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel(IHoloNavigationService navigationService) : base(navigationService){}

        public void ShowLoginSphereMicrosoftPage() => NavigationService.GoTo<LoginSphereMicrosoftViewModel>();

        public void ShowLoginThinkRealityPage() => NavigationService.GoTo<LoginThinkRealityViewModel>();

    }
}