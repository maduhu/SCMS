using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SCMS.Model
{
    [MetadataType(typeof(TenderingTypeMetaData))]
    public partial class TenderingType
    {
        public SelectList Currencies { get; set; }
    }

    public class TenderingTypeMetaData
    {
        [Required]
        public object Name { get; set; }
        [Required]
        public object MinValue { get; set; }
        [Required]
        public object MaxValue { get; set; }
        [Required]
        public object CurrencyId { get; set; }
    }
}
