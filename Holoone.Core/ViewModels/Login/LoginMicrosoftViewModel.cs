using Hanssens.Net;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using Holoone.Core.Services.Interfaces;
using Holoone.Core.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace Holoone.Core.ViewModels.Login
{
    public class LoginMicrosoftViewModel : BaseViewModel
    {
        private readonly ILoginService _apiLoginService;
        private readonly ILocalStorage _localeStorage;

        public LoginMicrosoftViewModel(
            IHoloNavigationService navigationService,
            ILoginService apiLoginService,
            ILocalStorage localeStorage)
        {
            _apiLoginService = apiLoginService;
            _localeStorage = localeStorage;

            LoginCredentials = new LoginCredentials { };
        }

        #region methods
        public async Task LoginWithMicrosoftAsync()
        {
            try
            {
                LoginCredentialsGraph credentialsGraph = new LoginCredentialsGraph
                {
                    LicenseType = "lite",
                    Token = "graphToken",
                    ExpiresAt = DateTime.Now.Millisecond, // "expiryOn.ToUnixTimeMilliseconds()",
                    DeviceId = new EasClientDeviceInformation().SystemProductName
            };

                var response = await _apiLoginService.LoginWithMicrosoftAsync(credentialsGraph);

                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    MessageBox.Show("Logged in successfully.");

                    var dynamic = await response.GetJsonAsync();

                    Instance.UserLogin.UserFullName = LoginCredentials.Username;
                    Instance.UserLogin.IsLoggedIn = true;
                    Instance.UserLogin.Token = dynamic.token;

                    _localeStorage.Store("user_login", Instance.UserLogin);

                    await NavigationService.GoTo<HomeViewModel>();
                }
                else
                    MessageBox.Show("Login failed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion



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