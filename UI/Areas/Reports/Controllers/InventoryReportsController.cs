using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.Reports.Utilities;
using SCMS.Utils.DTOs;
using SCMS.CoreBusinessLogic._Inventory;
using SCMS.Reports;
using System.IO;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Drawing;
using SCMS.Utils.File;

namespace SCMS.UI.Areas.Reports.Controllers
{
    [MyException]
    public class InventoryReportsController : PortalBaseController
    {
        private readonly IInventoryService inventorySvc;

        public InventoryReportsController(IPermissionService permissionService, IUserContext userContext, IInventoryService _inventorySvc)
            : base(userContext, permissionService) {

                inventorySvc = _inventorySvc;
        }
        //
        // GET: /Reports/InventoryReports/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InventoryAssets()
        {
            ViewBag.ViewName = "InventoryAssets";
            return View("PartialResponses");
        }

        public ActionResult InventoryItems()
        {
            ViewBag.ViewName = "InventoryItems";
            return View("PartialResponses");
        }

        public ActionResult GeneralInventory()
        {
            ViewBag.ViewName = "GeneralInventory";
            return View("PartialResponses");
        }

        public ActionResult SelectedInventoryPdf()
        {
            string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (_idList.Length > 0)
            {
                List<Guid> idList = new List<Guid>();

                foreach (String i in _idList)
                {
                    idList.Add(new Guid(i));
                }
                List<GeneralInventorySummary> inventoryItems = inventorySvc.Find(idList);

                String html = "<table class='summary default black-borders goods-detail'><thead><tr><th>#</th>";
                html += "<th>Classification</th><th>Item Name</th>";
                html += "<th>Category</th><th>Quantity</th></tr></thead><tbody>";

                int c = 1;

                foreach (GeneralInventorySummary z in inventoryItems)
                {
                    html += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>",
                        c, z.Classification, z.ItemName, z.Category, z.Quantity);
                    c++;
                }

                html += "</tbody></table>";
                Dictionary<String, String> data = new Dictionary<string, string>();
                data.Add("{DETAILS}", html);
                data.Add("{REPORT_DATE}", DateTime.Now.ToString("dd.MM.yyyy"));
                data.Add("{REPORT_TITLE}", "Selected General Inventory Items");

                String fileName = WkHtml2Pdf.CreatePersistedReport(data, "Summary-General-Template.htm");

                return Content("/Content/tmp_reports/" + fileName);
            }

            return Content("#N/A");
        }

        public ActionResult SelectedInventoryExcel()
        {

            string[] _idList = Request.Form["ids"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (_idList.Length > 0)
            {
                List<Guid> idList = new List<Guid>();

                foreach (String i in _idList)
                {
                    idList.Add(new Guid(i));
                }

                List<GeneralInventorySummary> inventoryItems = inventorySvc.Find(idList);

                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage())
                    {
                        var ws = xlPackage.Workbook.Worksheets.Add("Inventory");
                        ws.SetValue(2, 1, "Supply Chain Management Report");

                        ws.SetValue(4, 1, "Report:");
                        ws.SetValue(6, 1, "Date:");

                        ws.SetValue(4, 2, "Selected Inventory Items");
                        ws.SetValue(6, 2, DateTime.Now.ToString("dd.MM.yyyy"));

                        int row = 9;
                        ws.SetValue(row, 1, "Classification");
                        ws.SetValue(row, 2, "Item Name");
                        ws.SetValue(row, 3, "Category");
                        ws.SetValue(row, 4, "Quantity");

                        ws.Cells[2, 1, 2, 4].Merge = true; //Merge columns start and end range
                        ws.Cells[2, 1, 9, 4].Style.Font.Bold = true;

                        row++;
                        foreach (GeneralInventorySummary gi in inventoryItems)
                        {
                            ws.SetValue(row, 1, gi.Classification);
                            ws.SetValue(row, 2, gi.ItemName);
                            ws.SetValue(row, 3, gi.Category);
                            ws.SetValue(row, 4, gi.Quantity);

                            row++;
                        }

                        ws.Column(4).Style.Numberformat.Format = "#,##0";

                        Bitmap image = ImageUtility.GetLogosImage();
                        ExcelPicture picture = null;

                        if (image != null)
                        {
                            picture = ws.Drawings.AddPicture("logo-" + (new Guid()).ToString(), image);
                            picture.From.Column = 3;
                            picture.From.Row = 2;
                            picture.SetSize(91, 90);
                        }

                        for (int c = 1; c < 4; c++)
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
