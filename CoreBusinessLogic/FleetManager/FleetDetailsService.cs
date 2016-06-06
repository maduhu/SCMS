using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using System.Data.Objects.DataClasses;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.CoreBusinessLogic.FleetManager
{
    public class FleetDetailsService : IFleetDetailsService
    {
        public bool IsFleetDetailsSaved(FleetDetail Entity)
        {
            using (var db = new SCMSEntities())
            {
                db.FleetDetails.Add(Entity);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsDailyFleetSaved(FleetDailyManagement Entity)
        {
            using (var db = new SCMSEntities())
            {
                clearSession();
                db.FleetDailyManagements.Add(Entity);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsGarageInfoSaved(GarageInfo Entity, IEnumerable<Guid> ServicedItems)
        {
            using (var db = new SCMSEntities())
            {
                clearSession();
                if (Entity.ReasonType == "Service")
                    Entity.IfServiceNextMilleage = Entity.IfServiceCurrentMilleage + db.CheckListCategories.FirstOrDefault(p => p.Id == Entity.ChecKListCatId).Name;
                db.GarageInfoes.Add(Entity);
                if (ServicedItems != null && ServicedItems.Count() > 0)
                    foreach (var item in ServicedItems)
                        db.GarageSeviceChecks.Add(new GarageSeviceCheck() { ServiceCheckId = item, Id = Guid.NewGuid(), GarageInfoId = Entity.Id, ServiceId = Entity.ChecKListCatId.Value, IssueDate = DateTime.Now });
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsAccesoryAdded(Equipment2Fleet Entity)
        {
            using (var db = new SCMSEntities())
            {
                clearSession();
                db.Equipment2Fleet.Add(Entity);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsFleetDetailsEdited(FleetDetail entity)
        {
            using (var db = new SCMSEntities())
            {
                clearSession();
                db.FleetDetails.Attach(entity);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsGarageInfoEdited(GarageInfo entity)
        {
            using (var db = new SCMSEntities())
            {
                clearSession();
                var existing = db.GarageInfoes.FirstOrDefault(p => p.Id == entity.Id);
                db.Entry(existing).CurrentValues.SetValues(entity);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsDailyFleetEdited(FleetDailyManagement entity)
        {
            using (var db = new SCMSEntities())
            {
                clearSession();
                db.FleetDailyManagements.Attach(entity);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsFleetDeleted(Guid fleetId)
        {
            using (var db = new SCMSEntities())
            {
                clearSession();
                db.FleetDetails.Remove(db.FleetDetails.FirstOrDefault(p => p.Id == fleetId));
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsGarageInfoDeleted(Guid garageInfoId)
        {
            using (var db = new SCMSEntities())
            {
                clearSession();
                db.GarageInfoes.Remove(db.GarageInfoes.FirstOrDefault(p => p.Id == garageInfoId));
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsDailyFleetInfoDeleted(Guid dailyFleetInfoId)
        {
            using (var db = new SCMSEntities())
            {
                clearSession();
                db.FleetDailyManagements.Remove(db.FleetDailyManagements.FirstOrDefault(p => p.Id == dailyFleetInfoId));
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsAccesoryRemoved(Guid accesoryId)
        {
            using (var db = new SCMSEntities())
            {
                clearSession();
                db.Equipment2Fleet.Remove(db.Equipment2Fleet.FirstOrDefault(p => p.Id == accesoryId));
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        private void clearSession()
        {
            SessionData.CurrentSession.FleetDetailsList = null;
        }

        public List<Model.FleetDetail> GetFleetDetails()
        {
            return SessionData.CurrentSession.FleetDetailsList.OrderByDescending(p => p.IssueDate).ToList();
        }

        public Model.FleetDetail GetGenFleetDetailsById(Guid fleetId)
        {
            return SessionData.CurrentSession.FleetDetailsList.FirstOrDefault(p => p.Id == fleetId);
        }

        public float GetAverageFuelConsumed(Guid fleetId)
        {
            using (var db = new SCMSEntities())
            {
                float totalFuel = 0, totalDistance = 0, monthtotalDistance = 0;
                if (db.FleetDailyManagements.Where(p => p.FleetId == fleetId).Count() > 0)
                    totalDistance = (float)db.FleetDailyManagements.Where(p => p.FleetId == fleetId).Sum(p => p.EndMilleage);
                if (db.FleetDailyManagements.Where(p => p.FleetId == fleetId && p.Date.Value.Month == DateTime.Now.Month).Count() > 0)
                {
                    totalFuel = (float)db.FleetDailyManagements.Where(p => p.FleetId == fleetId && p.Date.Value.Month == DateTime.Now.Month).Sum(p => p.TotalFuelConsumed);
                    monthtotalDistance = (float)db.FleetDailyManagements.Where(p => p.FleetId == fleetId && p.Date.Value.Month == DateTime.Now.Month).Sum(p => p.EndMilleage);
                    return (float)Math.Round((totalDistance - monthtotalDistance) / totalFuel, 2);
                }
                else return 0;
            }
        }

        public Model.GarageInfo GetGarageInfoById(Guid garageInfoId)
        {
            using (var db = new SCMSEntities())
            {
                return db.GarageInfoes.FirstOrDefault(p => p.Id == garageInfoId);
            }
        }

        public Model.FleetDailyManagement GetDailyFleetById(Guid dailyFleetId)
        {
            using (var db = new SCMSEntities())
            {
                return db.FleetDailyManagements.FirstOrDefault(p => p.Id == dailyFleetId);
            }
        }

        public Model.FleetDailyManagement GetLastDailyFleetRecord(Guid fleetId)
        {
            using (var db = new SCMSEntities())
            {
                return db.FleetDailyManagements.Where(p => p.FleetId == fleetId).OrderByDescending(p => p.IssueDate).FirstOrDefault();
            }
        }

        public Model.FleetDailyManagement GetLastDailyFleetRecord(FleetDailyManagement entity)
        {
            using (var db = new SCMSEntities())
            {
                return db.FleetDailyManagements.Where(p => p.IssueDate < entity.IssueDate).OrderByDescending(p => p.IssueDate).FirstOrDefault();
            }
        }

        public List<FleetEquipment> GetFleetAccesories()
        {
            return SessionData.CurrentSession.FleetEquipmentList.ToList();
        }

        public List<Model.FleetMajorIncidence> GetFleetIncidences()
        {
            return SessionData.CurrentSession.FleetMajorIncidenceList.ToList();
        }

        public Model.CheckListCategory GetCheckListCatById(Guid catId)
        {
            return SessionData.CurrentSession.CheckListCategoryList.FirstOrDefault(p => p.Id == catId);
        }

        public Model.CheckListCategory GetCheckListCatByGarageInfo(Model.GarageInfo Entity)
        {
            float serviceCatgory = (float)(Entity.IfServiceNextMilleage - Entity.IfServiceCurrentMilleage);
            return SessionData.CurrentSession.CheckListCategoryList.FirstOrDefault(p => p.Name == serviceCatgory);
        }

        public List<Model.CheckListCategory> GetCheckListCategory()
        {
            return SessionData.CurrentSession.CheckListCategoryList.ToList();
        }

        public List<Model.ServiceCheckList> GetServiceCheckList(Guid serviceCatId)
        {
            return SessionData.CurrentSession.ServiceCheckListList.Where(p => p.ServiceCategoryId == serviceCatId).ToList();
        }

        public List<VehicleMake> GetVehicleMake()
        {
            return SessionData.CurrentSession.VehicleMakeList.ToList();
        }

        public List<Model.InsuranceType> GetInsuranceTypes()
        {
            return SessionData.CurrentSession.InsuranceTypeList.ToList();
        }

        public List<Model.DutyType> GetDutyTypes()
        {
            return SessionData.CurrentSession.DutyTypeList.ToList();
        }

        public List<Model.VehicleModel> GetVehicleModel(Guid mkId)
        {
            return SessionData.CurrentSession.VehicleModelList.Where(p => p.MakeId == mkId).ToList();
        }

    }

}
