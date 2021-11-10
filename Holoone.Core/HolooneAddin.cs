using Autodesk.Navisworks.Api.Plugins;
using Caliburn.Micro;
using Flurl.Http;
using Flurl.Http.Configuration;
using Holoone.Api.Helpers.Constants;
using HolooneNavis;
using HolooneNavis.ViewModels;
using HolooneNavis.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HolooneNavis
{
    //[Plugin("Holoone.NavisAddin", "Holoone", DisplayName = "Holoone plugin", ToolTip = "")]
    //[RibbonTab("Holoone.Ribon", DisplayName = "Holoone")]
    // [AddInPlugin(AddInLocation.AddIn, CallCanExecute = 0, CanToggle = true, Icon = "holoone_logo.ico", LargeIcon = "holoone_logo.ico", LoadForCanExecute = true, Shortcut = "", ShortcutWindowTypes = "")]
    //[RibbonLayout("AddinRibbon.xaml")]
    //[RibbonTab("ID_HolooneTab", DisplayName = "Holoone")]
    //[Command("ID_HolooneButton_1", Icon = "holoone_logo.png", LargeIcon = "holoone_logo.png", ToolTip = "Holoone Navisworks Addin")]

    [AddInPlugin(AddInLocation.AddIn,
        CanToggle = true,
        LoadForCanExecute = true,
        CallCanExecute = CallCanExecute.Always,
        Icon = @"holoone_logo.png",
        LargeIcon = @"holoone_logo.png",
        Shortcut = "Ctrl+Shift+H",
        ShortcutWindowTypes = "")]
    [Plugin("HolooneNavis.HolooneAddin", "HOLO", DisplayName = "Holoone", ToolTip = "", SupportsIsSelfEnabled = true)]
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

                // Assembly resolver
                AppDomain.CurrentDomain.AssemblyResolve += ForceLibraryLoad;

                //MergeDefaultAppConfig();

                EnsureApplicationResources();

                var bootstraper = new Bootstrapper();

                var windowManager = IoC.Get<IWindowManager>();
                var shellViewModel = (Screen)IoC.Get<ShellViewModel>();
                var result = windowManager.ShowWindowAsync(shellViewModel, null);

                ////Get the ViewModel for the screen from Container
                //ShellViewModel relayListViewModel = bootstraper._container.GetInstance<ShellViewModel>();
                //IWindowManager windowManager = bootstraper._container.GetInstance<IWindowManager>();
                //windowManager.ShowWindowAsync(relayListViewModel);

                //// Open Plugin Dialog that will run the Main Plugin 
                //OpenPluginWindow dialog = new OpenPluginWindow();
                //dialog.Topmost = true;
                //dialog.ShowDialog();

                //var view = new ShellView(); //shellViewModel.GetView() as Window;
                //view.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;
        }

        private void MergeDefaultAppConfig()
        {

            var configPath = Path.GetDirectoryName(Path.Combine(Assembly.GetExecutingAssembly().Location, "HolooneNavis.dll.config")) ?? string.Empty;

            AppConfig.Change(configPath);
        }

        private void EnsureApplicationResources()
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            // merge in your application resources
            Application.Current.Resources.MergedDictionaries.Add( Application.LoadComponent(
                    new Uri("HolooneNavis;component/Resources/Styles/Style.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
                    new Uri("MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Button.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
                    new Uri("MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
                    new Uri("MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
                    new Uri("MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.LightBlue.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);

            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
        new Uri("MaterialDesignExtensions;component/Themes/Generic.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
        new Uri("HolooneNavis;component/Resources/Styles/Colors.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
        new Uri("HolooneNavis;component/Resources/Styles/Buttons.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
        new Uri("HolooneNavis;component/Resources/Styles/ScrollBar.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
        new Uri("HolooneNavis;component/Resources/Styles/ToolTip.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(
        new Uri("HolooneNavis;component/Resources/Styles/TreeView.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);
        }

        /// <summary>
        /// Force loading of libs by creating a dummy object once
        /// </summary>
        private Assembly ForceLibraryLoad(object sender, ResolveEventArgs args)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string[] assemblies = new string[] { "Newtonsoft.Json.dll", "LocalStorage.dll", "Microsoft.Xaml.Behaviors.dll" };

            foreach (string assembly in assemblies)
                Assembly.LoadFrom(Path.Combine(assemblyPath, assembly));

            return null;
            //// For Revit 2017, force loading of System.Windows.Interactivity
            //// otherwise, an error will happen when trying to reference interactivity from XAML
            //// see https://stackoverflow.com/questions/13514027/could-not-load-file-or-assembly-system-windows-interactivity
            //EventTrigger t = new EventTrigger();
        }

    }


}
