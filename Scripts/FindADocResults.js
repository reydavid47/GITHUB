(function ($) {
    Array.prototype.SortByDistance = (function (direction) {
        this.sort(function (a, b) {
            var res, aMi, bMi;
            //Get the numeric distance in miles from Google for sorting
            if (a.Distance.indexOf("ft") > -1) { aMi = a.Distance.replace(" ft", "") / 5280; }
            else if (a.Distance.indexOf("mi") > -1) { aMi = a.Distance.replace(" mi", "") * 1; }
            //Get the numeric distance in miles from Google for sorting
            if (b.Distance.indexOf("ft") > -1) { bMi = b.Distance.replace(" ft", "") / 5280; }
            else if (b.Distance.indexOf("mi") > -1) { bMi = b.Distance.replace(" mi", "") * 1; }

            //Set the result depending on the specified direction
            res = (direction == "ASC" ? aMi - bMi : bMi - aMi);

            if (res == 0) { //If the distances match
                //Secondary Sort by Pharmacy Name alphabetically
                res = (a.PracticeName.toLowerCase() == b.PracticeName.toLowerCase() ? 0 : (a.PracticeName.toLowerCase() > b.PracticeName.toLowerCase() ? 1 : -1));
            }
            return res;
        });
    });
    Array.prototype.SortByPractice = (function (direction) {
        switch (direction) {
            case "ASC":
                this.sort(function (a, b) {
                    var aPrac = a.PracticeName.toLowerCase(),
                        bPrac = b.PracticeName.toLowerCase(),
                        dif = (aPrac == bPrac ? 0 : (aPrac > bPrac ? 1 : -1));
                    if (dif === 0) {
                        dif = (a.Distance.replace(" mi", "") - b.Distance.replace(" mi", ""));
                    }
                    return dif;
                });

                break;
            case "DESC":
                this.sort(function (a, b) {
                    var aPrac = a.PracticeName.toLowerCase(),
                        bPrac = b.PracticeName.toLowerCase(),
                        dif = (aPrac == bPrac ? 0 : (aPrac > bPrac ? -1 : 1));
                    if (dif === 0) {
                        dif = (a.Distance.replace(" mi", "") - b.Distance.replace(" mi", ""));
                    }
                    return dif;
                });

                break;
            default:
                this.sort(function (a, b) {
                    var aPrac = a.PracticeName.toLowerCase(),
                        bPrac = b.PracticeName.toLowerCase(),
                        dif = (aPrac == bPrac ? 0 : (aPrac > bPrac ? 1 : -1));
                    if (dif === 0) {
                        dif = (a.Distance.replace(" mi", "") - b.Distance.replace(" mi", ""));
                    }
                    return dif;
                });

                break;
        }
    });
    Array.prototype.SortByFairPrice = (function (direction) {
        switch (direction) {
            case "ASC":
                this.sort(function (a, b) {
                    var aNum = (a.FairPrice ? 0 : 1),
                        bNum = (b.FairPrice ? 0 : 1),
                        dif = aNum - bNum;
                    if (dif === 0) {
                        dif = (a.Distance.replace(" mi", "") - b.Distance.replace(" mi", ""));
                    }
                    return dif;
                });
                break;
            case "DESC":
                this.sort(function (a, b) {
                    var aNum = (a.FairPrice ? 0 : 1),
                        bNum = (b.FairPrice ? 0 : 1),
                        dif = bNum - aNum;
                    if (dif === 0) {
                        dif = (a.Distance.replace(" mi", "") - b.Distance.replace(" mi", ""));
                    }
                    return dif;
                });
                break;
            default:
                this.sort(function (a, b) {
                    var aNum = (a.FairPrice ? 0 : 1),
                        bNum = (b.FairPrice ? 0 : 1),
                        dif = aNum - bNum;
                    if (dif === 0) {
                        dif = (a.Distance.replace(" mi", "") - b.Distance.replace(" mi", ""));
                    }
                    return dif;
                });
                break;
        }
    });
    Array.prototype.SortByTotalCost = (function (direction) {
        switch (direction) {
            case "ASC":
                this.sort(function (a, b) {
                    var aNum = a.RangeMin; //.replace("$", ""),
                    bNum = b.RangeMin; //.replace("$", ""),
                    dif = aNum - bNum;
                    if (dif === 0) { dif = (a.Distance.replace(" mi", "") - b.Distance.replace(" mi", "")); }
                    return dif;
                });
                break;
            case "DESC":
                this.sort(function (a, b) {
                    var aNum = a.RangeMin; //.replace("$", ""),
                    bNum = b.RangeMin; //.replace("$", ""),
                    dif = bNum - aNum;
                    if (dif === 0) { dif = (b.Distance.replace(" mi", "") - a.Distance.replace(" mi", "")); }
                    return dif;
                });
                break;
            default:
                var aNum = a.RangeMin; //.replace("$", ""),
                bNum = b.RangeMin; //.replace("$", ""),
                dif = aNum - bNum;
                if (dif === 0) { dif = (a.Distance.replace(" mi", "") - b.Distance.replace(" mi", "")); }
                return dif;
        }
    });
    var cleanResults = [], rawResults = [], bufferedResults = [], preferredResults = [],
        resultTable = null, headers = null, radioHeaders = null, spinLoader = null, yourCostTog = null, globSettings = null,
        showingYC = false, firstMapView = true, running = false,
        scrollPane = null, rtHeight = 0, ieEightST = 0,
        map, infoWindow,
        SelectResult = function (TableIndex, DocIndex) {
            var selectedResult = cleanResults[TableIndex],
                selectedDoc = selectedResult.Docs[DocIndex],
                theForm = document.forms['form1'];
            if (!theForm) {
                theForm = document.form1;
            }
            if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
                theForm.POSTNAV.value = selectedDoc.Nav;
                theForm.POSTDIST.value = selectedResult.Distance;
                theForm.submit();
            }
        },
        SelectPreferredResult = function (TableIndex, DocIndex) {
            var selectedResult = preferredResults[TableIndex - 1],
                selectedDoc = selectedResult.Docs[DocIndex],
                theForm = document.forms['form1'];
            if (!theForm) {
                theForm = document.form1;
            }
            if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
                theForm.POSTNAV.value = selectedDoc.Nav;
                theForm.POSTDIST.value = selectedResult.Distance;
                theForm.submit();
            }
        },
        methods = {
            init: function (options) {
                if (options) {
                    if (options.SpinLoader) { spinLoader = $(options.SpinLoader); }
                    if (options.ResultTable) { resultTable = $(options.ResultTable); rtHeight = resultTable.height(); }
                    if (options.YourCostOn) {
                        showingYC = options.YourCostOn;
                        if (showingYC) {
                            $(".YC").toggle();
                            if (showingYC) {
                                $("td.PRAC").css("width", "323px");
                                $("td.YC").css("width", "113px");
                                $("td.EC").css("width", "113px");
                                $("td.DIST").css("width", "73px");
                                $("td.FP").css("width", "73px");
                                $("td.HG").css("width", "112px");
                            } else {
                                $("td.PRAC").css("width", "323px");
                                $("td.YC").css("width", "0px");
                                $("td.EC").css("width", "162px");
                                $("td.DIST").css("width", "97px");
                                $("td.FP").css("width", "88px");
                                $("td.HG").css("width", "137px");
                            }
                        }
                    }
                    if (options.ScrollPane) {
                        pane = $(options.ScrollPane);
                        $(options.ScrollPane).scroll(function () {
                            var paneBottom = (pane.first().position().top + pane.first().height()),
                                tableBottom = (resultTable.position().top + resultTable.height());
                            ieEightST = pane.first().scrollTop;
                            if (paneBottom == tableBottom) { methods["GetMoreResults"](globSettings.finished); }
                        });
                    }
                    //                    if (options.HeaderTable) {
                    //                        headers = $(options.HeaderTable).find("td[sort]");
                    //                        $(headers).each(function (index, header) {
                    //                            $(header).children("a").first().click(function () {
                    //                                var dir;
                    //                                //running = false;
                    //                                if (globSettings.CurrentSort === $(this).parent("td").attr("sort")) { //If they clicked the header they are already sorting on
                    //                                    //ChangeDirection
                    //                                    if (globSettings.CurrentDirection === "ASC") {
                    //                                        $(headers).children("a").removeClass("sortAsc"); //Remove any previous sort arrows
                    //                                        dir = "DESC"; //Change the direction in the settings                               
                    //                                        $(this).addClass("sortDesc"); //Add the sortDesc image to the clicked link
                    //                                    } else {
                    //                                        $(headers).children("a").removeClass("sortDesc"); //Remove any previous sort arrows
                    //                                        dir = "ASC"; //Change the direction in the settings
                    //                                        $(this).addClass("sortAsc"); //Add the sortAsc image to the clicked link
                    //                                    }
                    //                                } else {
                    //                                    $(headers).children("a").removeClass("sortAsc").removeClass("sortDesc"); //Remove any previous sort arrows
                    //                                    $(this).addClass("sortAsc");
                    //                                    dir = "ASC";
                    //                                }
                    //                                $(radioHeaders).children("input.sortHeader[checked]").attr("checked", "");
                    //                                $(radioHeaders).children("input.sortHeader[sortCol=" + $(this).parent("td").attr("sort") + "]").attr("checked", "checked");
                    //                                methods["GetResults"]({
                    //                                    CurrentSort: $(this).parent("td").attr("sort"),
                    //                                    CurrentDirection: dir,
                    //                                    FromRow: 0,
                    //                                    ToRow: 25,
                    //                                    Latitude: globSettings.Latitude,
                    //                                    Longitude: globSettings.Longitude
                    //                                });
                    //                            });
                    //                        });
                    //                    }
                    if (options.RadioHeaderDiv) {
                        radioHeaders = $(options.RadioHeaderDiv).find("span span");
                        $(radioHeaders).each(function (index, header) {
                            $(header).children("input.sortHeader").change(function () {
                                $(headers).children("a").removeClass("sortAsc").removeClass("sortDesc");  //Remove any previous sort arrows
                                $(headers).filter("[sort=" + $(this).attr("sortCol") + "]").children("a").first().addClass("sortAsc");
                                methods["GetResults"]({
                                    CurrentSort: $(this).attr("sortCol"),
                                    CurrentDirection: "ASC",
                                    FromRow: 0,
                                    ToRow: 25,
                                    Latitude: globSettings.Latitude,
                                    Longitude: globSettings.Longitude
                                });
                            });
                        });
                    }
                    if (options.YourCostTog) {
                        yourCostTog = $(options.YourCostTog);
                        $(yourCostTog).children("img.YC").click(function () {
                            $(".YC").toggle();
                            showingYC = !showingYC;
                            if (showingYC) {
                                $("td.PRAC").css("width", "323px");
                                $("td.YC").css("width", "113px");
                                $("td.EC").css("width", "113px");
                                $("td.DIST").css("width", "73px");
                                $("td.FP").css("width", "73px");
                                $("td.HG").css("width", "112px");
                            } else {
                                $("td.PRAC").css("width", "323px");
                                $("td.YC").css("width", "0px");
                                $("td.EC").css("width", "162px");
                                $("td.DIST").css("width", "97px");
                                $("td.FP").css("width", "88px");
                                $("td.HG").css("width", "137px");
                            }
                        });
                    }
                    if (!$("a.table-map").data("events")) {
                        $("a.table-map").click(function () {
                            if ($(this).attr("id") === "showmapview") {
                                $("#tableview").removeClass("showview").addClass("hideview");
                                $("#mapview").removeClass("hideview").addClass("showview");
                                $("#showtableview").removeClass("viewshows").css("cursor", "pointer")
                                    .closest("div.buttonview").removeClass("viewshows").css("cursor", "pointer");
                                $("#showmapview").addClass("viewshows").css("cursor", "default")
                                    .closest("div.buttonview").addClass("viewshows").css("cursor", "default");
                                $("#mapview").removeClass("hideview").addClass("showview");
                                methods["ShowMap"]();
                                firstMapView = false;
                            }
                            if ($(this).attr("id") === "showtableview") {
                                $("#mapview").removeClass("showview").addClass("hideview");
                                $("#tableview").removeClass("hideview").addClass("showview");
                                $("#showmapview").removeClass("viewshows").css("cursor", "pointer")
                                    .parent("div.buttonview").removeClass("viewshows").css("cursor", "pointer");
                                $("#showtableview").addClass("viewshows").css("cursor", "default")
                                    .parent("div.buttonview").addClass("viewshows").css("cursor", "default");
                            }
                            $("a.table-map").attr("style", "").parent("div").attr("style", "");
                        });
                    }
                }
                $("a.sortHeader").click(function () {
                    $("img.loadingSpinner").show();
                    if ($(this).hasClass("sortAsc")) {
                        $(this).removeClass("sortAsc").addClass("sortDesc");
                        switch ($(this).attr("sortCol")) {
                            case "NumericDistance":
                                $("a.sortHeader[sortCol='NumericDistance']").addClass("sortDesc");
                                $("input.sortHeader[sortCol='NumericDistance']").attr("checked", "checked");
                                globSettings.CurrentDirection = "DESC";
                                break;
                        }
                    } else if ($(this).hasClass("sortDesc")) {
                        $(this).removeClass("sortDesc").addClass("sortAsc");
                        switch ($(this).attr("sortCol")) {
                            case "NumericDistance":
                                $("a.sortHeader[sortCol='NumericDistance']").addClass("sortAsc");
                                $("input.sortHeader[sortCol='NumericDistance']").attr("checked", "checked");
                                globSettings.CurrentDirection = "ASC";
                                break;
                        }
                    }
                    $("table#tblSearchResults").html("");
                    methods["GetResults"](globSettings);
                });
                $.ajaxSetup({
                    url: "../Handlers/FindADocResults.ashx",
                    cache: false,
                    dataType: "jsonp",
                    type: "POST",
                    success: methods["GotResults"]
                });
                if (cleanResults && cleanResults.length > 0) {
                    return cleanResults;
                } else {
                    return [];
                }
            },
            GetResults: function (options) {
                if (!running) {
                    running = true;
                    resultTable.html("");
                    cleanResults = [];
                    rawResults = [];
                    bufferedResults = [];
                    spinLoader.show();
                    globSettings = options;
                    $.ajax({ data: globSettings });
                }
            },
            GetMoreResults: function (options) {
                if (!running) {
                    running = true;
                    if (!options) {
                        spinLoader.show();
                        globSettings.FromRow = cleanResults.length + bufferedResults.length + 1;
                        globSettings.ToRow = cleanResults.length + bufferedResults.length + 20;
                        $.ajax({ data: globSettings });
                    }
                }
            },
            GotResults: function (data) {
                if (data.LearnMore) { $("#SpecialtyMoreInfo").text(data.LearnMore.LearnMore); }
                if (data.EmptyResults === undefined) {
                    rawResults = data.Results;
                    if (data.PreferredResults != undefined && preferredResults.length == 0) {
                        preferredResults = data.PreferredResults;
                        if (!google.maps) {
                            //                            google.load("maps", "3.9", {
                            //                                "other_params": "client=gme-clearcosthealth&sensor=false",
                            //                                "callback": methods["GeocodePreferredResults"]
                            //                            });
                            google.load("maps", "3.9", {
                                "other_params": "sensor=false",
                                "callback": methods["GeocodePreferredResults"]
                            });
                        } else {
                            methods["GeocodePreferredResults"]();
                        }
                    } else { methods["ProcessRawResults"](); }
                } else {
                    if (cleanResults.length == 0 && bufferedResults.length == 0) {
                        $(data.EmptyResults).appendTo(resultTable);
                        spinLoader.hide();
                    } else {
                        if (bufferedResults.length > 0) {
                            methods["SortResults"]();
                        } else {
                            globSettings.finished = true;
                            spinLoader.hide();
                        }
                    }
                }
            },
            ProcessRawResults: function () {
                if (!google.maps) {
                    //                    google.load("maps", "3.9", {
                    //                        "other_params": "client=gme-clearcosthealth&sensor=false",
                    //                        "callback": methods["GeocodePreferredResults"]
                    //                    });
                    google.load("maps", "3.9", {
                        "other_params": "sensor=false",
                        "callback": methods["GeocodeResults"]
                    });
                } else {
                    methods["GeocodeResults"]();
                }
            },
            GeocodeResults: function () {
                var service = new google.maps.DistanceMatrixService(),
                    dests = [],
                    dataForGoogle,
                    rawResultsLength = rawResults.length;
                for (var rrIndex = 0; rrIndex < rawResultsLength; rrIndex++) {
                    dests.push(rawResults[rrIndex].Latitude + "," + rawResults[rrIndex].Longitude);
                }
                if (dests.length > 0) {
                    dataForGoogle = {
                        origins: [globSettings.Latitude + "," + globSettings.Longitude],
                        destinations: dests,
                        travelMode: google.maps.TravelMode.DRIVING,
                        unitSystem: google.maps.UnitSystem.IMPERIAL,
                        avoidHighways: false,
                        avoidTolls: false
                    };
                    service.getDistanceMatrix(dataForGoogle, function (gBack) {
                        var rws = gBack.rows[0].elements,
                            rawResultsLength = rawResults.length;
                        for (var rwIndex = 0; rwIndex < rawResultsLength; rwIndex++) {
                            if (gBack) {
                                if (rws[rwIndex].status == "OK") {
                                    rawResults[rwIndex].Distance = rws[rwIndex].distance.text;
                                }
                            }
                            bufferedResults.push(rawResults[rwIndex]);
                        }
                        rawResults = [];
                        methods["SortResults"]();
                    });
                }
            },
            GeocodePreferredResults: function () {
                var service = new google.maps.DistanceMatrixService(),
                    dests = [],
                    dataForGoogle,
                    preferredResultsLength = preferredResults.length;
                for (var prIndex = 0; prIndex < preferredResultsLength; prIndex++) {
                    dests.push(preferredResults[prIndex].Latitude + "," + preferredResults[prIndex].Longitude);
                }
                if (dests.length > 0) {
                    dataForGoogle = {
                        origins: [globSettings.Latitude + "," + globSettings.Longitude],
                        destinations: dests,
                        travelMode: google.maps.TravelMode.DRIVING,
                        unitSystem: google.maps.UnitSystem.IMPERIAL,
                        avoidHighways: false,
                        avoidTolls: false
                    };
                    service.getDistanceMatrix(dataForGoogle, function (gBack) {
                        var rws = gBack.rows[0].elements,
                            preferredResultsLength = preferredResults.length;
                        for (var rwIndex = 0; rwIndex < preferredResultsLength; rwIndex++) {
                            if (gBack) {
                                if (rws[rwIndex].status == "OK") {
                                    preferredResults[rwIndex].Distance = rws[rwIndex].distance.text;
                                }
                            }
                        }
                        methods["SortPreferredResults"]();
                    });
                }
            },
            SortResults: function () {
                switch (globSettings.CurrentSort) {
                    case "Distance":
                        bufferedResults.SortByDistance(globSettings.CurrentDirection);
                        break;
                    case "PracticeName":
                        bufferedResults.SortByPractice(globSettings.CurrentDirection);
                        break;
                    case "FairPrice":
                        bufferedResults.SortByFairPrice(globSettings.CurrentDirection);
                        break;
                    case "TotalCost":
                        bufferedResults.SortByTotalCost(globSettings.CurrentDirection);
                        break;
                    default:
                        break;
                }
                methods["DrawTable"]();
                pane[0].scrollTop = ieEightST;
            },
            SortPreferredResults: function () {
                preferredResults.SortByDistance("ASC");
                methods["DrawPreferredTable"]();
            },
            DrawTable: function () {
                var htmlModified = null, fullHtml = "", waItem, tableIndex,
                    workingArray = bufferedResults.splice(0, 20),
                    workingArrayLength = workingArray.length;
                for (var waIndex = 0; waIndex < workingArrayLength; waIndex++) {
                    waItem = workingArray[waIndex];
                    tableIndex = cleanResults.length;
                    var htmlSet = $(waItem.RowHTML),
                        docCount = htmlSet.length,
                        IsSingleDoc = (docCount == 1);
                    if (IsSingleDoc) {
                        htmlModified = $(waItem.RowHTML);
                        htmlModified.attr("tableIndex", tableIndex);
                        if (tableIndex == 0) { htmlModified.addClass("graytop"); }
                        if ((tableIndex % 2) == 0) { htmlModified.addClass("roweven"); }
                        htmlModified.find("td.Dist").find("span").text(waItem.Distance);
                        htmlModified.find("div.result").attr("tableIndex", tableIndex).attr("docIndex", 0);
                        if (showingYC) {
                            htmlModified.find("td.PRAC").css("width", "323px");
                            htmlModified.find("td.YC").css("width", "113px").show();
                            htmlModified.find("td.EC").css("width", "113px");
                            htmlModified.find("td.DIST").css("width", "73px");
                            htmlModified.find("td.FP").css("width", "73px");
                            htmlModified.find("td.HG").css("width", "112px");
                        }
                        fullHtml += htmlModified[0].outerHTML;
                        waItem.Docs[0].docIndex = 0;
                    } else {
                        var pracRow = $(htmlSet.splice(0, 1)[0]);
                        pracRow.attr("tableIndex", tableIndex);
                        if (tableIndex == 0) { pracRow.addClass("graytop"); }
                        if ((tableIndex % 2) == 0) { pracRow.addClass("roweven"); }
                        pracRow.find("td.Dist").find("span").text(waItem.Distance);
                        pracRow.find("div.result").attr("tableIndex", tableIndex);
                        if (showingYC) {
                            pracRow.find("td.PRAC").css("width", "323px");
                            pracRow.find("td.YC").css("width", "113px").show();
                            pracRow.find("td.EC").css("width", "113px");
                            pracRow.find("td.DIST").css("width", "73px");
                            pracRow.find("td.FP").css("width", "73px");
                            pracRow.find("td.HG").css("width", "112px");
                        }
                        fullHtml += pracRow[0].outerHTML;
                        for (var docIndex = 0; docIndex < htmlSet.length; docIndex++) {
                            var workingDoc = $(htmlSet[docIndex]);
                            workingDoc.attr("tableIndex", tableIndex);
                            if ((tableIndex % 2) == 0) { workingDoc.addClass("roweven"); }
                            //workingDoc.find("td.DIST").find("span").text(waItem.Distance);
                            workingDoc.find("div.result").attr("tableIndex", tableIndex).attr("docIndex", docIndex);
                            if (showingYC) {
                                workingDoc.find("td.PRAC").css("width", "323px");
                                workingDoc.find("td.YC").css("width", "113px").show();
                                workingDoc.find("td.EC").css("width", "113px");
                                workingDoc.find("td.DIST").css("width", "73px");
                                workingDoc.find("td.FP").css("width", "73px");
                                workingDoc.find("td.HG").css("width", "112px");
                            }
                            fullHtml += workingDoc[0].outerHTML;
                            waItem.Docs[docIndex].docIndex = docIndex
                        }
                    }
                    cleanResults.push(waItem);
                    cleanResults[tableIndex].tableIndex = tableIndex;
                }
                $(fullHtml).appendTo(resultTable);
                rtHeight = resultTable.height();
                $(resultTable).find(".NameLoc a.navLink").click(function (clickItem) {
                    SelectResult(clickItem.target.attributes["tableIndex"].value * 1, clickItem.target.attributes["docIndex"].value * 1);
                });
                $(resultTable).find("a.AhideDoc").click(function (clickItem) {
                    var clickedLink = $(clickItem.target),
                        parentTr = clickedLink.parent("div").parent("td").parent("tr"),
                        tableIndex = parentTr.attr("tableindex");
                    $(".docRow[tableindex=" + tableIndex + "]").hide();
                    clickedLink.hide();
                    clickedLink.parent("div").find("a.AshowDoc").show();
                    parentTr.find("a.AresHide").hide();
                    parentTr.find("a.AresShow").show();
                });
                $(resultTable).find("a.AshowDoc").click(function (clickItem) {
                    var clickedLink = $(clickItem.target),
                        parentTr = clickedLink.parent("div").parent("td").parent("tr"),
                        tableIndex = parentTr.attr("tableindex");
                    $(".docRow[tableindex=" + tableIndex + "]").show();
                    clickedLink.hide();
                    clickedLink.parent("div").find("a.AhideDoc").show();
                    parentTr.find("a.AresHide").show();
                    parentTr.find("a.AresShow").hide();
                });
                $(resultTable).find("a.AresHide").click(function (clickItem) {
                    var clickedLink = $(clickItem.target),
                        parentTr = clickedLink.parent("div").parent("div").parent("td").parent("tr"),
                        tableIndex = parentTr.attr("tableindex");
                    $(".docRow[tableindex=" + tableIndex + "]").hide();
                    clickedLink.hide();
                    clickedLink.parent("div").find("a.AresShow").show();
                    parentTr.find("a.AhideDoc").hide();
                    parentTr.find("a.AshowDoc").show();
                });
                $(resultTable).find("a.AresShow").click(function (clickItem) {
                    var clickedLink = $(clickItem.target),
                        parentTr = clickedLink.parent("div").parent("div").parent("td").parent("tr"),
                        tableIndex = parentTr.attr("tableindex");
                    $(".docRow[tableindex=" + tableIndex + "]").show();
                    clickedLink.hide();
                    clickedLink.parent("div").find("a.AresHide").show();
                    parentTr.find("a.AhideDoc").show();
                    parentTr.find("a.AshowDoc").hide();
                });
                globSettings.finished = (bufferedResults.length == 0);
                spinLoader.hide();
                running = false;
            },
            DrawPreferredTable: function () {
                var htmlModified = null, fullHtml = "", waItem, tableIndex,
                    workingArray = preferredResults,
                    workingArrayLength = workingArray.length;
                for (var waIndex = 0; waIndex < workingArrayLength; waIndex++) {
                    waItem = workingArray[waIndex];
                    tableIndex = preferredResults.length;
                    var htmlSet = $(waItem.RowHTML),
                        docCount = htmlSet.length,
                        IsSingleDoc = (docCount == 1);
                    if (IsSingleDoc) {
                        htmlModified = $(waItem.RowHTML);
                        htmlModified.attr("tableIndex", tableIndex);
                        if (tableIndex == 0) { htmlModified.addClass("graytop"); }
                        if ((tableIndex % 2) == 0) { htmlModified.addClass("roweven"); }
                        htmlModified.find("td.Dist").find("span").text(waItem.Distance);
                        htmlModified.find("div.result").attr("tableIndex", tableIndex).attr("docIndex", 0);
                        if (showingYC) {
                            htmlModified.find("td.PRAC").css("width", "323px");
                            htmlModified.find("td.YC").css("width", "113px").show();
                            htmlModified.find("td.EC").css("width", "113px");
                            htmlModified.find("td.DIST").css("width", "73px");
                            htmlModified.find("td.FP").css("width", "73px");
                            htmlModified.find("td.HG").css("width", "112px");
                        }
                        fullHtml += htmlModified[0].outerHTML;
                        preferredResults[waIndex].docIndex = 0;
                    } else {
                        var pracRow = $(htmlSet.splice(0, 1)[0]);
                        pracRow.attr("tableIndex", tableIndex);
                        if (tableIndex == 0) { pracRow.addClass("graytop"); }
                        if ((tableIndex % 2) == 0) { pracRow.addClass("roweven"); }
                        pracRow.find("td.Dist").find("span").text(waItem.Distance);
                        pracRow.find("div.result").attr("tableIndex", tableIndex);
                        if (showingYC) {
                            pracRow.find("td.PRAC").css("width", "323px");
                            pracRow.find("td.YC").css("width", "113px").show();
                            pracRow.find("td.EC").css("width", "113px");
                            pracRow.find("td.DIST").css("width", "73px");
                            pracRow.find("td.FP").css("width", "73px");
                            pracRow.find("td.HG").css("width", "112px");
                        }
                        fullHtml += pracRow[0].outerHTML;
                        for (var docIndex = 0; docIndex < htmlSet.length; docIndex++) {
                            var workingDoc = $(htmlSet[docIndex]);
                            workingDoc.attr("tableIndex", tableIndex);
                            if ((tableIndex % 2) == 0) { workingDoc.addClass("roweven"); }
                            //workingDoc.find("td.DIST").find("span").text(waItem.Distance);
                            workingDoc.find("div.result").attr("tableIndex", tableIndex).attr("docIndex", docIndex);
                            if (showingYC) {
                                workingDoc.find("td.PRAC").css("width", "323px");
                                workingDoc.find("td.YC").css("width", "113px").show();
                                workingDoc.find("td.EC").css("width", "113px");
                                workingDoc.find("td.DIST").css("width", "73px");
                                workingDoc.find("td.FP").css("width", "73px");
                                workingDoc.find("td.HG").css("width", "112px");
                            }
                            fullHtml += workingDoc[0].outerHTML;
                            preferredResults[waIndex].docIndex = docIndex;
                        }
                    }
                    preferredResults[waIndex].tableIndex = tableIndex;
                }


                //                $(fullHtml).find(".docRow").hide();
                //                $(fullHtml).find("a.AshowDoc").show();
                //                $(fullHtml).find("a.AhideDoc").hide();
                //                $(fullHtml).find("a.AresShow").show();
                //                $(fullHtml).find("a.AresHide").hide();

                $(fullHtml).appendTo($("#tblPreferredResults"));
                $("#tblPreferredResults").find(".NameLoc a.navLink").click(function (clickItem) {
                    SelectPreferredResult(clickItem.target.attributes["tableIndex"].value * 1, clickItem.target.attributes["docIndex"].value * 1);
                });
                $("#tblPreferredResults").find("a.AhideDoc").click(function (clickItem) {
                    var clickedLink = $(clickItem.target),
                        parentTr = clickedLink.parent("div").parent("td").parent("tr"),
                        tableIndex = parentTr.attr("tableindex");
                    $(".docRow[tableindex=" + tableIndex + "]").hide();
                    clickedLink.hide();
                    clickedLink.parent("div").find("a.AshowDoc").show();
                    parentTr.find("a.AresHide").hide();
                    parentTr.find("a.AresShow").show();
                });
                $("#tblPreferredResults").find("a.AshowDoc").click(function (clickItem) {
                    var clickedLink = $(clickItem.target),
                        parentTr = clickedLink.parent("div").parent("td").parent("tr"),
                        tableIndex = parentTr.attr("tableindex");
                    $(".docRow[tableindex=" + tableIndex + "]").show();
                    clickedLink.hide();
                    clickedLink.parent("div").find("a.AhideDoc").show();
                    parentTr.find("a.AresHide").show();
                    parentTr.find("a.AresShow").hide();
                });
                $("#tblPreferredResults").find("a.AresHide").click(function (clickItem) {
                    var clickedLink = $(clickItem.target),
                        parentTr = clickedLink.parent("div").parent("div").parent("td").parent("tr"),
                        tableIndex = parentTr.attr("tableindex");
                    $(".docRow[tableindex=" + tableIndex + "]").hide();
                    clickedLink.hide();
                    clickedLink.parent("div").find("a.AresShow").show();
                    parentTr.find("a.AhideDoc").hide();
                    parentTr.find("a.AshowDoc").show();
                });
                $("#tblPreferredResults").find("a.AresShow").click(function (clickItem) {
                    var clickedLink = $(clickItem.target),
                        parentTr = clickedLink.parent("div").parent("div").parent("td").parent("tr"),
                        tableIndex = parentTr.attr("tableindex");
                    $(".docRow[tableindex=" + tableIndex + "]").show();
                    clickedLink.hide();
                    clickedLink.parent("div").find("a.AresHide").show();
                    parentTr.find("a.AhideDoc").show();
                    parentTr.find("a.AshowDoc").hide();
                });
                $("#fsPreferred").show();
                methods["ProcessRawResults"]();
            },
            ShowMap: function () {
                if (!google.maps) {
                    //                    google.load("maps", "3.8", {
                    //                        "other_params": "client=gme-clearcosthealth&sensor=false",
                    //                        "callback": methods["drawMap"]
                    //                    });
                    google.load("maps", "3.8", {
                        "other_params": "sensor=false",
                        "callback": methods["drawMap"]
                    });
                } else { methods["drawMap"](); }
            },
            drawMap: function () {
                if (firstMapView) {
                    var patientCenter = new google.maps.LatLng(globSettings.Latitude, globSettings.Longitude),
                        myOptions = {
                            zoom: 10,
                            center: patientCenter,
                            mapTypeId: google.maps.MapTypeId.ROADMAP
                        },
                        mapCanvas = $("#resultmap")[0],
                        centerPin;
                    map = new google.maps.Map(mapCanvas, myOptions);
                    centerPin = new google.maps.Marker({
                        position: patientCenter,
                        map: map,
                        icon: "../Images/icon_map_pin.png"
                    });
                    infoWindow = new google.maps.InfoWindow();
                    google.maps.event.addListener(map, "click", function () { infoWindow.close(); });
                    $(cleanResults).each(function (index, item) {
                        if (!item.marker) {
                            item.point = new google.maps.LatLng(item.Latitude, item.Longitude);
                            item.marker = new google.maps.Marker({ position: item.point, map: map, icon: item.MapMarker });
                            google.maps.event.addListener(item.marker, "click", function () {
                                var oldA = $(item.InfoHTML).find("a.readmore")[0],
                                    newA = $(item.InfoHTML).find("a.readmore").attr("href", "javascript:SelectResult(" + item.tableIndex + "," + item.Docs[item.tableIndex].docIndex + ");")[0];
                                infoWindow.setContent(item.InfoHTML.replace(oldA.outerHTML, newA.outerHTML));
                                infoWindow.open(map, item.marker);
                            });
                        }
                    });
                }
            }
        };

    $.fn.FADR = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist on jQuery.FASR");
        }
    }

    window.SelectResult = SelectResult;
})(jQuery);