using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Security;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.GeneralHelper
{
    public partial class SessionData
    {

        #region .private variables.

        private IUserContext userContext;
        private IEnumerable<Model.GoodsReceivedNote> _grnList;
        private IEnumerable<Model.Inventory> _inventoryList;
        private IEnumerable<Model.Asset> _assetList;
        private IEnumerable<Model.GoodsReceivedNoteItem> _grnItemList;
        private IEnumerable<Model.Bin> _binList;
        private IEnumerable<Model.BinItem> _binItemList;
        private IEnumerable<Model.WarehouseRelease> _releaseOrderList;
        private IEnumerable<Model.WarehouseReleaseItem> _releaseOrderItemList;
        private IEnumerable<Model.GoodsIssuedVoucher> _givList;
        private IEnumerable<Model.Depreciation> _depreciationList;
        private IEnumerable<Model.FleetDetail> _fleetDetailList;

        #endregion

        public SessionData(IUserContext userContext)
        {
            this.userContext = userContext;
        }

        private static SessionData mm;

        public static SessionData CurrentSession
        {

            get
            {
                SessionData session = (SessionData)HttpContext.Current.Session["__SessionData__"];
                if (session == null)
                {
                    session = new SessionData(DependencyResolver.Current.Resolve<IUserContext>());
                    HttpContext.Current.Session["__SessionData__"] = session;
                }
                return session;
            }
            set
            {
                HttpContext.Current.Session["__SessionData__"] = value;
            }
        }

        public Guid CountryProgrammeId
        {
            get { return userContext.CurrentUser.Staff.CountrySubOffice.CountryProgramme.Id; }
        }

        public CountryProgramme CountryProg
        {
            get { return userContext.CurrentUser.Staff.CountrySubOffice.CountryProgramme; }
        }

        public IEnumerable<Model.GoodsReceivedNote> GoodsReceivedNoteList
        {
            get
            {
                if (_grnList == null || _grnList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _grnList = db.GoodsReceivedNotes
                    .IncludeSupplier()
                    .IncludeStaff()
                    .IncludeCountrySubOffice()
                    .IncludePurchaseOrderItem()
                    .IncludePurchaseOrder()
                    .IncludeOrderRequest()
                    .Where(p => p.CountryProgrammeId == CountryProgrammeId)
                    .OrderByDescending(g => g.PreparedOn).ToList();
                    }
                }
                return _grnList;
            }
            set
            {
                _grnList = value;
            }
        }

        public IEnumerable<Model.Inventory> InventoryList
        {
            get
            {
                if (_inventoryList == null || _inventoryList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _inventoryList = dbContext.Inventories
                            .IncludeItem()
                            .IncludeWareHouse()
                            .IncludeItemClassification()
                            .IncludeItemCategory()
                            .IncludeUnitOfMeasure()
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _inventoryList;
            }
            set
            {
                _inventoryList = value;
            }
        }

        public IEnumerable<Model.Asset> AssetList
        {
            get
            {
                if (_assetList == null || _assetList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _assetList = dbContext.Assets
                            .IncludeItemClassification()
                            .IncludeProject1()
                            .IncludeDonor1()
                            .IncludeWareHouse()
                            .IncludePerson()
                            .IncludeCurrency()
                            .IncludeAssetManagments()
                            .IncludeProject()
                            .IncludeDonor()
                            .IncludeDepreciation()
                            .IncludeFleetDetails()
                            .IncludeAssetManagmentsProjectDonor()
                            .Where(p => p.CountryProgramId == CountryProgrammeId).ToList();
                    }
                }
                return _assetList;
            }
            set
            {
                _assetList = value;
            }
        }

        public IEnumerable<Model.GoodsReceivedNoteItem> GoodsReceivedNoteItemList
        {
            get
            {
                if (_grnItemList == null || _grnItemList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _grnItemList = dbContext.GoodsReceivedNoteItems
                            .IncludeGoodsReceivedNote()
                            .IncludePurchaseOrder()
                            .IncludeOrderRequestItem()
                            .IncludeProcurementPlanItem()
                            .IncludeAssets()
                            .IncludePurchaseOrderItem()
                            .IncludeBinItems()
                            .Where(p => p.GoodsReceivedNote.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _grnItemList;
            }
            set
            {
                _grnItemList = value;
            }
        }

        public IEnumerable<Model.Bin> BinList
        {
            get
            {
                if (_binList == null || _binList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _binList = dbContext.Bins
                            .IncludeBinItems()
                            .IncludeItemPackage()
                            .IncludeStaff()
                            .IncludeWareHouse()
                            .IncludeORItem()
                            .IncludeOrderRequest()
                            .IncludeProcurementPlan()
                            .IncludeBinItemWHRelease()
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _binList;
            }
            set
            {
                _binList = value;
            }
        }

        public IEnumerable<Model.BinItem> BinItemList
        {
            get
            {
                if (_binItemList == null || _binItemList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _binItemList = dbContext.BinItems
                            .IncludeGRNItemAndGRN()
                            .IncludeBin()
                            .IncludeBinPP()
                            .IncludeWareHouseRelease()
                            .Where(p => p.Bin.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _binItemList;
            }
            set
            {
                _binItemList = value;
            }
        }

        public IEnumerable<Model.WarehouseRelease> ReleaseOrderList
        {
            get
            {
                if (_releaseOrderList == null || _releaseOrderList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _releaseOrderList = dbContext.WarehouseReleases
                          .IncludeReleaseOrderItems()
                          .IncludeStaff()
                          .IncludeWareHouse()
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _releaseOrderList;
            }
            set
            {
                _releaseOrderList = value;
            }
        }

        public IEnumerable<Model.WarehouseReleaseItem> ReleaseOrderItemList
        {
            get
            {
                if (_releaseOrderItemList == null || _releaseOrderItemList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _releaseOrderItemList = dbContext.WarehouseReleaseItems
                            .IncludeReleaseOrderItemsDetails()
                            .IncludeWarehouseRelease()
                            .IncludeBinItems()
                            .Where(p => p.WarehouseRelease.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _releaseOrderItemList;
            }
            set
            {
                _releaseOrderItemList = value;
            }
        }

        public IEnumerable<Model.GoodsIssuedVoucher> GIVList
        {
            get
            {
                if (_givList == null || _givList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _givList = dbContext.GoodsIssuedVouchers
                          .IncludeGoodsIssuedVoucherItems()
                          .IncludeWarehouseRelease()
                          .IncludeStaff()
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _givList;
            }
            set
            {
                _givList = value;
            }
        }

        public IEnumerable<Model.Depreciation> DepreciationList
        {
            get
            {
                if (_depreciationList == null || _depreciationList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _depreciationList = dbContext.Depreciations
                          .IncludeAssetDetails()
                            .Where(p => p.Asset.CountryProgramId == CountryProgrammeId).ToList();
                    }
                }
                return _depreciationList;
            }
            set
            {
                _depreciationList = value;
            }
        }

        public IEnumerable<Model.FleetDetail> FleetDetailsList
        {
            get
            {
                if (_fleetDetailList == null || _fleetDetailList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _fleetDetailList = dbContext.FleetDetails
                            .IncludeStaff()
                            .IncludeFleetDetails()
                            .IncludeGarageInfoes()
                            .IncludeEquipment2Fleet()
                            .IncludeFleetDailyManagements()
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _fleetDetailList;
            }
            set
            {
                _fleetDetailList = value;
            }
        }
    }
}
