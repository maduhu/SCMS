using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SCMS.Model
{
    [MetadataType(typeof(WayBillMetaData))]
    public partial class WayBill { }

    public class WayBillMetaData
    {
        [Required]
        public object RefNumber { get; set; }
        [Required]
        public object WRNId { get; set; }
        [Required]
        public object IssuingOffice { get; set; }
        [Required]
        public object IssuerWarehouse { get; set; }
        [Required]
        public object DestinationOffice { get; set; }
        [Required]
        public object DestinationWarehouse { get; set; }
        [Required]
        public object Consignee { get; set; }
        [Required]
        public object DRCVehicleRegNo { get; set; }
        [Required]
        public object DRCVehicleMileage { get; set; }
        [Required]
        public object DRCVehicleContactNo { get; set; }
        [Required]
        public object PreparedBy { get; set; }

    }
}
