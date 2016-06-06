using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Users;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.UI.GeneralHelper;
using SCMS.Model;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    public class PasswordResetController : Controller
    {
        private readonly ISystemUserService systemUserService;
        private readonly INotificationService notificationService;
        public PasswordResetController(ISystemUserService systemUserService, INotificationService notificationService)
        {
            this.systemUserService = systemUserService;
            this.notificationService = notificationService;
        }
        //
        // GET: /PasswordReset/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">user's email</param>
        /// <returns></returns>
        public ActionResult CheckForUser(string id)
        {
            var user = systemUserService.GetUserByEmail(id, false);
            if (user != null)
            {
                ViewBag.Response = "SUCCESS";
                UserSession.CurrentSession.SystemUser = user;
            }
            else
                ViewBag.Response = "FAILURE";
            return View("GeneralResponse");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">user's email</param>
        /// <returns></returns>
        public ActionResult HandleRequest()
        {
            var user = UserSession.CurrentSession.SystemUser;
            if (user != null)
            {
                //Generate password reset code for this user
                user.PasswordResetCode = Guid.NewGuid();
                user.GenerationTime = DateTime.Now;
                systemUserService.UpdateUser(user);
                //Prepare URL
                string url = Request.Url.OriginalString;
                url = url.Replace("HandleRequest", "ResetMyPassword") + "/" + user.PasswordResetCode;
                string msgBody = PrepareEmailMsgBody(user.Staff.Person.FirstName, url);
                string subject = "Password Reset Request";
                notificationService.SendNotification(user.Staff.Person.OfficialEmail, msgBody, subject, true);
                return View("LinkSent");
            }
            return View("ResetRequestForm");
        }

        private string PrepareEmailMsgBody(string name, string url)
        {
            string msgBody = "<html><head><title>" + Resources.PasswordResetController_String_Header + "</title></head><body style=\"font-family:Verdana; font-size: 11px;\">";
            msgBody += "<h2>SCMS</h2><hr style=\"background: #A7B601; height: 5px; border: none;\" />" + Resources.PasswordResetController_String_Hi + ",";
            msgBody += "<p>" + Resources.PasswordResetController_String_Message1 + "</p>";
            msgBody += "<p>" + Resources.PasswordResetController_String_Message2 + " <a href=\"{1}\" target=\"_blank\">{1}</a>.</p>";
            msgBody += "<p>" + Resources.PasswordResetController_String_Message3 + "</p>";
            msgBody += "<p>" + Resources.PasswordResetController_String_Message4 + "</p>";
            msgBody += "<p>" + Resources.PasswordResetController_String_ThankYou + ",<br />" + Resources.PasswordResetController_String_SCMSSupport + ".</p></body></html>";

            //replace placeholders
            msgBody = string.Format(msgBody, name, url);
            return msgBody;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">user's PasswordResetCode</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ResetMyPassword(Guid id)
        { 
            Models.ResetPasswordModel model = new Models.ResetPasswordModel();
            model.PasswordResetCode = id;
            model.IsValid = systemUserService.ValidateResetPasswordCode(id);
            return View("ChangePassword", model);            
        }

        [HttpPost]
        public ActionResult ResetMyPassword(Models.ResetPasswordModel model)
        {
            SystemUser user = systemUserService.GetPasswordResetUser(model.Email, model.PasswordResetCode);
            if (user != null)
            {
                //clear password reset code, then change password
                user.PasswordResetCode = null;
                user.GenerationTime = null;
                systemUserService.UpdateUser(user);
                if (model.Password.IsNotNullOrWhiteSpace())
                {
                    systemUserService.ChangePassword(new ChangePasswordRequest(model.Email, false, model.Password));
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
