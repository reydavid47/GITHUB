<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="ClearCostWeb.Public.Index" %>
<%@ Register Src="~/Controls/unavPlaceHolder.ascx" TagPrefix="cch" TagName="UNav" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
  "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
<head>
<meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
<title>ClearCost Health - We bring price transparency to health</title>

<link href="Styles/old/style.css" rel="stylesheet" type="text/css" />

<script type="text/javascript" src="Scripts/jquery-1.4.1.min.js"></script>
<script type="text/javascript" src="Scripts/jquery.bxSlider.min.js"></script>
<script type="text/javascript">
    $(document).ready(function () {
        $('#graphs').bxSlider({
            mode: 'fade',
            speed: 180,
            auto: true,
            pause: 8000,
            controls: false
        });
    });
</script>
</head>
<body>

<div id="main">
<div id="topbar">

<div id="logo">
<a href="index.aspx"><img src="Images/clearcosthealth_logo.gif" alt="ClearCost Health" width="469" height="70" border="0" /></a>
</div>

<div id="tag">
<img src="Images/clearcosthealth_tag.gif" alt="We bring price transparency to health" width="343" height="21" border="0" />
</div><!-- end floatright -->

<div class="clearboth"></div>
</div>

<cch:UNav ID="unavIndex" runat="server" />

<div id="ctop"></div>
<div id="cbot">
	<div style="width:530px; float:left; margin-top: 15px; margin-left: 25px; margin-bottom: 40px;">
	<h1>The lack of price transparency leads to huge differences in provider costs in-network.</h1>
	<h2><a href="contact_us.aspx">Ask us for a free analysis of your plan's experience and we'll show you.</a> <a href="contact_us.aspx"><img src="Images/button_orange_arrow.png" alt="" width="18" height="18" border="0" /></a></h2>
	</div>
	<div style="float:right; margin-right: 25px; margin-top: 10px;">
	<ul id="graphs">
	<li><img src="Images/chart_echo.png" alt="" width="297" height="201" border="0" /></li>
	<li><img src="Images/chart_knee.png" alt="" width="297" height="201" border="0" /></li>
	<li><img src="Images/chart_liver.png" alt="" width="297" height="201" border="0" /></li>
	<li><img src="Images/chart_office.png" alt="" width="297" height="201" border="0" /></li>
	</ul>
	</div>
	<div class="clearboth"></div>
</div>



<div id="colleft">
<a href="PriceTransparency.aspx"><h3>Price Transparency</h3></a>
<p>
The lack of price transparency makes it virtually impossible to comparison shop and can frustrate patients who are being asked to shoulder more of the cost of care.
</p>
<b><a href="PriceTransparency.aspx">Learn more <img src="Images/button_orange_arrow.png" alt="" width="18" height="18" border="0" /></a></b>
</div>


<div id="colmid">
<a href="Expertise.aspx"><h3>Our Expertise</h3></a>
<p>
We provide a data-driven cost solution that springs from our expertise in medicine, reimbursement, software development and employee benefits.
</p><br/>
<b><a href="Expertise.aspx">Learn more <img src="Images/button_orange_arrow.png" alt="" width="18" height="18" border="0" /></a></b>
</div>


<div id="colright">
<a href="Services.aspx"><h3>Our Service</h3></a>
<p>
Our innovative service saves money for plan sponsors and participants and is low risk,  requiring no changes in plan design or vendors.
</p><br/>
<b><a href="Services.aspx">Learn more <img src="Images/button_orange_arrow.png" alt="" width="18" height="18" border="0" /></a></b>
</div>


<div class="clearboth"></div>
</div><!-- end main -->

<div id="botbar">
<div style="float:left">
<h4>ClearCostHealth.com</h4>
<div id="botnav">
<a href='PriceTransparency.aspx' >Price Transparency</a>&nbsp;|&nbsp;<a href='Expertise.aspx' >Our Expertise</a>&nbsp;|&nbsp;<a href='Services.aspx' >Our Service</a>&nbsp;|&nbsp;<a href='AboutUs.aspx' >Our Team</a>&nbsp;|&nbsp;<a href='Contact_Us.aspx' >Contact Us</a></div>


<!--<img src="Images/icon_facebook.gif" alt="" width="26" height="27" border="0" class="boticon" />-->
<a href="http://www.twitter.com/clearcosthealth" target="_new"><img src="Images/icon_twitter.gif" alt="" width="26" height="27" border="0" class="boticon" /></a>

</div>

<p class="copyright">
&copy;  ClearCost Health. All Rights Reserved.
</p>

<div class="clearboth"></div>
</div>
</body>
</html>