using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Budgeting;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.CoreBusinessLogic.PurchaseOrder;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using System.Text;
using SCMS.CoreBusinessLogic.Request4Payment;
using SCMS.CoreBusinessLogic.StaffServices;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.CoreBusinessLogic.WRF;
using SCMS.CoreBusinessLogic._ExchangeRate;
using SCMS.CoreBusinessLogic._GoodsReceivedNote;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.ProcurementPlan;
using SCMS.CoreBusinessLogic.CompletionCtificate;
using SCMS.Resource;
using System.Transactions;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class RequestReviewController : PortalBaseController
    {
        #region .Private Variables.

        private IOrderRequest orderRequestService;
        private IPurchaseOrderService purchaseOrderService;
        private IBudgetService budgetService;
        private IMasterBudgetService masterBudgetService;
        private IRequest4PaymentService request4PaymentService;
        private IStaffService staffService;
        private INotificationService notificationService;
        private IWareHouseReleaseService wroService;
        private IExchangeRateService exchangeRateService;
        private IGoodsReceivedNoteService grnService;
        private IProcurementPlanService ppService;
        private ICompletionCertificateService ccService;

        #endregion

        #region .Constructor.

        public RequestReviewController(IPermissionService permissionService, IUserContext userContext, IOrderRequest orderRequestService, IPurchaseOrderService purchaseOrderService,
            IBudgetService budgetService, IMasterBudgetService masterBudgetService, IRequest4PaymentService request4PaymentService, IStaffService staffService,
            INotificationService notificationService, IWareHouseReleaseService wrnService,
            IExchangeRateService exchangeRateService, IGoodsReceivedNoteService grnService,
            IProcurementPlanService ppService, ICompletionCertificateService ccService)
            : base(userContext, permissionService)
        {
            this.budgetService = budgetService;
            this.masterBudgetService = masterBudgetService;
            this.orderRequestService = orderRequestService;
            this.purchaseOrderService = purchaseOrderService;
            this.request4PaymentService = request4PaymentService;
            this.staffService = staffService;
            this.notificationService = notificationService;
            this.wroService = wrnService;
            this.exchangeRateService = exchangeRateService;
            this.grnService = grnService;
            this.ppService = ppService;
            this.ccService = ccService;
        }

        #endregion
        //
        // GET: /RequestReview/

        public ActionResult Index()
        {
            return View();
        }

        #region .Load Requests.

        public ActionResult GetRequests()
        {

            //REQUESTS FOR APPROVAL
            List<Model.OrderRequest> orderRequests = new List<Model.OrderRequest>();
            List<Model.PurchaseOrder> purchaseOrders = new List<PurchaseOrder>();

            orderRequests = orderRequestService.GetOrderRequestsForApproval(currentUser);
            purchaseOrders = purchaseOrderService.GetPurchaseOrdersForApproval(currentUser);

            //Add lists to ReviewModel to be included in Notificaion Menu
            Models.RequestReviewModel rrModel = new Models.RequestReviewModel();
            rrModel.ApprovalOrderRequests = orderRequests;

            //REQUESTS FOR REVIEW
            
            List<Model.PaymentRequest> paymentRequests = new List<PaymentRequest>();
            List<Model.WarehouseRelease> warehouseReleases = new List<WarehouseRelease>();
            List<Model.GoodsReceivedNote> goodsReceivedNotes = new List<GoodsReceivedNote>();
            List<Model.CompletionCertificate> completionCertificates = new List<CompletionCertificate>();

            orderRequests = orderRequestService.GetOrderRequestsForReview(currentUser);
            paymentRequests = request4PaymentService.GetPaymentRequests4Review(currentUser);
            warehouseReleases = wroService.GetWRNsForApproval(currentStaff);
            goodsReceivedNotes = grnService.GetGRNsForApproval(currentStaff);
            rrModel.ProcurementPlansForApproval = ppService.GetPPForApproval(currentUser);
            rrModel.ProcurementPlansForApproval2 = ppService.GetPPForApproval2(currentUser);
            rrModel.ProcurementPlansForAuth = ppService.GetPPForAuthorization(currentUser);
            rrModel.ProcurementPlansForReview = ppService.GetPPForReview(currentUser);
            completionCertificates = ccService.GetCCsForApproval(currentStaff);

            //Construct Lists for Notification menu
            List<Models.ViewR4Payment> viewRFPList = ConstructRFPListForNotification(paymentRequests);
            
            List<Model.GoodsReceivedNote> viewGRNList = goodsReceivedNotes;
            List<Models.CompletionCtificate> viewCCList = ConstructCCListForNotification(completionCertificates);

            rrModel.OrderRequests = orderRequests;
            rrModel.PurchaseOrders = purchaseOrders;
            rrModel.PaymentRequests = viewRFPList;
            rrModel.WarehouseReleases = warehouseReleases;
            rrModel.GoodsReceivedNotes = viewGRNList;
            rrModel.CompletionCertificates = viewCCList;

            //REQUESTS FOR AUTHORIZATION
            orderRequests = orderRequestService.GetOrderRequestsForAuth(currentUser);
            paymentRequests = request4PaymentService.GetPaymentRequests4Authorization(currentUser);

            viewRFPList = ConstructRFPListForNotification(paymentRequests);

            rrModel.AuthOrderRequests = orderRequests;
            rrModel.AuthPaymentRequests = viewRFPList;

            return View("ProcurementRequestLinks", rrModel);

        }

        private List<Models.ViewR4Payment> ConstructRFPListForNotification(List<Model.PaymentRequest> rfpList)
        {
            Models.ViewR4Payment viewRFP;
            List<Models.ViewR4Payment> viewRFPList = new List<Models.ViewR4Payment>();
            foreach (var rfp in rfpList)
            {
                viewRFP = new Models.ViewR4Payment();
                viewRFP.EntityPaymentRqst = rfp;
                viewRFPList.Add(viewRFP);
            }
            return viewRFPList;
        }

        private List<PPModel> ConstructPPListForNotification(List<ProcurementPlan> ppList)
        {
            List<PPModel> viewPPList = new List<PPModel>();
            foreach (var pp in ppList)
            {
                viewPPList.Add(new PPModel { EntityPP = pp });
            }
            return viewPPList;
        }

        private List<CompletionCtificate> ConstructCCListForNotification(List<CompletionCertificate> ccList)
        {
            List<CompletionCtificate> viewCCList = new List<CompletionCtificate>();
            foreach (var cc in ccList)
            {
                viewCCList.Add(new CompletionCtificate { EntityCC = cc });
            }
            return viewCCList;
        }

        #endregion

        #region .Order Request.

        public ActionResult LoadOrderRequestForApproval(Guid id, List<BudgetCheckResult> brcList = null)
        {
            Model.OrderRequest orderRequest = orderRequestService.GetOrderRequestById(id);
            orderRequest.ORItemList = orderRequest.OrderRequestItems.ToList();
            List<BudgetLineView> blList = orderRequestService.GetProjectBugdetLines(orderRequest.ProjectDonorId.Value);
            foreach (var orItem in orderRequest.ORItemList)
            {
                orItem.BudgetLines = new SelectList(blList, "Id", "Description", orItem.BudgetLineId);
            }
            orderRequest.Projects = new SelectList(orderRequestService.GetProject(), "Id", "Name", orderRequest.ProjectId);
            orderRequest.Currency1 = mbCurrency;
            orderRequest.MBCurrencyId = mbCurrency.Id;
            orderRequest.IsFinanceReview = false;
            orderRequest.BudgetCheckResults = brcList;
            return View("EditOrderRequest", orderRequest);
        }

        public ActionResult LoadOrderRequest(Guid id, List<BudgetCheckResult> brcList = null)
        {
            Model.OrderRequest orderRequest = orderRequestService.GetOrderRequestById(id);
            orderRequest.ORItemList = orderRequest.OrderRequestItems.ToList();
            List<BudgetLineView> blList = orderRequestService.GetProjectBugdetLines(orderRequest.ProjectDonorId.Value);
            foreach (var orItem in orderRequest.ORItemList)
            {
                orItem.BudgetLines = new SelectList(blList, "Id", "Description", orItem.BudgetLineId);
            }
            orderRequest.Projects = new SelectList(orderRequestService.GetProject(), "Id", "Name", orderRequest.ProjectId);
            orderRequest.Currency1 = mbCurrency;
            orderRequest.MBCurrencyId = mbCurrency.Id;
            orderRequest.IsFinanceReview = true;
            orderRequest.BudgetCheckResults = brcList;
            return View("EditOrderRequest", orderRequest);
        }

        public ActionResult LoadOrderRequestForAuth(Guid id, List<BudgetCheckResult> brcList = null)
        {
            Model.OrderRequest orderRequest = orderRequestService.GetOrderRequestById(id);
            orderRequest.ORItemList = orderRequest.OrderRequestItems.ToList();
            List<BudgetLineView> blList = orderRequestService.GetProjectBugdetLines(orderRequest.ProjectDonorId.Value);
            foreach (var orItem in orderRequest.ORItemList)
            {
                orItem.BudgetLines = new SelectList(blList, "Id", "Description", orItem.BudgetLineId);
            }
            orderRequest.Projects = new SelectList(orderRequestService.GetProject(), "Id", "Name", orderRequest.ProjectId);
            orderRequest.Currencies = new SelectList(orderRequestService.GetCurrencies(), "Id", "ShortName", orderRequest.CurrencyId);
            orderRequest.Currency1 = mbCurrency;
            orderRequest.MBCurrencyId = mbCurrency.Id;
            orderRequest.BudgetCheckResults = brcList;
            return View("AuthorizeOR", orderRequest);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApproveOrder(Model.OrderRequest entity)
        {
            Model.OrderRequestItem entityOrItem;
            Model.OrderRequest entityOR = orderRequestService.GetOrderRequestById(entity.Id);
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    entityOR.TotalAmount = 0;
                    if (entity.ORItemList != null)
                    {
                        foreach (Model.OrderRequestItem orItem in entity.ORItemList)
                        {
                            entityOrItem = orderRequestService.GetOrderRequestItemById(orItem.Id);
                            if (entityOrItem == null)
                                continue;
                            entityOrItem.Quantity = orItem.Quantity;
                            entityOrItem.EstimatedUnitPrice = orItem.EstimatedUnitPrice;
                            entityOrItem.EstimatedPrice = (orItem.Quantity * orItem.EstimatedUnitPrice);
                            //Add to the TotalAmount of the Order
                            entityOR.TotalAmount += entityOrItem.EstimatedPrice;
                            entityOrItem.ProjectBudget.Id = entityOrItem.BudgetLineId = orItem.BudgetLineId;
                            orderRequestService.UpdateOrderRequestItem(entityOrItem);
                        }
                    }

                    //Run another check on funds availability
                    List<BudgetCheckResult> bcrList = orderRequestService.RunFundsAvailableCheck(entity.Id);
                    if (bcrList.Count > 0)
                        return LoadOrderRequestForApproval(entity.Id, bcrList);

                    entityOR.ApprovedBy = currentStaff.Id;
                    entityOR.ApprovedOn = DateTime.Now;
                    entityOR.IsApproved = true;
                    orderRequestService.UpdateOrderRequest(entityOR);
                    //Notifiy Finance Reviewer
                    notificationService.SendToAppropriateApprover(NotificationHelper.orCode, NotificationHelper.reviewCode, entityOR.Id);
                    //orderRequestService.NotifiyAuthorizer(entityOR);
                    //Notify requestor
                    string msgBody = string.Format(NotificationHelper.orApprovedMsgBody, entityOR.Staff2.Person.FirstName, entityOR.RefNumber);
                    notificationService.SendNotification(entityOR.Staff2.Person.OfficialEmail, msgBody, NotificationHelper.orsubject);
                    scope.Complete();                    
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
            return Content(Resources.Global_String_Done);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReviewOrder(Model.OrderRequest entity)
        {
            Model.OrderRequestItem entityOrItem;
            Model.OrderRequest entityOR = orderRequestService.GetOrderRequestById(entity.Id);
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    entityOR.MBValue = entity.MBValue;
                    entityOR.TotalAmount = 0;
                    if (entity.ORItemList != null)
                    {
                        foreach (Model.OrderRequestItem orItem in entity.ORItemList)
                        {
                            entityOrItem = orderRequestService.GetOrderRequestItemById(orItem.Id);
                            if (entityOrItem == null)
                                continue;
                            entityOrItem.Quantity = orItem.Quantity;
                            entityOrItem.EstimatedUnitPrice = orItem.EstimatedUnitPrice;
                            entityOrItem.EstimatedPrice = (orItem.Quantity * orItem.EstimatedUnitPrice);
                            //Add to the TotalAmount of the Order
                            entityOR.TotalAmount += entityOrItem.EstimatedPrice;
                            entityOrItem.BudgetLineId = orItem.BudgetLineId;
                            orderRequestService.UpdateOrderRequestItem(entityOrItem);
                        }
                    }

                    //Run another check on funds availability
                    List<BudgetCheckResult> bcrList = orderRequestService.RunFundsAvailableCheck(entity.Id);
                    if (bcrList.Count > 0)
                        return LoadOrderRequest(entity.Id, bcrList);

                    entityOR.ReviewedBy = currentStaff.Id;
                    entityOR.ReviewedOn = DateTime.Now;
                    entityOR.IsReviewed = true;
                    entityOR.MBCurrencyId = mbCurrency.Id;
                    orderRequestService.UpdateOrderRequest(entityOR);
                    //Notifiy authorizer
                    notificationService.SendToAppropriateApprover(NotificationHelper.orCode, NotificationHelper.authorizationCode, entityOR.Id);
                    //orderRequestService.NotifiyAuthorizer(entityOR);
                    //Notify requestor
                    string msgBody = string.Format(NotificationHelper.orReviewedMsgBody, entityOR.Staff2.Person.FirstName, entityOR.RefNumber);
                    notificationService.SendNotification(entityOR.Staff2.Person.OfficialEmail, msgBody, NotificationHelper.orsubject);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
            return Content(Resources.Global_String_Done);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AuthorizeOrder(Model.OrderRequest entity)
        {
            Model.OrderRequestItem entityOrItem;
            Model.OrderRequest entityOR = orderRequestService.GetOrderRequestById(entity.Id);
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    entityOR.MBValue = entity.MBValue;
                    entityOR.Remarks = entity.Remarks;
                    entityOR.TotalAmount = 0;

                    if (entity.ORItemList != null)
                    {
                        foreach (Model.OrderRequestItem orItem in entity.ORItemList)
                        {
                            entityOrItem = orderRequestService.GetOrderRequestItemById(orItem.Id);
                            if (entityOrItem == null)
                                continue;
                            entityOrItem.Quantity = orItem.Quantity;
                            entityOrItem.EstimatedUnitPrice = orItem.EstimatedUnitPrice;
                            entityOrItem.EstimatedPrice = (orItem.Quantity * orItem.EstimatedUnitPrice);
                            //Add to the TotalAmount of the Order
                            entityOR.TotalAmount += entityOrItem.EstimatedPrice;
                            entityOrItem.BudgetLineId = orItem.BudgetLineId;
                            orderRequestService.UpdateOrderRequestItem(entityOrItem);
                        }
                    }

                    //Run another check on funds availability
                    List<BudgetCheckResult> bcrList = orderRequestService.RunFundsAvailableCheck(entity.Id);
                    if (bcrList.Count > 0)
                        return LoadOrderRequestForAuth(entity.Id, bcrList);

                    entityOR.AuthorizedBy = currentStaff.Id;
                    entityOR.AuthorizedOn = DateTime.Now;
                    entityOR.IsAuthorized = true;
                    orderRequestService.UpdateOrderRequest(entityOR);
                    orderRequestService.AuthorizeOrderRequest(entityOR);
                    //Notify requestor
                    string msgBody = string.Format(NotificationHelper.orAuthorizedMsgBody, entityOR.Staff2.Person.FirstName, entityOR.RefNumber);
                    notificationService.SendNotification(entityOR.Staff2.Person.OfficialEmail, msgBody, NotificationHelper.orsubject);
                    //Notify Project Manager
                    notificationService.SendAuthorizedMsgToPMs(NotificationHelper.orCode, entityOR.Id);
                    //Notify TA Prep In charge
                    notificationService.SendAuthorizedMsgToDocPreparers(NotificationHelper.orCode, entityOR.Id);

                    ViewBag.Message = string.Format(Resources.RequestReviewController_String_HasBeenAuthorized, entityOR.RefNumber);
                    ViewBag.PdfURL = "/PDFReports/OrderRequest?refNumber=" + entityOR.RefNumber;
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
            return View("AuthComplete");
        }

        public ActionResult LoadRejectOrder(Guid Id)
        {
            Model.OrderRequest or = orderRequestService.GetOrderRequestById(Id);
            Model.RejectOR model = new RejectOR { Id = Id, RefNumber = or.RefNumber };
            
            if (or.IsReviewed == true)
            {
                model.IsReview = false;
                model.RejectedReviewRemarks = Resources.Global_String_None;
            }
            else
            {
                model.IsReview = true;
                model.RejectedAuthorizeRemarks = Resources.Global_String_None;
            }
            return View("RejectOR", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RejectOrder(Model.RejectOR rejectedOR)
        {
            string notificationMsg;
            Model.OrderRequest entityOR = orderRequestService.GetOrderRequestById(rejectedOR.Id);
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    entityOR.IsRejected = true;
                    if (rejectedOR.IsReview)
                    {
                        entityOR.RejectedReviewRemarks = rejectedOR.RejectedReviewRemarks;
                        entityOR.ReviewedBy = currentStaff.Id;
                        entityOR.ReviewedOn = DateTime.Now;
                        notificationMsg = string.Format(NotificationHelper.orRejectedMsgBody, entityOR.Staff2.Person.FirstName, entityOR.RefNumber, entityOR.RejectedReviewRemarks);
                    }
                    else
                    {
                        entityOR.RejectedAuthorizeRemarks = rejectedOR.RejectedAuthorizeRemarks;
                        entityOR.AuthorizedBy = currentStaff.Id;
                        entityOR.AuthorizedOn = DateTime.Now;
                        notificationMsg = string.Format(NotificationHelper.orRejectedMsgBody, entityOR.Staff2.Person.FirstName, entityOR.RefNumber, entityOR.RejectedAuthorizeRemarks);
                    }
                    orderRequestService.UpdateOrderRequest(entityOR);
                    //Notify Project Manager
                    notificationService.SendRejectedMsgToPMs(NotificationHelper.orCode, entityOR.Id, notificationMsg);
                    //Notify Approver if approver is not the one logged in
                    if (entityOR.Staff != null && entityOR.Staff.Id != currentStaff.Id)
                    {
                        notificationService.SendNotification(entityOR.Staff.Person.OfficialEmail, notificationMsg, NotificationHelper.orsubject);
                    }
                    else if (entityOR.Staff == null)
                    {
                        //Notify Requestor
                        notificationService.SendNotification(entityOR.Staff2.Person.OfficialEmail, notificationMsg, NotificationHelper.orsubject);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
            return Content(Resources.Global_String_Done);
        }

        #endregion

        #region .Purchase Order.

        public ActionResult LoadPurchaseOrder(Guid id, List<BudgetCheckResult> bcrList = null)
        {
            Model.PurchaseOrder model = purchaseOrderService.GetPurchaseOrderById(id);
            model.POItems = model.PurchaseOrderItems.ToList();
            model.Currencies = new SelectList(orderRequestService.GetCurrencies(), "Id", "ShortName", mbCurrency.Id);
            model.BudgetCheckResults = bcrList;
            foreach (var poItem in model.POItems)
                poItem.BudgetLines = new SelectList(orderRequestService.GetProjectBugdetLines(poItem.ProjectBudget.BudgetCategory.ProjectDonorId.Value), "Id", "Description", poItem.BudgetLineId);
            return View("ApprovePO", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovePO(Model.PurchaseOrder reviewPO)
        {
            Model.PurchaseOrderItem entityPOItem;
            Model.PurchaseOrder entityPO = purchaseOrderService.GetPurchaseOrderById(reviewPO.Id);
            entityPO.MBValue = reviewPO.MBValue;
            entityPO.AdditionalRemarks = reviewPO.AdditionalRemarks;
            entityPO.TotalAmount = 0;
            if (reviewPO.POItems != null)
            {
                foreach (Model.PurchaseOrderItem poItem in reviewPO.POItems)
                {
                    entityPOItem = purchaseOrderService.GetPurchaseOrderItemById(poItem.Id);
                    if (entityPOItem == null)
                        continue;
                    entityPOItem.Quantity = poItem.Quantity;
                    entityPOItem.UnitPrice = poItem.UnitPrice;
                    entityPOItem.TotalPrice = (decimal)(poItem.Quantity * poItem.UnitPrice);
                    //Add to the TotalAmount of the Order
                    entityPO.TotalAmount += entityPOItem.TotalPrice;
                    entityPOItem.BudgetLineId = poItem.BudgetLineId;
                    purchaseOrderService.SaveReviewedPOItem(entityPOItem);
                }
            }

            //Run another check on funds availability
            List<BudgetCheckResult> bcrList = purchaseOrderService.RunFundsAvailableCheck(reviewPO.Id);
            if (bcrList.Count > 0)
                return LoadPurchaseOrder(reviewPO.Id, bcrList);
            
            entityPO.ApprovedBy = userContext.CurrentUser.StaffId;
            entityPO.ApprovedOn = DateTime.Now;
            entityPO.IsApproved = true;
            purchaseOrderService.AuthorizePurchaseOrder(entityPO);
            purchaseOrderService.SaveReviewedPO(entityPO);
            //Notify Project Manager about authorized PO
            notificationService.SendAuthorizedMsgToPMs(NotificationHelper.poCode, entityPO.Id);
            //Send notification to sender that PO has been authorized
            notificationService.SendNotification(entityPO.Staff1.Person.OfficialEmail, string.Format(NotificationHelper.poApprovedMsgBody, entityPO.Staff1.Person.FirstName, entityPO.RefNumber), NotificationHelper.posubject);
            //Notify TA Prep In charge
            notificationService.SendAuthorizedMsgToDocPreparers(NotificationHelper.poCode, entityPO.Id);

            ViewBag.Message = string.Format(Resources.RequestReviewController_String_HasBeenAuthorized, entityPO.RefNumber);
            ViewBag.PdfURL = "PDFReports/PurchaseOrderInternational?refNumber=" + entityPO.RefNumber;
            return View("AuthComplete");
        }

        public ActionResult LoadRejectPO(Guid Id)
        {
            Model.RejectPO model = new Model.RejectPO();
            PurchaseOrder entityPO = purchaseOrderService.GetPurchaseOrderById(Id);
            model.Id = entityPO.Id;
            model.RefNumber = entityPO.RefNumber;
            model.IsReview = false;
            model.RejectedReviewRemarks = Resources.Global_String_None;
            return View("RejectPO", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RejectPO(Model.RejectPO rejectedPO)
        {
            string notificationMsg;
            PurchaseOrder entityPO = purchaseOrderService.GetPurchaseOrderById(rejectedPO.Id);
            entityPO.IsRejected = true;

            entityPO.RejectedAuthorizeRemarks = rejectedPO.RejectedAuthRemarks;
            entityPO.ApprovedBy = currentStaff.Id;
            entityPO.ApprovedOn = DateTime.Now;
            notificationMsg = string.Format(NotificationHelper.poRejectedMsgBody, entityPO.Staff1.Person.FirstName, entityPO.RefNumber, entityPO.RejectedAuthorizeRemarks);

            purchaseOrderService.SaveReviewedPO(entityPO);
            //Notify Project Manager(s)
            notificationService.SendRejectedMsgToPMs(NotificationHelper.poCode, entityPO.Id, notificationMsg);
            //Notify requestor
            notificationService.SendNotification(entityPO.Staff1.Person.OfficialEmail, notificationMsg, NotificationHelper.posubject);
            return LoadRejectPO(rejectedPO.Id);
        }

        #endregion

        #region .Request For Payment.

        public ActionResult ReviewRFP(Guid id, List<BudgetCheckResult> brcList = null)
        {
            PaymentRequest rfp = request4PaymentService.GetRFPById(id);
            List<PaymentRequestBudgetLine> rfpBLs = request4PaymentService.GetRFPDetails(rfp.Id);
            if (rfp.PurchaseOrderId != Guid.Empty)
                ViewBag.RefDocType = Resources.Global_String_PONo;
            List<Models.ReviewRFPDetails> rfpDetailsList = new List<Models.ReviewRFPDetails>();
            List<ProjectDonor> pdList = budgetService.GetProjectDonors(countryProg);
            List<BudgetLineView> blList;

            foreach (PaymentRequestBudgetLine item in rfpBLs)
            {
                blList = orderRequestService.GetProjectBugdetLines(item.ProjectBudget.BudgetCategory.ProjectDonor.Id);
                var pdatils = new Models.ReviewRFPDetails()
                {
                    BudgetLineId = item.BudgetLineId,
                    ProjectDonorId = item.ProjectBudget.BudgetCategory.ProjectDonor.Id,
                    Amount = (decimal)item.Amount,
                    ProjectDonors = new SelectList(pdList, "Id", "ProjectNumber", item.ProjectBudget.BudgetCategory.ProjectDonorId),
                    BudgetLines = new SelectList(blList, "Id", "Description", item.BudgetLineId),
                    RFPDetailId = item.Id
                };
                rfpDetailsList.Add(pdatils);
            }

            var model = new Models.ReviewRFP()
            {
                EntityPaymentRqst = rfp,
                paymentDetails = rfpDetailsList,
                MBCurrencyId = mbCurrency.Id,
                Currencies = new SelectList(orderRequestService.GetCurrencies(), "Id", "ShortName", mbCurrency.Id),
                BudgetCheckResults = brcList
            };
            return View("ReviewRFP", model);
        }

        public ActionResult LoadRFPForAuth(Guid id, List<BudgetCheckResult> brcList = null)
        {
            PaymentRequest rfp = request4PaymentService.GetRFPById(id);
            List<PaymentRequestBudgetLine> rfpBLs = request4PaymentService.GetRFPDetails(rfp.Id);
            if (rfp.PurchaseOrderId != Guid.Empty)
                ViewBag.RefDocType = Resources.Global_String_PONo;
            List<Models.ReviewRFPDetails> rfpDetailsList = new List<Models.ReviewRFPDetails>();
            List<ProjectDonor> pdList = budgetService.GetProjectDonors(countryProg);
            List<BudgetLineView> blList;

            foreach (PaymentRequestBudgetLine item in rfpBLs)
            {
                blList = orderRequestService.GetProjectBugdetLines(item.ProjectBudget.BudgetCategory.ProjectDonor.Id);
                var pdatils = new Models.ReviewRFPDetails()
                {
                    BudgetLineId = item.BudgetLineId,
                    ProjectDonorId = item.ProjectBudget.BudgetCategory.ProjectDonor.Id,
                    Amount = (decimal)item.Amount,
                    ProjectDonors = new SelectList(pdList, "Id", "ProjectNumber", item.ProjectBudget.BudgetCategory.ProjectDonorId),
                    BudgetLines = new SelectList(blList, "Id", "Description", item.BudgetLineId),
                    RFPDetailId = item.Id
                };
                rfpDetailsList.Add(pdatils);
            }

            var model = new Models.ReviewRFP()
            {
                EntityPaymentRqst = rfp,
                paymentDetails = rfpDetailsList,
                MBCurrencyId = (Guid)rfp.MBCurrencyId,
                Currencies = new SelectList(orderRequestService.GetCurrencies(), "Id", "ShortName", rfp.MBCurrencyId),
                BudgetCheckResults = brcList
            };

            //Reviewer and Authorizer Details
            if (model.EntityPaymentRqst.ReviewedBy != null)
            {
                Staff reviewer = staffService.GetStaffById((Guid)model.EntityPaymentRqst.ReviewedBy);
                ViewBag.Reviewer = reviewer.Person.FirstName + " " + reviewer.Person.OtherNames;
                ViewBag.ReviewerTitle = reviewer.Designation.Name;
                ViewBag.ReviewedOn = model.EntityPaymentRqst.ReviewedOn;
            }
            return View("AuthorizeRFP", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApproveRFP(Models.ReviewRFP rfpModel)
        {
            Model.PaymentRequest entityRFP = request4PaymentService.GetRFPById(rfpModel.EntityPaymentRqst.Id);
            Model.PaymentRequestBudgetLine entityRFPDetail;
            entityRFP.MBCurrencyId = rfpModel.MBCurrencyId;
            entityRFP.MBValue = rfpModel.MBValue;
            entityRFP.TotalAmount = 0;
            foreach (Models.ReviewRFPDetails rfpDetail in rfpModel.paymentDetails)
            {
                entityRFPDetail = request4PaymentService.GetRFPDetailById(rfpDetail.RFPDetailId);
                entityRFPDetail.BudgetLineId = rfpDetail.BudgetLineId;
                entityRFPDetail.Amount = rfpDetail.Amount;
                entityRFP.TotalAmount += entityRFPDetail.Amount;
                request4PaymentService.SaveRFPDetail(entityRFPDetail);
            }

            //Run another check on funds availability
            List<BudgetCheckResult> bcrList = request4PaymentService.RunFundsAvailableCheck(rfpModel.EntityPaymentRqst.Id);
            if (bcrList.Count > 0)
                return ReviewRFP(rfpModel.EntityPaymentRqst.Id, bcrList);

            entityRFP.IsReviewed = true;
            entityRFP.ReviewedBy = currentStaff.Id;
            entityRFP.ReviewedOn = DateTime.Now;
            request4PaymentService.SaveRFP(entityRFP);
            //Notify Autorizer and PMs
            notificationService.SendToAppropriateApprover(NotificationHelper.rfpCode, NotificationHelper.authorizationCode, entityRFP.Id);
            //Send notification to sender that RFP has been approved
            notificationService.SendNotification(entityRFP.Staff1.Person.OfficialEmail, string.Format(NotificationHelper.rfpApprovedMsgBody, entityRFP.Staff1.Person.FirstName, entityRFP.RefNumber), NotificationHelper.rfpsubject);

            return GetRequests();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AuthorizeRFP(Models.ReviewRFP rfpModel)
        {
            Model.PaymentRequest entityRFP = request4PaymentService.GetRFPById(rfpModel.EntityPaymentRqst.Id);
            Model.PaymentRequestBudgetLine entityRFPDetail;
            entityRFP.MBValue = rfpModel.MBValue;
            entityRFP.TotalAmount = 0;
            foreach (Models.ReviewRFPDetails rfpDetail in rfpModel.paymentDetails)
            {
                entityRFPDetail = request4PaymentService.GetRFPDetailById(rfpDetail.RFPDetailId);
                entityRFPDetail.BudgetLineId = rfpDetail.BudgetLineId;
                entityRFPDetail.Amount = rfpDetail.Amount;
                entityRFP.TotalAmount += entityRFPDetail.Amount;
                request4PaymentService.SaveRFPDetail(entityRFPDetail);
            }

            //Run another check on funds availability
            List<BudgetCheckResult> bcrList = request4PaymentService.RunFundsAvailableCheck(rfpModel.EntityPaymentRqst.Id);
            if (bcrList.Count > 0)
                return LoadRFPForAuth(rfpModel.EntityPaymentRqst.Id, bcrList);

            entityRFP.IsAuthorized = true;
            entityRFP.AuthorizedBy = currentStaff.Id;
            entityRFP.AuthorizedOn = DateTime.Now;
            //TODO: EffectPosting to be done by Finance Dept
            if (request4PaymentService.CommitFunds(entityRFP))
            {
                request4PaymentService.SaveRFP(entityRFP);
                notificationService.SendToAppropriateApprover(NotificationHelper.rfpCode, NotificationHelper.postFundsCode, entityRFP.Id);
            }
            //Send notification to sender that RFP has been authorized
            notificationService.SendNotification(entityRFP.Staff1.Person.OfficialEmail, string.Format(NotificationHelper.rfpAuthorizedMsgBody, entityRFP.Staff1.Person.FirstName, entityRFP.RefNumber), NotificationHelper.rfpsubject);
            ViewBag.Message = string.Format(Resources.RequestReviewController_String_HasBeenAuthorized, entityRFP.RefNumber);
            ViewBag.PdfURL = "/Reports/Request4Payment/Pdf?RFPid=" + entityRFP.Id;
            return View("AuthComplete");
        }

        public ActionResult LoadRejectRFP(Guid Id)
        {
            Models.RejectRFP model = new Models.RejectRFP();
            PaymentRequest entityRFP = request4PaymentService.GetRFPById(Id);
            model.Id = entityRFP.Id;
            if (entityRFP.IsReviewed == true)
            {
                model.IsReview = false;
                model.RejectedReviewRemarks = Resources.Global_String_None;
            }
            else
            {
                model.IsReview = true;
                model.RejectedAuthRemarks = Resources.Global_String_None;
            }
            return View("RejectRFP", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RejectRFP(Models.RejectRFP rejectedRFP)
        {
            string notificationMsg;
            PaymentRequest entityRFP = request4PaymentService.GetRFPById(rejectedRFP.Id);
            entityRFP.IsRejected = true;
            if (rejectedRFP.IsReview)
            {
                entityRFP.RejectedReviewRemarks = rejectedRFP.RejectedReviewRemarks;
                entityRFP.ReviewedBy = currentStaff.Id;
                entityRFP.ReviewedOn = DateTime.Now;
                notificationMsg = string.Format(NotificationHelper.rfpRejectedMsgBody, entityRFP.Staff1.Person.FirstName, entityRFP.RefNumber, entityRFP.RejectedReviewRemarks);
            }
            else
            {
                entityRFP.RejectedAuthorizeRemarks = rejectedRFP.RejectedAuthRemarks;
                entityRFP.AuthorizedBy = currentStaff.Id;
                entityRFP.AuthorizedOn = DateTime.Now;
                notificationMsg = string.Format(NotificationHelper.rfpRejectedMsgBody, entityRFP.Staff1.Person.FirstName, entityRFP.RefNumber, entityRFP.RejectedAuthorizeRemarks);
            }
            request4PaymentService.SaveRFP(entityRFP);
            //Notify Project Manager(s)
            notificationService.SendRejectedMsgToPMs(NotificationHelper.rfpCode, entityRFP.Id, notificationMsg);
            //Notify Requestor
            notificationService.SendNotification(entityRFP.Staff1.Person.OfficialEmail, notificationMsg, NotificationHelper.rfpsubject);
            return LoadRejectRFP(rejectedRFP.Id);
        }

        #endregion

        #region .Warehouse Release Order.

        public ActionResult LoadWRO(Guid id)
        {
            return View(wroService.GetWRNs().FirstOrDefault(p => p.Id == id));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApproveWRO(Model.WarehouseRelease wroModel)
        {
            WarehouseRelease entityWRO = wroService.GetWRNs().FirstOrDefault(p => p.Id == wroModel.Id);
            entityWRO.IsApproved = true;
            entityWRO.ApprovedBy = currentStaff.Id;
            entityWRO.ApprovedOn = DateTime.Now;
            wroService.SaveApproved(entityWRO);
            //Send notification to sender that RFP has been approved
            notificationService.SendNotification(entityWRO.Staff1.Person.OfficialEmail, string.Format(NotificationHelper.wrnApprovedMsgBody, entityWRO.Staff1.Person.FirstName, entityWRO.RefNumber), NotificationHelper.wrnsubject);
            //Response.Redirect("/Request4Payment/ViewR4PDetails/" + entityRFP.Id);
            ViewBag.Message = string.Format(Resources.RequestReviewController_String_HasBeenApproved, entityWRO.RefNumber);
            ViewBag.PdfURL = "/Reports/PDFReports/WRO?refNumber=" + entityWRO.RefNumber;
            return View("AuthComplete");
        }

        public ActionResult LoadRejectWRO(Guid Id)
        {
            var ro = wroService.GetWRNs().FirstOrDefault(p => p.Id == Id);
            var model = new Model.RejectWRO()
            {
                Id = ro.Id,
                RefNumber = ro.RefNumber
            };
            return View("RejectWRO", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RejectWRO(Model.RejectWRO rejectedWRO)
        {
            string notificationMsg;
            WarehouseRelease entityWRO = wroService.GetWRNs().FirstOrDefault(p => p.Id == rejectedWRO.Id);
            entityWRO.IsApproved = false;
            entityWRO.IsRejected = true;
            entityWRO.RejectedWRFComments = rejectedWRO.RejectedReviewRemarks;
            entityWRO.RejectedOn = DateTime.Now;
            wroService.SaveApproved(entityWRO);
            wroService.RejectWRO(entityWRO.Id);
            //Notify preparer
            notificationMsg = string.Format(NotificationHelper.wroRejectedMsgBody, entityWRO.Staff1.Person.FirstName, entityWRO.RefNumber, entityWRO.RejectedWRFComments);
            notificationService.SendNotification(entityWRO.Staff1.Person.OfficialEmail, notificationMsg, NotificationHelper.wrnsubject);
            return LoadRejectWRO(rejectedWRO.Id);
        }

        #endregion

        #region .Procurement Plan.

        public ActionResult ApprovePP(Guid ppId, string actionType)
        {
            ProcurementPlan model = ppService.GetProcurementPlanById(ppId);
            List<BudgetLineView> budgetLines = orderRequestService.GetProjectBugdetLines(model.ProjectDonorId);
            List<Currency> currencies = orderRequestService.GetCurrencies();
            if (actionType == NotificationHelper.approvalCode)
                model.PPItemList = model.ProcurementPlanItems.Where(p => !p.IsApproved).OrderBy(p => p.Item.Name).ToList();
            else if(actionType == NotificationHelper.approvalIICode)
                model.PPItemList = model.ProcurementPlanItems.Where(p => p.IsReviewed && !p.IsApproved2).OrderBy(p => p.Item.Name).ToList();
            else if (actionType == NotificationHelper.reviewCode)
                model.PPItemList = model.ProcurementPlanItems.Where(p => p.IsApproved && !p.IsReviewed).OrderBy(p => p.Item.Name).ToList();
            else if (actionType == NotificationHelper.authorizationCode)
                model.PPItemList = model.ProcurementPlanItems.Where(p => p.IsApproved2 && !p.IsAuthorized).OrderBy(p => p.Item.Name).ToList();
            foreach (var ppItem in model.PPItemList)
            {
                ppItem.UnitCost = Math.Round(ppItem.UnitCost, 2);
                ppItem.TotalCost = Math.Round(ppItem.TotalCost, 2);
                ppItem.BudgetLines = new SelectList(budgetLines, "Id", "Description", ppItem.BudgetLineId);
                ppItem.Currencies = new SelectList(currencies, "Id", "ShortName", ppItem.CurrencyId);
                ppItem.IsApproved = true;
            }
            model.ActionType = actionType;
            return View("ApprovePP", model);
        }

        /// <summary>
        /// Approve PP
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ApprovePP(ProcurementPlan model)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    ProcurementPlanItem item;
                    bool actionPerformed = false;
                    foreach (var ppItem in model.PPItemList)
                    {
                        if (!ppItem.IsApproved)
                            continue;
                        item = ppService.GetProcurementPlanItemById(ppItem.Id);
                        item.Quantity = ppItem.Quantity;
                        item.UnitCost = ppItem.UnitCost;
                        item.TotalCost = ppItem.Quantity * ppItem.UnitCost;
                        item.BudgetLineId = ppItem.BudgetLineId;
                        if (model.ActionType == NotificationHelper.approvalCode)
                            item.IsApproved = true;
                        else if (model.ActionType == NotificationHelper.approvalIICode)
                            item.IsApproved2 = true;
                        else if (model.ActionType == NotificationHelper.reviewCode)
                            item.IsReviewed = true;
                        else if (model.ActionType == NotificationHelper.authorizationCode)
                            item.IsAuthorized = true;
                        ppService.SaveProcurementPlanItem(item);
                        actionPerformed = true;
                    }

                    if (actionPerformed)
                    {
                        //Reload model from db to have access to all properties
                        string actionType = model.ActionType;
                        model = ppService.GetProcurementPlanById(model.Id);
                        model.ActionType = actionType;
                        PPApproved(model);
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
            return Content(Resources.Global_String_Done);
        }

        private void PPApproved(ProcurementPlan pp)
        {
            if (pp.ActionType == NotificationHelper.approvalCode)
            {
                pp.IsApproved1 = true;
                pp.ApprovedBy1 = currentStaff.Id;
                pp.ApprovedOn1 = DateTime.Now;
                notificationService.SendToAppropriateApprover(NotificationHelper.ppCode, NotificationHelper.reviewCode, pp.Id);
            }
            else if (pp.ActionType == NotificationHelper.reviewCode)
            {
                pp.IsReviewed = true;
                pp.ReviewedBy = currentStaff.Id;
                pp.ReviewedOn = DateTime.Now;
                notificationService.SendToAppropriateApprover(NotificationHelper.ppCode, NotificationHelper.approvalIICode, pp.Id);
            }
            else if (pp.ActionType == NotificationHelper.approvalIICode)
            {
                pp.IsApproved2 = true;
                pp.ApprovedBy2 = currentStaff.Id;
                pp.ApprovedOn2 = DateTime.Now;
                notificationService.SendToAppropriateApprover(NotificationHelper.ppCode, NotificationHelper.authorizationCode, pp.Id);
            }
            else if (pp.ActionType == NotificationHelper.authorizationCode)
            {
                pp.IsAuthorized = true;
                pp.AuthorizedBy = currentStaff.Id;
                pp.AuthorizedOn = DateTime.Now;
                //Notifiy Preparer
                notificationService.SendNotification(pp.Staff4.Person.OfficialEmail, string.Format(NotificationHelper.ppAuthorizedMsgBody, pp.Staff4.Person.FirstName, pp.RefNumber), NotificationHelper.ppsubject);
                //Notify Project Manager
                notificationService.SendNotification(pp.ProjectDonor.Staff.Person.OfficialEmail, string.Format(NotificationHelper.ppPMNotifyAuthorizedMsgBody, pp.ProjectDonor.Staff.Person.FirstName, pp.RefNumber), NotificationHelper.ppsubject);
            }            
            ppService.SaveProcurementPlan(pp);
        }

        public ActionResult RejectPP(Guid ppId, string actionType)
        {
            ProcurementPlan pp = ppService.GetProcurementPlanById(ppId);
            return View("RejectPP", new RejectPP { Id = pp.Id, RefNumber = pp.RefNumber, ActionType = actionType });
        }

        [HttpPost]
        public ActionResult RejectPP(RejectPP model)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    ProcurementPlan pp = ppService.GetProcurementPlanById(model.Id);
                    pp.IsRejected = true;
                    string msgBody = string.Format(NotificationHelper.ppRejectedMsgBody, pp.Staff4.Person.FirstName, pp.RefNumber, model.RejectionRemarks);
                    if (model.ActionType == NotificationHelper.approvalCode)
                    {
                        pp.ApprovedBy1 = currentStaff.Id;
                        pp.ApprovedOn1 = DateTime.Now;
                        pp.Approval1RejectMessage = model.RejectionRemarks;
                    }
                    else if (model.ActionType == NotificationHelper.approvalIICode)
                    {
                        pp.ApprovedBy2 = currentStaff.Id;
                        pp.ApprovedOn2 = DateTime.Now;
                        pp.Approval2RejectMessage = model.RejectionRemarks;
                    }
                    else if (model.ActionType == NotificationHelper.reviewCode)
                    {
                        pp.ReviewedBy = currentStaff.Id;
                        pp.ReviewedOn = DateTime.Now;
                        pp.ReviewRejectMessage = model.RejectionRemarks;
                    }
                    else
                    {
                        pp.AuthorizedBy = currentStaff.Id;
                        pp.AuthorizedOn = DateTime.Now;
                        pp.AuthorizationRejectMessage = model.RejectionRemarks;
                    }
                    ppService.SaveProcurementPlan(pp);
                    //Notifiy Preparer
                    notificationService.SendNotification(pp.Staff4.Person.OfficialEmail, msgBody, NotificationHelper.ppsubject);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
            return Content(Resources.Global_String_Done);
        }

        #endregion

        #region .Completion Certificate.

        public ActionResult LoadCCForApproval(Guid id)
        {            
            return View("ApproveCC", ccService.GetCCById(id));
        }

        /// <summary>
        /// Approve PP
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult ApproveCC(Guid id)
        {
            CompletionCertificate entityCC = ccService.GetCCById(id);
            entityCC.IsApproved = true;
            entityCC.ApprovedBy = currentStaff.Id;
            entityCC.ApprovedOn = DateTime.Now;
            ccService.UpdateCC(entityCC);
            //Notify preparer that CC has been approved
            notificationService.SendNotification(entityCC.Staff1.Person.OfficialEmail, string.Format(NotificationHelper.ccApprovedMsgBody, entityCC.Staff1.Person.FirstName, entityCC.RefNumber), NotificationHelper.ccsubject);
            return Content(Resources.Global_String_Done);
        }

        public ActionResult RejectCC(Guid id)
        {
            Model.RejectCC model = new Model.RejectCC();
            CompletionCertificate cc = ccService.GetCCById(id);
            model.Id = cc.Id;
            model.RefNumber = cc.RefNumber;
            return View("RejectCC", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RejectCC(Model.RejectCC rejectedCC)
        {
            string notificationMsg;
            CompletionCertificate entityCC = ccService.GetCCById(rejectedCC.Id);
            entityCC.IsRejected = true;
            entityCC.ApprovedBy = currentStaff.Id;
            entityCC.ApprovedOn = DateTime.Now;
            ccService.UpdateCC(entityCC);
            //Notify preparer
            notificationMsg = string.Format(NotificationHelper.ccRejectedMsgBody, entityCC.Staff1.Person.FirstName, entityCC.RefNumber, rejectedCC.RejectedRemarks);
            notificationService.SendNotification(entityCC.Staff1.Person.OfficialEmail, notificationMsg, NotificationHelper.ccsubject);
            return RejectCC(rejectedCC.Id);
        }
        #endregion
    }
}
