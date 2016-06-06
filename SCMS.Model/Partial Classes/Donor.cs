using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SCMS.Model
{
    [MetadataType(typeof(DonorMetaData))]
    public partial class Donor
    {
        public SelectList Countries { get; set; }
    }

    public class DonorMetaData
    {
        [Required]
        public object Name { get; set; }
        [Required]
        public object CountryId { get; set; }
    }
}
