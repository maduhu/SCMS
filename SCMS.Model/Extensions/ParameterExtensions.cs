using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{

    public static class ParameterExtensions
    {
        public static IQueryable<WareHouse> IncludeProcurementChain(this IQueryable<WareHouse> queryable)
        {
            return ((DbQuery<WareHouse>)queryable).Include("GoodsReceivedNotes.GoodsReceivedNoteItems.PurchaseOrderItem.PurchaseOrder.OrderRequest")
                                                  .Include("GoodsReceivedNotes.GoodsReceivedNoteItems.PurchaseOrderItem.ProcurementPlanItem.ProcurementPlan");

        }

        public static IQueryable<WareHouse> IncludeLocation(this IQueryable<WareHouse> queryable)
        {
            return ((DbQuery<WareHouse>)queryable).Include("Location.Country")
                                                  .Include("CountrySubOffice");
        }
        
        public static IQueryable<VehicleMake> IncludeVehicleModel(this IQueryable<VehicleMake> queryable)
        {
            return ((DbQuery<VehicleMake>)queryable).Include("VehicleModels");
        }

        public static IQueryable<VehicleModel> IncludeVehicleMake(this IQueryable<VehicleModel> queryable)
        {
            return ((DbQuery<VehicleModel>)queryable).Include("VehicleMake");
        }

        public static IQueryable<ServiceCheckList> IncludeCheckListCategory(this IQueryable<ServiceCheckList> queryable)
        {
            return ((DbQuery<ServiceCheckList>)queryable).Include("CheckListCategory");
        }


        public static IQueryable<CheckListCategory> IncludeServiceCheckLists(this IQueryable<CheckListCategory> queryable)
        {
            return ((DbQuery<CheckListCategory>)queryable).Include("ServiceCheckLists");
        }
    }
}
