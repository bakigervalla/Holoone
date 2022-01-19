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
        void HideUnselectedItems(string input);
        List<string> CreateNewBIMModelDocument(IList<BIMLayer> bimLayers);
        int CreateNewDocument(string fileName, string selected);
    }
}
