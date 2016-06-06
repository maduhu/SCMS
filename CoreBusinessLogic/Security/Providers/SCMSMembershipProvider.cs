using System;
using System.Web.Mvc;
using System.Web.Security;

namespace SCMS.CoreBusinessLogic.Security.Providers
{
    public class SCMSMembershipProvider : MembershipProvider
    {
        private readonly IAuthenticationService m_AuthenticationService;

        public SCMSMembershipProvider()
        {
            m_AuthenticationService = DependencyResolver.Current.Resolve<IAuthenticationService>();
        }

        #region Instance Properties

        public override string ApplicationName
        {
            get { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
            set { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
        }

        public override bool EnablePasswordReset
        {
            get { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new Exception("SCMSMembershipProvider has no implementation for this method."); }
        }

        #endregion

        #region Instance Methods

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion,
            string newPasswordAnswer)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion,
            string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
            out int totalRecords)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
            out int totalRecords)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override string GetPassword(string username, string answer)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override bool UnlockUser(string userName)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new Exception("SCMSMembershipProvider has no implementation for this method.");
        }

        public override bool ValidateUser(string username, string password)
        {   
            return m_AuthenticationService.ValidateCustomer(username, password);
        }

        #endregion
    }
}
