using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCMS.Model;
using System.Web.Mvc;

namespace SCMS.UI.Models
{
    public class SCheckList
    {
        public ServiceCheckList EntitySCL { get; set; }
        public SelectList CheckListCatories { get; set; }
    }
}