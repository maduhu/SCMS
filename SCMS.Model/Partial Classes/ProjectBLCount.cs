using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    /// <summary>
    /// Used to store count of project budget lines added to document
    /// </summary>
    public class ProjectBLCount
    {
        /// <summary>
        /// Project Donor Id
        /// </summary>
        public Guid ProjectDonorId { get; set; }

        /// <summary>
        /// Number of Budget Lines for ProjectDonorId
        /// </summary>
        public int BudgetLineCount { get; set; }
    }
}
