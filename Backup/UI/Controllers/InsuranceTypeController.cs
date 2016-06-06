using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.InsuranceType;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class InsuranceTypeController : PortalBaseController
    {
        private InsuranceTypeService insureService;

        public InsuranceTypeController(IPermissionService permissionService, IUserContext userContext, InsuranceTypeService insureService)
            : base(userContext, permissionService)
        {
            this.insureService = insureService;
        }
        //
        // GET: /InsuranceType/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadInsureType(Guid? insureId = null)
        {
            var model = new InsureType()
            {
                EntityInsureType = insureId == null ? new Model.InsuranceType() : insureService.GetInsuranceTypeById((Guid)insureId)
            };
            ViewBag.Action = insureId == null ? "SaveInsureType" : "EditInsureType";
            return View(model);
        }

        public ActionResult SaveInsureType(InsureType modelEntity)
        {
            modelEntity.EntityInsureType.Id = Guid.NewGuid();
            modelEntity.EntityInsureType.CountryProgrammeId = countryProg.Id;
            insureService.IsInsuranceTypeSaved(modelEntity.EntityInsureType);
            return ViewInsureType();
        }

        public ActionResult EditInsureType(InsureType modelEntity)
        {
            insureService.IsInsuranceTypeEdited(modelEntity.EntityInsureType);
            return ViewInsureType();
        }

        public ActionResult DeleteInsureType(Guid insureId)
        {
            insureService.IsInsuranceTypeDeleted(insureId);
            return ViewInsureType();
        }

        public ActionResult ViewInsureType()
        {
            List<ViewInsureType> modellist = new List<Models.ViewInsureType>();
            foreach (var item in insureService.GetInsuranceTypes(countryProg.Id))
            {
                var model = new ViewInsureType()
                {
                    EntityInsureType = item
                };
                modellist.Add(model);
            }
            return View("ViewInsureType", modellist);

        }
    }
}
