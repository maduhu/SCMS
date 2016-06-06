using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCMS.UI.Areas.Admin.Models.SystemUser
{
    public class GridSystemUserModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string OfficialEmail { get; set; }
        public bool Active { get; set; }
        public bool Locked { get; set; }
        public string FinancialLimitName { get; set; }
        public string OfficialPhone { get; set; }
    }
}