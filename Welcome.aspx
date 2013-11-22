<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="Welcome.aspx.cs"
    Inherits="ClearCostWeb.Public.Welcome" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="robots" content="noindex,nofollow" />
    <title>ClearCost Health</title>
    <link href="styles/landing.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        div.compatWarn
        {
            margin-bottom: 30px;
            background-color: #fcb21f;
            font-size: 18px;
            font-weight: bold;
            color: #fff;
            display: inline-block;
            padding: 5px 10px;
        }
    </style>
    <script src="Scripts/jquery-1.5.1.js" type="text/javascript"></script>
    <script src="Scripts/jquery.zclip.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.bxSlider.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('input.prefilled').bind({
                click: function () {
                    $(this).val('');
                },
                focus: function () {
                    $(this).select();
                },
                blur: function () {
                    var valInit = $(this).attr("alt");
                    var valNew = $(this).val().trim();
                    if (valNew == '') $(this).val(valInit);
                }
            });

            // assign copy functionality to "copylink" anchor tag; input#friendlink is copied
            $('a#copylink').zclip({
                path: 'js/ZeroClipboard.swf',
                copy: $('input#friendlink').val()
            });

            /* for the More Info icons */
            $('div.learnmore').click(
		function () {
		    $('div.moreinfo:visible').css("z-index", "5").fadeOut(500);
		    $(this).find('div.moreinfo:hidden').css("z-index", "1005").fadeIn(500);
		}
	);
            $('div.moreinfo').click(
		function () { $(this).css("z-index", "5").fadeOut(500); }
	);

        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="smWelcome" runat="server">
    </asp:ScriptManager>
    <div id="main">
        <div id="topbar">
            <asp:Literal ID="ltlCompatabilityWarning" runat="server" Text="" />
            <div id="logo">
                <a href="landing.aspx">
                    <img src="Images/clearcosthealth_logo.gif" alt="ClearCost Health" width="469"
                        height="70" border="0" /></a>
            </div>
            <div id="loginregister">
                Already have an account? &nbsp;
                <div class="button-signin">
                    <a href="Sign_in.aspx">Sign In</a></div>
            </div>
            <!-- end floatright -->
            <div class="clearboth block">
            </div>
        </div>
        <div id="ctop">
        </div>
        <div id="cmid">
            <div id="cbotdemo">
                <div class="intwide">
                    <asp:UpdatePanel ID="upWelcome" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="pnlReg" runat="server">
                                <img src="Images/button_register_on.png" vspace="31" alt="Register" width="220" height="55"
                                    border="0" />
                                <img src="Images/button_account.png" vspace="31" alt="Create an Account" width="220"
                                    height="55" border="0" />
                                <img src="Images/button_save.png" vspace="31" alt="Begin to Shop and Save" width="220"
                                    height="55" border="0" />
                                <asp:Literal ID="ltlLogo" runat="server" Text="" />
                                <%--<img src="Images/logo.png" style="margin-left: 25px;" alt="Starbucks" width="119"
                                    height="118" border="0" />--%>
                                <asp:Panel ID="pnlWelcome" runat="server" DefaultButton="lbtnContinue">
                                    <div id="landingform">
                                        <p>
                                            Please enter your name, date of birth and the last four digits of your Social Security
                                            number (or your Premera ID).
                                        </p>
                                        <%--<p>
                                        <br />
                                        First Name<br />
                                        <asp:TextBox ID="txtFirstName" name="first_name" runat="server" Text="" TextMode="SingleLine" CssClass="boxed" ClientIDMode="Static" />
                                    </p>--%>
                                        <p>
                                            Last Name<br />
                                            <asp:TextBox ID="txtLastName" name="last_name" runat="server" Text="" TextMode="SingleLine"
                                                CssClass="boxed" ClientIDMode="Static" CausesValidation="true" ValidationGroup="upWelcome" />
                                            <div>
                                                <asp:CustomValidator ID="cvLastName" runat="server" ControlToValidate="txtLastName"
                                                    Display="Dynamic" ErrorMessage="Please enter your last name." ForeColor="Red"
                                                    SetFocusOnError="true" ValidateEmptyText="true" ValidationGroup="upWelcome" OnServerValidate="ValidateEmpty" />
                                            </div>
                                        </p>
                                        <p>
                                            Date of Birth<br />
                                            <asp:TextBox ID="txtMonth" name="month" Text="MM" alt="MM" MaxLength="2" runat="server"
                                                CssClass="boxed boxed-date prefilled" ClientIDMode="Static" />
                                            /
                                            <asp:TextBox ID="txtDay" name="day" Text="DD" alt="DD" MaxLength="2" runat="server"
                                                CssClass="boxed boxed-date prefilled" ClientIDMode="Static" />
                                            /
                                            <asp:TextBox ID="txtYear" name="year" Text="YYYY" alt="YYYY" MaxLength="4" runat="server"
                                                CssClass="boxed boxed-date prefilled" ClientIDMode="Static" />
                                            <div>
                                                <asp:CustomValidator ID="cvDOB" runat="server" Display="Dynamic" ErrorMessage="Please enter a valid date of birth."
                                                    ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true" ValidationGroup="upWelcome"
                                                    OnServerValidate="ValidateDOB" />
                                            </div>
                                        </p>
                                        <p>
                                            Last 4 digits of your Social Security number<br />
                                            <asp:TextBox ID="txtSSN" runat="server" Text="XXXX" alt="XXXX" name="ssn" MaxLength="4"
                                                CssClass="boxed boxed-ssn prefilled" ClientIDMode="Static" />
                                        </p>
                                        <asp:Panel ID="pnlPremera" runat="server" Visible="false">                                           
                                            <p>
                                                <b>OR</b>
                                            </p>
                                            <p>
                                                Your Premera ID&nbsp;(include two-digit suffix)<br />
                                                <asp:TextBox ID="txtPremera1" runat="server" Text="XXXXXXXXX" alt="XXXXXXXXX" name="premera1"
                                                    MaxLength="9" CssClass="boxed boxed-pid1 prefilled" ClientIDMode="Static" />
                                                <asp:TextBox ID="txtPremera2" runat="server" Text="XX" alt="XX" name="premera2" MaxLength="2"
                                                    CssClass="boxed boxed-pid2 prefilled" ClientIDMode="Static" />
                                            </p>                                        
                                        </asp:Panel> 
                                        <p>
                                            <div>
                                                <asp:CustomValidator ID="cvSSNorID" runat="server" Display="Dynamic" ErrorMessage="Please enter a valid number for either you SSN or Member ID."
                                                    ToolTip="Please enter a valid number for either you SSN or Member ID." ForeColor="Red"
                                                    SetFocusOnError="true" ValidateEmptyText="true" ValidationGroup="upWelcome" OnServerValidate="ValidateSSNorID" />
                                            </div>
                                        </p>
                                    </div>
                                    <br />
                                    <span style="color: Red;">
                                        <asp:Literal ID="ltlFailure" runat="server" Text="" /></span>
                                    <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false">
                                    Our appologies but there was an error validating your enrollment status.  Please try again in a little while.
                                    </asp:Label>
                                    <asp:Label ID="lblNotFound" runat="server" ForeColor="Red" Visible="false">
                                    There was an internal error validating your enrollment, please try again in a little while.<br />
                                    If you have questions or concerns, you can contact us at 877-728-9020.
                                    </asp:Label>
                                    <br />
                                    <br />
                                    <div class="button-gray-white-arrow-lg">
                                        <asp:LinkButton ID="lbtnContinue" runat="server" OnClick="Continue" Text="Continue"
                                            CausesValidation="true" ValidationGroup="upWelcome" />
                                    </div>
                                    <!-- end landingform -->
                                </asp:Panel>
                            </asp:Panel>
                            <asp:Panel ID="pnlCapture" runat="server" Visible="false">
                                <div id="landingform">
                                    <p>
                                        We're sorry, ClearCost Health is available only to partners currently enrolled in
                                        a Premera medical plan. If you recently enrolled, your account will become active
                                        on October 1st. Please leave us your email and we will contact you when your account
                                        is ready.
                                    </p>
                                    <p>
                                        Your email address:<br />
                                        <asp:TextBox ID="txtCapEmail" runat="server" Text="" CssClass="boxed" name="email" />
                                        <div>
                                            <asp:CustomValidator ID="cvEmail" runat="server" ControlToValidate="txtCapEmail"
                                                Display="Dynamic" ErrorMessage="Please enter a valid email address." ForeColor="Red"
                                                SetFocusOnError="true" ValidateEmptyText="true" ValidationGroup="pnlCapture"
                                                OnServerValidate="ValidateEmail" />
                                        </div>
                                    </p>
                                </div>
                                <!-- end landingform -->
                                <br />
                                <br />
                                <div class="button-gray-white-arrow-lg">
                                    <asp:LinkButton ID="lbtnCapContinue" runat="server" Text="Continue" CssClass="pointer opensuccessoverlay" ClientIDMode="Static"
                                        rel="success" OnClick="CaptureEmail" />
                                </div>
                                <!-- success overlay -->
                                <div id="successoverlay" class="success">
                                    <h1 class="center">
                                        Success!
                                    </h1>
                                    <br />
                                    <p class="center">
                                        You are now being redirected to the ClearCost Health home page.
                                    </p>
                                </div>
                                <!-- end successoverlay -->
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                            </asp:Panel>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="lbtnContinue" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="lbtnCapContinue" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <!-- end intwide -->
            </div>
            <!-- end cbot -->
        </div>
        <!-- end cmid -->
        <div class="clearboth">
        </div>
    </div>
    <!-- end main -->
    <div id="botbar">
        <div style="float: left" id="demobotnav">
            <a href='aboutus.aspx'>About Us</a><img src="../Images/nav_div.gif" alt="" width="1"
                height="17" border="0" /><a href='contact_us.aspx'>Contact Us</a>
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
