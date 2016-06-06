using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public class BudgetCheckResult
    {
        public bool FundsAvailable { get; set; }

        public ProjectBudget ProjectBudget { get; set; }

        public decimal AmountChecked { get; set; }

        public decimal AvailableAmount { get; set; }
    }
}
