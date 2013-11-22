/// <reference path="jquery-1.5.1-vsdoc.js" />
var firstmapview = true;
var map;
var script;
var infoWindow;
var pageResults = [];
var resultsToMap = [];
var gmarkers = [];
var showingYC = YourCostDefault;

Array.prototype.SortByDistance = (function (direction) {
    /// <summary>Sorts the given array of results by the Distance field, then the PharmacyName Field</summary>
    /// <param name="direction">The direction to sort; either ASC or DESC</param>
    /// <returns type="Array">The array sorted by Distance *direction*, PharmacyName ASC</returns>
    this.sort(function (a, b) {
        var res;
        if (direction == "ASC")
            res = (a.Distance.replace(" mi", "")) - (b.Distance.replace(" mi", "")); //Primary sort by Distance Ascending
        else if (direction == "DESC")
            res = (b.Distance.replace(" mi", "")) - (a.Distance.replace(" mi", "")); //Primary sort by Distance Descending
        else
            res = (a.Distance.replace(" mi", "")) - (b.Distance.replace(" mi", "")); //Primary sort by Distance Ascending DEFAULT

        if (res == 0) { //If the distances match
            //Secondary Sort by Pharmacy Name alphabetically
            res = (a.PharmacyName.toLowerCase() == b.PharmacyName.toLowerCase() ? 0 : (a.PharmacyName.toLowerCase() > b.PharmacyName.toLowerCase() ? 1 : -1));
        }
        return res;
    });
});
Array.prototype.SortByPrice = (function (direction) {
    /// <summary>Sorts the given array by the Price field, then the Distance, then the Pharmacy Name</summary>
    /// <param name="direction">The direction to sort; either ASC or DESC</param>
    /// <returns type="Array">The array sorted by Price *direction*, Distance ASC, Pharmacy ASC</returns>
    this.sort(function (a, b) {
        var res;
        if (direction == "ASC")
            res = (a.Price.replace("$", "").replace(",", "")) - (b.Price.replace("$", "").replace(",", "")); //Primary sort by Price Ascending
        else if (direction == "DESC")
            res = (b.Price.replace("$", "").replace(",", "")) - (a.Price.replace("$", "").replace(",", "")); //Primary sort by Price Descending
        else
            res = (a.Price.replace("$", "").replace(",", "")) - (b.Price.replace("$", "").replace(",", "")); //Primary sort by Price Ascending

        if (res == 0) { //If the Prices match
            //Secondary sort by Distance Ascending
            res = (a.Distance.replace(" mi", "")) - (b.Distance.replace(" mi", ""));
            if (res == 0) { //If both Prices and Distances match
                //Sort by pharmacy name Ascending
                res = (a.PharmacyName.toLowerCase() == b.PharmacyName.toLowerCase() ? 0 : (a.PharmacyName.toLowerCase() > b.PharmacyName.toLowerCase() ? 1 : -1));
            }
        }
        return res;
    });
});
Array.prototype.SortByYourCost = (function (direction) {
    /// <summary>Sorts the given array by the Your Cost, then the Distance, then the Pharmacy Name</summary>
    /// <param name="direction">The direction to sort; either ASC or DESC</param>
    /// <returns type="Array">The array sorted by Your Cost *direction*, Distance ASC, Parmacy ASC</returns>
    this.sort(function (a, b) {
        var res;
        if (direction == "ASC")
            res = (a.YourCost.replace("$", "").replace(",", "")) - (b.YourCost.replace("$", "").replace(",", "")); //Primary sort by Your Cost Ascending
        else if (direction == "DESC")
            res = (b.YourCost.replace("$", "").replace(",", "")) - (a.YourCost.replace("$", "").replace(",", "")); //Primary sort by Your Cost Descending
        else
            res = (a.YourCost.replace("$", "").replace(",", "")) - (b.YourCost.replace("$", "").replace(",", "")); //Primary sort by Your Cost Ascending

        if (res == 0) { //If the Your Costs match
            //Secondary sort by Distance Ascending
            res = (a.Distance.replace(" mi", "")) - (b.Distance.replace(" mi", ""));
            if (res == 0) { //If both Your Costs and Distances match
                //Sort by pharmacy name Ascending
                res = (a.PharmacyName.toLowerCase() == b.PharmacyName.toLowerCase() ? 0 : (a.PharmacyName.toLowerCase() > b.PharmacyName.toLowerCase() ? 1 : -1));
            }
        }
        return res;
    });
});
Array.prototype.SortByPharmacy = (function (direction) {
    /// <summary>Sorts the given array by the Pharmacy Name first and the Distance field</summary>
    /// <param name="direction">The direction to sort; either ASC or DESC</param>
    /// <returns type="Array">The array sorted by Pharmacy Name *direction*, Distance ASC</returns>
    this.sort(function (a, b) {
        var res;
        if (direction == "ASC")
            res = (a.PharmacyName.toLowerCase() == b.PharmacyName.toLowerCase() ? 0 : (a.PharmacyName.toLowerCase() > b.PharmacyName.toLowerCase() ? 1 : -1)); //Primary sort by Pharmacy Name
        else if (direction == "DESC")
            res = (b.PharmacyName.toLowerCase() == a.PharmacyName.toLowerCase() ? 0 : (b.PharmacyName.toLowerCase() > a.PharmacyName.toLowerCase() ? 1 : -1)); //Primary sort by Pharmacy Name
        else
            res = (a.PharmacyName.toLowerCase() == b.PharmacyName.toLowerCase() ? 0 : (a.PharmacyName.toLowerCase() > b.PharmacyName.toLowerCase() ? 1 : -1)); //Primary sort by Pharmacy Name

        if (res == 0) { //If the pharmacy names match
            //Secondary sort by Distance Ascending
            res = (a.Distance.replace(" mi", "")) - (b.Distance.replace(" mi", ""));
        }
        return res;
    });
});
$(document).ready(function () {
    $('a.table-map').click(function () {
        if ($(this).attr('id') == 'showmapview') {

            $('#tableview').removeClass('showview');
            $('#tableview').addClass('hideview');

            $('#showtableview').removeClass('viewshows');
            $('#showmapview').addClass('viewshows');

            $('#showtableview').closest('div.buttonview').removeClass('viewshows');
            $('#showmapview').closest('div.buttonview').addClass('viewshows');

            $('#mapview').removeClass('hideview');
            $('#mapview').addClass('showview');

            $("#showtableview").css("cursor", "pointer").parent("div.buttonview").css("cursor", "pointer");
            $("#showmapview").css("cursor", "default").parent("div.buttonview").css("cursor", "default");

            if (firstmapview) makeMap();
            firstmapview = false;
        }
        if ($(this).attr('id') == 'showtableview') {
            $('#mapview').removeClass('showview');
            $('#mapview').addClass('hideview');

            $('#showmapview').removeClass('viewshows');
            $('#showtableview').addClass('viewshows');

            $('#showtableview').closest('div.buttonview').addClass('viewshows');
            $('#showmapview').closest('div.buttonview').removeClass('viewshows');

            $('#tableview').removeClass('hideview');
            $('#tableview').addClass('showview');

            $("#showmapview").css("cursor", "pointer").parent("div.buttonview").css("cursor", "pointer");
            $("#showtableview").css("cursor", "default").parent("div.buttonview").css("cursor", "default");
        }
        $("a.table-map").attr("style", "").parent("div").attr("style", "");
    });
    $("input.sortHeader").change(function () {
        $(".sortAsc").removeClass("sortAsc");
        $(".sortDesc").removeClass("sortDesc");
        switch ($(this).attr("sortCol")) {
            case "Distance":
                $("a.sortHeader[sortCol='Distance']").addClass("sortAsc");
                pageResults.SortByDistance("ASC");
                break;
            case "TotalCost":
                $("a.sortHeader[sortCol='TotalCost']").addClass("sortAsc");
                pageResults.SortByPrice("ASC");
                break;
            case "YourCost":
                $("a.sortHeader[sortCol='YourCost']").addClass("sortAsc");
                pageResults.SortByYourCost("ASC");
                break;
            case "PharmacyName":
                $("a.sortHeader[sortCol='PharmacyName']").addClass("sortAsc");
                pageResults.SortByPharmacy("ASC");
                break;
            default:
                break;
        }
        $("table#tblSearchResults").html("");
        makeTable();
    });
    $("a.sortHeader").click(function () {
        if ($(this).hasClass("sortAsc")) {
            $(this).removeClass("sortAsc").addClass("sortDesc");
            if ($(this).attr("sortCol") == "Distance") {
                pageResults.SortByDistance("DESC");
            }
            else if ($(this).attr("sortCol") == "TotalCost") {
                pageResults.SortByPrice("DESC");
            }
            else if ($(this).attr("sortCol") == "YourCost") {
                pageResults.SortByYourCost("DESC");
            }
            else if ($(this).attr("sortCol") == "PharmacyName") {
                pageResults.SortByPharmacy("DESC");
            }
        }
        else if ($(this).hasClass("sortDesc")) {
            $(this).removeClass("sortDesc").addClass("sortAsc");
            if ($(this).attr("sortCol") == "Distance") {
                pageResults.SortByDistance("ASC");
            }
            else if ($(this).attr("sortCol") == "TotalCost") {
                pageResults.SortByPrice("ASC");
            }
            else if ($(this).attr("sortCol") == "YourCost") {
                pageResults.SortByYourCost("ASC");
            }
            else if ($(this).attr("sortCol") == "PharmacyName") {
                pageResults.SortByPharmacy("ASC");
            }
        }
        else {
            $("input.sortHeader").attr("checked", "");
            $(".sortAsc").removeClass("sortAsc");
            $(".sortDesc").removeClass("sortDesc");
            $(this).addClass("sortAsc");
            if ($(this).attr("sortCol") == "Distance") {
                $("input.sortHeader[sortCol='Distance']").attr("checked", "true");
                pageResults.SortByDistance("ASC");
            }
            else if ($(this).attr("sortCol") == "TotalCost") {
                $("input.sortHeader[sortCol='TotalCost']").attr("checked", "true");
                pageResults.SortByPrice("ASC");
            }
            else if ($(this).attr("sortCol") == "YourCost") {
                $("input.sortHeader[sortCol='YourCost']").attr("checked", "true");
                pageResults.SortByYourCost("ASC");
            }
            else if ($(this).attr("sortCol") == "PharmacyName") {
                $("input.sortHeader[sortCol='PharmacyName']").attr("checked", "true");
                pageResults.SortByPharmacy("ASC");
            }
        }

        $("table#tblSearchResults").html("");
        makeTable();
    });
    if (showingYC) {
        $(".YC").toggle();
        if (showingYC) {
            $("td.PHARM").css("width", "34%");
            $("td.DIST").css("width", "23%");
            $("td.PRICE").css("width", "23%");
            $("td.YC").css("width", "20%");
        } else {
            $("td.PHARM").css("width", "40%");
            $("td.DIST").css("width", "30%");
            $("td.PRICE").css("width", "30%");
            $("td.YC").css("width", "0%");
        }
    }
    $("div.toggle img.YC").click(function () {
        $(".YC").toggle();
        showingYC = !showingYC;
        if (showingYC) {
            $("td.PHARM").css("width", "34%");
            $("td.DIST").css("width", "23%");
            $("td.PRICE").css("width", "23%");
            $("td.YC").css("width", "20%");
        }
        else {
            $("td.PHARM").css("width", "40%");
            $("td.DIST").css("width", "30%");
            $("td.PRICE").css("width", "30%");
            $("td.YC").css("width", "0%");
        }        
    });

    $("img.loadingSpinner").show();
    $("table#tblSearchResults").html("");

    pageResults = [];
    resultsToMap = [];

    $.ajax({
        url: "../SearchInfo/Results_Rx.ashx",
        cache: false,
        data: { Distance: globDistance },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        responseType: "json",
        success: setDistances,
        error: function (result) { $("<tr>").appendTo("table#tblSearchResults"); $("<td>").html("There was an error searching for results").appendTo("table#tblSearchResults tr"); $("img.loadingSpinner").hide(); }
    });
});
function continueDistances() {
    var service = new google.maps.DistanceMatrixService(),
        resultBatch = pageResults.splice(0,25),
        resLeft = pageResults.length;

    var dests = [];
    $(resultBatch).each(function (index, result) {
        if (result.Mail_Retail_ind != "Mail") { dests.push(result.Latitude + "," + result.Longitude); }
        else { dests.push("0,0"); }
    });

    if (dests.length > 0) {
        var dataForGoogle = {
            origins: [$("#PATIENTLATITUDE").val() + "," + $("#PATIENTLONGITUDE").val()],
            destinations: dests,
            travelMode: google.maps.TravelMode.DRIVING,
            unitSystem: google.maps.UnitSystem.IMPERIAL,
            avoidHighways: false,
            avoidTolls: false
        };
        service.getDistanceMatrix(dataForGoogle, function (gBack) {
            var googResults = gBack.rows[0].elements;
            $(googResults).each(function (index, gr) {
                if (gr.status == "OK")
                    resultBatch[index].Distance = gr.distance.text;
            });

            $(resultBatch).each(function (rbIndex, rb) { pageResults.push(rb); });
            resultBatch = pageResults.splice(0, resLeft);
            dests = [];
            $(resultBatch).each(function (index, result) {
                if (result.Mail_Retail_ind != "Mail") { dests.push(result.Latitude + "," + result.Longitude); }
                else { dests.push("0,0"); }
            });
            if (dests.length > 0) {
                dataForGoogle.destinations = dests;
                service.getDistanceMatrix(dataForGoogle, function (gBack2) {
                    var googResults2 = gBack2.rows[0].elements;
                    $(googResults2).each(function (index, gr2) {
                        if (gr2.status == "OK")
                            if ((gr2.distance.text.replace("mi", "") * 1.0) <= globDistance) {
                                resultBatch[index].Distance = gr2.distance.text;
                            }
                    });
                    $(resultBatch).each(function (rbIndex2, rb2) { pageResults.push(rb2); });
                    sortRows(); //pageResults.SortByDistance("ASC");
                    makeTable();
                });
            }
            else {
                sortRows(); //pageResults.SortByDistance("ASC");

                makeTable();
            }
        });
    }
    else { //Either no results or all are mail order
        makeTable();
    }
}
function sortRows() {
    $(".sortAsc").removeClass("sortAsc");
    if (typeof (globDefSort) != "undefined") {
        pageResults.SortByPrice("ASC");
        $("a.sortHeader[sortCol='TotalCost']").addClass("sortAsc");
        $("input.sortHeader[sortCol=Distance]").attr("checked", false);
        $("input.sortHeader[sortCol=TotalCost]").attr("checked", "checked");
    } else {
        pageResults.SortByDistance("ASC");
        $("a.sortHeader[sortCol='Distance']").addClass("sortAsc");
    }
}
function setDistances(resultsBack) {
    pageResults = resultsBack.Results;

    if (!google.maps) {
        google.load("maps", "3.8", {
            "other_params": ((window.location.host.indexOf("localhost") > 0) ? "" : "client=gme-clearcosthealth&") + "sensor=false",
            "callback": continueDistances
        });
    }
    else {
        continueDistances();
    }
}
function makeTable() {    
    var content;
    var infoContent;

    if (pageResults.length > 0) {
        $(pageResults).each(function (index, result) {
            //Setup the result row
            content = "<tr class='graydiv";
            if (index == 0) { content += " graytop"; }
            if ((index % 2) == 0) { content += " roweven"; }
            content += "'>";

            //Setup the Pharmacy Name TD
            content += "<td class='graydiv tdfirst PHARM' styl='width:" + (showingYC ? "34" : "40") + "%;'>";
            content += "<a onclick=\"SelectResult('" + result.PharmacyID + "','" + result.PharmacyLocationID + "');\" class=\"readmore\" style=\"cursor:pointer;\">" + result.PharmacyName + "</a>";
            if (result.CurrentPharmText != "") {
                content += "<b> (Current Pharmacy)</b>";
            }
            if (result.BestPriceText != "") {
                content += "<b> (Best Price)</b>";
            }
            if (result.Address1 != "(Mail Order)") {
                content += "<br /><span id='ResultCity" + index + "' style='margin-left: 19px;'>" + result.Address1 + ", " + result.City + "</span>";
            }
            else {
                content += "<br /><span id='ResultsCity" + index + "' style='margin-left:19px;'>(Mail Order)</span>";
            }
            content += "</td>";

            //Setup the Distance TD
            switch (result.Mail_Retail_ind) {
                case "Retail":
                    content += "<td class='graydiv DIST' style='width:" + (showingYC ? "23" : "30") + "%;'>";
                    content += "<div style='text-align: center;'><span id='ResultDist" + index + "'>" + result.Distance + "</span></div>";
                    content += "</td>";
                    break;
                case "Mail":
                    content += "<td class='graydiv DIST' style='width:" + (showingYC ? "23" : "30") + "%;'>";
                    content += "<div style='text-align: center;'><span id='ResultDist" + index + "'>(Mail Order)</span></div>";
                    content += "</td>";
                    break;
                default:
                    content += "<td class='graydiv DIST' style='width:" + (showingYC ? "23" : "30") + "%;'>";
                    content += "<div style='text-align: center;'><span id='ResultDist" + index + "'>" + result.Distance + "</span></div>";
                    content += "</td>";
                    break;
            }

            //Setup the price TD
            content += "<td class='graydiv PRICE' style='width:" + (showingYC ? "23" : "30") + "%;'>";
            content += "<span id='ResultEC" + index + "'>" + result.Price + "</span>";
            content += "</td>";

            //Setup the YourCost TD
            content += "<td class='graydiv YC' style='width:" + (showingYC ? "20" : "0") + "%;display:" + (showingYC ? "table-cell" : "none") + ";'>";
            content += "<span id='ResultYC" + index + "'>" + result.YourCost + "</span>";
            content += "</td>";

            content += "</tr>";

            $(content).appendTo("table#tblSearchResults");

            if (resultsToMap && resultsToMap.length < pageResults.length) {
                if (result.Mail_Retail_ind != "Mail") {
                    infoContent = "<p class='smaller infoWin' style='width:220px;'>";
                    infoContent += "<a onclick=\"SelectResult('" + result.PharmacyID + "','" + result.PharmacyLocationID + "');\" class=\"readmore\" style=\"cursor:pointer;\">" + result.PharmacyName + "</a>";
                    if (result.CurrentPharmText != "") {
                        infoContent += "<br /><b> (Current Pharmacy)</b>";
                    }
                    if (result.BestPriceText != "") {
                        infoContent += "<br /><b> (Best Price)</b>";
                    }
                    infoContent += "<br />Total Estimated Cost: <b>" + result.Price + "</b>";
                    infoContent += "</p>";

                    var rm = {
                        Latitude: result.Latitude,
                        Longitude: result.Longitude,
                        InfoContent: infoContent
                    }
                    resultsToMap.push(rm);
                }
            }
        });
    }
    else {
        content = "<tr class='graydiv graytop'>";

        //Setup the Pharmacy Name TD
        content += "<td class='graydiv tdfirst' styl='width:100%;'>";
        content += "There are no results for the specific drug dose and quantity combination you selected within 50 miles of your location.   Try searching for a different quantity.";
        content += "</td>";

        content += "</tr>";

        $(content).appendTo("table#tblSearchResults");
    }

    $("img.loadingSpinner").hide();
}

function makeMap() {
    if (firstmapview) { proceedWMap(); }
    placeMarkers();
}
function proceedWMap() {
    if (!google.maps) {
        google.load("maps", "3.8", {
            "other_params": "client=gm-clearcosthealth&sensor=false",
            "callback": continueWMap
        });
    }
    else {
        continueWMap();
    }
}
function continueWMap() {
    var patientLat = $("#PATIENTLATITUDE").val();
    var patientLng = $("#PATIENTLONGITUDE").val();
    var patientCenter = new google.maps.LatLng(patientLat, patientLng);

    var myOptions = {
        zoom: 10,
        center: patientCenter,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    var mapCanvas = document.getElementById("resultmap");
    map = new google.maps.Map(mapCanvas, myOptions);

    var centerPin = new google.maps.Marker({ position: patientCenter, map: map, icon: "../Images/icon_map_pin.png" });

    infoWindow = new google.maps.InfoWindow();
    google.maps.event.addListener(map, 'click', function () { infoWindow.close(); });
}

function placeMarkers() {
    var icon = "../Images/icon_map_blue.png";
    var isThin = ($("div.resultsPane").attr("Thin") == "True");
    $(".legendfp").toggleClass("hidden", isThin);
    if (resultsToMap) {
        $(resultsToMap).each(function () {
            var point = new google.maps.LatLng(this.Latitude, this.Longitude);
            var marker;
            marker = createMarker(icon, map, infoWindow, point, this.InfoContent);
        });
        resultsToMap = [];
    }
}
function createMarker(myIcon, myMap, myBubble, latlng, popHtml) {
    var marker = new google.maps.Marker({
        position: latlng,
        map: myMap,
        icon: myIcon
    });
    google.maps.event.addListener(marker, 'click', function () {
        myBubble.setContent(popHtml);
        myBubble.open(myMap, marker);
    });
    gmarkers.push(marker);
}
function SelectResult(PharmID,PharmLocID) {
    __doPostBack("Result", PharmID + "|" + PharmLocID);
}