﻿using Autodesk.Navisworks.Api;
using HolooneNavis.Models;
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
        Task<ModelItemCollection> GetLayers();
        void DeleteDocument(string docName);

        IList<BIMLayer> ExportToNWD(IList<BIMLayer> bimLayersw);
        IList<BIMLayer> ExportToFBX(IList<BIMLayer> bimLayers);
    }
}
