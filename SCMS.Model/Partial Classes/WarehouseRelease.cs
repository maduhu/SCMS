using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SCMS.Model
{
    [MetadataType(typeof(WarehouseReleaseMetaData))]
    public partial class WarehouseRelease
    {
        public bool CanEdit { get; set; }
        public SelectList Staffs { get; set; }
        public SelectList Warehouses { get; set; }
        public SelectList ApproversList { get; set; }
        public string RequestorName { get; set; }
        public bool CanApprove { get; set; }
    }

    public class WarehouseReleaseMetaData
    {
        [Required]
        public object ReceivedBy { get; set; }
        [Required]
        public object ApprovedBy { get; set; }
        [Required]
        public object WareHouseId { get; set; }
    }

    [MetadataType(typeof(WarehouseReleaseItemMetaData))]
    public partial class WarehouseReleaseItem
    {
        public SelectList Items { get; set; }

        public string RefNoAndItemQTY { get { return WarehouseRelease.RefNumber + " (" + Quantity.ToString("##,###") + ")"; } }
    }

    public class WarehouseReleaseItemMetaData
    {
        //[Required]
        public object QuantityDamaged { get; set; }
        //[Required]
        public object QuantityReceived { get; set; }
        //[Required]
        public object Quantity { get; set; }
    }

    public class RejectWRO
    {
        [Required]
        public Guid Id { get; set; }

        public string RefNumber { get; set; }
        [Required]
        public string RejectedReviewRemarks { get; set; }
    }

    public class CheckAvailableQty
    {
        public Guid InventoryId { get; set; }

        public Int64 releasedQty { get; set; }

        public Guid wrnId { get; set; }

        public Guid? assetid { get; set; }
    }

    public class ROModelView
    {
        public Guid Id { get; set; }
        public string RefNumber { get; set; }
        public string FirstItem { get; set; }
        public string Requestor { get; set; }
        public string Recepient { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
