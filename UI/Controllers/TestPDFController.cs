using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.Reports;

namespace SCMS.UI.Controllers
{
    public class TestPDFController : Controller
    {
        //
        // GET: /TestPDF/

        public ActionResult Index()
        {

            //var pdfUrl = PdfGenerator.HtmlToPdf(pdfOutputLocation: "~/Content/tmp_reports/",
            //    outputFilenamePrefix: "GeneratedPDF",
            //    urls: new string[] { "http://localhost:56819/Content/tmp_reports/html/ex.htm" });

            //return Content("Trying WKHTMLtoPDF !");

            return View();
        }

    }
}
