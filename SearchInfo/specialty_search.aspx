<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="specialty_search.aspx.cs" Inherits="ClearCostWeb.SearchInfo.specialty_search" %>
<%@ Register Src="~/Controls/AlertBar.ascx" TagPrefix="cch" TagName="AlertBar" %>

<asp:Content ID="specialty_search_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <h1>
        Search</h1>
    <p class="displayinline larger">
        You have searched for:<b>
            <asp:Label ID="lblServiceName" runat="server" Text=""></asp:Label></b></p>
            <br /><br />
    <p class="displayinline">
    <table width="100%">
    <tr>
    <td width="150px"> 
        <asp:Image ID="imgStep1" runat="server" AlternateText="Step 1" ImageUrl="../Images/arrow_step1.png" /><%--                                       
        <asp:Label ID="lblStep1Title" runat="server" Text="Step 1:"></asp:Label>--%>
    </td>
    <td>
        <asp:Label ID="lblSelectSpecialtyTitle" runat="server" CssClass="h3Style" Text="Select the appropriate specialty:"></asp:Label>
        <br />
        <asp:SqlDataSource ID="dsSpecialtyList" runat="server" ConnectionString="<%$ ConnectionStrings:CCH_FrontEnd %>"
            SelectCommand="GetProviderSpecialtyList" 
            SelectCommandType="StoredProcedure"></asp:SqlDataSource>
        <asp:DropDownList ID="ddlSpecialty" runat="server" CssClass="boxed"  Width="400px"
            DataSourceID="dsSpecialtyList"  DataTextField="Specialty" DataValueField="SpecialtyID"
            ondatabound="ddlSpecialty_DataBound" 
            onselectedindexchanged="ddlSpecialty_SelectedIndexChanged" AutoPostBack="True">
        </asp:DropDownList>
        <br /><br /><br /></td>
    </tr>
    <tr>
    <td width="150px">
        <asp:Image ID="imgStep2" runat="server" AlternateText="Step 1" ImageUrl="../Images/arrow_step2.png" Visible="false" />
        <%-- <asp:Label ID="lblStep2Title" runat="server" Text="Step 2:" Visible="false"></asp:Label>--%>
    </td>
    <td>
        <asp:Label ID="lblSelectVisitType" runat="server" CssClass="h3Style" 
            Text="Select the type of office visit:" Visible="False"></asp:Label>
            <br />
        <asp:RadioButtonList ID="rblOfficeVisitTypes" runat="server" 
            AutoPostBack="True" 
            onselectedindexchanged="rblOfficeVisitTypes_SelectedIndexChanged" 
            Visible="False">
                                        
        </asp:RadioButtonList>
        </td>
    </tr>
    </table>
    </p>
    <cch:AlertBar ID="abSpecialtyNetworks" runat="server" TypeOfAlert="Normal">
    <MessageTemplate>
        <%# Container.StaticText %>
    </MessageTemplate>
</cch:AlertBar>
</asp:Content>

