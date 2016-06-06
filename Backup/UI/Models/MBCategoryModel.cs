using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class MBCategoryModel
    {
        public SCMS.Model.MasterBudgetCategory EntityMBCategory { get; set; }

        public Guid Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Number { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        [StringLength(255, MinimumLength = 3, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Model_MBCategoryModel_MBCategory3Characters")]
        public string Description { get; set; }
    }
}