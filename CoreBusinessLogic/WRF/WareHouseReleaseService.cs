using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using System.Collections;
using System.Transactions;
using SCMS.CoreBusinessLogic.NotificationsManager;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.CoreBusinessLogic.WRF
{
    public class WareHouseReleaseService : IWareHouseReleaseService
    {
        private INotificationService NotificationServicee;

        public WareHouseReleaseService(INotificationService NotificationServicee)
        {
            this.NotificationServicee = NotificationServicee;
        }

        public bool SaveWRF(Model.WarehouseRelease WR, Model.WarehouseReleaseItem entity)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (WR != null)
                        {
                            context.WarehouseReleases.Add(WR);
                            return AddItems(WR, entity, context, scope, true);
                        }
                        else
                        {
                            if (entity.Id.Equals(Guid.Empty))
                                return AddItems(WR, entity, context, scope);
                            else
                            {
                                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                                if ((context.SaveChanges() > 0)) { scope.Complete(); SessionData.CurrentSession.ReleaseOrderItemList = null; return true; } else { scope.Dispose(); return false; }
                            }
                        }
                    }
                    catch (Exception ex) { scope.Dispose(); throw ex; }
                }
            }
        }

        private bool AddItems(Model.WarehouseRelease wrEntity, Model.WarehouseReleaseItem entity, SCMSEntities context, TransactionScope scope, bool sendmail = false)
        {
            string itemCategory = context.Inventories.FirstOrDefault(p => p.Id == entity.InventoryId).Item.ItemCategory.CategoryCode;
            if (wrEntity == null)
                wrEntity = context.WarehouseReleases.FirstOrDefault(w => w.Id == entity.WarehouseReleaseId);
            entity.Id = Guid.NewGuid();
            if (itemCategory.Equals("C"))
            { entity.AssetId = Guid.Empty; context.WarehouseReleaseItems.Add(entity); }
            else
            {
                entity.Quantity = 1;
                Model.Asset ass = context.Assets.FirstOrDefault(p => p.Id == entity.AssetId);
                ass.IsReleased = true;
                ass.CurrentOwnerId = wrEntity.ReceivedBy;
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(ass, System.Data.EntityState.Modified);
                context.WarehouseReleaseItems.Add(entity);
            }
            Model.Inventory inv = context.Inventories.First(p => p.Id == entity.InventoryId);
            inv.Quantity -= (Int64)entity.Quantity;
            ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(inv, System.Data.EntityState.Modified);

            if ((context.SaveChanges() > 0))
            {
                SessionData.CurrentSession.ReleaseOrderList = null;
                SessionData.CurrentSession.ReleaseOrderItemList = null;
                SessionData.CurrentSession.AssetList = null;
                SessionData.CurrentSession.InventoryList = null;
                if (sendmail)
                    NotificationServicee.SendNotification(NotificationServicee.GetApproverEmailAddress(1, NotificationHelper.wrnCode), NotificationHelper.wrnMsgBody, NotificationHelper.wrnsubject);
                scope.Complete();
                return true;
            }
            else { scope.Dispose(); return false; }
        }

        public bool IsROrderDeleted(Guid ROId)
        {
            using (var context = new SCMSEntities())
            {
                var RO = context.WarehouseReleases.FirstOrDefault(p => p.Id == ROId);
                foreach (var item in RO.WarehouseReleaseItems)
                {
                    string itemCategory = context.Inventories.FirstOrDefault(p => p.Id == item.InventoryId).Item.ItemCategory.CategoryCode;
                    if (itemCategory.Equals("A"))
                    {
                        item.Quantity = 1;
                        Model.Asset ass = context.Assets.FirstOrDefault(p => p.Id == item.AssetId);
                        ass.IsReleased = false;
                        ass.CurrentOwnerId = null;
                        ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(ass, System.Data.EntityState.Modified);
                    }
                    Model.Inventory inv = context.Inventories.First(p => p.Id == item.InventoryId);
                    inv.Quantity += (Int64)item.Quantity;
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(inv, System.Data.EntityState.Modified);

                }
                context.WarehouseReleases.Remove(RO);
                int affectedRecords = context.SaveChanges();
                SessionData.CurrentSession.ReleaseOrderList = null;
                SessionData.CurrentSession.ReleaseOrderItemList = null;
                SessionData.CurrentSession.AssetList = null;
                SessionData.CurrentSession.InventoryList = null;
                return affectedRecords > 0 ? true : false;
            }
        }

        public bool IsWRNItemDeleted(Guid wrnItemId)
        {
            using (var context = new SCMSEntities())
            {
                Model.WarehouseReleaseItem wrnitm = context.WarehouseReleaseItems.FirstOrDefault(p => p.Id == wrnItemId);
                string itemCategory = context.Inventories.FirstOrDefault(p => p.Id == wrnitm.InventoryId).Item.ItemCategory.CategoryCode;
                if (itemCategory.Equals("A"))
                {
                    wrnitm.Quantity = 1;
                    Model.Asset ass = context.Assets.FirstOrDefault(p => p.Id == wrnitm.AssetId);
                    ass.IsReleased = false;
                    ass.CurrentOwnerId = null;
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(ass, System.Data.EntityState.Modified);
                }
                Model.Inventory inv = context.Inventories.First(p => p.Id == wrnitm.InventoryId);
                inv.Quantity += (Int64)wrnitm.Quantity;
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(inv, System.Data.EntityState.Modified);
                context.WarehouseReleaseItems.Remove(wrnitm);
                int affectedColumns = context.SaveChanges();
                SessionData.CurrentSession.ReleaseOrderList = null;
                SessionData.CurrentSession.ReleaseOrderItemList = null;
                SessionData.CurrentSession.AssetList = null;
                SessionData.CurrentSession.InventoryList = null;
                return affectedColumns > 0 ? true : false;
            }
        }

        public void RejectWRO(Guid wroId)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        var wrnItems = context.WarehouseReleaseItems.Where(w => w.WarehouseReleaseId == wroId).ToList();
                        foreach (var wrnItem in wrnItems)
                        {
                            string itemCategory = context.Inventories.FirstOrDefault(p => p.Id == wrnItem.InventoryId).Item.ItemCategory.CategoryCode;
                            if (itemCategory.Equals("A"))
                            {
                                wrnItem.Quantity = 1;
                                Model.Asset ass = context.Assets.FirstOrDefault(p => p.Id == wrnItem.AssetId);
                                ass.IsReleased = false;
                                ass.CurrentOwnerId = null;
                                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(ass, System.Data.EntityState.Modified);
                            }
                            Model.Inventory inv = context.Inventories.First(p => p.Id == wrnItem.InventoryId);
                            inv.Quantity += (Int64)wrnItem.Quantity;
                            ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(inv, System.Data.EntityState.Modified);
                        }
                        context.SaveChanges();
                        SessionData.CurrentSession.ReleaseOrderList = null;
                        SessionData.CurrentSession.ReleaseOrderItemList = null;
                        SessionData.CurrentSession.AssetList = null;
                        SessionData.CurrentSession.InventoryList = null;
                        scope.Complete();
                    }
                    catch (Exception ex) { scope.Dispose(); throw ex; }
                }
            }
        }

        public string GenerateUniquNumber(CountryProgramme cp)
        {
            string code = "WRO/DRC/" + cp.Country.ShortName + "/";
            string refNumber = "";
            long count = 1;

            var total = SessionData.CurrentSession.ReleaseOrderList.Where(p => p.IsSubmitted == true).Count();
            count = total;
            Model.WarehouseRelease m = null;
            do
            {
                count++;
                if (count < 100000)
                {
                    if (count < 10)
                        refNumber = code + "0000" + count;
                    if (count < 100 && count >= 10)
                        refNumber = code + "000" + count;
                    if (count < 1000 && count >= 100)
                        refNumber = code + "00" + count;
                    if (count < 10000 && count >= 1000)
                        refNumber = code + "0" + count;
                    if (count < 100000 && count >= 10000)
                        refNumber = code + count;
                }
                m = SessionData.CurrentSession.ReleaseOrderList.FirstOrDefault(p => p.RefNumber == refNumber);
            } while (m != null);
            return refNumber;

        }

        public List<Inventory> GetInventoryItems(Guid warehouseId)
        {
            return SessionData.CurrentSession.InventoryList.Where(p => p.WareHouseId == warehouseId).ToList();
        }

        public List<Model.Asset> GetAssets(Guid InventoryId)
        {
            var invt = SessionData.CurrentSession.InventoryList.FirstOrDefault(p => p.Id == InventoryId);
            return SessionData.CurrentSession.AssetList.Where(p => p.ItemId == invt.ItemId && p.CurrentWareHouseId == invt.WareHouseId && p.IsReleased == false && p.IsDesposed == false).ToList<Asset>();
        }

        public List<WarehouseRelease> GetWRNs()
        {
            return SessionData.CurrentSession.ReleaseOrderList.OrderByDescending(w => w.PreparedOn).ToList();
        }

        public List<WarehouseRelease> GetWRNsForApproval(Staff currentStaff)
        {
            return SessionData.CurrentSession.ReleaseOrderList.Where(w => w.IsSubmitted == true && w.IsApproved == false && w.IsRejected == false && w.ApprovedBy == currentStaff.Id).ToList();
        }

        public WarehouseRelease GetWROById(Guid Id)
        {
            return SessionData.CurrentSession.ReleaseOrderList.FirstOrDefault(p => p.Id == Id);
        }

        public bool EditRO(WarehouseRelease RO)
        {
            using (var context = new SCMSEntities())
            {
                var existing = context.WarehouseReleases.FirstOrDefault(p => p.Id == RO.Id);
                context.Entry(existing).CurrentValues.SetValues(RO);
                int affectedRecords = context.SaveChanges();
                SessionData.CurrentSession.ReleaseOrderList = null;
                SessionData.CurrentSession.ReleaseOrderItemList = null;
                return affectedRecords > 0;
            }
        }

        public bool SaveApproved(WarehouseRelease wro)
        {
            using (var context = new SCMSEntities())
            {
                context.WarehouseReleases.Attach(wro);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(wro, System.Data.EntityState.Modified);
                int affectedRecords = context.SaveChanges();
                SessionData.CurrentSession.ReleaseOrderList = null;
                SessionData.CurrentSession.ReleaseOrderItemList = null;
                return affectedRecords > 0;
            }
        }
    }
}
