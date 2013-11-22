$(document).ready(function () {
    $(document).click(function () {
        $("div.moreinfo:visible").fadeOut(500);
    });

    $("div.learnmore").click(function () {
        $("div.moreinfo:visible").fadeOut(500);
        $(this).find("div.moreinfo:hidden").fadeIn(500);
        return false;
    });

    $("div.moreinfo").click(function () {
        $(this).fadeOut(500);
    });
});