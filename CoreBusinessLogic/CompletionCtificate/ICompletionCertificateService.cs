using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using SCMS.Utils.DTOs;

namespace SCMS.CoreBusinessLogic.CompletionCtificate
{
    public interface ICompletionCertificateService
    {
        bool SaveCompletionC(Model.CompletionCertificate entity);
        bool UpdateCC(Model.CompletionCertificate entity);
        string GenerateUniquNumber(CountryProgramme cp);
        Model.CompletionCertificate GetCCById(Guid CCId);
        List<Model.CompletionCertificate> GetCCNotes();
        List<Model.PurchaseOrder> GetGRNPurchaseOrders();
        List<CompletionCertificate> GetCCsForApproval(Staff currentStaff);

        List<CompletionCertificateSummary> Find(List<Guid> ids);
    }
}
