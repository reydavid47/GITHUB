(function ($) {
    var count, thresh, navUrl, notified = false,
        wrapperDiv, closeImage, addLink, countDownText,
        CloseAndRemove = function () {
            closeImage.animate({ opacity: 0 }, 100, function () { wrapperDiv.animate({ height: 0, opacity: 0 }, 500); });
            notified = false;
        },
        AddMoreTime = function () {
            CloseAndRemove();
            sessionTimeoutCallServer();
            SetTimer();
        },
        Notify = function () {
            wrapperDiv
                .animate({ height: 155, opacity: 1 }, 500, function () {
                    closeImage.css({ opacity: 1 });
                });
            notified = true;
        },
        Timer = null,
        SetTimer = function () {
            var theForm = document.forms['form1'];
            if (!theForm) { theForm = document.form1; }
            count = theForm.TIMEOUT.value;
            thresh = theForm.TIMETILNOTIFY.value;
        },
        methods = {
            init: function (options) {
                if (options.NavUrl) {
                    navUrl = options.NavUrl;
                } else {
                    navUrl = "";
                }

                wrapperDiv = $(".stWrapper");
                countDownText = $(".stCountdown");

                closeImage = $(".stCloseImg");
                closeImage.click(CloseAndRemove);

                addLink = $(".stPreventLink");
                addLink.click(AddMoreTime);

                SetTimer();

                Timer = setInterval(function () {
                    if (--count < 1) {
                        clearInterval(Timer);
                        window.location.href = navUrl;
                    } else if (count < thresh && !notified) {
                        Notify();
                    }
                    var min = parseInt(count / 60),
                            sec = count % 60;
                    countDownText.text(min + " minute(s) and " + sec + " seconds");
                }, 1000);
            },
            handleServerResponse: function (returnValue) { }
        };

    $.fn.SessionTimeout = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method == 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist in jQuery.SessionTimeout.");
        }
    };
})(jQuery)