using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.UI.Models;
using SCMS.Model;
using SCMS.CoreBusinessLogic.Budgeting;
using System.Data;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Resource;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class DashBoardController : PortalBaseController
    {

        private IMasterBudgetService masterBudgetService;
        private IGeneralHelperService generalHService;
        private readonly IUserContext m_UserContext;

        public DashBoardController(IPermissionService permissionService, IMasterBudgetService masterBudgetService,
            IUserContext userContext, IGeneralHelperService generalHService)
            : base(userContext, permissionService)
        {
            this.masterBudgetService = masterBudgetService;
            m_UserContext = userContext;
            this.generalHService = generalHService;
        }

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult DocsDashBoard()
        {

            using (var dbContext = new SCMSEntities())
            {
                DocsDashBoard model = new DocsDashBoard();
                model.OrCreated = dbContext.OrderRequests.Count(p => p.CountryProgrammeId == countryProg.Id);
                model.OrApproved = dbContext.OrderRequests.Count(p => p.IsReviewed == true && p.IsAuthorized == false && p.CountryProgrammeId == countryProg.Id);
                model.OrOngoing = dbContext.OrderRequests.Count(p => p.IsAuthorized == true && p.CountryProgrammeId == countryProg.Id);
                if (dbContext.OrderRequests.Count(p => p.CountryProgrammeId == countryProg.Id) > 0)
                    model.OrTotalValue = (decimal)dbContext.OrderRequests.Where(q => q.IsAuthorized == true && q.CountryProgrammeId == countryProg.Id).Sum(p => p.TotalAmount);

                model.TaTotalValue = 0;

                model.PoCreated = dbContext.PurchaseOrders.Count(p => p.CountryProgrammeId == countryProg.Id);
                model.PoApproved = dbContext.PurchaseOrders.Count(p => p.IsApproved && p.CountryProgrammeId == countryProg.Id);
                model.PoOngoing = dbContext.PurchaseOrders.Count(p => p.IsApproved && p.CountryProgrammeId == countryProg.Id);
                if (dbContext.PurchaseOrders.Count(p => p.CountryProgrammeId == countryProg.Id && p.IsApproved == true) > 0)
                    model.PoTotalValue = (decimal)dbContext.PurchaseOrders.Where(q => q.IsApproved == true && q.CountryProgrammeId == countryProg.Id).Sum(p => p.TotalAmount);
                return View(model);
            }
        }

        public ActionResult GetDocsChart()
        {

            using (var dbContext = new SCMSEntities())
            {
                DataTable dd = new DataTable("Chart");
                dd.Columns.Add("Type", typeof(string));
                dd.Columns.Add(Resources.Dashboard_DocsDashboard_Created, typeof(int));
                dd.Columns.Add(Resources.Dashboard_DocsDashboard_Approved, typeof(int));
                dd.Columns.Add(Resources.Global_String_Authorized, typeof(int));
                DataRow Row = null;

                Row = dd.NewRow();

                dd.Rows.Add(Row);
                Row[0] = Resources.Global_String_OrderRequest;
                Row[1] = dbContext.OrderRequests.Count(p => p.CountryProgrammeId == countryProg.Id);
                Row[2] = dbContext.OrderRequests.Count(p => p.IsReviewed == true && p.IsAuthorized == false && p.CountryProgrammeId == countryProg.Id);
                Row[3] = dbContext.OrderRequests.Count(p => p.IsAuthorized == true && p.CountryProgrammeId == countryProg.Id);

                Row = dd.NewRow();
                dd.Rows.Add(Row);
                Row[0] = Resources.Global_String_PurchaseOrder;
                Row[1] = dbContext.PurchaseOrders.Count(p => p.CountryProgrammeId == countryProg.Id);
                Row[2] = dbContext.PurchaseOrders.Count(p => p.IsApproved == false && p.CountryProgrammeId == countryProg.Id);
                Row[3] = dbContext.PurchaseOrders.Count(p => p.IsApproved == true && p.CountryProgrammeId == countryProg.Id);

                Row = dd.NewRow();
                dd.Rows.Add(Row);
                Row[0] = Resources.Global_String_RFP;
                Row[1] = dbContext.PaymentRequests.Count(p => p.CountryProgrammeId == countryProg.Id);
                Row[2] = dbContext.PaymentRequests.Count(p => p.IsReviewed == true && p.IsAuthorized == false && p.CountryProgrammeId == countryProg.Id);
                Row[3] = dbContext.PaymentRequests.Count(p => p.IsAuthorized == true && p.CountryProgrammeId == countryProg.Id);

                //var myChart = new Chart(width: 375, height: 300, theme: ChartTheme.Green)
                //        .AddTitle(Resources.Global_String_DocumentStatistics)
                //        .AddLegend(title: "Key", name: null)
                //        .DataBindTable(dd.AsDataView(), dd.Columns[0].ColumnName)
                //        .Write("png");


                return null;

            }
        }

        public ActionResult MasterBDashBoard()
        {
            using (var dbContext = new SCMSEntities())
            {
                DocsDashBoard model = new DocsDashBoard();
                model.OrCreated = dbContext.OrderRequests.Count();
                model.OrApproved = dbContext.OrderRequests.Count(p => p.IsReviewed == true && p.IsAuthorized == false);
                model.OrOngoing = dbContext.OrderRequests.Count(p => p.IsAuthorized == true);
                model.OrTotalValue = (decimal)dbContext.OrderRequests.Where(q => q.IsAuthorized == true).Sum(p => p.TotalAmount);

                model.PoCreated = dbContext.PurchaseOrders.Count();
                model.PoApproved = dbContext.PurchaseOrders.Count(p => p.IsApproved);
                model.PoOngoing = dbContext.PurchaseOrders.Count(p => p.IsApproved);
                return View(model);
            }
        }

        public ActionResult MasterBudget()
        {
            List<MBCategory> mbcList = masterBudgetService.GenerateMasterBudget(mbCurrency, countryProg);
            ViewBag.Currency = mbCurrency.ShortName;
            ViewBag.CurrencyId = mbCurrency.Id;
            ComputeTotals(mbcList);
            return View("DashboardMasterBudget", mbcList);
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
            if (mbc.TotalBudget <= 0)
                mbc.PercentageSpent = 0;
            else
                mbc.PercentageSpent = (float?)(mbc.TotalPosted / mbc.TotalBudget) * 100;
            mbc.Surplus = mbc.RemainingBalance - mbc.CostProjection;
            mbcList.Add(mbc);
        }

        public ActionResult GetORChart()
        {

            DataTable dd = new DataTable("Chart");
            dd.Columns.Add("Type", typeof(string));
            dd.Columns.Add(Resources.Global_String_Values, typeof(float));

            DataRow Row = null;

            MBCategory mbc = masterBudgetService.GetMasterBudgetTotals(countryProg);
            if (mbc != null)
            {
                float tb, tc, tp, fr;
                this.ConvertToPercent(mbc, out tb, out tc, out tp, out fr);

                Row = dd.NewRow();
                dd.Rows.Add(Row);
                Row[0] = Resources.Budget_ViewBudget_TotalBudget;
                Row[1] = mbc.TotalBudget;

                Row = dd.NewRow();
                dd.Rows.Add(Row);
                Row[0] = Resources.Budget_ViewBudget_TotalCommitted;
                Row[1] = mbc.TotalCommitted;

                Row = dd.NewRow();
                dd.Rows.Add(Row);
                Row[0] = Resources.Budget_ViewBudget_TotalPosted;
                Row[1] = mbc.TotalPosted;

                Row = dd.NewRow();
                dd.Rows.Add(Row);
                Row[0] = Resources.Budget_ViewBudget_FundsRemaining;
                Row[1] = mbc.RemainingBalance;



                //var myChart = new Chart(width: 375, height: 300, theme: ChartTheme.Green)
                //        .AddTitle(Resources.General_String_MasterBudget)

                //        .AddLegend(title: "Key", name: null)
                //        .DataBindTable(dd.AsDataView(), dd.Columns[0].ColumnName)
                //        .Write("png");
            }
            return null;
        }

        private void ConvertToPercent(MBCategory mbc, out float tb, out float tc, out float tp, out float fr)
        {
            tb = 100;
            tc = (float)(mbc.TotalCommitted / mbc.TotalBudget) * 100;
            tp = (float)(mbc.TotalPosted / mbc.TotalBudget) * 100;
            fr = (float)(mbc.RemainingBalance / mbc.TotalBudget) * 100;
        }


    }
}
