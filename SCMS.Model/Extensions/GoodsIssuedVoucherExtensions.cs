using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class GoodsIssuedVoucherExtensions
    {
        public static IQueryable<GoodsIssuedVoucher> IncludeGoodsIssuedVoucherItems(this IQueryable<GoodsIssuedVoucher> queryable)
        {
            return ((DbQuery<GoodsIssuedVoucher>)queryable).Include("GoodsIssuedVoucherItems.WarehouseReleaseItem.WarehouseRelease")
                                                            .Include("GoodsIssuedVoucherItems.WarehouseReleaseItem.Inventory.Item.UnitOfMeasure");
        }

        public static IQueryable<GoodsIssuedVoucher> IncludeWarehouseRelease(this IQueryable<GoodsIssuedVoucher> queryable)
        {
            return ((DbQuery<GoodsIssuedVoucher>)queryable).Include("WarehouseRelease.WareHouse");
        }

        public static IQueryable<GoodsIssuedVoucher> IncludeStaff(this IQueryable<GoodsIssuedVoucher> queryable)
        {
            return ((DbQuery<GoodsIssuedVoucher>)queryable).Include("Staff.Person").Include("Staff.Designation")
                                                        .Include("Staff1.Person").Include("Staff1.Designation");
        }

    }
}
