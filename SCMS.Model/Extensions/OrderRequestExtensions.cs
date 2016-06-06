using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class OrderRequestExtensions
    {
        public static IQueryable<OrderRequest> IncludeStaff(this IQueryable<OrderRequest> queryable)
        {
            return ((DbQuery<OrderRequest>)queryable).Include("Staff.Person").Include("Staff.Designation")
                                                     .Include("Staff1.Person").Include("Staff1.Designation")
                                                     .Include("Staff2.Person").Include("Staff2.Designation")
                                                     .Include("Staff3.Person").Include("Staff3.Designation");
        }

        public static IQueryable<OrderRequest> IncludePurchaseOrders(this IQueryable<OrderRequest> queryable)
        {
            return ((DbQuery<OrderRequest>)queryable).Include("PurchaseOrders.PurchaseOrderItems.GoodsReceivedNoteItems.GoodsReceivedNote");

        }

        public static IQueryable<OrderRequest> IncludeCurrency(this IQueryable<OrderRequest> queryable)
        {
            return ((DbQuery<OrderRequest>)queryable).Include("Currency").Include("Currency1");
        }

        public static IQueryable<OrderRequest> IncludeProjectDonor(this IQueryable<OrderRequest> queryable)
        {
            return ((DbQuery<OrderRequest>)queryable).Include("ProjectDonor.Project").Include("ProjectDonor.Donor");
        }

        public static IQueryable<OrderRequest> IncludeOrderRequestItems(this IQueryable<OrderRequest> queryable)
        {
            return ((DbQuery<OrderRequest>)queryable).Include("OrderRequestItems.Item.UnitOfMeasure");
        }

        public static IQueryable<OrderRequest> IncludeLocation(this IQueryable<OrderRequest> queryable)
        {
            return ((DbQuery<OrderRequest>)queryable).Include("Location").Include("Location1");
        }

        public static IQueryable<OrderRequest> IncludeOrderRequestItemsWithProject(this IQueryable<OrderRequest> queryable)
        {
            return ((DbQuery<OrderRequest>)queryable).Include("OrderRequestItems.Item.UnitOfMeasure")
                            .Include("OrderRequestItems.ProjectBudget.BudgetCategory.ProjectDonor.Project");
        }

        public static IQueryable<OrderRequest> IncludeOrderRequestItemsWithPO(this IQueryable<OrderRequest> queryable)
        {
            return ((DbQuery<OrderRequest>)queryable).Include("OrderRequestItems.PurchaseOrderItems.PurchaseOrder");
        }

        public static IQueryable<OrderRequestItem> IncludeItem(this IQueryable<OrderRequestItem> queryable)
        {   
            return ((DbQuery<OrderRequestItem>) queryable).Include("Item.UnitOfMeasure");
        }

        public static IQueryable<OrderRequestItem> IncludeProjectBudget(this IQueryable<OrderRequestItem> queryable)
        {
            return ((DbQuery<OrderRequestItem>)queryable).Include("ProjectBudget");
        }

        public static IQueryable<OrderRequestItem> IncludeProjectDonor(this IQueryable<OrderRequestItem> queryable)
        {
            return ((DbQuery<OrderRequestItem>)queryable).Include("ProjectBudget.BudgetCategory.ProjectDonor");
        }

        public static IQueryable<OrderRequestItem> IncludeOrderRequest(this IQueryable<OrderRequestItem> queryable)
        {
            return ((DbQuery<OrderRequestItem>)queryable).Include("OrderRequest");
        }
    }
}
