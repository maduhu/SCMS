using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.WB;
using SCMS.CoreBusinessLogic._GoodsReceivedNote;
using SCMS.Model;
using SCMS.CoreBusinessLogic.CompletionCtificate;
using SCMS.CoreBusinessLogic._CountrySubOffice;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.Resource;
using SCMS.CoreBusinessLogic.StaffServices;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;
using Telerik.Web.Mvc.Infrastructure;
using System.Collections;
using System.IO;
using System.Text;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class CompletionCertificateController : PortalBaseController
    {
        private ICompletionCertificateService ccService;
        private IWayBillService wbService;
        private ICountrySubOfficeService subOfficeService;
        private readonly IStaffService staffService;
        private INotificationService notificationService;
        private static int TotalCCs;

        public CompletionCertificateController(IPermissionService permissionService, IUserContext userContext, IWayBillService wbService,
            IStaffService staffService, IGoodsReceivedNoteService gRNService, ICompletionCertificateService ccService, ICountrySubOfficeService subOfficeService,
            INotificationService notificationService)
            : base(userContext, permissionService)
        {
            this.wbService = wbService;
            this.staffService = staffService;
            this.ccService = ccService;
            this.subOfficeService = subOfficeService;
            this.notificationService = notificationService;
        }
        //
        // GET: /CompletionCertificate/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadCC()
        {
            var model = new CompletionCertificate()
            {
                StaffList = new SelectList(staffService.GetStaffByCountryProgramme(countryProg.Id), "StaffId", "StaffName"),
                Offices = new SelectList(subOfficeService.GetCountrySubOffices(countryProg.Id), "Id", "Name"),
                PurchaseOrders = new SelectList(ccService.GetGRNPurchaseOrders(), "Id", "RefNumber")
            };
            model.RefNumber = string.Format("--{0}--", Resources.Global_String_NewCC);
            model.PreparedOn = DateTime.Now;
            return View("LoadCC", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveCC(CompletionCertificate model)
        {
            model.CountryProgrammeId = countryProg.Id;
            model.PreparedBy = currentStaff.Id;
            model.PreparedOn = DateTime.Now;
            model.Id = Guid.NewGuid();
            ccService.SaveCompletionC(model);
            return ViewCCDetails(model.Id);
        }

        public ActionResult SubmitCC(Guid id)
        {
            Model.CompletionCertificate CC = ccService.GetCCById(id);
            CC.IsRejected = false;
            CC.IsSubmitted = true;
            CC.PreparedBy = currentStaff.Id;
            CC.PreparedOn = DateTime.Now;
            CC.RefNumber = ccService.GenerateUniquNumber(countryProg);
            ccService.UpdateCC(CC);
            notificationService.SendToAppropriateApprover(NotificationHelper.ccCode, NotificationHelper.approvalCode, CC.Id);
            return ViewCC();
        }

        public ActionResult ViewCC()
        {
            ViewData["total"] = TotalCCs;
            return View("ViewCC");
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _CCCustomBinding(GridCommand command)
        {
            var gridModel = CCData().AsQueryable().ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            IEnumerable data = gridModel.Data.AsQueryable().ToIList();
            return View(new GridModel { Data = data, Total = TotalCCs });
        }

        private string GetStatus(Model.CompletionCertificate model)
        {
            string status;
            if ((bool)model.IsRejected)
                status = Resources.Global_String_StatusRJ;
            else if (model.ApprovedBy != null)
                status = Resources.Global_String_StatusAP;
            else if (model.IsSubmitted)
                status = Resources.Global_String_StatusCR;
            else
                status = Resources.Global_String_StatusNEW;
            return status;
        }

        public ActionResult ExportCC(int page, int pageSize, string orderBy, string filter, string groupBy)
        {
            IEnumerable ccData = CCData().AsQueryable().ToGridModel(page, pageSize, orderBy, string.Empty, filter).Data;

            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);

            writer.WriteLine(new[] { Resources.Report_CompletionCertificate_CCNO, Resources.CompletionCerticate_ViewCC_PONo, Resources.Global_String_Office, Resources.Global_String_ProjectTitle,
            Resources.CompletionCertificate_ViewCC_Constructor,Resources.CompletionCertificate_ViewCC_ConfirmedBy,Resources.Global_String_Status}.StringJoin("\"", ","));

            foreach (Model.CCViewModel CC in ccData)
            {
                writer.WriteLine(new[] { CC.CCNo, CC.PONo, CC.Office, CC.ProjectTitle, CC.Contructor, CC.ConfirmedBy, CC.Status }.StringJoin("\"", ","));
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values", "CompletionCertificate.csv");
        }

        private IEnumerable<CCViewModel> CCData()
        {
            var gridModel = ccService.GetCCNotes()
                .Select(n => new Model.CCViewModel()
                {
                    Id = n.Id,
                    CCNo = n.RefNumber,
                    PONo = n.PurchaseOrder.RefNumber,
                    Office = n.CountrySubOffice.Name,
                    ProjectTitle = n.ProjectTitle,
                    Contructor = n.PurchaseOrder.Supplier.Name,
                    ConfirmedBy = n.Staff != null ? n.Staff.StaffName : string.Empty,
                    Status = GetStatus(n)
                });
            TotalCCs = gridModel.Count();
            return gridModel;
        }

        public ActionResult ViewCCDetails(Guid Id)
        {
            var model = ccService.GetCCById(Id);
            model.CanApprove = notificationService.CanApprove(currentUser, NotificationHelper.ccCode, NotificationHelper.approvalCode, model.Id)
                && model.IsSubmitted && !model.IsApproved
                && (bool)!model.IsRejected;
            return View("ViewCCDetails", model);
        }
    }
}
