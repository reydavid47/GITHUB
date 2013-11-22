/* -- requries cch_global.js -- */

/*** showmore usage list & middle handler between page calls and library usage : incase additional event are required for show more info ***/
function cch_showMore(iData) {
    /* 
    -- iData Structure --
    iData.action
    iData.method
    iData.callback
    */
    /* iData.method should match in WS */
    switch (iData.action) {
        case 'categoryDetail-RX':
            _cchGlobal.ajax.send({
                data: iData.data.join(','),
                method: iData.action,
                callback: function (data) {
                    if ((data) && (data.ok)) {
                        if (iData.callback) {
                            iData.callback(data);
                        }
                    }
                }
            });
            break;
        case 'categoryDetail-Imaging':
            _cchGlobal.ajax.send({
                data: iData.data.join(','),
                method: iData.action,
                callback: function (data) {
                    if ((data) && (data.ok)) {
                        if (iData.callback) {
                            iData.callback(data);
                        }
                    }
                }
            });
            break;
        case 'categoryDetail-Lab':
            _cchGlobal.ajax.send({
                data: iData.data.join(','),
                method: iData.action,
                callback: function (data) {
                    if ((data) && (data.ok)) {
                        if (iData.callback) {
                            iData.callback(data);
                        }
                    }
                }
            });
            break;
        case 'categoryDetail-MVP':
            _cchGlobal.ajax.send({
                data: iData.data.join(','),
                method: iData.action,
                callback: function (data) {
                    if ((data) && (data.ok)) {
                        if (iData.callback) {
                            iData.callback(data);
                        }
                    }
                }
            });
            break;

        default:

            break;
    };
};