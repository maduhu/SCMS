using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Security;

namespace SCMS.CoreBusinessLogic.Web
{
    public class AdminAuthorizeAttribute : SignedInAuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isSignedIn = base.AuthorizeCore(httpContext);
            if (isSignedIn)
            {
                var permissionService = DependencyResolver.Current.Resolve<IPermissionService>();
                bool result = permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel);
                return result;
            }
            return false;

        }
    }
}
