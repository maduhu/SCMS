using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SCMS.Resource;


namespace SCMS.UI.Models
{
    public class BudgetCategory
    {
        public SCMS.Model.BudgetCategory EntityBudgetCategory { get; set; }

        public string BudgetCategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Model_BudgetCategory_ValidBudgetCategoryMsg")]
        [StringLength(255, MinimumLength = 3, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Model_BudgetCategory_BudgetCategory3Characters")]
        public string Name
        {
            get { return EntityBudgetCategory.Name; }
            set { EntityBudgetCategory.Name = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Number
        {
            get { return EntityBudgetCategory.Number; }
            set { EntityBudgetCategory.Number = value; }
        }

        public string Description
        {
            get { return EntityBudgetCategory.Description; }
            set { EntityBudgetCategory.Description = value; }
        }

        public List<BudgetSubLine> SubLines { get; set; }

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