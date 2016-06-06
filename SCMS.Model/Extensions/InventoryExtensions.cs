using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class InventoryExtensions
    {
        public static IQueryable<Inventory> IncludeItem(this IQueryable<Inventory> queryable)
        {
            return ((DbQuery<Inventory>)queryable).Include("Item");
        }

        public static IQueryable<Inventory> IncludeWareHouse(this IQueryable<Inventory> queryable)
        {
            return ((DbQuery<Inventory>)queryable).Include("WareHouse");
        }

        public static IQueryable<Inventory> IncludeItemClassification(this IQueryable<Inventory> queryable)
        {
            return ((DbQuery<Inventory>)queryable).Include("Item.ItemClassification");
        }

        public static IQueryable<Inventory> IncludeItemCategory(this IQueryable<Inventory> queryable)
        {
            return ((DbQuery<Inventory>)queryable).Include("Item.ItemCategory");
        }

        public static IQueryable<Inventory> IncludeUnitOfMeasure(this IQueryable<Inventory> queryable)
        {
            return ((DbQuery<Inventory>)queryable).Include("Item.UnitOfMeasure");
        }
    }
}
