<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="pharmacy_list.aspx.cs" Inherits="ClearCostWeb.SearchInfo.pharmacy_list" %>

<asp:Content ID="pharmacy_list_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <h1>
        Search Results</h1>
    <%--<div id="result_buttons">
        <div class="button">
            <a href="#" onclick="javascript:window.print(); return false;">Print Results</a>
        </div>
        <%--
        <div class="button">
            <a href="results_care_share.aspx">Share with my Doctor</a>
        </div>--%>
        <!--
        <div class="button">
            <a href="#">Save this Search</a>
        </div>
        -->
    <%--</div>--%>--%>
    <!-- end result buttons -->
    <div id="header_search_info">
        <h3 class="displayinline">
            Search Information</h3>
        &nbsp;&nbsp;
        <p class="displayinline smaller">
            <span style="display:inline-block;">[ <b><a href="search.aspx">Edit</a></b> ]</span> &nbsp; <span style="display:inline-block;">[ <b><a href="search.aspx">New Search</a></b> ]</span></p>
        <br class="clearboth" />
    </div>
    <p>
        <b>Drug (active ingredient): <span class="upper">Lipitor (atorvastin)</span> &nbsp;
            &nbsp; &nbsp; Dose: <span class="upper">20mg</span> &nbsp; &nbsp; &nbsp; Frequency:
            <span class="upper">1 tablet per day</span> </b>
    </p>
    <hr />
    <span class="displayinline smaller"><b>Sort by:</b>
        <input type="radio" name="sort" value="facility" />
        Facility
        <input type="radio" name="sort" value="distance" />
        Distance
        <input type="radio" name="sort" value="estimated cost" />
        Estimated Cost </span>
    <table cellspacing="0" cellpadding="4" border="0" class="searchresults" width="100%">
        <th class="tdfirst" valign="bottom" width="30%">
            <img src="../Images/icon_arrow_down.gif" border="0" alt="" width="7" height="6">&nbsp;Name
            of Facility
        </th>
        <th valign="bottom" width="10%">
            Location
        </th>
        <th valign="bottom" width="10%">
            Mail or Retail
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Mail or Retail</b><br />
                        Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                        arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                        Etiam aliquet massa et lorem.</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </th>
        <th>
            <a href="#">Distance</a>
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Distance</b><br />
                        Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                        arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                        Etiam aliquet massa et lorem.</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </th>
        <th>
            <a href="#">Monthly Cost</a>
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Monthly Cost</b><br />
                        Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                        arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                        Etiam aliquet massa et lorem.</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </th>
        <th>
            Annual Cost
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Annual Cost</b><br />
                        Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                        arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                        Etiam aliquet massa et lorem.</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </th>
        </tr>
        <tr class="roweven whitediv">
            <td class="tdfirst whitediv">
                Pharmacy 147965
            </td>
            <td class="whitediv">
                Naperville
            </td>
            <td class="whitediv">
                Retail
            </td>
            <td class="whitediv">
                7.0 mi
            </td>
            <td class="whitediv">
                $115 - $125
            </td>
            <td>
                $1380 - $1440
            </td>
        </tr>
        <tr>
            <td class="tdfirst">
                Pharmacy 458303
            </td>
            <td>
                Naperville
            </td>
            <td>
                Retail
            </td>
            <td>
                7.0 mi
            </td>
            <td>
                $125 - $135
            </td>
            <td>
                $1500 - $1620
            </td>
        </tr>
        <tr class="roweven whitediv">
            <td class="tdfirst whitediv">
                Drugstore.com
            </td>
            <td class="whitediv">
                www.drugstore.com/lipitor
            </td>
            <td class="whitediv">
                Mail
            </td>
            <td class="whitediv">
                N/A
            </td>
            <td class="whitediv">
                $140 - $150
            </td>
            <td class="whitediv">
                $1680 - $1800
            </td>
        </tr>
        <tr>
            <td class="tdfirst">
                HealthWarehouse
            </td>
            <td>
                www.healthwarehouse.com
            </td>
            <td>
                Mail
            </td>
            <td>
                N/A
            </td>
            <td>
                $150 - $160
            </td>
            <td>
                $1800 - $1920
            </td>
        </tr>
        <tr class="roweven whitediv">
            <td class="tdfirst whitediv">
                Costco
            </td>
            <td class="whitediv">
                www.costco.com/lip20
            </td>
            <td class="whitediv">
                Both
            </td>
            <td class="whitediv">
                N/A
            </td>
            <td class="whitediv">
                $165 - $175
            </td>
            <td class="whitediv">
                $1980 - $2100
            </td>
        </tr>
    </table>
</asp:Content>

