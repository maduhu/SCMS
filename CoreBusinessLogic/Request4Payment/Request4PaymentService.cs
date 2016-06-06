using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using System.Data.Objects.DataClasses;
using System.Transactions;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.CoreBusinessLogic._ExchangeRate;
using SCMS.Utils.DTOs;
using SCMS.CoreBusinessLogic.Budgeting;
using System.Data.Entity.Infrastructure;

namespace SCMS.CoreBusinessLogic.Request4Payment
{
    public class Request4PaymentService : IRequest4PaymentService
    {
        private INotificationService notificationService;
        private IExchangeRateService exchangeRateService;
        private IBudgetService budgetService;

        public Request4PaymentService(INotificationService notificationService, IExchangeRateService exchangeRateService, IBudgetService budgetService)
        {
            this.notificationService = notificationService;
            this.exchangeRateService = exchangeRateService;
            this.budgetService = budgetService;
        }

        public bool SaveRequest4Payment(Model.PaymentRequest entity, EntityCollection<PaymentRequestBudgetLine> collection)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        context.PaymentRequests.Add(entity);
                        //if (context.SaveChanges() > 0)
                        //{
                        foreach (PaymentRequestBudgetLine item in collection)
                        {
                            context.PaymentRequestBudgetLines.Add(item);
                            if (!(context.SaveChanges() > 0)) { scope.Complete(); return false; }
                        }
                        //Send notification to Finance
                        notificationService.SendToAppropriateApprover(NotificationHelper.rfpCode, NotificationHelper.reviewCode, entity.Id);
                        scope.Complete();
                        return true;
                        //}
                        //else { scope.Dispose(); return false; }
                    }
                    catch (Exception ex) { scope.Dispose(); throw new Exception(ex.Message); }
                }
            }
        }

        public string GenerateUniquNumber(CountryProgramme cp)
        {
            string code = "RFP/DRC/" + cp.Country.ShortName + "/";
            string refNumber = "";
            long count = 1;
            using (var dbContext = new SCMSEntities())
            {
                var total = dbContext.PaymentRequests.Where(p => p.CountryProgrammeId == cp.Id && p.IsSubmitted == true).Count();
                count = total;
                Model.PaymentRequest m = null;
                do
                {
                    count++;
                    if (count < 100000)
                    {
                        if (count < 10)
                            refNumber = code + "0000" + count;
                        if (count < 100 & count >= 10)
                            refNumber = code + "000" + count;
                        if (count < 1000 & count >= 100)
                            refNumber = code + "00" + count;
                        if (count < 10000 & count >= 1000)
                            refNumber = code + "0" + count;
                        if (count < 100000 && count >= 10000)
                            refNumber = code + count;
                    }
                    m = dbContext.PaymentRequests.FirstOrDefault(p => p.RefNumber == refNumber);
                } while (m != null);
                return refNumber;
            }
        }

        public List<Model.PaymentType> GetPaymentType(Guid countryProgId)
        {
            using (var context = new SCMSEntities())
            {
                List<Model.PaymentType> PTyp = context.PaymentTypes.Where(p => p.CountryProgrammeId == countryProgId).OrderBy(p => p.Description).ToList();
                return PTyp;
            }
        }

        public List<Model.PaymentRequest> GetPaymentRequests(Guid countryProgId)
        {
            using (var context = new SCMSEntities())
            {
                List<Model.PaymentRequest> PR = context.PaymentRequests.Where(p => p.CountryProgrammeId == countryProgId).OrderByDescending(p => p.PreparedOn).ToList();
                foreach (PaymentRequest item in PR)
                {
                    Currency c = item.Currency;
                    PaymentType pt = item.PaymentType;
                    Supplier sup = item.Supplier;
                    if (item.Staff != null)
                    {
                        var person = item.Staff.Person;
                        var desg = item.Staff.Designation;
                    }
                    if (item.Staff1 != null)
                    {
                        var person = item.Staff1.Person;
                        var desg = item.Staff1.Designation;
                    }
                    if (item.Staff2 != null)
                    {
                        var person = item.Staff2.Person;
                        var desg = item.Staff2.Designation;
                    }
                    if (item.Staff3 != null)
                    {
                        var person = item.Staff3.Person;
                        var desg = item.Staff3.Designation;
                    }
                    if (item.PurchaseOrder != null)
                    {
                        var pd = item.PurchaseOrder.ProjectDonor;
                    }
                }
                return PR;
            }
        }

        public List<Model.PaymentRequest> GetPostedPaymentRequests(Guid countryProgId)
        {
            using (var context = new SCMSEntities())
            {
                List<Model.PaymentRequest> PR = context.PaymentRequests.Where(p => p.CountryProgrammeId == countryProgId && p.FundsPosted == true).OrderByDescending(p => p.PreparedOn).ToList();
                foreach (PaymentRequest item in PR)
                {
                    Currency c = item.Currency;
                    PaymentType pt = item.PaymentType;
                    Supplier sup = item.Supplier;
                    if (item.Staff != null)
                    {
                        var person = item.Staff.Person;
                        var desg = item.Staff.Designation;
                    }
                    if (item.Staff1 != null)
                    {
                        var person = item.Staff1.Person;
                        var desg = item.Staff1.Designation;
                    }
                    if (item.Staff2 != null)
                    {
                        var person = item.Staff2.Person;
                        var desg = item.Staff2.Designation;
                    }
                    if (item.Staff3 != null)
                    {
                        var person = item.Staff3.Person;
                        var desg = item.Staff3.Designation;
                    }
                    if (item.PurchaseOrder != null)
                    {
                        var pd = item.PurchaseOrder.ProjectDonor;
                    }
                }
                return PR;
            }
        }

        public ICollection<PurchaseOrderItem> GetPurchaseOItems(string PORefNumber)
        {
            using (var context = new SCMSEntities())
            {
                Model.PurchaseOrder po = context.PurchaseOrders.SingleOrDefault(p => p.RefNumber == PORefNumber);
                ICollection<PurchaseOrderItem> poitem = po.PurchaseOrderItems;
                return poitem;
            }
        }

        public List<Model.PaymentRequest> GetPaymentRequests4Review(SystemUser currentUser)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    List<Model.PaymentRequest> requestsForPayment = new List<Model.PaymentRequest>();
                    context.SystemUsers.Attach(currentUser);
                    var approvers = context.Approvers.Where(a => a.UserId == currentUser.Id && a.ActivityCode == NotificationHelper.rfpCode && a.ActionType == NotificationHelper.reviewCode).ToList();
                    var delegateApprovers = context.Approvers.Where(a => a.AssistantId == currentUser.Id && a.ActivityCode == NotificationHelper.rfpCode && a.ActionType == NotificationHelper.reviewCode).ToList();
                    if (approvers != null)
                    {
                        foreach (var approver in approvers)
                        {
                            var rfpList = context.PaymentRequests.Where(r => r.CountryProgrammeId == currentUser.Staff.CountrySubOffice.CountryProgrammeId && (r.IsReviewed == false && r.IsRejected == false)
                                && r.Notifications.Where(n => (Guid)n.PaymentRequestId == r.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                            foreach (var rfp in rfpList)
                            {
                                requestsForPayment.Add(rfp);
                            }
                        }
                    }
                    if (delegateApprovers != null)
                    {
                        foreach (var delegateApprover in delegateApprovers)
                        {
                            var rfpList = context.PaymentRequests.Where(r => r.CountryProgrammeId == currentUser.Staff.CountrySubOffice.CountryProgrammeId && (r.IsReviewed == false && r.IsRejected == false)
                                && r.Notifications.Where(n => (Guid)n.PaymentRequestId == r.Id && n.IsRespondedTo == false && n.SentToDelegate == true && n.ApproverId == delegateApprover.Id).Count() > 0).ToList();
                            foreach (var rfp in rfpList)
                            {
                                requestsForPayment.Add(rfp);
                            }
                        }
                    }
                    return requestsForPayment;
                }
            }
            catch (Exception ex)
            {
                return new List<Model.PaymentRequest>();
            }
        }

        public List<Model.PaymentRequest> GetPaymentRequests4Authorization(SystemUser currentUser)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    List<Model.PaymentRequest> requestsForPayment = new List<Model.PaymentRequest>();
                    context.SystemUsers.Attach(currentUser);
                    var approvers = context.Approvers.Where(a => a.UserId == currentUser.Id && a.ActivityCode == NotificationHelper.rfpCode && a.ActionType == NotificationHelper.authorizationCode).ToList();
                    var delegateApprovers = context.Approvers.Where(a => a.AssistantId == currentUser.Id && a.ActivityCode == NotificationHelper.rfpCode && a.ActionType == NotificationHelper.authorizationCode).ToList();
                    if (approvers != null)
                    {
                        foreach (var approver in approvers)
                        {
                            var rfpList = context.PaymentRequests.Where(r => r.CountryProgrammeId == currentUser.Staff.CountrySubOffice.CountryProgrammeId && (r.IsReviewed == true && r.IsRejected == false && r.IsAuthorized != null && r.IsAuthorized == false)
                                && r.Notifications.Where(n => (Guid)n.PaymentRequestId == r.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                            foreach (var rfp in rfpList)
                            {
                                requestsForPayment.Add(rfp);
                            }
                        }
                    }
                    if (delegateApprovers != null)
                    {
                        foreach (var delegateApprover in delegateApprovers)
                        {
                            var rfpList = context.PaymentRequests.Where(r => r.CountryProgrammeId == currentUser.Staff.CountrySubOffice.CountryProgrammeId && (r.IsReviewed == true && r.IsRejected == false && r.IsAuthorized != null && r.IsAuthorized == false)
                                && r.Notifications.Where(n => (Guid)n.PaymentRequestId == r.Id && n.IsRespondedTo == false && n.SentToDelegate == true && n.ApproverId == delegateApprover.Id).Count() > 0).ToList();
                            foreach (var rfp in rfpList)
                            {
                                requestsForPayment.Add(rfp);
                            }
                        }
                    }
                    return requestsForPayment;
                }
            }
            catch (Exception ex)
            {
                return new List<Model.PaymentRequest>();
            }
        }

        public List<PaymentRequest> GetPaymentRequestsForPosting(Guid countryProgId, SystemUser currentUser)
        {
            using (var context = new SCMSEntities())
            {
                List<PaymentRequest> paymentRequests = new List<PaymentRequest>();
                var approvers = context.Approvers.Where(a => a.CountryProgrammeId == countryProgId && a.ActivityCode == NotificationHelper.rfpCode && a.ActionType == NotificationHelper.postFundsCode
                    && (a.UserId == currentUser.Id || a.AssistantId == currentUser.Id));
                var financeLimit = context.SystemUsers.FirstOrDefault(s => s.Id == currentUser.Id).Staff.FinanceLimit;
                if (financeLimit == null)
                    return new List<PaymentRequest>();
                if (approvers.ToList().Count == 0)
                    return new List<PaymentRequest>();
                if (approvers.Where(a => a.ProjectDonorId == null).Count() > 0)
                {
                    paymentRequests = context.PaymentRequests.Where(p => p.CountryProgrammeId == countryProgId && p.IsAuthorized == true && p.FundsPosted == false
                        && (p.MBValue.Value <= financeLimit.Limit || financeLimit.Limit == 0)).ToList();
                    foreach (var rfp in paymentRequests)
                    {
                        var supplier = rfp.Supplier;
                        var payt = rfp.PaymentType;
                        var curr = rfp.Currency;
                    }
                    return paymentRequests;
                }

                foreach (var approver in approvers.ToList())
                {
                    var rfps = context.PaymentRequests.Where(p => p.CountryProgrammeId == countryProgId && p.IsAuthorized == true && p.FundsPosted == false
                        && (p.MBValue.Value <= financeLimit.Limit || financeLimit.Limit == 0)
                        && p.PaymentRequestBudgetLines.Where(i => i.ProjectBudget.BudgetCategory.ProjectDonorId.Value == approver.ProjectDonorId.Value).Count() > 0).ToList();
                    foreach (var rfp in rfps)
                    {
                        if (!RfpExistsInList(rfp, paymentRequests))
                            paymentRequests.Add(rfp);
                    }
                }

                foreach (var rfp in paymentRequests)
                {
                    var supplier = rfp.Supplier;
                    var payt = rfp.PaymentType;
                    var curr = rfp.Currency;
                }

                return paymentRequests;
            }
        }

        private bool RfpExistsInList(PaymentRequest rfp, List<PaymentRequest> rfpList)
        {
            foreach (var e in rfpList)
            {
                if (e.Equals(rfp))
                    return true;
            }
            return false;
        }

        public PaymentRequest GetRFPById(Guid Id)
        {

            using (var context = new SCMSEntities())
            {
                return context.PaymentRequests
                    .IncludeCurrency()
                    .IncludePaymentType()
                    .IncludePaymentTerm()
                    .IncludeSupplier()
                    .IncludePurchaseOrder()
                    .IncludeStaff()
                    .IncludeStaffDesignation()
                    .IncludeStaff1()
                    .IncludeStaff1Designation()
                    .IncludeStaff2()
                    .IncludeStaff2Designation()
                    .IncludeStaff3()
                    .IncludeStaff3Designation()
                    .FirstOrDefault(pr => pr.Id == Id);
            }

        }

        public List<PaymentRequestBudgetLine> GetRFPDetails(Guid RfpId)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    var rfpBLs = context.PaymentRequestBudgetLines.Where(p => p.PaymentRequestId == RfpId).ToList();
                    foreach (var rfpBL in rfpBLs)
                    {
                        var pd = rfpBL.ProjectBudget.BudgetCategory.ProjectDonor;
                    }
                    return rfpBLs;
                }
            }
            catch (Exception ex)
            {
                return new List<PaymentRequestBudgetLine>();
            }
        }

        public PaymentRequestBudgetLine GetRFPDetailById(Guid Id)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    var prb = context.PaymentRequestBudgetLines.FirstOrDefault(p => p.Id == Id);
                    var pd = prb.ProjectBudget.BudgetCategory.ProjectDonor;
                    return prb;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool SaveRFP(PaymentRequest rfp)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    context.PaymentRequests.Attach(rfp);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(rfp, System.Data.EntityState.Modified);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SaveRFPDetail(PaymentRequestBudgetLine rfpDetail)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    context.PaymentRequestBudgetLines.Attach(rfpDetail);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(rfpDetail, System.Data.EntityState.Modified);
                    return context.SaveChanges() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CommitFunds(PaymentRequest rfp)
        {
            decimal rfpCommited = 0;
            decimal poCommitAmt = 0;//this amount is in the currency of the rfp;
            using (var context = SCMSEntities.Define())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == rfp.PurchaseOrderId);
                        var poItemCommits = context.BudgetCommitments.Where(c => c.PurchaseOrderItem.PaymentRequestBudgetLines.Where(p => p.PaymentRequestId == rfp.Id).Count() > 0).ToList();
                        //REMOVE FUNDS FROM PO COMMITTED
                        foreach (var poItemCommit in poItemCommits)
                        {
                            var rfpBL = poItemCommit.PurchaseOrderItem.PaymentRequestBudgetLines.FirstOrDefault(p => p.PurchaseOrderItemId == poItemCommit.PurchaseOrderItemId);
                            if (rfpBL == null)
                                continue;
                            //Get committed amount in rfp currency
                            poCommitAmt = (decimal)exchangeRateService.GetForeignCurrencyValue(po.Currency, poItemCommit.ProjectBudget.BudgetCategory.ProjectDonor.Currency, poItemCommit.AmountCommitted, rfp.CountryProgrammeId);
                            rfpCommited += poCommitAmt;
                            var pb = context.ProjectBudgets.FirstOrDefault(p => p.Id == poItemCommit.BudgetLineId);
                            if (rfpBL.Amount >= poCommitAmt)
                            {
                                //Delete commitment
                                context.BudgetCommitments.Remove(poItemCommit);
                                //Deduct value from budget commitment
                                pb.TotalCommitted -= poItemCommit.AmountCommitted;
                            }
                            else
                            {
                                //convert rfpBL.Amount to project budget currency and deduct it
                                poCommitAmt = (decimal)exchangeRateService.GetForeignCurrencyValue(poItemCommit.ProjectBudget.BudgetCategory.ProjectDonor.Currency, po.Currency, rfpBL.Amount, rfp.CountryProgrammeId);
                                //Deduct this amount from budget commitments and also from project budget commitments
                                poItemCommit.AmountCommitted -= poCommitAmt;
                                pb.TotalCommitted -= poCommitAmt;
                            }
                        }
                        //MOVE FUNDS TO COMMITTED
                        var rfpBudgetLines = context.PaymentRequestBudgetLines.Where(p => p.PaymentRequestId == rfp.Id).ToList();
                        foreach (var rfpBL in rfpBudgetLines)
                        {
                            var budgetLine = context.ProjectBudgets.FirstOrDefault(pb => pb.Id == rfpBL.BudgetLineId);
                            var newCommit = new BudgetCommitment();
                            newCommit.Id = Guid.NewGuid();
                            newCommit.AmountCommitted = (decimal)exchangeRateService.GetForeignCurrencyValue(budgetLine.BudgetCategory.ProjectDonor.Currency, po.Currency, rfpBL.Amount, rfp.CountryProgrammeId);
                            newCommit.DateCommitted = DateTime.Now;
                            newCommit.RFPBudgetLineId = rfpBL.Id;
                            newCommit.BudgetLineId = budgetLine.Id;
                            context.BudgetCommitments.Add(newCommit);
                            budgetLine.TotalCommitted += newCommit.AmountCommitted;
                        }
                        //SAVE ALL CHANGES
                        if (context.SaveChanges() > 0)
                        {
                            scope.Complete();
                            return true;
                        }
                        else
                        {
                            scope.Dispose();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw ex;
                    }
                }
            }
        }

        public bool EffectPosting(PaymentRequest rfp, Staff poster)
        {
            using (var context = SCMSEntities.Define())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        var rfpCommits = context.BudgetCommitments.Where(b => b.PaymentRequestBudgetLine.PaymentRequestId == rfp.Id).ToList();
                        foreach (var rfpCommit in rfpCommits)
                        {
                            var pb = rfpCommit.ProjectBudget;
                            pb.TotalCommitted -= rfpCommit.AmountCommitted;

                            var posting = new BudgetPosting();
                            posting.AmountPosted = rfpCommit.AmountCommitted;
                            posting.DatePosted = DateTime.Now;
                            posting.Id = Guid.NewGuid();
                            posting.PostedBy = poster.Id;
                            posting.RFPBudgetLineId = rfpCommit.RFPBudgetLineId;
                            pb.TotalPosted += posting.AmountPosted;

                            //Delete commitment and add posting
                            context.BudgetCommitments.Remove(rfpCommit);
                            context.BudgetPostings.Add(posting);
                        }

                        var paymentRequest = context.PaymentRequests.FirstOrDefault(a => a.Id == rfp.Id);
                        paymentRequest.FundsPosted = true;
                        paymentRequest.PostedBy = poster.Id;
                        paymentRequest.PostedOn = DateTime.Now;

                        //SAVE ALL CHANGES
                        if (context.SaveChanges() > 0)
                        {
                            scope.Complete();
                            return true;
                        }
                        else
                        {
                            scope.Dispose();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Send notification to requestor and project managers of the affected budget lines
        /// </summary>
        /// <param name="rfa"></param>
        public void NotifyAffected(PaymentRequest rfp)
        {
            using (var context = new SCMSEntities())
            {
                //var rfp = context.PaymentRequests.FirstOrDefault(r => r.Id == rfpId);
                //Get ProjectBLCount list order by highest budget line count to least for this RFA
                List<ProjectBLCount> projects = context.PaymentRequestBudgetLines.Where(p => p.PaymentRequestId == rfp.Id).GroupBy(bl => bl.ProjectBudget.BudgetCategory.ProjectDonor).
                    Select(projList => new ProjectBLCount
                    {
                        ProjectDonorId = projList.FirstOrDefault().ProjectBudget.BudgetCategory.ProjectDonor.Id,
                        BudgetLineCount = projList.Count()
                    }).OrderByDescending(p => p.BudgetLineCount).ToList<ProjectBLCount>();
                //Notify requestor
                var requestor = context.Staffs.FirstOrDefault(s => s.Id == rfp.PreparedBy).Person;
                var msgBody = string.Format(NotificationHelper.rfpFundsPostedMsgBody, requestor.FirstName, rfp.RefNumber);
                notificationService.SendNotification(requestor.OfficialEmail, msgBody, NotificationHelper.rfpsubject);

                //Notify the Project Manager(s)
                foreach (var project in projects)
                {
                    var proj = context.ProjectDonors.FirstOrDefault(p => p.Id == project.ProjectDonorId);
                    var name = proj.Staff.Person.FirstName;
                    var email = proj.Staff.Person.OfficialEmail;
                    msgBody = string.Format(NotificationHelper.rfpPMNotifyFundsPostedMsgBody, name, rfp.RefNumber, proj.ProjectNumber);
                    notificationService.SendNotification(email, msgBody, NotificationHelper.rfpsubject);
                }
            }
        }

        public bool PurchaseOrderHasRFP(Guid poId)
        {
            using (var context = new SCMSEntities())
            {
                var rfps = context.PaymentRequests.Where(r => r.PurchaseOrderId == poId).ToList();
                return rfps != null && rfps.Count > 0;
            }
        }

        public decimal GetPOItemRemainingBalance(Guid poItemId)
        {
            using (var context = new SCMSEntities())
            {
                var poItem = context.PurchaseOrderItems.FirstOrDefault(p => p.Id == poItemId);
                decimal paid = 0;
                foreach (var rfpItem in poItem.PaymentRequestBudgetLines.ToList())
                    paid += rfpItem.Amount;
                if (poItem.TotalPrice > paid)
                    return poItem.TotalPrice - paid;
                return 0;
            }
        }

        public List<RequestForPaymentSummary> Find(List<Guid> ids)
        {
            List<RequestForPaymentSummary> rfps = new List<RequestForPaymentSummary>();

            using (var context = new SCMSEntities())
            {
                var results = from reqs in context.PaymentRequests
                              where ids.Contains(reqs.Id)
                              select reqs;

                foreach (Model.PaymentRequest pItem in results.ToList())
                {
                    RequestForPaymentSummary tmp = new RequestForPaymentSummary();
                    tmp.Id = pItem.Id;
                    tmp.PONo = pItem.PurchaseOrder.RefNumber;
                    tmp.Amount = pItem.TotalAmount;
                    tmp.PN = pItem.PurchaseOrder.ProjectDonor.ProjectNumber;
                    tmp.Currency = pItem.Currency.ShortName;
                    tmp.Supplier = pItem.Supplier.Name;
                    tmp.RFPNo = pItem.RefNumber;

                    if (pItem.FundsPosted)
                    {
                        tmp.Status = "FP";
                        tmp.StatusDate = (DateTime)pItem.PostedOn;
                    }
                    else
                        if (pItem.IsAuthorized.HasValue && ((bool)pItem.IsAuthorized))
                        {
                            tmp.Status = "AU";
                            tmp.StatusDate = (DateTime)pItem.AuthorizedOn;
                        }
                        else if (pItem.IsReviewed.HasValue && ((bool)pItem.IsReviewed))
                        {
                            tmp.Status = "RV";

                            tmp.StatusDate = (DateTime)pItem.ReviewedOn;
                        }
                        else if (pItem.IsRejected)
                        {
                            tmp.Status = "RJ";

                            if (pItem.AuthorizedOn.HasValue)
                            {
                                tmp.StatusDate = (DateTime)pItem.AuthorizedOn;
                            }
                            else
                            {
                                tmp.StatusDate = (DateTime)pItem.ReviewedOn;
                            }

                        }
                        else
                        {
                            tmp.Status = "CR";
                            tmp.StatusDate = (DateTime)pItem.PreparedOn;
                        }

                    rfps.Add(tmp);
                }

            }
            return rfps;
        }

        public List<BudgetCheckResult> RunFundsAvailableCheck(Guid rfpId)
        {
            var bcrList = new List<BudgetCheckResult>();
            using (var context = new SCMSEntities())
            {
                var rfpItems = context.PaymentRequestBudgetLines.IncludePaymentRequest().IncludeProjectDonor().Where(o => o.PaymentRequestId == rfpId);
                var rfp = rfpItems.ToList()[0].PaymentRequest;
                //Construct list of project budgets for that OR
                List<ProjectBudget> pbList = new List<ProjectBudget>();
                foreach (var rfpItem in rfpItems.ToList())
                {
                    if (!pbList.Contains(rfpItem.ProjectBudget))
                        pbList.Add(rfpItem.ProjectBudget);
                }
                foreach (var pb in pbList)
                {
                    decimal totalAmount = rfpItems.Where(b => b.BudgetLineId == pb.Id).Sum(b => b.Amount);
                    decimal previousCommits = 0;
                    //Get previously committed funds if any
                    foreach (var rfpItem in rfpItems)
                    {
                        var commit = context.BudgetCommitments.FirstOrDefault(bc => bc.BudgetLineId == pb.Id && bc.OrderRequestItemId == rfpItem.PurchaseOrderItemId);
                        if (commit != null)
                            previousCommits += commit.AmountCommitted;
                    }
                    //convert previous commits, if any, to po currency and subtract them from total amount
                    if (previousCommits > 0)
                    {
                        previousCommits = (decimal)exchangeRateService.GetForeignCurrencyValue(rfp.CurrencyId, pb.BudgetCategory.ProjectDonor.CurrencyId.Value, previousCommits, rfp.CountryProgrammeId);
                        totalAmount -= previousCommits;
                    }

                    if (!budgetService.SufficientFundsAvailable(totalAmount, pb.Id.ToString(), rfp.CurrencyId.ToString(), Guid.Empty, Guid.Empty, Guid.Empty))
                    {
                        decimal availableFunds = pb.TotalBudget - (decimal)(pb.TotalCommitted + pb.TotalPosted);
                        availableFunds = (decimal)exchangeRateService.GetForeignCurrencyValue(rfp.CurrencyId, pb.BudgetCategory.ProjectDonor.CurrencyId.Value, availableFunds, rfp.CountryProgrammeId);
                        bcrList.Add(new BudgetCheckResult { ProjectBudget = pb, AmountChecked = totalAmount, AvailableAmount = availableFunds, FundsAvailable = false });
                    }
                }
            }
            return bcrList;
        }
    }
}
