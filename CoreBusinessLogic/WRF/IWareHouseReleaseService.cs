using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.WRF
{
    public interface IWareHouseReleaseService
    {
        bool SaveWRF(Model.WarehouseRelease WR, WarehouseReleaseItem WRItem);
        bool IsROrderDeleted(Guid ROId);
        bool IsWRNItemDeleted(Guid wrnItemId);
        bool EditRO(WarehouseRelease RO);
        string GenerateUniquNumber(CountryProgramme cp);
        List<Inventory> GetInventoryItems(Guid warehouseId);
        List<WarehouseRelease> GetWRNs();
        List<WarehouseRelease> GetWRNsForApproval(Staff currentStaff);
        List<Model.Asset> GetAssets(Guid InventoryId);
        WarehouseRelease GetWROById(Guid Id);
        bool SaveApproved(WarehouseRelease wro);
        void RejectWRO(Guid wroId);
    }
}
