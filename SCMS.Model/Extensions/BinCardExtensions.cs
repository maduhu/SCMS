using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class BinCardExtensions
    {
        public static IQueryable<Bin> IncludeBinItems(this IQueryable<Bin> queryable)
        {
            return ((DbQuery<Bin>)queryable).Include("BinItems.GoodsReceivedNoteItem.GoodsReceivedNote");
        }

        public static IQueryable<Bin> IncludeOrderRequest(this IQueryable<Bin> queryable)
        {
            return ((DbQuery<Bin>)queryable).Include("OrderRequestItem.OrderRequest.ProjectDonor");
        }

        public static IQueryable<Bin> IncludeProcurementPlan(this IQueryable<Bin> queryable)
        {
            return ((DbQuery<Bin>)queryable).Include("ProcurementPlanItem.ProcurementPlan.ProjectDonor")
                                            .Include("ProcurementPlanItem.Item.UnitOfMeasure");
        }

        public static IQueryable<Bin> IncludeBinItemWHRelease(this IQueryable<Bin> queryable)
        {
            return ((DbQuery<Bin>)queryable).Include("BinItems.WarehouseReleaseItem.WarehouseRelease");
        }

        public static IQueryable<Bin> IncludeWareHouse(this IQueryable<Bin> queryable)
        {
            return ((DbQuery<Bin>)queryable).Include("WareHouse");
        }

        public static IQueryable<Bin> IncludeStaff(this IQueryable<Bin> queryable)
        {
            return ((DbQuery<Bin>)queryable).Include("Staff.Person").Include("Staff.Designation");
        }

        public static IQueryable<Bin> IncludeItemPackage(this IQueryable<Bin> queryable)
        {
            return ((DbQuery<Bin>)queryable).Include("ItemPackage");
        }

        public static IQueryable<Bin> IncludeORItem(this IQueryable<Bin> queryable)
        {
            return ((DbQuery<Bin>)queryable).Include("OrderRequestItem.Item.UnitOfMeasure");
        }

        #region Bin Items

        public static IQueryable<BinItem> IncludeGRNItemAndGRN(this IQueryable<BinItem> queryable)
        {
            return ((DbQuery<BinItem>)queryable).Include("GoodsReceivedNoteItem.GoodsReceivedNote");
        }

        public static IQueryable<BinItem> IncludeBin(this IQueryable<BinItem> queryable)
        {
            return ((DbQuery<BinItem>)queryable).Include("Bin.OrderRequestItem.Item.UnitOfMeasure")
                                                .Include("Bin.OrderRequestItem.OrderRequest.ProjectDonor");
        }

        public static IQueryable<BinItem> IncludeBinPP(this IQueryable<BinItem> queryable)
        {
            return ((DbQuery<BinItem>)queryable).Include("Bin.ProcurementPlanItem.Item.UnitOfMeasure")
                                                .Include("Bin.ProcurementPlanItem.ProcurementPlan.ProjectDonor");
        }

        public static IQueryable<BinItem> IncludeWareHouseRelease(this IQueryable<BinItem> queryable)
        {
            return ((DbQuery<BinItem>)queryable).Include("WarehouseReleaseItem.WarehouseRelease");
        } 

        #endregion
    }
}
