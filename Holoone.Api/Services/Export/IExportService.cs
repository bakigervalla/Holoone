﻿using Flurl.Http;
using Holoone.Api.Models;
using Holoone.Api.Services.MicrosoftGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Services.Interfaces
{
    public interface IExportService
    {
        Task<IFlurlResponse> ExportModelAsync(ModelItem modelItem);
    }
}