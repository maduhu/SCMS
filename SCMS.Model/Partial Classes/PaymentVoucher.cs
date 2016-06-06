using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SCMS.Model
{
    [MetadataType(typeof(PaymentVoucherMetaData))]
    public partial class PaymentVoucher
    {
        public VStaffDetail PreparedByStaff { get; set; }
        public VStaffDetail AuthourisedByStaff { get; set; }
        public string AmountInWords { get; set; }
        public bool CanEdit { get; set; }
    }

    public class PaymentVoucherMetaData
    {
        [Required]
        public object PaymentRequestId { get; set; }
        [Required]
        public object ProjectBankAccount { get; set; }
    }

    [MetadataType(typeof(PaymentVoucherBudgetLineMetaData))]
    public partial class PaymentVoucherBudgetLine { }

    public class PaymentVoucherBudgetLineMetaData
    {
        [Required]
        public object GeneralLedgerId { get; set; }
        [Required]
        public object Description { get; set; }
    }
}
