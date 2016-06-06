using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SCMS.Model;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class UItemCategory
    {
        public UItemCategory()
        {
            this._ItemCategory = new SCMS.Model.ItemCategory();
        }

        public ItemCategory _ItemCategory { get; set; }

        public string Id
        {
            get
            {
                return _ItemCategory.Id.ToString(); 
            }
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    _ItemCategory.Id = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Code
        {
            get { return _ItemCategory.CategoryCode; }
            set { _ItemCategory.CategoryCode = value; }
        }

        //[Required(ErrorMessage = "Name required")]
        public string Name
        {
            get { return _ItemCategory.CategoryName; }
            set { _ItemCategory.CategoryName = value; }
        }
    }
}