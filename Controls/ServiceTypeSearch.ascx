<%@ Control Language="C#" AutoEventWireup="True" CodeFile="ServiceTypeSearch.ascx.cs" Inherits="ClearCostWeb.Controls.ServiceTypeSearch" %>
<%@ Register Src="~/Controls/LetterSearch.ascx" TagPrefix="cch" TagName="LetterSearch" %>

<h3 style="float:left;height:25px;">
    Search by type of service we target:</h3>
    <asp:UpdateProgress ID="upLoading" runat="server" DisplayAfter="0">
        <ProgressTemplate>
            <asp:Image ID="imgLoader" runat="server" AlternateText="" ImageUrl="~/Images/ajax-loader-AltCircle.gif" style="float:left;margin-left:5px;height:25px;" />
        </ProgressTemplate>
    </asp:UpdateProgress>
<div class="clearboth"></div>
<asp:DropDownList ID="ddlSpecialties" runat="server" CssClass="boxed" Width="507px"
    DataTextField="Specialty" DataValueField="Specialty" style="display:block;"
    AutoPostBack="True" OnSelectedIndexChanged="ddlSpecialties_SelectedIndexChanged">
</asp:DropDownList>
<asp:DropDownList ID="ddlSubCategories" runat="server" CssClass="boxed" Width="507px"
    DataTextField="SubCategory" DataValueField="SubCategory"
    Visible="false" AutoPostBack="True" style="display:block;"
    OnSelectedIndexChanged="ddlSubCategories_SelectedIndexChanged">
</asp:DropDownList>
<asp:DropDownList ID="ddlLastCategory" runat="server" CssClass="boxed" Width="507px"
    DataTextField="CategoryLvl4" DataValueField="CategoryLvl4" style="display:block;"
    Visible="false" AutoPostBack="True" OnSelectedIndexChanged="ddlLastCategory_SelectedIndexChanged">
</asp:DropDownList>
<asp:Label ID="lblCareSearchNote2" runat="server" Text="" />
<asp:LinkButton ID="lnkBtnSearch2" runat="server" CssClass="submitlink" OnClick="lnkBtnSearch2_Click" Visible="false">Search</asp:LinkButton>
<asp:ListBox ID="lbCareSearchResults2" runat="server" Visible="false" CssClass="boxed"
     AutoPostBack="true" OnSelectedIndexChanged="lbCareSearchResults2_SelectedIndexChanged" style="display:block;" />
<asp:UpdatePanel ID="pnlLabLetters" runat="server" Visible="false" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <h3 style="color: Black; float:left; height: 25px; padding-top: 10px;">
            Select lab test by first letter:</h3>
        <asp:UpdateProgress ID="upLetterSpin" runat="server" DisplayAfter="0" AssociatedUpdatePanelID="pnlLabLetters">
            <ProgressTemplate>
                <asp:Image ID="Image1" runat="server" AlternateText="" ImageUrl="~/Images/CCHSpinLoader.gif" 
                    style="z-index: 1002; width: 35px; height: 35px; float:left;" />
            </ProgressTemplate>
        </asp:UpdateProgress>
        <br class="clearboth" />
        <cch:LetterSearch ID="lsLabSearch" runat="server" runMethod="SearchPage" />
    </ContentTemplate>
</asp:UpdatePanel>
