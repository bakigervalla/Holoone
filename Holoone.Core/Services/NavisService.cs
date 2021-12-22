using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
using HolooneNavis.Models;
using HolooneNavis.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
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

        public void HideUnselectedItems()
        {
            ModelItemCollection hidden = new ModelItemCollection();
            //create a store for the visible items
            ModelItemCollection visible = new ModelItemCollection();

            //Add all the items that are visible to the visible collection
            foreach (ModelItem item in Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems)
            {
                if (item.AncestorsAndSelf != null)
                    visible.AddRange(item.AncestorsAndSelf);
                if (item.Descendants != null)
                    visible.AddRange(item.Descendants);
            }
            //mark as invisible all the siblings of the visible items as well as the visible items
            foreach (ModelItem toShow in visible)
            {
                if (toShow.Parent != null)
                {
                    hidden.AddRange(toShow.Parent.Children);
                }
            }
            //remove the visible items from the collection
            foreach (ModelItem toShow in visible)
            {
                hidden.Remove(toShow);
            }
            //hide the remaining items
            Autodesk.Navisworks.Api.Application.ActiveDocument.Models.SetHidden(hidden, true);
        }

        public Task<ModelItemCollection> GetLayers()
        {
            string path = @$"e:\Users\BGERVALLA\Downloads\Navis\NwGeoPrimitives-main\{Guid.NewGuid().ToString()}.txt";

            Document doc = Application.ActiveDocument;
            string currentFilename = doc.CurrentFileName;
            string filename = doc.FileName;
            string title = doc.Title;

            Units units = doc.Units;
            DocumentModels models = doc.Models;
            DocumentInfoPart info = doc.DocumentInfo;
            string currentSheetId = info.Value.CurrentSheetId; // "little_house_2021.rvt"
            DocumentDatabase db = doc.Database;
            bool ignoreHidden = true;
            BoundingBox3D bb = doc.GetBoundingBox(ignoreHidden);
            Point3D min = bb.Min;
            Point3D max = bb.Max;
            int n;

            n = models.Count;

            // Retrieving root item descendants:
            foreach (Model model in models)
            {
                ModelItem rootItem = model.RootItem;

                ModelItemEnumerableCollection mis = rootItem.DescendantsAndSelf;

                n = mis.Count();

                ItemTree mitree = new ItemTree(mis);

                using (StreamWriter writer
                  = new StreamWriter(path))
                {
                    mitree.WriteTo(writer);
                }

                if (50 > n)
                {
                    List<ModelItem> migeos
                      = new List<ModelItem>();

                    foreach (ModelItem mi in mis)
                    {
                        if (mi.HasGeometry)
                        {
                            migeos.Add(mi);
                        }
                    }

                    foreach (ModelItem mi in migeos)
                    {
                        if ("Floor" == mi.DisplayName)
                        {
                            RvtProperties.DumpProperties(mi);
                            RvtProperties props = new RvtProperties(mi);
                            int id = props.ElementId;
                        }
                    }
                }
            }
            return null;
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