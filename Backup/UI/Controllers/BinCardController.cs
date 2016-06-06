using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic._Item;
using SCMS.CoreBusinessLogic.BinCard;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.Resource;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;

namespace SCMS.UI.Controllers
{
    public class BinCardController : PortalBaseController
    {
        #region .private variables.
        private IBinCardService _binService;
        private IOrderRequest _orService;
        #endregion

        public BinCardController(IPermissionService permissionService, IUserContext userContext, IBinCardService _binService,
            IOrderRequest _orService)
            : base(userContext, permissionService)
        {
            this._binService = _binService;
            this._orService = _orService;
        }

        //
        // GET: /BinCard/

        public ActionResult Index()
        {
            SelectStockItem();
            return View();
        }

        public ActionResult LoadBin()
        {
            var model = new Model.Bin()
            {
                //                Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name"),
                Warehouses = new SelectList(new List<Model.WareHouse>(), "Id", "Name"),
                ItemPackages = new SelectList(new List<Model.ItemPackage>(), "Id", "Name"),
                RefNumber = string.Format("--{0}--", Resources.BinCard_LoadBin___NewBinCard___),
                PPlans = new SelectList(_binService.GetBinPPs(), "Id", "RefNumber"),
                PPlansItems = new SelectList(new List<Model.ProcurementPlanItem>(), "Id", "ItemName"),
                ORequests = new SelectList(_binService.GetBinORs(), "Id", "RefNumber"),
                ORItemsItems = new SelectList(new List<Model.OrderRequestItem>(), "Id", "ItemName"),
                RefLable = Resources.Global_String_OrderRequestNo,
                RefItemLable = Resources.OrderRequest_String_OrderRequestItems
            };
            return View("LoadBin", model);
        }

        public ActionResult LoadEditBC(Guid binId)
        {
            var model = _binService.GetAll().FirstOrDefault(p => p.Id == binId);
            model.Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name");
            model.ItemPackages = new SelectList(_binService.GetItemPackages((Guid)model.OrderRequestItemId), "Id", "Name");
            model.ORequests = new SelectList(SessionData.CurrentSession.OrderRequestList.Where(a => a.IsAuthorized == true && a.PurchaseOrders.Count() > 0), "Id", "RefNumber");
            model.BinType = model.OrderRequestItem == null ? "PP" : "OR";
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveBC(Model.Bin entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CountryProgrammeId = countryProg.Id;
            entity.PreparedBy = currentStaff.Id;
            entity.PreparedOn = DateTime.Now;
            entity.RefNumber = _binService.GenerateUniquBCNo();
            entity.QTY = 0;
            if (!_binService.IsBinAdded(entity))
            {
                LoadBin();
            }

            return ViewBinCard();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditBC(Model.Bin enitty)
        {
            _binService.IsBinEdited(enitty);
            return ManageBin(enitty.Id);
        }

        public ActionResult DeleteBC(Guid binId)
        {
            _binService.IsBinDeleted(binId);
            return ViewBinCard();
        }

        public ActionResult EditItemUpdate(Guid updateId)
        {
            return View();
        }

        public ActionResult SelectBin()
        {
            var model = new Model.BinItem()
            {
                BinCards = new SelectList(_binService.GetAll(), "Id", "BinNoAndItem")
            };
            return View(model);
        }

        public ActionResult ManageBin(Guid binId)
        {
            var model = _binService.GetAll().FirstOrDefault(p => p.Id == binId);
            model.CanEdit = true;// model.PreparedBy == currentStaff.Id ? true : false;
            model.BinItemz = _binService.GetAll().FirstOrDefault(p => p.Id == binId).BinItems.OrderByDescending(k => k.IssueDate).Select(p => new BinItems
            {
                Id = p.Id,
                RefNumber = p.GoodsReceivedNoteItem != null ? p.GoodsReceivedNoteItem.GoodsReceivedNote.RefNumber : p.WarehouseReleaseItem.WarehouseRelease.RefNumber,
                DnNumber = p.GoodsReceivedNoteItem != null ? p.GoodsReceivedNoteItem.GoodsReceivedNote.DnNumber : "",
                QTYReceived = p.QTYReceived,
                QTYIssued = p.QTYIssued,
                BalanceStock = p.BalanceStock,
                Remarks = p.Remarks,
                IssueDate = p.IssueDate
            }).ToList<Model.BinItems>();

            return View("ManageBin", model);
        }

        public ActionResult BinItemz(GridCommand command, Guid binId)
        {
            var gridModel = (_binService.GetAll().FirstOrDefault(p => p.Id == binId).BinItems.OrderByDescending(k => k.IssueDate).Select(p => new BinItems
            {
                Id = p.Id,
                RefNumber = p.GoodsReceivedNoteItem != null ? p.GoodsReceivedNoteItem.GoodsReceivedNote.RefNumber : p.WarehouseReleaseItem.WarehouseRelease.RefNumber,
                DnNumber = p.GoodsReceivedNoteItem != null ? p.GoodsReceivedNoteItem.GoodsReceivedNote.DnNumber : "",
                QTYReceived = p.QTYReceived,
                QTYIssued = p.QTYIssued,
                BalanceStock = p.BalanceStock,
                Remarks = p.Remarks,
                IssueDate = p.IssueDate
            }).AsQueryable())
                .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            var data = gridModel.Data.Cast<BinItems>().ToArray();

            gridModel.Data = data;


            return new JsonResult
            {
                Data = gridModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult LoadIntialAdd(Bin entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CountryProgrammeId = countryProg.Id;
            entity.PreparedBy = currentStaff.Id;
            entity.PreparedOn = DateTime.Now;
            entity.RefNumber = _binService.GenerateUniquBCNo();
            entity.QTY = 0;
            if (entity.BinType == "OR") entity.ProcurementPlanItemId = null; else entity.OrderRequestItemId = null;
            SCMS.UI.GeneralHelper.UserSession.CurrentSession.NewBin = entity;
            var model = new Model.BinItem()
            {
                GRNItems = new SelectList(_binService.GetGRNItemz4rmbin(entity), "Id", "GRNNoAndQtyDelivered"),
                BinId = entity.Id
            };
            return View("LoadAddBCItems", model);
        }

        public ActionResult LoadAddBCItems(Guid binId)
        {
            var model = new Model.BinItem()
            {
                GRNItems = new SelectList(_binService.GetGRNItemz4rmOR(binId), "Id", "GRNNoAndQtyDelivered"),
                BinId = binId
            };
            return View("LoadAddBCItems", model);
        }

        public ActionResult GetPrevQTYReceived(Guid grnItemId)
        {
            var QTYReceived = SessionData.CurrentSession.GoodsReceivedNoteItemList.FirstOrDefault(p => p.Id == grnItemId).BinItems.Sum(x => x.QTYReceived);
            return Json(QTYReceived, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPrevQTYIssued(Guid roItemId)
        {
            var QTYIssued = SessionData.CurrentSession.ReleaseOrderItemList.FirstOrDefault(p => p.Id == roItemId).BinItems.Sum(x => x.QTYIssued);
            return Json(QTYIssued, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsQTYInRange(Int64 QTYReceived, Int64? PreviouslyReceved, Guid? GoodsReceivedNoteItemId)
        {
            if (!PreviouslyReceved.HasValue) return Json(true, JsonRequestBehavior.AllowGet);
            var totalQty = SessionData.CurrentSession.GoodsReceivedNoteItemList.FirstOrDefault(p => p.Id == GoodsReceivedNoteItemId).QuantityDelivered;
            if (QTYReceived > (totalQty - PreviouslyReceved))
                return Json(Resources.Global_String_Quantityreceivedcannotexceedquantitydelivered, JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult IsQTYIssuedInRange(Int64 QTYIssued, Guid? WarehouseReleaseItemId, Int64? PreviouslyReleased)
        {
            if (!PreviouslyReleased.HasValue) return Json(true, JsonRequestBehavior.AllowGet);
            var totalQty = SessionData.CurrentSession.ReleaseOrderItemList.FirstOrDefault(k => k.Id == WarehouseReleaseItemId).Quantity;
            if (QTYIssued > (totalQty - PreviouslyReleased))
                return Json(Resources.Global_String_QuantityIssuedcannotexceedquantityreleased, JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadIssueBCItems(Guid binId)
        {
            var model = new Model.BinItem()
            {
                WHRItems = new SelectList(_binService.GetWRItemByBinId(binId), "Id", "RefNoAndItemQTY"),
                BinId = binId
            };

            return View(model);
        }

        public ActionResult UpdateBinCard(Model.BinItem entity)
        {
            entity.Id = Guid.NewGuid();
            entity.WHManagerId = currentStaff.Id;
            entity.IssueDate = DateTime.Now;
            entity.ExpirelyDate = DateTime.Now;
            _binService.IsBinItemAdded(entity, SCMS.UI.GeneralHelper.UserSession.CurrentSession.NewBin);
            SCMS.UI.GeneralHelper.UserSession.CurrentSession.NewBin = null;
            return ManageBin(entity.BinId);
        }

        public ActionResult ViewBinCard()
        {
            return View("ViewBinCard");
        }

        public ActionResult SelectStockItem()
        {
            var model = new Model.Bin()
            {
                ORItemsItems = new SelectList(new List<Model.Item>(), "Id", "Name"),
                Projects = new SelectList(_orService.GetProjectNos(), "Id", "ProjectNumber")
            };
            return View(model);
        }

        public ActionResult GetStockItemzByProject(Guid projectId)
        {
            StringBuilder blineOption = new StringBuilder();
            blineOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" name=\"ItemId\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            foreach (var item in _binService.GetAll().Where(p => p.ProjecDonor.Id == projectId).Select(j => j.Item).GroupBy(p => p.Id).Select(g => g.First()))
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.Name + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"ItemId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");
            return Content(blineOption.ToString(), "text/html");
        }

        public ActionResult ViewStockCard(Guid itemId, Guid ProjectId)
        {
            ViewBag.ItemId = itemId;
            ViewBag.ProjectDonorId = ProjectId;
            var proDonor = SessionData.CurrentSession.ProjectDonorList.FirstOrDefault(p => p.Id == ProjectId);
            ViewBag.ProjectName = proDonor.Project.Name + " (" + proDonor.ProjectNumber + ")";
            ViewBag.ItemName = SessionData.CurrentSession.ItemList.FirstOrDefault(p => p.Id == itemId).Name;
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GetBinCardList(GridCommand command)
        {
            var gridModel = (_binService.GetAll().Select(p => new BinCard
            {
                Id = p.Id,
                RefNumber = p.RefNumber,
                Description = p.Description,
                ItemName = p.Item.Name,
                QTY = (long)p.QTY,
                WareHouseName = p.WareHouse.Name,
                WHDemarcation = p.WHDemarcation
            }).AsQueryable())
                .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            var data = gridModel.Data.Cast<BinCard>().ToArray();
            gridModel.Data = data;

            return new JsonResult
            {
                Data = gridModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GetBinItemList(GridCommand command, Guid binId)
        {
            var gridModel = (_binService.GetAll().FirstOrDefault(p => p.Id == binId).BinItems.OrderBy(k => k.IssueDate).Select(p => new BinItems
            {
                Id = p.Id,
                RefNumber = p.GoodsReceivedNoteItem != null ? p.GoodsReceivedNoteItem.GoodsReceivedNote.RefNumber : p.WarehouseReleaseItem.WarehouseRelease.RefNumber,
                DnNumber = p.GoodsReceivedNoteItem != null ? p.GoodsReceivedNoteItem.GoodsReceivedNote.DnNumber : "",
                QTYReceived = p.QTYReceived,
                QTYIssued = p.QTYIssued,
                BalanceStock = p.BalanceStock,
                Remarks = p.Remarks,
                IssueDate = p.IssueDate
            }).AsQueryable()).ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            var data = gridModel.Data.Cast<Model.BinItems>().ToArray();
            gridModel.Data = data;

            return new JsonResult
            {
                Data = gridModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult LoadEditBCItem(Guid bcItemId)
        {
            var model = SessionData.CurrentSession.BinItemList.FirstOrDefault(p => p.Id == bcItemId);
           // model.GRNItems = new SelectList(_binService.GetGRNItemz4rmOR(model.BinId), "Id", "GRNNoAndQtyDelivered");
            return View(model);
        }

        public ActionResult ExportBinCardList()
        {
            return View();
        }

        public ActionResult GetORItems(Guid orId)
        {
            StringBuilder blineOption = new StringBuilder();
            //ORItems
            blineOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" name=\"OrderRequestItemId\" onchange=\"javascript:GetItemPackages(this);\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            foreach (Model.OrderRequestItem item in _orService.GetOrderRequestItems(orId))
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.ItemName + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"OrderRequestItemId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");

            blineOption.Append("~");//Append Warehouses

            blineOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"WareHouseId\" name=\"WareHouseId\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            foreach (Model.WareHouse item in _binService.GetWarehousesFromOR(orId))
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.Name + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"WareHouseId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");

            blineOption.Append("~");//Append ProjectDonor/Project No.

            var donor = SessionData.CurrentSession.OrderRequestList.FirstOrDefault(p => p.Id == orId);
            var don = donor.ProjectDonor.Donor.Name + " (" + donor.ProjectDonor.Project.ProjectNumber + ")";
            blineOption.Append(don);

            return Content(blineOption.ToString(), "text/html");
        }

        public ActionResult GetPPItems(Guid ppId)
        {
            StringBuilder blineOption = new StringBuilder();
            blineOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" name=\"ProcurementPlanItemId\" onchange=\"javascript:GetPPItemPackages(this);\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            foreach (Model.ProcurementPlanItem item in _binService.GetProcurementPlanItems(ppId))
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.ItemName + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"ProcurementPlanItemId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");

            blineOption.Append("~");//Append Warehouses

            blineOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"WareHouseId\" name=\"WareHouseId\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            foreach (Model.WareHouse item in _binService.GetWarehousesFromPP(ppId))
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.Name + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"WareHouseId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");

            blineOption.Append("~");//Append ProjectDonor/Project No.

            var donor = SessionData.CurrentSession.ProcurementPlanList.FirstOrDefault(p => p.Id == ppId);
            var don = donor.ProjectDonor.Donor.Name + " (" + donor.ProjectDonor.Project.ProjectNumber + ")";
            blineOption.Append(don);

            return Content(blineOption.ToString(), "text/html");
        }

        public ActionResult GetPPItemPackages(Guid ppItemId)
        {
            StringBuilder blineOption = new StringBuilder();
            blineOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"PackageId\" name=\"PackageId\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            foreach (Model.ItemPackage item in _binService.GetPPItemPackages(ppItemId))
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.Name + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"PackageId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");
            return Content(blineOption.ToString(), "text/html");
        }

        public ActionResult GetItemPackages(Guid orItemId)
        {
            StringBuilder blineOption = new StringBuilder();
            blineOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"PackageId\" name=\"PackageId\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            foreach (Model.ItemPackage item in _binService.GetItemPackages(orItemId))
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.Name + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"PackageId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");
            return Content(blineOption.ToString(), "text/html");
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GetItemStockSummary(GridCommand command, Guid itemId, Guid ProjectDonorId)
        {

            Int64 ReceivedStock, IssueStock, balStock;
            var gridModel = (_binService.GetAllBinItems().Where(p => p.Bin.Item.Id == itemId && p.Bin.ProjecDonor.Id == ProjectDonorId).GroupBy(j => j.IssueDate.Date).Select(g => g.First())
               .OrderBy(k => k.IssueDate).Select(p => new StockSummary
            {
                ItemId = p.Bin.Item.Id,
                ProjectDonorId = p.Bin.ProjecDonor.Id,
                IssueDate = p.IssueDate.ToShortDateString(),
                OpengStock = GetStockSummaryValues(ProjectDonorId, itemId, p.IssueDate, out ReceivedStock, out IssueStock, out balStock),
                ReceivedStock = ReceivedStock,
                IssueStock = IssueStock,
                BalStock = balStock,
                Id = Guid.NewGuid()
            }).AsQueryable())
                .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            var data = gridModel.Data.Cast<StockSummary>().ToArray();

            gridModel.Data = data;


            return new JsonResult
            {
                Data = gridModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        public Int64 GetStockSummaryValues(Guid projectDonorId, Guid itemId, DateTime currentDate, out Int64 ReceivedStock, out Int64 IssueStock, out Int64 balStock)
        {
            var lastReceiveDate = _binService.GetAllBinItems().Where(b => b.Bin.Item.Id == itemId && b.Bin.ProjecDonor.Id == projectDonorId && b.IssueDate.Date < currentDate.Date).OrderByDescending(b => b.IssueDate).Select(b => b.IssueDate).FirstOrDefault();

            Int64 qtyRecvdyesto = _binService.GetAllBinItems().Where(p => p.Bin.Item.Id == itemId && p.Bin.ProjecDonor.Id == projectDonorId && p.IssueDate.Date <= lastReceiveDate.Date).Sum(m => m.QTYReceived);
            Int64 qtyissuedyesto = _binService.GetAllBinItems().Where(p => p.Bin.Item.Id == itemId && p.Bin.ProjecDonor.Id == projectDonorId && p.IssueDate.Date <= lastReceiveDate.Date).Sum(m => m.QTYIssued);
            ReceivedStock = _binService.GetAllBinItems().Where(p => p.Bin.Item.Id == itemId && p.Bin.ProjecDonor.Id == projectDonorId && p.IssueDate.Date == currentDate.Date).Sum(m => m.QTYReceived);
            IssueStock = _binService.GetAllBinItems().Where(p => p.Bin.Item.Id == itemId && p.Bin.ProjecDonor.Id == projectDonorId && p.IssueDate.Date == currentDate.Date).Sum(m => m.QTYIssued);
            //Gets 2de's balance stock
            Int64 qtyRecvdnow = _binService.GetAllBinItems().Where(p => p.Bin.Item.Id == itemId && p.Bin.ProjecDonor.Id == projectDonorId && p.IssueDate.Date <= currentDate.Date).Sum(m => m.QTYReceived);
            Int64 qtyissuednow = _binService.GetAllBinItems().Where(p => p.Bin.Item.Id == itemId && p.Bin.ProjecDonor.Id == projectDonorId && p.IssueDate.Date <= currentDate.Date).Sum(m => m.QTYIssued);
            balStock = qtyRecvdnow - qtyissuednow;

            return qtyRecvdyesto - qtyissuedyesto;
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GetstockBinCardList(GridCommand command, string effectDate, Guid itemId, Guid projectDonorId)
        {

            DateTime effectDt = DateTime.Parse(effectDate);
            var gridModel = (_binService.GetAll().Where(k => k.Item.Id == itemId && k.ProjecDonor.Id == projectDonorId && k.BinItems.Where(w => w.IssueDate.Date == effectDt.Date).Count() > 0)
                .Select(p => new BinCard
            {
                Id = p.Id,
                RefNumber = p.RefNumber,
                Description = p.Description,
                ItemName = p.Item.Name,
                QTY = (long)p.QTY,
                WareHouseName = p.WareHouse.Name,
                WHDemarcation = p.WHDemarcation,

                IssueDate = effectDt.ToShortDateString(),
                ItemId = itemId,
                RecordId = Guid.NewGuid()
            }).AsQueryable())
                .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            var data = gridModel.Data.Cast<BinCard>().ToArray();
            gridModel.Data = data;

            return new JsonResult
            {
                Data = gridModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GetStockBinItemList(GridCommand command, Guid binId, string effectDate)
        {
            DateTime effectDt = DateTime.Parse(effectDate);
            var gridModel = (_binService.GetAll().FirstOrDefault(p => p.Id == binId).BinItems.Where(j => j.IssueDate.Date == effectDt.Date)
               .OrderBy(k => k.IssueDate).Select(p => new BinItems
                {
                    Id = p.Id,
                    RefNumber = p.GoodsReceivedNoteItem != null ? p.GoodsReceivedNoteItem.GoodsReceivedNote.RefNumber : p.WarehouseReleaseItem.WarehouseRelease.RefNumber,
                    DnNumber = p.GoodsReceivedNoteItem != null ? p.GoodsReceivedNoteItem.GoodsReceivedNote.DnNumber : "",
                    QTYReceived = p.QTYReceived,
                    QTYIssued = p.QTYIssued,
                    BalanceStock = p.BalanceStock,
                    Remarks = p.Remarks,
                    IssueDate = p.IssueDate
                }).AsQueryable()).ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);

            var data = gridModel.Data.Cast<Model.BinItems>().ToArray();
            gridModel.Data = data;

            return new JsonResult
            {
                Data = gridModel,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
