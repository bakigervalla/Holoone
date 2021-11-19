using Autodesk.Navisworks.Api;
using Caliburn.Micro;
using Holoone.Api.Services.Interfaces;
using HolooneNavis.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HolooneNavis.ViewModels.Export.BIM.Existing
{
    public class ExportBIMExistingViewModel : BaseViewModel
    {
        private readonly IExportService _exportService;
        private readonly INavisService _navisService;
        private readonly IEventAggregator _eventAggregator;

        public ExportBIMExistingViewModel(
            IHoloNavigationService navigationService,
            IExportService exportService,
            INavisService navisService,
            IEventAggregator eventAggregator
            )
        {
            _exportService = exportService;
            _navisService = navisService;
            _eventAggregator = eventAggregator;
        }

        #region navigation

        public string _state = "SelectLayer";
        public string State { get => _state; set { _state = value; NotifyOfPropertyChange("State"); } }

        public void NavigateToAddLayerlPage()
        {
            State = "AddLayer";
        }

        public void NavigateToSelectModelPage()
        {
            State = "SelectModel";

            QueryNavisModel().AsResult();

            SelectedFiles = new List<string>();
        }

        #endregion

        #region properties

        private ModelItemCollection _navisItems;
        public ModelItemCollection NavisItems { get => _navisItems; set { _navisItems = value; NotifyOfPropertyChange(nameof(NavisItems)); } }

        public IList<string> SelectedFiles { get; set; }

        #endregion

        private async Task QueryNavisModel()
        {
            try
            {
                await _eventAggregator.PublishOnUIThreadAsync(true);

                NavisItems = await _navisService.GetModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                await _eventAggregator.PublishOnUIThreadAsync(false);
            }
        }

        public void GetSelectedModelItemAsync(ModelItem model)
        {
            if (model.Model == null || string.IsNullOrEmpty(model.Model.SourceFileName) || !File.Exists(model.Model.SourceFileName))
                SelectedFiles.Add(Autodesk.Navisworks.Api.Application.ActiveDocument.FileName);
            else
                SelectedFiles.Add(model.Model.SourceFileName);
        }

        public async Task AddLayerAsync()
        {

        }

    }
}
