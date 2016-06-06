using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections;
using SCMS.Resource;

namespace SCMS.Model
{
    [MetadataType(typeof(GoodsReceivedNoteMetaData))]
    public partial class GoodsReceivedNote
    {
        public SelectList ApproversList { get; set; }
        public List<POItemsView> POItemz { get; set; }
        public SelectList staffs { get; set; }
        public SelectList SubOffices { get; set; }
        public SelectList PurchaseOrders { get; set; }
        public SelectList Warehouses { get; set; }
        public string SupplierName { get; set; }
        public Model.VStaffDetail ReceivedByStaff { get; set; }
        public bool CanApprove { get; set; }
        // public bool CanEdit { get; set; }
        public string PONo { get; set; }


        //Temprary
        public List<Model.GoodsReceivedNoteItem> ItemColl { get; set; }


    }

    //[Bind(Exclude = "")]
    public class GoodsReceivedNoteMetaData
    {
        [Required]
        [Remote("IsDNNoExist", "GoodsReceivedNote", AdditionalFields="Id")]
        [Editable(true)]
        public object DnNumber { get; set; }
        [Required]
        public object SupplierId { get; set; }
        [Required]
        public object CountryOfficeId { get; set; }
        [Required]
        public object PurchaseOrderId { get; set; }
        [Required]
        public object DeliveredBy { get; set; }
        [Required]
        public object ReceivedBy { get; set; }
        [Required]
        public object IsRejected { get; set; }
        [Required]
        public object IsSubmitted { get; set; }
        [Required]
        public object Verified { get; set; }
        [Required]
        public object DeliverDate { get; set; }
        [Required]
        public object RefNumber { get; set; }
        [Required]
        public object WareHouseId { get; set; }
        [Required]
        public object DeliveryCompany { get; set; }
        [Required]
        public object VehicleRegNumber { get; set; }
        [Required]
        public object DriverLicenseNumber { get; set; }
        [Required]
        public object ReceptionApprovedBy { get; set; }

    }

    public partial class GoodsReceivedNoteItem
    {
        public string ItemName { get { return PurchaseOrderItem.Item != null ? PurchaseOrderItem.Item.Name : string.Empty; } }
        public string Unit { get { return PurchaseOrderItem.Item.UnitOfMeasure != null ? PurchaseOrderItem.Item.UnitOfMeasure.Code : string.Empty; } }
        public int POQty { get { return PurchaseOrderItem != null ? PurchaseOrderItem.Quantity : 0; } }
        public string GRNNo { get { return GoodsReceivedNote != null ? GoodsReceivedNote.RefNumber : string.Empty; } }
        public decimal PreviouslyReceived { get { return PurchaseOrderItem != null ? PurchaseOrderItem.GoodsReceivedNoteItems.Where(g => !g.GoodsReceivedNoteId.Equals(this.GoodsReceivedNoteId)).Sum(p => p.QuantityDelivered) : 0; } }
    }

    public class POItemsView
    {
        public Guid Id { get; set; }
        public Guid POItemId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int QtyOrdered { get; set; }
        [Required(ErrorMessage = "Required Field")]
        public decimal QtyDelivered { get; set; }
        [Required(ErrorMessage = "Required Field")]
        public decimal QtyDamaged { get; set; }
        public decimal PreviouslyReceived { get; set; }
        public string comments { get; set; }
        public string unitOfMessure { get; set; }
        [Required]
        public bool IsInventory { get; set; }
        public IEnumerable<Model.ItemPackage> packs { get; set; }
        public PurchaseOrderItem PurchaseOrderItem { get; set; }
    }
    //http://hellspawndev.blogspot.com/2011/10/using-data-annotations-and-validators.html
    //http://codetunnel.com/blog/post/40/validation-with-entity-framework
    //http://www.asp.net/mvc/tutorials/older-versions/models-%28data%29/validation-with-the-data-annotation-validators-cs
    //http://goneale.com/2009/03/04/using-metadatatype-attribute-with-aspnet-mvc-xval-validation-framework/

    public class GRNModelView
    {
        public Guid Id { get; set; }
        public string RefNo { get; set; }
        public string PONo { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string SupplierName { get; set; }
        public string Office { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
