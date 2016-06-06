using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.StaffServices;
using SCMS.CoreBusinessLogic.Security;
using SCMS.Model;
using SCMS.UI.Areas.Admin.Models.Approver;
using SCMS.CoreBusinessLogic.Budgeting;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.UI.GeneralHelper;

namespace SCMS.UI.Areas.Admin.Controllers
{
    [MyException]
    public class ApproverController : PortalBaseController
    {

        private IStaffService staffService;
        private IBudgetService budgetService;

        public ApproverController(IPermissionService permissionService, IUserContext userContext, IStaffService staffService, IBudgetService budgetService)
            : base(userContext, permissionService)
        {
            this.staffService = staffService;
            this.budgetService = budgetService;
        }

        //
        // GET: /Admin/Approver/

        public ActionResult Index()
        {
            if (!permissionService.Authorize(StandardPermissionProvider.ApproversView))
            {
                return AccessDeniedView();
            }
            return View();
        }

        //
        // GET: /Admin/Approver/Details/5

        public ActionResult ApproverList()
        {
            if(UserSession.CurrentSession.ApproverList==null)
                UserSession.CurrentSession.ApproverList = staffService.GetCPGlobalApprovers(countryProg.Id);
            return View("ApproverList", UserSession.CurrentSession.ApproverList);
        }

        public ActionResult CCApprovers()
        {
            if (UserSession.CurrentSession.ApproverList == null)
                UserSession.CurrentSession.ApproverList = staffService.GetCPGlobalApprovers(countryProg.Id);
            return View("CCApprovers", UserSession.CurrentSession.ApproverList);
        }

        public ActionResult ParamsApprovers()
        {
            if (UserSession.CurrentSession.ApproverList == null)
                UserSession.CurrentSession.ApproverList = staffService.GetCPGlobalApprovers(countryProg.Id);
            return View("ParamsApprovers", UserSession.CurrentSession.ApproverList);
        }

        //
        // GET: /Admin/Approver/Create

        public ActionResult CreateApprover(string id)
        {
            ApproverModel approver = new ApproverModel();
            List<FinanceLimit> financeLimits = budgetService.GetFinanceLimits(countryProg.Id);
            Guid flId = financeLimits.Count > 0 ? financeLimits[0].Id : Guid.Empty;
            List<StaffView> staffList = (id != NotificationHelper.grnCode && id != NotificationHelper.wrnCode && id != NotificationHelper.ccCode && id != NotificationHelper.paramsCode) ? 
                staffService.GetStaffByFinanceLimit(flId, countryProg.Id) : staffService.GetStaffByCountryProgramme(countryProg.Id);            
            approver.ActivityCode = id;
            approver.ActionTypes = new SelectList(NotificationHelper.GetActionType(id), "Name", "Name");
            approver.FinancialLimits = new SelectList(financeLimits, "Id", "Name");
            approver.SystemUsers = new SelectList(staffList, "Id", "StaffName");
            approver.DocumentTypes = new SelectList(SCMS.CoreBusinessLogic.NotificationsManager.NotificationHelper.GetDocumentTypes(), "DocumentCode", "DocumentName");
            ViewBag.Unlimited = FinanceLimitUlimited(flId, financeLimits);

            return View(approver);
        }

        [HttpGet]
        public ActionResult GetStaffByFL(ApproverModel model)
        {
            ApproverModel approver = new ApproverModel();
            string viewName = "CreateApprover";
            if (!model.Id.Equals(Guid.Empty))
            {
                Approver entityApprover = staffService.GetApproverById(model.Id);
                approver = ApproverExtension.ToModel(entityApprover);
                viewName = "EditApprover";
            }
            List<FinanceLimit> financeLimits = budgetService.GetFinanceLimits(countryProg.Id);
            Guid flId = model.FinancialLimitId != null ? (Guid)model.FinancialLimitId : Guid.Empty;
            List<StaffView> staffList = (model.ActivityCode != NotificationHelper.grnCode && model.ActivityCode != NotificationHelper.wrnCode) ?
                staffService.GetStaffByFinanceLimit(flId, countryProg.Id) : staffService.GetStaffByCountryProgramme(countryProg.Id); 
            approver.FinancialLimitId = model.FinancialLimitId;
            approver.ActivityCode = model.ActivityCode;
            approver.ActionType = model.ActionType;
            approver.ActionTypes = new SelectList(SCMS.CoreBusinessLogic.NotificationsManager.NotificationHelper.GetActionType(model.ActivityCode), "Name", "Name");
            approver.FinancialLimits = new SelectList(financeLimits, "Id", "Name", model.FinancialLimitId);
            approver.SystemUsers = new SelectList(staffList, "Id", "StaffName");
            approver.DocumentTypes = new SelectList(SCMS.CoreBusinessLogic.NotificationsManager.NotificationHelper.GetDocumentTypes(), "DocumentCode", "DocumentName");
            ViewBag.Unlimited = FinanceLimitUlimited(flId, financeLimits);

            return View(viewName, approver);
        }

        /// <summary>
        /// Check if the selected finance limit is unlimited
        /// </summary>
        /// <param name="flId"></param>
        /// <param name="flList"></param>
        /// <returns></returns>
        private bool FinanceLimitUlimited(Guid flId, List<FinanceLimit> flList)
        {
            foreach (FinanceLimit fl in flList)
                if (fl.Id == flId)
                    return fl.Limit == 0;
            return false;
        }

        public ActionResult SaveApprover(ApproverModel approver)
        {
            try
            {
                //Set Priority to 1
                approver.Priority = 1;
                approver.Id = Guid.NewGuid();
                Approver entityApprover = ApproverExtension.ToEntity(approver);
                entityApprover.CountryProgrammeId = countryProg.Id;
                staffService.InsertApprover(entityApprover);
                UserSession.CurrentSession.ApproverList = staffService.GetCPGlobalApprovers(countryProg.Id);
                return ApproverList();
            }
            catch (Exception ex)
            {
                return Index();
            }
        }

        public ActionResult EditApprover(Guid Id)
        {
            Approver entityApprover = staffService.GetApproverById(Id);
            ApproverModel approver = ApproverExtension.ToModel(entityApprover);
            List<FinanceLimit> financeLimits = budgetService.GetFinanceLimits(countryProg.Id);
            List<StaffView> staffList = (approver.ActivityCode != NotificationHelper.grnCode && approver.ActivityCode != NotificationHelper.wrnCode
                && approver.ActivityCode != NotificationHelper.ppCode && approver.ActivityCode != NotificationHelper.ccCode && approver.ActivityCode != NotificationHelper.paramsCode) ?
            staffService.GetStaffByFinanceLimit((Guid)approver.FinancialLimitId, countryProg.Id) : staffService.GetStaffByCountryProgramme(countryProg.Id);
            approver.ActionTypes = new SelectList(NotificationHelper.GetActionType(entityApprover.ActivityCode), "Name", "Name", entityApprover.ActionType);
            approver.SystemUsers = new SelectList(staffList, "Id", "StaffName", entityApprover.UserId);
            approver.Assistants = new SelectList(staffList, "Id", "StaffName", entityApprover.AssistantId);
            approver.DocumentTypes = new SelectList(NotificationHelper.GetDocumentTypes(), "DocumentCode", "DocumentName", entityApprover.ActivityCode);
            approver.FinancialLimits = new SelectList(financeLimits, "Id", "Name", entityApprover.FinanceLimitId);
            if (approver.ActivityCode != NotificationHelper.grnCode && approver.ActivityCode != NotificationHelper.wrnCode
                && approver.ActivityCode != NotificationHelper.ppCode && approver.ActivityCode != NotificationHelper.ccCode && approver.ActivityCode != NotificationHelper.paramsCode)
                ViewBag.Unlimited = FinanceLimitUlimited((Guid)entityApprover.FinanceLimitId, financeLimits);
            return View(approver);
        }

        public ActionResult UpdateApprover(ApproverModel approver)
        {
            //Set Priority to 1
            approver.Priority = 1;
            Approver entityApprover = ApproverExtension.ToEntity(approver);
            entityApprover.CountryProgrammeId = countryProg.Id;
            staffService.UpdateApprover(entityApprover);
            UserSession.CurrentSession.ApproverList = staffService.GetCPGlobalApprovers(countryProg.Id);
            return ApproverList();
        }

        public ActionResult DeleteApprover(Guid Id)
        {
            Approver entityApprover = staffService.GetApproverById(Id);
            staffService.DeleteApprover(entityApprover);
            UserSession.CurrentSession.ApproverList = staffService.GetCPGlobalApprovers(countryProg.Id);
            return ApproverList();
        }

        //
        // POST: /Admin/Approver/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Admin/Approver/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Admin/Approver/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Admin/Approver/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Admin/Approver/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #region .Finance Limits.

        public ActionResult ViewFinanceLimits()
        {
            List<FinanceLimit> flList = budgetService.GetFinanceLimits(countryProg.Id);
            ViewBag.Currency = mbCurrency.ShortName;
            return View("ViewFinanceLimits", flList);
        }

        public ActionResult CreateFinanceLimit()
        {
            FinanceLimit model = new FinanceLimit();
            ViewBag.Currency = mbCurrency.ShortName;
            return View("FinanceLimitForm", model);
        }

        public ActionResult EditFinanceLimit(Guid Id)
        {
            FinanceLimit model = budgetService.GetFinanceLimitById(Id);
            ViewBag.Currency = mbCurrency.ShortName;
            return View("FinanceLimitForm", model);
        }

        public ActionResult SaveFinanceLimit(FinanceLimit model)
        {
            if (model.CountryProgrammeId.Equals(Guid.Empty))
                model.CountryProgrammeId = countryProg.Id;
            budgetService.SaveFinanceLimit(model);
            return ViewFinanceLimits();
        }

        public ActionResult DeleteFinanceLimit(Guid id)
        {
            budgetService.DeleteFinanceLimit(id);
            return ViewFinanceLimits();
        }

        #endregion
    }
}
