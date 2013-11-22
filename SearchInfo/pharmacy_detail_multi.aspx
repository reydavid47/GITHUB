<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true"
    CodeFile="pharmacy_detail_multi.aspx.cs" Inherits="ClearCostWeb.SearchInfo.pharmacy_detail_multi" %>
<%@ MasterType VirtualPath="~/SearchInfo/Results.master" %>

<asp:Content ID="pharmacy_detail_multi_Content" ContentPlaceHolderID="ResultsContent"
    runat="Server">
    <script type="text/javascript">
        function HandleOverviewClick() {
            if (map == null) {
                GenerateMapAndDirections(tempStart, tempEnd);
            }
        }
    </script>
    <script type="text/javascript">
        var directionsDisplay;
        var directionsService;
        var map;
        var start;
        var end;
        function GenerateMapAndDirections(startLatLong, endLatLong) {
            if (!google.maps) {
                google.load("maps", "3.8",
                    {
                        "other_params": ((window.location.host.indexOf("localhost") > 0) ? "" : "client=gme-clearcosthealth&") + "sensor=false",
                        "callback": GenerateMapAndDirections
                    }
                );
            }
            else {
                directionsService = new google.maps.DirectionsService()
                var directionsDisplay = new google.maps.DirectionsRenderer();
                var query = window.location.search.substring(1);
                //var vars = query.split("&");
                //var start = results[0].geometry.location;
                var startText = startLatLong.split(",");
                var endText = endLatLong.split(","); // //vars[1].split("=")[1].split(",");
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
                        //Added by JM to handle the event that there is a Distance label on the page that would need to be updated as a result of the directions search
                        var distElem = document.getElementById("<%= lblDistance.ClientID %>");
                        if (distElem != undefined) {
                            distElem.innerHTML = response.routes[0].legs[0].distance.text.replace(" mi", "");
                        }
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
        });
    </script>
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
            <div id="savesearch">
                <input type="text" name="searchname" onclick="this.value='';" onfocus="this.select()"
                    onblur="this.value=!this.value?'Name this search (e.g. Knee MRI)':this.value;"
                    value="Name this search (e.g. Knee MRI)" style="width: 230px;" />
                <br />
                <br />
                <a class="submitlink" onclick="document.savesearch.submit();return false;">Save</a>
                &nbsp;&nbsp;&nbsp;<a class="submitlink" id="cancelsavesearch">Cancel</a>
            </div>
        </div>
        -->
    </div>--%>
    <!-- end result buttons -->
    <p>
        <a class="back" href="results_rx_multi.aspx">Return to search results</a>
    </p>
    <h3>
        Search Information</h3>
    <br />
    <table cellspacing="0" cellpadding="4" border="0" class="searchquery">
        <tr>
            <th class="alignleft">
                Drug
            </th>
            <th>
                Dose
            </th>
            <th>
                Quantity
            </th>
        </tr>
        <tr class="graydiv graytop">
            <td class="graydiv alignleft">
                Nexium
            </td>
            <td class="graydiv">
                40mg
            </td>
            <td class="graydiv">
                30 pills (1 pill per day)
            </td>
        </tr>
        <tr class="graydiv">
            <td class="graydiv alignleft">
                Lunesta
            </td>
            <td class="graydiv">
                3mg
            </td>
            <td class="graydiv">
                30 pills (1 pill per day)
            </td>
        </tr>
        <tr class="graydiv">
            <td class="graydiv alignleft">
                Lisinopril
            </td>
            <td class="graydiv">
                20mg
            </td>
            <td class="graydiv">
                30 pills (1 pill per day)
            </td>
        </tr>
    </table>
    <br />
    <p class="smaller alertmini">
        Price at your current pharmacies for these drugs: <b>$336.46</b>
    </p>
    <hr />
    <div class="h2bg">
        Target Pharmacy</div>
    <!-- start collapsible table -->
    <table width="100%" cellspacing="0" cellpadding="0" border="0" id="doctorfacilitytable">
        <tr class="h3 rowclosed category" id="overview" onclick="HandleOverviewClick(); return false;">
            <td>
                Overview
            </td>
        </tr>
        <tr class="careinstance">
            <td>
                <div id="searchdetails">
                    <div id="detailinfo">
                        <table id="detailtable" cellspacing="0" cellpadding="4" border="0">
                            <tr>
                                <td width="50%">
                                    <b>Estimated Cost:</b>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td class="addldrug">
                                    Nexium
                                </td>
                                <td align="right">
                                    $159.85
                                </td>
                            </tr>
                            <tr>
                                <td class="addldrug">
                                    Lunesta
                                </td>
                                <td align="right">
                                    $164.26
                                </td>
                            </tr>
                            <tr>
                                <td class="addldrug">
                                    Lisinopril
                                </td>
                                <td align="right">
                                    $4.00
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" class="addldrug">
                                    <hr style="margin: 0;" />
                                </td>
                            </tr>
                            <tr>
                                <td class="addldrug">
                                    <b>Total</b>
                                </td>
                                <td align="right">
                                    <b>$328.11</b>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Estimated Savings<br />
                                        (versus current pharmacy):</b>
                                </td>
                                <td align="right">
                                    <b>$8.35</b>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Address:
                                </td>
                                <td>
                                    180 Somerville Ave<br />
                                    Somerville, MA 02143
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Phone:
                                </td>
                                <td>
                                    (617) 776-4919
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Hours:
                                </td>
                                <td>
                                    9am - 9pm (M-F)<br />
                                    9am - 6pm (Sat, Sun)
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Website:
                                </td>
                                <td>
                                    www.target.com
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="detailmap">
                        <div id="map_canvas" style="height: 350px; width: 425px">
                        </div>
                        <a href="LargerMap.aspx" style="color: #0000FF; text-align: left">View Larger Map</a>
                        <p>
                            <%--2.8&nbsp;miles from your location--%>
                            <asp:Label ID="lblDistance" runat="server" Text=""></asp:Label>&nbsp;miles from
                            your location
                        </p>
                        <%--<p>
                            <a href="#">Get directions</a></p>--%>
                        <div class="directions">
                        <a title="Get Directions">Get Directions </a>
                        <div class="directiondetails" id="directionsPanel" style="width: 390px;">
                            <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                                style="cursor: pointer;" /><br />
                        </div>
                    </div>
                    <div class="clearboth">
                    </div>
                </div>
            </td>
        </tr>
        <tr class="h3 rowclosed category" id="transfer">
            <td>
                Transfer Prescription to this Pharmacy
            </td>
        </tr>
        <tr class="careinstance">
            <td>
                <div class="rxinfo">
                    <p>
                        <b>Information You May Need:</b>
                    </p>
                    <table cellspacing="0" cellpadding="0" border="0" width="100%">
                        <tr>
                            <td>
                                Drug:
                            </td>
                            <td>
                                Nexium
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Quantity:
                            </td>
                            <td>
                                30 days (1 pill per day)
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Dosage:
                            </td>
                            <td>
                                40mg
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                Old Pharmacy:
                            </td>
                            <td>
                                Star Pharmacy @ 370 Western Ave<br />
                                Boston, MA 02135
                            </td>
                        </tr>
                        <tr>
                            <td width="50%">
                                Prescription #:
                            </td>
                            <td>
                                04151987
                            </td>
                        </tr>
                    </table>
                    <hr />
                    <table cellspacing="0" cellpadding="0" border="0" width="100%">
                        <tr>
                            <td>
                                Drug:
                            </td>
                            <td>
                                Lunesta
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Quantity:
                            </td>
                            <td>
                                30 days (1 pill per day)
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Dosage:
                            </td>
                            <td>
                                3mg
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                Old Pharmacy:
                            </td>
                            <td>
                                Star Pharmacy @ 370 Western Ave<br />
                                Boston, MA 02135
                            </td>
                        </tr>
                        <tr>
                            <td width="50%">
                                Prescription #:
                            </td>
                            <td>
                                04151988
                            </td>
                        </tr>
                    </table>
                    <hr />
                    <table cellspacing="0" cellpadding="0" border="0" width="100%">
                        <tr>
                            <td>
                                Drug:
                            </td>
                            <td>
                                Lisinopril
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Quantity:
                            </td>
                            <td>
                                30 days (1 pill per day)
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Dosage:
                            </td>
                            <td>
                                20mg
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                Old Pharmacy:
                            </td>
                            <td>
                                Star Pharmacy @ 370 Western Ave<br />
                                Boston, MA 02135
                            </td>
                        </tr>
                        <tr>
                            <td width="50%">
                                Prescription #:
                            </td>
                            <td>
                                04151989
                            </td>
                        </tr>
                    </table>
                </div>
                <!-- end rxinfo -->
                <p class="transfer">
                    Call the pharmacy at <b>(617) 782-1628</b> and let them know you would like to transfer
                    your prescription to Target Pharmacy and buy from their $4 Generics Program.
                </p>
            </td>
        </tr>
    </table>
    <!-- end doctorfacilitytable -->
    <p>
        <asp:Label ID="lblAllResult1DisclaimerText" CssClass="smaller" runat="server" Font-Italic="True"></asp:Label>
    </p>
</asp:Content>
