using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using SCMS.Utils.DTOs;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.CoreBusinessLogic._Inventory
{
    public class InventoryService : IInventoryService
    {
        private GeneralService generalObj;
        private _Item.ItemService itemObj;

        public InventoryService()
        {
            generalObj = new GeneralService();
            itemObj = new _Item.ItemService();
        }

        public List<Model.Inventory> GetInventoryList(Guid? itemId = null, Guid? warehouseId = null, string categoryCode = null)
        {
            List<Model.Inventory> InList;
            if (itemId != null & categoryCode != null & warehouseId != null)
                InList = SessionData.CurrentSession.InventoryList.Where(p => p.Item.ItemCategory.CategoryCode == categoryCode & p.ItemId == itemId).ToList();
            else if (itemId != null & categoryCode != null)
                InList = SessionData.CurrentSession.InventoryList.Where(p => p.ItemId == itemId & p.Item.ItemCategory.CategoryCode == categoryCode).ToList();
            else if (itemId != null & warehouseId != null)
                InList = SessionData.CurrentSession.InventoryList.Where(p => p.ItemId == itemId & p.WareHouseId == warehouseId).ToList();
            else if (itemId != null)
                InList = SessionData.CurrentSession.InventoryList.Where(p => p.ItemId == itemId).ToList();
            else if (warehouseId != null)
                InList = SessionData.CurrentSession.InventoryList.Where(p => p.WareHouseId == warehouseId).ToList();
            else if (categoryCode != null)
                InList = SessionData.CurrentSession.InventoryList.Where(p => p.Item.ItemCategory.CategoryCode == categoryCode).ToList();
            else
                InList = SessionData.CurrentSession.InventoryList.ToList();

            return InList;

        }

        public List<Model.Asset> GetAssetInventoryList(Guid? AssetId = null, Guid? ItemId = null, Guid? warehouseId = null)
        {
            List<Model.Asset> astList;
            if (ItemId != null && warehouseId != null)
                astList = SessionData.CurrentSession.AssetList.Where(p => p.ItemId == ItemId & p.CurrentWareHouseId == warehouseId & p.IsDesposed == false).ToList();
            else if (ItemId != null && warehouseId == null)
                astList = SessionData.CurrentSession.AssetList.Where(p => p.ItemId == ItemId & p.IsDesposed == false).ToList();
            else
            {
                if (AssetId == null) astList = SessionData.CurrentSession.AssetList.Where(p => p.IsDesposed == false).ToList();
                else astList = SessionData.CurrentSession.AssetList.Where(p => p.Id == AssetId & p.IsDesposed == false).ToList();
            }
            return astList;
        }

        public string GetAssetCurrentProject(Model.Asset asset)
        {
            string currentProject;
            if (!asset.IsAssetStateChanged) { currentProject = asset.ProjectDonor.ProjectNumber + " (" + asset.ProjectDonor.Donor.ShortName + ")"; }
            else
            {
                var asetmgt = asset.AssetManagments.OrderByDescending(p => p.IssueDate).FirstOrDefault();
                if (asetmgt.currentProjectId != null) { currentProject = asetmgt.ProjectDonor.ProjectNumber + " (" + asetmgt.ProjectDonor.Donor.ShortName + ")"; }
                else { currentProject = asetmgt.PartnerName; }
            }
            return currentProject;
        }

        public bool IsAssetEdited(Model.Asset entity)
        {
            using (var context = new SCMSEntities())
            {
                if (entity.IsFleet)
                    if (context.FleetDetails.FirstOrDefault(p => p.AssetId == entity.Id) == null)
                        context.FleetDetails.Add(new FleetDetail() { Id = Guid.NewGuid(), CountryProgrammeId = entity.CountryProgramId, AssetId = entity.Id, IssueDate = DateTime.Now });
                context.Assets.Attach(entity);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                return (context.SaveChanges() > 0) ? true : false;
            }
        }

        public List<Model.Asset> GetAssetList()
        {
            return SessionData.CurrentSession.AssetList.Where(p => p.IsDesposed == false).OrderBy(a => a.Name).ToList();
        }

        public List<Model.Asset> GetAssetList(Guid cpId, bool isFleet, bool returnUnregisteredFleetAsset)
        {
            if (!returnUnregisteredFleetAsset)
                return GetAssetList().Where(p => p.IsFleet == isFleet).OrderBy(a => a.Name).ToList();
            //This is supposed to substitute the one below but not yet tested.
            else
            {
                return GetAssetList().Where(p => p.IsFleet == true && !(p.FleetDetails.Select(d => d.AssetId)).Contains(p.Id)).ToList();
            }
            //using (var db = new SCMSEntities())
            //{
            //    var query = from asst in db.Assets
            //                where asst.CountryProgramId == cpId & asst.IsFleet == true & asst.IsDesposed == false &
            //                !(from flit in db.FleetDetails
            //                  select flit.AssetId).Contains(asst.Id)
            //                select asst;

            //    return query.ToList<Model.Asset>();
            //}
        }

        public List<Depreciation> GetAnnualDepreciation(Guid assetId)
        {
            return  SessionData.CurrentSession.DepreciationList.Where(p => p.Period % 12 == 0 && p.AssetId == assetId).OrderBy(p => p.Period).ToList();
        }

        public List<Depreciation> GetDetailedDepreciation(Guid annualYearId)
        {
            var dpreciationEntity = SessionData.CurrentSession.DepreciationList.FirstOrDefault(p => p.Id == annualYearId);
            DateTime dprecStatdate = dpreciationEntity.Date.AddMonths(-11);
            return SessionData.CurrentSession.DepreciationList.Where(p => p.Date >= dprecStatdate && p.Date <= dpreciationEntity.Date && p.AssetId == dpreciationEntity.AssetId).OrderBy(p => p.Period).ToList();
        }

        public Asset GetAssetById(Guid id)
        {
            return SessionData.CurrentSession.AssetList.FirstOrDefault(a => a.Id == id);
        }

        public string GetAssetCurrentProjetNoByAssetId(Guid assetId)
        {
            Model.Asset aset = SessionData.CurrentSession.AssetList.FirstOrDefault(p => p.Id == assetId);
            if (!aset.IsAssetStateChanged) return aset.ProjectDonor.ProjectNumber;
            else
            {
                var asetmgt = aset.AssetManagments.Where(p => p.AssetId == assetId).OrderByDescending(p => p.IssueDate).FirstOrDefault();
                if (asetmgt.currentProjectId != null) return asetmgt.ProjectDonor.ProjectNumber; else return asetmgt.PartnerName;
            }
        }

        public List<GeneralInventorySummary> Find(List<Guid> ids)
        {
            List<GeneralInventorySummary> invs = new List<GeneralInventorySummary>();

            using (var context = new SCMSEntities())
            {
                var results = from myInventory in SessionData.CurrentSession.InventoryList
                              where ids.Contains(myInventory.Id)
                              select myInventory;

                foreach (Model.Inventory item in results.ToList())
                {
                    GeneralInventorySummary tmp = new GeneralInventorySummary();
                    tmp.Id = item.Id;
                    tmp.Classification = item.Item.ItemClassification.Name;
                    tmp.ItemName = item.Item.Name;
                    tmp.Category = item.Item.ItemCategory.CategoryName;
                    tmp.Quantity = item.Quantity;

                    invs.Add(tmp);
                }

            }

            return invs;

        }

    }


}
