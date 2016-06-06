using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Utils.DTOs
{
    public class OrderRequestSummary
    {
        public Guid Id { get; set; }
        public string RefNumber { get; set; }
        public string FirstItem { get; set; }
        public string ProjectNum { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public Guid CountryProgrammeId { get; set; }
        public string Donor { get; set; }
        public string PrepStaffNames { get; set; }
        public string Requestor { get; set; } // same as PrepStaffNames
        public string TotalValue { get; set; }
        public Decimal ORValue { get; set; } // same as TotalValue
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }

    }


    public class OrderRequestPagePacket
    {
        public List<OrderRequestSummary> Orders { get; set; }
        public Int32 TotalOrders { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

}
