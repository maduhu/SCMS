using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using System.Web;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Tables;
using System.Data;
using SCMS.Reports.Utilities;
using SCMS.Model;

namespace SCMS.Reports.Orders
{
    public class OrderRequest
    {
        private Table summaryTable;
        Section section;

        public MemoryStream CreateOrderStream(Guid orderId)
        {

            MigraDoc.DocumentObjectModel.Document document = new MigraDoc.DocumentObjectModel.Document();
            document.Info.Title = "Order Request";
            document.Info.Subject = "COUNTRY OFFICE, UGANDA";
            document.Info.Author = "DRC";

            this.section = document.AddSection();

            Image image = section.Headers.Primary.AddImage(HttpContext.Current.Server.MapPath("/Content/reports/logos-90.png"));
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = "13cm";
            image.WrapFormat.Style = WrapStyle.Through;

            TextFrame heading = this.section.AddTextFrame();

            Paragraph tmpHeadParagraph = heading.AddParagraph("ORDER REQUEST");
            tmpHeadParagraph.Format.Alignment = ParagraphAlignment.Center;

            heading.Top = ShapePosition.Top;
            heading.Width = "12.0cm";
            heading.Height = "0.8cm";

            TextFrame countryOfficeAddressFrame;
            countryOfficeAddressFrame = section.AddTextFrame();
            countryOfficeAddressFrame.Height = "3.0cm";
            countryOfficeAddressFrame.Width = "3.0cm";
            countryOfficeAddressFrame.Left = ShapePosition.Right;
            countryOfficeAddressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            countryOfficeAddressFrame.Top = "3.7cm";
            countryOfficeAddressFrame.RelativeVertical = RelativeVertical.Page;

            Paragraph paragraph;
            paragraph = countryOfficeAddressFrame.AddParagraph("DANISH REFUGEE COUNCIL");
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 7;
            paragraph.Format.SpaceAfter = 3;
            paragraph.AddLineBreak();
            paragraph.AddText("Borgergade 10");
            paragraph.AddLineBreak();
            paragraph.AddText("DK-1300 Copenhagen");
            paragraph.AddLineBreak();
            paragraph.AddText("Denmark");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddFormattedText("COUNTRY OFFICE, UGANDA", TextFormat.Bold);
            paragraph.AddLineBreak();
            paragraph.AddText("4688 Kalungi Road");
            paragraph.AddLineBreak();
            paragraph.AddText("Muyenga, Kampala");

            paragraph = countryOfficeAddressFrame.AddParagraph("www.drc.dk");
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 7;
            paragraph.Format.Font.Color = new Color(58, 162, 213);
            paragraph.Format.Font.Underline = Underline.Single;
            paragraph.AddLineBreak();
            paragraph.AddText("WWW.danishdemininggroup.dk");


            List<double> sizes = new List<double>();
            sizes.AddRange(new double[] { 4.0, 4.0, 4.0 });
            DataTable t = this.GetTable(orderId);
            PrintTable printTable = new PrintTable(t, sizes);

            this.summaryTable = section.AddTable();
            printTable.GenerateTable(this.summaryTable);



            heading = this.section.AddTextFrame();
            heading.Top = "0.9cm";
            heading.Width = Unit.FromCentimeter(14.0);
            tmpHeadParagraph = heading.AddParagraph("Specification/Description, Unit, Quantities, Estimated Prices & PN/BL.");
            tmpHeadParagraph.Format.Alignment = ParagraphAlignment.Center;
            heading.Height = Unit.FromCentimeter(2.0);

            sizes = new List<double>();
            sizes.AddRange(new double[] { 1.0, 3.5, 1.0, 1.0, 2.5, 3.5, 1.0, 2.0, 2.5 });
            t = this.GetDetailsTable(orderId);
            TableOptions options = new TableOptions();
            options.FontSizeCm = 9.0;
            printTable = new PrintTable(t, sizes, options);

            Table dTable = section.AddTable();
            printTable.GenerateTable(dTable);

            section.AddParagraph(Environment.NewLine);

            sizes = new List<double>();
            sizes.AddRange(new double[] { 4.5, 4.5, 4.5, 4.5 });
            t = this.GetTableApproval(orderId);
            options = new TableOptions();
            options.FontSizeCm = 9.0;
            printTable = new PrintTable(t, sizes, options);

            dTable = section.AddTable();
            printTable.GenerateTable(dTable);


            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

            MemoryStream stream = new MemoryStream();

            pdfRenderer.Save(stream, false);

            return stream;
        }


        private DataTable GetTable(Guid orderId)
        {
            var dbContext = new SCMSEntities();
            var orderDetail = dbContext.OrderRequests.First(r => r.Id == orderId);

            Model.OrderRequest myOrder = (Model.OrderRequest)orderDetail;
            Model.Currency currency = dbContext.Currencies.First(c => c.Id == myOrder.CurrencyId);
            Model.CountryProgramme prgm = dbContext.CountryProgrammes.First(p => p.Id == myOrder.CountryProgrammeId);
            Model.Project subP = dbContext.Projects.FirstOrDefault(p => p.Id == myOrder.ProjectId);
            Model.Donor donor = dbContext.ProjectDonors.First(d => d.Id == myOrder.ProjectDonorId).Donor;
            Model.Location reqDes = dbContext.Locations.First(l => l.Id == myOrder.RequestedDestinationId);
            Model.Location finDes = dbContext.Locations.First(l => l.Id == myOrder.FinalDestinationId);

            DataTable table = new DataTable();

            table.Columns.Add("Order Request", typeof(string));
            table.Columns.Add("Date", typeof(string));
            table.Columns.Add("Currency of OR", typeof(string));

            table.Rows.Add(myOrder.RefNumber, myOrder.OrderDate.Value.ToShortDateString(), currency.Name);
            table.Rows.Add("Program:", "Project:", "Donor");
            table.Rows.Add(prgm.ProgrammeName, subP.Name, donor.Name);
            table.Rows.Add("Requested Delivery Date:", "Requested Delivery Destination:", "Final Delivery Destination:");
            table.Rows.Add(myOrder.DeliveryDate.HasValue ? ((DateTime)myOrder.DeliveryDate).ToString("dd.MMM.yyyy") : "-",
                reqDes.Name, finDes.Name);

            return table; // Return reference.
        }


        private DataTable GetDetailsTable(Guid orderId)
        {
            DataTable table = new DataTable();

            var dbContext = new SCMSEntities();

            IEnumerable<Model.OrderRequestItem> items = dbContext.OrderRequestItems.Where(i => i.OrderRequestId == orderId);

            table.Columns.Add("No.", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Unit", typeof(string));
            table.Columns.Add("Qty", typeof(string));
            table.Columns.Add("Estimated Price", typeof(string));
            table.Columns.Add("Estimated Total Price", typeof(string));
            table.Columns.Add("BL", typeof(string));
            table.Columns.Add("PN", typeof(string));
            table.Columns.Add("Remarks", typeof(string));

            decimal total1 = 0;
            decimal total2 = 0;

            if (items != null)
            {
                int count = 1;
                foreach (Model.OrderRequestItem i in items)
                {
                    Model.Item it = dbContext.Items.First(t => t.Id == i.ItemId);
                    Model.UnitOfMeasure um = dbContext.UnitOfMeasures.First(m => m.Id == i.Item.UnitOfMeasure.Id);

                    Model.ProjectBudget line = dbContext.ProjectBudgets.First(m => m.Id == i.BudgetLineId);
                    Model.ProjectDonor pDonor = dbContext.ProjectDonors.First(m => m.Id == i.OrderRequest.ProjectDonorId);

                    table.Rows.Add(count.ToString(), it.Name, um.Code, i.Quantity.ToString(), i.EstimatedUnitPrice.ToString("#,###.##"), i.EstimatedPrice.ToString("#,###.##"), line.LineNumber, pDonor.ProjectNumber, i.Remarks);
                    count++;

                    total1 += i.EstimatedPrice;
                    total2 += i.EstimatedPrice;
                }

                table.Rows.Add("", "", "", "", "Total", total1.ToString("#,###.##"), "OR Value", "", "");//Currency of Request
                table.Rows.Add("", "", "", "", "Total", total2.ToString("#,###.##"), "OR Value", "", "");//Master Budget Currency
            }


            return table; // Return reference.
        }


        private DataTable GetTableApproval(Guid orderId)
        {
            DataTable table = new DataTable();

            table.Columns.Add("Authorization", typeof(string));
            table.Columns.Add("Authorized Requestor or Unit", typeof(string));
            table.Columns.Add("Finance Review/Approval of BL/PN", typeof(string));
            table.Columns.Add("Authorized by Manager/CD", typeof(string));

            table.Rows.Add("Name", "Robert Robert", "", "");
            table.Rows.Add("Title", "Logisitics Manager", "", "");
            table.Rows.Add("Date", "16.MAY.2012", "", "");
            table.Rows.Add("Signature", "", "", "");

            return table; // Return reference.
        }

        #region commentedout
        //private void CreateSummaryTable()
        //{
        //    Color blueRow = new Color(219, 229, 241);

        //    this.summaryTable = section.AddTable();

        //    // Before you can add a row, you must define the columns
        //    Column column = this.summaryTable.AddColumn("4cm");
        //    column.Format.Alignment = ParagraphAlignment.Center;

        //    column = this.summaryTable.AddColumn("4cm");
        //    column.Format.Alignment = ParagraphAlignment.Right;

        //    column = this.summaryTable.AddColumn("4cm");
        //    column.Format.Alignment = ParagraphAlignment.Right;


        //    // Create the header of the table
        //    Row row = summaryTable.AddRow();
        //    row.HeadingFormat = true;
        //    row.Format.Alignment = ParagraphAlignment.Center;
        //    row.Format.Font.Bold = true;
        //    row.Shading.Color = blueRow;

        //    row.Cells[0].AddParagraph("Order Request Number:");
        //    row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
        //    row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
        //    row.Cells[1].AddParagraph("Date:");
        //    row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
        //    row.Cells[1].VerticalAlignment = VerticalAlignment.Center; 
        //    row.Cells[2].AddParagraph("Currency of OR");
        //    row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
        //    row.Cells[2].VerticalAlignment = VerticalAlignment.Center;


        //    Row tmpRow;
        //    //first row
        //    tmpRow = summaryTable.AddRow();
        //    tmpRow.HeadingFormat = true;
        //    tmpRow.Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Format.Font.Color = new Color(255, 0, 0);
        //    tmpRow.Cells[0].AddParagraph("10987");
        //    tmpRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
        //    tmpRow.Cells[1].AddParagraph("M");
        //    tmpRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Cells[1].VerticalAlignment = VerticalAlignment.Center;
        //    tmpRow.Cells[2].AddParagraph("M");
        //    tmpRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Cells[2].VerticalAlignment = VerticalAlignment.Center;


        //    //second title set
        //    row = summaryTable.AddRow();
        //    row.HeadingFormat = true;
        //    row.Format.Alignment = ParagraphAlignment.Center;
        //    row.Format.Font.Bold = true;
        //    row.Shading.Color = blueRow;

        //    row.Cells[0].AddParagraph("Program:");
        //    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
        //    row.Cells[1].AddParagraph("Project:");
        //    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[2].AddParagraph("Donor");
        //    row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[2].VerticalAlignment = VerticalAlignment.Bottom;




        //    tmpRow = summaryTable.AddRow();
        //    tmpRow.Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Format.Font.Color = new Color(255, 0, 0);
        //    tmpRow.Cells[0].AddParagraph("M");
        //    tmpRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
        //    tmpRow.Cells[1].AddParagraph("M");
        //    tmpRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Cells[1].VerticalAlignment = VerticalAlignment.Center;
        //    tmpRow.Cells[2].AddParagraph("M");
        //    tmpRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Cells[2].VerticalAlignment = VerticalAlignment.Center;



        //    //3rd title set
        //    row = summaryTable.AddRow();
        //    row.HeadingFormat = true;
        //    row.Format.Alignment = ParagraphAlignment.Center;
        //    row.Format.Font.Bold = true;
        //    row.Shading.Color = blueRow;

        //    row.Cells[0].AddParagraph("Requested Delivery Date:");
        //    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
        //    row.Cells[1].AddParagraph("Requested Delivery Destination:");
        //    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[2].AddParagraph("Final Delivery Destination:");
        //    row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[2].VerticalAlignment = VerticalAlignment.Bottom;


        //    tmpRow = summaryTable.AddRow();
        //    tmpRow.Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Format.Font.Color = new Color(255, 0, 0);
        //    tmpRow.Cells[0].AddParagraph("M");
        //    tmpRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
        //    tmpRow.Cells[1].AddParagraph("M");
        //    tmpRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Cells[1].VerticalAlignment = VerticalAlignment.Center;
        //    tmpRow.Cells[2].AddParagraph("M");
        //    tmpRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
        //    tmpRow.Cells[2].VerticalAlignment = VerticalAlignment.Center;

        //    //border
        //    for (int r = 0; r < summaryTable.Rows.Count; r++)
        //    {
        //        //this.summaryTable.SetEdge(1, 0, 1, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        //        for (int c = 0; c < summaryTable.Columns.Count; c++)
        //        {
        //            this.summaryTable.SetEdge(c, r, 1, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        //        }
        //    }

        //}
        #endregion
    }
}
