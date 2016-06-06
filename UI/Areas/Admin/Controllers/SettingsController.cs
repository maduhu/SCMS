using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Settings;
using SCMS.CoreBusinessLogic.Web;
using SCMS.UI.Areas.Admin.Models.Settings;

namespace SCMS.UI.Areas.Admin.Controllers
{
    public class SettingsController : AdminBaseController
    {
        private readonly ISettingService m_SettingService;
        private readonly IPermissionService m_PermissionService;

        public SettingsController(ISettingService settingService, IPermissionService permissionService, IUserContext userContext)
            : base(userContext)
        {
            m_SettingService = settingService;
            m_PermissionService = permissionService;
        }

        //
        // GET: /Admin/Settings/
        [HttpGet]
        public ActionResult Index()
        {
            if(!m_PermissionService.Authorize(StandardPermissionProvider.SettingsManage))
            {
                return AccessDeniedView();
            }

            var model = new SettingsModel();
            var commonSettings = SettingsHelper<CommonSettings>.Settings;
            Mapper.DynamicMap(commonSettings, model.CommonSettings);
            model.CommonSettings.SetImageTypesFromSettings();

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(SettingsModel model)
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.SettingsManage))
            {
                return AccessDeniedView();
            }

            if(ModelState.IsValid)
            {
                model.CommonSettings.SetImageTypesFromModel();
                var commonSettings = SettingsHelper<CommonSettings>.Settings;
                Mapper.DynamicMap(model.CommonSettings, commonSettings);
                m_SettingService.SaveSetting(commonSettings);
            }

            return View(model);
        }
    }
}
