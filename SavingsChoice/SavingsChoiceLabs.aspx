<%@ Page Title="" Language="C#" MasterPageFile="~/SavingsChoice/SavingsChoice.master" AutoEventWireup="true" CodeFile="SavingsChoiceLabs.aspx.cs" Inherits="ClearCostWeb.SavingsChoice.SavingsChoiceLabs" %>
<%@ Register TagName="starRating" TagPrefix="cchUIControl" Src="~/Controls/StarRating.ascx" %>
<%@ Register TagName="addNoAddressProviderControl" TagPrefix="cchSCIQ" Src="~/Controls/SavingsChoiceIQAddProviderNoAddress.ascx" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" Runat="Server">
    <link rel="stylesheet" href="../Styles/jquery-ui-1.10.3.css" />
    <link href="../Styles/overlay.css" rel="Stylesheet" />    
    <link href="../Styles/cch_gloabl.css" rel="stylesheet" type="text/css" />   
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="body" Runat="Server">
    <h4 class="sqiqDisplaySteps">Step 3 of 4</h4>
    <h3>Getting to Know You: Labs</h3>
    <p><asp:Literal ID="ratingText" runat="server"></asp:Literal></p>
    
        <asp:Repeater ID="rptSatisfaction" runat="server">
        <HeaderTemplate>
            <div class="satisfaction">
                <h2 class="avatar">Patient</h2>
                <h2 class="provider">Laboratory</h2>
                <h2 class="slider">Overall Satisfaction</h2>
            </div>
        </HeaderTemplate>
        <ItemTemplate>
            <div class="satisfaction row existing">
                <div class="avatar">
                    <%# ReadyDataForItem(Container) %>
                    &nbsp;
                </div>
                <div class="provider" pid="<%# ProviderID %>" oid="<%# OrganizationID %>">
                    <b><%# ProviderName%></b>
                    <div class="providerDetails">
                        <%# Address1 %>
                        <br />
                        <%# Address2 != string.Empty ? Address2 + "<br />" : "" %>
                        <%# String.Concat(City, ", ", State, " ", Zipcode) %>
                    </div>
                    <cchSCIQ:addNoAddressProviderControl ID="AddNoAddressProviderControl" runat="Server" />
                </div>
                <div class="userRatings" existingRating="<%# UserRating %>">
                     <cchUIControl:starRating runat="server" />
                </div>
                <div class="userAddedProvider existingProvider">
                    <div class="ui-state-default ui-corner-all review">
                        <span class="ui-icon ui-icon-comment"></span>
                        <label>write a review</label>
                        <textarea><%# Review %></textarea>
                    </div>        
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
        </FooterTemplate>
        </asp:Repeater>
    
    <asp:Panel ID="addProviderControl" runat="server"></asp:Panel>
    <div class="addProvider">
        <a href="#" 
            class="cch_showdialog" 
            cch_dialogtemplate="addProviderDialog"
            cch_click="addProvider()"
            cch_prerender="prepDisplayData()">
	            <img width="20" height="20" class="expand" alt="expand" src="../images/buttons/expand.png">
	            Add a Lab
        </a>    
    </div>    
    <div class="next">
        <a href="" class="sciqReturnLink">Return to Previous Page</a>
        <a href="" class="sciqCompleteLater">Complete IQ Later</a>
        <asp:HyperLink runat="server" CssClass="continueButton" NavigateUrl="~/SavingsChoice/SavingsChoiceImaging.aspx" Text="Continue" />
    </div>
        <script src="../Scripts/jquery-ui-1.10.3.js" type="text/javascript" language="javascript"></script>
    <script src="../Scripts/cch_global.js" type="text/javascript"></script>        
    <script src="../Scripts/cch_savingsChoiceUI.js" type="text/javascript"></script>   
    <script src="../Scripts/cch_starRating.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        /*cch_ui - page*/
        _cchGlobal.pageData["cchid"] = '<%= PrimaryCCHID %>';
        _cchGlobal.pageData["eid"] = '<%= EmployerID %>';
        _cchGlobal.pageData["sid"] = '<%= SessionID %>';
        _cchGlobal.pageData["newProviderCategory"] = 'lab';
        _cchGlobal.pageData["pageURL"] = 'SavingsChoiceLabs.aspx';

        $(document).ready(function () {
            _cchGlobal.actions.loadPage();
            gotoPreviousPage({
                action: 'redirect',
                goto: '/SavingsChoice/SavingsChoiceMedicalProviders.aspx'
            });
            addProvider({
                action: 'init'
            });
            avatarPlus1({
                action: 'init',
                parent: '.satisfaction.row div.avatar',
                children: 'img',
                filter: 'pointer'
            });
            completeIQlater({
                action: 'init'
            });
            starRating({
                action: 'init'
            });
            continueBtn({
                action: 'init',
                element: '.next a.continueButton'
            });
            addProvider({
                action: 'initReviewHandler'
            });
            starRating({
                action: 'showOriginalRating'
            });
            noAddressProvider({
                action:'init'
            });
        });   
    </script>
</asp:Content>

