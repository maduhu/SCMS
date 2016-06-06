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
    public class UCountry
    {
        public UCountry()
        {
            this._country = new SCMS.Model.Country();
        }

        public SelectList CurrencySelect
        {
            get;
            set;
        }

        public Country _country { get; set; }

        public string Id
        {
            get
            {
                return _country.Id.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _country.Id = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Name
        {
            get { return _country.Name; }
            set { _country.Name = value; }
        }

        public string ShortName
        {
            get { return _country.ShortName; }
            set { _country.ShortName = value; }
        }

        public string CurrencyId
        {
            get 
            {
                return _country.CurrencyId.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _country.CurrencyId = new Guid(value);
                }
            }
        }
    }
}