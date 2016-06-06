using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.FleetIncidences
{
    public class FleetIncidenceService : IFleetIncidenceService
    {
        public bool IsIncidenceSaved(FleetMajorIncidence entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.FleetMajorIncidenceList = null;
                db.FleetMajorIncidences.Add(entity);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsFleetIncidenceEdited(FleetMajorIncidence entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.FleetMajorIncidenceList = null;
                db.FleetMajorIncidences.Attach(entity);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public FleetMajorIncidence GetFleetIncidenceById(Guid incidenceId)
        {
            return SessionData.CurrentSession.FleetMajorIncidenceList.FirstOrDefault(p => p.Id == incidenceId);
        }

        public List<FleetMajorIncidence> GetFleetIncidencs(Guid cpId)
        {
            return SessionData.CurrentSession.FleetMajorIncidenceList.ToList();
        }

        public bool IsFleetIncidenceDeleted(Guid incidenceId)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.FleetMajorIncidenceList = null;
                var incidence = new FleetMajorIncidence() { Id = incidenceId };
                db.FleetMajorIncidences.Attach(incidence);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(incidence, System.Data.EntityState.Deleted);
                return db.SaveChanges() > 0 ? true : false;
            }
        }
    }
}
