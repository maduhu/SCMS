using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCMS.Model;
using System.ComponentModel.DataAnnotations;

namespace SCMS.UI.Models
{
    public class SubLine
    {
        public SCMS.Model.ProjectBudget EntityBudgetSubLine { get; set; }

        public string SubLineId { get; set; }

        public string BudgetLineId { get; set; }

        public string LineNumber
        {
            get { return EntityBudgetSubLine.LineNumber; }
            set { EntityBudgetSubLine.LineNumber = value; }
        }

        public string Description
        {
            get { return EntityBudgetSubLine.Description; }
            set { EntityBudgetSubLine.Description = value; }
        }
        public string BudgetCategoryId { get; set; }

        public string BudgetCategoryNumber { get; set; }

        public bool AddToBudget { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal TotalBudget { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? TotalCommitted { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? TotalPosted { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? RemainingBalance { get; set; }
    }
}