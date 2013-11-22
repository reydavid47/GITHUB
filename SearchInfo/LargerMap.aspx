<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/SearchMaster.master" AutoEventWireup="true" CodeFile="LargerMap.aspx.cs" Inherits="ClearCostWeb.SearchInfo.LargerMap" %>
<%@ MasterType VirtualPath="~/SearchInfo/SearchMaster.master" %>

<asp:Content ID="LargerMap_Content" ContentPlaceHolderID="MainBody" Runat="Server">
    <script type="text/javascript">
        var map;
        var directionsDisplay;
        var directionsService;
        function GenerateMapAndDirections() {
            if (!google.maps) {
                google.load("maps", "3.8",
                    {
                        "other_params": ((window.location.host.indexOf("localhost") > 0) ? "" : "client=gme-clearcosthealth&") + "sensor=false",
                        "callback": GenerateMapAndDirections
                    }
                );
            }
            else {
                directionsService = new google.maps.DirectionsService();
                directionsDisplay = new google.maps.DirectionsRenderer();
                var patientLat = $("#PATIENTLATITUDE").val();
                var patientLng = $("#PATIENTLONGITUDE").val();
                var pharmLat = $("#map_canvas").attr("pLat");
                var pharmLng = $("#map_canvas").attr("pLng");
                var start = new google.maps.LatLng(patientLat, patientLng);
                var end = new google.maps.LatLng(pharmLat, pharmLng);
                var myOptions = {
                    zoom: 7,
                    mapTypeId: google.maps.MapTypeId.ROADMAP,
                    center: end
                }
                map = new google.maps.Map(document.getElementById('map_canvas'), myOptions);
                var marker = new google.maps.Marker({
                    position: end,
                    map: map,
                    title: 'Facility'  //  lam, 20130724, MSF-298
                });
                marker.setZIndex(0);  //  lam, 20130724, MSF-298
                directionsDisplay.setMap(map);
                var request = {
                    origin: start,
                    destination: end,
                    travelMode: google.maps.DirectionsTravelMode.DRIVING
                };
                directionsService.route(request, function (response, status) {
                    if (status == google.maps.DirectionsStatus.OK) {
                        directionsDisplay.setDirections(response);
                    }
                });
            }
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () { GenerateMapAndDirections(); });
    </script>
    <div align="center">
        <p align="left">
            <a href="#" class="back" onclick="javascript:history.back(); return false;">Return to search result details</a>
        </p>
        <asp:Panel runat="server" ID="map_canvas" ClientIDMode="Static" align="center" style="height: 500px; width: 700px;" />
    </div>
</asp:Content>

