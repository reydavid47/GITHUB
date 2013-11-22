<%@ Page Title="" Language="C#" MasterPageFile="Starbucks.master" AutoEventWireup="true" CodeFile="sign_in.aspx.cs" Inherits="ClearCostWeb.Starbucks.Default" %>
<%@ Register Src="~/Controls/SignIn.ascx" TagPrefix="cch" TagName="SignIn" %>

<asp:Content ID="SignIn_Content" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Image ID="iLogo" runat="server" ImageUrl="~/Starbucks/Images/logo.png" AlternateText="Analog Devices Logo" ImageAlign="Right" style="" />
    <p class="headline">
        Sign in to ClearCost Health</p>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td valign="top">
                <cch:SignIn ID="StarbucksSignIn" runat="server" ClientUrl="Starbucks" />     
            </td>
            <td align="right" valign="baseline">
                <img src="Images/laptop.png" border="0" alt="ClearCost Health Website on Laptop" />
            </td>
        </tr>
    </table>
</asp:Content>

