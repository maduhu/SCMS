using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using SCMS.Utils.DTOs;

namespace SCMS.CoreBusinessLogic._Inventory
{
    public interface IInventoryService
    {
        bool IsAssetEdited(Model.Asset entity);
        List<Model.Asset> GetAssetList();
        List<Depreciation> GetAnnualDepreciation(Guid assetId);
        List<Model.Asset> GetAssetList(Guid cpId, bool isFleet,bool returnUnregisteredFleetAsset);
        List<Model.Inventory> GetInventoryList(Guid? itemId = null, Guid? warehouseId = null, string categoryCode = null);
        List<Model.Asset> GetAssetInventoryList(Guid? AssetId = null, Guid? ItemId = null, Guid? warehouseId = null);
        List<Depreciation> GetDetailedDepreciation(Guid annualYearId);
        Asset GetAssetById(Guid id);
        string GetAssetCurrentProject(Model.Asset asset);
        string GetAssetCurrentProjetNoByAssetId(Guid assetId);
        List<GeneralInventorySummary> Find(List<Guid> ids);
    }
}
