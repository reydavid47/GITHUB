<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Err.aspx.cs" Inherits="ClearCostWeb.Public.Err" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ClearCost Health</title>
    <link href="Styles/skin.css" rel="Stylesheet" type="text/css" />
    <link href="Styles/old/style.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="main">
        <div id="topbar">
            <div id="logo">
                <img src='<%= ResolveUrl("~/Images/clearcosthealth_logo.gif") %>' alt="ClearCost Health" width="469" height="70"
                    border="0" />
            </div>
            <%--<div id="tag">
                <img src='<%= ResolveUrl("~/Images/clearcosthealth_tag.gif") %>' alt="ClearCost Health" width="343" height="21"
                    border="0" />
            </div>--%>
            <div class="clearboth"></div>
        </div>
        <div id="ctop"></div>
        <div id="cmid">
            <div id="cbotdemo">
                <div class="intwide">
                    <h1>We've Got Our Best People On It</h1>
                    <asp:Literal ID="ltlErrorMessage" runat="server" Text="" />
                </div>
            </div>
        </div>
        <div class="clearboth"></div>
    </div>
    <div id="botbar">
        <div style="float:left;" id="demobotnav">
            <a href='<%= ResolveUrl("~/AboutUs.aspx") %>'>About Us</a>
            <img src='<%= ResolveUrl("~/Images/nav_div.gif") %>' alt="" width="1" height="17"
                border="0" />
            <a href='<%= ResolveUrl("~/contact_us.aspx") %>'>Contact Us</a>
        </div>
        <p class="copyright">
            &copy; ClearCost Health. All Rights Reserved.
        </p>
        <div class="clearboth"></div>
    </div>
    </form>
</body>
</html>
