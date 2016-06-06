namespace SCMS.Reports.Reports
{
    partial class Report1
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup9 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup10 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup11 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector1 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector2 = new Telerik.Reporting.Drawing.DescendantSelector();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.detail = new Telerik.Reporting.DetailSection();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.TestSource = new Telerik.Reporting.ObjectDataSource();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.TestSource2 = new Telerik.Reporting.ObjectDataSource();
            this.table2 = new Telerik.Reporting.Table();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(2D);
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(2D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1,
            this.table2});
            this.detail.Name = "detail";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(2D);
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // TestSource
            // 
            this.TestSource.DataMember = "GetAll";
            this.TestSource.DataSource = typeof(SCMS.CoreBusinessLogic.BinCard.BinCardService);
            this.TestSource.Name = "TestSource";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D)));
            this.table1.Body.SetCellContent(0, 0, this.textBox5);
            this.table1.Body.SetCellContent(0, 1, this.textBox6);
            this.table1.Body.SetCellContent(0, 2, this.textBox7);
            this.table1.Body.SetCellContent(0, 3, this.textBox8);
            tableGroup1.ReportItem = this.textBox1;
            tableGroup2.ReportItem = this.textBox2;
            tableGroup3.ReportItem = this.textBox3;
            tableGroup4.ReportItem = this.textBox4;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.DataSource = this.TestSource;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1,
            this.textBox2,
            this.textBox3,
            this.textBox4,
            this.textBox5,
            this.textBox6,
            this.textBox7,
            this.textBox8});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.table1.Name = "table1";
            tableGroup5.Groupings.AddRange(new Telerik.Reporting.Grouping[] {
            new Telerik.Reporting.Grouping(null)});
            tableGroup5.Name = "Detail";
            this.table1.RowGroups.Add(tableGroup5);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4999213218688965D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.table1.StyleName = "Civic.TableNormal";
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox1.StyleName = "Civic.TableHeader";
            this.textBox1.Value = "Ref Number";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox2.StyleName = "Civic.TableHeader";
            this.textBox2.Value = "Description";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox3.StyleName = "Civic.TableHeader";
            this.textBox3.Value = "WHDemarcation";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox4.StyleName = "Civic.TableHeader";
            this.textBox4.Value = "Prepared On";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox5.StyleName = "Civic.TableBody";
            this.textBox5.Value = "=Fields.RefNumber";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox6.StyleName = "Civic.TableBody";
            this.textBox6.Value = "=Fields.Description";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox7.StyleName = "Civic.TableBody";
            this.textBox7.Value = "=Fields.WHDemarcation";
            // 
            // textBox8
            // 
            this.textBox8.Format = "{0:d}";
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6249803304672241D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox8.StyleName = "Civic.TableBody";
            this.textBox8.Value = "=Fields.PreparedOn";
            // 
            // TestSource2
            // 
            this.TestSource2.DataMember = "GetAssetList";
            this.TestSource2.DataSource = typeof(SCMS.CoreBusinessLogic._Inventory.InventoryService);
            this.TestSource2.Name = "TestSource2";
            // 
            // table2
            // 
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D)));
            this.table2.Body.SetCellContent(0, 0, this.textBox14);
            this.table2.Body.SetCellContent(0, 1, this.textBox15);
            this.table2.Body.SetCellContent(0, 2, this.textBox16);
            this.table2.Body.SetCellContent(0, 3, this.textBox17);
            this.table2.Body.SetCellContent(0, 4, this.textBox18);
            tableGroup6.ReportItem = this.textBox9;
            tableGroup7.ReportItem = this.textBox10;
            tableGroup8.ReportItem = this.textBox11;
            tableGroup9.ReportItem = this.textBox12;
            tableGroup10.ReportItem = this.textBox13;
            this.table2.ColumnGroups.Add(tableGroup6);
            this.table2.ColumnGroups.Add(tableGroup7);
            this.table2.ColumnGroups.Add(tableGroup8);
            this.table2.ColumnGroups.Add(tableGroup9);
            this.table2.ColumnGroups.Add(tableGroup10);
            this.table2.DataSource = this.TestSource2;
            this.table2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox9,
            this.textBox10,
            this.textBox11,
            this.textBox12,
            this.textBox13,
            this.textBox14,
            this.textBox15,
            this.textBox16,
            this.textBox17,
            this.textBox18});
            this.table2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.025019964203238487D), Telerik.Reporting.Drawing.Unit.Inch(0.80000019073486328D));
            this.table2.Name = "table2";
            tableGroup11.Groupings.AddRange(new Telerik.Reporting.Grouping[] {
            new Telerik.Reporting.Grouping(null)});
            tableGroup11.Name = "Detail";
            this.table2.RowGroups.Add(tableGroup11);
            this.table2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4749407768249512D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.table2.StyleName = "Civic.TableNormal";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox9.StyleName = "Civic.TableHeader";
            this.textBox9.Value = "Asset Number";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox10.StyleName = "Civic.TableHeader";
            this.textBox10.Value = "Name";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox11.StyleName = "Civic.TableHeader";
            this.textBox11.Value = "Serial Number";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox12.StyleName = "Civic.TableHeader";
            this.textBox12.Value = "Lifespan";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox13.StyleName = "Civic.TableHeader";
            this.textBox13.Value = "Depreciation Periods";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox14.StyleName = "Civic.TableBody";
            this.textBox14.Value = "=Fields.AssetNumber";
            // 
            // textBox15
            // 
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox15.StyleName = "Civic.TableBody";
            this.textBox15.Value = "=Fields.Name";
            // 
            // textBox16
            // 
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox16.StyleName = "Civic.TableBody";
            this.textBox16.Value = "=Fields.SerialNumber";
            // 
            // textBox17
            // 
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox17.StyleName = "Civic.TableBody";
            this.textBox17.Value = "=Fields.Lifespan";
            // 
            // textBox18
            // 
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2949881553649902D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox18.StyleName = "Civic.TableBody";
            this.textBox18.Value = "=Fields.DepreciationPeriods";
            // 
            // Report1
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "Report1";
            this.PageSettings.Margins.Bottom = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.PageSettings.Margins.Left = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.PageSettings.Margins.Right = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.PageSettings.Margins.Top = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
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
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.TableHeader")});
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector1});
            styleRule2.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            styleRule2.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule2.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(238)))), ((int)(((byte)(243)))));
            styleRule2.Style.Font.Name = "Georgia";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            styleRule2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            descendantSelector2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.TableBody")});
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector2});
            styleRule3.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule3.Style.Font.Name = "Georgia";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6.5D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.ObjectDataSource TestSource;
        private Telerik.Reporting.Table table2;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.ObjectDataSource TestSource2;
    }
}