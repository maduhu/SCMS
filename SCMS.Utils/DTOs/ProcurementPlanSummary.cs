using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Utils.DTOs
{
    public class ProcurementPlanSummary
    {
        public Guid Id { get; set; }
        public string RefNumber { get; set; }
        public string ProjectTitle { get; set; }
        public string ProjectNumber { get; set; }
        public string Donor { get; set; }
        public string PrepOffice { get; set; }
        public DateTime DatePrepared { get; set; }
    }
}