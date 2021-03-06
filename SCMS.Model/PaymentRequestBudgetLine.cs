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
    
    public partial class PaymentRequestBudgetLine
    {
        public PaymentRequestBudgetLine()
        {
            this.BudgetCommitments = new HashSet<BudgetCommitment>();
            this.BudgetPostings = new HashSet<BudgetPosting>();
            this.PaymentVoucherBudgetLines = new HashSet<PaymentVoucherBudgetLine>();
        }
    
        public System.Guid Id { get; set; }
        public System.Guid PaymentRequestId { get; set; }
        public System.Guid BudgetLineId { get; set; }
        public decimal Amount { get; set; }
        public Nullable<System.Guid> PurchaseOrderItemId { get; set; }
    
        public virtual ICollection<BudgetCommitment> BudgetCommitments { get; set; }
        public virtual ICollection<BudgetPosting> BudgetPostings { get; set; }
        public virtual PaymentRequest PaymentRequest { get; set; }
        public virtual ICollection<PaymentVoucherBudgetLine> PaymentVoucherBudgetLines { get; set; }
        public virtual ProjectBudget ProjectBudget { get; set; }
        public virtual PurchaseOrderItem PurchaseOrderItem { get; set; }
    }
}
