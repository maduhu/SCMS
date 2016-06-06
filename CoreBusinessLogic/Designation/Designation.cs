using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._Designation
{
    public class DesignationService : IDesignationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Designation GetDesignation(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return dbContext.Designations.Where(c => c.Id.Equals(id)).FirstOrDefault<Designation>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<Designation> GetDesignations(Guid countryProgId, string search = null)
        {
            List<Designation> designationList = new List<Designation>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    designationList = dbContext.Designations.Where(d => d.CountryProgrammeId == countryProgId).OrderBy(d => d.Name).ToList<Designation>();
                }
                else
                {
                    designationList = dbContext.Designations
                        .Where(c => (countryProgId == null || c.CountryProgrammeId == countryProgId) && (c.Name.Contains(search) || c.ShortName.Contains(search) || c.Description.Contains(search)))
                        .OrderBy(c => c.Name).ToList<Designation>();
                }
            }
            return designationList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="designation"></param>
        /// <returns></returns>
        public bool AddDesignation(Designation designation)
        {
            bool isSaved = false;

            if (designation.Id.Equals(Guid.Empty))
            {
                designation.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.Designations.Add(designation);
                if (dbContext.SaveChanges() > 0)
                {
                    isSaved = true;
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="designation"></param>
        /// <returns></returns>
        public bool EditDesignation(Designation designation)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.Designations.Attach(designation);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(designation, System.Data.EntityState.Modified);

                if (context.SaveChanges() > 0)
                {
                    isSaved = true;
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteDesignation(Guid id)
        {
            bool isDeleted = false;
            using (var dbContext = new SCMSEntities())
            {
                Designation designation = dbContext.Designations.SingleOrDefault(c => c.Id.Equals(id));
                if (designation != null)
                {
                    dbContext.Designations.Remove(designation);
                    if (dbContext.SaveChanges() > 0)
                    {
                        isDeleted = true;
                    }
                }
            }
            return isDeleted;
        }
    }
}
