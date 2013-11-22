<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="results_care.aspx.cs" Inherits="ClearCostWeb.SearchInfo.results_care" MaintainScrollPositionOnPostback="true" %>

<%@ Register Src="~/Controls/TextSearch.ascx" TagPrefix="cch" TagName="TextSearch" %>
<%@ Register Src="~/Controls/LearnMore.ascx" TagPrefix="cch" TagName="LearnMore" %>
<%--<%@ Register Src="~/Controls/Results.ascx" TagPrefix="cch" TagName="Results" %>--%>
<%@ Register Src="~/Controls/Print.ascx" TagPrefix="cch" TagName="Print" %>
<%@ Register Src="~/Controls/FindAServiceResults.ascx" TagPrefix="cch" TagName="FindAServiceResults" %>

<asp:Content ID="Results_Care_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <%--<cch:Print ID="pResultsCare" runat="server" PrintDiv="tabcare" />--%>
    <div id="tabcare">
        <div style="<%= ShowSingleSearch %>">
            <h1>Search Results</h1>
            <p class="displayinline larger">
                Searched Service:
                <b>
                    <% if (ClearCostWeb.ThisSession.IsShotVaccine || this.IsMammogram || this.IsCaesarsOphthalmology)
                       { %>*<% } %><asp:Label ID="lblServiceName" runat="server" Text="" />
                </b>
            </p>
            <asp:panel ID="pServiceLM" class="learnmore" ClientIDMode="Static" runat="server">
                <a title="Learn more">
                    <asp:Image ID="imgSvcLearnMore" runat="server" ImageUrl="../Images/icon_question_mark.png"
                        Width="12" Height="13" border="0" alt="Learn More" /></a>
                    <div class="moreinfo">
                        <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                            style="cursor: pointer;" />
                        <p>
                            <b class="upper">
                                <asp:Label ID="lblServiceName_MoreInfoTitle" runat="server" Text="" Font-Bold="true"></asp:Label></b><br />
                            <%--<asp:Label ID="lblServiceMoreInfo" runat="server" Text=""></asp:Label>--%>
                            <span id="ServiceMoreInfoText">Loading Detailed Information...</span>
                        </p>
                    </div>
                    <!-- end moreinfo -->
            </asp:panel>
            <p class="displayinline smaller">
                &nbsp;&nbsp;<span style="display:inline-block;">[ <b><a href="search.aspx">New Search</a></b> ]</span>
            </p>
            <br class="clearboth" />
            <asp:Label ID="lblServiceVerification" runat="server" Text="" Visible="false" CssClass="smaller" Style="padding-left: 135px"></asp:Label>
            <p class="displayinline larger">
                <asp:Label ID="lblFacilityTitle" runat="server" Text="You went to: "></asp:Label>
                <b>
                    <asp:Label ID="lblFacilityName" runat="server" CssClass="smaller" Text=""></asp:Label>
                </b>
            </p>
            <asp:Panel ID="medRef" ClientIDMode="Static" runat="server" Visible="false">
                <p class="displayinline larger">
                    Medicare reference price for this service: <b><asp:Label ID="lblCMSRate" runat="server" /></b>
                </p>
                <cch:LearnMore ID="lmCMS" runat="server">
                    <TitleTemplate>
                        Medicare Reimbursement
                    </TitleTemplate>
                    <TextTemplate>
                        The price for a service paid by the government entity, Centers for Medicare and
                        Medicaid Services (CMS), is considered a reference price because it is commonly
                        used by private hospitals, practices, and physicians as a baseline to set prices
                        for their services. A price that is less than 1.5 times the Medicare price is considered
                        to be a low price
                    </TextTemplate>
                </cch:LearnMore>
            </asp:Panel>
        </div>
        <div style="<%= ShowMultiSearch %>" >
            <h1>Lab Test Search Results</h1>            
            <asp:UpdatePanel ID="upMultiLab" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Repeater ID="rptLabTests" runat="server">
                        <HeaderTemplate>
                            <table>
                                <tbody>
                                    <tr>
                                        <td style="padding-right: 20px;">
                                            <p class="displayinline larger">
                                                Searched services:                
                                            </p>
                                        </td>
                                        <td style="padding-right: 20px;">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <p class="displayinline larger">
                                <b>
                                    <%# Eval("ServiceName") %>
                                </b>
                            </p>
                            <div class="learnmore">
                                <a title="Learn more">
                                    <asp:Image ID="imgSvcLearnMore" runat="server" ImageUrl="../Images/icon_question_mark.png" Width="12" Height="13" border="0" alt="Learn More" />
                                    <div class="moreinfo">
                                        <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right" style="cursor: pointer;" />
                                        <p>
                                            <b class="upper">
                                                <%# Eval("ServiceName")%>
                                            </b>
                                            <br />
                                            <div id="ServiceMoreInfoText<%# DataBinder.Eval(Container,"ItemIndex") %>"></div>
                                        </p>
                                    </div>
                                    <!-- end moreinfo -->
                            </div>
                        </ItemTemplate>
                        <SeparatorTemplate><br /></SeparatorTemplate>
                        <FooterTemplate>
                                        </td>
                                        <td style="padding-bottom: 3px;">
                                            <p class="smaller">
                                                <span style="display:inline-block;">[ <a href='<%= ResolveUrl("AddLab.aspx") %>'>Edit</a> ]</span> &nbsp; <span style="display:inline-block;">[ <b><a href='<%= ResolveUrl("Search.aspx") %>#tabcare'>New Search</a></b> ]</span>
                                            </p>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </ContentTemplate>
            </asp:UpdatePanel>
            
        </div>
        <hr />
        <asp:Panel ID="pnlThinPriceLegend" runat="server" Visible="false">
            <p class="smaller">
                <b class="green larger">$</b> Low price &nbsp;&nbsp; <b class="green larger">$$</b>
                High price &nbsp;&nbsp; <b class="green larger">$$$</b> Highest price
            </p>
        </asp:Panel>
        <asp:Panel ID="pnlPastCareDetails" runat="server" Visible="false">
            <div class="smaller alertsmall">
                <table cellspacing="0" cellpadding="0" border="0" width="500">
                    <tr>
                        <td>
                            Total price paid at
                            <asp:Label ID="lblFacilityNameDetails" runat="server" Text="Label"></asp:Label>:
                        </td>
                        <td align="right">
                            $1,084
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Best price:
                        </td>
                        <td align="right" style="border-bottom: 1px black solid;">
                            - &nbsp;&nbsp;&nbsp; $428
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Potential savings to you and your employer
                        </td>
                        <td align="right">
                            $656
                        </td>
                    </tr>
                </table>
            </div>
            <br class="clearboth" />
            <br />
        </asp:Panel>
        <%--<div class="buttonview viewshows" id="tableDiv" style="cursor:default;">
            <a class="table-map viewshows" id="showtableview" style="cursor:default;">Table View</a>
        </div>
        <div class="buttonview" id="mapDiv">
            <a class="table-map" id="showmapview">Map View</a>
        </div>
        <br class="clearboth" />
        <div id="tableview" class="showview">--%>
            <%--<asp:UpdatePanel ID="upFacility" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <%--<div class="displayinline smaller">
                        <table>
                            <tr>
                                <td style="vertical-align: middle;">
                                    <b>Sort by:</b>
                                </td>
                                <td style="vertical-align: middle;">
                                    <table id="rbHeaders">
                                        <tr>
                                            <td>
                                                <input id="iPracticeName" value="PracticeName" type="radio" sort="PracticeName"/>Name of Facility
                                            </td>
                                            <td>
                                                <input id="iDistance" value="NumericDistance" type="radio" checked="checked" sort="Distance" />Distance
                                            </td>
                                            <td>
                                                <input id="iEC" value="RangeMin" type="radio" sort="EstimatedCost" />Total Estimated Cost
                                            </td>
                                            <td class="iYC" style="display:none;">
                                                <input id="iYC" value="YourCostMin" type="radio" sort="YourCost" />Your Estimated Cost
                                            </td>
                                            <td>
                                                <input id="iFP" value="FP" type="radio" sort="FairPrice" />Fair Price
                                            </td>
                                        </tr>
                                    </table>
                                    <%--<asp:RadioButtonList ID="rblSort" runat="server" RepeatDirection="Horizontal" AutoPostBack="True"
                                        OnSelectedIndexChanged="rblSort_SelectedIndexChanged">
                                        <asp:ListItem Text="Facility" Value="PracticeName"></asp:ListItem>
                                        <asp:ListItem Text="Distance" Value="NumericDistance" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Total Cost" Value="RangeMin"></asp:ListItem>
                                        <asp:ListItem Text="Your Estimated Cost" Value="RangeMin"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="rbHeaders">
                        <span class="displayinline smaller"><b>Sort by:</b>
                            <span class="PRAC">
                                <input type="radio" name="sort" value="PracticeName" class="sortHeader" sortCol="PracticeName" />
                                Name of Facility
                            </span>
                            <span class="DIST">
                                <input type="radio" name="sort" value="distance" class="sortHeader" sortCol="Distance" checked="checked" />
                                Distance
                            </span>
                            <span class="EC">
                                <input type="radio" name="sort" value="RangeMin" class="sortHeader" sortCol="TotalCost" />
                                Total Estimated Cost
                            </span>
                            <span class="YC" style="display:none;">
                                <input type="radio" name="sort" value="your cost" class="sortHeader" sortCol="YourCost" />
                                Your Estimated Cost
                            </span>
                            <span class="FP">
                                <input type="radio" name="sort" value="Fair Price" class="sortHeader" sortCol="FP" />
                                Fair Price
                            </span>
                        </span>
                    </div>
                    <div class="toggle">
                        <b style="vertical-align:middle;">Your Estimated Cost:&nbsp;</b>
                        <img src="../Images/toggle_off.png" alt="" class="YC" style="cursor:pointer;vertical-align:middle;" />
                        <img src="../Images/toggle_on.png" alt="" class="YC" style="cursor:pointer;vertical-align:middle;display:none;" />
                    </div>
                    <cch:Results ID="rsltsFacility" runat="server" OnResultsSorted="UpdateRBL" OnCMSLoaded="rsltsFacility_CMSLoaded" OnLearnMoreLoaded="RefreshLearnMore" />
                </ContentTemplate>
            </asp:UpdatePanel>--%>
            <%--<center>
                
            </center><asp:UpdateProgress ID="dvLoading" runat="server" DisplayAfter="1">
                    <ProgressTemplate>
                        <img src="../Images/CCHSpinLoader.gif" id="imgLoading" width="35" height="35" alt="" />
                        <asp:Label ID="lblLoading" runat="server" Text="Retrieving..." Font-Size="Larger" Height="35" />
                    </ProgressTemplate>
                </asp:UpdateProgress>--%>
            <cch:FindAServiceResults runat="server" />
        </div>
        <%--<div id="mapview" class="hideview">
            <p class="smaller">
                Click on facility to see details.</p>
            <div style="position: relative;">
                <div id="resultmap" style="width: 840px; height: 500px; overflow: hidden; padding: 0px;
                    margin: 0px; position: relative; background-color: rgb(229, 227, 223);">
                </div>
                <div id="legend">
                    <p class="smaller" style="line-height: 35px;">
                        <span class="legendpin">Patient Location</span> <span class="legendfp">Fair Price provider</span>
                        <span class="legendp">Provider</span>
                    </p>
                </div>
            </div>
        </div>--%>
        <br />
        <br />
        <p style="display:inline;">
            <i class="smaller">Note: ClearCost Health does not have data for all possible medical services</i>            
        </p>
        <cch:LearnMore ID="LearnMore1" runat="server" style="cursor:default;">
            <TitleTemplate></TitleTemplate>
            <TextTemplate>
                We only provide price data for routine services that are performed in an outpatient setting.
                Try using our <a style="text-decoration:underline;" onclick="location.href='Search.aspx';">pulldowns</a> to see if we have coverage for the service you are looking for.
            </TextTemplate>
        </cch:LearnMore>
        <p>
            <asp:Label ID="lblAllResult1DisclaimerText" CssClass="smaller" runat="server" Font-Italic="True"></asp:Label>
        </p>
        <p>
            <asp:Label ID="lblAllResult2DisclaimerText" CssClass="smaller" runat="server" Font-Italic="True"></asp:Label>
        </p>
        <% if (ClearCostWeb.ThisSession.IsShotVaccine)
           { %>
            <p>
                <i class="smaller"><b>*</b>Cost of administration is not included</i>
            </p>
        <% } %>
        <% if (this.IsMammogram)
           { %>
            <p>
                <i class="smaller"><b>*</b>Cost of Computer Aided Detection (CAD) is not included.</i>
            </p>
        <% } %>
        <% if (this.IsCaesarsOphthalmology)
           { %>
            <p>
                <i class="smaller"><b>*</b>Providers listed on the ClearCost site are included in the Cigna network. For providers included in the Eye Med network, please contact Eye Med.</i>
            </p>
        <% } %>
        <% if (this.IsMentalHealth)
           { %>
            <p>
                <asp:Label ID="lblMentalHealthDisclaimerText" CssClass="smaller" runat="server" Font-Italic="True"></asp:Label>
            </p>
        <% } %>
    </div>
</asp:Content>

