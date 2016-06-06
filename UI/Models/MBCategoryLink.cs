using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCMS.Model;

namespace SCMS.UI.Models
{
    public class MBCategoryLink
    {
        public MasterBudgetCategory EntityMasterBudgetCateogry { get; set; }

        public List<BudgetLine> ProjectBudgetLines { get; set; }
    }
}