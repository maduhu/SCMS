using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SCMS.Model;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class UCurrency
    {
        public UCurrency()
        {
            this._Currency = new SCMS.Model.Currency();
        }

        public Currency _Currency { get; set; }

        public string Id
        {
            get
            { 
                return _Currency.Id.ToString(); 
            }
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    _Currency.Id = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Name
        {
            get { return _Currency.Name; }
            set { _Currency.Name = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string ShortName
        {
            get { return _Currency.ShortName; }
            set { _Currency.ShortName = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Symbol
        {
            get { return _Currency.Symbol; }
            set { _Currency.Symbol = value; }
        }

    }
}