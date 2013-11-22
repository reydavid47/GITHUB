<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true"
    CodeFile="pharmacy_detail.aspx.cs" Inherits="ClearCostWeb.SearchInfo.pharmacy_detail" %>
<%@ Register Src="~/Controls/AlertBar.ascx" TagPrefix="cch" TagName="AlertBar" %>
<%@ Import Namespace="ClearCostWeb" %>

<asp:Content ID="pharmacy_detail_Content" ContentPlaceHolderID="ResultsContent" runat="Server">
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

            if (map == null) { GenerateMapAndDirections(); }
        });
    </script>
    <script type="text/javascript">
        var map;
        var directionsDisplay;
        var directionsService;
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
                if ($("#<%= lblPharmacyName.ClientID %>").attr("pLat") != "0" && $("#<%= lblPharmacyName.ClientID %>").attr("pLng") != "0") {
                    directionsService = new google.maps.DirectionsService();
                    directionsDisplay = new google.maps.DirectionsRenderer();
                    var patientLat = $("#PATIENTLATITUDE").val();
                    var patientLng = $("#PATIENTLONGITUDE").val();
                    var pharmLat = $("#<%= lblPharmacyName.ClientID %>").attr("pLat");
                    var pharmLng = $("#<%= lblPharmacyName.ClientID %>").attr("pLng");
                    var start = new google.maps.LatLng(patientLat, patientLng);
                    var end = new google.maps.LatLng(pharmLat, pharmLng);
                    var myOptions = {
                        zoom: 7,
                        mapTypeId: google.maps.MapTypeId.ROADMAP,
                        center: end
                    }
                    map = new google.maps.Map(document.getElementById('map_canvas'), myOptions);
                    var marker = new google.maps.Marker({
                        position: end,
                        map: map,
                        title: 'Pharmacy'
                    });
                    marker.setZIndex(0);  //  lam, 20130729, MSF-298
                    directionsDisplay.setMap(map);
                    directionsDisplay.setPanel(document.getElementById('directionsPanel'));
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
                else {
                    $("div#detailmap").hide();
                }
            }
        }
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
        <%--<asp:HyperLink ID="hlReturnToSearchResults" runat="server" CssClass="back" NavigateUrl="results_rx.aspx#tabrx" Text="Return to search results" />--%>
        <a onclick="history.back();" style="cursor:pointer;" class="back">Return to search results</a>
    </p>
    <asp:Panel ID="pnlSingleDrug" runat="server" Visible="true">
        <p class="displayinline larger">
            Searched Drug:
            <b> 
                <asp:Label ID="lblSingleDrugName" runat="server" Text="" />
            </b>
            &nbsp; &nbsp; &nbsp; Searched Dose:
            <b>
                <asp:Label ID="lblSingleDrugDose" runat="server" Text="" />
            </b>
            &nbsp; &nbsp; &nbsp; Searched Quantity: 
            <b>
                <asp:Label ID="lblSingleDrugQuantity" runat="server" Text="" />
            </b>&nbsp;&nbsp;
            <p class="displayinline smaller">
                <span style="display:inline-block;">[ <b><asp:LinkButton ID="lbNew" runat="server" OnClick="NavNew" Text="New Search" /></b> ]</span></p>
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
                <span style="display:inline-block;">[ <b><asp:LinkButton ID="LinkButton1" runat="server" OnClick="NavNew" Text="New Search" /></b> ]</span></p>
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
        <!-- <a class="xclose" onclick="cancelOverlay()">X</a> -->
        <%-- <h3>
            Add this Drug to Med List</h3>--%>
        <%--<form name="meduser">--%>
        <div id="meduser">
            <p>
                Select the family member who will use this drug
                <select>
                    <option>Mary Smith</option>
                    <option>John Smith</option>
                </select>
                <br />
                <br />
                <a class="submitlink" onclick="cancelOverlay()" href="#">Save</a> &nbsp;&nbsp;&nbsp;<a
                    class="submitlink" onclick="cancelOverlay()">Cancel</a>
            </p>
        </div>
        <%--</form>--%>
    </div>
    <cch:AlertBar runat="server" ID="abCurrentPrice" TypeOfAlert="Mini" SaveTotal="" Visible="true">
        <MessageTemplate>
            Current price at your pharmacy <%# Container.PharmacyName.Trim() %> for this drug: <b><%# Container.SaveTotal %></b>
        </MessageTemplate>
    </cch:AlertBar>
    <hr />
    <h2 class="bg">
        <asp:Label ID="lblPharmacyName" runat="server" Text="" CssClass="h2bgStyle" style="color:Black;" />
    </h2>
    <!-- start collapsible table -->
    <table width="100%" cellspacing="0" cellpadding="0" border="0" id="doctorfacilitytable">
        <tr class="h3 category rowopen" id="overview">
            <td>
                Overview
            </td>
        </tr>
        <tr class="careinstance" style="display: table-row;">
            <td>
                <div id="searchdetails">
                    <div id="detailinfo">
                        <table id="detailtable" cellspacing="0" cellpadding="4" border="0">
                            <tr>
                                <td>
                                    <b>Total Estimated Cost:</b>
                                </td>
                                <td align="right">
                                    <b><asp:Label ID="lblPrice" runat="server" Text="" /></b>
                                    <%--$<asp:Label ID="lblRangeMin" runat="server" Text=""></asp:Label>
                                    - $<asp:Label ID="lblRangeMax" runat="server" Text=""></asp:Label>--%>
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
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:panel ID="pnlSavingsTitle" runat="server">
                                        <b>Estimated Savings:<br />
                                        (versus current pharmacy)</b>
                                    </asp:panel>
                                </td>
                                <td align="right">
                                    <b><asp:Label ID="lblSavings" runat="server" Text="" /></b>
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
                                    <asp:Label ID="lblHoursTitle" runat="server" Text="Hours:" Visible="false" />
                                </td>
                                <td>
                                    <asp:Label ID="lblHours" runat="server" Text="" />
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
                                    <asp:Literal ID="ltlURL" runat="server" Text="" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="detailmap">
                        <div id="map_canvas" style="height: 350px; width: 425px">
                        </div>
                        <a href="LargerMap.aspx" style="color: #0000FF; text-align: left; display: block;">View Larger Map</a>
                        <%--<p>
                            <%--2.8&nbsp;miles from your location
                            <asp:Label ID="lblDistance" runat="server" Text=""></asp:Label>&nbsp;miles from
                            your location
                        </p>--%>
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
        <% if (UITransferVisible)
           { %>
        <tr class="h3 category open" id="transfer" style="display:table-row;">
            <td>
                Transfer Prescription to this Pharmacy
            </td>
        </tr>
        <tr class="careinstance" style="display: table-row;" id="transferDetails">
        <% }
           else
           { %>
        <tr class="h3 category open" id="transfer" style="display:none;">
            <td>
                Transfer Prescription to this Pharmacy
            </td>
        </tr>
        <tr class="careinstance" style="display:none;" id="transferDetails">
        <% } %>
            
            <td>
                <div class="rxinfo">
                    <p>
                        <b>Information You May Need:</b>
                    </p>
                    <asp:Repeater ID="rptTransfer" runat="server">
                        <ItemTemplate>
                            <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                <tbody>
                                    <tr>
                                        <td>
                                            Drug:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblTransferDrug" runat="server" Text='<%# Eval("DrugName") == "" ? (ThisSession.ChosenDrugs == null ? "" : ThisSession.ChosenDrugs.Rows[0][5].ToString() + " " + Eval("Strength")) : Eval("DrugName") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Quantity:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblTransferQuantity" runat="server" Text='<%# Eval("Quantity") == "" ? (ThisSession.ChosenDrugs == null ? "" : String.Format("{0:0.00}", ThisSession.ChosenDrugs.Rows[0][2])) : String.Format("{0} {1}", Eval("Quantity"), Eval("QuantityUOM")) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Dosage:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblTransferDosage" runat="server" Text='<%# Eval("Strength") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            Old Pharmacy:
                                        </td>
                                        <td>
                                            <asp:Literal Text='<%# Eval("OldPharmText") %>' runat="server" ID="ltlOldPharmacy" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="40%">
                                            Prescription #:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPerscriptionNumber" runat="server" Text='<%# Eval("PrescriptionNumber") %>' />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </ItemTemplate>
                        <SeparatorTemplate><hr /></SeparatorTemplate>
                    </asp:Repeater>
                    <%--<table cellspacing="0" cellpadding="0" border="0" width="100%">
                        <tr>
                            <td>
                                Drug:
                            </td>
                            <td>
                                <asp:Label ID="lblTransferDrug" runat="server"  Text="" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Quantity:
                            </td>
                            <td>
                                <asp:Label ID="lblTransferQuantity" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Dosage:
                            </td>
                            <td>
                                <asp:Label ID="lblTransferDosage" runat="server" Text="" />
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                Old Pharmacy:
                            </td>
                            <td>
                                <asp:Literal Text="" runat="server" ID="ltlOldPharmacy" />
                            </td>
                        </tr>
                        <tr>
                            <td width="40%">
                                Prescription #:
                            </td>
                            <td>
                                <asp:Label ID="lblPerscriptionNumber" runat="server" Text="" />
                            </td>
                        </tr>
                    </table>--%>
                </div>
                <p class="transfer">
                    Call the pharmacy at <b><asp:Label ID="lblTransferPhone" runat="server" Text="" /></b> and let them know you would like to transfer
                    your prescription to the <%= lblPharmacyName.Text %> Pharmacy<%-- and buy from their $4/$10 Prescription
                    Program--%>.
                </p>
            </td>
        </tr>
    </table>
    <!-- end doctorfacilitytable -->
    <p>
        <asp:Label ID="lblAllResult1DisclaimerText" CssClass="smaller" runat="server" Font-Italic="True"></asp:Label>
    </p>
</asp:Content>