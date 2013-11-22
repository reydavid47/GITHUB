function DisplayValidation(element, IsValid) {
    var indicatorElement = element.parentNode.parentNode.getElementsByTagName("td")[0];
    if (IsValid) {
        var vSpan = document.createElement("span");
        vSpan.innerHTML = "&#10003;";
        vSpan.setAttribute("style",
            "color:green;float:right;font-size:20px;margin:0px;padding-right:5px;font-weight:bolder;height:30px;");

        if (indicatorElement.getElementsByTagName("span").length == 0) {
            indicatorElement.appendChild(vSpan);
        }
        else {
            if (indicatorElement.getElementsByTagName("span").innerHTML != "&#10003") {
                var oSpan = indicatorElement.getElementsByTagName("span");
                indicatorElement.removeChild(oSpan[0]);
            }
            indicatorElement.appendChild(vSpan);
        }
        element.style.border = "2px solid green";
    }
    else {
        var vSpan = document.createElement("span");
        vSpan.innerHTML = "&#215;";
        vSpan.setAttribute("style",
        "color:red;float:right;font-size:20px;margin:0px;padding-right:5px;font-weight:bolder;height:30px;");

        if (indicatorElement.getElementsByTagName("span").length == 0) {
            indicatorElement.appendChild(vSpan);
        }
        else {
            if (indicatorElement.getElementsByTagName("span").innerHTML != "&#215;") {
                var oSpan = indicatorElement.getElementsByTagName("span");
                indicatorElement.removeChild(oSpan[0]);
            }
            indicatorElement.appendChild(vSpan);
        }
        element.style.border = "2px solid red";
        element.focus();
    }
}
function ValidateEmpty(element) {
    ///<summary>Validates whether or not the sending element has any text at all</summary>
    var valid = (element.value.toString().trim() != "");

    DisplayValidation(element, valid);
    
    return valid;
}
function ValidateName(NameElement) {
    var HasValidity = ValidateEmpty(NameElement);
    if (HasValidity) {
        var validName = new RegExp("^[A-Za-z0-9 ]{1,30}$", "i");
        HasValidity = validName.test(NameElement.value);
        DisplayValidation(NameElement, HasValidity);
    }
    return HasValidity;
}
function ValidateID(IDElement) {
    var HasValidity = false;

    if (IDElement.id === "txtMemID" && IDElement.value != "") {
        document.getElementById("txtSSN").value = "";
        document.getElementById("txtSSN").style.border = "1px solid #CCC";
        HasValidity = ValidateEmpty(IDElement);
    }
    else if (IDElement.id === "txtSSN" && IDElement.value != "") {
        document.getElementById("txtMemID").value = "";
        document.getElementById("txtMemID").style.border = "1px solid #CCC";
        HasValidity = ValidateEmpty(IDElement);
        if (HasValidity) {
            var validSSN = new RegExp("^[0-9]{4}$", "i");
            HasValidity = validSSN.test(IDElement.value);
            DisplayValidation(IDElement, HasValidity);
        }
    }
    else {
        document.getElementById("txtSSN").style.border = "1px solid #CCC";
        document.getElementById("txtMemID").style.border = "1px solid #CCC";
        HasValidity = ValidateEmpty(IDElement);
    }
        
    return HasValidity;
}
function ValidateDOB(DOBElement) {
    var HasValidity = ValidateEmpty(DOBElement);
    if (HasValidity) {
        var validDate = new RegExp("^((0?[1-9]|1[012])[- /.](0?[1-9]|[12][0-9]|3[01])[- /.](19|20)?[0-9]{2})*$", "i");
        HasValidity = validDate.test(DOBElement.value);
        DisplayValidation(DOBElement, HasValidity);
    }
    return HasValidity;
}
function ValidateForm() {
    var HasValidity = true;
//    var HasValidity = ValidateName(document.getElementById("txtFirstName"));

//    if (HasValidity) {
//        HasValidity = ValidateName(document.getElementById("txtLastName"));
//        if (HasValidity) {
            if(document.getElementById("txtMemID").value != "") {
                HasValidity = ValidateID(document.getElementById("txtMemID"));
            } else {
                HasValidity = ValidateID(document.getElementById("txtSSN"));
            }
            if(HasValidity) {
                HasValidity = ValidateDOB(document.getElementById("txtDOB"));
            }
//        }
//    }

    return HasValidity;
}