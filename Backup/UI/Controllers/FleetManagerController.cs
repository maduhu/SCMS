using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Security;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.FleetManager;
using SCMS.CoreBusinessLogic._Inventory;
using System.Text;
using SCMS.Model;
using System.Data.Objects.DataClasses;
using SCMS.CoreBusinessLogic.OrderRequest;
using System.Web.UI;
using System.Globalization;
using SCMS.Resource;
using SCMS.UI.GeneralHelper;
using SCMS.CoreBusinessLogic.StaffServices;
using SCMS.CoreBusinessLogic.PurchaseOrder;
using SCMS.CoreBusinessLogic.GeneralHelper;
using Telerik.Web.Mvc;
using SCMS.Utils;

namespace SCMS.UI.Controllers
{
    //[OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    [MyException]
    public class FleetManagerController : PortalBaseController
    {
        #region Declaration
        private IOrderRequest orService;
        private IPurchaseOrderService poService;
        private IFleetDetailsService fleetservice;
        private InventoryService inventoryService;
        private readonly IStaffService staffService;
        #endregion

        public FleetManagerController(IPermissionService permissionService, IUserContext userContext, IFleetDetailsService fleetservice,
            IStaffService staffService, InventoryService inventoryService, IOrderRequest ORService, IPurchaseOrderService poService)
            : base(userContext, permissionService)
        {
            this.orService = ORService;
            this.staffService = staffService;
            this.fleetservice = fleetservice;
            this.inventoryService = inventoryService;
            this.poService = poService;
        }

        //
        // GET: /FleetManager/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadFleetDetails(Guid? id = null)
        {
            Guid fleetid = id.GetValueOrDefault();
            var model = new FleetDetail() { FDList = new SelectList(fleetservice.GetFleetDetails(), "Id", "AssetOrPlateNo") };

            if (id.IsNotNull())
            {
                model = fleetservice.GetGenFleetDetailsById(fleetid);
                model.FDList = new SelectList(fleetservice.GetFleetDetails(), "Id", "AssetOrPlateNo");
                model.AvrgFuelConsumption = fleetservice.GetAverageFuelConsumed(fleetid).ToString(Constants.NUMBER_FORMAT_TWO_DECIMAL);
            }
            ViewBag.fleetId = id;
            ViewBag.AssetNo = model != null ? "[" + model.PlateNo + "]" : "";
            return View("LoadFleetDetails", model);
        }

        public ActionResult LoadNewFleetDetail(Guid? fleetId = null)
        {
            Guid fleetid = fleetId.GetValueOrDefault();
            Model.FleetDetail FDetails = fleetservice.GetGenFleetDetailsById(fleetid);

            var model = fleetId == null ? new Model.FleetDetail() : FDetails;
            model.Brands = new SelectList(fleetservice.GetVehicleMake(), "Id", "Name", fleetId == null ? Guid.Empty : FDetails.VehicleModel != null ? FDetails.VehicleModel.VehicleMake.Id : Guid.Empty);
            model.Modelz = new SelectList(fleetservice.GetVehicleModel(fleetId == null ? Guid.Empty : FDetails.VehicleModel != null ? FDetails.VehicleModel.VehicleMake.Id : Guid.Empty), "Id", "Name");
            model.Assets = new SelectList(inventoryService.GetAssetList(countryProg.Id, true, fleetId == null ? true : false), "Id", "Name");
            model.InsuranceTypes = new SelectList(fleetservice.GetInsuranceTypes(), "Id", "Name");
            model.DutyTypes = new SelectList(fleetservice.GetDutyTypes(), "Id", "Name");
            model.Staffs = new SelectList(SessionData.CurrentSession.StaffList, "Id", "StaffName");
            model.Action = fleetId == null ? "SaveNewFleet" : "EditGenFleet";
            return View(model);
        }

        public JsonResult IsPlateNoExist(string PlateNo, Guid Id)
        {
            bool exists = SessionData.CurrentSession.FleetDetailsList.Any(item => item.PlateNo == PlateNo && item.Id != Id);
            if (exists)
                return Json(Resources.FleetManagerController_String_PlateNoalreadyexists, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);

            //if (!fleetservice.IsPlateNoExist(countryProg.Id, PlateNo))
            //    return Json(true, JsonRequestBehavior.AllowGet);
            //string suggested = String.Format(CultureInfo.InvariantCulture, Resources.FleetManagerController_String_AlreadyRegistered, PlateNo);
            //return Json(suggested, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsEngineNoExist(string EngineNo, Guid Id)
        {
            bool exists = SessionData.CurrentSession.FleetDetailsList.Any(item => item.EngineNo == EngineNo && item.Id != Id);
            if (exists)
                return Json(Resources.FleetManagerController_String_EngineNoalreadyexists, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsChassisNoExist(string ChassisNo, Guid Id)
        {
            bool exists = SessionData.CurrentSession.FleetDetailsList.Any(item => item.ChassisNo == ChassisNo && item.Id != Id);
            if (exists)
                return Json(Resources.FleetManagerController_String_ChasisNoalreadyexists, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadNewDailyFleet(Guid id, Guid? dailyFleetInfoId = null)
        {
            Guid dailyFleetInfoid = dailyFleetInfoId.GetValueOrDefault();

            Model.FleetDailyManagement FDailyMgtDetails = fleetservice.GetDailyFleetById(dailyFleetInfoid);
            var model = dailyFleetInfoId == null ? new Model.FleetDailyManagement() : FDailyMgtDetails;
            model.Incidences = new SelectList(fleetservice.GetFleetIncidences(), "Id", "Name");
            model.Staffs = new SelectList(SessionData.CurrentSession.StaffList, "Id", "StaffName");
            model.FleetId = id;

            Model.FleetDailyManagement lastDailyFleet = dailyFleetInfoId == null ? fleetservice.GetLastDailyFleetRecord(id) : fleetservice.GetLastDailyFleetRecord(FDailyMgtDetails);
            if (dailyFleetInfoId == null) model.StartMilleage = lastDailyFleet != null ? lastDailyFleet.EndMilleage : double.Parse(fleetservice.GetGenFleetDetailsById(id).InitialMilleage);
            if (dailyFleetInfoId == null) model.Date = lastDailyFleet != null ? lastDailyFleet.Date.Value.AddDays(1).Date : DateTime.Now.Date;
            if (dailyFleetInfoId == null) model.FuelCost = lastDailyFleet != null ? lastDailyFleet.FuelCost : 0;
            model.LastMilleage = lastDailyFleet != null ? lastDailyFleet.EndMilleage : 0;
            model.CurrentStartMilleage = dailyFleetInfoId == null ? lastDailyFleet != null ? lastDailyFleet.EndMilleage : 0 : FDailyMgtDetails.EndMilleage;
            model.Action = dailyFleetInfoId == null ? "SaveNewDailyFleetMgt" : "EditDailyFleetMgt";
            model.FleetTypLable = fleetservice.GetGenFleetDetailsById(id).FleetType == "Motor" ? Resources.FleetManager_LoadFleetDetails_DistanceCovered : Resources.FleetManagerController_String_TimeSpent;
            return View(model);
        }

        [GridAction]
        public JsonResult Filtering(Guid? fleetId)
        {
            var FDetail = SessionData.CurrentSession.FleetDetailsList.FirstOrDefault(p => p.Id == fleetId);
            if (FDetail == null) FDetail = new FleetDetail();

            return Json(FDetail.FleetDailyManagements.OrderByDescending(s => s.IssueDate).Select(n => new
            {
                Id = n.Id,
                Date = n.Date,
                StartMilleage = n.StartMilleage,
                EndMilleage = n.EndMilleage,
                DistancedCovered = n.DistancedCovered,
                TotalFuelConsumed = n.TotalFuelConsumed,
                IncidenceName = n.FleetMajorIncidence.Name,
                FleetId = n.FleetId
            }),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadNewGarageInfo(Guid id, Guid? garageInfoId = null, Guid? category = null)
        {
            Guid garageInfoid = garageInfoId.GetValueOrDefault();
            Model.GarageInfo FGarageDetails = fleetservice.GetGarageInfoById(garageInfoid);

            var model = garageInfoId == null ? new GarageInfo() : FGarageDetails;
            model.Currencies = new SelectList(orService.GetCurrencies(), "Id", "ShortName");
            model.Suppliers = new SelectList(poService.GetSuppliers(), "Id", "Name");
            model.MileageCategory = new SelectList(fleetservice.GetCheckListCategory(), "Id", "Name");
            model.FleetId = id;
            if (category != null) model.ChecKListCatId = category;

            if (FGarageDetails != null)
            {
                CheckListCategory CListCat = fleetservice.GetCheckListCatByGarageInfo(FGarageDetails);
                model.ChecKListCatId = CListCat != null ? CListCat.Id : (Guid?)null;
            }
            ViewBag.Action = garageInfoId == null ? "SaveNewGarageInfo" : "EditGarageInfo";
            return View("LoadNewGarageInfo", model);

        }

        public ActionResult LoadServiceCheckListPopUp(Guid categoryId, Guid FleetId, Guid? gInfoId = null)
        {
            ViewBag.FleetId = FleetId;
            ViewBag.gInforId = gInfoId;
            ViewBag.category = categoryId;
            return View(fleetservice.GetServiceCheckList(categoryId));
        }

        [GridAction]
        public JsonResult GarageInfoFiltering(Guid? fleetId)
        {
            var FDetail = SessionData.CurrentSession.FleetDetailsList.FirstOrDefault(p => p.Id == fleetId);
            if (FDetail == null) FDetail = new FleetDetail();

            return Json(FDetail.GarageInfoes.OrderByDescending(s => s.Dateken).Select(n => new
            {
                Id = n.Id,
                SupplierName = n.SupplierName,
                Dateken = n.Dateken,
                OtherProblemsDescription = n.OtherProblemsDescription,
                IfServiceCurrentMilleage = n.IfServiceCurrentMilleage,
                IfServiceNextMilleage = n.IfServiceNextMilleage,
                MilleageOut = n.MilleageOut,
                DateOutOfGarage = n.DateOutOfGarage,
                FleetId = n.FleetId
            }),
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveServiceCList(ServiceCheckList model)
        {
            UserSession.CurrentSession.ServiceItems = model.ServiceCList;
            return LoadNewGarageInfo(model.FleetId, model.gInfoId == Guid.Empty ? null : model.gInfoId, model.ServiceCategoryId);
        }

        public ActionResult LoadFleetAccecories(Guid id)
        {
            var model = new Equipment2Fleet()
            {
                Accesories = new SelectList(fleetservice.GetFleetAccesories(), "Id", "EquipmentName"),
                FleetId = id
            };
            return View(model);
        }

        [GridAction]
        public JsonResult AccesoryFiltering(Guid? fleetId)
        {
            var FDetail = SessionData.CurrentSession.FleetDetailsList.FirstOrDefault(p => p.Id == fleetId);
            if (FDetail == null) FDetail = new FleetDetail();

            int rowCount = 0;
            return Json(FDetail.Equipment2Fleet.Select(n => new
            {
                Id = n.Id,
                EquipmentName = n.EquipmentName,
                FleetId = n.FleetId,
                rowCount = rowCount++
            }),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssetModel(Guid mkId)
        {
            StringBuilder blineOption = new StringBuilder();
            blineOption.Append("<select id=\"ModelId\" name=\"ModelId\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            foreach (var item in fleetservice.GetVehicleModel(mkId))
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.Name + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"ModelId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");
            ViewBag.Html = blineOption.ToString();
            return View("HTMLResponse");
        }

        [HttpPost]
        public ActionResult SaveNewFleet(FleetDetail entitymodel)
        {
            entitymodel.Id = Guid.NewGuid();
            entitymodel.CountryProgrammeId = countryProg.Id;
            fleetservice.IsFleetDetailsSaved(entitymodel);
            return LoadFleetDetails();
        }

        [HttpPost]
        public ActionResult SaveNewDailyFleetMgt(FleetDailyManagement entitymodel)
        {
            entitymodel.Id = Guid.NewGuid();
            entitymodel.IssueDate = DateTime.Now;
            fleetservice.IsDailyFleetSaved(entitymodel);
            return LoadFleetDetails(entitymodel.FleetId);
        }

        [HttpPost]
        public ActionResult SaveNewGarageInfo(GarageInfo entitymodel)
        {
            entitymodel.Id = Guid.NewGuid();
            entitymodel.IssueDate = DateTime.Now;
            if (entitymodel.ReasonType != "Service") entitymodel.IfServiceNextMilleage = entitymodel.IfServiceCurrentMilleage;
            if (entitymodel.ReasonType == "Maintenance") { entitymodel.ReasonType += " " + entitymodel.MaintenaceType; }
            if (fleetservice.IsGarageInfoSaved(entitymodel, UserSession.CurrentSession.ServiceItems)) UserSession.CurrentSession.ServiceItems = null;
            return LoadFleetDetails(entitymodel.FleetId);
        }

        [HttpPost]
        public ActionResult AddNewAccesory(Equipment2Fleet entitymodel)
        {
            entitymodel.Id = Guid.NewGuid();
            fleetservice.IsAccesoryAdded(entitymodel);
            return LoadFleetDetails(entitymodel.FleetId);
        }

        public ActionResult EditGenFleet(FleetDetail entitymodel)
        {
            fleetservice.IsFleetDetailsEdited(entitymodel);
            return LoadFleetDetails(entitymodel.Id);
        }

        public ActionResult EditGarageInfo(GarageInfo entitymodel)
        {
            entitymodel.Id = new Guid(entitymodel.SavedId);
            fleetservice.IsGarageInfoEdited(entitymodel);
            return LoadFleetDetails(entitymodel.FleetId);
        }

        public ActionResult EditDailyFleetMgt(FleetDailyManagement entitymodel)
        {
            entitymodel.Id = new Guid(entitymodel.SavedId);
            fleetservice.IsDailyFleetEdited(entitymodel);
            return LoadFleetDetails(entitymodel.FleetId);
        }

        public ActionResult DeleteFleet(Guid fleetId)
        {
            fleetservice.IsFleetDeleted(fleetId);
            return LoadFleetDetails();
        }

        public ActionResult DeleteDailyFleetInfo(Guid dailyFleetInfoId, Guid fleetId)
        {
            fleetservice.IsDailyFleetInfoDeleted(dailyFleetInfoId);
            return LoadFleetDetails(fleetId);
        }

        public ActionResult DeleteGarageInfo(Guid garageInfoId, Guid fleetId)
        {
            fleetservice.IsGarageInfoDeleted(garageInfoId);
            return LoadFleetDetails(fleetId);
        }

        public ActionResult RemoveAccesory(Guid accesoryId, Guid fleetId)
        {
            fleetservice.IsAccesoryRemoved(accesoryId);
            return LoadFleetDetails(fleetId);
        }
    }
}
