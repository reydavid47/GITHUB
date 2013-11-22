(function (window) {
    var PatientAddress = {
        GoogleGeocoder: null,
        Street: "",
        City: "",
        State: "",
        Zip: "",
        ControlIDs: {
            UseNewRadio: "#rbUseNew",
            ChangeLocation: "chgLoc",
            ChangeLocationLink: "a.changelocation",
            LocationForm: "div.locationform",
            LocationDiv: "div#location",
            CancelLink: "a#cancellocation",
            CancelLabelLink: "a#lblCancel",
            SavedAddressDropDown: "#ddlSavedAddresses",
            SavedAddressDropDown_Selected: "#ddlSavedAddresses option:selected",
            StreetTextbox: "#txtChgAddress",
            CityTextbox: "#txtChgCity",
            StateDropDown: "#ddlState",
            StateDropDown_Selected: "#ddlState option:selected",
            ZipCodeTextbox: "#txtChgZipCode",
            SaveButton: "#lbtnSaveButton"
        }
    };

    PatientAddress.ParseAddress = function (FullAddress) {
        var segs = FullAddress.split(",");
        PatientAddress.Street = segs[0].trim();
        PatientAddress.City = segs[1].trim();
        var stateNzip = segs[2].trim().split(" ");
        PatientAddress.State = stateNzip[0].trim();
        PatientAddress.Zip = stateNzip[1].trim();
    };
    PatientAddress.Perform = function (action) {
        if (!google.maps) {
            google.load("maps", "3.8",
                {
                    "other_params": ((window.location.host.indexOf("localhost") > 0) ? "" : "client=gme-clearcosthealth&") + "sensor=false",
                    "callback": action
                }
            );
        }
        else {
            action();
        }
    };
    PatientAddress.DoGeocode = function () {
        if ($(PatientAddress.ControlIDs.UseNewRadio).attr("checked")) {
            if (!Page_ClientValidate(PatientAddress.ControlIDs.ChangeLocation)) { return true; }
        }
        var address = PatientAddress.Street + ",+" + PatientAddress.City + ",+" + PatientAddress.State + " " + PatientAddress.Zip;
        PatientAddress.GoogleGeocoder = new google.maps.Geocoder();
        PatientAddress.GoogleGeocoder.geocode({ "address": address }, PatientAddress.SaveGeocode);
    };
    PatientAddress.SaveGeocode = function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            var theForm = document.forms['form1'];
            if (!theForm) { theForm = document.form1; }
            if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
                theForm.PATIENTLATITUDE.value = results[0].geometry.location.lat();
                theForm.PATIENTLONGITUDE.value = results[0].geometry.location.lng();
                theForm.LOCATIONHASH.value = window.location.hash;
                theForm.submit();
            }
        }
    };
    PatientAddress.CheckAddress = function (source, arguments) {
        if ($(PatientAddress.ControlIDs.UseNewRadio).attr("checked") == "checked") {
            arguments.IsValid = (arguments.Value != null);
            if (arguments.IsValid) { arguments.IsValid = !(arguments.Value.trim().toLowerCase() == "enter street address"); }
        }
    };
    PatientAddress.CheckCity = function (source, arguments) {
        if ($(PatientAddress.ControlIDs.UseNewRadio).attr("checked") == "checked") {
            arguments.IsValid = (arguments.Value != null);
            if (arguments.IsValid) { arguments.IsValid = !(arguments.Value.trim().toLowerCase() == "enter city"); }
        }
    };
    PatientAddress.CheckState = function (source, arguments) {
        if ($(PatientAddress.ControlIDs.UseNewRadio).attr("checked") == "checked") {
            arguments.IsValid = (arguments.Value != null);
            if (arguments.IsValid) { arguments.IsValid = !(arguments.Value.trim().toLowerCase() == "state"); }
        }
    };
    PatientAddress.CheckZip = function (source, arguments) {
        if ($(PatientAddress.ControlIDs.UseNewRadio).attr("checked") == "checked") {
            arguments.IsValid = (arguments.Value != null);
            if (arguments.IsValid) { arguments.IsValid = !(arguments.Value.trim().toLowerCase() == "zip code"); }
        }
    };

    window.PatientAddress = PatientAddress;
})(window);

$(document).ready(function () {
    $(PatientAddress.ControlIDs.ChangeLocationLink).click(function () {
        cancelOverlay();
        $("<div />").addClass("whitescreen").appendTo("body").show().click(function () { cancelOverlay(); $(PatientAddress.ControlIDs.LocationForm).hide(); });
        $(this).closest(PatientAddress.ControlIDs.LocationDiv).nextAll(PatientAddress.ControlIDs.LocationForm).show();        
    });
    $(PatientAddress.ControlIDs.CancelLink).click(function () {
        $(this).closest(PatientAddress.ControlIDs.LocationForm).hide();
        cancelOverlay();
        PatientAddress.Street = "";
        PatientAddress.City = "";
        PatientAddress.State = "";
        PatientAddress.Zip = "";
    });
    $(PatientAddress.ControlIDs.CancelLabelLink).click(function () {
        $(this).closest(PatientAddress.ControlIDs.LocationForm).hide();
        cancelOverlay();
        PatientAddress.Street = "";
        PatientAddress.City = "";
        PatientAddress.State = "";
        PatientAddress.Zip = "";
    });
    $(PatientAddress.ControlIDs.SavedAddressDropDown).change(function () { PatientAddress.ParseAddress($(PatientAddress.ControlIDs.SavedAddressDropDown_Selected).text()); });
    $(PatientAddress.ControlIDs.StreetTextbox).change(function () { PatientAddress.Street = this.value; });
    $(PatientAddress.ControlIDs.CityTextbox).change(function () { PatientAddress.City = this.value; });
    $(PatientAddress.ControlIDs.StateDropDown).change(function () { PatientAddress.State = $(PatientAddress.ControlIDs.StateDropDown_Selected).text(); });
    $(PatientAddress.ControlIDs.ZipCodeTextbox).change(function () { PatientAddress.Zip = this.value; });
    $(PatientAddress.ControlIDs.SaveButton).click(function () { $("body").css("cursor", "progress"); PatientAddress.Perform(PatientAddress.DoGeocode); });
});