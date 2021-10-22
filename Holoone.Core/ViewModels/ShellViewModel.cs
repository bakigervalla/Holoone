using Caliburn.Micro;
using Hanssens.Net;
using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using Holoone.Core.Services.Interfaces;
using Holoone.Core.ViewModels;
using Holoone.Core.ViewModels.Anchor;
using Holoone.Core.ViewModels.Export;
using Holoone.Core.ViewModels.Home;
using Holoone.Core.ViewModels.Login;
using Holoone.Core.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Holoone.Core.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        private readonly ILocalStorage _localeStorage;

        public ShellViewModel(
            IHoloNavigationService navigationService,
            ILocalStorage localeStorage)
        {
            _localeStorage = localeStorage;

            ShowHomePage();
        }

        public void ShowHomePage() => NavigationService.GoTo<HomeViewModel>();

        public async Task ShowSettingsPage() => await NavigationService.GoTo<SettingsViewModel>();

        public async Task Logout()
        {
            Instance.UserLogin.IsLoggedIn = false;
            Instance.UserLogin.UserFullName = "Welcome";
            Instance.UserLogin.Token = string.Empty;

            _localeStorage.Clear();

            await NavigationService.GoTo<HomeViewModel>();
        }

        //public async Task ShowHomeScreenAsync()
        //{
        //    await ActivateItemAsync(
        //            new HomeViewModel(NavigationService)
        //        );
        //}

        //public async Task ShowLoginSphereMicrosoftAsync()
        //{
        //    await ActivateItemAsync(
        //            new LoginViewModel(
        //                    NavigationService,
        //                    IoC.Get<ILoginService>()
        //                )
        //        );
        //}

        //public void ShowLogin()
        //{
        //    ActivateItemAsync(new LoginViewModel(IoC.Get<ILoginService>(), IoC.Get<INavigationService>()));
        //}
    }
}
