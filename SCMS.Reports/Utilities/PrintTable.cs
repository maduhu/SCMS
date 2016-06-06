using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;

namespace SCMS.Reports.Utilities
{
    public class PrintTable
    {
        private DataTable dataTable;
        private List<double> columnWidths;
        TableOptions options;

        public PrintTable(DataTable tableData, List<double> columnWidths, TableOptions options = null)
        {
            this.dataTable = tableData;
            this.columnWidths = columnWidths;
            this.options = options;
        }


        /// <summary>
        /// Takes in a reference to the pdf DOM table object,, created before.
        /// </summary>
        /// <param name="domTable"></param>
        /// <returns></returns>
        public Table GenerateTable(Table domTable)
        {
            Color blueRow = new Color(219, 229, 241);

            foreach (double colWidth in this.columnWidths)
            {
                Column column = domTable.AddColumn(Unit.FromCentimeter(colWidth));
            }

            Row row = domTable.AddRow();
            row.Format.Alignment = ParagraphAlignment.Center;

            if (this.options != null && this.options.FontSizeCm > 0)
            {
                row.Format.Font = new Font("Calibri", Unit.FromPoint(this.options.FontSizeCm));
            }
            else
            {
                row.Format.Font = new Font("Calibri", Unit.FromPoint(11.0));
            }

            row.TopPadding = Unit.FromPoint(2.0);
            row.BottomPadding = Unit.FromPoint(2.0);

            for (int i = 0; i < this.dataTable.Columns.Count; i++)
            {
                row.Cells[i].AddParagraph(this.dataTable.Columns[i].ColumnName);
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].VerticalAlignment = VerticalAlignment.Bottom;

                domTable.SetEdge(i, 0, 1, 1, Edge.Box, BorderStyle.Single, 0.5, Color.Empty);
            }

            row.Shading.Color = blueRow; 

            for (int rowCounter = 0; rowCounter < this.dataTable.Rows.Count; rowCounter++)
            {
                row = domTable.AddRow();
                row.Format.Alignment = ParagraphAlignment.Center;

                if (this.options != null && this.options.FontSizeCm > 0)
                {
                    row.Format.Font = new Font("Calibri", Unit.FromPoint(this.options.FontSizeCm));
                }
                else
                {
                    row.Format.Font = new Font("Calibri", Unit.FromPoint(11.0));
                }
 
                row.Format.Font.Color = row.Format.Font.Color = new Color(255, 0, 0);
                row.TopPadding = Unit.FromPoint(4.0);
                row.BottomPadding = Unit.FromPoint(4.0);
                int colCounter = 0;

                foreach (DataColumn Dcol in this.dataTable.Columns)
                {
                    if (this.dataTable.Rows[rowCounter][colCounter] is DateTime)
                    {
                        row.Cells[colCounter].AddParagraph(Convert.ToDateTime(this.dataTable.Rows[rowCounter][colCounter]).ToString("dd.MMM.yyyy"));
                    }
                    else
                    {
                        row.Cells[colCounter].AddParagraph(this.dataTable.Rows[rowCounter][colCounter].ToString());
                    }


                    row.Cells[colCounter].Format.Alignment = ParagraphAlignment.Center;
                    row.Cells[colCounter].VerticalAlignment = VerticalAlignment.Center;

                    domTable.SetEdge(colCounter, rowCounter + 1, 1, 1, Edge.Box, BorderStyle.Single, 0.5, Color.Empty);

                    colCounter++;
                }
                
            }

            return domTable;
        }
    }
}
