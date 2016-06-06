using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Utils.DTOs
{
    public class CompletionCertificateSummary
    {
        public Guid Id { get; set; }
        public string RefNumber { get; set; }
        public string PONumber { get; set; }
        public string ProjectTitle { get; set; }
        public string Office { get; set; }
        public string Constructor { get; set; }
        public string ConfirmedBy { get; set; }
        public string Status { get; set; }
    }
}
