<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true"
    CodeFile="results_past_care.aspx.cs" Inherits="ClearCostWeb.SearchInfo.results_past_care" %>

<%@ Register Src="~/Controls/Print.ascx" TagPrefix="cch" TagName="Print" %>
<%@ Register Src="~/Controls/LearnMore.ascx" TagPrefix="cch" TagName="LearnMore" %>
<%@ Register Src="~/Controls/Slider.ascx" TagPrefix="cch" TagName="Slider" %>
<asp:Content ID="results_past_care_Content" ContentPlaceHolderID="ResultsContent"
    runat="Server">
    <script type = "text/javascript">
        //  lam, 20130321, MSF-274, new script segment
        function changeSelection() {
            var rblClientSort = document.getElementById('<%=rblSort.ClientID%>');
            var radio = rblClientSort.getElementsByTagName('input');
            if (radio[3].checked) {
                radio[2].checked = true;
                radio[2].click();
            }
        }
    </script>    
    <script type="text/javascript">
//        var showingYC = false;
        function rigJava() {
            $('a.table-map').click(function () {
                if ($(this).attr('id') == 'showmapview') {

                    $('#tableview').removeClass('showview');
                    $('#tableview').addClass('hideview');

                    $('#showtableview').removeClass('viewshows');
                    $('#showmapview').addClass('viewshows');

                    $('#showtableview').closest('div.buttonview').removeClass('viewshows');
                    $('#showmapview').closest('div.buttonview').addClass('viewshows');

                    $('#mapview').removeClass('hideview');
                    $('#mapview').addClass('showview');

                    if (firstmapview) makeMap();
                    firstmapview = false;
                }
                if ($(this).attr('id') == 'showtableview') {
                    $('#mapview').removeClass('showview');
                    $('#mapview').addClass('hideview');

                    $('#showmapview').removeClass('viewshows');
                    $('#showtableview').addClass('viewshows');

                    $('#showtableview').closest('div.buttonview').addClass('viewshows');
                    $('#showmapview').closest('div.buttonview').removeClass('viewshows');

                    $('#tableview').removeClass('hideview');
                    $('#tableview').addClass('showview');
                }
            });
            $("div.toggle img.YC").click(function () {
                $(".YC").toggle();
                $("td.PHARM").toggleClass("expanded");
                $("td.DIST").toggleClass("expanded");
                $("td.PRICE").toggleClass("expanded");
                showingYC = !showingYC;
            });
        }
    </script>
    <script type="text/javascript">
        var firstmapview = true;
        var map;
        var script;
        var infoWindow;
        function makeMap() {
            GenerateMapAndDirections();
        }
        function GenerateMapAndDirections() {
            if (!google.maps) {
                google.load("maps", "3.8",
                    {
                        "other_params": ((window.location.host.indexOf("localhost") > 0) ? "" : "client=gme-clearcosthealth&") + "sensor=false",
                        "callback": GenerateMapAndDirections
                    }
                );
            }
            else {
                PageMethods.GetPatientCenter(function (result) {
                    eval("var rawString = " + result);
                    var rawArray = rawString.split(',');
                    eval("var patLat = " + rawArray[0]);
                    eval("var patLng = " + rawArray[1]);
                    var patCenter = new google.maps.LatLng(patLat, patLng);

                    var myOptions = {
                        zoom: 10,
                        center: patCenter,
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    }

                    var mapCanvas = document.getElementById("resultmap");
                    map = new google.maps.Map(mapCanvas, myOptions);

                    var centerPin = new google.maps.Marker({
                        position: patCenter,
                        map: map,
                        icon: '../Images/icon_map_pin.png'
                    });

                    infoWindow = new google.maps.InfoWindow({
                        size: new google.maps.Size(100, 50)
                    });

                    google.maps.event.addListener(map, 'click', function () {
                        infoWindow.close();
                    });

                    var icon_FP = '../Images/icon_map_green.png';
                    var icon = '../Images/icon_map_blue.png';

                    PageMethods.GetMarkerCount(function (gmcResult) {
                        eval("var numOfMarkers = " + gmcResult + ";");
                        for (var j = 0; j < numOfMarkers; j++) {
                            PageMethods.GetMarker(j, function (gmResult) {
                                eval(gmResult);
                            });
                        }
                    });
                });
            }
        }

        var gmarkers = [];
        var i = 0;
        function createMarker(myIcon, myMap, myBubble, latlng, popHtml, linkIndex) {
            var javaLink = "javascript:__doPostBack('ctl00$ResultsContent$rptResults$ctl" + linkIndex + "$lbtnSelectFacility','')";
            var contentString = popHtml.replace("#", javaLink);


            var marker = new google.maps.Marker({
                position: latlng,
                map: myMap,
                icon: myIcon//,
                //zIndex: Math.round(latlng.lat() * -100000) << 5
            });
            var linkid = "link" + i;
            google.maps.event.addListener(marker, 'click', function () {
                myBubble.setContent(contentString);
                myBubble.open(myMap, marker);
            });

            // save the info we need to use later for the sidebar
            gmarkers.push(marker);
            i++;
        }
    </script>
    <%--<cch:Print ID="pResultsPastCare" runat="server" PrintDiv="upPastCare" />--%>
    <h1>
        Search Results</h1>
    <asp:UpdatePanel ID="upPastCare" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <a class="back" href="search.aspx">Return to Past Care</a>
            <br />
            <p class="displayinline larger">
                Searched Service:<b>
                    <asp:Label ID="lblServiceName" runat="server" Text=""></asp:Label></b>
            </p>
            <div class="learnmore">
                <a title="Learn more">
                    <asp:Image ID="imgSvcLearnMore" runat="server" ImageUrl="../Images/icon_question_mark.png"
                        Width="12" Height="13" border="0" alt="Learn More" />
                    <div class="moreinfo">
                        <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                            style="cursor: pointer;" />
                        <p>
                            <b class="upper">
                                <asp:Label ID="lblServiceName_MoreInfoTitle" runat="server" Text="" Font-Bold="true"></asp:Label></b><br />
                            <asp:Label ID="lblServiceMoreInfo" runat="server" Text=""></asp:Label></p>
                    </div>
                    <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
            <p class="displayinline smaller">
                &nbsp;&nbsp;<span style="display: inline-block;">[ <b><a href="search.aspx">New Search</a></b>
                    ]</span>
            </p>
            <br class="clearboth" />
            <asp:Label ID="lblServiceVerification" runat="server" Text="" Visible="false" CssClass="smaller"
                Style="padding-left: 135px"></asp:Label>
            <p class="displayinline larger">
                <asp:Label ID="lblFacilityTitle" runat="server" Text="You went to: "></asp:Label>
                <b>
                    <asp:Label ID="lblFacilityName" runat="server" CssClass="smaller" Text=""></asp:Label>
                </b>
            </p>
            <hr />
            <asp:Panel ID="pnlPastCareDetails" runat="server" Visible="false">
                <div class="smaller alertsmall">
                    <table cellspacing="0" cellpadding="0" border="0" width="500">
                        <tr>
                            <td>
                                Total price paid at
                                <asp:Label ID="lblFacilityNameDetails" runat="server" Text="Label"></asp:Label>:
                            </td>
                            <td align="right">
                                <asp:Label ID="lblAllowedAmount" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Best price:
                            </td>
                            <td align="right" style="border-bottom: 1px black solid; white-space: nowrap;">- &nbsp;&nbsp;&nbsp;<asp:Label ID="lblDifference" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Potential savings to you and your employer
                            </td>
                            <td align="right">
                                <asp:Label ID="lblYouCouldHaveSaved" runat="server" Text="" />
                            </td>
                        </tr>
                    </table>
                </div>
                <br class="clearboth" />
                <br />
            </asp:Panel>
            <div class="buttonview viewshows" id="tableDiv" style="cursor: default;">
                <a class="table-map viewshows" id="showtableview" style="cursor: default;">Table View</a>
            </div>
            <div class="buttonview" id="mapDiv">
                <a class="table-map" id="showmapview">Map View</a>
            </div>
            <br class="clearboth" />
            <div id="tableview" class="showview">
                <div class="displayinline smaller">
                    <table>
                        <tr>
                            <td style="vertical-align: middle;">
                                <b>Sort by:</b>
                            </td>
                            <td style="vertical-align: middle;">
                                <asp:RadioButtonList ID="rblSort" runat="server" RepeatDirection="Horizontal" AutoPostBack="True"
                                    OnSelectedIndexChanged="rblSort_SelectedIndexChanged">
                                    <asp:ListItem Text="Facility" Value="PracticeName"></asp:ListItem>
                                    <asp:ListItem Text="Distance" Value="NumericDistance"></asp:ListItem>
                                    <asp:ListItem Text="Total Estimated Cost" Value="AllowedAmount" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Your Estimated Cost" Value="YourCost" class="YC" style="display:none;"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="toggle">
                    <b style="vertical-align:middle;">Your Estimated Cost:&nbsp;</b>
                    <img src="../Images/toggle_off.png" alt="" class="YC" onclick="toggleYC(true)" style="cursor:pointer;vertical-align:middle;" />
                    <img src="../Images/toggle_on.png" alt="" class="YC" onclick="toggleYC(false)" style="cursor:pointer;vertical-align:middle;display:none;" />
                </div>
                <asp:HiddenField ID="hfdShowYC" Value="" runat="server" />
                <div class="slider">
                    <asp:Label AssociatedControlID="sFindPastCare" runat="server" Text="Distance range: " />
                    <cch:Slider ID="sFindPastCare" runat="server" Min="1" Max="100" Value="25" Width="200px" OnSlideChanged="updateDistance" Visible="false"
                        style="display:inline-block;vertical-align:middle;" />
                    <asp:Label ID="lblSliderValue" runat="server" AssociatedControlID="sFindPastCare" Text=" 25 miles" />
                </div>
                <asp:HiddenField ID="hfdSortDirection" runat="server" /><asp:HiddenField ID="hfdCurrentSort" runat="server" />
                <asp:Repeater ID="rptResults" runat="server" OnItemDataBound="rptResults_ItemDataBound">
                    <HeaderTemplate>
                        <table id="SearchResultsTbl" cellspacing="0" cellpadding="4" border="0" class="searchresults"
                            style="width: 100%">
                            <th class="tdfirst" valign="bottom" style="width: 30%">
                                <asp:Image ID="imgFacilitySort" runat="server" ImageUrl="../Images/icon_arrow_down.gif" 
                                    border="0" alt="" Width="7" Height="6" Visible="false" />
                                <asp:LinkButton ID="lbtnFacilitySort" runat="server" OnClick="FacilitySort">Name of Doctor</asp:LinkButton>
                            </th>
                            <th valign="bottom" style="width: 10%">
                                Location
                            </th>
                            <th valign="bottom" style="width: 16%" style="white-space: nowrap">
                                <asp:Image ID="imgDistanceSort" runat="server" ImageUrl="../Images/icon_arrow_down.gif" 
                                    border="0" alt="" Width="7" Height="6" Visible="true" />
                                <asp:LinkButton ID="lbtnDistanceSort" runat="server" OnClick="DistanceSort">Distance</asp:LinkButton>
                                <div class="learnmore">
                                    <a title="Learn more">
                                        <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                                    <div class="moreinfo">
                                        <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                                            style="cursor: pointer;" />
                                        <p style="white-space: normal">
                                            <b class="upper">Distance</b><br />
                                            Estimated distance (miles) from patient location to this facility</p>
                                    </div>
                                    <!-- end moreinfo -->
                                </div>
                                <!-- end learnmore -->
                                </a>
                            </th>
                            <th valign="bottom" style="width: 16%">
                                <asp:Image ID="imgEstimatedCostSort" runat="server" ImageUrl="../Images/icon_arrow_down.gif" 
                                    border="0" alt="" Width="7" Height="6" Visible="false" />
                                <asp:LinkButton ID="lbtnEstimatedCostSort" runat="server" OnClick="EstimatedCostSort">Total Estimated Cost</asp:LinkButton>
                                <cch:LearnMore runat="server">
                                    <TitleTemplate>
                                        Total Estimated Cost
                                    </TitleTemplate>
                                    <TextTemplate>
                                        Expected total price for this service or drug, based on recent payments made to
                                        this provider from your health plan. How much of this total price is paid by you
                                        will vary based on your type of health plan and what other medical expenses you
                                        have had this year. Because prices can change and the exact services and drug you
                                        receive can vary, it is possible that the total price will fall outside of the range
                                        presented here.
                                    </TextTemplate>
                                </cch:LearnMore>
                                </a>
                            </th>
                            <th valign="bottom" style="width:0%;display:none;" class="YC">
                                <asp:Image ID="imgYourCostSort" runat="server" ImageUrl="../Images/icon_arrow_down.gif" 
                                    border="0" AlternateText="" Width="7" Height="6" Visible="false" />
                                <asp:LinkButton ID="lbtnYourCostSort" runat="server" OnClick="YourCostSort">Your Estimated Cost</asp:LinkButton>
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
                            </th>
                            <th valign="bottom" style="width: 10%">
                                Fair Price
                                <cch:LearnMore ID="lmFP" runat="server">
                                    <TitleTemplate>
                                        Fair Price
                                    </TitleTemplate>
                                    <TextTemplate>
                                        Checked if a physician's practice has Fair Prices, based on algorithms developed
                                        by ClearCost Health. The designation of a Fair Price Practice is based not only
                                        on what is done directly by the doctors at that practice, but also in terms of where
                                        those doctors send patients for lab tests and imaging studies.
                                    </TextTemplate>
                                </cch:LearnMore>
                            </th>
                            <th valign="bottom" style="width: 18%">
                                Healthgrades&trade;
                                <br />
                                Recognized Physician</span>
                                <cch:LearnMore runat="server">
                                    <TitleTemplate>
                                        Healthgrades&trade;
                                    </TitleTemplate>
                                    <TextTemplate>
                                        Healthgrades Recognized ratings are assigned to physicians who: <br />
                                        1.are board-certified in their specialty of practice <br />
                                        2.have never had their medical license revoked <br />
                                        3.are free of state/federal disciplinary sanctions in the last five years <br />
                                        4.are free of any malpractice claims
                                    </TextTemplate>
                                </cch:LearnMore>
                                </a>
                            </th>
                            <th style="display: none">
                            </th>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class='<%# GetItemClass(Container.ItemIndex, Eval("PracticeName") ) %>'>
                            <td class="tdfirst graydiv">
                                <asp:LinkButton ID="lbtnSelectFacility" runat="server" CssClass="readmore" Text='<%# Eval("PracticeName") %>'
                                    OnClick="SelectFacility"></asp:LinkButton>
                            </td>
                            <td class="graydiv">
                                <%# Eval("LocationCity") %>
                            </td>
                            <td class="graydiv distancealignright">
                                <div id='<%# Eval("Distance") %>'>
                                    <%# Eval("Distance") %></div>
                            </td>
                            <td class="distancealigncenter graydiv" style="padding-right: 2em; padding-left: 2em; white-space: nowrap;">
                                <asp:Label ID="lblAllowedAmount" runat="server" Text=""></asp:Label>
                                <asp:Image ID="imgLearnMore" ImageUrl="../Images/icon_question_mark.png" border="" AlternateText="Learn More" ToolTip="Learn More" runat="server" />
                            </td>
                            <td class="distancealignright graydiv YC" style="display:none;">
                                <asp:Label ID="lblYourCost" runat="server" Text=""></asp:Label>
                            </td>
                            <td class="tdcheck graydiv">
                                <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/check_green.png" Width="23"
                                    Height="23" border="" AlternateText="FairPrice?" runat="server" Visible="false" /><asp:Image
                                        ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23" border=""
                                        AlternateText="FairPrice?" runat="server" Visible="false" />
                            </td>
                            <td class="tdcheck graydiv">
                                <asp:Image ID="imgHGRecognizedTrue" ImageUrl="../Images/check_purple.png" Width="23"
                                    Height="23" border="" AlternateText="Health Grades Recognized?" runat="server"
                                    Visible="false" />
                                <asp:Image ID="imgHGRecognizedFalse" ImageUrl="../Images/s.gif" Width="23" Height="23"
                                    border="" AlternateText="Health Grades Recognized?" runat="server" Visible="false" />
                                <asp:Label ID="lblHGCount" runat="server" Text="" />
                            </td>
                            <td style="display: none">
                                <%# Eval("LatLong") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class='<%# GetItemClass(Container.ItemIndex, Eval("PracticeName") ) %>'>
                            <td class="tdfirst graydiv">
                                <asp:LinkButton ID="lbtnSelectFacility" runat="server" CssClass="readmore" Text='<%# Eval("Practicename") %>'
                                    OnClick="SelectFacility"></asp:LinkButton>
                            </td>
                            <td class="graydiv">
                                <%# Eval("LocationCity") %>
                            </td>
                            <td class="graydiv distancealignright">
                                <div id='<%# Eval("Distance") %>'>
                                    <%# Eval("Distance") %></div>
                            </td>
                            <td class="distancealigncenter graydiv" style="padding-right: 2em; padding-left: 2em; white-space: nowrap;">
                                <asp:Label ID="lblAllowedAmount" runat="server" Text=""></asp:Label>
                                <asp:Image ID="imgLearnMore" ImageUrl="../Images/icon_question_mark.png" border="" AlternateText="Learn More" ToolTip="Learn More" runat="server" />
                            </td>
                            <td class="distancealignright graydiv YC" style="display:none;">
                                <asp:Label ID="lblYourCost" runat="server" Text=""></asp:Label>
                            </td>
                            <td class="tdcheck graydiv">
                                <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/check_green.png" Width="23"
                                    Height="23" border="" AlternateText="FairPrice?" runat="server" /><asp:Image ID="imgFairPriceFalse"
                                        ImageUrl="../Images/s.gif" Width="23" Height="23" border="" AlternateText="FairPrice?"
                                        runat="server" />
                            </td>
                            <td class="tdcheck graydiv">
                                <asp:Image ID="imgHGRecognizedTrue" ImageUrl="../Images/check_purple.png" Width="23"
                                    Height="23" border="" AlternateText="Health Grades Recognized?" runat="server" />
                                <asp:Image ID="imgHGRecognizedFalse" ImageUrl="../Images/s.gif" Width="23" Height="23"
                                    border="" AlternateText="Health Grades Recognized?" runat="server" />
                                <asp:Label ID="lblHGCount" runat="server" Text="" />
                            </td>
                            <td style="display: none">
                                <%# Eval("LatLong") %>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        </table></FooterTemplate>
                </asp:Repeater>
            </div>
            <div id="mapview" class="hideview">
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
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
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
        <i class="smaller">Note:  Past Care includes only routine lab tests, imaging tests, office 
        visits, basic outpatient procedures, and prescription drugs.  It does not include all 
        services that you may have had in the last twelve months.  It also does not includes many 
        sensitive medical services and drug prescriptions, such as those related to reproductive
        health, mental health, HIV/AIDS, and other sexually-trasmitted diseases.</i>
    </p>
    <p>
        <asp:Label ID="lblAllResult1DisclaimerText" CssClass="smaller" runat="server" Font-Italic="True"></asp:Label>
    </p>
</asp:Content>
