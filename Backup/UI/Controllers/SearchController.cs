using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;

namespace SCMS.UI.Controllers
{
    public class SearchController : PortalBaseController
    {
        public SearchController(IPermissionService permissionService, IUserContext userContext)
            : base(userContext, permissionService)
        {
            
        }

        public ActionResult ItemsByName(String query)
        {
            return View();
        }

        public ActionResult ItemsByNo(String query)
        {
            return View();
        }

        public ActionResult Documents(String query)
        {
            return View();
        }

        public ActionResult ReportBudgets(String query)
        {
            return View();
        }
    }
}
