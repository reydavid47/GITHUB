<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="doctor_specialty_detail.aspx.cs" Inherits="ClearCostWeb.SearchInfo.doctor_specialty_detail" %>
<%@ MasterType VirtualPath="~/SearchInfo/Results.master" %>
<%@ Register Src="~/Controls/LearnMore.ascx" TagPrefix="cch" TagName="LearnMore" %>

<asp:Content ID="doctor_specialty_detail_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <style type="text/css">
        .fairPrice p { display:inline; }
    </style>
    <script type="text/javascript">
        (function ($) {
            var MapCanvas, DirectionsPane,
                    directionsDisplay, directionsService,
                    map, startLL, endLL,
                    op = ((window.location.host.indexOf("localhost") > 0) ? "" : "client=gme-clearcosthealth&"),
                    methods = {
                        init: function (options) {
                            MapCanvas = this[0];
                            if (options) {
                                if (options.StartLatLng) { startLL = options.StartLatLng.split(','); }
                                if (options.EndLatLng) { endLL = options.EndLatLng.split(','); }
                                if (options.DirectionsPane) { DirectionsPane = options.DirectionsPane; }
                                SetupDirections();
                            }
                            else {
                                $.error("jQuery.DocDetails did not receive any Latitude and Longitude to initialize with");
                            }
                        }
                    },
                    SetupDirections = function () {
                        if (!google.maps) {
                            google.load("maps", "3.8",
                                {
                                    "other_params": op + "sensor=false",
                                    "callback": SetupDirections
                                }
                            );
                        }
                        else {
                            var startPoint = new google.maps.LatLng(startLL[0], startLL[1]),
                                endPoint = new google.maps.LatLng(endLL[0], endLL[1]),
                                mapOptions = {
                                    zoom: 7,
                                    mapTypeId: google.maps.MapTypeId.ROADMAP,
                                    center: endPoint
                                },
                                request = {
                                    origin: startPoint,
                                    destination: endPoint,
                                    travelMode: google.maps.DirectionsTravelMode.DRIVING
                                },
                                marker;
                            directionsService = new google.maps.DirectionsService();
                            directionsDisplay = new google.maps.DirectionsRenderer();
                            map = new google.maps.Map(map_canvas, mapOptions);
                            marker = new google.maps.Marker({
                                position: endPoint,
                                map: map,
                                title: "Facility"
                            });
                            marker.setZIndex(0);  //  lam, 20130402, MSF-298
                            directionsDisplay.setMap(map);
                            directionsDisplay.setPanel(DirectionsPane);
                            directionsService.route(request, function (response, status) {
                                if (status === google.maps.DirectionsStatus.OK)
                                    directionsDisplay.setDirections(response);
                            });
                        }
                    };
            $.fn.DocDetails = function (method) {
                if (methods[method]) {
                    return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
                } else if (typeof method === 'object' || !method) {
                    return methods.init.apply(this, arguments);
                } else {
                    $.error("Method " + method + " does not exist on jQuery.DocDetails");
                }
            }
        })(jQuery);
    </script>
    <%--<script type="text/javascript">
        function HandleOverviewClick() {
            if (map == null) {
                GenerateMapAndDirections();
            }
        }
    </script>--%>
    <%--<script type="text/javascript">
        var directionsDisplay;
        var directionsService;
        var map;
        var start;
        var end;
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
                directionsService = new google.maps.DirectionsService();
                var directionsDisplay = new google.maps.DirectionsRenderer();
                var query = window.location.search.substring(1);
                //var vars = query.split("&");
                //var start = results[0].geometry.location;
                var startText = tempStart.split(",");
                var endText = tempEnd.split(","); // //vars[1].split("=")[1].split(",");
                var start = new google.maps.LatLng(startText[0], startText[1]);
                var end = new google.maps.LatLng(endText[0], endText[1]);

                var myOptions = {
                    zoom: 7,
                    mapTypeId: google.maps.MapTypeId.ROADMAP,
                    center: end
                }
                map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);

                var marker = new google.maps.Marker({
                    position: end,
                    map: map,
                    title: "Facility"
                });

                directionsDisplay.setMap(map);
                directionsDisplay.setPanel(document.getElementById("directionsPanel"));
                var request = {
                    origin: start,
                    destination: end,
                    travelMode: google.maps.DirectionsTravelMode.DRIVING
                };
                directionsService.route(request, function (response, status) {
                    if (status == google.maps.DirectionsStatus.OK) {
                        directionsDisplay.setDirections(response);
                    }
                });
            }
        }
    </script>--%>
    <%--<script type="text/javascript">
        $(document).ready(function () {
            /* for the Directions icons */
            $('div.directions').click(function () {
                $('div.directiondetails:visible').fadeOut(500);
                $(this).find('div.directiondetails:hidden').fadeIn(500);
            });

            $('div.directiondetails').click(function () { $(this).fadeOut(500); });
            /* Added by Blue Note */

            /* for the View Larger map */
            $('div.viewlarger').click(function () {
                $('div.largermap:visible').fadeOut(500);
                $(this).find('div.largermap:hidden').fadeIn(500);
            });

            $('div.largermap').click(function () { $(this).fadeOut(500); });

            /* this is meant to prevent the outline around the images that you click on */
            $("img,a,map,area").mouseup(function () { $(this).blur(); });

            GenerateMapAndDirections(tempStart, tempEnd);
        });
    </script>--%>
    <asp:HiddenField ID="hfStartLL" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfEndLL" runat="server" ClientIDMode="Static" />
    <h1>
        Search Result Details</h1>
    <%--<div id="result_buttons">
        <div class="button">
            <a href="print.aspx" target="_blank" onclick="javascript:window.print(); return false;">Print Results</a>
        </div>
        <!--
        <div class="button">
            <a id="savesearch" class="pointer">Save this Search</a>
        </div>
        <div id="savesearchform">
            <h3>
                Save this Search</h3>
            <%--<form name="savesearch">--%>
            <%--<div id="savesearch">
                <input type="text" name="searchname" onclick="this.value='';" onfocus="this.select()"
                    onblur="this.value=!this.value?'Name this search (e.g. Knee MRI)':this.value;"
                    value="Name this search (e.g. Knee MRI)" style="width: 230px;" />
                <br />
                <br />
                <a class="submitlink" onclick="document.savesearch.submit();return false;">Save</a>
                &nbsp;&nbsp;&nbsp;<a class="submitlink" id="cancelsavesearch">Cancel</a>
            </div>--%>
            <%--</form>--%>
        <%--</div>
        -->
    </div>--%>
    <!-- end result buttons -->
    <p>
        <%--<a class="back" href="results_specialty.aspx#tabdoc">Return to search results</a>--%>
        <%  //  lam, 20130313, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"  %>
        <%--<a href="results_specialty.aspx#tabdoc" style="cursor:pointer;" class="back">Return to search results</a>--%>
        <a href="results_specialty.aspx#<%=ReturnTab%>" style="cursor:pointer;" class="back">Return to search results</a>
    </p>
    <p class="displayinline larger">                            
        Searched Specialty:
        <asp:Label ID="lblSpecialty" runat="server" Text="" Font-Bold="true"></asp:Label></p>
    <div class="learnmore">
        <a title="Learn more">
            <asp:Image ID="imgSpcLearnMore" runat="server" ImageUrl="../Images/icon_question_mark.png"
                Width="12" Height="13" border="0" alt="Learn More" />
        </a>
        <div class="moreinfo">
            <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                style="cursor: pointer;" />
            <p>
                <b class="upper">
                    <asp:Label ID="lblSpecialty_MoreInfoTitle" runat="server" Text="" Font-Bold="true"></asp:Label></b><br />
                <asp:Label ID="lblSpecialtyMoreInfo" runat="server" Text=""></asp:Label></p>
        </div>
        <!-- end moreinfo -->
    </div>
    <!-- end learnmore -->
    <p class="displayinline smaller">
        <%  //  lam, 20130313, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"  %>
        <%--<a href="search.aspx#tabdoc">--%>
        &nbsp;&nbsp;&nbsp;<span style="display:inline-block;">[ <b><a href="search.aspx#<%=ReturnTab%>">New Search</a></b> ]</span></p>
    <br class="clearboth" />
    <br class="clearboth" />
    <div class="h2bg">
        <div class="floatright">
            <table cellspacing="0" cellpadding="4" border="0">
                <tr>
                    <td align="center">
                        <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/check_green.png" Width="23" Height="23"
                            border="" runat="server" AlternateText="Yes" />
                        <asp:Image ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23" border=""
                            runat="server" AlternateText="No" />
                    </td>
                    <td valign="middle">
                        <asp:Label ID="lblFairPriceTitle" runat="server" Text="Fair Price"></asp:Label>                                                
                        <div class="learnmore">
                            <a title="Learn more">
                                <asp:Image ID="imgFPLearnMore" runat="server" ImageUrl="../Images/icon_question_mark.png"
                                    Width="12" Height="13" border="0" alt="Learn More" />
                                    </a>
                            <div class="moreinfo">
                                <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                                    style="cursor: pointer;" />
                                <p>
                                    <b class="upper">Fair Price</b><br />
                                    Checked if a physician's practice has Fair Prices, based on algorithms developed by ClearCost Health.  
                                    The designation of a Fair Price Practice is based not only on what is done directly by the doctors at that practice, 
                                    but also in terms of where those doctors send patients for lab tests and imaging studies.
                            </div>
                            <!-- end moreinfo -->
                        </div>
                        <!-- end learnmore -->
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Image ID="imgHGRecognizedTrue" ImageUrl="../Images/check_purple.png" Width="23"
                            Height="23" border="" AlternateText="Health Grades Recognized" runat="server" />
                        <asp:Image ID="imgHGRecognizedFalse" ImageUrl="../Images/s.gif" Width="23" Height="23"
                            border="" runat="server" AlternateText="No" />
                    </td>
                    <td valign="middle">                                            
                        <asp:Label ID="lblHGRecTitle" runat="server" Text="Healthgrades Recognized Physician"></asp:Label>  
                        <div class="learnmore">
                            <a title="Learn more">
                                <asp:Image ID="imgHGLearnMore" runat="server" ImageUrl="../Images/icon_question_mark.png"
                                    Width="12" Height="13" border="0" alt="Learn More" />
                                </a>
                            <div class="moreinfo">
                                <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                                    style="cursor: pointer;" />
                                <p>
                                    <b class="upper">
                                        Healthgrades&trade;
                                    </b><br />
                                    Healthgrades Recognized ratings are assigned to physicians who: <br />
                                    1.are board-certified in their specialty of practice <br />
                                    2.have never had their medical license revoked <br />
                                    3.are free of state/federal disciplinary sanctions in the last five years <br />
                                    4.are free of any malpractice claims
                                </p>
                            </div>
                            <!-- end moreinfo -->
                        </div>
                        <!-- end learnmore -->
                    </td>
                </tr>
            </table>
        </div>
        <!-- end floatright -->
        <asp:Label ID="lblProviderName" runat="server" Text="" CssClass="h2bgStyle"></asp:Label><br />
        <asp:Label ID="lblPracticeName" runat="server" Text="" CssClass="h2bgStyle" ForeColor="Black" style="background-color:transparent;"></asp:Label>
    </div>
    <!-- end h2bg -->
    <!-- start collapsible table -->
    <table width="100%" cellspacing="0" cellpadding="0" border="0" id="doctorfacilitytable">
        <tr class="h3 category rowopen" id="overview" onclick="">
            <td>
                Overview
            </td>
        </tr>
        <tr class="careinstance" style="display: table-row;">
            <%--careinstance--%>
            <td>
                <div id="searchdetails">
                    <div id="detailinfo">
                        <table id="detailtable" cellspacing="0" cellpadding="4" border="0">
                            <tr>
                                <td>
                                    <%
                                    //  lam, 20130304, MSF-10, make this text conditional
                                    //  Estimated Cost of a New Patient Office Visit:
                                    %>
                                    Estimated Cost of <asp:Label ID="lblServiceName" runat="server" Text=""></asp:Label>:
                                </td>
                                <td>
                                     <% if (IsAntiTransparency)  //  lam, 20130308, MSF-141 antitransparency
                                        { %>
                                    Undisclosed
                                    <div class="learnmore">
                                        <a title="Learn more">
                                            <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                                        <div class="moreinfo">
                                            <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                                                style="cursor: pointer;" />
                                            <p>
                                                <b class="upper">ANTI-TRANSPARENCY PROVIDERS</b><br />
                                                This provider has chosen not to show prices to patients.
                                                Please note that in some instances, refusal to show this information
                                                can be an indication of high prices.
                                            </p>
                                        </div>
                                        <!-- end moreinfo -->
                                    </div>
                                    <% }
                                        else
                                        { %>
                                    $<asp:Label ID="lblRangeMin" runat="server" Text=""></asp:Label>
                                    - $<asp:Label ID="lblRangeMax" runat="server" Text=""></asp:Label>
                                    <div class="learnmore">
                                        <a title="Learn more">
                                            <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                                        <div class="moreinfo">
                                            <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                                                style="cursor: pointer;" />
                                            <p>
                                                <b class="upper">Total Estimated Cost</b><br />
                                                Expected total price for this service or drug, based on recent payments made to this 
                                                provider from your health plan.  How much of this total price is paid by you will vary 
                                                based on your type of health plan and what other medical expenses you have had this year.
                                                Because prices can change and the exact services and drug you receive can vary, it is 
                                                possible that the total price will fall outside of the range presented here.
                                            </p>
                                        </div>
                                        <!-- end moreinfo -->
                                    </div>
                                    <!-- end learnmore -->
                                    <% } %>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Primary Office Address:
                                </td>
                                <td>
                                    <asp:Label ID="lblAddressLine1" runat="server" Text=""></asp:Label>
                                    <br />
                                    <asp:Label ID="lblAddressLine2" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <%-- <tr>
                                <td>
                                    Alternate Office Address:
                                </td>
                                <td>
                                    224 Sansome St.<br />
                                    Naperville, IL 60564
                                </td>
                            </tr>--%>
                            <tr>
                                <td>
                                    <asp:Label ID="lblPhoneTitle" runat="server" Text="Phone:" Visible="false"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblFaxTitle" runat="server" Text="Fax:" Visible="false"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblFax" runat="server" Text="" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblEmailTitle" runat="server" Text="Email:" Visible="false"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblEmail" runat="server" Text="" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblWebsiteTitle" runat="server" Text="Website:" Visible="false"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblWebsite" runat="server" Text="" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Primary Specialty:
                                </td>
                                <td>
                                    <asp:Label ID="lblPrimarySpecialty" runat="server" Text=""></asp:Label>
                                    - Board Certified
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="detailmap">
                        <%if (!IsTeleMedicine)
                          { %>
                        <div id="map_canvas" style="height: 350px; width: 425px">
                        </div>
                        <a href="LargerMap.aspx" style="color: #0000FF; text-align: left; display:block;">View Larger Map</a>
                        <%--<p>
                            <asp:Label ID="lblDistance" runat="server" Text=""></asp:Label>&nbsp;miles from
                            your location
                        </p>--%>
                        <div class="directions">
                            <a title="Get Directions">Get Directions </a>
                            <div class="directiondetails" id="directionsPanel" style="width: 390px;">
                                <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                                    style="cursor: pointer;" /><br />
                            </div>
                            <!-- end direction details -->
                        </div>
                        <!-- end directions -->
                        <%} %>
                    </div>
                    <div class="clearboth">
                    </div>
                </div>
            </td>
        </tr>
        <tr class="h3 category rowopen hgEdu" id="education">
            <td>
                Education
            </td>
        </tr>
        <tr class="careinstance  hgEdu" style="display: table-row;">
            <%--careinstance--%>
            <td>
                <asp:Repeater ID="rptEducation" runat="server" OnItemDataBound="rptEducation_ItemDataBound">
                    <HeaderTemplate>
                        <tr class="careinstance" style="display:table-row;">
                            <td>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <p>
                            <b>
                                <%# Eval("EducationTitle")%>:</b><br />
                            <asp:Label ID="lblEducationInfo" runat="server" Text=""></asp:Label>
                        </p>
                    </ItemTemplate>
                    <FooterTemplate>
                        </td></tr>
                    </FooterTemplate>
                </asp:Repeater>
            </td>
        </tr>
        <%-- rld, SCIQ-189, 20131115, removed Fair Price table --%>
        <%if (!IsTeleMedicine)
          { %>
        <tr class="h3 category rowopen" id="rating">
            <td>
                Healthgrades&trade; Patient Satisfaction
            </td>
        </tr>
        <tr class="careinstance" style="display: table-row;">
            <td>
                <table cellspacing="0" cellpadding="4" border="0" width="100%">
                    <tr class="roweven">
                        <td>
                            <b>OVERALL RATING</b><br />
                            <asp:Label ID="lblPatientCount" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="center">
                            <table cellspacing="0" cellpadding="4" border="0" style="border-style: none">
                                <tr style="border-style: none">
                                    <td style="border-style: none">
                                        <asp:Label ID="lblStar1" runat="server" Text="" CssClass="star_none" Width="16px"></asp:Label>
                                    </td>
                                    <td style="border-style: none">
                                        <asp:Label ID="lblStar2" runat="server" Text="" CssClass="star_none" Width="16px"></asp:Label>
                                    </td>
                                    <td style="border-style: none">
                                        <asp:Label ID="lblStar3" runat="server" Text="" CssClass="star_none" Width="16px"></asp:Label>
                                    </td>
                                    <td style="border-style: none">
                                        <asp:Label ID="lblStar4" runat="server" Text="" CssClass="star_none" Width="16px"></asp:Label>
                                    </td>
                                    <td style="border-style: none">
                                        <asp:Label ID="lblStar5" runat="server" Text="" CssClass="star_none" Width="16px"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <asp:Repeater ID="rptRatings" runat="server" OnItemDataBound="rptRatings_ItemDataBound">
                        <HeaderTemplate>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <%--IF first row (overall) class="roweven"--%>
                                <%--Left off here!!  Need to get Overall rating and patient count and add it to rows when gathering data.
                    Then need to special handle background color on this first row.--%>
                                <td>
                                    <b>
                                        <%# Eval("SurveyTitle")%></b><br />
                                    <%# Eval("SurveyDescription")%>
                                </td>
                                <td align="center">
                                    <table cellspacing="0" cellpadding="4" border="0" style="border-style: none">
                                        <tr style="border-style: none">
                                            <td style="border-style: none">
                                                <asp:Label ID="lblStar1" runat="server" Text="" CssClass="star_none" Width="16px"></asp:Label>
                                            </td>
                                            <td style="border-style: none">
                                                <asp:Label ID="lblStar2" runat="server" Text="" CssClass="star_none" Width="16px"></asp:Label>
                                            </td>
                                            <td style="border-style: none">
                                                <asp:Label ID="lblStar3" runat="server" Text="" CssClass="star_none" Width="16px"></asp:Label>
                                            </td>
                                            <td style="border-style: none">
                                                <asp:Label ID="lblStar4" runat="server" Text="" CssClass="star_none" Width="16px"></asp:Label>
                                            </td>
                                            <td style="border-style: none">
                                                <asp:Label ID="lblStar5" runat="server" Text="" CssClass="star_none" Width="16px"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                        </FooterTemplate>
                    </asp:Repeater>
                </table>
            </td>
        </tr>
        <%} %>
    </table>
    <!-- end doctorfacilitytable -->
    <script type="text/javascript">
        $(".directions").click(function () {
            $(".directiondetails:visible").fadeOut(500);
            $(this).find(".directiondetails:hidden").fadeIn(500);
        });
        $(".directiondetails").click(function () { $(this).fadeOut(500); });
        $(".viewlarger").click(function () {
            $(".largermap:visible").fadeOut(500);
            $(this).find(".largermap:hidden").fadeIn(500);
        });
        $(".largermap").click(function () { $(this).fadeOut(500); });
        $("img,a,map,area").mouseup(function () { $(this).blur(); });
        $("#map_canvas").DocDetails({
            StartLatLng: document.getElementById("hfStartLL").value,
            EndLatLng: document.getElementById("hfEndLL").value,
            DirectionsPane: document.getElementById("directionsPanel")
        });
    </script>
</asp:Content>

