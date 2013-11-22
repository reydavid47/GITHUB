<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Slider.ascx.cs" Inherits="ClearCostWeb.Controls.Slider" EnableViewState="true" %>


<div id="slider" runat="server" clientidmode="AutoID" style="margin:0px 5px;"></div>
<asp:HiddenField id="_SLIDER" runat="server" ClientIDMode="AutoID" Value="10" EnableViewState="true" />

<asp:Literal ID="ScriptWrapperStart" runat="server" Visible="false">
    <script type="text/javascript" language="javascript">
        function userDefinedSliderSuccess(e,u) {
</asp:Literal>
<asp:PlaceHolder runat="server" ID="ScriptPlaceHolder" />
<asp:Literal ID="ScriptWrapperEnd" runat="server" Visible="false">
        }
    </script>
</asp:Literal>
<script type="text/javascript" language="javascript">
    function defaultSliderSuccess(e,u) {
        <%= ControlPostBack %>
    }
    var sliderElem = $('#<%= this.slider.ClientID %>');
    var sliderOptions = {
        range: "max",
        max: <%= this.max %>,
        min: <%= this.min %>,
        value: <%= this.value %>,
        slide: function(e,u) { 
            $('#<%= this._SLIDER.ClientID %>')[0].value = u.value;
            sliderElem.find("a").attr("tooltip", u.value);
        }
    };

    if(typeof userDefinedSliderSuccess != "undefined")
        sliderOptions.stop = userDefinedSliderSuccess;
    else
        sliderOptions.stop = defaultSliderSuccess;

    sliderElem.slider(sliderOptions);
    $('#<%= this._SLIDER.ClientID %>')[0].value = $('#<%= this.slider.ClientID %>').slider("value");
    sliderElem.find("a").attr("tooltip", sliderElem.slider("value"));
</script>
