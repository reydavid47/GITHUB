<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GoogleMap.ascx.cs" Inherits="ClearCostWeb.Controls.GoogleMap" %>

<script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=false" ></script>

<script type="text/javascript">
    function ReceiveServerData(arg, context) {
        var dirs = arg.split(",");
        var mapDiv = document.getElementById('map_canvas');
        var map = new google.maps.Map(mapDiv, {
            center: new google.maps.LatLng(dirs[0], dirs[1]),
            zoom: 13,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        });
    }
</script>

<div id="map_canvas" style="width:500px;height:400px">

</div>

<input type="button" value="CreateMap" onclick="CallServer(1,null)" />