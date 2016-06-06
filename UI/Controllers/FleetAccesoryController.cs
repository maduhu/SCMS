using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.FleetAcceories;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.UI.Models;
using SCMS.Model;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class FleetAccesoryController : PortalBaseController
    {
        private IFleetAccesoriesService fleetAccService;

        public FleetAccesoryController(IPermissionService permissionService, IUserContext userContext, IFleetAccesoriesService fleetAccService)
            : base(userContext, permissionService)
        {
            this.fleetAccService = fleetAccService;
        }
        //
        // GET: /FleetAccesory/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadFleetAccesory(Guid? accesoryId = null)
        {
            var model = new FleetAccesorry()
            {
                EntityFleetAcc = accesoryId == null ? new FleetEquipment() : fleetAccService.GetFleetAccesoryById((Guid)accesoryId)
            };
            ViewBag.Action = accesoryId == null ? "SaveFleetAccesory" : "EditFleetAccesory";
            return View(model);
        }

        public ActionResult SaveFleetAccesory(FleetAccesorry modelEntity)
        {
            modelEntity.EntityFleetAcc.Id = Guid.NewGuid();
            modelEntity.EntityFleetAcc.CountryProgrammeId = countryProg.Id;
            fleetAccService.IsFleetAccesorySaved(modelEntity.EntityFleetAcc);
            return ViewFleetAccesory();
        }

        public ActionResult EditFleetAccesory(FleetAccesorry modelEntity)
        {
            fleetAccService.IsFleetAccesoryEdited(modelEntity.EntityFleetAcc);
            return ViewFleetAccesory();
        }

        public ActionResult DeleteFleetAccesory(Guid accesoryId)
        {
            fleetAccService.IsFleetAccesoryDeleted(accesoryId);
            return ViewFleetAccesory();
        }

        public ActionResult ViewFleetAccesory()
        {
            List<ViewFleetAccesory> modellist = new List<Models.ViewFleetAccesory>();
            foreach (var item in fleetAccService.GetFleetAccesories(countryProg.Id))
            {
                var model = new ViewFleetAccesory()
                {
                   EntityFleetAcc = item
                };
                modellist.Add(model);
            }
            return View("ViewFleetAccesory", modellist);

        }
    }
}
