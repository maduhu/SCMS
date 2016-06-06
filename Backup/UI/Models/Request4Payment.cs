using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public enum Requestor4
    {
        FullPayment,
        Rate_Instalment,
        Adv_Payment_percentage,
        Adv_Final_Payment_percentage
    }

    public class ChooseR4PaymentType
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string R4PType { get; set; }
    }

    public class Request4Payment
    {
        public Model.PaymentRequest EntityPaymentRqst { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public Guid ReferenceId { get; set; }

        public DateTime PreparedOn { get { return (DateTime)EntityPaymentRqst.PreparedOn; } set { EntityPaymentRqst.PreparedOn = value; } }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public decimal TotalPayment { get { return EntityPaymentRqst.TotalAmount; } set { EntityPaymentRqst.TotalAmount = value; } }

        public string Reason4Payment { get { return EntityPaymentRqst.Subject; } set { EntityPaymentRqst.Subject = value; } }

        public string Remarks { get { return EntityPaymentRqst.Remarks; } set { EntityPaymentRqst.Remarks = value; } }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public Guid SupplierId { get { return (Guid)EntityPaymentRqst.SupplierId; } set { EntityPaymentRqst.SupplierId = value; } }
        public SelectList Suppliers { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public Guid PaymentTermId { get { return EntityPaymentRqst.PaymentTermId; } set { EntityPaymentRqst.PaymentTermId = value; } }
        public SelectList PaymentTerms { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public Guid CurrencyId { get { return EntityPaymentRqst.CurrencyId; } set { EntityPaymentRqst.CurrencyId = value; } }
        public SelectList Currencies { get; set; }

        public Guid PaymentTypeId { get { return (Guid)EntityPaymentRqst.PaymentTypeId; } set { EntityPaymentRqst.PaymentTypeId = value; } }
        public SelectList Paymenttype { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string RefNumber { get { return EntityPaymentRqst.RefNumber; } set { EntityPaymentRqst.RefNumber = value; } }
        public SelectList dropdownlist { get; set; }

        public string ReqstFor { get { return EntityPaymentRqst.RequestFor; } set { EntityPaymentRqst.RequestFor = value; } }

        public List<R4PpaymentDetails> paymentDetais { get; set; }

        public string R4PType { get { return EntityPaymentRqst.PaymentRqstType; } set { EntityPaymentRqst.PaymentRqstType = value; } }

        public string usermsg { get; set; }

        public bool IsSaved { get; set; }
    }

    public class ViewR4Payment
    {
        public Model.PaymentRequest EntityPaymentRqst { get; set; }

        public  List<R4PpaymentDetails> paymentDetais { get; set; }

        public bool CanApprove { get; set; }

        public bool CanPostFunds { get; set; }

    }

    public class R4PpaymentDetails
    {
        public decimal Amount { get; set; }

        public string BudgetLine { get; set; }
        public Guid BudgetLineId { get; set; }

        public string BudgetLineDescription { get; set; }

        public string projectNo { get; set; }
        public Guid PorjectNoId { get; set; }

        public Guid PoItemId { get; set; }
    }
    
    public class RejectRFP
    {
        [Required]
        public Guid Id { get; set; }

        public string RefNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string RejectedAuthRemarks { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string RejectedReviewRemarks { get; set; }

        public bool IsReview { get; set; }
    }
}