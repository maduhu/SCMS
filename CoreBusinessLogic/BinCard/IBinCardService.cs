using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.CoreBusinessLogic.BinCard
{
    public interface IBinCardService
    {
        bool IsBinAdded(Model.Bin binEntity);
        bool IsBinEdited(Model.Bin binEntity);
        bool IsBinDeleted(Guid id);
        bool IsBinItemAdded(Model.BinItem binItemEntity, Model.Bin binEntity);
        string GenerateUniquBCNo();
        List<Model.Bin> GetAll();
        List<Model.BinItem> GetAllBinItems();
        List<Model.OrderRequest> GetBinORs();
        List<Model.ProcurementPlan> GetBinPPs();
        List<Model.WareHouse> GetWarehousesFromPP(Guid ppId);
        List<Model.WareHouse> GetWarehousesFromOR(Guid orId);
        List<Model.ItemPackage> GetItemPackages(Guid ORitemId);
        List<Model.ItemPackage> GetPPItemPackages(Guid PPitemId);
        List<Model.GoodsReceivedNoteItem> GetGRNItemz4rmOR(Guid binId);
        List<Model.WarehouseReleaseItem> GetWRItemByBinId(Guid binId);
        List<Model.ProcurementPlanItem> GetProcurementPlanItems(Guid ppId);
        List<Model.GoodsReceivedNoteItem> GetGRNItemz4rmbin(Model.Bin bin);
        
    }
}
