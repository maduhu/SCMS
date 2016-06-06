using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.CoreBusinessLogic.PurchaseOrder;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;

namespace SCMS.UI.Controllers
{
    public class TenderingTypeController : PortalBaseController
    {
        private IPurchaseOrderService poService;
        private IOrderRequest orService;

        public TenderingTypeController(IPermissionService permissionService, IUserContext userContext, IPurchaseOrderService poService, IOrderRequest orService)
            : base(userContext, permissionService)
        {
            this.poService = poService;
            this.orService = orService;
        }
        //
        // GET: /TenderingType/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListView()
        {
            return View("ListView", poService.GetTenderingTypes());
        }

        public ActionResult Create()
        {
            return View("CreateEdit", new TenderingType { Currencies = new SelectList(orService.GetCurrencies(), "Id", "ShortName", mbCurrency.Id), CurrencyId = mbCurrency.Id });
        }

        [HttpPost]
        public ActionResult CreateEdit(TenderingType entity)
        {
            if (entity.Id.Equals(Guid.Empty))
            {
                entity.Id = Guid.NewGuid();
                entity.CountryProgrammeId = countryProg.Id;
                poService.InsertTenderingType(entity);
            }
            else
            {
                var tt = poService.GetTenderingTypeById(entity.Id);
                tt.Name = entity.Name;
                tt.Description = entity.Description;
                tt.MinValue = entity.MinValue;
                tt.MaxValue = entity.MaxValue;
                tt.CurrencyId = entity.CurrencyId;
                tt.IsInternational = entity.IsInternational;
                poService.UpdateTenderingType(tt);
            }
            return ListView();
        }

        public ActionResult Edit(Guid id)
        {
            var tt = poService.GetTenderingTypeById(id);
            tt.Currencies = new SelectList(orService.GetCurrencies(), "Id", "ShortName", tt.CurrencyId);
            return View("CreateEdit", tt);
        }

        public ActionResult Delete(Guid id)
        {
            poService.DeleteTenderingTypeById(id);
            return ListView();

        }
    }
}
