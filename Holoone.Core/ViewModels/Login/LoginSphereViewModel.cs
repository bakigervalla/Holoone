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

        public LoginSphereViewModel(
            IHoloNavigationService navigationService,
            ILoginService apiLoginService) : base(navigationService)
        {
            _apiLoginService = apiLoginService;

            // CRITICAL: remove on deploy
            LoginCredentials = new LoginCredentials { Username = "baki.test@holo-one.com" };
        }


        #region navigation
        public async Task ShowLoginMicrosoftPage() => await NavigationService.GoTo<LoginMicrosoftViewModel>();

        #endregion

        #region methods

        public async Task<int> LoginAsync(object parameter)
        {

            if (LoginCredentials.HasErrors)
                return 0;

            var deviceInformation = new EasClientDeviceInformation();

            //var passwordBox = parameter as System.Windows.Controls.PasswordBox;
            //LoginCredentials.Password = passwordBox.Password;
            //LoginCredentials.DeviceId = deviceInformation.SystemProductName;

            try
            {
                var response = await _apiLoginService.LoginAsync(LoginCredentials);

                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    MessageBox.Show("Logged in successfully.");

                    RequestConstants.UserLogin.UserFullName = $"{RequestConstants.UserLogin.UserFullName} {LoginCredentials.Username}";
                    RequestConstants.UserLogin.IsLoggedIn = true;
                    RequestConstants.UserLogin.Token = "Sphere";

                    await NavigationService.GoTo<HomeViewModel>();
                }
                else
                    MessageBox.Show("Login failed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return default;
            //var request = await Post(url, credentials);
            //if (request.Response.IsSuccess)
            //{
            //    var jsonToken = JObject.Parse(request.Response.DataAsText);
            //    PlayerPrefConstants.AuthToken.Value = ((string)jsonToken["token"]);
            //    PlayerPrefs.Save();
            //    return request.Response.StatusCode;
            //}
            // return request.Response.StatusCode;
        }

        #endregion

        #region --Public properties--

        //private IList<LoginCredentials> _loginCredentials;
        //public IList<LoginCredentials> LoginCredentials
        //{
        //    get { return _loginCredentials; }
        //    set { NotifyOfPropertyChange(nameof(LoginCredentials)); }
        //}

        //private UserPermissions _userPermission;
        //public UserPermissions UserPermission
        //{
        //    get { return _userPermission; }
        //    set { NotifyOfPropertyChange(nameof(UserPermission)); }
        //}

        #endregion

        #region --Private helpers--

        //public async Task OnDeleteCommandAsync()
        //{
        //    await _apiLoginService.DeleteLoginAsync(UserPermission);

        //    await NavigationService.GoTo<HomeViewModel>();
        //}

        //public async Task OnAddCommandAsync()
        //{
        //    await _apiLoginService.AddLoginAsync(UserPermission);
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

        //public async Task ShowLoginThinkRealtyAsync()
        //{
        //    await ActivateItemAsync(
        //            new LoginViewModel(
        //                    NavigationService,
        //                    IoC.Get<ILoginService>()
        //                )
        //        );
        //}


        #endregion

    }
}