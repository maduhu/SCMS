using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using System.Transactions;
using SCMS.CoreBusinessLogic.NotificationsManager;
using System.Data.Entity.Infrastructure;

namespace SCMS.CoreBusinessLogic.WB
{
    public class WayBillService : IWayBillService
    {
        private INotificationService NotificationService;

        public WayBillService(INotificationService NotificationService)
        {
            this.NotificationService = NotificationService;
        }

        public bool SaveWB(Model.WayBill entity)
        {
            using (var db = new SCMSEntities())
            {
                db.WayBills.Add(entity);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool ReceiveWB(Model.WayBill entity, List<WarehouseReleaseItem> writemList)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        StringBuilder msg = new StringBuilder();
                        Model.WayBill wb = context.WayBills.First(p => p.Id == entity.Id);
                        wb.ReceivedBy = entity.ReceivedBy;
                        wb.ArrivalDate = entity.ArrivalDate;
                        wb.RecvDRCVehicleOdometer = entity.RecvDRCVehicleOdometer;
                        wb.RecvDRCVehicleTotalDistance = entity.RecvDRCVehicleTotalDistance;
                        wb.RecvDstnationOfficeId = entity.RecvDstnationOfficeId;
                        wb.IsReceived = true;
                        ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(wb, System.Data.EntityState.Modified);
                        int count = 0;
                        foreach (WarehouseReleaseItem item in writemList)
                        {
                            Model.WarehouseReleaseItem it = context.WarehouseReleaseItems.First(p => p.Id == item.Id);
                            if (it.Quantity > item.QuantityReceived)
                            {
                                count++;
                                msg.Append(string.Format("{0}.  Item Name: {1}\t\tIssued Qty: {2}\tQty Received: {3}\tQty damaged: {4}\n", count, it.Inventory.Item.Name, it.Quantity, item.QuantityReceived, item.QuantityDamaged));
                            }
                            it.QuantityReceived = item.QuantityReceived;
                            it.QuantityDamaged = item.QuantityDamaged;
                            ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(it, System.Data.EntityState.Modified);
                        }
                        if ((context.SaveChanges() > 0))
                        {
                            if (!msg.ToString().IsNullOrEmpty())
                                NotificationService.SendNotification(NotificationService.GetStaffEmailAddress(wb.PreparedBy), string.Format(string.Empty, wb.RefNumber, msg.ToString()), NotificationHelper.wbsubject);
                            scope.Complete(); return true;
                        }
                        else { scope.Dispose(); return false; }
                    }
                    catch (Exception ex) { scope.Dispose(); throw new Exception(ex.Message); }
                }
            }
        }

        public List<Model.WayBill> GetWayBillsNotReceived(Guid CPId)
        {
            using (var db = new SCMSEntities())
            {
                return db.WayBills.Where(p => p.CountryProgrammeId == CPId & p.IsReceived == false).OrderBy(w => w.RefNumber).ToList();
            }
        }

        public string GenerateUniquNumber(CountryProgramme cp)
        {
            string code = "WB/DRC/" + cp.Country.ShortName + "/";
            string refNumber = "";
            long count = 1;
            using (var dbContext = new SCMSEntities())
            {
                var total = dbContext.WayBills.Where(p => p.CountryProgrammeId == cp.Id).Count();
                count = total;
                Model.WayBill m = null;
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
                    m = dbContext.WayBills.FirstOrDefault(p => p.RefNumber == refNumber);
                } while (m != null);
                return refNumber;
            }
        }

        public List<Model.CountrySubOffice> GetSubOffices(Guid CPId)
        {
            using (var db = new SCMSEntities())
            {

                return db.CountrySubOffices.Where(c => c.CountryProgrammeId == CPId).OrderBy(c => c.Name).ToList();

                //List<object> mm = (from n in db.CountrySubOffices
                //                   join i in db.Locations on n.LocationId equals i.Id
                //                   where n.CountryProgrammeId == CPId
                //                   select new
                //                   {
                //                       n.Id,
                //                       i.Name
                //                   }).ToList<object>();
                //return mm;
            }
        }
    }
}
