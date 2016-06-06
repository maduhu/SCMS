using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class AssetExtensions
    {

        public static IQueryable<Asset> IncludeAssetManagmentsProjectDonor(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("AssetManagments.ProjectDonor");
        }

        public static IQueryable<Asset> IncludeDonor(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("ProjectDonor.Donor");
        }

        public static IQueryable<Asset> IncludeProject(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("ProjectDonor.Project");
        }

        public static IQueryable<Asset> IncludeAssetManagments(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("AssetManagments");
        }

        public static IQueryable<Asset> IncludeCurrency(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("Currency");
        }

        public static IQueryable<Asset> IncludeWareHouse(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("WareHouse");
        }

        //public static IQueryable<Asset> IncludeStaff(this IQueryable<Asset> queryable)
        //{
        //    return ((DbQuery<Asset>)queryable).Include("Staff");
        //}

        public static IQueryable<Asset> IncludePerson(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("Staff.Person");
        }

        public static IQueryable<Asset> IncludeItemClassification(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("Item.ItemClassification");
        }

        public static IQueryable<Asset> IncludeProject1(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("ProjectDonor1.Project");
        }

        public static IQueryable<Asset> IncludeDonor1(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("ProjectDonor1.Donor");
        }

        public static IQueryable<Asset> IncludeDepreciation(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("Depreciations");
        }

        public static IQueryable<Asset> IncludeFleetDetails(this IQueryable<Asset> queryable)
        {
            return ((DbQuery<Asset>)queryable).Include("FleetDetails");
        }

    }
}
