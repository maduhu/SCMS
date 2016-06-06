using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.Caching;
using SCMS.CoreBusinessLogic.People;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.StaffServices;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Users
{
    public partial class SystemUserService : ISystemUserService
    {
        private readonly ICacheService m_CacheService;
        private readonly IEncryptionService m_EncryptionService;
        private readonly IStaffService m_StaffService;
        private readonly IPersonService m_PersonService;
        private const string RoleCacheKeyPattern = "SystemUserService.Roles.";
        private const string AllRolesCacheKey = "SystemUserService.Roles.all.{0}";
        private const string RoleCacheKey = "SystemUserService.Roles.role.{0}";
        private const string RoleBySystemNameCacheKey = "SystemUserService.Roles.systemname.{0}";

        public SystemUserService(ICacheService cacheService, IEncryptionService encryptionService, IStaffService staffService, IPersonService personService)
        {
            m_CacheService = cacheService;
            m_EncryptionService = encryptionService;
            m_StaffService = staffService;
            m_PersonService = personService;
        }

        #region Implementation of ISystemUserService

        public void InsertRole(Role role)
        {
            using (var context = SCMSEntities.Define())
            {
                context.Roles.Add(role);
                context.SaveChanges();
                m_CacheService.RemoveByPattern(RoleCacheKeyPattern);
            }
        }

        public void UpdateRole(Role role)
        {
            using (var context = SCMSEntities.Define())
            {
                context.Roles.Attach(role);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(role, System.Data.EntityState.Modified);
                context.SaveChanges();
                m_CacheService.RemoveByPattern(RoleCacheKeyPattern);
            }
        }

        public IList<Role> GetAllRoles(bool activeOnly)
        {
            using (var context = SCMSEntities.Define())
            {
                return m_CacheService.Get(AllRolesCacheKey.F(activeOnly), CacheTimeSpan.Infinite,
                () => context.Roles.Where(r => (!activeOnly) || r.Active)
                                                .OrderBy(r => r.Name).ToList());    
            }
            
        }

        public Role GetRoleById(Guid roleId, bool useCache)
        {
            if (roleId == Guid.Empty)
                return null;
            using (var context = SCMSEntities.Define())
            {
                return
                    useCache
                        ? m_CacheService.Get(string.Format(RoleCacheKey, roleId),
                                             CacheTimeSpan.Infinite,
                                             () =>
                                             context.Roles.Where(r => r.Id == roleId).IncludePermissionRecords().FirstOrDefault())
                        : context.Roles.Where(r => r.Id == roleId).IncludePermissionRecords().FirstOrDefault();
            }
        }

        public Role GetRoleBySystemName(string systemName, bool useCache)
        {
            if (systemName.IsNullOrWhiteSpace())
                return null;
            using (var context = SCMSEntities.Define())
            {
                return useCache
                           ? m_CacheService.Get(string.Format(RoleBySystemNameCacheKey, systemName),
                                                CacheTimeSpan.Infinite,
                                                () =>
                                                context.Roles.Where(r => r.SystemName == systemName).IncludePermissionRecords().
                                                    FirstOrDefault())
                           : context.Roles.Where(r => r.SystemName == systemName).IncludePermissionRecords().
                                 FirstOrDefault();
            }
        }

        #endregion
    }
}
