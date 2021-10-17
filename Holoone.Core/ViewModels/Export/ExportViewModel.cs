using Holoone.Core.Services.Interfaces;
using Holoone.Core.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Core.ViewModels.Export
{
    public class ExportViewModel : BaseViewModel
    {

        public ExportViewModel(IHoloNavigationService navigationService) : base(navigationService)
        {

        }

        public void ShowHomePage() => NavigationService.GoTo<HomeViewModel>();

        public async Task ExportAsync(object model)
        {

        }
    }
}
