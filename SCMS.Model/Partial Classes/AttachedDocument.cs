using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SCMS.Model
{
    [MetadataType(typeof(AttachedDocumentMetaData))]
    public partial class AttachedDocument
    {
        public Model.PurchaseOrder POEntity { get; set; }
        public IEnumerable<Model.AttachedDocument> DocList { get; set; }
        public string Action { get; set; }
    }


    public class AttachedDocumentMetaData
    {
        [Required]
        public object Name { get; set; }
        [Required]
        public object FileType { get; set; }
        [Required]
        public object RefNo { get; set; }
    }
}
