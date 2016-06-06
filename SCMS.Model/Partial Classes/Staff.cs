using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public partial class Staff
    {
        public string StaffName
        {
            get
            {
                return Person != null ? string.Format("{0} {1}", Person.FirstName, Person.OtherNames) : string.Empty;
            }
        }

        public string StaffDesignation
        {
            get
            {
                return Designation != null ? Designation.Name : string.Empty;
            }
        }

        public byte[] StaffSignature
        {
            get
            {
                return Person != null ? Person.SignatureImage : null;
            }
        }
    }
}
