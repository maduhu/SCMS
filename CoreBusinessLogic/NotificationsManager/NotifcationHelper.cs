using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using SCMS.Resource;

namespace SCMS.CoreBusinessLogic.NotificationsManager
{
    public class NotificationHelper
    {
        #region .Activity Codes/Document Codes.

        public const string orCode = "OR";
        public const string poCode = "PO";
        public const string rfpCode = "RFP";
        public const string wrnCode = "WRN";
        public const string grnCode = "GRN";
        public const string wbCode = "WB";
        public const string ccCode = "CC";
        public const string paramsCode = "PARAMS";
        public const string ppCode = "PP";

        #endregion

        #region .Action Types.

        public const string approvalCode = "Approval";
        public const string authorizationCode = "Authorization";
        public const string reviewCode = "Finance Review";
        public const string postFundsCode = "Fund Posting";
        public const string verificationCode = "Verification";
        public const string approvalIICode = "Approval II";

        #endregion

        #region .Message Bodies.

        public const string ppMsgBody = "Dear {0} \nYou have a Procurement Plan (Ref No. {1}) waiting for your approval. \nPlease login to your system account and take appropriate action.\n \nThank You";
        
        public const string orMsgBody = "Dear {0}, \nYou have an Order Request waiting for your approval. \nPlease login to your system account and take appropriate action. \nThank You";
        public const string poMsgBody = "Dear {0}, \nYou have a Purchase Order waiting for your approval. \nPlease login to your system account and take appropriate action. \nThank You";
        public const string rfpMsgBody = "Dear {0}, \nYou have a Request For Payment waiting for your financial review. \nPlease login to your system account and take appropriate action. \nThank You";
        public const string wrnMsgBody = "Dear {0}, \nYou have a Warehouse Release Order waiting for your approval. \nPlease login to your system account and take appropriate action. \nThank You";
        public const string ccMsgBody = "Dear {0}, \nYou have a Completion Certificate waiting for your approval. \nPlease login to your system account and take appropriate action. \nThank You";
        public const string grnMsgBody = "Dear {0}, \nYou have an Goods Received Note waiting for your verification. \nPlease login to your system account and take appropriate action. \nThank You";
        
        public const string orMsgReviewBody = "Dear {0}, \nOrder Request Ref No. {1} has been approved and is waiting for your financial review. \nPlease log in into your system account for details. \nThank You";
        public const string ppReviewMsgBody = "Dear {0} \nProcurement Plan Ref No. {1} has been approved and is waiting for your financial review. \nPlease login to your system account and take appropriate action.\n \nThank You";
        
        public const string orAuthMsgBody = "Dear {0}, \nOrder Request Ref No. {1} has been approved and financially reviewed and is waiting for your authorization. \nPlease login to your system account and take appropriate action. \nThank You";
        public const string ppApproval2MsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been financially reviewed and is waiting for your approval. \nPlease login to your system account and take appropriate action. \nThank You";
        public const string ppAuthMsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been approved and is waiting for your authorization. \nPlease login to your system account and take appropriate action. \nThank You";
        public const string rfpAuthMsgBody = "Dear {0}, \nRequest For Payment Ref No. {1} has been approved and is waiting for your authorization. \nPlease login to your system account and take appropriate action. \nThank You";
        public const string wrnAuthMsgBody = "Dear {0}, \nWarehouse Release Note Ref No. {1} has been approved and is waiting for your authorization. \nPlease login to your system account and take appropriate action. \nThank You";
        public const string grnRegAssetsMsgBody = "Dear {0}, \nGoods Received Note Ref No. {1} has been approved. \nPlease login to your system account and proceed with Asset Registration. \nThank You";
        
        public const string orApprovedMsgBody = "Dear {0}, \nOrder Request Ref No. {1} has been approved and forwarded for review. \nThank You";
        public const string ppApproved1MsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been approved and forwarded for review. \nThank You";
        public const string poApprovedMsgBody = "Dear {0}, \nPurchase Order Ref No. {1} has been approved. \nPlease proceed with the necessary steps. \nThank You";
        public const string rfpApprovedMsgBody = "Dear {0}, \nRequest For Payment Ref No. {1} has been approved and forwarded for authorization. \nThank You";
        public const string wrnApprovedMsgBody = "Dear {0}, \nWarehouse Release Order Ref No. {1} has been approved. Please proceed with the necessary steps. \nThank You";
        public const string ccApprovedMsgBody = "Dear {0}, \nCompletion Certificate Ref No. {1} has been approved. Please proceed with the necessary steps. \nThank You";
        public const string grnApprovedMsgBody = "Dear {0}, \nGoods Received Note Ref No. {1} has been verified. \nThank You";

        public const string orReviewedMsgBody = "Dear {0}, \nOrder Request Ref No. {1} has been financially reviewed and forwarded for authorization. \nThank You";
        public const string ppReviewedMsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been financially reviewed and forwarded for approval. \nThank You";
        public const string ppApproved2MsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been approved and forwarded for authorization. \nThank You";

        public const string orAuthorizedMsgBody = "Dear {0}, \nOrder Request Ref No. {1} has been authorized. \nPlease proceed with the necessary steps. \nThank You";
        public const string ppAuthorizedMsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been authorized. \nPlease proceed with the necessary steps. \nThank You";
        public const string rfpAuthorizedMsgBody = "Dear {0}, \nRequest For Payment Ref No. {1} has been authorized. \nPlease proceed with the necessary steps. \nThank You";
        public const string rfpMsgBodyForPosting = "Dear {0}, \nRequest For Payment Ref No. {1} has been authorized. \nPlease proceed and post the funds in the system. \nThank You";
        
        public const string rfpFundsPostedMsgBody = "Dear {0}, \nFunds for Request For Payment Ref No. {1} have been posted. \nThank you for using SCMS";
        
        public const string orRejectedMsgBody = "Dear {0}, \nOrder Request Ref No. {1} has been rejected. \nBelow are the remarks: \n\n{2}";
        public const string ppRejectedMsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been rejected. \nBelow are the remarks: \n\n{2}";
        public const string poRejectedMsgBody = "Dear {0}, \nPurchase Order Ref No. {1} has been rejected. \nBelow are the remarks: \n\n{2}";
        public const string rfpRejectedMsgBody = "Dear {0}, \nRequest For Payment Ref No. {1} has been rejected. \nBelow are the remarks: \n\n{2}";
        public const string wroRejectedMsgBody = "Dear {0}, \nWarehouse Release Order request Ref No. {1} has been rejected. \nBelow are the remarks: \n\n{2}";
        public const string ccRejectedMsgBody = "Dear {0}, \nCompletion Certificate Ref No. {1} has been rejected. \nBelow are the remarks: \n\n{2}";
        
        #endregion

        #region .Project Manager Notifications.

        public const string orPMNotifyMsgBody = "Dear {0}, \nOrder Request Ref No. {1} has been prepared and sent for approval. \nThank You";
        public const string ppPMNotifyMsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been prepared and sent for approval. \nThank You";
        public const string poPMNotifyMsgBody = "Dear {0}, \nPurchase Order Ref No. {1} has been prepared and sent for approval. \nThank You";
        public const string rfpPMNotifyMsgBody = "Dear {0}, \nRequest For Payment Ref No. {1} has been prepared and sent for financial review. \nThank You";
        
        public const string orPMNotifyApprovedMsgBody = "Dear {0}, \nOrder Request Ref No. {1} has been approved and sent for financial review. \nThank You";
        public const string ppPMNotifyApproved1MsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been approved and sent for financial review. \nThank You";
        public const string ppPMNotifyReviewedMsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been financially reviewed and sent for approval. \nThank You";
        public const string ppPMNotifyApproved2MsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been approved and sent for authorization. \nThank You";
        public const string orPMNotifyReviewedMsgBody = "Dear {0}, \nOrder Request Ref No. {1} has been financially reviewed and sent for authorization. \nThank You";
        public const string poPMNotifyApprovedMsgBody = "Dear {0}, \nPurchase Order Ref No. {1} has been approved.This action has resulted in commitment of funds in your project budget [{2}].  \nThank You";
        public const string rfpPMNotifyApprovedMsgBody = "Dear {0}, \nRequest For Payment Ref No. {1} has been financially reviewed and sent for authorization. \nThank You";
        
        public const string orPMNotifyAuthorizedMsgBody = "Dear {0}, \nOrder Request Ref No. {1} has been authorized. This action has resulted in commitment of funds in your project budget [{2}]. \nThank You";
        public const string ppPMNotifyAuthorizedMsgBody = "Dear {0}, \nProcurement Plan Ref No. {1} has been authorized. \nThank You";
        public const string rfpPMNotifyAuthorizedMsgBody = "Dear {0}, \nRequest For Payment Ref No. {1} has been authorized and is ready for posting. This action has resulted in commitment of funds in your project budget [{2}]. \nThank You";
        
        public const string rfpPMNotifyFundsPostedMsgBody = "Dear {0}, \nRequest For Payment Ref No. {1} has been posted. This action has resulted in posting of funds in your project budget [{2}]. \nThank You";
        
        #endregion

        #region .Document Preparer Notifications.

        public const string taDocPrepNotifyMsgBody = "Dear {0}, \n Order Request Ref No. {1} has been authorized. You can now go ahead and prepare a Tender Analysis for this OR. \nThank You";
        public const string poDocPrepNotifyMsgBody = "Dear {0}, \n Tender Analysis Ref No. {1} has been evaluated and authorized. You can now go ahead and prepare a Purchase Order from this TA. \nThank You";
        public const string grnDocPrepNotifyMsgBody = "Dear {0}, \n Purchase Order Ref No. {1} has been authorized. You can now go ahead and prepare a Goods Received Note for this PO. \nThank You";
        public const string rfpDocPrepNotifyMsgBody = "Dear {0}, \n Goods Received Note Ref No. {1} has been authorized. You can now go ahead and prepare a Request For Payment. \nThank You";
        
        #endregion

        #region .Notification Subjects.

        public const string orsubject = "Order Request Notification";
        public const string posubject = "Purchase Order Notification";
        public const string rfpsubject = "Request For Payment Notification";
        public const string wrnsubject = "Warehouse Release Order Notification";
        public const string grnsubject = "Goods Received Notification";
        public const string wbsubject = "Way Bill Notification";
        public const string ccsubject = "Completion Certificate Notification";
        public const string ppsubject = "Procurement Plan Notification";

        #endregion

        public const string LoginAttemptsExceededUserAccountLocked = "";

        public const string Logistics_Dev_Group = "logistics-dev@googlegroups.com";
        public const string Logistics_Dev_Group_Subject = "SCMS ERROR";
        
        //http://msdn.microsoft.com/en-us/library/ezwyzy7b.aspx

        #region .Lists.

        public static List<DocumentType> GetDocumentTypes()
        {
            List<DocumentType> docTypeList = new List<DocumentType>();
            DocumentType docType;

            docType = new DocumentType();
            docType.DocumentCode = NotificationHelper.ccCode;
            docType.DocumentName = Resources.CompletionCertificate_Index_Header;
            docTypeList.Add(docType);

            docType = new DocumentType();
            docType.DocumentCode = NotificationHelper.grnCode;
            docType.DocumentName = Resources.Global_String_GoodsReceivedNote;
            docTypeList.Add(docType);

            docType = new DocumentType();
            docType.DocumentCode = NotificationHelper.orCode;
            docType.DocumentName = Resources.Global_String_OrderRequest;
            docTypeList.Add(docType);

            docType = new DocumentType();
            docType.DocumentCode = NotificationHelper.poCode;
            docType.DocumentName = Resources.Global_String_PurchaseOrder;
            docTypeList.Add(docType);

            docType = new DocumentType();
            docType.DocumentCode = NotificationHelper.rfpCode;
            docType.DocumentName = Resources.Global_String_RequestForPayment;
            docTypeList.Add(docType);

            docType = new DocumentType();
            docType.DocumentCode = NotificationHelper.paramsCode;
            docType.DocumentName = Resources.Global_String_SystemParameters;
            docTypeList.Add(docType);

            docType = new DocumentType();
            docType.DocumentCode = NotificationHelper.wrnCode;
            docType.DocumentName = Resources.Global_String_WarehouseReleaseOrder;
            docTypeList.Add(docType);

            return docTypeList;
        }

        public static List<DocumentType> GetDocumentTypes4Project()
        {
            List<DocumentType> docTypeList = new List<DocumentType>();
            DocumentType docType = new DocumentType();

            docType = new DocumentType();
            docType.DocumentCode = NotificationHelper.orCode;
            docType.DocumentName = Resources.Global_String_OrderRequest;
            docTypeList.Add(docType);

            docType = new DocumentType();
            docType.DocumentCode = NotificationHelper.ppCode;
            docType.DocumentName = Resources.Global_String_ProcurementPlan;
            docTypeList.Add(docType);

            docType = new DocumentType();
            docType.DocumentCode = NotificationHelper.poCode;
            docType.DocumentName = Resources.Global_String_PurchaseOrder;
            docTypeList.Add(docType);

            docType = new DocumentType();
            docType.DocumentCode = NotificationHelper.rfpCode;
            docType.DocumentName = Resources.Global_String_RequestForPayment;
            docTypeList.Add(docType);

            return docTypeList;
        }

        public static List<ActionType> GetActionType(string docType)
        {
            List<ActionType> atList = new List<ActionType>();
            ActionType at = new ActionType();
            switch (docType)
            {
                case orCode:
                    at.Name = approvalCode;
                    atList.Add(at);

                    at = new ActionType();
                    at.Name = authorizationCode;
                    atList.Add(at);

                    at = new ActionType();
                    at.Name = reviewCode;
                    atList.Add(at);
                    break;
                case poCode:
                    at.Name = approvalCode;
                    atList.Add(at);
                    break;
                case rfpCode:
                    at.Name = authorizationCode;
                    atList.Add(at);

                    at = new ActionType();
                    at.Name = reviewCode;
                    atList.Add(at);

                    at = new ActionType();
                    at.Name = postFundsCode;
                    atList.Add(at);
                    break;
                case ppCode:
                    at.Name = approvalCode;
                    atList.Add(at);

                    at = new ActionType();
                    at.Name = reviewCode;
                    atList.Add(at);

                    at = new ActionType();
                    at.Name = approvalIICode;
                    atList.Add(at);

                    at = new ActionType();
                    at.Name =authorizationCode;
                    atList.Add(at);
                    break;
                case grnCode:
                    at.Name = verificationCode;
                    atList.Add(at);
                    break;
                case wrnCode:
                    at.Name = approvalCode;
                    atList.Add(at);
                    break;
                case ccCode:
                    at.Name = approvalCode;
                    atList.Add(at);
                    break;
                case paramsCode:
                    at.Name = approvalCode;
                    atList.Add(at);
                    break;
            }
            return atList;
        }

        #endregion

        public static bool ProjectExistsInList(ProjectBLCount pbl, List<ProjectBLCount> pblList)
        {
            foreach (var proj in pblList)
            {
                if (proj.ProjectDonorId.Equals(pbl.ProjectDonorId))
                    return true;
            }
            return false;
        }
    }

    public class DocumentType
    {
        public string DocumentCode { get; set; }

        public string DocumentName { get; set; }
    }

    public class ActionType
    {
        public string Name { get; set; }
    }


}
