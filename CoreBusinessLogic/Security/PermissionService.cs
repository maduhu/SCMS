using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using SCMS.CoreBusinessLogic.Caching;
using SCMS.CoreBusinessLogic.Users;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Security
{
    public class PermissionService : IPermissionService
    {
        private readonly IUserContext m_UserContext;
        private readonly ICacheService m_CacheService;
        private readonly ISystemUserService m_SystemUserService;
        private static readonly string m_RoleCacheKeyPattern = "Role.Permissions.";
        private static readonly string m_PermissionCacheKeyPattern = "Role.Permissions.{0}";
        private static readonly string m_RolePermissionsCacheKey = "Role.Permissions.role.{0}";

        public PermissionService(ISystemUserService customerService, IUserContext userContext, ICacheService cacheService)
        {
            m_UserContext = userContext;
            m_CacheService = cacheService;
            m_SystemUserService = customerService;
        }

        private void ClearCache()
        {
            m_CacheService.RemoveByPattern(m_RoleCacheKeyPattern);
            m_CacheService.RemoveByPattern("UserMenuCacheKey.");
        }

        #region Implementation of IPermissionService

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void DeletePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");
            using (var context = SCMSEntities.Define())
            {
                context.PermissionRecords.Attach(permission);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(permission, System.Data.EntityState.Modified);
                context.PermissionRecords.Remove(permission);
                context.SaveChanges();
            }
            ClearCache();
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordById(Guid permissionId)
        {
            if (permissionId == Guid.Empty)
                return null;
            using (var context = SCMSEntities.Define())
            {
                return context
                    .PermissionRecords
                    .Where(pr => pr.Id == permissionId)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordBySystemName(string systemName, bool userCache=false)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;
            using (var context = SCMSEntities.Define())
            {
                if(userCache)
                {
                    return m_CacheService.Get(m_PermissionCacheKeyPattern.F(systemName),
                                              CacheTimeSpan.Infinite,
                                              () => (from pr in context.PermissionRecords
                                                     orderby pr.Id
                                                     where pr.SystemName == systemName
                                                     select pr).FirstOrDefault());
                }

                var query = from pr in context.PermissionRecords
                            orderby pr.Id
                            where pr.SystemName == systemName
                            select pr;

                var permission = query.FirstOrDefault();
                return permission;
            }
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IList<PermissionRecord> GetAllPermissionRecords()
        {
            using (var context = SCMSEntities.Define())
            {
                return (from cr in context.PermissionRecords.Include("RolePermissionRecords")
                        orderby cr.Name
                        select cr).ToArray().ToList();
            }
        }

        public void InsertRolePermissionRecord(RolePermissionRecord rolePermissionRecord)
        {
            using (var context = SCMSEntities.Define())
            {
                context.RolePermissionRecords.Add(rolePermissionRecord);
                context.SaveChanges();
            }

            ClearCache();
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void InsertPermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");
            using(var context = SCMSEntities.Define())
            {
                context.PermissionRecords.Add(permission);
                context.SaveChanges();
            }

            ClearCache();
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void UpdatePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            using(var context = SCMSEntities.Define())
            {
                context.PermissionRecords.Attach(permission);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(permission, System.Data.EntityState.Modified);
                context.SaveChanges();
            }

            ClearCache();
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void InstallPermissions(IPermissionProvider permissionProvider)
        {
            //install new permissions
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 == null)
                {
                    //new permission (install it)
                    permission1 = new PermissionRecord
                    {
                        Id = Guid.NewGuid(),
                        Name = permission.Name,
                        SystemName = permission.SystemName,
                        Category = permission.Category,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    };

                    //save new permission
                    InsertPermissionRecord(permission1);

                    //default customer role mappings
                    var defaultPermissions = permissionProvider.GetDefaultPermissions();
                    foreach (var defaultPermission in defaultPermissions)
                    {
                        var role = m_SystemUserService.GetRoleBySystemName(defaultPermission.RoleSystemName);
                        if (role == null)
                        {
                            //new role (save it)
                            role = new Role
                            {
                                Id = Guid.NewGuid(),
                                Name = defaultPermission.RoleSystemName,
                                Active = true,
                                SystemName = defaultPermission.RoleSystemName,
                                Created = DateTime.UtcNow,
                                Modified = DateTime.UtcNow
                            };
                            m_SystemUserService.InsertRole(role);
                        }

                        role = m_SystemUserService.GetRoleById(role.Id, false);

                        var defaultMappingProvided = (from p in defaultPermission.PermissionRecords
                                                      where p.SystemName == permission1.SystemName
                                                      select p).Any();
                        var mappingExists = (from p in role.RolePermissionRecords.Select(rpr=>rpr.PermissionRecord)
                                             where p.SystemName == permission1.SystemName
                                             select p).Any();
                        if (defaultMappingProvided && !mappingExists)
                        {
                            InsertRolePermissionRecord(new RolePermissionRecord
                                                                      {
                                                                          Id = Guid.NewGuid(),
                                                                          Created = DateTime.UtcNow,
                                                                          Modified = DateTime.UtcNow,
                                                                          RoleId = role.Id,
                                                                          PermissionRecordId = permission1.Id
                                                                      });
                        }
                    }

                    
                }
            }

            ClearCache();
        }

        

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void UninstallPermissions(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 != null)
                {
                    DeletePermissionRecord(permission1);
                }
            }

            ClearCache();
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordSystemName)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var permission = GetPermissionRecordBySystemName(permissionRecordSystemName,true);
            return Authorize(permission);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission)
        {
           
            if (permission == null)
                return false;

            if (m_UserContext.CurrentUser == null)
                return false;

            var customerRoles = m_UserContext.CurrentUser.SystemUserRoles.Where(cr => cr.Role.Active);
            foreach (var role in customerRoles)
            {
                
                foreach (var permission1 in GetRolePermissions(role.RoleId))
                {
                    if (permission1.SystemName.Equals(permission.SystemName,StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        private IEnumerable<PermissionRecord> GetRolePermissions(Guid roleId)
        {
            using (var context = SCMSEntities.Define())
            {
                return m_CacheService.Get(m_RolePermissionsCacheKey.F(roleId),
                                      CacheTimeSpan.Infinite,
                                      () => context.
                                                RolePermissionRecords.Where(
                                                    p => p.RoleId == roleId).
                                                Select(p => p.PermissionRecord).ToArray());
            }
            
        }

        public void DeleteRolePermissionRecord(RolePermissionRecord rolePermission)
        {
            using (var context = SCMSEntities.Define())
            {
                context.RolePermissionRecords.Attach(rolePermission);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(rolePermission, System.Data.EntityState.Modified);
                context.RolePermissionRecords.Remove(rolePermission);
                context.SaveChanges();
            }
            ClearCache();
        }

        #endregion
    }
}
