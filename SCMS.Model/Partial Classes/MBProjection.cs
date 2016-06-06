using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SCMS.Model
{
    public class MBProjection
    {
        public AnnualCostProjection EntityProjection { get; set; }
        [Required(ErrorMessage="Amount must be entered.")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal AmountProjected { get; set; }
        [Required(ErrorMessage="MB Category must be selected")]
        public Guid MasterBudgetCategoryId { get; set; }
        public System.Web.Mvc.SelectList MasterBudgetCategories { get; set; }
        [Required(ErrorMessage="Currency must be selected")]
        public Guid? CurrencyId { get; set; }
        public System.Web.Mvc.SelectList Currencies { get; set; }
        public Guid CountryProgrammeId { get; set; }
        public Guid Id { get; set; }
    }
}
