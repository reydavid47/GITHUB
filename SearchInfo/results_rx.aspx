<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="results_rx.aspx.cs" Inherits="ClearCostWeb.SearchInfo.results_rx" %>
<%@ Register Src="~/Controls/AlertBar.ascx" TagPrefix="cch" TagName="AlertBar" %>
<%@ Register Src="~/Controls/Print.ascx" TagPrefix="cch" TagName="Print" %>
<%@ Register Src="~/Controls/LearnMore.ascx" TagPrefix="cch" TagName="LearnMore" %>
<%@ Register Src="~/Controls/Slider.ascx" TagPrefix="cch" TagName="Slider" %>

<asp:Content ID="results_rx_Content" ContentPlaceHolderID="ResultsContent" runat="Server">
    <style type="text/css">
        a.sortHeader 
        {
            cursor: pointer;
        }
        div#ResultContent
        {
            max-height:500px;
            min-height:100px;
            overflow-y:auto;
            float:left;
            width:824px;
        }
        img.loadingSpinner
        {
            height:40px;
            width:40px;
            padding:5px;
            border:1px solid gray;
            position:absolute;  
            background-color:White;
            border-radius:5px;
            margin:10px 383.5px;
            box-shadow:1px 1px 3px rgb(0,0,0);
            z-index:1040;
        }
        table.searchresults
        {
            width:807px;
            max-height: 500px;
            overflow-y:auto;
        }
        .sortAsc
        {
            background-image: url("../Images/icon_arrow_down.gif");
            background-repeat: no-repeat;
            padding-left: 10px;
        }
        .sortDesc
        {
            background-image: url("../Images/icon_arrow_up.gif");
            background-repeat: no-repeat;
            padding-left: 10px;
        }
    </style>
    <%--<script type="text/javascript" src="../Scripts/Results_Rx.js">
    </script>--%>
    <%--<cch:Print ID="pResultsRx" runat="server" PrintDiv="cbotdemo" />--%>
    <script type="text/javascript">
        function showSpecialtyDrugDisclaimer() {
            element1 = document.getElementById("SpecialtyDrugDisclaimer");

            if (element1.style.display == 'none')
                element1.style.display = 'block';

            return;
        }  
    </script>
    <script type="text/javascript" language="javascript">var globDistance = <%= this.distance %>;</script>
    <h1>
        Search Results</h1>
    <p>
        <asp:Literal ID="ltlReturn" runat="server" Text="" />
    </p>
    <asp:Panel ID="pnlSingleDrug" runat="server" Visible="true">
        <p class="displayinline larger">
            Searched Drug:
            <b> 
                <asp:Label ID="lblSingleDrugName" runat="server" Text="" />
            </b>
            <asp:Literal ID="ltlSingleDrugDose" runat="server" Text="" />
            <%--&nbsp; &nbsp; &nbsp; Searched Dose:
            <b>
                <asp:Label ID="lblSingleDrugDose" runat="server" Text="" />
            </b>--%>
            &nbsp; &nbsp; &nbsp; Searched Quantity: 
            <b>
                <asp:Label ID="lblSingleDrugQuantity" runat="server" Text="" />
            </b>&nbsp;&nbsp;
            <p class="displayinline smaller">
                <span style="display:inline-block;">[ <b><asp:LinkButton ID="lbNew" runat="server" Text="New Search" PostBackUrl="search.aspx" /></b> ]</span></p>
            <br />
            <br />
            <%--<a class="readmore pointer smaller<%= IsHidden %>" id="addmed">Add this drug to Med List</a>--%>
        </p>
    </asp:Panel>
    <asp:Panel ID="pnlMultiDrug" runat="server" Visible="false">
        <div id="header_search_info">
            <h3 class="displayinline">
                Drugs Searched
            </h3>
            <p class="displayinline smaller">
                [ <asp:LinkButton ID="LinkButton1" runat="server" Text="New" /> ]</p>
            <br class="clearboth" />
        </div>
        <br />
        <asp:Repeater ID="rptMultiDrugTable" runat="server">
            <HeaderTemplate>
                <table class="searchquery" border="0" cellspacing="0" cellpadding="4">
                    <tbody>
                        <tr>
                            <th class="alignleft">DRUG</th>
                            <th>DOSE</th>
                            <th>QUANTITY</th>
                        </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="graydiv<%# IsTopRow(Container.ItemIndex) %>">
                    <td class="graydiv alignleft">
                        <asp:Label ID="lblMultiDrugName" runat="server" Text='<%# Eval("DrugName") %>' />
                    </td>
                    <td class="graydiv">
                        <asp:Label ID="lblMultiDrugDose" runat="server" Text='<%# Eval("DrugStrength") %>' />
                    </td>
                    <td class="graydiv">
                        <asp:Label ID="lblMultiDrugQuantity" runat="server" Text='<%# String.Format("{0} {1}", Eval("Quantity"), Eval("QuantityUOM")) %>' />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>
    <div id="meduserx" class="overlay">
        <div id="meduser" >
            <p>
                Select the family member who will use this drug
                <asp:DropDownList ID="ddlMembers" DataValueField="DVF" DataTextField="Name" runat="server" AutoPostBack="false"></asp:DropDownList>
                <br />
                <br />                
                <asp:LinkButton ID="lbSaveMedTo" runat="server" CssClass="submitlink" Text="Save" OnClientClick="cancelOverlay()" OnClick="SaveMed" /> &nbsp;&nbsp;&nbsp;<a
                    class="submitlink" onclick="cancelOverlay()">Cancel</a>
            </p>
        </div>
    </div>
    
    <cch:AlertBar runat="server" ID="abCurrentPrice" TypeOfAlert="Mini" SaveTotal="" Visible="true">
        <MessageTemplate>
            Current price at your pharmacy <%# Container.PharmacyName %> for this drug: <b><%# Container.SaveTotal %></b>
        </MessageTemplate>
    </cch:AlertBar>
    <hr />
    <cch:AlertBar runat="server" ID="abCouldSave" TypeOfAlert="Small" SaveTotal="" Visible="true">
        <MessageTemplate>
            <asp:Literal ID="ltlCouldSave" runat="server" Visible="<%# Container.CouldVisible %>" Text="You and your employer could save <b>" />
            <asp:Literal ID="lblSavings" runat="server" Visible="<%# Container.CouldVisible %>" Text="<%# Container.SaveTotal %>" />
            <asp:Literal ID="ltlEndBold" runat="server" Visible="<%# Container.CouldVisible %>" Text="</b>" />
            <asp:Label ID="lblMaxSave" runat="server" Text="You are currently maximizing your savings!" Visible="<%# Container.MaxVisible %>" /> 
        </MessageTemplate>
    </cch:AlertBar>
    <br class="clearboth" />
    <div class="buttonview viewshows" id="tableDiv" style="cursor:default;">
        <a class="table-map viewshows" id="showtableview" style="cursor:default;">Table View</a>
    </div>
    <div class="buttonview" id="mapDiv">
        <a class="table-map" id="showmapview">Map View</a>
    </div>
    <br class="clearboth" />
    <div id="tableview" class="showview">
    <div>
        <span class="displayinline smaller"><b>Sort by:</b>
            <span class="PHARM">
                <input type="radio" name="sort" value="Pharmacy" class="sortHeader" sortCol="PharmacyName" />
                Pharmacy
            </span>
            <span class="DIST">
                <input type="radio" name="sort" value="distance" class="sortHeader" sortCol="Distance" checked="checked" />
                Distance
            </span>
            <span class="PRICE">
                <input type="radio" name="sort" value="estimated cost" class="sortHeader" sortCol="TotalCost" />
                Total Estimated Cost
            </span>
            <span class="YC" style="display:none;">
                <input type="radio" name="sort" value="your cost" class="sortHeader" sortCol="YourCost" />
                Your Estimated Cost
            </span>
        </span>
    </div>
    <div class="toggle">
        <b style="vertical-align:middle;">Your Estimated Cost:&nbsp;</b>
        <img src="../Images/toggle_off.png" alt="" class="YC" style="cursor:pointer;vertical-align:middle;" />
        <img src="../Images/toggle_on.png" alt="" class="YC" style="cursor:pointer;vertical-align:middle;display:none;" />
    </div>
    <div class="slider">
        <asp:Label ID="Label1" AssociatedControlID="sFindRx" runat="server" Text="Distance range: " Visible="false" />
        <cch:Slider ID="sFindRx" runat="server" Min="1" Max="100" Value="25" Width="200px" OnSlideChanged="updateDistance" Visible="false"
            style="display:inline-block;vertical-align:middle;" />
        <asp:Label ID="lblSliderValue" runat="server" AssociatedControlID="sFindRx" Text=" 25 miles" Visible="false" />
    </div>
    <p>
        <asp:Label ID="lblRxResultDisclaimerText" runat="server" CssClass="smaller" Font-Italic="True"></asp:Label>
    </p>
    <p class="smaller">
        Click on a pharmacy for details.
    </p>
    <div id="pnlService" style="position:relative;">
        <div id="loader" style="width:823px; display:none;">
            <img src="../Images/CCHSpinLoader.gif" alt="" style="z-index: 1032; position:absolute; left: 376.5px; width: 70px; height: 70px; padding-top: 15px;" />
        </div>
        <table cellspacing='0' cellpadding='4' border='0' class='searchresults' style='width:824px;'>
            <tbody>
                <tr>
                    <td>
                        <div style="width:824px;">
                            <table cellspacing='0' cellpadding='4' border='0' class='searchresults' style='width:807px;'>                       
                                <tbody>
                                    <tr>
                                        <td class="tdfirst PHARM" valign="bottom" style='width:40%;'>
                                            <a class="sortHeader" sortCol="PharmacyName">Pharmacy</a>
                                        </td>
                                        <td valign="bottom" align="center" class="DIST" style='width:30%;'>
                                            <a class="sortHeader sortAsc" sortCol="Distance">Distance</a>
                                            <cch:LearnMore ID="LearnMore2" runat="server">
                                                <TitleTemplate>
                                                    Distance
                                                </TitleTemplate>
                                                <TextTemplate>
                                                    Estimated distance (miles) from patient location to this facility.
                                                </TextTemplate>
                                            </cch:LearnMore>
                                        </td>
                                        <td valign="bottom" align="center" class="PRICE" style='width:30%;'>
                                            <a class="sortHeader" sortCol="TotalCost">Total Estimated Cost</a>
                                            <cch:LearnMore ID="LearnMore3" runat="server">
                                                <TitleTemplate>
                                                    Total Estimated Cost
                                                </TitleTemplate>
                                                <TextTemplate>
                                                    Expected total price for this service or drug, based on recent payments made to this 
                                                    provider from your health plan.  How much of this total price is paid by you will vary 
                                                    based on your type of health plan and what other medical expenses you have had this year.
                                                    Because prices can change and the exact services and drug you receive can vary, it is 
                                                    possible that the total price will fall outside of the range presented here.
                                                </TextTemplate>
                                            </cch:LearnMore>
                                        </td>
                                        <td valign="bottom" align="center" style="width:0%;display:none;" class="YC">
                                            <a class="sortHeader" sortCol="YourCost">Your Estimated Cost</a>
                                            <cch:LearnMore ID="LearnMore1" runat="server">
                                                <TitleTemplate>
                                                    Your Estimated Cost
                                                </TitleTemplate>
                                                <TextTemplate>
                                                    Estimated out-of-pocket cost to you for this service, based on your health care spending 
                                                    since the start of the plan year.  Because of the timing involved in claims submission and 
                                                    processing, please be aware that we may not have all of your most recent health care 
                                                    services in our system.   Additionally, this is based on the cost you would pay to obtain 
                                                    this service, rather than other people on your plan.  As a result of these factors, it is 
                                                    possible that the price you pay out-of-pocket may fall outside of the range presented here.
                                                </TextTemplate>
                                            </cch:LearnMore>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </td>                    
                </tr>
                <tr>
                    <td>
                        <div id="ResultContent">
                            <img class="loadingSpinner" src="../Images/ajax-loader-AltCircle.gif" alt="" />
                            <table id='tblSearchResults' cellspacing='0' cellpadding='4' border='0' class='searchresults'>                       
                            </table>
                            <div id="fade" class="black_overlay"></div>
                        </div>                        
                    </td>
                </tr>
            </tbody>
        </table>        
    </div>
    </div>
    <div id="mapview" class="hideview">
        <p class="smaller">
            Click on a pharmacy for details.</p>
        <div style="position: relative;">
            <div id="resultmap" style="width: 840px; height: 500px; overflow: hidden; padding: 0px;
                margin: 0px; position: relative; background-color: rgb(229, 227, 223);">
            </div>
            <div id="legend">
                <p class="smaller" style="line-height: 35px;">
                    <span class="legendpin">Patient Location</span>
                    <span class="legendp">Pharmacy</span>
                </p>
            </div>
        </div>
    </div>
    <br />
    <br />
    <p id="SpecialtyDrugDisclaimer" style="display: none">
        <asp:Label ID="lblSpecialtyDrugDisclaimer" runat="server" CssClass="smaller" Font-Italic="True"></asp:Label>
    </p>
    <p>
        <!--%= RxDisclaimerForAvayaCaesars %-->
        <!--%= MailOrderDisclaimerForSanminaStarbucks %-->
        <i class="smaller">
            Note: Some hospital and clinic pharmacies may not fill prescriptions unless you receive care there. If you choose one of these pharmacies, please check with them to be sure
        </i>
    </p>
</asp:Content>
