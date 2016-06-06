using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCMS.Resource;

namespace SCMS.Model
{
    [MetadataType(typeof(ProcurementPlanMetaData))]
    public partial class ProcurementPlan
    {
        public SelectList SubOffices { get; set; }

        public Guid ProjectId { get; set; }
        public SelectList Projects { get; set; }

        public SelectList ProjectDonors { get; set; }

        public Guid OrderRequestId { get; set; }

        public Guid PurchaseOrderId { get; set; }

        public List<ProcurementPlanItem> PPItemList { get; set; }

        public List<PurchaseOrderItem> POItems { get; set; }

        public string ActionType { get; set; }

        public bool CanApprove { get; set; }

        public bool CanApproveII { get; set; }

        public bool CanReview { get; set; }

        public bool CanAuthorize { get; set; }
    }

    public class ProcurementPlanMetaData
    {
        [Required]
        public object ProjectDonorId { get; set; }
        [Required]
        public object PreparingOfficeId { get; set; }
        [Required]
        public object PreparedBy { get; set; }
        [Required]
        public object PreparedOn { get; set; }
    }

    public class PPModelView
    {
        public Guid Id { get; set; }
        public string RefNo { get; set; }
        public string ProjectName { get; set; }
        public string ProjectNo { get; set; }
        public string DonorShortName { get; set; }
        public string CountrySubOffice { get; set; }
        public DateTime PreparedOn { get; set; }
    }

    public class RejectPP
    {
        [Required]
        public Guid Id { get; set; }

        public string RefNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string RejectionRemarks { get; set; }

        public string ActionType { get; set; }
    }
}
