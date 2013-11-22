<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FindAServiceResults.ascx.cs" Inherits="ClearCostWeb.Controls.FindAServiceResults" %>
<%@ Register Src="~/Controls/LearnMore.ascx" TagPrefix="cch" TagName="LearnMore" %>
<%@ Register Src="~/Controls/Slider.ascx" TagPrefix="cch" TagName="Slider" %>

<asp:HiddenField ID="POSTNAV" runat="server" Value="" ClientIDMode="Static" />
<asp:HiddenField ID="POSTDIST" runat="server" Value="" ClientIDMode="Static" />
<script type="text/javascript">    
    function StartFindAServiceResults() {
        //Setup and attach the system to the needed elements
        var FASROptions = {
            SpinLoader: "img.loadingSpinner",
            ScrollPane: "#pnlServices",
            ResultTable: "table#tblSearchResults",
            HeaderTable: "table#Headers",
            RadioHeaderDiv: "div.rbHeaders",
            YourCostTog: "div.ycToggle"
        },
        OptionsForFirstRun = {
            CurrentSort: "Distance",
            CurrentDirection: "ASC",
            FromRow: 0,
            ToRow: 25,
            Distance: <%= this.distance %>,
            Latitude: $("#PATIENTLATITUDE").val(),
            Longitude: $("#PATIENTLONGITUDE").val()
        };
        if (typeof (globDefSort) != "undefined") {OptionsForFirstRun.CurrentSort = globDefSort; }
        if (typeof(YourCostDefault) != "undefined") { FASROptions.YourCostOn = YourCostDefault; }
        $("table#tblSearchResults").FASR(FASROptions);
        //Run the first set with the starting settings
        $("table#tblSearchResults").FASR("GetResults", OptionsForFirstRun);
    }
</script>
<div class="buttonview viewshows" id="tableDiv" style="cursor:default;">
    <a class="table-map viewshows" id="showtableview" style="cursor:default;">
        Table View
    </a>
</div>
<div class="buttonview" id="mapDiv">
    <a class="table-map" id="showmapview">
        Map View
    </a>
</div>
<br class="clearboth" />
<div id="tableview" class="showview">
    <div class="rbHeaders">
        <span class="displayinline smaller"><b>Sort by:</b>
            <span class="PRAC">
                <input type="radio" name="sort" value="PracticeName" class="sortHeader" sortCol="PracticeName" />
                Name of Facility
            </span>
            <span class="DIST">
                <input type="radio" name="sort" value="distance" class="sortHeader" sortCol="Distance" checked="checked" />
                Distance
            </span>
            <span class="EC">
                <input type="radio" name="sort" value="RangeMin" class="sortHeader" sortCol="TotalCost" />
                Total Estimated Cost
            </span>
            <span class="YC" style="display:none;">
                <input type="radio" name="sort" value="your cost" class="sortHeader" sortCol="YourCost" />
                Your Estimated Cost
            </span>
            <span class="FP">
                <input type="radio" name="sort" value="Fair Price" class="sortHeader" sortCol="FairPrice" />
                Fair Price
            </span>
        </span>
    </div>
    <div class="ycToggle">
        <b style="vertical-align:middle;">Your Estimated Cost:&nbsp;</b>
        <asp:Image runat="server" ID="imgOff" AlternateText="" CssClass="YC" ImageUrl="~/Images/toggle_off.png" Width="52px" Height="20px"
            style="cursor:pointer;vertical-align:middle;width:52px;height:20px;" />
        <asp:Image runat="server" ID="imgOn" AlternateText="" CssClass="YC" ImageUrl="~/Images/toggle_on.png" Width="52px" Height="20px"
            style="cursor:pointer;vertical-align:middle;width:52px;height:20px;display:none;" />
    </div>
    <div class="slider">
        <asp:Label AssociatedControlID="sFindAService" runat="server" Text="Distance range: " Visible="false" />
        <cch:Slider ID="sFindAService" runat="server" Min="1" Max="100" Value="25" Width="200px" OnSlideChanged="updateDistance" Visible="false"
            style="display:inline-block;vertical-align:middle;">
            <ScriptTemplate>
                var sliderElms = $("#<%# Container.SliderID %>");
                var OptionsForFirstRun = {
                    FromRow: 0,
                    ToRow: 25,
                    Distance: sliderElms[0].value,
                    finished: false
                };
                if (typeof (globDefSort) != "undefined") {OptionsForFirstRun.CurrentSort = globDefSort; }
                $("table#tblSearchResults").FASR("GetResults", OptionsForFirstRun, false);
                sliderElms.nextAll("label")[0].innerHTML = " " + sliderElms[0].value + " miles";
            </ScriptTemplate>
        </cch:Slider>
        <asp:Label ID="lblSliderValue" runat="server" AssociatedControlID="sFindAService" Text=" 25 miles" Visible="false" />
    </div>
    <table id="Headers" cellspacing="0" cellpadding="4" border="0" class="searchResults" width="807px">
        <tbody>
            <tr>
                <td class="tdfirst PRAC" sort="PracticeName" style="width:323px;vertical-align:bottom;text-align:left;">
                    <a style="cursor:pointer;">Name Of Facility</a>
                </td>
                <td class="DIST" sort="Distance" style="width:97px;vertical-align:bottom;text-align:center;">
                    <a style="cursor:pointer;" class="sortAsc">Distance</a>
                    <cch:LearnMore ID="lmDist" runat="server">
                        <TitleTemplate>
                            Distance
                        </TitleTemplate>
                        <TextTemplate>
                            Estimated distance (miles) from patient location to this facility.
                        </TextTemplate>
                    </cch:LearnMore>
                </td>
                <td class="EC" sort="TotalCost" style="width:162px;vertical-align:bottom;text-align:center;">
                    <a style="cursor:pointer;">Total Estimated Cost</a>
                    <cch:LearnMore ID="lmEC" runat="server">
                        <TitleTemplate>
                            Total Estimated Cost
                        </TitleTemplate>
                        <TextTemplate>
                            Expected total price for this service or drug, based on recent payments made to this 
                            provider from your health plan.  How much of this total price is paid by you will vary 
                            based on your type of health plan and what other medical expenses you have had this year.
                            Because prices can change and the exact services and drug you receive can vary, it is 
                            possible that the total price will fall outside of the range presented here.  Estimated 
                            out-of-pocked amounts reflected in "Your Cost" will either need to be paid by you directly 
                            or withdrawn from your HSA account, if applicable.  It is also possible that the total 
                            price reflected here may be billed to you on more than one bill (e.g., you may be billed 
                            separately by a hospital where a procedure is performed and by the doctor who performs 
                            the procedure).
                        </TextTemplate>
                    </cch:LearnMore>
                </td>
                <td class="YC" sort="YourCost" style="width:0px;vertical-align:bottom;display:none;text-align:center;">
                    <a style="cursor:pointer;">Your Estimated Cost</a>
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
                            Estimated out-of-pocked amounts reflected in "Your Cost" will either need to be paid by you 
                            directly or withdrawn from your HSA account, if applicable It is also possible that the 
                            total amount reflected here that you pay out-of-pocket may be billed to you on more than one 
                            bill (e.g., you may be billed separately by a hospital where a procedure is performed and 
                            by the doctor who performs the procedure).
                        </TextTemplate>
                    </cch:LearnMore>
                </td>
                <td class="FP" sort="FairPrice" style="width:88px;vertical-align:bottom;text-align:center;">
                    <a style="cursor:pointer;">Fair Price</a>
                    <cch:LearnMore ID="lmFP" runat="server">
                        <TitleTemplate>
                            Fair Price
                        </TitleTemplate>
                        <TextTemplate>
                            Fair Price is checked if a provider is willing to display prices and if a provider's estimated price 
                            for the service is less than 150% of what the federal Medicare program pays for comparable services.
                        </TextTemplate>
                    </cch:LearnMore>
                </td>
                <td class="HG" sort="Healthgrades" style="width:137px;vertical-align:bottom;text-align:center;">
                    <a style="cursor:pointer;color:Black;">Healthgrades&trade; Recognized Physician</a>
                    <cch:LearnMore ID="lmHG" runat="server">
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
            </tr>
        </tbody>
    </table>
    <div>
        <fieldset id="fsPreferred">
            <legend>Caesars Preferred Providers</legend>
            <div id="dPreferredProviders">
                <table id="tblPreferredResults" cellspacing="0" cellpadding="4" border="0" class="searchResults" style="width:807px;">
                </table>
            </div>
        </fieldset>
        <asp:Literal ID="ltlSpinLoader" runat="server" Text="" />
        <asp:Panel ID="pnlServices" runat="server" ClientIDMode="Static">
            <table id="tblSearchResults" cellspacing="0" cellpadding="4" border="0" class="searchResults" style="width:807px;">        
            </table>
        </asp:Panel>
    </div>
</div>
<div id="mapview" class="hideview">
    <p class="smaller">
        Click on a facility to see details.
    </p>
    <div style="position:relative;">
        <div id="resultmap" style="width:840px;height:500px;overflow:hidden;padding:0px;
            margin:0px;position:relative;background-color:rgb(229,227,223);">            
        </div>
        <div id="legend">
            <p class="smaller" style="line-height:35px;">
                <span class="legendpin">Patient Location</span>
                <span class="legendfp">Fair Price Provider</span>
                <span class="legendp">Provider</span>
            </p>
        </div>
    </div>
</div>
<script type="text/javascript">
    var mapRegResults = [];  //  lam, 20130725, MSF-395
    var mapPrefResults = [];  //  lam, 20130725, MSF-395
    $(document).ready(StartFindAServiceResults);
</script>