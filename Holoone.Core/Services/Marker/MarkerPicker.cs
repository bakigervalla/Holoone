using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using Caliburn.Micro;
using HolooneNavis.Helpers;
using HolooneNavis.Models;
using System;
using View = Autodesk.Navisworks.Api.View;

namespace HolooneNavis.Services
{
    /// <summary>
    /// Navisworks tool plugin that selects a point for a new marker when clicking on a 3D model.
    /// </summary>
    [Plugin("MarkerPointPicker", "ADSK")]
    public class MarkerPicker : ToolPlugin
    {
        public MarkerPicker()
        {
            // For some reason, MarkerPicker is created and activated at application start,
            // so the navisworks tool has to be reset to Tool.Select to prevent the marker window from opening on mouse down.
            Application.ActiveDocument.Tool.Set(Tool.Select);
        }

        /// <summary>
        /// Sets the cursor icon of the marker tool plugin to the redline cursor.
        /// </summary>
        public override Cursor GetCursor(View view, KeyModifiers modifier)
        {
            return Cursor.Redline;
        }

        public override bool MouseUp(View view, KeyModifiers modifiers, ushort button, int x, int y, double timeOffset)
        {
            try
            {
                if (button == 1)
                {
                    var itemResult = view.PickItemFromPoint(x, y);
                    Point3D point = itemResult.Point;

                    // Round the coordinates to 2 digits.
                    var rounded = new Point3D(Math.Round(point.X.FromInternal(), 2, MidpointRounding.AwayFromZero), Math.Round(point.Y.FromInternal(), 2, MidpointRounding.AwayFromZero),
                         Math.Round(point.Z.FromInternal(), 2, MidpointRounding.AwayFromZero));
                    var orgPoint = new Point3D(x, y, 0);

                    // Set the select tool as the active tool, disabling the MarkerPicker.
                    Application.MainDocument.Tool.Set(Tool.Select);

                    var marker = new Marker { Point3D = rounded, ExactPoint3D = orgPoint, ModelItem = itemResult.ModelItem };

                    IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
                    eventAggregator.PublishOnUIThreadAsync(marker);
                }
                else
                {
                    // Enable cancellation.
                    Application.MainDocument.Tool.Set(Tool.Select);
                }

            }
            catch
            {

            }
            return false;
        }

        /// <summary>
        /// When a user clicks on a 3D model, a ShowMarkerDialogMessage is sent to open the marker dialog.
        /// </summary>
        //public override bool MouseDown(View view, KeyModifiers modifiers, ushort button, int x, int y, double timeOffset)
        //{
        //    //try
        //    //{
        //    //    if (button == 1)
        //    //    {
        //    //        var itemResult = view.PickItemFromPoint(x, y);
        //    //        Point3D point = itemResult.Point;

        //    //        // Round the coordinates to 2 digits.
        //    //        var rounded = new Point3D(Math.Round(point.X, 2, MidpointRounding.AwayFromZero), Math.Round(point.Y, 2, MidpointRounding.AwayFromZero),
        //    //             Math.Round(point.Z, 2, MidpointRounding.AwayFromZero));
        //    //        var orgPoint = new Point3D(x, y, 0);

        //    //        // Set the select tool as the active tool, disabling the MarkerPicker.
        //    //        Application.MainDocument.Tool.Set(Tool.Select);

        //    //        _eventAggregator.PublishAsync(new Marker { MarkerPosX = rounded.X, MarkerPosY = rounded.X, MarkerPosZ = rounded.Z, ModelItem = itemResult.ModelItem }, null );
        //    //        //* Messenger.Default.Send(new ShowMarkerDialogMessage(rounded, itemResult.ModelItem, orgPoint));
        //    //    }
        //    //    else
        //    //    {
        //    //        // Uncomment to enable cancellation.
        //    //        // Application.MainDocument.Tool.Set(Tool.Select);
        //    //        // Messenger.Default.Send(new ShowMarkerDialogMessage(cancel: true));
        //    //    }

        //    //}
        //    //catch
        //    //{

        //    //}

        //    return false;
        //}

    }

    public class MarkerStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}