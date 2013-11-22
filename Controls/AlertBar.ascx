<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AlertBar.ascx.cs" Inherits="ClearCostWeb.Controls.AlertBar" %>

<asp:Panel ID="alertbar" runat="server" CssClass="class_box_shadow">
    <asp:Panel ID="alert" runat="server">
        <p class="<%= pClass %>">
            <asp:PlaceHolder runat="server" ID="AlertPlaceHolder" />
        </p>
    </asp:Panel>
    <div class="clearboth"></div>
</asp:Panel>