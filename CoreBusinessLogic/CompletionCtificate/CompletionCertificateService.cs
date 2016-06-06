using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.Utils.DTOs;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.CoreBusinessLogic.CompletionCtificate
{
    public class CompletionCertificateService : ICompletionCertificateService
    {

        private INotificationService notificationService;

        private static void ClearSessionData()
        {
            SessionData.CurrentSession.CompletionCertificateList = null;
        }

        public CompletionCertificateService(INotificationService NotificationService)
        {
            this.notificationService = NotificationService;
        }

        public bool SaveCompletionC(Model.CompletionCertificate entity)
        {
            using (var db = new SCMSEntities())
            {
                try
                {
                    ClearSessionData();
                    entity.IsRejected = false;
                    db.CompletionCertificates.Add(entity);
                    return (db.SaveChanges() > 0) ? true : false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public bool UpdateCC(Model.CompletionCertificate entity)
        {
            using (var context = new SCMSEntities())
            {
                ClearSessionData();
                context.CompletionCertificates.Attach(entity);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                return context.SaveChanges() > 0;
            }
        }

        public string GenerateUniquNumber(CountryProgramme cp)
        {
            string code = "CC/DRC/" + cp.Country.ShortName + "/";
            string refNumber = "";
            long count = 1;
            using (var dbContext = new SCMSEntities())
            {
                var total = dbContext.CompletionCertificates.Where(p => p.CountryProgrammeId == cp.Id && p.IsSubmitted == true).Count();
                count = total;
                Model.CompletionCertificate m = null;
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
                    m = dbContext.CompletionCertificates.FirstOrDefault(p => p.RefNumber == refNumber);
                } while (m != null);
                return refNumber;
            }
        }

        public Model.CompletionCertificate GetCCById(Guid CCId)
        {
            return SessionData.CurrentSession.CompletionCertificateList.FirstOrDefault(c => c.Id == CCId);
        }

        public List<Model.PurchaseOrder> GetGRNPurchaseOrders()
        {
            return (from po in SessionData.CurrentSession.PurchaseOrderList
                    where po.IsApproved &&
                    !(from cc in SessionData.CurrentSession.CompletionCertificateList
                      where (bool)!cc.IsRejected
                      select cc.PurchaseOrderId).Contains(po.Id)
                    select po).ToList();
        }

        public List<Model.CompletionCertificate> GetCCNotes()
        {
            return SessionData.CurrentSession.CompletionCertificateList.OrderByDescending(c => c.PreparedOn).ToList();
        }

        public List<CompletionCertificate> GetCCsForApproval(Staff currentStaff)
        {
            return SessionData.CurrentSession.CompletionCertificateList.Where(c => c.IsSubmitted && !c.IsApproved && (bool)!c.IsRejected && c.ConfirmedBy == currentStaff.Id).ToList();
        }

        public List<CompletionCertificateSummary> Find(List<Guid> ids)
        {

            List<CompletionCertificateSummary> certs = new List<CompletionCertificateSummary>();

            using (var context = new SCMSEntities())
            {
                var results = from myCerts in context.CompletionCertificates
                              where ids.Contains(myCerts.Id)
                              select myCerts;

                foreach (Model.CompletionCertificate c in results.ToList())
                {
                    CompletionCertificateSummary tmp = new CompletionCertificateSummary();
                    tmp.Id = c.Id;
                    tmp.RefNumber = c.RefNumber;
                    tmp.ProjectTitle = c.ProjectTitle;
                    tmp.PONumber = c.PurchaseOrder.RefNumber;
                    tmp.Office = c.CountrySubOffice.Name;
                    tmp.Constructor = c.PurchaseOrder.Supplier.Name;

                    Staff confirmer = context.Staffs.First(s => s.Id == c.ConfirmedBy);

                    if (confirmer != null)
                    {
                        tmp.ConfirmedBy = confirmer.Person.FirstName + " " + confirmer.Person.OtherNames;
                    }
                    else
                    {
                        tmp.ConfirmedBy = "";
                    }
                    //
                    if (c.IsApproved)
                    {
                        tmp.Status = "AP";
                    }
                    else if (c.IsRejected == true)
                    {
                        tmp.Status = "RJ";
                    }
                    else if (c.IsSubmitted)
                    {
                        tmp.Status = "CR";
                    }
                    else
                    {
                        tmp.Status = "NEW";
                    }

                    certs.Add(tmp);
                }

            }

            return certs;
        }

    }
}
