using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._ItemCategory
{
    public class ItemCategoryService : IItemCategoryService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemCategory GetItemCategory(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return dbContext.ItemCategories.Where(c => c.Id.Equals(id)).FirstOrDefault<ItemCategory>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<ItemCategory> GetItemCategories(string search = null)
        {
            List<ItemCategory> itemCategoryList = new List<ItemCategory>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    itemCategoryList = dbContext.ItemCategories.ToList<ItemCategory>();
                }
                else
                {
                    itemCategoryList = dbContext.ItemCategories
                        .Where(c => c.CategoryCode.Contains(search) || c.CategoryName.Contains(search))
                        .OrderBy(c => c.CategoryCode).ToList<ItemCategory>();
                }
            }
            return itemCategoryList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemCategory"></param>
        /// <returns></returns>
        public bool AddItemCategory(ItemCategory itemCategory)
        {
            bool isSaved = false;

            if (itemCategory.Id.Equals(Guid.Empty))
            {
                itemCategory.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.ItemCategories.Add(itemCategory);
                if(dbContext.SaveChanges() > 0)
                {
                    isSaved = true;
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemCategory"></param>
        /// <returns></returns>
        public bool EditItemCategory(ItemCategory itemCategory)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.ItemCategories.Attach(itemCategory);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(itemCategory, System.Data.EntityState.Modified);
                
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
        /// <param name="currency"></param>
        /// <returns></returns>
        public bool DeleteItemCategory(Guid id)
        {
            bool isDeleted = false;
            using (var dbContext = new SCMSEntities())
            {
                ItemCategory itemCategory = dbContext.ItemCategories.Single(c => c.Id.Equals(id));
                dbContext.ItemCategories.Remove(itemCategory);
                if (dbContext.SaveChanges() > 0)
                {
                    isDeleted = true;
                }
            }
            return isDeleted;
        }
    }
}
