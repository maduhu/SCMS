using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCMS.Model;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class Project
    {
        public SCMS.Model.Project EntityProject { get; set; }

        public string Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string ProjectNumber { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        [StringLength(255, MinimumLength = 3, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Model_Project_ProjectName3Characters")]
        public string ProjectName
        {
            get { return EntityProject.Name; }
            set { EntityProject.Name = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string DonorId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string CurrencyId { get; set; }

        public System.Web.Mvc.SelectList Donors { get; set; }

        public System.Web.Mvc.SelectList Currencies { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public Guid ProjectManagerId { get; set; }

        public SelectList StaffList { get; set; }

        [Range(0, 100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Budget_ProjectDonor_MustBePercentage")]
        public double? OverrunAdjustment { get; set; }
    }

    public class DocPreparer
    {
        public DocumentPreparer EntityDocPreparer { get; set; }

        [Required]
        public Guid PreparerId
        {
            get { return EntityDocPreparer.PreparerId; }
            set { EntityDocPreparer.PreparerId = value; }
        }

        public SelectList StaffList { get; set; }
    }
}