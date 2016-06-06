using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SCMS.Resource;

namespace SCMS.Model
{
    [MetadataType(typeof(GoodsIssuedVoucherMetaData))]
    public partial class GoodsIssuedVoucher
    {
        public SelectList ReleaseOrders { get; set; }
        public SelectList Staffs { get; set; }
        public List<GivItemz> ROItems { get; set; }
        public string RONumber { get { return WarehouseRelease.RefNumber; } }
        public string Status { get { return IsSubmitted ? Resources.Global_String_StatusCR : Resources.Global_String_StatusNEW; } }

    }

    public class GoodsIssuedVoucherMetaData
    {
        [Required]
        public object WareHouseReleaseId { get; set; }
        [Required]
        public object RefNumber { get; set; }
        [Required]
        public object Receivedby { get; set; }
        [Required]
        public object Consignee { get; set; }
    }

    public class GivItemz
    {
        public Guid GIVItemId { get; set; }
        public Guid ROItemId { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public Int64 PreviouslyReceived { get; set; }
        [Required]
       // [Remote("IsQTYInRange", "GoodsIssuedVoucher", AdditionalFields = "QTYReleased", ErrorMessage = "Qty received cant exceed or be less than QtyReleased")]
        public Int64 QTYReceived { get; set; }
        public Int64 QTYReleased { get; set; }
        public string Remarks { get; set; }
        public bool IsRemoved { get; set; }

        //[Required]
        //[StringLength(6, MinimumLength = 3)]
        //[Remote("IsUID_Available", "Validation")]
        //[RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed.")]
        //[Editable(true)]
        //public override string MyProperty { get; set; }
        //http://edspencer.me.uk/2011/07/19/using-asp-net-mvc-remote-validation-the-real-world-additionalfields/
    }

    //http://www.a2zdotnet.com/View.aspx?Id=198#.UYIqg0rLtHk
}
