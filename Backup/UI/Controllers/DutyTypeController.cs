using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.DutyType;
using SCMS.Model;
using SCMS.UI.Models;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class DutyTypeController : PortalBaseController
    {
        private IDutyTypeService dutyService;

        public DutyTypeController(IPermissionService permissionService, IUserContext userContext, IDutyTypeService dutyService)
            : base(userContext, permissionService)
        {
            this.dutyService = dutyService;
        }
        //
        // GET: /DutyType/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult LoadDutyType(Guid? dutyId = null)
        {
            var model = new DutyTyppe()
            {
               EntityDutyType = dutyId == null ? new DutyType() :dutyService.GetDutyTypeById((Guid)dutyId)
            };
            ViewBag.Action = dutyId == null ? "SaveDutyType" : "EditDutyType";
            return View(model);
        }

        public ActionResult SaveDutyType(DutyTyppe modelEntity)
        {
            modelEntity.EntityDutyType.Id = Guid.NewGuid();
            modelEntity.EntityDutyType.CountryProgramId = countryProg.Id;
            dutyService.IsDutyTypeSaved(modelEntity.EntityDutyType);
            return ViewDutyTypes();
        }

        public ActionResult EditDutyType(DutyTyppe modelEntity)
        {
           dutyService.IsDutyTypeEdited(modelEntity.EntityDutyType);
            return ViewDutyTypes();
        }

        public ActionResult DeleteDutyType(Guid dutyId)
        {
            dutyService.IsDutyTypeDeleted(dutyId);
            return ViewDutyTypes();
        }

        public ActionResult ViewDutyTypes()
        {
            List<ViewDutyType> modellist = new List<ViewDutyType>();
            foreach (var item in dutyService.GetDutyTypes(countryProg.Id))
            {
                var model = new ViewDutyType()
                {
                   EntityDutyType = item
                };
                modellist.Add(model);
            }
            return View("ViewDutyTypes", modellist);

        }
    }
}
