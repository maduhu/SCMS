using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Caching;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Users;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.UI.Areas.Admin.Models.Role;
using SCMS.UI.Areas.Admin.Models.Security;
using SCMS.Resource;

namespace SCMS.UI.Areas.Admin.Controllers
{
    public class SecurityController : AdminBaseController
    {
        //
        // GET: /Admin/Security/

        #region Fields

        private readonly IUserContext m_WorkContext;
        private readonly IPermissionService m_PermissionService;
        private readonly ISystemUserService m_CustomerService;
        private readonly ICacheService m_CacheService;

        #endregion

        public SecurityController(IUserContext workContext, IPermissionService permissionService,
            ISystemUserService customerService, ICacheService cacheService, IUserContext userContext)
            : base(userContext)
        {
            m_WorkContext = workContext;
            m_PermissionService = permissionService;
            m_CustomerService = customerService;
            m_CacheService = cacheService;
        }
        
        public ActionResult AccessDenied(string pageUrl)
        {
            return View();
        }


        public ActionResult InstallPermissions()
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.AclManage))
                return AccessDeniedView();

            m_PermissionService.InstallPermissions(new StandardPermissionProvider());

            return RedirectToAction("Permissions");
        }

        public ActionResult Permissions()
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.AclManage))
                return AccessDeniedView();

            var model = new PermissionMappingModel();

            var permissionRecords = m_PermissionService.GetAllPermissionRecords();
            var roles = m_CustomerService.GetAllRoles(true);
            foreach (var pr in permissionRecords)
            {
                model.AvailablePermissions.Add(new PermissionRecordModel
                {
                    Name = pr.Name,
                    SystemName = pr.SystemName
                });
            }
            foreach (var cr in roles)
            {
                model.AvailableRoles.Add(new RoleModel
                {
                    Id = cr.Id,
                    Name = cr.Name
                });
            }
            foreach (var pr in permissionRecords)
                foreach (var cr in roles)
                {
                    var cr1 = cr;
                    var allowed = pr.RolePermissionRecords.Where(x => x.RoleId == cr1.Id).ToList().Count() > 0;
                    if (!model.Allowed.ContainsKey(pr.SystemName))
                        model.Allowed[pr.SystemName] = new Dictionary<Guid, bool>();
                    model.Allowed[pr.SystemName][cr.Id] = allowed;
                }

            return View(model);
        }

        [HttpPost, ActionName("Permissions")]
        public ActionResult PermissionsSave(FormCollection form)
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.AclManage))
                return AccessDeniedView();

            var permissionRecords = m_PermissionService.GetAllPermissionRecords();
            var roles = m_CustomerService.GetAllRoles(true);


            foreach (var cr in roles)
            {
                var formKey = "allow_" + cr.Id;
                var permissionRecordSystemNamesToRestrict = form[formKey] != null ? form[formKey].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

                foreach (var pr in permissionRecords)
                {

                    var allow = permissionRecordSystemNamesToRestrict.Contains(pr.SystemName);
                    if (allow)
                    {
                        var cr1 = cr;
                        if (pr.RolePermissionRecords.Where(x => x.RoleId == cr1.Id).FirstOrDefault() == null)
                        {
                            m_PermissionService.InsertRolePermissionRecord(new RolePermissionRecord
                            {
                                Id = Guid.NewGuid(),
                                Created = DateTime.Now,
                                Modified = DateTime.Now,
                                RoleId = cr.Id,
                                PermissionRecordId = pr.Id
                            });
                        }
                    }
                    else
                    {
                        var cr1 = cr;
                        var rolePermissionRecord = pr.RolePermissionRecords.Where(x => x.RoleId == cr1.Id).FirstOrDefault();
                        if (rolePermissionRecord != null)
                        {
                            m_PermissionService.DeleteRolePermissionRecord(rolePermissionRecord);
                        }
                    }
                }
            }
            m_CacheService.Clear();
            TempData["Security.Permission.Message"] = Resources.SecurityController_String_SavedPermissionSuccessfully;
            return RedirectToAction("Permissions");
        }

    }
}
