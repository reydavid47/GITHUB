(function ($) {
    var count, thresh, navUrl, textToDisplay,
        CloseAndRemove = function () {
            closeImage.delay(500).animate({ opacity: 0 }, 500);
            outerDiv.animate({
                height: "0px",
                opacity: 0
            }, 500, function () {
                outerDiv.remove();
            });
        },
        AddMoreTime = function () {
            window.location.reload();
        },
        outerDiv = $("<div />").css({
            position: "fixed",
            bottom: "0px",
            left: "30px",
            border: "none",
            border: "0px none rgb(51,51,51)",
            borderBottom: "none",
            height: "0px",
            minWidth: "200px",
            maxWidth: "285px",
            padding: "20px",
            boxShadow: "1px 1px 3px black",
            borderTopLeftRadius: "6px",
            borderTopRightRadius: "6px",
            backgroundColor: "#F0EAF4",
            opacity: "0"
        }),
        closeImage = $("<img />").attr({
            src: "https://www.clearcosthealth.com/Images/icon_x_sm.png",
            alt: "",
            width: "14px",
            height: "14px"
        }).css({
            width: "14px",
            height: "14px",
            position: "absolute",
            right: "7px",
            top: "7px",
            cursor: "pointer",
            opacity: 0
        }).click(CloseAndRemove),
        attentionH = $("<h2 />").text("ATTENTION").css("textAlign", "center"),
        verbageH = "", // $("<h3 />").text(textToDisplay).css("textAlign", "left"),
        timeleftH = $("<h3 />").text(" minute(s) and  seconds").css({
            textAlign: "center",
            color: "black",
            padding: "2px",
            marginBottom: "15px",
            display: "inline-block"
        }),
        addlinkH = $("<h3 />").text("To prevent this please click here").css({
            textAlign: "center",
            cursor: "pointer",
            textDecoration: "underline"
        }).click(AddMoreTime),
        Notify = function () {
            outerDiv.animate({
                height: "155px",
                opacity: 1
            }, 500, function () { closeImage.css("opacity", "1"); });
        },
        Timer = setInterval(function () {
            if (--count < 1) {
                clearInterval(Timer);
                window.location.href = navUrl;
            } else if (count < thresh) {
                Notify();
            }
            var min = parseInt(count / 60),
                sec = count % 60;
            timeleftH.text(min + " minute(s) and " + sec + " seconds");
        }, 1000),
        methods = {
            init: function (options) {
                if (options) {
                    if (options.TimeTilTimeout) {
                        count = options.TimeTilTimeout;
                    } else {
                        count = 120;
                    }
                    if (options.TimeTilNotify) {
                        thresh = options.TimeTilNotify;
                    } else {
                        thresh = parseInt(count * .1);
                    }
                    if (options.NavUrl) {
                        navUrl = options.NavUrl;
                    } else {
                        navUrl = "";
                    }
                    if (options.Text) {
                        textToDisplay = options.Text;
                    } else {
                        textToDisplay = "You will be signed out due to inactivity in\:";
                    }
                    verbageH = $("<h3 />").text(textToDisplay).css("textAlign", "left");
                }
                var newTimer = outerDiv;
                closeImage.appendTo(newTimer);
                attentionH.appendTo(newTimer);
                verbageH.appendTo(newTimer);
                timeleftH.appendTo(newTimer);
                addlinkH.appendTo(newTimer);
                newTimer.appendTo(this);
            }
        };

    $.fn.TimeoutTimer = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist in jQuery.TimeoutTimer.");
        }
    }
})(jQuery);