using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Reports;

namespace SCMS.Reports.Budgets
{
    public class ProjectBudgetLine
    {
        public String Line { get; set; }
        public String Description { get; set; }
        public Double Total { get; set; }
        public Double Committed { get; set; }
        public Double ActualPosting { get; set; }
        public Double RemainingFunds { get; set; }
        public Double PercentageSpent { get; set; }
        public BudgetLineType Type { get; set; }
    }
}
