using Caliburn.Micro;
using Flurl.Http.Configuration;
using Hanssens.Net;
using Holoone.Api.Services;
using Holoone.Api.Services.Interfaces;
using Holoone.Api.Services.MicrosoftGraph;
using HolooneNavis.Services;
using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels;
using HolooneNavis.ViewModels.Anchors;
using HolooneNavis.ViewModels.Export;
using HolooneNavis.ViewModels.Export.BIM;
using HolooneNavis.ViewModels.Export.BIM.Existing;
using HolooneNavis.ViewModels.Export.BIM.New;
using HolooneNavis.ViewModels.Export.Default;
using HolooneNavis.ViewModels.Home;
using HolooneNavis.ViewModels.Login;
using HolooneNavis.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HolooneNavis
{
    public class Bootstrapper : BootstrapperBase
    {
        public SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container = new SimpleContainer();

            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.Singleton<ILoginService, LoginService>();
            _container.Singleton<IExportService, ExportService>();
            _container.Singleton<IFlurlClientFactory, DefaultFlurlClientFactory>();
            _container.Singleton<IHoloNavigationService, HoloNavigationService>();
            _container.Singleton<ILocalStorage, LocalStorage>();
            _container.Singleton<IMicrosoftGraphService, MicrosoftGraphService>();
            
            _container.Singleton<INavisService, NavisService>();

            _container.PerRequest<ShellViewModel>();
            _container.PerRequest<HomeViewModel>();
            _container.PerRequest<SettingsViewModel>();
            _container.PerRequest<LoginViewModel>();
            _container.PerRequest<LoginSphereAndMicrosoftViewModel>();
            _container.PerRequest<LoginThinkRealityViewModel>();
            _container.PerRequest<AnchorsViewModel>();
            _container.PerRequest<ExportViewModel>();
            _container.PerRequest<ExportDefaultViewModel>();
            _container.PerRequest<ExportBIMViewModel>();
            _container.PerRequest<ExportBIMNewViewModel>();
            _container.PerRequest<ExportBIMExistingViewModel>();

            //DisplayRootViewFor<ShellViewModel>();
        }

        protected override void OnStartup(object obj, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return _container.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { Assembly.GetExecutingAssembly() };
        }

    }
}