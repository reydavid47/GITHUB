function getUrlVariable() {
    var variables = {};
    var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
        variables[key] = value;
    });
    return variables;
}


var __bindPlusMinus = function (iData) {
    //  --iData Model--
    //  iData.event
    //  iData.element
    $(iData.element).on('click', function () {
        $(this).next().toggle();
    });
    $(iData.element).toggle(function () {
        $(this).removeClass('listclosed').addClass('listopen');
    }, function () {
        $(this).removeClass('listopen').addClass('listclosed');
    });
}

function __generalValidation(iData) {
    //  --iData Model--
    //  iData.type
    //  iData.page   
    //  iData.event
    var requiresSelection = '.stepWrapper .subpanel .switchradio input.answer';
    var requiredItems = $(requiresSelection).length;
    var selectedItems = $(requiresSelection + ':checked').length;
    if (selectedItems == 0 && requiredItems >= 1) {
        iData.event.preventDefault();
        alert('general validation\n\nPlease make a selection');
    }
};

var _switchStep = function () {
    var _t = this;
    var _bindEvent = function (iData) {
        //  --iData Model--
        //  iData.event
        //  iData.element
        switch (iData.event) {
            case 'plusminus':
                __bindPlusMinus(iData);
                break;
        }
    };
    var __validatePage = function (iData) {
        //  --iData Model--
        //  iData.type
        //  iData.page   
        //  iData.event     
        //specification page based validation below
        switch (iData.page) {
            case "2":
                _t.saveDisplyTable();
                break;
        //case not needed, kept in case needed
//            case "3":
//                __generalValidation(iData);
//                var skipStep;
//                $('.answer').each(function(){
//                    if($(this).prop('checked')==true){
//                        skipStep = $(this).index('.answer');
//                        //if first checkbox (index[0]) is checked, skip to step 5
//                        if(skipStep==0){
//                            iData.event.preventDefault();
//                            var curl = window.location.href;
//                            var nurl;
//                            var currentStep = getUrlVariable()["scswitch_step"];
//                            nurl = curl.replace('_step='+currentStep,'_step='+parseInt(parseInt(currentStep)+2));
//                            window.location.href = nurl;                        
//                        }
//                    }
//                });                                                                   
//                break;
            default:
                __generalValidation(iData);
        }
    };
    _t.ajaxURL = '../handlers/SwitchStep.ashx';
    _t.bind = function (iData) {
        _bindEvent(iData);
    };
    _t.validate = function (iData) {
        //  --iData Model--
        //  iData.type
        //  iData.page
        //  iData.event
        switch (iData.type) {
            case "page":
                __validatePage(iData);
                break;
        }
    };
    _t.sendChoices = function () {
        var sendItems = '.answer';
        var method = 'savechoice';
        $(sendItems).on('click', function () {
            var oData = {
                type:'post',
                method:method,
                sessionid:$('#switchStepGuIDe').attr('ssid'), 
                cchid:$('#switchStepGuIDe').attr('cchid'), 
                stepnum:$(this).attr('stepnum'), 
                decisionid:$(this).attr('decisionid')
            };
            $.ajax({
                url: _t.ajaxURL,
                data: oData,
                type: oData.type,
            }).done(function (msg) {
                
            });
        });
    };
    _t.sendEmailReminder = function(){
        var method = 'savechoiceemail';
        var oData = {
            type:'post',
            method:method,
            sessionid:$('#switchStepGuIDe').attr('ssid'), 
            cchid:$('#switchStepGuIDe').attr('cchid'), 
            stepnum:4, 
            decisionid:6,
            emaildate:$('#reminderdate').val()
        };
        $.ajax({
            url: _t.ajaxURL,
            data: oData,
            type: oData.type,
        }).done(function (msg) {
                
        });    
    };
    _t.closeSession = function(iData){
        //iData Structure
        //iData.callback
        var method = 'endsession';
        var oData = {
            type:'post',
            method:method,
            sessionid:$('#switchStepGuIDe').attr('ssid'), 
            state:1
        };
        $.ajax({
            url: _t.ajaxURL,
            data: oData,
            type: oData.type,
        }).done(function (msg) {
            iData.callback();                
        });    
    };
    _t.saveSelectedFairPriceProviders = function(iData){
        //iData Structure
        //iData.callback
        //iData.providerlist
        var method = 'selectedproviders';
        var oData = {
            type:'post',
            method:method,
            sessionid:$('#switchStepGuIDe').attr('ssid'), 
            providerlist:iData.providerlist
        };
        $.ajax({
            url: _t.ajaxURL,
            data: oData,
            type: oData.type,
        }).done(function (msg) {
            iData.callback();                
        });    
    };
    _t.saveDisplyTable = function(){
        var displayTable = '.cch_SCtable';
        sessionStorage.switchStepDisplay = $(displayTable).html();  
    };
    _t.getDisplayTable = function(){
        return sessionStorage.switchStepDisplay;
    };
}
var switchStep = new _switchStep();

$(document).ready(function () {
    var ssCategory, ssStep;
    ssCategory = getUrlVariable()["scswitch_category"];
    ssStep = getUrlVariable()["scswitch_step"];
    $('#ResultsContent_cphContentBar_ctl00_switchContinue').on('click', function (e) {
        switchStep.validate({
            type: 'page',
            page: ssStep,
            event: e
        });
    });
    switchStep.sendChoices();
});