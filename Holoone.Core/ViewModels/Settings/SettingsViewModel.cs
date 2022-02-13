using Holoone.Api.Helpers.Constants;
using HolooneNavis.Services.Interfaces;

namespace HolooneNavis.ViewModels.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel(IHoloNavigationService navigationService) //: base(navigationService)
        {

        }

        public string ApiVersion => $"API Version: {RequestConstants.API_VERSION}";
    }
}
