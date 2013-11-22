<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FindAService.ascx.cs"
    Inherits="ClearCostWeb.Controls.FindAService" %>
<%@ Register Src="~/Controls/TextSearch.ascx" TagPrefix="cch" TagName="TextSearch" %>
<%@ Register Src="~/Controls/ServiceTypeSearch.ascx" TagPrefix="cch" TagName="ServiceTypeSearch" %>
<h1>
    Search</h1>
<asp:Panel runat="server">
<cch:TextSearch ID="tsSearch" runat="server" SearchDescription="Search for your office visit, lab test, x-ray, or outpatient procedure:"
    runMethod="SearchPage"></cch:TextSearch>
    </asp:Panel>
<p>
    <b>OR</b>
</p>
<asp:UpdatePanel ID="upServiceType" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <cch:ServiceTypeSearch id="stsSearch" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
