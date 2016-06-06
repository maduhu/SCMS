using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Utils.DTOs
{
    public class TenderAnalysisSummary
    {
        public Guid Id { get; set; }
        public string RefNo { get; set; }
        public string PreparedBy { get; set; }
        public string OrderRefNumber { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
