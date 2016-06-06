using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class BudgetSubLine
    {
        public SCMS.Model.ProjectBudget EntityBudgetSubLine { get; set; }

        public string SubLineId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string LineNumber
        {
            get { return EntityBudgetSubLine.LineNumber; }
            set { EntityBudgetSubLine.LineNumber = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Model_BudgetSubline_ValidBudgetSublineMsg")]
        [StringLength(255, MinimumLength = 3, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Model_BudgetSubline_BudgetSubline3Characters")]
        public string Description
        {
            get { return EntityBudgetSubLine.Description; }
            set { EntityBudgetSubLine.Description = value; }
        }
        public string BudgetCategoryId { get; set; }

        public string BudgetCategoryNumber { get; set; }

        public string BudgetCategoryDesc { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        [DataType(DataType.Currency, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_OnlyNumericRequired")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal TotalBudget { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? TotalCommitted { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? TotalPosted { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? RemainingBalance { get; set; }

        [Range(0, 100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Budget_ProjectDonor_MustBePercentage")]
        public double? OverrunAdjustment
        {
            get { return EntityBudgetSubLine.OverrunAdjustment; }
            set { EntityBudgetSubLine.OverrunAdjustment = value; }
        }
    }
}