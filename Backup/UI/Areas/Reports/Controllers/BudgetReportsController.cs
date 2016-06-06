using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.Reports.Utilities;

namespace SCMS.UI.Areas.Reports.Controllers
{
    [MyException]
    public class BudgetReportsController : Controller
    {
        //
        // GET: /Reports/BudgetReports/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BudgetPipeLine()
        {
            ViewBag.ViewName = "BudgetPipeLine";
            return View("PartialResponses");
        }
    }
}
