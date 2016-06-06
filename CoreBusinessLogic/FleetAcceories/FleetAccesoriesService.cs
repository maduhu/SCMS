using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.FleetAcceories
{
    public class FleetAccesoriesService : IFleetAccesoriesService
    {
        public bool IsFleetAccesorySaved(FleetEquipment entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.FleetEquipmentList = null;
                db.FleetEquipments.Add(entity);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsFleetAccesoryEdited(FleetEquipment entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.FleetEquipmentList = null;
                db.FleetEquipments.Attach(entity);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public FleetEquipment GetFleetAccesoryById(Guid accesoryId)
        {
            return SessionData.CurrentSession.FleetEquipmentList.FirstOrDefault(p => p.Id == accesoryId);
        }

        public List<FleetEquipment> GetFleetAccesories(Guid cpId)
        {
            return SessionData.CurrentSession.FleetEquipmentList.ToList();
        }

        public bool IsFleetAccesoryDeleted(Guid accesoryId)
        {
            using (var db = new SCMSEntities())
            {
                var fe = new FleetEquipment { Id = accesoryId };
                db.FleetEquipments.Attach(fe);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(fe, System.Data.EntityState.Deleted);
                return db.SaveChanges() > 0 ? true : false;
            }
        }
    }
}
