<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Results.ascx.cs" Inherits="ClearCostWeb.Controls.Results" %>
<%@ Register Src="~/Controls/LearnMore.ascx" TagPrefix="cch" TagName="LearnMore" %>
<style type="text/css">
    div#pnlServices
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
    table#SearchResultsTbl
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
</style>
<asp:Literal ID="ltlCallbackScript" runat="server" />
<script type="text/javascript">
    var lastResult = false;
    var curSort = "Distance";
    var curDir = "ASC";
    $(document).ready(function () {
        $("#pnlServices").scroll(function () {
            if (!lastResult) {
                var dvResultsOffTop = $("#pnlServices").offset().top;
                var dvResultsScrollTop = $("#pnlServices").scrollTop();
                var dvResultsHeight = $("#pnlServices").height();
                var tblResultsHeight = $("#SearchResultsTbl").height();

                if (Math.abs((tblResultsHeight - dvResultsHeight - dvResultsOffTop) - (dvResultsScrollTop - dvResultsOffTop)) < 2) {
                    $("img.loadingSpinner").show();
                    var actionRequest = "{\"ToDo\": \"GetNextResults\", \"ResCount\": \"" + pageResults.length + "\", ";
                    actionRequest += "\"Sort\": \"" + curSort + "\", ";
                    actionRequest += "\"Direction\": \"" + curDir + "\",";
                    actionRequest += "\"EndOfResults\": \"" + lastResult + "\",";
                    actionRequest += "\"Lat\": \"" + $("span.patientAddress").attr("Lat") + "\", \"Lng\": \"" + $("span.patientAddress").attr("Lng") + "\",";
                    actionRequest += "\"showYC\": \"" + showingYC.toString() + "\"}";
                    CallServer(actionRequest, this);
                }
            }
        });
        $("#Headers").contents("tbody").contents("tr").contents("td").contents("a").click(function () {
            if ($(this).parent("td").attr("class") != "HG" && $(this).parent("td").attr("class") != "FP") {
                if ($(this).hasClass("sortAsc")) {
                    $(this).removeClass("sortAsc");
                    $(this).addClass("sortDesc");
                } else if ($(this).hasClass("sortDesc")) {
                    $(this).removeClass("sortDesc");
                    $(this).addClass("sortAsc");
                } else {
                    $("a.sortAsc").attr("class", "");
                    $("a.sortDesc").attr("class", "");
                    $(this).addClass("sortAsc");
                }
                $(this).css("background-position-y", ($(this).height() - 10) + "px");

                curSort = $(this).parent("td").attr("sort");
                curDir = ($(this).hasClass("sortAsc") ? "ASC" : "DESC");

                $(".rbHeaders input[checked='checked']").attr("checked", "");
                $(".rbHeaders input[sort='" + curSort + "']").attr("checked", "checked");

                $("img.loadingSpinner").show();
                $("#SearchResultsTbl").html("");

                pageResults = [];
                resultsToMap = [];

                var actionRequest = "{\"ToDo\": \"ChangeSort\", \"ResCount\": \"0\", \"Sort\": \"";
                actionRequest += curSort + "\", \"Direction\": \"";
                actionRequest += curDir + "\", \"EndOfResults\": \"";
                actionRequest += lastResult + "\",";
                actionRequest += "\"Lat\": \"" + $("span.patientAddress").attr("Lat") + "\", \"Lng\": \"" + $("span.patientAddress").attr("Lng") + "\",";
                actionRequest += "\"showYC\": \"" + showingYC.toString() + "\"}";
                CallServer(actionRequest, this);
            }
        });
        $(".rbHeaders input").click(function () {
            $(".rbHeaders input[checked='checked']").attr("checked", "");
            $(this).attr("checked", "checked");
            $("a.sortAsc").removeClass("sortAsc");
            $("a.sortDesc").removeClass("sortDesc");

            curSort = $(this).attr("sortCol")
            curDir = "ASC";

            $("#Headers td[sort='" + curSort + "']").children("a").addClass("sortAsc");
            $("#Headers td[sort='" + curSort + "']").children("a").css("background-position-y", ($(this).height() - 10) + "px");

            $("img.loadingSpinner").show();
            $("#SearchResultsTbl").html("");

            pageResults = [];
            resultsToMap = [];

            var actionRequest = "{\"ToDo\": \"ChangeSort\", \"ResCount\": \"0\", \"Sort\": \"";
            actionRequest += curSort + "\", \"Direction\": \"";
            actionRequest += curDir + "\", \"EndOfResults\": \"";
            actionRequest += lastResult + "\",";
            actionRequest += "\"Lat\": \"" + $("span.patientAddress").attr("Lat") + "\", \"Lng\": \"" + $("span.patientAddress").attr("Lng") + "\",";
            actionRequest += "\"showYC\": \"" + showingYC.toString() + "\"}";
            CallServer(actionRequest, this);
        });
        $("a.sortAsc").css("background-position-y", "5px");
        $("img.loadingSpinner").show();

        var actionRequest = "{\"ToDo\": \"GetFirstResults\", \"ResCount\": \"0\",\"Sort\": \"Distance\", \"Direction\":\"ASC\", \"Lat\": \"" + $("span.patientAddress").attr("Lat") + "\", \"Lng\": \"" + $("span.patientAddress").attr("Lng") + "\", \"showYC\": \"false\"}";
        CallServer(actionRequest, this);
    });
</script>
<script type="text/javascript">
    var pageResults = [];
    var resultsToMap = [];
    function ReceiveServerData(rValue) {
        var rResults = jQuery.parseJSON(rValue);
        lastResult = rResults.EndOfResults;
        if (rResults.LearnMore) {
            if (rResults.LearnMore.length == 1) {
                $("#ServiceMoreInfoText").text(rResults.LearnMore[0].toString());
            }
            else {
                for (var i = 0; i < rResults.LearnMore.length; i++) {
                    $("#ServiceMoreInfoText" + i).text(rResults.LearnMore[i].toString());
                }
            }
        }
        $(rResults.Results).each(function () {
            $(this.HTML).appendTo("#SearchResultsTbl");
            pageResults.push(this);
            resultsToMap.push(this);
        });
        $("img.loadingSpinner").hide();
    }
    function SelectResult(id) {
        var resNum = id.replace("Result", "");
        var result = pageResults[resNum - 1];
        __doPostBack("Result", result.Nav);
    }
</script>
<table id="Headers" cellspacing="0" cellpadding="4" border="0" class="searchresults" width="807px">
    <tbody>
        <tr>
            <td class="PRAC" sort="PracticeName" class="tdfirst" style="width:40%; vertical-align:bottom;text-align:left;">
                <a href="javascript:void();">Name of Facility</a>
            </td>
            <td class="DIST" sort="Distance" style="width:15%; vertical-align:bottom;">
                <a href="javascript:void();" class="sortAsc" >Distance</a>
                <cch:LearnMore ID="lmDist" runat="server">
                    <TitleTemplate>
                        Distance
                    </TitleTemplate>
                    <TextTemplate>
                        Estimated distance (miles) from patient location to this facility.
                    </TextTemplate>
                </cch:LearnMore>
            </td>
            <td class="EC" sort="TotalCost" style="width:15%; vertical-align:bottom;">
                <a href="javascript:void();">Total Estimated Cost</a>
                <cch:LearnMore ID="lmEC" runat="server">
                    <TitleTemplate>
                        Total Estimated Cost
                    </TitleTemplate>
                    <TextTemplate>
                        Expected total price for this service or drug, based on recent payments made to this 
                        provider from your health plan.  How much of this total price is paid by you will vary 
                        based on your type of health plan and what other medical expenses you have had this year.
                        Because prices can change and the exact services and drug you receive can vary, it is 
                        possible that the total price will fall outside of the range presented here.
                    </TextTemplate>
                </cch:LearnMore>
            </td>
            <td class="YC" sort="YourCost" style="width:0%; vertical-align:bottom;display:none;">
                <a href="javascript:void();">Your Estimated Cost</a>
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
            <td class="FP" sort="FairPrice" style="width:10%; vertical-align:bottom;">
                <a style="cursor: default;color:Black;">Fair Price</a>
                <cch:LearnMore ID="lmFP" runat="server">
                    <TitleTemplate>
                        Fair Price
                    </TitleTemplate>
                    <TextTemplate>
                        Fair Price column is checked if a provider's highest estimated cost to patients for the 
                        service is less than 150% of what the federal Medicare program pays for comparable services.
                    </TextTemplate>
                </cch:LearnMore>
            </td>
            <td class="HG" sort="Healthgrades" style="width:18%; vertical-align:bottom;">
                <a style="cursor: default;color:Black;">Healthgrades&trade; Recognized Physician</a>
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
    <img class="loadingSpinner" src="../Images/ajax-loader-AltCircle.gif" alt="" />
    <div id="pnlServices" style="position:relative;">        
        <table id="SearchResultsTbl" cellspacing="0" cellpadding="4" border="0" class="searchresults">    
        </table>
    </div>
</div>
