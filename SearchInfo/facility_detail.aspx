<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="facility_detail.aspx.cs" Inherits="ClearCostWeb.SearchInfo.facility_detail" %>

<asp:Content ID="facility_detail_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <h1>
    Search Result Details</h1>
    <p>
        <a href="results_facility.aspx" class="back">Return to search</a>
    </p>
    <div id="header_search_info">
        <h3 class="displayinline">
            Search Information</h3>
        &nbsp;&nbsp;
        <p class="displayinline smaller">
            [ <a href="search.aspx">Edit</a> ] &nbsp; <span style="display:inline-block;">[ <b><a href="search.aspx">New Search</a></b> ]</span></p>
        <br class="clearboth" />
    </div>
    <b class="smaller">Service: <span class="upper">
        <asp:Label ID="lblServiceName" runat="server" Text=""></asp:Label>
    </span></b>
    <div class="learnmore">
        <a title="Learn more">
            <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
        <div class="moreinfo">
            <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                style="cursor: pointer;" />
            <p>
                <b class="upper">Pediatrician, New Patient Office Visit</b><br />
                Search results for "Pediatrician, New Patient Office Visit" are based on the most common types of new patient office visits with pediatricians. </p>
        </div>
        <!-- end moreinfo -->
    </div>
    <!-- end learnmore -->
    <br />
    <span class="smaller"><b>Facility: <span class="upper">Rockman Pediatrics</span></b>
    </span>
    <hr />
    <!-- <h2 class="bg">Dr. Jan Smith</h2> -->
    <p>
        This search also displays results for other nearby facilities to help you understand
        relative cost and quality
    </p>
    <%--<form>--%>
    <span class="displayinline smaller"><b>Sort by:</b>
        <input type="radio" name="sort" value="doctor" />
        Facility
        <input type="radio" name="sort" value="distance" />
        Distance
        <input type="radio" name="sort" value="estimated cost" />
        Total Estimated Cost </span>
    <%--</form>--%>
    <table cellspacing="0" cellpadding="4" border="0" class="searchresults" width="100%">
        <th class="tdfirst" valign="bottom" width="38%">
            <img src="../Images/icon_arrow_down.gif" border="0" alt="" width="7" height="6">&nbsp;Name
            of Facility
        </th>
        <th valign="bottom" width="10%">
            Location
        </th>
        <th valign="bottom" width="10%">
            <a href="#">Distance</a>
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Location</b><br />
                        Estimated distance (miles) from patient location to this facility</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </th>
        <th colspan="3" valign="bottom" width="15%">
            <a href="#">Total Estimated Cost</a>
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
        </th>
        <th valign="bottom" width="11%">
            Fair Cost
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Fair Cost</b><br />
                        Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                        arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                        Etiam aliquet massa et lorem.</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </th>
        <th valign="bottom" width="16%">
            Healthgrades&trade;
            <br />
            Quality Indicator</span>
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Healthgrades&trade;</b><br />
                        Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                        arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                        Etiam aliquet massa et lorem.</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </th>
        </tr>
        <tr class="roweven graytop graydiv">
            <td class="tdfirst graydiv">
                <a class="readmore" href="individual_facility_detail.aspx">Rockman Pediatrics</a>
            </td>
            <td class="graydiv">
                Oak Park, IL
            </td>
            <td class="graydiv">
            </td>
            <td style="text-align: right">
                $115
            </td>
            <td>
                -
            </td>
            <td class="graydiv" style="text-align: right">
                $125
            </td>
            <td class="tdcheck graydiv">
            </td>
            <td class="tdcheck graydiv">
                <img src="../Images/check_purple.png" alt="X" width="23" height="23" border="" />
            </td>
        </tr>
        <tr class="graydiv">
            <td class="tdfirst graydiv">
                <a class="readmore" href="individual_facility_detail.aspx">Central DuMage Physician
                    Group</a>
            </td>
            <td class="graydiv">
                Glen Elyn, IL
            </td>
            <td class="graydiv">
                5.1 mi
            </td>
            <td style="text-align: right">
                $65
            </td>
            <td>
                -
            </td>
            <td style="text-align: right" class="graydiv">
                $75
            </td>
            <td class="tdcheck graydiv">
                <img src="../Images/check_green.png" alt="X" width="23" height="23" border="" />
            </td>
            <td class="tdcheck graydiv">
                <img src="../Images/check_purple.png" alt="X" width="23" height="23" border="" />
            </td>
        </tr>
        <tr class="roweven graydiv">
            <td class="tdfirst graydiv">
                <a class="readmore" href="individual_facility_detail.aspx">Navarro and Chavda, MDs</a>
            </td>
            <td class="graydiv">
                Lisle, IL
            </td>
            <td class="graydiv">
                5.3 mi
            </td>
            <td style="text-align: right">
                $65
            </td>
            <td>
                -
            </td>
            <td class="graydiv" style="text-align: right">
                $75
            </td>
            <td class="tdcheck graydiv">
                <img src="../Images/check_green.png" alt="X" width="23" height="23" border="" />
            </td>
            <td class="tdcheck graydiv">
                <img src="../Images/check_purple.png" alt="X" width="23" height="23" border="" />
            </td>
        </tr>
    </table>
    <p>
        <asp:Label ID="lblAllResult1DisclaimerText" CssClass="smaller" runat="server" Font-Italic="True"></asp:Label>
    </p>
</asp:Content>

