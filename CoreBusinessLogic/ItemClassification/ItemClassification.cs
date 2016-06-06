using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._ItemClassification
{
    public class ItemClassificationService : IItemClassificationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemClassification GetItemClassification(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return dbContext.ItemClassifications.Where(c => c.Id.Equals(id)).FirstOrDefault<ItemClassification>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<ItemClassification> GetItemClassifications(Guid cpId, string search = null)
        {
            List<ItemClassification> itemClassificationList = new List<ItemClassification>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    itemClassificationList = dbContext.ItemClassifications.Where(i => i.CountryProgrammeId == cpId).ToList<ItemClassification>();
                }
                else
                {
                    itemClassificationList = dbContext.ItemClassifications
                        .Where(c => c.CountryProgrammeId == cpId && (c.Name.Contains(search) || c.Description.Contains(search)))
                        .OrderBy(c => c.Name).ToList<ItemClassification>();
                }
            }
            return itemClassificationList;
        }


        public List<UnitOfMeasure> GetUnitOfMessures(Guid CPId)
        {

            using (var dbContext = new SCMSEntities())
            {
                return dbContext.UnitOfMeasures.Where(p => p.CountryProgrammeId == CPId).ToList<UnitOfMeasure>();

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemClassification"></param>
        /// <returns></returns>
        public bool AddItemClassification(ItemClassification itemClassification)
        {
            bool isSaved = false;

            if (itemClassification.Id.Equals(Guid.Empty))
            {
                itemClassification.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.ItemClassifications.Add(itemClassification);
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
        /// <param name="itemClassification"></param>
        /// <returns></returns>
        public bool EditItemClassification(ItemClassification itemClassification)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.ItemClassifications.Attach(itemClassification);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(itemClassification, System.Data.EntityState.Modified);

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
        public bool DeleteItemClassification(Guid id)
        {
            bool isDeleted = false;
            using (var dbContext = new SCMSEntities())
            {
                ItemClassification itemClassification = dbContext.ItemClassifications.Single(c => c.Id.Equals(id));
                dbContext.ItemClassifications.Remove(itemClassification);
                if (dbContext.SaveChanges() > 0)
                {
                    isDeleted = true;
                }
            }
            return isDeleted;
        }
    }
}
