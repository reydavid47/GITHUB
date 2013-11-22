$(document).ready(function () {
    $("#tabs").tabs();

    $("#addmed").click(function () {
        //$('<div />').addClass('whitescreen').appendTo('body').show();
        $('div#meduser').show();
    });

    // hide/show the family members' meds on the Find Rx tab
    $('a.member').click(function () {
        if (!$(this).closest('.medlistcontainer').find('div.membermed').is('div.membermed:visible')) {
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

    //Hide the fade screen if it's open
    $("#whitescreen").click(function () { $("#whitescreen").hide(); });    
});
jQuery.fn.center = function () {
    this.css("position", "absolute");
    this.css("top", "180px");
    this.css("left", (($(window).width() - this.outerWidth()) / 2) + $(window).scrollLeft() + "px");
    return this;
}