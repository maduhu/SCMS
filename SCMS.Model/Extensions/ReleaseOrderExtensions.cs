using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class ReleaseOrderExtensions
    {
        public static IQueryable<WarehouseRelease> IncludeReleaseOrderItems(this IQueryable<WarehouseRelease> queryable)
        {
            return ((DbQuery<WarehouseRelease>)queryable).Include("WarehouseReleaseItems.Asset")
                                                        .Include("WarehouseReleaseItems.GoodsIssuedVoucherItems")
                                                        .Include("WarehouseReleaseItems.Inventory.Item.ItemCategory")
                                                        .Include("WarehouseReleaseItems.Inventory.Item.UnitOfMeasure");
        }

        public static IQueryable<WarehouseRelease> IncludeStaff(this IQueryable<WarehouseRelease> queryable)
        {
            return ((DbQuery<WarehouseRelease>)queryable).Include("Staff.Person").Include("Staff.Designation").Include("Staff.CountrySubOffice")
                                                        .Include("Staff1.Person").Include("Staff1.Designation").Include("Staff1.CountrySubOffice")
                                                        .Include("Staff2.Person").Include("Staff2.Designation").Include("Staff2.CountrySubOffice");
                                                        
        }

        public static IQueryable<WarehouseRelease> IncludeWareHouse(this IQueryable<WarehouseRelease> queryable)
        {
            return ((DbQuery<WarehouseRelease>)queryable).Include("WareHouse");
        }
    }

    public static class WarehouseReleaseItemExtensions
    {
        public static IQueryable<WarehouseReleaseItem> IncludeReleaseOrderItemsDetails(this IQueryable<WarehouseReleaseItem> queryable)
        {
            return ((DbQuery<WarehouseReleaseItem>)queryable).Include("Asset")
                                                        .Include("GoodsIssuedVoucherItems")
                                                        .Include("Inventory.Item.ItemCategory")
                                                        .Include("Inventory.Item.UnitOfMeasure");
        }

        public static IQueryable<WarehouseReleaseItem> IncludeWarehouseRelease(this IQueryable<WarehouseReleaseItem> queryable)
        {
            return ((DbQuery<WarehouseReleaseItem>)queryable).Include("WarehouseRelease");
        }

        public static IQueryable<WarehouseReleaseItem> IncludeBinItems(this IQueryable<WarehouseReleaseItem> queryable)
        {
            return ((DbQuery<WarehouseReleaseItem>)queryable).Include("BinItems");
        }
    }
}
