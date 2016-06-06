namespace SCMS.Reports.Reports
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using System.Collections.Generic;

    /// <summary>
    /// Summary description for Report2.
    /// </summary>
    public partial class Report2 : Telerik.Reporting.Report
    {
        public Report2()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private const int TableRowsCount = 21;

        private void table1_NeedDataSource(object sender, EventArgs e)
        {

            Telerik.Reporting.Processing.Report report = (Telerik.Reporting.Processing.Report)sender;
            this.sqlDataSource1.Parameters[0].Value = this.ReportParameters["RefNo"].Value;
            
            report.DataSource = this.sqlDataSource1;
        }

        private void Report2_NeedDataSource(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.Report report = (Telerik.Reporting.Processing.Report)sender;
            this.ReportDatasource.Parameters[0].Value = this.ReportParameters["RefNo"].Value;

            report.DataSource = this.ReportDatasource;

        } 
        
        
      

        
    }
}