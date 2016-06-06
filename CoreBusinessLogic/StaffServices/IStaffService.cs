using System;
using System.Collections.Generic;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.StaffServices
{
    public interface IStaffService
    {
        void InsertStaff(Model.Staff staff);
        void UpdateStaff(Model.Staff staff);
        Model.Staff GetStaffById(Guid id);
        List<StaffView> GetStaffByCountryProgramme(Guid CountryProgId);
        List<Model.Approver> GetCPGlobalApprovers(Guid countryProgId);
        List<Model.Approver> GetProjectApprovers(Guid projectDonorId);
        void InsertApprover(Approver approver);
        void UpdateApprover(Approver approver);
        void DeleteApprover(Approver approver);
        Approver GetApproverById(Guid id);
        List<StaffView> GetApproversByDocumentType(string activityCode, string actionType, Guid countryProgId);
        List<StaffView> GetStaffByFinanceLimit(Guid FLId, Guid countryProgId);
        List<Staff> GetStaffByApprovalDoc(string documentCode, Guid countryProgId);

        List<DocumentPreparer> GetProjectDocPrepares(Guid projectDonorId);
        void InsertDocPreparer(DocumentPreparer docPreparer);
        void DeleteDocPreparerById(Guid id);
    }
}
