using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.Reports;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.CompletionCtificate;
using SCMS.UI.Models;
using SCMS.Utils.DTOs;
using System.IO;
using OfficeOpenXml;
using System.Drawing;
using SCMS.Utils.File;
using OfficeOpenXml.Drawing;

namespace SCMS.UI.Areas.Reports.Controllers
{
    [MyException]
    public class CompletionCController : PortalBaseController
    {
        private ICompletionCertificateService ccService;
        public CompletionCController(IPermissionService permissionService, IUserContext userContext, ICompletionCertificateService ccService)
            : base(userContext, permissionService)
        {
            this.ccService = ccService;
        }

        //
        // GET: /Reports/CompletionC/

        public ActionResult Index(Guid CCid)
        {
            Dictionary<String, String> data = new Dictionary<string, string>();
            Models.ViewCC modelCC = CompletionCExtention.PrepareCC(CCid, ccService);
            using (var db = new SCMSEntities())
            {
                data.Add("{DATE}", modelCC.EntityCC.PreparedOn.ToString());
                data.Add("{RegNo}", modelCC.EntityCC.RefNumber);
                data.Add("{OFFICE}", modelCC.EntityCC.CountrySubOffice.Name);
                data.Add("{COUNTRY}", modelCC.EntityCC.CountrySubOffice.CountryProgramme.Country.Name);
                data.Add("{PREPAREDBY}", modelCC.PreparedBy.StaffName);
                data.Add("{TITLE}", modelCC.PreparedBy.Designation);
                if (modelCC.PreparedBy != null && modelCC.PreparedBy.SignatureImage != null)
                    data.Add("{SIGNATURE}", "data:image/png;base64," + Convert.ToBase64String(modelCC.PreparedBy.SignatureImage));
                data.Add("{PURPOSE}", modelCC.EntityCC.Purpose);

                data.Add("{CONTRUCTOR}", modelCC.EntityCC.PurchaseOrder.Supplier.Name);
                data.Add("{PONO}", modelCC.EntityCC.PurchaseOrder.RefNumber);
                data.Add("{PROJECTTITLE}", modelCC.EntityCC.ProjectTitle);
                data.Add("{LOCATION}", modelCC.EntityCC.Location);
                data.Add("{DRCOFFICE}", modelCC.EntityCC.CountrySubOffice1.Name);
                data.Add("{COMFIRMED}", modelCC.ComfirmedBy.StaffName);

                data.Add("{COMPLETE}", modelCC.EntityCC.WorksService == "Completed satisfactory" ? "\" checked=\"checked\"" : "");
                data.Add("{OUTSTANDING}", modelCC.EntityCC.WorksService == "Outstanding issues" ? "\" checked=\"checked\"" : "");
                data.Add("{NOTCOMPLETED}", modelCC.EntityCC.WorksService == "Not Completed" ? "\" checked=\"checked\"" : "");

                data.Add("{REMARKS}", modelCC.EntityCC.Remarks);

               
                data.Add("{APPROVTITLE}", (modelCC.AprovedBy != null)?modelCC.AprovedBy.Designation:"");
                data.Add("{APPROVERNAME}", (modelCC.AprovedBy != null) ? modelCC.AprovedBy.StaffName : "");
                data.Add("{APPROVERSIGNATURE}", (modelCC.AprovedBy != null && modelCC.AprovedBy.SignatureImage != null) ? "data:image/png;base64," + Convert.ToBase64String(modelCC.AprovedBy.SignatureImage) : "");
            }

            List<String> options = new List<string>();
            options.Add(" --copies 2 ");
            Byte[] output = WkHtml2Pdf.CreateReport(data, "CompletionCertificate.htm", options);

            return File(output, "application/pdf", "CC_" + DateTime.Now.FormatDDMMMYYYYHHmm());
        }


        public ActionResult SelectedCompletedCertsPdf()
        {
            string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (_idList.Length > 0)
            {
                List<Guid> idList = new List<Guid>();

                foreach (String i in _idList)
                {
                    idList.Add(new Guid(i));
                }
                List<CompletionCertificateSummary> ccerts = ccService.Find(idList);

                String html = "<table class='summary default black-borders goods-detail'><thead><tr><th>#</th>";
                html += "<th>CC Ref No.</th><th>PO No.</th><th>Office</th><th>Project Title</th><th>Constructor</th><th>Confirmed By</th><th>Status</th>";
                html += "</tr></thead><tbody>";

                int c = 1;

                foreach (CompletionCertificateSummary cf in ccerts)
                {
                    html += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td></tr>",
                        c, cf.RefNumber, cf.PONumber, cf.Office, cf.ProjectTitle, cf.Constructor, cf.ConfirmedBy, cf.Status);
                    c++;
                }

                html += "</tbody></table>";
                Dictionary<String, String> data = new Dictionary<string, string>();
                data.Add("{DETAILS}", html);
                data.Add("{REPORT_DATE}", DateTime.Now.ToString("dd.MM.yyyy"));
                data.Add("{REPORT_TITLE}", "Selected Completion Certificates");


                String fileName = WkHtml2Pdf.CreatePersistedReport(data, "Summary-General-Template.htm");

                return Content("/Content/tmp_reports/" + fileName);
            }

            return Content("#N/A");
        }


        public ActionResult SelectedCompletedCertsExcel()
        {

            string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (_idList.Length > 0)
            {
                List<Guid> idList = new List<Guid>();

                foreach (String i in _idList)
                {
                    idList.Add(new Guid(i));
                }
                List<CompletionCertificateSummary> ccert = ccService.Find(idList);


                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage())
                    {
                        var ws = xlPackage.Workbook.Worksheets.Add("Completion Cert");
                        ws.SetValue(2, 1, "Supply Chain Management Report");

                        ws.SetValue(4, 1, "Report:");
                        ws.SetValue(6, 1, "Date:");

                        ws.SetValue(4, 2, "Selected Completion Certificates");
                        ws.SetValue(6, 2, DateTime.Now.ToString("dd.MM.yyyy"));

                        int row = 9;
                        
                        ws.SetValue(row, 1, "CC Ref No.");
                        ws.SetValue(row, 2, "PO No.");
                        ws.SetValue(row, 3, "Office");
                        ws.SetValue(row, 4, "Project Title");
                        ws.SetValue(row, 5, "Constructor");
                        ws.SetValue(row, 6, "Confirmed By");
                        ws.SetValue(row, 7, "Status");

                        ws.Cells[2, 1, 2, 7].Merge = true; //Merge columns start and end range
                        ws.Cells[2, 1, 9, 7].Style.Font.Bold = true;

                        row++;
                        foreach (CompletionCertificateSummary cc in ccert)
                        {
                            ws.SetValue(row, 1, cc.RefNumber);
                            ws.SetValue(row, 2, cc.PONumber);
                            ws.SetValue(row, 3, cc.Office);
                            ws.SetValue(row, 4, cc.ProjectTitle);
                            ws.SetValue(row, 5, cc.Constructor);
                            ws.SetValue(row, 6, cc.ConfirmedBy);
                            ws.SetValue(row, 7, cc.Status);

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


    }
}