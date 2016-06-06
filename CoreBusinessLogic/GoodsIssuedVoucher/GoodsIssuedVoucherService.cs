using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Transactions;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.GoodsIssuedVoucher
{
    public class GoodsIssuedVoucherService : IGoodsIssuedVoucherService
    {
        public bool IsGIVSaved(Model.GoodsIssuedVoucher giventity)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        context.GoodsIssuedVouchers.Add(giventity);
                        foreach (var item in giventity.ROItems)
                        {
                            if (item.IsRemoved)
                                continue;
                            var givitem = new GoodsIssuedVoucherItem()
                            {
                                Id = Guid.NewGuid(),
                                QTYDelivered = item.QTYReceived,
                                GoodsIssuedVoucherId = giventity.Id,
                                WarehouseReleaseItemId = item.ROItemId,
                                Remarks = item.Remarks
                            };
                            context.GoodsIssuedVoucherItems.Add(givitem);
                        }
                        int affectedRecords = context.SaveChanges();
                        scope.Complete();
                        SessionData.CurrentSession.GIVList = null;
                        SessionData.CurrentSession.ReleaseOrderList = null;
                        return affectedRecords > 0;
                    }
                    catch (Exception ex) { scope.Dispose(); throw ex; }
                }
            }
        }

        public bool IsGIVEdited(Model.GoodsIssuedVoucher giventity)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        var existing = context.GoodsIssuedVouchers.FirstOrDefault(p => p.Id == giventity.Id);
                        context.Entry(existing).CurrentValues.SetValues(giventity);
                        foreach (var item in giventity.ROItems)
                        {
                            GoodsIssuedVoucherItem mm;
                            var existingItem = mm = context.GoodsIssuedVoucherItems.FirstOrDefault(p => p.Id == item.GIVItemId);
                            mm.Remarks = item.Remarks;
                            mm.QTYDelivered = item.QTYReceived;
                            context.Entry(existingItem).CurrentValues.SetValues(mm);
                        }
                        int affectedRecords = context.SaveChanges();
                        scope.Complete();
                        SessionData.CurrentSession.GIVList = null;
                        SessionData.CurrentSession.ReleaseOrderList = null;
                        return affectedRecords > 0;
                    }
                    catch (Exception ex) { scope.Dispose(); throw ex; }
                }
            }
        }

        public bool IsGIVSubmited(Model.GoodsIssuedVoucher giventity)
        {
            using (var context = new SCMSEntities())
            {
                var existing = context.GoodsIssuedVouchers.FirstOrDefault(p => p.Id == giventity.Id);
                context.Entry(existing).CurrentValues.SetValues(giventity);
                int affectedRecords = context.SaveChanges();
                SessionData.CurrentSession.GIVList = null;
                return affectedRecords > 0;
            }
        }

        public bool IsGIVDeleted(Guid givId)
        {
            using (var context = new SCMSEntities())
            {
                var giv = context.GoodsIssuedVouchers.FirstOrDefault(p => p.Id == givId);
                context.GoodsIssuedVouchers.Remove(giv);
                int affectedRecords = context.SaveChanges();
                SessionData.CurrentSession.GIVList = null;
                SessionData.CurrentSession.ReleaseOrderList = null;
                return affectedRecords > 0;
            }
        }

        public bool IsGIVItemAdded(Model.GoodsIssuedVoucher giventity)
        {
            using (var context = new SCMSEntities())
            {
                foreach (var item in giventity.ROItems)
                {
                    if (item.IsRemoved)
                        continue;
                    var givitem = new GoodsIssuedVoucherItem()
                    {
                        Id = Guid.NewGuid(),
                        QTYDelivered = item.QTYReceived,
                        GoodsIssuedVoucherId = giventity.Id,
                        WarehouseReleaseItemId = item.ROItemId,
                        Remarks = item.Remarks
                    };
                    context.GoodsIssuedVoucherItems.Add(givitem);
                }
                int affectedRecords = context.SaveChanges();
                SessionData.CurrentSession.GIVList = null;
                SessionData.CurrentSession.ReleaseOrderList = null;
                return affectedRecords > 0;
            }
        }

        public bool IsGIVItemDeleted(Guid givItemId)
        {
            using (var context = new SCMSEntities())
            {
                var giv = context.GoodsIssuedVoucherItems.FirstOrDefault(p => p.Id == givItemId);
                context.GoodsIssuedVoucherItems.Remove(giv);
                int affectedRecords = context.SaveChanges();
                SessionData.CurrentSession.GIVList = null;
                SessionData.CurrentSession.ReleaseOrderList = null;
                return affectedRecords > 0;
            }
        }

        public string GenerateUniquNumber(CountryProgramme cp)
        {
            string code = "GIV/DRC/" + cp.Country.ShortName + "/";
            string refNumber = "";
            long count = 1;

            var total = SessionData.CurrentSession.GIVList.Where(p => p.IsSubmitted == true).Count();
            count = total;
            Model.GoodsIssuedVoucher m = null;
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
                m = SessionData.CurrentSession.GIVList.FirstOrDefault(p => p.RefNumber == refNumber);
            } while (m != null);
            return refNumber;
        }

        public List<Model.GoodsIssuedVoucher> GetGIVs()
        {
            return SessionData.CurrentSession.GIVList.OrderByDescending(p => p.PreparedOn).ToList();
        }

        public List<WarehouseRelease> GetGIVROs()
        {
            return SessionData.CurrentSession.ReleaseOrderList.Where(p => p.IsApproved).Where(ro => ro.WarehouseReleaseItems
                .Where(roitm => roitm.GoodsIssuedVoucherItems.Where(j => j.WarehouseReleaseItemId == roitm.Id).Sum(f => f.QTYDelivered) < roitm.Quantity)
                .Count() > 0).OrderByDescending(w => w.PreparedOn).ToList(); ;

        }

        public List<GivItemz> GetROItemsToAdd(Guid roId)
        {
            var WROs = SessionData.CurrentSession.ReleaseOrderList.OrderByDescending(w => w.PreparedOn).ToList();
            return WROs.FirstOrDefault(p => p.Id == roId).WarehouseReleaseItems
                      .Where(roitm => roitm.GoodsIssuedVoucherItems.Where(j => j.WarehouseReleaseItemId == roitm.Id).Sum(g => g.QTYDelivered) < roitm.Quantity)
                      .Select(k => new GivItemz
                      {
                          ROItemId = k.Id,
                          ItemName = k.Inventory.Item.Name,
                          Unit = k.Inventory.Item.UnitOfMeasure.Code,
                          QTYReleased = (Int64)k.Quantity,
                          PreviouslyReceived = k.GoodsIssuedVoucherItems.Where(y => y.WarehouseReleaseItemId == k.Id).Sum(q => q.QTYDelivered)
                      }).ToList<GivItemz>();
        }
    }
}
