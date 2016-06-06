using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._UnitOfMeasure;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class UnitOfMeasureController : PortalBaseController
    {
        private IUnitOfMeasureService unitOfMeasureService;

        public UnitOfMeasureController(IPermissionService permissionService, IUserContext userContext, IUnitOfMeasureService _unitOfMeasureService)
            : base(userContext, permissionService)
        {
            this.unitOfMeasureService = _unitOfMeasureService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult CreateEdit(string uid)
        {
            UUnitOfMeasure unitOfMeasure = new UUnitOfMeasure();

            Guid unitOfMeasureId;
            if (Guid.TryParse(uid, out unitOfMeasureId))
            {
                ViewBag.Action = Resources.Global_String_Edit;
                unitOfMeasure._UnitOfMeasure = unitOfMeasureService.GetUnitOfMeasure(unitOfMeasureId);
            }
            return View(unitOfMeasure);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(UUnitOfMeasure unitOfMeasure, string Action)
        {
            if (ModelState.IsValid)
            {
                if (Action.Equals("Edit"))
                {
                    unitOfMeasure._UnitOfMeasure.CountryProgrammeId = countryProg.Id;
                    if (unitOfMeasureService.EditUnitOfMeasure(unitOfMeasure._UnitOfMeasure))
                    {
                        unitOfMeasure = new UUnitOfMeasure();
                        ModelState.Clear();
                    }
                }
                else
                {
                    unitOfMeasure._UnitOfMeasure.CountryProgrammeId = countryProg.Id;
                    if (unitOfMeasureService.AddUnitOfMeasure(unitOfMeasure._UnitOfMeasure))
                    {
                        unitOfMeasure = new UUnitOfMeasure();
                        ModelState.Clear();
                    }
                }
            }
            return ListView();
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            List<UnitOfMeasure> unitsOfMeasure = unitOfMeasureService.GetUnitsOfMeasure(countryProg.Id, search);
            return View("ListView", unitsOfMeasure);
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string uid)
        {
            Guid uofMeasureId;
            if (Guid.TryParse(uid, out uofMeasureId))
            {
                unitOfMeasureService.DeleteUnitOfMeasure(uofMeasureId);
            }

            List<UnitOfMeasure> unitsOfMeasure = unitOfMeasureService.GetUnitsOfMeasure(countryProg.Id);
            return View("ListView", unitsOfMeasure);
        }
    }
}
