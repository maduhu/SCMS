using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Utils.DTOs
{
    public class FleetRegistrationSummary
    {
        public Guid Id { get; set; }
        public string AssetNumber { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public int ModelYear { get; set; }
        public string NumberPlate { get; set; }
        public Decimal AverageFuelConsumed { get; set; }
    }
}
