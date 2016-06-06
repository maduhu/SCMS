using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Projects;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic._Country;
using SCMS.Model;
using SCMS.CoreBusinessLogic.ActionFilters;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class DonorController : PortalBaseController
    {
        private IProjectService projectService;
        private ICountryService countryService;
        public DonorController(IProjectService projectService, ICountryService countryService, IPermissionService permissionService, IUserContext userContext)
            : base(userContext, permissionService)
        {
            this.countryService = countryService;
            this.projectService = projectService;
        }

        //
        // GET: /Donor/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListView()
        {
            return View("ListView", projectService.GetDonors(countryProg.Id));
        }

        public ActionResult Create()
        {
            return View("CreateEdit", new Donor { Countries = new SelectList(countryService.GetCountries(), "Id", "Name") });
        }

        public ActionResult Edit(Guid id)
        {
            Donor donor = projectService.GetDonorById(id);
            donor.Countries = new SelectList(countryService.GetCountries(), "Id", "Name");
            return View("CreateEdit", donor);
        }

        [HttpPost]
        public ActionResult CreateEdit(Donor model)
        {
            model.CountryProgrammeId = countryProg.Id;
            projectService.SaveDonor(model);
            return ListView();
        }

        public ActionResult Delete(Guid id)
        {
            projectService.DeleteDonorById(id);
            return ListView();
        }
    }
}
