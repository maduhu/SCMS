using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Request4Payment;
using SCMS.Model;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.CoreBusinessLogic.PurchaseOrder;
using System.Collections;
using SCMS.UI.GeneralHelper;
using System.Data.Objects.DataClasses;
using SCMS.CoreBusinessLogic.StaffServices;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic._ExchangeRate;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class Request4PaymentController : PortalBaseController
    {
        private IRequest4PaymentService R4PSevice;
        private IOrderRequest OrderrequestService;
        private IPurchaseOrderService POservice;
        private IStaffService staffService;
        private IExchangeRateService exchangeRateService;
        private INotificationService notificationService;

        public Request4PaymentController(IPermissionService permissionService, IRequest4PaymentService R4PSevice, IOrderRequest OrderrequestService,
            IPurchaseOrderService POservice, IStaffService staffService, 
            IExchangeRateService exchangeRateService, INotificationService notificationService, IUserContext userContext)
            : base(userContext, permissionService)
        {
            this.R4PSevice = R4PSevice;
            this.OrderrequestService = OrderrequestService;
            this.POservice = POservice;
            this.staffService = staffService;
            this.exchangeRateService = exchangeRateService;
            this.notificationService = notificationService;
        }
        //
        // GET: /Request4Payment/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadRequst4P()
        {
            Request4Payment model = new Request4Payment();
            model.dropdownlist = new SelectList(POservice.GetPurchaseOrdersForRFP(countryProg.Id), "Id", "RefNumber");
                   
            model.Currencies = new SelectList(OrderrequestService.GetCurrencies(), "Id", "ShortName");
            model.Paymenttype = new SelectList(R4PSevice.GetPaymentType(countryProg.Id), "Id", "Description");
            model.Suppliers = new SelectList(POservice.GetSuppliers(), "Id", "Name");
            model.PaymentTerms = new SelectList(POservice.GetPaymentTerms(), "Id", "Description");
            model.EntityPaymentRqst = new PaymentRequest();
            model.EntityPaymentRqst.RequestDate = DateTime.Today;
            model.EntityPaymentRqst.PreparedOn = DateTime.Now;
            model.paymentDetais = new List<R4PpaymentDetails>();
            model.EntityPaymentRqst.PaymentRqstType = Resources.Global_String_PurchaseOrder;
            model.EntityPaymentRqst.RefNumber = string.Format("--{0}--", Resources.Global_String_NewRFP);
            model.usermsg = "";
            model.IsSaved = false;
            UserSession.CurrentSession.R4Pmodel = model;
            return View("LoadRequst4P", model);
        }

        private ActionResult LoadR4P(string r4ptype, string umsage = "", bool issaved = false)
        {
            Request4Payment model = new Request4Payment();
            
            model.dropdownlist = new SelectList(POservice.GetPurchaseOrdersForRFP(countryProg.Id), "Id", "RefNumber");
            model.Currencies = new SelectList(OrderrequestService.GetCurrencies(), "Id", "ShortName");
            model.Paymenttype = new SelectList(R4PSevice.GetPaymentType(countryProg.Id), "Id", "Description");
            model.Suppliers = new SelectList(POservice.GetSuppliers(), "Id", "Name");
            model.PaymentTerms = new SelectList(POservice.GetPaymentTerms(), "Id", "Description");
            model.EntityPaymentRqst = new PaymentRequest();
            model.EntityPaymentRqst.RequestDate = DateTime.Today;
            model.EntityPaymentRqst.PreparedOn = DateTime.Now;
            model.paymentDetais = new List<R4PpaymentDetails>();
            model.EntityPaymentRqst.PaymentRqstType = r4ptype;
            model.EntityPaymentRqst.RefNumber = string.Format("--{0}--", Resources.Global_String_NewRFP);
            model.usermsg = umsage;
            model.IsSaved = issaved;
            UserSession.CurrentSession.R4Pmodel = model;
            return View("LoadRequst4P", model);
        }

        public ActionResult LoadR4P(Guid id)
        {
            Models.Request4Payment model = UserSession.CurrentSession.R4Pmodel;
            model.paymentDetais = paymentD((Guid)id, model);
            model.ReferenceId = id;
            return View("LoadRequst4P", model);
        }

        private List<Models.R4PpaymentDetails> paymentD(Guid id, Models.Request4Payment model)
        {
            Models.R4PpaymentDetails pd;
            List<Models.R4PpaymentDetails> pdList = new List<R4PpaymentDetails>();
            using (var context = new SCMSEntities())
            {
                Model.PurchaseOrder po = context.PurchaseOrders.SingleOrDefault(p => p.Id == id);
                bool poHasRFP = R4PSevice.PurchaseOrderHasRFP(po.Id);
                ICollection<PurchaseOrderItem> poitem = po.PurchaseOrderItems;
                foreach (PurchaseOrderItem item in poitem)
                {
                    if (item.BudgetCommitments.Count < 1)
                        continue;
                    var pb = context.ProjectBudgets.SingleOrDefault(p => p.Id == item.BudgetLineId);
                    pd = new R4PpaymentDetails();
                    //var commitAmt = exchangeRateService.GetForeignCurrencyValue(po.Currency, pb.BudgetCategory.ProjectDonor.Currency, itemBLCommit.AmountCommitted, countryProg.Id);
                    if (!poHasRFP)
                        pd.Amount = Math.Round(item.TotalPrice, 2);
                    else
                        pd.Amount = Math.Round(R4PSevice.GetPOItemRemainingBalance(item.Id), 2);
                    pd.BudgetLineId = (Guid)item.BudgetLineId;
                    pd.BudgetLine = pb.LineNumber;
                    pd.BudgetLineDescription = pb.Description;
                    pd.PorjectNoId = (Guid)item.PurchaseOrder.ProjectDonorId;//.ProjectNoId;
                    pd.PoItemId = item.Id;
                    pd.projectNo = item.ProjectBudget.BudgetCategory.ProjectDonor.ProjectNumber; // context.ProjectDonors.SingleOrDefault(p => p.Id == item.ProjectNoId).ProjectNumber;
                    pdList.Add(pd);
                }
                model.EntityPaymentRqst.CurrencyId = po.CurrencyId;
                model.EntityPaymentRqst.SupplierId = po.SupplierId;
            }
            return pdList;
        }

        public ActionResult SaveR4P(Models.Request4Payment model)
        {
            try
            {
                model.EntityPaymentRqst.PurchaseOrderId = model.ReferenceId;
                //model.EntityPaymentRqst.RequestForAdvanceId = Guid.Empty;
                //model.EntityPaymentRqst.TravelAuthorisationId = Guid.Empty;

                model.EntityPaymentRqst.Id = Guid.NewGuid();
                model.EntityPaymentRqst.PreparedBy = currentStaff.Id;
                model.EntityPaymentRqst.PreparedOn = DateTime.Now;
                model.EntityPaymentRqst.CountryProgrammeId = countryProg.Id;
                model.EntityPaymentRqst.RequestDate = model.EntityPaymentRqst.PreparedOn;
                model.EntityPaymentRqst.IsSubmitted = true;
                model.EntityPaymentRqst.IsAuthorized = false;
                model.EntityPaymentRqst.IsReviewed = false;
                PaymentRequestBudgetLine m;
                EntityCollection<Model.PaymentRequestBudgetLine> RABlineList = new EntityCollection<PaymentRequestBudgetLine>();
                foreach (Models.R4PpaymentDetails item in model.paymentDetais)
                {
                    m = new PaymentRequestBudgetLine();
                    m.Id = Guid.NewGuid();
                    m.PaymentRequestId = model.EntityPaymentRqst.Id;
                    m.BudgetLineId = item.BudgetLineId;
                    m.Amount = item.Amount;
                    m.PurchaseOrderItemId = item.PoItemId;
                    RABlineList.Add(m);
                }
                string usmsge; bool issaved = false;
                model.EntityPaymentRqst.RefNumber = R4PSevice.GenerateUniquNumber(countryProg);
                if (R4PSevice.SaveRequest4Payment(model.EntityPaymentRqst, RABlineList))
                { usmsge = Resources.Global_String_SavedSuccessfully; issaved = true; }
                else { usmsge = Resources.Global_String_AnErrorOccurred; }
                ModelState.Clear();
                //return LoadR4P(model.EntityPaymentRqst.PaymentRqstType, usmsge, issaved);
                return ViewR4P();
            }
            catch (Exception ex)
            {
                return LoadR4P(model.EntityPaymentRqst.PaymentRqstType, ex.Message, false);
            }
        }

        public ActionResult ViewR4P()
        {
            List<ViewR4Payment> model = new List<ViewR4Payment>();
            List<PaymentRequest> PR = R4PSevice.GetPaymentRequests(countryProg.Id);
            foreach (PaymentRequest item in PR)
            {
                var p4pobject = new ViewR4Payment()
                {
                    EntityPaymentRqst = item
                };
                model.Add(p4pobject);
            }
            return View("ViewR4P", model);
        }

        public ActionResult ViewR4PDetails(Guid id, bool checkPost = false)
        {

            using (var context = new SCMSEntities())
            {
                PaymentRequest PR = context.PaymentRequests.SingleOrDefault(p => p.Id == id);

                List<R4PpaymentDetails> pdlist = new List<R4PpaymentDetails>();
                List<PaymentRequestBudgetLine> ARBlineList = context.PaymentRequestBudgetLines.Where(p => p.PaymentRequestId == PR.Id).ToList();
                foreach (PaymentRequestBudgetLine item in ARBlineList)
                {
                    var pb = context.ProjectBudgets.SingleOrDefault(p => p.Id == item.BudgetLineId);
                    var pdatils = new R4PpaymentDetails()
                    {
                        BudgetLine = pb.LineNumber,
                        BudgetLineDescription = pb.Description,
                        projectNo = context.ProjectDonors.SingleOrDefault(p => p.Id == item.ProjectBudget.BudgetCategory.ProjectDonorId).ProjectNumber,
                        Amount = (decimal)item.Amount
                    };
                    pdlist.Add(pdatils);
                }
                Currency c = PR.Currency;
                PaymentType pt = PR.PaymentType;
                Supplier sup = PR.Supplier;
                Staff st = PR.Staff1;
                Person psn = st.Person;
                Designation d = st.Designation;
                PaymentTerm pterm = PR.PaymentTerm;
                PurchaseOrder po = PR.PurchaseOrder;
                var model = new ViewR4Payment()
                {
                    EntityPaymentRqst = PR,
                    paymentDetais = pdlist
                };
                var mbcurr = model.EntityPaymentRqst.Currency1;
                if (model.EntityPaymentRqst.MBValue != null)
                    ViewBag.MBValue = ((decimal)model.EntityPaymentRqst.MBValue).ToString("#,##0.00");
                //Reviewer and Authorizer Details
                if (model.EntityPaymentRqst.ReviewedBy != null)
                {
                    Staff reviewer = staffService.GetStaffById((Guid)model.EntityPaymentRqst.ReviewedBy);
                    ViewBag.Reviewer = reviewer.Person.FirstName + " " + reviewer.Person.OtherNames;
                    ViewBag.ReviewerTitle = reviewer.Designation.Name;
                    ViewBag.ReviewedOn = model.EntityPaymentRqst.ReviewedOn;
                    if (reviewer.Person.SignatureImage != null)
                        ViewBag.ReviewerSignature = "<img src=\"/Person/Photo/" + reviewer.Person.Id + "\" style=\"max-width: 150px;\" />";
                }

                if (model.EntityPaymentRqst.AuthorizedBy != null)
                {
                    Staff authorizer = staffService.GetStaffById((Guid)model.EntityPaymentRqst.AuthorizedBy);
                    ViewBag.Authorizer = authorizer.Person.FirstName + " " + authorizer.Person.OtherNames;
                    ViewBag.AuthorizerTitle = authorizer.Designation.Name;
                    ViewBag.AuthorizedOn = model.EntityPaymentRqst.AuthorizedOn;
                    if (authorizer.Person.SignatureImage != null)
                        ViewBag.AuthorizerSignature = "<img src=\"/Person/Photo/" + authorizer.Person.Id + "\" style=\"max-width: 150px;\" />";
                }

                //Manage approval link
                string actionType = null;
                if (model.EntityPaymentRqst.IsReviewed == true && model.EntityPaymentRqst.IsAuthorized != true)
                    actionType = NotificationHelper.authorizationCode;
                else if (model.EntityPaymentRqst.IsSubmitted && model.EntityPaymentRqst.IsReviewed != true)
                    actionType = NotificationHelper.reviewCode;
                if (actionType != null)
                    model.CanApprove = notificationService.CanApprove(currentUser, NotificationHelper.rfpCode, actionType, model.EntityPaymentRqst.Id);
                else
                    model.CanApprove = false;

                //Manage Post Funds Button
                if (userContext.HasPermission(StandardPermissionProvider.RequestForPaymentPostFunds) && checkPost)
                    foreach (var rfp in R4PSevice.GetPaymentRequestsForPosting(countryProg.Id, currentUser))
                    {
                        if (rfp.Id.Equals(model.EntityPaymentRqst.Id))
                        {
                            model.CanPostFunds = true;
                            break;
                        }
                    }

                return View(model);
            }

        }
    }
}
