using Hanssens.Net;
using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using HolooneNavis.Helpers;
using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels.Home;
using HolooneNavis.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HolooneNavis.ViewModels.Login
{
    public class LoginThinkRealityViewModel : BaseViewModel
    {
        private readonly ILoginService _apiLoginService;
        private readonly ILocalStorage _localeStorage;

        public LoginThinkRealityViewModel(
            IHoloNavigationService navigationService,
            ILoginService apiLoginService,
            ILocalStorage localeStorage)
        {
            _apiLoginService = apiLoginService;
            _localeStorage = localeStorage;

            LoginCredentials = new LoginCredentials { };

            // BrowserConfig.SetWebBrowserFeatures();
        }

        public LCPOrganization LCPOrganization { get; set; } = new();

        private void NavigateWebView(string Url)
        {
            var window2 = new BrowserWindow(Url);
            window2.Show();
        }

        public async Task PLCLoginAsync()
        {
            try
            {
                if (LCPOrganization.ValidateObject(LCPOrganization).HasErrors)
                    return;

                string deviceId = Util.GetDeviceIdentifier(DeviceType.MAC_ADDRESS);

                // Open Browser for loging
                string hostKey = LoginCredentials.Hosts.Single(x => x.IsChecked).Text;
                string regionUrl = RequestConstants.LenovoBaseUrls[hostKey];

                // Generate Code
                string temp_auth_token = await _apiLoginService.LCPLoginGenerateAuthCode(regionUrl, deviceId);

                if (string.IsNullOrEmpty(temp_auth_token))
                {
                    MessageBox.Show("Could not generate authentication code");
                    return;
                }

                // Open Browser for Login and Account validation

                regionUrl += @$"integration/thinkreality/authorize/?orgID={LCPOrganization.Organization}&origin=external_browser&device_id={deviceId}&auth_code={temp_auth_token}";

                //NavigateWebView(regionUrl);
                //return;

                Process.Start(regionUrl);
                Thread.Sleep(2000);

                // Get Token
                LCPLogin token = null;

                async Task<LCPLogin> loginPolling()
                {
                    regionUrl = RequestConstants.LenovoBaseUrls[hostKey];
                    dynamic result = await _apiLoginService.LCPLoginPolling(regionUrl, deviceId, temp_auth_token);

                    return result;
                };

                while (token is null)
                {
                    token = await loginPolling();
                    Thread.Sleep(1000);
                }
                
                if(!string.IsNullOrEmpty(token.AccessToken))
                {
                    Instance.UserLogin.Username = ""; // LoginCredentials.Username;
                    Instance.UserLogin.Password = ""; // LoginCredentials.Password;
                    Instance.UserLogin.UserFullName = "LCP Account"; // LoginCredentials.Username;
                    Instance.UserLogin.IsLoggedIn = true;
                    Instance.UserLogin.LoginType = new LoginType { Type = "LCP", Region = LoginCredentials.Hosts.Single(x => x.IsChecked).Text };
                    Instance.UserLogin.Token = token.AccessToken;
                    Instance.UserLogin.RefreshToken = token.RefreshToken;

                    _localeStorage.Store("user_login", Instance.UserLogin);

                    await NavigationService.GoTo<HomeViewModel>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
