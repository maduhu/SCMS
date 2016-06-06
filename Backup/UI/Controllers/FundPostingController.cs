using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Request4Payment;
using SCMS.UI.Models;
using SCMS.Model;
using SCMS.CoreBusinessLogic.ActionFilters;
using System.Web.Security;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class FundPostingController : PortalBaseController
    {
        private IRequest4PaymentService rfpService;

        public FundPostingController(IPermissionService permissionService, IUserContext userContext, IRequest4PaymentService rfpService)
            : base(userContext, permissionService)
        {
            this.rfpService = rfpService;
        }
        //
        // GET: /FundPosting/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /FundPosting/FundPostingList

        public ActionResult FundPostingList()
        {
            return View();
        }

        #region .Request For Payment.

        public ActionResult RFPList()
        {
            List<ViewR4Payment> model = new List<ViewR4Payment>();
            List<PaymentRequest> rfpList = rfpService.GetPaymentRequestsForPosting(countryProg.Id, currentUser);
            foreach (PaymentRequest item in rfpList)
            {
                var p4pobject = new ViewR4Payment()
                {
                    EntityPaymentRqst = item
                };
                model.Add(p4pobject);
            }
            return View(model);
        }

        public ActionResult PostRFP(Guid id)
        {
            ViewBag.RfpId = id;
            ViewBag.Username = currentStaff.Person.OfficialEmail;
            return View();
        }

        public ActionResult PostRFPFunds(Guid id)
        {
            string response = "";
            PaymentRequest rfp = rfpService.GetRFPById(id);
            if (rfpService.EffectPosting(rfp, currentStaff))
            {
                rfpService.NotifyAffected(rfp);
                response = "SUCCESS";
            }
            else
                response = "FAILURE";
            return Content(response);
        }

        #endregion

        public ActionResult AuthenticatePoster(string password)
        {
            string response;
            if (Membership.ValidateUser(currentStaff.Person.OfficialEmail, password))
                response = "SUCCESS";
            else
                response = "FAILURE";
            return Content(response);
        }
    }
}
