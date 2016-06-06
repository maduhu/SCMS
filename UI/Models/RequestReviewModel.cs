using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class RequestReviewModel
    {
        //List for Approval
        public List<Model.OrderRequest> ApprovalOrderRequests { get; set; }
        public List<Model.WarehouseRelease> WarehouseReleases { get; set; }
        public List<Model.GoodsReceivedNote> GoodsReceivedNotes { get; set; }

        //Lists for Review
        public List<Model.OrderRequest> OrderRequests { get; set; }

        public List<Model.PurchaseOrder> PurchaseOrders { get; set; }

        public List<ViewR4Payment> PaymentRequests { get; set; }

        public List<Model.ProcurementPlan> ProcurementPlansForApproval { get; set; }

        public List<Model.ProcurementPlan> ProcurementPlansForApproval2 { get; set; }

        public List<Model.ProcurementPlan> ProcurementPlansForReview { get; set; }

        public List<Model.ProcurementPlan> ProcurementPlansForAuth { get; set; }

        public List<CompletionCtificate> CompletionCertificates { get; set; }

        //Lists for Authorization
        public List<Model.OrderRequest> AuthOrderRequests { get; set; }

        public List<ViewR4Payment> AuthPaymentRequests { get; set; }

    }

    public class ReviewRFP
    {
        public Model.PaymentRequest EntityPaymentRqst { get; set; }

        public List<ReviewRFPDetails> paymentDetails { get; set; }

        public Guid MBCurrencyId { get; set; }
        public SelectList Currencies { get; set; }
        public decimal? MBValue { get; set; }

        public List<Model.BudgetCheckResult> BudgetCheckResults { get; set; }
    }

    public class ReviewRFPDetails
    {
        [DataType(DataType.Currency)]
        [Required]
        public decimal Amount { get; set; }
        public Guid BudgetLineId { get; set; }
        public SelectList BudgetLines { get; set; }
        public Guid ProjectDonorId { get; set; }
        public SelectList ProjectDonors { get; set; }
        public Guid RFPDetailId { get; set; }
    }
}