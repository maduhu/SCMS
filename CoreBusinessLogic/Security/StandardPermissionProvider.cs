using System.Collections.Generic;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Security
{
    public partial class StandardPermissionProvider : IPermissionProvider
    {

        #region .Administration Related Permissions.

        public static readonly PermissionRecord AccessAdminPanel = new PermissionRecord { Name = "Access Administration Area", SystemName = "AccessAdminPanel", Category = "Standard" };
        public static readonly PermissionRecord SystemUsersManage = new PermissionRecord { Name = "System Users Manage", SystemName = "SystemUsersManage", Category = "SystemUsers" };
        public static readonly PermissionRecord SystemUsersView = new PermissionRecord { Name = "System Users View", SystemName = "SystemUsersView", Category = "SystemUsers" };
        public static readonly PermissionRecord RolesManage = new PermissionRecord { Name = "System Roles Manage", SystemName = "RolesManage", Category = "SystemUsers" };
        public static readonly PermissionRecord SettingsManage = new PermissionRecord { Name = "SCMS Settings Manage", SystemName = "SettingsManage", Category = "Configuration" };
        public static readonly PermissionRecord GlobalSettingsManage = new PermissionRecord { Name = "Global Settings Manage", SystemName = "GlobalSettingsManage", Category = "Configuration" };
        public static readonly PermissionRecord GlobalSettingsView = new PermissionRecord { Name = "Global Settings View", SystemName = "GlobalSettingsView", Category = "Configuration" };
        public static readonly PermissionRecord AclManage = new PermissionRecord { Name = "ACL Manage", Category = "Configuration", SystemName = "AclManage" };
        public static readonly PermissionRecord ApproversManage = new PermissionRecord { Name = "Approvers Manage", SystemName = "ApproversManage", Category = "Standard" };
        public static readonly PermissionRecord ApproversView = new PermissionRecord { Name = "Approvers View", SystemName = "ApproversView", Category = "Standard" };
        public static readonly PermissionRecord CountryProgrammesManage = new PermissionRecord { Name = "Country Programmes Manage", SystemName = "CountryProgrammesManage", Category = "Standard" };
        public static readonly PermissionRecord MultipleCountryProgrammeAccess = new PermissionRecord { Name = "Multiple Country Programme Access", SystemName = "MultipleCountryProgrammeAccess", Category = "Standard" };
        
        #endregion
        
        #region .Procurement Related Permissions.

        //ORDER REQUEST
        public static readonly PermissionRecord OrderRequestViewAll = new PermissionRecord { Name = "Order Request View All", SystemName = "OrderRequestViewAll", Category = "Standard" };
        public static readonly PermissionRecord OrderRequestViewDeparmental = new PermissionRecord { Name = "Order Request View Deparmental", SystemName = "OrderRequestViewDeparmental", Category = "Standard" };
        public static readonly PermissionRecord OrderRequestPrint = new PermissionRecord { Name = "Order Request Print", SystemName = "OrderRequestPrint", Category = "Standard" };
        public static readonly PermissionRecord OrderRequestManage = new PermissionRecord { Name = "Order Request Manage", SystemName = "OrderRequestManage", Category = "Standard" };
        public static readonly PermissionRecord OrderRequestApprove = new PermissionRecord { Name = "Order Request Approve", SystemName = "OrderRequestApprove", Category = "Standard" };
        public static readonly PermissionRecord OrderRequestReview = new PermissionRecord { Name = "Order Request Review", SystemName = "OrderRequestReview", Category = "Standard" };
        public static readonly PermissionRecord OrderRequestAuthorize = new PermissionRecord { Name = "Order Request Authorize", SystemName = "OrderRequestAuthorize", Category = "Standard" };
        //PURCHASE ORDER
        public static readonly PermissionRecord PurchaseOrderViewAll = new PermissionRecord { Name = "Purchase Order View All", SystemName = "PurchaseOrderViewAll", Category = "Standard" };
        public static readonly PermissionRecord PurchaseOrderViewDepartmental = new PermissionRecord { Name = "Purchase Order View Departmental", SystemName = "PurchaseOrderViewDepartmental", Category = "Standard" };
        public static readonly PermissionRecord PurchaseOrderPrint = new PermissionRecord { Name = "Purchase Order Print", SystemName = "PurchaseOrderPrint", Category = "Standard" };
        public static readonly PermissionRecord PurchaseOrderManage = new PermissionRecord { Name = "Purchase Order Manage", SystemName = "PurchaseOrderManage", Category = "Standard" };
        public static readonly PermissionRecord PurchaseOrderReview = new PermissionRecord { Name = "Purchase Order Review", SystemName = "PurchaseOrderReview", Category = "Standard" };
        public static readonly PermissionRecord PurchaseOrderAuthorize = new PermissionRecord { Name = "Purchase Order Authorize", SystemName = "PurchaseOrderAuthorize", Category = "Standard" };
        //OTHERS
        public static readonly PermissionRecord DocumentStatisticsView = new PermissionRecord { Name = "Document Statistics View", SystemName = "DocumentStatisticsView", Category = "Standard" };
        public static readonly PermissionRecord ProcurementPlanView = new PermissionRecord { Name = "Procurement Plan View", SystemName = "ProcurementPlanView", Category = "Standard" };
        public static readonly PermissionRecord ProcurementPlanPrint = new PermissionRecord { Name = "Procurement Plan Print", SystemName = "ProcurementPlanPrint", Category = "Standard" };
        public static readonly PermissionRecord ProcurementPlanManage = new PermissionRecord { Name = "Procurement Plan Manage", SystemName = "ProcurementPlanManage", Category = "Standard" };
        public static readonly PermissionRecord NotificationsManage = new PermissionRecord { Name = "Notifications Manage", SystemName = "NotificationsManage", Category = "Standard" };
        public static readonly PermissionRecord BackDateDocument = new PermissionRecord { Name = "Back Date Document", SystemName = "BackDateDocument", Category = "Standard" };

        public static readonly PermissionRecord CompletionCertificateView = new PermissionRecord { Name = "Completion Certificate View", SystemName = "CompletionCertificateView", Category = "Standard" };
        public static readonly PermissionRecord CompletionCertificatePrint = new PermissionRecord { Name = "Completion Certificate Print", SystemName = "CompletionCertificatePrint", Category = "Standard" };
        public static readonly PermissionRecord CompletionCertificateManage = new PermissionRecord { Name = "Completion Certificate Manage", SystemName = "CompletionCertificateManage", Category = "Standard" };
        
        #endregion

        #region .Budget & Finance Related Permissions.
        //BUDGET & MASTER BUDGET
        public static readonly PermissionRecord ProjectBudgetView = new PermissionRecord { Name = "Project Budget View", SystemName = "ProjectBudgetView", Category = "Standard" };
        public static readonly PermissionRecord MasterBudgetView = new PermissionRecord { Name = "Master Budget View", SystemName = "MasterBudgetView", Category = "Standard" };
        public static readonly PermissionRecord ProjectBudgetManage = new PermissionRecord { Name = "Project Budget Manage", SystemName = "ProjectBudgetManage", Category = "Standard" };
        public static readonly PermissionRecord MasterBudgetProjectionView = new PermissionRecord { Name = "Master Budget Projection View", SystemName = "MasterBudgetProjectionView", Category = "Standard" };
        public static readonly PermissionRecord MasterBudgetProjectionManage = new PermissionRecord { Name = "Master Budget Projection Manage", SystemName = "MasterBudgetProjectionManage", Category = "Standard" };
        public static readonly PermissionRecord MasterBudgetCategoriesView = new PermissionRecord { Name = "Master Budget Categories View", SystemName = "MasterBudgetCategoriesView", Category = "Standard" };
        public static readonly PermissionRecord MasterBudgetCategoriesManage = new PermissionRecord { Name = "Master Budget Categories Manage", SystemName = "MasterBudgetCategoriesManage", Category = "Standard" };
        public static readonly PermissionRecord ProjectApproversView = new PermissionRecord { Name = "Project Approvers View", SystemName = "ProjectApproversView", Category = "Standard" };
        public static readonly PermissionRecord ProjectApproversManage = new PermissionRecord { Name = "Project Approvers Manage", SystemName = "ProjectApproversManage", Category = "Standard" };
        public static readonly PermissionRecord PayrollItemsManage = new PermissionRecord { Name = "Payroll Items Manage", SystemName = "PayrollItemsManage", Category = "Standard" };
        public static readonly PermissionRecord RebookPosting = new PermissionRecord { Name = "Budget Posting Rebook", SystemName = "RebookPosting", Category = "Standard" };

        //REQUEST FOR PAYMENT
        public static readonly PermissionRecord RequestForPaymentViewAll = new PermissionRecord { Name = "Request For Payment View All", SystemName = "RequestForPaymentViewAll", Category = "Standard" };
        public static readonly PermissionRecord RequestForPaymentViewDepartmental = new PermissionRecord { Name = "Request For Payment View Departmental", SystemName = "RequestForPaymentViewDepartmental", Category = "Standard" };
        public static readonly PermissionRecord RequestForPaymentPrint = new PermissionRecord { Name = "Request For Payment Print", SystemName = "RequestForPaymentPrint", Category = "Standard" };
        public static readonly PermissionRecord RequestForPaymentManage = new PermissionRecord { Name = "Request For Payment Manage", SystemName = "RequestForPaymentManage", Category = "Standard" };
        public static readonly PermissionRecord RequestForPaymentReview = new PermissionRecord { Name = "Request For Payment Review", SystemName = "RequestForPaymentReview", Category = "Standard" };
        public static readonly PermissionRecord RequestForPaymentAuthorize = new PermissionRecord { Name = "Request For Payment Authorize", SystemName = "RequestForPaymentAuthorize", Category = "Standard" };
        public static readonly PermissionRecord RequestForPaymentPostFunds = new PermissionRecord { Name = "Request For Payment Post Funds", SystemName = "RequestForPaymentPostFunds", Category = "Standard" };
        
        #endregion

        #region .Inventory/Warehousing Related Permissions.

        //GOODS RECEIVED NOTES
        public static readonly PermissionRecord GoodsReceivedNoteViewAll = new PermissionRecord { Name = "Goods Received Note View All", SystemName = "GoodsReceivedNoteViewAll", Category = "Standard" };
        public static readonly PermissionRecord GoodsReceivedNoteViewDepartmental = new PermissionRecord { Name = "Goods Received Note View Departmental", SystemName = "GoodsReceivedNoteViewDepartmental", Category = "Standard" };
        public static readonly PermissionRecord GoodsReceivedNotePrint = new PermissionRecord { Name = "Goods Received Note Print", SystemName = "GoodsReceivedNotePrint", Category = "Standard" };
        public static readonly PermissionRecord GoodsReceivedNoteManage = new PermissionRecord { Name = "Goods Received Note Manage", SystemName = "GoodsReceivedNoteManage", Category = "Standard" };
        public static readonly PermissionRecord GoodsReceivedNoteVerify = new PermissionRecord { Name = "Goods Received Note Verify", SystemName = "GoodsReceivedNoteVerify", Category = "Standard" };
        public static readonly PermissionRecord GoodsReceivedNoteDownloadTempate = new PermissionRecord { Name = "Goods Received Note Download Tempate", SystemName = "GoodsReceivedNoteDownloadTempate", Category = "Standard" };
        //GOODS ISSUED VOUCHER
        public static readonly PermissionRecord GoodsIssuedVoucherViewAll = new PermissionRecord { Name = "Goods Issued Voucher View All", SystemName = "GoodsIssuedVoucherViewAll", Category = "Standard" };
        public static readonly PermissionRecord GoodsIssuedVoucherDepartmental = new PermissionRecord { Name = "Goods Issued Voucher Departmental", SystemName = "GoodsIssuedVoucherDepartmental", Category = "Standard" };
        public static readonly PermissionRecord GoodsIssuedVoucherPrint = new PermissionRecord { Name = "Goods Issued Voucher Print", SystemName = "GoodsIssuedVoucherPrint", Category = "Standard" };
        public static readonly PermissionRecord GoodsIssuedVoucherManage = new PermissionRecord { Name = "Goods Issued Voucher Manage", SystemName = "GoodsIssuedVoucherManage", Category = "Standard" };
        public static readonly PermissionRecord GoodsIssuedVoucherApprove = new PermissionRecord { Name = "Goods Issued Voucher Approve", SystemName = "GoodsIssuedVoucherApprove", Category = "Standard" };
        //STOCK MANAGEMENT
        public static readonly PermissionRecord StockManagementViewAll = new PermissionRecord { Name = "Stock Management View All", SystemName = "StockManagementViewAll", Category = "Standard" };
        public static readonly PermissionRecord StockManagementDepartmental = new PermissionRecord { Name = "Stock Management Departmental", SystemName = "StockManagementDepartmental", Category = "Standard" };
        public static readonly PermissionRecord StockManagementPrint = new PermissionRecord { Name = "Stock Management Print", SystemName = "StockManagementPrint", Category = "Standard" };
        public static readonly PermissionRecord StockManagementManage = new PermissionRecord { Name = "Stock Management Manage", SystemName = "StockManagementManage", Category = "Standard" };
        //WAREHOUSE RELEASE ORDERS
        public static readonly PermissionRecord WarehouseReleaseOrderViewAll = new PermissionRecord { Name = "Warehouse Release Order View All", SystemName = "WarehouseReleaseOrderViewAll", Category = "Standard" };
        public static readonly PermissionRecord WarehouseReleaseOrderDepartmental = new PermissionRecord { Name = "Warehouse Release Order Departmental", SystemName = "WarehouseReleaseOrderDepartmental", Category = "Standard" };
        public static readonly PermissionRecord WarehouseReleaseOrderPrint = new PermissionRecord { Name = "Warehouse Release Order Print", SystemName = "WarehouseReleaseOrderPrint", Category = "Standard" };
        public static readonly PermissionRecord WarehouseReleaseOrderManage = new PermissionRecord { Name = "Warehouse Release Order Manage", SystemName = "WarehouseReleaseOrderManage", Category = "Standard" };
        public static readonly PermissionRecord WarehouseReleaseOrderApprove = new PermissionRecord { Name = "Warehouse Release Order Approve", SystemName = "WarehouseReleaseOrderApprove", Category = "Standard" };
        //INVENTORY
        public static readonly PermissionRecord InventoryViewGeneral = new PermissionRecord { Name = "Inventory View General", SystemName = "InventoryViewGeneral", Category = "Standard" };
        public static readonly PermissionRecord InventoryViewAsset = new PermissionRecord { Name = "Inventory View Asset", SystemName = "InventoryViewAsset", Category = "Standard" };
        public static readonly PermissionRecord InventoryViewConsumable = new PermissionRecord { Name = "Inventory View Consumable", SystemName = "InventoryViewConsumable", Category = "Standard" };
        public static readonly PermissionRecord InventoryRegisterAssets = new PermissionRecord { Name = "Inventory Register Assets", SystemName = "InventoryRegisterAssets", Category = "Standard" };
        public static readonly PermissionRecord InventoryViewAssetDepreciation = new PermissionRecord { Name = "Inventory View Asset Depreciation", SystemName = "InventoryViewAssetDepreciation", Category = "Standard" }; 

        #endregion

        #region .Get Permissions.

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[] 
            {
                AccessAdminPanel,
                SystemUsersManage,
                SystemUsersView,
                RolesManage,
                SettingsManage,
                GlobalSettingsManage,
                GlobalSettingsView,
                AclManage,
                ApproversManage,
                ApproversView,
                OrderRequestViewAll,
                OrderRequestManage,
                OrderRequestApprove,
                OrderRequestReview,
                OrderRequestApprove,
                OrderRequestAuthorize,
                OrderRequestManage,
                OrderRequestPrint,
                OrderRequestReview,
                OrderRequestViewAll,
                OrderRequestViewDeparmental,
                PurchaseOrderAuthorize,
                PurchaseOrderManage,
                PurchaseOrderPrint,
                PurchaseOrderReview,
                PurchaseOrderViewAll,
                PurchaseOrderViewDepartmental,
                DocumentStatisticsView,
                ProcurementPlanManage,
                ProcurementPlanPrint,
                ProcurementPlanView,
                CompletionCertificateManage,
                CompletionCertificatePrint,
                CompletionCertificateView,
                NotificationsManage,
                RequestForPaymentAuthorize,
                RequestForPaymentManage,
                RequestForPaymentPostFunds,
                RequestForPaymentPrint,
                RequestForPaymentReview,
                RequestForPaymentViewAll,
                RequestForPaymentViewDepartmental,
                PayrollItemsManage,
                ProjectBudgetManage,
                ProjectBudgetView,
                ProjectApproversManage,
                ProjectApproversView,
                MasterBudgetView,
                MasterBudgetCategoriesManage,
                MasterBudgetCategoriesView,
                MasterBudgetProjectionManage,
                MasterBudgetProjectionView,
                GoodsReceivedNoteDownloadTempate,
                GoodsReceivedNoteManage,
                GoodsReceivedNotePrint,
                GoodsReceivedNoteVerify,
                GoodsReceivedNoteViewAll,
                GoodsReceivedNoteViewDepartmental,
                GoodsIssuedVoucherApprove,
                GoodsIssuedVoucherDepartmental,
                GoodsIssuedVoucherManage,
                GoodsIssuedVoucherPrint,
                GoodsIssuedVoucherViewAll,
                InventoryRegisterAssets,
                InventoryViewAsset,
                InventoryViewAssetDepreciation,
                InventoryViewConsumable,
                InventoryViewGeneral,
                RebookPosting,
                StockManagementDepartmental,
                StockManagementManage,
                StockManagementPrint,
                StockManagementViewAll,
                WarehouseReleaseOrderApprove,
                WarehouseReleaseOrderDepartmental,
                WarehouseReleaseOrderManage,
                WarehouseReleaseOrderPrint,
                WarehouseReleaseOrderViewAll,
                CountryProgrammesManage,
                MultipleCountryProgrammeAccess,
                BackDateDocument
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[] 
            {
                new DefaultPermissionRecord 
                {
                    RoleSystemName = SystemRoleNames.Administrators,
                    PermissionRecords = new[] 
                    {
                        AccessAdminPanel,
                        SystemUsersManage,
                        SystemUsersView,
                        RolesManage,
                        SettingsManage,
                        GlobalSettingsManage,
                        GlobalSettingsView,
                        AclManage,
                        ApproversManage,
                        ApproversView,
                        OrderRequestViewAll,
                        OrderRequestManage,
                        OrderRequestApprove,
                        OrderRequestReview,
                        CountryProgrammesManage,
                        MultipleCountryProgrammeAccess,
                        BackDateDocument
                    }
                },
            };
        }

        #endregion
    }
}