<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="results_specialty.aspx.cs" Inherits="ClearCostWeb.SearchInfo.results_specialty" %>
<%@ Register Src="../Controls/LearnMore.ascx" TagPrefix="cch" TagName="LearnMore" %>
<%@ Register Src="~/Controls/Slider.ascx" TagPrefix="cch" TagName="Slider" %>
<asp:Content ID="results_specialty_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <style type="text/css">
        div#results
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
        table#tblSearchResults
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
        #fsPreferred 
        {
            padding: 2px;
            margin-left: -3px;
            display: inline-block;
            border: 1px solid #ccc;
            width: 824px;
            margin-bottom: 10px;
            padding-top: 0px;
        }
        #fsPreferred legend 
        {
            font-weight: bold;
            text-transform: uppercase;
            margin-left: 35px;
        }
        #fsPreferred #dPreferredProviders 
        {
            max-height: 165px;
            overflow-y: auto;
            max-width: 824px;
            min-width: 807px;
        }
        #tblPreferredResults td 
        {
            background-color: rgb(225,238,218);
        }
        #fsPreferred
        {
            display:none;
        }
    </style>
    <%--<script src="../Scripts/Results_Specialty.js" type="text/javascript"></script>--%>
    <asp:HiddenField ID="POSTNAV" runat="server" Value="" ClientIDMode="Static" />
    <asp:HiddenField ID="POSTDIST" runat="server" Value="" ClientIDMode="Static" />
    <script type="text/javascript">
        var firstRun = true;
        var globDistance = <%= this.distance %>;
        function StartFindADocResults() {
            if (firstRun) {
                if ("<%=InitialSortOption%>" == "Distance")
                    $("a.sortHeader[sortCol='NumericDistance']").addClass("sortAsc");
                else
                    $("a.sortHeader[sortCol='FPDoc']").addClass("sortAsc");
                firstRun = false;
            }
            var FADROptions = {
                SpinLoader: "img.loadingSpinner",
                ScrollPane: ".resultsPane",
                ResultTable: "table#tblSearchResults",
                HeaderTable: "table#Headers",
                RadioHeaderDiv: "div.rbHeaders",
                YourCostTog: "div.ycToggle"
            },
            OptionsForFirstRun = {
                //  CurrentSort: "Distance",  lam, 20130618, MSB-324
                CurrentSort: "<%=InitialSortOption%>",
                CurrentDirection: "ASC",
                FromRow: 0,
                ToRow: 25,
                Distance: globDistance,
                Latitude: $("#PATIENTLATITUDE").val(),
                Longitude: $("#PATIENTLONGITUDE").val()
            };
            if (typeof (YourCostDefault) != "undefined") { FADROptions.YourCostOn = YourCostDefault; }
            $("table#tblSearchResults").FADR(FADROptions);
            //Run the first set with the starting settings
            $("table#tblSearchResults").FADR("GetResults", OptionsForFirstRun);
        }
    </script>
    <h1>
        Search Results</h1>
    <%--<div id="result_buttons">
        <table>
            <tr valign="top">
                <td>
                        <%--<asp:ImageButton ID="ibtnPrintResults" runat="server" ImageUrl="~/Images/PrintResults.PNG"
                        OnClick="ibtnPrintResults_Click" />--%>
                       <%-- <asp:ImageButton ID="ibtnPrintResults" runat="server" ImageUrl="~/Images/PrintResults.PNG" 
                        OnClientClick="javascript:window.print(); return false;" />
                </td>
                <td>--%>
                <!--
                    <div class="button">
                        <a id="savesearch" class="pointer">Save this Search</a>
                    </div>
                    <div id="savesearchform">
                        <h3>
                            Save this Search</h3>
                        <%--<form name="savesearch">--%>
                        <div id="savesearch">
                            <input type="text" name="searchname" onclick="this.value='';" onfocus="this.select()"
                                onblur="this.value=!this.value?'Name this search (e.g. Knee MRI)':this.value;"
                                value="Name this search (e.g. Knee MRI)" style="width: 230px;" />
                            <br />
                            <br />
                            <a class="submitlink" id="applysavesearch">Save</a> &nbsp;&nbsp;&nbsp;<a class="submitlink"
                                id="cancelsavesearch">Cancel</a>
                        </div>
                        <%--</form>--%>
                    </div>
                    -->
                    <%-- <asp:ImageButton ID="ibtnSaveSearch" runat="server" ImageUrl="~/Images/SaveSearch.PNG"
                        OnClick="ibtnSaveSearch_Click" />--%>
                <%--</td>
            </tr>
        </table>
    </div>--%>
    <!-- end result buttons -->
    <p>
        <a href="search.aspx" class="back">Return to search</a>
    </p>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <p class="displayinline larger">
                Searched Specialty:<b>
                    <asp:Label ID="lblSpecialty" runat="server" Text=""></asp:Label>
                </b>
            </p>
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
                        <%--<asp:Label ID="lblSpecialtyMoreInfo" runat="server" Text="" ClientIDMode="Static"></asp:Label>--%>
                        <span id="SpecialtyMoreInfo">Loading Detailed Information...</span>
                    </p>
                </div>
                <!-- end moreinfo -->
            </div>
            &nbsp;&nbsp;
            <p class="displayinline smaller">
                <span style="display:inline-block;">[ <b><a href="search.aspx">New Search</a></b> ]</span></p>
            <br class="clearboth" />
            <hr />
            <%-- <form>--%>
            <div class="buttonview viewshows" id="tableDiv" style="cursor:default;">
                <a class="table-map viewshows" id="showtableview" style="cursor:default;">Table View</a>
            </div>
            <div class="buttonview" id="mapDiv">
                <a class="table-map" id="showmapview">Map View</a>
            </div>
            <br class="clearboth" />
            <div id="tableview" class="showview">
                <div class="displayinline smaller">
                    <%--<b>Sort by:</b>--%>
                    <%--<input type="radio" name="sort" value="Doctor Last Name" class="sortHeader" sortCol="ProviderName" />Doctor Last Name--%>
                    <%--<span class="Dist">
                        <input type="radio" name="sort" value="Distance" class="sortHeader" sortCol="NumericDistance" checked="checked" />
                        Distance
                    </span>--%>
                    <% if (!(ClearCostWeb.ThisSession.ServiceEnteredFrom == "DropDowns")) 
                       { %>
                    <span class="EIOVC" style="display:none;">
                        <input type="radio" name="sort" value="Estimated Initial Office Visit Cost" class="sortHeader" sortCol="EIOVC" />
                        Estimated Initial Office Visit Cost
                    </span>
                    <span class="YC" style="display:none;">
                        <input type="radio" name="sort" value="Your Estimated Cost" class="sortHeader" sortCol="YC" />
                        Your Estimated Cost
                    </span>
                    <% } %>
                    <%--<input type="radio" name="sort" value="FairPrice Doc" class="sortHeader FP" sortCol="FPDoc" />FairPrice--%>
                    <%--<input type="radio" name="sort" value="Healthgrades Recognized Physician" class="sortHeader" sortCol="Healthgrades" />Healthgrades&trade;--%>
                    <%--<input type="radio" name="sort" value="Patient Rating" class="sortHeader" sortCol="HGOverallRating" />Patient Rating--%>
                </div>
                <% if (!(ClearCostWeb.ThisSession.ServiceEnteredFrom == "DropDowns"))
                   { %>
                <div class="EIOVCToggle" style="display:none;visibility:hidden;">
                    <b style="vertical-align:middle;">Estimated Initial Office Visit Cost:&nbsp;</b>
                    <img src="../Images/toggle_off.png" alt="" class="EIOVC" style="vertical-align:bottom;cursor:pointer;" />
                    <img src="../Images/toggle_on.png" alt="" class="EIOVC" style="vertical-align:bottom;cursor:pointer;display:none;" />
                </div>
                <%--<div class="YCToggle">
                    <b style="vertical-align:middle;">Your Estimated Cost:&nbsp;</b>
                    <img src="../Images/toggle_off.png" alt="" class="YC" style="vertical-align:bottom;cursor:pointer;" />
                    <img src="../Images/toggle_on.png" alt="" class="YC" style="vertical-align:bottom;cursor:pointer;display:none;" />
                </div>--%>
                <% } %>
                <div class="slider">
                    <asp:Label AssociatedControlID="sFindADoc" runat="server" Text="Distance range: " Visible="false" />
                    <cch:Slider ID="sFindADoc" runat="server" Min="1" Max="100" Value="25" Width="200px" OnSlideChanged="updateDistance" Visible="false"
                        style="display:inline-block;vertical-align:middle;" />
                    <asp:Label ID="lblSliderValue" runat="server" AssociatedControlID="sFindADoc" Text=" 25 miles" Visible="false" />
                </div>
                <table cellspacing="0" cellpadding="4" border="0" class="searchresults" style="width:824px;">
                    <tbody>
                        <tr>
                            <td>
                                <div style="width:824px;">
                                    <table cellspacing="0" cellpadding="4" border="0" class="searchresults" style="width:807px;">
                                        <tbody>
                                            <tr>
                                                <td class="tdfirst NameLoc" valign="bottom" style="width:32%;vertical-align:bottom;">
                                                    <%--<a class="sortHeader" sortCol="ProviderName">Name Of Doctor</a>--%>
                                                    <a style="color:Black;cursor:default;">Name Of Doctor</a>
                                                </td>
                                                <td class="Dist" style="width:10%;vertical-align:bottom;">
                                                    <a class="sortHeader" sortCol="NumericDistance">Distance</a>
                                                    <cch:LearnMore ID="lmDist" runat="server">
                                                        <TitleTemplate>
                                                            Distance
                                                        </TitleTemplate>
                                                        <TextTemplate>
                                                            Estimated distance (miles) from patient location to this facility.
                                                        </TextTemplate>
                                                    </cch:LearnMore>                                                  
                                                </td>
                                                <% if (!(ClearCostWeb.ThisSession.ServiceEnteredFrom == "DropDowns"))
                                                   { %>
                                                <td class="EIOVC" style="width:12%;vertical-align:bottom;display:none;">
                                                    <a class="sortHeader" sortCol="EIOVC">Estimated Initial Office Visit Cost</a>
                                                </td>
                                                <td class="YC" style="width:12%;vertical-align:bottom;display:none;">
                                                    <a class="sortHeader" sortCol="YC">Your Estimated Cost</a>
                                                    <cch:LearnMore runat="server">
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
                                                <% } %>                                                
                                                <td class="FP" style="width:12%;vertical-align:bottom;">
                                                    <!--  lam, 20130618, MSB-324 -->
                                                    <%if (AllowFairPriceSort)
                                                      {%>
                                                    <a class="sortHeader" sortCol="FPDoc">Fair Price Doc</a>
                                                    <%}
                                                      else
                                                      { %>
                                                    <a class="" style="color:Black;cursor:default;" sortCol="FPDoc">Fair Price Doc</a>
                                                    <%} %>
                                                    <cch:LearnMore runat="server">
                                                        <TitleTemplate>
                                                            Fair Price
                                                        </TitleTemplate>
                                                        <TextTemplate>
                                                            Checked if a physician's practice has Fair Prices, based on algorithms developed by ClearCost Health.  
                                                            The designation of a Fair Price Practice is based not only on what is done directly by the doctors at that practice, 
                                                            but also in terms of where those doctors send patients for lab tests and imaging studies.
                                                        </TextTemplate>
                                                    </cch:LearnMore>
                                                </td>
                                                <td class="HG" style="width:17%;vertical-align:bottom;">
                                                    <a style="cursor:default;color:Black;">Healthgrades&trade; Recognized Physician</a>
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
                                                </td>
                                                <td class="ratingTD" style="width:17%;vertical-align:bottom;">
                                                    <%--<a class="sortHeader" sortCol="HGOverallRating">Patient Rating</a>--%>
                                                    <a style="color:Black;cursor:default;">Healthgrades Patient Satisfaction</a>
                                                    <cch:LearnMore runat="server">
                                                        <TitleTemplate>
                                                            Patient Satisfaction
                                                        </TitleTemplate>
                                                        <TextTemplate>
                                                            Healthgrades&trade; compiles reviews from patients, based on the following 9 criteria:<br />
                                                            1.Would you recommend this physician to family/friends?<br />
                                                            2.Do you trust this physician to make decisions/recommendations that are in your best interests?<br />
                                                            3.Does the provider help you understand your medical condition(s)?<br />
                                                            4.Does this physician listen to you and answer your questions?<br />
                                                            5.Do you feel this physician spends an appropriate amount of time with you?<br />
                                                            6.Ease of scheduling urgent appointments when you feel ill<br />
                                                            7.Office environment (cleanliness, comfort, lighting, temperature, location)<br />
                                                            8.Friendliness and courtesy of the office staff<br />
                                                            9.Once you arrive for a scheduled appointment, how long do you have to wait(including waiting room and exam room) before you see this physician?<br />
                                                            <b>Click on a physician's name to see more detail on patient reviews.</b>
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
                                <fieldset id="fsPreferred">
                                    <legend align="left">Caesars Preferred Providers</legend>
                                    <div id="dPreferredProviders">
                                        <table id="tblPreferredResults" cellspacing="0" cellpadding="4" border="0" class="searchResults" style="width:807px;">
                                        </table>
                                    </div>
                                </fieldset>
                                <div id="results" class="resultsPane">
                                    <img class="loadingSpinner" src="../Images/ajax-loader-AltCircle.gif" alt="" />
                                    <table id="tblSearchResults" cellspacing="0" cellpadding="4" border="0" class="searchresults">
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
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
    <p>
        <asp:Label ID="lblAllResult1DisclaimerText" CssClass="smaller" runat="server" Font-Italic="True"></asp:Label>
    </p>
    <% if (this.IsCaesarsOphthalmology)
        { %>
        <p>
            <i class="smaller"><b>*</b>Providers listed on the ClearCost site are included in the Cigna network. For providers included in the Eye Med network, please contact Eye Med.</i>
        </p>
    <% } %>
    <% if (this.IsMentalHealth)
        { %>
        <p>
            <asp:Label ID="lblMentalHealthDisclaimerText" CssClass="smaller" runat="server" Font-Italic="True"></asp:Label>
        </p>
    <% } %>
    <script type="text/javascript">
        StartFindADocResults();
    </script>
</asp:Content>

