using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SCMS.Model;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class UUnitOfMeasure
    {
        public UUnitOfMeasure()
        {
            this._UnitOfMeasure = new SCMS.Model.UnitOfMeasure();
        }

        public UnitOfMeasure _UnitOfMeasure { get; set; }

        public string Id
        {
            get
            {
                return _UnitOfMeasure.Id.ToString(); 
            }
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    _UnitOfMeasure.Id = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Code
        {
            get { return _UnitOfMeasure.Code; }
            set { _UnitOfMeasure.Code = value; }
        }

        public string Description
        {
            get { return _UnitOfMeasure.Description; }
            set { _UnitOfMeasure.Description = value; }
        }
    }
}