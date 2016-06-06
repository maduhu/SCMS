using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Utils.DTOs
{
    public class GeneralInventorySummary
    {
        public Guid Id { get; set; }
        public string Classification { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public long Quantity { get; set; }
    }
}
