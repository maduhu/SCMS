using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.Paging;
using SCMS.Model;
using SCMS.CoreBusinessLogic.NotificationsManager;

namespace SCMS.CoreBusinessLogic.Users
{
    public interface ISystemUserService
    {
        #region Roles section

        void InsertRole(Role role);
        void UpdateRole(Role role);
        IList<Role> GetAllRoles(bool activeOnly = false);
        Role GetRoleById(Guid roleId, bool useCache = false);
        Role GetRoleBySystemName(string systemName, bool useCache = false);

        #endregion

        #region Users

        void InsertUser(SystemUser user);
        void UpdateUser(SystemUser user);
        void DeleteUserRole(SystemUserRole userRole);
        void InsertUserRole(SystemUserRole userRole);
        SystemUser GetUserById(Guid id, bool userCache);
        SystemUser GetUserByEmail(String email, bool userCache);
        IPagedList<SystemUser> FindUsers(UserFilter filter, int pageIndex, int pageSize);
        PasswordChangeResult ChangePassword(ChangePasswordRequest changePasswordRequest);
        PINChangeResult ChangePIN(ChangePINRequest changePINRequest);
        void AssignRoles(SystemUser user, params Guid[] roleIds);
        IEnumerable<SystemUserRole> GetUserRoles(Guid userId, bool userCache);
        SystemUser GetCurrentUser();
        SystemUser GetPasswordResetUser(string Email, Guid PasswordResetCode);
        bool ValidateResetPasswordCode(Guid rpCode);
        void SendWelcomeMessages(INotificationService notificationService, Guid cpId);

        #endregion

    }

    public class UserFilter
    {
        public bool? Active { get; set; }
        public bool? Locked { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid[] RoleIds { get; set; }
        public Guid? CountryProgrammeId { get; set; }
    }
}
