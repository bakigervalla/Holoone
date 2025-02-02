﻿using Caliburn.Micro;
using Hanssens.Net;
using HolooneNavis.Models;
using HolooneNavis.Services.Interfaces;
using HolooneNavis.ViewModels.Anchors;
using HolooneNavis.ViewModels.Home;
using HolooneNavis.ViewModels.Settings;
using System.Threading;
using System.Threading.Tasks;

namespace HolooneNavis.ViewModels
{
    public class ShellViewModel : BaseViewModel, IHandle<bool>
    {
        private readonly ILocalStorage _localeStorage;

        public ShellViewModel(
            ILocalStorage localeStorage,
            IEventAggregator eventAggregator)
        {
            _localeStorage = localeStorage;
            eventAggregator.SubscribeOnUIThread(this);

            ShowHomePage();
        }

        public void ShowHomePage() => NavigationService.GoTo<HomeViewModel>();

        public async Task ShowSettingsPage() => await NavigationService.GoTo<SettingsViewModel>();
        public async Task ShowAnchorsPage() => await NavigationService.GoTo<AnchorsViewModel>();

        public async Task Logout()
        {
            Instance.UserLogin.IsLoggedIn = false;
            Instance.UserLogin.UserFullName = "Welcome";
            Instance.UserLogin.Token = string.Empty;

            _localeStorage.Clear();

            await NavigationService.GoTo<HomeViewModel>();
        }

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set { _isBusy = value; NotifyOfPropertyChange(nameof(IsBusy)); } }

        public async Task HandleAsync(bool message, CancellationToken cancellationToken)
        {
            await Task.Run(() => IsBusy = message);
        }

    }
}
