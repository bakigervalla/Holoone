using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis.Helpers
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
        public override Autodesk.Navisworks.Api.Cursor GetCursor(View view, KeyModifiers modifier)
        {
            return Autodesk.Navisworks.Api.Cursor.Redline;
        }

        /// <summary>
        /// When a user clicks on a 3D model, a ShowMarkerDialogMessage is sent to open the marker dialog.
        /// </summary>
        public override bool MouseDown(View view, KeyModifiers modifiers, ushort button, int x, int y, double timeOffset)
        {
            try
            {
                if (button == 1)
                {
                    var itemResult = view.PickItemFromPoint(x, y);
                    Point3D point = itemResult.Point;

                    // Round the coordinates to 2 digits.
                    var rounded = new Point3D(Math.Round(point.X, 2, MidpointRounding.AwayFromZero), Math.Round(point.Y, 2, MidpointRounding.AwayFromZero),
                         Math.Round(point.Z, 2, MidpointRounding.AwayFromZero));
                    var orgPoint = new Point3D(x, y, 0);

                    // Set the select tool as the active tool, disabling the MarkerPicker.
                    Application.MainDocument.Tool.Set(Tool.Select);

                    //* Messenger.Default.Send(new ShowMarkerDialogMessage(rounded, itemResult.ModelItem, orgPoint));
                }
                else
                {
                    // Uncomment to enable cancellation.
                    // Application.MainDocument.Tool.Set(Tool.Select);
                    // Messenger.Default.Send(new ShowMarkerDialogMessage(cancel: true));
                }

            }
            catch
            {

            }
            return false;
        }
    }

    public class MarkerStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}