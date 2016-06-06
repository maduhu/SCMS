using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using SCMS.CoreBusinessLogic.Caching;
using SCMS.CoreBusinessLogic.Paging;
using SCMS.Model;
using SCMS.CoreBusinessLogic.NotificationsManager;
using System.Data.Entity.Infrastructure;

namespace SCMS.CoreBusinessLogic.Users
{
    public partial class SystemUserService
    {
        private const string UsersCacheKeyPattern = "SystemUserService.SystemUser.";
        private const string UserCacheKeyPattern = "SystemUserService.SystemUser.user";
        private const string UserByIdCacheKey = "SystemUserService.SystemUser.user.id.{0}";
        private const string UserByEmailCacheKey = "SystemUserService.SystemUser.user.email.{0}";
        private const string UserRoleCacheKey = "SystemUserService.SystemUser.user.roles.{0}";
        

        private void ClearUserCache(Guid? id, string  email)
        {
            if(!id.IsNullOrEmpty())
            {
                m_CacheService.RemoveByPattern(UserByIdCacheKey.F(id));
                m_CacheService.RemoveByPattern(UserRoleCacheKey.F(id));
                m_CacheService.RemoveByPattern("UserMenuCacheKey.{0}".F(id));
            }

            if(email.IsNotNullOrWhiteSpace())
            {
                m_CacheService.RemoveByPattern(UserByEmailCacheKey.F(email));
            }
        }

        #region Implementation of ISystemUserService

        public void InsertUser(SystemUser user)
        {
            using(var context = SCMSEntities.Define())
            {
                context.SystemUsers.Add(user);
                
                context.SaveChanges();
            }
        }

        public void UpdateUser(SystemUser user)
        {
            var userId = user.Id;
            
            using (var context = SCMSEntities.Define())
            {
                context.SystemUsers.Attach(user);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(user, System.Data.EntityState.Modified);
                context.SaveChanges();
                ClearUserCache(userId, null);
            }
        }

        public void DeleteUserRole(SystemUserRole userRole)
        {
            var userId = userRole.SystemUserId;
            using (var context = SCMSEntities.Define())
            {
                context.SystemUserRoles.Attach(userRole);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(userRole, System.Data.EntityState.Modified);
                context.SystemUserRoles.Remove(userRole);
                context.SaveChanges();
                ClearUserCache(userId, null);
            }
        }

        public void InsertUserRole(SystemUserRole userRole)
        {
            var userId = userRole.SystemUserId;
            using (var context = SCMSEntities.Define())
            {
                context.SystemUserRoles.Add(userRole);
                context.SaveChanges();
                ClearUserCache(userId,null);
            }
        }

        public SystemUser GetUserById(Guid id, bool userCache)
        {
            using (var context = SCMSEntities.Define())
            {
                return userCache ? m_CacheService.Get(UserByIdCacheKey.F(id), CacheTimeSpan.FiveMinutes,
                                          () => context.SystemUsers.Where(p=>p.Id == id).IncludeRoles().IncludePerson().FirstOrDefault())
                                          : context.SystemUsers.Where(p => p.Id == id).IncludeRoles().IncludePerson().FirstOrDefault();
            }
        }

        public SystemUser GetUserByEmail(String email, bool userCache)
        {
            using(var context = SCMSEntities.Define())
            {

                return userCache ? m_CacheService.Get(UserByEmailCacheKey.F(email), CacheTimeSpan.FiveMinutes,
                                          () => context.SystemUsers.Where(p => p.Staff.Person.OfficialEmail == email).IncludeRoles().IncludePerson().IncludeCountry().IncludeCurrency().IncludeDesignation().FirstOrDefault())
                                          : context.SystemUsers.Where(p => p.Staff.Person.OfficialEmail == email).IncludeRoles().IncludePerson().IncludeCountry().IncludeCurrency().IncludeDesignation().FirstOrDefault();
            }
        }

        public IPagedList<SystemUser> FindUsers(UserFilter filter, int pageIndex, int pageSize)
        {
            using (var context = SCMSEntities.Define())
            {
                var query = context.SystemUsers.AsQueryable();

                if (filter.Active.HasValue)
                    query = query.Where(p => p.Active == filter.Active);

                if (filter.Locked.HasValue)
                    query = query.Where(p => p.Locked == filter.Locked);

                if (filter.Email.IsNotNullOrWhiteSpace())
                    query = query.Where(p => p.Staff.Person.OfficialEmail == filter.Email);

                if(filter.Name.IsNotNullOrWhiteSpace())
                    query = query.Where(p=>p.Staff.Person.FirstName.Contains(filter.Name) || p.Staff.Person.OtherNames.Contains(filter.Name));
                if (filter.CountryProgrammeId.HasValue)
                    query = query.Where(p => p.Staff.CountrySubOffice.CountryProgrammeId == filter.CountryProgrammeId);

                if(filter.RoleIds.IsNotNullOrEmpty())
                {
                    query = (from user in query
                            join userRole in context.SystemUserRoles on user.Id equals userRole.SystemUserId
                            where filter.RoleIds.Contains(userRole.RoleId)
                            select user).Distinct();
                }

                query = query.IncludeRoles().IncludePerson().IncludeFinanceLimit().OrderBy(p => p.Staff.Person.FirstName).ThenBy(p => p.Staff.Person.OtherNames);

                
                return new PagedList<SystemUser>(query,pageIndex,pageSize);
            }
        }

        public PINChangeResult ChangePIN(ChangePINRequest request)
        {
            var result = new PINChangeResult();
            if (request == null)
            {
                result.AddError("The change PIN request was not valid.");
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError("The email is not entered");
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.NewPIN))
            {
                result.AddError("The PIN is not entered");
                return result;
            }

            var user = GetUserByEmail(request.Email, false);
            if (user == null)
            {
                result.AddError("The specified email could not be found");
                return result;
            }

            var requestIsValid = false;
            if (request.ValidateRequest)
            {
                var oldPIN = m_EncryptionService.GetSHA256(request.OldPIN, "", true);

                var oldPINIsValid = oldPIN == user.PIN;
                if (!oldPINIsValid)
                    result.AddError("Old PIN doesn't match");

                if (oldPINIsValid)
                    requestIsValid = true;
            }
            else
                requestIsValid = true;


            //at this point request is valid
            if (requestIsValid)
            {
                user.PIN = m_EncryptionService.GetSHA256(request.NewPIN, "", true);
                UpdateUser(user);
            }

            return result;
        }

        public PasswordChangeResult ChangePassword(ChangePasswordRequest request)
        {
            var result = new PasswordChangeResult();
            if (request == null)
            {
                result.AddError("The change password request was not valid.");
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError("The email is not entered");
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError("The password is not entered");
                return result;
            }

            var user = GetUserByEmail(request.Email,false);
            if (user == null)
            {
                result.AddError("The specified email could not be found");
                return result;
            }

            var requestIsValid = false;
            if (request.ValidateRequest)
            {
                var oldPwd = m_EncryptionService.GetSHA256(request.OldPassword, user.PasswordSalt,true);

                var oldPasswordIsValid = oldPwd == user.Password;
                if (!oldPasswordIsValid)
                    result.AddError("Old password doesn't match");
                else
                    requestIsValid = true;
            }
            else
                requestIsValid = true;


            //at this point request is valid
            if (requestIsValid)
            {
                var saltKey = m_EncryptionService.CreateSaltKey(5);
                user.PasswordSalt = saltKey;
                user.Password = m_EncryptionService.GetSHA256(request.NewPassword, saltKey,true);
                UpdateUser(user);
            }

            return result;
        }

        public IEnumerable<SystemUserRole> GetUserRoles(Guid userId, bool userCache)
        {
            using (var context = SCMSEntities.Define())
            {
                return userCache ? m_CacheService.Get(UserRoleCacheKey.F(userId), CacheTimeSpan.FiveMinutes,
                                          () => context.SystemUserRoles.Where(p=>p.SystemUserId == userId).IncludeRole().ToArray())
                                          : context.SystemUserRoles.Where(p => p.SystemUserId == userId).IncludeRole().ToArray();
            }
        }

        public void AssignRoles(SystemUser user, params Guid[] roleIds)
        {
            roleIds = roleIds ?? Enumerable.Empty<Guid>().ToArray();
            var userRoles = GetUserRoles(user.Id, false);
            using(var context = SCMSEntities.Define())
            {
                var toDelete = userRoles.Where(p => !roleIds.Contains(p.RoleId)).ToArray();
                var roleIdsToAdd = roleIds.Select(p => p).Delete<Guid>(userRoles.Select(p => p.RoleId));

                toDelete.ForEach(DeleteUserRole);

                roleIdsToAdd.ForEach(p => context.SystemUserRoles
                                              .Add(new SystemUserRole
                                                             {
                                                                 Id = Guid.NewGuid(),
                                                                 Created = DateTime.Now,
                                                                 Modified = DateTime.Now,
                                                                 RoleId = p,
                                                                 SystemUserId = user.Id,
                                                             }));

                context.SaveChanges();
            }
        }

        public SystemUser GetCurrentUser()
        {
            return HttpContext.Current.User.Identity.Name.IsNotNullOrWhiteSpace()
                       ? GetUserByEmail(HttpContext.Current.User.Identity.Name,true)
                       : null;
        }

        public SystemUser GetPasswordResetUser(string Email, Guid PasswordResetCode)
        {
            using (var context = SCMSEntities.Define())
            {
                var user = context.SystemUsers.FirstOrDefault(s => s.Staff.Person.OfficialEmail == Email && s.PasswordResetCode == PasswordResetCode);
                if (user != null)
                {
                    var person = user.Staff.Person;
                }
                return user;
            }
        }

        public bool ValidateResetPasswordCode(Guid rpCode)
        {
            using (var context = SCMSEntities.Define())
            { 
                DateTime date = DateTime.Now;
                var users = context.SystemUsers.Where(s => s.PasswordResetCode == rpCode).ToList();
                TimeSpan ts;
                foreach (var user in users)
                {
                    ts = date - user.GenerationTime.Value;
                    if (ts.TotalMinutes <= 60)
                        return true;
                }
                return false;                
            }
        }

        public void SendWelcomeMessages(INotificationService notificationService, Guid cpId)
        {
            string msgBody = "";
            using (var context = SCMSEntities.Define())
            {
                var userList = context.SystemUsers.Where(s => s.Staff.CountrySubOffice.CountryProgrammeId == cpId).ToList();
                foreach (var user in userList)
                {
                    msgBody = PrepareEmailMsgBody(user.Staff.Person.FirstName, user.Staff.Person.OfficialEmail);
                    notificationService.SendNotification(user.Staff.Person.OfficialEmail, msgBody, "DRC SCMS -- Getting Started", true);
                }
            }
        }

        private string PrepareEmailMsgBody(string name, string email)
        {
            string msgBody = "<html><head><title>DRC Supply Chain Management System - Getting Started</title></head><body style=\"font-family:Verdana; font-size: 12px;\">";
            msgBody += "<h2>SCMS</h2><hr style=\"background: #A7B601; height: 5px; border: none;\" />Dear DRC Staff,";
            msgBody += "<p>Welcome to Supply Chain Management System (SCMS)! Please read this email carefully and save it for your records.</p>";
            msgBody += "<p><b>In order to start using SCMS you are required to do the following;</b></p>";
            msgBody += "<p>Open Firefox (browser) and enter link: <a href=\"http://50.62.136.192:877/\" target=\"_blank\">http://50.62.136.192:877/</a>.</p>";
            msgBody += "<p><font color=\"Blue\">Click on login</font> on the SCMS website</p>";
            msgBody += "<p>In the log in popup,</p>";
            msgBody += "<p>Enter your username: <i style=\"color: Blue;\">{1}</i></p>";
            msgBody += "<p>Enter your password: <i style=\"color: Blue;\">root</i></p>";
            msgBody += "<hr />";
            msgBody += "Upon entering SCMS:";
            msgBody += "<p>Go to <font color=\"Blue\">Administration</font> heading, scroll down to my profile and <font color=\"Blue\">click on my profile</font></p>";
            msgBody += "<p style=\"color: Blue;\">Click on change password</p>";
            msgBody += "<p>Enter old password: <i style=\"color: Blue;\">root</i></p>";
            msgBody += "<p>Enter new password: <i style=\"color: Blue;\">“use minimum 6 letters”</i></p>";
            msgBody += "<p>Confirm your new password: <i style=\"color: Blue;\">“same as above”</i></p>";
            msgBody += "<p>Click on change password –-> <i style=\"color: Green;\">“Your password has now been updated”</i></p>";
            msgBody += "<hr />";
            msgBody += "<p>After change of password, please double check your information on the profile.</p>";
            msgBody += "<p><b>VERY IMPORTANT: verify your signature is correct!</b></p>";
            msgBody += "<hr />";
            msgBody += "<p>If you forget your password, you can reset it at: <a href=\"http://50.62.136.192:877/PasswordReset/\" target=\"_blank\">http://50.62.136.192:877/PasswordReset/</a></p>";
            msgBody += "<p>Please <b>DO NOT</b> reply to this email address because it is primarily used by SCMS to send out system emails. To contact Support, ";
            msgBody += "send an email to <i>logistics-dev@googlegroups.com</i></p>";
            msgBody += "<p>Thank you</p><p><br>SCMS Support.</p></body></html>";

            //replace placeholders
            msgBody = string.Format(msgBody, name, email);
            return msgBody;
        }

        #endregion
    }
}
