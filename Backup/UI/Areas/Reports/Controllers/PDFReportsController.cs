using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Reports;
using SCMS.Reports.Reports;

namespace SCMS.UI.Areas.Reports.Controllers
{
    public class PDFReportsController : PortalBaseController
    {
        #region Declarations
        private IExportHelper pdfReportService;
        #endregion

        public PDFReportsController(IPermissionService permissionService, IUserContext userContext, IExportHelper pdfService)
            : base(userContext, permissionService)
        {
            this.pdfReportService = pdfService;
        }
        //
        // GET: /Reports/PDFReports/

        public ActionResult Index()
        {
            return View();
        }

        public void mm()
        {
            Report1 ls = new Report1();
            pdfReportService.ExportToPDF(ls);
        }

        public void GoodsReceivedNote(string refNumber)
        {
            GRNPDF GRN = new GRNPDF();
            GRN.ReportParameters["RefNo"].Value = refNumber;
            pdfReportService.ExportToPDF(GRN);
        }

        public void OrderRequest(string refNumber)
        {
            Report2 PP = new Report2();
            PP.ReportParameters["RefNo"].Value = refNumber;
            pdfReportService.ExportToPDF(PP);
        }

        public void ProcurementPlan(string refNumber)
        {
            ProcurementPlanPDF PP = new ProcurementPlanPDF();
            PP.ReportParameters["RefNo"].Value = refNumber;
            pdfReportService.ExportToPDF(PP);
        }

        public void PurchaseOrderInternational(string refNumber)
        {
            POPDFInternational PurchaseOrder = new POPDFInternational();
            PurchaseOrder.ReportParameters["RefNo"].Value = refNumber;
            pdfReportService.ExportToPDF(PurchaseOrder);
        }

        public void PurchaseOrder(string refNumber)
        {
            PurchaseOrderPrintPDF PurchaseOrder = new PurchaseOrderPrintPDF();
            PurchaseOrder.ReportParameters["RefNo"].Value = refNumber;
            pdfReportService.ExportToPDF(PurchaseOrder);
        }

        public void BinCard(string refNumber)
        {
            BinCardPDF binCard = new BinCardPDF();
            binCard.ReportParameters["RefNo"].Value = refNumber;
            pdfReportService.ExportToPDF(binCard);
        }

        public void GIV(string refNumber)
        {
            GIVPDF Giv = new GIVPDF();
            Giv.ReportParameters["RefNo"].Value = refNumber;
            pdfReportService.ExportToPDF(Giv);
        }

        public void WRO(string refNumber)
        {
            WROPDF Wro = new WROPDF();
            Wro.ReportParameters["RefNo"].Value = refNumber;
            pdfReportService.ExportToPDF(Wro);
        }

        public void CC(string refNumber)
        {
            CompletionCertificatePDF cc = new CompletionCertificatePDF();
            cc.ReportParameters["RefNo"].Value = refNumber;
            pdfReportService.ExportToPDF(cc);
        }
    }
}
