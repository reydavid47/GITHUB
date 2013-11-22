//Added by Blue Note
$(document).click(function () {
	$('div.moreinfo:visible').fadeOut(500);
});

$(document).ready(function () {
    //$("#tabs").tabs();
    //    $("#tabs").tabs({
    //        select: function (event, ui) {
    //            var url = $.data(ui.tab, 'load.tabs');
    //            location.href = "search.aspx" + ui.tab.hash.toString();
    //            return false;
    //        }
    //    });

    $("#searchbydoctor").click(function () {
        $("#doctorsearch").show();
        $("#facilitysearch").hide();
        $("#cptsearch").hide();
        $("a.advsearch").removeClass("callout");
        $("a#searchbydoctor").addClass("callout");
    });
    $("#searchbyfacility").click(function () {
        $("#facilitysearch").show();
        $("#doctorsearch").hide();
        $("#cptsearch").hide();
        $("a.advsearch").removeClass("callout");
        $("a#searchbyfacility").addClass("callout");
    });
    $("#searchbycpt").click(function () {
        $("#cptsearch").show();
        $("#doctorsearch").hide();
        $("#facilitysearch").hide();
        $("a.advsearch").removeClass("callout");
        $("a#searchbycpt").addClass("callout");
    });
    $("#closedoctor").bind('click', revertsearch);
    $("#closefacility").bind('click', revertsearch);
    $("#closecpt").bind('click', revertsearch);

    // open dialog windows following certain searches
    $("#caresearch").click(function () {
        $('<div />').addClass('whitescreen').appendTo('body').show();
        $('div#mrisearch').show();
    });
    $("#caresearch2").click(function () {
        $('<div />').addClass('whitescreen').appendTo('body').show();
        $('div#labsearch').show();
    });
    $("#caresearch3").click(function () {
        $('<div />').addClass('whitescreen').appendTo('body').show();
        $('div#officesearch').show();
    });
    $("#addmed").click(function () {
        //$('<div />').addClass('whitescreen').appendTo('body').show();
        $('div#meduser').show();
    });

    // hide/show the family members' meds on the Find Rx tab
    $('a.member').click(function () {
        if ($(this).closest('.medlistcontainer').find('div.membermed').is('div.membermed:visible')) {
            $(this).closest('.medlistcontainer').find('div.membermed').hide();
            $(this).removeClass("listopen");
            $(this).addClass("listclosed");
            $(this).closest('.medlistcontainer').find('.managemember').hide();
            // if ANY membermed is visible, leave the Submit button visible
            if ($('.membermed').is('.membermed:visible')) {
            } else {
                $('#managemedsubmit').hide();
            }
        } else {
            $(this).closest('.medlistcontainer').find('div.membermed').show();
            $(this).removeClass("listclosed");
            $(this).addClass("listopen");
            $(this).closest('.medlistcontainer').find('.managemember').show();
            $('#managemedsubmit').show();
        }
    });

    // hide/show "add an additional med" on the Find Rx tab
    $('#addmedlink').click(function () { $('#addmed').toggle(); });

    // Past Care, Facility/Doctor details 
    $('tr.category').toggle(hiderows, showrows);

    /* for the letter-based prescription nav */
    $(".letternav").bind('click', letternav);

    /* for the More Info icons */
    $('div.learnmore').click(
		function () {
		    $('div.moreinfo:visible').fadeOut(500);
		    $(this).find('div.moreinfo:hidden').fadeIn(500);
		    //Added by Blue Note
		    return false;
		}
	);
    $('div.moreinfo').click(
    /* function(){$(this).find('div.moreinfo:visible').fadeOut(500);} */
		function () { $(this).fadeOut(500); }
	);

    /* for the Change Location link */
    $('a.changelocation').click(
		function () {
		    $(this).closest('div#location').nextAll('div.locationform').fadeIn(500);
		}
	);
    $('a#cancellocation').click(
		function () { $(this).closest('div.locationform').fadeOut(500); }
	);

    /* for the Save Search button */
    $('a#savesearch').click(
		function () {
		    $(this).closest("div.button").next('div#savesearchform').fadeIn(500);
		}
	);
    $('a#cancelsavesearch').click(
		function () { $(this).closest('div#savesearchform').fadeOut(500); }
	);

    // for selecting dosage of Rx result
    $("input.refinedosage").change(function () {
        $(this).closest("table").find("span.refineoptions").hide();
        $(this).closest("tr").find("span.refineoptions").show();
    });

    // for the scrollbar on long search results
    $('.scroll-pane').jScrollPane({ showArrows: true });

    /* this is meant to prevent the outline around the images that you click on */
    $("img,a,map,area").mouseup(function () { $(this).blur(); });

});               // end of $(document).ready

function cancelOverlay() {
	$(".overlay").hide();
	$(".whitescreen").hide();
}

function revertsearch() {
	$("#doctorsearch").hide();
	$("#facilitysearch").hide();
	$("#cptsearch").hide();
	$("a.advsearch").removeClass("callout");
}

function letternav() {
	$("div.letterbox").hide();
	$("a.letternav").removeClass("callout");
	var letter = $(this).attr("id");
	$("div#letter" + letter).show();
	$("a#" + letter).addClass("callout");
}

// show hidden Past Care table rows
function showrows() {
    if ($(this).parent().parent().attr("id") == "tablepastcare") {
        $(this).nextAll('tr').each(function () {
            if ($(this).hasClass('category') || $(this).hasClass('trfooter')) {
                return false;
            }
            $(this).hide();
        });
        if ($(this).hasClass('rowopen')) {
            $(this).removeClass('rowopen');
            $(this).addClass('rowclosed');
        }
    }
    else {
        $(this).nextAll('tr').each(function () {
            if ($(this).hasClass('category') || $(this).hasClass('trfooter')) {
                return false;
            }
            $(this).show();
        });
        if ($(this).hasClass('rowclosed')) {
            $(this).removeClass('rowclosed');
            $(this).addClass('rowopen');
        }
    }
}
function hiderows() {
    if ($(this).parent().parent().attr("id") == "tablepastcare") {
        $(this).nextAll('tr').each(function () {
            if ($(this).hasClass('category') || $(this).hasClass('trfooter')) {
                return false;
            }
            $(this).show();
        });
        if ($(this).hasClass('rowclosed')) {
            $(this).removeClass('rowclosed');
            $(this).addClass('rowopen');
        }
    }
    else {
        $(this).nextAll('tr').each(function () {
            if ($(this).hasClass('category') || $(this).hasClass('trfooter')) {
                return false;
            }
            $(this).hide();
        });
        if ($(this).hasClass('rowopen')) {
            $(this).removeClass('rowopen');
            $(this).addClass('rowclosed');
        }
    }
}

function trimstring(str) {
	if (str.trim)
		return str.trim()
	else
		return str.replace(/(^\s*)|(\s*$)/g, "") //find and remove spaces from left and right hand side of string
}
