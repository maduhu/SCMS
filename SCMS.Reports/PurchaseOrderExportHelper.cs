using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Text;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using SCMS.Model;
using System.Web;
using SCMS.Utils;

namespace SCMS.Reports
{
    public partial class ExportHelper 
    {
        #region Info to generate purchase order

        private class ViewPOrderItems
        {
            public Model.PurchaseOrderItem EntityPOItem { get; set; }

            public string ItemName { get { return EntityPOItem.OrderRequestItem.Item.Name; } set { EntityPOItem.OrderRequestItem.Item.Name = value; } }

            public Guid POItemId { get { return EntityPOItem.Id; } set { EntityPOItem.Id = value; } }

            public double UnitCost { get { return EntityPOItem.UnitPrice; } set { EntityPOItem.UnitPrice = value; } }

            public int Qunatity { get { return EntityPOItem.Quantity; } set { EntityPOItem.Quantity = value; } }

            public string Unitmessue { get; set; }

            public decimal TotalCost { get { return EntityPOItem.TotalPrice; } set { EntityPOItem.TotalPrice = value; } }

            public string ProjectNo { get; set; }

            public string BudgetLine { get; set; }

            public string Remarks { get { return EntityPOItem.Remarks; } set { EntityPOItem.Remarks = value; } }
        }

        private class ViewPOs
        {
            public Model.PurchaseOrder POEntiy { get; set; }

            public string ORNumber { get { return POEntiy.OrderRequest.RefNumber; } set { POEntiy.OrderRequest.RefNumber = value; } }

            public string DRCRemarks { get { return POEntiy.Remarks; } set { POEntiy.Remarks = value; } }

            public string paymentTerm { get; set; }

            public string shippingTerm { get; set; }

            public string supplier { get; set; }

            public DateTime deleliveryDate { get; set; }

            public string deliveryAddress { get { return POEntiy.Location.Name; } set { POEntiy.Location.Name = value; } }

            public string PONo { get { return POEntiy.RefNumber; } set { POEntiy.RefNumber = value; } }

            public DateTime PODate { get; set; }

            public Guid POId { get { return POEntiy.Id; } set { POEntiy.Id = value; } }

            public Model.Staff StaffEntity { get; set; }

            public List<ViewPOrderItems> POItems { get; set; }

            public string ReviewdBy { get; set; }

            public string ReviewTitle { get; set; }

            public string ReveiwDate { get; set; }

            public string AuthorizdBy { get; set; }

            public string AuthorizTitle { get; set; }

            public string AuthorizDate { get; set; }

            public decimal? TotalAmount { get; set; }

            public decimal? MBValue { get; set; }

            public string Currency { get; set; }

            public string MBCurrency { get; set; }
        }

        private ViewPOs ViewPurchaseOrdersDetails(Guid id)
        {
            ViewPOrderItems POItmz;
            List<ViewPOrderItems> POItemList = new List<ViewPOrderItems>();
            using (var context = new SCMSEntities())
            {
                Model.PurchaseOrder Po = context.PurchaseOrders.SingleOrDefault(p => p.Id == id);
                ICollection<PurchaseOrderItem> POItemz = Po.PurchaseOrderItems;
                foreach (PurchaseOrderItem item in POItemz)
                {
                    POItmz = new ViewPOrderItems();
                    POItmz.EntityPOItem = item;
                    POItmz.BudgetLine =
                        context.ProjectBudgets.SingleOrDefault(p => p.Id == item.BudgetLineId).LineNumber;
                    POItmz.ProjectNo = item.PurchaseOrder.OrderRequest.ProjectDonor.ProjectNumber;
                    POItmz.Unitmessue = context.UnitOfMeasures.SingleOrDefault(p => p.Id == item.OrderRequestItem.Item.UnitOfMessureId).Code;
                    OrderRequestItem orit = item.OrderRequestItem;
                    Item itm = orit.Item;
                    POItemList.Add(POItmz);
                }

                ViewPOs model = new ViewPOs();

                model.POEntiy = Po;
                model.supplier = context.Suppliers.SingleOrDefault(p => p.Id == Po.SupplierId).Name;
                model.paymentTerm = context.PaymentTerms.SingleOrDefault(p => p.Id == Po.PaymentTermId).Code;
                model.shippingTerm = context.ShippingTerms.SingleOrDefault(p => p.Id == Po.ShippingTermId).Name;
                model.POItems = POItemList;
                model.deleliveryDate = (DateTime) Po.LatestDeliveryDate;
                model.PODate = (DateTime) Po.PreparedOn;
                model.StaffEntity = context.Staffs.SingleOrDefault(p => p.Id == Po.PreparedBy);
                model.TotalAmount = Po.TotalAmount;
                model.MBValue = Po.MBValue;
                Person ps = model.StaffEntity.Person;
                Designation ds = model.StaffEntity.Designation;
                if ((bool) Po.IsApproved)
                {
                    Staff authoriz = context.Staffs.SingleOrDefault(p => p.Id == Po.ApprovedBy );
                    model.AuthorizTitle = authoriz.Designation.Name;
                    model.AuthorizdBy = authoriz.Person.FirstName + " " + authoriz.Person.OtherNames;
                    model.AuthorizDate = Po.ApprovedOn.Value.ToString(Constants.DATETIME_FORMAT);
                }

                Model.OrderRequest or = Po.OrderRequest;
                Location loc = Po.Location;
                model.Currency = or.Currency.ShortName;
                model.MBCurrency = or.CountryProgramme.Currency.ShortName;
                return model;
            }
        }

        #endregion
        #region Implementation of IExportHelper

        public void ExportPurchaseOrderToPDF(Guid purchaseOrderId, Stream stream)
        {
            var model = ViewPurchaseOrdersDetails(purchaseOrderId);

            var document = PDFHelper.NewDocument();
            document.WithDocumentInfo("PURCHASE ORDER", "COUNTRY OFFICE, UGANDA", "DRC");
            var section = document.AddSection();
            section.AddLogo();
            section.AddVerticalSpacing("0.3cm");
            section.AddHeading("PURCHASE ORDER", (heading, paragraph) =>
            {
                heading.Width = "16cm";
                heading.Height = "0.8cm";
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                var font = paragraph.Format.Font.Clone();
                font.Size = 22.PixelsToUnitPoints();
                font.Bold = true;
                paragraph.Format.Font = font;
            });
            section.AddHeading("Date: {0}".F(model.POEntiy.PreparedOn.Value.ToShortDateString()), (textFrame, paragraph) =>
            {
                textFrame.Width = Unit.FromCentimeter(14.0);
                textFrame.Height = "0.8cm";
            });                
            var orderRequestTable = section.AddNewTable(new[] { "Purchase Order Number:", "Order Request Ref No.:", "Payment Terms:" },
                new Unit[] { 1.76.Inches(), 1.88.Inches(), 1.69.Inches() },
                row =>
                {
                    row.Shading.Color = PDFHelper.SkyCloudyBlueColor;
                    row.Format.Font.Bold = true;
                    row.Format.Alignment = ParagraphAlignment.Left;
                }, (cell, paragraph, columnIndex) =>
                {
                    cell.Format.Alignment = ParagraphAlignment.Left;
                });
            orderRequestTable.AddNewRow(new[] { model.POEntiy.RefNumber, model.POEntiy.OrderRequest.RefNumber, model.paymentTerm },
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

            orderRequestTable.AddNewRow(new[] { "Quotation Ref.:", "Shipping/Delivery Terms:", "Latest Delivery Date:" },
                row =>
                {
                    row.Shading.Color = PDFHelper.SkyCloudyBlueColor;
                    row.Format.Font.Bold = true;
                    row.Format.Alignment = ParagraphAlignment.Left;
                }, (cell, paragraph, columIndex) => { cell.Format.Alignment = ParagraphAlignment.Left; });


            orderRequestTable.AddNewRow(new[] { model.POEntiy.QuotationRef, model.shippingTerm, model.POEntiy.LatestDeliveryDate.ToString("dd/MM/yyyy") },
                    row => { row.Height = 0.29.Inches(); row.Format.Font.Bold = true; },
                (cell, paragraph, columnIndex) =>
                {
                    if (columnIndex == 1)
                    {
                        var font = cell.Format.Font.Clone();
                        cell.Format.Font = font;
                        font.Color = PDFHelper.RedColor;
                    }
                });

            orderRequestTable.AddNewRow(new[] { "To Supplier:", "Delivery Address:", "DRC Internal use only:" },
                row =>
                {
                    row.Shading.Color = PDFHelper.SkyCloudyBlueColor;
                    row.Format.Font.Bold = true;
                    row.Format.Alignment = ParagraphAlignment.Left;
                }, (cell, paragraph, columIndex) => { cell.Format.Alignment = ParagraphAlignment.Left; });

            orderRequestTable.AddNewRow(new[] { model.supplier,
                model.POEntiy.Location.Name, "" },
                row =>
                {
                    row.Height = 0.29.Inches();
                    row.Format.Font.Bold = true;
                });

            section.AddVerticalSpacing("0.3cm");

            var poItemsTable = section.AddNewTable(
                    new[] { "No.", "Description", "Unit", "Qty", "Unit Price", "Total Price", "BL", "PN", "Remarks", },
                    new Unit[] { 0.33.Inches(), 2.25.Inches(), 0.44.Inches(), 0.56.Inches(), 0.75.Inches(), 0.75.Inches(), 0.44.Inches(), 0.63.Inches(), 1.13.Inches() },
                    row =>
                    {
                        row.Shading.Color = PDFHelper.SkyCloudyBlueColor;
                        row.Format.Font.Bold = true;
                    });

            poItemsTable.AddNewRows(
            model.POItems.Select((p, i) =>
                new object[] {i+1,p.EntityPOItem.OrderRequestItem.Item.Name,
                p.Unitmessue,p.Qunatity,p.EntityPOItem.UnitPrice.ToString("##,##0.00"),
                p.EntityPOItem.TotalPrice.ToString("##,##0.00"),p.BudgetLine, p.ProjectNo,p.EntityPOItem.Remarks}).ToArray());

            poItemsTable.AddNewRow(new object[]{ "Currency of Purchase Order", "", "", model.Currency, "Total", model.TotalAmount.FormatMoney(), "PO Value", "", "" }, null,
                (cell, paragraph, columnIndex) =>
                {
                    if (columnIndex < 1) { cell.MergeRight = 2; cell.Format.Alignment = ParagraphAlignment.Right; }
                    if (columnIndex == 6) { cell.MergeRight = 2; cell.Format.Alignment = ParagraphAlignment.Left; }

                    if (columnIndex < 3 || columnIndex == 4 || columnIndex > 5)
                    {
                        cell.Shading.Color = PDFHelper.SkyCloudyBlueColor;
                    }
                });
            //poItemsTable.AddNewRows(new object[][]{ new object[]{ "Currency of Purchase Order", "", "", model.Currency, "Total", model.TotalAmount.FormatMoney(), "PO Value", "", "" },
            //new object[]{"Master Budget Currency","","",model.MBCurrency,"Total",model.MBValue.FormatMoney(), "PO Value", "", "" }},
            //        null,
            //        (cell, paragraph, rowIndex, columnIndex) =>
            //        {
            //            if (columnIndex < 1) { cell.MergeRight = 2; cell.Format.Alignment = ParagraphAlignment.Right; }
            //            if (columnIndex == 6) { cell.MergeRight = 2; cell.Format.Alignment = ParagraphAlignment.Left; }

            //            if (columnIndex < 3 || columnIndex == 4 || columnIndex > 5)
            //            {
            //                cell.Shading.Color = (rowIndex == 0) ? PDFHelper.SkyCloudyBlueColor : PDFHelper.GreenishGrayColor;
            //            }
            //        });

            section.AddVerticalSpacing("0.3cm");

            var authorizationTable = section.AddNewTable(
                    new[] { "Authorization", "Authorized Requestor or Unit", "Finance Review/Approval of BL/PN", "Authorized by Manager/CD" },
                    new Unit[] { 0.95.Inches(), 2.0.Inches(), 2.13.Inches(), 2.19.Inches() }, null,
                    (cell, paragraph, columnIndex) =>
                    {
                        if (columnIndex == 0)
                        {
                            var font = paragraph.Format.Font.Clone();
                            font.Underline = Underline.Single;
                            paragraph.Format.Font = font;
                        }
                    });
            authorizationTable.AddNewRows(
                new[] {
                    new object[]{"Name", "{0} {1}".F(model.StaffEntity.Person.FirstName ,model.StaffEntity.Person.OtherNames), model.ReviewdBy, model.AuthorizdBy},
                    new object[]{"Title", model.StaffEntity.Designation.Name, model.ReviewTitle, model.AuthorizTitle},
                    new object[]{"Date", model.POEntiy.PreparedOn.Value.ToString("dd/MM/yyyy h:mm tt"), model.ReveiwDate, model.AuthorizDate},
                    new object[]{"Signature", "", "", ""}
                }, null, (cell,paragraph,rowIndex,columnIndex)=>
                             {
                                 if (rowIndex == 3 && columnIndex == 1)
                                     cell.MergeRight = 2;
                             });

            section.AddVerticalSpacing("0.3cm");

            var instructionsTbale = section.AddNewTable(new[] {"General Instructions:", "Additional Instructions:"},
                                                        new Unit[] {3.635.Inches(), 3.635.Inches()},
                                                        row =>
                                                            {
                                                                row.Table.Borders.Visible = false;
                                                            },(cell,paragraph,columnIndex)=>
                                                                  { 
                                                                      var font = paragraph.Format.Font.Clone();
                                                                      paragraph.Format.Font = font;
                                                                      font.Bold = true;
                                                                      font.Underline = Underline.Single;
                                                                  });
            instructionsTbale.AddNewRow(new[]{"Please retain this original and return the copy duly signed and dated as evidence of your acceptance of this order.",
            "Please refer to above mentioned Purchase Order number in all correspondence concerning the order."},
            row => row.Table.SetEdge(0, 0, 2, 2, Edge.Box, BorderStyle.Single, new Unit(0.5, UnitType.Point), Color.Empty));

            section.AddVerticalSpacing("0.3cm");
            section.AddHeading("Documentation Required from Supplier", (textFrame, paragraph) =>
                                                                           {
                                                                               var font = paragraph.Format.Font.Clone();
                                                                               paragraph.Format.Font = font;
                                                                               font.Color = PDFHelper.BlueColor;
                                                                               textFrame.Width = Unit.FromCentimeter(14.0);
                                                                               textFrame.Height = "0.8cm";
                                                                           });
            
            var documentationRequiredFromSupplierTable = section.AddTable();
            documentationRequiredFromSupplierTable.AddColumn(1.8175.Inches());
            documentationRequiredFromSupplierTable.AddColumn(1.8175.Inches());
            documentationRequiredFromSupplierTable.AddColumn(1.8175.Inches());
            documentationRequiredFromSupplierTable.AddColumn(1.8175.Inches());
            documentationRequiredFromSupplierTable.AddNewRows(
                new[]
                    {
                        new object[] {"Pro-Forma Invoice", "Commercial Invoice", "Waybill/Airbill", "Packing List"},
                        new object[] {"Delivery Note", "Manuals", "Certificates", ""},
                    }, (row,index) => { row.VerticalAlignment = VerticalAlignment.Top;
                    }, (cell, paragraph, rowIndex, columnIndex) =>
                                {
                                    cell.Format.Alignment = ParagraphAlignment.Left;
                                    paragraph.Format.Alignment = ParagraphAlignment.Left;
                                    string imageUrl;
                                    if((rowIndex == 0 && (columnIndex == 0 || columnIndex == 2))||
                                        (rowIndex == 1 && (columnIndex == 0 || columnIndex == 1)))
                                    {
                                        imageUrl = "/Content/reports/checked.gif";
                                    }else
                                    {
                                        imageUrl = "/Content/reports/unchecked.gif";
                                    }
                                    var image = new Image(HttpContext.Current.Server.MapPath(imageUrl));
                                    image.WrapFormat.Style = WrapStyle.None;
                                    cell.Elements.InsertObject(0, image);
                                    paragraph.Format.LeftIndent = 18.PixelsToUnitPoints();
                                });
            section.AddVerticalSpacing("0.3cm");
            //section = document.AddNewSection();
            section.AddHeading("Terms and Conditions", (textFrame, paragraph) =>
            {
                var font = paragraph.Format.Font.Clone();
                paragraph.Format.Font = font;
                font.Color = PDFHelper.BlueColor;
                textFrame.Width = Unit.FromCentimeter(14.0);
                textFrame.Height = "0.8cm";
            });

            var textWidth = document.DefaultPageSetup.PageWidth;
            textWidth -= document.DefaultPageSetup.LeftMargin;
            textWidth -= document.DefaultPageSetup.RightMargin;

            section.AddTextBlock(@"Receipt of the signed PO acknowledgement copy, supplier's written confirmation of order shall constitute acceptance of the order by the supplier, including all terms and conditions herein contained. Acceptance is limited to the terms herein stated or incorporated by reference and any additional or different terms or conditions proposed by the supplier are rejected unless expressly approved in writing by an authorised representative of the DRC/DDG.
Prices on this purchase order are fixed and unalterable unless a replacement order is forwarded. All packages, Delivery Noted, Invoices and Shipping documents should indicate this unique purchase order number. All items must be delivered as stated in this purchase order.
This Purchase Order supersedes all prior oral or written agreements, if any, between the parties and constitutes the entire agreement in respect of this purchase. In reference to your offer, we hereby place the following order, subject to the terms described herein and Danish Refugee Council General Conditions for Purchase Orders that are incorporated by reference, to supply the item(s) and quantities(s) written in the PO.
");
            section.AddVerticalSpacing("0.3cm");
            
            section.AddHeading("Guarantee/Warranty", (textFrame, paragraph) =>
            {
                var font = paragraph.Format.Font.Clone();
                paragraph.Format.Font = font;
                font.Color = PDFHelper.BlueColor;
                textFrame.Width = textWidth;
                textFrame.Height = "0.8cm";
            });
            section.AddTextBlock(@"The supplier warrants that all items furnished hereunder are new and unused unless otherwise stated in PO. The supplier warrants that all the items furnished hereunder shall conform fully to all requirements of this order, shall conform to approved samples, if any, submitted against this requirement, shall be fit for the use for which they are intended, and shall be free from defects in material, workmanship and/or design.");
            section.AddVerticalSpacing("0.3cm");
            
            section.AddHeading("Pre-financing Guarantee", (textFrame, paragraph) =>
            {
                var font = paragraph.Format.Font.Clone();
                paragraph.Format.Font = font;
                font.Color = PDFHelper.BlueColor;
                textFrame.Width = Unit.FromCentimeter(14.0);
                textFrame.Height = "0.8cm";
            });

            var prefinancingTimeFrame = section.AddTextFrame();
            var prefinancingTimeFrameImage = new Image(HttpContext.Current.Server.MapPath("/Content/reports/unchecked.gif"));
            prefinancingTimeFrameImage.WrapFormat.Style = WrapStyle.None;
            prefinancingTimeFrame.Add(prefinancingTimeFrameImage);
            var prefinancingTimeFrameParagraph =prefinancingTimeFrame.AddParagraph(
@" The supplier is required to establish a pre-financing guarantee through a first class bank of 50 percent of the total value of this order.");
            prefinancingTimeFrameParagraph.Format.LeftIndent = 18.PixelsToUnitPoints();
            prefinancingTimeFrameParagraph.Format.Font = PDFHelper.CallibriFont11.Clone();
            prefinancingTimeFrame.Width = textWidth;
            section.AddVerticalSpacing("0.3cm");


            document.WriteAsPDFToStream(stream);   
        }

        #endregion
    }
}
