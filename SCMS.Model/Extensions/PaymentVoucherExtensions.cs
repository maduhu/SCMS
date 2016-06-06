using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;

namespace SCMS.Model
{
    public static class PaymentVoucherExtensions
    {
        public static IQueryable<PaymentVoucher> IncludePaymentType(this IQueryable<PaymentVoucher> queryable)
        {
            return ((DbQuery<PaymentVoucher>)queryable).Include("PaymentRequest.PaymentType");
        }

        public static IQueryable<PaymentVoucher> IncludeGeneralLedger(this IQueryable<PaymentVoucher> queryable)
        {
            return ((DbQuery<PaymentVoucher>)queryable).Include("PaymentVoucherBudgetLines.GeneralLedger");
        }

        public static IQueryable<PaymentVoucher> IncludeSupplier(this IQueryable<PaymentVoucher> queryable)
        {
            return ((DbQuery<PaymentVoucher>)queryable).Include("PaymentRequest.Supplier");
        }

        public static IQueryable<PaymentVoucher> IncludePurchaseOrder(this IQueryable<PaymentVoucher> queryable)
        {
            return ((DbQuery<PaymentVoucher>)queryable).Include("PaymentRequest.PurchaseOrder.ProjectDonor.Project");
        }

        public static IQueryable<PaymentVoucher> IncludePaymentRequestBudgetLines(this IQueryable<PaymentVoucher> queryable)
        {
            return ((DbQuery<PaymentVoucher>)queryable).Include("PaymentVoucherBudgetLines.PaymentRequestBudgetLine.ProjectBudget");
        }

        public static IQueryable<PaymentVoucher> IncludeStaff(this IQueryable<PaymentVoucher> queryable)
        {
            return ((DbQuery<PaymentVoucher>)queryable).Include("Staff.Person");
        }

        public static IQueryable<PaymentVoucher> IncludeStaffDesignation(this IQueryable<PaymentVoucher> queryable)
        {
            return ((DbQuery<PaymentVoucher>)queryable).Include("Staff.Designation");
        }

        public static IQueryable<PaymentVoucher> IncludeStaff1(this IQueryable<PaymentVoucher> queryable)
        {
            return ((DbQuery<PaymentVoucher>)queryable).Include("Staff1.Person");
        }

        public static IQueryable<PaymentVoucher> IncludeStaff1Designation(this IQueryable<PaymentVoucher> queryable)
        {
            return ((DbQuery<PaymentVoucher>)queryable).Include("Staff1.Designation");
        }
    }
}
