using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCMS.Resource;

namespace SCMS.Model
{
    #region .OrderRequest.

    [MetadataType(typeof(OrderRequestMetaData))]
    public partial class OrderRequest
    {
        public SelectList Currencies { get; set; }

        public SelectList Projects { get; set; }

        public SelectList ProjectDonors { get; set; }

        public SelectList Locations { get; set; }

        public bool FromPP { get; set; }

        public string BackDatingReason { get; set; }

        public Guid BackDatedBy { get; set; }

        public bool CanEdit { get; set; }

        /// <summary>
        /// This applies to approve/finance review/authorize
        /// </summary>
        public bool CanApprove { get; set; }
        /// <summary>
        /// This applies to create po right from authorized OR
        /// </summary>
        public bool CanPreparePO { get; set; }
        /// <summary>
        /// This applies to funds available before submit/approval/review/authroization
        /// </summary>
        public List<BudgetCheckResult> BudgetCheckResults { get; set; }

        public bool IsFinanceReview { get; set; }

        public List<OrderRequestItem> ORItemList { get; set; }
        //Needed by PO
        public List<PurchaseOrderItem> POItems { get; set; }
        
    }

    public class OrderRequestMetaData
    {
        [Required]
        public object OrderDate { get; set; }
        [Required]
        public object CurrencyId { get; set; }
        [Required]
        public object ProjectId { get; set; }
        [Required]
        public object ProjectDonorId { get; set; }
        [Required]
        public object RequestedDestinationId { get; set; }
        [Required]
        public object FinalDestinationId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public object DeliveryDate { get; set; }
    }


    public class RejectOR
    {
        [Required]
        public Guid Id { get; set; }

        public string RefNumber { get; set; }

        [Required]
        public string RejectedAuthorizeRemarks { get; set; }

        [Required]
        public string RejectedReviewRemarks { get; set; }

        public bool IsReview { get; set; }
    }

    public class ORViewModel
    {
        public Guid Id { get; set; }
        public string RefNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string FirstItem { get; set; }
        public float ORValue { get; set; }
        public string Requestor { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }

    #endregion

    #region .OrderRequestItem.

    [MetadataType(typeof(OrderRequestItemMetaData))]
    public partial class OrderRequestItem
    {
        public SelectList Items { get; set; }

        public SelectList BudgetLines { get; set; }

        public string ItemName
        {
            get
            {
                return Item != null ? Item.Name : string.Empty;
            }
        }

        public bool AddToPO { get; set; }
    }

    public class OrderRequestItemMetaData
    {
        [Required]
        public object ItemId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_MaximumText50")]
        public object ItemDescription { get; set; }

        [Required]
        public object Quantity { get; set; }
        [Required]
        public object EstimatedUnitPrice { get; set; }
        [Required]
        public object EstimatedPrice { get; set; }
        [Required]
        public object BudgetLineId { get; set; }
    }

    #endregion
}
