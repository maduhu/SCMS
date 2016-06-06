using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.UI.Models;
using SCMS.Model;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic.Security;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class VehicleMakeController : PortalBaseController
    {
        private IVehicleMakeService vmakeService;

        public VehicleMakeController(IPermissionService permissionService, IUserContext userContext, IVehicleMakeService vmakeService)
            : base(userContext, permissionService)
        {
            this.vmakeService = vmakeService;
        }
        //
        // GET: /VehicleMake/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadVMake(Guid? makeId = null)
        {

            var model = new Vmake()
            {
                EntityVMake = makeId == null ? new VehicleMake() : vmakeService.GetMakeById((Guid)makeId)
            };
            ViewBag.Action = makeId == null ? "SaveVehicleMake" : "EditVehicleMake";
            return View(model);
        }

        public ActionResult SaveVehicleMake(Vmake modelEntity)
        {
            modelEntity.EntityVMake.Id = Guid.NewGuid();
            modelEntity.EntityVMake.CountryProgrammeId = countryProg.Id;
            vmakeService.IsVMakeSaved(modelEntity.EntityVMake);
            return ViewVMake();
        }

        public ActionResult EditVehicleMake(Vmake modelEntity)
        {
            vmakeService.IsMakeEdited(modelEntity.EntityVMake);
            return ViewVMake();
        }

        public ActionResult DeleteVMake(Guid makeId)
        {
            vmakeService.IsVMakeDeleted(makeId);
            return ViewVMake();
        }

        public ActionResult ViewVMake()
        {
            using (var db = new SCMSEntities())
            {
                List<ViewVMake> modellist = new List<Models.ViewVMake>();
                foreach (var item in db.VehicleMakes.Where(p => p.CountryProgrammeId == countryProg.Id).ToList())
                {
                    var model = new ViewVMake()
                        {
                            EntityVMake = item,
                            vmodels = item.VehicleModels.ToList()
                        };
                    modellist.Add(model);
                }
                return View("ViewVMake", modellist);
            }
        }

        public ActionResult LoadVModel(Guid? modelId = null)
        {
            var model = new VModel()
            {
                EntityVModel = modelId == null ? new Model.VehicleModel() : vmakeService.GetModelById((Guid)modelId),
                VMakes = new SelectList(vmakeService.GetVMakes(countryProg.Id), "Id", "Name")
            };
            ViewBag.Action = modelId == null ? "SaveVehicleModel" : "EditVehicleModel";
            return View(model);
        }

        public ActionResult SaveVehicleModel(VModel modelEntity)
        {
            modelEntity.EntityVModel.Id = Guid.NewGuid();
            modelEntity.EntityVModel.CountryProgrammeId = countryProg.Id;
            vmakeService.IsVModelSaved(modelEntity.EntityVModel);
            return ViewVMake();
        }

        public ActionResult EditVehicleModel(VModel modelEntity)
        {
            vmakeService.IsModelEdited(modelEntity.EntityVModel);
            return ViewVMake();
        }

        public ActionResult DeleteVModel(Guid modelId)
        {
            vmakeService.IsVModelDeleted(modelId);
            return ViewVMake();
        }
    }
}
