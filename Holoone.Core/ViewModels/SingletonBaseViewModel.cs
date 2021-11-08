using Caliburn.Micro;
using Holoone.Api.Models;
using HolooneNavis.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HolooneNavis.ViewModels
{
    public class SingletonBaseViewModel : Conductor<object>
    {
        public static IHoloNavigationService NavigationService { get; private set; } = IoC.Get<IHoloNavigationService>();

        private static readonly Lazy<SingletonBaseViewModel> sInstance = new Lazy<SingletonBaseViewModel>(() => CreateInstanceOfT());

        public static SingletonBaseViewModel Instance { get { return sInstance.Value; } }

        private static SingletonBaseViewModel CreateInstanceOfT()
        {
            return Activator.CreateInstance(typeof(SingletonBaseViewModel), true) as SingletonBaseViewModel;
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            return base.OnInitializeAsync(cancellationToken);
        }

        private UserLogin _userLogin = new UserLogin();
        public UserLogin UserLogin { get => _userLogin; set { _userLogin = value; NotifyOfPropertyChange(nameof(UserLogin)); } }

        public LoginCredentials LoginCredentials { get; set; }

        public EmployeeDisplay EmployeeDisplay { get; set; }
    }
}
