namespace SCMS.CoreBusinessLogic.Users
{
    public class ChangePasswordRequest
    {
        public string Email { get; set; }
        public bool ValidateRequest { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }

        public ChangePasswordRequest(string email, bool validateRequest, string newPassword, string oldPassword = "")
        {
            Email = email;
            ValidateRequest = validateRequest;
            NewPassword = newPassword;
            OldPassword = oldPassword;
        }
    }

    public class ChangePINRequest
    {
        public string Email { get; set; }
        public bool ValidateRequest { get; set; }
        public string NewPIN { get; set; }
        public string OldPIN { get; set; }

        public ChangePINRequest(string email, bool validateRequest, string newPIN, string oldPIN = "")
        {
            Email = email;
            ValidateRequest = validateRequest;
            NewPIN = newPIN;
            OldPIN = oldPIN;
        }
    }
}
