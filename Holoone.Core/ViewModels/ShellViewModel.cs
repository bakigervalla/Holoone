using Caliburn.Micro;
using Holoone.Api.Services.Interfaces;
using Holoone.Core.Services.Interfaces;
using Holoone.Core.ViewModels;
using Holoone.Core.ViewModels.Home;
using Holoone.Core.ViewModels.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            //_navigationService = navigationService;
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
            await ShowHomePage();
            await base.OnActivateAsync(cancellationToken);
        }

        private async Task ShowHomePage() => await NavigationService.GoTo<HomeViewModel>();

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
