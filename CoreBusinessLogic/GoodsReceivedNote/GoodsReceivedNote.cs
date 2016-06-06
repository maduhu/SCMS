using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using System.Transactions;
using System.Data.Objects.DataClasses;
using SCMS.CoreBusinessLogic.NotificationsManager;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.CoreBusinessLogic._GoodsReceivedNote
{
    public class GoodsReceivedNoteService : IGoodsReceivedNoteService
    {

        public bool SaveGRN(GoodsReceivedNote GRNEntity)
        {
            using (var dbContext = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        dbContext.GoodsReceivedNotes.Add(GRNEntity);
                        foreach (POItemsView item in GRNEntity.POItemz)
                        {
                            var newEntity = new GoodsReceivedNoteItem()
                            {
                                Id = Guid.NewGuid(),
                                GoodsReceivedNoteId = GRNEntity.Id,
                                PurchaseOrderItemId = item.POItemId,
                                QuantityDelivered = item.QtyDelivered,
                                QuantityDamaged = item.QtyDamaged,
                                Comments = item.comments,
                                IsInventory = item.IsInventory
                            };
                            dbContext.GoodsReceivedNoteItems.Add(newEntity);
                        }
                        if ((dbContext.SaveChanges() > 0))
                        {
                            scope.Complete();
                            SessionData.CurrentSession.GoodsReceivedNoteList = null;
                            SessionData.CurrentSession.GoodsReceivedNoteItemList = null;
                            SessionData.CurrentSession.PurchaseOrderList = null;
                            SessionData.CurrentSession.ProcurementPlanList = null;
                            return true;
                        }
                        else { scope.Dispose(); return false; }
                    }
                    //catch (DbEntityValidationException e)
                    //{
                    //    foreach (var eve in e.EntityValidationErrors)
                    //    {
                    //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    //        foreach (var ve in eve.ValidationErrors)
                    //        {
                    //            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                    //                ve.PropertyName, ve.ErrorMessage);
                    //        }
                    //    }
                    //    throw;
                    //}
                    catch (Exception ex) { scope.Dispose(); throw ex; }
                }
            }
        }

        public bool IsGRNVerified(GoodsReceivedNote GRNentity)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (GoodsReceivedNoteItem item in GRNentity.ItemColl)
                    {
                        GoodsReceivedNoteItem grit = context.GoodsReceivedNoteItems.FirstOrDefault(p => p.Id == item.Id);
                        if (context.Inventories.Count(p => p.ItemId == grit.PurchaseOrderItem.Item.Id && p.WareHouseId == grit.GoodsReceivedNote.WareHouseId) > 0)
                        {
                            if (grit.PurchaseOrderItem.Item.ItemCategory.CategoryCode == "C")
                            {
                                Model.Inventory invt = context.Inventories.FirstOrDefault(p => p.ItemId == grit.PurchaseOrderItem.Item.Id && p.WareHouseId == grit.GoodsReceivedNote.WareHouseId);
                                invt.Quantity += (long)item.QuantityDelivered;
                                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(invt, System.Data.EntityState.Modified);
                            }
                        }
                        else
                        {
                            long qty = 0;
                            if (grit.PurchaseOrderItem.Item.ItemCategory.CategoryCode == "C") qty = (long)item.QuantityDelivered;
                            Model.Inventory newinvetoryEntity =
                                new Model.Inventory() { Id = Guid.NewGuid(), ItemId = grit.PurchaseOrderItem.Item.Id, Quantity = qty, CountryProgrammeId = GRNentity.CountryProgrammeId, WareHouseId = (Guid)grit.GoodsReceivedNote.WareHouseId };
                            context.Inventories.Add(newinvetoryEntity);
                        }
                        Model.GoodsReceivedNoteItem grnitm = context.GoodsReceivedNoteItems.FirstOrDefault(p => p.Id == item.Id);
                        grnitm.QuantityDamaged = item.QuantityDamaged; grnitm.QuantityDelivered = item.QuantityDelivered;
                        ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(grnitm, System.Data.EntityState.Modified);
                        context.SaveChanges();
                    }
                    Model.GoodsReceivedNote grn = context.GoodsReceivedNotes.FirstOrDefault(p => p.Id == GRNentity.Id);
                    grn.Verified = true;
                    grn.ApprovedOn = DateTime.Now;
                    grn.ReceptionApprovedBy = GRNentity.ReceptionApprovedBy;
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(grn, System.Data.EntityState.Modified);
                    if (context.SaveChanges() > 0) { scope.Complete(); return true; } else { scope.Dispose(); return false; }
                }
            }
        }

        public bool IsAssetRegistered(Model.Asset assetEntity)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        assetEntity.IsAssigned = false;
                        assetEntity.Id = Guid.NewGuid();
                        //if (assetEntity.UseLifeSpan)
                        //    assetEntity.DepreciationType = "Straight Line";
                        context.Assets.Add(assetEntity);
                        if (context.Inventories.Count(p => p.ItemId == assetEntity.ItemId && p.WareHouseId == assetEntity.CurrentWareHouseId) > 0)
                        {
                            Model.Inventory invt = context.Inventories.FirstOrDefault(p => p.ItemId == assetEntity.ItemId && p.WareHouseId == assetEntity.CurrentWareHouseId);
                            invt.Quantity += 1;
                            ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(invt, System.Data.EntityState.Modified);
                        }
                        else
                        {
                            Model.Inventory newinvetoryEntity = new Model.Inventory() { Id = Guid.NewGuid(), ItemId = (Guid)assetEntity.ItemId, Quantity = 1, CountryProgrammeId = assetEntity.CountryProgramId, WareHouseId = (Guid)assetEntity.CurrentWareHouseId };
                            context.Inventories.Add(newinvetoryEntity);
                        }

                        if (assetEntity.IsFleet)
                        {
                            SessionData.CurrentSession.FleetDetailsList = null;
                            context.FleetDetails.Add(new FleetDetail() { Id = Guid.NewGuid(), CountryProgrammeId = assetEntity.CountryProgramId, AssetId = assetEntity.Id, IssueDate = DateTime.Now });
                        }

                        if (assetEntity.DepreciationType == "Zero Percentage")
                        {
                            if (!(context.SaveChanges() > 0)) { scope.Dispose(); return false; } else { scope.Complete(); return true; }
                        }
                        else
                            assetEntity.DepreciationPeriods = assetEntity.Lifespan * 12;
                        return this.IsDepreciationComputed(context, scope, assetEntity);
                    }
                    catch (Exception ex) { scope.Dispose(); throw ex; }
                }
            }
        }

        private bool IsDepreciationComputed(SCMSEntities db, TransactionScope scope, Asset assetEntity)
        {
            //var firstdateinMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
            //var lastdateinMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
            //float purchasePrc = (float)gitm.PurchaseOrderItem.UnitPrice
            Model.Depreciation deprEntity = null;
            Model.GoodsReceivedNoteItem gitm = db.GoodsReceivedNoteItems.FirstOrDefault(p => p.Id == assetEntity.GoodsReceivedNoteItemId);
            float purchasePrc = (float)assetEntity.PurchaseValue, percentageDepr = (float)assetEntity.PercentageDepr, accAnnualDepr = 0;
            float annualDepr = 0, monthlyDepr, netbookValue = purchasePrc, accumulatedDepr = 0, salvagevalue = (float)assetEntity.SalvageValue;
            int periodcount = 0, monthcount = 0, numberOfYears = 0;

            if (assetEntity.DepreciationType == "Reducing Balance")
                monthlyDepr = ReducingBalance(db, assetEntity, ref deprEntity, percentageDepr, ref accAnnualDepr, ref annualDepr, ref netbookValue, ref accumulatedDepr, salvagevalue, ref periodcount, ref monthcount, ref numberOfYears);
            else
                monthlyDepr = StraightLine(db, assetEntity, ref deprEntity, purchasePrc, percentageDepr, ref accAnnualDepr, ref annualDepr, ref netbookValue, ref accumulatedDepr, salvagevalue, ref periodcount, ref monthcount);

            if (!(db.SaveChanges() > 0)) { scope.Dispose(); SessionData.CurrentSession.DepreciationList = null; return false; }
            scope.Complete(); return true;
        }

        private static float ReducingBalance(SCMSEntities db, Asset assetEntity, ref Model.Depreciation deprEntity, float percentageDepr, ref float accAnnualDepr,
            ref float annualDepr, ref float netbookValue, ref float accumulatedDepr, float salvagevalue, ref int periodcount, ref int monthcount, ref int numberOfYears)
        {
            float monthlyDepr;
            while (true)
            {
                annualDepr = ((netbookValue - salvagevalue) * (percentageDepr / 100));
                monthlyDepr = annualDepr / 12; numberOfYears++;

                if (assetEntity.UseLifeSpan)
                { if (numberOfYears > assetEntity.Lifespan) break; }
                else { if (annualDepr <= 12) break; }

                do
                {
                    monthcount++; periodcount++;
                    accumulatedDepr += monthlyDepr;
                    accAnnualDepr += monthlyDepr;
                    netbookValue -= monthlyDepr;
                    deprEntity = new Depreciation();
                    deprEntity.Id = Guid.NewGuid();
                    deprEntity.AssetId = assetEntity.Id;
                    deprEntity.Period = periodcount;
                    deprEntity.NetbookValue = netbookValue;
                    deprEntity.AccDepreciation = accumulatedDepr;
                    deprEntity.MonthlyDepreciation = monthlyDepr;
                    deprEntity.AnnualDepreciation = accAnnualDepr;
                    deprEntity.Date = new DateTime(DateTime.Now.AddMonths(periodcount).Year, DateTime.Now.AddMonths(periodcount).Month, 1).AddDays(-1);
                    db.Depreciations.Add(deprEntity);

                } while (monthcount <= 11); monthcount = 0; accAnnualDepr = 0;

            }
            return monthlyDepr;
        }

        private static float StraightLine(SCMSEntities db, Asset assetEntity, ref Model.Depreciation deprEntity, float purchasePrc, float percentageDepr,
            ref float accAnnualDepr, ref float annualDepr, ref float netbookValue, ref float accumulatedDepr, float salvagevalue, ref int periodcount, ref int monthcount)
        {
            float monthlyDepr;
            while (true)
            {

                if (assetEntity.UseLifeSpan)
                    annualDepr = (float)((float)(purchasePrc - (float)assetEntity.SalvageValue) / assetEntity.Lifespan);
                else annualDepr = (float)((float)(purchasePrc - (float)assetEntity.SalvageValue) * (percentageDepr / 100));
                periodcount++; monthcount++;
                monthlyDepr = annualDepr / 12;
                accumulatedDepr += monthlyDepr;
                netbookValue -= monthlyDepr;
                accAnnualDepr += monthlyDepr;

                deprEntity = new Depreciation();
                deprEntity.Id = Guid.NewGuid();
                deprEntity.AssetId = assetEntity.Id;
                deprEntity.Period = periodcount;
                deprEntity.NetbookValue = netbookValue;
                deprEntity.AccDepreciation = accumulatedDepr;
                deprEntity.MonthlyDepreciation = monthlyDepr;
                deprEntity.AnnualDepreciation = accAnnualDepr;
                deprEntity.Date = new DateTime(DateTime.Now.AddMonths(periodcount).Year, DateTime.Now.AddMonths(periodcount).Month, 1).AddDays(-1);
                db.Depreciations.Add(deprEntity);
                if (monthcount == 12) { monthcount = 0; accAnnualDepr = 0; }
                if (Math.Round(netbookValue) <= Math.Round(salvagevalue)) break;
            }
            return monthlyDepr;
        }

        public bool IsInventoryUpdated(Inventory InventEntity)
        {
            using (var context = new SCMSEntities())
            {
                if (context.Inventories.Count(p => p.ItemId == InventEntity.ItemId && p.WareHouseId == InventEntity.WareHouseId) > 0)
                {
                    Model.Inventory invt = context.Inventories.FirstOrDefault(p => p.ItemId == InventEntity.ItemId && p.WareHouseId == InventEntity.WareHouseId);
                    invt.Quantity += (long)InventEntity.Quantity;
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(invt, System.Data.EntityState.Modified);
                }
                else
                {
                    Model.Inventory newinvetoryEntity = new Model.Inventory() { Id = Guid.NewGuid(), ItemId = InventEntity.ItemId, Quantity = (long)InventEntity.Quantity, CountryProgrammeId = InventEntity.CountryProgrammeId, WareHouseId = InventEntity.WareHouseId };
                    context.Inventories.Add(newinvetoryEntity);
                }
                return context.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsAssetDisposed(Model.Asset AssetEntity)
        {
            using (var context = new SCMSEntities())
            {
                Model.Asset aset = context.Assets.FirstOrDefault(p => p.Id == AssetEntity.Id);
                aset.IsDesposed = true;
                aset.ActionType = AssetEntity.ActionType;
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(aset, System.Data.EntityState.Modified);

                Model.Inventory inv = context.Inventories.FirstOrDefault(p => p.ItemId == aset.ItemId & p.WareHouseId == aset.CurrentWareHouseId);
                inv.Quantity -= 1;
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(inv, System.Data.EntityState.Modified);
                return context.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsAssetStateChanged(Model.AssetManagment Entity)
        {
            using (var context = new SCMSEntities())
            {
                context.AssetManagments.Add(Entity);

                Model.Asset aset = context.Assets.FirstOrDefault(p => p.Id == Entity.AssetId);
                aset.IsAssetStateChanged = true;
                if (Entity.currentProjectId != null)
                    aset.CurrentProjectDonorId = Entity.currentProjectId;
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(aset, System.Data.EntityState.Modified);
                return context.SaveChanges() > 0 ? true : false;
            }
        }

        public List<Model.WareHouse> GetSubOfficeWareHouses(Guid countrysubofficeId)
        {
            return SessionData.CurrentSession.WarehouseList.Where(p => p.SubOfficeId == countrysubofficeId).OrderBy(c => c.Name).ToList();
        }

        public List<Model.PurchaseOrder> GetGRNPurchaseOrders()
        {
            return SessionData.CurrentSession.PurchaseOrderList.Where(p => p.IsApproved)
                .Where(p => p.PurchaseOrderItems
                    .Where(poit => poit.GoodsReceivedNoteItems.Sum(s => s.QuantityDelivered) < poit.Quantity)
                        .Count() > 0).ToList();
        }

        public List<POItemsView> GetPOItemsDetails(Guid POId)
        {
            return SessionData.CurrentSession.PurchaseOrderList.FirstOrDefault(p => p.Id == POId).PurchaseOrderItems
                .Where(poit => poit.GoodsReceivedNoteItems.Sum(s => s.QuantityDelivered) < poit.Quantity)
                        .Select(n => new POItemsView()
                        {
                            POItemId = n.Id,
                            ItemName = n.Item.Name,
                            Description = n.ItemDescription,
                            QtyOrdered = n.Quantity,
                            PreviouslyReceived = (int?)n.GoodsReceivedNoteItems.Sum(a => a.QuantityDelivered) ?? 0,
                            packs = n.Item.ItemPackages.Where(p => p.ItemId == n.Item.Id),
                            unitOfMessure = n.Item.UnitOfMeasure.Code,
                            IsInventory = true,
                            PurchaseOrderItem = n
                        }).ToList<POItemsView>();
        }

        public Model.PurchaseOrder GetPO(Guid POid)
        {
            return SessionData.CurrentSession.PurchaseOrderList.FirstOrDefault(p => p.Id == POid);
        }

        public string GenerateAssetNo(Model.GoodsReceivedNoteItem GRNItem)
        {
            string code = GRNItem.PurchaseOrderItem.ProjectDonor.Donor.ShortName + "/";
            code += GRNItem.PurchaseOrderItem.ProjectDonor.Project.ShortName + "/";
            long count = 1;
            Model.Asset m = SessionData.CurrentSession.AssetList.OrderByDescending(p => p.Index).FirstOrDefault();
            if (m != null)
                count = m.Index + 1;

            if (count < 10000)
            {
                if (count < 10)
                    return code + "0000" + count;
                if (count < 100 & count >= 10)
                    return code + "000" + count;
                if (count < 1000 & count >= 100)
                    return code + "00" + count;
                if (count < 10000 & count >= 1000)
                    return code + "0" + count;
            }
            return code + count;
        }

        public string GenerateAssetNo(Guid ProjectDId)
        {
            using (var dbContext = new SCMSEntities())
            {
                Model.ProjectDonor pd = dbContext.ProjectDonors.FirstOrDefault(p => p.Id == ProjectDId);
                string code = pd.Donor.ShortName + "/";
                code += pd.Project.ShortName + "/";
                long count = 1;
                Model.Asset m = dbContext.Assets.OrderByDescending(p => p.Index).FirstOrDefault();
                if (m != null)
                    count = m.Index + 1;

                if (count < 10000)
                {
                    if (count < 10)
                        return code + "0000" + count;
                    if (count < 100 & count >= 10)
                        return code + "000" + count;
                    if (count < 1000 & count >= 100)
                        return code + "00" + count;
                    if (count < 10000 & count >= 1000)
                        return code + "0" + count;
                }
                return code + count;
            }
        }

        public string GenerateUniquNumber(CountryProgramme cp)
        {
            string code = "GRN/DRC/" + cp.Country.ShortName + "/";
            string refNumber = "";
            long count = 1;

            var total = SessionData.CurrentSession.GoodsReceivedNoteList.Where(p => p.IsSubmitted == true).Count();
            count = total;
            Model.GoodsReceivedNote m = null;
            do
            {
                count++;
                if (count < 10000)
                {
                    if (count < 10)
                        refNumber = code + "0000" + count;
                    if (count < 100 & count >= 10)
                        refNumber = code + "000" + count;
                    if (count < 1000 & count >= 100)
                        refNumber = code + "00" + count;
                    if (count < 10000 & count >= 1000)
                        refNumber = code + "0" + count;
                }
                m = SessionData.CurrentSession.GoodsReceivedNoteList.FirstOrDefault(p => p.RefNumber == refNumber);
            } while (m != null);
            return refNumber;
        }

        public List<GoodsReceivedNoteItem> GetUnregisteredGRNItems()
        {
            return SessionData.CurrentSession.GoodsReceivedNoteItemList
                .Where(p => p.GoodsReceivedNote.Verified == true &&
                    p.PurchaseOrderItem.Item.ItemCategory.CategoryCode == "A" && p.Assets.Count() < p.QuantityDelivered).ToList();
        }

        public List<GoodsReceivedNote> GetGRNsForApproval(Staff currentStaff)
        {
            try
            {
                return SessionData.CurrentSession.GoodsReceivedNoteList
                .Where(g => g.IsSubmitted == true && g.Verified == false && g.IsRejected == false && g.ReceptionApprovedBy == currentStaff.Id).ToList();
            }
            catch (Exception ex)
            {
                return new List<Model.GoodsReceivedNote>();
            }
        }

        public GoodsReceivedNote GetGRNById(Guid id)
        {
            return SessionData.CurrentSession.GoodsReceivedNoteList.FirstOrDefault(g => g.Id == id);
        }

        public void UpdateGRNItem(GoodsReceivedNoteItem grnItem)
        {
            using (var context = new SCMSEntities())
            {
                var existing = context.GoodsReceivedNoteItems.FirstOrDefault(p => p.Id == grnItem.Id);
                context.Entry(existing).CurrentValues.SetValues(grnItem);
                context.SaveChanges();
                SessionData.CurrentSession.PurchaseOrderList = null;
                SessionData.CurrentSession.GoodsReceivedNoteList = null;
                SessionData.CurrentSession.GoodsReceivedNoteItemList = null;
            }
        }

        public void UpdateGRN(GoodsReceivedNote grn)
        {
            using (var context = new SCMSEntities())
            {
                var existing = context.GoodsReceivedNotes.FirstOrDefault(p => p.Id == grn.Id);
                context.Entry(existing).CurrentValues.SetValues(grn);
                context.SaveChanges();
                SessionData.CurrentSession.PurchaseOrderList = null;
                SessionData.CurrentSession.GoodsReceivedNoteList = null;
                SessionData.CurrentSession.GoodsReceivedNoteItemList = null;
            }
        }

        public void DeleteGRNItem(Guid GRNItemId)
        {
            using (var context = new SCMSEntities())
            {
                var grnItem = new GoodsReceivedNoteItem { Id = GRNItemId };
                context.GoodsReceivedNoteItems.Attach(grnItem);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(grnItem, System.Data.EntityState.Deleted);
                context.SaveChanges();
                SessionData.CurrentSession.PurchaseOrderList = null;
                SessionData.CurrentSession.GoodsReceivedNoteList = null;
                SessionData.CurrentSession.GoodsReceivedNoteItemList = null;
            }
        }

        public void DeleteGRNById(Guid id)
        {
            using (var context = new SCMSEntities())
            {
                var grn = new GoodsReceivedNote { Id = id };
                context.GoodsReceivedNotes.Attach(grn);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(grn, System.Data.EntityState.Deleted);
                context.SaveChanges();
                SessionData.CurrentSession.PurchaseOrderList = null;
                SessionData.CurrentSession.GoodsReceivedNoteList = null;
                SessionData.CurrentSession.GoodsReceivedNoteItemList = null;
            }
        }
    }
}
