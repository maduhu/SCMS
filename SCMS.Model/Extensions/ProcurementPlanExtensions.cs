using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class ProcurementPlanExtensions
    {
        public static IQueryable<ProcurementPlan> IncludeCountrySubOffice(this IQueryable<ProcurementPlan> queryable)
        {
            return ((DbQuery<ProcurementPlan>)queryable).Include("CountrySubOffice");
        }

        public static IQueryable<ProcurementPlan> IncludeProjectDonor(this IQueryable<ProcurementPlan> queryable)
        {
            return ((DbQuery<ProcurementPlan>)queryable).Include("ProjectDonor.Project").Include("ProjectDonor.Donor")
                                                        .Include("ProjectDonor.Staff.Person").Include("ProjectDonor.Staff.Designation");
        }

        public static IQueryable<ProcurementPlan> IncludeStaff(this IQueryable<ProcurementPlan> queryable)
        {
            return ((DbQuery<ProcurementPlan>)queryable).Include("Staff.Person").Include("Staff.Designation")
                                                        .Include("Staff1.Person").Include("Staff1.Designation")
                                                        .Include("Staff2.Person").Include("Staff2.Designation")
                                                        .Include("Staff3.Person").Include("Staff3.Designation")
                                                        .Include("Staff4.Person").Include("Staff4.Designation");
        }

        public static IQueryable<ProcurementPlan> IncludeProcurementPlanItems(this IQueryable<ProcurementPlan> queryable)
        {
            return ((DbQuery<ProcurementPlan>)queryable).Include("ProcurementPlanItems.Item.UnitOfMeasure")
                                                        .Include("ProcurementPlanItems.Currency")
                                                        .Include("ProcurementPlanItems.ProjectBudget")
                                                        .Include("ProcurementPlanItems.OrderRequestItems.OrderRequest")
                                                        .Include("ProcurementPlanItems.PurchaseOrderItems.PurchaseOrder")
                                                        .Include("ProcurementPlanItems.PurchaseOrderItems.GoodsReceivedNoteItems.GoodsReceivedNote");

        }

        public static IQueryable<ProcurementPlan> IncludeNotifications(this IQueryable<ProcurementPlan> queryable)
        {
            return ((DbQuery<ProcurementPlan>)queryable).Include("Notifications");
        }

        public static IQueryable<ProcurementPlanItem> IncludeItem(this IQueryable<ProcurementPlanItem> queryable)
        {
            return ((DbQuery<ProcurementPlanItem>)queryable).Include("Item.UnitOfMeasure").Include("OrderRequestItems.OrderRequest");
        }

        public static IQueryable<ProcurementPlanItem> IncludeOrderRequest(this IQueryable<ProcurementPlanItem> queryable)
        {
            return ((DbQuery<ProcurementPlanItem>)queryable).Include("OrderRequestItems.OrderRequest");
        }

        public static IQueryable<ProcurementPlanItem> IncludePurchaseOrder(this IQueryable<ProcurementPlanItem> queryable)
        {
            return ((DbQuery<ProcurementPlanItem>)queryable).Include("PurchaseOrderItems.PurchaseOrder");
        }

        public static IQueryable<ProcurementPlanItem> IncludeProjectBudget(this IQueryable<ProcurementPlanItem> queryable)
        {
            return ((DbQuery<ProcurementPlanItem>)queryable).Include("ProjectBudget");
        }

        public static IQueryable<ProcurementPlanItem> IncludeProcurementPlan(this IQueryable<ProcurementPlanItem> queryable)
        {
            return ((DbQuery<ProcurementPlanItem>)queryable).Include("ProcurementPlan");
        }

    }
}
