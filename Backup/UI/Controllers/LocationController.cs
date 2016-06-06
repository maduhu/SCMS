using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._Location;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.UI.GeneralHelper;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class LocationController : PortalBaseController
    {
        private ILocationService locationService;
        private IOrderRequest orService;

        public LocationController(IPermissionService permissionService, IUserContext userContext, ILocationService _locationService, IOrderRequest orService)
            : base(userContext, permissionService)
        {
            this.locationService = _locationService;
            this.orService = orService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult CreateEdit(string lid)
        {

            ULocation location = new ULocation();
            location.CountrySelect = new SelectList(locationService.CountryObj.GetCountries(), "Id", "Name");

            Guid locationId;
            if (Guid.TryParse(lid, out locationId))
            {
                ViewBag.Action = Resources.Global_String_Edit;
                location._Location = locationService.GetLocation(locationId);
            }
            return View(location);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(ULocation location, string Action = "")
        {
            if (ModelState.IsValid)
            {
                if (Action.Equals("Edit"))
                {
                    location._Location.CountryProgrammeId = countryProg.Id;
                    if (locationService.EditLocation(location._Location))
                    {
                        ModelState.Clear();
                        location = new ULocation();
                    }
                }
                else
                {
                    location._Location.CountryProgrammeId = countryProg.Id;
                    if (locationService.AddLocation(location._Location))
                    {
                        ModelState.Clear();
                        location = new ULocation();
                    }
                }
            }

            return ListView();
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            List<LocationService.LocationView> locations = locationService.GetLocations1(countryProg.Id, search);
            return View("ListView", locations);
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string lid)
        {
            Guid locationId;
            if (Guid.TryParse(lid, out locationId))
            {
                if (locationService.DeleteLocation(locationId))
                {
                    ModelState.Clear();
                }
            }

            List<LocationService.LocationView> locations = locationService.GetLocations1(countryProg.Id);
            return View("ListView", locations);
        }

        public ActionResult SearchLocations()
        {
            string searchTerm = Request.QueryString["q"];
            if (UserSession.CurrentSession.LocationList == null)
                UserSession.CurrentSession.LocationList = orService.GetLocations();
            string searchResults = "";
            foreach (Location location in UserSession.CurrentSession.LocationList)
            {
                if (location.Name.StartsWith(searchTerm, true, System.Globalization.CultureInfo.CurrentCulture))
                    searchResults += location.Name + "\n";
            }
            searchResults = searchResults != "" ? searchResults : "\n";
            return Content(searchResults, "text/html");
        }
        
    }
}
