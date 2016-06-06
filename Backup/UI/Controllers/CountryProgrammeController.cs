using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._CountryProgramme;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic._Location;
using SCMS.CoreBusinessLogic._CountrySubOffice;
using SCMS.CoreBusinessLogic._Designation;
using SCMS.UI.GeneralHelper;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class CountryProgrammeController : PortalBaseController
    {
        private ICountryProgrammeService countryProgService;
        private ILocationService locationService;
        private ICountrySubOfficeService ctrySubOfficeService;
        private IDesignationService designationService;

        public CountryProgrammeController(IPermissionService permissionService, IUserContext userContext, ICountryProgrammeService _countryProgService,
            ILocationService locationService, ICountrySubOfficeService ctrySubOfficeService, IDesignationService designationService)
            : base(userContext, permissionService)
        {
            this.countryProgService = _countryProgService;
            this.locationService = locationService;
            this.ctrySubOfficeService = ctrySubOfficeService;
            this.designationService = designationService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult CreateEdit(Guid id)
        {
            UCountryProgramme countryProgramme = new UCountryProgramme();
            countryProgramme.CountrySelect = new SelectList(countryProgService.CountryObj.GetCountries(), "Id", "Name");
            countryProgramme.CurrencySelect = new SelectList(countryProgService.CurrencyObj.GetCurrencies(countryProg.Id), "Id", "Name");

            if (!id.Equals(Guid.Empty))
            {
                ViewBag.Action = "Edit";
                countryProgramme._CountryProgramme = countryProgService.GetCountryProgrammeById(id);
            }
            return View(countryProgramme);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(UCountryProgramme countryProgramme)
        {
            if (ModelState.IsValid)
            {
                if (UserSession.CurrentSession.UploadedFile != null)
                {
                    countryProgramme._CountryProgramme.Logo = UserSession.CurrentSession.UploadedFile;
                    UserSession.CurrentSession.UploadedFile = null;
                }
                if (!countryProgramme._CountryProgramme.Id.Equals(Guid.Empty))
                {
                    if (countryProgService.EditCountryProgramme(countryProgramme._CountryProgramme))
                    {
                        ModelState.Clear();
                        countryProgramme = new UCountryProgramme();
                    }
                }
                else
                {
                    if (countryProgService.AddCountryProgramme(countryProgramme._CountryProgramme))
                    {
                        ModelState.Clear();
                        countryProgramme = new UCountryProgramme();
                    }
                }
            }
            return ListView();
        }

        [HttpGet]
        public ActionResult CPLogo(Guid id)
        {
            var cp = countryProgService.GetCountryProgrammeById(id);
            if (cp == null || cp.Logo == null)
            {
                return Content("");
            }
            return new FileContentResult(cp.Logo, "image/jpeg");
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            List<CountryProgrammeService.CountryProgView> countryProgs = countryProgService.GetCountryProgrammes1(search);
            return View("ListView", countryProgs);
        }

        //
        // GET: DeleteItem
        public ActionResult Delete(Guid id)
        {
            if (!id.Equals(Guid.Empty))
            {
                if (countryProgService.DeleteCountryProgramme(id))
                {
                    ModelState.Clear();
                }
            }

            return ListView();
        }

        public ActionResult Details(Guid id)
        {
            return View("Details", countryProgService.GetCountryProgrammeForDisplay(id));
        }

        public ActionResult AddLocation(Guid countryProgId)
        {
            ULocation location = new ULocation();
            location._Location.CountryProgrammeId = countryProgId;
            location.CountrySelect = new SelectList(countryProgService.CountryObj.GetCountries(), "Id", "Name");
            return View(location);
        }

        [HttpPost]
        public ActionResult AddLocation(ULocation model)
        {
            locationService.AddLocation(model._Location);
            return Details(model._Location.CountryProgrammeId);
        }

        public ActionResult AddSubOffice(Guid countryProgId)
        {
            UCountrySubOffice ctrySubOffice = new UCountrySubOffice();
            ctrySubOffice._countrySubOffice.CountryProgrammeId = countryProgId;
            ctrySubOffice.LocationSelect =
                new SelectList(ctrySubOfficeService.LocationObj.GetLocations(countryProgId), "Id", "Name");
            return View(ctrySubOffice);
        }

        [HttpPost]
        public ActionResult AddSubOffice(UCountrySubOffice model)
        {
            ctrySubOfficeService.AddCountrySubOffice(model._countrySubOffice);
            return Details(model._countrySubOffice.CountryProgrammeId.HasValue ? model._countrySubOffice.CountryProgrammeId.Value : Guid.Empty);
        }

        public ActionResult AddDesignation(Guid countryProgId)
        {
            UDesignation designation = new UDesignation();
            designation._Designation.CountryProgrammeId = countryProgId;
            return View(designation);
        }

        [HttpPost]
        public ActionResult AddDesignation(UDesignation model)
        {
            designationService.AddDesignation(model._Designation);
            return Details(model._Designation.CountryProgrammeId);
        }
    }
}
