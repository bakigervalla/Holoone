﻿using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis.Services.Interfaces
{
    public interface INavisService
    {
        Task<ModelItemCollection> GetModel();
    }
}
