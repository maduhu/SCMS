using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.UI.GeneralHelper;
using SCMS.Reports.Utilities;

namespace SCMS.UI.Areas.Reports.Controllers
{
    [MyException]
    public class ProcurementReportsController : Controller
    {
        //
        // GET: /Reports/ProcurementReports/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewORequests()
        {
            ViewBag.ViewName = "ViewORequests";
            return View("PartialResponses");
        }

        public ActionResult ViewORequestDetails()
        {
            ViewBag.ViewName = "ViewORequestDetails";
            return View("PartialResponses");
        }

        public ActionResult POrderOverview()
        {
            ViewBag.ViewName = "POrderOverview";
            return View("PartialResponses");
        }

        public ActionResult POBudgeLines()
        {
            ViewBag.ViewName = "POBudgeLines";
            return View("PartialResponses");
        }

        public ActionResult GeneralProcument()
        {
            ViewBag.ViewName = "GeneralProcument";
            return View("PartialResponses");
        }

        public ActionResult AssetProcument()
        {
            ViewBag.ViewName = "AssetProcument";
            return View("PartialResponses");
        }
    }
}
