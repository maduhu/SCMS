using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCMS.UI.Models
{
    public class MasterBudgetHeader
    {
        public string CurrencyId { get; set; }

        public System.Web.Mvc.SelectList Currencies { get; set; }

        public bool IncludeProjection { get; set; }

        public List<SCMS.Model.MBCategory> MBCList { get; set; }
    }
}