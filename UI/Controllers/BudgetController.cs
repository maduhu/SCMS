using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic.Budgeting;
using SCMS.CoreBusinessLogic.Projects;
using SCMS.CoreBusinessLogic._ExchangeRate;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.UI.GeneralHelper;
using SCMS.CoreBusinessLogic.StaffServices;
using SCMS.UI.Areas.Admin.Models.Approver;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.CoreBusinessLogic._Currency;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.Resource;
using System.Text;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class BudgetController : PortalBaseController
    {
        private readonly IBudgetService budgetService;
        private readonly IProjectService projectService;
        private readonly IMasterBudgetService masterBudgetService;
        private readonly IExchangeRateService exchangeRateService;
        private readonly IStaffService staffService;
        private readonly ICurrencyService currencyService;
        private readonly IOrderRequest orderRequestService;

        private List<UI.Models.BudgetCategory> budgetCategories;
        private UI.Models.BudgetCategory budgetCategory;
        private List<Donor> _donors;
        private List<ProjectDonor> _projectDonors;
        private List<Currency> _currencies;

        public BudgetController(IPermissionService permissionService, IBudgetService budgetService, IProjectService projectService, IMasterBudgetService masterBudgetService,
            IExchangeRateService exchangeRateService, IStaffService staffService, IUserContext userContext, ICurrencyService currencyService,
            IOrderRequest orderRequestService)
            : base(userContext, permissionService)
        {
            this.budgetService = budgetService;
            this.masterBudgetService = masterBudgetService;
            this.projectService = projectService;
            this.exchangeRateService = exchangeRateService;
            this.staffService = staffService;
            this.currencyService = currencyService;
            this.orderRequestService = orderRequestService;
        }
        //
        // GET: /Budget/

        public ActionResult Index()
        {
            if (!permissionService.Authorize(StandardPermissionProvider.ProjectBudgetView))
            {
                return AccessDeniedView();
            }            
            _projectDonors = new List<ProjectDonor>();
            _projectDonors = projectService.GetCurrentProjectDonors(countryProg);
            return View(_projectDonors);
        }

        #region .Project Actions.

        public ActionResult CreateProject()
        {
            if (!permissionService.Authorize(StandardPermissionProvider.ProjectBudgetManage))
            {
                return AccessDeniedView();
            }

            Models.Project project = new Models.Project();
            project.EntityProject = new Project();
            _donors = projectService.GetDonors(countryProg.Id);
            _currencies = orderRequestService.GetCurrencies();
            project.Donors = new SelectList(_donors, "Id", "Name");
            project.Currencies = new SelectList(_currencies, "Id", "Name", countryProg.Country.CurrencyId);
            project.StaffList = new SelectList(staffService.GetStaffByCountryProgramme(countryProg.Id), "StaffId", "StaffName");
            project.StartDate = project.EndDate = DateTime.Now;
            ViewBag.FormHeader = Resources.BudgetController_String_NewProject;
            ViewBag.ButtonText = Resources.BudgetController_String_AddProject;
            ViewBag.EditMode = 0;
            return View(project);

        }

        [HttpPost]
        public ActionResult SaveProject(Models.Project project)
        {

            string pdId = "";
            project.EntityProject.ProjectNumber = project.ProjectNumber;
            project.EntityProject.CountryProgrammeId = countryProg.Id;
            ViewBag.ProjectNumber = project.ProjectNumber;
            if (project.Id == null || project.Id.Trim() == Guid.Empty.ToString().Trim())
            {
                projectService.CreateProject(project.EntityProject);
                projectService.CreateProjectDonor(project.EntityProject, project.StartDate, project.EndDate, project.ProjectManagerId, project.DonorId, project.CurrencyId, project.OverrunAdjustment, ref pdId);
                if (pdId == null)
                {
                    _donors = projectService.GetDonors(countryProg.Id);
                    _currencies = projectService.GetCurrencies(countryProg.Id);
                    project.Donors = new SelectList(_donors, "Id", "Name");
                    project.Currencies = new SelectList(_currencies, "Id", "Name");
                    ViewBag.FormHeader = Resources.BudgetController_String_NewProject;
                    ViewBag.ButtonText = Resources.BudgetController_String_AddProject;
                    ViewBag.EditMode = 0;
                    ViewBag.Status = 0;
                    return View("CreateProject", project);
                }
            }
            else
            {
                projectService.UpdateProject(project.EntityProject);
                projectService.UpdateProjectDonor(project.Id, project.EntityProject.ProjectNumber, project.StartDate, project.EndDate, project.ProjectManagerId, project.DonorId, project.CurrencyId, project.OverrunAdjustment, ref pdId);
            }
            if (pdId != null)
            {
                ViewBag.Status = 1;
                ViewBag.Id = pdId;
            }
            else
            {
                ViewBag.Status = 0;
                return EditProject(new Guid(project.Id));
            }
            return EditProject(new Guid(pdId));

        }

        public ActionResult ProjectList()
        {
            List<ProjectDonor> projects = projectService.GetCurrentProjectDonors(countryProg);
            ViewBag.HeaderText = Resources.BudgetController_String_CurrentProjects;
            return View("ProjectBudgetList", projects);
        }

        public ActionResult ProjectExpList()
        {
            List<ProjectDonor> projects = projectService.GetExpiredProjectDonors(countryProg);
            ViewBag.HeaderText = Resources.BudgetController_String_ExpiredProjects;
            return View("ProjectBudgetList", projects);
        }

        public ActionResult EditProject(Guid id)
        {
            ProjectDonor pd = projectService.GetProjectDonorById(id);
            Models.Project project = new Models.Project();
            project.Id = pd.Id.ToString();
            project.EntityProject = pd.Project;
            project.ProjectNumber = pd.ProjectNumber;
            project.CurrencyId = pd.CurrencyId.ToString();
            project.ProjectManagerId = pd.ProjectManagerId;
            project.DonorId = pd.DonorId.ToString();
            project.StartDate = (DateTime)pd.StartDate;
            project.EndDate = (DateTime)pd.EndDate;
            project.OverrunAdjustment = pd.OverrunAdjustment;
            _donors = projectService.GetDonors(countryProg.Id);
            _currencies = projectService.GetCurrencies(countryProg.Id);
            project.StaffList = new SelectList(staffService.GetStaffByCountryProgramme(countryProg.Id), "StaffId", "StaffName");
            project.Donors = new SelectList(_donors, "Id", "Name");
            project.Currencies = new SelectList(_currencies, "Id", "Name", pd.CurrencyId);
            ViewBag.FormHeader = "Project: " + pd.ProjectNumber;
            ViewBag.ButtonText = Resources.BudgetController_String_SaveProject;
            ViewBag.EditMode = 1;
            ViewBag.Id = id;
            List<BudgetCategory> bcList = budgetService.GetBudgetCategories(pd);
            if (bcList.Count > 0)
                ViewBag.HasBudget = 1;
            else
                ViewBag.HasBudget = 0;
            return View("CreateProject", project);

        }

        public ActionResult DeleteProject(string id)
        {

            projectService.DeleteProjectDonor(id);
            return ProjectList();

        }

        public ActionResult SearchProjects()
        {
            string searchTerm = Request.QueryString["q"];
            if (UserSession.CurrentSession.ProjectList == null)
                UserSession.CurrentSession.ProjectList = projectService.GetProjects(countryProg.Id);
            string searchResults = "";
            foreach(Project project in UserSession.CurrentSession.ProjectList)
            {
                if (project.Name.StartsWith(searchTerm, true, System.Globalization.CultureInfo.CurrentCulture))
                    searchResults += project.Name + "\n";
            }
            ViewBag.SearchResults = searchResults != "" ? searchResults : "\n";
            return View();
        }

        #endregion

        #region .Approval Settings.

        public ActionResult ApprovalSettings(Guid Id)
        {
            ProjectDonor pd = projectService.GetProjectDonorById(Id);
            List<Approver> approverList = staffService.GetProjectApprovers(Id);
            UserSession.CurrentSession.ProjectDonorId = Id;
            ViewBag.FormHeader = Resources.Global_String_Project + ": " + pd.ProjectNumber + " - " + Resources.Approver_Index_ApprovalSettings;
            ViewBag.Id = pd.Id;
            return View("ApprovalSettings", approverList);
        }

        public ActionResult CreateApprover(string id)
        {
            ApproverModel approver = new ApproverModel();
            if (UserSession.CurrentSession.ProjectDonorId != Guid.Empty)
                approver.ProjectDonorId = UserSession.CurrentSession.ProjectDonorId;
            List<FinanceLimit> financeLimits = budgetService.GetFinanceLimits(countryProg.Id);
            Guid flId = financeLimits.Count > 0 ? financeLimits[0].Id : Guid.Empty;
            List<StaffView> staffList = (id != NotificationHelper.grnCode && id != NotificationHelper.wrnCode && id != NotificationHelper.ppCode) ?
                staffService.GetStaffByFinanceLimit(flId, countryProg.Id) : staffService.GetStaffByCountryProgramme(countryProg.Id);
            approver.ActivityCode = id;
            approver.ActionTypes = new SelectList(NotificationHelper.GetActionType(id), "Name", "Name");
            approver.FinancialLimits = new SelectList(financeLimits, "Id", "Name");
            approver.SystemUsers = new SelectList(staffList, "Id", "StaffName");
            approver.DocumentTypes = new SelectList(SCMS.CoreBusinessLogic.NotificationsManager.NotificationHelper.GetDocumentTypes4Project(), "DocumentCode", "DocumentName");
            UserSession.CurrentSession.ProjectDonorId = (Guid)approver.ProjectDonorId;
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
            UserSession.CurrentSession.ProjectDonorId = (Guid)model.ProjectDonorId;
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
            approver.DocumentTypes = new SelectList(SCMS.CoreBusinessLogic.NotificationsManager.NotificationHelper.GetDocumentTypes4Project(), "DocumentCode", "DocumentName");
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
            //Set Priority to 1
            approver.Priority = 1;
            approver.Id = Guid.NewGuid();
            Approver entityApprover = ApproverExtension.ToEntity(approver);
            entityApprover.CountryProgrammeId = countryProg.Id;
            staffService.InsertApprover(entityApprover);
            return ApprovalSettings(entityApprover.ProjectDonorId.Value);
        }

        public ActionResult EditApprover(Guid Id)
        {
            Approver entityApprover = staffService.GetApproverById(Id);
            ApproverModel approver = ApproverExtension.ToModel(entityApprover);
            List<StaffView> staffList = (approver.ActivityCode != NotificationHelper.ppCode) ?
            staffService.GetStaffByFinanceLimit((Guid)approver.FinancialLimitId, countryProg.Id) : staffService.GetStaffByCountryProgramme(countryProg.Id);
            UserSession.CurrentSession.ProjectDonorId = (Guid)entityApprover.ProjectDonorId;
            List<FinanceLimit> financeLimits = budgetService.GetFinanceLimits(countryProg.Id);
            approver.ActionTypes = new SelectList(SCMS.CoreBusinessLogic.NotificationsManager.NotificationHelper.GetActionType(entityApprover.ActivityCode), "Name", "Name", entityApprover.ActionType);
            approver.SystemUsers = new SelectList(staffList, "Id", "StaffName", entityApprover.UserId);
            approver.Assistants = new SelectList(staffList, "Id", "StaffName", entityApprover.AssistantId);
            approver.DocumentTypes = new SelectList(SCMS.CoreBusinessLogic.NotificationsManager.NotificationHelper.GetDocumentTypes4Project(), "DocumentCode", "DocumentName", entityApprover.ActivityCode);
            approver.FinancialLimits = new SelectList(financeLimits, "Id", "Name", entityApprover.FinanceLimitId);
            if (approver.ActivityCode != NotificationHelper.ppCode)
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
            return ApprovalSettings(entityApprover.ProjectDonorId.Value);
        }

        public ActionResult DeleteApprover(Guid Id)
        {
            Approver entityApprover = staffService.GetApproverById(Id);
            Guid pdId = entityApprover.ProjectDonorId.Value;
            staffService.DeleteApprover(entityApprover);
            return ApprovalSettings(pdId);
        }

        #endregion

        #region .Doc Preparers.

        public ActionResult DocPrepList(Guid id)
        {
            List<DocumentPreparer> dpList = staffService.GetProjectDocPrepares(id);
            ViewBag.ProjectDonorId = id;
            return View("DocPrepList", dpList);
        }

        public ActionResult CreateDocPreparer(string id, Guid projId)
        {
            Models.DocPreparer model = new Models.DocPreparer();
            model.EntityDocPreparer = new DocumentPreparer();
            model.EntityDocPreparer.DocumentCode = id;
            model.EntityDocPreparer.ProjectDonorId = projId;
            List<StaffView> staffList = staffService.GetStaffByCountryProgramme(countryProg.Id);
            model.StaffList = new SelectList(staffList, "StaffId", "StaffName");
            return View(model);
        }

        public ActionResult SaveDocPreparer(Models.DocPreparer model)
        {
            staffService.InsertDocPreparer(model.EntityDocPreparer);
            return DocPrepList(model.EntityDocPreparer.ProjectDonorId);
        }

        public ActionResult DeleteDocPreper(Guid id, Guid projId)
        {
            staffService.DeleteDocPreparerById(id);
            return DocPrepList(projId);
        }

        #endregion

        #region .BudgetCategory Actions.

        public ActionResult CategoryList(Guid id)
        {

            ProjectDonor pd = projectService.GetProjectDonorById(id);
            Models.BudgetCategory modelBC;
            decimal totalBudget;
            List<BudgetCategory> bcList = budgetService.GetBudgetCategories(pd);
            budgetCategories = new List<Models.BudgetCategory>();
            foreach (BudgetCategory bc in bcList)
            {
                modelBC = new Models.BudgetCategory();
                modelBC.EntityBudgetCategory = bc;
                modelBC.BudgetCategoryId = bc.Id.ToString();
                modelBC.SubLines = PopulateCategorySubLines(modelBC.EntityBudgetCategory, out totalBudget);
                modelBC.TotalBudget = totalBudget;
                budgetCategories.Add(modelBC);
            }
            ViewBag.ProjectNo = pd.ProjectNumber;
            ViewBag.Donor = pd.Donor.Name;
            ViewBag.Currency = pd.Currency.ShortName;
            ViewBag.Id = pd.Id.ToString();
            return View("CategoryList", budgetCategories);

        }

        /// <summary>
        /// Used for form views with the SCMS.UI.Models.BudgetSubLine class
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        private List<Models.BudgetSubLine> PopulateCategorySubLines(BudgetCategory bc, out decimal totalBudget)
        {
            List<Models.BudgetSubLine> subLines = new List<Models.BudgetSubLine>();
            List<ProjectBudget> bsl = budgetService.GetBudgetLines(bc);
            Models.BudgetSubLine subLine;
            totalBudget = 0;
            foreach (ProjectBudget item in bsl)
            {
                subLine = new Models.BudgetSubLine();
                subLine.EntityBudgetSubLine = item;
                subLine.SubLineId = item.Id.ToString();
                subLine.BudgetCategoryId = bc.Id.ToString();
                subLine.BudgetCategoryNumber = bc.Number;
                subLine.TotalBudget = item.TotalBudget;
                subLines.Add(subLine);
                totalBudget += item.TotalBudget;
            }
            return subLines;
        }

        /// <summary>
        /// Used for display views with the SCMS.UI.Models.SubLine class
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        private List<Models.BudgetLine> GetCategorySubLines(BudgetCategory bc, ProjectDonor pd)
        {
            List<Models.BudgetLine> subLines = new List<Models.BudgetLine>();
            List<ProjectBudget> bsl = budgetService.GetBudgetLinesNotInBudget(pd, bc);
            Models.BudgetLine subLine;
            foreach (ProjectBudget item in bsl)
            {
                subLine = new Models.BudgetLine();
                subLine.EntityBudgetLine = item;
                subLine.SubLineId = item.Id;
                subLine.BudgetCategoryId = bc.Id.ToString();
                subLine.BudgetCategoryNumber = bc.Number;
                subLines.Add(subLine);
            }
            return subLines;
        }

        public ActionResult CreateCategory(Guid id)
        {

            ProjectDonor pd = projectService.GetProjectDonorById(id);
            budgetCategory = new Models.BudgetCategory();
            budgetCategory.EntityBudgetCategory = new BudgetCategory();
            budgetCategory.EntityBudgetCategory.ProjectDonorId = pd.Id;
            ViewBag.FormHeader = Resources.BudgetController_String_NewBudgetCategory;
            ViewBag.ButtonText = Resources.BudgetController_String_AddCategory;
            return View(budgetCategory);

        }

        [HttpPost]
        public ActionResult SaveCategory(Models.BudgetCategory bc)
        {

            ProjectDonor pd = projectService.GetProjectDonorById(bc.EntityBudgetCategory.ProjectDonorId.Value);
            BudgetCategory budgetCategory = budgetService.GetBudgetCategoryByNumber(bc.Number, pd.Id);
            if (bc.BudgetCategoryId == null || bc.BudgetCategoryId.Trim() == "")
            {
                if (budgetCategory == null)
                {
                    budgetService.CreateBudgetCategory(bc.EntityBudgetCategory, pd);
                }
                else
                {
                    ViewBag.ActionStatus = 0;
                    ViewBag.StatusMsg = string.Format(Resources.BudgetController_String_CategoryExists, bc.EntityBudgetCategory.Number);
                }

            }
            else
            {
                if (budgetCategory == null)
                {
                    budgetService.UpdateBudgetCategory(bc.EntityBudgetCategory);
                }
                else if (budgetCategory != null && budgetCategory.Id == bc.EntityBudgetCategory.Id)
                {
                    budgetService.UpdateBudgetCategory(bc.EntityBudgetCategory);
                }
                else
                {
                    ViewBag.ActionStatus = 0;
                    ViewBag.StatusMsg = string.Format(Resources.BudgetController_String_CategoryExists, bc.EntityBudgetCategory.Number);
                }
            }
            return CategoryList(pd.Id);

        }

        public ActionResult EditCategory(string id)
        {

            BudgetCategory bc = budgetService.GetBudgetCategoryById(id);
            Models.BudgetCategory budgetCategory = new Models.BudgetCategory();
            decimal totalBudget;
            budgetCategory.EntityBudgetCategory = bc;
            budgetCategory.BudgetCategoryId = bc.Id.ToString();
            budgetCategory.SubLines = PopulateCategorySubLines(budgetCategory.EntityBudgetCategory, out totalBudget);
            ViewBag.FormHeader = bc.Name;
            ViewBag.ButtonText = Resources.BudgetController_String_SaveCategory;
            return View("CreateCategory", budgetCategory);

        }

        public ActionResult DeleteCategory(string id)
        {

            BudgetCategory bc = budgetService.GetBudgetCategoryById(id);
            budgetService.DeleteBudgetCategory(id);
            return CategoryList(bc.ProjectDonorId.Value);

        }

        #endregion

        #region .SubLine Actions.

        public ActionResult CreateSubLine(string id)
        {
            Models.BudgetSubLine bsl = new Models.BudgetSubLine();
            bsl.EntityBudgetSubLine = new ProjectBudget();
            Models.BudgetCategory bc = new Models.BudgetCategory();
            bc.EntityBudgetCategory = budgetService.GetBudgetCategoryById(id);
            bsl.BudgetCategoryId = bc.EntityBudgetCategory.Id.ToString();
            bsl.BudgetCategoryNumber = bc.EntityBudgetCategory.Number;
            bsl.BudgetCategoryDesc = bc.EntityBudgetCategory.Description;
            bsl.EntityBudgetSubLine.OverrunAdjustment = bc.EntityBudgetCategory.ProjectDonor.OverrunAdjustment;
            ViewBag.FormHeader = Resources.BudgetController_String_NewBudgetLine;
            ViewBag.ButtonText = Resources.BudgetController_String_AddBudgetLine;
            return View(bsl);
        }

        [HttpPost]
        public ActionResult SaveSubLine(Models.BudgetSubLine bsl)
        {

            ProjectBudget subLine = budgetService.GetBudgetLineByNumber(bsl.LineNumber, bsl.BudgetCategoryId);
            bsl.EntityBudgetSubLine.BudgetCategoryId = new Guid(bsl.BudgetCategoryId);
            bsl.EntityBudgetSubLine.BudgetCategory = budgetService.GetBudgetCategoryById(bsl.BudgetCategoryId);
            if (bsl.SubLineId == null || bsl.SubLineId.Trim() == "")
            {
                if (subLine == null)
                {
                    bsl.EntityBudgetSubLine.TotalBudget = bsl.TotalBudget;
                    budgetService.CreateBudgetLine(bsl.EntityBudgetSubLine);
                }
                else
                {
                    ViewBag.ActionStatus = 0;
                    ViewBag.StatusMsg = string.Format(Resources.BudgetController_String_BudgetLineExists, bsl.EntityBudgetSubLine.LineNumber);
                }
            }
            else
            {
                if (subLine == null || (subLine != null && subLine.Id == bsl.EntityBudgetSubLine.Id))
                {
                    bsl.EntityBudgetSubLine.TotalBudget = bsl.TotalBudget;
                    budgetService.UpdateBudgetLine(bsl.EntityBudgetSubLine);
                }
                else
                {
                    ViewBag.ActionStatus = 0;
                    ViewBag.StatusMsg = string.Format(Resources.BudgetController_String_BudgetLineExists, bsl.EntityBudgetSubLine.LineNumber);
                }
            }
            return CategoryList(bsl.EntityBudgetSubLine.BudgetCategory.ProjectDonorId.Value);


        }

        public ActionResult EditSubLine(Guid id)
        {

            ProjectBudget bsl = budgetService.GetBudgetLineById(id);
            Models.BudgetSubLine subLine = new Models.BudgetSubLine();
            subLine.EntityBudgetSubLine = bsl;
            subLine.SubLineId = bsl.Id.ToString();
            subLine.BudgetCategoryId = bsl.BudgetCategoryId.ToString();
            subLine.BudgetCategoryNumber = bsl.BudgetCategory.Number;
            subLine.TotalBudget = bsl.TotalBudget;
            ViewBag.FormHeader = bsl.Description;
            ViewBag.ButtonText = Resources.BudgetController_String_SaveBudgetLine;
            return View("CreateSubLine", subLine);

        }

        public ActionResult DeleteSubLine(Guid id)
        {

            ProjectBudget bsl = budgetService.GetBudgetLineById(id);
            budgetService.DeleteBudgetLine(id);
            return CategoryList(bsl.BudgetCategory.ProjectDonorId.Value);

        }

        #endregion

        #region .Budget.

        public ActionResult ViewBudget(Guid id)
        {
            ProjectDonor pd = projectService.GetProjectDonorById(id);
            List<Models.Category> model = Models.BudgetExtension.PrepareBudgetModel(budgetService, exchangeRateService, currencyService, countryProg, pd, pd.Currency);
            return RenderBudgetView(model, pd);
        }

        public ActionResult ViewBudgetInCurr(ProjectDonor pdModel)
        {
            ProjectDonor pd = projectService.GetProjectDonorById(pdModel.Id);
            Currency displayCurrency = currencyService.GetCurrency((Guid)pdModel.CurrencyId);
            if (displayCurrency == null)
                displayCurrency = pd.Currency;
            List<Models.Category> model = Models.BudgetExtension.PrepareBudgetModel(budgetService, exchangeRateService, currencyService, countryProg, pd, displayCurrency);
            return RenderBudgetView(model, pd);
        }

        private ActionResult RenderBudgetView(List<Models.Category> categories, ProjectDonor pd)
        {
            
            //If budget has no sublines, then we can't render the edit budget page. 
            if (categories.Count == 0)
                return CategoryList(pd.Id);
            ViewBag.ProjectId = pd.Id.ToString();
            ViewBag.ProjectNo = pd.ProjectNumber;
            ViewBag.StartDate = pd.StartDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = pd.EndDate.ToString("dd/MM/yyyy");
            ViewBag.Currency = pd.Currency.ShortName;
            ViewBag.Donor = pd.Donor.Name;
            ViewBag.DonorId = pd.Id;
            ViewBag.ProjectName = pd.Project.Name;
            ViewBag.ProjectManager = pd.Staff.Person.FirstName + " " + pd.Staff.Person.OtherNames;
            ViewBag.AllowedOverrun = pd.OverrunAdjustment.HasValue ? pd.OverrunAdjustment.Value.ToString("#,##0.00") + "%" : "NOT SET";
            return View("ViewBudget", categories);
        }

        /// <summary>
        /// pass budget line id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CommitDetails(Guid id)
        {
            Models.CommitDetailModel model = new Models.CommitDetailModel();
            model.BudgetCommits = budgetService.GetBudgetCommitmentsByLineId(id);
            //Set Booleans
            foreach (var item in model.BudgetCommits)
            {
                if (item.RFPBudgetLineId.HasValue || item.PurchaseOrderItemId.HasValue || item.OrderRequestItemId.HasValue)
                {
                    model.HasProcurement = true;
                    break;
                }
            }
            return View("CommitDetails", model);
        }

        public ActionResult PostDetails(Guid id)
        {
            Models.PostDetailModel model = new Models.PostDetailModel();
            model.BudgetPostings = budgetService.GetBudgetPostingsByLineId(id);
            model.PartRebookings = budgetService.GetBudgetPostingPartRebookings(id);
            //Set Booleans
            foreach (var item in model.BudgetPostings)
            {
                if (item.RFPBudgetLineId.HasValue)
                {
                    model.HasProcurement = true;
                    break;
                }
            }

            return View("PostDetails", model);
        }

        #endregion

        #region .Rebooking.

        /// <summary>
        /// BudgetPosting Id is passed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RebookPostedAmount(Guid id)
        {
            BudgetPosting bp = budgetService.GetBudgetPostingById(id);
            SCMS.UI.Models.RebookModel model = new SCMS.UI.Models.RebookModel
            {
                CurrentBudgetLine = bp,
                NewBudgetLine = new BudgetPosting(),
                CurrentProjectId = bp.PaymentRequestBudgetLine.ProjectBudget.BudgetCategory.ProjectDonor.Id,
                FullRebooking = true,
                AmountRebooked = Math.Round(bp.AmountPosted, 2),
                Projects = new SelectList(orderRequestService.GetProject(), "Id", "Name"),
                ProjectDonors = new SelectList(orderRequestService.GetProjectNos(Guid.Empty), "Id", "ProjectNumber"),
                BudgetLines = new SelectList(orderRequestService.GetProjectBugdetLines(Guid.Empty), "Id", "Description")
            };
            return View("RebookBL", model);
        }

        [HttpPost]
        public ActionResult RebookPostedAmount(SCMS.UI.Models.RebookModel model)
        {
            if (model.FullRebooking)
            {
                budgetService.FullRebookPostedFunds(model.CurrentBudgetLine.Id, model.BudgetLineId, currentStaff);
            }
            else
            {
                budgetService.PartRebookPostedFunds(model.CurrentBudgetLine.Id, model.BudgetLineId, model.AmountRebooked, currentStaff);
            }
            return ViewBudget(model.CurrentProjectId);
        }

        public ActionResult GetProjectDonors(Guid id)
        {
            StringBuilder pdOption = new StringBuilder();
            pdOption.Append("<select id=\"ProjectDonorId\" name=\"ProjectDonorId\" onchange=\"javascript:GetBudgetLines4Rebooking(this)\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            List<Model.ProjectDonor> pdList = orderRequestService.GetProjectNos(id);
            foreach (ProjectDonor item in pdList)
                pdOption.Append("<option value=\"" + item.Id + "\">" + item.ProjectNumber + " (" + item.Donor.ShortName + ") " + "</option>");
            pdOption.Append("</select>");
            
            return Content(pdOption.ToString(), "text/html");
        }

        public ActionResult GetBudgetLines(Guid id)
        {
            StringBuilder blOption = new StringBuilder();
            blOption.Append("<select data-val=\"true\" data-val-required=" + Resources.Global_String_Required + " id=\"BudgetLineId\" name=\"BudgetLineId\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            List<Model.BudgetLineView> blList = orderRequestService.GetProjectBugdetLines(id);
            foreach (BudgetLineView item in blList)
                blOption.Append("<option value=\"" + item.Id + "\">" + item.Description + "</option>");
            blOption.Append("</select>");
            blOption.Append("<br /><span class=\"field-validation-valid\" data-valmsg-for=\"BudgetLineId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");
            return Content(blOption.ToString(), "text/html");
        }

        #endregion

        #region .Link To Master Budget.

        [HttpPost]
        public ActionResult SaveLinkToMB(Models.MasterBudgetLinker mbl)
        {

            string[] x = Request.Form.GetValues("projectId");
            ProjectDonor pd = projectService.GetProjectDonorById(new Guid(x[0]));
            if (mbl.BudgetCaterogies != null)
            {
                foreach (Models.Category cat in mbl.BudgetCaterogies)
                {
                    foreach (Models.BudgetLine bl in cat.BudgetLines)
                    {
                        if (bl.GeneralLedgerId.HasValue)
                            budgetService.LinkBudgetLineToMasterBudget(bl.BudgetLineId, bl.GeneralLedgerId);
                    }
                }
            }
            ModelState.Clear();
            return LinkToMB(pd.Id);

        }

        public ActionResult LinkToMB(Guid id)
        {

            ProjectDonor pd = projectService.GetProjectDonorById(id);
            List<MasterBudgetCategory> mbcList = masterBudgetService.GetMasterBudgetCategories(countryProg);
            List<GeneralLedger> glList = budgetService.GetGeneralLedgers(countryProg.Id);
            List<Models.Category> categories = ConstructCategoriesList(pd, glList);
            List<Models.MBCategoryLink> mbLinks = ConstructGLCodesList(pd, mbcList);
            Models.MasterBudgetLinker mbLinker = new Models.MasterBudgetLinker();
            mbLinker.BudgetCaterogies = categories;
            mbLinker.MasterBudgetCategories = mbLinks;
            mbLinker.Id = pd.Id.ToString();
            ViewBag.ProjectNo = pd.ProjectNumber;
            ViewBag.Currency = pd.Currency.ShortName;
            ViewBag.Donor = pd.Donor.Name;
            return View("LinkToMB", mbLinker);

        }

        public ActionResult EditLinkToMB(Guid id)
        {

            ProjectBudget pb = budgetService.GetProjectBudgetById(id);
            List<GeneralLedger> mbList = budgetService.GetGeneralLedgers(countryProg.Id);
            Models.BudgetLine bl = new Models.BudgetLine();
            bl.EntityBudgetLine = pb;
            bl.EntityBudgetLine.LineNumber = pb.LineNumber;
            bl.BudgetLineId = id;
            bl.GeneralLedgerId = pb.GeneralLedgerId;
            bl.GeneralLedgerCodes = new SelectList(mbList, "Id", "DataText");
            return View("EditLinkToMB", bl);

        }

        [HttpPost]
        public ActionResult UpdateLinkToMB(Models.BudgetLine bl)
        {
            ProjectBudget pb = new ProjectBudget();
            if (bl.BudgetLineId != null && bl.GeneralLedgerId != null)
            {
                pb = budgetService.GetProjectBudgetById(bl.BudgetLineId);
                budgetService.LinkBudgetLineToMasterBudget(bl.BudgetLineId, bl.GeneralLedgerId);
            }
            return LinkToMB(pb.BudgetCategory.ProjectDonor.Id);
        }

        public ActionResult UnlinkFromMB(Guid id)
        {
            ProjectBudget pb = new ProjectBudget();
            if (id != null)
            {
                pb = budgetService.GetProjectBudgetById(id);
                budgetService.UnLinkBudgetLineToMasterBudget(id);
            }
            return LinkToMB(pb.BudgetCategory.ProjectDonor.Id);
        }

        private List<Models.Category> ConstructCategoriesList(ProjectDonor pd, List<GeneralLedger> glList)
        {
            List<BudgetCategory> bcList = budgetService.GetBudgetCategories(pd);
            Models.Category modelBC;
            List<Models.Category> categories = new List<Models.Category>();
            foreach (BudgetCategory bc in bcList)
            {
                modelBC = new Models.Category();
                modelBC.EntityBudgetCategory = bc;
                modelBC.Id = bc.Id.ToString();
                modelBC.BudgetLines = PopulateCategoryBudgetLines(modelBC.EntityBudgetCategory, glList);
                if (modelBC.BudgetLines.Count > 0)
                    categories.Add(modelBC);
            }
            return categories;
        }

        private List<Models.MBCategoryLink> ConstructGLCodesList(ProjectDonor pd, List<MasterBudgetCategory> mbList)
        {
            Models.MBCategoryLink mbLink;
            List<Models.MBCategoryLink> mbLinkList = new List<Models.MBCategoryLink>();
            foreach (MasterBudgetCategory mbc in mbList)
            {
                mbLink = new Models.MBCategoryLink();
                mbLink.EntityMasterBudgetCateogry = mbc;
                mbLink.ProjectBudgetLines = PopulateMBCategoryBudgetLines(mbc, pd, mbList);
                if (mbLink.ProjectBudgetLines.Count > 0)
                    mbLinkList.Add(mbLink);
            }
            return mbLinkList;
        }

        private List<Models.BudgetLine> PopulateCategoryBudgetLines(BudgetCategory budgetCategory, List<GeneralLedger> glList)
        {
            List<Models.BudgetLine> blList = new List<Models.BudgetLine>();
            Models.BudgetLine bl;
            List<ProjectBudget> budgetLines = budgetService.GetBudgetLinesNotInMB(budgetCategory);

            foreach (ProjectBudget budgetLine in budgetLines)
            {
                bl = new Models.BudgetLine();
                bl.BudgetCategoryId = budgetCategory.Id.ToString();
                bl.EntityBudgetLine = budgetLine;
                bl.GeneralLedgerCodes = new SelectList(glList, "Id", "DataText");
                bl.GeneralLedgers = glList;
                bl.BudgetLineId = budgetLine.Id;
                blList.Add(bl);
            }
            return blList;
        }

        private List<Models.BudgetLine> PopulateMBCategoryBudgetLines(MasterBudgetCategory mbc, ProjectDonor pd, List<MasterBudgetCategory> mbcList)
        {
            List<Models.BudgetLine> blList = new List<Models.BudgetLine>();
            Models.BudgetLine bl;
            List<ProjectBudget> budgetLines = budgetService.GetBudgetLinesByMBCategory(mbc, pd);

            foreach (ProjectBudget budgetLine in budgetLines)
            {
                bl = new Models.BudgetLine();
                bl.BudgetCategoryId = mbc.Id.ToString();
                bl.EntityBudgetLine = budgetLine;
                bl.GeneralLedgerCodes = new SelectList(mbcList, "Id", "DataText");
                bl.BudgetLineId = budgetLine.Id;
                blList.Add(bl);
            }
            return blList;
        }

        #endregion

        #region .BudgetLineCheck & Currency Conversion.

        [HttpGet]
        public ActionResult VerifyAvailableFunds(SCMS.UI.Models.BudgetLineCheck blc)
        {

            if (budgetService.SufficientFundsAvailable(blc.TotalPrice, blc.BudgetLineId, blc.CurrencyId, blc.PurchaseOrderId, blc.RFPId, blc.OrItemId))
                ViewBag.Response = "SUCCESS";
            else
                ViewBag.Response = "FAILURE";
            return View("GeneralResponse");

        }

        [HttpGet]
        public ActionResult CalculateFXValue(SCMS.UI.Models.CurrencyConvert cc)
        {
            try
            {
                decimal amount = exchangeRateService.GetForeignCurrencyValue(new Guid(cc.FXCurrencyId), new Guid(cc.LocalCurrencyId), cc.Amount, countryProg.Id);
                ViewBag.Response = amount;
            }
            catch
            {
                ViewBag.Response = 0;
            }
            return View("GeneralResponse");
        }

        #endregion

        public IEnumerable<BudgetLineView> blList { get; set; }
    }
}
