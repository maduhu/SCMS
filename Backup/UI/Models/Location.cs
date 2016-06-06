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
    public class ULocation
    {
        public ULocation()
        {
            this._Location = new SCMS.Model.Location();
        }

        public SelectList CountrySelect { get; set; }

        public Location _Location { get; set; }

        public string Id
        {
            get
            {
                return _Location.Id.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _Location.Id = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Name
        {
            get { return _Location.Name; }
            set { _Location.Name = value; }
        }

        public string ShortName
        {
            get { return _Location.ShortName; }
            set { _Location.ShortName = value; }
        }

        public string Description
        {
            get { return _Location.Description; }
            set { _Location.Description = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public Guid CountryId
        {
            get
            {
                return _Location.CountryId;
            }
            set
            {
                _Location.CountryId = value;
            }
        }

        public bool IsFinal { get; set; }
    }
}