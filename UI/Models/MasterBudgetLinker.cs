using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCMS.UI.Models
{
    public class MasterBudgetLinker
    {
        public List<Category> BudgetCaterogies { get; set; }

        public List<MBCategoryLink> MasterBudgetCategories { get; set; }

        public string Id { get; set; }
    }
}