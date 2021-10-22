using Hanssens.Net;
using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using Holoone.Core.Services.Interfaces;
using Holoone.Core.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace Holoone.Core.ViewModels.Login
{
    public class LoginSphereViewModel : BaseViewModel
    {
        private readonly ILoginService _apiLoginService;
        private readonly ILocalStorage _localeStorage;

        public LoginSphereViewModel(
            IHoloNavigationService navigationService,
            ILoginService apiLoginService,
            ILocalStorage localeStorage)
        {
            _apiLoginService = apiLoginService;
            _localeStorage = localeStorage;

            LoginCredentials = new LoginCredentials {};
        }

        #region navigation
        public async Task ShowLoginMicrosoftPage() => await NavigationService.GoTo<LoginMicrosoftViewModel>();
        #endregion

        #region methods
        public async Task LoginAsync()
        {
            if (LoginCredentials.HasErrors)
                return;

            try
            {
                LoginCredentials.DeviceId = new EasClientDeviceInformation().SystemProductName;

                var response = await _apiLoginService.LoginSphereAsync(LoginCredentials);

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
    }
}