<%@ Page Title="" Language="C#" MasterPageFile="ClientPages.master" AutoEventWireup="true" CodeFile="sign_in.aspx.cs" Inherits="ClearCostWeb.ClientPages.Default" %>
<%@ Register Src="~/Controls/SignIn.ascx" TagPrefix="cch" TagName="SignIn" %>

<asp:Content ID="SignIn_Content" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <%--<asp:Image ID="iLogo" runat="server" ImageUrl="~/Starbucks/Images/logo.png" AlternateText="Analog Devices Logo" ImageAlign="Right" style="" />--%>
    <asp:Image ID="imgLogo" runat="server" AlternateText="logo" Visible="false" ImageAlign="Right" Height="119px" />
    <p class="headline">
        Sign in to ClearCost Health</p>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td valign="top">
                <cch:SignIn ID="StarbucksSignIn" runat="server" ClientUrl="Starbucks" />     
                <%--<asp:LinkButton ID="lbtnRegister" runat="server" Text="Register Here" CssClass="submitlink" TabIndex="5" 
                    PostBackUrl="Welcome.aspx" Visible="false" style="margin-left: 5px;position:relative;top:100px;" />--%>
                <asp:Literal ID="lbtnRegister" runat="server" Visible="false">
                    <a href="Welcome.aspx" class="submitlink" style="margin-left: 5px;position: relative; top: 100px;">Register Here</a>
                </asp:Literal>
            </td>
            <td align="right" valign="baseline">
                <img src="../Images/laptop.png" border="0" alt="ClearCost Health Website on Laptop" /> 
            </td>
        </tr>
    </table>
</asp:Content>

