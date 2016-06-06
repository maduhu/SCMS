using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.InsuranceType
{
    public class InsuranceTypeService : IInsuranceTypeService
    {
        public bool IsInsuranceTypeSaved(Model.InsuranceType entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.InsuranceTypeList = null;
                db.InsuranceTypes.Add(entity);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public bool IsInsuranceTypeEdited(Model.InsuranceType entity)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.InsuranceTypeList = null;
                db.InsuranceTypes.Attach(entity);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public Model.InsuranceType GetInsuranceTypeById(Guid insuranceId)
        {
            return SessionData.CurrentSession.InsuranceTypeList.FirstOrDefault(p => p.Id == insuranceId);
        }

        public List<Model.InsuranceType> GetInsuranceTypes(Guid cpId)
        {
            return SessionData.CurrentSession.InsuranceTypeList.ToList();
        }

        public bool IsInsuranceTypeDeleted(Guid makeId)
        {
            using (var db = new SCMSEntities())
            {
                SessionData.CurrentSession.InsuranceTypeList = null;
                var insuretype = new Model.InsuranceType { Id = makeId };
                db.InsuranceTypes.Attach(insuretype);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(insuretype, System.Data.EntityState.Deleted);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

    }
}
