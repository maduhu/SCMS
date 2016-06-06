using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Utils.DTOs
{
    public class RequestForPaymentSummary
    {
        public Guid Id { get; set; }
        public string RFPNo { get; set; }
        public string Supplier { get; set; }
        public string PONo { get; set; }
        public string PN { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
