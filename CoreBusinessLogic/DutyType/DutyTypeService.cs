using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.DutyType
{
    public class DutyTypeService : IDutyTypeService
    {
        public bool IsDutyTypeSaved(Model.DutyType entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.DutyTypeList = null;
                db.DutyTypes.Add(entity);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsDutyTypeEdited(Model.DutyType entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.DutyTypeList = null;
                db.DutyTypes.Attach(entity);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public Model.DutyType GetDutyTypeById(Guid dutyId)
        {
            return SessionData.CurrentSession.DutyTypeList.FirstOrDefault(p => p.Id == dutyId);
        }

        public List<Model.DutyType> GetDutyTypes(Guid cpId)
        {
            return SessionData.CurrentSession.DutyTypeList.ToList();
        }

        public bool IsDutyTypeDeleted(Guid dutyId)
        {
            using (var db = new SCMSEntities())
            {
                var dt = SessionData.CurrentSession.DutyTypeList.FirstOrDefault(p => p.Id == dutyId);
                db.DutyTypes.Remove(dt);
                SessionData.CurrentSession.DutyTypeList = null;
                return db.SaveChanges() > 0 ? true : false;
            }
        }
    }
}
