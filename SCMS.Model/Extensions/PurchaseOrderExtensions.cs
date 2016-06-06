using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;

namespace SCMS.Model
{
    public static class PurchaseOrderExtensions
    {
        #region .PurchaseOrder.

        public static IQueryable<PurchaseOrder> IncludeOrderRequest(this IQueryable<PurchaseOrder> queryable)
        {
            return ((DbQuery<PurchaseOrder>)queryable).Include("OrderRequest.ProjectDonor")
                                                      .Include("OrderRequest.Currency");
        }

        public static IQueryable<PurchaseOrder> IncludeStaff(this IQueryable<PurchaseOrder> queryable)
        {
            return ((DbQuery<PurchaseOrder>)queryable).Include("Staff.Person").Include("Staff.Designation")
                                                     .Include("Staff1.Person").Include("Staff1.Designation");
        }

        public static IQueryable<PurchaseOrder> IncludeCurrency(this IQueryable<PurchaseOrder> queryable)
        {
            return ((DbQuery<PurchaseOrder>)queryable).Include("Currency").Include("Currency1");
        }

        public static IQueryable<PurchaseOrder> IncludeProjectDonor(this IQueryable<PurchaseOrder> queryable)
        {
            return ((DbQuery<PurchaseOrder>)queryable).Include("ProjectDonor.Project").Include("ProjectDonor.Donor");
        }

        public static IQueryable<PurchaseOrder> IncludeLocation(this IQueryable<PurchaseOrder> queryable)
        {
            return ((DbQuery<PurchaseOrder>)queryable).Include("Location");
        }

        public static IQueryable<PurchaseOrder> IncludePaymentTerm(this IQueryable<PurchaseOrder> queryable)
        {
            return ((DbQuery<PurchaseOrder>)queryable).Include("PaymentTerm");
        }

        public static IQueryable<PurchaseOrder> IncludeShippingTerm(this IQueryable<PurchaseOrder> queryable)
        {
            return ((DbQuery<PurchaseOrder>)queryable).Include("ShippingTerm");
        }

        public static IQueryable<PurchaseOrder> IncludeSupplier(this IQueryable<PurchaseOrder> queryable)
        {
            return ((DbQuery<PurchaseOrder>)queryable).Include("Supplier");
        }

        public static IQueryable<PurchaseOrder> IncludeTenderingType(this IQueryable<PurchaseOrder> queryable)
        {
            return ((DbQuery<PurchaseOrder>)queryable).Include("TenderingType");
        }

        public static IQueryable<PurchaseOrder> IncludePurchaseOrderItems(this IQueryable<PurchaseOrder> queryable)
        {

            return ((DbQuery<PurchaseOrder>)queryable).Include("PurchaseOrderItems.OrderRequestItem.Item.UnitOfMeasure")
                                                        .Include("PurchaseOrderItems.OrderRequestItem.Item.ItemPackages")
                                                      .Include("PurchaseOrderItems.OrderRequestItem.ProjectBudget.BudgetCategory.ProjectDonor.Project")
                                                      .Include("PurchaseOrderItems.ProjectBudget.BudgetCategory.ProjectDonor.Project")
                                                      .Include("PurchaseOrderItems.OrderRequestItem.OrderRequest.ProjectDonor")
                                                      .Include("PurchaseOrderItems.ProcurementPlanItem.Item.UnitOfMeasure")
                                                      .Include("PurchaseOrderItems.ProcurementPlanItem.Item.ItemPackages")
                                                      .Include("PurchaseOrderItems.ProcurementPlanItem.ProcurementPlan");
        }

        public static IQueryable<PurchaseOrder> IncludePOItemsWithGRNItems(this IQueryable<PurchaseOrder> queryable)
        {
            return ((DbQuery<PurchaseOrder>)queryable).Include("PurchaseOrderItems.GoodsReceivedNoteItems");
        }

        public static IQueryable<PurchaseOrder> IncludeNotifications(this IQueryable<PurchaseOrder> queryable)
        {
            return ((DbQuery<PurchaseOrder>)queryable).Include("Notifications");
        }

        #endregion

        #region .PurchaseOrderItem.

        public static IQueryable<PurchaseOrderItem> IncludeProjectBudget(this IQueryable<PurchaseOrderItem> queryable)
        {
            return ((DbQuery<PurchaseOrderItem>)queryable).Include("ProjectBudget");
        }

        public static IQueryable<PurchaseOrderItem> IncludeProjectDonor(this IQueryable<PurchaseOrderItem> queryable)
        {
            return ((DbQuery<PurchaseOrderItem>)queryable).Include("ProjectBudget.BudgetCategory.ProjectDonor");
        }

        public static IQueryable<PurchaseOrderItem> IncludePurchaseOrder(this IQueryable<PurchaseOrderItem> queryable)
        {
            return ((DbQuery<PurchaseOrderItem>)queryable).Include("PurchaseOrder");
        }

        public static IQueryable<PurchaseOrderItem> IncludeOrderRequestItem(this IQueryable<PurchaseOrderItem> queryable)
        {
            return ((DbQuery<PurchaseOrderItem>)queryable).Include("OrderRequestItem.ProjectBudget.BudgetCategory.ProjectDonor.Project")
                                                          .Include("OrderRequestItem.Item.UnitOfMeasure");
        }

        public static IQueryable<PurchaseOrderItem> IncludeProcurementPlanItem(this IQueryable<PurchaseOrderItem> queryable)
        {
            return ((DbQuery<PurchaseOrderItem>)queryable).Include("ProcurementPlanItem.ProjectBudget.BudgetCategory.ProjectDonor.Project")
                                                          .Include("ProcurementPlanItem.Item.UnitOfMeasure")
                                                          .Include("ProcurementPlanItem.ProcurementPlan.ProjectDonor");
        }

        #endregion
    }
}
