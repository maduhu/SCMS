using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Reports;
using System.Data.Objects.DataClasses;
using SCMS.Model;
using SCMS.CoreBusinessLogic.PurchaseOrder;
using SCMS.CoreBusinessLogic.Security;
using SCMS.Utils.DTOs;
using SCMS.CoreBusinessLogic.StaffServices;
using System.IO;
using OfficeOpenXml;
using System.Drawing;
using SCMS.Utils.File;
using OfficeOpenXml.Drawing;
using SCMS.Utils;

namespace SCMS.UI.Areas.Reports.Controllers
{
    [MyException]
    public class POController : PortalBaseController
    {

        private readonly IPurchaseOrderService POSvc;
        private readonly IStaffService staffSvc;

        public POController(IUserContext userContext, IPermissionService permissionService, IPurchaseOrderService _POSvc, IStaffService _staffSvc)
            : base(userContext, permissionService)
        {
            this.POSvc = _POSvc;
            this.staffSvc = _staffSvc;
        }
        //
        // GET: /Reports/PO/

        public ActionResult Index(Guid POid)
        {
            Dictionary<String, String> data = new Dictionary<string, string>();

            using (var db = new SCMSEntities())
            {
                Model.PurchaseOrder POentity = db.PurchaseOrders.FirstOrDefault(p => p.Id == POid);
                string taNumber = "Single Quote";
                data.Add("{DATE}", POentity.PODate.ToString("dd/MM/yyyy"));
                data.Add("{PONo}", POentity.RefNumber);
                data.Add("{TANo}", taNumber);
                data.Add("{ORNo}", POentity.OrderRequest.RefNumber);

                data.Add("{PAYMENT-TERM}", POentity.PaymentTerm.Description);
                data.Add("{QUOTATION-REF}", POentity.QuotationRef);
                data.Add("{SHIPPINGTERM}", POentity.ShippingTerm.Description);
                data.Add("{PO-CURRENCY}", POentity.Currency.ShortName);

                data.Add("{TO-SUPPLIER}", POentity.Supplier.Name);
                data.Add("{DELIVER-ADDRESS}", POentity.Location.Name);
                data.Add("{DELIVERY-DATE}", POentity.LatestDeliveryDate.ToString("dd/MM/yyyy"));
                data.Add("{DRC-REMARKS}", POentity.Remarks);

                ICollection<PurchaseOrderItem> POItems = POentity.PurchaseOrderItems;

                int count = 1;
                String detailHtml = "";

                foreach (PurchaseOrderItem poitem in POItems)
                {
                    detailHtml += "<tr><td>" + count++ + "</td><td>";
                    detailHtml += poitem.OrderRequestItem.Item.Name;
                    detailHtml += "</td><td>";
                    detailHtml += poitem.OrderRequestItem.Item.UnitOfMeasure.Code;
                    detailHtml += "</td><td class='central'>";
                    detailHtml += poitem.Quantity.ToString("##,##0");
                    detailHtml += "</td><td class='central'>";
                    detailHtml += poitem.UnitPrice.ToString("#,##0.00");
                    detailHtml += "</td><td class='central'>";
                    detailHtml += poitem.TotalPrice.ToString("#,##0.00");
                    detailHtml += "</td><td>";
                    detailHtml += poitem.ProjectBudget.BudgetCategory.ProjectDonor.ProjectNumber;
                    detailHtml += "</td><td>";
                    detailHtml += db.ProjectBudgets.FirstOrDefault(p => p.Id == poitem.BudgetLineId).LineNumber;
                    detailHtml += "</td><td>";
                    detailHtml += poitem.Remarks;
                    detailHtml += "</td><tr>";
                }
                while (count < 18)
                {
                    detailHtml += "<tr><td>" + count++ + "</td><td>";
                    detailHtml += "</td><td>";
                    detailHtml += "</td><td class='central'>";
                    detailHtml += "</td><td class='central'>";
                    detailHtml += "</td><td class='central'>";
                    detailHtml += "</td><td>";
                    detailHtml += "</td><td>";
                    detailHtml += "</td><td>";
                    detailHtml += "</td><tr>";
                }
                data.Add("{DETAILS}", detailHtml);

                data.Add("{CURRENCY}", POentity.Currency.ShortName);
                data.Add("{TOTAL-AMOUNT}", POentity.TotalAmount.Value.ToString("#,##0.00"));
                data.Add("{ADDITIONAL-REMARKS}", POentity.AdditionalRemarks);

                String authourisation = "";
                Model.Staff s = db.Staffs.SingleOrDefault(p => p.Id == POentity.PreparedBy);
                authourisation = AuthName(db, POentity, authourisation, s);
                authourisation = AuthTitle(db, POentity, authourisation, s);
                authourisation = AuthDate(db, POentity, authourisation);
                authourisation = AuthSignature(db, POentity, authourisation, s);
                data.Add("{AUTHORISATION}", authourisation);

                data.Add("{PRO-FORMA}", string.Format("<input type=\"checkbox\" {0} />", POentity.ProformaRequired ? "value=\"\" checked=\"checked\"" : ""));
                data.Add("{COMMERCIAL}", string.Format("<input type=\"checkbox\" {0} />", POentity.CommercialInvoiceRequired ? "value=\"\" checked=\"checked\"" : ""));
                data.Add("{WB-AB}", string.Format("<input type=\"checkbox\" {0} />", POentity.WayBillRequired ? "value=\"\" checked=\"checked\"" : ""));
                data.Add("{EPACKING}", string.Format("<input type=\"checkbox\" {0} />", POentity.PackingListRequired ? "value=\"\" checked=\"checked\"" : ""));
                data.Add("{DELIVERYNN}", string.Format("<input type=\"checkbox\" {0} />", POentity.DeliveryNoteRequired ? "value=\"\" checked=\"checked\"" : ""));
                data.Add("{MANUAL}", string.Format("<input type=\"checkbox\" {0} />", POentity.ManualsRequired ? "value=\"\" checked=\"checked\"" : ""));
                data.Add("{CERTIFICATES}", string.Format("<input type=\"checkbox\" {0} />", POentity.CertificatesReqired ? "value=\"\" checked=\"checked\"" : ""));
                data.Add("{OTHERS}", string.Format("<input type=\"checkbox\" {0} />", POentity.OtherRequired ? "value=\"\" checked=\"checked\"" : ""));
                data.Add("{SPECIFY}", POentity.OtherSpecify);


                data.Add("{SHIPING-MARKS}", POentity.ShippingMarks);
                data.Add("{CONSIGNEE-ADDRESS}", POentity.ConsigneeAddress);
                data.Add("{CONSIGNEE-EMAIL}", POentity.ConsigneeEmail1);
                data.Add("{CONSIGNEE-EMANIL2}", POentity.ConsigneeEmail2);

                data.Add("{PRE-FINANCING}", string.Format("<input type=\"checkbox\" {0} />", POentity.PrefinancingGuaranteeRequired ? "value=\"\" checked=\"checked\"" : ""));
                data.Add("{PERCENTAGE-VALUE}", POentity.PFGPercentage != null ? POentity.PFGPercentage.Value.ToString("#,##0.00") + "%" : " ");

            }
            List<String> options = new List<string>();
            options.Add(" --copies 3 ");
            Byte[] output = WkHtml2Pdf.CreateReport(data, "Purchase-Order.htm", options);

            return File(output, "application/pdf", "PO_" + DateTime.Now.FormatDDMMMYYYYHHmm());
        }


        public ActionResult SelectedPurchaseOrdersPdf()
        {
            string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (_idList.Length > 0)
            {
                List<Guid> idList = new List<Guid>();

                foreach (String i in _idList)
                {
                    idList.Add(new Guid(i));
                }
                List<PurchaseOrderSummary> purchases = POSvc.Find(idList);

                String html = "<table class='summary default black-borders goods-detail'><thead><tr><th>#</th>";
                html += "<th>PO No.</th><th>OR Number</th><th>Supplier</th><th>Delivery Date</th>";
                html += "<th>Delivery Address</th><th>PO Value</th><th>Status</th><th>Status Date</th></tr></thead><tbody>";

                int c = 1;


                foreach (PurchaseOrderSummary p in purchases)
                {
                    html += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td></tr>",
                        c, p.PONumber, p.OrderRequestRefNumber, p.Supplier, p.DeliveryDate.ToString("dd.MM.yyyy"), p.DeliveryAddress, p.POValue.ToString("#,##0.00"),
                        p.Status, p.StatusDate.ToString("dd/MM/yyyy"));
                    c++;
                }

                html += "</tbody></table>";
                Dictionary<String, String> data = new Dictionary<string, string>();
                data.Add("{DETAILS}", html);
                data.Add("{REPORT_DATE}", DateTime.Now.ToString("dd.MM.yyyy"));
                data.Add("{REPORT_TITLE}", "Selected Purchase Orders");



                String fileName = WkHtml2Pdf.CreatePersistedReport(data, "Summary-Purchase-Requests.htm");

                return Content("/Content/tmp_reports/" + fileName);
            }

            return Content("#N/A");
        }

        public ActionResult SelectedPurchaseOrdersExcel()
        {

            string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (_idList.Length > 0)
            {
                List<Guid> idList = new List<Guid>();

                foreach (String i in _idList)
                {
                    idList.Add(new Guid(i));
                }
                List<PurchaseOrderSummary> purchases = POSvc.Find(idList);


                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage())
                    {
                        var ws = xlPackage.Workbook.Worksheets.Add("Purchase Orders");
                        ws.SetValue(2, 1, "Supply Chain Management Report");

                        ws.SetValue(4, 1, "Report:");
                        ws.SetValue(6, 1, "Date:");

                        ws.SetValue(4, 2, "Selected Purchase Orders");
                        ws.SetValue(6, 2, DateTime.Now.ToString("dd.MM.yyyy"));

                        int row = 10;
                        ws.SetValue(row, 1, "PO No.");
                        ws.SetValue(row, 2, "OR No.");
                        ws.SetValue(row, 3, "Supplier");
                        ws.SetValue(row, 4, "Delivery Date");
                        ws.SetValue(row, 5, "Delivery Address");
                        ws.SetValue(row, 6, "PO Value");
                        ws.SetValue(row, 7, "Status");
                        ws.SetValue(row, 8, "Status Date");

                        ws.Cells[2, 1, 2, 7].Merge = true; //Merge columns start and end range
                        ws.Cells[2, 1, 10, 7].Style.Font.Bold = true;

                        ws.Column(6).Style.Numberformat.Format = "#,##0.00";

                        row++;
                        foreach (PurchaseOrderSummary p in purchases)
                        {
                            ws.SetValue(row, 1, p.PONumber);
                            ws.SetValue(row, 2, p.OrderRequestRefNumber);
                            ws.SetValue(row, 3, p.Supplier);
                            ws.SetValue(row, 4, p.DeliveryDate.ToString("dd.MM.yyyy"));
                            ws.SetValue(row, 5, p.DeliveryAddress);
                            ws.SetValue(row, 6, p.POValue);
                            ws.SetValue(row, 7, p.Status);
                            ws.SetValue(row, 8, p.StatusDate.ToString("dd.MM.yyyy"));

                            row++;
                        }

                        Bitmap image = ImageUtility.GetLogosImage();
                        ExcelPicture picture = null;

                        if (image != null)
                        {
                            picture = ws.Drawings.AddPicture("logo-" + (new Guid()).ToString(), image);
                            picture.From.Column = 5;
                            picture.From.Row = 2;
                            picture.SetSize(91, 90);
                        }

                        for (int c = 1; c < 9; c++)
                        {
                            ws.Column(c).AutoFit();
                        }

                        xlPackage.SaveAs(stream);
                    }


                    String tmpFilename = Guid.NewGuid().ToString() + ".xlsx";

                    String fullExcelFilePath = Server.MapPath(Url.Content("~/Content/tmp_reports/")) + tmpFilename;

                    FileStream eFile = new FileStream(fullExcelFilePath, FileMode.Create);

                    stream.WriteTo(eFile);
                    eFile.Close();
                    return Content("/Content/tmp_reports/" + tmpFilename);
                }

            }

            return Content("#N/A");
        }


        private String AuthSignature(SCMSEntities db, Model.PurchaseOrder POentity, String authourisation, Model.Staff s)
        {
            authourisation += "<tr><td class=\"gridheader\">Signature:</td><td>";
            if (s.Person.SignatureImage != null)
                authourisation += string.Format("<img src=\"{0}\" alt=\"\" style=\"max-width: 100px;\" />", "data:image/png;base64," + Convert.ToBase64String(s.Person.SignatureImage));
            authourisation += "</td><td>";
            Model.Staff r = db.Staffs.FirstOrDefault(p => p.Id == POentity.ApprovedBy);
            if ((bool)POentity.IsApproved && POentity.ApprovedBy != null && r.Person.SignatureImage != null)
                authourisation += string.Format("<img src=\"{0}\" alt=\"\" style=\"max-width: 100px;\" />", "data:image/png;base64," + Convert.ToBase64String(r.Person.SignatureImage));
            authourisation += "</td><td>";
            Model.Staff auth = db.Staffs.FirstOrDefault(p => p.Id == POentity.ApprovedBy);
            if ((bool)POentity.IsApproved && POentity.ApprovedBy != null && auth.Person.SignatureImage != null)
                authourisation += string.Format("<img src=\"{0}\" alt=\"\" style=\"max-width: 100px;\" />", "data:image/png;base64," + Convert.ToBase64String(s.Person.SignatureImage));
            authourisation += "</td><tr>";
            return authourisation;
        }

        private static String AuthDate(SCMSEntities db, Model.PurchaseOrder POentity, String authourisation)
        {
            authourisation += "<tr><td class=\"gridheader\">Date:</td><td>";
            authourisation += POentity.PreparedOn.Value.ToString("dd/MM/yyyy h:mm tt");
            authourisation += "</td><td>";
            //if ((bool)POentity.IsReviewed && POentity.ReviewedBy != null)
            //{
            //    Model.Staff r = db.Staffs.FirstOrDefault(p => p.Id == POentity.ReviewedBy);
            //    authourisation += POentity.ReviewedOn.Value.ToString("dd/MM/yyyy h:mm tt");
            //}
            authourisation += "</td><td>";
            if ((bool)POentity.IsApproved && POentity.ApprovedBy != null)
            {
                Model.Staff auth = db.Staffs.FirstOrDefault(p => p.Id == POentity.ApprovedBy);
                authourisation += POentity.ApprovedOn.Value.ToString(Constants.DATETIME_FORMAT);
            }
            authourisation += "</td><tr>";
            return authourisation;
        }

        private static String AuthTitle(SCMSEntities db, Model.PurchaseOrder POentity, String authourisation, Model.Staff s)
        {
            authourisation += "<tr><td class=\"gridheader\">Title:</td><td>";
            authourisation += s.Designation.Name;
            authourisation += "</td><td>";
            //if ((bool)POentity.IsReviewed && POentity.ReviewedBy != null)
            //{
            //    Model.Staff r = db.Staffs.FirstOrDefault(p => p.Id == POentity.ReviewedBy);
            //    authourisation += r.Designation.Name;
            //}
            authourisation += "</td><td>";
            if ((bool)POentity.IsApproved && POentity.ApprovedBy != null)
            {
                Model.Staff auth = db.Staffs.FirstOrDefault(p => p.Id == POentity.ApprovedBy);
                authourisation += auth.Designation.Name;
            }
            authourisation += "</td><tr>";
            return authourisation;
        }

        private static String AuthName(SCMSEntities db, Model.PurchaseOrder POentity, String authourisation, Model.Staff s)
        {
            authourisation += "<tr><td class=\"gridheader\">Name:</td><td>";
            authourisation += s.Person.FirstName + " " + s.Person.OtherNames;
            authourisation += "</td><td>";
            //if ((bool)POentity.IsReviewed && POentity.ReviewedBy != null)
            //{
            //    Model.Staff r = db.Staffs.FirstOrDefault(p => p.Id == POentity.ReviewedBy);
            //    authourisation += r.Person.FirstName + " " + r.Person.OtherNames;
            //}
            authourisation += "</td><td>";
            if ((bool)POentity.IsApproved && POentity.ApprovedBy != null)
            {
                Model.Staff auth = db.Staffs.FirstOrDefault(p => p.Id == POentity.ApprovedBy);
                authourisation += auth.Person.FirstName + " " + auth.Person.OtherNames;
            }
            authourisation += "</td><tr>";
            return authourisation;
        }


    }
}
