<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true"
    CodeFile="results_rx_multi.aspx.cs" Inherits="ClearCostWeb.SearchInfo.results_rx_multi" %>

<asp:Content ID="results_rx_multi_Content" ContentPlaceHolderID="ResultsContent"
    runat="Server">
    <h1>
        Search Results</h1>
    <%--<div id="result_buttons">
        <div class="button">
            <a href="print.aspx" target="_blank" onclick="javascript:window.print();return false;">Print Results</a>
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
    <div id="header_search_info">
        <h3 class="displayinline">
            Search Information</h3>
        &nbsp;&nbsp;
        <p class="displayinline smaller">
            [ <a href="search.aspx">Edit</a> ] &nbsp; <span style="display:inline-block;">[ <b><a href="search.aspx#tabrx">New Search</a></b>
            ]</span></p>
        <br class="clearboth" />
    </div>
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
                30 pills
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
                30 pills
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
                30 pills
            </td>
        </tr>
    </table>
    <br />
    <p class="smaller alertmini">
        Price at your current pharmacies for these drugs: <b>$336.46</b>
    </p>
    <hr />
    <p class="smaller alertsmall">
        You and your employer could save <b>$8.35</b>
    </p>
    <br class="clearboth" />
    <span class="displayinline smaller"><b>Sort by:</b>
        <input type="radio" name="sort" value="Pharmacy" />
        Pharmacy
        <input type="radio" name="sort" value="distance" />
        Distance
        <input type="radio" name="sort" value="estimated cost" />
        Estimated Cost </span>
    <p class="smaller">
        Click on a pharmacy for details.
    </p>
    <!-- this table with the headers is narrower than the body to match the width minus the scrollbar -->
    <table cellspacing="0" cellpadding="4" border="0" class="searchresults" width="820">
        <th class="tdfirst" valign="bottom" width="50%">
            <a href="#">Pharmacy</a>
        </th>
        <th valign="bottom" width="12%">
            Location
        </th>
        <th width="14%">
            <img src="../images/icon_arrow_down.gif" border="0" alt="" width="7" height="6"/>&nbsp;<a
                href="#">Distance</a>
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper"><a href="#">Distance</a></b><br />
                        Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                        arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                        Etiam aliquet massa et lorem.</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </th>
        <th width="19%">
            <a href="#">Estimated Cost</a>
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Estimated Cost</b><br />
                        Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                        arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                        Etiam aliquet massa et lorem.</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </th>
<%--        </tr>   Where did I come from?  No matching opening tag --%>
    </table>
    <div class="scroll-pane">
        <table cellspacing="0" cellpadding="4" border="0" class="searchresults" width="100%">
            <tr class="roweven graydiv graytop">
                <td class="tdfirst graydiv" width="50%">
                    <a class="readmore" href="pharmacy_detail_multi.aspx">CVS</a>
                </td>
                <td class="graydiv" width="12%">
                    Boston
                </td>
                <td class="graydiv" width="14%">
                    1.2 mi
                </td>
                <td class="graydiv alignrightcenter" width="19%">
                    $336.40
                </td>
            </tr>
            <tr class="graydiv">
                <td class="tdfirst graydiv">
                    <a class="readmore" href="pharmacy_detail_multi.aspx">Walgreens</a>
                </td>
                <td class="graydiv">
                    Cambridge
                </td>
                <td class="graydiv">
                    2.4 mi
                </td>
                <td class="graydiv alignrightcenter">
                    $337.17
                </td>
            </tr>
            <tr class="roweven graydiv">
                <td class="tdfirst graydiv">
                    <a class="readmore" href="pharmacy_detail_multi.aspx">Target Pharmacy</a> <b>(Best Price)</b>
                </td>
                <td class="graydiv">
                    Somerville
                </td>
                <td class="graydiv">
                    2.8 mi
                </td>
                <td class="graydiv alignrightcenter">
                    $328.11
                </td>
            </tr>
            <tr class="graydiv">
                <td class="tdfirst graydiv">
                    <a class="readmore" href="pharmacy_detail_multi.aspx">Star Pharmacy</a> <b>(Current
                        Pharmacy)</b>
                </td>
                <td class="graydiv">
                    Cambridge
                </td>
                <td class="graydiv">
                    3.8 mi
                </td>
                <td class="graydiv alignrightcenter">
                    $336.46
                </td>
            </tr>
            <tr class="roweven graydiv">
                <td class="tdfirst graydiv">
                    <a class="readmore" href="pharmacy_detail_multi.aspx">WalMart</a>
                </td>
                <td class="graydiv">
                    Quincy
                </td>
                <td class="graydiv">
                    13.8 mi
                </td>
                <td class="graydiv alignrightcenter">
                    $332.72
                </td>
            </tr>
        </table>
    </div>
    <div id="alertbar">
        <div id="alert">
            <p>
                You and your employer could save up to <b>$100</b> a year by switching one or more
                of your medications. To learn more, click on <a id="targetpast">Past Care</a>.</p>
        </div>
        <!-- end alert -->
        <div class="clearboth">
        </div>
    </div>
    <!-- end alertbar -->
</asp:Content>
