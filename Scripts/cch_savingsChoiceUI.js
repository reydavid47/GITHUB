/* -- builds states list, only one db call for all lists/dropdowns -- */
_cchGlobal.pageData['states'] = [];
_cchGlobal.pageData['cities'] = [];

function ajaxBlock(iData) { 
    /*
    iData Structure
    iData.action
    iData.callback
    */
    switch (iData.action) {
        case 'block':
            $.blockUI({
                message: '<img src="/Images/ajax-loader-AltCircle.gif"></br><label style="font-size:12px;font-family:verdana">loading, please wait</label>'
            });
            break;
        case 'unblock':
            $.unblockUI();
            break;
        case 'special':
            if (iData.callback != undefined) {
                iData.callback();
            }
            break;
    };
};

function buildStates() {
    _cchGlobal.ajax.send({
        data: "category:" + _cchGlobal.pageData.newProviderCategory,
        method: 'getProviderStates',
        callback: function (data) {
            if ((data) && (data.ok)) {
                /* -- handles single add provider -- */
                var statesDD = '#stateWrapper #states'
                $(statesDD).unbind('change');
                $(statesDD).children().remove();
                $(statesDD).append('<option value="void">Select State</option>');
                for (state in data.data.states) {
                    $(statesDD).append('<option value="' + data.data.states[state][0] + '">' + data.data.states[state][1] + '</option>');
                }
                $('#stateWrapper').fadeTo('slow', 1);
                $(statesDD).on('change', function () {
                    $('.ui-dialog-buttonset').hide();
                    $('#providerWrapper, .done a').hide().css('opacity', 0);
                    buildCities({
                        state: $(this).children(':selected').html()
                    });
                });
                /* -- handles all other state dropdown needed on sciq*/
                function loadAddtionalStateDropDown() {
                    var statesDD = '.states'
                    $(statesDD).unbind('change');
                    $(statesDD).children().remove();
                    $(statesDD).append('<option value="void">Select State</option>');
                    for (state in data.data.states) {
                        /* - load defualt state
                        if (_cchGlobal.pageData.pstate.toLowerCase() == data.data.states[state][0].toLowerCase()) {
                            $(statesDD).append('<option value="' + data.data.states[state][0] + '" selected>' + data.data.states[state][1] + '</option>');
                        }
                        else {
                        */
                            $(statesDD).append('<option value="' + data.data.states[state][0] + '">' + data.data.states[state][1] + '</option>');
                        /*
                        }
                        */

                    }
                    $(statesDD).on('change', function () {
                        var _t = this;
                        $('.ui-dialog-buttonset').hide();
                        $(_t).parent().parent().children('.providerWrapper').css('opacity', 0);
                        buildCities({
                            state: $(this).children(':selected').html(),
                            element: $(this)
                        });
                    });
                    //prefill state city
                    /*
                    $(statesDD).each(function () {
                        if ($(this).children(':selected').val() != 'void') {
                            $(this).trigger('change');
                        }
                    });
                    */
                };
                if ($('.states:visible').length > 0) {
                    //prefill state city
                    //ajaxBlock({ action: 'block' });
                    loadAddtionalStateDropDown();
                    //prefill state city
                    /*
                    ajaxBlock({
                        action: 'special',
                        callback: function () {
                            var x = window.setInterval(function () {
                                var noprovidersfound = $('.states:visible').length;
                                var selects = 3;
                                if ((noprovidersfound * selects) == $('.states:visible').closest('.noAddressProvider').find('select:visible').length) {
                                    window.setTimeout(function () {
                                        window.clearInterval(x);
                                        ajaxBlock({ action: 'unblock' });
                                    }, 500);
                                };
                            }, 500);
                        }
                    });
                    */
                }
            }
        }
    });
}
function buildCities(iData) {
    /*
    iData structure
    iData.state,
    iData.element
    */
    _cchGlobal.ajax.send({
        data: "state:" + iData.state + ",category:" + _cchGlobal.pageData.newProviderCategory,
        method: 'getProviderCities',
        callback: function (data) {
            if ((data) && (data.ok)) {
                function loadAddProviderCityDropdown() {
                    var citiesDD = '#cityWrapper #cities'
                    $(citiesDD).unbind('change');
                    $(citiesDD).children().remove();
                    $(citiesDD).append('<option value="void">Select City</option>');
                    for (city in data.data.cities) {
                        $(citiesDD).append('<option value="' + data.data.cities[city][0] + '">' + data.data.cities[city][1] + '</option>');
                    }
                    $(citiesDD).parent().show().fadeTo('slow', 1);
                    $(citiesDD).on('change', function () {
                        $('.ui-dialog-buttonset').hide();
                        $('#providerWrapper').hide().css('opacity', 0);
                        $('#providerWrapper #provider').children().remove();
                        $('#providerRating').hide();
                        buildProviders({
                            state: iData.state,
                            city: $(this).children(':selected').html()
                        });
                    });
                }
                /* -- handles all other cities dropdown needed on sciq*/
                function loadNoAddressProviderCityDropDown() {
                    var citiesDD = $(iData.element).parent().next().children('.cities');
                    $(citiesDD).unbind('change');
                    $(citiesDD).children().remove();
                    $(citiesDD).append('<option value="void">Select City</option>');
                    for (city in data.data.cities) {
                        /* - load default city
                        if (_cchGlobal.pageData.pcity.toLowerCase() == data.data.cities[city][0].toLowerCase()) {
                            $(citiesDD).append('<option value="' + data.data.cities[city][0] + '" selected>' + data.data.cities[city][1] + '</option>');
                        }
                        else {
                        */
                            $(citiesDD).append('<option value="' + data.data.cities[city][0] + '">' + data.data.cities[city][1] + '</option>');
                        /*
                        }
                        */
                    }
                    $(citiesDD).parent().show().fadeTo('slow', 1);
                    $(citiesDD).on('change', function () {
                        $('.ui-dialog-buttonset').hide();
                        buildProviders({
                            state: iData.state,
                            city: $(this).children(':selected').html(),
                            element: $(this),
                            oid: $(this).closest('.provider').attr('oid')
                        });
                    });
                    //prefill state city
                    /*
                    $(citiesDD).each(function () {
                        if ($(this).children(':selected').val() != 'void') {
                            $(this).trigger('change');
                        }
                    });
                    */
                };
                if (iData.element != undefined) {
                    loadNoAddressProviderCityDropDown();
                }
                else {
                    loadAddProviderCityDropdown();
                }  
            }     
        }
    });
}
function buildProviders(iData) {
    /*
    iData structure
    iData.state
    iData.city
    iData.element
    iData.oid
    */
    var _buildStandardProviders = function (iData) {
        _cchGlobal.ajax.send({
            data: "state:" + iData.state + ",city:" + iData.city + ",category:" + _cchGlobal.pageData.newProviderCategory,
            method: 'getProviders',
            callback: function (data) {
                if ((data) && (data.ok)) {
                    var providerList = [];
                    for (provider in data.data.providers) {
                        providerList.push({
                            'label':data.data.providers[provider][1].replace('||', ' - '),
                            'value': data.data.providers[provider][0]
                        });
                    }
                    function loadProviderDropdown() {
                        $('#providerWrapper').show().fadeTo('slow', 1);
                        $("#providers_autocomplete").val('');
                        $("#providers_autocomplete").autocomplete({
                            source: providerList,
                            select: function (event,ui){
                                $('#addProviderBtn.btn').fadeTo('fast', 1);
                                $('#providers_autocomplete').val(ui.item.label);
                                $('#newProviderInput .satisfaction.row .provider').attr('pid',ui.item.value).attr('oid','-1');
                                return false;
                            },
                            focus: function( event, ui ) {
                                $('#providers_autocomplete').val(ui.item.label);
                                return false;  
                             }
                        });
                    }
                    loadProviderDropdown();
                }
            }
        });
    };
    var _buildNoLocationProviders = function (iData) {
        _cchGlobal.ajax.send({
            data: "state:" + iData.state + ",city:" + iData.city + ",category:" + _cchGlobal.pageData.newProviderCategory + ",orgid:" + iData.oid,
            method: 'getProvidersByORG',
            callback: function (data) {
                if ((data) && (data.ok)) {
                    var providerList, providerHTMLSelectOptions;
                    providerList = [];
                    providerHTMLSelectOptions = '';
                    switch (data.data.providers.length) {
                        case (0):
                            providerHTMLSelectOptions = '<option val="0">No Locations found</option>';
                            break;
                        default:
                            for (provider in data.data.providers) {
                                providerList.push(data.data.providers[provider][1]);
                                providerHTMLSelectOptions += '<option val="' + data.data.providers[provider][0] + '">' + data.data.providers[provider][1].replace('||',' - ') + '</option>';
                            }
                            break;
                    }
                    function loadNoAddressProviderDropdown() {
                        $(iData.element).parent().next().find('select').children().remove();
                        $(iData.element).parent().next().find('select').append(providerHTMLSelectOptions);
                        $(iData.element).parent().next().fadeTo('slow', 1);

                    }
                    loadNoAddressProviderDropdown();
                }
            }
        });
    };
    if(iData.oid != undefined) {
        _buildNoLocationProviders(iData);
    }else{
        _buildStandardProviders(iData);            
    }
}

//previous page functionality - will need to be copied to each SCIQ page
function gotoPreviousPage(iData) {
    /*
    iData Structure
    iData.goto
    iData.action
    */
    var _t = this;
    switch (iData.action) {
        case 'init':
            $('.sciqReturnLink').on('click', function (e) {
                e.preventDefault();
                gotoPreviousPage({
                    action: 'redirect',
                    goto: '/SearchInfo/Search.aspx'
                });
            });
            break;
        case 'redirect':
            $('.sciqReturnLink').on('click', function (e) {
                e.preventDefault();
                window.location.href = iData.goto;
            });            
            break;
    }
}

function avatarPlus1(iData) {
    /*
    iData Structure
    iData.action
    iData.callback
    iData.parent
    iData.children
    iData.filter
    iData.avatarClass
    */

    var _t = this;

    //default loaded avatars
    var avatarClass = 'showFoundAvatars';
    //newly added avatars
    if (iData.avatarClass) {
        avatarClass = iData.avatarClass;
    }

    _t.init = function () {
        //preloaded avatars
        $(iData.parent).each(function () {
            //$('p.showFoundAvatars').remove();
            if (!$(this).hasClass(iData.filter)) {
                var avatarsFound = $(this).children('img').length;
                if (avatarsFound > 1) {
                    avatarsFound--;
                    var foundHTML = '<p class="' + avatarClass + '">+' + avatarsFound + '</p>';
                    $(this).children('p:last-child').remove();
                    $(this).append(foundHTML);
                }
            }
        });
        _t.setHandlers();
    };

    _t.setHandlers = function () {
        $('p.' + avatarClass).each(function () {
            $(this).hover(function () {
                $(this).attr('oValue', $(this).text());
                $(this).html('');
                $(this).parent().children('img:hidden').each(function () {
                    $(this).clone().appendTo($(this).parent().children(':last-child'));
                });
                $(this).addClass('expanded');
                //_t.setTooltips();
                _t.adjustWidth({ element: $(this) });
            }, function () {
                $(this).html($(this).attr('oValue')).removeClass('expanded').removeAttr('oValue');
                _t.adjustWidth({ element: $(this) });
            });
        });
        //_t.setTooltips();
    }

    _t.setTooltips = function () {
        $('.satisfaction.row .avatar img').each(function () {
            $(this).tooltip();
        });
    }
    _t.adjustWidth = function (iData) {
        /*
        iData Structure
        iData.element
        */
        var newWidth;
        var avatarWidth = 60;
        if ($(iData.element).children().length > 5) {
            newWidth = 5 * avatarWidth;
        }
        else {
            if ($(iData.element).children().length == 0) {
                $(iData.element).css({
                    'width':'20px'   
                });
            }
            else {
                newWidth = $(iData.element).children().length * avatarWidth;
            }
        }
        $(iData.element).css({ 'width': newWidth });
    }

    switch (iData.action) {
        case 'init':
            _t.init();
            break;
    }
};

//add provider funtionality, utilizing .net user control, will need to be applied to each SCIQ page
function addProvider(iData) {
    /*
    iData Structure
    iData.action
    iData.callback
    */
    var _t = this;
    switch (iData.action) {
        case 'init':
            $('.addProvider a').on('click', function (e) {
                e.preventDefault();
                $('#body_addProviderControl').slideDown();
            });
            $('.avatar.pointer').on('click', function () {
                var chooseAvatarHTML = '#addProviderAvatar';
                $(chooseAvatarHTML).dialog({
                    modal: true
                });
                $(chooseAvatarHTML + ' div img').each(function () {
                    $(this).tooltip();
                });
            });
            $('#addAvatars').on('click', function (e) {
                e.preventDefault();
                var avatarsSelected = [];
                $('#addProviderAvatar div input:checkbox').each(function () {
                    if ($(this).prop('checked')) {
                        avatarsSelected.push($(this).prev().prev().clone());
                    }
                });
                function insertNewAvatars(iData) {
                    /*
                    iData Structure
                    iData.avatars
                    */
                    if (iData.avatars.length > 0) {
                        $('#newProviderInput .avatar.pointer').children().remove();
                        for (each in iData.avatars) {
                            $('#newProviderInput .avatar.pointer').append(iData.avatars[each]);
                        }
                        $('#addProviderAvatar').dialog('close');
                        if (iData.callback) {
                            iData.callback();
                        }
                    }
                    else {
                        return;
                    }
                }
                insertNewAvatars({
                    avatars: avatarsSelected,
                    callback: function () {
                        avatarPlus1({
                            action: 'init',
                            parent: '#newProviderInput div.avatar',
                            children: 'img',
                            avatarClass: 'newAvatars'
                        });
                    }
                });
            });
            $('#addProviderBtn').on('click', function (e) {
                e.preventDefault();
                var insertHTML = $(this).closest('.satisfaction.row').clone();
                //$('#providerContent').append(insertHTML);
                $(insertHTML).insertBefore('#body_addProviderControl');
                $('#body_addProviderControl').prev().children(':last-child div:nth-child(2)').html('');
                avatarPlus1({
                    action: 'init',
                    parent: '.satisfaction.row div.avatar',
                    children: 'img'
                });
                function updateProviderDetails(iData) {
                    /*
                    iData Structure
                    iData.callback
                    */
                    var state = $('#states option:selected').val();
                    var city = $('#cities option:selected').val();
                    var provider = $('#providers_autocomplete').val();
                    $('#body_addProviderControl').prev().children(':last-child div.provider').html('<strong>' + provider + '</strong></br>' + city + ', ' + state);
                    $('#body_addProviderControl').prev().children(':last-child div.userAddedProvider').show();
                    if (iData.callback != undefined) {
                        iData.callback();
                    }
                }
                updateProviderDetails({
                    callback: function () {
                        addProvider({
                            action: 'initDeleteHandler'
                        });
                        addProvider({
                            action: 'initReviewHandler'
                        });
                        addProvider({
                            action: 'resetAddProviderState'
                        });
                    }
                });
            });
            break
        case 'initDeleteHandler':
            var deleteButton;
            deleteButton = '.userAddedProvider div.delete';
            $(deleteButton).each(function () {
                $(this).unbind('click');
                $(this).on('click', function () {
                    $(this).closest('.satisfaction.row').slideUp().remove();
                });
            });
            break;
        case 'initReviewHandler':
            var _t, reviewButton, dialogContent, existingReview, reviewStatus, alertUserReviewText;
            reviewButton = '.userAddedProvider div.review';
            $(reviewButton).each(function () {
                $(this).unbind('click');
                $(this).on('click', function () {
                    _t = this;
                    alertUserReviewText = '<div style="width:340px; text-align:center; margin:auto;"><label style="font-size:10px; line-height:10px; color:#333;">Please note: These reviews will be publicly available on the ClearCost site. The reviewer name will remain anonymous, but please do not reveal any information in your review that you would not want to be shared with others.</label></div>';
                    dialogContent = '';
                    existingReview = '';
                    reviewStatus = 'new'
                    existingReview = $(_t).children('textarea').text();
                    if ($(_t).children('label').text() != 'write a review') {
                        reviewStatus = 'existing';
                    }
                    dialogContent = '<textarea class="dialogReview">' + existingReview + '</textarea>';
                    if (reviewStatus == 'new') {
                        $(dialogContent).dialog({
                            modal: true,
                            buttons: [{
                                text: "Save",
                                click: function () {
                                    $(_t).children('textarea').text($('.dialogReview:visible').val());
                                    $(_t).children('label').text('update review');
                                    $('.dialogReview').dialog("close");
                                }
                            }]
                        });
                    }
                    else {
                        $(dialogContent).dialog({
                            modal: true,
                            buttons: [{
                                text: "Delete",
                                click: function () {
                                    starRating({
                                        action: 'restore',
                                        element: $(_t).closest('.satisfaction.row').find('.userRatings .starWrapper ul li:nth-child(1)')
                                    });
                                    $(_t).children('textarea').text('');
                                    $(_t).children('label').text('write a review');
                                    $('.dialogReview').dialog("close");
                                }
                            }, {
                                text: "Save",
                                click: function () {
                                    $(_t).children('textarea').text($('.dialogReview:visible').val());
                                    $(_t).children('label').text('update review');
                                    $('.dialogReview').dialog("close");
                                }
                            }]
                        });
                        var buttonCount;
                        buttonCount = $('.ui-dialog-buttonset:visible').children('button').length;
                        //>1:cancel/save
                        if (buttonCount > 1) {
                            $('.ui-dialog-buttonset:visible').children('button:first-child').css('margin-right', '100px');
                        }
                    }
                    function loadUserContentAlert() {
                        $(alertUserReviewText).insertAfter('.dialogReview')
                    };
                    loadUserContentAlert();
                });
            });
            break;
        case 'resetAddProviderState':
            var statesDropdown, hideAllElements, ghostAllElements, defaultAvatarHTML;
            /* -- elements set to hide -- */
            hideAllElements = ['#newProviderInput #cityWrapper', '#newProviderInput #providerWrapper'];
            /* -- elements set to non visible -- */
            ghostAllElements = ['#newProviderInput #addProviderBtn'];
            statesDropdown = '#newProviderInput #stateWrapper #states';
            $('#body_addProviderControl').slideUp();
            $(statesDropdown).children(':selected').each(function () {
                $(this).removeAttr('selected');
            });
            $(statesDropdown).children(':first-child').prop('selected', true);
            for (items in hideAllElements) {
                $(hideAllElements[items]).hide();
            }
            for (items in ghostAllElements) {
                $(ghostAllElements[items]).css('opacity', 0);
            }
            /* -- avatar : restores original state or selector, and original state of available avatars in dialog -- */
            /* - selectable - */
            defaultAvatarHTML = '<img width="60" height="60" src="../images/avatars/outline.png" alt="outline">';
            $('#newProviderInput .avatar.pointer').children().remove();
            $('#newProviderInput .avatar.pointer').append(defaultAvatarHTML);
            /* - dialog - */
            $('#addProviderAvatar').children('div').find('input').each(function () {
                $(this).prop('checked', false);
            });
            /* -- star ratings : restores original state -- */
            starRating({
                action: 'restore',
                element: $('#newProviderInput .userRatings ul li:first-child')
            });
            starRating({
                action: 'init'
            });

            /* -- clean additional page header text : showing no records, will show please rate -- */
            (function updateHeaderText() {
                var undesiredText;
                undesiredText = [
                    'You did not visit any pharmacies in the past year.',
                    'You did not visit any medical providers in the past year.',
                    'You did not visit any laboratories in the past year.',
                    'You did not visit any radiology facilities in the past year.'
                    ];
                $('#contentMain p').each(function () {
                    for (items in undesiredText) {
                        if ($(this).text() == undesiredText[items]) {
                            $(this).remove();
                        }                    
                    }                    
                });
            })();

            break;
    }

}

function completeIQlater(iData) { 
    /*
    iData Structure
    iData.action
    iData.callback
    */
    switch (iData.action) {
        case 'init':
            var IQBtn, returnURL;

            IQBtn = '.sciqCompleteLater';
            returnURL = '/SearchInfo/Search.aspx';

            $(IQBtn).on('click', function (e) {
                e.preventDefault();
                var _t = this;
                e.preventDefault();
                auditSCIQ({
                    method: 'quit',
                    cchid: _cchGlobal.pageData.cchid,
                    sid: _cchGlobal.pageData.sid,
                    url: _cchGlobal.pageData.pageURL,
                    category: _cchGlobal.pageData.newProviderCategory,
                    callback: function () {
                        window.location.href = returnURL;
                    }
                });                
            });

            if (iData.callback != undefined) {
                iData.callback();
            }
            break;
    }
}


function validate(iData) {
    /*
    iData Structure
    iData.validate
    iData.callback
    */
    switch (iData.action) {
        case 'stars':
            var starLocations;
            /* -- location of ratings : existing, added -- */
            starLocations = '#contentMain .satisfaction.row.existing, #contentMain .satisfaction.row.added';
            $(starLocations).find('div.userRatings').find('.starWrapper').find('ul').each(function () {
                /* - skips add new provider stars - */
                if ($(this).closest('#body_addProviderControl').attr('id') == 'body_addProviderControl') {

                }
                else {
                    if ($(this).children('li.userSelectedStar').length == 0) {
                        $(this).parent().addClass('noStarRating');
                    }
                }
            });
            if ($('.noStarRating').length > 0) {
                if ($('.validationRequired:visible') == true) {

                }
                else {
                    $('.next').append('<label class="validationRequired">Please rate your satisfaction with all providers</label>');
                }
            }
            else {
                if ($('.next .validationRequired:visible').length > 0) {
                    $('.next .validationRequired:visible').remove();
                }
            }
            if (iData.callback != undefined) {
                iData.callback();
            }
            break;
    }
};

function continueBtn(iData) { 
    /*
    iData Structure
    iData.action
    iData.element
    iData.callback
    */
    var _loadHandler = function (iData) {
        /*
        iData Structure
        iData.action
        */
        switch (iData.action) {
            case 'hover':
                $(iData.element).hover(
                    function () {
                        validate({
                            action: 'stars',
                            callback: function () {
                                //addProvider({
                                //  action: 'resetAddProviderState'
                                //});
                            }
                        });
                    },
                    function () {

                    }
                );
                break;

            case 'click':
                $(iData.element).on('click', function (e) {
                    addProvider({
                        action: 'resetAddProviderState'
                    });
                    //NOTE -- remove code when dev complete -- : BEGIN
                    //e.preventDefault();
                    //NOTE -- remove code when dev complete -- : END
                    validate({
                        action: 'stars',
                        callback: function () {
                            //addProvider({
                            //  action: 'resetAddProviderState'
                            //});
                        }
                    });
                    function sendUserFoward() {
                        window.location.href = $(iData.element).attr('href');
                    };

                    if ($('.validationRequired').length > 0) {
                        e.preventDefault();
                    }
                    else {
                        e.preventDefault();
                        $(iData.element).fadeTo('slow', 0);
                        var postData = {};
                        var dataSet = {};
                        var dataSetLength;
                        dataSet = gather({ action: 'getDataSet' });
                        dataSetLength = dataSet.length;
                        if (dataSetLength == 0) {
                            /* -- sends user forward when nothing to save -- */
                            sendUserFoward()
                        }
                        else {
                            /* -- saves data and moves user forward -- */
                            for (items in dataSet) {
                                $.ajax({
                                    url: _cchGlobal.statics.ajaxMethods.addUserReview.url,
                                    data: dataSet[items]
                                }).done(function (msg) {
                                    dataSetLength--;
                                    if (dataSetLength < 1) {
                                        window.setTimeout(function () {
                                            sendUserFoward()
                                        }, 250);
                                    }
                                });
                            }
                        }
                    }
                    return;
                });
                break;
        }
    };
    switch (iData.action) {
        case 'init':
            /*
            - hover handler removed
            _loadHandler({
            action: 'hover',
            element: iData.element
            });
            */
            _loadHandler({
                action: 'click', 
                element: iData.element 
            });
            break;
    }
};

function gather(iData) {
    /*
    iData Structure
    iData.action
    iData.callback
    */
    switch (iData.action) {
        case 'getTotalRows':
            return $('.satisfaction.row:visible').length;
            break;
        case 'getDataSet':
            var dataSet;
            dataSet = [];
            /* -- provider locations -- */
            $('.satisfaction.row').each(function () {
                // only parsing and collection data found within containMain
                if ($(this).parent().attr('id') == 'contentMain') {
                    var _t, recordData;
                    _t = this;
                    recordData = {};
                    recordData["cchid"] = _cchGlobal.pageData.cchid;
                    function getPID() {
                        var rID, pID, noAddreessPID;
                        pID = '';
                        pID = $(_t).find('.provider').attr('pid');
                        noAddreessPID = $(_t).find('.provider .noAddressProviderDetails .noAddressProvider .providerWrapper select option:selected').attr('val');
                        /*no provider selected*/
                        if (noAddreessPID == undefined) { noAddreessPID = 0; }
                        if (pID == '0') {
                            rID = noAddreessPID;
                            recordData["type"] = 'noaddress';
                        }
                        else {
                            rID = pID;
                            recordData["type"] = 'existing';
                        }
                        switch ($(_t).find('.provider').attr('oid')) {
                            case '-1':
                                recordData["type"] = 'added';
                                break;
                        };
                        return rID;
                    };
                    function getCCHIDS() {
                        var cchids = [];
                        $(_t).find('.avatar img').each(function () {
                            cchids.push($(this).attr('cid'));
                        });
                        return cchids.join('|');
                    };
                    recordData["cchids"] = getCCHIDS();
                    recordData["pid"] = getPID();
                    recordData["oid"] = $(_t).find('.provider').attr('oid');
                    recordData["rating"] = starRating({ action: 'getCurrentRating', element: $(_t).find('.userRatings ul li:first-child') });
                    recordData["eid"] = _cchGlobal.pageData.eid;
                    recordData["category"] = _cchGlobal.pageData.newProviderCategory;
                    recordData["review"] = $(_t).find('.userAddedProvider div textarea').val();
                    dataSet.push(recordData);
                }
            });
            return dataSet;
            break; 
    };
};

function noAddressProvider(iData) { 
    /*
    iData Stucture
    iData.action
    */
    var _init = function () {
        $('.satisfaction.row.existing').each(function () {
            var _t = this;
            if ($(this).children('.provider').attr('pid') == '0') {
                $(_t).children('.provider').find('.providerDetails').hide();
                $(_t).children('.provider').find('.noAddressProviderDetails').find('.stateWrapper').fadeTo('fast', 1);
            }
            else {
                $(_t).children('.provider').find('.noAddressProviderDetails').hide();
            }
        });
    };
    var _readyState = function () {
        $('.noAddressProviderDetails').each(function () {
            var _t = this;
            $(this).find('select.states').children().each(function () {
                if ($(this).val().toLowerCase() == _cchGlobal.pageData.pstate.toLowerCase()) {
                    $(this).prop('selected', true).trigger('change');
                    window.setInterval(function () {
                        var x = $(_t).find('select.cities');
                    }, 500);
                }
            });
        });
    };
    var _readyCity = function () {
        $('.noAddressProviderDetails').each(function () {
            $(this).find('select.cities').children().each(function () {
                if ($(this).val().toLowerCase() == _cchGlobal.pageData.pcity.toLowerCase()) {
                    $(this).prop('selected', true).trigger('change');
                }
            });
        });
    };
    switch (iData.action) {
        case 'init':
            _init();
            break;      
        case 'readyState':
            _readyState();
            break;
        case 'readyCity':
            _readyCity();
            break;             
    }
};

function auditSCIQ(iData) { 
    /*
    iData Structure
    iData.method
    iData.cchid
    iData.sid
    iData.category
    iData.url
    iData.callback
    */

    var postData = {};
    postData["method"]=iData.method;
    postData["cchid"]=iData.cchid;
    postData["sid"]=iData.sid;
    postData["category"]=iData.category;    
    postData["url"]=iData.url;
    $.ajax({
        url: _cchGlobal.statics.ajaxMethods.auditSCIQ.url,
        data: postData
    }).done(function (response) {
        if (iData.callback != undefined) {
            iData.callback();
        }
    });
};

function enterFuntionality(iData) {
    /*
    iData Structure
    iData.action
    iData.callback
    */
    switch (iData.action) {
        case 'init':
            $('body').on('keypress', function (e) {
                if (e.keyCode == 13) {
                    e.preventDefault();
                }
            });
            break;
    };
};
$(document).ready(function () {
    enterFuntionality({
        action:'init'
    });
});

