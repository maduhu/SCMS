using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Users;
using SCMS.Resource;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.UI.Controllers
{
    public class AccountController : BaseController
    {
        private IAuthenticationService authSvc;
        //private readonly ISystemUserService m_SystemUserService;

        public AccountController(IAuthenticationService authService)//, ISystemUserService systemUserService)
        {
            this.authSvc = authService;
            //this.m_SystemUserService = systemUserService;
        }
        
        [HttpPost]
        public ActionResult LogIn(FormCollection form)
        {
            if (form.AllKeys.Contains("username") && form.AllKeys.Contains("password")
                
                && Membership.ValidateUser(form["username"], form["password"]))
            {
                bool rememberMe = false;

                if (form.AllKeys.Contains("rememberMe"))
                {
                    if (form["rememberMe"] == "on") {
                        rememberMe = true;
                    }
                }

                //var systemUser = m_SystemUserService.GetUserByEmail(form["username"], false);

                //authSvc.SignIn(systemUser, rememberMe);
                
                FormsAuthentication.SetAuthCookie(form["username"], rememberMe);

                var returnUrl = form["returnUrl"];
                
                if (returnUrl.IsNotNullOrWhiteSpace())
                {
                    return Redirect(returnUrl.UrlDecode());
                }
                return this.RedirectToAction("Index", "DashBoard");
            }
            else
            {
                TempData["feedback"] = Resources.AccountController_String_InvalidUserNamePassword;

                return this.RedirectToAction("Index", "Home");
            }
        }


        public ActionResult LogOut()
        {
            authSvc.SignOut();
            SessionData.CurrentSession = null;
            return this.RedirectToAction("Index", "Home");
        }

    }
}
