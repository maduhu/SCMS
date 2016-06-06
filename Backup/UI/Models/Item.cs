using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCMS.Model;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class UItem
    {
        public UItem()
        {
            this._item = new SCMS.Model.Item();
        }

        public SelectList ItemCatSelect { get; set; }

        public SelectList ItemClassSelect { get; set; }

        public SelectList ItemUnitsSelect { get; set; }

        public Item _item { get; set; }

        public string Id
        {
            get
            {
                return _item.Id.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _item.Id = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Name
        {
            get { return _item.Name; }
            set { _item.Name = value; }
        }

        public string Description
        {
            get { return _item.Description; }
            set { _item.Description = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public Guid ItemCategoryId
        {
            get
            {
                return _item.ItemCategoryId;
            }
            set
            {
                _item.ItemCategoryId = value;
            }
        }

        public Guid UnitId
        {
            get
            {
                return _item.UnitOfMessureId;
            }
            set
            {
                _item.UnitOfMessureId = value;
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public Guid ItemClassificationId
        {
            get
            {
                return _item.ItemClassificationId;
            }
            set
            {
                _item.ItemClassificationId = value;
            }
        }

        /// <summary>
        /// Used to differentiate Add New Item in OR between Add and Edit Mode of OR
        /// </summary>
        public bool EditMode { get; set; }
    }

    public class ItemPack
    {
        public ItemPackage EntityItemPackage { get; set; }

        public SelectList Items { get; set; }
    }

    public class viewItems
    {
        public Model.Item item { get; set; }
        public List<Model.ItemPackage> pack { get; set; }
    }

    public class GridItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemCategoryName { get; set; }
        public string ItemClassificationName { get; set; }
    }

    public class GridItemPack
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int PackSize { get; set; }
    }
}