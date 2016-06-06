using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SCMS.CoreBusinessLogic.Web
{
    public class BaseController : Controller
    {
        public ActionResult AccessDeniedView()
        {
            string action = "/AccessDenied/Index";
            if (Request.Url.OriginalString.Contains("/Admin/") || Request.Url.OriginalString.Contains("/Reports/"))
                action = "../../AccessDenied/Index";
            return RedirectToAction(action);
        }

        public ActionResult AccessDeniedPartialView()
        {
            string htmlContent = "<p style=\"color: Red; font-weight: bold; font-size: 14px;\">Access Denied</p>";
            return Content(htmlContent, "text/html"); 
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
