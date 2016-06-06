using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SCMS.Model;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class UItemClassification
    {
        public UItemClassification()
        {
            this._itemClassification = new SCMS.Model.ItemClassification();
        }

        public ItemClassification _itemClassification { get; set; }

        public string Id
        {
            get
            {
                return _itemClassification.Id.ToString(); 
            }
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    _itemClassification.Id = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Name
        {
            get { return _itemClassification.Name; }
            set { _itemClassification.Name = value; }
        }

        public string Description
        {
            get { return _itemClassification.Description; }
            set { _itemClassification.Description = value; }
        }
    }
}