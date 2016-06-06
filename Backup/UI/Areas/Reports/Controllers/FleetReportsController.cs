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
    public class FleetReportsController : Controller
    {
        //
        // GET: /Reports/FleetReports/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult FleetOverview()
        {
            ViewBag.ViewName = "FleetOverview";
            return View("PartialResponses");
        }

        public ActionResult GarageOverview()
        {
            ViewBag.ViewName = "GarageOverview";
            return View("PartialResponses");
        }

        public ActionResult FleetVehicle()
        {
            ViewBag.ViewName = "FleetVehicle";
            return View("PartialResponses");
        }
    }
}
