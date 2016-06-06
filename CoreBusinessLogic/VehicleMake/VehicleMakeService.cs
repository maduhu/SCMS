using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using System.Data.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.CoreBusinessLogic
{
    public class VehicleMakeService : IVehicleMakeService
    {
        public bool IsVMakeSaved(VehicleMake entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.VehicleMakeList = null;
                db.VehicleMakes.Add(entity);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsMakeEdited(VehicleMake entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.VehicleMakeList = null;
                db.VehicleMakes.Attach(entity);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public VehicleMake GetMakeById(Guid makeId)
        {
            return SessionData.CurrentSession.VehicleMakeList.FirstOrDefault(p => p.Id == makeId);
        }

        public List<VehicleMake> GetVMakes(Guid cpId)
        {
            return SessionData.CurrentSession.VehicleMakeList.ToList();
        }

        public bool IsVMakeDeleted(Guid makeId)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.VehicleMakeList = null;
                db.VehicleMakes.Remove(db.VehicleMakes.FirstOrDefault(p => p.Id == makeId));
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsVModelSaved(Model.VehicleModel entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.VehicleModelList = null;
                db.VehicleModels.Add(entity);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsModelEdited(Model.VehicleModel entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.VehicleModelList = null;
                db.VehicleModels.Attach(entity);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public Model.VehicleModel GetModelById(Guid modelId)
        {
            return SessionData.CurrentSession.VehicleModelList.FirstOrDefault(p => p.Id == modelId);
        }

        public bool IsVModelDeleted(Guid modelId)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.VehicleModelList = null;
                db.VehicleModels.Remove(db.VehicleModels.FirstOrDefault(p => p.Id == modelId));
                return db.SaveChanges() > 0 ? true : false;
            }
        }
    }
}
