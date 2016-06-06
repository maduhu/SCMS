using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCMS.Model;

namespace SCMS.UI.Models
{
    public class InsureType
    {
        public Model.InsuranceType EntityInsureType { get; set; }
    }

    public class ViewInsureType
    {
        public InsuranceType EntityInsureType { get; set; }
    }
}