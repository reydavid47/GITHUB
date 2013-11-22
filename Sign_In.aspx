<%@ Page Title="" Language="C#" MasterPageFile="~/SignIn.master" AutoEventWireup="true" CodeFile="Sign_In.aspx.cs" Inherits="ClearCostWeb.Public.Sign_In" %>
<%@ Register Src="~/Controls/SignIn.ascx" TagPrefix="cch" TagName="SignIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <p class="headline">
        Sign in to ClearCost Health</p>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td valign="top">
                <%--<p>
                        With health care costs rising, patients must pay more out-of-pocket for their health care.
                        ClearCost Health partners with your employer to provide key pricing and quality
                        information about local health care facilities.</p>
                            --%>
                <cch:SignIn id="MainSignIn" runat="server" />  
            </td>
            <td align="right" valign="baseline">
                <img src="Images/laptop.jpg" border="0" alt="ClearCost Health Website on Laptop"/>
            </td>
        </tr>
    </table>
</asp:Content>

