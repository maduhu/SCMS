<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=6.1.12.611, Culture=neutral, PublicKeyToken=a9d7983dfcc261be"
    Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>
<form clientidmode="Static" id="frep" runat="server">
<telerik:ReportViewer ID="ReportViewer1" Width="100%" Height="450px" runat="server">
<Resources ReportParametersNullText="ALL" 
    ReportParametersPreviewButtonText="View Report"></Resources>
</telerik:ReportViewer>
<script type="text/javascript">
    ReportViewer.prototype.GenerateReport = ReportViewer.prototype.RefreshReportArea;

    ReportViewer.prototype.RefreshReportArea = function (refresh) {
        if (refresh) {
            this.GenerateReport(refresh);
        }
        else {
            this.DisplayError("<br /><br />Click Refresh or Preview button to generate the report.");

            this.EnableRefreshButton(true);
        }
    }
</script>
</form>
<script runat="server">
        
    public override void VerifyRenderingInServerForm(Control control)
    {
        // to avoid the server form (<form runat="server">) requirement
    }

    protected override void OnLoad(EventArgs e)
    {
        // bind the report viewer
        base.OnLoad(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Telerik.Reporting.InstanceReportSource instanceReportSource = new Telerik.Reporting.InstanceReportSource();
            instanceReportSource.ReportDocument = new SCMS.Reports.Reports.ORDetails();
            this.ReportViewer1.ReportSource = instanceReportSource;
        }
    }
</script>
