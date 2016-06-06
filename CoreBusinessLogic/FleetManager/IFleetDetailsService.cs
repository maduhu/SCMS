using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.FleetManager
{
    public interface IFleetDetailsService
    {
        bool IsGarageInfoSaved(GarageInfo Entit, IEnumerable<Guid> ServicedItems);
        bool IsFleetDetailsSaved(FleetDetail Entity);
        bool IsAccesoryAdded(Equipment2Fleet Entity);
        bool IsDailyFleetSaved(FleetDailyManagement Entity);
        bool IsFleetDetailsEdited(FleetDetail entity);
        bool IsGarageInfoEdited(GarageInfo entity);
        bool IsDailyFleetEdited(FleetDailyManagement entity);
        bool IsFleetDeleted(Guid fleetId);
        bool IsAccesoryRemoved(Guid accesoryId);
        bool IsGarageInfoDeleted(Guid garageInfoId);
        bool IsDailyFleetInfoDeleted(Guid dailyFleetInfoId);
        float GetAverageFuelConsumed(Guid fleetId);
        List<Model.FleetDetail> GetFleetDetails();
        List<VehicleMake> GetVehicleMake();
        List<Model.DutyType> GetDutyTypes();
        List<Model.VehicleModel> GetVehicleModel(Guid mkId);
        List<Model.InsuranceType> GetInsuranceTypes();
        List<FleetEquipment> GetFleetAccesories();
        List<Model.FleetMajorIncidence> GetFleetIncidences();
        List<Model.CheckListCategory> GetCheckListCategory();
        List<Model.ServiceCheckList> GetServiceCheckList(Guid serviceCatId);
        Model.CheckListCategory GetCheckListCatById(Guid catId);
        Model.FleetDetail GetGenFleetDetailsById(Guid fleetId);
        Model.GarageInfo GetGarageInfoById(Guid garageInfoId);
        Model.FleetDailyManagement GetDailyFleetById(Guid dailyFleetId);
        Model.FleetDailyManagement GetLastDailyFleetRecord(Guid fleetId);
        Model.CheckListCategory GetCheckListCatByGarageInfo(Model.GarageInfo Entity);
        Model.FleetDailyManagement GetLastDailyFleetRecord(FleetDailyManagement entity);
        
    }
}
