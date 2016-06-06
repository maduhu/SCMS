using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCMS.Model;
using System.Web.Mvc;

namespace SCMS.UI.Models
{
    public class PPModel
    {
        public ProcurementPlan EntityPP { get; set; }

        public SelectList SubOffices { get; set; }

        public Guid ProjectId { get; set; }
        public SelectList Projects { get; set; }
        
        public SelectList ProjectDonors { get; set; }

        public List<PPItemModel> PPItems { get; set; }

        public Guid OrderRequestId { get; set; }
    }

    public class PPItemModel
    {
        public ProcurementPlanItem EntityPPItem { get; set; }

        public SelectList Items { get; set; }

        public SelectList Currencies { get; set; }

        public SelectList BudgetLines { get; set; }

        public SelectList Locations { get; set; }

        /// <summary>
        /// This is required for Add New function to know which view to render after Add New is done
        /// </summary>
        public bool EditMode { get; set; }
    }
}