using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SCMS.Model
{
    [MetadataType(typeof(GeneralLedgerMetaData))]
    public partial class GeneralLedger
    {
        public string DataText 
        {
            get { return string.Format("{0} - {1}", this.Code, this.Description); }
        }
    }

    public class GeneralLedgerMetaData
    {
        [Required]
        public object Code { get; set; }
        [Required]
        public object Description { get; set; }
    }
}
