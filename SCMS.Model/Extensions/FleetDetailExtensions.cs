using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class FleetDetailExtensions
    {
        public static IQueryable<FleetDetail> IncludeGarageInfoes(this IQueryable<FleetDetail> queryable)
        {
            return ((DbQuery<FleetDetail>)queryable).Include("GarageInfoes.Currency")
                                                    .Include("GarageInfoes.Supplier");
        }

        public static IQueryable<FleetDetail> IncludeEquipment2Fleet(this IQueryable<FleetDetail> queryable)
        {
            return ((DbQuery<FleetDetail>)queryable).Include("Equipment2Fleet.FleetEquipment");
        }

        public static IQueryable<FleetDetail> IncludeFleetDailyManagements(this IQueryable<FleetDetail> queryable)
        {
            return ((DbQuery<FleetDetail>)queryable).Include("FleetDailyManagements.FleetMajorIncidence");
        }

        public static IQueryable<FleetDetail> IncludeFleetDetails(this IQueryable<FleetDetail> queryable)
        {
            return ((DbQuery<FleetDetail>)queryable).Include("VehicleModel.VehicleMake")
                                                    .Include("InsuranceType")
                                                    .Include("Asset")
                                                    .Include("DutyType");
        }

        public static IQueryable<FleetDetail> IncludeStaff(this IQueryable<FleetDetail> queryable)
        {
            return ((DbQuery<FleetDetail>)queryable).Include("Staff.Person")
                                                    .Include("Staff.Designation");
        }

    }
}
