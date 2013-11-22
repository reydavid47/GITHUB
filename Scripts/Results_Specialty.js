var firstmapview = true;
var map;
var script;
var infoWindow;
var pageResults = [];
var resultsToMap = [];
var gmarkers = [];
var showingEIOC = false;
var showingYC = false;

function GetURLParameter(param) {
    var searchString = window.location.search.substring(1),
        i, val, params = searchString.split("&");
    for (i = 0; i < params.length; i++) {
        val = params[i].split("=");
        if (val[0] == param) {
            return unescape(val[1]);
        }
    }
    return null;
}
Array.prototype.SortByDistance = (function (direction) {
    switch (direction) {
        case "ASC":
            this.sort(function (a, b) {
                var aMi, bMi;
                if (a.Distance.indexOf("ft") > -1) { aMi = a.Distance.replace(" ft", "") / 5280; }
                else if (a.Distance.indexOf("mi") > -1) { aMi = a.Distance.replace(" mi", "") * 1; }
                if (b.Distance.indexOf("ft") > -1) { bMi = b.Distance.replace(" ft", "") / 5282; }
                else if (b.Distance.indexOf("mi") > -1) { bMi = b.Distance.replace(" mi", "") * 1; }
                return (aMi - bMi);
            });
            if (history.replaceState) {
                history.replaceState(null, "ClearCost Health", window.location.href.replace(window.location.hash, "").replace(window.location.search, "") + "?s=NumericDistance&d=ASC" + window.location.hash);
            }
            break;
        case "DESC":
            this.sort(function (a, b) {
                var aMi, bMi;
                if (a.Distance.indexOf("ft") > -1) { aMi = a.Distance.replace(" ft", "") / 5280; }
                else if (a.Distance.indexOf("mi") > -1) { aMi = a.Distance.replace(" mi", "") * 1; }
                if (b.Distance.indexOf("ft") > -1) { bMi = b.Distance.replace(" ft", "") / 5282; }
                else if (b.Distance.indexOf("mi") > -1) { bMi = b.Distance.replace(" mi", "") * 1; }
                return (bMi - aMi);
            });
            if (history.replaceState) {
                history.replaceState(null, "ClearCost Health", window.location.href.replace(window.location.hash, "").replace(window.location.search, "") + "?s=NumericDistance&d=DESC" + window.location.hash);
            }
            break;
        default:
            this.sort(function (a, b) {
                var aMi, bMi;
                if (a.Distance.indexOf("ft") > -1) { aMi = a.Distance.replace(" ft", "") / 5280; }
                else if (a.Distance.indexOf("mi") > -1) { aMi = a.Distance.replace(" mi", "") * 1; }
                if (b.Distance.indexOf("ft") > -1) { bMi = b.Distance.replace(" ft", "") / 5282; }
                else if (b.Distance.indexOf("mi") > -1) { bMi = b.Distance.replace(" mi", "") * 1; }
                return (aMi - bMi);
            });
            if (history.replaceState) {
                history.replaceState(null, "ClearCost Health", window.location.href.replace(window.location.hash, "").replace(window.location.search, "") + "?s=NumericDistance&d=ASC" + window.location.hash);
            }
            break;
    }
});
Array.prototype.SortByDoc = (function (direction) {
    switch (direction) {
        case "ASC":
            this.sort(function (a, b) {
                var aPrac = a.PracticeName.toLowerCase();
                var bPrac = b.PracticeName.toLowerCase();
                return (aPrac == bPrac ? 0 : (aPrac > bPrac ? 1 : -1));
            });
            
            break;
        case "DESC":
            this.sort(function (a, b) {
                var aPrac = a.PracticeName.toLowerCase();
                var bPrac = b.PracticeName.toLowerCase();
                return (aPrac == bPrac ? 0 : (aPrac > bPrac ? -1 : 1));
            });
            
            break;
        default:
            this.sort(function (a, b) {
                var aPrac = a.PracticeName.toLowerCase();
                var bPrac = b.PracticeName.toLowerCase();
                return (aPrac == bPrac ? 0 : (aPrac > bPrac ? 1 : -1));
            });
           
            break;
    }
});
Array.prototype.SortByEIOC = (function (direction) {
    switch (direction) {
        case "ASC":
            this.sort(function (a, b) {
                var aNum = a.PracticeRangeMin.replace("$", "");
                var bNum = b.PracticeRangeMin.replace("$", "");
                return aNum - bNum;
            });
            break;
        case "DESC":
            this.sort(function (a, b) {
                var aNum = a.PracticeRangeMin.replace("$", "");
                var bNum = b.PracticeRangeMin.replace("$", "");
                return bNum - aNum;
            });
            break;
        default:
            this.sort(function (a, b) {
                var aNum = a.PracticeRangeMin.replace("$", "");
                var bNum = b.PracticeRangeMin.replace("$", "");
                return aNum - bNum;
            });
            break;
    }
});
Array.prototype.SortByYC = (function (direction) {
    switch (direction) {
        case "ASC":
            this.sort(function (a, b) {
                var aNum = a.PracticeYourCostMin.replace("$", "");
                var bNum = b.PracticeYourCostMin.replace("$", "");
                return aNum - bNum;
            });
            break;
        case "DESC":
            this.sort(function (a, b) {
                var aNum = a.PracticeYourCostMin.replace("$", "");
                var bNum = b.PracticeYourCostMin.replace("$", "");
                return bNum - aNum;
            });
            break;
        default:
            this.sort(function (a, b) {
                var aNum = a.PracticeYourCostMin.replace("$", "");
                var bNum = b.PracticeYourCostMin.replace("$", "");
                return aNum - bNum;
            });
            break;
    }
});
Array.prototype.SortByFairPrice = (function (direction) {
    switch (direction) {
        case "ASC":
            this.sort(function (a, b) {
                var aNum = (a.PracticeFairPrice ? 0 : 1),
                    bNum = (b.PracticeFairPrice ? 0 : 1),
                    dif = aNum - bNum;
                if (dif == 0) { dif = (a.Distance.replace(" mi", "") - b.Distance.replace(" mi", "")); }
                return dif;
            });
            if (history.replaceState) {
                history.replaceState(null, "ClearCost Health", window.location.href.replace(window.location.hash, "").replace(window.location.search, "") + "?s=FPDoc&d=ASC" + window.location.hash);
            }
            break;
        case "DESC":
            this.sort(function (a, b) {
                var aNum = (a.PracticeFairPrice ? 0 : 1),
                    bNum = (b.PracticeFairPrice ? 0 : 1),
                    dif = bNum - aNum;
                if (dif == 0) { dif = (b.Distance.replace(" mi", "") - a.Distance.replace(" mi", "")); }
                return dif;
            });
            if(history.replaceState) {
                history.replaceState(null, "ClearCost Health", window.location.href.replace(window.location.hash, "").replace(window.location.search, "") + "?s=FPDoc&d=DESC" + window.location.hash);
            }
            break;
        default:
            this.sort(function (a, b) {
                var aNum = (a.PracticeFairPrice ? 0 : 1),
                    bNum = (b.PracticeFairPrice ? 0 : 1),
                    dif = aNum - bNum;
                if (dif == 0) { dif = (a.Distance.replace(" mi", "") - b.Distance.replace(" mi", "")); }
                return dif;
            });
            if(history.replaceState) {
                history.replaceState(null, "ClearCost Health", window.location.href.replace(window.location.hash, "").replace(window.location.search, "") + "?s=FPDoc&d=ASC" + window.location.hash);
            }
            break;
    }
});
Array.prototype.SortByHG = (function (direction) {
    switch (direction) {
        case "ASC":
            this.sort(function (a, b) {
                var aNum = (a.PracticeHGRecognized ? 0 : 1);
                var bNum = (b.PracticeHGRecognized ? 0 : 1);
                return aNum - bNum;
            });
            break;
        case "DESC":
            this.sort(function (a, b) {
                var aNum = (a.PracticeHGRecognized ? 0 : 1);
                var bNum = (b.PracticeHGRecognized ? 0 : 1);
                return bNum - aNum;
            });
            break;
        default:
            this.sort(function (a, b) {
                var aNum = (a.PracticeHGRecognized ? 0 : 1);
                var bNum = (b.PracticeHGRecognized ? 0 : 1);
                return aNum - bNum;
            });
            break;
    }
});
Array.prototype.SortByRating = (function (direction) {
    switch (direction) {
        case "ASC":
            this.sort(function (a, b) {
                var aNum = a.PracticeAvgRating;
                var bNum = b.PracticeAvgRating;
                return bNum - aNum;
            });
            break;
        case "DESC":
            this.sort(function (a, b) {
                var aNum = a.PracticeAvgRating;
                var bNum = b.PracticeAvgRating;
                return aNum - bNum;
            });
            break;
        default:
            this.sort(function (a, b) {
                var aNum = a.PracticeAvgRating;
                var bNum = b.PracticeAvgRating;
                return bNum - aNum;
            });
            break;
    }
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
        $("img.loadingSpinner").show();
        $(".sortAsc").removeClass("sortAsc");
        $(".sortDesc").removeClass("sortDesc");
        switch ($(this).attr("sortCol")) {
            case "ProviderName":
                $("a.sortHeader[sortCol='ProviderName']").addClass("sortAsc");
                pageResults[0].SortByDoc("ASC");
                break;
            case "NumericDistance":
                $("a.sortHeader[sortCol='NumericDistance']").addClass("sortAsc");
                pageResults[0].SortByDistance("ASC");
                break;
            case "EIOVC":
                $("a.sortHeader[sortCol='EIOVC']").addClass("sortAsc");
                pageResults[0].SortByEIOC("ASC");
                break;
            case "YC":
                $("a.sortHeader[sortCol='YC']").addClass("sortAsc");
                pageResults[0].SortByYC("ASC");
                break;
            case "FPDoc":
                $("a.sortHeader[sortCol='FPDoc']").addClass("sortAsc");
                pageResults[0].SortByFairPrice("ASC");
                break;
            case "Healthgrades":
                $("a.sortHeader[sortCol='Healthgrades']").addClass("sortAsc");
                pageResults[0].SortByHG("ASC");
                break;
            case "HGOverallRating":
                $("a.sortHeader[sortCol='HGOverallRating']").addClass("sortAsc");
                pageResults[0].SortByRating("ASC");
                break;
            default:
                break;
        }
        $("table#tblSearchResults").html("");
        makeTable();
    });
    $("a.sortHeader").click(function () {
        $("img.loadingSpinner").show();
        if ($(this).hasClass("sortAsc")) {
            $(this).removeClass("sortAsc").addClass("sortDesc");
            switch ($(this).attr("sortCol")) {
                case "ProviderName":
                    $("a.sortHeader[sortCol='ProviderName']").addClass("sortDesc");
                    $("input.sortHeader[sortCol='ProviderName']").attr("checked", "checked");
                    pageResults[0].SortByDoc("DESC");
                    break;
                case "NumericDistance":
                    $("a.sortHeader[sortCol='NumericDistance']").addClass("sortDesc");
                    $("input.sortHeader[sortCol='NumericDistance']").attr("checked", "checked");
                    pageResults[0].SortByDistance("DESC");
                    break;
                case "EIOVC":
                    $("a.sortHeader[sortCol='EIOVC']").addClass("sortDesc");
                    $("input.sortHeader[sortCol='EIOVC']").attr("checked", "checked");
                    pageResults[0].SortByEIOC("DESC");
                    break;
                case "YC":
                    $("a.sortHeader[sortCol='YC']").addClass("sortDesc");
                    $("input.sortHeader[sortCol='YC']").attr("checked", "checked");
                    pageResults[0].SortByYC("DESC");
                    break;
                case "FPDoc":
                    $("a.sortHeader[sortCol='FPDoc']").addClass("sortDesc");
                    $("input.sortHeader[sortCol='FPDoc']").attr("checked", "checked");
                    pageResults[0].SortByFairPrice("DESC");
                    break;
                case "Healthgrades":
                    $("a.sortHeader[sortCol='Healthgrades']").addClass("sortDesc");
                    $("input.sortHeader[sortCol='Healthgrades']").attr("checked", "checked");
                    pageResults[0].SortByHG("DESC");
                    break;
                case "HGOverallRating":
                    $("a.sortHeader[sortCol='HGOverallRating']").addClass("sortDesc");
                    $("input.sortHeader[sortCol='HGOverallRating']").attr("checked", "checked");
                    pageResults[0].SortByRating("DESC");
                    break;
                default:
                    break;
            }
        }
        else if ($(this).hasClass("sortDesc")) {
            $(this).removeClass("sortDesc").addClass("sortAsc");
            switch ($(this).attr("sortCol")) {
                case "ProviderName":
                    $("a.sortHeader[sortCol='ProviderName']").addClass("sortAsc");
                    pageResults[0].SortByDoc("ASC");
                    break;
                case "NumericDistance":
                    $("a.sortHeader[sortCol='NumericDistance']").addClass("sortAsc");
                    pageResults[0].SortByDistance("ASC");
                    break;
                case "EIOVC":
                    $("a.sortHeader[sortCol='EIOVC']").addClass("sortAsc");
                    pageResults[0].SortByEIOC("ASC");
                    break;
                case "YC":
                    $("a.sortHeader[sortCol='YC']").addClass("sortAsc");
                    pageResults[0].SortByYC("ASC");
                    break;
                case "FPDoc":
                    $("a.sortHeader[sortCol='FPDoc']").addClass("sortAsc");
                    pageResults[0].SortByFairPrice("ASC");
                    break;
                case "Healthgrades":
                    $("a.sortHeader[sortCol='Healthgrades']").addClass("sortAsc");
                    pageResults[0].SortByHG("ASC");
                    break;
                case "HGOverallRating":
                    $("a.sortHeader[sortCol='HGOverallRating']").addClass("sortAsc");
                    pageResults[0].SortByRating("ASC");
                    break;
                default:
                    break;
            }
        }
        else {
            $("input.sortHeader").attr("checked", "");
            $(".sortAsc").removeClass("sortAsc");
            $(".sortDesc").removeClass("sortDesc");
            $(this).addClass("sortAsc");
            switch ($(this).attr("sortCol")) {
                case "ProviderName":
                    $("a.sortHeader[sortCol='ProviderName']").addClass("sortAsc");
                    $("input.sortHeader[sortCol='ProviderName']").attr("checked", "checked");
                    pageResults[0].SortByDoc("ASC");
                    break;
                case "NumericDistance":
                    $("a.sortHeader[sortCol='NumericDistance']").addClass("sortAsc");
                    $("input.sortHeader[sortCol='NumericDistance']").attr("checked", "checked");
                    pageResults[0].SortByDistance("ASC");
                    break;
                case "EIOVC":
                    $("a.sortHeader[sortCol='EIOVC']").addClass("sortAsc");
                    $("input.sortHeader[sortCol='EIOVC']").attr("checked", "checked");
                    pageResults[0].SortByEIOC("ASC");
                    break;
                case "YC":
                    $("a.sortHeader[sortCol='YC']").addClass("sortAsc");
                    $("input.sortHeader[sortCol='YC']").attr("checked", "checked");
                    pageResults[0].SortByYC("ASC");
                    break;
                case "FPDoc":
                    $("a.sortHeader[sortCol='FPDoc']").addClass("sortAsc");
                    $("input.sortHeader[sortCol='FPDoc']").attr("checked", "checked");
                    pageResults[0].SortByFairPrice("ASC");
                    break;
                case "Healthgrades":
                    $("a.sortHeader[sortCol='Healthgrades']").addClass("sortAsc");
                    $("input.sortHeader[sortCol='Healthgrades']").attr("checked", "checked");
                    pageResults[0].SortByHG("ASC");
                    break;
                case "HGOverallRating":
                    $("a.sortHeader[sortCol='HGOverallRating']").addClass("sortAsc");
                    $("input.sortHeader[sortCol='HGOverallRating']").attr("checked", "checked");
                    pageResults[0].SortByRating("ASC");
                    break;
                default:
                    break;
            }
        }
        $("table#tblSearchResults").html("");
        makeTable();
    });

//    $("div.YCToggle img.YC").click(function () {
//        $(".YC").toggle();
//        showingYC = !showingYC;
//        setTableWidths();
//    });
    $("div.EIOVCToggle img.EIOVC").click(function () {
        $(".EIOVC").toggle();
        showingEIOC = !showingEIOC;
        setTableWidths();
    });

    $("img.loadingSpinner").show();
    $("table#tblSearchResults").html("");


    pageResults = [];
    resultsToMap = [];

    $.ajax({
        url: "../Handlers/FindADocResults.ashx",
        cache: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: { "Lat": $("span.patientAddress").attr("Lat"), "Lng": $("span.patientAddress").attr("Lng") },
        responseType: "json",
        success: setDistances,
        error: function (result) { $("<tr>").appendTo("table#tblSearchResults"); $("<td>").html("There was an error searching for results").appendTo("table#tblSearchResults tr"); $("img.loadingSpinner").hide(); }
    });
});
function setTableWidths() {
    if (showingYC && !showingEIOC) {
        $("td.NameLoc").css("width", "32%");
        $("td.Dist").css("width", "10%");
        $("td.FP").css("width", "12%");
        $("td.HG").css("width", "17%");
        $("td.ratingTD").css("width", "17%");
        $("td.YC").css("width", "12%");
    }
    else if (!showingYC && showingEIOC) {
        $("td.NameLoc").css("width", "32%");
        $("td.Dist").css("width", "10%");
        $("td.FP").css("width", "12%");
        $("td.HG").css("width", "17%");
        $("td.ratingTD").css("width", "17%");
        $("td.EIOVC").css("width", "12%");
    }
    else if (showingYC && showingEIOC) {
        $("td.NameLoc").css("width", "30.4%");
        $("td.Dist").css("width", "8.4%");
        $("td.FP").css("width", "10.4%");
        $("td.HG").css("width", "15.4%");
        $("td.ratingTD").css("width", "15.4%");
        $("td.YC").css("width", "10.4%");
        $("td.EIOVC").css("width", "10.4%");
    }
    else if (!showingYC && !showingEIOC) {
        $("td.NameLoc").css("width", "34.6%");
        $("td.Dist").css("width", "12.6%");
        $("td.FP").css("width", "14.6%");
        $("td.HG").css("width", "21.6%");
        $("td.ratingTD").css("width", "21.6%");
    }
}
function makeMap() {
    if (firstmapview) { proceedWMap(); }
    placeMarkers();
}
function proceedWMap() {
    if (!google.maps) {
        google.load("maps", "3.8", {
            "other_params": ((window.location.host.indexOf("localhost") > 0) ? "" : "client=gme-clearcosthealth&") + "sensor=false",
            "callback": continueWMap
        });
    }
    else {
        continueWMap();
    }
}
function continueWMap() {
    var patientLat = $(".patientAddress").attr("Lat");
    var patientLng = $(".patientAddress").attr("Lng");
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
    var icon_FP = "../Images/icon_map_green.png";
    var icon = "../Images/icon_map_blue.png";
    if (resultsToMap) {
        $(resultsToMap).each(function () {
            var point = new google.maps.LatLng(this.Latitude, this.Longitude);
            var marker;
            if (this.fp) {
                marker = createMarker(icon_FP, map, infoWindow, point, this.InfoContent);
            }
            else {
                marker = createMarker(icon, map, infoWindow, point, this.InfoContent);
            }
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
function makeTable() {
    $(pageResults[0]).each(function (index, result) {
        if (result.ProviderName.length == 1) {
            makeOneDocRow(index, result);
        }
        else if (result.ProviderName.length > 1) {
            makeMultiDocRows(index, result);
        }
        if (resultsToMap && resultsToMap.length < pageResults[0].length) {
            var infoContent = $("<p>").addClass("smaller").addClass("infoWin").css("width", "280px;");
            var innerDiv = $("<div>");

            $("<a>").attr("rindex", index).addClass("readmore").addClass("result").css("cursor", "pointer").attr("subres", "0").bind("click", SelectResult).appendTo(innerDiv);
            $("<div>").html(result.PracticeName).appendTo($(innerDiv).children("a"));
            $("<span>").css("marginLeft", "19px").html(result.LocationAddress1 + ", " + result.LocationCity).appendTo(innerDiv);
            $(innerDiv).appendTo(infoContent);

            innerDiv = $("<div>").addClass("EOIVC").html("<b>Estimated Initial Office Visit Cost:&nbsp;</b>");
            $("<b>").addClass("alignright").html("$" + result.RangeMin[0]).appendTo(innerDiv);
            $("<b>").addClass("dashcol").html("-").appendTo(innerDiv);
            $("<b>").addClass("alignleft").html("$" + result.RangeMax[0]).appendTo(innerDiv);
            $(innerDiv).appendTo(infoContent);

//            innerDiv = $("<div>").addClass("YC").html("<b>Your Estimated Cost:&nbsp;</b>");
//            $("<b>").addClass("alignrignt").html("$" + result.YourCostMin[0]).appendTo(innerDiv);
//            $("<b>").addClass("dashcol").html("-").appendTo(innerDiv);
//            $("<b>").addClass("alignleft").html("$" + result.YourCostMax[0]).appendTo(innerDiv);
//            $(innerDiv).appendTo(infoContent);

            if (result.PracticeFairPrice) {
                innerDiv = $("<div>");
                $("<img />").attr("src", "../Images/check_green.png").attr("alt", "FairPrice Doc?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(innerDiv);
                $(innerDiv).append("FairPrice").appendTo(infoContent);
            }

            if (result.PracticeHGRecognized) {
                innerDiv = $("<div>");
                $("<img />").attr("src", "../Images/check_purple.png").attr("alt", "Healthgrades Recognized?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(innerDiv);
                $(innerDiv).append("Healthgrades").appendTo(infoContent);
            }

            var rm = {
                Latitude: result.Latitude,
                Longitude: result.Longitude,
                fp: result.PracticeFairPrice,
                InfoContent: $(infoContent).html()
            }
            resultsToMap.push(rm);
        }
    });
    $("img.loadingSpinner").hide();
}
function makeOneDocRow(indx, res) {
    var tr, td, inner;
    tr = $("<tr>").addClass("graydiv");
    if (indx == 0) { $(tr).addClass("graytop"); }
    if ((indx % 2) == 0) { $(tr).addClass("roweven"); }

    td = $("<td>").addClass("graydiv").addClass("tdfirst").addClass("NameLoc").css("width", "32%");
    $("<a>").attr("rindex", indx).attr("subres", "0").bind("click", SelectResult).appendTo(td);
    $("<div>").html(res.ProviderName[0]).addClass("readmore").addClass("result").css("cursor", "pointer").appendTo($(td).children("a"));
    $("<span>").css("marginLeft", "19px").html(res.PracticeName).appendTo(td);
    $("<br />").appendTo(td);
    $("<span>").css("marginLeft", "19px").html(res.LocationAddress1 + ", " + res.LocationCity).appendTo(td);
    $(td).appendTo(tr);

    td = $("<td>").addClass("graydiv").addClass("Dist").css({ "whiteSpace": "nowrap", "width": "10%" });
    $("<div>").css("textAlign", "center").appendTo(td);
    $("<span>").html(res.Distance).appendTo($(td).children("div"));
    $(td).appendTo(tr);

    td = $("<td>").addClass("graydiv").addClass("EIOVC").css({ "width": "12%", "display": (showingEIOC ? "table-cell" : "none") });
    $("<div>").appendTo(td);
    $("<b>").addClass("alignright").html("$" + res.RangeMin[0]).appendTo($(td).children("div"));
    $("<b>").addClass("dashcol").html(" - ").appendTo($(td).children("div"));
    $("<b>").addClass("alignleft").html("$" + res.RangeMax[0]).appendTo($(td).children("div"));
    $(td).appendTo(tr);

    td = $("<td>").addClass("graydiv").addClass("YC").css({ "width": "12%", "display": (showingYC ? "table-cell" : "none") });
    $("<div>").appendTo(td);
    $("<b>").addClass("alignright").html("$" + res.YourCostMin[0]).appendTo($(td).children("div"));
    $("<b>").addClass("dashcol").html(" - ").appendTo($(td).children("div"));
    $("<b>").addClass("alignleft").html("$" + res.YourCostMax[0]).appendTo($(td).children("div"));
    $(td).appendTo(tr);

    td = $("<td>").addClass("tdcheck").addClass("graydiv").addClass("FP").css("width", "12%");
    if (res.FairPrice[0]) {
        $("<img />").attr("src", "../Images/check_green.png").attr("alt", "FairPrice Doc?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(td);
    }
    else {
        $("<img />").attr("src", "../Images/s.gif").attr("alt", "FairPrice Doc?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(td);
    }
    $(td).appendTo(tr);

    td = $("<td>").addClass("tdcheck").addClass("graydiv").addClass("HG").css("width", "17%");
    if (res.HGRecognized[0] == 1) {
        $("<img />").attr("src", "../Images/check_purple.png").attr("alt", "Healthgrades Recognized?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(td);
    }
    else if (res.HGRecognized[0] == 0) {
        $("<img />").attr("src", "../Images/s.gif").attr("alt", "FairPrice Doc?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(td);
    }
    else if (res.HGRecognized[0] == -1) {
        $("<span>").html("N/A").appendTo(td);
    }
    $(td).appendTo(tr);

    td = $("<td>").addClass("graydiv").addClass("ratingTD").css("width", "17%");
    inner = $("<div>").addClass("ratingRow");
    if (res.HGOverallRating[0] == -1) {
        for (var i = 1; i <= 5; i++) {
            $("<div>").addClass("star_none").appendTo(inner);
        }
    }
    else {
        for (var i = 1; i <= (res.HGOverallRating[0] - (res.HGOverallRating[0] % 1)); i++) {
            $("<div>").addClass("star_full").appendTo(inner);
        }
        if ((res.HGOverallRating[0] % 1) > 0.0) {
            $("<div>").addClass("star_half").appendTo(inner);
        }
        for (var i = 1; i <= (5 - ((res.HGOverallRating[0] - (res.HGOverallRating[0] % 1)) + ((res.HGOverallRating[0] % 1) > 0.0 ? 1 : 0))); i++) {
            $("<div>").addClass("star_none").appendTo(inner);
        }
    }
    $("<p>").addClass("ratings").appendTo(inner);
    $("<span>").html(res.HGPatientCount[0] + " patient survey" + (res.HGPatientCount[0] == 1 ? "" : "s")).appendTo($(inner).children("p"));
    $(inner).appendTo(td);
    $(td).appendTo(tr);

    $(tr).appendTo("table#tblSearchResults");
}
function makeMultiDocRows(indx, res) {
    var tr, td, inner;
    tr = $("<tr>").addClass("graydiv");
    if (indx == 0) { $(tr).addClass("graytop"); }
    if ((indx % 2) == 0) { $(tr).addClass("roweven"); }

    td = $("<td>").addClass("graydiv").addClass("tdfirst").addClass("NameLoc").css("width", "32%");
    $("<a>").attr("rindex", indx).bind("click", ShowDocs).appendTo(td);
    $("<div>").html(res.PracticeName).css({ "cursor": "default", "color": "black", "marginLeft":"19px" }).addClass("result").appendTo($(td).children("a"));
    inner = $("<span>").css("marginLeft", "19px");
    $("<a>").addClass("AshowDoc").attr("parent", indx).css("display", "none").bind("click", ShowDocs).html("(see " + res.ProviderName.length + " physicians)").appendTo(inner);
    $("<a>").addClass("AhideDoc").attr("parent", indx).css("display", "inline").bind("click", HideDocs).html("(hide physicians)").appendTo(inner);
    $(inner).appendTo(td);
    $("<br />").appendTo(td);
    $("<span>").css("marginLeft", "19px").html(res.LocationAddress1 + ", " + res.LocationCity).appendTo(td);
    $(td).appendTo(tr);

    td = $("<td>").addClass("graydiv").addClass("Dist").css({ "whiteSpace": "nowrap", "width": "10%" });
    $("<div>").css("textAlign", "center").appendTo(td);
    $("<span>").html(res.Distance).appendTo($(td).children("div"));
    $(td).appendTo(tr);

    td = $("<td>").addClass("graydiv").addClass("EIOVC").css({ "width": "12%", "display": (showingEIOC ? "table-cell" : "none") });
    $("<div>").appendTo(td);
    $("<b>").addClass("alignright").html(res.PracticeRangeMin).appendTo($(td).children("div"));
    $("<b>").addClass("dashcol").html(" - ").appendTo($(td).children("div"));
    $("<b>").addClass("alignleft").html(res.PracticeRangeMax).appendTo($(td).children("div"));
    $(td).appendTo(tr);

    td = $("<td>").addClass("graydiv").addClass("YC").css({ "width": "12%", "display": (showingYC? "table-cell" : "none") });
    $("<div>").appendTo(td);
    $("<b>").addClass("alignright").html(res.PracticeYourCostMin).appendTo($(td).children("div"));
    $("<b>").addClass("dashcol").html(" - ").appendTo($(td).children("div"));
    $("<b>").addClass("alignleft").html(res.PracticeYourCostMax).appendTo($(td).children("div"));
    $(td).appendTo(tr);

    td = $("<td>").addClass("tdcheck").addClass("graydiv").addClass("FP").css("width", "12%");
    if (res.PracticeFairPrice) {
        $("<img />").attr("src", "../Images/check_green.png").attr("alt", "FairPrice Doc?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(td);
    }
    else {
        $("<img />").attr("src", "../Images/s.gif").attr("alt", "FairPrice Doc?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(td);
    }
    $(td).appendTo(tr);

    $("<td>").addClass("tdcheck").addClass("graydiv").addClass("HG").css("width", "17%").html(res.PracticeHGRecognized).appendTo(tr);

    td = $("<td>").addClass("graydiv").addClass("ratingTD").css("width", "17%");
    inner = $("<div>");
    $("<a>").addClass("AresShow").attr("parent", indx).css("display", "none").bind("click", ShowDocs).html("Click for Ratings").appendTo(inner);
    $("<a>").addClass("AresHide").attr("parent", indx).css("display", "inline").bind("click", HideDocs).html("Hide Ratings").appendTo(inner);
    $(inner).appendTo(td);
    $(td).appendTo(tr);

    $(tr).appendTo("table#tblSearchResults");

    for (var j = 0; j < res.ProviderName.length; j++) {
        tr = $("<tr>").addClass("docRow").addClass("graydiv").attr("parent", indx).css("display", "table-row");
        if ((indx % 2) == 0) { $(tr).addClass("roweven"); }

        td = $("<td>").addClass("tdfirst").addClass("graydiv").addClass("NameLoc").css("width", "32%");
        $("<a>").attr("rindex", indx).attr("subres", j.toString()).bind("click", SelectResult).css({ "marginLeft": "19px", "display": "inline-block" }).appendTo(td);
        $("<div>").html(res.ProviderName[j]).addClass("readmore").addClass("result").css({ "backgroundPositionY": "center", "marginLeft": "19px" }).appendTo($(td).children("a"));
        $(td).appendTo(tr);

        td = $("<td>").addClass("graydiv").addClass("Dist").css({ "whiteSpace": "nowrap", "width": "10%" });
        $("<div>").css("textAlign", "center").appendTo(td);
        $("<span>").appendTo($(td).children("div"));
        $(td).appendTo(tr);

        td = $("<td>").addClass("graydiv").addClass("EIOVC").css({ "width": "12%", "display": (showingEIOC ? "table-cell" : "none") });
        $("<div>").appendTo(td);
        $("<b>").addClass("alignright").html("$" + res.RangeMin[j]).appendTo($(td).children("div"));
        $("<b>").addClass("dashcol").html(" - ").appendTo($(td).children("div"));
        $("<b>").addClass("alignleft").html("$" + res.RangeMax[j]).appendTo($(td).children("div"));
        $(td).appendTo(tr);

        td = $("<td>").addClass("graydiv").addClass("YC").css({ "width": "12%", "display": (showingYC ? "table-cell" : "none") });
        $("<div>").appendTo(td);
        $("<b>").addClass("alignright").html("$" + res.YourCostMin[j]).appendTo($(td).children("div"));
        $("<b>").addClass("dashcol").html(" - ").appendTo($(td).children("div"));
        $("<b>").addClass("alignleft").html("$" + res.YourCostMax[j]).appendTo($(td).children("div"));
        $(td).appendTo(tr);

        td = $("<td>").addClass("tdcheck").addClass("graydiv").addClass("FP").css("width", "12%");
        if (res.FairPrice[j]) {
            $("<img />").attr("src", "../Images/check_green.png").attr("alt", "FairPrice Doc?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(td);
        }
        else {
            $("<img />").attr("src", "../Images/s.gif").attr("alt", "FairPrice Doc?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(td);
        }
        $(td).appendTo(tr);

        td = $("<td>").addClass("tdcheck").addClass("graydiv").addClass("HG").css("width", "17%");
        if (res.HGRecognized[j] == 1) {
            $("<img />").attr("src", "../Images/check_purple.png").attr("alt", "Healthgrades Recognized?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(td);
        }
        else if (res.HGRecognized[j] == 0) {
            $("<img />").attr("src", "../Images/s.gif").attr("alt", "FairPrice Doc?").css({ "width": "23px", "height": "23px", "borderWidth": "0px" }).appendTo(td);
        }
        else if (res.HGRecognized[j] == -1) {
            $("<span>").html("N/A").appendTo(td);
        }
        $(td).appendTo(tr);

        td = $("<td>").addClass("graydiv").addClass("ratingTD").css("width", "17%");
        inner = $("<div>").addClass("ratingRow");
        if (res.HGOverallRating[j] == -1) {
            for (var i = 1; i <= 5; i++) {
                $("<div>").addClass("star_none").appendTo(inner);
            }
        }
        else {
            for (var i = 1; i <= (res.HGOverallRating[j] - (res.HGOverallRating[j] % 1)); i++) {
                $("<div>").addClass("star_full").appendTo(inner);
            }
            if ((res.HGOverallRating[j] % 1) > 0.0) {
                $("<div>").addClass("star_half").appendTo(inner);
            }
            for (var i = 1; i <= (5 - ((res.HGOverallRating[j] - (res.HGOverallRating[j] % 1)) + ((res.HGOverallRating[j] % 1) > 0.0 ? 1 : 0))); i++) {
                $("<div>").addClass("star_none").appendTo(inner);
            }
        }
        $("<p>").addClass("ratings").appendTo(inner);
        $("<span>").html(res.HGPatientCount[j] + " patient survey" + (res.HGPatientCount[j] == 1 ? "" : "s")).appendTo($(inner).children("p"));
        $(inner).appendTo(td);
        $(td).appendTo(tr);

        $(tr).appendTo("table#tblSearchResults");
    }
}
function setDistances(resultsBack) {
    var results = resultsBack.Results;
    if (pageResults.length == 0 && results.length == 0) {
        $("<tr>").appendTo("table#tblSearchResults");
        $("<td>").html("There were no results found for your search.<br>Please revise your search criterea and try again.").appendTo("table#tblSearchResults tr");
        $("img.loadingSpinner").hide();
    }
    else {
        pageResults.push(results);

        if (!google.maps) {
            google.load("maps", "3.8", {
                "other_params": ((window.location.host.indexOf("localhost") > 0) ? "" : "client=gme-clearcosthealth&") + "sensor=false",
                "callback": continueDistances
            });
        }
        else { continueDistances(); }
    }
}
function continueDistances() {
    var service = new google.maps.DistanceMatrixService(),
        preLen = pageResults[0].length,
        iterations = (preLen / 20), iteration,
        resBatch,
        dataForGoogle = {
            origins: [$("span.patientAddress").attr("Lat") + "," + $("span.patientAddress").attr("Lng")],
            destinations: [],
            travelMode: google.maps.TravelMode.DRIVING,
            unitSystem: google.maps.UnitSystem.IMPERIAL,
            avoidHighways: false,
            avoidTolls: false
        },
        distBatch = function (Results, OriginArray) {
            var gd = {};
            gd.origins = OriginArray;
            gd.travelMode = google.maps.TravelMode.DRIVING;
            gd.unitSystem = google.maps.UnitSystem.IMPERIAL;
            gd.avoidHighways = false;
            gd.avoidTolls = false;
            gd.destinations = [];

            $(Results).each(function (index, result) { gd.destinations.push(result.Latitude + "," + result.Longitude); });

            service.getDistanceMatrix(gd, function (gBack) {
                var googResults;
                if (gBack.rows[0]) {
                    googResults = gBack.rows[0].elements;
                }
                else {
                    googResults = {};
                }
                $(googResults).each(function (index, gr) {
                    if (gr.status == "OK")
                        Results[index].Distance = gr.distance.text;
                    pageResults[0].push(Results[index]);
                });
                if (pageResults[0].length == preLen) { finishDistances(); }
            });
        };
        if (iterations < 1) { iterations = 1; }
        for (iteration = 1; iteration <= iterations; iteration++) {
            dataForGoogle.destinations = [];
            if (pageResults[0].length > 0) {
                distBatch(pageResults[0].splice(0, 20), [$("span.patientAddress").attr("Lat") + "," + $("span.patientAddress").attr("Lng")]);
            }
            else { iteration = iterations + 1; }
        }
}
function finishDistances() {
        var defSort = GetURLParameter("s"),
            defDir = GetURLParameter("d");
        $(".sortAsc").removeClass("sortAsc");
        $(".sortDesc").removeClass("sortDesc");
        if (defDir == "ASC") {
            $("a.sortHeader[sortCol='" + defSort + "']").addClass("sortAsc");
            $("input.sortHeader[sortCol='" + defSort + "']").attr("checked", "checked");
        }
        else if (defDir = "DESC") {
            $("a.sortHeader[sortCol='" + defSort + "']").addClass("sortDesc");
            $("input.sortHeader[sortCol='" + defSort + "']").attr("checked", "checked");
        }
        else {
            $("a.sortHeader[sortCol='NumericDistance']").addClass("sortAsc");
            $("input.sortHeader[sortCol='NumericDistance']").attr("checked", "checked");
        }
        switch (defSort) {
            case "ProviderName":
                pageResults[0].SortByDoc(defDir);
                break;
            case "NumericDistance":
                pageResults[0].SortByDistance(defDir);
                break;
            case "EIOVC":
                pageResults[0].SortByEIOC(defDir);
                break;
            case "FPDoc":
                pageResults[0].SortByFairPrice(defDir);
                break;
            case "Healthgrades":
                pageResults[0].SortByHG(defDir);
                break;
            case "HGOverallRating":
                pageResults[0].SortByRating(defDir);
                break;
            default:
                pageResults[0].SortByDistance("ASC");
                break;
        }

        makeTable();
}
function SelectResult() {
    $("body").css("cursor", "progress");
    var rindex = $(this).attr("rindex");
    var subres = $(this).attr("subres");
    var rslt = pageResults[0][rindex];
    var SelectArgs = rslt.PracticeName + "|" + rslt.ProviderName[subres] + "|" + rslt.NPI[subres] + "|" + rslt.Distance + "|" + rslt.TaxID[subres] + "|" + rslt.OrganizationLocationID;

    __doPostBack("Result", SelectArgs);
}
function ShowDocs() {
    var index = $(this).attr("parent");
    $("tr.docRow[parent=" + index + "]").show();
    $("a.AhideDoc[parent=" + index + "]").show();
    $("a.AresHide[parent=" + index + "]").show();
    $("a.AshowDoc[parent=" + index + "]").hide();
    $("a.AresShow[parent=" + index + "]").hide();
}
function HideDocs() {
    var index = $(this).attr("parent");
    $("tr.docRow[parent=" + index + "]").hide();
    $("a.AhideDoc[parent=" + index + "]").hide();
    $("a.AresHide[parent=" + index + "]").hide();
    $("a.AshowDoc[parent=" + index + "]").show();
    $("a.AresShow[parent=" + index + "]").show();
}