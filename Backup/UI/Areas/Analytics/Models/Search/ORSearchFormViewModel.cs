using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SCMS.UI.Areas.Analytics.Models.Search
{
    public class ORSearchFormViewModel
    {
        public ORSearchFormViewModel()
        {
            this.startDate = this.endDate = DateTime.Now;
        }

        [Required]
        public DateTime startDate { get; set; }

        [Required]
        public DateTime endDate { get; set; }
    }
}