using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SCMS.Model
{
    [MetadataType(typeof(BinMetaData))]
    public partial class Bin
    {
        public SelectList ORequests { get; set; }
        public SelectList ORItemsItems { get; set; }
        public SelectList Projects { get; set; }
        public SelectList ItemPackages { get; set; }
        public SelectList Warehouses { get; set; }
        public SelectList PPlans { get; set; }
        public SelectList PPlansItems { get; set; }
        public bool CanEdit { get; set; }
        public List<BinItems> BinItemz { get; set; }

        public Guid ItemId { get; set; }
        public Guid ProjectId { get; set; }
        public string RefLable { get; set; }
        public string RefItemLable { get; set; }
        public string BinType { get; set; }
        public Item Item { get { return OrderRequestItem != null ? OrderRequestItem.Item : ProcurementPlanItem.Item; } }
        public ProjectDonor ProjecDonor { get { return OrderRequestItem != null ? OrderRequestItem.OrderRequest.ProjectDonor : ProcurementPlanItem.ProcurementPlan.ProjectDonor; } }
        public string BinNoAndItem { get { return Item != null ? RefNumber + " (" + Item.Name + ")" : RefNumber; } }
        public string PPOrORNo { get { return OrderRequestItem != null ? OrderRequestItem.OrderRequest.RefNumber : ProcurementPlanItem.ProcurementPlan.RefNumber; } }
    }

    public class BinMetaData
    {
        [Required]
        public object BinType { get; set; }
        [Required]
        public object RefNumber { get; set; }
        [Required]
        public object QTY { get; set; }
        [Required]
        public object ItemId { get; set; }
        [Required]
        public object PackageId { get; set; }
        [Required]
        public object WareHouseId { get; set; }
    }

    [MetadataType(typeof(BinItemMetaData))]
    public partial class BinItem
    {
        public SelectList GRNItems { get; set; }
        public SelectList BinCards { get; set; }
        public SelectList WHRItems { get; set; }
        public Int64? PreviouslyReceved { get; set; }
        public Int64? PreviouslyReleased { get; set; }
    }

    public class BinItemMetaData
    {
        [Required]
        [Remote("IsQTYInRange", "BinCard", AdditionalFields = "PreviouslyReceved,GoodsReceivedNoteItemId")]
        public object QTYReceived { get; set; }
        [Required]
        [Remote("IsQTYIssuedInRange", "BinCard", AdditionalFields = "WarehouseReleaseItemId,PreviouslyReleased")]
        public object QTYIssued { get; set; }
    }

    public class StockSummary
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Guid ProjectDonorId { get; set; }
        public string IssueDate { get; set; }
        public Int64 OpengStock { get; set; }
        public Int64 ReceivedStock { get; set; }
        public Int64 IssueStock { get; set; }
        public Int64 BalStock { get; set; }
    }

    public class BinCard
    {
        public Guid Id { get; set; }
        public Int64 QTY { get; set; }
        public string RefNumber { get; set; }
        public string Description { get; set; }
        public string ItemName { get; set; }
        public string WareHouseName { get; set; }
        public string WHDemarcation { get; set; }

        public string IssueDate { get; set; }
        public Guid ItemId { get; set; }
        public Guid RecordId { get; set; }
    }

    public class BinItems
    {
        public Guid Id { get; set; }
        public DateTime IssueDate { get; set; }
        public string DnNumber { get; set; }
        public string RefNumber { get; set; }
        public Int64 QTYReceived { get; set; }
        public Int64 QTYIssued { get; set; }
        public Int64 BalanceStock { get; set; }
        public string Remarks { get; set; }
    }
}
