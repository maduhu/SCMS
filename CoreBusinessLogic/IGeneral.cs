using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic
{
    public interface IGeneralService
    {
        List<Model.PurchaseOrder> GetPurchaseOrders(Guid cpId, string search = null);
        List<Project> GetProjects(Guid cpId, string search = null);
        List<GeneralService.StaffView> GetStaff(Guid cpId, string search = null);
        List<Model.OrderRequest> GetOrderRequests(Guid cpId, string search = null);
        List<Person> GetPersons(Guid cpId, string search = null);
        List<GeneralService.OrderRequestItemView> GetOrderRequestItems(Guid orderRequestId);
        string GenerateUniquNumber(GeneralService.CodeType type);
    }
}
