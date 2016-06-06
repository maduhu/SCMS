using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.NotificationsManager
{
    public interface INotificationService
    {
        void SendNotification(string ToAddress, string msgBody, string subject, bool isHtml = false);
        string GetStaffEmailAddress(Guid staffId);
        string GetApproverEmailAddress(int Priority, string activityCode);
        string GetApproverEmailAddress(int Priority, string activityCode, string actionType);
        bool SaveNotification(Notification notification);
        bool UpdateNotification(Notification notification);
        /// <summary>
        /// Gets Appropriate Approver for document basing on finance limit and sends notification to that user
        /// </summary>
        /// <param name="documentCode">Type of document to consider, e.g. OR, PO...</param>
        /// <param name="actionType">Type of action to consider, e.g. Approval, Finance Review...</param>
        /// <param name="documentId">Id of document in question, e.g. OrderRequest.Id</param>
        /// <returns></returns>
        void SendToAppropriateApprover(string documentCode, string actionType, Guid documentId);
        /// <summary>
        /// Send notification to Project Managers that document has been authorized
        /// </summary>
        /// <param name="documentCode"></param>
        /// <param name="documentId"></param>
        void SendAuthorizedMsgToPMs(string documentCode, Guid documentId);
        /// <summary>
        /// Send notification to Project Managers that document has been rejected
        /// </summary>
        /// <param name="documentCode"></param>
        /// <param name="documentId"></param>
        /// <param name="msg"></param>
        void SendRejectedMsgToPMs(string documentCode, Guid documentId, string msg);
        /// <summary>
        /// Send notification to Project Managers that a document affecting their budgets has been posted
        /// </summary>
        /// <param name="documentCode"></param>
        /// <param name="documentId"></param>
        void SendFundsPostedMsgToPMs(string documentCode, Guid documentId);
        /// <summary>
        /// Check if current user has the rights to approve/review/authorize the document 
        /// </summary>
        /// <param name="currentUser">current user</param>
        /// <param name="activityCode">document to approve</param>
        /// <param name="actionType">type of approval</param>
        /// <returns></returns>
        bool CanApprove(SystemUser currentUser, string activityCode, string actionType, Guid documentId);
        /// <summary>
        /// Send notification to the person(s) in charge of preparing the next level doc
        /// </summary>
        /// <param name="docCode">type of doc authorized</param>
        /// <param name="documentId">id of doc authorized</param>
        void SendAuthorizedMsgToDocPreparers(string docCode, Guid documentId);
    }
}
