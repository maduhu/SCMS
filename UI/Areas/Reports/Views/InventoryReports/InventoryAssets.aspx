<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%--<%@ Register Assembly="SCMS.Resource ,Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" Namespace="SCMS.Resource" TagPrefix="SCMS" %>--%>
<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=6.1.12.611, Culture=neutral, PublicKeyToken=a9d7983dfcc261be"
    Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>

<form clientidmode="Static" id="frep" runat="server">
<telerik:ReportViewer ID="ReportViewer1" runat="server" Height="500px" Width="100%">
<Resources ReportParametersNullText="All" 
    ReportParametersPreviewButtonText="View Report"></Resources>

<typereportsource 
    typename="SCMS.Reports.Reports.AssetInventory, SCMS.Reports, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"></typereportsource>
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

</script>
