using Caliburn.Micro;
using Holoone.Api.Helpers.Constants;
using Holoone.Api.Models;
using HolooneNavis.Services.Interfaces;
using System;
using System.Collections.Generic;
namespace HolooneNavis.ViewModels
{
    public class BaseViewModel : SingletonBaseViewModel
    {
        public BaseViewModel()
        {
           
        }
        public new UserLogin UserLogin { get => Instance.UserLogin; }

    }
}