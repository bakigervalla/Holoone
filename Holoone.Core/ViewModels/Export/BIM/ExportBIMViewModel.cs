using HolooneNavis.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis.ViewModels.Export.BIM
{
    public class ExportBIMViewModel : BaseViewModel
    {
        public enum ViewState
        {
            Selection,
            Export
        }

        public ExportBIMViewModel(IHoloNavigationService navigationService)
        {

        }

        public ViewState State { get; set; }

        public async Task ExportAsync(object model)
        {

        }
    }
}
