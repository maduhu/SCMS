using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic.Budgeting;
using SCMS.CoreBusinessLogic.Caching;
using SCMS.CoreBusinessLogic.People;
using SCMS.CoreBusinessLogic.Projects;
using SCMS.CoreBusinessLogic._Country;
using SCMS.CoreBusinessLogic._Currency;
using SCMS.CoreBusinessLogic._Location;
using SCMS.CoreBusinessLogic._CountryProgramme;
using SCMS.CoreBusinessLogic._Designation;
using SCMS.CoreBusinessLogic._Inventory;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Settings;
using SCMS.CoreBusinessLogic.StaffServices;
using SCMS.CoreBusinessLogic._ItemCategory;
using SCMS.CoreBusinessLogic._ItemClassification;
using SCMS.CoreBusinessLogic._UnitOfMeasure;
using SCMS.CoreBusinessLogic._ExchangeRate;
using SCMS.CoreBusinessLogic._Supplier;
using SCMS.CoreBusinessLogic._CountrySubOffice;
using SCMS.CoreBusinessLogic._WareHouse;
using SCMS.CoreBusinessLogic._Item;
using SCMS.CoreBusinessLogic.PurchaseOrder;
using SCMS.CoreBusinessLogic.Request4Payment;
using SCMS.CoreBusinessLogic._GoodsReceivedNote;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.CoreBusinessLogic.WRF;
using SCMS.CoreBusinessLogic.WB;
using SCMS.CoreBusinessLogic.Users;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Reports;
using SCMS.CoreBusinessLogic.CompletionCtificate;
using SCMS.CoreBusinessLogic.FleetManager;
using SCMS.CoreBusinessLogic.InsuranceType;
using SCMS.CoreBusinessLogic.FleetAcceories;
using SCMS.CoreBusinessLogic.ProcurementPlan;
using SCMS.CoreBusinessLogic.FleetIncidences;
using SCMS.CoreBusinessLogic.DutyType;
using SCMS.Reports.Utilities;
using SCMS.CoreBusinessLogic.GeneralHelper;
using System.Data.Objects.DataClasses;
using SCMS.CoreBusinessLogic.BinCard;
using SCMS.CoreBusinessLogic.GoodsIssuedVoucher;
//using SCMS.CoreBusinessLogic.ActionFilters;


namespace SCMS.UI.Infrastructure
{
    /// <summary>
    /// Binds implementation objects to interfaces defined in the controllers. So no need to instatiate object in the controller
    /// for these interfaces
    /// </summary>
    public class NinjectControllerFactory : DefaultControllerFactory, IDependencyResolver
    {
        private static IKernel ninjectKernel;

        static NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }

        /// <summary>
        /// Add bindings of classes to interfaces that ninject will instatiate at runtime
        /// </summary>
        private static void AddBindings()
        {
            ninjectKernel.Bind<IBudgetService>().To<BudgetService>();
            ninjectKernel.Bind<IMasterBudgetService>().To<MasterBudgetService>();
            ninjectKernel.Bind<IProjectService>().To<ProjectService>();
            ninjectKernel.Bind<ICurrencyService>().To<CurrencyService>();
            ninjectKernel.Bind<ICountryService>().To<CountryService>();
            ninjectKernel.Bind<ILocationService>().To<LocationService>();
            ninjectKernel.Bind<ICountryProgrammeService>().To<CountryProgrammeService>();
            ninjectKernel.Bind<IDesignationService>().To<DesignationService>();
            ninjectKernel.Bind<IOrderRequest>().To<OrderRequest>();
            ninjectKernel.Bind<IItemCategoryService>().To<ItemCategoryService>();
            ninjectKernel.Bind<IItemClassificationService>().To<ItemClassificationService>();
            ninjectKernel.Bind<IUnitOfMeasureService>().To<UnitOfMeasureService>();
            ninjectKernel.Bind<IExchangeRateService>().To<ExchangeRateService>();
            ninjectKernel.Bind<ISupplierService>().To<SupplierService>();
            ninjectKernel.Bind<ICountrySubOfficeService>().To<CountrySubOfficeService>();
            ninjectKernel.Bind<IWareHouseService>().To<WareHouseService>();
            ninjectKernel.Bind<IItemService>().To<ItemService>();
            ninjectKernel.Bind<IPurchaseOrderService>().To<PurchaseOrderService>();
            ninjectKernel.Bind<IAuthenticationService>().To<AuthenticationService>();
            ninjectKernel.Bind<IRequest4PaymentService>().To<Request4PaymentService>();
            ninjectKernel.Bind<IGoodsReceivedNoteService>().To<GoodsReceivedNoteService>();
            ninjectKernel.Bind<INotificationService>().To<NotificationService>();
            ninjectKernel.Bind<IWareHouseReleaseService>().To<WareHouseReleaseService>();
            ninjectKernel.Bind<IWayBillService>().To<WayBillService>();
            ninjectKernel.Bind<ISystemUserService>().To<SystemUserService>().InSingletonScope();
            ninjectKernel.Bind<ICacheService>().To<CacheService>().InSingletonScope();
            ninjectKernel.Bind<IEncryptionService>().To<EncryptionService>().InSingletonScope();
            ninjectKernel.Bind<IPermissionService>().To<PermissionService>().InSingletonScope();
            ninjectKernel.Bind<IUserContext>().To<WebUserContext>().InSingletonScope();
            ninjectKernel.Bind<ISettingService>().To<SettingsService>().InSingletonScope();
            ninjectKernel.Bind<IPersonService>().To<PersonService>().InSingletonScope();
            ninjectKernel.Bind<IStaffService>().To<StaffService>().InSingletonScope();
            ninjectKernel.Bind<IImageService>().To<ImageService>().InSingletonScope();
            ninjectKernel.Bind<IExportHelper>().To<ExportHelper>().InSingletonScope();
            ninjectKernel.Bind<IFleetDetailsService>().To<FleetDetailsService>().InSingletonScope();
            ninjectKernel.Bind<ICompletionCertificateService>().To<CompletionCertificateService>().InSingletonScope();
            ninjectKernel.Bind<IVehicleMakeService>().To<VehicleMakeService>().InSingletonScope();
            ninjectKernel.Bind<IInsuranceTypeService>().To<InsuranceTypeService>().InSingletonScope();
            ninjectKernel.Bind<IFleetAccesoriesService>().To<FleetAccesoriesService>().InSingletonScope();
            ninjectKernel.Bind<IProcurementPlanService>().To<ProcurementPlanService>().InSingletonScope();
            ninjectKernel.Bind<IFleetIncidenceService>().To<FleetIncidenceService>().InSingletonScope();
            ninjectKernel.Bind<IDutyTypeService>().To<DutyTypeService>().InSingletonScope();
            ninjectKernel.Bind<IInventoryService>().To<InventoryService>().InSingletonScope();
            ninjectKernel.Bind<IBinCardService>().To<BinCardService>();
            //ninjectKernel.Bind<IReportHelper>().To<ReportHelper>().InSingletonScope();
            ninjectKernel.Bind<IGeneralHelperService>().To<GeneralHelperService>();
            ninjectKernel.Bind<IGoodsIssuedVoucherService>().To<GoodsIssuedVoucherService>();
        }

        #region Implementation of IDependencyResolver

        public object GetService(Type serviceType)
        {
            return ninjectKernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return ninjectKernel.GetAll(serviceType);
            }
            catch (Exception)
            {
                return new List<object>();
            }
        }

        #endregion
    }
}