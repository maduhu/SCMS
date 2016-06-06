using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Web;

namespace SCMS.UI.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserContext m_UserContext;

        public HomeController(IUserContext userContext)
        {
            m_UserContext = userContext;
        }

        public ActionResult Index()
        {
            if (m_UserContext.IsAuthenticated) {
                return this.RedirectToAction("Index", "DashBoard");
            }

            ViewBag.feeback = null;

            if (TempData["feedback"]!=null) {
                ViewBag.feeback = TempData["feedback"];
                TempData["feeback"] = null;
            }

            return View();
        }
    }
}
