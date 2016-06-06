using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Utils.DTOs
{
    public class WarehouseReleaseOrderSummary
    {
        public Guid Id { get; set; }
        public string WRONo { get; set; }
        public string Item { get; set; }
        public string Requestor { get; set; }
        public string Recipient { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
