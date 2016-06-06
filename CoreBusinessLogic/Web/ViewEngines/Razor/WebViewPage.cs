using System;
using System.ComponentModel.Design;
using System.IO;
using System.Web.Mvc;
using System.Web.WebPages;
using SCMS.CoreBusinessLogic.Security;

namespace SCMS.CoreBusinessLogic.Web.ViewEngines.Razor
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        
        public IUserContext UserContext { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();
            UserContext = DependencyResolver.Current.GetService(typeof(IUserContext)) as IUserContext;
        }
    }

    public abstract class WebViewPage : WebViewPage<object>
    {
    }
}
