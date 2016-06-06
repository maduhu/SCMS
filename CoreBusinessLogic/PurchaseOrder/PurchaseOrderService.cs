using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Transactions;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.Utils.DTOs;
using System.Linq.Expressions;
using SCMS.CoreBusinessLogic.Budgeting;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.Utils;
using System.Data.Entity.Validation;

namespace SCMS.CoreBusinessLogic.PurchaseOrder
{
    public class PurchaseOrderService : IPurchaseOrderService
    {

        private INotificationService NotificationService;
        private _ExchangeRate.IExchangeRateService exchangeRateService;
        private IBudgetService budgetService;

        public PurchaseOrderService(INotificationService NotificationService, _ExchangeRate.IExchangeRateService exchangeRateService, IBudgetService budgetService)
        {
            this.NotificationService = NotificationService;
            this.exchangeRateService = exchangeRateService;
            this.budgetService = budgetService;
        }

        private static void ClearPOSessionData()
        {
            SessionData.CurrentSession.PurchaseOrderList = null;
        }

        private static void ClearORSessionData()
        {
            SessionData.CurrentSession.OrderRequestList = null;
            SessionData.CurrentSession.ProcurementPlanItemList = null;
            SessionData.CurrentSession.ProcurementPlanList = null;
        }

        private static void ClearAttachedDocSession()
        {
            SessionData.CurrentSession.AttachedDocumentList = null;
        }

        private static void ClearTTSessionData()
        {
            SessionData.CurrentSession.TenderingTypeList = null;
        }

        public List<Model.PaymentTerm> GetPaymentTerms()
        {
            return SessionData.CurrentSession.PaymentTermList.OrderBy(p => p.Description).ToList();
        }

        public List<Model.PaymentType> GetPaymentTypes()
        {
            return SessionData.CurrentSession.PaymentTypeList.OrderBy(p => p.Description).ToList();
        }

        public List<Model.ShippingTerm> GetShippingTerms()
        {
            return SessionData.CurrentSession.ShippingTermList.OrderBy(p => p.Description).ToList();
        }

        public List<Model.Supplier> GetSuppliers()
        {
            return SessionData.CurrentSession.SupplierList.OrderBy(p => p.Name).ToList();
        }

        public bool Save<T>(T entity) where T : AttachedDocument
        {
            using (var context = new SCMSEntities())
            {
                ClearAttachedDocSession();
                ((IObjectContextAdapter)context).ObjectContext.CreateObjectSet<T>().AddObject(entity);
                return context.SaveChanges() > 0 ? true : false;
            }
        }

        public bool Update<T>(T entity) where T : AttachedDocument
        {
            using (var context = new SCMSEntities())
            {
                ClearAttachedDocSession();
                ((IObjectContextAdapter)context).ObjectContext.CreateObjectSet<T>().Attach(entity);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
                return context.SaveChanges() > 0 ? true : false;
            }
        }

        public bool Delete<T>(T entity) where T : AttachedDocument
        {
            using (var context = new SCMSEntities())
            {
                ClearAttachedDocSession();
                ((IObjectContextAdapter)context).ObjectContext.CreateObjectSet<T>().DeleteObject(entity);
                return context.SaveChanges() > 0 ? true : false;
            }
        }

        public bool DeleteAttachedDoc(Guid docId)
        {
            using (var db = new SCMSEntities())
            {
                ClearAttachedDocSession();
                db.AttachedDocuments.Remove(new AttachedDocument { Id = docId });
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool SavePuchaseOrder(Model.PurchaseOrder entity)
        {
            using (var context = new SCMSEntities())
            {
                if (entity.Id.Equals(Guid.Empty))
                {
                    entity.Id = Guid.NewGuid();
                    context.PurchaseOrders.Add(entity);
                }
                else
                {
                    var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == entity.Id);
                    po.CurrencyId = entity.CurrencyId;
                    po.DeliveryAddress = entity.DeliveryAddress;
                    po.LatestDeliveryDate = entity.LatestDeliveryDate;
                    po.PaymentTermId = entity.PaymentTermId;
                    po.PODate = entity.PODate;
                    po.ProjectDonorId = entity.ProjectDonorId;
                    po.QuotationRef = entity.QuotationRef;
                    po.Remarks = entity.Remarks;
                    po.ShippingTermId = entity.ShippingTermId;
                    po.SupplierId = entity.SupplierId;
                    po.IsInternational = entity.IsInternational;
                }
                ClearPOSessionData();
                return context.SaveChanges() > 0;
            }
        }

        public bool SavePOItem(Model.PurchaseOrderItem entity)
        {
            using (var context = new SCMSEntities())
            {
                if (entity.Id.Equals(Guid.Empty))
                {
                    var poItem = context.PurchaseOrderItems.FirstOrDefault(p => p.OrderRequestItemId.HasValue && p.OrderRequestItemId == entity.OrderRequestItemId
                        && p.PurchaseOrderId == entity.PurchaseOrderId && p.BudgetLineId == entity.BudgetLineId && !p.ProcurementPlanItemId.HasValue);
                    var ppPoItem = context.PurchaseOrderItems.FirstOrDefault(p => !p.OrderRequestItemId.HasValue && p.ProcurementPlanItemId.HasValue && 
                        p.ProcurementPlanItemId == entity.ProcurementPlanItemId
                        && p.PurchaseOrderId == entity.PurchaseOrderId && p.BudgetLineId == entity.BudgetLineId);
                    if (poItem == null && ppPoItem == null)
                    {
                        entity.Id = Guid.NewGuid();
                        context.PurchaseOrderItems.Add(entity);
                    }
                    else if(poItem != null)
                    {
                        poItem.Quantity += entity.Quantity;
                        poItem.TotalPrice = (decimal)(poItem.Quantity * poItem.UnitPrice);
                    }
                    else if (ppPoItem != null)
                    {
                        ppPoItem.Quantity += entity.Quantity;
                        ppPoItem.TotalPrice = (decimal)(ppPoItem.Quantity * ppPoItem.UnitPrice);
                    }
                }
                else
                {
                    var poItem = context.PurchaseOrderItems.FirstOrDefault(p => p.Id == entity.Id);
                    //update purchase order total
                    poItem.PurchaseOrder.TotalAmount -= poItem.TotalPrice;
                    poItem.PurchaseOrder.TotalAmount += entity.TotalPrice;

                    poItem.BudgetLineId = entity.BudgetLineId;
                    poItem.Quantity = entity.Quantity;
                    poItem.Remarks = entity.Remarks;
                    poItem.TotalPrice = entity.TotalPrice;
                    poItem.UnitPrice = entity.UnitPrice;
                }
                ClearPOSessionData();
                ClearORSessionData();
                return context.SaveChanges() > 0;
            }
        }

        public void DeletePOItem(Guid id)
        {
            using (var context = new SCMSEntities())
            {
                var poItem = context.PurchaseOrderItems.FirstOrDefault(p => p.Id == id);
                poItem.PurchaseOrder.TotalAmount -= poItem.TotalPrice;
                context.PurchaseOrderItems.Remove(poItem);
                context.SaveChanges();
                var poItemCount = context.PurchaseOrderItems.Where(p => p.PurchaseOrderId == poItem.PurchaseOrderId).Count();
                if (poItemCount == 0)
                {
                    var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == poItem.PurchaseOrderId);
                    po.OrderRequestId = null;
                    context.SaveChanges();
                }
                ClearPOSessionData();
                ClearORSessionData();
            }
        }

        public List<Model.OrderRequest> GetPOrderORs(Guid PoId)
        {
            return SessionData.CurrentSession.PurchaseOrderList.FirstOrDefault(p => p.Id == PoId)
                .PurchaseOrderItems.Where(p => p.OrderRequestItem != null).Select(p => p.OrderRequestItem.OrderRequest).Distinct().OrderBy(o => o.RefNumber).ToList();
        }

        public List<Model.ProcurementPlan> GetPOrderPPs(Guid PoId)
        {
            return SessionData.CurrentSession.PurchaseOrderList.FirstOrDefault(p => p.Id == PoId)
                .PurchaseOrderItems.Where(p => p.ProcurementPlanItem != null).Select(p => p.ProcurementPlanItem.ProcurementPlan).Distinct().OrderBy(p => p.RefNumber).ToList();
        }

        public List<Model.PurchaseOrderItem> GetPOItemsByOrId(Guid PoId, Guid OrId)
        {
            return SessionData.CurrentSession.PurchaseOrderList.FirstOrDefault(p => p.Id == PoId)
                .PurchaseOrderItems.Where(p => p.OrderRequestItemId.HasValue && p.OrderRequestItem.OrderRequestId == OrId).ToList();
        }

        public List<Model.PurchaseOrderItem> GetPOItemsByPPId(Guid PoId, Guid ppId)
        {
            return SessionData.CurrentSession.PurchaseOrderList.FirstOrDefault(p => p.Id == PoId)
                .PurchaseOrderItems.Where(p => p.ProcurementPlanItemId.HasValue && p.ProcurementPlanItem.ProcurementPlanId == ppId).ToList();
        }

        public void DeletePO(Guid PoId)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == PoId);
                        po.PurchaseOrderItems.ToArray().ForEach(p => context.PurchaseOrderItems.Remove(p));
                        context.PurchaseOrders.Remove(po);
                        context.AttachedDocuments.Where(d => d.DocumentId == po.Id && d.DocumentType == NotificationHelper.poCode).ToArray().ForEach(d => context.AttachedDocuments.Remove(d));
                        if (context.SaveChanges() > 0)
                            scope.Complete();
                        else
                            scope.Dispose();
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw ex;
                    }
                    ClearPOSessionData();
                    ClearORSessionData();
                }
            }
        }

        public string GenerateUniquNumber(CountryProgramme cp)
        {
            string code = "PO/DRC/" + cp.Country.ShortName + "/";
            string refNumber = "";
            long count = 1;
            using (var dbContext = new SCMSEntities())
            {
                var total = dbContext.PurchaseOrders.Where(p => p.CountryProgrammeId == cp.Id && p.IsSubmitted == true).Count();
                //Model.PurchaseOrder m = dbContext.PurchaseOrders.OrderByDescending(p => p.RecordCount).FirstOrDefault();
                //if (m != null)
                //    count = m.RecordCount + 1;
                count = total;
                Model.PurchaseOrder m = null;
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
                    m = dbContext.PurchaseOrders.FirstOrDefault(p => p.RefNumber == refNumber);
                } while (m != null);
                return refNumber;
            }
        }

        public List<Model.PurchaseOrder> GetPurchaseOrders()
        {
            return SessionData.CurrentSession.PurchaseOrderList.OrderByDescending(p => p.PreparedOn).ToList();
        }

        public List<Model.ProjectBudget> GetProjectBudgets()
        {
            using (var context = new SCMSEntities())
            {
                List<Model.ProjectBudget> mm = context.ProjectBudgets.ToList();
                return mm;
            }
        }

        public List<Model.OrderRequest> GetOrderRequestsForPO(Guid countryprogramId)
        {
            using (var context = new SCMSEntities())
            {
                return context.OrderRequests
                        .IncludeCurrency()
                        .IncludeProjectDonor()
                        .IncludeOrderRequestItems()
                        .IncludeOrderRequestItemsWithPO()
                        .Where(o => o.CountryProgrammeId == countryprogramId && o.IsAuthorized == true && o.OrderRequestItems
                            .Where(i =>
                                (i.PurchaseOrderItems.Where(p => !p.PurchaseOrder.IsRejected).Count() == 0) ||
                                (i.Quantity > i.PurchaseOrderItems.Sum(p => p.Quantity))
                            ).Count() > 0)
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();
            }
        }

        //Return the id of the ProjectDonor with most budget lines on PO
        public Guid DeterminePOProjectDonorId(Guid poId)
        {
            using (var context = new SCMSEntities())
            {
                var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == poId);
                var pd = po.PurchaseOrderItems.Select(p => p.ProjectBudget.BudgetCategory.ProjectDonor)
                    .OrderByDescending(p => p.BudgetCategories
                        .Where(b => b.ProjectBudgets
                            .Where(pb => pb.PurchaseOrderItems
                                .Where(i => i.PurchaseOrderId == poId).Count() > 0).Count() > 0).Count()).FirstOrDefault();
                return pd.Id;
            }
        }

        public List<Model.PurchaseOrder> GetApprovedPurchaseOrders(Guid countryProgId)
        {
            return SessionData.CurrentSession.PurchaseOrderList.Where(p => p.IsApproved).ToList();
        }

        public Model.PurchaseOrder GetPurchaseOrderById(Guid Id)
        {
            return SessionData.CurrentSession.PurchaseOrderList.FirstOrDefault(p => p.Id == Id);
        }

        public List<Model.PurchaseOrderItem> GetPurchaseOrderItems(Guid purchaseOrderId)
        {
            using (var context = new SCMSEntities())
            {
                return context.PurchaseOrderItems
                                        .IncludeOrderRequestItem()
                                        .IncludeProjectDonor()
                                        .IncludePurchaseOrder()
                                        .Where(p => p.PurchaseOrderId == purchaseOrderId).OrderByDescending(p => p.OrderRequestItemId).ToList();
            }
        }

        public Model.PurchaseOrderItem GetPurchaseOrderItemById(Guid id)
        {
            using (var context = new SCMSEntities())
            {
                var poItem = context.PurchaseOrderItems.IncludePurchaseOrder().IncludeProcurementPlanItem().IncludeOrderRequestItem().FirstOrDefault(p => p.Id == id);
                return poItem;
            }
        }

        /// <summary>
        /// Get list of PO for which we can prepare RFP. Note that if a PO already has an RFP that is pending approval and/or authorization then we cannot prepare
        /// and RFP for it. Secondly, if a PO no longer has BudgetCommitments then we cannot also prepare any other RFP for it. If the PO's RFP has been rejected 
        /// then we can prepared another one for it.
        /// </summary>
        /// <param name="cpId"></param>
        /// <returns></returns>
        public List<Model.PurchaseOrder> GetPurchaseOrdersForRFP(Guid cpId)
        {
            using (var context = SCMSEntities.Define())
            {
                return (from pos in context.PurchaseOrders
                        where !(from rfp in context.PaymentRequests
                                where rfp.CountryProgrammeId == cpId && (rfp.IsReviewed == false || rfp.IsAuthorized == false
                                || rfp.IsRejected == true)
                                select (Guid)rfp.PurchaseOrderId).Contains(pos.Id)
                               && (from poItem in context.PurchaseOrderItems
                                   where (from bc in context.BudgetCommitments select bc.PurchaseOrderItemId).Contains(poItem.Id)
                                   select poItem.PurchaseOrderId).Contains(pos.Id)
                        select pos).Where(p => p.CountryProgrammeId == cpId && p.IsApproved).ToList<Model.PurchaseOrder>();
            }
        }

        public List<Model.PurchaseOrder> GetPurchaseOrdersForApproval(SystemUser currentUser)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    List<Model.PurchaseOrder> purchaseOrders = new List<Model.PurchaseOrder>();
                    context.SystemUsers.Attach(currentUser);
                    var approvers = context.Approvers.Where(a => a.UserId == currentUser.Id && a.ActivityCode == NotificationHelper.poCode && a.ActionType == NotificationHelper.approvalCode).ToList();
                    var delegateApprovers = context.Approvers.Where(a => a.AssistantId == currentUser.Id && a.ActivityCode == NotificationHelper.poCode && a.ActionType == NotificationHelper.approvalCode).ToList();
                    if (approvers != null)
                    {
                        foreach (var approver in approvers)
                        {
                            var poList = SessionData.CurrentSession.PurchaseOrderList.Where(p => (p.IsSubmitted && !p.IsRejected && !p.IsApproved)
                                && p.Notifications.Where(n => (Guid)n.PurchaseOrderId == p.Id && n.IsRespondedTo == false && n.SentToDelegate == false && n.ApproverId == approver.Id).Count() > 0).ToList();
                            foreach (var po in poList)
                            {
                                purchaseOrders.Add(po);
                            }
                        }
                    }
                    if (delegateApprovers != null)
                    {
                        foreach (var delegateApprover in delegateApprovers)
                        {
                            var poList = SessionData.CurrentSession.PurchaseOrderList.Where(p => (p.IsSubmitted && !p.IsRejected && !p.IsApproved)
                                && p.Notifications.Where(n => (Guid)n.PurchaseOrderId == p.Id && !n.IsRespondedTo && n.SentToDelegate && n.ApproverId == delegateApprover.Id).Count() > 0).ToList();
                            foreach (var po in poList)
                            {
                                purchaseOrders.Add(po);
                            }
                        }
                    }
                    return purchaseOrders.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<Model.PurchaseOrder>();
            }
        }

        public bool SaveReviewedPOItem(Model.PurchaseOrderItem poItem)
        {
            using (var context = new SCMSEntities())
            {
                var item = context.PurchaseOrderItems.FirstOrDefault(p => p.Id == poItem.Id);
                context.Entry(item).CurrentValues.SetValues(poItem);
                ClearPOSessionData();
                ClearORSessionData();
                return (context.SaveChanges() > 0) ? true : false;
            }
        }

        public bool SaveReviewedPO(Model.PurchaseOrder po)
        {
            using (var context = new SCMSEntities())
            {
                var pOrder = context.PurchaseOrders.FirstOrDefault(p => p.Id == po.Id);
                context.Entry(pOrder).CurrentValues.SetValues(po);
                ClearPOSessionData();
                ClearORSessionData();
                return context.SaveChanges() > 0;
            }
        }


        public List<Model.PurchaseOrder> Search(String refNum)
        {
            using (var dbContext = new SCMSEntities())
            {

                List<Model.PurchaseOrder> orlist = dbContext.PurchaseOrders
                    .Where(p => p.RefNumber.Contains(refNum))
                    .OrderByDescending(p => p.PreparedOn).ToList();
                return orlist;
            }
        }

        public bool AuthorizePurchaseOrder(Model.PurchaseOrder po)
        {
            decimal? amount;
            decimal? currAmount;//Amount in ProjectBudget currency
            BudgetCommitment orCommit;
            using (var context = new SCMSEntities())
            {
                var poItems = context.PurchaseOrderItems.Where(p => p.PurchaseOrderId == po.Id).ToList();
                foreach (var poItem in poItems)
                {
                    amount = poItem.TotalPrice;
                    //Get PO Item amount in Budget Currency
                    currAmount = exchangeRateService.GetForeignCurrencyValue(poItem.ProjectBudget.BudgetCategory.ProjectDonor.Currency, poItem.PurchaseOrder.Currency, amount, (Guid)po.CountryProgrammeId);
                    orCommit = new BudgetCommitment();
                    orCommit.AmountCommitted = 0;
                    //Get amount committed by OrderRequestItem
                    if (poItem.OrderRequestItem != null)
                    {
                        var commitmentList = poItem.OrderRequestItem.BudgetCommitments.ToList<BudgetCommitment>();
                        if (commitmentList.Count > 0)
                        {
                            //List should have exactly one item
                            foreach (var commitment in commitmentList)
                            {
                                orCommit = commitment;
                                break;
                            }
                            //Remove initial commitment by OR Item
                            poItem.OrderRequestItem.ProjectBudget.TotalCommitted -= orCommit.AmountCommitted;
                            context.BudgetCommitments.Remove(orCommit);
                        }
                    }
                    //Add commitment by PO Item
                    poItem.ProjectBudget.TotalCommitted += currAmount;

                    //Add to BudgetCommitment table
                    var budgetCommitment = new BudgetCommitment();
                    budgetCommitment.Id = Guid.NewGuid();
                    budgetCommitment.PurchaseOrderItemId = poItem.Id;
                    budgetCommitment.AmountCommitted = (decimal)currAmount;
                    budgetCommitment.DateCommitted = DateTime.Now;
                    budgetCommitment.BudgetLineId = poItem.ProjectBudget.Id;
                    context.BudgetCommitments.Add(budgetCommitment);
                }
                return (context.SaveChanges() > 0) ? true : false;
            }

        }

        public List<PurchaseOrderSummary> Find(List<Guid> ids)
        {
            List<PurchaseOrderSummary> purchases = new List<PurchaseOrderSummary>();

            using (var context = new SCMSEntities())
            {
                var results = from myOrders in context.PurchaseOrders
                              where ids.Contains(myOrders.Id)
                              select myOrders;
                //context.ExecuteStoreQuery<OrderRequestView>("SELECT * FROM SomeView").ToList();

                foreach (Model.PurchaseOrder pItem in results.ToList())
                {
                    PurchaseOrderSummary tmp = new PurchaseOrderSummary();
                    tmp.Id = pItem.Id;
                    tmp.PONumber = pItem.RefNumber;
                    tmp.OrderRequestRefNumber = pItem.OrderRequest.RefNumber;
                    tmp.RequestDate = pItem.PODate;

                    tmp.Supplier = pItem.Supplier.Name;
                    tmp.DeliveryAddress = pItem.Location.Name;
                    tmp.DeliveryDate = pItem.LatestDeliveryDate;

                    tmp.POValue = pItem.TotalAmount.HasValue ? ((decimal)pItem.TotalAmount) : 0;

                    if (pItem.IsApproved)
                    {
                        tmp.Status = "AP";
                        tmp.StatusDate = (DateTime)pItem.ApprovedOn;
                    }
                    else if (pItem.IsRejected)
                    {
                        tmp.Status = "RJ";
                        tmp.StatusDate = (DateTime)pItem.ApprovedOn;
                    }
                    else if (pItem.IsSubmitted)
                    {
                        tmp.Status = "CR";
                        tmp.StatusDate = (DateTime)pItem.PreparedOn;
                    }
                    else
                    {
                        tmp.Status = "NEW";
                        tmp.StatusDate = (DateTime)pItem.PreparedOn;
                    }

                    purchases.Add(tmp);
                }

            }

            return purchases;
        }

        public IQueryable<T> GetDocAttachments<T>(Expression<Func<T, bool>> expression) where T : EntityObject
        {
            using (var context = new SCMSEntities())
            {
                return ((IObjectContextAdapter)context).ObjectContext.CreateObjectSet<T>().Where(expression);
            }
        }

        public T GetEntityById<T>(object id) where T : EntityObject
        {
            using (var context = new SCMSEntities())
            {
                return ((IObjectContextAdapter)context).ObjectContext.CreateObjectSet<T>().FirstOrDefault(t => t.EntityKey.EntityKeyValues[0].Value == id);
            }
        }

        public Model.AttachedDocument GetDocById(Guid docId)
        {
            using (var db = new SCMSEntities())
            {
                var doc = db.AttachedDocuments.FirstOrDefault(p => p.Id == docId);
                doc.Action = "UpdateAttachedDoc";
                return doc;
            }
        }

        public Model.AttachedDocument GetAttachedDocumentById(Guid Id)
        {
            return SessionData.CurrentSession.AttachedDocumentList.FirstOrDefault(d => d.Id == Id);
        }

        public List<Model.AttachedDocument> GetList(Guid DocId, Guid cpId)
        {
            using (var db = new SCMSEntities())
            {
                return db.AttachedDocuments.Where(p => p.DocumentId == DocId && p.CountryProgrammeId == cpId).ToList();
            }
        }

        public void BackDatePO(Model.PurchaseOrder po)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        //Get Current OR from DB
                        var currentPO = context.PurchaseOrders.FirstOrDefault(o => o.Id == po.Id);
                        //Initialize BackDate object
                        var backDate = new DocumentBackDating();
                        backDate.Id = Guid.NewGuid();
                        backDate.BackDatedBy = po.BackDatedBy;
                        backDate.BackDatedOn = DateTime.Now;
                        backDate.NewDate = po.PODate;
                        backDate.PurchaseOrderId = po.Id;
                        backDate.PreviousDate = currentPO.PODate;
                        backDate.Reason = po.BackDatingReason;
                        //Insert BackDate details into db
                        context.DocumentBackDatings.Add(backDate);
                        //Update date on OR in db
                        currentPO.PODate = po.PODate;
                        context.SaveChanges();
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw ex;
                    }
                }
            }
        }

        public List<BudgetCheckResult> RunFundsAvailableCheck(Guid poId)
        {
            var bcrList = new List<BudgetCheckResult>();
            using (var context = new SCMSEntities())
            {
                var poItems = context.PurchaseOrderItems.IncludePurchaseOrder().IncludeProjectDonor().Where(o => o.PurchaseOrderId == poId);
                var po = poItems.ToList()[0].PurchaseOrder;
                //Construct list of project budgets for that OR
                List<ProjectBudget> pbList = new List<ProjectBudget>();
                foreach (var poItem in poItems.ToList())
                {
                    if (!pbList.Contains(poItem.ProjectBudget))
                        pbList.Add(poItem.ProjectBudget);
                }
                foreach (var pb in pbList)
                {
                    decimal totalAmount = poItems.Where(b => b.BudgetLineId == pb.Id).Sum(b => b.TotalPrice);
                    decimal previousCommits = 0;
                    //Get previously committed funds if any
                    foreach (var poItem in poItems)
                    {
                        var commit = context.BudgetCommitments.FirstOrDefault(bc => bc.BudgetLineId == pb.Id && bc.OrderRequestItemId == poItem.OrderRequestItemId);
                        if (commit != null)
                            previousCommits += commit.AmountCommitted;
                    }
                    //convert previous commits, if any, to po currency and subtract them from total amount
                    if (previousCommits > 0)
                    {
                        previousCommits = (decimal)exchangeRateService.GetForeignCurrencyValue(po.CurrencyId, pb.BudgetCategory.ProjectDonor.CurrencyId.Value, previousCommits, po.CountryProgrammeId.Value);
                        totalAmount -= previousCommits;
                    }

                    if (!budgetService.SufficientFundsAvailable(totalAmount, pb.Id.ToString(), po.CurrencyId.ToString(), Guid.Empty, Guid.Empty, Guid.Empty))
                    {
                        decimal availableFunds = pb.TotalBudget - (decimal)(pb.TotalCommitted + pb.TotalPosted);
                        availableFunds = (decimal)exchangeRateService.GetForeignCurrencyValue(po.CurrencyId, pb.BudgetCategory.ProjectDonor.CurrencyId.Value, availableFunds, po.CountryProgrammeId.Value);
                        bcrList.Add(new BudgetCheckResult { ProjectBudget = pb, AmountChecked = totalAmount, AvailableAmount = availableFunds, FundsAvailable = false });
                    }
                }
            }
            return bcrList;
        }

        public bool QuotationRefExists(string quotationRef, Guid? poId)
        {
            return SessionData.CurrentSession.PurchaseOrderList.Any(p => p.QuotationRef == quotationRef && p.Id != poId);
        }

        public bool TenderNumberExists(string TenderNumber, Guid? poId)
        {
            return SessionData.CurrentSession.PurchaseOrderList.Any(p => p.TenderNumber == TenderNumber && p.Id != poId);
        }

        public List<AttachedDocument> GetPOAttachedDocuments(Guid poId)
        {
            return SessionData.CurrentSession.AttachedDocumentList.Where(a => a.DocumentId == poId && a.DocumentType == NotificationHelper.poCode).ToList();
        }

        public void AddPOItemsFromOR(Model.OrderRequest or, Guid poId)
        {
            using (var context = new SCMSEntities())
            {
                var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == poId);
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        foreach (var orItem in or.OrderRequestItems.Where(o => o.Quantity > o.PurchaseOrderItems.Sum(p => p.Quantity)))
                        {
                            var poItem = new Model.PurchaseOrderItem
                            {
                                Id = Guid.NewGuid(),
                                Quantity = (int)orItem.Quantity - orItem.PurchaseOrderItems.Sum(p => p.Quantity),
                                OrderRequestItemId = orItem.Id,
                                BudgetLineId = orItem.BudgetLineId,
                                PurchaseOrderId = poId,
                                Remarks = orItem.Remarks,
                                UnitPrice = (double)exchangeRateService.GetForeignCurrencyValue(po.CurrencyId, or.CurrencyId, orItem.EstimatedUnitPrice, or.CountryProgrammeId.Value)
                            };
                            poItem.TotalPrice = (decimal)(poItem.Quantity * poItem.UnitPrice);
                            context.PurchaseOrderItems.Add(poItem);
                            //Add to PO total amount
                            po.TotalAmount = po.TotalAmount.HasValue ? po.TotalAmount + poItem.TotalPrice : poItem.TotalPrice;
                        }
                        po.OrderRequestId = or.Id;
                        context.SaveChanges();
                        scope.Complete();
                        ClearPOSessionData();
                        ClearORSessionData();
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                    }
                }
            }
        }

        public TenderingType DetermineTenderingType(Model.PurchaseOrder po)
        {
            decimal poValue;
            foreach (var tt in SessionData.CurrentSession.TenderingTypeList)
            {
                if (tt.CurrencyId == po.CurrencyId)
                {
                    if (po.TotalAmount >= tt.MinValue && po.TotalAmount <= tt.MaxValue)
                        return tt;
                    if (po.TotalAmount >= tt.MinValue && tt.MaxValue == 0)
                        return tt;
                }
                else
                {
                    poValue = (decimal)exchangeRateService.GetForeignCurrencyValue(tt.CurrencyId, po.CurrencyId, po.TotalAmount.Value, po.CountryProgrammeId.Value);
                    if (poValue >= tt.MinValue && po.TotalAmount <= tt.MaxValue)
                        return tt;
                    if (poValue >= tt.MinValue && tt.MaxValue == 0)
                        return tt;
                }
            }
            return null;
        }

        public List<TenderingType> GetTenderingTypes()
        {
            return SessionData.CurrentSession.TenderingTypeList.OrderBy(t => t.MinValue).ToList();
        }

        public TenderingType GetTenderingTypeById(Guid id)
        {
            return SessionData.CurrentSession.TenderingTypeList.FirstOrDefault(t => t.Id == id);
        }

        public void InsertTenderingType(TenderingType entity)
        {
            using (var context = new SCMSEntities())
            {
                context.TenderingTypes.Add(entity);
                context.SaveChanges();
                ClearTTSessionData();
            }
        }

        public void UpdateTenderingType(TenderingType entity)
        {
            using (var context = new SCMSEntities())
            {
                var existing = context.TenderingTypes.FirstOrDefault(t => t.Id == entity.Id);
                if (existing != null)
                {
                    context.Entry(existing).CurrentValues.SetValues(entity);
                    context.SaveChanges();
                    ClearTTSessionData();
                }
            }
        }

        public void DeleteTenderingTypeById(Guid id)
        {
            using (var context = new SCMSEntities())
            {
                var poList = context.PurchaseOrders.Where(p => p.TenderingTypeId == id).ToList();
                foreach (var po in poList)
                    po.TenderingTypeId = null;
                var tt = new TenderingType { Id = id };
                context.TenderingTypes.Attach(tt);
                context.TenderingTypes.Remove(tt);
                context.SaveChanges();
                ClearTTSessionData();
            }
        }
    }
}
