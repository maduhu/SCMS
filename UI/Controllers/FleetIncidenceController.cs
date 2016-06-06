using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.FleetIncidences;
using SCMS.Model;
using SCMS.UI.Models;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class FleetIncidenceController : PortalBaseController
    {
        private IFleetIncidenceService fleetincidenceService;

        public FleetIncidenceController(IPermissionService permissionService, IUserContext userContext, IFleetIncidenceService fleetincidenceService)
            : base(userContext, permissionService)
        {
            this.fleetincidenceService = fleetincidenceService;
        }
        //
        // GET: /FleetIncidence/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult LoadFleetIncidence(Guid? incidenceId = null)
        {
            var model = new FleetIncidence()
            {
               EntityFleetInc = incidenceId == null ? new FleetMajorIncidence() : fleetincidenceService.GetFleetIncidenceById((Guid)incidenceId)
            };
            ViewBag.Action = incidenceId == null ? "SaveFleetIncidence" : "EditFleetIncidence";
            return View(model);
        }

        public ActionResult SaveFleetIncidence(FleetIncidence modelEntity)
        {
            modelEntity.EntityFleetInc.Id = Guid.NewGuid();
            modelEntity.EntityFleetInc.CountryProgramId = countryProg.Id;
            fleetincidenceService.IsIncidenceSaved(modelEntity.EntityFleetInc);
            return ViewFleetIncidence();
        }

        public ActionResult EditFleetIncidence(FleetIncidence modelEntity)
        {
            fleetincidenceService.IsFleetIncidenceEdited(modelEntity.EntityFleetInc);
            return ViewFleetIncidence();
        }

        public ActionResult DeleteFleetIncidence(Guid incidenceId)
        {
            fleetincidenceService.IsFleetIncidenceDeleted(incidenceId);
            return ViewFleetIncidence();
        }

        public ActionResult ViewFleetIncidence()
        {
            List<ViewFleetIncidence> modellist = new List<ViewFleetIncidence>();
            foreach (var item in fleetincidenceService.GetFleetIncidencs(countryProg.Id))
            {
                var model = new ViewFleetIncidence()
                {
                   EntityFleetInc = item
                };
                modellist.Add(model);
            }
            return View("ViewFleetIncidence", modellist);

        }
    }
}
