using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Users;
using SCMS.CoreBusinessLogic.Web;
using SCMS.UI.Areas.Admin.Models.Role;
using SCMS.Resource;

namespace SCMS.UI.Areas.Admin.Controllers
{
    public class RoleController : AdminBaseController
    {
        private readonly ISystemUserService m_SystemUserService;
        private readonly IPermissionService m_PermissionService;

        public RoleController(ISystemUserService systemUserService, IPermissionService permissionService, IUserContext userContext)
            : base(userContext)
        {
            m_SystemUserService = systemUserService;
            m_PermissionService = permissionService;
        }

        //
        // GET: /Admin/Role/

        public ActionResult Index()
        {
            if(!m_PermissionService.Authorize(StandardPermissionProvider.RolesManage))
            {
                return AccessDeniedView();
            }
            return View();
        }

       
        public ActionResult List()
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.RolesManage))
            {
                return AccessDeniedView();
            }
            var roles = m_SystemUserService.GetAllRoles(false) ?? Enumerable.Empty<Model.Role>();
            return View("List", roles.Select(p=>p.ToModel()));
        }


        [HttpGet]
        public ActionResult Create()
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.RolesManage))
            {
                return AccessDeniedView();
            }
            var model = new RoleModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(RoleModel model)
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.RolesManage))
            {
                return AccessDeniedView();
            }

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = model.ToEntity();
            entity.Id = Guid.NewGuid();
            entity.Modified = entity.Created = DateTime.Now;
            
            m_SystemUserService.InsertRole(entity);

            return List();
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.RolesManage))
            {
                return AccessDeniedView();
            }

            var role  = m_SystemUserService.GetRoleById(id);

            if(role == null)
            {
                return AccessDeniedView();
            }

            return View(role.ToModel());
        }

        [HttpPost]
        public ActionResult Edit(RoleModel model)
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.RolesManage))
            {
                return AccessDeniedView();
            }

            var role = m_SystemUserService.GetRoleById(model.Id);
            if (role == null)
            {
                throw new ArgumentException(Resources.RoleController_String_NoCustomerRoleFound);
            }

            if (ModelState.IsValid)
            {
                if (role.IsSystemRole && !model.Active)
                {
                    ModelState.AddModelError("", Resources.RoleController_String_CantEditSystemRoles);
                }

                if (role.IsSystemRole &&
                    !role.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                {
                    ModelState.AddModelError("", Resources.RoleController_String_CantChangeSystemRoleName);
                }

                if (!ModelState.IsValid)
                    return View(model);

                role.Name = model.Name;
                role.SystemName = model.SystemName;
                role.Description = model.Description;
                role.Active = model.Active;
                role.Modified = DateTime.Now;
                
                m_SystemUserService.UpdateRole(role);

                return List();
            }

            return View(model);
        }

    }
}
