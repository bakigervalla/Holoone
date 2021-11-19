using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels.Export.BIM.Existing;
using HolooneNavis.ViewModels.Export.BIM.New;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis.ViewModels.Export.BIM
{
    public class ExportBIMViewModel : BaseViewModel
    {
        public void ShowBIMNewPage() => NavigationService.GoTo<ExportBIMNewViewModel>();
        public void ShowBIMExistingPage() => NavigationService.GoTo<ExportBIMExistingViewModel>();
    }
}
