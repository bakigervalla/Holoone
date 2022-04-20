using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using Caliburn.Micro;
using Holoone.Api.Models;
using HolooneNavis.Helpers;
using HolooneNavis.Models;
using HolooneNavis.Services;
using HolooneNavis.Services.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Application = Autodesk.Navisworks.Api.Application;

namespace HolooneNavis.ViewModels.Anchors
{
    public class AnchorsViewModel : BaseViewModel, IHandle<Marker>
    {

        private readonly IEventAggregator _eventAggregator;
        private readonly INavisService _navisService;

        public AnchorsViewModel(IEventAggregator eventAggregator, INavisService navisService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);

            _navisService = navisService;
        }

        #region navigation

        public string _state = "ExistingAnchors";
        public string State { get => _state; set { _state = value; NotifyOfPropertyChange("State"); } }

        public void NavigateToAddAnchorPage()
        {
            State = "AddAnchor";
        }

        public void NavigateToExistingAnchorsPage()
        {
            State = "ExistingAnchors";
        }

        #endregion

        #region members

        private ObservableCollection<Anchor> _anchors = Util.Anchors != null ? Util.Anchors : new ObservableCollection<Anchor>();
        public ObservableCollection<Anchor> Anchors { get => _anchors; set { _anchors = value; NotifyOfPropertyChange(nameof(_anchors)); } }

        private Anchor _anchor;
        public Anchor SelectedAnchor { get => _anchor; set { _anchor = value; NotifyOfPropertyChange(nameof(SelectedAnchor)); } }

        /// <summary>
        /// The selected marker in the DataGrid.
        /// </summary>
        private Marker _selectedMarker;
        public Marker SelectedMarker
        {
            get => _selectedMarker;
            set => Set(ref _selectedMarker, value);
        }

        /// <summary>
        /// If true, the message for selecting a marker position is displayed above the marker datagrid.
        /// </summary>
        private bool _markerSelectionActive;
        public bool MarkerSelectionActive
        {
            get { return _markerSelectionActive; }
            set { Set(ref _markerSelectionActive, value); }
        }

        #endregion

        public void AddNewAnchor()
        {
            SelectedAnchor = new();

            NavigateToAddAnchorPage();
        }

        public void DeleteAnchor(Anchor anchor)
        {
            Anchors.Remove(anchor);
            Util.Anchors = Anchors;

            _navisService.DeleteDocument(anchor.FullName);

            _eventAggregator.PublishOnUIThreadAsync(ViewState.Activate);
        }

        public object PlaceAnchorOnModel()
        {
            SelectedAnchor.IsDirty = true;

            if (SelectedAnchor.ValidateObject(SelectedAnchor).HasErrors)
                return null;

            if (Anchors.Count(x => x.FullName == SelectedAnchor.FullName) > 0)
                return MessageBox.Show("Anchor already exists. Delete or add a new one");

            Anchors.Add(SelectedAnchor);
            Util.Anchors = Anchors;

            EnableMarkerToolPluginCommandHandler();

            _eventAggregator.PublishOnUIThreadAsync(ViewState.Minimize);

            return null;
        }

        /// <summary>
        /// Sets the MarkerPointPicker tool plugin as the current tool in navisworks.
        /// This causes the next click on a 3D-Model to open the marker dialog.
        /// </summary>
        private void EnableMarkerToolPluginCommandHandler()
        {
            if (MarkerSelectionActive)
            {
                MarkerSelectionActive = false;
                Application.MainDocument.Tool.Set(Tool.Select);
            }

            MarkerSelectionActive = true;
            ToolPluginRecord toolPluginRecord = Application.Plugins.FindPlugin("MarkerPointPicker.ADSK") as ToolPluginRecord;
            var plugin = toolPluginRecord.LoadPlugin();
            Application.MainDocument.Tool.SetCustomToolPlugin(plugin);
        }

        public Task HandleAsync(Marker message, CancellationToken cancellationToken)
        {
            SelectedMarker = message as Marker;
            SelectedMarker.Id = (Anchors.Count + 1).ToString();

            //var parentModels = SelectedMarker.ModelItem.Ancestors;
            //List<ModelItem> parents= new();
            //foreach (var parent in parentModels)
            //    parents.Add(parent);

            SelectedAnchor.ParentDocument = SelectedMarker.ModelItem.FindFirstObjectAncestor()?.DisplayName; // parents.First(x => x.ClassDisplayName.Equals("File", System.StringComparison.OrdinalIgnoreCase) && x.Parent == null)?.DisplayName;

            var vrmlPath = Util.MarkerPath(SelectedAnchor.FullName);
            MarkerSphereCreator.CreateMarkerSphere(vrmlPath, SelectedMarker);

            NavigateToExistingAnchorsPage();

            return _eventAggregator.PublishOnUIThreadAsync(ViewState.Normal);
        }

    }
}
