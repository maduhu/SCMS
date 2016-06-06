using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Reports;

namespace SCMS.UI.Areas.Reports.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IExportHelper m_ExportHelper;

        public HomeController(IExportHelper exportHelper)
        {
            m_ExportHelper = exportHelper;
        }

        public ActionResult PurchaseOrder(Guid id)
        {
            var memoryStream = new MemoryStream();
            m_ExportHelper.ExportPurchaseOrderToPDF(id, memoryStream);
            return new FileStreamResult(memoryStream, "application/pdf") { FileDownloadName = "PurchaseOrder-{0}.pdf".F(id) };
        }
    }
}