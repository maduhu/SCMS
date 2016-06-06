using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class UDesignation
    {
        public UDesignation()
        {
            _Designation = new Model.Designation();
        }

        public Model.Designation _Designation { get; set; }

        public Guid Id
        { 
            get 
            { 
                return _Designation.Id; 
            }
            set
            {
                _Designation.Id = value;
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string Name
        {
            get { return _Designation.Name; }
            set { _Designation.Name = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string ShortName
        {
            get { return _Designation.ShortName; }
            set { _Designation.ShortName = value; }
        }

        public string Description
        {
            get { return _Designation.Description; }
            set { _Designation.Description = value; }
        }

    }
}