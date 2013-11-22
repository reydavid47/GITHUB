<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SessionTimeout.ascx.cs" Inherits="ClearCostWeb.Controls.SessionTimeout"  %>

<asp:HiddenField ID="TIMEOUT" runat="server" ClientIDMode="Static" Value="" />
<asp:HiddenField ID="TIMETILNOTIFY" runat="server" ClientIDMode="Static" Value="" />
<div class="stWrapper">
    <asp:Image ID="imgClose" runat="server" ImageUrl="~/Images/icon_x_sm.png" CssClass="stCloseImg" AlternateText="" />
    <h2 class="textCenter">ATTENTION</h2>
    <h3 class="textLeft"><asp:Literal ID="ltlTimeoutText" runat="server" Text="" /></h3>
    <h3 class="textCenter stCountdown"></h3>
    <!--h3 class="textcenter stPreventLink">To prevent this please click here</h3-->
    <h3 onclick="$(document).SessionTimeout({ NavUrl: '<%= NavURL %>', PingUrl: '<%= PingURL %>' });CloseAndRemove();" class="textcenter stPreventLink" >To prevent this please click here</h3>
</div>
<script type="text/javascript" language="javascript">$(document).SessionTimeout({ NavUrl: '<%= NavURL %>', PingUrl: '<%= PingURL %>' });</script>