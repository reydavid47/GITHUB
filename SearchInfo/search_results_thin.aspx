<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="search_results_thin.aspx.cs" Inherits="ClearCostWeb.SearchInfo.search_results_thin" %>

<asp:Content ID="search_results_thin_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <div id="tabcare">
    <h1>
        Search Results</h1>
    <%--<div id="result_buttons">
        <div class="button">
            <a href="print.aspx" target="_blank">Print Results</a>
        </div>
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
    </div>--%>
    <!-- end result buttons -->
    <p class="displayinline larger">
        Searched Service: <b>
            <asp:Label ID="lblServiceName" runat="server" Text=""></asp:Label></b></p>
    <div class="learnmore">
        <a title="Learn more">
            <asp:Image ID="imgSvcLearnMore" runat="server" ImageUrl="../images/icon_question_mark.png"
                Width="12" Height="13" border="0" alt="Learn More" />
            <div class="moreinfo">
                <img src="../img/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
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
        &nbsp;&nbsp;&nbsp;<span style="display:inline-block;">[ <b><a href="search.aspx">New Search</a></b> ]</span></p>
    <br class="clearboth" />
    <p class="displayinline larger">
        Medicare reference price for this service: <b>$606</b></p>
    <div class="learnmore">
        <a title="Learn more">
            <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
        <div class="moreinfo">
            <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                style="cursor: pointer;" />
            <p>
                <b class="upper">Medicare Reimbursement</b><br />
                The price for a service paid by the government entity, Centers for Medicare and
                Medicaid Services (CMS), is considered a reference price because it is commonly
                used by private hospitals, practices, and physicians as a baseline to set prices
                for their services. A price that is less than 1.5 times the Medicare price is considered
                to be a low price</p>
        </div>
        <!-- end moreinfo -->
    </div>
    <!-- end learnmore -->
    <br class="clearboth" />
    <hr class="heavy" />
    <p class="smaller">
        <b class="green larger">$</b> Low price &nbsp;&nbsp; <b class="green larger">$$</b>
        High price &nbsp;&nbsp; <b class="green larger">$$$</b> Highest price
    </p>
    <span class="displayinline smaller"><b>Sort by:</b>
        <input type="radio" name="sort" value="facility" />
        Facility
        <input type="radio" name="sort" value="distance" />
        Distance
        <input type="radio" name="sort" value="estimated cost" />
        Estimated Cost </span>
    <table cellspacing="0" cellpadding="4" border="0" class="searchresults" width="100%">
        <th class="tdfirst" valign="bottom" width="37%">
            <a href="#">Name of Facility</a>
        </th>
        <th valign="bottom" width="10%">
            Location
        </th>
        <th valign="bottom" width="11%">
            <img src="../images/icon_arrow_down.gif" border="0" alt="" width="7" height="6">
            <a href="#">Distance</a>
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Distance</b><br />
                        Estimated distance (miles) from patient location to this facility</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </th>
        <th valign="bottom" width="15%">
            <a href="#">Estimated Cost</a>
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Estimated Cost</b><br />
                        Expected total price for this service or drug, based on recent payments made to this 
                        provider from your health plan.  How much of this total price is paid by you will vary 
                        based on your type of health plan and what other medical expenses you have had this year.
                        Because prices can change and the exact services and drug you receive can vary, it is 
                        possible that the total price will fall outside of the range presented here.</p>
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
                    <img src="../images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Healthgrades&trade;</b><br />
                        Checked if at least one physician at this facility is listed as "recognized" by
                        Healthgrades, a leading provider of information on physician quality. Healthgrades
                        Recognized Doctors Are:
                        <br />
                        1. Board certified in the specialty they practice
                        <br />
                        2. Have never had their license restricted/revoked
                        <br />
                        3. Free of state or federal disciplinary actions in the last 5 years
                        <br />
                        4. Free of any malpractice claims</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </th>
        </tr>
        <tr class="roweven graydiv graytop">
            <td class="tdfirst graydiv">
                <a class="readmore" href="#">Advanced Diagnostic Imaging</a>
                <%--href="results_care_detail.aspx"--%>
            </td>
            <td class="graydiv">
                Exeter
            </td>
            <td class="graydiv">
                7.5 mi
            </td>
            <td class="graydiv">
                <b class="green larger">$</b>
            </td>
            <td class="tdcheck graydiv">
                <img src="../images/check_purple.png" alt="X" width="23" height="23" border="" />
            </td>
        </tr>
        <tr class="graydiv">
            <td class="tdfirst graydiv">
                <a class="readmore" href="#">Access Sports Medicine & Ortho</a>
            </td>
            <td class="graydiv">
                Exeter
            </td>
            <td class="graydiv">
                8.5 mi
            </td>
            <td class="graydiv">
                <b class="green larger">$</b>
            </td>
            <td class="tdcheck graydiv">
                <img src="../images/check_purple.png" alt="X" width="23" height="23" border="" />
            </td>
        </tr>
        <tr class="roweven graydiv">
            <td class="tdfirst graydiv">
                <a class="readmore" href="#">Core Physicians</a>
            </td>
            <td class="graydiv">
                Exeter
            </td>
            <td class="graydiv">
                8.8 mi
            </td>
            <td class="graydiv">
                <b class="green larger">$$</b>
            </td>
            <td class="tdcheck graydiv">
                <img src="../images/check_purple.png" alt="X" width="23" height="23" border="" />
            </td>
        </tr>
        <tr class="graydiv">
            <td class="tdfirst graydiv">
                <a class="readmore" href="#">Derry Imaging Center</a>
            </td>
            <td class="graydiv">
                Derry
            </td>
            <td class="graydiv">
                17.7 mi
            </td>
            <td class="graydiv">
                <b class="green larger">$$</b>
            </td>
            <td class="tdcheck graydiv">
                <img src="../images/check_purple.png" alt="X" width="23" height="23" border="" />
            </td>
        </tr>
        <tr class="roweven graydiv">
            <td class="tdfirst graydiv">
                <a class="readmore" href="#">Parkland Medical Center</a>
            </td>
            <td class="graydiv">
                Derry
            </td>
            <td class="graydiv">
                20.7 mi
            </td>
            <td class="graydiv">
                <b class="green larger">$$</b>
            </td>
            <td class="graydiv">
                <img src="../images/check_purple.png" alt="X" width="23" height="23" border="" />
                                   
            </td>
        </tr>
        <tr class="graydiv">
            <td class="tdfirst graydiv">
                <a class="readmore" href="#">Merrimack Valley Health</a>
            </td>
            <td class="graydiv">
                Lawrence
            </td>
            <td class="graydiv">
                20.5 mi
            </td>
            <td class="graydiv">
                <b class="green larger">$</b>
            </td>
            <td class="tdcheck graydiv">
                <img src="../images/check_purple.png" alt="X" width="23" height="23" border="" />
            </td>
        </tr>
        <tr class="roweven graydiv">
            <td class="tdfirst graydiv">
                <a class="readmore" href="#">Elliot Hospital</a>
            </td>
            <td class="graydiv">
                Manchester
            </td>
            <td class="graydiv">
                26.2 mi
            </td>
            <td class="graydiv">
                <b class="green larger">$$$</b>
            </td>
            <td class="tdcheck graydiv">
                <img src="../images/check_purple.png" alt="X" width="23" height="23" border="" />
            </td>
        </tr>
    </table>
    <p>
        <i class="smaller">Note: Ranges shown here are based on data provided to ClearCost Health.
            Actual current prices may vary.</i>
    </p>
    </div>
</asp:Content>

