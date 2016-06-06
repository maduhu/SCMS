using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml.Linq;
using SCMS.CoreBusinessLogic.Caching;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Web;

namespace SCMS.UI.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IUserContext m_UserContext;
        private readonly ICacheService m_CacheService;
        private const string MenuSitemapCacheKey = "MenuSitemapCacheKey.{0}";
        private readonly string m_UserMenuCacheKey = "UserMenuCacheKey.{0}";
        private const string UserMenuCacheKeyPattern = "UserMenuCacheKey.";
        private static readonly object s_MenuLoadLockObject=new object();

        public MenuController(IUserContext userContext, ICacheService cacheService)
        {
            m_UserContext = userContext;
            m_CacheService = cacheService;
        }

        //
        // GET: /Menu/

        public ActionResult LeftMenu()
        {
            var content = m_CacheService.Get(
                m_UserMenuCacheKey.F(m_UserContext.IsAuthenticated
                                         ? m_UserContext.CurrentUser.Id.ToString()
                                         : "Anonymous"),
                                         m_UserContext.IsAuthenticated ? CacheTimeSpan.FiveMinutes :CacheTimeSpan.TwoMinutes,
                () => RenderPartialViewToString("XmlMenu", GetMenuElements("~/Menu.Sitemap")));

            return Content(content ?? "");
        }

        public XElement GetMenuElements(string fileName)
        {
            var rootElement = HttpContext.Cache[MenuSitemapCacheKey.F(fileName)] as XElement;
            if (rootElement == null)
            {
                lock (s_MenuLoadLockObject)
                {
                    if (rootElement == null)
                    {
                        var mappedFileName = Server.MapPath(fileName);
                        var sitemap = XDocument.Load(mappedFileName);
                        if (sitemap.Root.HasElements)
                            rootElement = sitemap.Root.Elements().FirstOrDefault();
                        HttpContext.Cache.Add(MenuSitemapCacheKey.F(fileName),
                                              rootElement,
                                              new CacheDependency(mappedFileName),
                                              DateTime.Now + TimeSpan.FromMinutes(120),
                                              Cache.NoSlidingExpiration, CacheItemPriority.Normal, 
                                              (key,item, reason)=> m_CacheService.RemoveByPattern(UserMenuCacheKeyPattern));
                    }
                }

            }
            return rootElement;
        }
    }
}
