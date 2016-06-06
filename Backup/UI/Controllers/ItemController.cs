using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Settings;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._Item;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using System.Data.Objects.DataClasses;
using SCMS.UI.GeneralHelper;
using SCMS.Resource;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;

namespace SCMS.UI.Controllers
{
    public class ItemController : PortalBaseController
    {
        private IItemService itemService;

        public ItemController(IPermissionService permissionService, IUserContext userContext, IItemService _itemService)
            : base(userContext, permissionService)
        {
            this.itemService = _itemService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult CreateEdit(string iid)
        {
            UItem item = new UItem();

            item.ItemCatSelect =
                new SelectList(itemService.ItemCatObj.GetItemCategories(), "Id", "CategoryName");

            item.ItemClassSelect = new SelectList(itemService.ItemClassObj.GetItemClassifications(countryProg.Id), "Id", "Name");

            item.ItemUnitsSelect = new SelectList(itemService.ItemClassObj.GetUnitOfMessures(countryProg.Id), "Id", "Code");

            Guid iId;
            if (Guid.TryParse(iid, out iId))
            {
                ViewBag.Action = Resources.Global_String_Edit;
                item._item = itemService.GetItem(iId);
            }

            ViewBag.Inventory = false;
            if (Request["Inventory"] != null)
            {
                ViewBag.Inventory = true;
            }

            return View(item);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(UItem item, string Action = "")
        {
            if (ModelState.IsValid)
            {
                if (Action.Equals("Edit"))
                {
                    item._item.CountryProgrammeId = countryProg.Id;
                    if (itemService.EditItem(item._item))
                    {
                        ModelState.Clear();
                        item = new UItem();
                    }
                }
                else
                {
                    item._item.CountryProgrammeId = countryProg.Id;
                    if (itemService.AddItem(item._item))
                    {
                        ModelState.Clear();
                        item = new UItem();
                    }
                }
            }
            return RedirectToAction("ListView");
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            using (var dbContext = new SCMSEntities())
            {
                List<viewItems> itmz = new List<viewItems>();
                foreach (Model.Item item in dbContext.Items.Where(p => p.CountryProgrammeId == countryProg.Id).ToList())
                {
                    ItemCategory ic = item.ItemCategory;
                    ItemClassification ics = item.ItemClassification;
                    itmz.Add(new viewItems() { item = item, pack = item.ItemPackages.ToList() });
                }
                return View(itmz);
            }
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string iid)
        {
            Guid iId;
            if (Guid.TryParse(iid, out iId))
            {
                if (itemService.DeleteItem(iId))
                {
                    ModelState.Clear();
                }
            }
            return RedirectToAction("ListView");
        }

        public ActionResult DeletePack(Guid pkgId)
        {
            if (itemService.DeletePack(pkgId))
            {
                ModelState.Clear();
            }
            return RedirectToAction("ListView");
        }

        public ActionResult CreateEditItemPackage(Guid? pkgId)
        {
            using (var db = new SCMSEntities())
            {
                ItemPack model = new ItemPack();
                if (pkgId.IsNullOrEmpty())
                    model.EntityItemPackage = new ItemPackage();
                else { model.EntityItemPackage = db.ItemPackages.FirstOrDefault(p => p.Id == pkgId); ViewBag.Action = "Edit"; }
                model.Items = new SelectList(itemService.GetItems(countryProg.Id), "Id", "Name");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult CreateEditItemPackage(ItemPack model, string Action = "")
        {
            if (ModelState.IsValid)
            {
                if (Action.Equals("Edit"))
                {
                    if (itemService.EditPackage(model.EntityItemPackage))
                    {
                        ModelState.Clear();
                        model.EntityItemPackage = new ItemPackage();
                    }
                }
                else
                {
                    model.EntityItemPackage.Id = Guid.NewGuid();
                    if (itemService.AddPackage(model.EntityItemPackage))
                    {
                        ModelState.Clear();
                        model.EntityItemPackage = new ItemPackage();
                    }
                }
            }
            return RedirectToAction("ListView");
        }

        public ActionResult SearchItems()
        {
            string searchTerm = Request.QueryString["q"];
            if (UserSession.CurrentSession.ItemList == null)
                UserSession.CurrentSession.ItemList = itemService.GetItems(countryProg.Id);
            string searchResults = "";
            foreach (Item item in UserSession.CurrentSession.ItemList)
            {
                if (item.Name.StartsWith(searchTerm, true, System.Globalization.CultureInfo.CurrentCulture))
                    searchResults += item.Name + "\n";
            }
            searchResults = searchResults != "" ? searchResults : "\n";
            return Content(searchResults, "text/html");
        }

        private IQueryable<GridItem> GetGridItemsQuery(SCMSEntities context, Guid itemPackageId)
        {
            var countryProgramId = countryProg.Id;
            return      from itemPackage in context.ItemPackages
                        join item in context.Items on itemPackage.ItemId equals  item.Id
                        join itemCategory in context.ItemCategories on item.ItemCategoryId equals itemCategory.Id
                        join itemClassification in context.ItemClassifications on item.ItemClassificationId equals itemClassification.Id
                        where itemPackage.Id == itemPackageId && item.CountryProgrammeId == countryProgramId
                        select new GridItem()
                                   {
                                       Id = item.Id,
                                       Name = item.Name,
                                       ItemCategoryName = itemCategory.CategoryName,
                                       ItemClassificationName = itemClassification.Name,
                                       Description = item.Description
                                   };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GetItemsList(GridCommand command, Guid itemPackageId)
        {
            using (var context = SCMSEntities.Define())
            {
                var gridModel = GetGridItemsQuery(context,itemPackageId).ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
                var data = gridModel.Data.Cast<GridItem>().ToArray();
                gridModel.Data = data;

                return new JsonResult
                {
                    Data = gridModel,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        private IQueryable<GridItemPack> GetGridItemsPackQuery(SCMSEntities context)
        {
            var countryProgramId = countryProg.Id;
            return from itemPackages in context.ItemPackages
                   where itemPackages.Item.CountryProgrammeId == countryProgramId
                   select new GridItemPack()
                              {
                                  Id = itemPackages.Id,
                                  Name = itemPackages.Name,
                                  PackSize = itemPackages.PackSize,
                                  ShortName = itemPackages.shortName
                              };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GetItemPacksList(GridCommand command)
        {
            using (var context = SCMSEntities.Define())
            {
                var gridModel = GetGridItemsPackQuery(context).ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
                var data = gridModel.Data.Cast<GridItemPack>().ToArray();
                gridModel.Data = data;

                return new JsonResult
                {
                    Data = gridModel,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        public ActionResult ExportItemPacksList(int page, string orderBy, string filter, string type)
        {
            using (var context = SCMSEntities.Define())
            {
                var gridModel = GetGridItemsPackQuery(context).ToGridModel(page, SettingsHelper<CommonSettings>.Settings.GridPageSize, orderBy, string.Empty, filter);
                var data = gridModel.Data.Cast<GridItemPack>().ToArray();
                
                switch (type)
                {
                    case "pdf":
                        return ExportToPdf(data);
                    default:
                        return ExportToXls(data);
                }
            }
        }

        private ActionResult ExportToXls(IEnumerable<GridItemPack> models)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.WriteLine(new[]
                                        {
                                            Resources.Item_ListView_PackName,
                                            Resources.Item_CreateEditItemPackage_PackShortName,
                                            Resources.Item_CreateEditItemPackage_PackSize
                                        }.StringJoin("\"", ","));

            foreach (var model in models)
            {   
                streamWriter.Write('"'); streamWriter.Write(model.Name ?? ""); streamWriter.Write("\",");
                streamWriter.Write('"'); streamWriter.Write(model.ShortName ?? ""); streamWriter.Write("\",");
                streamWriter.WriteLine(model.PackSize);
            }

            streamWriter.Flush();

            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "export-{0}.csv".F(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")) };
        }


        private ActionResult ExportToPdf(IEnumerable<GridItemPack> models)
        {
            return Content("Not Implemented");
        }
    }
}
