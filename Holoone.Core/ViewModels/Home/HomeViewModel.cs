using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels.Anchor;
using HolooneNavis.ViewModels.Export;
using HolooneNavis.ViewModels.Login;
using System.Threading.Tasks;

namespace HolooneNavis.ViewModels.Home
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel(IHoloNavigationService navigationService) //: base(navigationService)
        {
            
        }

        public void ShowLoginPage() => NavigationService.GoTo<LoginViewModel>();

        public async Task ShowAnchorPage() => await NavigationService.GoTo<AnchorViewModel>();

        public async Task ShowExportPage() => await NavigationService.GoTo<ExportViewModel>();
    }
}