<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=6.1.12.611, Culture=neutral, PublicKeyToken=a9d7983dfcc261be"
    Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>
<form clientidmode="Static" id="frep" runat="server">
<telerik:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="500px">
<Resources ReportParametersNullText="ALL" 
    ReportParametersPreviewButtonText="View Report"></Resources>
<typereportsource 
    
    typename="SCMS.Reports.Reports.OrderRequests, SCMS.Reports, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"></typereportsource>

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
    
    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    if (!IsPostBack)
    //    {
    //        Telerik.Reporting.InstanceReportSource instanceReportSource = new Telerik.Reporting.InstanceReportSource();
    //        instanceReportSource.ReportDocument = new SCMS.Reports.Reports.OrderRequests();
    //        this.ReportViewer1.ReportSource = instanceReportSource;
    //    }
    //}
</script>
