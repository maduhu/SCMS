using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Budgeting;
using SCMS.CoreBusinessLogic.Projects;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic._Currency;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class MasterBudgetController : PortalBaseController
    {
        //
        // GET: /MasterBudget/

        private IBudgetService budgetService;
        private IProjectService projectService;
        private IMasterBudgetService masterBudgetService;
        private ICurrencyService currencyService;

        public MasterBudgetController(IPermissionService permissionService, IBudgetService budgetService, ICurrencyService currencyService,
            IMasterBudgetService masterBudgetService, IProjectService projectService, IUserContext userContext)
            : base(userContext, permissionService)
        {
            this.budgetService = budgetService;
            this.currencyService = currencyService;
            this.masterBudgetService = masterBudgetService;
            this.projectService = projectService;
        }

        public ActionResult Index()
        {
            if (!permissionService.Authorize(StandardPermissionProvider.MasterBudgetView))
                return AccessDeniedView();
            return View();
        }

        public ActionResult GLMasterBudget()
        {
            if (!permissionService.Authorize(StandardPermissionProvider.MasterBudgetView))
                return AccessDeniedView();
            return View();
        }

        public ActionResult MBCIndex()
        {
            if (!permissionService.Authorize(StandardPermissionProvider.MasterBudgetCategoriesView))
                return AccessDeniedView();
            return View();
        }

        public ActionResult MBPIndex()
        {
            if (!permissionService.Authorize(StandardPermissionProvider.MasterBudgetProjectionView))
                return AccessDeniedView();
            return View();
        }

        public ActionResult MBHeader()
        {
            Models.MasterBudgetHeader mbh = new Models.MasterBudgetHeader();
            List<Currency> currencies = projectService.GetCurrencies(countryProg.Id);
            List<MBCategory> mbcList = masterBudgetService.GenerateMasterBudget(mbCurrency, countryProg);
            ComputeTotals(mbcList);
            ViewBag.Currency = mbCurrency.ShortName;
            ViewBag.CurrencyId = mbCurrency.Id;
            ViewBag.CountryProgrammeId = countryProg.Id;
            ViewBag.CPName = countryProg.ProgrammeName;
            mbh.MBCList = mbcList;
            mbh.Currencies = new SelectList(currencies, "Id", "Name", mbCurrency.Id);
            return View("MasterBudgetHeader", mbh);
        }

        public ActionResult GLMBHeader()
        {
            Models.MasterBudgetHeader mbh = new Models.MasterBudgetHeader();
            List<Currency> currencies = projectService.GetCurrencies(countryProg.Id);
            List<MBCategory> mbcList = masterBudgetService.GenerateGLMasterBudget(mbCurrency, countryProg);
            ComputeGLTotals(mbcList);
            ViewBag.Currency = mbCurrency.ShortName;
            ViewBag.CurrencyId = mbCurrency.Id;
            ViewBag.CountryProgrammeId = countryProg.Id;
            ViewBag.CPName = countryProg.ProgrammeName;
            mbh.MBCList = mbcList;
            mbh.Currencies = new SelectList(currencies, "Id", "Name", mbCurrency.Id);
            return View("GLMBHeader", mbh);
        }


        [HttpPost]
        public ActionResult GenerateMB(Models.MasterBudgetHeader mbh)
        {

            Currency currency = currencyService.GetCurrency(new Guid(mbh.CurrencyId));
            List<MBCategory> mbcList = masterBudgetService.GenerateMasterBudget(currency, countryProg);
            ViewBag.Currency = currency.ShortName;
            ViewBag.CurrencyId = currency.Id;
            ViewBag.CountryProgrammeId = countryProg.Id;
            ComputeTotals(mbcList);
            return View("MasterBudget", mbcList);
        }

        [HttpPost]
        public ActionResult GenerateGLMB(Models.MasterBudgetHeader mbh)
        {
            Currency currency = currencyService.GetCurrency(new Guid(mbh.CurrencyId));
            List<MBCategory> mbcList = masterBudgetService.GenerateGLMasterBudget(currency, countryProg);
            ViewBag.Currency = currency.ShortName;
            ViewBag.CurrencyId = currency.Id;
            ViewBag.CountryProgrammeId = countryProg.Id;
            ComputeGLTotals(mbcList);
            return View("MasterBudgetGL", mbcList);
        }

        private void ComputeTotals(List<MBCategory> mbcList)
        {
            MBCategory mbc = new MBCategory();
            mbc.TotalBudget = 0;
            mbc.TotalCommitted = mbc.TotalPosted = mbc.RemainingBalance = mbc.CostProjection = mbc.Surplus = 0;
            foreach (MBCategory mbCat in mbcList)
            {
                mbc.TotalBudget += mbCat.TotalBudget;
                mbc.TotalCommitted += mbCat.TotalCommitted;
                mbc.TotalPosted += mbCat.TotalPosted;
                mbc.RemainingBalance += mbCat.RemainingBalance;
                mbc.CostProjection += mbCat.CostProjection;
            }
            mbc.PercentageSpent = mbc.TotalBudget > 0 ? (float?)(mbc.TotalPosted / mbc.TotalBudget) * 100 : 0;
            mbc.Surplus = mbc.RemainingBalance - mbc.CostProjection;
            mbcList.Add(mbc);
        }

        private void ComputeGLTotals(List<MBCategory> mbcList)
        {
            MBCategory mbc = new MBCategory();
            mbc.TotalBudget = 0;
            mbc.TotalCommitted = mbc.TotalPosted = mbc.RemainingBalance = mbc.CostProjection = mbc.Surplus = 0;
            foreach (MBCategory mbCat in mbcList)
            {
                mbc.TotalBudget += mbCat.TotalBudget;
                mbc.TotalCommitted += mbCat.TotalCommitted;
                mbc.TotalPosted += mbCat.TotalPosted;
                mbc.RemainingBalance += mbCat.RemainingBalance;
            }
            mbc.PercentageSpent = mbc.TotalBudget > 0 ? (float?)(mbc.TotalPosted / mbc.TotalBudget) * 100 : 0;
            mbcList.Add(mbc);
        }

        #region .MB Cateogries .

        public ActionResult MBCategories()
        {

            List<MasterBudgetCategory> mbcList = masterBudgetService.GetMasterBudgetCategories(countryProg);

            return View("MBCategories", mbcList);

        }

        public ActionResult CreateCategory()
        {

            ViewBag.FormHeader = Resources.MasterBudgetController_String_NewMBCategory;
            ViewBag.ButtonText = Resources.BudgetController_String_AddCategory;
            return View("CreateCategory", new MBCategoryModel());

        }

        public ActionResult EditCategory(string id)
        {

            MasterBudgetCategory mbc = masterBudgetService.GetMasterBudgetCategoryById(id);
            MBCategoryModel mbcModel = new MBCategoryModel();
            mbcModel.EntityMBCategory = mbc;
            mbcModel.Id = mbc.Id;
            mbcModel.Number = mbc.Number;
            mbcModel.Description = mbc.Description;
            @ViewBag.FormHeader = Resources.Global_String_Category + ": " + mbc.Number;
            @ViewBag.ButtonText = Resources.BudgetController_String_SaveCategory;
            return View("CreateCategory", mbcModel);

        }

        public ActionResult DeleteCategory(string id)
        {

            masterBudgetService.DeleteMBCategory(id);
            return MBCategories();

        }

        [HttpPost]
        public ActionResult SaveCategory(MBCategoryModel mbc)
        {

            MasterBudgetCategory mbCat = new MasterBudgetCategory();
            MasterBudgetCategory existingMBC = masterBudgetService.GetMasterBudgetCategoryByNumber(mbc.Number, countryProg);
            if (mbc.Id.Equals(Guid.Empty))
            {
                if (existingMBC == null)
                {
                    mbCat.CountryProgrammeId = countryProg.Id;
                    mbCat.Number = mbc.Number;
                    mbCat.Description = mbc.Description;
                    masterBudgetService.CreateMBCategory(mbCat);
                }
                else
                {
                    ViewBag.ActionStatus = 0;
                    ViewBag.StatusMsg = string.Format(Resources.BudgetController_String_CategoryExists, mbc.Number);
                }
            }
            else
            {
                mbCat = masterBudgetService.GetMasterBudgetCategoryById(mbc.Id.ToString());
                mbCat.Number = mbc.Number;
                mbCat.Description = mbc.Description;
                if (existingMBC == null || (existingMBC != null && existingMBC.Id == mbCat.Id))
                {
                    masterBudgetService.UpdateMBCategory(mbCat);
                }
                else
                {
                    ViewBag.ActionStatus = 0;
                    ViewBag.StatusMsg = string.Format(Resources.BudgetController_String_CategoryExists, mbCat.Number);
                }
            }
            return MBCategories();

        }

        #endregion

        #region .General Ledger.

        /// <summary>
        /// id is MasterBudgetCategoryId
        /// </summary>
        /// <param name="mbcId"></param>
        /// <returns></returns>
        public ActionResult AddNewGL(Guid mbcId)
        {
            return View("CreateGL", new GeneralLedger { Id = Guid.Empty, MasterBudgetCategoryId = mbcId });
        }

        public ActionResult EditGL(Guid id)
        {
            GeneralLedger gl = masterBudgetService.GetGeneralLedgerById(id);
            return View("CreateGL", gl);
        }

        public ActionResult SaveGL(GeneralLedger gl)
        {
            masterBudgetService.SaveGeneralLedger(gl);
            return MBCategories();
        }

        public ActionResult DeleteGL(Guid id)
        {
            masterBudgetService.DeleteGeneralLedgerById(id);
            return MBCategories();
        }

        #endregion

        #region .MB Projections.

        public ActionResult MBProjections()
        {

            List<AnnualCostProjection> mbProjections = masterBudgetService.GetMBProjections(countryProg);
            List<MBProjection> mbpList = new List<MBProjection>();
            MBProjection mbp;
            foreach (var mbProjection in mbProjections)
            {
                mbp = new MBProjection();
                mbp.Id = mbProjection.Id;
                mbp.EntityProjection = mbProjection;
                mbp.AmountProjected = mbProjection.AmountProjected;
                mbp.CurrencyId = mbProjection.CurrencyId;
                mbp.MasterBudgetCategoryId = mbProjection.MasterBudgetCategoryId;
                mbpList.Add(mbp);
            }
            return View("MBProjections", mbpList);

        }

        public ActionResult CreateProjection()
        {

            MBProjection mbp = new MBProjection();
            mbp.Currencies = new SelectList(projectService.GetCurrencies(countryProg.Id), "Id", "ShortName", mbCurrency.Id);
            mbp.MasterBudgetCategories = new SelectList(masterBudgetService.GetMBCListNotProjected(countryProg), "Id", "Description");
            @ViewBag.FormHeader = Resources.MasterBudgetController_String_NewProjection;
            @ViewBag.ButtonText = Resources.MasterBudgetController_String_SaveProjection;
            return View("CreateProjection", mbp);

        }

        public ActionResult EditProjection(string id)
        {
            MBProjection mbp = new MBProjection();
            if (id != null)
            {
                AnnualCostProjection acp = masterBudgetService.GetMasterBudgetProjectionById(id);

                mbp.EntityProjection = acp;
                mbp.Id = acp.Id;
                mbp.CurrencyId = acp.CurrencyId;
                mbp.MasterBudgetCategoryId = acp.MasterBudgetCategoryId;
                mbp.AmountProjected = acp.AmountProjected;
                mbp.Currencies = new SelectList(projectService.GetCurrencies(countryProg.Id), "Id", "ShortName", mbp.CurrencyId);
                mbp.MasterBudgetCategories = new SelectList(masterBudgetService.GetMBCListNotProjected(countryProg), "Id", "Description", mbp.MasterBudgetCategoryId);
                @ViewBag.FormHeader = Resources.MasterBudgetController_String_EditProjection;
                @ViewBag.ButtonText = Resources.MasterBudgetController_String_UpdateProjection;

            }
            return View("CreateProjection", mbp);
        }

        public ActionResult SaveProjection(MBProjection mbp)
        {

            AnnualCostProjection acp = new AnnualCostProjection();
            if (mbp.Id.ToString().Contains("0000-0000"))
            {
                acp.MasterBudgetCategoryId = mbp.MasterBudgetCategoryId;
                acp.CurrencyId = mbp.CurrencyId;
                acp.AmountProjected = mbp.AmountProjected;
                acp.CountryProgrammeId = countryProg.Id;
                masterBudgetService.CreateMBProjection(acp);
            }
            else
            {
                acp.Id = mbp.Id;
                acp.CurrencyId = mbp.CurrencyId;
                acp.AmountProjected = mbp.AmountProjected;
                acp.CountryProgrammeId = countryProg.Id;
                masterBudgetService.UpdateMBProjection(acp);
            }
            return MBProjections();

        }

        public ActionResult DeleteProjection(string id)
        {

            if (id != null)
            {
                masterBudgetService.DeleteMBProjection(id);
            }
            return MBProjections();

        }

        #endregion

    }
}
