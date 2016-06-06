using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SCMS.Model
{
    class GeneralSettingsMetaData
    {
    }

    [MetadataType(typeof(CheckListCategoryMetaData))]
    public partial class CheckListCategory { }

    public class CheckListCategoryMetaData
    {
        [Required]
        public object Name { get; set; }
    }

    [MetadataType(typeof(ServiceCheckListMetaData))]
    public partial class ServiceCheckList
    {

        public Guid[] ServiceCList { get; set; }
        public Guid FleetId { get; set; }
        public Guid? gInfoId { get; set; }
    }

    public class ServiceCheckListMetaData
    {
        [Required]
        public object Name { get; set; }
        [Required]
        public object ServiceCategoryId { get; set; }
    }
}
