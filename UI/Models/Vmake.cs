using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCMS.Model;
using System.Web.Mvc;

namespace SCMS.UI.Models
{
    public class Vmake
    {
        public VehicleMake EntityVMake { get; set; }
    }

    public class VModel
    {
        public VehicleModel EntityVModel { get; set; }
        public SelectList VMakes { get; set; }
    }

    public class ViewVMake
    {
        public VehicleMake EntityVMake { get; set; }
        public List<Model.VehicleModel> vmodels { get; set; }
    }
}