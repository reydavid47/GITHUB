var debug = function () {
    this.log = function (iData) {
//        if (console && console.log) {
//            console.log('----logging data----');
//            console.log(iData);
//        }
//        else {
//            alert(iData); 
//        }
    };
};
_d = new debug();

/*switch scenarios used in cchGlobal for UI Setup*/
var _progress_mini__build = function () {
    var templateHTML = '<table id="progress_mini" class="cch_ui" cch_type="progress_mini" cch_defaultvalue="{defaultValue}">' +
                        '<tbody><tr>' +
                            '<td>Low</td>' +
                            '<td><div><div style="width:{progress}%;">&nbsp;</div></div></td>' +
                            '<td>High</td>' +
                        '</tr><tr>' +
                            '<td colspan="3">{displayText}</td>' +
                        '</tr>' +
                        '</tbody>' +
                        '</table>';
    var cchUI = '.cch_ui';
    $(cchUI).each(function () {
        if ($(this).attr('cch_type') == 'progress_mini') {
            var defaultValue = $(this).attr('cch_defaultvalue');
            var displayText = $(this).html();
            var appendHTML = '';
            appendHTML = templateHTML.replace("{defaultValue}", defaultValue);
            appendHTML = appendHTML.replace("{displayText}", displayText);
            appendHTML = appendHTML.replace("{progress}", defaultValue);
            $(this).parent().children().empty().parent().append(appendHTML).show().children(':first-child').remove();
        }
    });
}

var _progress_main_build = function () {
    var templateHTML = '<div class="{rateColor}" style="width:{width}px; position: relative;">' +
                        '<img width="300" height="22" src="../images/savings_bar.png">' +
                        '<b style="position:absolute; left: {left}px; top: -1px; color: white;">{rate}%</b></div>';
    var cchUI = '.cch_ui.progress_main';
    $(cchUI).each(function () {
        var appendHTML = '';
        var barColor = "greenbar";
        var rate = parseFloat($(this).attr('cch_defaultvalue')) * 100;
        var colorWidth = parseInt(290 * rate / 100);
        var left = colorWidth-40;
        if (rate < 50) { barColor = "redbar" } else if (rate < 80) { barColor = "yellowbar"; } else if (rate <= 100) { barColor = "greenbar"; }
        appendHTML = templateHTML.replace("{rateColor}", barColor);
        appendHTML = appendHTML.replace("{rate}", parseInt(rate));
        appendHTML = appendHTML.replace("{width}", colorWidth);
        appendHTML = appendHTML.replace("{left}", left);
        $(this).append(appendHTML);
    });
}

/*cch_ui - global cch ui script file*/
var cchGlobal = function (iData) {
    /* 
    --iData Structure-- 
    iData.init
    */
    var _t = this;
    var _buildUI = function (iData) {
        /*
        -- iData Model --
        iData.uiType
        */
        switch (iData.uiType) {            
            case 'progress_mini':
                _progress_mini__build();
                break;
            case 'progress_main':
                _progress_main_build();
                break;
        }
    };
    var _setupAjax = function () {
        _d.log('testing');
        $.ajaxSetup({
            type: 'post',
            cache: false,
            error: function (x, e) {
                if (x.status == 550)
                    alert("550 Error Message");
                else if (x.status == "403")
                    alert("403. Not Authorized");
                else if (x.status == "500")
                    alert("500. Internal Server Error");
                else
                    alert("Error...");
                console.log(x);
                console.log(e);
            },
            success: function (x) {

            }
        });
        $(document).ajaxStart(function () {
            _d.log('ajax started');
        });

        $(document).ajaxStop(function () {
            _d.log('ajax stopped');
        });
    };
    var _sendAjax = function (iData) {
        /*
        --iData Format--
        iData.data
        iData.method
        iData.callback
        */
        var ajaxURL = _t.statics.getAjaxMethodURL(iData.method);
        if (ajaxURL == '' || ajaxURL == undefined) {
            return;
        }
        var postingData = iData.data.split(',');
        var oData = {};

        for (keyval in postingData) {
            var splitter = postingData[keyval].split(':');
            oData[splitter[0]] = splitter[1];
        }
        oData['method'] = iData.method;
        $.ajax({
            url: ajaxURL,
            data: oData
        }).done(function (msg) {
            iData.callback(msg);
        });
    };
    var _init = function () {
        _t.ajax.setup();
    };
    _t.actions = {
        buildUI: function (iData) {
            _buildUI(iData);
        },
        loadPage: function () {
            $(_t.statics.dom.ob()[0]).fadeTo('slow', 1);
        },
        init: function () {
            _init();
        }
    };
    _t.ajax = {
        setup: function () {
            _setupAjax();
        },
        send: function (iData) {
            _sendAjax(iData);
        }
    };
    _t.mapping = {
        updateProviderRating: '../handlers/scratings.ashx',
        getProvider: '../handlers/providerlookup.ashx',
        auditSCIQ:'../handlers/auditsciq.ashx',
        showMore:'../handlers/showmore.ashx'
    };
    _t.statics = {
        dom: {
            type: function () {
                return ['#', '.'];
            },
            ob: function () {
                return ['body'];
            }
        },
        ajaxMethods: {
            updateMedicalProviderRating: {
                url: _t.mapping.updateProviderRating,
                other: 'something else can go here'
            },
            getProviderStates: {
                url: _t.mapping.getProvider,
                other: 'other info'
            },
            getProviderCities: {
                url: _t.mapping.getProvider,
                other: 'other info'
            },
            getProvidersByORG: {
                url: _t.mapping.getProvider,
                other: 'other info'
            },
            getProviders: {
                url: _t.mapping.getProvider,
                other: 'other info'
            },
            addProvider: {
                url: _t.mapping.getProvider,
                other: 'other info'
            },
            addUserReview: {
                url: _t.mapping.updateProviderRating,
                other: 'other info'
            },
            auditSCIQ: {
                url: _t.mapping.auditSCIQ,
                other: 'other info'
            },
            'categoryDetail-RX': {
                url: _t.mapping.showMore,
                other: 'other info'
            },
            'categoryDetail-Imaging': {
                url: _t.mapping.showMore,
                other: 'other info'
            },
            'categoryDetail-Lab': {
                url: _t.mapping.showMore,
                other: 'other info'
            },
            'categoryDetail-MVP': {
                url: _t.mapping.showMore,
                other: 'other info'
            }
        },
        getAjaxMethodURL: function (iAjaxMethod) {
            var methods = _t.statics.ajaxMethods
            var methodURL = '';
            for (methodInfo in methods) {
                if (iAjaxMethod == methodInfo) {
                    var methodDetails = methods[methodInfo];
                    methodURL = methodDetails.url;
                }
            }
            return methodURL;
        }
    };
    _t.pageData = {};
    if (iData.init == true) {/// <reference path="cch_global.js" />

        _t.actions.init();
    }
};

if (!_cchGlobal) {
    var _cchGlobal = new cchGlobal({ init: true });
}
