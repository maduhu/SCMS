using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.NotificationsManager;


namespace SCMS.CoreBusinessLogic.ActionFilters
{
    public class MyExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        private INotificationService NotificationMail;
        public MyExceptionAttribute(INotificationService NotificationService)
        {
            this.NotificationMail = NotificationService;
        }
        public MyExceptionAttribute() { }
        //static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled && filterContext.Exception is Exception && !filterContext.RequestContext.HttpContext.Request.IsLocal)
            {
                filterContext.Result = new RedirectResult("/CustomError/");
                filterContext.ExceptionHandled = true;
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();
                string loggerName = string.Format("{0}Controller.{1}", controller, action);
                NLog.LogManager.GetLogger(loggerName).Error(filterContext.Exception);
                NotificationMail.SendNotification(SCMS.CoreBusinessLogic.NotificationsManager.NotificationHelper.Logistics_Dev_Group,
                    filterContext.Exception.ToString(), SCMS.CoreBusinessLogic.NotificationsManager.NotificationHelper.Logistics_Dev_Group_Subject);
                //logger.Fatal(filterContext.Exception);
            }
        }
    }
}
