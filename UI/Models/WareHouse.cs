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
    public class UWareHouse
    {
        public UWareHouse()
        {
            this._wareHouse = new SCMS.Model.WareHouse();
        }

        public SelectList SubOffSelect { get; set; }

        public SelectList LocationSelect { get; set; }

        public WareHouse _wareHouse { get; set; }

        public string Id
        {
            get
            {
                return _wareHouse.Id.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _wareHouse.Id = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Name
        {
            get { return _wareHouse.Name; }
            set { _wareHouse.Name = value; }
        }

        public string LocationId
        {
            get
            {
                return _wareHouse.LocationId.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _wareHouse.LocationId = new Guid(value);
                }
            }
        }

        public string SubOfficeId
        {
            get
            {
                return _wareHouse.SubOfficeId.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _wareHouse.SubOfficeId = new Guid(value);
                }
            }
        }
    }
}