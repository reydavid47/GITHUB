<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true"
    CodeFile="results_rx_name.aspx.cs" Inherits="ClearCostWeb.SearchInfo.results_rx_name" %>

<asp:Content ID="results_rx_name_Content" ContentPlaceHolderID="ResultsContent" runat="Server">
    <h1>
        Search Results
    </h1>
    <p class="displayinline larger">
        Searched Drug:
        <b>
            <asp:Label ID="lblDrugName" runat="server" Text="" />
        </b>
    </p>
    &nbsp;&nbsp;
    <p class="displayinline smaller">
            <span style="display:inline-block;">[ <b><a href="Search.aspx">New Search</a></b> ]</span></p>
    <br class="clearboth" />
    <asp:Label ID="lblDrugVerification" runat="server" Text="" Visible="false" CssClass="smaller" style="padding-left: 135px" />
    <hr />
    <p>
        Choose the drug dose and quantity that applies to you:
    </p>
    <div id="refinebyname">
        <asp:Panel ID="pblRadioButtons" runat="server">
            <asp:Repeater ID="rptDrugDetails" runat="server" OnItemDataBound="rptDrugDetails_ItemDataBound" EnableViewState="true">
                <HeaderTemplate>
                    <table cellspacing="0" cellpadding="5" border="0" style="min-width:350px; max-width:650px;">
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="height:39px;">
                        <td style="min-width:30%; max-width:60%; overflow-x:auto;vertical-align:middle;">
                            <asp:RadioButton runat="server" ID="rbRefineDosage" CssClass="rbMed"/>
                            <%--<input type="radio" name="dosage" class="refinedosage" />
                            <asp:Label ID="lblRefineDosage" runat="server" Text="" />--%>
                        </td>
                        <td style="width:25%;vertical-align:middle;">
                            <span class="<%# IsHidden(Container) %> refineoptions">
                                <asp:DropDownList ID="ddlOptions" runat="server" DataTextField="DisplayQty" DataTextFormatString="{0}" DataValueField="Quantity" />
                            </span>
                        </td>
                        <td style="width:15%;vertical-align:middle;">
                            <span class="<%# IsHidden(Container) %> refineoptions">
                                <asp:LinkButton ID="lbtnSearch" runat="server" CssClass="submitlink" Text="Search" OnClick="SelectDrug" />
                            </span>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                    <asp:Literal ID="ltlNameJava" runat="server" Text="<script>$('.rbMed input').attr('name','selectMed');</script>" />
                </FooterTemplate>
            </asp:Repeater>
        </asp:Panel>
    </div>
</asp:Content>
