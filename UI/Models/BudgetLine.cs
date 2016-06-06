using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCMS.Model;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class BudgetLine
    {
        public SCMS.Model.ProjectBudget EntityBudgetLine { get; set; }

        public Guid SubLineId { get; set; }

        public Guid BudgetLineId { get; set; }

        public string LineNumber
        {
            get { return EntityBudgetLine.LineNumber; }
            set { EntityBudgetLine.LineNumber = value; }
        }

        public string Description
        {
            get { return EntityBudgetLine.Description; }
            set { EntityBudgetLine.Description = value; }
        }
        public string BudgetCategoryId { get; set; }

        public string BudgetCategoryNumber { get; set; }

        public Guid? GeneralLedgerId { get; set; }

        public System.Web.Mvc.SelectList GeneralLedgerCodes { get; set; }

        public List<GeneralLedger> GeneralLedgers { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal TotalBudget { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? TotalCommitted { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? TotalPosted { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? RemainingBalance { get; set; }

        [Range(0, 100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Budget_ProjectDonor_MustBePercentage")]
        public double? OverrunAdjustment
        {
            get { return EntityBudgetLine != null ? EntityBudgetLine.OverrunAdjustment : null; }
            set { EntityBudgetLine.OverrunAdjustment = value; }
        }
    }

    public class RebookModel
    {
        public BudgetPosting CurrentBudgetLine { get; set; }

        public BudgetPosting NewBudgetLine { get; set; }

        public bool FullRebooking { get; set; }

        [DataType(DataType.Currency)]
        [Required]
        public decimal AmountRebooked
        {
            get { return NewBudgetLine.AmountPosted; }
            set { NewBudgetLine.AmountPosted = value; }
        }

        public Guid ProjectId { get; set; }
        public SelectList Projects { get; set; }

        public Guid ProjectDonorId { get; set; }
        public SelectList ProjectDonors { get; set; }
        [Required]
        public Guid BudgetLineId { get; set; }
        public SelectList BudgetLines { get; set; }

        public Guid CurrentProjectId { get; set; }
    }

    public class PostDetailModel
    {
        public List<BudgetPosting> BudgetPostings { get; set; }

        public List<Rebooking> PartRebookings { get; set; }

        public bool HasFinance { get; set; }

        public bool HasProcurement { get; set; }
    }

    public class CommitDetailModel
    {
        public List<BudgetCommitment> BudgetCommits { get; set; }

        public bool HasFinance { get; set; }

        public bool HasProcurement { get; set; }
    }
}