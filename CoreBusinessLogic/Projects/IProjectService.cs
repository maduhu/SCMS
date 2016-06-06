using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Projects
{
    public interface IProjectService
    {
        void CreateProject(Project project);
        void UpdateProject(Project project);
        void CreateProjectDonor(Project project, DateTime startDate, DateTime endDate, Guid projectManagerId, string donorId, string currencyId, double? overrunAdjustment, ref string pdId);
        void UpdateProjectDonor(string projectDonorId, string projectNumber, DateTime startDate, DateTime endDate, Guid projectManagerId, string donorId, string currencyId, double? overrunAdjustment, ref string pdId);
        void DeleteProjectDonor(string projectDonorId);
        List<ProjectDonor> GetCurrentProjectDonors(CountryProgramme cp);
        List<ProjectDonor> GetExpiredProjectDonors(CountryProgramme cp);
        List<Project> GetProjects(Guid countryProgId);
        ProjectDonor GetProjectDonorById(Guid projectDonorId);
        Donor GetDonorById(Guid donorId);
        List<Donor> GetDonors(Guid countryProgId);
        void SaveDonor(Donor donor);
        void DeleteDonorById(Guid donorId);
        List<Currency> GetCurrencies(Guid countryProgId);
    }
}