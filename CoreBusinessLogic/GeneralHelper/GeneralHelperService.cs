using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using SCMS.Model;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic._GoodsReceivedNote;
using SCMS.CoreBusinessLogic._WareHouse;
using SCMS.CoreBusinessLogic._Inventory;

namespace SCMS.CoreBusinessLogic.GeneralHelper
{
    public class GeneralHelperService : IGeneralHelperService
    {
        //http://blogs.infosupport.com/mvc4autofacentityframework/

        private IGoodsReceivedNoteService gRNService;
        private IWareHouseService wareHouseService;
        private InventoryService inventoryService;

        public GeneralHelperService(IGoodsReceivedNoteService gRNService, IWareHouseService wareHouseService, InventoryService inventoryService)
        {
            this.gRNService = gRNService;
            this.wareHouseService = wareHouseService;
            this.inventoryService = inventoryService;
        }

        public void LoadSessionData(CountryProgramme countryProg, Staff currentStaff)
        {
            //if (SessionData.CurrentSession.GoodsReceivedNoteList == null) SessionData.CurrentSession.GoodsReceivedNoteList = gRNService.GetGRNs(countryProg.Id);
            //if (SessionData.CurrentSession.WarehouseList == null) SessionData.CurrentSession.WarehouseList = wareHouseService.GetAllWareHouse(countryProg.Id);
            //if (SessionData.CurrentSession.InventoryList == null) SessionData.CurrentSession.InventoryList = inventoryService.GetInventory(countryProg.Id);
            //if (SessionData.CurrentSession.AssetList == null) SessionData.CurrentSession.AssetList = inventoryService.GetAssets(countryProg.Id);

        }
    }
}
