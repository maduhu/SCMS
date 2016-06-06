using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SCMS.Model
{
    [MetadataType(typeof(FinanceLimitMetaData))]
    public partial class FinanceLimit { }

    public class FinanceLimitMetaData
    {
        [Required]
        public object Name { get; set; }
        [Required]
        public object Limit { get; set; }
    }

}
