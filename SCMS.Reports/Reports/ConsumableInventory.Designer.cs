namespace SCMS.Reports.Reports
{
    partial class ConsumableInventory
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsumableInventory));
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector1 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector2 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector3 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule5 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector4 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule6 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector5 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule7 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector6 = new Telerik.Reporting.Drawing.DescendantSelector();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox31 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.ConsumableInventoryDatasource = new Telerik.Reporting.SqlDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.currentTimeTextBox = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.crosstab2 = new Telerik.Reporting.Crosstab();
            this.textBox24 = new Telerik.Reporting.TextBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox22 = new Telerik.Reporting.TextBox();
            this.textBox23 = new Telerik.Reporting.TextBox();
            this.textBox35 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox26 = new Telerik.Reporting.TextBox();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.pictureBox5 = new Telerik.Reporting.PictureBox();
            this.textBox95 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.61250007152557373D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox10.Style.Font.Name = "Calibri";
            this.textBox10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox10.StyleName = "Civic.TableGroup";
            this.textBox10.Value = "=Fields.Location";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.706250011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox11.Style.Font.Name = "Calibri";
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox11.StyleName = "Civic.GrandTotal";
            this.textBox11.Value = "= SCMS.Reports.Utilities.ReportsInternalisation.Report_ConsumableInventory_TotalQ" +
    "ty()";
            // 
            // textBox31
            // 
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox31.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.textBox31.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox31.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox31.Style.Font.Name = "Calibri";
            this.textBox31.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox31.StyleName = "";
            this.textBox31.Value = "= Fields.UnitCode";
            // 
            // textBox15
            // 
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.925000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox15.Style.Font.Name = "Calibri";
            this.textBox15.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox15.StyleName = "Civic.TableGroup";
            this.textBox15.Value = "=Fields.ItemDescription";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox14.Style.Font.Name = "Calibri";
            this.textBox14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox14.StyleName = "Civic.TableGroup";
            this.textBox14.Value = "=Fields.ItemName";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.82083338499069214D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox13.Style.Font.Name = "Calibri";
            this.textBox13.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox13.StyleName = "Civic.TableGroup";
            this.textBox13.Value = "=Fields.ClasificationName";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox12.Style.Font.Name = "Calibri";
            this.textBox12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox12.StyleName = "Civic.TableGroup";
            this.textBox12.Value = "=Fields.SerialNumber";
            // 
            // ConsumableInventoryDatasource
            // 
            this.ConsumableInventoryDatasource.ConnectionString = "SCMS.Reports.Properties.Settings.SCMS";
            this.ConsumableInventoryDatasource.Name = "ConsumableInventoryDatasource";
            this.ConsumableInventoryDatasource.SelectCommand = resources.GetString("ConsumableInventoryDatasource.SelectCommand");
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D);
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            this.pageHeaderSection1.Style.Font.Name = "Calibri";
            this.pageHeaderSection1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            // 
            // currentTimeTextBox
            // 
            this.currentTimeTextBox.Format = "{0:d}";
            this.currentTimeTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.currentTimeTextBox.Name = "currentTimeTextBox";
            this.currentTimeTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3191666603088379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.currentTimeTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.currentTimeTextBox.StyleName = "PageInfo";
            this.currentTimeTextBox.Value = "=NOW()";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.4999995231628418D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.crosstab2});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Calibri";
            this.detail.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            // 
            // crosstab2
            // 
            this.crosstab2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.61250007152557373D)));
            this.crosstab2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.706250011920929D)));
            this.crosstab2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D)));
            this.crosstab2.Body.SetCellContent(0, 0, this.textBox24);
            this.crosstab2.Body.SetCellContent(0, 1, this.textBox25);
            tableGroup1.Groupings.AddRange(new Telerik.Reporting.Grouping[] {
            new Telerik.Reporting.Grouping("=Fields.Location")});
            tableGroup1.Name = "Location1";
            tableGroup1.ReportItem = this.textBox10;
            tableGroup1.Sortings.AddRange(new Telerik.Reporting.Sorting[] {
            new Telerik.Reporting.Sorting("=Fields.Location", Telerik.Reporting.SortDirection.Asc)});
            tableGroup2.ReportItem = this.textBox11;
            this.crosstab2.ColumnGroups.Add(tableGroup1);
            this.crosstab2.ColumnGroups.Add(tableGroup2);
            this.crosstab2.Corner.SetCellContent(0, 0, this.textBox20);
            this.crosstab2.Corner.SetCellContent(0, 1, this.textBox21);
            this.crosstab2.Corner.SetCellContent(0, 2, this.textBox22);
            this.crosstab2.Corner.SetCellContent(0, 3, this.textBox23);
            this.crosstab2.Corner.SetCellContent(0, 4, this.textBox35);
            this.crosstab2.DataSource = this.ConsumableInventoryDatasource;
            this.crosstab2.Filters.AddRange(new Telerik.Reporting.Filter[] {
            new Telerik.Reporting.Filter("=Cstr(Fields.LocationId)", Telerik.Reporting.FilterOperator.In, "=isnull(Parameters.Location.Value, Fields.LocationId)"),
            new Telerik.Reporting.Filter("=Fields.ItemId", Telerik.Reporting.FilterOperator.Equal, "=isnull(Parameters.Item.Value, Fields.ItemId)"),
            new Telerik.Reporting.Filter("=Fields.CountryProgrammeId", Telerik.Reporting.FilterOperator.Equal, "= SCMS.Reports.Utilities.ReportHelper.GetCountryProgram()")});
            this.crosstab2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox24,
            this.textBox25,
            this.textBox10,
            this.textBox11,
            this.textBox20,
            this.textBox21,
            this.textBox22,
            this.textBox23,
            this.textBox35,
            this.textBox12,
            this.textBox13,
            this.textBox14,
            this.textBox15,
            this.textBox31});
            this.crosstab2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(7.8837074397597462E-05D));
            this.crosstab2.Name = "crosstab2";
            tableGroup8.Name = "Group1";
            tableGroup7.ChildGroups.Add(tableGroup8);
            tableGroup7.Name = "Group3";
            tableGroup7.ReportItem = this.textBox31;
            tableGroup6.ChildGroups.Add(tableGroup7);
            tableGroup6.Groupings.AddRange(new Telerik.Reporting.Grouping[] {
            new Telerik.Reporting.Grouping("=Fields.ItemDescription")});
            tableGroup6.Name = "ItemDescription1";
            tableGroup6.ReportItem = this.textBox15;
            tableGroup6.Sortings.AddRange(new Telerik.Reporting.Sorting[] {
            new Telerik.Reporting.Sorting("=Fields.ItemDescription", Telerik.Reporting.SortDirection.Asc)});
            tableGroup5.ChildGroups.Add(tableGroup6);
            tableGroup5.Groupings.AddRange(new Telerik.Reporting.Grouping[] {
            new Telerik.Reporting.Grouping("=Fields.ItemName")});
            tableGroup5.Name = "ItemName1";
            tableGroup5.ReportItem = this.textBox14;
            tableGroup5.Sortings.AddRange(new Telerik.Reporting.Sorting[] {
            new Telerik.Reporting.Sorting("=Fields.ItemName", Telerik.Reporting.SortDirection.Asc)});
            tableGroup4.ChildGroups.Add(tableGroup5);
            tableGroup4.Groupings.AddRange(new Telerik.Reporting.Grouping[] {
            new Telerik.Reporting.Grouping("=Fields.ClasificationName")});
            tableGroup4.Name = "ClasificationName1";
            tableGroup4.ReportItem = this.textBox13;
            tableGroup4.Sortings.AddRange(new Telerik.Reporting.Sorting[] {
            new Telerik.Reporting.Sorting("=Fields.ClasificationName", Telerik.Reporting.SortDirection.Asc)});
            tableGroup3.ChildGroups.Add(tableGroup4);
            tableGroup3.Groupings.AddRange(new Telerik.Reporting.Grouping[] {
            new Telerik.Reporting.Grouping("=Fields.SerialNumber")});
            tableGroup3.Name = "SerialNumber1";
            tableGroup3.ReportItem = this.textBox12;
            tableGroup3.Sortings.AddRange(new Telerik.Reporting.Sorting[] {
            new Telerik.Reporting.Sorting("=Fields.SerialNumber", Telerik.Reporting.SortDirection.Asc)});
            this.crosstab2.RowGroups.Add(tableGroup3);
            this.crosstab2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.164583683013916D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.crosstab2.Style.Font.Name = "Calibri";
            this.crosstab2.StyleName = "Civic.TableNormal";
            // 
            // textBox24
            // 
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.61250007152557373D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox24.Style.Font.Name = "Calibri";
            this.textBox24.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox24.StyleName = "Civic.TableBody";
            this.textBox24.Value = "=isnull(Sum(Fields.Quantity),0)";
            // 
            // textBox25
            // 
            this.textBox25.Format = "{0:N0}";
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.706250011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox25.Style.Font.Name = "Calibri";
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox25.StyleName = "Civic.GrandTotal";
            this.textBox25.Value = "=Sum(Fields.Quantity)";
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox20.Style.Font.Name = "Calibri";
            this.textBox20.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox20.StyleName = "Civic.TableHeader";
            this.textBox20.Value = "= SCMS.Reports.Utilities.ReportsInternalisation.Report_ConsumableInventory_Serial" +
    "No()";
            // 
            // textBox21
            // 
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.82083338499069214D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox21.Style.Font.Name = "Calibri";
            this.textBox21.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox21.StyleName = "Civic.TableHeader";
            this.textBox21.Value = "= SCMS.Reports.Utilities.ReportsInternalisation.Budget_CategoryList_CategoryPopup" +
    "Header()";
            // 
            // textBox22
            // 
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox22.Style.Font.Name = "Calibri";
            this.textBox22.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox22.StyleName = "Civic.TableHeader";
            this.textBox22.Value = "= SCMS.Reports.Utilities.ReportsInternalisation.Global_String_ItemName()";
            // 
            // textBox23
            // 
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.925000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox23.Style.Font.Name = "Calibri";
            this.textBox23.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox23.StyleName = "Civic.TableHeader";
            this.textBox23.Value = "= SCMS.Reports.Utilities.ReportsInternalisation.Budget_CategoryList_Description()" +
    "";
            // 
            // textBox35
            // 
            this.textBox35.Name = "textBox35";
            this.textBox35.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox35.Style.Font.Name = "Calibri";
            this.textBox35.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox35.StyleName = "Civic.TableHeader";
            this.textBox35.Value = "= SCMS.Reports.Utilities.ReportsInternalisation.Global_String_Unit()";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox8});
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.Font.Name = "Calibri";
            this.pageFooterSection1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            // 
            // textBox8
            // 
            this.textBox8.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.5000009536743164D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4999994039535523D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox8.Value = "=PageNumber+\" of \"+ PageCount";
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.497838020324707D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1,
            this.textBox2,
            this.textBox5,
            this.textBox6,
            this.textBox7,
            this.textBox26,
            this.textBox27,
            this.currentTimeTextBox,
            this.pictureBox5,
            this.textBox95});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.reportHeaderSection1.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.reportHeaderSection1.Style.Font.Name = "Calibri";
            this.reportHeaderSection1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            // 
            // textBox1
            // 
            this.textBox1.Format = "{0:d}";
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D));
            this.textBox1.Style.Font.Name = "Calibri";
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox1.Value = ": {SCMS.Reports.Utilities.ReportHelper.GetDefaultstr(Parameters.Item.Label)}\r\n";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99992114305496216D), Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D));
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.Font.Name = "Calibri";
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox2.Value = "= SCMS.Reports.Utilities.ReportsInternalisation.Global_String_ItemName()";
            // 
            // textBox5
            // 
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3192849159240723D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8807148933410645D), Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D));
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.Style.Font.Name = "Calibri";
            this.textBox5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(18D);
            this.textBox5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox5.Value = "= SCMS.Reports.Utilities.ReportsInternalisation.SCMSReports()";
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.4750000536441803D));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99992114305496216D), Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D));
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.Style.Font.Name = "Calibri";
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox6.Value = "= SCMS.Reports.Utilities.ReportsInternalisation.Global_String_Report()";
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.78750008344650269D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99992114305496216D), Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D));
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.Font.Name = "Calibri";
            this.textBox7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox7.Value = "= SCMS.Reports.Utilities.ReportsInternalisation.Global_String_Location()";
            // 
            // textBox26
            // 
            this.textBox26.Format = "{0:d}";
            this.textBox26.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.78750008344650269D));
            this.textBox26.Name = "textBox26";
            this.textBox26.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D));
            this.textBox26.Style.Font.Name = "Calibri";
            this.textBox26.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox26.Value = ": {SCMS.Reports.Utilities.ReportHelper.GetDefaultstrings(Parameters.Location.Labe" +
    "l)}\r\n";
            // 
            // textBox27
            // 
            this.textBox27.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.4750000536441803D));
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D));
            this.textBox27.Style.Font.Name = "Calibri";
            this.textBox27.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox27.Value = ": {SCMS.Reports.Utilities.ReportsInternalisation.Global_String_Inventory()}";
            // 
            // pictureBox5
            // 
            this.pictureBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.3000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pictureBox5.MimeType = "";
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(1.0478378534317017D));
            this.pictureBox5.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pictureBox5.Style.Font.Name = "Calibri";
            this.pictureBox5.Value = "= SCMS.Reports.Utilities.ReportHelper.GetLogoUrl()";
            // 
            // textBox95
            // 
            this.textBox95.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.3000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(1.0479167699813843D));
            this.textBox95.Name = "textBox95";
            this.textBox95.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5979160070419312D), Telerik.Reporting.Drawing.Unit.Inch(0.44992122054100037D));
            this.textBox95.Style.Font.Name = "Calibri";
            this.textBox95.Value = "= SCMS.Reports.Utilities.ReportHelper.GetCountryProgramAddress()";
            // 
            // ConsumableInventory
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "ConsumableInventory";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D);
            this.PageSettings.Margins.Left = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.Margins.Right = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.Margins.Top = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.ConsumableInventoryDatasource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Location";
            reportParameter1.AvailableValues.Filters.AddRange(new Telerik.Reporting.Filter[] {
            new Telerik.Reporting.Filter("=Fields.CountryProgrammeId", Telerik.Reporting.FilterOperator.Equal, "= SCMS.Reports.Utilities.ReportHelper.GetCountryProgram()")});
            reportParameter1.AvailableValues.ValueMember = "=Cstr(Fields.LocationId)";
            reportParameter1.MultiValue = true;
            reportParameter1.Name = "Location";
            reportParameter1.Text = "Location";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.ConsumableInventoryDatasource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.ItemName";
            reportParameter2.AvailableValues.Filters.AddRange(new Telerik.Reporting.Filter[] {
            new Telerik.Reporting.Filter("=Cstr(Fields.LocationId)", Telerik.Reporting.FilterOperator.In, "=isnull(Parameters.Location.Value, Fields.LocationId)"),
            new Telerik.Reporting.Filter("=Fields.CountryProgrammeId", Telerik.Reporting.FilterOperator.Equal, "= SCMS.Reports.Utilities.ReportHelper.GetCountryProgram()")});
            reportParameter2.AvailableValues.ValueMember = "=Cstr(Fields.ItemId)";
            reportParameter2.Name = "Item";
            reportParameter2.Text = "Item";
            reportParameter2.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.Table), "Civic.TableNormal")});
            styleRule1.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule1.Style.Color = System.Drawing.Color.Black;
            styleRule1.Style.Font.Name = "Georgia";
            styleRule1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            descendantSelector1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.TableGroup")});
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector1});
            styleRule2.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            styleRule2.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule2.Style.Font.Name = "Georgia";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            descendantSelector2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.GrandTotal")});
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector2});
            styleRule3.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(214)))), ((int)(((byte)(214)))));
            styleRule3.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule3.Style.Font.Bold = true;
            styleRule3.Style.Font.Italic = false;
            styleRule3.Style.Font.Name = "Georgia";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            styleRule3.Style.Font.Strikeout = false;
            styleRule3.Style.Font.Underline = false;
            descendantSelector3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.SubTotal")});
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector3});
            styleRule4.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(235)))), ((int)(((byte)(234)))));
            styleRule4.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule4.Style.Font.Name = "Georgia";
            styleRule4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            descendantSelector4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.TableHeader")});
            styleRule5.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector4});
            styleRule5.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            styleRule5.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule5.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(238)))), ((int)(((byte)(243)))));
            styleRule5.Style.Font.Name = "Georgia";
            styleRule5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            styleRule5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            descendantSelector5.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.TableBody")});
            styleRule6.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector5});
            styleRule6.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule6.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule6.Style.Font.Name = "Georgia";
            styleRule6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            descendantSelector6.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.TableCorner")});
            styleRule7.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector6});
            styleRule7.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule7.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule7.Style.Font.Name = "Georgia";
            styleRule7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4,
            styleRule5,
            styleRule6,
            styleRule7});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(10D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.SqlDataSource ConsumableInventoryDatasource;
        private Telerik.Reporting.Crosstab crosstab2;
        private Telerik.Reporting.TextBox textBox24;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox22;
        private Telerik.Reporting.TextBox textBox23;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.TextBox currentTimeTextBox;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox26;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.TextBox textBox35;
        private Telerik.Reporting.TextBox textBox31;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.PictureBox pictureBox5;
        private Telerik.Reporting.TextBox textBox95;
    }
}