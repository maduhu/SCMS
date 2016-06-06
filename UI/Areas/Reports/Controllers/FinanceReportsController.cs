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
    public class FinanceReportsController : Controller
    {
        //
        // GET: /Reports/FinanceReports/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RFPOverview()
        {
            ViewBag.ViewName = "RFPOverview";
            return View("PartialResponses");
        }

        public ActionResult RFPBudgeLines()
        {
            ViewBag.ViewName = "RFPBudgeLines";
            return View("PartialResponses");
        }

    }
}
