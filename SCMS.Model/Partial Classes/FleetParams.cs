using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SCMS.Model
{
    class FleetParams
    {
    }


    [MetadataType(typeof(VehicleMakeMetaData))]
    public partial class VehicleMake { }

    public class VehicleMakeMetaData
    {
        [Required]
        public object Name { get; set; }
    }


    [MetadataType(typeof(VehicleModelMetaData))]
    public partial class VehicleModel { }

    public class VehicleModelMetaData
    {
        [Required]
        public object MakeId { get; set; }
        [Required]
        public object Name { get; set; }
    }


    [MetadataType(typeof(InsuranceTypeMetaData))]
    public partial class InsuranceType { }

    public class InsuranceTypeMetaData
    {
        [Required]
        public object Name { get; set; }
    }


    [MetadataType(typeof(FleetEquipmentMetaData))]
    public partial class FleetEquipment { }

    public class FleetEquipmentMetaData
    {
        [Required]
        public object EquipmentName { get; set; }
    }


    [MetadataType(typeof(FleetMajorIncidenceMetaData))]
    public partial class FleetMajorIncidence { }

    public class FleetMajorIncidenceMetaData
    {
        [Required]
        public object Name { get; set; }
    }

    [MetadataType(typeof(DutyTypeMetaData))]
    public partial class DutyType { }

    public class DutyTypeMetaData
    {
        [Required]
        public object Name { get; set; }
    }
}
