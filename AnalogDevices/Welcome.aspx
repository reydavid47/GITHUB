<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="Welcome.aspx.cs" Inherits="ClearCostWeb.AnalogDevices.Welcome" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>ClearCost Health</title>
    <asp:Literal ID="ltlStyleSheets" runat="server" Text="" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="main">
        <div id="topbar">
            <div id="logo">
                <img src='<%= ResolveUrl("~/Images/clearcosthealth_logo.gif") %>' alt="ClearCost Health"
                    width="469" height="70" border="0" />
            </div>
            <div id="tag">
                <img src='<%= ResolveUrl("~/Images/clearcosthealth_tag.gif") %>' alt="ClearCost Health"
                    width="343" height="21" border="0" />
            </div>
            <div class="clearboth">
            </div>
        </div>
        <div id="ctop">
        </div>
        <div id="cmid">
            <div id="cbotdemo">
                <div class="intwide">
                    <asp:ScriptManager ID="smWelcome" runat="server">
                    </asp:ScriptManager>
                    <asp:Image ID="iLogo" runat="server" ImageUrl="~/AnalogDevices/Images/analog_devices_logo.gif"
                        AlternateText="Analog Devices Logo" ImageAlign="Right" />
                    <h1>
                        Welcome</h1>
                    <h2>
                        To ensure the privacy of your account, confirm your identity by providing the following
                        information:</h2>
                    <asp:Panel ID="pnlWelcome" runat="server" DefaultButton="lbtnContinue">
                        <asp:UpdatePanel ID="upWelcome" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Table ID="Table1" runat="server" CellPadding="0" CellSpacing="0" BorderWidth="0">
                                    <asp:TableRow>
                                        <asp:TableCell Width="300px">
                                            <h3 style="float: left;">
                                                Your First Name:
                                                <asp:RequiredFieldValidator ID="reqfieFirstName" runat="server" ControlToValidate="txtFirstName"
                                                    ErrorMessage="Required" SetFocusOnError="true" ValidationGroup="Reg" ForeColor="Red"
                                                    ToolTip="" Style="cursor: default; margin-left: 5px;" /></h3>
                                            <asp:TextBox ID="txtFirstName" runat="server" Text="" TextMode="SingleLine" CssClass="boxed"
                                                ValidationGroup="Reg" />
                                        </asp:TableCell>
                                        <asp:TableCell Style="vertical-align: bottom;">
                                            <asp:RegularExpressionValidator ID="regexFirstName" runat="server" ControlToValidate="txtFirstName"
                                                ErrorMessage="Your first name is not in an acceptable format" SetFocusOnError="true"
                                                ValidationGroup="Reg" ForeColor="Red" ToolTip="Can be up to 40 uppercase and lowercase characters and a few special characters that are common to some names"
                                                ValidationExpression="^[a-zA-Z''-'\s]{1,40}$" Style="cursor: default; margin-left: 5px;" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <h3>
                                                Your Last Name:
                                                <asp:RequiredFieldValidator ID="reqfieLastName" runat="server" ControlToValidate="txtLastName"
                                                    ErrorMessage="Required" SetFocusOnError="true" ValidationGroup="Reg" ForeColor="Red"
                                                    ToolTip="" Style="cursor: default; margin-left: 5px;" /></h3>
                                            <asp:TextBox ID="txtLastName" runat="server" Text="" TextMode="SingleLine" CssClass="boxed"
                                                ValidationGroup="Reg" />
                                        </asp:TableCell>
                                        <asp:TableCell Style="vertical-align: bottom;">
                                            <asp:RegularExpressionValidator ID="regexLastName" runat="server" ControlToValidate="txtLastName"
                                                ErrorMessage="Your last name is not in an acceptable format" SetFocusOnError="true"
                                                ValidationGroup="Reg" ForeColor="Red" ToolTip="Can be up to 40 uppercase and lowercase characters and a few special characters that are common to some names"
                                                ValidationExpression="^[a-zA-Z''-'\s]{1,40}$" Style="cursor: default; margin-left: 5px;" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <h3>
                                                Your Member ID:
                                                <asp:RequiredFieldValidator ID="reqfieMemID" runat="server" ControlToValidate="txtMemID"
                                                    ErrorMessage="Required" SetFocusOnError="true" ValidationGroup="Reg" ForeColor="Red"
                                                    ToolTip="" Style="cursor: default; margin-left: 5px;" /></h3>
                                            <asp:TextBox ID="txtMemID" runat="server" Text="" TextMode="SingleLine" CssClass="boxed"
                                                ValidationGroup="Reg" />
                                        </asp:TableCell>
                                        <asp:TableCell Style="vertical-align: bottom;">
                                            <%--<asp:RegularExpressionValidator ID="regexMemID" runat="server" ControlToValidate="txtMemId"
                                                ErrorMessage="Your Member ID is not in an acceptable format" SetFocusOnError="true"
                                                ValidationGroup="Reg" ForeColor="Red" ToolTip="Must consist of 3 characters followed by 6 numeric characters"
                                                ValidationExpression="^[a-zA-Z]{3}\d{6}$" Style="cursor: default; margin-left: 5px;" />--%>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <h3>
                                                Your Date Of Birth:
                                                <asp:RequiredFieldValidator ID="reqfieDOB" runat="server" ControlToValidate="txtDOB"
                                                    ErrorMessage="Required" SetFocusOnError="true" ValidationGroup="Reg" ForeColor="Red"
                                                    ToolTip="" Style="cursor: default; margin-left: 5px;" /></h3>
                                            <asp:TextBox ID="txtDOB" runat="server" Text="" TextMode="SingleLine" CssClass="boxed"
                                                ValidationGroup="Reg" />
                                        </asp:TableCell>
                                        <asp:TableCell Style="vertical-align: bottom;">
                                            <asp:RegularExpressionValidator ID="regexDOB" runat="server" ControlToValidate="txtDOB"
                                                ErrorMessage="Your Date Of Birth is not in an acceptable format" SetFocusOnError="true"
                                                ValidationGroup="Reg" ForeColor="Red" ToolTip="Must be in MM/DD/YYYY format"
                                                ValidationExpression="^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$"
                                                Style="cursor: default; margin-left: 5px;" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableFooterRow>
                                        <asp:TableCell ColumnSpan="2">
                                            <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false">
                                                Our appologies but there was an error validating your enrollment status.  Please try again in a little while.
                                            </asp:Label>
                                            <asp:Label ID="lblNotFound" runat="server" ForeColor="Red" Visible="false">
                                                We were not able to validate your enrollment with the information you provided.<br />
                                                Please check the information you've entered and follow up with your HR department if you continue to have trouble.
                                            </asp:Label>
                                        </asp:TableCell>
                                    </asp:TableFooterRow>
                                </asp:Table>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="lbtnContinue" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <asp:LinkButton ID="lbtnContinue" runat="server" CssClass="submitlink" OnClick="Continue"
                            CausesValidation="true" Text="Continue" ValidationGroup="Reg" />
                    </asp:Panel>
                </div>
            </div>
        </div>
        <div class="clearboth">
        </div>
    </div>
    <div id="botbar">
        <div style="float: left;" id="demobotnav">
            <a href='<%= ResolveUrl("~/AboutUs.aspx") %>'>About Us</a>
            <img src='<%= ResolveUrl("~/Images/nav_div.gif") %>' alt="" width="1" height="17"
                border="0" />
            <a href='<%= ResolveUrl("~/contact_us.aspx") %>'>Contact Us</a>
        </div>
        <p class="copyright">
            &copy; ClearCost Health. All Rights Reserved.
        </p>
        <div class="clearboth">
        </div>
    </div>
    </form>
</body>
</html>
