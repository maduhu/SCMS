using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web;

namespace SCMS.Model
{
    [MetadataType(typeof(AssetMetaData))]
    public partial class Asset
    {
        public SelectList Assets { get; set; }
        public SelectList ProjectDonors { get; set; }
        public SelectList Warehouses { get; set; }
        public SelectList Items { get; set; }
        public string AssetPicture { get; set; }
        public HttpPostedFileBase AssetPictureUpload { get; set; }
        public SelectList Currencies { get; set; }
        public Guid ProjectId { get; set; }
        public SelectList Projects { get; set; }
        public FileContentResult AssetImage { get; set; }
        public SelectList Staffs { get; set; }

        public string AssetNameAndNo { get { return Name + " (" + AssetNumber + ")"; } }
    }

    public class AssetMetaData
    {
        //[Required]
        public object ActionType { get; set; }
        [Required]
        public object CurrentProjectDonorId { get; set; }
        [Required]
        public object DepreciationCurrencyId { get; set; }
        [Required]
        public object CurrentWareHouseId { get; set; }
        [Required]
        public object AssetNumber { get; set; }
        [Required]
        public object ItemId { get; set; }
        [Required]
        public object Name { get; set; }
        [Required]
        public object SerialNumber { get; set; }
        [Required]
        public object Lifespan { get; set; }
        [Required]
        public object Weight { get; set; }
        [Required]
        public object PurchaseValue { get; set; }
        [Required]
        public object PercentageDepr { get; set; }
        //[Required]
        public object OpeningAccDepreciaiton { get; set; }
        [Required]
        public object AcquisionDate { get; set; }
        [Required]
        public object OriginalProjectId { get; set; }
    }

    [MetadataType(typeof(AssetManagmentMetaData))]
    public partial class AssetManagment
    {
        public SelectList ProjectDonors { get; set; }
    }

    public class AssetManagmentMetaData
    {
        [Required]
        public object ManagementAction { get; set; }
        //[Required]
        //public object PartnerName { get; set; }
        //[Required]
        //public object currentProjectId { get; set; }
        [Required]
        public object IssueDate { get; set; }
        [Required]
        public object ActionTakenBy { get; set; }
    }

    public class AssetView
    {
        public Guid AssetId { get; set; }
        public string AssetNumber { get; set; }
        public string AssetName { get; set; }
        public string currentProject { get; set; }
        public string OriginalProject { get; set; }
        public string WarehouseName { get; set; }
        public string AssignedTo { get; set; }
        public string Classification { get; set; }
    }
}
