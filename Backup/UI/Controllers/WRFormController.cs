using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.UI.Models;
using SCMS.Model;
using SCMS.CoreBusinessLogic.WRF;
using SCMS.UI.GeneralHelper;
using System.Collections;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using System.Text;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.CoreBusinessLogic.StaffServices;
using SCMS.CoreBusinessLogic.WB;
using SCMS.Resource;
using SCMS.CoreBusinessLogic.GeneralHelper;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;
using System.IO;
using SCMS.Utils;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class WRFormController : PortalBaseController
    {
        private IWareHouseReleaseService WRFservice;
        private INotificationService notificationService;
        private readonly IStaffService staffService;
        private static int TotalOR;

        public WRFormController(IPermissionService permissionService, IUserContext userContext, IWareHouseReleaseService WRFservice,
            INotificationService notificationService, IStaffService staffService)
            : base(userContext, permissionService)
        {
            this.WRFservice = WRFservice;
            this.notificationService = notificationService;
            this.staffService = staffService;
        }
        //
        // GET: /WRForm/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadWRF()
        {
            var model = new Model.WarehouseRelease()
             {
                 RefNumber = string.Format("--{0}--", Resources.Global_String_NewWRO),
                 RequestDate = DateTime.Today,
                 ReceivedBy = currentStaff.Id,
                 Staffs = new SelectList(SessionData.CurrentSession.StaffList, "Id", "StaffName"),
                 Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name"),
                 ApproversList = new SelectList(staffService.GetStaffByApprovalDoc(NotificationHelper.wrnCode, countryProg.Id), "Id", "StaffName"),
                 RequestorName = currentStaff.Person.FirstName + " " + currentStaff.Person.OtherNames
             };
            return View(model);
        }

        public ActionResult LoadWRFItem(WarehouseRelease model)
        {
            model.Id = Guid.NewGuid();
            model.NotReleased = true;
            model.CountryProgrammeId = countryProg.Id;
            model.PreparedOn = DateTime.Now;
            model.PreparedBy = currentStaff.Id;
            UserSession.CurrentSession.NewWRN = model;

            return ReleaseItem(model.Id);
        }

        public ActionResult ReleaseItem(Guid ROId)
        {
            WarehouseRelease rOrder = null;
            rOrder = WRFservice.GetWRNs().FirstOrDefault(k => k.Id == ROId);
            if (rOrder == null) rOrder = UserSession.CurrentSession.NewWRN;

            var modal = new WarehouseReleaseItem()
            {
                Items = new SelectList(WRFservice.GetInventoryItems((Guid)rOrder.WareHouseId), "Id", "ItemName"),
                WarehouseReleaseId = ROId
            };
            return View("LoadWRFItem", modal);
        }

        public ActionResult GetAssets(Guid id)
        {
            StringBuilder blineOption = new StringBuilder();
            if (SessionData.CurrentSession.InventoryList.FirstOrDefault(p => p.Id == id).Item.ItemCategory.CategoryCode == "C")
            {
                blineOption.Append("<input data-val=\"true\" data-val-number=\"" + Resources.Global_String_InvalidValue + "\" id=\"txtqtyordered\" name=\"Quantity\" type=\"text\" value=\"0\" />");
                blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"Quantity\" data-valmsg-replace=\"true\"></span>");
            }
            else
            {
                blineOption.Append("<select id=\"dplassetId\" name=\"AssetId\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
                foreach (Asset item in WRFservice.GetAssets(id))
                    blineOption.Append("<option value=\"" + item.Id + "\">" + item.Name + "</option>");
                blineOption.Append("</select>");
                blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"AssetId\" data-valmsg-replace=\"true\"></span>");

                blineOption.Append("<input type=\"hidden\" id=\"txtqtyordered\" value=\"0\" />");
            }
            ViewBag.Html = blineOption.ToString();
            return View("HTMLResponses");
        }

        public ActionResult SaveWRN(WarehouseReleaseItem entitymodel)
        {
            if (WRFservice.SaveWRF(UserSession.CurrentSession.NewWRN, entitymodel))
            {
                ViewBag.Response = 1;
                ViewBag.msg = Resources.WRFormController_String_ItemAdded;
                UserSession.CurrentSession.NewWRN = null;
                ModelState.Clear();
            }
            else { ViewBag.Response = 0; ViewBag.msg = Resources.Global_String_AnErrorOccurred; }
            return this.ViewNRNDetails(entitymodel.WarehouseReleaseId);
        }

        public ActionResult LoadEditRO(Guid ROId)
        {
            var model = WRFservice.GetWRNs().FirstOrDefault(p => p.Id == ROId);
            model.Staffs = new SelectList(SessionData.CurrentSession.StaffList, "Id", "StaffName");
            model.Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name");
            model.ApproversList = new SelectList(staffService.GetStaffByApprovalDoc(NotificationHelper.wrnCode, countryProg.Id), "Id", "StaffName");
            model.RequestorName = currentStaff.Person.FirstName + " " + currentStaff.Person.OtherNames;
            return View(model);
        }

        public ActionResult DeletRO(Guid ROId)
        {
            WRFservice.IsROrderDeleted(ROId);
            return viewWRN();
        }

        public ActionResult DeleteWRNItem(Guid wrnId, Guid wrnItmId)
        {
            WRFservice.IsWRNItemDeleted(wrnItmId);
            ViewBag.Response = 1;
            ViewBag.msg = Resources.WRFormController_String_ItemDeleted;
            return ViewNRNDetails(wrnId);
        }

        public ActionResult IsQtyenough(Model.CheckAvailableQty bc)
        {
            if (bc.assetid != null)
                if (SessionData.CurrentSession.AssetList.FirstOrDefault(p => p.Id == bc.assetid).IsReleased)
                {
                    ViewBag.Html = "0~ " + Resources.WRFormController_String_ItemAlreadyReleased; return View("HTMLResponses");
                }

            Model.Inventory entity = SessionData.CurrentSession.InventoryList.First(p => p.Id == bc.InventoryId);
            ViewBag.Html = bc.releasedQty <= entity.Quantity ? "1" : "0~ " + string.Format(Resources.WRFormController_String_ItemsLeftInStock, (Int64)entity.Quantity, entity.Item.Name);
            return View("HTMLResponses");
        }

        public ActionResult EditRO(Model.WarehouseRelease model)
        {
            var wrn = WRFservice.GetWRNs().FirstOrDefault(p => p.Id == model.Id);
            wrn.RequestDate = model.RequestDate;
            wrn.ReceivedBy = model.ReceivedBy;
            wrn.ApprovedBy = model.ApprovedBy;
            WRFservice.EditRO(wrn);
            return this.ViewNRNDetails(model.Id);
        }

        public ActionResult SubmitWRN(Guid id)
        {
            var wrn = WRFservice.GetWRNs().FirstOrDefault(p => p.Id == id);
            wrn.PreparedOn = DateTime.Now;
            wrn.IsApproved = false;
            wrn.IsSubmitted = true;
            wrn.PreparedBy = currentStaff.Id;
            wrn.RefNumber = WRFservice.GenerateUniquNumber(countryProg);
            WRFservice.SaveApproved(wrn);
            //Send notification
            notificationService.SendToAppropriateApprover(NotificationHelper.wrnCode, NotificationHelper.approvalCode, wrn.Id);
            return viewWRN();
        }

        public ActionResult viewWRN()
        {
            //IEnumerable data = GetORData(new GridCommand());
            ViewData["total"] = TotalOR;
            return View("viewWRN");
        }

        private IEnumerable GetORData(GridCommand command)
        {
            DateTime StatusDate;
            var gridModel = (WRFservice.GetWRNs()
                .Select(n => new ROModelView()
                {
                    Id = n.Id,
                    RefNumber = n.RefNumber,
                    FirstItem = n.WarehouseReleaseItems.Count() > 0 ? n.WarehouseReleaseItems.ToList()[0].Inventory.Item.Name : string.Empty,
                    Requestor = n.Staff1.StaffName,
                    Recepient = n.Staff2.StaffName,
                    Status = ORStatus(n, out StatusDate),
                    StatusDate = StatusDate
                }).AsQueryable())
                  .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            TotalOR = WRFservice.GetWRNs().Count();
            return gridModel.Data.AsQueryable().ToIList();
        }

        private string ORStatus(Model.WarehouseRelease or, out DateTime StatusDate)
        {
            string orStatus;

            if (or.IsApproved)
                orStatus = Resources.Global_String_StatusAP;
            else if (or.IsRejected)
                orStatus = Resources.Global_String_StatusRJ;
            else if (or.IsSubmitted)
                orStatus = Resources.Global_String_StatusCR;
            else
                orStatus = Resources.Global_String_StatusNEW;

            if (or.IsApproved)
                StatusDate = or.ApprovedOn.Value;
            else if (or.IsRejected)
                StatusDate = or.RejectedOn.Value;
            else
                StatusDate = or.RequestDate;

            return orStatus;
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _GetORCustomBinding(GridCommand command)
        {
            IEnumerable data = GetORData(command);
            return View(new GridModel
            {
                Data = data,
                Total = TotalOR
            });
        }

        public ActionResult ExportReleaseOrder(int page, int pageSize, string orderBy, string filter, string groupBy)
        {
            DateTime StatusDate;
            var gridModel = WRFservice.GetWRNs()
                .Select(n => new ROModelView()
                {
                    Id = n.Id,
                    RefNumber = n.RefNumber,
                    FirstItem = n.WarehouseReleaseItems.Count() > 0 ? n.WarehouseReleaseItems.ToList()[0].Inventory.Item.Name : string.Empty,
                    Requestor = n.Staff1.StaffName,
                    Recepient = n.Staff2.StaffName,
                    Status = ORStatus(n, out StatusDate),
                    StatusDate = StatusDate
                });
            IEnumerable ROs = gridModel.AsQueryable().ToGridModel(page, pageSize, orderBy, string.Empty, filter).Data;

            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            //Create data header row
            writer.WriteLine(new[] { Resources.WRForm_LoadWRF_WRFNo, Resources.Global_String_Item, Resources.WRForm_LoadWRF_Requestor, Resources.WRForm_LoadWRF_Recipient, Resources.Global_String_Status, Resources.Global_String_StatusDate }.StringJoin("\"", ","));

            // writer.WriteLine();//Creates an empty line
            //write data
            foreach (Model.ROModelView ro in ROs)
            {
                writer.WriteLine(new[] { ro.RefNumber, ro.FirstItem, ro.Requestor, ro.Recepient, ro.Status, ro.StatusDate.ToShortDateString() }.StringJoin("\"", ","));
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values", "ReleaseOrder.csv");

        }

        public ActionResult ViewNRNDetails(Guid id)
        {
            var model = WRFservice.GetWRNs().FirstOrDefault(p => p.Id == id);
            model.CanEdit = !model.IsSubmitted && model.PreparedBy == currentStaff.Id ? true : false;
            string actionType = null;
            if (model.IsSubmitted && !model.IsApproved && !model.IsRejected)
                actionType = NotificationHelper.approvalCode;
            if (actionType != null)
                model.CanApprove = notificationService.CanApprove(currentUser, NotificationHelper.wrnCode, actionType, model.Id);
            else
                model.CanApprove = false;

            return View("ViewNRNDetails", model);
        }
    }
}
