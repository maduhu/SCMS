using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Users;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Web
{
    public class WebUserContext : IUserContext
    {
        #region Implementation of IUserContext

        private static IPermissionService PermissionService { get { return DependencyResolver.Current.Resolve<IPermissionService>(); } }
        private readonly ISystemUserService m_SystemUserService;
        private const string HttpContextCurrentCustomer = "WebUserContext.HttpContextCurrentCustomer";

        public WebUserContext(ISystemUserService customerService)
        {
            m_SystemUserService = customerService;
        }

        public bool IsAuthenticated
        {
            get
            {
                if (HttpContext.Current.Items["IsAuthenticated"] == null)
                {
                    HttpContext.Current.Items["IsAuthenticated"] = HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated;
                }

                return (bool)HttpContext.Current.Items["IsAuthenticated"];
            }
            set { HttpContext.Current.Items["IsAuthenticated"] = value; }
        }

        public T Resolve<T>()
        {
            return DependencyResolver.Current.Resolve<T>();
        }

        public SystemUser CurrentUser
        {
            get
            {
                if (IsAuthenticated && HttpContext.Current.Items[HttpContextCurrentCustomer] == null)
                {
                    HttpContext.Current.Items[HttpContextCurrentCustomer] = m_SystemUserService.GetCurrentUser();
                    if (HttpContext.Current.Items[HttpContextCurrentCustomer] == null)
                    {
                        FormsAuthentication.SignOut();
                        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl);
                    }
                }
                return HttpContext.Current.Items[HttpContextCurrentCustomer] as SystemUser;
            }
            set { HttpContext.Current.Items[HttpContextCurrentCustomer] = value; }
        }

        public bool HasPermission(PermissionRecord permissionRecord)
        {
            return PermissionService.Authorize(permissionRecord);
        }

        public bool HasPermission(String permissionName)
        {
            return PermissionService.Authorize(permissionName);
        }

        #endregion
    }
}
