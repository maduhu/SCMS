using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._UnitOfMeasure
{
    public class UnitOfMeasureService : IUnitOfMeasureService
    {
        private static void ClearUnitOfMeasureSessionData()
        {
            SessionData.CurrentSession.UnitOfMeasureList = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UnitOfMeasure GetUnitOfMeasure(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return dbContext.UnitOfMeasures.Where(c => c.Id.Equals(id)).FirstOrDefault<UnitOfMeasure>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<UnitOfMeasure> GetUnitsOfMeasure(Guid cpId, string search = null)
        {
            List<UnitOfMeasure> unitOfMeasureList = new List<UnitOfMeasure>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    unitOfMeasureList = dbContext.UnitOfMeasures.Where(u => u.CountryProgrammeId == cpId).ToList<UnitOfMeasure>();
                }
                else
                {
                    unitOfMeasureList = dbContext.UnitOfMeasures
                        .Where(c => c.CountryProgrammeId == cpId && (c.Code.Contains(search) || c.Description.Contains(search)))
                        .OrderBy(c => c.Code).ToList<UnitOfMeasure>();
                }
            }
            return unitOfMeasureList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public bool AddUnitOfMeasure(UnitOfMeasure unitOfMeasure)
        {
            bool isSaved = false;

            if (unitOfMeasure.Id.Equals(Guid.Empty))
            {
                unitOfMeasure.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.UnitOfMeasures.Add(unitOfMeasure);
                if(dbContext.SaveChanges() > 0)
                {
                    isSaved = true;
                    ClearUnitOfMeasureSessionData();
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public bool EditUnitOfMeasure(UnitOfMeasure unitOfMeasure)
        {
            bool isSaved = false; 
            using (var context = new SCMSEntities())
            {
                context.UnitOfMeasures.Attach(unitOfMeasure);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(unitOfMeasure, System.Data.EntityState.Modified);
                
                if (context.SaveChanges() > 0)
                {
                    isSaved = true;
                    ClearUnitOfMeasureSessionData();
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public bool DeleteUnitOfMeasure(Guid id)
        {
            bool isDeleted = false;
            using (var dbContext = new SCMSEntities())
            {
                UnitOfMeasure unitOfMeasure = dbContext.UnitOfMeasures.Single(c => c.Id.Equals(id));
                dbContext.UnitOfMeasures.Remove(unitOfMeasure);
                if (dbContext.SaveChanges() > 0)
                {
                    isDeleted = true;
                    ClearUnitOfMeasureSessionData();
                }
            }
            return isDeleted;
        }
    }
}
