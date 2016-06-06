using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ProcurementPlan;
using SCMS.Utils.DTOs;
using SCMS.Reports;
using OfficeOpenXml.Drawing;
using System.Drawing;
using SCMS.Utils.File;
using System.IO;
using OfficeOpenXml;

namespace SCMS.UI.Areas.Reports.Controllers
{
    [MyException]
    public class PPController : PortalBaseController
    {
        private readonly IProcurementPlanService procPlanSvc;

        public PPController(IUserContext userContext, IPermissionService permissionService, IProcurementPlanService _procPlanSvc)
            : base(userContext, permissionService)
        {
            procPlanSvc = _procPlanSvc;
        }
        //
        // GET: /Reports/PP/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult SelectedPPPdf()
        {
            string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (_idList.Length > 0)
            {
                List<Guid> idList = new List<Guid>();

                foreach (String i in _idList)
                {
                    idList.Add(new Guid(i));
                }
                List<ProcurementPlanSummary> pplans = procPlanSvc.Find(idList);

                String html = "<table class='summary default black-borders goods-detail'><thead><tr><th>#</th>";
                html += "<th>Ref No.</th><th>Project Title</th><th>Project No.</th><th>Donor</th><th>Prep Office</th><th>Date Prepared</th>";
                html += "</tr></thead><tbody>";

                int c = 1;

                foreach (ProcurementPlanSummary p in pplans)
                {
                    html += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>",
                        c, p.RefNumber, p.ProjectTitle, p.ProjectNumber, p.Donor, p.PrepOffice, p.DatePrepared.ToString("dd/MM/yyyy"));
                    c++;
                }

                html += "</tbody></table>";
                Dictionary<String, String> data = new Dictionary<string, string>();
                data.Add("{DETAILS}", html);
                data.Add("{REPORT_DATE}", DateTime.Now.ToString("dd.MM.yyyy"));
                data.Add("{REPORT_TITLE}", "Selected Pocurement Plans");


                String fileName = WkHtml2Pdf.CreatePersistedReport(data, "Summary-General-Template.htm");

                return Content("/Content/tmp_reports/" + fileName);
            }

            return Content("#N/A");
        }


        public ActionResult SelectedPPsExcel()
        {

            string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (_idList.Length > 0)
            {
                List<Guid> idList = new List<Guid>();

                foreach (String i in _idList)
                {
                    idList.Add(new Guid(i));
                }

                List<ProcurementPlanSummary> pplans = procPlanSvc.Find(idList);

                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage())
                    {
                        var ws = xlPackage.Workbook.Worksheets.Add("Procurement Plans");
                        ws.SetValue(2, 1, "Supply Chain Management Report");

                        ws.SetValue(4, 1, "Report:");
                        ws.SetValue(6, 1, "Date:");

                        ws.SetValue(4, 2, "Selected Procurement Plans");
                        ws.SetValue(6, 2, DateTime.Now.ToString("dd.MM.yyyy"));

                        int row = 9;
                        ws.SetValue(row, 1, "Ref. No.");
                        ws.SetValue(row, 2, "Project Title");
                        ws.SetValue(row, 3, "Project No.");
                        ws.SetValue(row, 4, "Donor");
                        ws.SetValue(row, 5, "Prep Office");
                        ws.SetValue(row, 6, "Date Prep");

                        ws.Cells[2, 1, 2, 6].Merge = true; //Merge columns start and end range
                        ws.Cells[2, 1, 9, 6].Style.Font.Bold = true;

                        row++;
                        foreach (ProcurementPlanSummary p in pplans)
                        {
                            ws.SetValue(row, 1, p.RefNumber);
                            ws.SetValue(row, 2, p.ProjectTitle);
                            ws.SetValue(row, 3, p.ProjectNumber);
                            ws.SetValue(row, 4, p.Donor);
                            ws.SetValue(row, 5, p.PrepOffice);
                            ws.SetValue(row, 6, p.DatePrepared.ToString("dd.MM.yyyy"));

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

                        for (int c = 1; c < 7; c++)
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
