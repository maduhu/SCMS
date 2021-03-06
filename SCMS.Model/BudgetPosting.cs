//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCMS.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class BudgetPosting
    {
        public BudgetPosting()
        {
            this.Rebookings = new HashSet<Rebooking>();
        }
    
        public System.Guid Id { get; set; }
        public decimal AmountPosted { get; set; }
        public System.DateTime DatePosted { get; set; }
        public Nullable<System.Guid> PostedBy { get; set; }
        public Nullable<System.Guid> RFPBudgetLineId { get; set; }
    
        public virtual PaymentRequestBudgetLine PaymentRequestBudgetLine { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual ICollection<Rebooking> Rebookings { get; set; }
    }
}
