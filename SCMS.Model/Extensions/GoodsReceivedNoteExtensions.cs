using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class GoodsReceivedNoteExtensions
    {
        /// <summary>
        /// Includes supplier
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static IQueryable<GoodsReceivedNote> IncludeSupplier(this IQueryable<GoodsReceivedNote> queryable)
        {
            return ((DbQuery<GoodsReceivedNote>)queryable).Include("Supplier");
        }
        /// <summary>
        /// Includes country suboffices
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static IQueryable<GoodsReceivedNote> IncludeCountrySubOffice(this IQueryable<GoodsReceivedNote> queryable)
        {
            return ((DbQuery<GoodsReceivedNote>)queryable).Include("CountrySubOffice.Location");
        }
        /// <summary>
        /// Includes GRN Items
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static IQueryable<GoodsReceivedNote> IncludeGoodsReceivedNoteItems(this IQueryable<GoodsReceivedNote> queryable)
        {
            return ((DbQuery<GoodsReceivedNote>)queryable).Include("GoodsReceivedNoteItems");
        }
        /// <summary>
        /// Includes GRN Items, PO Items, OR Item, Item, and finally unit of measure
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static IQueryable<GoodsReceivedNote> IncludePurchaseOrderItem(this IQueryable<GoodsReceivedNote> queryable)
        {
            return ((DbQuery<GoodsReceivedNote>)queryable).Include("GoodsReceivedNoteItems.PurchaseOrderItem.OrderRequestItem.Item.UnitOfMeasure")
                                                          .Include("GoodsReceivedNoteItems.PurchaseOrderItem.ProcurementPlanItem.Item.UnitOfMeasure");
        }
        /// <summary>
        /// Includes GRN Items, PO Items and finally PurchaseOrder
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static IQueryable<GoodsReceivedNote> IncludePurchaseOrder(this IQueryable<GoodsReceivedNote> queryable)
        {
            return ((DbQuery<GoodsReceivedNote>)queryable).Include("GoodsReceivedNoteItems.PurchaseOrderItem.PurchaseOrder.PurchaseOrderItems.OrderRequestItem.OrderRequest")
                                                          .Include("GoodsReceivedNoteItems.PurchaseOrderItem.PurchaseOrder.PurchaseOrderItems.ProcurementPlanItem.ProcurementPlan");
        }
        /// <summary>
        /// Includes GRN Items, PO Items, PurchaseOrder, and 
        /// finally Orderrequest from which the GRN was generated
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static IQueryable<GoodsReceivedNote> IncludeOrderRequest(this IQueryable<GoodsReceivedNote> queryable)
        {
            return ((DbQuery<GoodsReceivedNote>)queryable).Include("GoodsReceivedNoteItems.PurchaseOrderItem.PurchaseOrder.OrderRequest")
                                                          .Include("GoodsReceivedNoteItems.PurchaseOrderItem.PurchaseOrder.PurchaseOrderItems.ProcurementPlanItem.ProcurementPlan");
        }

        public static IQueryable<GoodsReceivedNote> IncludeStaff(this IQueryable<GoodsReceivedNote> queryable)
        {
            return ((DbQuery<GoodsReceivedNote>)queryable).Include("Staff.Person").Include("Staff.Designation")
                                                          .Include("Staff1.Person").Include("Staff1.Designation");
        }
    }
}
