using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using SCMS.Model;
using SCMS.Utils.DTOs;
using System.Linq.Expressions;

namespace SCMS.CoreBusinessLogic.PurchaseOrder
{
    public interface IPurchaseOrderService
    {
        bool Save<T>(T entity) where T : AttachedDocument;
        bool Update<T>(T entity) where T : AttachedDocument;
        bool Delete<T>(T entity) where T : AttachedDocument;
        bool SavePuchaseOrder(Model.PurchaseOrder entity);
        bool SavePOItem(Model.PurchaseOrderItem entity);
        void DeletePOItem(Guid id);
        void DeletePO(Guid PoId);
        bool DeleteAttachedDoc(Guid docId);
        string GenerateUniquNumber(CountryProgramme cp);

        Model.AttachedDocument GetDocById(Guid docId);
        List<Model.OrderRequest> GetPOrderORs(Guid PoId);
        List<Model.ProcurementPlan> GetPOrderPPs(Guid PoId);
        List<Model.PurchaseOrderItem> GetPOItemsByOrId(Guid PoId, Guid OrId);
        List<Model.PurchaseOrderItem> GetPOItemsByPPId(Guid PoId, Guid ppId);
        List<AttachedDocument> GetPOAttachedDocuments(Guid poId);
        Model.AttachedDocument GetAttachedDocumentById(Guid Id);
        List<Model.PurchaseOrder> GetPurchaseOrders();
        List<Model.ProjectBudget> GetProjectBudgets();
        List<Model.OrderRequest> GetOrderRequestsForPO(Guid countryprogramId);
        Model.PurchaseOrder GetPurchaseOrderById(Guid Id);
        List<Model.PurchaseOrder> GetApprovedPurchaseOrders(Guid countryProgId);
        List<Model.PurchaseOrderItem> GetPurchaseOrderItems(Guid purchaseOrderId);
        Model.PurchaseOrderItem GetPurchaseOrderItemById(Guid id);
        List<Model.PurchaseOrder> GetPurchaseOrdersForApproval(SystemUser currentUser);
        bool SaveReviewedPOItem(Model.PurchaseOrderItem poItem);
        bool SaveReviewedPO(Model.PurchaseOrder po);
        bool QuotationRefExists(string quotationRef, Guid? poId);
        bool TenderNumberExists(string TenderNumber, Guid? poId);
        List<Model.PurchaseOrder> Search(String refNum);
        bool AuthorizePurchaseOrder(Model.PurchaseOrder po);
        List<Model.PurchaseOrder> GetPurchaseOrdersForRFP(Guid cpId);
        void BackDatePO(Model.PurchaseOrder po);

        List<PurchaseOrderSummary> Find(List<Guid> ids);
        IQueryable<T> GetDocAttachments<T>(Expression<Func<T, bool>> expression) where T : EntityObject;
        T GetEntityById<T>(object id) where T : EntityObject;

        List<Model.AttachedDocument> GetList(Guid DocId, Guid cpId);
        List<BudgetCheckResult> RunFundsAvailableCheck(Guid poId);

        List<Model.PaymentTerm> GetPaymentTerms();
        List<Model.PaymentType> GetPaymentTypes();
        List<Model.ShippingTerm> GetShippingTerms();
        List<Model.Supplier> GetSuppliers();
        void AddPOItemsFromOR(Model.OrderRequest or, Guid poId);
        TenderingType DetermineTenderingType(Model.PurchaseOrder po);
        List<TenderingType> GetTenderingTypes();
        TenderingType GetTenderingTypeById(Guid id);
        void InsertTenderingType(TenderingType entity);
        void UpdateTenderingType(TenderingType entity);
        void DeleteTenderingTypeById(Guid id);
        Guid DeterminePOProjectDonorId(Guid poId);
    }
}
