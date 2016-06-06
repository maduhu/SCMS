using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using SCMS.Utils.DTOs;

namespace SCMS.CoreBusinessLogic.OrderRequest
{
    public interface IOrderRequest
    {
        bool UpdateOrderRequest(Model.OrderRequest entity);
        bool UpdateORWithPossibleProjectChange(Model.OrderRequest or, bool projectChanged);
        void DeleteOrderRequst(Guid id);
        bool AddOrderRequstItem(Model.OrderRequestItem entity, Model.OrderRequest ORentity);
        bool UpdateOrderRequestItem(Model.OrderRequestItem entity);
        bool ItemAlreadyAddedToOR(Guid itemId, Guid orId);
        void DeleteOrderRequestItem(Guid id);
        void AuthorizeOrderRequest(Model.OrderRequest or);
        void UndoAuthorization(Model.OrderRequest or);
        string GenerateUniquNumber(CountryProgramme cp);
        List<Model.OrderRequest> Search(Model.OrderRequest entity);
        List<Model.OrderRequest> Search(String refNum);
        List<Model.OrderRequest> Search(DateTime fromDate, DateTime toDate, int startIndex = 0, int pageSize = 10);
        long SearchCount(DateTime fromDate, DateTime toDate);
        List<Model.OrderRequest> GetOrderRequests();
        List<Model.OrderRequest> GetOrderRequestsForApproval(SystemUser currentUser);
        List<Model.OrderRequest> GetOrderRequestsForReview(SystemUser currentUser);
        List<Model.OrderRequest> GetOrderRequestsForAuth(SystemUser currentUser);
        Model.OrderRequest GetOrderRequestById(Guid id);
        OrderRequestItem GetOrderRequestItemById(Guid OrId);
        Model.OrderRequestItem GetORItemWithoutIncludes(Guid id);
        List<CountrySubProgramme> CountrySubProgs(Guid pd);
        ProjectDonor GetProjectDonorById(Guid id);
        ProjectBudget GetProjectBudgetDetails(Guid id);
        List<Model.BudgetLineView> GetProjectBugdetLines(Guid pdid);
        List<OrderRequestItem> GetOrderRequestItems(Guid orderRequestid);
        List<ProjectDonor> GetProjectNos(Guid? projectId = null);
        List<Project> GetProject();
        List<ProjectDonor> GetProjectNosWithPP(Guid? projectId = null);
        List<Project> GetProjectsWithPP();
        List<UnitOfMeasure> GetUnitMesures();
        List<Currency> GetCurrencies();
        List<Location> GetLocations();
        List<Item> GetItems(string category = null);
        void NotifiyAuthorizer(Model.OrderRequest or);
        void NotifiyPreparer(Model.OrderRequest or);
        float GetLastPOItemPrice(Guid itemId);
        void BackDateOR(Model.OrderRequest or);
        List<OrderRequestSummary> Find(List<Guid> ids);
        OrderRequestPagePacket getPagedOrderRequests(int page = 1, int size = 25, Dictionary<String, String> args = null);
        List<BudgetCheckResult> RunFundsAvailableCheck(Guid orId);
        bool CanPreparePO(Guid orId);
        List<Project> GetProjectsWithoutPP();
        List<ProjectDonor> GetProjectNosWithoutPP(Guid projectId);
    }
}
