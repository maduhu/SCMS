using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class ItemExtensions
    {
        public static IQueryable<Item> IncludeItemCatetory(this IQueryable<Item> queryable)
        {
            return ((DbQuery<Item>)queryable).Include("ItemCategory");
        }

        public static IQueryable<Item> IncludeUnitOfMeasure(this IQueryable<Item> queryable)
        {
            return ((DbQuery<Item>)queryable).Include("UnitOfMeasure");
        }

        public static IQueryable<Item> IncludeItemClassification(this IQueryable<Item> queryable)
        {
            return ((DbQuery<Item>)queryable).Include("ItemClassification");
        }
    }
}
