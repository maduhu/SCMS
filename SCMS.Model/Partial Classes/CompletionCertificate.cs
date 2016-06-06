using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCMS.Resource;

namespace SCMS.Model
{
    [MetadataType(typeof(CompletionCertificateMetaData))]
    public partial class CompletionCertificate
    {
        public SelectList StaffList { get; set; }

        public SelectList Offices { get; set; }

        public SelectList PurchaseOrders { get; set; }

        public bool CanApprove { get; set; }
    }

    public class CompletionCertificateMetaData
    {
        [Required]
        public object RefNumber { get; set; }
        [Required]
        public object Purpose { get; set; }
        [Required]
        public object ProjectTitle { get; set; }
        [Required]
        public object Location { get; set; }
        [Required]
        public object WorksService { get; set; }
        [Required]
        public object PurchaseOrderId { get; set; }
        [Required]
        public object OfficeId { get; set; }
        [Required]
        public object DRCOfficeId { get; set; }
        [Required]
        public object ConfirmedBy { get; set; }
        
    }

    public class RejectCC
    {
        [Required]
        public Guid Id { get; set; }

        public string RefNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string RejectedRemarks { get; set; }
    }

    public class CCViewModel
    {
        public Guid Id { get; set; }
        public string CCNo { get; set; }
        public string PONo { get; set; }
        public string Office { get; set; }
        public string ProjectTitle { get; set; }
        public string Contructor { get; set; }
        public string ConfirmedBy { get; set; }
        public string Status { get; set; }
    }
}
