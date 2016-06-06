using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SCMS.UI.Areas.Admin.Models.SystemUser
{
    public class SwitchCPModel
    {
        [Required]
        public Guid CountryProgrammeId { get; set; }
        [Required]
        public Guid SubOfficeId { get; set; }

        public SelectList CountryProgrammes { get; set; }

        public SelectList SubOffices { get; set; }
    }
}