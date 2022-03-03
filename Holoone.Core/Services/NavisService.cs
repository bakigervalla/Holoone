using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Controls;
using Autodesk.Navisworks.Api.DocumentParts;
using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Internal.ApiImplementation;
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

                // Model model = models.First;
                //ModelItem rootItem = model.RootItem;
                //items.Add(rootItem);

                foreach (var model in models)
                    items.Add(model.RootItem);
                
                return items;
            });
        }

        public IList<BIMLayer> ExportToNWD(IList<BIMLayer> bimLayers)
        {
            Document oDoc = Application.ActiveDocument;
            DocumentModels models = oDoc.Models;

            List<ModelItem> visible = new List<ModelItem>(),
                hidden = new List<ModelItem>();
            ModelItemEnumerableCollection parents;

            string basePath = Path.Combine(Path.GetTempPath(), "HolooneNavis");
            IEnumerable<ModelItem> allModelItems = bimLayers.Select(x => x.ModelItem);
            
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            //Add all the items that are visible to the visible collection
            hidden = oDoc.Models.RootItems.SelectMany(x => x.DescendantsAndSelf).ToList();
                  
            foreach (var layer in bimLayers.Where(x => x.ModelItem != null)) // && !string.IsNullOrEmpty(x.Name)))
            {
                //Add all the items that are visible to the visible collection
                // hidden.AddRange(layer.ModelItem.Ancestors.Last().DescendantsAndSelf.ToList());

                // if not the root item is selected find selected layer
                if (oDoc.Models.RootItems.FirstOrDefault(x => x.DisplayName == layer.ModelItem.DisplayName) == null)
                {
                    if (!layer.ModelItem.IsLayer)
                    {
                        parents = layer.ModelItem.Ancestors;
                        foreach (var parent in parents)
                            visible.Add(parent);

                        foreach (var itm in layer.ModelItem.DescendantsAndSelf)
                            visible.Add(itm);
                    }
                    else
                    {
                        //Add all the items that are visible to the visible collection
                        // hidden.AddRange(layer.ModelItem.Ancestors.Last().DescendantsAndSelf.ToList());

                        visible = hidden.Where(x => x.InstanceHashCode == layer.ModelItem.InstanceHashCode).SelectMany(x => x.DescendantsAndSelf).ToList();
                    }

                    foreach (var itm in visible)
                        hidden.Remove(itm);

                    //Assign the ModelItemCollection to the selection
                    Application.ActiveDocument.CurrentSelection.CopyFrom(visible);

                    //hide the remaining items
                    Application.ActiveDocument.Models.SetHidden(hidden, true);
                }
                
                string fileName = getLayerName(layer);

                layer.FilePath = Path.Combine(basePath, Path.GetFileNameWithoutExtension(fileName) + ".nwd");

                //Save the Navisworks file
                // oDoc.SaveFile(layer.FilePath);
                oDoc.PublishFile(layer.FilePath, GetPublishProperties(fileName));

                oDoc.CurrentSelection.Clear();

                foreach (Model model in models)
                {
                    ModelItem rootItem = model.RootItem;
                    ModelItemEnumerableCollection modelItems = rootItem.DescendantsAndSelf;
                    oDoc.Models.SetHidden(modelItems, false);
                }
            }

            return bimLayers;
        }

        private string getLayerName(BIMLayer layer)
        {
            if (!string.IsNullOrEmpty(layer.ModelItem.DisplayName))
                return layer.ModelItem.DisplayName;
            else if (!string.IsNullOrEmpty(layer.ModelItem.Descendants.FirstOrDefault()?.DisplayName))
                return layer.ModelItem.Descendants.FirstOrDefault()?.DisplayName;
            else
                return layer.ModelItem.ClassDisplayName;
        }

        private PublishProperties GetPublishProperties(string title)
        {
            var returnProperties = new PublishProperties
            {
                AllowResave = true,
                Author = "Holo-one",
                Comments = "Navisworks export nwd format",
                Copyright = "Holo-one 2022",
                Keywords = "Navisworks, Export, NWD, Holo-one",
                PublishDate = DateTime.Now,
                Publisher = "Holo-one Navis Plugin",
                Title = title
            };

            return returnProperties;
        }

        public IList<BIMLayer> ExportToFBX(IList<BIMLayer> bimLayers)
        {
            PluginRecord FBXPluginrecord = Application.Plugins.FindPlugin("NativeExportPluginAdaptor_LcFbxExporterPlugin_Export.Navisworks");

            if (FBXPluginrecord != null)
            {
                if (!FBXPluginrecord.IsLoaded)
                {
                    FBXPluginrecord.LoadPlugin();
                }

                NativeExportPluginAdaptor FBXplugin = FBXPluginrecord.LoadedPlugin as NativeExportPluginAdaptor;

                //ModelItemCollection modelCollection = new ModelItemCollection();
                //List<string> filePaths = new List<string>();
                //string path = string.Empty;

                //var collection = bimLayers.Select(x => x.ModelItem);

                ExportFBX(bimLayers, FBXplugin);
            }

            return bimLayers;
        }

        private void ExportFBX(IList<BIMLayer> bimLayers, NativeExportPluginAdaptor FBXplugin)
        {
            Document oDoc = Application.ActiveDocument;
            DocumentModels models = oDoc.Models;

            IEnumerable<ModelItem> visible,
                hidden;
            string basePath = Path.Combine(Path.GetTempPath(), "HolooneNavis");
            IEnumerable<ModelItem> allModelItems = bimLayers.Select(x => x.ModelItem);

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            //var visible = from a in childItems.SelectMany(x => x.DescendantsAndSelf)
            //              join b in modelCollection on a.DisplayName equals b.DisplayName
            //              select a;
            //var hidden = childItems.SelectMany(x => x.DescendantsAndSelf).Except(modelCollection);


            //Add all the items that are visible to the visible collection
            var childItems = oDoc.Models.RootItems.SelectMany(x => x.Children);

            foreach (var layer in bimLayers.Where(x => x.ModelItem != null && !string.IsNullOrEmpty(x.Name)))
            {
                visible = childItems.Where(x => x.DisplayName == layer.ModelItem.DisplayName).SelectMany(x => x.DescendantsAndSelf);
                hidden = childItems.Where(x => x.DisplayName != layer.ModelItem.DisplayName).SelectMany(x => x.DescendantsAndSelf);

                //Assign the ModelItemCollection to the selection
                Application.ActiveDocument.CurrentSelection.CopyFrom(visible);

                //hide the remaining items
                Application.ActiveDocument.Models.SetHidden(hidden, true);

                layer.FilePath = Path.Combine(basePath, layer.Name + ".fbx");

                FBXplugin.Execute(layer.FilePath);

                oDoc.CurrentSelection.Clear();

                foreach (Model model in models)
                {
                    ModelItem rootItem = model.RootItem;
                    ModelItemEnumerableCollection modelItems = rootItem.DescendantsAndSelf;
                    oDoc.Models.SetHidden(modelItems, false);
                }

                // 
            }

        }


        /// <summary>
        /// OBSOLETE
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        public int CreateNewDocument(string fileName, string selected)
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

                Document nwDoc;
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".nwd");

                //delete all files.
                var rs = oDoc.Models.FirstOrDefault();

                foreach (Model oEachModel in oDoc.Models)
                {
                    var items = oEachModel.RootItem.Children;
                    foreach (var item in items)
                    {
                        if (item.DisplayName == selected)
                        {
                            // State.DeleteSelectedFiles failed to delete 
                            // all files at one time. 
                            // so have to delete them one by one.
                            ModelItemCollection oMC = new ModelItemCollection();
                            oMC.Add(item); // oEachModel.RootItem);
                            oDoc.CurrentSelection.CopyFrom(oMC);
                            try
                            {
                                Autodesk.Navisworks.Api.ComApi.ComApiBridge.State.DeleteSelectedFiles();
                            }
                            catch (Exception e)
                            {

                            }
                            break;
                        }
                    }
                }

                oDoc.PublishFile(path, new PublishProperties { Author = "Holoone" });

                //append them again with the new order
                oDoc.AppendFiles(fileArray);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// OBSOLETE
        /// </summary>
        /// <param name="input"></param>
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

        /// <summary>
        /// OBSOLETE
        /// </summary>
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

        /// <summary>
        /// OBSOLETE
        /// </summary>
        /// <returns></returns>
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
    }
}