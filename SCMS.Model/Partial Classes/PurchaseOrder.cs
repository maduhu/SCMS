using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCMS.Resource;

namespace SCMS.Model
{
    #region .PurchaseOrder.

    [MetadataType(typeof(PurchaseOrderMetaData))]
    public partial class PurchaseOrder
    {

        private string ppNumber;

        public List<PurchaseOrderItem> POItems { get; set; }

        public List<OrderRequestItem> ORItems { get; set; }

        public List<OrderRequest> ORList { get; set; }

        public List<ProcurementPlan> PPList { get; set; }

        public SelectList Currencies { get; set; }

        public Guid ProjectId { get; set; }

        public SelectList Projects { get; set; }

        public SelectList ProjectDonors { get; set; }

        public SelectList OrderRequests { get; set; }

        public SelectList Locations { get; set; }

        public SelectList PaymentTerms { get; set; }

        public SelectList ShippingTerms { get; set; }

        public SelectList Suppliers { get; set; }

        public bool SameCurrency { get; set; }

        public string BackDatingReason { get; set; }

        public Guid BackDatedBy { get; set; }

        public bool CanEdit { get; set; }

        public bool HasMoreThanOneOR { get; set; }

        public string PPNumber
        {
            get
            {
                if (ppNumber != null)
                    return ppNumber;
                var poItems = PurchaseOrderItems.Where(p => p.ProcurementPlanItemId.HasValue).ToList();
                if (poItems != null && poItems.Count > 0)
                    ppNumber = poItems[0].ProcurementPlanItem.ProcurementPlan.RefNumber;
                else
                {
                    var orItems = PurchaseOrderItems.Where(p => p.OrderRequestItemId.HasValue).ToList();
                    if (orItems != null && orItems.Count > 0)
                        ppNumber = orItems[0].OrderRequestItem.OrderRequest.RefNumber;
                    else
                        ppNumber = string.Empty;
                }
                return ppNumber;
            }
        }

        /// <summary>
        /// This applies to approve/finance review/authorize
        /// </summary>
        public bool CanApprove { get; set; }
        /// <summary>
        /// This applies to funds available before submit/approval/review/authroization
        /// </summary>
        public List<BudgetCheckResult> BudgetCheckResults { get; set; }

        public List<AttachedDocument> AttachedDocuments { get; set; }
    }

    public class PurchaseOrderMetaData
    {
        [Required]
        public object PODate { get; set; }
        [Required]
        public object DeliveryAddress { get; set; }
        [Required]
        public object LatestDeliveryDate { get; set; }
        [Required]
        public object ShippingTermId { get; set; }
        [Required]
        public object PaymentTermId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        //[Remote("ValidateQuotationRef", "PurchaseOrder", AdditionalFields = "Id")]
        public object QuotationRef { get; set; }
        [Required]
        public object SupplierId { get; set; }
        [Required]
        public object CurrencyId { get; set; }
    }

    public class RejectPO
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

    public class POViewModel
    {
        public Guid Id { get; set; }
        public string PONo { get; set; }
        public string ORNo { get; set; }
        public string Supplier { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public float POValue { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }

    public class PurchaseOrderTenderingType
    {
        public Guid PurchaseOrderId { get; set; }

        [Required]
        public Guid TenderingTypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        //[Remote("ValidateTenderNumber", "PurchaseOrder", AdditionalFields = "PurchaseOrderId")]
        public string TenderNumber { get; set; }

        public SelectList TenderingTypes { get; set; }

        public decimal POValue { get; set; }

        public bool? WaiverAcquired { get; set; }
    }

    #endregion
    
    #region .PurchaseOrderItem.

    [MetadataType(typeof(PurchaseOrderItemMetaData))]
    public partial class PurchaseOrderItem
    {
        public SelectList BudgetLines { get; set; }

        public ProjectDonor ProjectDonor
        {
            get
            {
                if (OrderRequestItemId.HasValue)
                    return OrderRequestItem.OrderRequest.ProjectDonor;
                else
                    return ProcurementPlanItem.ProcurementPlan.ProjectDonor;
            }
        }

        public Item Item
        {
            get
            {
                if (OrderRequestItemId.HasValue)
                    return OrderRequestItem.Item;
                else
                    return ProcurementPlanItem.Item;
            }
        }

        public string ItemDescription
        {
            get
            {
                if (OrderRequestItemId.HasValue)
                    return OrderRequestItem.ItemDescription;
                else
                    return ProcurementPlanItem.ItemDescription;
            }
        }
    }

    public class PurchaseOrderItemMetaData
    {
        [Required]
        public object UnitPrice { get; set; }
        [Required]
        public object TotalPrice { get; set; }
        [Required]
        public object BudgetLineId { get; set; }
    }

    #endregion
}