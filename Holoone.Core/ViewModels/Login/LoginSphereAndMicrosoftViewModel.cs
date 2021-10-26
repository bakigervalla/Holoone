using Hanssens.Net;
using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using Holoone.Core.Helpers.Extensions;
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
    public class LoginSphereAndMicrosoftViewModel : BaseViewModel
    {
        private readonly ILoginService _apiLoginService;
        private readonly ILocalStorage _localeStorage;

        public LoginSphereAndMicrosoftViewModel(
            IHoloNavigationService navigationService,
            ILoginService apiLoginService,
            ILocalStorage localeStorage)
        {
            _apiLoginService = apiLoginService;
            _localeStorage = localeStorage;

            // CRITICAL: remove on deploy
            LoginCredentials = new LoginCredentials { Username = "baki.test@holo-one.com" };
        }

        #region methods

        public async Task LoginWithSphereAsync()
        {
            LoginCredentials.IsDirty = true;

            if (LoginCredentials.ValidateObject(LoginCredentials).HasErrors)
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

        public async Task LoginWithMicrosoftAsync()
        {
            try
            {
                var response = await _apiLoginService.LoginWithMicrosoftAsync();

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("Logged in successfully.");

                    Instance.UserLogin = response.User;

                    _localeStorage.Store("user_login", Instance.UserLogin);

                    await NavigationService.GoTo<HomeViewModel>();
                }
                else
                    MessageBox.Show("Login failed.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                MessageBox.Show("Login failed");
            }
        }

        #endregion
    }
}