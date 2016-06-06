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
    public class UCountrySubOffice
    {
        public UCountrySubOffice()
        {
            this._countrySubOffice = new SCMS.Model.CountrySubOffice();
        }

        public SelectList CountryProgrammeSelect { get; set; }

        public SelectList LocationSelect { get; set; }

        public CountrySubOffice _countrySubOffice { get; set; }

        public Guid Id
        {
            get
            {
                return _countrySubOffice.Id;
            }
            set
            {
                _countrySubOffice.Id = value;
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Name
        {
            get { return _countrySubOffice.Name; }
            set { _countrySubOffice.Name = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Address
        {
            get { return _countrySubOffice.Address; }
            set { _countrySubOffice.Address = value; }
        }

        public bool IsCountryheadOffice
        {
            get { return _countrySubOffice.IsCountryHeadOffice; }
            set { _countrySubOffice.IsCountryHeadOffice = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string LocationId
        {
            get
            {
                return _countrySubOffice.LocationId.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _countrySubOffice.LocationId = new Guid(value);
                }
            }
        }
    }
}