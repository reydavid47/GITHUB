<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomMemberIDField.ascx.cs" Inherits="ClearCostWeb.Controls.CustomMemberIDField" %>

<asp:Repeater ID="rptFields" runat="server" 
    onitemdatabound="rptFields_ItemDataBound">
    <HeaderTemplate>
        Your <%# this.InsurerName %> ID
        <br />
    </HeaderTemplate>
    <ItemTemplate>
        <asp:TextBox ID="MemberID" 
            runat="server" 
            AutoCompleteType="Disabled" 
            CssClass="boxed prefilled" />
    </ItemTemplate>
    <SeparatorTemplate>
        <asp:Label runat="server" Text="-" style="margin:0px 10px;" />
    </SeparatorTemplate>
    <FooterTemplate>
        <br />
    </FooterTemplate>
</asp:Repeater>