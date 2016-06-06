using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class GoodsReceivedNoteItemExtensions
    {
        public static IQueryable<GoodsReceivedNoteItem> IncludePurchaseOrder(this IQueryable<GoodsReceivedNoteItem> queryable)
        {
            return ((DbQuery<GoodsReceivedNoteItem>)queryable).Include("PurchaseOrderItem.PurchaseOrder.Currency");
        }

        public static IQueryable<GoodsReceivedNoteItem> IncludeGoodsReceivedNote(this IQueryable<GoodsReceivedNoteItem> queryable)
        {
            return ((DbQuery<GoodsReceivedNoteItem>)queryable).Include("GoodsReceivedNote");
        }

        public static IQueryable<GoodsReceivedNoteItem> IncludeOrderRequestItem(this IQueryable<GoodsReceivedNoteItem> queryable)
        {
            return ((DbQuery<GoodsReceivedNoteItem>)queryable).Include("PurchaseOrderItem.OrderRequestItem.Item.ItemClassification")
                                                              .Include("PurchaseOrderItem.OrderRequestItem.Item.UnitOfMeasure")
                                                              .Include("PurchaseOrderItem.OrderRequestItem.Item.ItemCategory")
                                                              .Include("PurchaseOrderItem.OrderRequestItem.OrderRequest.ProjectDonor.Donor")
                                                              .Include("PurchaseOrderItem.OrderRequestItem.OrderRequest.ProjectDonor.Project");
        }

        public static IQueryable<GoodsReceivedNoteItem> IncludeProcurementPlanItem(this IQueryable<GoodsReceivedNoteItem> queryable)
        {
            return ((DbQuery<GoodsReceivedNoteItem>)queryable).Include("PurchaseOrderItem.ProcurementPlanItem.Item.ItemClassification")
                                                              .Include("PurchaseOrderItem.ProcurementPlanItem.Item.UnitOfMeasure")
                                                              .Include("PurchaseOrderItem.ProcurementPlanItem.Item.ItemCategory")
                                                              .Include("PurchaseOrderItem.ProcurementPlanItem.ProcurementPlan.ProjectDonor.Donor")
                                                              .Include("PurchaseOrderItem.ProcurementPlanItem.ProcurementPlan.ProjectDonor.Project");
        }

        public static IQueryable<GoodsReceivedNoteItem> IncludePurchaseOrderItem(this IQueryable<GoodsReceivedNoteItem> queryable)
        {
            return ((DbQuery<GoodsReceivedNoteItem>)queryable).Include("PurchaseOrderItem.GoodsReceivedNoteItems");

        }

        public static IQueryable<GoodsReceivedNoteItem> IncludeAssets(this IQueryable<GoodsReceivedNoteItem> queryable)
        {
            return ((DbQuery<GoodsReceivedNoteItem>)queryable).Include("Assets");
        }

        public static IQueryable<GoodsReceivedNoteItem> IncludeBinItems(this IQueryable<GoodsReceivedNoteItem> queryable)
        {
            return ((DbQuery<GoodsReceivedNoteItem>)queryable).Include("BinItems");
        }
    }
}
