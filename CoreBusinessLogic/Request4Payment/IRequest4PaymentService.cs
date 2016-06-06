using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using SCMS.Model;
using SCMS.Utils.DTOs;

namespace SCMS.CoreBusinessLogic.Request4Payment
{
    public interface IRequest4PaymentService
    {
        bool SaveRequest4Payment(Model.PaymentRequest entity, EntityCollection<PaymentRequestBudgetLine> collection);
        string GenerateUniquNumber(CountryProgramme cp);
        List<Model.PaymentRequest> GetPaymentRequests(Guid countryProgId);
        List<Model.PaymentType> GetPaymentType(Guid countryProgId);
        List<Model.PaymentRequest> GetPostedPaymentRequests(Guid countryProgId);
        List<Model.PaymentRequest> GetPaymentRequests4Review(SystemUser currentUser);
        List<Model.PaymentRequest> GetPaymentRequests4Authorization(SystemUser currentUser);
        List<PaymentRequest> GetPaymentRequestsForPosting(Guid countryProgId, SystemUser currentUser);
        PaymentRequest GetRFPById(Guid Id);
        List<PaymentRequestBudgetLine> GetRFPDetails(Guid RfpId);
        PaymentRequestBudgetLine GetRFPDetailById(Guid Id);
        bool SaveRFP(PaymentRequest rfp);
        bool SaveRFPDetail(PaymentRequestBudgetLine rfpDetail);
        bool CommitFunds(PaymentRequest rfp);
        bool EffectPosting(PaymentRequest rfp, Staff poster);
        bool PurchaseOrderHasRFP(Guid poId);
        // <summary>
        /// Send notification to requestor and project managers of the affected budget lines
        /// </summary>
        /// <param name="rfa"></param>
        void NotifyAffected(PaymentRequest rfp);
        /// <summary>
        /// Get outstanding balance on PoItem. This applies to the 2nd RFP onwards that are serving the same PO
        /// </summary>
        /// <param name="poItemId"></param>
        /// <returns></returns>
        decimal GetPOItemRemainingBalance(Guid poItemId);

        List<RequestForPaymentSummary> Find(List<Guid> ids);
        List<BudgetCheckResult> RunFundsAvailableCheck(Guid rfpId);
    }
}
