using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._Inventory;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic._GoodsReceivedNote;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.CoreBusinessLogic.WB;
using SCMS.UI.GeneralHelper;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.CoreBusinessLogic.StaffServices;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;
using Telerik.Web.Mvc.Infrastructure;
using SCMS.Resource;
using System.IO;
using System.Text;
using SCMS.Utils;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class InventoryController : PortalBaseController
    {
        private InventoryService inventoryService;
        private readonly IStaffService staffService;
        private IWayBillService wbService;
        private static int TotalConsumables;
        private static int TotalGenInventory;
        private static int TotalAssets, TotalDisposedAssets;
        private static int TotalAssetPopUp;

        public InventoryController(IPermissionService permissionService, IUserContext userContext, InventoryService _inventoryService,
            IStaffService staffService, IWayBillService wbService)
            : base(userContext, permissionService)
        {
            this.inventoryService = _inventoryService;
            this.staffService = staffService;
            this.wbService = wbService;
        }

        //
        // GET: /Inventory/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewGeneralInventory()
        {
            var command = new GridCommand();
            var gridModel = (GenInventData().AsQueryable()).ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            IEnumerable data = gridModel.Data.AsQueryable().ToIList();
            ViewData["total"] = TotalGenInventory;
            return View(data);
        }

        private IEnumerable GetGenInventData(GridCommand command)
        {
            var gridModel = (inventoryService.GetInventoryList().GroupBy(p => p.ItemId).Select(g => g.First())
                .Select(n => new Model.Inventory()
                {
                    ItemId = n.ItemId,
                    ItName = n.Item.Name,
                    ItemClassification = n.Item.ItemClassification.Name,
                    Unit = n.Item.UnitOfMeasure.Code,
                    WarehouseName = n.WareHouse.Name,
                    CategoryName = n.Item.ItemCategory.CategoryName,
                    CategoryCode = n.Item.ItemCategory.CategoryCode,
                    ItemQTY = SessionData.CurrentSession.InventoryList.Where(p => p.ItemId == n.ItemId).Sum(p => p.Quantity)
                }).Where(i => i.ItemQTY > 0).AsQueryable())
                  .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            TotalGenInventory = inventoryService.GetInventoryList().GroupBy(p => p.ItemId).Select(g => g.First()).Count();
            return gridModel.Data.AsQueryable().ToIList();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _GenInventCustomBinding(GridCommand command)
        {
            var gridModel = GenInventData().AsQueryable().ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            IEnumerable data = gridModel.Data.AsQueryable().ToIList();
            return View(new GridModel { Data = data, Total = TotalGenInventory });
        }

        public ActionResult GeneralInventoryPopUp(Guid itemId)
        {
            //return View(inventoryService.GetAssetInventoryList(ItemId: itemId));

            var modle = inventoryService.GetAssetInventoryList(ItemId: itemId).Select(n => new Model.AssetView
            {
                AssetId = n.Id,
                AssetNumber = n.AssetNumber,
                AssetName = n.Name,
                WarehouseName = n.WareHouse.Name,
                currentProject = inventoryService.GetAssetCurrentProject(n),
                Classification = n.Item.ItemClassification.Name
            }).ToList();
            ViewData["total"] = TotalAssetPopUp = inventoryService.GetAssetInventoryList(ItemId: itemId).Count();
            return View(modle);
        }

        public ActionResult ExportGenInventory(int page, int pageSize, string orderBy, string filter, string groupBy)
        {
            IEnumerable genInventory = GenInventData().AsQueryable().ToGridModel(page, pageSize, orderBy, string.Empty, filter).Data;

            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            //Create data header row
            writer.WriteLine(new[] { Resources.Global_String_Classification, Resources.Global_String_ItemName, Resources.Global_String_Category, Resources.Global_String_Qty }.StringJoin("\"", ","));

            // writer.WriteLine();//Creates an empty line
            //write data
            foreach (Model.Inventory Gen in genInventory)
            {
                writer.WriteLine(new[] { Gen.ItemClassification, Gen.ItName, Gen.CategoryName, Gen.ItemQTY.ToString(Constants.NUMBER_FORMAT_NO_DECIMAL) }.StringJoin("\"", ","));
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values", "GeneralInventory.csv");

        }

        private IEnumerable<Inventory> GenInventData()
        {
            var gridModel = inventoryService.GetInventoryList().GroupBy(p => p.ItemId).Select(g => g.First())
                .Select(n => new Model.Inventory()
                {
                    ItemId = n.ItemId,
                    ItName = n.Item.Name,
                    ItemClassification = n.Item.ItemClassification.Name,
                    Unit = n.Item.UnitOfMeasure.Code,
                    WarehouseName = n.WareHouse.Name,
                    CategoryName = n.Item.ItemCategory.CategoryName,
                    CategoryCode = n.Item.ItemCategory.CategoryCode,
                    ItemQTY = SessionData.CurrentSession.InventoryList.Where(p => p.ItemId == n.ItemId).Sum(p => p.Quantity)
                }).AsQueryable().Where(k => k.ItemQTY > 0);
                
                
                //.Where(k => { k.CategoryCode == "A" ? k.ItemQTY > 0 : string.Empty; });


            TotalGenInventory = gridModel.Count();
            return gridModel;
        }

        [GridAction]
        public JsonResult AssetPopupFiltering(Guid itemId)
        {
            ViewData["total"] = TotalAssetPopUp;
            return Json(inventoryService.GetAssetInventoryList(ItemId: itemId).Select(n => new
            {
                AssetId = n.Id,
                AssetNumber = n.AssetNumber,
                AssetName = n.Name,
                WarehouseName = n.WareHouse.Name,
                currentProject = inventoryService.GetAssetCurrentProject(n),
                Classification = n.Item.ItemClassification.Name
            }),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewConsumableInventory()
        {
            IEnumerable data = GetData(new GridCommand());
            ViewData["total"] = TotalConsumables;
            return View(data);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _CustomBinding(GridCommand command)
        {
            IEnumerable data = GetData(command);
            return View(new GridModel
            {
                Data = data,
                Total = TotalConsumables
            });
        }

        private IEnumerable GetData(GridCommand command)
        {
            var gridModel = (inventoryService.GetInventoryList(categoryCode: "C").GroupBy(p => p.ItemId).Select(g => g.First())
                .Select(n => new Model.Inventory()
                {
                    ItName = n.Item.Name,
                    ItemClassification = n.Item.ItemClassification.Name,
                    Unit = n.Item.UnitOfMeasure.Code,
                    WarehouseName = n.WareHouse.Name,
                    ItemQTY = SessionData.CurrentSession.InventoryList.Where(p => p.ItemId == n.ItemId).Sum(p => p.Quantity)
                }).AsQueryable())
                  .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            TotalConsumables = inventoryService.GetInventoryList(categoryCode: "C").GroupBy(p => p.ItemId).Select(g => g.First()).Count();
            return gridModel.Data.AsQueryable().ToIList();
        }

        public ActionResult ConsumableInventoryPopUp(Guid itemId)
        {
            var modle = inventoryService.GetInventoryList(itemId: itemId).Select(n => new Inventory
            {
                ItemId = n.ItemId,
                ItName = n.Item.Name,
                ItemClassification = n.Item.ItemClassification.Name,
                Unit = n.Item.UnitOfMeasure.Code,
                WarehouseName = n.WareHouse.Name,
                ItemQTY = n.Quantity
            }).ToList();
            return View(modle);
        }

        public ActionResult ExportConsumables(int page, int pageSize, string orderBy, string filter, string groupBy)
        {
            var gridModel = inventoryService.GetInventoryList(categoryCode: "C").GroupBy(p => p.ItemId).Select(g => g.First())
                .Select(n => new Model.Inventory()
                {
                    ItName = n.Item.Name,
                    ItemClassification = n.Item.ItemClassification.Name,
                    Unit = n.Item.UnitOfMeasure.Code,
                    WarehouseName = n.WareHouse.Name,
                    ItemQTY = SessionData.CurrentSession.InventoryList.Where(p => p.ItemId == n.ItemId).Sum(p => p.Quantity)
                });
            IEnumerable consumables = gridModel.AsQueryable().ToGridModel(page, pageSize, orderBy, string.Empty, filter).Data;

            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            writer.WriteLine(new[] { Resources.Global_String_ItemName, Resources.Global_String_Classification, Resources.Global_String_Qty, Resources.Global_String_Unit, Resources.Global_String_Warehouse }.StringJoin("\"", ","));

            // writer.WriteLine();//Creates an empty line
            foreach (Model.Inventory C in consumables)
            {
                writer.WriteLine(new[] { C.ItName, C.ItemClassification, C.ItemQTY.ToString(Constants.NUMBER_FORMAT_NO_DECIMAL), C.Unit, C.WarehouseName }.StringJoin("\"", ","));
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values", "ConsumableInventory.csv");

        }

        [GridAction]
        public JsonResult Filtering(Guid itemId)
        {
            return Json(inventoryService.GetInventoryList(itemId: itemId).Select(n => new
            {
                ItemId = n.ItemId,
                ItName = n.Item.Name,
                ItemClassification = n.Item.ItemClassification.Name,
                Unit = n.Item.UnitOfMeasure.Code,
                WarehouseName = n.WareHouse.Name,
                ItemQTY = n.Quantity
            }),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewAssetInventory()
        {
            //return View(inventoryService.GetAssetInventoryList());

            IEnumerable data = GetAssetInventData(new GridCommand());
            ViewData["total"] = TotalAssets;
            return View(data);
        }

        private IEnumerable GetAssetInventData(GridCommand command)
        {
            var gridModel = (inventoryService.GetAssetInventoryList()
                .Select(n => new Model.AssetView
                {
                    AssetId = n.Id,
                    AssetNumber = n.AssetNumber,
                    AssetName = n.Name,
                    OriginalProject = n.ProjectDonor1.ProjectNumber + " (" + n.ProjectDonor1.Donor.ShortName + ")",
                    AssignedTo = (bool)n.IsReleased ? n.Staff.Person.FirstName + " " + n.Staff.Person.OtherNames : Resources.Global_String_NotAssigned,
                    WarehouseName = n.WareHouse.Name,
                    currentProject = inventoryService.GetAssetCurrentProject(n)
                }).AsQueryable())
                  .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            TotalAssets = inventoryService.GetAssetInventoryList().Count();
            return gridModel.Data.AsQueryable().ToIList();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AssetInventCustomBinding(GridCommand command)
        {
            IEnumerable data = GetAssetInventData(command);
            return View(new GridModel
            {
                Data = data,
                Total = TotalAssets
            });
        }

        public ActionResult ExportDisposedAssetsCsv(int page, int pageSize, string orderBy, string filter, string groupBy)
        {
            var gridModel = SessionData.CurrentSession.AssetList.Where(p => p.IsDesposed).OrderBy(a => a.Name)
                .Select(n => new Model.AssetView
                {
                    AssetId = n.Id,
                    AssetNumber = n.AssetNumber,
                    AssetName = n.Name,
                    WarehouseName = n.WareHouse.Name,
                    currentProject = inventoryService.GetAssetCurrentProject(n)
                });
            IEnumerable assets = gridModel.AsQueryable().ToGridModel(page, pageSize, orderBy, string.Empty, filter).Data;

            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            writer.WriteLine(new[] { Resources.Global_String_AssetNo, Resources.Global_String_AssetName, Resources.Global_String_CurrentProject, Resources.Global_String_Warehouse }.StringJoin("\"", ","));
            foreach (AssetView order in assets)
                writer.WriteLine(new[] { order.AssetNumber, order.AssetName, order.currentProject, order.WarehouseName }.StringJoin("\"", ","));

            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values", "DisposedAssets.csv");
        }

        public ActionResult ExportCsv(int page, int pageSize, string orderBy, string filter, string groupBy)
        {
            var gridModel = inventoryService.GetAssetInventoryList()
                .Select(n => new Model.AssetView
                {
                    AssetId = n.Id,
                    AssetNumber = n.AssetNumber,
                    AssetName = n.Name,
                    OriginalProject = n.ProjectDonor1.ProjectNumber + " (" + n.ProjectDonor1.Donor.ShortName + ")",
                    AssignedTo = (bool)n.IsReleased ? n.Staff.Person.FirstName + " " + n.Staff.Person.OtherNames : Resources.Global_String_NotAssigned,
                    WarehouseName = n.WareHouse.Name,
                    currentProject = inventoryService.GetAssetCurrentProject(n)
                });
            IEnumerable assets = gridModel.AsQueryable().ToGridModel(page, pageSize, orderBy, string.Empty, filter).Data;

            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            writer.WriteLine(new[]
                                        {
                                            Resources.Global_String_AssetNo,
                                            Resources.Global_String_AssetName,
                                            Resources.GoodsReceivedNote_LoadOtherAssets_OriginalProject,
                                            Resources.Global_String_CurrentProject,
                                            Resources.Global_String_Warehouse,
                                            Resources.Inventory_AssetInventoryPopup_AssignedTo
                                        }.StringJoin("\"", ","));

            // writer.WriteLine();//Creates an empty line

            foreach (AssetView order in assets)
            {
                writer.Write(order.AssetNumber);
                writer.Write(",\"");
                writer.Write(order.AssetName);
                writer.Write("\",\"");
                writer.Write(order.OriginalProject);
                writer.Write("\",\"");
                writer.Write(order.currentProject);
                writer.Write("\",\"");
                writer.Write(order.WarehouseName);
                writer.Write("\",");
                writer.Write(order.AssignedTo);
                writer.WriteLine();
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values", "AssetsInventory.csv");

        }

        public ActionResult AssetInventoryPopUp(Guid id)
        {
            ViewBag.CurrentProject = inventoryService.GetAssetCurrentProjetNoByAssetId(id);
            return View(inventoryService.GetAssetInventoryList(AssetId: id).FirstOrDefault());
        }

        public ActionResult ViewDisposedAssets()
        {
            ViewData["total"] = TotalDisposedAssets = 0;
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _DisposedAssetsCustomBinding(GridCommand command)
        {
            var gridModel = (SessionData.CurrentSession.AssetList.Where(p => p.IsDesposed).OrderBy(a => a.Name)
                .Select(n => new Model.AssetView
                {
                    AssetId = n.Id,
                    AssetNumber = n.AssetNumber,
                    AssetName = n.Name,
                    WarehouseName = n.WareHouse.Name,
                    currentProject = inventoryService.GetAssetCurrentProject(n)
                }).AsQueryable())
                  .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            TotalDisposedAssets = SessionData.CurrentSession.AssetList.Where(p => p.IsDesposed).OrderBy(a => a.Name).Count();
            var data = gridModel.Data.AsQueryable().ToIList();

            return View(new GridModel { Data = data, Total = TotalDisposedAssets });
        }

        [HttpGet]
        public ActionResult AssetImage(Guid id)
        {
            var asset = inventoryService.GetAssetById(id);
            if (asset == null || asset.Image == null)
            {
                return Content("");
            }
            return new FileContentResult(asset.Image, "image/jpeg");
        }

        public ActionResult LoadEditAsset(Guid assetId)
        {
            Model.Asset model = inventoryService.GetAssetInventoryList(AssetId: assetId).FirstOrDefault();
            model.Projects = new SelectList(SessionData.CurrentSession.ProjectDonorList, "Id", "ProjectNumber");
            model.Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name");
            model.Staffs = new SelectList(SessionData.CurrentSession.StaffList, "Id", "StaffName");
            model.SalvageValue = Math.Round(model.SalvageValue, 2);
            ViewBag.CurrentProject = inventoryService.GetAssetCurrentProjetNoByAssetId(assetId);
            return View(model);
        }

        public ActionResult EditAssetDetails(Model.Asset assetModel)
        {
            //check for uploaded image
            if (UserSession.CurrentSession.UploadedFile != null)
            {
                assetModel.Image = UserSession.CurrentSession.UploadedFile;
            }
            assetModel.IsReleased = assetModel.CurrentOwnerId.HasValue;
            if (inventoryService.IsAssetEdited(assetModel))
                SessionData.CurrentSession.AssetList = null;
            //clear uploaded image
            UserSession.CurrentSession.UploadedFile = null;
            return View("AssetInventoryPopUp", inventoryService.GetAssetInventoryList(AssetId: assetModel.Id).FirstOrDefault());
        }

        public ActionResult LoadAssetDepreciation(Guid? assetId = null)
        {
            Asset model;
            if (assetId == null)
                model = new Asset()
                {
                    Assets = new SelectList(inventoryService.GetAssetList(), "Id", "AssetNameAndNo"),
                    Depreciations = inventoryService.GetAnnualDepreciation(assetId: Guid.Empty)
                };
            else
            {
                model = SessionData.CurrentSession.AssetList.FirstOrDefault(p => p.Id == assetId);
                model.Assets = new SelectList(inventoryService.GetAssetList(), "Id", "AssetNameAndNo");
                model.Depreciations = model.Depreciations.Where(p => p.Period % 12 == 0).OrderBy(p => p.Period).ToObservableCollection();
            }
            ViewData["assetId"] = assetId == null ? Guid.Empty : model.Id;
            return View(model);
        }

        [GridAction]
        public JsonResult DeprecsumFiltering(Guid assetId)
        {
            var asset = SessionData.CurrentSession.AssetList.FirstOrDefault(p => p.Id == assetId);
            ICollection<Depreciation> jj = asset != null ? SessionData.CurrentSession.AssetList.FirstOrDefault(p => p.Id == assetId).Depreciations : new List<Depreciation>();
            var data = jj.Where(p => p.Period % 12 == 0).OrderBy(p => p.Period)
                .Select(n => new { Id = n.Id, Date = n.Date, AnnualDepreciation = n.AnnualDepreciation, AccDepreciation = n.AccDepreciation, NetbookValue = n.NetbookValue });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewDetailDepr(Guid id)
        {
            return View(inventoryService.GetDetailedDepreciation(id));
        }

    }
}
