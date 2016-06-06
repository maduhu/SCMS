using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using MigraDoc.DocumentObjectModel;
using SCMS.Model;
//using System.Web.UI.WebControls;
using MigraDoc.DocumentObjectModel.Shapes;
using Telerik.Reporting;
using System.Web;



namespace SCMS.Reports
{
    public partial class ExportHelper : IExportHelper
    {
        #region Implementation of IExportHelper

        public void ExportOrderRequestToPDF(Guid orderRequestId, Stream stream)
        {
            using (var context = SCMSEntities.Define())
            {
                var orderRequest = context.OrderRequests.FirstOrDefault(r => r.Id == orderRequestId);
                var currency = context.Currencies.First(c => c.Id == orderRequest.CurrencyId);
                var mbCurrency = context.Currencies.FirstOrDefault(c => c.Id == orderRequest.MBCurrencyId);
                var countryProgramme = context.CountryProgrammes.First(p => p.Id == orderRequest.CountryProgrammeId);
                var project = context.ProjectDonors.FirstOrDefault(p => p.Id == (Guid)orderRequest.ProjectDonorId);
                var donor = context.ProjectDonors.First(d => d.Id == orderRequest.ProjectDonorId).Donor;
                var reqDesLocation = context.Locations.First(l => l.Id == orderRequest.RequestedDestinationId);
                var finDesLocation = context.Locations.First(l => l.Id == orderRequest.FinalDestinationId);


                var document = PDFHelper.NewDocument();
                document.WithDocumentInfo("Order Request", "COUNTRY OFFICE, UGANDA", "DRC");
                var section = document.AddSection();
                section.AddLogo();
                section.AddHeading("ORDER REQUEST", (heading, paragraph) =>
                                                       {
                                                           heading.Width = "16cm";
                                                           heading.Height = "0.8cm";
                                                           paragraph.Format.Alignment = ParagraphAlignment.Center;
                                                           var font = paragraph.Format.Font.Clone();
                                                           font.Size = 22.PixelsToUnitPoints();
                                                           font.Bold = true;
                                                           paragraph.Format.Font = font;
                                                       });
                section.AddVerticalSpacing("0.5cm");

                var orderRequestTable = section.AddNewTable(new[] { "Order Request Number:", "Date:", "Currency of OR:" },
                    new MigraDoc.DocumentObjectModel.Unit[] { 1.76.Inches(), 1.88.Inches(), 1.69.Inches() },
                    row =>
                    {
                        row.Shading.Color = PDFHelper.SkyCloudyBlueColor;
                        row.Format.Font.Bold = true;
                        row.Format.Alignment = ParagraphAlignment.Left;
                    }, (cell, paragraph, columnIndex) =>
                              {
                                  cell.Format.Alignment = ParagraphAlignment.Left;
                              });
                orderRequestTable.AddNewRow(new[] { orderRequest.RefNumber, orderRequest.OrderDate.Value.ToString("dd/MM/yyyy"), currency.ShortName },
                     row => { row.Height = 0.29.Inches(); row.Format.Font.Bold = true; },
                    (cell, paragraph, columnIndex) =>
                    {
                        if (columnIndex == 0)
                        {
                            var font = cell.Format.Font.Clone();
                            cell.Format.Font = font;
                            font.Color = PDFHelper.RedColor;

                        }
                    });

                orderRequestTable.AddNewRow(new[] { "Program:", "Project Number:", "Donor:" },
                    row =>
                    {
                        row.Shading.Color = PDFHelper.SkyCloudyBlueColor;
                        row.Format.Font.Bold = true;
                        row.Format.Alignment = ParagraphAlignment.Left;
                    }, (cell, paragraph, columIndex) => { cell.Format.Alignment = ParagraphAlignment.Left; });


                orderRequestTable.AddNewRow(new[] { project.Project.Name, project.ProjectNumber, donor.ShortName },
                     row => { row.Height = 0.29.Inches(); row.Format.Font.Bold = true; },
                    (cell, paragraph, columnIndex) =>
                    {
                        if (columnIndex == 1)
                        {
                            var font = cell.Format.Font.Clone();
                            cell.Format.Font = font;
                        }
                    });

                orderRequestTable.AddNewRow(new[] { "Requested Delivery Date:", "Requested Delivery Destination:", "Final Delivery Destination:" },
                    row =>
                    {
                        row.Shading.Color = PDFHelper.SkyCloudyBlueColor;
                        row.Format.Font.Bold = true;
                        row.Format.Alignment = ParagraphAlignment.Left;
                    }, (cell, paragraph, columIndex) => { cell.Format.Alignment = ParagraphAlignment.Left; });

                orderRequestTable.AddNewRow(new[] { orderRequest.DeliveryDate.Value.ToString("dd/MM/yyyy"),
                    reqDesLocation.Name, finDesLocation.Name },
                    row =>
                    {
                        row.Height = 0.29.Inches();
                        row.Format.Font.Bold = true;
                    });

                section.AddVerticalSpacing("0.3cm");
                section.AddHeading("Specification/Description, Unit, Quantities, Estimated Prices & PN/BL.",
                    (textFrame, paragraph) =>
                    {
                        textFrame.Width = MigraDoc.DocumentObjectModel.Unit.FromCentimeter(14.0);
                        textFrame.Height = "0.8cm";
                    });
                var specificationsTable = section.AddNewTable(
                    new[] { "No.", "Description", "Unit", "Qty", "Estimated Price", "Estimated Total Price", "BL", "Remarks", },
                    new MigraDoc.DocumentObjectModel.Unit[] { 0.33.Inches(), 2.25.Inches(), 0.44.Inches(), 0.56.Inches(), 1.06.Inches(), 1.07.Inches(), 0.44.Inches(), 1.13.Inches() },
                    row =>
                    {
                        row.Shading.Color = PDFHelper.SkyCloudyBlueColor;
                        row.Format.Font.Bold = true;
                    });


                var orderRequestItems = (from orderRequestItem in context.OrderRequestItems
                                         join item in context.Items on orderRequestItem.ItemId equals item.Id
                                         join unitOfMeasure in context.UnitOfMeasures on orderRequestItem.Item.UnitOfMessureId equals unitOfMeasure.Id
                                         join projectBudget in context.ProjectBudgets on orderRequestItem.BudgetLineId equals projectBudget.Id
                                         join projectDonor in context.ProjectDonors on orderRequestItem.OrderRequest.ProjectDonorId equals projectDonor.Id
                                         where orderRequestItem.OrderRequestId == orderRequestId
                                         select new
                                                    {
                                                        item.Name,
                                                        unitOfMeasure.Code,
                                                        orderRequestItem.Quantity,
                                                        orderRequestItem.EstimatedUnitPrice,
                                                        orderRequestItem.EstimatedPrice,

                                                        projectBudget.LineNumber,
                                                        projectDonor.ProjectNumber,
                                                        orderRequestItem.Remarks
                                                    }).ToArray();
                //Find number of extraRows to add
                var extraRows = 14 - orderRequestItems.Length;

                specificationsTable.AddNewRows(
                    orderRequestItems.Select((p, i) => new object[]{ i+1,p.Name,p.Code,p.Quantity,p.EstimatedUnitPrice.FormatMoney(),p.EstimatedPrice.FormatMoney(),
                    p.LineNumber,p.Remarks}).ToArray());
                //Add extra blank rows
                for (int i = orderRequestItems.Length + 1; i <= 14; i++)
                {
                    specificationsTable.AddNewRow(new object[] { i, "", "", "", "", "", "", "" });
                }

                var specificationsTableTotalPrice = orderRequestItems.Sum(p => p.EstimatedPrice);
                specificationsTable.AddNewRow(new object[] { "Currency of Order Request", "", "", currency.ShortName, "Total", specificationsTableTotalPrice.FormatMoney(), "OR Value", "" },
                    null,
                    (cell, paragraph, columnIndex) =>
                    {
                        if (columnIndex < 1) { cell.MergeRight = 2; cell.Format.Alignment = ParagraphAlignment.Right; }
                        if (columnIndex == 6) { cell.MergeRight = 1; cell.Format.Alignment = ParagraphAlignment.Left; }

                        if (columnIndex < 3 || columnIndex == 4 || columnIndex > 5)
                        {
                            cell.Shading.Color = PDFHelper.SkyCloudyBlueColor;
                        }
                    });

                specificationsTable.AddNewRow(new object[] { "Master Budget Currency", "", "", mbCurrency.ShortName, "Total", orderRequest.MBValue.FormatMoney(), "OR Value", "" },
                    null,
                    (cell, paragraph, columnIndex) =>
                    {
                        if (columnIndex < 1) { cell.MergeRight = 2; cell.Format.Alignment = ParagraphAlignment.Right; }
                        if (columnIndex == 6) { cell.MergeRight = 1; cell.Format.Alignment = ParagraphAlignment.Left; }

                        if (columnIndex < 3 || columnIndex == 4 || columnIndex > 5)
                        {
                            cell.Shading.Color = PDFHelper.GreenishGrayColor;
                        }
                    });
                section.AddVerticalSpacing("0.3cm");

                var authorizationTable = section.AddNewTable(
                    new[] { "Authorization", "Authorized Requestor or Unit", "Finance Review/Approval of BL/PN", "Authorized by Manager/CD" },
                    new MigraDoc.DocumentObjectModel.Unit[] { 0.95.Inches(), 2.0.Inches(), 2.13.Inches(), 2.19.Inches() }, null,
                    (cell, paragraph, columnIndex) =>
                    {
                        if (columnIndex == 0)
                        {
                            var font = paragraph.Format.Font.Clone();
                            font.Underline = Underline.Single;
                            paragraph.Format.Font = font;
                        }
                    });

                //Get authorization values
                Person requestor = orderRequest.Staff2.Person;
                Person approver = orderRequest.Staff != null ? orderRequest.Staff.Person : null;
                Person reviewer = orderRequest.Staff3 != null ? orderRequest.Staff3.Person : null;
                Person authorizer = orderRequest.Staff1 != null ? orderRequest.Staff1.Person : null;

                string requestorName = requestor.FirstName + " " + requestor.OtherNames;
                string approverName = approver != null ? approver.FirstName + " " + approver.OtherNames : "";
                string reviewerName = reviewer != null ? reviewer.FirstName + " " + reviewer.OtherNames : "";
                string authorizerName = authorizer != null ? authorizer.FirstName + " " + authorizer.OtherNames : "";

                string requestorTitle = orderRequest.Staff2 != null ? orderRequest.Staff2.Designation.Name : "";
                string approverTitle = orderRequest.Staff != null ? orderRequest.Staff.Designation.Name : "";
                string reviewerTitle = orderRequest.Staff3 != null ? orderRequest.Staff3.Designation.Name : "";
                string authorizerTitle = orderRequest.Staff1 != null ? orderRequest.Staff1.Designation.Name : "";

                string requestDate = orderRequest.PreparedOn.Value.ToString("dd/MM/yyyy h:mm tt");
                string approvalDate = orderRequest.IsApproved ? orderRequest.ApprovedOn.Value.ToString("dd/MM/yyyy h:mm tt") : "";
                string reviewDate = (bool)orderRequest.IsReviewed ? orderRequest.ReviewedOn.Value.ToString("dd/MM/yyyy h:mm tt") : "";
                string authDate = (bool)orderRequest.IsAuthorized ? orderRequest.AuthorizedOn.Value.ToString("dd/MM/yyyy h:mm tt") : "";

                authorizationTable.AddNewRow(new object[] { "Name", requestorName, reviewerName, authorizerName });
                authorizationTable.AddNewRow(new object[] { "Title", requestorTitle, reviewerTitle, authorizerTitle });
                authorizationTable.AddNewRow(new object[] { "Date", requestDate, reviewDate, authDate });
                //authorizationTable.AddNewRow(new object[] { "Signature", "", "", "" });


                authorizationTable.AddNewRow(new object[] { "Signature", "", "", "" }, null,
                (cell, paragraph, columnIndex) =>
                {
                    if (columnIndex == 1)
                    {
                        //cell.AddSignature(GetSignature(requestor));
                        cell.Add(GetSignature(requestor));
                        cell.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;


                    }
                    if (columnIndex == 2)
                    {
                        //cell.AddSignature(GetSignature(reviewer));
                        cell.Add(GetSignature(reviewer));
                    }
                    if (columnIndex == 3)
                    {
                        //cell.AddSignature(GetSignature(authorizer));
                        cell.Add(GetSignature(authorizer));
                    }
                });

                document.WriteAsPDFToStream(stream);
            }
        }

        private Image GetSignature(Person person)
        {
            if (person != null && person.SignatureImage != null)
            {
                string signatureFileName = Path.GetTempFileName();
                signatureFileName = signatureFileName.Replace(".tmp", ".png");

                FileStream fs = File.Create(signatureFileName, person.SignatureImage.Count());
                fs.Write(person.SignatureImage, 0, person.SignatureImage.Count());
                fs.Close();



                Image img = new Image();
                //set the url for image
                img.GetFilePath(signatureFileName);
                img.Width = 80; //set the image width
                img.Height = 80; //set the image hight


                return img;
            }
            return null;
        }

        public void ExportToPDF(Report reportToExport)
        {
            var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
            var instanceReportSource = new InstanceReportSource();
            instanceReportSource.ReportDocument = reportToExport;
            var result = reportProcessor.RenderReport("PDF", instanceReportSource, null);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = result.MimeType;
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Private);
            HttpContext.Current.Response.Expires = -1;
            HttpContext.Current.Response.Buffer = true;

            /* Uncomment to handle the file as attachment
             Response.AddHeader("Content-Disposition",
                            string.Format("{0};FileName=\"{1}\"",
                                            "attachment",
                                            fileName));
             */

            HttpContext.Current.Response.BinaryWrite(result.DocumentBytes);
            HttpContext.Current.Response.End();
        }

        #endregion
    }
}
