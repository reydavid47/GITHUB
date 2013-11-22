﻿<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="sc_results_care_detail.aspx.cs" Inherits="ClearCostWeb.SavingsChoice.sc_results_care_detail" %>
<%@ Register TagName="starRating" TagPrefix="cchUIControl" Src="~/Controls/StarRating.ascx" %>

<asp:Content ID="sc_results_care_detail_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <script type="text/javascript">
        var map;
        var directionsDisplay;
        var directionsService;
        function GenerateMapAndDirections() {
            if (!google.maps) {
                google.load("maps", "3.8", {
                    "other_params": ((window.location.host.indexOf("localhost") > 0) ? "" : "client=gme-clearcosthealth&") + "sensor=false",
                    "callback": GenerateMapAndDirections
                });
            }
            else {
                directionsService = new google.maps.DirectionsService();
                var directionsDisplay = new google.maps.DirectionsRenderer();
                var patientLat = $("#PATIENTLATITUDE").val(); //$(".patientAddress").attr("Lat");
                var patientLng = $("#PATIENTLONGITUDE").val();// $(".patientAddress").attr("Lng");
                var pracLat = $("#<%= lblPracticeName.ClientID %>").attr("pLat");
                var pracLng = $("#<%= lblPracticeName.ClientID %>").attr("pLng");
                var start = new google.maps.LatLng(patientLat, patientLng);
                var end = new google.maps.LatLng(pracLat, pracLng);

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
    </script>
    <script type="text/javascript">
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

            GenerateMapAndDirections();
        });
    </script>
    <%--<div id="result_buttons">
        <table>
            <tr valign="top">
                <td>
                    <asp:ImageButton ID="ibtnPrintResults" runat="server" ImageUrl="../Images/PrintResults.png" OnClientClick="javascript:window.print(); return false;" />
                </td>
            </tr>
        </table>
    </div>--%>
    <div style="<%= ShowSingleSearch %>">
        <h1>Search Results Details</h1>
        <p>
            <asp:HyperLink ID="hlReturnToSearchResults" runat="server" class="back" NavigateUrl="~/SearchInfo/results_care.aspx">Return to Previous Page</asp:HyperLink>
        </p>
        <p class="displayinline larger">
            Searched Service:
            <b>
                <asp:Label ID="lblServiceName" runat="server" Text="" />
            </b>
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
    </div>
    <div style="<%= ShowMultiSearch %>" >
        <h1>Lab Test Search Results Details</h1>  
        <p>
            <asp:HyperLink ID="HyperLink1" runat="server" class="back" NavigateUrl="~/SearchInfo/results_care.aspx">Return to Previous Page</asp:HyperLink>
        </p>          
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
                                <%# Eval("Description") %>
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
                                    [ <a href='<%= ResolveUrl("AddLab.aspx") %>'>Edit</a> ] &nbsp; [ <b><a href='<%= ResolveUrl("Search.aspx") %>#tabcare'>New</a></b> ]
                                </p>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <hr />
    <h2 class="bg">
        <asp:Label ID="lblPracticeName" runat="server" Text="" CssClass="h2bgStyle" style="color:Black;background-color:transparent;"/>
    </h2>
    <table width="100%" cellspacing="0" cellpadding="0" border="0" id="doctorfacilitytable">
        <tr class="h3 rowopen category" id="overview">
            <td>
                Overview
            </td>
        </tr>
        <tr class="">
            <%--class="careinstance"--%>
            <td>
                <div id="searchdetails">
                    <div id="detailinfo">
                        <table id="detailtable" cellspacing="0" cellpadding="4" border="0">
                            <tr>
                                <td>
                                    Address:
                                </td>
                                <td>
                                    <asp:Label ID="lblAddressLine1" runat="server" Text=""></asp:Label>
                                    <br />
                                    <asp:Label ID="lblAddressLine2" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
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
                                    <asp:Label ID="lblFax" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblEmailTitle" runat="server" Text="Email:" Visible="false"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblWebSiteTitle" runat="server" Text="Website:" Visible="false"></asp:Label>
                                </td>
                                <td>
                                    <asp:HyperLink ID="hlWebsite" runat="server"></asp:HyperLink>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblPrimarySpecialtyTitle" runat="server" Text="Primary Specialty:"
                                        Visible="false"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblSpecialty" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblOtherKeyItemTitle" runat="server" Text="" Visible="false"></asp:Label>
                                </td>
                                <td valign="top">
                                    <asp:Label ID="lblOtherKeyItemText" runat="server" Text=""></asp:Label>
                                    <div class="learnmore">
                                        <a title="Learn more">
                                            <asp:Image ID="imgOtherKeyLearnMoreIcon" runat="server" ImageUrl="../Images/icon_question_mark.png"
                                                Width="12" Height="13" border="0" alt="Learn More" Visible="false" />
                                            <div class="moreinfo">
                                                <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                                                    style="cursor: pointer;" />
                                                <p>
                                                    <b class="upper">MRI Tesla Rating</b><br />
                                                    Describes the Tesla Rating of MRI devices used at this facility. A Tesla Rating
                                                    is used to judge the strength of an MRI device which influences the resolution quality
                                                    of the resulting image. In general, the higher the Tesla rating, the better the
                                                    image quality. A clinical MRI machine will typically have a Tesla Rating of 1.5T
                                                    or 3.0T.</p>
                                            </div>
                                            <!-- end moreinfo -->
                                    </div>
                                    <!-- end learnmore -->
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="detailmap">
                        <div id="map_canvas" style="height: 350px; width: 425px">
                        </div>
                        <a href="LargerMap.aspx" style="color: #0000FF; text-align: left;display:block;">View Larger Map</a>
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
                    </div>
                </div>
            </td>
        </tr>
        <tr id="trSatisfaction">
            <td>
                <table width="100%">
                    <tr class="h3 openrow category" id="satisfaction">
                        <td>
                            <asp:Literal ID="ltlSatisfaction" runat="server" Text="Patient Satisfaction" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td style="width: 40%; padding: 12px;">
                                        <h3 class="savingschoice">
                                            Average Patient Satisfaction:
                                        </h3>
                                    </td>
                                    <td style="width: 60%;">
                                        <div class="userRatings inactiveStar miniStar" existingRating="2">
                                            <cchUIControl:starRating ID="StarRating2" runat="server" />
                                        </div>
                                        <div style="clear: left;">
                                            <asp:Label ID="lblRatingsCount" runat="server">(4 ratings)</asp:Label>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>            
                    </tr>
                    <tr>
                        <td>
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <h3 class="savingschoice">
                                Reviews <asp:Label ID="lblReviewsCount" runat="server">(5/16 Reviews)</asp:Label>
                            </h3>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr class="h3 rowopen category" id="docs">
            <td>
                <asp:Literal ID="ltlShowDocs" runat="server" Text="" />
            </td>
        </tr>
        <tr class="careinstance" style="display: table-row;">
            <td>
                <asp:DataList ID="dlDoctors" runat="server" RepeatColumns="2"  Width="100%"
                    onitemdatabound="dlDoctors_ItemDataBound">
                <ItemTemplate>
                    <%--<asp:LinkButton ID="lbtnDoctor" runat="server" CssClass="readmore"></asp:LinkButton>--%>
                    <asp:Label ID="lblDoctor" runat="server" Text="" />
                </ItemTemplate>
                </asp:DataList>
                <%--  <table cellspacing="0" cellpadding="2" border="0" width="100%">
                    <tr>
                        <td width="40%">
                            <a class="readmore" href="facility_doctor_detail.aspx">Dr. Samuel Abe</a>
                        </td>
                        <td width="60%">
                            <a class="readmore" href="facility_doctor_detail.aspx">Dr. Sam Abington</a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <a class="readmore" href="facility_doctor_detail.aspx">Dr. Elizabeth Anapole</a>
                        </td>
                        <td>
                            <a class="readmore" href="facility_doctor_detail.aspx">Dr. Anjali Joel</a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <a class="readmore" href="facility_doctor_detail.aspx">Dr. Christopher Bint</a>
                        </td>
                        <td>
                            <a class="readmore" href="facility_doctor_detail.aspx">Dr. Jose Barros</a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <a class="readmore" href="facility_doctor_detail.aspx">Dr. Arthur Chriss</a>
                        </td>
                        <td>
                            <a class="readmore" href="facility_doctor_detail.aspx">Dr. Grace Crane</a>
                        </td>
                    </tr>
                </table>--%>
            </td>
        </tr>
    </table>
    <p>
        <asp:Label ID="lblAllResult1DisclaimerText" CssClass="smaller" runat="server" Font-Italic="True"></asp:Label>
    </p>

    <link href="../Styles/starRatings.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/SavingsChoiceDetail.css" rel="stylesheet" type="text/css" />

</asp:Content>

