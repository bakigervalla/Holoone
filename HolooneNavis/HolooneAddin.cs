using Autodesk.Navisworks.Api.Plugins;
using Holoone.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Add two new namespaces
using Autodesk.Navisworks.Api;
using Holoone.Core.Views;
using WPFSpark;

namespace HolooneNavis
{
    //[Plugin("Holoone.NavisAddin", "Holoone", DisplayName = "Holoone plugin", ToolTip = "")]
    //[RibbonTab("Holoone.Ribon", DisplayName = "Holoone")]
    // [AddInPlugin(AddInLocation.AddIn, CallCanExecute = 0, CanToggle = true, Icon = "holoone_logo.ico", LargeIcon = "holoone_logo.ico", LoadForCanExecute = true, Shortcut = "", ShortcutWindowTypes = "")]
    //[RibbonLayout("AddinRibbon.xaml")]
    //[RibbonTab("ID_HolooneTab", DisplayName = "Holoone")]
    //[Command("ID_HolooneButton_1", Icon = "holoone_logo.png", LargeIcon = "holoone_logo.png", ToolTip = "Holoone Navisworks Addin")]

    [Plugin("HolooneNavis.HolooneAddin", "HOLO", DisplayName = "Holoone", ToolTip = "")]
    public class HolooneAddin : AddInPlugin
    {
        public static readonly string Path_Plugin = Path.GetDirectoryName(typeof(HolooneAddin).Assembly.Location);

        public override int Execute(params string[] parameters)
        {
            try
            {
                if (Autodesk.Navisworks.Api.Application.IsAutomated)
                {
                    throw new InvalidOperationException("Invalid when running using Automation");
                }

                var bootstraper = new ABootstrapper();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;
        }
    }
}
