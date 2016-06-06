using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Security;

namespace SCMS.CoreBusinessLogic.Web
{
    public class SignedInAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return DependencyResolver.Current.Resolve<IUserContext>().IsAuthenticated;
        }
    }
}
