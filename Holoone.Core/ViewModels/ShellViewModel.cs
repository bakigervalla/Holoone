using Caliburn.Micro;
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

        private readonly ILoginService _apiLoginService;
        //private readonly IHoloNavigationService _navigationService;

        public ShellViewModel(
            ILoginService apiLoginService,
            IHoloNavigationService navigationService) : base(navigationService)
        {
            _apiLoginService = apiLoginService;
            
            ShowHomePage();
        }

        /// <summary>
        /// add logic which should execute only the first time that the screen is activated. After initialization is complete, IsInitialized will be true.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            return base.OnInitializeAsync(cancellationToken);
        }

        /// <summary>
        /// add logic which should execute every time the screen is activated. After activation is complete, IsActive will be true.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
        }

        public bool IsUserLoggedIn { get; set; }

        public SolidColorBrush _theme = Application.Current.Resources["GrayDark1Color"] as SolidColorBrush;
        public SolidColorBrush Theme
        {
            get => _theme;
            set { _theme = value; NotifyOfPropertyChange(nameof(Theme)); }
        }

        public string _themeName = "Dark Theme";
        public string ThemeName
        {
            get => _themeName;
            set { _themeName = value; NotifyOfPropertyChange(nameof(ThemeName)); }
        }

        public void ToggleTheme()
        {
            if (Theme.Color == (Application.Current.Resources["GrayDark1Color"] as SolidColorBrush).Color)
            {
                Theme = Application.Current.Resources["WhiteColor"] as SolidColorBrush;
                ThemeName = "White Theme";
            }
            else
            {
                Theme = Application.Current.Resources["GrayDark1Color"] as SolidColorBrush;
                ThemeName = "Dark Theme";
            }
        }

        public void ShowHomePage() => NavigationService.GoTo<HomeViewModel>();

        public async Task ShowViewPage() => await NavigationService.GoTo<HomeViewModel>();

        public async Task ShowSpherePage() => await NavigationService.GoTo<HomeViewModel>();

        public async Task ShowSettingsPage() => await NavigationService.GoTo<SettingsViewModel>();

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
