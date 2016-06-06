using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCMS.UI.Models
{
    public class BudgetLineCheck
    {
        public string BudgetLineId { get; set; }
        public string CurrencyId { get; set; }
        public decimal TotalPrice { get; set; }

        //Purchase Order we are checking balance for. We should exclude underlying OR commitments from check
        public Guid PurchaseOrderId { get; set; }
        //RFP we are checking balance for. We should exclude underlying PO commitments from check
        public Guid RFPId { get; set; }
        //OR Item we are adding to PO. We should exclude underlying commits for this OR Item from check
        public Guid OrItemId { get; set; }
        //Document Code and Document Id
        public string DocumentCode { get; set; }
        public Guid DocumentId { get; set; }
    }
    
    /// <summary>
    /// This object helps with the currency conversion at the UI e.g. when changing MB Currency during review, this object
    /// contains the attributes needed for the convertion i.e. the local currency, the foreign currency and the amount to 
    /// be converted.
    /// </summary>
    public class CurrencyConvert
    {
        public string LocalCurrencyId { get; set; }
        public string FXCurrencyId { get; set; }
        public decimal Amount { get; set; }
    }
}