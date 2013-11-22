var cch = {};

google.load('visualization', '1', {packages:['table']});

$(document).ready(function(){

    cch.dateRange();
    
    $('.panel').click(cch.togglePanel);

    $('#metrics-options-form').submit(function(){
      return cch.submitMetricsOptions();
    });
    
    // Activates knockout.js
    var viewModel = new cch.AdminViewModel();
    ko.applyBindings(viewModel);

});


cch.Metric = function(data) {
    this.name = ko.observable(data.Name);
    this.value = ko.observable(data.Value);
}

cch.AdminViewModel = function () {
    var self = this;

    self.firstName = 'Bert';
    self.data = ko.observableArray();
    self.getData = function(){
        alert("GETTING DATA");
        cch.ajaxCall('ServerResponse.ashx', {}, 
            {type:'GET', 
            //callback function sends data to observable array
             callback:function(data){
                console.log(data);
                var mappedData = $.map(data, function(item) { return new cch.Metric(item) });
                //self.tasks(mappedTasks);
                self.data(mappedData)
            }
        });
    }

}


// SUBMIT METRICS FORMS
cch.submitMetricsOptions = function(){
    var form = $('#metrics-options-form').serializeArray();
    console.log(form);
    //google chart submit
    cch.ajaxCall('ServerResponse.ashx', form, {type:'GET', callback:cch.metricsCallback});
    return false;
}


cch.metricsCallback = function(data){
    console.log('CALL BACK HIT');
    console.log(data);
    cch.enrollmentsTable(data);
}


//GENERAL PURPOSE AJAX FUNCTION CALL
cch.ajaxCall = function (url, data, opts) {
    $.ajax({
        url: '../adminportal/handlers/' + url,
        type: opts.type || 'POST',
        data: data,
        contentType: opts.contentType || 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (resData) { opts.callback(resData); },
    });
}


//DROP DOWN PANEL ANIMATION
cch.togglePanel = function(evt){
   $(evt.currentTarget).next().toggle();
   return false;
}


//GOOGLE VISUALIZATION TABLE
cch.enrollmentsTable = function(ajaxData){
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Name');
    data.addColumn('number', 'Value');
        

    $.each(ajaxData, function() {
        data.addRows([
        [this.Name,  this.Value],
    ]);
    });

    var table = new google.visualization.Table(document.getElementById('table-div'));
    var cssClassNames = {headerRow: 'gtable-header', tableRow: 'gtable-row', tableCell: 'gtable-cell'};
    table.draw(data, {cssClassNames: cssClassNames});
}




//
//DATEPICKER
//
cch.dateRange = function() {
	var from = $( '#from' ).datepicker({
		defaultDate: '-1w',
		changeMonth: true,
		numberOfMonths: 1,
        maxDate: '0d',
		onSelect: function( selectedDate ) {
			$( '#to' ).datepicker( 'option', 'minDate', selectedDate );
		}
	});
	var to = $( '#to' ).datepicker({
		defaultDate: '+0d',
		changeMonth: true,
		numberOfMonths: 1,
        maxDate: '0',
		onSelect: function( selectedDate ) {
			$( '#from' ).datepicker( 'option', 'maxDate', selectedDate );
		}
	});

    from.datepicker('setDate', new Date());
    to.datepicker('setDate', new Date());    
}
