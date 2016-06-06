using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCMS.Model;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class WBill
    {
        public Model.WayBill EntityWBill { get; set; }

        public List<WarehouseReleaseItem> WRItems { get; set; }

        public SelectList Staff { get; set; }

        public SelectList Warehouses { get; set; }

        public SelectList WRNs { get; set; }

        public SelectList IssuingOffice { get; set; }
    }

    public class ReceiveWB
    {
        public Model.WayBill EntityWBill { get; set; }

        public List<WarehouseReleaseItem> WRItems { get; set; }
        public SelectList IssuingOffice { get; set; }

        public SelectList WBlist { get; set; }
        [Required]
        public decimal qtyreceived { get; set; }
    }

    public class ViewWayBill
    {
        public WayBill EntityWBill { get; set; }

        public Model.VStaffDetail issuer { get; set; }

        public Model.VStaffDetail consignee { get; set; }

        public Model.VStaffDetail ReceivedBy { get; set; }

        public Model.WareHouse OrrignWH { get; set; }

        public Model.WareHouse destinationWH { get; set; }

        public Model.CountrySubOffice OrignSOfiice { get; set; }

        public Model.CountrySubOffice DestnationOfiice { get; set; }

        public List<WarehouseReleaseItem> WRItems { get; set; }
    }

    public class ReceiveWBB
    {
        public Guid Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public decimal QuantityReceived { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public decimal QuantityDamaged { get; set; }
    }

    public class WayBillExtension
    {
        public static ViewWayBill prepareWB(Guid wbId)
        {
            using (var db = new SCMSEntities())
            {
                WayBill entitymodel = db.WayBills.First(p => p.Id == wbId);
                ViewWayBill model = new ViewWayBill();
                model.EntityWBill = entitymodel;
                WarehouseRelease wrn = model.EntityWBill.WarehouseRelease;
                model.issuer = db.VStaffDetails.FirstOrDefault(p => p.StaffID == entitymodel.PreparedBy);
                model.consignee = db.VStaffDetails.FirstOrDefault(p => p.StaffID == entitymodel.Consignee);
                model.ReceivedBy = db.VStaffDetails.FirstOrDefault(p => p.StaffID == entitymodel.ReceivedBy);
                model.OrrignWH = db.WareHouses.FirstOrDefault(p => p.Id == entitymodel.IssuerWarehouse);
                model.destinationWH = db.WareHouses.FirstOrDefault(p => p.Id == entitymodel.DestinationWarehouse);
                model.DestnationOfiice = db.CountrySubOffices.FirstOrDefault(p => p.Id == entitymodel.DestinationOffice);
                Model.Location loc = model.destinationWH.Location;
                Model.CountrySubOffice cso = model.destinationWH.CountrySubOffice;
                model.OrignSOfiice = db.CountrySubOffices.First(p => p.Id == entitymodel.IssuingOffice);
                Model.Location l = model.OrignSOfiice.Location;
                Model.Location lok = model.DestnationOfiice.Location;
                List<WarehouseReleaseItem> writems = entitymodel.WarehouseRelease.WarehouseReleaseItems.ToList();
                foreach (WarehouseReleaseItem item in writems)
                {
                    Model.Inventory inv = item.Inventory;
                    Model.Item it = inv.Item;
                    Model.ItemCategory ic = it.ItemCategory;
                    Model.UnitOfMeasure u = it.UnitOfMeasure;
                    Model.Asset ast = item.Asset;
                }
                model.WRItems = writems;
                //dummies
                var person = entitymodel.Staff.Person;
                if (entitymodel.Staff1 != null)
                    person = entitymodel.Staff1.Person;
                return model;
            }
        }
    }
}