using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.UI.Models;
using SCMS.Reports;

namespace SCMS.UI.Areas.Reports.Controllers
{
    public class WBController : Controller
    {
        //
        // GET: /Reports/WB/

        public ActionResult pdf(Guid wbId)
        {
            Dictionary<String, String> data = new Dictionary<string, string>();
            ViewWayBill Model = WayBillExtension.prepareWB(wbId);

            data.Add("{REFNO}", Model.EntityWBill.RefNumber);
            data.Add("{ORIGIN-OFFICE}", Model.OrignSOfiice.Name);
            data.Add("{ORIGIN-WH}", Model.OrrignWH.Name);
            data.Add("{DESTINATION-NAME}", Model.destinationWH.Location.Name);
            data.Add("{DESTINATION-WH}", Model.destinationWH.Name);

            data.Add("{LOADING-DATE}", Model.EntityWBill.LoadingDate.Value.ToString("dd/MM/yyyy"));
            data.Add("{DISPATCH-DATE}", Model.EntityWBill.DispatchDate.Value.ToString("dd/MM/yyyy"));
            data.Add("{CONSIGNEE}", Model.consignee.ShortName);
            data.Add("{CONSIGNEE-CONTACT}", Model.consignee.OfficialPhone);


            data.Add("{WH-NO}", Model.EntityWBill.WarehouseRelease.RefNumber);
            data.Add("{DRC-VEHICLE-NO}", Model.EntityWBill.DRCVehicleRegNo);
            data.Add("{DRC-VEHICLE-KM}", Model.EntityWBill.DRCVehicleMileage.ToString("##,###"));
            data.Add("{VEHICLE-CONTACT}", Model.EntityWBill.DRCVehicleContactNo);

            int count = 1;
            String itemName, serialNo, weight = "N/A", detailHtml = "";
            foreach (var item in Model.WRItems)
            {
                if (item.Inventory.Item.ItemCategory.CategoryCode == "C")
                {
                    itemName = item.Inventory.Item.Name;
                    serialNo = item.Inventory.Item.SerialNumber;
                }
                else
                {
                    itemName = item.Asset.Name;
                    serialNo = item.Asset.SerialNumber;
                }
                count++;
                detailHtml += "<tr ><td style=\"text-align: left;\">";
                detailHtml += serialNo;
                detailHtml += "</td><td style=\"text-align: left;\">";
                detailHtml += itemName;
                detailHtml += "</td><td style=\"text-align: center;\">";
                detailHtml += item.Inventory.Item.UnitOfMeasure.Code;
                detailHtml += "</td><td style=\"text-align: right;\">";
                detailHtml += item.Inventory.Item.ItemCategory.CategoryCode == "C" ? weight : item.Asset.Weight.ToString("##,###");
                detailHtml += "</td><td style=\"text-align: right;\">";
                detailHtml += item.Quantity;
                detailHtml += "</td><td style=\"text-align: right;\">";
                detailHtml += item.QuantityReceived;
                detailHtml += "</td><td style=\"text-align: right;\">";
                detailHtml += item.QuantityDamaged;
                detailHtml += "</td><td style=\"text-align: left;\">";
                detailHtml += item.Comments;
                detailHtml += "  </td></tr>";
            }
            while (count < 8)
            {
                count++;
                detailHtml += "<tr ><td style=\"text-align: left;\">&nbsp;&nbsp;";
                detailHtml += "</td><td style=\"text-align: left;\">";
                detailHtml += "</td><td style=\"text-align: center;\">";
                detailHtml += "</td><td style=\"text-align: right;\">";
                detailHtml += "</td><td style=\"text-align: right;\">";
                detailHtml += "</td><td style=\"text-align: right;\">";
                detailHtml += "</td><td style=\"text-align: right;\">";
                detailHtml += "</td><td style=\"text-align: left;\">";
                detailHtml += "  </td></tr>";
            }
            data.Add("{DETAILS}", detailHtml);
            data.Add("{LOADING-REMARKS}", Model.EntityWBill.LoadingRemarks);

            data.Add("{DRIVER-NAME}", Model.EntityWBill.DriverName);
            data.Add("{ISSUER-STAFF}", Model.issuer.StaffName);
            data.Add("{VEHICLE-REG-NO}", Model.EntityWBill.DRCVehicleRegNo);

            data.Add("{ISSUER-TITLE}", Model.issuer.Designation);
            data.Add("{TRANSPORT-COMPANY}", Model.EntityWBill.TransportCompany);
            data.Add("{DRIVER-LICENSEE}", Model.EntityWBill.DriverLicenseNo);

            if (Model.EntityWBill.Staff != null && Model.EntityWBill.Staff.Person.SignatureImage != null)
                data.Add("{ISSUER-SIGNATURE}", "data:image/png;base64," + Convert.ToBase64String(Model.EntityWBill.Staff.Person.SignatureImage));

            data.Add("{DESTINATION-LOC}", Model.DestnationOfiice.Location.Name);
            data.Add("{ARRIVAL-DATE}", Model.EntityWBill.ArrivalDate.HasValue ? Model.EntityWBill.ArrivalDate.Value.ToString("dd/MM/yyyy") : "");

            data.Add("{RECEIVED-BY}", Model.ReceivedBy != null ? Model.ReceivedBy.StaffName : "");
            data.Add("{RECEIVED-KM}", Model.EntityWBill.RecvDRCVehicleOdometer.ToString());

            data.Add("{RECEIVED-BY-TITLE}", Model.ReceivedBy != null ? Model.ReceivedBy.Designation : "");
            data.Add("{RECEIVED-TOTAL-DISTANCE}", Model.EntityWBill.RecvDRCVehicleTotalDistance.ToString());


            if (Model.EntityWBill.Staff1 != null && Model.EntityWBill.Staff1.Person.SignatureImage != null)
                data.Add("{RECEIVEDBY-SIGNATURE}", "data:image/png;base64," + Convert.ToBase64String(Model.EntityWBill.Staff1.Person.SignatureImage));

            List<String> options = new List<string>();
            options.Add(" --copies 3 ");
            Byte[] output = WkHtml2Pdf.CreateReport(data, "Way-Bill.htm", options);
            return File(output, "application/pdf", "WB_" + DateTime.Now.FormatDDMMMYYYYHHmm());

        }
    }
}
