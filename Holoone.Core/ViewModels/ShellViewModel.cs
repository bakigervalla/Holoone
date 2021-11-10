using Caliburn.Micro;
using Hanssens.Net;
using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels;
using HolooneNavis.ViewModels.Anchor;
using HolooneNavis.ViewModels.Export;
using HolooneNavis.ViewModels.Home;
using HolooneNavis.ViewModels.Login;
using HolooneNavis.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HolooneNavis.ViewModels
{
    public class ShellViewModel : BaseViewModel, IHandle<bool>
    {
        private readonly ILocalStorage _localeStorage;

        public ShellViewModel(
            IHoloNavigationService navigationService,
            ILocalStorage localeStorage,
            IEventAggregator eventAggregator)
        {
            _localeStorage = localeStorage;
            eventAggregator.SubscribeOnPublishedThread(this);

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

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set { _isBusy = value; NotifyOfPropertyChange(nameof(IsBusy)); } }

        public async Task HandleAsync(bool message, CancellationToken cancellationToken)
        {
            await Task.Run(() => IsBusy = message);
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
