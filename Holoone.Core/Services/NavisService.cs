using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Controls;
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

        public void HideUnselectedItems(string input)
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


            // CreateNewDocument(input);
            SetNewDoc();

            //Document nwDoc;
            //using (var docControl = new DocumentControl())
            //{
            //    // Set the control as the primary document
            //    docControl.SetAsMainDocument();
            //    nwDoc = docControl.Document;
            //}

            //nwDoc.CurrentSelection.CopyFrom(newCollection);
            //nwDoc.PublishFile(@"e:\Users\BGERVALLA\Downloads\baki_1.nwd", new PublishProperties { Author = "baki" });

            //Assign the ModelItemCollection to the selection
            //* Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.CopyFrom(newCollection);


            //hide the remaining items
            //* Autodesk.Navisworks.Api.Application.ActiveDocument.Models.SetHidden(hidden, true);
        }

        private void SetNewDoc()
        {
            Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;

            try
            {
                ////sorting the names of the models             
                //IEnumerable<Model> oNewSortedModels = oDoc.Models.OrderBy(per => per.RootItem.DisplayName);
                //List<string> fileArray = new List<string>();
                //foreach (Model oEachModel in oNewSortedModels)
                //{
                //    fileArray.Add(oEachModel.FileName);
                //}

                //Create a new ModelItemCollection
                ModelItemCollection newCollection = new ModelItemCollection();
                //iterate over the selected Items
                foreach (ModelItem item in Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems)
                {
                    //Add the children of the selected item to a new collection
                    newCollection.AddRange(item.Children);
                }

                var nc = newCollection.ToList();

                //delete all files.
                oDoc.Clear();

                //foreach (Model oEachModel in oDoc.Models)
                //{
                //    // State.DeleteSelectedFiles failed to delete 
                //    // all files at one time. 
                //    // so have to delete them one by one.
                //    ModelItemCollection oMC = new ModelItemCollection();
                //    oMC.Add(oEachModel.RootItem);
                //    oDoc.CurrentSelection.CopyFrom(oMC);
                //    // ComBridge.State.DeleteSelectedFiles();
                //    oDoc.Clear();
                //}

                //append them again with the new order
                oDoc.CurrentSelection.AddRange(nc);
                // oDoc.Append(fileArray);
            }
            catch (Exception ex)
            {

            }
        }

        private int CreateNewDocument(string fileName)
        {
            Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;

            try
            {
                //sorting the names of the models             
                IEnumerable<Model> oNewSortedModels = oDoc.Models.OrderBy(per => per.RootItem.DisplayName);
                List<string> fileArray = new List<string>();
                foreach (Model oEachModel in oNewSortedModels)
                {
                    if (oEachModel.FileName == fileName)
                        fileArray.Add(oEachModel.FileName);
                }

                //delete all files.
                var rs = oDoc.Models.FirstOrDefault();

                foreach (Model oEachModel in oDoc.Models)
                {
                    // State.DeleteSelectedFiles failed to delete 
                    // all files at one time. 
                    // so have to delete them one by one.
                    ModelItemCollection oMC = new ModelItemCollection();
                    oMC.Add(oEachModel.RootItem);
                    oDoc.CurrentSelection.CopyFrom(oMC);
                    Autodesk.Navisworks.Api.ComApi.ComApiBridge.State.DeleteSelectedFiles();
                }

                //append them again with the new order
                oDoc.AppendFiles(fileArray);
            }
            catch (Exception ex)
            {

            }

            return 0;
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

        public void CreateNewBIMModelDocument(IList<BIMLayer> bimLayers)
        {
            ModelItemCollection modelCollection = new ModelItemCollection();

            foreach (var item in bimLayers)
            {
                modelCollection.AddRange(item.ModelItem.Descendants);
            }

            Document nwDoc;
            using (var docControl = new DocumentControl())
            {
                // Set the control as the primary document
                nwDoc = docControl.Document;
                nwDoc.CurrentSelection.CopyFrom(modelCollection);
            }

            nwDoc.PublishFile(Path.Combine(Path.GetTempPath(), DateTime.Now.ToString("yyyyMMddHHmmss"), ".nwd"), new PublishProperties { Author = "Holoone" });
        }

        public ModelItem GetModelItem()
        {
            throw new NotImplementedException();
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