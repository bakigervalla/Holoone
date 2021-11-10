using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
using HolooneNavis.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolooneNavis.Services
{
    public class NavisService : INavisService
    {
        public async Task<ModelItemCollection> GetModel()
        {
            ModelItemCollection items = new();

            return await Task.Run(() =>
            {
                Document doc = Application.ActiveDocument;
                DocumentModels models = doc.Models;

                if (models == null || models.Count() == 0)
                    return new ModelItemCollection();

                Model model = models.First;
                ModelItem rootItem = model.RootItem;

                items.Add(rootItem);

                return items;
            });
        }


        //IEnumerable<ModelItem> modelItems = rootItem.DescendantsAndSelf;

        //Guid PlaceholderGuid = new Guid("00000000-0000-0000-0000-000000000000");

        //        foreach (ModelItem mi in modelItems)
        //            if (mi.InstanceGuid != PlaceholderGuid)
        //                items.Add(mi);

        // ModelItemEnumerableCollection rootItems = models.RootItems;


        //return rootItem.DescendantsAndSelf.Where(x => x.ClassDisplayName.Equals("File", StringComparison.OrdinalIgnoreCase))
        //    .Select(c => new Item
        //    {
        //        DisplayName = c.DisplayName,
        //        FilePath = c.DisplayName,
        //        Children = c.Children.Select(f => new Item { DisplayName = f.DisplayName, FilePath = f.DisplayName }).ToList()
        //    }).ToList();

        //IList<Item> result = new List<Item>();

        //foreach (ModelItem modelItem in modelItems)
        //{
        //    result.Add(new Item
        //    {
        //        DisplayName = modelItem.DisplayName,
        //        FilePath = "",
        //        Children = modelItem.Children.Select(x => new Item { DisplayName = x.DisplayName, FilePath = "" }).ToList()
        //    });
        //}

        //return result;

    }
}
