using Hanssens.Net;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels.Home;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace HolooneNavis.ViewModels.Login
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

            // !CRITICAL: Remove on live
            LoginCredentials = new LoginCredentials { }; // Username = "baki.test@holo-one.com", Password = "g6hN!(3#" };
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
                    var dynamic = await response.GetJsonAsync();

                    Instance.UserLogin.Username = LoginCredentials.Username;
                    Instance.UserLogin.Password = LoginCredentials.Password;
                    Instance.UserLogin.UserFullName = LoginCredentials.Username;
                    Instance.UserLogin.LoginType = new LoginType { Type = "Sphere", Region = LoginCredentials.Hosts.Single(x => x.IsChecked).Text };
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
                var response = await _apiLoginService.LoginWithMicrosoftAsync(LoginCredentials);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Instance.UserLogin = response.User;
                    Instance.UserLogin.Username = response.User.Username;
                    Instance.UserLogin.Password = response.User.Password;
                    Instance.UserLogin.UserFullName = response.User.UserFullName;
                    Instance.UserLogin.IsLoggedIn = true;
                    Instance.UserLogin.LoginType = new LoginType { Type = "Microsoft", Region = LoginCredentials.Hosts.Single(x => x.IsChecked).Text };
                    Instance.UserLogin.Token = response.User.Token;

                    _localeStorage.Store("user_login", Instance.UserLogin);

                    await NavigationService.GoTo<HomeViewModel>();
                }
                else
                    MessageBox.Show("Login failed.");
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
                MessageBox.Show("Login failed");
            }
        }

        #endregion
    }
}