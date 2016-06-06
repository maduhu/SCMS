using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telerik.Reporting;

namespace SCMS.Reports
{
    public interface IExportHelper
    {
        void ExportOrderRequestToPDF(Guid orderRequestId, Stream stream);
        void ExportPurchaseOrderToPDF(Guid orderRequestId, Stream stream);
        void ExportToPDF(Report reportToExport);
    }
}
