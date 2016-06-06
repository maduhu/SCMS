using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.CoreBusinessLogic.GoodsIssuedVoucher;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.WRF;
using SCMS.Model;
using SCMS.Resource;
using Telerik.Web.Mvc;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class GoodsIssuedVoucherController : PortalBaseController
    {
        private IGoodsIssuedVoucherService givservice;
        private static GoodsIssuedVoucher modo;
        public GoodsIssuedVoucherController(IPermissionService permissionService, IUserContext userContext, IGoodsIssuedVoucherService givservice)
            : base(userContext, permissionService)
        {
            this.givservice = givservice;

        }
        //
        // GET: /GoodsIssuedVoucher/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGiv(Guid? roId = null)
        {
            var modle = new GoodsIssuedVoucher()
            {
                ReleaseOrders = new SelectList(givservice.GetGIVROs(), "Id", "RefNumber"),
                Staffs = new SelectList(SessionData.CurrentSession.StaffList, "Id", "StaffName"),
                RefNumber = string.Format("--{0}--", Resources.GoodsIssuedVoucher_LoadGiv___NewGiv___)
            };
            if (roId == null) modle.ROItems = new List<GivItemz>();
            else
            {
                modle.ROItems = givservice.GetROItemsToAdd((Guid)roId);
                modle.WareHouseReleaseId = (Guid)roId;
            }
            modo = modle;
            return View("LoadGiv", modle);
        }

        public ActionResult RemoveItem(Guid roItemId, string ViewName)
        {
            var item = modo.ROItems.FirstOrDefault(p => p.ROItemId == roItemId);
            modo.ROItems.Remove(item);
            return View(ViewName, modo);
        }

        public ActionResult AddItems(Guid givId, Guid roId)
        {
            var modle = new GoodsIssuedVoucher()
            {
                ROItems = givservice.GetROItemsToAdd(roId),
                Id = givId
            };
            modo = modle;
            return View(modle);
        }

        public ActionResult SaveItems(GoodsIssuedVoucher model)
        {
            givservice.IsGIVItemAdded(model);
            return ViewGIVDetails(model.Id);

        }

        [HttpGet]
        public JsonResult IsQTYInRange(Int64 QTYReceived, Int64 QTYReleased)
        {
            var test = QTYReceived > QTYReleased && QTYReceived < QTYReleased;
            return Json(test, JsonRequestBehavior.AllowGet);
            //http://edspencer.me.uk/2011/07/19/using-asp-net-mvc-remote-validation-the-real-world-additionalfields/
        }

        public ActionResult SaveGIV(Model.GoodsIssuedVoucher entity)
        {
            entity.CountryProgrammeId = countryProg.Id;
            entity.Id = Guid.NewGuid();
            entity.PreparedBy = currentStaff.Id;
            entity.PreparedOn = DateTime.Now;
            entity.IsSubmitted = false;
            var isSaved = givservice.IsGIVSaved(entity);
            return ViewGIVDetails(entity.Id);
        }
        //[HttpPost, ActionName("")]
        public ActionResult LoadEditGIV(Guid GIVId)
        {
            var model = givservice.GetGIVs().FirstOrDefault(p => p.Id == GIVId);
            model.Staffs = new SelectList(SessionData.CurrentSession.StaffList, "Id", "StaffName");
            model.ROItems = model.GoodsIssuedVoucherItems.Select(k => new GivItemz
            {
                GIVItemId = k.Id,
                ItemName = k.WarehouseReleaseItem.Inventory.Item.Name,
                Unit = k.WarehouseReleaseItem.Inventory.Item.UnitOfMeasure.Code,
                QTYReleased = (Int64)k.WarehouseReleaseItem.Quantity,
                QTYReceived = k.QTYDelivered,
                Remarks = k.Remarks
            }).ToList<GivItemz>();
            return View(model);
        }

        public ActionResult EditGIV(Model.GoodsIssuedVoucher model)
        {
            var giv = givservice.GetGIVs().FirstOrDefault(p => p.Id == model.Id);
            giv.WaybillNo = model.WaybillNo;
            giv.TransporterVehiclePlateNo = model.TransporterVehiclePlateNo;
            giv.Consignee = model.Consignee;
            giv.Receivedby = model.Receivedby;
            giv.ROItems = model.ROItems;
            var isSaved = givservice.IsGIVEdited(giv);
            return ViewGIVDetails(model.Id);
        }

        public ActionResult ViewGIVs()
        {
            return View("ViewGIVs", givservice.GetGIVs());
        }

        public ActionResult DeleteGIV(Guid GIVId)
        {
            givservice.IsGIVDeleted(GIVId);
            return ViewGIVs();
        }

        public ActionResult DeleteGivItem(Guid givItemId, Guid givId)
        {
            givservice.IsGIVItemDeleted(givItemId);
            return ViewGIVDetails(givId);
        }

        [GridAction]
        public JsonResult Filtering()
        {
            return Json(givservice.GetGIVs().Select(n => new
            {
                Id = n.Id,
                RefNumber = n.RefNumber,
                RONumber = n.RONumber,
                WaybillNo = n.WaybillNo,
                Consignee = n.Consignee,
                Status = n.Status,
                PreparedOn = n.PreparedOn
            }),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewGIVDetails(Guid givId)
        {
            return View("ViewGIVDetails", givservice.GetGIVs().FirstOrDefault(p => p.Id == givId));
        }

        public ActionResult SubmitGIV(Guid Id)
        {
            var giv = givservice.GetGIVs().FirstOrDefault(p => p.Id == Id);
            giv.PreparedOn = DateTime.Now;
            giv.IsSubmitted = true;
            giv.PreparedBy = currentStaff.Id;
            giv.RefNumber = givservice.GenerateUniquNumber(countryProg);
            givservice.IsGIVSubmited(giv);
            return ViewGIVs();
        }
    }
}
