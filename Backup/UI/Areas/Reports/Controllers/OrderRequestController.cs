using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SCMS.Reports;
using SCMS.Reports.Orders;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.UI.Models;
using SCMS.Model;
using SCMS.Utils.DTOs;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Drawing;
using SCMS.Utils.File;
using SCMS.Resource;
using SCMS.Utils;


namespace SCMS.UI.Areas.Reports.Controllers
{
    public class OrderRequestController : PortalBaseController
    {
        private readonly IExportHelper m_ExportHelper;
        private SCMS.Reports.Orders.OrderRequest orderRequest;
        private IOrderRequest orderRequestService;

        public OrderRequestController(IExportHelper exportHelper, IUserContext userContext, IOrderRequest orderRequestService, IPermissionService permissionService)
            : base(userContext, permissionService)
        {
            m_ExportHelper = exportHelper;
            orderRequest = new SCMS.Reports.Orders.OrderRequest();
            this.orderRequestService = orderRequestService;
        }

        public ActionResult ViewOrder(Guid? orId = null)
        {
            Guid orid = orId.GetValueOrDefault();
            var memoryStream = new MemoryStream();
            m_ExportHelper.ExportOrderRequestToPDF(orid, memoryStream);
            return new FileStreamResult(memoryStream, "application/pdf") { FileDownloadName = "OrderRequest-{0}.pdf".F(orId) };
            
        }

        public ActionResult pdf(Guid orId)
        {
            //string original = "original", copy = "copy";
            //int pagecount = 1;
            return PreparePdf(orId, 1);
        }

        public ActionResult SelectedOrderRequests()
        {
            string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (_idList.Length > 0) {
                List<Guid> idList = new List<Guid>();

                foreach (String i in _idList)
                {
                    idList.Add(new Guid(i));
                }
                    List<OrderRequestSummary> orders = orderRequestService.Find(idList);

                    String html = "<table class='summary default black-borders goods-detail'><thead><tr><th>#</th>";
                    html += "<th>OR No.</th><th>Project No.</th><th>First Item</th><th>OR Value</th>";
                    html += "<th>Requestor</th><th>Status</th><th>Status Date</th></tr></thead><tbody>";

                    int c = 1;


                        foreach (OrderRequestSummary s in orders)
                        {

                            html += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td  style='text-align:right;'>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td></tr>", 
                                c, s.RefNumber, s.ProjectNum, s.FirstItem, s.TotalValue, s.PrepStaffNames,
                                s.Status, s.StatusDate.ToString("dd/MM/yyyy"));
                            c++;
                        }

                        html += "</tbody></table>";
                Dictionary<String, String> data = new Dictionary<string, string>();
                data.Add("{DETAILS}", html);
                data.Add("{REPORT_DATE}", DateTime.Now.ToString("dd.MM.yyyy"));
                data.Add("{REPORT_TITLE}", "Selected Order Requests");

                if (orders.Count > 0)
                {
                    data.Add("{PROJECT_NUMBER}", orders[0].ProjectNum);
                }
                else {
                    data.Add("{PROJECT_NUMBER}", "");
                }

                String fileName = WkHtml2Pdf.CreatePersistedReport(data, "Summary-Order-Requests.htm");

                return Content("/Content/tmp_reports/" + fileName);
            }
            
            return Content("#N/A");
        }


        public ActionResult SelectedOrderRequestsExcel()
        {

               string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

               if (_idList.Length > 0)
               {
                   List<Guid> idList = new List<Guid>();

                   foreach (String i in _idList)
                   {
                       idList.Add(new Guid(i));
                   }
                   List<OrderRequestSummary> orders = orderRequestService.Find(idList);


                   using (var stream = new MemoryStream())
                   {
                       using (var xlPackage = new ExcelPackage())
                       {
                           var ws = xlPackage.Workbook.Worksheets.Add("Order Reqs");
                           ws.SetValue(2, 1, "Supply Chain Management Report");

                           ws.SetValue(4, 1, "Report:");
                           ws.SetValue(6, 1, "Date:");
                           ws.SetValue(6, 1, "Project No.:");

                           ws.SetValue(4, 2, "Selected Order Requests");
                           ws.SetValue(6, 2, DateTime.Now.ToString("dd.MM.yyyy"));

                           if (orders.Count > 0)
                           {
                               ws.SetValue(8, 2, orders[0].ProjectNum);
                           }
                           else
                           {
                               ws.SetValue(8, 2, "");
                           }

                           ws.SetValue(10, 1, "OR No.");
                           ws.SetValue(10, 2, "Project No.");
                           ws.SetValue(10, 3, "First Item");
                           ws.SetValue(10, 4, "OR Value");
                           ws.SetValue(10, 5, "Requestor");
                           ws.SetValue(10, 6, "Status");
                           ws.SetValue(10, 7, "Status Date");

                           ws.Cells[2, 1, 2, 7].Merge = true; //Merge columns start and end range
                           ws.Cells[2, 1, 10, 7].Style.Font.Bold = true; 
                           ws.Column(4).Style.Numberformat.Format = "#,##0.00";

                           int row = 11;
                           foreach (OrderRequestSummary s in orders)
                           {
                               ws.SetValue(row, 1, s.RefNumber);
                               ws.SetValue(row, 2, s.ProjectNum);
                               ws.SetValue(row, 3, s.FirstItem);
                               ws.SetValue(row, 4, s.TotalValue);
                               ws.SetValue(row, 5, s.PrepStaffNames);
                               ws.SetValue(row, 6, s.Status);
                               ws.SetValue(row, 7, s.StatusDate.ToString("dd.MM.yyyy"));

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

                           for (int c = 1; c < 8; c++)
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

        private ActionResult PreparePdf(Guid orId, int pagecount)
        {
            string original = "original", copy = "copy";
            Dictionary<String, String> data = new Dictionary<string, string>();
            Model.OrderRequest model = orderRequestService.GetOrderRequestById(orId);
            //prepare labels
            PrepareLabels(data);
            data.Add("{REQ_NO.}", model.RefNumber);
            data.Add("{ISSUE-DATE}", model.OrderDate.HasValue ? model.OrderDate.Value.ToString(Constants.DATE_FORMAT) : string.Empty);
            data.Add("{CURRENCY}", model.Currency != null ? model.Currency.ShortName : string.Empty);

            data.Add("{PROJECT-NAME}", model.ProjectDonor.Project.Name);
            data.Add("{PROJECT-NO}", model.ProjectDonor.ProjectNumber);
            data.Add("{DONOR}", model.ProjectDonor.Donor.ShortName);

            data.Add("{DELIVERY-DATE}", model.DeliveryDate.HasValue ? model.DeliveryDate.Value.ToString(Constants.DATE_FORMAT) : string.Empty);
            data.Add("{DELIVERY-DESTINATION}", model.Location1 != null ? model.Location1.Name : string.Empty);
            data.Add("{FINAL-DESTINATION}", model.Location != null ? model.Location.Name : string.Empty);

            int count = 1;
            String detailHtml = "";
            foreach (var item in model.OrderRequestItems.ToList())
            {
                detailHtml += "<tr ><td align=\"center\">" + count++ + "</td><td>";
                detailHtml += item.Item.Name;
                detailHtml += "</td><td>";
                detailHtml += item.ItemDescription;
                detailHtml += "</td><td>";
                detailHtml += item.Item.UnitOfMeasure.Code;
                detailHtml += "</td><td align=\"center\">";
                detailHtml += item.Quantity.ToString(Constants.NUMBER_FORMAT_NO_DECIMAL);
                detailHtml += "</td><td align=\"right\">";
                detailHtml += item.EstimatedUnitPrice.ToString(Constants.NUMBER_FORMAT_TWO_DECIMAL);
                detailHtml += "</td><td align=\"right\">";
                detailHtml += item.EstimatedPrice.ToString(Constants.NUMBER_FORMAT_TWO_DECIMAL);
                detailHtml += "</td><td align=\"center\">";
                detailHtml += item.ProjectBudget.BudgetCategory.ProjectDonor.ProjectNumber;
                detailHtml += "</td><td align=\"center\">";
                detailHtml += item.ProjectBudget.LineNumber;
                detailHtml += "</td><td>";
                detailHtml += item.Remarks;
                detailHtml += "</td></tr>";

            }
            while (count < 18)
            {
                detailHtml += "<tr ><td align=\"center\">" + count++ + "</td><td>";
                detailHtml += "</td><td>";
                detailHtml += "</td><td>";
                detailHtml += "</td><td align=\"center\">";
                detailHtml += "</td><td align=\"right\">";
                detailHtml += "</td><td align=\"right\">";
                detailHtml += "</td><td align=\"center\">";
                detailHtml += "</td><td align=\"center\">";
                detailHtml += "</td><td>";
                detailHtml += "</td></tr>";
            }
            data.Add("{DETAILS}", detailHtml);

            data.Add("{OR-CURRENCY}", model.Currency != null ? model.Currency.ShortName : string.Empty);
            data.Add("{OR-VALUE}", model.TotalAmount.HasValue ? model.TotalAmount.Value.ToString(Constants.NUMBER_FORMAT_TWO_DECIMAL) : string.Empty);

            data.Add("{MB-CURRENCY}", model.Currency1 != null ? model.Currency1.ShortName : string.Empty);
            data.Add("{MB-VALUE}", model.MBValue.HasValue ? model.MBValue.Value.ToString(Constants.NUMBER_FORMAT_TWO_DECIMAL) : string.Empty);

            data.Add("{REMARKS}", model.Remarks);

            String authourisation = "";
            authourisation = AuthName(model, authourisation);
            authourisation = AuthTitle(model, authourisation);
            authourisation = AuthDate(model, authourisation);
            authourisation = AuthSignature(model, authourisation);
            data.Add("{AUTHORISATION}", authourisation);

            //data.Add("{PAGETYPE}", pagecount == 1 ? original : copy);
            List<String> options = new List<string>();
            options.Add(" --copies 2 ");
            //options.Add(" --footer-center page 1 of 1 – original ");

            Byte[] output = WkHtml2Pdf.CreateReport(data, "Order-Request.htm", options);
            //if (pagecount == 1)
            //    return PreparePdf(orId, ++pagecount);
            //else 
            return File(output, "application/pdf", "OR_" + DateTime.Now.FormatDDMMMYYYYHHmm());
        }

        private void PrepareLabels(Dictionary<String, String> data)
        {
            data.Add("{LABEL_ORDER_REQUEST}", Resources.Global_String_OrderRequest);
            data.Add("{LABEL_ORDER_REQUEST_NO}", Resources.Global_String_OrderRequestNo);
            data.Add("{LABEL_DATE}", Resources.Global_String_Date);
            data.Add("{LABEL_CURRENCY_OF_OR}", Resources.Global_String_Currency);

            data.Add("{LABEL_PROJECT}", Resources.Global_String_Project);
            data.Add("{LABEL_PROJECT_NO}", Resources.Budget_CategoryList_ProjectNo);
            data.Add("{LABEL_DONOR}", Resources.Budget_CategoryList_Donor);

            data.Add("{LABEL_REQUESTED_DELIVERY_DATE}", Resources.OrderRequest_EditOrderRequest_RequestedDeliveryDate);
            data.Add("{LABEL_REQUESTED_DELIVERY_DESTINATION}", Resources.OrderRequest_EditOrderRequest_RequestedDeliveryDestination);
            data.Add("{LABEL_FINAL_DELIVERY_DESITNATION}", Resources.OrderRequest_EditOrderRequest_FinalDeliveryDestination);

            data.Add("{LABEL_DANISH_REFUGEE_COUNCIL}", Resources.Global_String_DanishRefugeeCouncil.ToUpper());
            
        }

        private static String AuthName(Model.OrderRequest or, String authourisation)
        {
            authourisation += "<tr><th>Name:</th><td>";
            authourisation += or.Staff2 != null ? or.Staff2.Person.FirstName + " " + or.Staff2.Person.OtherNames : string.Empty;
            authourisation += "</td><td>";
            if (or.Staff != null)
                authourisation += or.Staff != null ? or.Staff.Person.FirstName + " " + or.Staff.Person.OtherNames : string.Empty;
            authourisation += "</td><td>";
            if (or.Staff3 != null)
                authourisation += or.Staff3.Person.FirstName + " " + or.Staff3.Person.OtherNames;
            authourisation += "</td><td>";
            if (or.Staff1 != null)
                authourisation += or.Staff1.Person.FirstName + " " + or.Staff1.Person.OtherNames;
            authourisation += "</td><tr>";
            return authourisation;
        }

        private static String AuthTitle(Model.OrderRequest or, String authourisation)
        {
            authourisation += "<tr><th>Title:</th><td>";
            authourisation += or.Staff2 != null ? or.Staff2.Designation.Name : string.Empty;
            authourisation += "</td><td>";
            if (or.Staff != null)
                authourisation += or.Staff != null ? or.Staff.Designation.Name : string.Empty;
            authourisation += "</td><td>";
            if (or.Staff3 != null)
                authourisation += or.Staff3.Designation.Name;
            authourisation += "</td><td>";
            if (or.Staff1 != null)
                authourisation += or.Staff1.Designation.Name;
            authourisation += "</td><tr>";
            return authourisation;
        }

        private static String AuthDate(Model.OrderRequest or, String authourisation)
        {
            authourisation += "<tr><th>Date:</th><td>";
            authourisation += or.PreparedOn.HasValue ? or.PreparedOn.Value.ToString(Constants.DATETIME_FORMAT) : string.Empty;
            authourisation += "</td><td>";
            if (or.IsApproved)
                authourisation += or.ApprovedOn.Value.ToString(Constants.DATETIME_FORMAT);
            authourisation += "</td><td>";
            if (or.IsReviewed.HasValue && or.IsReviewed.Value)
                authourisation += or.ReviewedOn.Value.ToString(Constants.DATETIME_FORMAT);
            authourisation += "</td><td>";
            if (or.IsAuthorized.HasValue && or.IsAuthorized.Value)
                authourisation += or.AuthorizedOn.Value.ToString(Constants.DATETIME_FORMAT);
            authourisation += "</td><tr>";
            return authourisation;
        }

        private static String AuthSignature(Model.OrderRequest or, String authourisation)
        {
            authourisation += "<tr><th>Signature:</th><td>";
            if (or.Staff2 != null && or.Staff2.Person.SignatureImage != null)
                authourisation += string.Format("<img src=\"{0}\" alt=\"\" style=\"max-width: 100px;\" />", "data:image/png;base64," + Convert.ToBase64String(or.Staff2.Person.SignatureImage));
            authourisation += "</td><td>";
            if (or.Staff != null && or.Staff.Person.SignatureImage != null)
                    authourisation += string.Format("<img src=\"{0}\" alt=\"\" style=\"max-width: 100px;\" />", "data:image/png;base64," + Convert.ToBase64String(or.Staff.Person.SignatureImage));
            authourisation += "</td><td>";
            if (or.IsReviewed.HasValue && or.IsReviewed.Value && or.Staff3.Person.SignatureImage != null)
                authourisation += string.Format("<img src=\"{0}\" alt=\"\" style=\"max-width: 100px;\" />", "data:image/png;base64," + Convert.ToBase64String(or.Staff3.Person.SignatureImage));
            authourisation += "</td><td>";
            if (or.IsAuthorized.HasValue && or.IsAuthorized.Value && or.Staff1.Person.SignatureImage != null)
                authourisation += string.Format("<img src=\"{0}\" alt=\"\" style=\"max-width: 100px;\" />", "data:image/png;base64," + Convert.ToBase64String(or.Staff1.Person.SignatureImage));
            authourisation += "</td><tr>";
            return authourisation;
        }
    }
}