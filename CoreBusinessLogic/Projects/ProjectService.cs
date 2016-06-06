using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Projects
{
    public class ProjectService : IProjectService
    {
        public void CreateProjectDonor(Project project, DateTime startDate, DateTime endDate, Guid projectManagerId, string donorId, string currencyId, double? overrunAdjustment, ref string pdId)
        {
            if (ProjectNumberExits(project.ProjectNumber, null))
            {
                pdId = null;
                return;
            }
            using (var context = new SCMSEntities())
            {
                ProjectDonor pd = new ProjectDonor();
                pd.Id = Guid.NewGuid();
                pd.StartDate = startDate;
                pd.EndDate = endDate;
                pd.ProjectId = project.Id;
                pd.ProjectManagerId = projectManagerId;
                pd.DonorId = new Guid(donorId);
                pd.CurrencyId = new Guid(currencyId);
                pd.ProjectNumber = project.ProjectNumber;
                pd.OverrunAdjustment = overrunAdjustment;
                context.ProjectDonors.Add(pd);
                context.SaveChanges();
                pdId = pd.Id.ToString();
            }
        }

        private bool ProjectNumberExits(string projectNumber, string pdId)
        {
            using (var context = new SCMSEntities())
            {
                if (pdId != null)
                {
                    var pd = context.ProjectDonors.Where(p => p.ProjectNumber.Equals(projectNumber, StringComparison.CurrentCultureIgnoreCase) && p.Id != new Guid(pdId)).ToList();
                    if (pd != null && pd.Count > 0)
                        return true;
                }
                else
                {
                    var pd = context.ProjectDonors.Where(p => p.ProjectNumber.Equals(projectNumber, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    if (pd != null && pd.Count > 0)
                        return true;
                }
            }
            return false;
        }

        public void UpdateProjectDonor(string projectDonorId, string projectNumber, DateTime startDate, DateTime endDate, Guid projectManagerId, string donorId, string currencyId, double? overrunAdjustment, ref string pdId)
        {
            if (ProjectNumberExits(projectNumber, projectDonorId))
            {
                pdId = null;
                return;
            }
            using (var context = new SCMSEntities())
            {
                var pd = context.ProjectDonors.FirstOrDefault(p => p.Id == new Guid(projectDonorId));
                pd.StartDate = startDate;
                pd.EndDate = endDate;
                pd.ProjectManagerId = projectManagerId;
                pd.DonorId = new Guid(donorId);
                pd.CurrencyId = new Guid(currencyId);
                pd.ProjectNumber = projectNumber;

                var blList = context.ProjectBudgets.Where(b => b.BudgetCategory.ProjectDonor.Id == pd.Id).ToList();
                foreach (var bl in blList)
                {
                    if (bl.OverrunAdjustment == pd.OverrunAdjustment)
                        bl.OverrunAdjustment = overrunAdjustment;
                }

                pd.OverrunAdjustment = overrunAdjustment;
                context.SaveChanges();
                pdId = projectDonorId;
            }
        }

        public void DeleteProjectDonor(string projectDonorId)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var pd = context.ProjectDonors.First(p => p.Id == new Guid(projectDonorId));
                    context.ProjectDonors.Remove(pd);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            { 
                
            }
        }

        public void CreateProject(Project project)
        {
            try
            {
                //set project's country programme
                SaveProject(project);
                //if project exists, then update the name, short name of the existing project
            }
            catch (Exception ex)
            { 
                
            }
        }

        public void UpdateProject(Project project)
        {
            using (var context = new SCMSEntities())
            {
                var proj = context.Projects.First(p => p.Id == project.Id);
                proj.Name = project.Name;
                proj.ShortName = project.ShortName;
                context.SaveChanges();
            }
        }
        
        /// <summary>
        /// Check if the project name already exists. If it does, then use the existing project instead of creating a new one
        /// The check is made at country programme level
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        private void SaveProject(Project project)
        {
            List<Project> existingProjects;
            using (var context = new SCMSEntities())
            {
                existingProjects = context.Projects.Where(p => p.Name.Trim().Equals(project.Name.Trim(), StringComparison.CurrentCultureIgnoreCase) && p.CountryProgrammeId == project.CountryProgrammeId).ToList<Project>();
                if (existingProjects.Count > 0)
                {
                    existingProjects[0].Name = project.Name;
                    existingProjects[0].ShortName = project.ShortName;
                    context.SaveChanges();
                    project.Id = existingProjects[0].Id;
                }
                else
                {
                    Project proj = new Project();
                    proj.Name = project.Name;
                    proj.ShortName = project.ShortName;
                    proj.ProjectNumber = project.ProjectNumber;
                    proj.CountryProgrammeId = project.CountryProgrammeId;
                    proj.Id = project.Id = Guid.NewGuid();
                    context.Projects.Add(proj);
                    context.SaveChanges();
                }
            }
        }

        public List<ProjectDonor> GetCurrentProjectDonors(CountryProgramme cp)
        {
            List<ProjectDonor> projects = new List<ProjectDonor>();
            using (var context = new SCMSEntities())
            {
                projects = context.ProjectDonors.Where(c => c.Project.CountryProgrammeId == cp.Id && c.EndDate >= DateTime.Today).OrderBy(c => c.Project.Name).ToList<ProjectDonor>();
                //dot the related entities 'coz they will be accessed in the view
                foreach (ProjectDonor pd in projects)
                {
                    Project p = pd.Project;
                    Donor d = pd.Donor;
                    Currency c = pd.Currency;
                    Person person = pd.Staff.Person;
                    List<BudgetCategory> bcs = pd.BudgetCategories.ToList();
                }
            }
            return projects;
        }

        public List<ProjectDonor> GetExpiredProjectDonors(CountryProgramme cp)
        {
            List<ProjectDonor> projects = new List<ProjectDonor>();
            using (var context = new SCMSEntities())
            {
                projects = context.ProjectDonors.Where(c => c.Project.CountryProgrammeId == cp.Id && c.EndDate < DateTime.Today).OrderBy(c => c.Project.Name).ToList<ProjectDonor>();
                //dot the related entities 'coz they will be accessed in the view
                foreach (ProjectDonor pd in projects)
                {
                    Project p = pd.Project;
                    Donor d = pd.Donor;
                    Currency c = pd.Currency;
                    Person person = pd.Staff.Person;
                    List<BudgetCategory> bcs = pd.BudgetCategories.ToList();
                }
            }
            return projects;
        }

        public List<Project> GetProjects(Guid countryProgId)
        {
            using (var context = SCMSEntities.Define())
            {
                return context.Projects.Where(p => p.CountryProgrammeId == countryProgId).ToList();
            }
        }

        public List<Donor> GetDonors(Guid countryProgId)
        {
            List<Donor> donors = new List<Donor>();
            using (var context = new SCMSEntities())
            {
                donors = context.Donors.Where(d => d.CountryProgrammeId == countryProgId).OrderBy(d => d.Name).ToList();
                foreach (var donor in donors)
                {
                    var country = donor.Country;
                    var cp = donor.CountryProgramme;
                }
            }
            return donors;
        }

        public List<Currency> GetCurrencies(Guid countryProgId)
        {
            List<Currency> currencies = new List<Currency>();
            using (var context = new SCMSEntities())
            {
                currencies = context.Currencies.Where(c => c.CountryProgrammeId == countryProgId).ToList();
            }
            return currencies;
        }

        public Donor GetDonorById(Guid id)
        {
            using (var context = new SCMSEntities())
            {
                var donor = context.Donors.FirstOrDefault(d => d.Id == id);
                var cp = donor.CountryProgramme;
                var country = donor.Country;
                return donor;
            }            
        }

        public void SaveDonor(Donor donor)
        {
            using (var context = new SCMSEntities())
            {
                if (donor.Id.Equals(Guid.Empty))
                {
                    donor.Id = Guid.NewGuid();
                    context.Donors.Add(donor);
                }
                else
                {
                    context.Donors.Attach(donor);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(donor, System.Data.EntityState.Modified);
                }
                context.SaveChanges();
            }
        }

        public void DeleteDonorById(Guid donorId)
        {
            using (var context = new SCMSEntities())
            {
                var donor = context.Donors.FirstOrDefault(d => d.Id == donorId);
                if (donor != null)
                    context.Donors.Remove(donor);
                context.SaveChanges();
            }
        }

        public ProjectDonor GetProjectDonorById(Guid projectDonorId)
        {
            using (var context = new SCMSEntities())
            {
                ProjectDonor pd = (ProjectDonor)context.ProjectDonors.Where(p => p.Id == projectDonorId).FirstOrDefault();
                Project pj = pd.Project;
                Donor d = pd.Donor;
                Currency c = pd.Currency;
                Person person = pd.Staff.Person;
                return pd;
            }
        }
    }
}