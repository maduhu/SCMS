using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using  MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;

namespace MigraDoc.DocumentObjectModel
{
    public static class PDFHelper
    {
        public static readonly Font CallibriFont11=new Font("Calibri", 12.PixelsToUnitPoints());
        public static readonly Color SkyCloudyBlueColor = new Color(219, 229, 241);
        public static readonly Color GreenishGrayColor = new Color(234, 241, 221);
        public static readonly Color RedColor = new Color(255, 0, 0);
        public static readonly Color BlueColor = new Color(49, 132, 155);

        

        public static Document NewDocument()
        {
            var document = new Document();
            document.DefaultPageSetup.PageFormat = PageFormat.A4;
            var oneInch = new Unit(0.5, UnitType.Inch);
            document.DefaultPageSetup.LeftMargin =
                document.DefaultPageSetup.RightMargin =
                document.DefaultPageSetup.TopMargin =
                document.DefaultPageSetup.BottomMargin = oneInch;
            
            return document;
        }

        public static Document WithDocumentInfo(this Document document, string title, string subject, string author)
        {
            document.Info.Title = title;
            document.Info.Subject = subject;
            document.Info.Author = author;
            return document;
        }

        public static Section AddNewSection(this Document document)
        {
            var section = document.AddSection();
            return section;
        }

        public static double PixelsToPoints(this int pixels) { return pixels * 72.0 / 96.0; }
        public static Unit PixelsToUnitPoints(this int pixels) { return new Unit(pixels.PixelsToPoints()); }
        public static double Inches(this double value) { return new Unit(value, UnitType.Inch); }

        public static Section AddLogo(this Section section, Action<Image> callback=null, string logoFile=null)
        {
            var image = section.AddImage(HttpContext.Current.Server.MapPath(logoFile.IsNullOrWhiteSpace() ? "/Content/reports/pdf-logo.gif" : logoFile));
            
            image.LockAspectRatio = false;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = "14cm";
            image.WrapFormat.Style = WrapStyle.Through;
            if(logoFile.IsNullOrWhiteSpace())
            {
                image.Width = (187-0).PixelsToUnitPoints();
                image.Height = (234-0).PixelsToUnitPoints();
            }
            

            if (callback != null) callback(image);

            return section;
        }

        public static Cell AddSignature(this Cell cell, string signatureFile, Action<Image> callback = null)
        {
            if (signatureFile == null)
                return cell;
            var image = cell.AddImage(signatureFile);

            image.LockAspectRatio = false;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Column;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Center;
            image.WrapFormat.Style = WrapStyle.Through;
            //image.ScaleWidth = (150).PixelsToPoints();
            image.Width = (100 - 0).PixelsToUnitPoints();
            image.Height = (100 - 0).PixelsToUnitPoints();

            if (callback != null) callback(image);

            return cell;
        }

        public static Section AddHeading(this Section section,string heading, Action<TextFrame,Paragraph> callback=null)
        {
            var headingTextFrame = section.AddTextFrame();
            var tmpHeadParagraph = headingTextFrame.AddParagraph(heading);
            headingTextFrame.Top = ShapePosition.Top;
            tmpHeadParagraph.Format.Font = CallibriFont11.Clone();
            tmpHeadParagraph.Format.Font.Size = 15.PixelsToUnitPoints();

            if (callback != null) callback(headingTextFrame, tmpHeadParagraph);

            return section;
        }

        public static Section AddVerticalSpacing(this Section section, Unit height)
        {
            var verticalSpace = section.AddTextFrame();
            verticalSpace.Height = height;
            verticalSpace.Width = section.Document.DefaultPageSetup.PageWidth;
            return section;
        }

        public static Table AddNewRow(this Table table,object [] columns, Action<Row> rowCallback=null, Action<Cell,Paragraph, int> cellCallback=null)
        {
            var row = table.AddRow();
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font = CallibriFont11.Clone();
            row.TopPadding = Unit.FromPoint(2.0);
            row.BottomPadding = Unit.FromPoint(2.0);

            if (columns.IsNotNullOrEmpty())
            {
                for (var columnIndex=0; columnIndex < columns.Length; columnIndex++)
                {
                    row.Cells[columnIndex].AddParagraph(columns[columnIndex] == null ? "" : columns[columnIndex].ToString());
                    row.Cells[columnIndex].Format.Alignment = ParagraphAlignment.Center;
                    row.Cells[columnIndex].VerticalAlignment = VerticalAlignment.Center;
                    //table.SetEdge(columnIndex, table.Rows.Count - 1, 1, 1, Edge.Box, BorderStyle.Single, new Unit(0.5,UnitType.Point), Color.Empty);
                    if (cellCallback != null)
                        cellCallback(row.Cells[columnIndex], (Paragraph) row.Cells[columnIndex].Elements[0], columnIndex);
                }
            }
            if (rowCallback != null) rowCallback(row);

            return table;
        }

        public static Table AddNewRows(this Table table, object[][] matrix, Action<Row,int> rowCallback = null, Action<Cell, Paragraph, int, int> cellCallback = null)
        {
            for(var rowIndex=0; rowIndex < matrix.Length; rowIndex++)
            {
                Action<Row> matrixRowCallback = null;
                Action<Cell, Paragraph, int> matrixRowCellCallback = null;
                if(rowCallback!= null)
                {
                    var index = rowIndex;
                    matrixRowCallback = row => rowCallback(row, index);
                }
                if (cellCallback != null)
                {
                    var index = rowIndex;
                    matrixRowCellCallback = (cell, paragraph, columnIndex) => cellCallback(cell, paragraph, index, columnIndex);
                }

                table.AddNewRow(matrix[rowIndex],matrixRowCallback,matrixRowCellCallback);
            }

            return table;
        }

        public static Table AddNewTable(this Section section, string[] hearders, Unit[] columnSizes = null, Action<Row> rowCallback = null, Action<Cell, Paragraph, int> cellCallback = null)
        {
            
            var table = section.AddTable();
            table.Borders.Visible = true;
            if(hearders.IsNotNullOrEmpty())
            {
                var pageWidth = section.Document.DefaultPageSetup.PageWidth;
                var columnWidth = new Unit(pageWidth.Value/hearders.Length, pageWidth.Type);

                // Add columns
                for (var headerIndex = 0; headerIndex < hearders.Length; headerIndex++)
                {
                    table.AddColumn((columnSizes != null && headerIndex < columnSizes.Length)
                                        ? columnSizes[headerIndex]
                                        : columnWidth);

                }

                table.AddNewRow(hearders, row => { row.Shading.Color = SkyCloudyBlueColor; if (rowCallback != null)
                    rowCallback(row); }, cellCallback);
            }

            return table;
        }

        public static void WriteAsPDFToStream(this Document document, Stream stream)
        {
            var pdfRenderer = new PdfDocumentRenderer(true) {Document = document};
            pdfRenderer.RenderDocument();
            pdfRenderer.Save(stream, false);
        }
        
        public static Section AddTextBlock(this Section section,string text)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(14.0));
            table.AddNewRow(new object[] {text}, row =>
                                                     {
                                                         row.Format.Alignment = ParagraphAlignment.Left;
                                                         row.VerticalAlignment = VerticalAlignment.Top;
                                                         row.Format.Font = PDFHelper.CallibriFont11.Clone();
                                                     },(cell,paragraph,columnIndex)=>
                                                           {
                                                               cell.Format.Alignment = ParagraphAlignment.Left;
                                                               paragraph.Format.Alignment = ParagraphAlignment.Left;
                                                           });
            return section;
        }

        
    }
}
