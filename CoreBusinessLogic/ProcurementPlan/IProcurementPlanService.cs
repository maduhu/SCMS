using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using SCMS.Utils.DTOs;

namespace SCMS.CoreBusinessLogic.ProcurementPlan
{
    public interface IProcurementPlanService
    {
        /// <summary>
        /// Get list of procurement plans for country programme
        /// </summary>
        /// <param name="countryProgId"></param>
        /// <returns></returns>
        List<Model.ProcurementPlan> GetProcurementPlans(Guid countryProgId);
        /// <summary>
        /// Get Procurement Plan by Id
        /// </summary>
        /// <param name="Id">ProcurementPlan Id</param>
        /// <returns></returns>
        Model.ProcurementPlan GetProcurementPlanById(Guid Id);
        /// <summary>
        /// Get Project's Procurement Plan
        /// </summary>
        /// <param name="ProjectDonorId">ProjectDonor Id</param>
        /// <returns></returns>
        Model.ProcurementPlan GetProcurementPlanByProjectId(Guid ProjectDonorId);
        /// <summary>
        /// Get procurement plan items
        /// </summary>
        /// <param name="ppId">ProcurementPlan Id</param>
        /// <returns></returns>
        List<ProcurementPlanItem> GetProcurementPlanItems(Guid ppId);
        /// <summary>
        /// Get procurement plan items by project donor
        /// </summary>
        /// <param name="ProjectDonorId">ProjectDonor Id</param>
        /// <returns></returns>
        List<ProcurementPlanItem> GetProcurementPlanItemsByProjectId(Guid ProjectDonorId);
        /// <summary>
        /// Get Procurement Plan Item by Id
        /// </summary>
        /// <param name="Id">Procurement Plan Item Id</param>
        /// <returns></returns>
        ProcurementPlanItem GetProcurementPlanItemById(Guid Id);
        /// <summary>
        /// Insert/Update Procurement Plan
        /// </summary>
        /// <param name="pp"></param>
        /// <returns></returns>
        bool SaveProcurementPlan(Model.ProcurementPlan pp);
        /// <summary>
        /// Insert/Update Procurement Plan Item
        /// </summary>
        /// <param name="ppItem"></param>
        /// <returns></returns>
        bool SaveProcurementPlanItem(ProcurementPlanItem ppItem);
        /// <summary>
        /// Delete Procurement Plan Item
        /// </summary>
        /// <param name="ppItemId"></param>
        /// <returns></returns>
        bool DeleteProcurementPlanItem(Guid ppItemId);
        /// <summary>
        /// Delete Procurement Plan
        /// </summary>
        /// <param name="ppId"></param>
        /// <returns></returns>
        bool DeleteProcumentPlan(Guid ppId);
        /// <summary>
        /// Get PP Items for OR creation
        /// </summary>
        /// <param name="ProjectDonorId"></param>
        /// <returns></returns>
        List<ProcurementPlanItem> GetPPItemsForOR(Guid ProjectDonorId);

        List<Model.ProcurementPlan> GetPPForApproval(SystemUser currentUser);

        List<Model.ProcurementPlan> GetPPForApproval2(SystemUser currentUser);

        List<Model.ProcurementPlan> GetPPForReview(SystemUser currentUser);

        List<Model.ProcurementPlan> GetPPForAuthorization(SystemUser currentUser);

        List<Model.ProcurementPlanItem> GetPPItemsForApproval(Guid ppId);

        List<ProcurementPlanSummary> Find(List<Guid> ids);
    }
}
