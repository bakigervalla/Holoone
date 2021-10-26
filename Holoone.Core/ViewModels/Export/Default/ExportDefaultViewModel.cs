using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using Holoone.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Core.ViewModels.Export.Default
{
    public class ExportDefaultViewModel : BaseViewModel
    {
        private readonly IExportService _exportService;

        public ExportDefaultViewModel(
            IHoloNavigationService navigationService,
            IExportService exportService
            )
        {
            _exportService = exportService;
        }

        #region navigation

        public string _state = "Selection";
        public string State { get => _state; set { _state = value; NotifyOfPropertyChange("State"); } }

        public void NavigateToExportPage()
        {
            State = "ExportData";
        }

        #endregion

        public async Task ExportAsync(ModelItem model)
        {
            var result = await _exportService.ExportModelAsync(model);
        }

    }
}
