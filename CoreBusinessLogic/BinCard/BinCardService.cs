using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.BinCard
{
    public class BinCardService : IBinCardService
    {

        public bool IsBinAdded(Model.Bin binEntity)
        {
            using (var db = new SCMSEntities())
            {
                if (binEntity.BinType == "OR") binEntity.ProcurementPlanItemId = null; else binEntity.OrderRequestItemId = null;
                db.Bins.Add(binEntity);
                if (db.SaveChanges() > 0) { SessionData.CurrentSession.BinList = null; return true; } else return false;
            }

        }

        public bool IsBinEdited(Model.Bin binEntity)
        {
            using (var db = new SCMSEntities())
            {
                db.Bins.Attach(binEntity);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(binEntity, System.Data.EntityState.Modified);
                if (db.SaveChanges() > 0) { SessionData.CurrentSession.BinList = null; return true; } else return false;

            }
        }

        public bool IsBinDeleted(Guid id)
        {
            using (var db = new SCMSEntities())
            {
                var bin = new Bin() { Id = id };
                db.Bins.Attach(bin);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(bin, System.Data.EntityState.Deleted);
                if (db.SaveChanges() > 0) { SessionData.CurrentSession.BinList = null; return true; } else return false;


            }
        }

        public bool IsBinItemAdded(Model.BinItem binItemEntity, Model.Bin binEntity)
        {
            using (var db = new SCMSEntities())
            {
                if (binEntity != null)
                {
                    binEntity.QTY = binItemEntity.QTYReceived;
                    db.Bins.Add(binEntity);
                    binItemEntity.BalanceStock = binItemEntity.QTYReceived;
                    db.BinItems.Add(binItemEntity);
                }
                else
                {
                    var bin = db.Bins.FirstOrDefault(p => p.Id == binItemEntity.BinId); bin.BinType = "any";
                    var binitem = db.BinItems.Where(p => p.BinId == binItemEntity.BinId).OrderByDescending(k => k.IssueDate).FirstOrDefault();
                    if (binItemEntity.QTYReceived > 0) { binItemEntity.BalanceStock = binItemEntity.QTYReceived + (binitem != null ? binitem.BalanceStock : 0); bin.QTY += binItemEntity.QTYReceived; }
                    else { binItemEntity.BalanceStock = binitem != null ? binitem.BalanceStock - binItemEntity.QTYIssued : binItemEntity.QTYIssued; bin.QTY -= binItemEntity.QTYIssued; }
                    db.BinItems.Add(binItemEntity);
                    ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(bin, System.Data.EntityState.Modified);
                }
                if (db.SaveChanges() > 0)
                {
                    SessionData.CurrentSession.BinList = null; SessionData.CurrentSession.BinItemList = null;
                    SessionData.CurrentSession.GoodsReceivedNoteItemList = null; SessionData.CurrentSession.ReleaseOrderItemList = null; return true;
                }
                else return false;
            }
        }

        public bool IsBinItemEdited(BinItem ItemEntity)
        {
            using (var db = new SCMSEntities())
            {
                var lastReceiveDate = GetAllBinItems().Where(b => b.IssueDate > ItemEntity.IssueDate);


                db.BinItems.Attach(ItemEntity);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(ItemEntity, System.Data.EntityState.Modified);
                if (db.SaveChanges() > 0) { SessionData.CurrentSession.BinList = null; return true; } else return false;

            }
        }

        public List<Model.Bin> GetAll()
        {
            return SessionData.CurrentSession.BinList.OrderByDescending(p => p.PreparedOn).ToList();
        }

        public List<Model.BinItem> GetAllBinItems()
        {
            return SessionData.CurrentSession.BinItemList.ToList();
        }

        public string GenerateUniquBCNo()
        {
            string code = "BC/";
            string refNumber = "";
            long count = 1;

            var total = this.GetAll().Count();// SessionData.CurrentSession.BinList.Count();
            count = total;
            Model.Bin m = null;
            do
            {
                count++;
                if (count < 100000)
                {
                    if (count < 10)
                        refNumber = code + "00000" + count;
                    if (count < 100 && count >= 10)
                        refNumber = code + "0000" + count;
                    if (count < 1000 && count >= 100)
                        refNumber = code + "000" + count;
                    if (count < 10000 && count >= 1000)
                        refNumber = code + "00" + count;
                    if (count < 100000 && count >= 10000)
                        refNumber = code + "0" + count;
                }
                m = SessionData.CurrentSession.BinList.FirstOrDefault(p => p.RefNumber == refNumber);
            } while (m != null);
            return refNumber;
        }

        public List<Model.OrderRequest> GetBinORs()
        {
            return SessionData.CurrentSession.OrderRequestList
                .Where(a => a.IsAuthorized == true && a.PurchaseOrders
                    .Where(po => po.PurchaseOrderItems
                        .Where(poit => poit.GoodsReceivedNoteItems.
                            Where(grnit => grnit.GoodsReceivedNote.Verified).Count() > 0).Count() > 0).Count() > 0).ToList();
        }

        public List<Model.ProcurementPlan> GetBinPPs()
        {
            return SessionData.CurrentSession.ProcurementPlanList
                .Where(a => a.IsAuthorized == true && a.ProcurementPlanItems
                    .Where(ppit => ppit.PurchaseOrderItems
                        .Where(poit => poit.GoodsReceivedNoteItems.
                            Where(grnit => grnit.GoodsReceivedNote.Verified).Count() > 0).Count() > 0).Count() > 0).ToList();
        }

        public List<Model.ProcurementPlanItem> GetProcurementPlanItems(Guid ppId)
        {
            var pp = SessionData.CurrentSession.ProcurementPlanList.FirstOrDefault(o => o.Id == ppId);
            return pp.ProcurementPlanItems.ToList();
        }

        public List<WareHouse> GetWarehousesFromPP(Guid ppId)
        {
            var pp = SessionData.CurrentSession.ProcurementPlanList.FirstOrDefault(p => p.Id == ppId);
            return SessionData.CurrentSession.WarehouseList
                .Where(wh => wh.GoodsReceivedNotes
                    .Where(k => k.GoodsReceivedNoteItems
                        .Where(u => u.PurchaseOrderItem.ProcurementPlanItem.IsNotNull()
                            && u.PurchaseOrderItem.ProcurementPlanItem.ProcurementPlan.Id == pp.Id).Count() > 0).Count() > 0).ToList();
        }

        public List<ItemPackage> GetPPItemPackages(Guid PPitemId)
        {
            var pp = SessionData.CurrentSession.ProcurementPlanList.Where(o => o.ProcurementPlanItems.Select(i => i.Id).Contains(PPitemId)).FirstOrDefault();
            return SessionData.CurrentSession.ItemPackageList.Where(p => p.ItemId == pp.ProcurementPlanItems.FirstOrDefault(o => o.Id == PPitemId).ItemId).ToList();
        }

        public List<ItemPackage> GetItemPackages(Guid ORitemId)
        {
            var or = SessionData.CurrentSession.OrderRequestList.Where(o => o.OrderRequestItems.Select(i => i.Id).Contains(ORitemId)).FirstOrDefault();
            return SessionData.CurrentSession.ItemPackageList.Where(p => p.ItemId == or.OrderRequestItems.FirstOrDefault(o => o.Id == ORitemId).ItemId).ToList();
        }

        public List<Model.GoodsReceivedNoteItem> GetGRNItemz4rmbin(Model.Bin bin)
        {
            var or = SessionData.CurrentSession.OrderRequestList.FirstOrDefault(p => p.OrderRequestItems.Where(k => k.Id == bin.OrderRequestItemId).Count() > 0);
            var ppitm = SessionData.CurrentSession.ProcurementPlanItemList.FirstOrDefault(p => p.Id == bin.ProcurementPlanItemId);
            if (ppitm != null)
                return SessionData.CurrentSession.GoodsReceivedNoteItemList
                    .Where(gitmz => gitmz.PurchaseOrderItem.ProcurementPlanItem != null
                        && gitmz.PurchaseOrderItem.ProcurementPlanItem.ProcurementPlan.Id == ppitm.ProcurementPlan.Id
                            && gitmz.GoodsReceivedNote.WareHouseId == bin.WareHouseId
                                && (gitmz.BinItems.Count == 0 || gitmz.BinItems.Sum(d => d.QTYReceived) < (Int64)gitmz.QuantityDelivered)
                                    && gitmz.PurchaseOrderItem.Item.Id == ppitm.ItemId && gitmz.GoodsReceivedNote.Verified).ToList();
            else
            {
                var oritm = or.OrderRequestItems.FirstOrDefault(p => p.Id == bin.OrderRequestItemId);
                return SessionData.CurrentSession.GoodsReceivedNoteItemList
                     .Where(gitmz => gitmz.PurchaseOrderItem.OrderRequestItem != null
                         && gitmz.PurchaseOrderItem.OrderRequestItem.OrderRequest.Id == oritm.OrderRequest.Id
                             && gitmz.GoodsReceivedNote.WareHouseId == bin.WareHouseId
                                 && (gitmz.BinItems.Count == 0 || gitmz.BinItems.Sum(d => d.QTYReceived) < (Int64)gitmz.QuantityDelivered)
                                     && gitmz.PurchaseOrderItem.Item.Id == oritm.ItemId && gitmz.GoodsReceivedNote.Verified).ToList();
            }
        }

        public List<Model.GoodsReceivedNoteItem> GetGRNItemz4rmOR(Guid binId)
        {
            var bin = SessionData.CurrentSession.BinList.FirstOrDefault(p => p.Id == binId);
            var oritm = bin.OrderRequestItem;
            var ppitm = bin.ProcurementPlanItem;
            if (oritm != null)
                return SessionData.CurrentSession.GoodsReceivedNoteItemList
                    .Where(gitmz => gitmz.PurchaseOrderItem.OrderRequestItem != null
                        && gitmz.PurchaseOrderItem.OrderRequestItem.OrderRequest.Id == oritm.OrderRequest.Id
                            && gitmz.GoodsReceivedNote.WareHouseId == bin.WareHouseId
                                && (gitmz.BinItems.Count == 0 || gitmz.BinItems.Sum(d => d.QTYReceived) < (Int64)gitmz.QuantityDelivered)
                                    && gitmz.PurchaseOrderItem.Item.Id == oritm.ItemId && gitmz.GoodsReceivedNote.Verified).ToList();
            else
                return SessionData.CurrentSession.GoodsReceivedNoteItemList
                    .Where(gitmz => gitmz.PurchaseOrderItem.ProcurementPlanItem != null
                        && gitmz.PurchaseOrderItem.ProcurementPlanItem.ProcurementPlan.Id == ppitm.ProcurementPlan.Id
                            && gitmz.GoodsReceivedNote.WareHouseId == bin.WareHouseId
                                && (gitmz.BinItems.Count == 0 || gitmz.BinItems.Sum(d => d.QTYReceived) < (Int64)gitmz.QuantityDelivered)
                                    && gitmz.PurchaseOrderItem.Item.Id == ppitm.ItemId && gitmz.GoodsReceivedNote.Verified).ToList();
        }

        public List<WareHouse> GetWarehousesFromOR(Guid orId)
        {
            var or = SessionData.CurrentSession.OrderRequestList.FirstOrDefault(p => p.Id == orId);
            return SessionData.CurrentSession.WarehouseList
                .Where(p => p.GoodsReceivedNotes
                    .Where(k => k.GoodsReceivedNoteItems
                        .Where(u => u.PurchaseOrderItem.PurchaseOrder.OrderRequest.IsNotNull() && u.PurchaseOrderItem.PurchaseOrder.OrderRequest.Id == or.Id).Count() > 0).Count() > 0).ToList();
        }

        public List<Model.WarehouseReleaseItem> GetWRItemByBinId(Guid binId)
        {
            var bin = SessionData.CurrentSession.BinList.FirstOrDefault(p => p.Id == binId);
            if (bin.BinItems.Count(p => p.GoodsReceivedNoteItemId.IsNotNull()) == 0) return new List<Model.WarehouseReleaseItem>();
            var oritm = bin.OrderRequestItem;
            var ppitm = bin.ProcurementPlanItem;
            if (oritm != null)
                return SessionData.CurrentSession.ReleaseOrderItemList.Where(roit => roit.Inventory.ItemId == oritm.ItemId
                    && roit.WarehouseRelease.WareHouseId == bin.WareHouseId
                        && roit.BinItems.Sum(d => d.QTYIssued) < roit.Quantity).ToList();
            else
                return SessionData.CurrentSession.ReleaseOrderItemList.Where(roit => roit.Inventory.ItemId == ppitm.ItemId
                && roit.WarehouseRelease.WareHouseId == bin.WareHouseId
                    && roit.BinItems.Sum(d => d.QTYIssued) < roit.Quantity).ToList();
        }
    }
}
