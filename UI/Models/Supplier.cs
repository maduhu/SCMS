using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCMS.Model;
//using Microsoft.Web.Mvc;
using System.IO;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class USupplier
    {
        public USupplier()
        {
            this._Supplier = new SCMS.Model.Supplier();
        }

        public SelectList CountrySelect { get; set; }

        public Supplier _Supplier { get; set; }

        public string Id
        {
            get
            {
                return _Supplier.Id.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _Supplier.Id = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Name
        {
            get { return _Supplier.Name; }
            set { _Supplier.Name = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Address
        {
            get { return _Supplier.Address; }
            set { _Supplier.Address = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string PrimaryPhone
        {
            get { return _Supplier.PrimaryPhone; }
            set { _Supplier.PrimaryPhone = value; }
        }

        public string SecondaryPhone
        {
            get { return _Supplier.SecondaryPhone; }
            set { _Supplier.SecondaryPhone = value; }
        }

        public string PrimaryEmail
        {
            get { return _Supplier.PrimaryEmail; }
            set { _Supplier.PrimaryEmail = value; }
        }

        public string SecondaryEmail
        {
            get { return _Supplier.SecondaryEmail; }
            set { _Supplier.SecondaryEmail = value; }
        }

        public string Fax
        {
            get { return _Supplier.Fax; }
            set { _Supplier.Fax = value; }
        }

        public string WebAddress
        {
            get { return _Supplier.WebAddress; }
            set { _Supplier.WebAddress = value; }
        }

        public HttpPostedFileBase Logo
        {
            get;
            set;
        }

        public string LogoLocation
        {
            get { return _Supplier.LogoLocation; }
            set { _Supplier.LogoLocation = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string CountryId
        {
            get { return _Supplier.CountryId.ToString(); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _Supplier.CountryId = new Guid(value);
                }
            }
        }
    }
}