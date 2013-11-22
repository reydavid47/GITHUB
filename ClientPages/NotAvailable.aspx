<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NotAvailable.aspx.cs" Inherits="ClearCostWeb.ClientPages.NotAvailable" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="robots" content="noindex,nofollow" />
    <title>ClearCost Health</title>
    <link href="../Styles/skin.css" rel="Stylesheet" type="text/css" />
    <link href="../Styles/old/style.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="main">
        <div id="topbar">
            <div id="logo">
                <img src="../Images/clearcosthealth_logo.gif" alt="ClearCost Health" width="469" height="70"
                    border="0" />
            </div>
            <div class="clearboth"></div>
        </div>
        <div id="ctop"></div>
        <div id="cmid">
            <div id="cbotdemo">
                <div class="intwide">
                    <h1>ClearCost Health is not yet available <asp:label ID="lblFor" runat="server" Text="for " Visible="false" /><asp:Label ID="lblClient" runat="server" /></h1>
                    <h2>Please check back soon.</h2>
                    <h3>Sincerely,<br />
                    ClearCost Health</h3>
                </div>
            </div>
        </div>
        <div class="clearboth"></div>
    </div>
    <div id="botbar">
        
        <p class="copyright">
            &copy; ClearCost Health. All Rights Reserved.
        </p>
        <div class="clearboth"></div>
    </div>
    </form>
</body>
</html>
