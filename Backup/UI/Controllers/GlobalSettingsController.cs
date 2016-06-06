using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;

namespace SCMS.UI.Controllers
{
    public class GlobalSettingsController : PortalBaseController
    {
        //
        // GET: /GlobalSettings/

        public GlobalSettingsController(IPermissionService permissionService, IUserContext userContext)
            : base(userContext, permissionService)
        { 
        
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
