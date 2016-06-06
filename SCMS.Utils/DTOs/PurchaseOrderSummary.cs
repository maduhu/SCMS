using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Utils.DTOs
{
    public class PurchaseOrderSummary
    {
        public Guid Id { get; set; }
        public string PONumber { get; set; }
        public DateTime RequestDate { get; set; }
        public string OrderRequestRefNumber { get; set; }
        public string Supplier { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal POValue { get; set; }
        public string Status { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime StatusDate { get; set; }
        public string TotalValue { get; set; }
        public string Requester { get; set; }
        public string Authorizer { get; set; }
    }
}
