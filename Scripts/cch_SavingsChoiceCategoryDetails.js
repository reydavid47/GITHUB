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
            case 'click':
                $(iData.element).on('click', function (e) {
                    function sendUserFoward() {
                        window.location.href = $(iData.element).attr('href');
//                        console.log('-- sending user forward --');
//                        console.log($(iData.element).children('a').attr('href'));
                    };

                    if ($('.validationRequired').length > 0) {
                        e.preventDefault();
                    }
                    else {
                        e.preventDefault();
                        $(iData.element).parent().fadeTo('slow', 0);
                        var postData = {};
                        var dataSet = {};
                        var dataSetLength;
                        dataSet = gather({ action: 'getDataSet' });
                        dataSetLength = dataSet.length;
//                                                console.log(dataSet);
//                                                return;
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
            $('.userRatings.ratingRequired').each(function () {
                var _t, recordData;
                _t = this;
                recordData = {};
                recordData["cchid"] = _cchGlobal.pageData.cchid;
                function getPID() {
                    var pID;
                    pID = '';
                    pID = $(_t).attr('pid');
                    recordData["type"] = 'existing';
                    return pID;
                };
                recordData["cchid"] = _cchGlobal.pageData.cchid;
                recordData["eid"] = _cchGlobal.pageData.eid;
                recordData["category"] = _cchGlobal.pageData.category;
                recordData["pid"] = getPID();
                recordData["rating"] = starRating({ action: 'getCurrentRating', element: $(_t).find('ul li:first-child') });
                dataSet.push(recordData);
            });
            return dataSet;
            break;
    };
};

function recentProviderTable(iData) {
    /*
    -- iData Structure --
    iData.action
    */
    switch (iData.action) {
        case 'checkLength':
            if ($('.cch_SCtable.recurring').find('tbody tr').length == 0) {
                $('.cch_SCtable.recurring').hide();
                $('.cch_SCtable.recurring').prev().hide();
                $('.cch_SCtable.recurring').next().hide();
                $('.cch_SCtable.recurring').next().next().hide();
            }
            else {
                $('.cch_SCtable.recurring').show();
                $('.cch_SCtable.recurring').prev().show();
            }
            break;
    }
};

function fairPriceProviderTable(iData) {
    /*
    -- iData Structure --
    iData.action
    iData.category
    */
    switch (iData.action) {
        case 'checkLength':
            if ($('#fairprice_providers').find('tbody tr').length == 0) {
                $('#fairprice_providers').find('tbody').append('<tr><td colspan="10">There were no Fair Price Providers found with 25 miles.</td></tr>');
                $('#fairprice_providers').parent().find('h3').eq(1).show();
            }
            else {
                $('#fairprice_providers').parent().find('h3').eq(1).show();
            }
            break;
    }
};