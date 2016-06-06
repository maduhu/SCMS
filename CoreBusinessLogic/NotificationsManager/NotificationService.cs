using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Mail;
using System.Threading.Tasks;
using SCMS.Model;
using SCMS.CoreBusinessLogic._ExchangeRate;
using System.Data.Entity.Infrastructure;

namespace SCMS.CoreBusinessLogic.NotificationsManager
{
    public class NotificationService : INotificationService
    {
        private IExchangeRateService exchangeRateService;

        public NotificationService(IExchangeRateService exchangeRateService)
        {
            this.exchangeRateService = exchangeRateService;
        }

        public void SendNotification(string ToAddress, string msgBody, string subject, bool isHtml = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(new MailAddress(ToAddress).Address))
                    Task.Factory.StartNew(() => SendEmail(ToAddress, msgBody, subject, isHtml));
            }
            catch (Exception ex) { }
        }

        private static void SendEmail(string ToAddress, string msgBody, string subject, bool isHtml = false)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                MailMessage msg = new MailMessage();
                msg.To.Add(ToAddress.Trim());
                msg.From = new MailAddress("logistics.dev@gmail.com", "SCMS");
                msg.Subject = subject;
                msg.IsBodyHtml = isHtml;
                msg.Body = msgBody.Trim();
                SmtpClient clent = new SmtpClient("smtp.gmail.com", 587);//smtp.gmail.com
                //clent.Port = 587;
                clent.Credentials = new System.Net.NetworkCredential("logistics.dev@gmail.com", "logistics.dev");
                clent.EnableSsl = true;
                clent.Send(msg);
            }
            catch (Exception ex)
            {
            }
        }

        public string GetApproverEmailAddress(int Priority, string activityCode)
        {
            using (var dbContext = new SCMSEntities())
            {
                Approver approv = dbContext.Approvers.FirstOrDefault(p => p.Priority == Priority && p.ActivityCode == activityCode);
                if (approv != null)
                {
                    SystemUser su = approv.SystemUser;
                    return su.Staff.Person.OfficialEmail;
                }
                else return string.Empty;

            }
        }

        public string GetApproverEmailAddress(int Priority, string activityCode, string actionType)
        {
            using (var dbContext = new SCMSEntities())
            {
                Approver approv = dbContext.Approvers.SingleOrDefault(p => p.Priority == Priority && p.ActivityCode == activityCode && p.ActionType == actionType);
                if (approv != null)
                {
                    SystemUser su = approv.SystemUser;
                    return su.Staff.Person.OfficialEmail;
                }
                else return string.Empty;

            }
        }

        public string GetStaffEmailAddress(Guid staffId)
        {
            using (var dbContext = new SCMSEntities())
            {
                Staff staff = dbContext.Staffs.FirstOrDefault(p => p.Id == staffId);
                if (staff != null)
                    return staff.Person.OfficialEmail;
                else return string.Empty;

            }
        }

        public bool SaveNotification(Notification notification)
        {
            using (var context = new SCMSEntities())
            {
                context.Notifications.Add(notification);
                return context.SaveChanges() > 0;
            }
        }

        public bool UpdateNotification(Notification notification)
        {
            using (var context = new SCMSEntities())
            {
                context.Notifications.Attach(notification);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(notification, System.Data.EntityState.Modified);
                return context.SaveChanges() > 0;
            }
        }

        public void SendToAppropriateApprover(string documentCode, string actionType, Guid documentId)
        {
            Approver approver = null;
            List<ProjectBLCount> projects;
            switch (documentCode)
            { 
                case NotificationHelper.orCode:
                    approver = GetOrderRequestApprover(documentId, actionType);
                    SendOrderRequestNotification(documentId, actionType, approver);
                    break;
                case NotificationHelper.poCode:
                    approver = GetPurchaseOrderApprover(documentId, actionType, out projects);
                    SendPurchaseOrderNotification(documentId, actionType, approver, projects);
                    break;
                case NotificationHelper.rfpCode:
                    approver = GetRFPApprover(documentId, actionType, out projects);
                    SendRequestForPaymentNotification(documentId, actionType, approver, projects);
                    break;
                case NotificationHelper.ppCode:
                    approver = GetPPApprover(documentId, actionType);
                    SendProcurementPlanNotification(documentId, approver);
                    break;
                case NotificationHelper.grnCode:
                    //approver = GetGRNApprover(documentId, actionType);
                    SendGoodsReceivedNoteNotification(documentId);
                    break;
                case NotificationHelper.wrnCode:
                    //approver = GetWRNApprover(documentId, actionType);
                    SendWarehouseReleaseNotification(documentId);
                    break;
                case NotificationHelper.ccCode:
                    SendCompletionCertificateNotification(documentId);
                    break;
            }
        }

        #region .Document Approval Senders.

        private void SendOrderRequestNotification(Guid orId, string actionType, Approver approver)
        {
            if (approver != null)
            {
                using (var context = new SCMSEntities())
                {
                    var or = context.OrderRequests.FirstOrDefault(o => o.Id == orId);
                    string name = approver.SystemUser.IsAvailable ? approver.SystemUser.Staff.Person.FirstName : approver.SystemUser1.Staff.Person.FirstName;
                    string email = approver.SystemUser.IsAvailable ? approver.SystemUser.Staff.Person.OfficialEmail : approver.SystemUser1.Staff.Person.OfficialEmail;
                    string msgBody = "";
                    if (actionType == NotificationHelper.approvalCode)
                        msgBody = string.Format(NotificationHelper.orMsgBody, name);
                    else if (actionType == NotificationHelper.reviewCode)
                        msgBody = string.Format(NotificationHelper.orMsgReviewBody, name, or.RefNumber);
                    else
                        msgBody = string.Format(NotificationHelper.orAuthMsgBody, name, or.RefNumber);
                    //Send notification
                    this.SendNotification(email, msgBody, NotificationHelper.orsubject);
                    //Save notification
                    Notification notification = new Notification();
                    notification.Id = Guid.NewGuid();
                    notification.ApproverId = approver.Id;
                    notification.SentToDelegate = !approver.SystemUser.IsAvailable;
                    notification.SendDate = DateTime.Now;
                    notification.OrderRequestId = orId;
                    this.SaveNotification(notification);
                    //Notify Project Manager
                    name = or.ProjectDonor.Staff.Person.FirstName;
                    email = or.ProjectDonor.Staff.Person.OfficialEmail;
                    if (actionType == NotificationHelper.approvalCode)
                        msgBody = string.Format(NotificationHelper.orPMNotifyMsgBody, name, or.RefNumber);
                    else if (actionType == NotificationHelper.reviewCode)
                        msgBody = string.Format(NotificationHelper.orPMNotifyApprovedMsgBody, name, or.RefNumber);
                    else
                        msgBody = string.Format(NotificationHelper.orPMNotifyReviewedMsgBody, name, or.RefNumber);
                    this.SendNotification(email, msgBody, NotificationHelper.orsubject);
                }
            }
        }

        private void SendPurchaseOrderNotification(Guid poId, string actionType, Approver approver, List<ProjectBLCount> projects)
        {
            if (approver != null)
            {
                using (var context = new SCMSEntities())
                {
                    var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == poId);
                    //Choose responsible staff or delegate
                    string name = approver.SystemUser.IsAvailable ? approver.SystemUser.Staff.Person.FirstName : approver.SystemUser1.Staff.Person.FirstName;
                    string email = approver.SystemUser.IsAvailable ? approver.SystemUser.Staff.Person.OfficialEmail : approver.SystemUser1.Staff.Person.OfficialEmail;
                    string msgBody = "";
                    msgBody = string.Format(NotificationHelper.poMsgBody, name, po.RefNumber);
                    //Send notification
                    this.SendNotification(email, msgBody, NotificationHelper.posubject);
                    //Save notification
                    Notification notification = new Notification();
                    notification.Id = Guid.NewGuid();
                    notification.ApproverId = approver.Id;
                    notification.SentToDelegate = !approver.SystemUser.IsAvailable;
                    notification.SendDate = DateTime.Now;
                    notification.PurchaseOrderId = poId;
                    this.SaveNotification(notification);
                    //Notify the Project Manager(s)
                    foreach (var project in projects)
                    {
                        var proj = context.ProjectDonors.FirstOrDefault(p => p.Id == project.ProjectDonorId);
                        name = proj.Staff.Person.FirstName;
                        email = proj.Staff.Person.OfficialEmail;
                        msgBody = string.Format(NotificationHelper.poPMNotifyMsgBody, name, po.RefNumber, proj.ProjectNumber);
                        this.SendNotification(email, msgBody, NotificationHelper.posubject);
                    }
                }
            }
        }

        private void SendRequestForPaymentNotification(Guid rfpId, string actionType, Approver approver, List<ProjectBLCount> projects)
        {
            if (approver != null)
            {
                using (var context = new SCMSEntities())
                {
                    var rfp = context.PaymentRequests.FirstOrDefault(r => r.Id == rfpId);
                    //Choose responsible staff or delegate
                    string name = approver.SystemUser.IsAvailable ? approver.SystemUser.Staff.Person.FirstName : approver.SystemUser1.Staff.Person.FirstName;
                    string email = approver.SystemUser.IsAvailable ? approver.SystemUser.Staff.Person.OfficialEmail : approver.SystemUser1.Staff.Person.OfficialEmail;
                    string msgBody = "";
                    if (actionType == NotificationHelper.reviewCode)
                        msgBody = string.Format(NotificationHelper.rfpMsgBody, name, rfp.RefNumber);
                    else if (actionType == NotificationHelper.authorizationCode)
                        msgBody = string.Format(NotificationHelper.rfpAuthMsgBody, name, rfp.RefNumber);
                    else
                        msgBody = string.Format(NotificationHelper.rfpMsgBodyForPosting, name, rfp.RefNumber);
                    //Send notification
                    this.SendNotification(email, msgBody, NotificationHelper.rfpsubject);
                    //Save notification
                    Notification notification = new Notification();
                    notification.Id = Guid.NewGuid();
                    notification.ApproverId = approver.Id;
                    notification.SentToDelegate = !approver.SystemUser.IsAvailable;
                    notification.SendDate = DateTime.Now;
                    notification.PaymentRequestId = rfpId;
                    this.SaveNotification(notification);
                    //Notify the Project Manager(s)
                    foreach (var project in projects)
                    {
                        var proj = context.ProjectDonors.FirstOrDefault(p => p.Id == project.ProjectDonorId);
                        name = proj.Staff.Person.FirstName;
                        email = proj.Staff.Person.OfficialEmail;
                        if (actionType == NotificationHelper.reviewCode)
                            msgBody = string.Format(NotificationHelper.rfpPMNotifyMsgBody, name, rfp.RefNumber);
                        else if (actionType == NotificationHelper.authorizationCode)
                            msgBody = string.Format(NotificationHelper.rfpPMNotifyApprovedMsgBody, name, rfp.RefNumber);
                        this.SendNotification(email, msgBody, NotificationHelper.rfpsubject);
                    }
                }
            }
        }

        private void SendWarehouseReleaseNotification(Guid wrnId)
        {
            using (var context = new SCMSEntities())
            {
                var wrn = context.WarehouseReleases.FirstOrDefault(r => r.Id == wrnId);
                var ap = context.Staffs.FirstOrDefault(s => s.Id == (Guid)wrn.ApprovedBy);
                var name = ap.Person.FirstName;
                var email = ap.Person.OfficialEmail;
                string msgBody = "";
                msgBody = string.Format(NotificationHelper.wrnMsgBody, name, wrn.RefNumber);
                //Send notification
                this.SendNotification(email, msgBody, NotificationHelper.wrnsubject);
            }
        }

        private void SendProcurementPlanNotification(Guid ppId, Approver approver)
        {
            if (approver == null)
                return;
            using (var context = new SCMSEntities())
            {
                var pp = context.ProcurementPlans.FirstOrDefault(r => r.Id == ppId);
                var staff = context.SystemUsers.FirstOrDefault(s => s.Id == approver.UserId).Staff;
                var name = staff.Person.FirstName;
                var email = staff.Person.OfficialEmail;
                string msgBody = "";
                if (approver.ActionType == NotificationHelper.approvalCode)
                    msgBody = string.Format(NotificationHelper.ppMsgBody, name, pp.RefNumber);
                else if (approver.ActionType == NotificationHelper.reviewCode)
                    msgBody = string.Format(NotificationHelper.ppReviewMsgBody, name, pp.RefNumber);
                else if (approver.ActionType == NotificationHelper.approvalIICode)
                    msgBody = string.Format(NotificationHelper.ppApproval2MsgBody, name, pp.RefNumber);
                else if (approver.ActionType == NotificationHelper.authorizationCode)
                    msgBody = string.Format(NotificationHelper.ppAuthMsgBody, name, pp.RefNumber);
                //Send notification
                this.SendNotification(email, msgBody, NotificationHelper.ppsubject);
                //Save notification
                Notification notification = new Notification();
                notification.Id = Guid.NewGuid();
                notification.ApproverId = approver.Id;
                notification.SentToDelegate = !approver.SystemUser.IsAvailable;
                notification.SendDate = DateTime.Now;
                notification.ProcurementPlanId = ppId;
                this.SaveNotification(notification);
                //Notifiy Project Manager
                var proj = pp.ProjectDonor;
                name = proj.Staff.Person.FirstName;
                email = proj.Staff.Person.OfficialEmail;
                if (approver.ActionType == NotificationHelper.approvalCode)
                    msgBody = string.Format(NotificationHelper.ppPMNotifyMsgBody, name, pp.RefNumber);
                else if (approver.ActionType == NotificationHelper.reviewCode)
                    msgBody = string.Format(NotificationHelper.ppPMNotifyApproved1MsgBody, name, pp.RefNumber);
                else if (approver.ActionType == NotificationHelper.approvalIICode)
                    msgBody = string.Format(NotificationHelper.ppPMNotifyReviewedMsgBody, name, pp.RefNumber);
                else if (approver.ActionType == NotificationHelper.authorizationCode)
                    msgBody = string.Format(NotificationHelper.ppPMNotifyApproved2MsgBody, name, pp.RefNumber);
                this.SendNotification(email, msgBody, NotificationHelper.rfpsubject);
            }
        }

        private void SendGoodsReceivedNoteNotification(Guid grnId)
        {
            using (var context = new SCMSEntities())
            {
                var grn = context.GoodsReceivedNotes.FirstOrDefault(r => r.Id == grnId);
                var verifier = context.Staffs.FirstOrDefault(s => s.Id == (Guid)grn.ReceptionApprovedBy);
                var name = verifier.Person.FirstName;
                var email = verifier.Person.OfficialEmail;
                string msgBody = "";
                msgBody = string.Format(NotificationHelper.grnMsgBody, name, grn.RefNumber);
                //Send notification
                this.SendNotification(email, msgBody, NotificationHelper.grnsubject);
            }
        }

        private void SendCompletionCertificateNotification(Guid ccId)
        {
            using (var context = new SCMSEntities())
            {
                var cc = context.CompletionCertificates.FirstOrDefault(r => r.Id == ccId);
                var ap = context.Staffs.FirstOrDefault(s => s.Id == cc.ConfirmedBy);
                var name = ap.Person.FirstName;
                var email = ap.Person.OfficialEmail;
                string msgBody = "";
                msgBody = string.Format(NotificationHelper.ccMsgBody, name, cc.RefNumber);
                //Send notification
                this.SendNotification(email, msgBody, NotificationHelper.ccsubject);
            }
        }

        #endregion

        #region .Document Approver Getters.

        /// <summary>
        /// Get Order Request Approver/Reviewer/Authorizer
        /// </summary>
        /// <param name="orId"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private Approver GetOrderRequestApprover(Guid orId, string actionType, Guid? userId = null)
        {
            using (var context = new SCMSEntities())
            {
                var or = context.OrderRequests.FirstOrDefault(o => o.Id == orId);
                decimal orAmount = (decimal)exchangeRateService.GetForeignCurrencyValue(or.CountryProgramme.Currency, or.Currency, or.TotalAmount, (Guid)or.CountryProgrammeId);
                var approvers = context.Approvers.Where(a => (a.ProjectDonorId != null && a.ProjectDonorId == or.ProjectDonorId) && a.ActivityCode == NotificationHelper.orCode
                    && a.ActionType == actionType && a.FinanceLimit.Limit >= orAmount).ToList();
                if (approvers != null && approvers.Count > 0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        }
                    }
                }
                approvers = context.Approvers.Where(a => (a.ProjectDonorId != null && a.ProjectDonorId == or.ProjectDonorId) && a.ActivityCode == NotificationHelper.orCode
                    && a.ActionType == actionType && a.FinanceLimit.Limit == 0).ToList();
                if (approvers != null && approvers.Count > 0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        }
                    }
                }
                approvers = context.Approvers.Where(a => a.ProjectDonorId == null && a.CountryProgrammeId == (Guid)or.CountryProgrammeId && a.ActivityCode == NotificationHelper.orCode
                    && a.ActionType == actionType && a.FinanceLimit.Limit >= orAmount).ToList();
                if (approvers != null && approvers.Count > 0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        }
                    }
                }
                approvers = context.Approvers.Where(a => a.ProjectDonorId == null && a.CountryProgrammeId == (Guid)or.CountryProgrammeId && a.ActivityCode == NotificationHelper.orCode
                    && a.ActionType == actionType && a.FinanceLimit.Limit == 0).ToList();
                if (approvers != null && approvers.Count>0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        }
                        return null;
                    }
                }
                return approvers.Count > 0 ? approvers[0] : null;
            }
        }

        /// <summary>
        /// Get Purchase Order Reviewer/Authorizer
        /// </summary>
        /// <param name="poId"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private Approver GetPurchaseOrderApprover(Guid poId, string actionType, out List<ProjectBLCount> projects, Guid? userId = null)
        {
            List<Approver> approvers;
            using (var context = new SCMSEntities())
            {
                var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == poId);
                var cp = context.CountryProgrammes.FirstOrDefault(c => c.Id == (Guid)po.CountryProgrammeId);
                decimal poAmount = (decimal)exchangeRateService.GetForeignCurrencyValue(cp.Currency, po.Currency, po.TotalAmount, cp.Id);
                //Get ProjectBLCount list order by highest budget line count to least for this PO
                projects = po.PurchaseOrderItems.GroupBy(bl => bl.ProjectBudget.BudgetCategory.ProjectDonor).
                    Select(projList => new ProjectBLCount
                    {
                        ProjectDonorId = projList.FirstOrDefault().ProjectBudget.BudgetCategory.ProjectDonor.Id,
                        BudgetLineCount = projList.Count()
                    }).OrderByDescending(p => p.BudgetLineCount).ToList<ProjectBLCount>();
                //Loop through the project approvers from the project with the highest number of BL's and to the least
                // and get the first approver with the appropriate finance limit
                foreach (var pdBLCount in projects)
                {
                    approvers = context.Approvers.Where(a => a.CountryProgrammeId == cp.Id && a.ProjectDonorId == pdBLCount.ProjectDonorId && a.ActivityCode == NotificationHelper.poCode
                        && a.ActionType == actionType && a.FinanceLimit.Limit >= poAmount).ToList();
                    if (approvers != null && approvers.Count > 0)
                    {
                        if (!userId.HasValue)
                        {
                            var person = approvers[0].SystemUser.Staff.Person;
                            person = approvers[0].SystemUser1.Staff.Person;
                            return approvers[0];
                        }
                        else
                        {
                            foreach (var app in approvers)
                            {
                                if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                    return app;
                            }
                        }
                    }
                }

                //Loop through the project approvers from the project with the highest number of BL's and to the least
                // and get the first approver with unlimted finance rights
                foreach (var pdBLCount in projects)
                {
                    approvers = context.Approvers.Where(a => a.CountryProgrammeId == cp.Id && a.ProjectDonorId == pdBLCount.ProjectDonorId && a.ActivityCode == NotificationHelper.poCode
                        && a.ActionType == actionType && a.FinanceLimit.Limit == 0).ToList();
                    if (approvers != null && approvers.Count > 0)
                    {
                        if (!userId.HasValue)
                        {
                            var person = approvers[0].SystemUser.Staff.Person;
                            person = approvers[0].SystemUser1.Staff.Person;
                            return approvers[0];
                        }
                        else
                        {
                            foreach (var app in approvers)
                            {
                                if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                    return app;
                            }
                        }
                    }
                }

                approvers = context.Approvers.Where(a => a.ProjectDonorId == null && a.CountryProgrammeId == cp.Id && a.ActivityCode == NotificationHelper.poCode
                    && a.ActionType == actionType && a.FinanceLimit.Limit >= poAmount).ToList();
                if (approvers != null && approvers.Count > 0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        }
                    }
                }
                approvers = context.Approvers.Where(a => a.ProjectDonorId == null && a.CountryProgrammeId == cp.Id && a.ActivityCode == NotificationHelper.poCode
                    && a.ActionType == actionType && a.FinanceLimit.Limit == 0).ToList();
                if (approvers != null && approvers.Count > 0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        }
                        return null;
                    }
                }
                return approvers.Count > 0 ? approvers[0] : null;
            }
        }

        /// <summary>
        /// Get RFP Reviewer/Authorizer
        /// </summary>
        /// <param name="rfpId"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private Approver GetRFPApprover(Guid rfpId, string actionType, out List<ProjectBLCount> projects, Guid? userId = null)
        {
            List<Approver> approvers;
            using (var context = new SCMSEntities())
            {
                var rfp = context.PaymentRequests.FirstOrDefault(r => r.Id == rfpId);
                var cp = context.CountryProgrammes.FirstOrDefault(c => c.Id == (Guid)rfp.CountryProgrammeId);
                decimal rfpAmount = (decimal)exchangeRateService.GetForeignCurrencyValue(cp.Currency, rfp.Currency, rfp.TotalAmount, cp.Id);
                //Get ProjectBLCount list order by highest budget line count to least for this RFP
                projects = rfp.PaymentRequestBudgetLines.GroupBy(bl => bl.ProjectBudget.BudgetCategory.ProjectDonor).
                    Select(projList => new ProjectBLCount
                    {
                        ProjectDonorId = projList.FirstOrDefault().ProjectBudget.BudgetCategory.ProjectDonor.Id,
                        BudgetLineCount = projList.Count()
                    }).OrderByDescending(p => p.BudgetLineCount).ToList<ProjectBLCount>();

                //Loop through the project approvers from the project with the highest number of BL's and to the least
                // and get the first approver with the appropriate finance limit
                foreach (var pdBLCount in projects)
                {
                    approvers = context.Approvers.Where(a => a.CountryProgrammeId == cp.Id && a.ProjectDonorId == pdBLCount.ProjectDonorId && a.ActivityCode == NotificationHelper.rfpCode
                        && a.ActionType == actionType && a.FinanceLimit.Limit >= rfpAmount).ToList();
                    if (approvers != null && approvers.Count > 0)
                    {
                        if (!userId.HasValue)
                        {
                            var person = approvers[0].SystemUser.Staff.Person;
                            person = approvers[0].SystemUser1.Staff.Person;
                            return approvers[0];
                        }
                        else
                        {
                            foreach (var app in approvers)
                            {
                                if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                    return app;
                            }
                        }
                    }
                }

                //Loop through the project approvers from the project with the highest number of BL's and to the least
                // and get the first approver with unlimted finance rights
                foreach (var pdBLCount in projects)
                {
                    approvers = context.Approvers.Where(a => a.CountryProgrammeId == cp.Id && a.ProjectDonorId == pdBLCount.ProjectDonorId && a.ActivityCode == NotificationHelper.rfpCode
                        && a.ActionType == actionType && a.FinanceLimit.Limit == 0).ToList();
                    if (approvers != null && approvers.Count > 0)
                    {
                        if (!userId.HasValue)
                        {
                            var person = approvers[0].SystemUser.Staff.Person;
                            person = approvers[0].SystemUser1.Staff.Person;
                            return approvers[0];
                        }
                        else
                        {
                            foreach (var app in approvers)
                            {
                                if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                    return app;
                            }
                        }
                    }
                }

                approvers = context.Approvers.Where(a => a.ProjectDonorId == null && a.CountryProgrammeId == cp.Id && a.ActivityCode == NotificationHelper.rfpCode
                    && a.ActionType == actionType && a.FinanceLimit.Limit >= rfpAmount).ToList();
                if (approvers != null && approvers.Count > 0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        }
                    }
                }
                approvers = context.Approvers.Where(a => a.ProjectDonorId == null && a.CountryProgrammeId == cp.Id && a.ActivityCode == NotificationHelper.rfpCode
                    && a.ActionType == actionType && a.FinanceLimit.Limit == 0).ToList();
                if (approvers != null && approvers.Count > 0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        } 
                        return null;
                    }
                }
                return approvers.Count > 0 ? approvers[0] : null;
            }
        }

        /// <summary>
        /// Get WRN Reviewer/Authorizer
        /// </summary>
        /// <param name="wrnId"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private Approver GetWRNApprover(Guid wrnId, string actionType, Guid? userId = null)
        {
            List<Approver> approvers;
            using (var context = new SCMSEntities())
            {
                var wrn = context.WarehouseReleases.FirstOrDefault(r => r.Id == wrnId);

                approvers = context.Approvers.Where(a => a.ProjectDonorId == null && a.CountryProgrammeId == wrn.CountryProgrammeId && a.ActivityCode == NotificationHelper.wrnCode
                    && a.ActionType == actionType).ToList();
                if (approvers != null && approvers.Count > 0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        } 
                        return null;
                    }
                }
                return approvers.Count > 0 ? approvers[0] : null;
            }
        }

        /// <summary>
        /// Get CC Approver
        /// </summary>
        /// <param name="ccId"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private Approver GetCCApprover(Guid ccId, string actionType, Guid? userId = null)
        {
            List<Approver> approvers;
            using (var context = new SCMSEntities())
            {
                var cc = context.CompletionCertificates.FirstOrDefault(r => r.Id == ccId);

                approvers = context.Approvers.Where(a => a.ProjectDonorId == null && a.CountryProgrammeId == cc.CountryProgrammeId && a.ActivityCode == NotificationHelper.ccCode
                    && a.ActionType == actionType).ToList();
                if (approvers != null && approvers.Count > 0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        }
                        return null;
                    }
                }
                return approvers.Count > 0 ? approvers[0] : null;
            }
        }

        /// <summary>
        /// Get PP Reviewer/Authorizer
        /// </summary>
        /// <param name="ppId"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private Approver GetPPApprover(Guid ppId, string actionType, Guid? userId = null)
        {
            List<Approver> approvers;
            using (var context = new SCMSEntities())
            {
                var pp = context.ProcurementPlans.FirstOrDefault(p => p.Id == ppId);

                approvers = context.Approvers.Where(a => a.ProjectDonorId == pp.ProjectDonorId && a.CountryProgrammeId == pp.ProjectDonor.Project.CountryProgrammeId && a.ActivityCode == NotificationHelper.ppCode
                    && a.ActionType == actionType).ToList();
                if (approvers != null && approvers.Count > 0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        }
                        return null;
                    }
                }
                return approvers.Count > 0 ? approvers[0] : null;
            }
        }

        /// <summary>
        /// Get GRN Reviewer/Authorizer
        /// </summary>
        /// <param name="grnId"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private Approver GetGRNApprover(Guid grnId, string actionType, Guid? userId = null)
        {
            List<Approver> approvers;
            using (var context = new SCMSEntities())
            {
                var wrn = context.GoodsReceivedNotes.FirstOrDefault(r => r.Id == grnId);

                approvers = context.Approvers.Where(a => a.ProjectDonorId == null && a.CountryProgrammeId == wrn.CountryProgrammeId && a.ActivityCode == NotificationHelper.grnCode
                    && a.ActionType == actionType).ToList();
                if (approvers != null && approvers.Count > 0)
                {
                    if (!userId.HasValue)
                    {
                        var person = approvers[0].SystemUser.Staff.Person;
                        person = approvers[0].SystemUser1.Staff.Person;
                        return approvers[0];
                    }
                    else
                    {
                        foreach (var app in approvers)
                        {
                            if (app.UserId == userId.Value || app.AssistantId == userId.Value)
                                return app;
                        } 
                        return null;
                    }
                }
                return approvers.Count > 0 ? approvers[0] : null;
            }
        }

        #endregion

        /// <summary>
        /// Send notification to Project Managers that document has been authorized
        /// </summary>
        /// <param name="documentCode"></param>
        /// <param name="documentId"></param>
        public void SendAuthorizedMsgToPMs(string documentCode, Guid documentId)
        {
            switch (documentCode)
            {
                case NotificationHelper.orCode:
                    SendORAuthorizedMsgToPM(documentId);
                    break;
                case NotificationHelper.poCode:
                    SendPOApprovedMsgToPM(documentId);
                    break;
                case NotificationHelper.rfpCode:
                    SendRFPAuthorizedMsgToPM(documentId);
                    break;
            }
        }

        #region .Send Authorized Messages to PMs.

        private void SendORAuthorizedMsgToPM(Guid orId)
        {
            using (var context = new SCMSEntities())
            {
                var or = context.OrderRequests.FirstOrDefault(o => o.Id == orId);
                var name = or.ProjectDonor.Staff.Person.FirstName;
                var email = or.ProjectDonor.Staff.Person.OfficialEmail;
                var msg = string.Format(NotificationHelper.orPMNotifyAuthorizedMsgBody, name, or.RefNumber, or.ProjectDonor.ProjectNumber);
                this.SendNotification(email, msg, NotificationHelper.orsubject);
            }
        }

        private void SendPOApprovedMsgToPM(Guid poId)
        {
            using (var context = new SCMSEntities())
            {
                var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == poId);
                //Get Projects affected by PO
                var projects = po.PurchaseOrderItems.GroupBy(bl => bl.ProjectBudget.BudgetCategory.ProjectDonor).
                    Select(projList => new ProjectBLCount
                    {
                        ProjectDonorId = projList.FirstOrDefault().ProjectBudget.BudgetCategory.ProjectDonor.Id,
                        BudgetLineCount = projList.Count()
                    }).OrderByDescending(p => p.BudgetLineCount).ToList<ProjectBLCount>();
                foreach (var project in projects)
                {
                    var pd = context.ProjectDonors.FirstOrDefault(p => p.Id == project.ProjectDonorId);
                    var name = pd.Staff.Person.FirstName;
                    var email = pd.Staff.Person.OfficialEmail;
                    var msg = string.Format(NotificationHelper.poPMNotifyApprovedMsgBody, name, po.RefNumber, pd.ProjectNumber);
                    this.SendNotification(email, msg, NotificationHelper.posubject);
                }
            }
        }

        private void SendRFPAuthorizedMsgToPM(Guid rfpId)
        {
            using (var context = new SCMSEntities())
            {
                var rfp = context.PaymentRequests.FirstOrDefault(r => r.Id == rfpId);
                //Get Projects affected by PO
                var projects = rfp.PaymentRequestBudgetLines.GroupBy(bl => bl.ProjectBudget.BudgetCategory.ProjectDonor).
                    Select(projList => new ProjectBLCount
                    {
                        ProjectDonorId = projList.FirstOrDefault().ProjectBudget.BudgetCategory.ProjectDonor.Id,
                        BudgetLineCount = projList.Count()
                    }).OrderByDescending(p => p.BudgetLineCount).ToList<ProjectBLCount>();
                foreach (var project in projects)
                {
                    var pd = context.ProjectDonors.FirstOrDefault(p => p.Id == project.ProjectDonorId);
                    var name = pd.Staff.Person.FirstName;
                    var email = pd.Staff.Person.OfficialEmail;
                    var msg = string.Format(NotificationHelper.rfpPMNotifyAuthorizedMsgBody, name, rfp.RefNumber, pd.ProjectNumber);
                    this.SendNotification(email, msg, NotificationHelper.rfpsubject);
                }
            }
        }

        #endregion

        /// <summary>
        /// Send notification to Project Managers that document has been rejected
        /// </summary>
        /// <param name="documentCode"></param>
        /// <param name="documentId"></param>
        /// <param name="msg"></param>
        public void SendRejectedMsgToPMs(string documentCode, Guid documentId, string msg)
        {
            switch (documentCode)
            {
                case NotificationHelper.orCode:
                    SendORRejectedMsgToPM(documentId, msg);
                    break;
                case NotificationHelper.poCode:
                    SendPORejectedMsgToPM(documentId, msg);
                    break;
                case NotificationHelper.rfpCode:
                    SendRFPRejectedMsgToPM(documentId, msg);
                    break;
            }
        }

        #region .Send Rejection Messages to PMs.

        private void SendORRejectedMsgToPM(Guid orId, string rejectMsg)
        {
            using (var context = new SCMSEntities())
            {
                var or = context.OrderRequests.FirstOrDefault(o => o.Id == orId);
                var name = or.ProjectDonor.Staff.Person.FirstName;
                var email = or.ProjectDonor.Staff.Person.OfficialEmail;
                var msg = string.Format(NotificationHelper.orRejectedMsgBody, name, or.RefNumber, rejectMsg);
                this.SendNotification(email, msg, NotificationHelper.orsubject);
            }
        }

        private void SendPORejectedMsgToPM(Guid poId, string rejectMsg)
        {
            using (var context = new SCMSEntities())
            {
                var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == poId);
                //Get Projects affected by PO
                var projects = po.PurchaseOrderItems.GroupBy(bl => bl.ProjectBudget.BudgetCategory.ProjectDonor).
                    Select(projList => new ProjectBLCount
                    {
                        ProjectDonorId = projList.FirstOrDefault().ProjectBudget.BudgetCategory.ProjectDonor.Id,
                        BudgetLineCount = projList.Count()
                    }).OrderByDescending(p => p.BudgetLineCount).ToList<ProjectBLCount>();
                foreach (var project in projects)
                {
                    var pd = context.ProjectDonors.FirstOrDefault(p => p.Id == project.ProjectDonorId);
                    var name = pd.Staff.Person.FirstName;
                    var email = pd.Staff.Person.OfficialEmail;
                    var msg = string.Format(NotificationHelper.poRejectedMsgBody, name, po.RefNumber, rejectMsg);
                    this.SendNotification(email, msg, NotificationHelper.posubject);
                }
            }
        }

        private void SendRFPRejectedMsgToPM(Guid rfpId, string rejectMsg)
        {
            using (var context = new SCMSEntities())
            {
                var rfp = context.PaymentRequests.FirstOrDefault(r => r.Id == rfpId);
                //Get Projects affected by PO
                var projects = rfp.PaymentRequestBudgetLines.GroupBy(bl => bl.ProjectBudget.BudgetCategory.ProjectDonor).
                    Select(projList => new ProjectBLCount
                    {
                        ProjectDonorId = projList.FirstOrDefault().ProjectBudget.BudgetCategory.ProjectDonor.Id,
                        BudgetLineCount = projList.Count()
                    }).OrderByDescending(p => p.BudgetLineCount).ToList<ProjectBLCount>();
                foreach (var project in projects)
                {
                    var pd = context.ProjectDonors.FirstOrDefault(p => p.Id == project.ProjectDonorId);
                    var name = pd.Staff.Person.FirstName;
                    var email = pd.Staff.Person.OfficialEmail;
                    var msg = string.Format(NotificationHelper.rfpRejectedMsgBody, name, rfp.RefNumber, rejectMsg);
                    this.SendNotification(email, msg, NotificationHelper.rfpsubject);
                }
            }
        }

        #endregion

        /// <summary>
        /// Send notification to Project Managers that a document affecting their budgets has been posted
        /// </summary>
        /// <param name="documentCode"></param>
        /// <param name="documentId"></param>
        public void SendFundsPostedMsgToPMs(string documentCode, Guid documentId)
        {
            switch (documentCode)
            {
                case NotificationHelper.rfpCode:
                    SendRFPFundsPostedMsgToPM(documentId);
                    break;
            }
        }

        #region .Send Funds Posted Messages to PMs.

        private void SendRFPFundsPostedMsgToPM(Guid rfpId)
        {
            using (var context = new SCMSEntities())
            {
                var rfp = context.PaymentRequests.FirstOrDefault(r => r.Id == rfpId);
                //Get Projects affected by PO
                var projects = rfp.PaymentRequestBudgetLines.GroupBy(bl => bl.ProjectBudget.BudgetCategory.ProjectDonor).
                    Select(projList => new ProjectBLCount
                    {
                        ProjectDonorId = projList.FirstOrDefault().ProjectBudget.BudgetCategory.ProjectDonor.Id,
                        BudgetLineCount = projList.Count()
                    }).OrderByDescending(p => p.BudgetLineCount).ToList<ProjectBLCount>();
                foreach (var project in projects)
                {
                    var pd = context.ProjectDonors.FirstOrDefault(p => p.Id == project.ProjectDonorId);
                    var name = pd.Staff.Person.FirstName;
                    var email = pd.Staff.Person.OfficialEmail;
                    var msg = string.Format(NotificationHelper.rfpPMNotifyFundsPostedMsgBody, name, rfp.RefNumber);
                    this.SendNotification(email, msg, NotificationHelper.rfpsubject);
                }
            }
        }

        #endregion


        public bool CanApprove(SystemUser currentUser, string activityCode, string actionType, Guid documentId)
        {
            List<ProjectBLCount> projects;
            using (var context = SCMSEntities.Define())
            {
                if (activityCode == NotificationHelper.orCode)
                {
                    var approver = GetOrderRequestApprover(documentId, actionType, currentUser.Id);
                    return approver != null;
                }
                if (activityCode == NotificationHelper.ppCode)
                {
                    var approver = GetPPApprover(documentId, actionType, currentUser.Id);
                    return approver != null;
                }
                if (activityCode == NotificationHelper.poCode)
                {
                    var approver = GetPurchaseOrderApprover(documentId, actionType, out projects, currentUser.Id);
                    return approver != null;
                }
                if (activityCode == NotificationHelper.rfpCode)
                {
                    var approver = GetRFPApprover(documentId, actionType, out projects, currentUser.Id);
                    return approver != null;
                }
                if (activityCode == NotificationHelper.grnCode)
                {
                    var approver = GetGRNApprover(documentId, actionType, currentUser.Id);
                    return approver != null;
                }
                if (activityCode == NotificationHelper.wrnCode)
                {
                    var approver = GetWRNApprover(documentId, actionType, currentUser.Id);
                    return approver != null;
                }
                if (activityCode == NotificationHelper.ccCode)
                {
                    var approver = GetCCApprover(documentId, actionType, currentUser.Id);
                    return approver != null;
                }
            }
            return false;
        }

        public void SendAuthorizedMsgToDocPreparers(string docCode, Guid documentId)
        {
            switch (docCode)
            {
                case NotificationHelper.orCode:
                    SendORAuthMsgToDocPreparers(documentId);
                    break;
                case NotificationHelper.poCode:
                    SendPOAuthMsgToDocPreparers(documentId);
                    break;
                case NotificationHelper.grnCode:
                    SendGRNAuthMsgToDocPreparers(documentId);
                    break;
            }
        }

        #region .Send Authorized Msgs to Doc Preparers.

        private void SendORAuthMsgToDocPreparers(Guid orId)
        {
            using (var context = new SCMSEntities())
            {
                var or = context.OrderRequests.FirstOrDefault(o => o.Id == orId);
                var docPreps = context.DocumentPreparers.Where(d => d.DocumentCode == NotificationHelper.poCode && d.ProjectDonorId == (Guid)or.ProjectDonorId).ToList();
                string msgBody = "";
                foreach (var prep in docPreps)
                {
                    msgBody = string.Format(NotificationHelper.taDocPrepNotifyMsgBody, prep.Staff.Person.FirstName, or.RefNumber);
                    this.SendNotification(prep.Staff.Person.OfficialEmail, msgBody, NotificationHelper.orsubject);
                }
            }
        }

        private void SendPOAuthMsgToDocPreparers(Guid poId)
        {
            using (var context = new SCMSEntities())
            {
                var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == poId);
                var docPreps = context.DocumentPreparers.Where(d => d.DocumentCode == NotificationHelper.grnCode && d.ProjectDonorId == (Guid)po.ProjectDonorId).ToList();
                string msgBody = "";
                foreach (var prep in docPreps)
                {
                    msgBody = string.Format(NotificationHelper.grnDocPrepNotifyMsgBody, prep.Staff.Person.FirstName, po.RefNumber);
                    this.SendNotification(prep.Staff.Person.OfficialEmail, msgBody, NotificationHelper.posubject);
                }
            }
        }

        private void SendGRNAuthMsgToDocPreparers(Guid grnId)
        {
            using (var context = new SCMSEntities())
            {
                var grn = context.GoodsReceivedNotes.FirstOrDefault(g => g.Id == grnId);
                var po = context.PurchaseOrders.FirstOrDefault(p => p.Id == grn.PurchaseOrderId);
                var docPreps = context.DocumentPreparers.Where(d => d.DocumentCode == NotificationHelper.rfpCode && d.ProjectDonorId == (Guid)po.ProjectDonorId).ToList();
                string msgBody = "";
                foreach (var prep in docPreps)
                {
                    msgBody = string.Format(NotificationHelper.rfpDocPrepNotifyMsgBody, prep.Staff.Person.FirstName, grn.RefNumber);
                    this.SendNotification(prep.Staff.Person.OfficialEmail, msgBody, NotificationHelper.grnsubject);
                }
            }
        }

        #endregion

    }
}
