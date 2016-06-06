using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.Reports;
using SCMS.Model;
using System.Data.Objects.DataClasses;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic._GoodsReceivedNote;

namespace SCMS.UI.Areas.Reports.Controllers
{
    public class GoodsReceivedController : BaseController
    {
        //
        // GET: /Reports/GoodsReceived/

        public ActionResult Index(Guid id)
        {


            Dictionary<String, String> data = new Dictionary<string, string>();

            using (var db = new SCMSEntities())
            {
                Model.GoodsReceivedNote grnentity = db.GoodsReceivedNotes.FirstOrDefault(p => p.Id == id);

                data.Add("{SUPPLIER}", grnentity.Supplier.Name);
                data.Add("{GRN}", grnentity.RefNumber);

                data.Add("{DNN}", grnentity.DnNumber);

                data.Add("{OFFICE}", grnentity.CountrySubOffice.Name);
                data.Add("{LOCATION}", grnentity.CountrySubOffice.Location.Name);

                ICollection<GoodsReceivedNoteItem> grnItems = grnentity.GoodsReceivedNoteItems;

                int r = 1;
                String detailHtml = "";

                foreach (GoodsReceivedNoteItem grnitem in grnItems)
                {
                    if (r == 1)
                    {
                        data.Add("{ORNO}", grnitem.PurchaseOrderItem.PurchaseOrder.OrderRequest.RefNumber);
                        data.Add("{PONO}", grnitem.PurchaseOrderItem.PurchaseOrder.RefNumber);
                        DateTime d = (DateTime)grnitem.PurchaseOrderItem.PurchaseOrder.LatestDeliveryDate;
                        data.Add("{DELIVERY_DATE}", d.ToShortDateString());
                    }
                    detailHtml += "<tr><td align=\"center\">" + r++ + "</td><td>";
                    detailHtml += grnitem.PurchaseOrderItem.OrderRequestItem.Item.Name;
                    detailHtml += "</td><td>";
                    detailHtml += grnitem.PurchaseOrderItem.OrderRequestItem.Item.UnitOfMeasure.Code;
                    detailHtml += "</td><td class='central'>";
                    detailHtml += grnitem.PurchaseOrderItem.Quantity.ToString("##,###");
                    detailHtml += "</td><td class='central'>";
                    detailHtml += grnitem.QuantityDelivered.ToString("##,###");
                    detailHtml += "</td><td class='central'>";
                    detailHtml += grnitem.QuantityDamaged.ToString("##,###");
                    detailHtml += "</td><td>";
                    detailHtml += grnitem.Comments;
                }
                while (r < 21)
                {
                    detailHtml += "<tr><td align=\"center\">" + r++ + "</td><td>";
                    detailHtml += "</td><td>";
                    detailHtml += "</td><td class='central'>";
                    detailHtml += "</td><td class='central'>";
                    detailHtml += "</td><td class='central'>";
                    detailHtml += "</td><td>";
                }
                data.Add("{DETAILS}", detailHtml);
                data.Add("{ADDITONALRMARKS}", grnentity.Remarks);

                var recBy = db.VStaffDetails.FirstOrDefault(p => p.StaffID == grnentity.ReceivedBy);

                if (grnentity.DeliverDate.HasValue)
                {
                    if (recBy.SignatureImage.IsNotNull())
                        data.Add("{SIGNATURE2}", string.Format("<img src=\"{0}\" alt=\"\" style=\"max-width: 80px;\" />", "data:image/png;base64," + Convert.ToBase64String(recBy.SignatureImage)));
                    data.Add("{SIGNATURE_DATE}", grnentity.DeliverDate.FormatDDMMMYYYY());
                }
                else
                {
                    data.Add("{SIGNATURE_DATE}", "N/A");
                }
                data.Add("{TRANSPORTER_NAME}", grnentity.DeliveredBy);
                data.Add("{TRANSPORTER_COMPANY}", grnentity.DeliveryCompany);
                data.Add("{DRIVER_ID}", grnentity.DriverLicenseNumber);
                data.Add("{VEHICLE_PLATE}", grnentity.VehicleRegNumber);

                data.Add("{RECEIVED_BY}", recBy.StaffName);
                data.Add("{TITLE}", recBy.Designation);
            }
            List<String> options = new List<string>();
            options.Add(" --copies 2 ");
            Byte[] output = WkHtml2Pdf.CreateReport(data, "goods-received-note.html", options);

            return File(output, "application/pdf", "GRN_" + DateTime.Now.FormatDDMMMYYYYHHmm());
        }

        public ActionResult GRNTemplate(Model.GoodsReceivedNote model)
        {
            Dictionary<String, String> data = new Dictionary<string, string>();

            using (var db = new SCMSEntities())
            {
                Model.GoodsReceivedNote grnentity = model;

                Model.PurchaseOrder po = db.PurchaseOrders.FirstOrDefault(p => p.Id == model.PurchaseOrderId);
                CountrySubOffice csof = db.CountrySubOffices.FirstOrDefault(p => p.Id == model.CountryOfficeId);

                data.Add("{SUPPLIER}", po != null ? po.Supplier.Name : "<sub>________________________________________</sub>");
                data.Add("{GRN}", "<sub>________________________________________</sub>");
                //data.Add("{GRN}", grnentity.RefNumber.IsNotNullOrEmpty() ? grnentity.RefNumber : "<sub>________________________________________</sub>");

                data.Add("{DNN}", grnentity.DnNumber ?? "<sub>________________________________________</sub>");

                data.Add("{OFFICE}", csof != null ? csof.Name : "<sub>________________________________________</sub>");
                data.Add("{LOCATION}", csof != null ? csof.Location.Name : "<sub>________________________________________</sub>");

                data.Add("{ORNO}", po != null ? po.OrderRequest.RefNumber : "<sub>________________________________________</sub>");
                data.Add("{PONO}", po != null ? po.RefNumber : "<sub>________________________________________</sub>");
                data.Add("{DELIVERY_DATE}", "<sub>________________________________________</sub>");

                int r = 1;
                String detailHtml = "";

                foreach (POItemsView poItem in model.POItemz)
                {

                    detailHtml += "<tr><td align=\"center\">" + r++ + "</td><td>";
                    detailHtml += poItem.ItemName;
                    detailHtml += "</td><td>";
                    detailHtml += poItem.unitOfMessure;
                    detailHtml += "</td><td class='central'>";
                    detailHtml += poItem.QtyOrdered.ToString("##,###");
                    detailHtml += "</td><td class='central'>";
                    detailHtml += poItem.QtyDelivered.ToString("##,###");
                    detailHtml += "</td><td class='central'>";
                    detailHtml += poItem.QtyDamaged.ToString("##,###");
                    detailHtml += "</td><td>";
                    detailHtml += poItem.comments;
                }
                while (r < 21)
                {
                    detailHtml += "<tr><td align=\"center\">" + r++ + "</td><td>";
                    detailHtml += "</td><td>";
                    detailHtml += "</td><td class='central'>";
                    detailHtml += "</td><td class='central'>";
                    detailHtml += "</td><td class='central'>";
                    detailHtml += "</td><td>";
                }
                data.Add("{DETAILS}", detailHtml);

                var recBy = db.VStaffDetails.FirstOrDefault(p => p.StaffID == grnentity.ReceivedBy);


                data.Add("{SIGNATURE2}", "<sub>___________________________________________</sub>");
                data.Add("{SIGNATURE_DATE}", grnentity.DeliverDate.Value.ToShortDateString());

                data.Add("{TRANSPORTER_NAME}", grnentity.DeliveredBy.IsNotNullOrWhiteSpace() ? grnentity.DeliveredBy : "<sub>_________________________________________</sub>");
                data.Add("{TRANSPORTER_COMPANY}", grnentity.DeliveryCompany.IsNotNullOrEmpty() ? grnentity.DeliveryCompany : "<sub>_____________________________________</sub>");
                data.Add("{DRIVER_ID}", grnentity.DriverLicenseNumber.IsNotNullOrEmpty() ? grnentity.DriverLicenseNumber : "<sub>________________________________________</sub>");
                data.Add("{VEHICLE_PLATE}", grnentity.VehicleRegNumber.IsNotNullOrEmpty() ? grnentity.VehicleRegNumber : "<sub>_________________________________________</sub>");

                data.Add("{RECEIVED_BY}", recBy != null ? recBy.StaffName : "<sub>_______________________________________________</sub>");
                data.Add("{TITLE}", recBy != null ? recBy.Designation : "<sub>____________________________________________________</sub>");
            }

            Byte[] output = WkHtml2Pdf.CreateReport(data, "goods-received-note.html");

            return File(output, "application/pdf", "GRN_Template_" + DateTime.Now.FormatDDMMMYYYYHHmm());
        }


    }
}
