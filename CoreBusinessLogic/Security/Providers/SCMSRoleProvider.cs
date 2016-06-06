using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Caching;
using SCMS.CoreBusinessLogic.Users;
using System.Web.Security;

namespace SCMS.CoreBusinessLogic.Security.Providers
{
    public class SCMSRoleProvider : RoleProvider
    {
        private ISystemUserService m_SystemUserService;
        private ISystemUserService SystemUserService
        {
            get { return m_SystemUserService ?? (m_SystemUserService = DependencyResolver.Current.Resolve<ISystemUserService>()); }
        }

        private ICacheService m_CacheService;
        private ICacheService CacheService{get { return m_CacheService ?? (m_CacheService = DependencyResolver.Current.Resolve<ICacheService>()); }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return GetRolesForUser(username).Contains(roleName);
        }

        private const string GetRolesForUserCacheKey = "SystemUserService.SystemUser.user.roles.{0}";
        public override string[] GetRolesForUser(string username)
        {
            return CacheService.Get(GetRolesForUserCacheKey.F(username), CacheTimeSpan.TwoMinutes,
                                    () =>
                                        {
                                            var user = SystemUserService.GetUserByEmail(username,true);
                                            var roles = SystemUserService.GetUserRoles(user.Id, true);
                                            return roles.IsNull() || roles.IsEmpty()
                                                       ? new String[] {}
                                                       : roles.Select(role => role.Role.SystemName).ToArray();
                                        });
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
