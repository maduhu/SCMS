using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.UI.Models;
using SCMS.Model;
using SCMS.CoreBusinessLogic.WRF;
using SCMS.CoreBusinessLogic.WB;
using System.Text;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.Resource;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.CoreBusinessLogic.StaffServices;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class WayBillController : PortalBaseController
    {
        private IWareHouseReleaseService WRFservice;
        private readonly IStaffService staffService;
        private IWayBillService WBService;

        public WayBillController(IPermissionService permissionService, IUserContext userContext,
            IStaffService staffService, IWareHouseReleaseService WRFservice, IWayBillService WBService)
            : base(userContext, permissionService)
        {
            this.WRFservice = WRFservice;
            this.WBService = WBService;
            this.staffService = staffService;
        }
        //
        // GET: /WayBill/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadWB()
        {
            return WBlod();
        }

        private ActionResult WBlod()
        {
            var model = new WBill();

            model.EntityWBill = new WayBill();
            model.WRNs = new SelectList(new List<WarehouseRelease>(), "Id", "RefNumber");
            model.Warehouses = new SelectList(SessionData.CurrentSession.WarehouseList, "Id", "Name");
            model.Staff = new SelectList(staffService.GetStaffByCountryProgramme(countryProg.Id), "StaffId", "StaffName");
            model.IssuingOffice = new SelectList(WBService.GetSubOffices(countryProg.Id), "Id", "Name");
            model.EntityWBill.RefNumber = string.Format("--{0}--", Resources.Global_String_NewWB);

            return View("LoadWB", model);
        }

        public ActionResult SaveWBill(WBill modelEntity)
        {
            if (this.IsWRNAdded(modelEntity.EntityWBill.WRNId))
            {
                modelEntity.EntityWBill.IsReceived = false;
                modelEntity.EntityWBill.Id = Guid.NewGuid();
                modelEntity.EntityWBill.CountryProgrammeId = countryProg.Id;
                modelEntity.EntityWBill.PreparedBy = currentStaff.Id;
                modelEntity.EntityWBill.PreparedOn = DateTime.Now;
                modelEntity.EntityWBill.RefNumber = WBService.GenerateUniquNumber(countryProg);
                if (WBService.SaveWB(modelEntity.EntityWBill))
                {
                    ViewBag.Response = 1;
                    ViewBag.msg = Resources.Global_String_ProcessCompleted;
                }
                ModelState.Clear();
            }
            else { ViewBag.Response = 0; ViewBag.msg = Resources.WayBillController_String_WayBillAreadyCreated; return WBlod(); }

            return LoadWBindex();
        }

        private bool IsWRNAdded(Guid WRNId)
        {
            using (var db = new SCMSEntities())
            {
                int vv = db.WayBills.Where(p => p.WRNId == WRNId).Count();
                return vv == 0 ? true : false;
            }

        }

        public ActionResult GetWBItems(Guid id)
        {

            using (var db = new SCMSEntities())
            {
                WarehouseRelease wrn = db.WarehouseReleases.SingleOrDefault(p => p.Id == id);

                StringBuilder blineOption = new StringBuilder();
                blineOption.Append("<table width=\"95%\" align=\"center\" cellspacing=\"0\" cellpadding=\"0\" border=\"1\" style=\"border: 1px solid grey;");
                blineOption.Append("border-collapse: collapse; padding: 5px; font-size: 12px;\">");
                blineOption.Append("<tr class=\"gridheader\"><th colspan=\"6\" align=\"left\"><h4>" + Resources.WayBill_LoadWB_LOADINGDISPATCHDETAILS + "</h4></th></tr>");
                blineOption.Append("<tr><th style=\"width: 5%; text-align: center;\"></th>");
                blineOption.Append("<th style=\"width: 15%; text-align: center;\">" + Resources.WayBillController_String_ItemSerialNo + "</th>");
                blineOption.Append("<th style=\"width: 28%; text-align: center;\">" + Resources.WayBill_LoadWB_EquipmentitemCommodity + "</th>");
                blineOption.Append("<th style=\"width: 10%; text-align: center;\">" + Resources.Global_String_Unit + "</th>");
                blineOption.Append("<th style=\"width: 17%; text-align: center;\">" + Resources.Global_String_Weight + "</th>");
                blineOption.Append("<th style=\"width: 17%; text-align: center;\">" + Resources.WayBill_LoadWB_QtyDispatched + "</th></tr>");

                if (wrn != null)
                {
                    Int64 count = 1;
                    foreach (WarehouseReleaseItem item in wrn.WarehouseReleaseItems.ToList())
                    {
                        blineOption.Append("<tr><td align=\"center\">" + count + "</td>");
                        if (item.Inventory.Item.ItemCategory.CategoryCode == "C")
                            blineOption.Append("<td>" + item.Inventory.Item.SerialNumber + "</td>");
                        else blineOption.Append("<td>" + db.Assets.First(p => p.Id == item.AssetId).SerialNumber + "</td>");
                        if (item.Inventory.Item.ItemCategory.CategoryCode == "C")
                            blineOption.Append("<td>" + item.Inventory.Item.Name + "</td>");
                        else blineOption.Append("<td>" + db.Assets.First(p => p.Id == item.AssetId).Name + "</td>");
                        blineOption.Append("<td>" + item.Inventory.Item.UnitOfMeasure.Code + "</td>");
                        if (item.Inventory.Item.ItemCategory.CategoryCode == "C")
                            blineOption.Append("<td></td>");
                        else blineOption.Append("<td>" + item.Asset.Weight + "</td>");
                        blineOption.Append("<td>" + item.Quantity + "</td></tr>");
                        count++;
                    }
                }

                blineOption.Append("</table>");
                ViewBag.Html = blineOption.ToString();
                return View("HTMLResponses");
            }
        }

        public ActionResult GetWBReceivdItems(Guid id)
        {
            using (var db = new SCMSEntities())
            {
                WayBill wb = db.WayBills.First(p => p.Id == id);
                WarehouseRelease wrn = db.WarehouseReleases.FirstOrDefault(p => p.Id == wb.WRNId);
                StringBuilder blineOption = new StringBuilder();
                blineOption.Append("<table width=\"98%\" align=\"center\" cellspacing=\"0\" cellpadding=\"0\" border=\"1\" style=\"border: 1px solid grey;");
                blineOption.Append("border-collapse: collapse; padding: 5px; font-size: 12px;\">");
                blineOption.Append("<tr class=\"gridheader\"><th colspan=\"8\" align=\"left\"><h4>" + Resources.WayBill_ViewWBDetails_WayBillDetails + "</h4></th></tr>");
                blineOption.Append("<tr><th style=\"width: 5%; text-align: center;\"></th>");
                blineOption.Append("<th style=\"width: 15%; text-align: center;\">" + Resources.WayBillController_String_ItemSerialNo + "</th>");
                blineOption.Append("<th style=\"width: 20%; text-align: center;\">" + Resources.WayBill_LoadWB_EquipmentitemCommodity + "</th>");
                blineOption.Append("<th style=\"width: 8%; text-align: center;\">" + Resources.Global_String_Unit + "</th>");
                blineOption.Append("<th style=\"width: 12%; text-align: center;\">" + Resources.Global_String_Weight + "</th>");
                blineOption.Append("<th style=\"width: 12%; text-align: center;\">" + Resources.WayBill_LoadWB_QtyDispatched + "</th>");
                blineOption.Append("<th style=\"width: 10%; text-align: center;\">" + Resources.Global_String_QtyReceived + "</th>");
                blineOption.Append("<th style=\"width: 10%; text-align: center;\">" + Resources.Global_String_QtyDamaged + "</th></tr>");

                if (wrn != null)
                {
                    Int64 count = 1, cunt2 = 0;
                    foreach (WarehouseReleaseItem item in wrn.WarehouseReleaseItems.ToList())
                    {
                        blineOption.Append("<tr><td align=\"center\">" + count + "");
                        blineOption.Append("<input id=\"WRItems_" + cunt2 + "__Id\" name=\"WRItems[" + cunt2 + "].Id\" type=\"hidden\" value=\"" + item.Id + "\" /></td>");
                        if (item.Inventory.Item.ItemCategory.CategoryCode == "C")
                            blineOption.Append("<td>" + item.Inventory.Item.SerialNumber + "</td>");
                        else blineOption.Append("<td>" + db.Assets.First(p => p.Id == item.AssetId).SerialNumber + "</td>");
                        if (item.Inventory.Item.ItemCategory.CategoryCode == "C")
                            blineOption.Append("<td>" + item.Inventory.Item.Name + "</td>");
                        else blineOption.Append("<td>" + db.Assets.First(p => p.Id == item.AssetId).Name + "</td>");
                        blineOption.Append("<td>" + item.Inventory.Item.UnitOfMeasure.Code + "</td>");
                        if (item.Inventory.Item.ItemCategory.CategoryCode == "C")
                            blineOption.Append("<td>N/A</td>");
                        else blineOption.Append("<td>" + item.Asset.Weight + "</td>");
                        blineOption.Append("<td>" + item.Quantity + "</td>");

                        blineOption.Append("<td><input class=\"WBnumericTb\" data-val=\"true\" data-val-number=\"" + Resources.Global_String_InvalidValue + "\" ");
                        blineOption.Append("data-val-required=\"" + Resources.Global_String_Required + "\" id=\"WRItems_" + cunt2 + "__QuantityReceived\" name=\"WRItems[" + cunt2 + "].QuantityReceived\" type=\"text\" value=\"\" />");
                        blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"WRItems[" + cunt2 + "].QuantityReceived\" data-valmsg-replace=\"true\"></span></td>");

                        blineOption.Append("<td><input class=\"WBnumericTb\" data-val=\"true\" data-val-number=\"" + Resources.Global_String_InvalidValue + "\" ");
                        blineOption.Append("data-val-required=\"" + Resources.Global_String_Required + "\" id=\"WRItems_" + cunt2 + "__QuantityDamaged\" name=\"WRItems[" + cunt2 + "].QuantityDamaged\" type=\"text\" value=\"\" />");
                        blineOption.Append(" <span class=\"field-validation-valid\" data-valmsg-for=\"WRItems[" + cunt2 + "].QuantityDamaged\" data-valmsg-replace=\"true\"></span></td></tr>");

                        count++; cunt2++;
                    }
                }

                blineOption.Append("</table>");
                ViewBag.Html = blineOption.ToString();
                return View("HTMLResponses");
            }
        }

        public ActionResult WBReceive()
        {
            using (var db = new SCMSEntities())
            {
                WarehouseRelease wrn = db.WarehouseReleases.FirstOrDefault<WarehouseRelease>();
                List<WarehouseReleaseItem> itmz = wrn != null ? wrn.WarehouseReleaseItems.ToList() : new List<WarehouseReleaseItem>();
                foreach (WarehouseReleaseItem item in itmz)
                {
                    Model.Inventory iv = item.Inventory;
                    Model.Item it = iv.Item;
                    Model.ItemCategory itc = it.ItemCategory;
                    Model.Asset ast = item.Asset;
                    Model.UnitOfMeasure u = it.UnitOfMeasure;
                }
                //Use this to populate items on load for any changes in the stolen code
                //WayBill wb = db.WayBills.First(p => p.Id == new Guid("0D88562F-BBEC-4722-941B-91FC4D82BB1C"));
                //List<WarehouseReleaseItem> wrnn = db.WarehouseReleases.FirstOrDefault(p => p.Id == wb.WRNId).WarehouseReleaseItems.ToList();
                //foreach (WarehouseReleaseItem item in wrnn)
                //{
                //    Model.Asset ass = item.Asset;
                //    Model.Inventory iv = item.Inventory;
                //    Model.Item it = iv.Item;
                //    Model.ItemCategory ic = it.ItemCategory;
                //    Model.UnitOfMeasure u = it.UnitOfMeasure;
                //}

                var model = new ReceiveWB()
                {
                    EntityWBill = new WayBill(),
                    IssuingOffice = new SelectList(WBService.GetSubOffices(countryProg.Id), "Id", "Name"),
                    WBlist = new SelectList(WBService.GetWayBillsNotReceived(countryProg.Id), "Id", "RefNumber"),
                    WRItems = new List<WarehouseReleaseItem>()//wrnn
                };
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult WBReceive(Models.ReceiveWB modelEntity)
        {
            modelEntity.EntityWBill.Id = modelEntity.EntityWBill.Id;
            modelEntity.EntityWBill.ReceivedBy = currentStaff.Id;
            modelEntity.EntityWBill.IsReceived = true;
            if (!WBService.ReceiveWB(modelEntity.EntityWBill, modelEntity.WRItems))
            {
                ViewBag.Response = 0;
                ViewBag.msg = Resources.Global_String_AnErrorOccurred;
            }
            ViewBag.Response = 1;
            ViewBag.msg = Resources.WayBillController_String_ReceivedSuccessfully;
            ModelState.Clear();

            return LoadWBindex();
        }

        public ActionResult ViewWBill()
        {
            return LoadWBindex();
        }

        private ActionResult LoadWBindex()
        {
            using (var db = new SCMSEntities())
            {
                ViewWayBill model;
                List<ViewWayBill> wblist = new List<ViewWayBill>();
                foreach (WayBill item in db.WayBills.Where(w => w.CountryProgrammeId == countryProg.Id).ToList())
                {
                    model = new ViewWayBill();
                    model.EntityWBill = item;
                    model.issuer = db.VStaffDetails.First(p => p.StaffID == item.PreparedBy);
                    model.consignee = db.VStaffDetails.First(p => p.StaffID == item.Consignee);
                    model.OrrignWH = db.WareHouses.First(p => p.Id == item.IssuerWarehouse);
                    model.destinationWH = db.WareHouses.First(p => p.Id == item.DestinationWarehouse);
                    model.OrignSOfiice = db.CountrySubOffices.First(p => p.Id == item.IssuingOffice);
                    wblist.Add(model);
                }
                return View("ViewWBill", wblist);
            }
        }

        public ActionResult ViewWBDetails(Guid id)
        {
            return View("ViewWBDetails", WayBillExtension.prepareWB(id));
        }
    }
}
