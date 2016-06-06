using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.Utils.DTOs;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.CoreBusinessLogic.ProcurementPlan
{
    public class ProcurementPlanService : IProcurementPlanService
    {

        private static void ClearPPSessionData()
        {
            SessionData.CurrentSession.ProjectList = null;
            SessionData.CurrentSession.ProjectDonorList = null;
            SessionData.CurrentSession.ProcurementPlanList = null;
        }

        private static void ClearPPItemSessionData()
        {
            SessionData.CurrentSession.ProjectList = null;
            SessionData.CurrentSession.ProjectDonorList = null;
            SessionData.CurrentSession.ProcurementPlanItemList = null;
        }

        #region IProcurementPlanService Members

        public List<Model.ProcurementPlan> GetProcurementPlans(Guid countryProgId)
        {
            return SessionData.CurrentSession.ProcurementPlanList.Where(p => p.ProjectDonor.EndDate >= DateTime.Today).OrderByDescending(p => p.PreparedOn).ToList();
        }

        public Model.ProcurementPlan GetProcurementPlanById(Guid Id)
        {
            return SessionData.CurrentSession.ProcurementPlanList.FirstOrDefault(p => p.Id == Id).To<Model.ProcurementPlan>();
        }

        public Model.ProcurementPlan GetProcurementPlanByProjectId(Guid ProjectDonorId)
        {
            return SessionData.CurrentSession.ProcurementPlanList.FirstOrDefault(p => p.ProjectDonorId == ProjectDonorId).To<Model.ProcurementPlan>();
        }

        public List<Model.ProcurementPlanItem> GetProcurementPlanItems(Guid ppId)
        {
            return SessionData.CurrentSession.ProcurementPlanItemList.Where(p => p.ProcurementPlanId == ppId).OrderBy(p => p.Item.Name).ToList();
        }

        public List<Model.ProcurementPlanItem> GetProcurementPlanItemsByProjectId(Guid ProjectDonorId)
        {
            return SessionData.CurrentSession.ProcurementPlanItemList.Where(p => p.ProcurementPlan.ProjectDonorId == ProjectDonorId).ToList();
        }

        public Model.ProcurementPlanItem GetProcurementPlanItemById(Guid Id)
        {
            return SessionData.CurrentSession.ProcurementPlanItemList.FirstOrDefault(p => p.Id == Id).To<Model.ProcurementPlanItem>();
        }

        public bool SaveProcurementPlan(Model.ProcurementPlan pp)
        {
            ClearPPSessionData();
            using (var context = new SCMSEntities())
            {
                if (pp.Id.Equals(Guid.Empty))
                {
                    var ppEntity = context.ProcurementPlans.FirstOrDefault(p => p.ProjectDonorId == pp.ProjectDonorId);
                    var pd = context.ProjectDonors.FirstOrDefault(p => p.Id == pp.ProjectDonorId);
                    pp.RefNumber = "PP/DRC/" + pd.Project.CountryProgramme.Country.ShortName + "/" + pd.ProjectNumber;
                    if (ppEntity != null)
                    {
                        ppEntity.PreparedBy = pp.PreparedBy;
                        ppEntity.PreparedOn = pp.PreparedOn;
                        ppEntity.PreparingOfficeId = pp.PreparingOfficeId;
                        //let the object reference point to the existing entity from the db
                        pp.Id = ppEntity.Id;
                    }
                    else
                    {
                        pp.Id = Guid.NewGuid();
                        context.ProcurementPlans.Add(pp);
                    }
                }
                else
                {
                    var existing = context.ProcurementPlans.FirstOrDefault(p => p.Id == pp.Id);
                    context.Entry(existing).CurrentValues.SetValues(pp);
                }
                return context.SaveChanges() > 0;
            }
        }

        public bool SaveProcurementPlanItem(Model.ProcurementPlanItem ppItem)
        {
            ClearPPItemSessionData();
            ClearPPSessionData();
            using (var context = new SCMSEntities())
            {
                if (ppItem.Id.Equals(Guid.Empty))
                {
                    ppItem.Id = Guid.NewGuid();
                    context.ProcurementPlanItems.Add(ppItem);
                }
                else
                {
                    var existing = context.ProcurementPlanItems.FirstOrDefault(p => p.Id == ppItem.Id);
                    context.Entry(existing).CurrentValues.SetValues(ppItem);
                }
                return context.SaveChanges() > 0;
            }
        }

        public bool DeleteProcurementPlanItem(Guid ppItemId)
        {
            ClearPPItemSessionData();
            ClearPPSessionData();
            using (var context = new SCMSEntities())
            {
                context.ProcurementPlanItems.Remove(context.ProcurementPlanItems.FirstOrDefault(p => p.Id == ppItemId));
                return context.SaveChanges() > 0;
            }
        }

        public bool DeleteProcumentPlan(Guid ppId)
        {
            ClearPPSessionData();
            using (var context = new SCMSEntities())
            {
                context.ProcurementPlans.Remove(context.ProcurementPlans.FirstOrDefault(p => p.Id == ppId));
                return context.SaveChanges() > 0;
            }
        }

        public List<ProcurementPlanItem> GetPPItemsForOR(Guid ProjectDonorId)
        {
            return SessionData.CurrentSession.ProcurementPlanItemList.Where(p => p.ProcurementPlan.ProjectDonorId == ProjectDonorId && p.IsAuthorized == true && p.IsRemoved == false && p.AddedToOR == false).OrderBy(p => p.Item.Name).ToList();                
        }

        public List<Model.ProcurementPlan> GetPPForApproval(SystemUser currentUser)
        {
            using (var context = new SCMSEntities())
            {
                List<Model.ProcurementPlan> procurementPlans = new List<Model.ProcurementPlan>();
                var approvers = context.Approvers.Where(a => a.UserId == currentUser.Id && a.ActivityCode == NotificationHelper.ppCode && a.ActionType == NotificationHelper.approvalCode).ToList();
                var delegateApprovers = context.Approvers.Where(a => a.AssistantId == currentUser.Id && a.ActivityCode == NotificationHelper.ppCode && a.ActionType == NotificationHelper.approvalCode).ToList();
                if (approvers != null)
                {
                    foreach (var approver in approvers)
                    {
                        var ppList = SessionData.CurrentSession.ProcurementPlanList.Where(p => (p.ProjectDonorId == approver.ProjectDonorId && p.IsSubmitted &&
                            p.ProcurementPlanItems.Where(pi => !pi.IsApproved).Count() > 0 && !p.IsRejected)
                            && p.Notifications.Where(n => (Guid)n.ProcurementPlanId == p.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                        foreach (var pp in ppList)
                        {
                            pp.ActionType = NotificationHelper.approvalCode;
                            procurementPlans.Add(pp);
                        }
                    }
                }
                if (delegateApprovers != null)
                {
                    foreach (var approver in delegateApprovers)
                    {
                        var ppList = SessionData.CurrentSession.ProcurementPlanList.Where(p => (p.ProjectDonorId == approver.ProjectDonorId && p.IsSubmitted &&
                            p.ProcurementPlanItems.Where(pi => !pi.IsApproved).Count() > 0 && !p.IsRejected)
                            && p.Notifications.Where(n => (Guid)n.ProcurementPlanId == p.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                        foreach (var pp in ppList)
                            if (!procurementPlans.Contains(pp))
                            {
                                pp.ActionType = NotificationHelper.approvalCode;
                                procurementPlans.Add(pp);
                            }
                    }
                }
                return procurementPlans;
            }
        }

        public List<Model.ProcurementPlan> GetPPForApproval2(SystemUser currentUser)
        {
            using (var context = new SCMSEntities())
            {
                List<Model.ProcurementPlan> procurementPlans = new List<Model.ProcurementPlan>();
                var approvers = context.Approvers.Where(a => a.UserId == currentUser.Id && a.ActivityCode == NotificationHelper.ppCode && a.ActionType == NotificationHelper.approvalIICode).ToList();
                var delegateApprovers = context.Approvers.Where(a => a.AssistantId == currentUser.Id && a.ActivityCode == NotificationHelper.ppCode && a.ActionType == NotificationHelper.approvalIICode).ToList();
                if (approvers != null)
                {
                    foreach (var approver in approvers)
                    {
                        var ppList = SessionData.CurrentSession.ProcurementPlanList.Where(p => (p.ProjectDonorId == approver.ProjectDonorId &&
                            p.ProcurementPlanItems.Where(pi => pi.IsReviewed && !pi.IsApproved2).Count() > 0 && !p.IsRejected)
                            && p.Notifications.Where(n => (Guid)n.ProcurementPlanId == p.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                        foreach (var pp in ppList)
                        {
                            pp.ActionType = NotificationHelper.approvalIICode;
                            procurementPlans.Add(pp);
                        }
                    }
                }
                if (delegateApprovers != null)
                {
                    foreach (var approver in delegateApprovers)
                    {
                        var ppList = SessionData.CurrentSession.ProcurementPlanList.Where(p => (p.ProjectDonorId == approver.ProjectDonorId &&
                            p.ProcurementPlanItems.Where(pi => pi.IsReviewed && !pi.IsApproved2).Count() > 0 && !p.IsRejected)
                            && p.Notifications.Where(n => (Guid)n.ProcurementPlanId == p.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                        foreach (var pp in ppList)
                            if (!procurementPlans.Contains(pp))
                            {
                                pp.ActionType = NotificationHelper.approvalIICode;
                                procurementPlans.Add(pp);
                            }
                    }
                }
                return procurementPlans;
            }
        }

        public List<Model.ProcurementPlan> GetPPForReview(SystemUser currentUser)
        {
            using (var context = new SCMSEntities())
            {
                List<Model.ProcurementPlan> procurementPlans = new List<Model.ProcurementPlan>();
                var approvers = context.Approvers.Where(a => a.UserId == currentUser.Id && a.ActivityCode == NotificationHelper.ppCode && a.ActionType == NotificationHelper.reviewCode).ToList();
                var delegateApprovers = context.Approvers.Where(a => a.AssistantId == currentUser.Id && a.ActivityCode == NotificationHelper.ppCode && a.ActionType == NotificationHelper.reviewCode).ToList();
                if (approvers != null)
                {
                    foreach (var approver in approvers)
                    {
                        var ppList = SessionData.CurrentSession.ProcurementPlanList.Where(p => (p.ProjectDonorId == approver.ProjectDonorId &&
                            p.ProcurementPlanItems.Where(pi => pi.IsApproved && !pi.IsReviewed).Count() > 0 && !p.IsRejected)
                            && p.Notifications.Where(n => (Guid)n.ProcurementPlanId == p.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                        foreach (var pp in ppList)
                        {
                            pp.ActionType = NotificationHelper.reviewCode;
                            procurementPlans.Add(pp);
                        }
                    }
                }
                if (delegateApprovers != null)
                {
                    foreach (var approver in delegateApprovers)
                    {
                        var ppList = SessionData.CurrentSession.ProcurementPlanList.Where(p => (p.ProjectDonorId == approver.ProjectDonorId &&
                            p.ProcurementPlanItems.Where(pi => pi.IsApproved && !pi.IsReviewed).Count() > 0 && !p.IsRejected)
                            && p.Notifications.Where(n => (Guid)n.ProcurementPlanId == p.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                        foreach (var pp in ppList)
                            if (!procurementPlans.Contains(pp))
                            {
                                pp.ActionType = NotificationHelper.reviewCode;
                                procurementPlans.Add(pp);
                            }
                    }
                }
                return procurementPlans;
            }
        }

        public List<Model.ProcurementPlan> GetPPForAuthorization(SystemUser currentUser)
        {
            using (var context = new SCMSEntities())
            {
                List<Model.ProcurementPlan> procurementPlans = new List<Model.ProcurementPlan>();
                var approvers = context.Approvers.Where(a => a.UserId == currentUser.Id && a.ActivityCode == NotificationHelper.ppCode && a.ActionType == NotificationHelper.authorizationCode).ToList();
                var delegateApprovers = context.Approvers.Where(a => a.AssistantId == currentUser.Id && a.ActivityCode == NotificationHelper.ppCode && a.ActionType == NotificationHelper.authorizationCode).ToList();
                if (approvers != null)
                {
                    foreach (var approver in approvers)
                    {
                        var ppList = SessionData.CurrentSession.ProcurementPlanList.Where(p => (p.ProjectDonorId == approver.ProjectDonorId &&
                            p.ProcurementPlanItems.Where(pi => pi.IsApproved2 && !pi.IsAuthorized).Count() > 0 && !p.IsRejected)
                            && p.Notifications.Where(n => (Guid)n.ProcurementPlanId == p.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                        foreach (var pp in ppList)
                        {
                            pp.ActionType = NotificationHelper.authorizationCode;
                            procurementPlans.Add(pp);
                        }
                    }
                }
                if (delegateApprovers != null)
                {
                    foreach (var approver in delegateApprovers)
                    {
                        var ppList = SessionData.CurrentSession.ProcurementPlanList.Where(p => (p.ProjectDonorId == approver.ProjectDonorId &&
                            p.ProcurementPlanItems.Where(pi => pi.IsApproved2 && !pi.IsAuthorized).Count() > 0 && !p.IsRejected)
                            && p.Notifications.Where(n => (Guid)n.ProcurementPlanId == p.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                        foreach (var pp in ppList)
                            if (!procurementPlans.Contains(pp))
                            {
                                pp.ActionType = NotificationHelper.authorizationCode;
                                procurementPlans.Add(pp);
                            }
                    }
                }
                return procurementPlans;
            }
        }

        public List<Model.ProcurementPlanItem> GetPPItemsForApproval(Guid ppId)
        {
            return SessionData.CurrentSession.ProcurementPlanItemList.Where(p => p.ProcurementPlanId == ppId && p.IsApproved == false).ToList();
        }


        public List<ProcurementPlanSummary> Find(List<Guid> ids)
        {

            List<ProcurementPlanSummary> plans = new List<ProcurementPlanSummary>();
            var results = from myPlans in SessionData.CurrentSession.ProcurementPlanList
                          where ids.Contains(myPlans.Id)
                          select myPlans;

            foreach (Model.ProcurementPlan item in results.ToList())
            {
                ProcurementPlanSummary tmp = new ProcurementPlanSummary();
                tmp.Id = item.Id;
                tmp.RefNumber = item.RefNumber;
                tmp.ProjectTitle = item.ProjectDonor.Project.Name;
                tmp.ProjectNumber = item.ProjectDonor.Project.ProjectNumber;
                tmp.PrepOffice = item.CountrySubOffice.Name;
                tmp.Donor = item.ProjectDonor.Donor.ShortName;
                tmp.DatePrepared = item.PreparedOn;

                plans.Add(tmp);
            }
            return plans;
        }

        #endregion
    }
}
