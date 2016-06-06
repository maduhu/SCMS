using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SCMS.Model
{
    [MetadataType(typeof(InventoryMetaData))]
    public partial class Inventory
    {
        public string ItemName
        {
            get { return Item != null ? string.Format("{0}", Item.Name) : string.Empty; }
        }

        public SelectList Items { get; set; }
        public SelectList Warehouses { get; set; }
        public List<Model.Inventory> Inventories { get; set; }

        public IEnumerable<Model.Inventory> GetInventory { get; set; }

        public string ItName { get; set; }
        public string ItemClassification { get; set; }
        public string Unit { get; set; }
        public string WarehouseName { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public long ItemQTY { get; set; }
        public long TotalRecords { get; set; }
    }

    public class InventoryMetaData
    {
        [Required]
        public object Quantity { get; set; }
        [Required]
        public object ItemId { get; set; }

    }
}
