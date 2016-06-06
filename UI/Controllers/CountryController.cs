using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._Country;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class CountryController : PortalBaseController
    {
        private ICountryService countryService;

        public CountryController(IPermissionService permissionService, IUserContext userContext, ICountryService _countryService)
            : base(userContext, permissionService)
        {
            this.countryService = _countryService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult CreateEdit(string cid)
        {
            UCountry country = new UCountry();
            country.CurrencySelect = new SelectList(countryService.CurrencyObj.GetCurrencies(countryProg.Id), "Id", "Name");

            Guid countryId;
            if (Guid.TryParse(cid, out countryId))
            {
                ViewBag.Action = "Edit";
                country._country = countryService.GetCountry(countryId);
            }
            return View(country);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(UCountry country)
        {
            if (ModelState.IsValid)
            {
                if (!country._country.Id.Equals(Guid.Empty))
                {
                    if (countryService.EditCountry(country._country))
                    {
                        ModelState.Clear();
                        country = new UCountry();
                        ViewBag.Message = Resources.CountryController_String_CountryUpdated;
                    }
                    else
                    {
                        ViewBag.Error = Resources.CountryController_String_CountryNotSaved;
                    }
                }
                else
                {
                    if (countryService.AddCountry(country._country))
                    {
                        ModelState.Clear();
                        country = new UCountry();
                        ViewBag.Message = Resources.CountryController_String_CountryAdded;
                    }
                    else
                    {
                        ViewBag.Error = Resources.CountryController_String_CountryNotSaved;
                    }
                }
            }
            country.CurrencySelect = new SelectList(countryService.CurrencyObj.GetCurrencies(countryProg.Id), "Id", "Name");
            return ListView();
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            List<CountryService.CountryView> countries = countryService.GetCountries1(search);
            return View("ListView", countries);
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string cid)
        {
            Guid countryId;
            if (Guid.TryParse(cid, out countryId))
            {
                if (countryService.DeleteCountry(countryId))
                {
                    ModelState.Clear();
                    ViewBag.Message = Resources.CountryController_String_CountryDeleted;
                }
                else
                {
                    ViewBag.Error = Resources.CountryController_String_CountryNotDeleted;
                }
            }
            else
            {
                ViewBag.Error = Resources.CountryController_String_CountryNotDeleted;
            }

            return ListView();
        }
    }
}
