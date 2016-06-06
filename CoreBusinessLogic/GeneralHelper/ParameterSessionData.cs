using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.GeneralHelper
{
    public partial class SessionData
    {

        #region .private variables.

        private IEnumerable<Model.CountrySubProgramme> _countrySubProgList;
        private IEnumerable<Model.CountrySubOffice> _countrySubOfficeList;
        private IEnumerable<Model.UnitOfMeasure> _unitOfMeastureList;
        private IEnumerable<Model.Currency> _currencyList;
        private IEnumerable<Model.Location> _locationList;
        private IEnumerable<Model.Item> _itemList;
        private IEnumerable<Model.WareHouse> _warehouseList;
        private IEnumerable<Model.ItemPackage> _ItemPackageList;
        private IEnumerable<Model.PaymentTerm> _paymentTermList;
        private IEnumerable<Model.VehicleMake> _vehicleMakeList;
        private IEnumerable<Model.VehicleModel> _vehicleModelList;
        private IEnumerable<Model.DutyType> _dutyTypeList;
        private IEnumerable<Model.InsuranceType> _insuranceTypeList;
        private IEnumerable<Model.ServiceCheckList> _serviceCheckListList;
        private IEnumerable<Model.CheckListCategory> _checkListCategoryList;
        private IEnumerable<Model.FleetMajorIncidence> _fleetMajorIncidenceList;
        private IEnumerable<Model.FleetEquipment> _fleetEquipmentList;
        private IEnumerable<Model.ShippingTerm> _shippingTermList;
        private IEnumerable<Model.PaymentType> _paymentTypeList;
        private IEnumerable<Model.Supplier> _supplierList;
        private IEnumerable<Model.Staff> _staffList;

        #endregion
        public IEnumerable<Model.CountrySubOffice> CountrySubOfficeList
        {
            get
            {
                if (_countrySubOfficeList == null || _countrySubOfficeList.Count() == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _countrySubOfficeList = db.CountrySubOffices
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _countrySubOfficeList;
            }
            set
            {
                _countrySubOfficeList = value;
            }
        }

        public IEnumerable<Model.CountrySubProgramme> CountrySubProgrammeList
        {
            get
            {
                if (_countrySubProgList == null || _countrySubProgList.Count() == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _countrySubProgList = db.CountrySubProgrammes
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _countrySubProgList;
            }
            set
            {
                _countrySubProgList = value;
            }
        }

        public IEnumerable<Model.UnitOfMeasure> UnitOfMeasureList
        {
            get
            {
                if (_unitOfMeastureList == null || _unitOfMeastureList.Count() == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _unitOfMeastureList = db.UnitOfMeasures
                            .Where(u => u.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _unitOfMeastureList;
            }
            set
            {
                _unitOfMeastureList = value;
            }
        }

        public IEnumerable<Model.Currency> CurrencyList
        {
            get
            {
                if (_currencyList == null || _currencyList.Count() == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _currencyList = db.Currencies
                            .Where(c => c.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _currencyList;
            }
            set
            {
                _currencyList = value;
            }
        }

        public IEnumerable<Model.Location> LocationList
        {
            get
            {
                if (_locationList == null)
                {
                    using (var db = new SCMSEntities())
                    {
                        _locationList = db.Locations
                            .Where(c => c.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _locationList;
            }
            set
            {
                _locationList = value;
            }
        }

        public IEnumerable<Model.Item> ItemList
        {
            get
            {
                if (_itemList == null || _itemList.Count() == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _itemList = db.Items
                            .IncludeItemCatetory()
                            .IncludeUnitOfMeasure()
                            .IncludeItemClassification()
                            .Where(c => c.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _itemList;
            }
            set
            {
                _itemList = value;
            }
        }

        public IEnumerable<Model.WareHouse> WarehouseList
        {
            get
            {
                if (_warehouseList == null)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _warehouseList = dbContext.WareHouses
                            .IncludeProcurementChain()
                            .IncludeLocation()
                            .Where(c => c.CountryProgrammeId == CountryProgrammeId).OrderBy(w => w.Name).ToList();
                    }
                }
                return _warehouseList;
            }
            set
            {
                _warehouseList = value;
            }
        }

        public IEnumerable<Model.ItemPackage> ItemPackageList
        {
            get
            {
                if (_ItemPackageList == null)
                {
                    using (var db = new SCMSEntities())
                    {
                        _ItemPackageList = db.ItemPackages
                            .Where(c => c.Item.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _ItemPackageList;
            }
            set
            {
                _ItemPackageList = value;
            }
        }

        public IEnumerable<Model.PaymentTerm> PaymentTermList
        {
            get
            {
                if (_paymentTermList == null)
                {
                    using (var db = new SCMSEntities())
                    {
                        _paymentTermList = db.PaymentTerms
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _paymentTermList;
            }
            set
            {
                _paymentTermList = value;
            }
        }

        public IEnumerable<Model.VehicleMake> VehicleMakeList
        {
            get
            {
                if (_vehicleMakeList == null || _vehicleMakeList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _vehicleMakeList = db.VehicleMakes
                            .IncludeVehicleModel()
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _vehicleMakeList;
            }
            set
            {
                _vehicleMakeList = value;
            }
        }

        public IEnumerable<Model.VehicleModel> VehicleModelList
        {
            get
            {
                if (_vehicleModelList == null || _vehicleModelList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _vehicleModelList = db.VehicleModels
                            .IncludeVehicleMake()
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _vehicleModelList;
            }
            set
            {
                _vehicleModelList = value;
            }
        }

        public IEnumerable<Model.DutyType> DutyTypeList
        {
            get
            {
                if (_dutyTypeList == null || _dutyTypeList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _dutyTypeList = db.DutyTypes
                            .Where(p => p.CountryProgramId == CountryProgrammeId).ToList();
                    }
                }
                return _dutyTypeList;
            }
            set
            {
                _dutyTypeList = value;
            }
        }

        public IEnumerable<Model.InsuranceType> InsuranceTypeList
        {
            get
            {
                if (_insuranceTypeList == null || _insuranceTypeList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _insuranceTypeList = db.InsuranceTypes
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _insuranceTypeList;
            }
            set
            {
                _insuranceTypeList = value;
            }
        }

        public IEnumerable<Model.ServiceCheckList> ServiceCheckListList
        {
            get
            {
                if (_serviceCheckListList == null || _serviceCheckListList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _serviceCheckListList = db.ServiceCheckLists
                            .IncludeCheckListCategory()
                            .Where(p => p.CheckListCategory.CountryprogrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _serviceCheckListList;
            }
            set
            {
                _serviceCheckListList = value;
            }
        }

        public IEnumerable<Model.CheckListCategory> CheckListCategoryList
        {
            get
            {
                if (_checkListCategoryList == null || _checkListCategoryList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _checkListCategoryList = db.CheckListCategories
                            .IncludeServiceCheckLists()
                            .Where(p => p.CountryprogrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _checkListCategoryList;
            }
            set
            {
                _checkListCategoryList = value;
            }
        }

        public IEnumerable<Model.FleetMajorIncidence> FleetMajorIncidenceList
        {
            get
            {
                if (_fleetMajorIncidenceList == null || _fleetMajorIncidenceList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _fleetMajorIncidenceList = db.FleetMajorIncidences
                            .Where(p => p.CountryProgramId == CountryProgrammeId).ToList();
                    }
                }
                return _fleetMajorIncidenceList;
            }
            set
            {
                _fleetMajorIncidenceList = value;
            }
        }

        public IEnumerable<Model.FleetEquipment> FleetEquipmentList
        {
            get
            {
                if (_fleetEquipmentList == null || _fleetEquipmentList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _fleetEquipmentList = db.FleetEquipments
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _fleetEquipmentList;
            }
            set
            {
                _fleetEquipmentList = value;
            }
        }

        public IEnumerable<Model.PaymentType> PaymentTypeList
        {
            get
            {
                if (_paymentTypeList == null)
                {
                    using (var db = new SCMSEntities())
                    {
                        _paymentTypeList = db.PaymentTypes
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _paymentTypeList;
            }
            set
            {
                _paymentTypeList = value;
            }
        }

        public IEnumerable<Model.ShippingTerm> ShippingTermList
        {
            get
            {
                if (_shippingTermList == null)
                {
                    using (var db = new SCMSEntities())
                    {
                        _shippingTermList = db.ShippingTerms
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _shippingTermList;
            }
            set
            {
                _shippingTermList = value;
            }
        }

        public IEnumerable<Model.Supplier> SupplierList
        {
            get
            {
                if (_supplierList == null)
                {
                    using (var db = new SCMSEntities())
                    {
                        _supplierList = db.Suppliers
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _supplierList;
            }
            set
            {
                _supplierList = value;
            }
        }

        public IEnumerable<Model.Staff> StaffList
        {
            get
            {
                if (_staffList == null || _staffList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _staffList = db.Staffs
                            .IncludeStaffDetails()
                            .Where(p => p.Person.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _staffList;
            }
            set
            {
                _staffList = value;
            }
        }
    }
}
