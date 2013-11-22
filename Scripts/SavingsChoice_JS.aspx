<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SavingsChoice_JS.aspx.cs" Inherits="ClearCostWeb.Scripts.SavingsChoice_JS"
    ContentType="application/x-javascript" EnableViewState="true" %>

function sendSliderData(e, u) {
    var staticData = {
            cchid: "<%= PrimaryCCHID %>",
            employerid: "<%= EmployerID %>",
            category: "<%= Category %>",
            review: ''
        };
    staticData.score = u.value;
    staticData.providerid = e.target.attributes["pid"].value;

    $.ajax({
        url: '<%= ResolveUrl("~/Handlers/SCRatings.ashx") %>',
        data: staticData,
        statusCode: {
            200: function() { $(".sliderInput." + e.target.attributes.id.value).show().fadeOut(500); },
            302: function() { }
        }
    });
}
$("div[id^='slider']").slider({
    orientation: "horizontal",
    range: "min",
    max: 10,
    value: -1,
    slide: refreshSwatch,
    change: refreshSwatch,
    start: changeSliderHandlers,
    stop: sendSliderData
});
