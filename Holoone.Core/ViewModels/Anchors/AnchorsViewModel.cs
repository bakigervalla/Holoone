using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using Holoone.Api.Models;
using HolooneNavis.Models;
using HolooneNavis.Services;
using HolooneNavis.Services.Exporters;
using HolooneNavis.Services.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Application = Autodesk.Navisworks.Api.Application;

namespace HolooneNavis.ViewModels.Anchors
{
    public class AnchorsViewModel : BaseViewModel
    {

        public AnchorsViewModel()
        {
            NavigateToExistingAnchorsPage();
        }

        #region navigation

        public string _state = "AddAnchor";
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

        private ObservableCollection<Anchor> _anchors = new ObservableCollection<Anchor>();
        public ObservableCollection<Anchor> Anchors { get => _anchors; set { _anchors = value; NotifyOfPropertyChange(nameof(_anchors)); } }
        
        public Anchor Anchor { get; set; }

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
            NavigateToAddAnchorPage();
        }

        public void DeleteAnchor(Anchor anchor)
        {
            Anchors.Remove(anchor);
        }

        public void PlaceAnchorOnModel(Anchor anchor)
        {
            EnableMarkerToolPluginCommandHandler();

            Anchors.Add(anchor);
            NavigateToExistingAnchorsPage();
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
                return;
            }
            MarkerSelectionActive = true;
            ToolPluginRecord toolPluginRecord = Application.Plugins.FindPlugin("MarkerPointPicker.ADSK") as ToolPluginRecord;
            Application.MainDocument.Tool.SetCustomToolPlugin(toolPluginRecord.LoadPlugin());
        }

        ///// <summary>
        ///// Opens a dialog to edit an existing marker.
        ///// </summary>
        //private void OpenEditMarkerDialogCommandHandler()
        //{
        //    try
        //    {
        //        Marker markerBeforeEdit = SelectedMarker;

        //        SelectedReview = Reviews.First(x => x.Id == markerBeforeEdit.ReviewId);

        //        var vm = new MarkerDialogViewModel(PluginSetup.Project, SelectedReview, _dataService, _messageService);

        //        if (vm.Show(SelectedMarker))
        //        {
        //            // If the marker was edited, the Marker collection is refreshed.
        //            // If the marker status has changed, the spheres have to be recreated as well to display the new color of the edited marker.

        //            bool sphereColorChanged = markerBeforeEdit.MarkerState != vm.Item.MarkerState;
        //            if (sphereColorChanged)
        //            {
        //                UpdateMarker(markerBeforeEdit, vm.Item);

        //                // Located current SavedView
        //                SelectedMarker = vm.Item;

        //                ExportViewAsImage(SelectedMarker);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AppConstants.Log(ex);
        //    }
        //}

        //private void UpdateMarker(Marker oldMarker, Marker newMarker)
        //{
        //    try
        //    {
        //        //MarkerSphereCreator.ClearMarkersAndSpheres();

        //        //bool spheresExists = MarkerSphereCreator.SphereExists();

        //        //// Activate currentView
        //        //_navisViewService.AtivateSpecificView(newMarker);

        //        //if (spheresExists)
        //        CreateSpheresCommandHandler();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        /// <summary>
        /// Sets the camera position and rotation to look at the selected marker.
        /// </summary>
        //private void GoToMarkerCommandHandler()
        //{
        //    try
        //    {
        //        if (SelectedMarker == null)
        //            return;

        //        Document document = Application.MainDocument;

        //        var beeFolder = document.SavedViewpoints.Value.Single(x => x.DisplayName == "bee Plugin") as FolderItem;
        //        var savedViews = beeFolder.Children.SelectMany(c => (c as FolderItem).Children);
        //        var savedView = savedViews?.FirstOrDefault(m => m.DisplayName == "Marker " + SelectedMarker.Id);

        //        document.SavedViewpoints.CurrentSavedViewpoint = savedView;
        //        //* document.ActiveView.CopyViewpointFrom(((SavedViewpoint)savedView).Viewpoint, ViewChange.JumpCut);

        //        // document.SavedViewpoints.CurrentSavedViewpoint = savedViews.FirstOrDefault(m => m.DisplayName == "Marker " + SelectedMarker.Id);

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        /// <summary>
        ///// Opens a file dialog to export markers or viewpoints.
        ///// </summary>
        ///// <param name="exporter">The exporter that is used after the user selects a file path.</param>
        //private void Export(IExporter exporter)
        //{
        //    try
        //    {
        //        string path = OpenFileDialog(exporter.FileExtension, new SaveFileDialog(), exporter.FileName);
        //        if (path != null)
        //        {
        //            exporter.Export(path);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Export markers as markerworks zip file containing .mw2 file and viewpoint images.
        ///// </summary>
        //private void ExportMarkersAsMarkerWorksFileCommandHandler()
        //{
        //    try
        //    {
        //        SavedItemCollection reviewItems = new SavedViewpointCreator(_dataService).GetReviewFolder(SelectedReview.Name).Children;

        //        Export(new MarkerWorksExporter(SelectedReview, organizer, Markers, PluginSetup.Project, reviewItems));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Exports markers as zip file containing wrl and xml files.
        ///// </summary>
        //private void ExportMarkersAsVdpTagsCommandHandler()
        //{
        //    Export(new VdpTagExporter(PluginSetup.Project, SelectedReview, Markers));
        //}

        ///// <summary>
        ///// Exports markers as mac file.
        ///// </summary>
        //private void ExportMarkersAsMacFileCommandHandler()
        //{
        //    Export(new MacExporter(Markers, SelectedReview));
        //}

        ///// <summary>
        ///// Opens a file dialog and returns the selected file path or null if the dialog is cancelled.
        ///// </summary>
        ///// <returns>The selected file path or null if the dialog is cancelled.</returns>
        //private string OpenFileDialog(string fileType, FileDialog dialog, string filename = "", string initialDirectory = null)
        //{
        //    try
        //    {
        //        dialog.InitialDirectory = initialDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //        dialog.Filter = $"{fileType} files (*.{fileType})|*.{fileType}|All files (*.*)|*.*";
        //        dialog.DefaultExt = fileType;
        //        dialog.AddExtension = true;
        //        dialog.FileName = filename;
        //        dialog.FilterIndex = 2;

        //        if ((bool)dialog.ShowDialog())
        //        {
        //            return dialog.FileName;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        throw;
        //    }
        //}

    }
}
