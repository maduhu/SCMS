using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCMS.Resource;


namespace SCMS.UI.Areas.Admin.Models.Approver
{
    public class ApproverModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string ActionType { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string ActivityCode { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public Guid UserId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public Guid AssistantId { get; set; }
        public int Priority { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public Guid? FinancialLimitId { get; set; }
        public Guid? ProjectDonorId { get; set; }

        //SelectLists
        public SelectList SystemUsers { get; set; }
        public SelectList Assistants { get; set; }
        public SelectList ActionTypes { get; set; }
        public SelectList DocumentTypes { get; set; }
        public SelectList FinancialLimits { get; set; }
    }

    public static class ApproverExtension
    {
        public static ApproverModel ToModel(this SCMS.Model.Approver approver)
        {
            return new ApproverModel
            {
                Id = approver.Id,
                ActionType = approver.ActionType,
                ActivityCode = approver.ActivityCode,
                UserId = approver.UserId,
                AssistantId = approver.AssistantId,
                Priority = approver.Priority,
                FinancialLimitId = approver.FinanceLimitId,
                ProjectDonorId = approver.ProjectDonorId
            };
        }

        public static SCMS.Model.Approver ToEntity(this ApproverModel approver)
        {
            return new Model.Approver
            {
                Id = approver.Id,
                ActionType = approver.ActionType,
                ActivityCode = approver.ActivityCode,
                UserId = approver.UserId,
                AssistantId = approver.AssistantId,
                Priority = approver.Priority,
                FinanceLimitId = approver.FinancialLimitId,
                ProjectDonorId = approver.ProjectDonorId
            };
        }
    }
}