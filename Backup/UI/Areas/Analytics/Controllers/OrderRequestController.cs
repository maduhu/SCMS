using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.UI.Models;
using SCMS.UI.Areas.Analytics.Models.Search;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.CoreBusinessLogic.Security;

namespace SCMS.UI.Areas.Analytics.Controllers
{
    public class OrderRequestController : PortalBaseController
    {

        private IOrderRequest orderRequestService;

        public OrderRequestController(IPermissionService permissionService, IOrderRequest orderRequestService, IUserContext userContext)
            : base(userContext, permissionService)
        {
            this.orderRequestService = orderRequestService;
        }

        //
        // GET: /Analytics/OrderRequest/

        public ActionResult Index()
        {
            ORSearchFormViewModel form = new ORSearchFormViewModel();

            return View(form);
        }

        [HttpPost]
        public ActionResult Search(ORSearchFormViewModel form)
        {
            List<Model.OrderRequest> hits = orderRequestService.Search(form.startDate, form.endDate);

            return View(hits);
        }

        [HttpPost]
        public JsonResult PagedSearch(ORSearchFormViewModel form)
        {
            //List<Model.OrderRequest> hits = orderRequestService.Search(form.startDate, form.endDate);
            int start = 0;
            int pageSize = 10;

            int _start= new int();
            if (int.TryParse(Request.QueryString["jtStartIndex"], out _start))
            {
                start = _start;
            }

            int _psize = new int();
            if (int.TryParse(Request.QueryString["jtPageSize"], out _psize))
            {
                pageSize = _psize;
            }

            var hits = orderRequestService.Search(form.startDate, form.endDate, start, pageSize)
                .Select(r => new { RefNumber = r.RefNumber, OrderDate = r.OrderDate.HasValue? ((DateTime)r.OrderDate).ToString("dd-MMM-yyyy"):"" });
            long total = orderRequestService.SearchCount(form.startDate, form.endDate);

            return Json(new { Result = "OK", Records = hits, TotalRecordCount = total });
        }
    }
}
