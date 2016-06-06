using System.Collections.Generic;

namespace SCMS.CoreBusinessLogic.Users
{
    public class PasswordChangeResult
    {
        public IList<string> Errors { get; set; }

        public PasswordChangeResult()
        {
            this.Errors = new List<string>();
        }

        public bool Success
        {
            get { return (this.Errors.Count == 0); }
        }

        public void AddError(string error)
        {
            this.Errors.Add(error);
        }
    }

    public class PINChangeResult
    {
        public IList<string> Errors { get; set; }

        public PINChangeResult()
        {
            this.Errors = new List<string>();
        }

        public bool Success
        {
            get { return (this.Errors.Count == 0); }
        }

        public void AddError(string error)
        {
            this.Errors.Add(error);
        }
    }
}
