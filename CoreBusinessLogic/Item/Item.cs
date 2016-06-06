using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using System.Data.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.CoreBusinessLogic._Item
{
    public class ItemService : IItemService
    {
        private _ItemCategory.ItemCategoryService itemCatObj;
        private _ItemClassification.ItemClassificationService itemClassObj;

        public ItemService()
        {
            itemCatObj = new _ItemCategory.ItemCategoryService();
            itemClassObj = new _ItemClassification.ItemClassificationService();
        }

        private static void ClearItemSessionData()
        {
            SessionData.CurrentSession.ItemList = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Item GetItem(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return dbContext.Items.Where(c => c.Id.Equals(id)).FirstOrDefault<Item>();
            }
        }

        public Item GetItemByName(string name, Guid countryProgId)
        {
            using (var context = new SCMSEntities())
            {
                return context.Items.FirstOrDefault(i => i.CountryProgrammeId == countryProgId && i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

        public ItemView GetItemView(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                var item = dbContext.Items.Where(c => c.Id.Equals(id)).FirstOrDefault<Item>();

                return new ItemView
                {
                    itemCategory = item.ItemCategory,
                    itemClassification = item.ItemClassification,
                    item = item
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<Item> GetItems(Guid Cpid, string search = null)
        {
            using (var dbContext = new SCMSEntities())
            {
                var itemList = from item in dbContext.Items
                               where item.CountryProgrammeId == Cpid
                               select item;
                List<Item> its = dbContext.Items.Where(p => p.CountryProgrammeId == Cpid).ToList();
                foreach (var item in its)
                {
                    ItemCategory ic = item.ItemCategory;
                    ItemClassification ics = item.ItemClassification;
                    ICollection<ItemPackage> pp = item.ItemPackages;
                }
                if (!string.IsNullOrEmpty(search))
                {
                    itemList = from item in itemList
                               where item.Name.Contains(search)
                               orderby item.Name
                               select item;
                }
                return its;// itemList.ToList<Item>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<ItemView> GetItems1(Guid cpid, string search = null)
        {
            using (var dbContext = new SCMSEntities())
            {
                var itemList = from item in dbContext.Items
                               where item.CountryProgrammeId == cpid
                               select item;

                if (!string.IsNullOrEmpty(search))
                {
                    itemList = from item in itemList
                               where item.Name.Contains(search)
                               select item;
                }

                return (from item in itemList
                        select new ItemView
                                  {
                                      itemCategory = item.ItemCategory,
                                      itemClassification = item.ItemClassification,
                                      item = item
                                  }
                        ).ToList();
            }
        }


        public List<ItemView> SearchBySerial(Guid Cpid, string search = null)
        {
            using (var dbContext = new SCMSEntities())
            {
                var itemList = from item in dbContext.Items
                               where item.CountryProgrammeId == Cpid
                               select item;

                if (!string.IsNullOrEmpty(search))
                {
                    itemList = from item in itemList
                               where item.SerialNumber.Contains(search)
                               select item;
                }

                return (from item in itemList
                        select new ItemView
                        {
                            itemCategory = item.ItemCategory,
                            itemClassification = item.ItemClassification,
                            item = item
                        }
                        ).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddItem(Item item)
        {
            bool isSaved = false;

            if (item.Id.Equals(Guid.Empty))
            {
                item.Id = Guid.NewGuid();
            }

            using (var context = new SCMSEntities())
            {
                UnitOfMeasure u = context.UnitOfMeasures.FirstOrDefault(p => p.Id == item.UnitOfMessureId);
                context.Items.Add(item);
                context.ItemPackages.Add(new ItemPackage() { Id = Guid.NewGuid(), ItemId = item.Id, PackSize = 1, shortName = u.Code, Name = u.Description });
                if (context.SaveChanges() > 0)
                {
                    isSaved = true;
                    ClearItemSessionData();
                }
            }
            return isSaved;
        }

        public bool AddPackage(ItemPackage pack)
        {
            using (var db = new SCMSEntities())
            {
                db.ItemPackages.Add(pack);
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool EditItem(Item item)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.Items.Attach(item);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(item, System.Data.EntityState.Modified);

                if (context.SaveChanges() > 0)
                {
                    isSaved = true;
                    ClearItemSessionData();
                }
            }
            return isSaved;
        }

        public bool EditPackage(ItemPackage pack)
        {
            using (var context = new SCMSEntities())
            {
                context.ItemPackages.Attach(pack);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(pack, System.Data.EntityState.Modified);
                return context.SaveChanges() > 0 ? true : false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteItem(Guid id)
        {
            bool isDeleted = false;
            using (var dbContext = new SCMSEntities())
            {
                Item item = dbContext.Items.FirstOrDefault(c => c.Id.Equals(id));
                dbContext.Items.Remove(item);
                foreach (ItemPackage pack in dbContext.ItemPackages.Where(p => p.ItemId == id))
                    dbContext.ItemPackages.Remove(pack);
                if (dbContext.SaveChanges() > 0)
                {
                    isDeleted = true;
                    ClearItemSessionData();
                }
            }
            return isDeleted;
        }

        public bool DeletePack(Guid pkgId)
        {
            using (var db = new SCMSEntities())
            {
                db.ItemPackages.Remove(db.ItemPackages.FirstOrDefault(p => p.Id == pkgId));
                return db.SaveChanges() > 0 ? true : false;
            }
        }

        public _ItemCategory.ItemCategoryService ItemCatObj
        {
            get
            {
                return itemCatObj;
            }
        }

        public _ItemClassification.ItemClassificationService ItemClassObj
        {
            get
            {
                return itemClassObj;
            }
        }

        public List<ItemPackage> GetItemPackages(Guid itemId)
        {
            using (var db = new SCMSEntities())
            {
                return db.ItemPackages.Where(p => p.ItemId == itemId).ToList();
            }
        }
    }


    public class ItemView
    {
        /// <summary>
        /// ItemCategory Entity
        /// </summary>
        public ItemCategory itemCategory;

        /// <summary>
        /// ItemClassification Entity
        /// </summary>
        public ItemClassification itemClassification;

        /// <summary>
        /// Item Entity
        /// </summary>
        public Item item;
    }

}
