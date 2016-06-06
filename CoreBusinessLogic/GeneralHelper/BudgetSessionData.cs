using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.GeneralHelper
{
    public partial class SessionData
    {

        #region .private variables.

        private IEnumerable<Model.Project> _projectList;
        private IEnumerable<Model.ProjectDonor> _pdList;
        private IEnumerable<Model.ProjectBudget> _pbList;

        #endregion

        public IEnumerable<Model.Project> ProjectList 
        {
            get
            {
                if (_projectList == null || _projectList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _projectList = db.Projects
                            .IncludeProjectDonors()
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();                        
                    }
                }
                return _projectList;
            }
            set
            {
                _projectList = value;
            }
        }

        public IEnumerable<Model.ProjectDonor> ProjectDonorList
        {
            get
            {
                if (_pdList == null || _pdList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _pdList = dbContext.ProjectDonors
                            .IncludeDonor()
                            .IncludeProject()
                            .IncludeProjectBudgets()
                            .IncludeProcurementPlans()
                            .Where(p => p.Project.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _pdList;
            }
            set
            {
                _pdList = value;
            }
        }

        public IEnumerable<Model.ProjectBudget> ProjectBudgetList 
        {
            get
            {
                if (_pbList == null || _pbList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _pbList = dbContext.ProjectBudgets
                            .IncludeGeneralLedger()
                            .IncludeProject()
                            .Where(p => p.BudgetCategory.ProjectDonor.Project.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _pbList;
            }
            set
            {
                _pbList = value;
            }
        }

    }
}
