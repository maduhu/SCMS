using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace SCMS.UI.Models
{
    public class DashBoard
    {
    }

    public class DocsDashBoard
    {
        public int OrCreated { get; set; }
        public int OrApproved { get; set; }
        public int OrOngoing { get; set; }
        public decimal OrTotalValue { get; set; }

        public int TaCreated { get; set; }
        public int TaApproved { get; set; }
        public int TaOngoing { get; set; }
        public decimal TaTotalValue { get; set; }


        public int PoCreated { get; set; }
        public int PoApproved { get; set; }
        public int PoOngoing { get; set; }
        public decimal PoTotalValue { get; set; }

    }

}