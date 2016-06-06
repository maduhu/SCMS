using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;

namespace SCMS.Model
{
    public static class PaymentRequestExtensions
    {
        public static IQueryable<PaymentRequest> IncludeCurrency(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("Currency");
        }

        public static IQueryable<PaymentRequest> IncludePaymentType(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("PaymentType");
        }

        public static IQueryable<PaymentRequest> IncludePaymentTerm(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("PaymentTerm");
        }

        public static IQueryable<PaymentRequest> IncludeSupplier(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("Supplier");
        }

        public static IQueryable<PaymentRequest> IncludeStaff(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("Staff.Person");
        }

        public static IQueryable<PaymentRequest> IncludeStaffDesignation(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("Staff.Designation");
        }

        public static IQueryable<PaymentRequest> IncludeStaff1(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("Staff1.Person");
        }

        public static IQueryable<PaymentRequest> IncludeStaff1Designation(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("Staff1.Designation");
        }

        public static IQueryable<PaymentRequest> IncludeStaff2(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("Staff2.Person");
        }

        public static IQueryable<PaymentRequest> IncludeStaff2Designation(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("Staff2.Designation");
        }

        public static IQueryable<PaymentRequest> IncludeStaff3(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("Staff3.Person");
        }

        public static IQueryable<PaymentRequest> IncludeStaff3Designation(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("Staff3.Designation");
        }

        public static IQueryable<PaymentRequest> IncludePurchaseOrder(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("PurchaseOrder.ProjectDonor.Project");
        }

        public static IQueryable<PaymentRequest> IncludePurchaseOrderItems(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("PurchaseOrder.PurchaseOrderItems");
        }

        public static IQueryable<PaymentRequest> IncludeOrderRequestItems(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("PurchaseOrder.PurchaseOrderItems.OrderRequestItem");
        }

        public static IQueryable<PaymentRequest> IncludePaymentRequestBudgetLines(this IQueryable<PaymentRequest> queryable)
        {
            return ((DbQuery<PaymentRequest>)queryable).Include("PaymentRequestBudgetLines");
        }

        public static IQueryable<PaymentRequestBudgetLine> IncludeProjectDonor(this IQueryable<PaymentRequestBudgetLine> queryable)
        {
            return ((DbQuery<PaymentRequestBudgetLine>)queryable).Include("ProjectBudget.BudgetCategory.ProjectDonor");
        }

        public static IQueryable<PaymentRequestBudgetLine> IncludeProjectBudget(this IQueryable<PaymentRequestBudgetLine> queryable)
        {
            return ((DbQuery<PaymentRequestBudgetLine>)queryable).Include("ProjectBudget");
        }


        public static IQueryable<PaymentRequestBudgetLine> IncludePaymentRequest(this IQueryable<PaymentRequestBudgetLine> queryable)
        {
            return ((DbQuery<PaymentRequestBudgetLine>)queryable).Include("PaymentRequest");
        }
    }
}
