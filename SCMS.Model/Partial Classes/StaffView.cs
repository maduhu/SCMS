using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public class StaffView
    {
        public Guid Id { get; set; }

        public Guid StaffId { get; set; }

        public string StaffName { get; set; }

        public string StaffDesignation { get; set; }

        /// <summary>
        /// Staff name and Designation combined
        /// </summary>
        public string StaffNameDesg 
        {
            get
            {
                return StaffName + " [" + StaffDesignation + "]";
            }
        }
    }
}
