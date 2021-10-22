using Caliburn.Micro;
using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using Holoone.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
namespace Holoone.Core.ViewModels
{
    public class BaseViewModel : SingletonBaseViewModel
    {
        public new UserLogin UserLogin { get => Instance.UserLogin; }
    }
}