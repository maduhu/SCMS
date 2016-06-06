using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Reports;
using SCMS.Model;

namespace SCMS.UI.Areas.Reports.Controllers
{
    public class WareHouseController : BaseController
    {
        //
        public ActionResult ReleaseForm(Guid WRNid)
        {
            Dictionary<String, String> data = new Dictionary<string, string>();

            using (var db = new SCMSEntities())
            {
                WarehouseRelease wrnenttiy = db.WarehouseReleases.First(p => p.Id == WRNid);

                data.Add("{DATE}", wrnenttiy.RequestDate.ToShortDateString());


                data.Add("{REF_NO}", wrnenttiy.RefNumber);
                data.Add("{OFFICE}", wrnenttiy.Staff1.CountrySubOffice.Name);

                var requestor = db.VStaffDetails.First(p => p.StaffID == wrnenttiy.PreparedBy);
                var recipient = db.VStaffDetails.First(p => p.StaffID == wrnenttiy.ReceivedBy);

                data.Add("{NAME}", requestor.StaffName);
                data.Add("{TITLE}", requestor.Designation);
                if (requestor.SignatureImage.IsNotNull())
                    data.Add("{SIGNATURE}", string.Format("<img src=\"{0}\" alt=\"\" style=\"max-width: 50px;\" />", "data:image/png;base64," + Convert.ToBase64String(requestor.SignatureImage)));

                data.Add("{APPROVER-NAME}", wrnenttiy.IsApproved ? wrnenttiy.Staff.Person.FirstName + " " + wrnenttiy.Staff.Person.OtherNames : "");
                data.Add("{APPROVER-TITLE}", wrnenttiy.IsApproved ? wrnenttiy.Staff.Designation.Name : "");
                data.Add("{APPROVER-OFICE}", wrnenttiy.IsApproved ? wrnenttiy.Staff.CountrySubOffice.Name : "");
                if (wrnenttiy.Staff.Person.SignatureImage.IsNotNull() && wrnenttiy.IsApproved)
                    data.Add("{SIGNATURE2}", string.Format("<img src=\"{0}\" alt=\"\" style=\"max-width: 50px;\" />", "data:image/png;base64," + Convert.ToBase64String(wrnenttiy.Staff.Person.SignatureImage)));

                data.Add("{RECEIVER_NAME}", recipient != null ? recipient.StaffName : "");
                data.Add("{RECEIVER_TITLE}", recipient != null ? recipient.Designation : "");
                data.Add("{RECEIVER_UNIT}", wrnenttiy.Staff2 != null ? wrnenttiy.Staff2.CountrySubOffice.Name : "");
                data.Add("{RECEIVER_DATE}", wrnenttiy.ReceiptDate.ToDDMMyyyHHmm());
                if (wrnenttiy.Staff2.Person.SignatureImage.IsNotNull())
                    data.Add("{RECEIVER_SIGNATURE}", string.Format("<img src=\"{0}\" alt=\"\" style=\"max-width: 100px;\" />", "data:image/png;base64," + Convert.ToBase64String(wrnenttiy.Staff2.Person.SignatureImage)));

                List<Model.WarehouseReleaseItem> writms = wrnenttiy.WarehouseReleaseItems.ToList();
                string tbody = "";
                int r = 1; string itemName, serialNo, AssetNo;
                foreach (WarehouseReleaseItem item in writms)
                {
                    if (item.Inventory.Item.ItemCategory.CategoryCode == "C")
                    {
                        itemName = item.Inventory.Item.Name;
                        serialNo = item.Inventory.Item.SerialNumber;
                        AssetNo = "N/A";
                    }
                    else
                    {
                        itemName = item.Asset.Name;
                        serialNo = item.Asset.SerialNumber;
                        AssetNo = item.Asset.AssetNumber;
                    }
                    tbody += "<tr><td>" + r++ + "</td><td>";
                    tbody += itemName + "</td><td>";
                    tbody += item.Inventory.Item.UnitOfMeasure.Code + "</td><td>";
                    tbody += serialNo + "</td><td>";
                    tbody += AssetNo + "</td><td>";
                    tbody += item.Quantity + "</td><td>";
                    tbody += item.Comments + "</td></tr>";
                }
                while (r < 21) tbody += "<tr><td>" + r++ + "</td><td></td><td></td><td></td><td></td><td></td><td></td></tr>";
                data.Add("{DETAILS}", tbody);
                List<String> options = new List<string>();
                options.Add(" --copies 2 ");
                Byte[] output = WkHtml2Pdf.CreateReport(data, "warehouse-release-form.html", options);

                return File(output, "application/pdf", "WRN_" + DateTime.Now.FormatDDMMMYYYYHHmm());
            }

        }

    }
}