using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SCMS.Model
{
    [MetadataType(typeof(ProcurementPlanItemMetaData))]
    public partial class ProcurementPlanItem
    {
        public SelectList Items { get; set; }

        public SelectList Currencies { get; set; }

        public SelectList BudgetLines { get; set; }

        public SelectList Locations { get; set; }

        public string ItemName
        {
            get
            {
                return Item != null ? Item.Name : string.Empty;
            }
        }

        public int BalanceNotDelivered
        {
            get
            {
                return (int)(ProcuredAmount - DeliveredAmount);
            }
        }

        public int BalanceNotOrdered
        {
            get
            {
                return Quantity - ProcuredAmount;
            }
        }

        public int ProcuredAmount
        {
            get
            {
                return OrderRequestItems.Where(o => !(o.OrderRequest.IsRejected == true && o.OrderRequest.IsReviewed == true)).Sum(o => (int)o.Quantity)
                    + PurchaseOrderItems.Where(o => o.PurchaseOrder.IsRejected == false).Sum(p => p.Quantity);
            }
        }

        //public int DeliveredAmount
        //{
        //    get
        //    {
        //        return (int)PurchaseOrderItems.Sum(p => p.GoodsReceivedNoteItems.Where(g => g.GoodsReceivedNote.Verified).Sum(g => (g.QuantityDelivered - g.QuantityDamaged)));
        //    }
        //}

        public int QuantityToOrder { get; set; }

        /// <summary>
        /// This is required for Add New function to know which view to render after Add New is done
        /// </summary>
        public bool EditMode { get; set; }
    }

    public class ProcurementPlanItemMetaData
    {
        [Required]
        public object ItemId { get; set; }
        [Required]
        public object Quantity { get; set; }
        [Required]
        public object UnitCost { get; set; }
        [Required]
        public object TotalCost { get; set; }
        [Required]
        public object CurrencyId { get; set; }
        [Required]
        public object BudgetLineId { get; set; }
    }
}
