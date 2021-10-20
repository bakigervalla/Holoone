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
    public class LoginMicrosoftViewModel : BaseViewModel
    {
        private readonly ILoginService _apiLoginService;

        public LoginMicrosoftViewModel(
            IHoloNavigationService navigationService,
            ILoginService apiLoginService) : base(navigationService)
        {
            _apiLoginService = apiLoginService;
        }


        #region --Public properties--

        private IList<UserPermissions> _userPermissions;
        public IList<UserPermissions> UserPermissions
        {
            get { return _userPermissions; }
            set { NotifyOfPropertyChange(nameof(UserPermissions)); }
        }

        private UserPermissions _userPermission;
        public UserPermissions UserPermission
        {
            get { return _userPermission; }
            set { NotifyOfPropertyChange(nameof(UserPermission)); }
        }

        #endregion

        #region --Private helpers--

        public async Task OnDeleteCommandAsync()
        {
            await _apiLoginService.DeleteLoginAsync(UserPermission);

            await NavigationService.GoTo<HomeViewModel>();
        }

        public async Task OnAddCommandAsync()
        {
            //await _apiLoginService.AddLoginAsync(UserPermission);
        }

        //public async Task ShowLoginSphereMicrosoftAsync()
        //{
        //    await ActivateItemAsync(
        //            new LoginViewModel(
        //                    NavigationService,
        //                    IoC.Get<ILoginService>()
        //                )
        //        );
        //}

        //public async Task ShowLoginThinkRealtyAsync()
        //{
        //    await ActivateItemAsync(
        //            new LoginViewModel(
        //                    NavigationService,
        //                    IoC.Get<ILoginService>()
        //                )
        //        );
        //}

        public async Task LoginAsync()
        {
            //await _apiLoginService.LoginAsync(UserPermission);
            await NavigationService.GoTo<HomeViewModel>();
        }

        #endregion

    }
}