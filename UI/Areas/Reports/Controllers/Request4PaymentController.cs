using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SCMS.Reports;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.StaffServices;
using System.Drawing;
using System.Drawing.Imaging;
using SCMS.Utils.DTOs;
using SCMS.CoreBusinessLogic.Request4Payment;
using OfficeOpenXml;
using SCMS.Utils.File;
using OfficeOpenXml.Drawing;

namespace SCMS.UI.Areas.Reports.Controllers
{
    public class Request4PaymentController : BaseController
    {
        IStaffService staffService;
        IRequest4PaymentService request4PaymentSvc;

        public Request4PaymentController(IStaffService _staffService, IRequest4PaymentService _R4PSvc)
        {
            this.staffService = _staffService;
            this.request4PaymentSvc = _R4PSvc;
        }

        public ActionResult Pdf(Guid RFPid)
        {
            Dictionary<String, String> data = new Dictionary<string, string>();

            using (var context = new SCMSEntities())
            {
                PaymentRequest PR = context.PaymentRequests.SingleOrDefault(p => p.Id == RFPid);

                List<R4PpaymentDetails> pdlist = new List<R4PpaymentDetails>();
                List<PaymentRequestBudgetLine> ARBlineList = context.PaymentRequestBudgetLines.Where(p => p.PaymentRequestId == PR.Id).ToList();
                foreach (PaymentRequestBudgetLine item in ARBlineList)
                {
                    var pb = context.ProjectBudgets.SingleOrDefault(p => p.Id == item.BudgetLineId);
                    var pdatils = new R4PpaymentDetails()
                    {
                        BudgetLine = pb.LineNumber,
                        BudgetLineDescription = pb.Description,
                        projectNo = context.ProjectDonors.SingleOrDefault(p => p.Id == item.ProjectBudget.BudgetCategory.ProjectDonorId).ProjectNumber,
                        Amount = (decimal)item.Amount
                    };
                    pdlist.Add(pdatils);
                }
                Currency c = PR.Currency;
                PaymentType pt = PR.PaymentType;
                Supplier sup = PR.Supplier;
                Staff st = PR.Staff1;
                Person psn = st.Person;
                Designation d = st.Designation;
                PaymentTerm pterm = PR.PaymentTerm;

                data.Add("{VOUCHER_NO.}", PR.VoucherNumber);
                data.Add("{REQ_NO.}", PR.RefNumber);
                data.Add("{POSPMNO.}", PR.PurchaseOrder.RefNumber);
                data.Add("{DATE}", PR.PreparedOn.ToShortDateString());
                data.Add("{CURRENCY}", PR.Currency.ShortName);
                data.Add("{PAYMENT_TERMS}", PR.PaymentTerm.Code);
                data.Add("{PAYMENT_TYPE}", PR.PaymentType.Description);
                data.Add("{TOTAL_PAYMENT}", PR.TotalAmount.ToString("##,###.00"));

                data.Add("{SUBJECT}", PR.Subject);
                data.Add("{PAYMENT_TO}", PR.Supplier.Name);
                data.Add("{CHOICE_FP}", PR.RequestFor == "FullPayment" ? "\" checked=\"checked\"" : "");
                data.Add("{CHOICE_RI}", PR.RequestFor == "Rate_Instalment" ? "\" checked=\"checked\"" : "");
                data.Add("{CHOICE_AP}", PR.RequestFor == "Adv_Payment_percentage" ? "\" checked=\"checked\"" : "");
                data.Add("{CHOICE_AFP}", PR.RequestFor == "Adv_Final_Payment_percentage" ? "\" checked=\"checked\"" : "");

                String payHtml = "";
                int t = 1;
                foreach (R4PpaymentDetails pd in pdlist)
                {
                    payHtml += "<tr><td align=\"center\">" +
                        t + "</td><td>" +
                        pd.projectNo + "</td><td>" + pd.BudgetLine + " " +
                        pd.BudgetLineDescription + "</td><td align=\"right\">" +
                        pd.Amount.ToString("##,##0.00") + "</td><td align=\"center\">" +
                        PR.Currency.ShortName + "</td></tr>";
                    t++;
                }
                while (t < 11)
                {
                    payHtml += "<tr><td align=\"center\">" + t++ + "</td><td></td><td></td><td></td><td></td></tr>";
                }
                data.Add("{PAYMENT_DETAILS}", payHtml);

                data.Add("{REMARKS}", PR.Remarks);

                string masterBUdgetCurrency = "";

                if (PR.Currency1 != null)
                {
                    masterBUdgetCurrency = PR.Currency1.ShortName;
                }

                data.Add("{MB_CURRENCY}", masterBUdgetCurrency);
                string total = "";

                if (PR.MBValue != null)
                {
                    total = ((decimal)PR.MBValue).ToString("#,##0.00");
                }

                data.Add("{TOTAL}", total);


                data.Add("{PREPAROR_NAME}", PR.Staff1.Person.FirstName + " " + PR.Staff1.Person.OtherNames);
                data.Add("{PREPAROR_TITLE}", PR.Staff1.Designation.Name);
                data.Add("{PREPAROR_DATE}", PR.PreparedOn.ToString("dd/MM/yyyy"));
                data.Add("{PREPAROR_SIG}", this.signatureFilePath(PR.Staff1.Person.SignatureImage));


                String reviewerName = "";
                String reviewerTitle = "";
                String reviewedOn = "";
                String reviewerSignature = "";

                if (PR.ReviewedOn.HasValue)
                {
                    reviewedOn = ((DateTime)PR.ReviewedOn).ToString("dd/MMM/yyyy");
                }

                if (PR.ReviewedBy != null)
                {
                    Staff reviewer = staffService.GetStaffById((Guid)PR.ReviewedBy);
                    reviewerName = reviewer.Person.FirstName + " " + reviewer.Person.OtherNames;
                    reviewerTitle = reviewer.Designation.Name;
                    reviewerSignature = this.signatureFilePath(reviewer.Person.SignatureImage);
                }

                data.Add("{REVIEWER_NAME}", reviewerName);
                data.Add("{REVIEWER_TITLE}", reviewerTitle);
                data.Add("{REVIEWER_DATE}", reviewedOn);
                data.Add("{REVIEWER_SIG}", reviewerSignature);


                String authorizerName = "";
                String authorizerTitle = "";
                String authorizedOn = "";
                String authorizerSignature = "";

                if (PR.AuthorizedOn.HasValue)
                {
                    authorizedOn = ((DateTime)PR.AuthorizedOn).ToString("dd-MMM-yyyy");
                }

                if (PR.AuthorizedBy != null)
                {
                    Staff authorizer = staffService.GetStaffById((Guid)PR.AuthorizedBy);
                    authorizerName = authorizer.Person.FirstName + " " + authorizer.Person.OtherNames;
                    authorizerTitle = authorizer.Designation.Name;
                    authorizerSignature = this.signatureFilePath(authorizer.Person.SignatureImage);
                }

                data.Add("{AUTHORIZER_NAME}", authorizerName);
                data.Add("{AUTHORIZER_TITLE}", authorizerTitle);
                data.Add("{AUTHORIZER_DATE}", reviewedOn);
                data.Add("{AUTHORIZER_SIG}", authorizerSignature);
            }
            List<String> options = new List<string>();
            options.Add(" --copies 2 ");
            Byte[] output = WkHtml2Pdf.CreateReport(data, "request4payment.html", options);

            return File(output, "application/pdf", "RRP_" + DateTime.Now.FormatDDMMMYYYYHHmm());
        }


        public ActionResult SelectedRFPsPdf()
        {
            string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (_idList.Length > 0)
            {
                List<Guid> idList = new List<Guid>();

                foreach (String i in _idList)
                {
                    idList.Add(new Guid(i));
                }
                List<RequestForPaymentSummary> reqs = request4PaymentSvc.Find(idList);

                String html = "<table class='summary default black-borders goods-detail'><thead><tr><th>#</th>";
                html += "<th>RFP No.</th><th>Supplier</th><th>PO No.</th><th>PN</th><th>Currency</th><th>Amount</th>";
                html += "<th>Status</th><th>Status Date</th></tr></thead><tbody>";

                int c = 1;


                foreach (RequestForPaymentSummary r in reqs)
                {
                    html += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td></tr>",
                        c, r.RFPNo, r.Supplier, r.PONo, r.PN, r.Currency, r.Amount.ToString("#,##0.00"), r.Status,
                        r.StatusDate.ToString("dd/MM/yyyy"));
                    c++;
                }

                html += "</tbody></table>";
                Dictionary<String, String> data = new Dictionary<string, string>();
                data.Add("{DETAILS}", html);
                data.Add("{REPORT_DATE}", DateTime.Now.ToString("dd.MM.yyyy"));
                data.Add("{REPORT_TITLE}", "Selected Requests for Payment");


                String fileName = WkHtml2Pdf.CreatePersistedReport(data, "Summary-General-Template.htm");

                return Content("/Content/tmp_reports/" + fileName);
            }

            return Content("#N/A");
        }


        public ActionResult SelectedRFPsExcel()
        {

            string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (_idList.Length > 0)
            {
                List<Guid> idList = new List<Guid>();

                foreach (String i in _idList)
                {
                    idList.Add(new Guid(i));
                }
                List<RequestForPaymentSummary> reqs = request4PaymentSvc.Find(idList);


                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage())
                    {
                        var ws = xlPackage.Workbook.Worksheets.Add("Req 4 Payment");
                        ws.SetValue(2, 1, "Supply Chain Management Report");

                        ws.SetValue(4, 1, "Report:");
                        ws.SetValue(6, 1, "Date:");

                        ws.SetValue(4, 2, "Selected Requests for Payment");
                        ws.SetValue(6, 2, DateTime.Now.ToString("dd.MM.yyyy"));

                        int row = 9;
                        ws.SetValue(row, 1, "RFP. No.");
                        ws.SetValue(row, 2, "Supplier");
                        ws.SetValue(row, 3, "PO No.");
                        ws.SetValue(row, 4, "PN");
                        ws.SetValue(row, 5, "Currency");
                        ws.SetValue(row, 6, "Amount");
                        ws.SetValue(row, 7, "Status");
                        ws.SetValue(row, 8, "Status Date");

                        ws.Cells[2, 1, 2, 8].Merge = true; //Merge columns start and end range
                        ws.Cells[2, 1, 9, 8].Style.Font.Bold = true;
                        ws.Column(6).Style.Numberformat.Format = "#,##0.00";

                        row++;
                        foreach (RequestForPaymentSummary r in reqs)
                        {
                            ws.SetValue(row, 1, r.RFPNo);
                            ws.SetValue(row, 2, r.Supplier);
                            ws.SetValue(row, 3, r.PONo);
                            ws.SetValue(row, 4, r.PN);
                            ws.SetValue(row, 5, r.Currency);
                            ws.SetValue(row, 6, r.Amount);
                            ws.SetValue(row, 7, r.PN);
                            ws.SetValue(row, 8, r.StatusDate.ToString("dd.MM.yyyy"));

                            row++;
                        }

                        Bitmap image = ImageUtility.GetLogosImage();
                        ExcelPicture picture = null;

                        if (image != null)
                        {
                            picture = ws.Drawings.AddPicture("logo-" + (new Guid()).ToString(), image);
                            picture.From.Column = 3;
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


        private String signatureFilePath(byte[] signData)
        {
            String iType = "jpg";

            if (signData != null)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(signData))
                    {
                        Image tmpI = Image.FromStream(ms);

                        if (tmpI != null)
                        {

                            if (ImageFormat.Jpeg.Equals(tmpI.RawFormat))
                            {
                                // JPEG
                                //iType = "jpg";
                            }
                            else if (ImageFormat.Png.Equals(tmpI.RawFormat))
                            {
                                // PNG
                                iType = "png";
                            }
                            else if (ImageFormat.Gif.Equals(tmpI.RawFormat))
                            {
                                // GIF
                                iType = "gif";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                String fName = "<img src=\"data:image/" + iType + ";base64," + Convert.ToBase64String(signData) + "\" alt=\"Signature image\" style=\"max-width: 80px;\" />";

                return fName;
            }
            return "";
        }

    }
}
