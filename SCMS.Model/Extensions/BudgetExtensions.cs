using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;

namespace SCMS.Model
{
    public static class BudgetExtensions
    {
        #region .BudgetCommitment.

        public static IQueryable<BudgetCommitment> IncludeOrderRequest(this IQueryable<BudgetCommitment> queryable)
        {
            return ((DbQuery<BudgetCommitment>)queryable).Include("OrderRequestItem.OrderRequest")
                                                         .Include("OrderRequestItem.Item");
        }

        public static IQueryable<BudgetCommitment> IncludePurchaseOrder(this IQueryable<BudgetCommitment> queryable)
        {
            return ((DbQuery<BudgetCommitment>)queryable).Include("PurchaseOrderItem.PurchaseOrder")
                                                      .Include("PurchaseOrderItem.OrderRequestItem.OrderRequest")
                                                      .Include("PurchaseOrderItem.OrderRequestItem.Item")
                                                      .Include("PurchaseOrderItem.ProcurementPlanItem.ProcurementPlan")
                                                      .Include("PurchaseOrderItem.ProcurementPlanItem.Item");
        }

        public static IQueryable<BudgetCommitment> IncludePaymentRequest(this IQueryable<BudgetCommitment> queryable)
        {
            return ((DbQuery<BudgetCommitment>)queryable).Include("PaymentRequestBudgetLine.PaymentRequest")
                                                      .Include("PaymentRequestBudgetLine.PurchaseOrderItem.PurchaseOrder")
                                                      .Include("PaymentRequestBudgetLine.PurchaseOrderItem.OrderRequestItem.OrderRequest")
                                                      .Include("PaymentRequestBudgetLine.PurchaseOrderItem.OrderRequestItem.Item")
                                                      .Include("PaymentRequestBudgetLine.PurchaseOrderItem.ProcurementPlanItem.ProcurementPlan")
                                                      .Include("PaymentRequestBudgetLine.PurchaseOrderItem.ProcurementPlanItem.Item");
        }

        #endregion

        #region .BudgetPosting.

        public static IQueryable<BudgetPosting> IncludeProjectBudgetViaRFP(this IQueryable<BudgetPosting> queryable)
        {
            return ((DbQuery<BudgetPosting>)queryable).Include("PaymentRequestBudgetLine.ProjectBudget");
        }

        public static IQueryable<BudgetPosting> IncludeRequestForPayment(this IQueryable<BudgetPosting> queryable)
        {
            return ((DbQuery<BudgetPosting>)queryable).Include("PaymentRequestBudgetLine.PaymentRequest");
        }

        public static IQueryable<BudgetPosting> IncludePurchaseOrder(this IQueryable<BudgetPosting> queryable)
        {
            return ((DbQuery<BudgetPosting>)queryable).Include("PaymentRequestBudgetLine.PurchaseOrderItem.PurchaseOrder");
        }

        public static IQueryable<BudgetPosting> IncludeOrderRequestViaRFP(this IQueryable<BudgetPosting> queryable)
        {
            return ((DbQuery<BudgetPosting>)queryable).Include("PaymentRequestBudgetLine.PurchaseOrderItem.OrderRequestItem.OrderRequest")
                                                      .Include("PaymentRequestBudgetLine.PurchaseOrderItem.ProcurementPlanItem.ProcurementPlan");
        }

        public static IQueryable<BudgetPosting> IncludeItemViaRFP(this IQueryable<BudgetPosting> queryable)
        {
            return ((DbQuery<BudgetPosting>)queryable).Include("PaymentRequestBudgetLine.PurchaseOrderItem.OrderRequestItem.Item")
                                                      .Include("PaymentRequestBudgetLine.PurchaseOrderItem.ProcurementPlanItem.Item");
        }

        public static IQueryable<BudgetPosting> IncludePerson(this IQueryable<BudgetPosting> queryable)
        {
            return ((DbQuery<BudgetPosting>)queryable).Include("Staff.Person");
        }

        public static IQueryable<BudgetPosting> IncludeDesignation(this IQueryable<BudgetPosting> queryable)
        {
            return ((DbQuery<BudgetPosting>)queryable).Include("Staff.Designation");
        }

        #endregion

        #region .Rebooking.

        public static IQueryable<Rebooking> IncludeProjectDonor(this IQueryable<Rebooking> queryable)
        {
            return ((DbQuery<Rebooking>)queryable).Include("BudgetPosting.PaymentRequestBudgetLine.ProjectBudget.BudgetCategory.ProjectDonor");
        }

        public static IQueryable<Rebooking> IncludeRequestForPayment(this IQueryable<Rebooking> queryable)
        {
            return ((DbQuery<Rebooking>)queryable).Include("BudgetPosting.PaymentRequestBudgetLine.PaymentRequest");
        }

        public static IQueryable<Rebooking> IncludePurchaseOrder(this IQueryable<Rebooking> queryable)
        {
            return ((DbQuery<Rebooking>)queryable).Include("BudgetPosting.PaymentRequestBudgetLine.PurchaseOrderItem.PurchaseOrder");
        }

        public static IQueryable<Rebooking> IncludeOrderRequestViaRFP(this IQueryable<Rebooking> queryable)
        {
            return ((DbQuery<Rebooking>)queryable).Include("BudgetPosting.PaymentRequestBudgetLine.PurchaseOrderItem.OrderRequestItem.OrderRequest");
        }

        public static IQueryable<Rebooking> IncludeItemViaRFP(this IQueryable<Rebooking> queryable)
        {
            return ((DbQuery<Rebooking>)queryable).Include("BudgetPosting.PaymentRequestBudgetLine.PurchaseOrderItem.OrderRequestItem.Item");
        }

        public static IQueryable<Rebooking> IncludePerson(this IQueryable<Rebooking> queryable)
        {
            return ((DbQuery<Rebooking>)queryable).Include("Staff.Person");
        }

        public static IQueryable<Rebooking> IncludeDesignation(this IQueryable<Rebooking> queryable)
        {
            return ((DbQuery<Rebooking>)queryable).Include("Staff.Designation");
        }

        #endregion

        #region .ProjectBudget.

        public static IQueryable<ProjectBudget> IncludeGeneralLedger(this IQueryable<ProjectBudget> queryable)
        {
            return ((DbQuery<ProjectBudget>)queryable).Include("GeneralLedger");
        }

        public static IQueryable<ProjectBudget> IncludeProject(this IQueryable<ProjectBudget> queryable)
        {
            return ((DbQuery<ProjectBudget>)queryable).Include("BudgetCategory.ProjectDonor.Project");
        }

        #endregion

        #region .BudgetCategory.

        public static IQueryable<BudgetCategory> IncludeProjectDonor(this IQueryable<BudgetCategory> queryable)
        {
            return ((DbQuery<BudgetCategory>)queryable).Include("ProjectDonor");
        }

        #endregion

        #region .ProjectDonor.

        public static IQueryable<ProjectDonor> IncludeProjectBudgets(this IQueryable<ProjectDonor> queryable)
        {
            return ((DbQuery<ProjectDonor>)queryable).Include("BudgetCategories.ProjectBudgets");
        }

        public static IQueryable<ProjectDonor> IncludeProject(this IQueryable<ProjectDonor> queryable)
        {
            return ((DbQuery<ProjectDonor>)queryable).Include("Project");
        }

        public static IQueryable<ProjectDonor> IncludeDonor(this IQueryable<ProjectDonor> queryable)
        {
            return ((DbQuery<ProjectDonor>)queryable).Include("Donor");
        }

        public static IQueryable<ProjectDonor> IncludeProcurementPlans(this IQueryable<ProjectDonor> queryable)
        {
            return ((DbQuery<ProjectDonor>)queryable).Include("ProcurementPlans.ProcurementPlanItems");
        }

        #endregion

        #region .Project.

        public static IQueryable<Project> IncludeProjectDonors(this IQueryable<Project> queryable)
        {
            return ((DbQuery<Project>)queryable).Include("ProjectDonors.ProcurementPlans.ProcurementPlanItems").Include("ProjectDonors.Donor");
        }

        #endregion

    }
}