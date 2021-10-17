using Caliburn.Micro;
using Flurl.Http.Configuration;
using Holoone.Api.Services;
using Holoone.Api.Services.Interfaces;
using Holoone.Core.Services;
using Holoone.Core.Services.Interfaces;
using Holoone.Core.ViewModels;
using Holoone.Core.ViewModels.Home;
using Holoone.Core.ViewModels.Login;
using Holoone.Core.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Holoone.Core
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();

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
            _container.Singleton<IFlurlClientFactory, DefaultFlurlClientFactory>();
            _container.Singleton<IHoloNavigationService, HoloNavigationService>();

            _container.PerRequest<ShellViewModel>();
            _container.PerRequest<HomeViewModel>();
            _container.PerRequest<SettingsViewModel>();
            _container.PerRequest<LoginViewModel>();
            _container.PerRequest<LoginSphereMicrosoftViewModel>();
            _container.PerRequest<LoginThinkRealityViewModel>();
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