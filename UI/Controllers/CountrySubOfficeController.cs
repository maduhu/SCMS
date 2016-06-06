using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._CountrySubOffice;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class CountrySubOfficeController : PortalBaseController
    {
        private ICountrySubOfficeService ctrySubOfficeService;

        public CountrySubOfficeController(IPermissionService permissionService, IUserContext userContext, ICountrySubOfficeService _ctrySubOfficeService)
            : base(userContext, permissionService)
        {
            this.ctrySubOfficeService = _ctrySubOfficeService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult Create()
        {
            UCountrySubOffice ctrySubOffice = new UCountrySubOffice();

            ctrySubOffice.LocationSelect = 
                new SelectList(ctrySubOfficeService.LocationObj
                    .GetLocations(countryId: countryProg.CountryId), "Id", "Name");
            return View("CreateEdit", ctrySubOffice);
        }

        public ActionResult Edit(Guid id)
        {
            UCountrySubOffice ctrySubOffice = new UCountrySubOffice();
            ctrySubOffice.LocationSelect = new SelectList(ctrySubOfficeService.LocationObj.GetLocations(countryId: countryProg.CountryId), "Id", "Name");
            ViewBag.Action = "Update";
            ctrySubOffice._countrySubOffice = ctrySubOfficeService.GetCountrySubOffice(id);
            return View("CreateEdit", ctrySubOffice);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(UCountrySubOffice ctrySubOffice)
        {
            if (ModelState.IsValid)
            {
                if (!ctrySubOffice.Id.Equals(Guid.Empty))
                {
                    ctrySubOffice._countrySubOffice.CountryProgrammeId = countryProg.Id;                    
                    if (ctrySubOfficeService.EditCountrySubOffice(ctrySubOffice._countrySubOffice))
                    {
                        ModelState.Clear();
                        ctrySubOffice = new UCountrySubOffice();
                    }
                }
                else
                {
                    ctrySubOffice._countrySubOffice.CountryProgrammeId = countryProg.Id;
                    if (ctrySubOfficeService.AddCountrySubOffice(ctrySubOffice._countrySubOffice))
                    {
                        ModelState.Clear();
                        ctrySubOffice = new UCountrySubOffice();
                    }
                }
            }
            return ListView();
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            List<CountrySubOfficeService.CountrySubOfficeView> ctrySubOffices = ctrySubOfficeService.GetCountrySubOffices1(countryProg.Id, search);
            return View("ListView", ctrySubOffices);
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string csoid)
        {
            Guid ctrySuboffId;
            if (Guid.TryParse(csoid, out ctrySuboffId))
            {
                if (ctrySubOfficeService.DeleteCountrySubOffice(ctrySuboffId))
                {
                    ModelState.Clear();
                }
            }

            List<CountrySubOfficeService.CountrySubOfficeView> ctrySubOffices = ctrySubOfficeService.GetCountrySubOffices1(countryProg.Id);
            return View("ListView", ctrySubOffices);
        }

    }
}
