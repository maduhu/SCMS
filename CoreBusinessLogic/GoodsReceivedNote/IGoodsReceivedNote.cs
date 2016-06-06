using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using System.Data.Objects.DataClasses;

namespace SCMS.CoreBusinessLogic._GoodsReceivedNote
{
    public interface IGoodsReceivedNoteService
    {
        bool SaveGRN(Model.GoodsReceivedNote GRNEntity);
        bool IsAssetDisposed(Model.Asset AssetEntity);
        bool IsAssetStateChanged(Model.AssetManagment Entity);
        string GenerateUniquNumber(CountryProgramme cp);
        bool IsInventoryUpdated(Inventory InventEntity);
        string GenerateAssetNo(Guid ProjectDId);
        string GenerateAssetNo(Model.GoodsReceivedNoteItem GRNItem);
        bool IsGRNVerified(GoodsReceivedNote entity);
        Model.PurchaseOrder GetPO(Guid POid);
        bool IsAssetRegistered(Model.Asset assetEntity);
        List<Model.WareHouse> GetSubOfficeWareHouses(Guid countrysubofficeId);
        List<POItemsView> GetPOItemsDetails(Guid POId);
        List<GoodsReceivedNoteItem> GetUnregisteredGRNItems();
        List<Model.PurchaseOrder> GetGRNPurchaseOrders();
        List<GoodsReceivedNote> GetGRNsForApproval(Staff currentStaff);
        void UpdateGRNItem(GoodsReceivedNoteItem grnItem);
        GoodsReceivedNote GetGRNById(Guid id);
        void DeleteGRNItem(Guid GRNItemId);
        void UpdateGRN(GoodsReceivedNote grn);
        void DeleteGRNById(Guid id);
    }
}
