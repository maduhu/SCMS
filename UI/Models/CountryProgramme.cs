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
    public class UCountryProgramme
    {
        public UCountryProgramme()
        {
            this._CountryProgramme = new SCMS.Model.CountryProgramme();
        }

        public SelectList CountrySelect { get; set; }
        public SelectList CurrencySelect { get; set; }

        public CountryProgramme _CountryProgramme { get; set; }

        public string Id
        {
            get
            {
                return _CountryProgramme.Id.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _CountryProgramme.Id = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string ProgrammeName
        {
            get { return _CountryProgramme.ProgrammeName; }
            set { _CountryProgramme.ProgrammeName = value; }
        }

        [Required]
        public string ShortName
        {
            get { return _CountryProgramme.ShortName; }
            set { _CountryProgramme.ShortName = value; }
        }

        public string Address
        {
            get { return _CountryProgramme.Address; }
            set { _CountryProgramme.Address = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string PrimaryPhone
        {
            get { return _CountryProgramme.PrimaryPhone; }
            set { _CountryProgramme.PrimaryPhone = value; }
        }

        public string SecondaryPhone
        {
            get { return _CountryProgramme.SecondaryPhone; }
            set { _CountryProgramme.SecondaryPhone = value; }
        }

        public string Fax
        {
            get { return _CountryProgramme.Fax; }
            set { _CountryProgramme.Fax = value; }
        }

        public string Email
        {
            get { return _CountryProgramme.Email; }
            set { _CountryProgramme.Email = value; }
        }

        public string WebAddress
        {
            get { return _CountryProgramme.WebAddress; }
            set { _CountryProgramme.WebAddress = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string CountryId
        {
            get
            {
                return _CountryProgramme.CountryId.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _CountryProgramme.CountryId = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string MBCurrencyId
        {
            get
            {
                return _CountryProgramme.MBCurrencyId.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _CountryProgramme.MBCurrencyId = new Guid(value);
                }
            }
        }
    }
}