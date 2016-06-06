using System;
using System.Web;
using System.Web.Security;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.CoreBusinessLogic.Settings;
using SCMS.CoreBusinessLogic.Users;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Security
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ISystemUserService m_SystemUserService;
        private readonly IEncryptionService m_EncryptionService;
        private readonly INotificationService m_NotificationService;

        public AuthenticationService(ISystemUserService systemUserService, IEncryptionService encryptionService, INotificationService notificationService)
        {
            m_SystemUserService = systemUserService;
            m_EncryptionService = encryptionService;
            m_NotificationService = notificationService;
        }

        public bool ValidateCustomer(string email, string password)
        {
            var isValid = false;
            if (email.IsNotNullOrWhiteSpace() && password.IsNotNullOrWhiteSpace())
            {
                var systemUser = m_SystemUserService.GetUserByEmail(email, false);                
                if (systemUser != null && systemUser.Locked == false)
                {
                    var adminEmail = systemUser.Staff.CountrySubOffice.CountryProgramme.AdministratorEmail;
                    var passwordHash = m_EncryptionService.GetSHA256(password, systemUser.PasswordSalt, true);
                    isValid = systemUser.Password.IsNotNullOrWhiteSpace() &&
                              systemUser.Password == passwordHash && systemUser.Active;
                    if (isValid)
                    {
                        systemUser.UserLoginCount = 0;
                        systemUser.LastLoginDate = DateTime.Now;
                        systemUser.LastIpAddress = WebHelper.GetCurrentIpAddress();
                        m_SystemUserService.UpdateUser(systemUser);
                    }
                    else
                    {
                        systemUser.UserLoginCount = systemUser.UserLoginCount + 1;
                        if(systemUser.UserLoginCount > Math.Min(3, SettingsHelper<CommonSettings>.Settings.MaxLoginRetries))
                        {
                            systemUser.Locked = true;
                        }
                        m_SystemUserService.UpdateUser(systemUser);
                        try
                        {
                            if (systemUser.Locked && adminEmail.Trim() != string.Empty)
                            {
                                //m_NotificationService.SendNotification(SettingsHelper<CommonSettings>.Settings.AdminEmail,
                                //    NotificationHelper.LoginAttemptsExceededUserAccountLocked.F(systemUser.Staff.Person.OfficialEmail),
                                //    "Account {0} Locked".F(systemUser.Staff.Person.OfficialEmail));
                                m_NotificationService.SendNotification(adminEmail,
                                    NotificationHelper.LoginAttemptsExceededUserAccountLocked.F(systemUser.Staff.Person.OfficialEmail),
                                    "Account {0} Locked".F(systemUser.Staff.Person.OfficialEmail));
                            }
                        }catch(Exception exception){}
                    }
                }
            }
            return isValid;
        }

        public void SignIn(SystemUser user, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1 ,
                user.Staff.Person.OfficialEmail,
                now,
                now.Add(FormsAuthentication.Timeout),
                createPersistentCookie,
                user.Staff.Person.OfficialEmail,
                FormsAuthentication.FormsCookiePath);

            FormsAuthentication.SetAuthCookie(user.Staff.Person.OfficialEmail, createPersistentCookie);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                             {
                                 HttpOnly = true,
                                 Expires = now.Add(FormsAuthentication.Timeout),
                                 Secure = FormsAuthentication.RequireSSL,
                                 Path = FormsAuthentication.FormsCookiePath
                             };
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}
