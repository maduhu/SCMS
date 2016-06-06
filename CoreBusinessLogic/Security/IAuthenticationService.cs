using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Security
{
    public interface IAuthenticationService
    {
        bool ValidateCustomer(string email, string password);
        void SignIn(SystemUser user, bool createPersistentCookie);
        void SignOut();
    }
}
