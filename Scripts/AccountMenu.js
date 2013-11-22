(function ($) {
    var AccountMenuLink, AccountMenuElement,
        closeSelector, editSelector, saveSelector, cancelSelector, headerSelector,
        methods = {
            init: function (options) {
                AccountMenuLink = this;
                if (options) {
                    if (options.AccountMenuElement) { AccountMenuElement = $(options.AccountMenuElement); }
                    if (options.CloseSelector) { closeSelector = options.CloseSelector; }
                    if (options.EditSelector) { editSelector = options.EditSelector; }
                    if (options.SaveSelector) { saveSelector = options.SaveSelector; }
                    if (options.CancelSelector) { cancelSelector = options.CancelSelector; }
                    if (options.HeaderSelector) { headerSelector = options.HeaderSelector; }
                    RigMenuLink();
                    RigMenuItems();
                } else {
                    $.error("jQuery.AccountMenu did not receive any setting to initialize with");
                }
            }
        },
        RigMenuLink = function () {
            //  Check to see if a popup is attached and if not, attach one
            if (!AccountMenuLink.attr("at_child")) {
                if (AccountMenuElement) {
                    at_attach(AccountMenuLink.attr("id"), AccountMenuElement.attr("id"), "click", "y", "pointer");
                }
            }
        },
        RigMenuItems = function () {
            //Check to make sure we have elements to attach to
            if (AccountMenuElement) {
                //Get all the links and act
                AccountMenuElement.find("a").each(function (index, menu) {
                    var ref = $("div#" + $(menu).attr("ref"));
                    //Check and make sure there isn't already a click event created
                    if (!$(menu).data("events")) {
                        //If there isn't already one, attach one
                        $(menu).click(function() {
                            cancelOverlay();
                            $("<div />").addClass("whitescreen").appendTo("body").show().click(function () {
                                cancelOverlay();
                            });
                            AccountMenuElement.css("visibility", "hidden");
                            ref.center().show();
                        });
                    }
                    //Bind the close event if it isn't already
                    if (!ref.find(closeSelector).data("events")) {
                        ref.find(closeSelector).click(function () { cancelOverlay(); });
                    }
                    //Bind the edit event if it isn't already
                    if (!ref.find(editSelector).data("events")) {
                        ref.find(editSelector).live("click",function () {
                            $(this).closest("tr").nextUntil(headerSelector).css("display", "table-row");
                        });
                    }
                    //Bind the save event if it isn't already
                    if (!ref.find(saveSelector).data("events")) {
                        ref.find(saveSelector).click(function () {
                            $(this).closest("tr").prevUntil(headerSelector).andSelf().hide();
                            return true;
                        });
                    }
                    //Bind the cancel event if it isn't already
                    if (!ref.find(cancelSelector).data("events")) {
                        ref.find(cancelSelector).click(function () {
                            $(this).closest("tr").prevUntil(headerSelector).andSelf().hide();
                        });
                    }
                });
            }
        }
    $.fn.AccountMenu = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist on jQuery.AccountMenu");
        }
    }
})(jQuery);