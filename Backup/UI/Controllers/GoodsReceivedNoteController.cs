using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._GoodsReceivedNote;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.PurchaseOrder;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.UI.GeneralHelper;
using System.Data.Objects.DataClasses;
using SCMS.CoreBusinessLogic.WB;
using System.Drawing;
using SCMS.CoreBusinessLogic.Settings;
using System.IO;
using SCMS.CoreBusinessLogic._Inventory;
using System.Text;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.CoreBusinessLogic.StaffServices;
using SCMS.Resource;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;
using System.Collections;
using System.Threading;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class GoodsReceivedNoteController : PortalBaseController
    {
        #region Declarations
        private IGoodsReceivedNoteService gRNService;
        private IOrderRequest ORService;
        private InventoryService invetService;
        private INotificationService notificationService;
        private readonly IStaffService staffService;//---
        private static int TotalAssets = 0;
        private static int TotalGRNs = 0;
        #endregion

        public GoodsReceivedNoteController(IPermissionService permissionService, IUserContext userContext, IGoodsReceivedNoteService _goodsReceivedNoteService,
             IOrderRequest ORService, InventoryService invetService, INotificationService notificationService, IStaffService staffService)
            : base(userContext, permissionService)
        {
            this.gRNService = _goodsReceivedNoteService;
            this.ORService = ORService;
            this.invetService = invetService;
            this.notificationService = notificationService;
            this.staffService = staffService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGRN()
        {
            //Create PDF from a single URL
            //var pdfUrl = SCMS.Reports.PdfGenerator.HtmlToPdf(pdfOutputLocation: "~/Log/",
            //    outputFilenamePrefix: "GeneratedPDF",
            //    urls: new string[] { "http://localhost:56819/GoodsReceivedNote/Index/LoadGRN/" });

            ////Create PDF from multiple URLs
            //var pdfUrl = PdfGenerator.HtmlToPdf(pdfOutputLocation: "~/PDFs/",
            //    outputFilenamePrefix: "GeneratedPDF",
            //    urls: new string[] { "http://www.google.co.uk", "http://news.bbc.co.uk" });

            return LGRN("LoadGRN");
        }

        private ActionResult LGRN(string viewname)
        {
            var model = new GoodsReceivedNote()
            {
                staffs = new SelectList(SessionData.CurrentSession.StaffList, "Id", "StaffName"),
                //ReceivedBy = currentStaff.Id,
                PurchaseOrders = new SelectList(gRNService.GetGRNPurchaseOrders(), "Id", "RefNumber"),
                SubOffices = new SelectList(SessionData.CurrentSession.CountrySubOfficeList, "Id", "Name"),
                Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name"),
                ApproversList = new SelectList(staffService.GetStaffByApprovalDoc(NotificationHelper.grnCode, countryProg.Id), "Id", "StaffName"),
                RefNumber = string.Format("--{0}--", Resources.GoodsReceivedNote_LoadGRN_NewGRN),
                POItemz = new List<POItemsView>()
            };
            return View(viewname, model);
        }

        //[HttpPost]
        //public JsonResult _GetDropDownListWarehouses(Guid? CountryOfficeId)
        //{
        //    List<Model.WareHouse> warehouses = SessionData.CurrentSession.WarehouseList.ToList();
        //    if (CountryOfficeId.HasValue)
        //    {
        //        warehouses = gRNService.GetSubOfficeWareHouses(CountryOfficeId.Value);
        //    }
        //    return Json(new SelectList(warehouses, "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public virtual JsonResult IsDNNoExist(string DnNumber, Guid Id)
        {
            bool exists = SessionData.CurrentSession.GoodsReceivedNoteList.Any(item => item.DnNumber == DnNumber && item.Id != Id);
            if (exists)
                return Json(Resources.GoodsReceivedNote_LoadEditGRN_DNNumberexists, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
            //http://edspencer.me.uk/2011/07/19/using-asp-net-mvc-remote-validation-the-real-world-additionalfields/
        }

        public ActionResult LoadGRNItemDetais(Guid PoId, string viewName)
        {
            var model = new GoodsReceivedNote()
            {
                PurchaseOrderId = PoId,
                ReceivedBy = currentStaff.Id,
                Supplier = gRNService.GetPO(PoId).Supplier,
                SupplierId = gRNService.GetPO(PoId).Supplier.Id,
                staffs = new SelectList(SessionData.CurrentSession.StaffList, "Id", "StaffName"),
                PurchaseOrders = new SelectList(gRNService.GetGRNPurchaseOrders(), "Id", "RefNumber"),
                SubOffices = new SelectList(SessionData.CurrentSession.CountrySubOfficeList, "Id", "Name"),
                Warehouses = new SelectList(gRNService.GetSubOfficeWareHouses(Guid.Empty), "Id", "Name"),
                ApproversList = new SelectList(staffService.GetStaffByApprovalDoc(NotificationHelper.grnCode, countryProg.Id), "Id", "StaffName"),
                RefNumber = string.Format("--{0}--", Resources.GoodsReceivedNote_LoadGRN_NewGRN),
                POItemz = gRNService.GetPOItemsDetails(PoId),
                SupplierName = gRNService.GetPO(PoId).Supplier.Name
            };
            return View(viewName, model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveGRN(Model.GoodsReceivedNote entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CountryProgrammeId = countryProg.Id;
            entity.PreparedBy = currentStaff.Id;
            entity.Verified = false;
            entity.PreparedOn = DateTime.Now;
            entity.IsSubmitted = false;
            //entity.RefNumber = gRNService.GenerateUniquNumber(countryProg);
            if (!gRNService.SaveGRN(entity))
            {
                ViewBag.Response = 0;
                ViewBag.msg = Resources.Global_String_AnErrorOccurred;
                entity.staffs = new SelectList(SessionData.CurrentSession.StaffList, "Id", "StaffName");
                entity.SupplierId = gRNService.GetPO(entity.PurchaseOrderId).Supplier.Id;
                entity.PurchaseOrders = new SelectList(gRNService.GetGRNPurchaseOrders(), "Id", "RefNumber");
                entity.SubOffices = new SelectList(SessionData.CurrentSession.CountrySubOfficeList, "Id", "Name");
                entity.POItemz = gRNService.GetPOItemsDetails(entity.PurchaseOrderId);
                entity.SupplierName = gRNService.GetPO(entity.PurchaseOrderId).Supplier.Name;
                return View("LoadGRN", entity);
            }
            //Send notification
            notificationService.SendToAppropriateApprover(NotificationHelper.grnCode, NotificationHelper.verificationCode, entity.Id);
            ViewBag.Response = 1;
            ViewBag.msg = Resources.Global_String_ProcessCompleted;

            return ViewGRNDetails(new GRNDetailsParams { GRNId = entity.Id, Verify = false });
        }

        public ActionResult LoadEditGRN(Guid GRNId)
        {
            var model = gRNService.GetGRNById(GRNId);
            model.staffs = new SelectList(SessionData.CurrentSession.StaffList, "Id", "StaffName");
            model.PurchaseOrders = new SelectList(gRNService.GetGRNPurchaseOrders(), "Id", "RefNumber");
            model.SubOffices = new SelectList(SessionData.CurrentSession.CountrySubOfficeList, "Id", "Name");
            model.Warehouses = new SelectList(gRNService.GetSubOfficeWareHouses(model.CountryOfficeId), "Id", "Name");
            model.ApproversList = new SelectList(staffService.GetStaffByApprovalDoc(NotificationHelper.grnCode, countryProg.Id), "Id", "StaffName");
            model.SupplierName = model.Supplier.Name;
            model.PONo = gRNService.GetPO(model.PurchaseOrderId).RefNumber;
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditGRN(Model.GoodsReceivedNote entity)
        {
            var grn = gRNService.GetGRNById(entity.Id);
            grn.DeliverDate = entity.DeliverDate;
            grn.DnNumber = entity.DnNumber;
            grn.ReceivedBy = entity.ReceivedBy;
            grn.CountryOfficeId = entity.CountryOfficeId;
            grn.WareHouseId = entity.WareHouseId;
            grn.DeliveredBy = entity.DeliveredBy;
            grn.DeliveryCompany = entity.DeliveryCompany;
            grn.VehicleRegNumber = entity.VehicleRegNumber;
            grn.DriverLicenseNumber = entity.DriverLicenseNumber;
            grn.ReceptionApprovedBy = entity.ReceptionApprovedBy;
            grn.Remarks = entity.Remarks;
            gRNService.UpdateGRN(grn);
            return GRNDetailz(entity.Id);
        }

        public ActionResult ViewGRNs()
        {
            IEnumerable data = GRNData(new GridCommand());
            ViewData["total"] = TotalGRNs;
            return View("ViewGRNs", data);
        }

        private string GRNStatus(Model.GoodsReceivedNote grn)
        {
            string orStatus;
            if (grn.IsRejected)
                orStatus = Resources.Global_String_StatusRJ;
            else if (grn.Verified)
                orStatus = Resources.Global_String_StatusAP;
            else
                orStatus = Resources.Global_String_StatusCR;

            return orStatus;
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _GetGRNCustomBinding(GridCommand command)
        {
            IEnumerable data = GRNData(command);
            return View(new GridModel { Data = data, Total = TotalGRNs });
        }

        private IEnumerable GRNData(GridCommand command)
        {
            var gridModel = (SessionData.CurrentSession.GoodsReceivedNoteList
                .Select(n => new Model.GRNModelView()
                {
                    Id = n.Id,
                    RefNo = n.RefNumber,
                    PONo = n.GoodsReceivedNoteItems.Count > 0 ? n.GoodsReceivedNoteItems.ToList()[0].PurchaseOrderItem.PurchaseOrder.RefNumber : string.Empty,
                    DeliveryDate = n.DeliverDate.Value.Date,
                    SupplierName = n.Supplier.Name,
                    Office = n.CountrySubOffice.Name,
                    Status = GRNStatus(n),
                    StatusDate = n.Verified || n.IsRejected ? n.ApprovedOn.Value.Date : n.PreparedOn.Value.Date

                }).AsQueryable())
                  .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);

            IEnumerable data = gridModel.Data.AsQueryable().ToIList();
            TotalGRNs = SessionData.CurrentSession.GoodsReceivedNoteList.Count();
            return data;
        }

        public ActionResult ExportCsv(int page, int pageSize, string orderBy, string filter, string groupBy)
        {
            var gridModel = SessionData.CurrentSession.GoodsReceivedNoteList
               .Select(n => new Model.GRNModelView()
               {
                   Id = n.Id,
                   RefNo = n.RefNumber,
                   PONo = n.GoodsReceivedNoteItems.Count > 0 ? n.GoodsReceivedNoteItems.ToList()[0].PurchaseOrderItem.PurchaseOrder.RefNumber : string.Empty,
                   DeliveryDate = n.DeliverDate.Value.Date,
                   SupplierName = n.Supplier.Name,
                   Office = n.CountrySubOffice.Name,
                   Status = GRNStatus(n),
                   StatusDate = n.Verified || n.IsRejected ? n.ApprovedOn.Value.Date : n.PreparedOn.Value.Date

               });
            IEnumerable orders = gridModel.AsQueryable().ToGridModel(page, pageSize, orderBy, string.Empty, filter).Data;

            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            writer.Write(Resources.Global_String_GRNNo + ",");
            writer.Write(Resources.CompletionCerticate_ViewCC_PONo + ",");
            writer.Write(Resources.Global_String_Supplier + ",");
            writer.Write(Resources.GoodsReceivedNote_LoadGRNVerifyItems_DestinationOffice + ",");
            writer.Write(Resources.GoodsReceivedNote_LoadGRNVerifyItems_DeliveryDate + ",");
            writer.Write(Resources.Global_String_Status + ",");
            writer.Write(Resources.Global_String_StatusDate);
            // writer.WriteLine();

            foreach (GRNModelView order in orders)
            {
                writer.Write(order.RefNo);
                writer.Write(",");
                writer.Write("\"");
                writer.Write(order.PONo);
                writer.Write("\"");
                writer.Write(",");
                writer.Write("\"");
                writer.Write(order.SupplierName);
                writer.Write("\"");
                writer.Write(",");
                writer.Write("\"");
                writer.Write(order.Office);
                writer.Write("\"");
                writer.Write(",");
                writer.Write("\"");
                writer.Write(order.DeliveryDate.ToShortDateString());
                writer.Write("\"");
                writer.Write(",");
                writer.Write("\"");
                writer.Write(order.Status);
                writer.Write("\"");
                writer.Write(",");
                writer.Write(order.StatusDate.ToShortDateString());
                writer.WriteLine();
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values", "GRN.csv");

            //return File(output.ToArray(), "application/vnd.ms-excel", "GridExcelExport.xls");
        }

        public ActionResult LoadEditGRNItem(Guid itemId)
        {
            return View("LoadEditGRNItem", SessionData.CurrentSession.GoodsReceivedNoteItemList.FirstOrDefault(p => p.Id == itemId));
        }

        public ActionResult EditGRNItem(Model.GoodsReceivedNoteItem model)
        {
            var grnItem = SessionData.CurrentSession.GoodsReceivedNoteItemList.FirstOrDefault(p => p.Id == model.Id);
            grnItem.Comments = model.Comments;
            grnItem.IsInventory = model.IsInventory;
            grnItem.QuantityDelivered = model.QuantityDelivered;
            grnItem.QuantityDamaged = model.QuantityDamaged;
            gRNService.UpdateGRNItem(grnItem);
            return GRNDetailz(model.GoodsReceivedNoteId);
        }

        public ActionResult DeleteGRNItem(Guid itemId, Guid GRNId)
        {
            gRNService.DeleteGRNItem(itemId);
            return GRNDetailz(GRNId);
        }

        public ActionResult DeleteGRN(Guid GRNId)
        {
            gRNService.DeleteGRNById(GRNId);
            return ViewGRNs();
        }

        public ActionResult VerifyGRN(Guid id)
        {
            Model.GoodsReceivedNote grn = SessionData.CurrentSession.GoodsReceivedNoteList.FirstOrDefault(p => p.Id == id);
            using (var db = new SCMSEntities())
            {
                grn.ReceivedByStaff = db.VStaffDetails.FirstOrDefault(p => p.StaffID == grn.ReceivedBy);
                grn.ItemColl = grn.GoodsReceivedNoteItems.ToList();
                return View(grn);
            }
        }

        public ActionResult ViewGRNDetails(GRNDetailsParams parammodel)
        {
            return GRNDetailz(parammodel.GRNId);
        }

        private ActionResult GRNDetailz(Guid GRNId)
        {
            Model.GoodsReceivedNote grn = SessionData.CurrentSession.GoodsReceivedNoteList.FirstOrDefault(p => p.Id == GRNId);
            using (var db = new SCMSEntities()) { grn.ReceivedByStaff = db.VStaffDetails.FirstOrDefault(p => p.StaffID == grn.ReceivedBy); }
            string actionType = null;
            if (grn.IsSubmitted && !grn.Verified && !grn.IsRejected) actionType = NotificationHelper.verificationCode;
            if (actionType != null) grn.CanApprove = notificationService.CanApprove(currentUser, NotificationHelper.grnCode, actionType, grn.Id);
            else grn.CanApprove = false;
            return View("ViewGRNDetails", grn);
        }

        public ActionResult LoadGRNVerifyItems()
        {
            return View(gRNService.GetGRNsForApproval(currentStaff));
        }

        [HttpPost]
        public ActionResult LoadGRAssetItems(GoodsReceivedNote model)
        {
            model.ReceptionApprovedBy = currentStaff.Id;
            model.CountryProgrammeId = countryProg.Id;
            if (gRNService.IsGRNVerified(model))
                SessionData.CurrentSession.GoodsReceivedNoteList = null;
            GoodsReceivedNote grn = gRNService.GetGRNById(model.Id);
            //send notification to preparer that GRN has been verified
            string notificationMsg = string.Format(NotificationHelper.grnApprovedMsgBody, grn.Staff1.Person.FirstName, grn.RefNumber);
            notificationService.SendNotification(grn.Staff1.Person.OfficialEmail, notificationMsg, NotificationHelper.grnsubject);
            //Notify RFP Prep Incharge
            notificationService.SendAuthorizedMsgToDocPreparers(NotificationHelper.grnCode, grn.Id);

            return ViewGRNs();
        }

        public ActionResult LoadRegsiterAsset(Guid id)
        {
            return RegisterAsset(id);
        }

        private ActionResult RegisterAsset(Guid id)
        {
            var grnItem = SessionData.CurrentSession.GoodsReceivedNoteItemList.FirstOrDefault(p => p.Id == id);
            Model.ProjectDonor pd = SessionData.CurrentSession.GoodsReceivedNoteItemList.FirstOrDefault(p => p.Id == id).PurchaseOrderItem.ProjectDonor;
            var model = new Asset()
            {
                Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name"),
                DepreciationCurrencyId = grnItem.PurchaseOrderItem.PurchaseOrder.Currency.Id,
                CurrentProjectDonorId = pd.Id,
                OpeningAccDepreciaiton = 0,
                AssetNumber = gRNService.GenerateAssetNo(SessionData.CurrentSession.GoodsReceivedNoteItemList.FirstOrDefault(p => p.Id == id)),
                PurchaseValue = SessionData.CurrentSession.GoodsReceivedNoteItemList.FirstOrDefault(p => p.Id == id).PurchaseOrderItem.UnitPrice,
                PercentageDepr = SessionData.CurrentSession.GoodsReceivedNoteItemList.FirstOrDefault(p => p.Id == id).PurchaseOrderItem.Item.ItemClassification.PercentageDepr
            };

            ViewBag.TotalItems = grnItem.QuantityDelivered;
            ViewBag.CurrentItem = SessionData.CurrentSession.AssetList.Count(p => p.GoodsReceivedNoteItemId == id) + 1;
            ViewBag.GRNItemId = id;
            ViewBag.Currency = grnItem.PurchaseOrderItem.PurchaseOrder.Currency.ShortName;
            ViewBag.ItemName = grnItem.PurchaseOrderItem.Item.Name;
            ViewBag.CurrentProject = pd.ProjectNumber;
            return View("LoadRegsiterAsset", model);
        }

        public ActionResult LoadOtherConsumables()
        {
            var model = new Inventory()
            {
                Items = new SelectList(ORService.GetItems("C"), "Id", "Name"),
                Inventories = invetService.GetInventoryList(categoryCode: "C"),
                Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name")
            };
            ViewBag.Action = "SaveOtherConsumables";
            return View("LoadOtherConsumables", model);
        }

        public ActionResult RemoveConsumableItem(Guid iventId)
        {
            using (var db = new SCMSEntities())
            {
                db.Inventories.Remove(db.Inventories.FirstOrDefault(p => p.Id == iventId));
                db.SaveChanges();
                SessionData.CurrentSession.InventoryList = null;
                return LoadOtherConsumables();
            }
        }

        public ActionResult LoadEditConsumables(Guid iventId)
        {
            Model.Inventory model = SessionData.CurrentSession.InventoryList.FirstOrDefault(p => p.Id == iventId);
            model.Items = new SelectList(ORService.GetItems("C"), "Id", "Name");
            model.Inventories = invetService.GetInventoryList(categoryCode: "C");
            model.Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name");
            ViewBag.Action = "EditInventory";
            return View("LoadOtherConsumables", model);
        }

        public ActionResult EditInventory(Model.Inventory model)
        {
            using (var context = new SCMSEntities())
            {

                context.Inventories.Attach(model);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                context.SaveChanges();
                ModelState.Clear();
                SessionData.CurrentSession.InventoryList = null;
                return LoadOtherConsumables();
            }
        }

        public ActionResult SaveOtherConsumables(Model.Inventory model)
        {
            model.CountryProgrammeId = countryProg.Id;
            if (!gRNService.IsInventoryUpdated(model))
            {
                ViewBag.Response = 0;
                ViewBag.Action = "SaveOtherConsumables";
                ViewBag.msg = Resources.Global_String_AnErrorOccurred;
                model.Items = new SelectList(ORService.GetItems(), "Id", "Name");
                return View("LoadOtherConsumables", model);
            }

            ViewBag.Response = 1;
            ViewBag.msg = Resources.Global_String_ProcessCompleted;
            ModelState.Clear();
            SessionData.CurrentSession.InventoryList = null;
            return LoadOtherConsumables();
        }

        public ActionResult LoadOtherAssets()
        {
            return LoadOtherAssetz();
        }

        [HttpPost]
        public ActionResult SaveOtherAsset(Asset AssetEntity)
        {
            AssetEntity.GoodsReceivedNoteItemId = Guid.Empty;
            AssetEntity.CountryProgramId = countryProg.Id;
            //check for uploaded image
            if (UserSession.CurrentSession.UploadedFile != null)
            {
                AssetEntity.Image = UserSession.CurrentSession.UploadedFile;
            }
            if (!gRNService.IsAssetRegistered(AssetEntity))
            {
                ViewBag.Response = 0;
                ViewBag.msg = Resources.Global_String_AnErrorOccurred;
                AssetEntity.Projects = new SelectList(ORService.GetProjectNos(), "Id", "ProjectNumber");
                AssetEntity.Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name");
                return View(AssetEntity);
            }
            //clear session image variable for further uploads
            UserSession.CurrentSession.UploadedFile = null;
            ViewBag.Response = 1;
            ViewBag.msg = Resources.Global_String_ProcessCompleted;
            ModelState.Clear();
            SessionData.CurrentSession.InventoryList = null;
            SessionData.CurrentSession.AssetList = null;
            return LoadOtherAssetz();
            //return RedirectToAction("LoadOtherAssets");
        }

        private ActionResult LoadOtherAssetz()
        {
            var model = new Asset()
            {
                Projects = new SelectList(ORService.GetProject(), "Id", "Name"),
                ProjectDonors = new SelectList(ORService.GetProjectNos(), "Id", "ProjectNumber"),
                Currencies = new SelectList(ORService.GetCurrencies(), "Id", "ShortName"),
                Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name"),
                Items = new SelectList(ORService.GetItems("A"), "Id", "Name"),
                DepreciationCurrencyId = countryProg.Country.CurrencyId.HasValue ? countryProg.Country.CurrencyId.Value : mbCurrency.Id,
                UseLifeSpan = true,
                OpeningAccDepreciaiton = 0,
                AcquisionDate = DateTime.Today
            };
            return View("LoadOtherAssets", model);
        }

        public ActionResult GetAssetNo(Guid ProjectDId)
        {
            ViewBag.Html = gRNService.GenerateAssetNo(ProjectDId);
            return View("HTMLResponses");
        }

        [HttpPost]
        public ActionResult RegsiterItem(Guid id, Model.Asset model)
        {
            model.GoodsReceivedNoteItemId = id;
            model.CountryProgramId = countryProg.Id;
            //check for uploaded image
            if (UserSession.CurrentSession.UploadedFile != null)
            {
                model.Image = UserSession.CurrentSession.UploadedFile;
            }
            var grnItm = SessionData.CurrentSession.GoodsReceivedNoteItemList.FirstOrDefault(p => p.Id == id);
            model.ItemId = grnItm.PurchaseOrderItem.Item.Id;
            model.PurchaseValue = grnItm.PurchaseOrderItem.UnitPrice;
            model.AcquisionDate = (DateTime)grnItm.GoodsReceivedNote.ApprovedOn;
            model.OriginalProjectId = model.CurrentProjectDonorId;
            gRNService.IsAssetRegistered(model);
            SessionData.CurrentSession.AssetList = null;
            int totalItems, currentItemcount;
            totalItems = (Int32)grnItm.QuantityDelivered;
            currentItemcount = SessionData.CurrentSession.AssetList.Count(p => p.GoodsReceivedNoteItemId == id);
            //clear session image variable to allow for more uploads
            UserSession.CurrentSession.UploadedFile = null;
            SessionData.CurrentSession.InventoryList = null;
            SessionData.CurrentSession.GoodsReceivedNoteItemList = null;
            if (currentItemcount == totalItems)
            {
                return ViewUnregisteredAssets();
            }
            else
            {
                ModelState.Clear();
                return RegisterAsset(id);
            }
        }

        public ActionResult ViewUnregisteredAssets()
        {
            return View("ViewUnregisteredAssets", gRNService.GetUnregisteredGRNItems());
        }

        [GridAction]
        public JsonResult AssetFiltering()
        {
            return Json(gRNService.GetUnregisteredGRNItems().Select(n => new
            {
                Id = n.Id,
                GRNNo = n.GRNNo,
                ItemName = n.ItemName,
                Unit = n.Unit,
                POQty = n.POQty,
                QuantityDelivered = n.QuantityDelivered,
                QuantityDamaged = n.QuantityDamaged,
                Comments = n.Comments
            }),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadTemplate()
        {
            return LGRN("DownloadTemplate");
        }

        public ActionResult ComputeQty(Guid packId, int? qty)
        {
            if (qty.HasValue)
            {
                Model.ItemPackage pack = SessionData.CurrentSession.ItemPackageList.FirstOrDefault(p => p.Id == packId);
                ViewBag.Html = (pack.PackSize * (qty ?? 0)).ToString();
            }
            else ViewBag.Html = 0.ToString();//"1";
            return View("HTMLResponses");
        }

        public ActionResult ComputePack(Guid packId, int? qty)
        {
            if (qty.HasValue)
            {
                Model.ItemPackage pack = SessionData.CurrentSession.ItemPackageList.FirstOrDefault(p => p.Id == packId);
                ViewBag.Html = (qty.Value / pack.PackSize).ToString();
            }
            else ViewBag.Html = 0.ToString();//"1";
            return View("HTMLResponses");
        }

        public ActionResult GetDepreciationPctge(Guid itemId)
        {
            ViewBag.Html = SessionData.CurrentSession.ItemList.FirstOrDefault(p => p.Id == itemId).ItemClassification.PercentageDepr.ToString();
            return View("HTMLResponses");
        }

        [HttpPost]
        public ActionResult HandleImageUpload(HttpPostedFileBase qqFile)
        {
            if (qqFile != null)
            {
                var fileContent = new byte[qqFile.ContentLength];
                qqFile.InputStream.Seek(0, SeekOrigin.Begin);
                qqFile.InputStream.Read(fileContent, 0, qqFile.ContentLength);
                UserSession.CurrentSession.UploadedFile = fileContent;
                UserSession.CurrentSession.UploadedDocDetails = new UploadedDoc() { FileContent = fileContent, FileName = qqFile.FileName, ContentLength = qqFile.ContentLength, ContentType = qqFile.ContentType };
                return Json(new { success = true }, "text/html");
            }
            else if (Request != null)
            {
                var fileContent = new byte[Request.ContentLength];
                Request.InputStream.Seek(0, SeekOrigin.Begin);
                Request.InputStream.Read(fileContent, 0, Request.ContentLength);
                UserSession.CurrentSession.UploadedFile = fileContent;
                UserSession.CurrentSession.UploadedDocDetails = new UploadedDoc() { FileContent = fileContent, ContentType = GetContentType(Request.QueryString.GetValues(0).LastOrDefault()), FileName = Request.QueryString.GetValues(0).LastOrDefault(), ContentLength = Request.ContentLength, };
                return Json(new { success = true }, "text/html");
            }
            else
            {
                UserSession.CurrentSession.UploadedFile = null;
                UserSession.CurrentSession.UploadedDocDetails = null;
                return Json(new { success = false });
            }
        }

        private string GetContentType(string fileName)
        {
            //http://codeasp.net/blogs/raghav_khunger/microsoft-net/531/how-to-get-content-type-of-a-file-in-c
            //http://www.prideparrot.com/blog/archive/2012/8/uploading_and_returning_files

            string contentType = "application/octetstream";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (registryKey != null && registryKey.GetValue("Content Type") != null)
                contentType = registryKey.GetValue("Content Type").ToString();
            return contentType;
        }

        public ActionResult SearchAssetDesc()
        {
            string searchTerm = Request.QueryString["q"];
            string searchResults = "";
            foreach (Asset asset in invetService.GetAssetList())
            {
                if (asset.Name.StartsWith(searchTerm, true, System.Globalization.CultureInfo.CurrentCulture))
                    searchResults += asset.Name + "\n";
            }
            searchResults = searchResults != "" ? searchResults : "\n";
            return Content(searchResults, "text/html");
        }

        public ActionResult GetWareHouses(Guid countrysubOfficeId)
        {
            StringBuilder blineOption = new StringBuilder();
            using (var db = new SCMSEntities())
            {
                blineOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"WareHouseId\" name=\"WareHouseId\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
                foreach (WareHouse item in gRNService.GetSubOfficeWareHouses(countrysubOfficeId))
                    blineOption.Append("<option value=\"" + item.Id + "\">" + item.Name + "</option>");
                blineOption.Append("</select>");
                blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"WareHouseId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");
            }
            ViewBag.Html = blineOption.ToString();
            return View("HTMLResponses");
        }

        public ActionResult SubmitGRN(Guid GRNId)
        {
            var grn = gRNService.GetGRNById(GRNId);
            grn.IsSubmitted = true;
            grn.Verified = false;
            grn.ApprovedOn = DateTime.Now;
            grn.PreparedBy = currentStaff.Id;
            grn.RefNumber = gRNService.GenerateUniquNumber(countryProg);
            gRNService.UpdateGRN(grn);
            return ViewGRNs();
        }

        public ActionResult RejectGRN(Guid id)
        {
            var grn = gRNService.GetGRNById(id);
            grn.IsRejected = true;
            grn.Verified = false;
            grn.ApprovedOn = DateTime.Now;
            grn.ReceptionApprovedBy = currentStaff.Id;
            gRNService.UpdateGRN(grn);
            return Content(Resources.Global_String_Done);
        }

        public ActionResult AssetManagement()
        {
            ViewData["total"] = TotalAssets;
            return View("AssetManagement");
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AssetInventCustomBinding(GridCommand command)
        {
            var gridModel = (invetService.GetAssetInventoryList()
                .Select(n => new Model.AssetView
                {
                    AssetId = n.Id,
                    AssetNumber = n.AssetNumber,
                    AssetName = n.Name,
                    OriginalProject = n.ProjectDonor1.ProjectNumber + " (" + n.ProjectDonor1.Donor.ShortName + ")",
                    AssignedTo = (bool)n.IsReleased ? n.Staff.Person.FirstName + " " + n.Staff.Person.OtherNames : Resources.Global_String_NotAssigned,
                    WarehouseName = n.WareHouse.Name,
                    currentProject = invetService.GetAssetCurrentProject(n)
                }).AsQueryable())
                  .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            TotalAssets = invetService.GetAssetInventoryList().Count();
            IEnumerable data = gridModel.Data.AsQueryable().ToIList();
            return View(new GridModel { Data = data, Total = TotalAssets });
        }

        public ActionResult LoadDesposeAsset(Guid assetId)
        {
            return View(invetService.GetAssetById(assetId));
        }

        public ActionResult LoadTransferAsset(Guid assetId)
        {
            var model = new AssetManagment()
            {
                ProjectDonors = new SelectList(ORService.GetProjectNos(), "Id", "ProjectNumber"),
                AssetId = assetId
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult DisposeAsset(Model.Asset model)
        {
            if (gRNService.IsAssetDisposed(model))
                SessionData.CurrentSession.AssetList = null;
            return AssetManagement();
        }

        [HttpPost]
        public ActionResult TransferAsset(Model.AssetManagment model)
        {
            model.Id = Guid.NewGuid();
            model.ActionTakenBy = currentStaff.Id;
            model.IssueDate = DateTime.Now;
            if (gRNService.IsAssetStateChanged(model))
                SessionData.CurrentSession.AssetList = null;
            return AssetManagement();

        }
    }

}
