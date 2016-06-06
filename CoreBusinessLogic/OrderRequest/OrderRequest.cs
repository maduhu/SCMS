using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using System.Data.Objects.DataClasses;
using System.Transactions;
using SCMS.CoreBusinessLogic.NotificationsManager;
using System.Data;
using SCMS.Utils.DTOs;
using SCMS.CoreBusinessLogic.Budgeting;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.CoreBusinessLogic.OrderRequest
{
    public class OrderRequest : IOrderRequest
    {

        private INotificationService NotificationService;
        private IBudgetService budgetService;
        private _ExchangeRate.IExchangeRateService exchangeRateService;

        public OrderRequest(_ExchangeRate.IExchangeRateService exchangeRateService, INotificationService NotificationService, IBudgetService budgetService)
        {
            this.exchangeRateService = exchangeRateService;
            this.NotificationService = NotificationService;
            this.budgetService = budgetService;
        }

        private void ClearORSessionData()
        {
            SessionData.CurrentSession.OrderRequestList = null;
        }

        private void ClearPPSessionData()
        {
            SessionData.CurrentSession.ProcurementPlanList = null;
        }

        private void ClearPPItemSessionData()
        {
            SessionData.CurrentSession.ProcurementPlanItemList = null;
        }

        public List<OrderRequestSummary> Find(List<Guid> ids)
        {
            List<OrderRequestSummary> orders = new List<OrderRequestSummary>();

            using (var context = new SCMSEntities())
            {
                var results = from myOrders in context.OrderRequests
                              where ids.Contains(myOrders.Id)
                              select myOrders;

                foreach (Model.OrderRequest item in results.ToList())
                {
                    Staff prepBy = context.Staffs.SingleOrDefault(p => p.Id == item.PreparedBy);
                    Person ps = prepBy.Person; Designation d = prepBy.Designation;
                    DateTime issueDate = (DateTime)item.PreparedOn;
                    DateTime requestDate = (DateTime)item.OrderDate;
                    var productQuery = from b in context.OrderRequestItems
                                       where b.OrderRequestId == item.Id
                                       select b;
                    String firstItem = productQuery.Count() != 0 ? productQuery.First().Item.Name : "";
                    String ProjNo = productQuery.Count() != 0 ? productQuery.First().ProjectBudget.BudgetCategory.ProjectDonor.ProjectNumber : "";
                    String ProjName = productQuery.Count() != 0 ? productQuery.First().ProjectBudget.BudgetCategory.ProjectDonor.Project.Name : "";
                    String donor = item.ProjectDonor.Donor.ShortName;

                    OrderRequestSummary tmp = new OrderRequestSummary();
                    tmp.Id = item.Id;
                    tmp.RefNumber = item.RefNumber;
                    tmp.FirstItem = firstItem;
                    tmp.ProjectNum = ProjNo;
                    tmp.ProjectName = ProjName;
                    tmp.PrepStaffNames = ps.FirstName + " " + ps.OtherNames;
                    tmp.TotalValue = item.TotalAmount.HasValue ? ((decimal)item.TotalAmount).ToString("#,###.00") : "";

                    if (item.IsAuthorized.HasValue && ((bool)item.IsAuthorized))
                    {
                        tmp.Status = "AU";
                        tmp.StatusDate = (DateTime)item.AuthorizedOn;
                    }
                    else if (item.IsRejected.HasValue && ((bool)item.IsRejected))
                    {
                        tmp.Status = "RJ";

                        if (item.AuthorizedOn.HasValue)
                        {
                            tmp.StatusDate = (DateTime)item.AuthorizedOn;
                        }
                        else
                            if (item.ReviewedOn.HasValue)
                            {
                                tmp.StatusDate = (DateTime)item.ReviewedOn;
                            }
                            else
                            {
                                tmp.StatusDate = (DateTime)item.ApprovedOn;
                            }

                    }
                    else if (item.IsReviewed.HasValue && ((bool)item.IsReviewed))
                    {
                        tmp.Status = "RV";
                        tmp.StatusDate = (DateTime)item.ReviewedOn;
                    }
                    else if (item.IsApproved)
                    {
                        tmp.Status = "AP";
                        tmp.StatusDate = (DateTime)item.ApprovedOn;
                    }
                    else if (item.IsSubmitted)
                    {
                        tmp.Status = "CR";
                        tmp.StatusDate = (DateTime)item.PreparedOn;
                    }
                    else
                    {
                        tmp.Status = "NEW";
                        tmp.StatusDate = (DateTime)item.PreparedOn;
                    }

                    orders.Add(tmp);
                }

            }

            return orders;

        }

        public bool UpdateOrderRequest(Model.OrderRequest entity)
        {
            using (var context = new SCMSEntities())
            {
                var existing = context.OrderRequests.FirstOrDefault(o => o.Id == entity.Id);
                context.Entry(existing).CurrentValues.SetValues(entity);
                if (context.SaveChanges() > 0)
                {
                    //Clear session data
                    ClearORSessionData();
                    return true;
                } 
                return false;
            }

        }

        public bool UpdateORWithPossibleProjectChange(Model.OrderRequest or, bool projectChanged)
        {
            ClearORSessionData();
            if (!projectChanged)
                return this.UpdateOrderRequest(or);
            using (var context = new SCMSEntities())
            {
                var entity = context.OrderRequests.FirstOrDefault(o => o.Id == or.Id);
                var defaultBLId = SessionData.CurrentSession.ProjectDonorList.FirstOrDefault(p => p.Id == or.ProjectDonorId).BudgetCategories.FirstOrDefault().ProjectBudgets.FirstOrDefault();
                if (defaultBLId == null)
                    return false;
                foreach (var orItem in entity.OrderRequestItems)
                {
                    orItem.BudgetLineId = defaultBLId.Id;
                }
                context.Entry(entity).CurrentValues.SetValues(or);
                return context.SaveChanges() > 0;
            }
        }

        public void DeleteOrderRequst(Guid id)
        {
            ClearORSessionData();
            ClearPPItemSessionData();

            using (var dbContext = new SCMSEntities())
            {
                Model.OrderRequest odaRequest = dbContext.OrderRequests.FirstOrDefault(o => o.Id == id);

                odaRequest.OrderRequestItems.ToArray().ForEach(s => dbContext.OrderRequestItems.Remove(s));
                dbContext.OrderRequests.Remove(odaRequest);
                dbContext.SaveChanges();
            }
        }

        public void NotifiyAuthorizer(Model.OrderRequest or)
        {
            string msgBody = string.Format(NotificationHelper.orAuthMsgBody, or.RefNumber);
            NotificationService.SendNotification(NotificationService.GetApproverEmailAddress(1, NotificationHelper.orCode, NotificationHelper.authorizationCode), msgBody, NotificationHelper.orsubject);
        }

        public void NotifiyPreparer(Model.OrderRequest or)
        {

        }

        public bool AddOrderRequstItem(Model.OrderRequestItem entity, Model.OrderRequest ORentity)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        var orderRequest = context.OrderRequests.FirstOrDefault(o => o.Id == entity.OrderRequestId);
                        if (ORentity != null && orderRequest == null)
                        {
                            ORentity.TotalAmount = entity.EstimatedPrice;
                            context.OrderRequests.Add(ORentity);
                            entity.Id = Guid.NewGuid();
                            context.OrderRequestItems.Add(entity);
                            if ((context.SaveChanges() > 0))
                            {
                                scope.Complete();
                            }
                            else
                            {
                                scope.Dispose();
                                return false;
                            }
                        }
                        else
                        {
                            if (entity.Id.Equals(Guid.Empty))
                            {
                                entity.Id = Guid.NewGuid();
                                context.OrderRequestItems.Add(entity);
                                if ((context.SaveChanges() > 0))
                                {
                                    orderRequest.TotalAmount += entity.EstimatedPrice;
                                    if ((context.SaveChanges() > 0))
                                    { 
                                        scope.Complete();
                                    }
                                    else 
                                    { 
                                        scope.Dispose(); 
                                        return false; 
                                    }
                                }
                                else
                                {
                                    scope.Dispose();
                                    return false;
                                }
                            }
                            else
                            {
                                var orItem = context.OrderRequestItems.FirstOrDefault(p => p.Id == entity.Id);
                                orderRequest.TotalAmount -= orItem.EstimatedPrice;
                                orderRequest.TotalAmount += entity.EstimatedPrice;

                                orItem.BudgetLineId = entity.BudgetLineId;
                                orItem.EstimatedPrice = entity.EstimatedPrice;
                                orItem.EstimatedUnitPrice = entity.EstimatedUnitPrice;
                                orItem.ItemDescription = entity.ItemDescription;
                                orItem.ItemId = entity.ItemId;
                                orItem.Quantity = entity.Quantity;
                                orItem.Remarks = entity.Remarks;

                                if ((context.SaveChanges() > 0))
                                {
                                    scope.Complete();
                                }
                                else
                                {
                                    scope.Dispose();
                                    return false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw new Exception(ex.Message);
                    }
                }

                if (entity.ProcurementPlanItemId != null)
                {
                    ClearPPSessionData();
                    ClearPPItemSessionData();
                }
                ClearORSessionData();
                return true;
            }
        }

        public bool UpdateOrderRequestItem(Model.OrderRequestItem entity)
        {
            using (var context = new SCMSEntities())
            {
                var existing = context.OrderRequestItems.FirstOrDefault(o => o.Id == entity.Id);
                context.Entry(existing).CurrentValues.SetValues(entity);
                return (context.SaveChanges() > 0) ? true : false;
            }
        }

        public void DeleteOrderRequestItem(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                Model.OrderRequestItem orItem = dbContext.OrderRequestItems.SingleOrDefault(c => c.Id.Equals(id));
                var or = dbContext.OrderRequests.SingleOrDefault(o => o.Id == orItem.OrderRequestId);
                or.TotalAmount -= orItem.EstimatedPrice;

                dbContext.OrderRequestItems.Remove(orItem);
                dbContext.SaveChanges();
                ClearPPItemSessionData();
                ClearORSessionData();
            }
        }

        public string GenerateUniquNumber(CountryProgramme cp)
        {
            string code = "OR/DRC/" + cp.Country.ShortName + "/";
            string refNumber = "";
            long count = 1;
            using (var dbContext = new SCMSEntities())
            {
                var total = SessionData.CurrentSession.OrderRequestList.Where(p => p.IsSubmitted).Count();
                count = total;
                Model.OrderRequest m = null;
                do
                {
                    count++;
                    if (count < 100000)
                    {
                        if (count < 10)
                            refNumber = code + "0000" + count;
                        if (count < 100 && count >= 10)
                            refNumber = code + "000" + count;
                        if (count < 1000 && count >= 100)
                            refNumber = code + "00" + count;
                        if (count < 10000 && count >= 1000)
                            refNumber = code + "0" + count;
                        if (count < 100000 && count >= 10000)
                            refNumber = code + count;
                    }
                    m = SessionData.CurrentSession.OrderRequestList.FirstOrDefault(p => p.RefNumber == refNumber);
                } while (m != null);
                return refNumber;
            }
        }

        public List<Model.OrderRequest> Search(Model.OrderRequest entity)
        {
            List<Model.OrderRequest> orlist = SessionData.CurrentSession.OrderRequestList.Where(p => p.RefNumber.Contains(entity.RefNumber)).OrderByDescending(p => p.PreparedOn).ToList();
            return orlist;
        }

        public List<Model.OrderRequest> Search(String refNum)
        {
            List<Model.OrderRequest> orlist = SessionData.CurrentSession.OrderRequestList.Where(p => p.RefNumber.Contains(refNum))
                    .OrderByDescending(p => p.PreparedOn).ToList();
            return orlist;
        }


        public List<Model.OrderRequest> Search(DateTime fromDate, DateTime toDate, int startIndex = 0, int pageSize = 10)
        {
            List<Model.OrderRequest> orlist = SessionData.CurrentSession.OrderRequestList.Where(order => order.OrderDate >= fromDate && order.OrderDate <= toDate)
                    .OrderByDescending(p => p.PreparedOn).Skip(startIndex).Take(pageSize).ToList();
            return orlist;
        }

        public long SearchCount(DateTime fromDate, DateTime toDate)
        {
            long count = SessionData.CurrentSession.OrderRequestList.Where(order => order.OrderDate >= fromDate && order.OrderDate <= toDate).Count();
            return count;
        }

        public List<Model.OrderRequest> GetOrderRequests()
        {
            return SessionData.CurrentSession.OrderRequestList.OrderByDescending(p => p.PreparedOn).ToList();
        }

        public Model.OrderRequestItem GetOrderRequestItemById(Guid id)
        {
            using (var context = new SCMSEntities())
            {
                return context.OrderRequestItems.IncludeOrderRequest().IncludeItem().IncludeProjectDonor().FirstOrDefault(p => p.Id == id);
            }
        }

        public Model.OrderRequestItem GetORItemWithoutIncludes(Guid id)
        {
            using (var context = new SCMSEntities())
            {
                return context.OrderRequestItems.FirstOrDefault(o => o.Id == id);
            }
        }

        public bool ItemAlreadyAddedToOR(Guid itemId, Guid orId)
        {
            var or = SessionData.CurrentSession.OrderRequestList.FirstOrDefault(o => o.Id == orId);
            if (or != null)
            {
                var itemCount = or.OrderRequestItems.Where(p => p.ItemId == itemId).Count();
                return itemCount > 0 ? true : false;
            }
            return false;
        }

        public List<Model.CountrySubProgramme> CountrySubProgs(Guid id)
        {
            return SessionData.CurrentSession.CountrySubProgrammeList.OrderBy(p => p.Name).ToList();
        }

        public List<Model.BudgetLineView> GetProjectBugdetLines(Guid pdid)
        {
            try
            {
                List<BudgetLineView> blView = new List<BudgetLineView>();
                var blList = SessionData.CurrentSession.ProjectBudgetList.Where(p => p.BudgetCategory.ProjectDonorId == pdid).ToList();
                blList = SCMS.Utils.BudgetLineSorter.SortBudgetLine(blList);
                foreach (var bl in blList)
                {
                    blView.Add(new BudgetLineView
                    {
                        Id = bl.Id,
                        Description = bl.LineNumber + " " + bl.Description
                    });
                }
                return blView;
            }
            catch (Exception ex)
            {
                return new List<BudgetLineView>();
            }
        }

        public List<Model.OrderRequestItem> GetOrderRequestItems(Guid orId)
        {
            var or = SessionData.CurrentSession.OrderRequestList.FirstOrDefault(o => o.Id == orId);
            return or.OrderRequestItems.ToList<OrderRequestItem>();
        }

        public List<Model.ProjectDonor> GetProjectNos(Guid? projectId)
        {
            List<ProjectDonor> pd; DateTime tude = DateTime.Now.Date;
            if (projectId.HasValue)
                return SessionData.CurrentSession.ProjectDonorList.Where(p => p.ProjectId == projectId && p.EndDate >= DateTime.Today).ToList();
            else
                return SessionData.CurrentSession.ProjectDonorList.Where(p => p.EndDate >= DateTime.Today).OrderBy(p => p.ProjectNumber).ToList();
        }

        public List<Project> GetProject()
        {
            using (var db = new SCMSEntities())
            {
                return SessionData.CurrentSession.ProjectList.Where(p => p.ProjectDonors.Where(pd => pd.EndDate >= DateTime.Today).Count() > 0).OrderBy(p => p.Name).ToList();
            }
        }

        public List<Project> GetProjectsWithPP()
        {
            return SessionData.CurrentSession.ProjectList.Where(p => p.ProjectDonors.Where(r => r.EndDate >= DateTime.Today && r.ProcurementPlans.Where(pp => pp.IsAuthorized && pp.ProcurementPlanItems.Count > 0).Count() > 0).Count() > 0).OrderBy(p => p.Name).ToList();
        }

        public List<ProjectDonor> GetProjectNosWithPP(Guid? projectId = null)
        {
            DateTime tude = DateTime.Now.Date;
            if (projectId != null)
                return SessionData.CurrentSession.ProjectDonorList.Where(p => p.ProjectId == projectId && p.ProcurementPlans.Where(pp => pp.IsAuthorized && pp.ProcurementPlanItems.Count > 0).Count() > 0).ToList();
            else
                return SessionData.CurrentSession.ProjectDonorList.Where(p => p.ProcurementPlans.Where(pp => pp.IsAuthorized && pp.ProcurementPlanItems.Count > 0).Count() > 0).OrderBy(p => p.ProjectNumber).ToList();
        }

        public List<Project> GetProjectsWithoutPP()
        {
            return SessionData.CurrentSession.ProjectList
                                .Where(p => SessionData.CurrentSession.ProcurementPlanList
                                    .Count(pp => pp.ProjectDonor.ProjectId == p.Id
                                    && pp.ProjectDonor.EndDate >= DateTime.Today) == 0
                                    && p.ProjectDonors.Count(pd => pd.EndDate >= DateTime.Today) > 0)
                                .OrderBy(p => p.Name).ToList();
        }

        public List<ProjectDonor> GetProjectNosWithoutPP(Guid projectId)
        {
            return SessionData.CurrentSession.ProjectDonorList.Where(p => p.ProjectId == projectId && p.EndDate >= DateTime.Today 
                                            && !SessionData.CurrentSession.ProcurementPlanList.Select(pp => pp.ProjectDonorId)
                                            .ToList().Contains(p.Id)).ToList();
        }

        public Model.ProjectDonor GetProjectDonorById(Guid id)
        {
            return SessionData.CurrentSession.ProjectDonorList.FirstOrDefault(p => p.Id == id);
        }

        public Model.ProjectBudget GetProjectBudgetDetails(Guid id)
        {
            return SessionData.CurrentSession.ProjectBudgetList.FirstOrDefault(p => p.Id == id);
        }

        public List<Model.UnitOfMeasure> GetUnitMesures()
        {
            return SessionData.CurrentSession.UnitOfMeasureList.ToList();
        }

        public List<Model.Currency> GetCurrencies()
        {
            return SessionData.CurrentSession.CurrencyList.OrderBy(c => c.ShortName).ToList();
        }

        public List<Model.Location> GetLocations()
        {
            return SessionData.CurrentSession.LocationList.OrderBy(l => l.Name).ToList();
        }

        public List<Model.Item> GetItems(string category = null)
        {

            switch (category)
            {
                case "C":
                    return SessionData.CurrentSession.ItemList.Where(p => p.ItemCategory.CategoryCode.StartsWith("C")).OrderBy(i => i.Name).ToList();
                case "A":
                    return SessionData.CurrentSession.ItemList.Where(p => p.ItemCategory.CategoryCode.StartsWith("A")).OrderBy(i => i.Name).ToList();
                default:
                    return SessionData.CurrentSession.ItemList.OrderBy(i => i.Name).ToList();
            }

        }

        public List<Model.OrderRequest> GetOrderRequestsForApproval(SystemUser currentUser)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    List<Model.OrderRequest> orderRequests = new List<Model.OrderRequest>();
                    context.SystemUsers.Attach(currentUser);
                    var approvers = context.Approvers.Where(a => a.UserId == currentUser.Id && a.ActivityCode == NotificationHelper.orCode && a.ActionType == NotificationHelper.approvalCode).ToList();
                    var delegateApprovers = context.Approvers.Where(a => a.AssistantId == currentUser.Id && a.ActivityCode == NotificationHelper.orCode && a.ActionType == NotificationHelper.approvalCode).ToList();
                    if (approvers != null)
                    {
                        foreach (var approver in approvers)
                        {
                            var orList = context.OrderRequests
                                            .IncludeStaff()
                                            .IncludeOrderRequestItemsWithProject()
                                            .Where(o => o.CountryProgrammeId == SessionData.CurrentSession.CountryProgrammeId && (o.IsApproved == false && o.IsSubmitted == true && o.IsRejected == false)
                                && o.Notifications.Where(n => (Guid)n.OrderRequestId == o.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                            foreach (var or in orList)
                                orderRequests.Add(or);
                        }
                    }
                    if (delegateApprovers != null)
                    {
                        foreach (var delegateApprover in delegateApprovers)
                        {
                            var orList = context.OrderRequests
                                            .IncludeStaff()
                                            .IncludeOrderRequestItemsWithProject()
                                            .Where(o => o.CountryProgrammeId == SessionData.CurrentSession.CountryProgrammeId && (o.IsApproved == false && o.IsSubmitted == true && o.IsRejected == false)
                                && o.Notifications.Where(n => (Guid)n.OrderRequestId == o.Id && n.IsRespondedTo == false && n.SentToDelegate == true && n.ApproverId == delegateApprover.Id).Count() > 0).ToList();
                            foreach (var or in orList)
                                orderRequests.Add(or);
                        }
                    }
                    return orderRequests;
                }
            }
            catch (Exception ex)
            {
                return new List<Model.OrderRequest>();
            }
        }

        public List<Model.OrderRequest> GetOrderRequestsForReview(SystemUser currentUser)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    List<Model.OrderRequest> orderRequests = new List<Model.OrderRequest>();
                    context.SystemUsers.Attach(currentUser);
                    var approvers = context.Approvers.Where(a => a.UserId == currentUser.Id && a.ActivityCode == NotificationHelper.orCode && a.ActionType == NotificationHelper.reviewCode).ToList();
                    var delegateApprovers = context.Approvers.Where(a => a.AssistantId == currentUser.Id && a.ActivityCode == NotificationHelper.orCode && a.ActionType == NotificationHelper.reviewCode).ToList();
                    if (approvers != null)
                    {
                        foreach (var approver in approvers)
                        {
                            var orList = context.OrderRequests
                                            .IncludeStaff()
                                            .IncludeOrderRequestItemsWithProject()
                                            .Where(o => o.CountryProgrammeId == SessionData.CurrentSession.CountryProgrammeId && (o.IsReviewed == false && o.IsApproved == true && o.IsRejected == false)
                                && o.Notifications.Where(n => (Guid)n.OrderRequestId == o.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                            foreach (var or in orList)
                                orderRequests.Add(or);
                        }
                    }
                    if (delegateApprovers != null)
                    {
                        foreach (var delegateApprover in delegateApprovers)
                        {
                            var orList = context.OrderRequests
                                            .IncludeStaff()
                                            .IncludeOrderRequestItemsWithProject()
                                            .Where(o => o.CountryProgrammeId == SessionData.CurrentSession.CountryProgrammeId && (o.IsReviewed == false && o.IsApproved == true && o.IsRejected == false)
                                && o.Notifications.Where(n => (Guid)n.OrderRequestId == o.Id && n.IsRespondedTo == false && n.SentToDelegate == true && n.ApproverId == delegateApprover.Id).Count() > 0).ToList();
                            foreach (var or in orList)
                                orderRequests.Add(or);
                        }
                    }
                    return orderRequests;
                }
            }
            catch (Exception ex)
            {
                return new List<Model.OrderRequest>();
            }
        }

        public List<Model.OrderRequest> GetOrderRequestsForAuth(SystemUser currentUser)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    List<Model.OrderRequest> orderRequests = new List<Model.OrderRequest>();
                    context.SystemUsers.Attach(currentUser);
                    var approvers = context.Approvers.Where(a => a.UserId == currentUser.Id && a.ActivityCode == NotificationHelper.orCode && a.ActionType == NotificationHelper.authorizationCode).ToList();
                    var delegateApprovers = context.Approvers.Where(a => a.AssistantId == currentUser.Id && a.ActivityCode == NotificationHelper.orCode && a.ActionType == NotificationHelper.authorizationCode).ToList();
                    if (approvers != null)
                    {
                        foreach (var approver in approvers)
                        {
                            var orList = context.OrderRequests
                                            .IncludeStaff()
                                            .IncludeOrderRequestItemsWithProject()
                                            .Where(o => o.CountryProgrammeId == SessionData.CurrentSession.CountryProgrammeId && (o.IsReviewed == true && o.IsRejected == false && o.IsAuthorized == false)
                                && o.Notifications.Where(n => (Guid)n.OrderRequestId == o.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                            foreach (var or in orList)
                                orderRequests.Add(or);
                        }
                    }
                    if (delegateApprovers != null)
                    {
                        foreach (var delegateApprover in delegateApprovers)
                        {
                            var orList = context.OrderRequests
                                            .IncludeStaff()
                                            .IncludeOrderRequestItemsWithProject()
                                            .Where(o => o.CountryProgrammeId == SessionData.CurrentSession.CountryProgrammeId && (o.IsReviewed == true && o.IsRejected == false && o.IsAuthorized == false)
                                && o.Notifications.Where(n => (Guid)n.OrderRequestId == o.Id && n.IsRespondedTo == false && n.SentToDelegate == true && n.ApproverId == delegateApprover.Id).Count() > 0).ToList();
                            foreach (var or in orList)
                                orderRequests.Add(or);
                        }
                    }
                    return orderRequests;
                }
            }
            catch (Exception ex)
            {
                return new List<Model.OrderRequest>();
            }
        }

        /// <summary>
        /// Use raw SQL for speed and custom pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public OrderRequestPagePacket getPagedOrderRequests(int page = 1, int size = 25, Dictionary<String, String> args = null)
        {
            // Raw sql
            string qString = @"SELECT * FROM (SELECT oq.Id, RefNumber, TotalAmount AS ORValue, pd.ProjectNumber,
                                CASE 
	                                WHEN oq.IsAuthorized=1 THEN 'AU'
	                                WHEN oq.IsRejected=1 THEN 'RJ'
	                                WHEN oq.IsReviewed=1 THEN 'RV'
	                                WHEN oq.IsApproved=1 THEN 'AP'
	                                WHEN oq.IsSubmitted=1 THEN 'CR'
	                                ELSE 'NEW' END 
                                AS Status, 
                                CASE 
	                                WHEN oq.IsAuthorized=1 THEN AuthorizedOn
	                                WHEN oq.IsRejected=1 AND oq.AuthorizedOn IS NOT NULL THEN oq.AuthorizedOn
	                                WHEN oq.IsRejected=1 AND oq.ReviewedOn IS NOT NULL THEN oq.ReviewedOn
	                                WHEN oq.IsRejected=1 AND oq.ApprovedOn IS NOT NULL THEN oq.ApprovedOn
	                                WHEN oq.IsReviewed=1 THEN oq.ReviewedOn
	                                WHEN oq.IsApproved=1 THEN oq.ApprovedOn
	                                WHEN oq.IsSubmitted=1 THEN oq.PreparedOn
	                                ELSE oq.PreparedOn END 
                                AS StatusDate,
                                pn.FirstName+' '+ pn.OtherNames AS Requestor,
                                ri2.Name AS FirstItem,
                                oq.CountryProgrammeId,
                                ROW_NUMBER() OVER(ORDER BY oq.RefNumber) AS RowNum
                                FROM orderRequest oq
                                JOIN projectDonor pd ON oq.ProjectDonorId=pd.Id 
                                JOIN Staff sf ON sf.id= oq.PreparedBy
                                JOIN Person pn ON pn.Id = sf.PersonId
                                CROSS APPLY (select top(1) ri.ItemId, ri.Id,ri.OrderRequestId, im.Name from orderrequestitem ri JOIN Item im ON im.Id=ri.ItemId WHERE ri.OrderRequestId=oq.Id) ri2 
                                ) orders
                                WHERE RowNum>({PAGE_NUM}-1)*{PAGE_SIZE} AND RowNum<={PAGE_NUM}*{PAGE_SIZE}  {PREDICATE_QUERY}";

            String totalQuery = "SELECT COUNT(*) AS TotalCount FROM OrderRequest {PREDICATE_TOTAL}";

            int _page = page;

            if (_page < 1)
            {
                _page = 1;
            }

            int _size = 10;

            if (size > 10)
            {
                _size = size;
            }


            qString = qString.Replace("{PAGE_SIZE}", _size.ToString());

            Dictionary<String, String> options = new Dictionary<string, string>();

            if (args != null)
            {
                options = args;
            }

            if (!options.ContainsKey("CountryProgrammeId"))
            {
                //force
            }


            totalQuery = totalQuery.Replace("{PREDICATE_TOTAL}", "WHERE CountryProgrammeId='" + options["CountryProgrammeId"] + "'");

            String queryPredicate = " AND CountryProgrammeId='" + options["CountryProgrammeId"] + "'";

            qString = qString.Replace("{PREDICATE_QUERY}", queryPredicate);

            OrderRequestPagePacket pack = new OrderRequestPagePacket();

            using (var context = new SCMSEntities())
            {
                pack.TotalOrders = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<Int32>(totalQuery).First<Int32>();

                if ((pack.TotalOrders - _page * _size) <= _size)
                {
                    _page++;
                }

                qString = qString.Replace("{PAGE_NUM}", _page.ToString());

                pack.Orders = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<OrderRequestSummary>(qString).ToList<OrderRequestSummary>();
            }

            return pack;
        }

        public Model.OrderRequest GetOrderRequestById(Guid id)
        {
            try
            {
                return SessionData.CurrentSession.OrderRequestList.FirstOrDefault(o => o.Id == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void AuthorizeOrderRequest(Model.OrderRequest or)
        {
            try
            {
                decimal? amount;
                decimal? currAmount;//Amount in ProjectBudget currency
                using (var context = new SCMSEntities())
                {
                    var orItems = context.OrderRequestItems.Where(i => i.OrderRequestId == or.Id).ToList();
                    foreach (var orItem in orItems)
                    {
                        amount = orItem.EstimatedPrice;
                        currAmount = exchangeRateService.GetForeignCurrencyValue(orItem.ProjectBudget.BudgetCategory.ProjectDonor.Currency, orItem.OrderRequest.Currency, amount, (Guid)or.CountryProgrammeId);
                        orItem.ProjectBudget.TotalCommitted += currAmount;

                        //Add to BudgetCommitment
                        var budgetCommitment = new BudgetCommitment();
                        budgetCommitment.Id = Guid.NewGuid();
                        budgetCommitment.OrderRequestItemId = orItem.Id;
                        budgetCommitment.AmountCommitted = (decimal)currAmount;
                        budgetCommitment.DateCommitted = DateTime.Now;
                        budgetCommitment.BudgetLineId = orItem.ProjectBudget.Id;
                        context.BudgetCommitments.Add(budgetCommitment);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void UndoAuthorization(Model.OrderRequest or)
        {
            try
            {
                decimal? amount;
                decimal? currAmount;//Amount in ProjectBudget currency
                using (var context = new SCMSEntities())
                {
                    var orItems = context.OrderRequestItems.Where(i => i.OrderRequestId == or.Id).ToList();
                    foreach (var orItem in orItems)
                    {
                        amount = orItem.EstimatedPrice;
                        currAmount = exchangeRateService.GetForeignCurrencyValue(orItem.ProjectBudget.BudgetCategory.ProjectDonor.Currency, orItem.OrderRequest.Currency, amount, (Guid)or.CountryProgrammeId);
                        orItem.ProjectBudget.TotalCommitted -= currAmount;

                        var budgetCommitmentList = context.BudgetCommitments.Where(b => b.OrderRequestItemId == orItem.Id).ToList();

                        foreach (var budgetCommitment in budgetCommitmentList)
                        {
                            context.BudgetCommitments.Remove(budgetCommitment);
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public float GetLastPOItemPrice(Guid itemId)
        {
            using (var db = new SCMSEntities())
            {
                double query = (from orit in db.OrderRequestItems
                                join poit in db.PurchaseOrderItems on orit.Id equals poit.OrderRequestItemId
                                join po in db.PurchaseOrders on poit.PurchaseOrderId equals po.Id
                                where orit.ItemId == itemId
                                orderby po.PreparedOn descending
                                select poit.UnitPrice).FirstOrDefault();
                return (float)((0.1 * query) + query);

            }
        }

        public void BackDateOR(Model.OrderRequest or)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        //Get Current OR from DB
                        var currentOR = context.OrderRequests.FirstOrDefault(o => o.Id == or.Id);
                        //Initialize BackDate object
                        var backDate = new DocumentBackDating();
                        backDate.Id = Guid.NewGuid();
                        backDate.BackDatedBy = or.BackDatedBy;
                        backDate.BackDatedOn = DateTime.Now;
                        backDate.NewDate = or.OrderDate.Value;
                        backDate.OrderRequestId = or.Id;
                        backDate.PreviousDate = currentOR.OrderDate.Value;
                        backDate.Reason = or.BackDatingReason;
                        //Insert BackDate details into db
                        context.DocumentBackDatings.Add(backDate);
                        //Update date on OR in db
                        currentOR.OrderDate = or.OrderDate;
                        context.SaveChanges();
                        scope.Complete();
                        ClearORSessionData();
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw ex;
                    }
                }
            }
        }

        public List<BudgetCheckResult> RunFundsAvailableCheck(Guid orId)
        {
            var bcrList = new List<BudgetCheckResult>();
            using (var context = new SCMSEntities())
            {
                var orItems = context.OrderRequestItems.IncludeOrderRequest().IncludeProjectDonor().Where(o => o.OrderRequestId == orId);
                var or = orItems.ToList()[0].OrderRequest;
                //Construct list of project budgets for that OR
                List<ProjectBudget> pbList = new List<ProjectBudget>();
                foreach (var orItem in orItems.ToList())
                {
                    if (!pbList.Contains(orItem.ProjectBudget))
                        pbList.Add(orItem.ProjectBudget);
                }
                foreach (var pb in pbList)
                {
                    decimal totalAmount = orItems.Where(b => b.BudgetLineId == pb.Id).Sum(b => b.EstimatedPrice);
                    if (!budgetService.SufficientFundsAvailable(totalAmount, pb.Id.ToString(), or.CurrencyId.ToString(), Guid.Empty, Guid.Empty, Guid.Empty))
                    {
                        decimal availableFunds = pb.TotalBudget - (decimal)(pb.TotalCommitted + pb.TotalPosted);
                        availableFunds = (decimal)exchangeRateService.GetForeignCurrencyValue(or.CurrencyId, pb.BudgetCategory.ProjectDonor.CurrencyId.Value, availableFunds, or.CountryProgrammeId.Value);
                        bcrList.Add(new BudgetCheckResult { ProjectBudget = pb, AmountChecked = totalAmount, AvailableAmount = availableFunds, FundsAvailable = false });
                    }
                }
            }
            return bcrList;
        }

        public bool CanPreparePO(Guid orId)
        {
            var or = SessionData.CurrentSession.OrderRequestList.FirstOrDefault(o => o.Id == orId && o.IsAuthorized == true);
            if (or != null)
            {
                foreach (var orItem in or.OrderRequestItems)
                {
                    if (orItem.PurchaseOrderItems == null || orItem.PurchaseOrderItems.Count == 0)
                        return true;
                    var quantity = orItem.PurchaseOrderItems.Sum(p => p.Quantity);
                    if (quantity < orItem.Quantity)
                        return true;
                }
            }
            return false;
        }
    }
}
