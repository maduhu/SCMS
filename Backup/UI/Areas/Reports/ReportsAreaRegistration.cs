using System.Web.Mvc;

namespace SCMS.UI.Areas.Reports
{
    public class ReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Reports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Reports_default",
                "Reports/{controller}/{action}/{id}",
                new { controller = "OrderRequest", action = "ViewOrder", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Reports_MASTEr_budget_summary",
                "Reports/{controller}/{action}/{currencyId}/{countryProgrammeId}",
                new { controller = "MasterBudget", action = "Summary", currencyId = "", countryProgrammeId="" }
            );
            context.MapRoute(
                "Reports_MASTEr_budget_detail",
                "Reports/{controller}/{action}/{currencyId}/{countryProgrammeId}",
                new { controller = "MasterBudget", action = "Detail", currencyId = "", countryProgrammeId = "" }
            );

            context.MapRoute(
                "Reports_ProJect_budget_detail",
                "Reports/{controller}/{action}/{id}",
                new { controller = "ProjectBudget", action = "Detail", id ="" }
                );

            context.MapRoute(
                "Reports_GRN",
                "Reports/{controller}/{action}/{id}",
                new { controller = "GoodsReceived", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Reports_WH_release_form",
                "Reports/{controller}/{action}/{id}",
                new { controller = "WareHouse", action = "ReleaseForm", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Reports_req4pay",
                "Reports/{controller}/{action}/{id}",
                new { controller = "Request4Payment", action = "Pdf", id = UrlParameter.Optional }
            );
        }
    }
}
