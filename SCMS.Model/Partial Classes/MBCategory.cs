using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SCMS.Model
{
    public class MBCategory
    {
        public MasterBudgetCategory EntityBudgetCategory { get; set; }
        public GeneralLedger EntityGeneralLedger { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal TotalBudget { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? TotalCommitted { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? TotalPosted { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? RemainingBalance { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public float? PercentageSpent { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? CostProjection { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? Surplus { get; set; }
        public List<MasterBudgetProject> Projects { get; set; }
    }
}
