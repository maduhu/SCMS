using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.UI.Controllers;
using System.Web.Security;
using SCMS.UI.Infrastructure;

namespace SCMS.UI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                new string[] { "SCMS.UI.Controllers" }
            );

            //routes.MapRoute("expenseClaim", "ExpenseClaim/LoadExpseItem/{x}", new { controller = "ExpenseClaim", action = "LoadExpseItem", x = Guid.Empty });
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //SecurityConfigurator.Configure(configuration =>
            //{
            //    configuration.GetAuthenticationStatusFrom(SecurityHelper.UserIsAuthenticated);
            //    //configuration.GetRolesFrom(Helpers.SecurityHelper.UserRoles);

            //    configuration.ResolveServicesUsing(type =>
            //    {
            //        var results = new List<object>();
            //        if (type == typeof(IPolicyViolationHandler)) results.Add(new DefaultPolicyViolationHandler());
            //        return results;
            //    });

            //    configuration.ForAllControllers().DenyAnonymousAccess();

            //    configuration.For<HomeController>(h => h.Index()).Ignore();
            //    configuration.For<AccountController>().Ignore();
            //});



            RegisterGlobalFilters(GlobalFilters.Filters);

            RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            
            ModelValidatorProviders.Providers.Add(
                new FluentValidationModelValidatorProvider(new SCMSValidatorFactory()){AddImplicitRequiredValidator = false});

            DependencyResolver.SetResolver(new NinjectControllerFactory());

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "")
                return;

            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            //// retrieve roles from UserData
            //string[] roles = authTicket.UserData.Split(';');

            //if (Context.User != null)
            //    Context.User = new GenericPrincipal(Context.User.Identity, roles);
        }

    }
}