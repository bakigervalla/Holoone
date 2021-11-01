using Holoone.Core.Services.Interfaces;
using Holoone.Core.ViewModels.Anchor;
using Holoone.Core.ViewModels.Export;
using Holoone.Core.ViewModels.Login;
using System.Threading.Tasks;

namespace Holoone.Core.ViewModels.Home
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