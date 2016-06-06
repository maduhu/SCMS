using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._WareHouse;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Resource;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class WareHouseController : PortalBaseController
    {
        private IWareHouseService wareHouseService;

        public WareHouseController(IPermissionService permissionService, IUserContext userContext, IWareHouseService _wareHouseService)
            : base(userContext, permissionService)
        {
            this.wareHouseService = _wareHouseService;
        }
        
        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult CreateEdit(string wid)
        {
            UWareHouse wareHouse = new UWareHouse();

            wareHouse.LocationSelect =
                new SelectList(wareHouseService.LocationObj.GetLocations(), "Id", "Name");

            wareHouse.SubOffSelect = new SelectList(wareHouseService.CtrySubOffObj.GetCountrySubOffices(countryProg.Id),
                    "Id", "Address");

            Guid whId;
            if (Guid.TryParse(wid, out whId))
            {
                ViewBag.Action = Resources.Global_String_Edit;
                wareHouse._wareHouse = wareHouseService.GetWareHouse(whId);
            }
            return View(wareHouse);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(UWareHouse wareHouse, string Action = "")
        {
            if (ModelState.IsValid)
            {
                if (!wareHouse._wareHouse.Id.Equals(Guid.Empty))
                {
                    wareHouse._wareHouse.CountryProgrammeId = countryProg.Id;
                    if (wareHouseService.EditWareHouse(wareHouse._wareHouse))
                    {
                        ModelState.Clear();
                        wareHouse = new UWareHouse();
                    }
                }
                else
                {
                    wareHouse._wareHouse.CountryProgrammeId = countryProg.Id;
                    if (wareHouseService.AddWareHouse(wareHouse._wareHouse))
                    {
                        ModelState.Clear();
                        wareHouse = new UWareHouse();
                    }
                }
            }


            wareHouse.LocationSelect =
               new SelectList(wareHouseService.LocationObj.GetLocations(), "Id", "Name");

            wareHouse.SubOffSelect = new SelectList(wareHouseService.CtrySubOffObj.GetCountrySubOffices(countryProg.Id),
                    "Id", "Address");
            return ListView();
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            List<WareHouseService.WareHouseView> warehouses = wareHouseService.GetWareHouses1(search);
            return View("ListView", warehouses);
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string wid)
        {
            Guid whseId;
            if (Guid.TryParse(wid, out whseId))
            {
                if (wareHouseService.DeleteWareHouse(whseId))
                {
                    ModelState.Clear();
                }
            }
            return ListView();
        }

    }
}
