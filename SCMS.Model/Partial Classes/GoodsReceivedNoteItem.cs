using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SCMS.Model
{
    [MetadataType(typeof(GoodsReceivedNoteItemMetaData))]
    public partial class GoodsReceivedNoteItem
    {
        public string GRNNoAndQtyDelivered { get { return GoodsReceivedNote != null ? GoodsReceivedNote.RefNumber + " (" + QuantityDelivered + ")" : QuantityDelivered.ToString(); } }
    }

    public class GoodsReceivedNoteItemMetaData
    {
        [Required(ErrorMessage = "Required Field")]
        public decimal QuantityDelivered { get; set; }
        [Required(ErrorMessage = "Required Field")]
        public decimal QuantityDamaged { get; set; }
    }
}
