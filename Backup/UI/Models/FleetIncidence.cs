using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCMS.Model;

namespace SCMS.UI.Models
{
    public class FleetIncidence
    {
        public FleetMajorIncidence EntityFleetInc { get; set; }
    }


    public class ViewFleetIncidence
    {
        public FleetMajorIncidence EntityFleetInc { get; set; }
    }
}