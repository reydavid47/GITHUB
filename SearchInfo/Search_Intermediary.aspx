<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="Search_Intermediary.aspx.cs" Inherits="ClearCostWeb.SearchInfo.Search_Intermediary" %>
<%@ Register TagPrefix="cch" TagName="TextSearch" Src="~/Controls/TextSearch.ascx" %>

<asp:Content ID="Search_Intermediary_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <div id="tabcare">
        <h1>Search Results</h1>
        <cch:TextSearch ID="tsIntermediary" runat="server" SearchDescription="Search for your Office Visit, Lab Test, X-Ray, or Outpatient Procedure:" runMethod="Intermediary" />
        <hr />
        <%if (IsServiceNotFound)
          { %>
            <asp:Label ID="lblServiceNotFoundDisclaimerTitle" runat="server" text="" style="font-weight:bold;"  />
            <br />
            <asp:Label ID="lblServiceNotFoundDisclaimerText" runat="server" text="" />
            <br /><br />
        <%} %>
        <span>
            <b><asp:Label ID="lblResultCount" runat="server" Text="" /></b>
            <asp:Label ID="lblBetween" runat="server" Text="possible results for " />
            <asp:Label ID="lblSearchTerm" runat="server" style="font-style:italic;" text="" />
            <asp:label id="lblNoResults" runat="server" 
                Text="<br />We were unable to determine the service you may have been searching for.<br />Please try your search again using a different word or phrase, or check our pulldown menus on Find a Service to make sure the service you are looking for is offered by ClearCost Health." Visible="false" />
        </span>
        <br class="clearboth" />
        <asp:Panel ID="pnlInstead" runat="server" Visible="false">
            <p class="displayinline smaller">
                Search instead for
                <asp:LinkButton ID="lbOldSearch" runat="server" OnClick="lbOldSearch_Click" />
            </p>
            <br class="clearboth" />
        </asp:Panel>
        <asp:Repeater ID="rptSearchList" runat="server" >
            <HeaderTemplate>
                <table style="margin-top: 1em; margin-left:2em;">
                    <tr>
                        <td>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:LinkButton ID="lbtnSelectSearch" runat="server" CssClass="readmore" Text='<%# Eval("ServiceName") %>'
                    CommandArgument='<%# Eval("ServiceID") %>' CommandName="Select" OnClick="lbtnSelectSearch_Click" />
            </ItemTemplate>
            <SeparatorTemplate>
                        </td>
                    </tr>
                    <tr>
                        <td>    
            </SeparatorTemplate>
            <FooterTemplate>
                        </td>
                    </tr>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>

