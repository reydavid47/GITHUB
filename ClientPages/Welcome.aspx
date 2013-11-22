<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="Welcome.aspx.cs"
    Inherits="ClearCostWeb.ClientPages.Welcome" %>

<%@ Register Src="~/Controls/CustomMemberIDField.ascx" TagName="CustomMemberIDField" TagPrefix="cch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="robots" content="noindex,nofollow" /><meta name="viewport" content="initial-scale=1, maximum-scale=1" />
    <title>ClearCost Health</title>
    <%--<link href="../styles/landing.css" rel="stylesheet" type="text/css" />--%>
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
    <script src="../Scripts/jquery-1.5.1.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.zclip.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.bxSlider.min.js" type="text/javascript"></script>
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
    <script type="text/javascript">
        /*** Google Analytics ***/
        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-34081191-1']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="smWelcome" runat="server">
    </asp:ScriptManager>
    <asp:HiddenField ID="hfEmployerIDFromURL" runat="server" />
    <div id="main">
        <div id="topbar">
            <asp:Literal ID="ltlCompatabilityWarning" runat="server" Text="" />
            <div id="logo">
                <a href="landing.aspx">
                    <img src="../Images/clearcosthealth_logo.gif" alt="ClearCost Health" width="469"
                        height="70" border="0" /></a>
            </div>
            <asp:Panel ID="loginregister" runat="server" ClientIDMode="Static">
                Already have an account? &nbsp;
                <div class="button-signin">
                    <a href="../Sign_in.aspx">Sign In</a>
                </div>
            </asp:Panel>
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
                                <img src="../Images/button_register_on.png" vspace="31" alt="Register" width="220" height="55"
                                    border="0" />
                                <img src="../Images/button_account.png" vspace="31" alt="Create an Account" width="220"
                                    height="55" border="0" />
                                <img src="../Images/button_save.png" vspace="31" alt="Begin to Shop and Save" width="220"
                                    height="55" border="0" />
                                <asp:Image ID="imgLogo" runat="server" AlternateText="logo" Visible="false" style="max-width:156px;" />
                                <%--<img src="../Images/logo.png" style="margin-left: 25px;" alt="Starbucks" width="119"
                                    height="118" border="0" />--%>
                                <asp:Panel ID="pnlWelcome" runat="server" DefaultButton="lbtnContinue">
                                    <div id="landingform">
                                        <p>
                                            Please enter your last name (including any hyphens or suffixes, such as "Jr"), date of birth,
                                            and the last four digits of your Social Security Number<asp:Literal ID="ltlAltID" runat="server" Text=" (or your {0} ID)" Visible="false" />
                                             to continue.
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
                                            <p>
                                            </p>
                                            <p>
                                                Date of Birth<br />
                                                <asp:TextBox ID="txtMonth" runat="server" alt="MM" ClientIDMode="Static" 
                                                    CssClass="boxed boxed-date prefilled" MaxLength="2" name="month" Text="MM" />
                                                /
                                                <asp:TextBox ID="txtDay" runat="server" alt="DD" ClientIDMode="Static" 
                                                    CssClass="boxed boxed-date prefilled" MaxLength="2" name="day" Text="DD" />
                                                /
                                                <asp:TextBox ID="txtYear" runat="server" alt="YYYY" ClientIDMode="Static" 
                                                    CssClass="boxed boxed-date prefilled" MaxLength="4" name="year" Text="YYYY" />
                                                <div>
                                                    <asp:CustomValidator ID="cvDOB" runat="server" Display="Dynamic" 
                                                        ErrorMessage="Please enter a valid date of birth." ForeColor="Red" 
                                                        OnServerValidate="ValidateDOB" SetFocusOnError="true" ValidateEmptyText="true" 
                                                        ValidationGroup="upWelcome" />
                                                    <asp:CustomValidator ID="cvAGE" runat="server" Display="Dynamic"
                                                        ErrorMessage="You must be at least 18 years old to sign up for an account with ClearCost Health. Please call {0} or email {1} if you feel there has been an error." ForeColor="Red"
                                                        OnServerValidate="ValidateAge" SetFocusOnError="true" ValidateEmptyText="true"
                                                        ValidationGroup="upWelcome" />
                                                </div>
                                                <p>
                                                </p>    
                                                <p>
                                                    Last 4 digits of your Social Security number<br />
                                                    <asp:TextBox ID="txtSSN" runat="server" alt="XXXX" ClientIDMode="Static" 
                                                        CssClass="boxed boxed-ssn prefilled" MaxLength="4" name="ssn" Text="XXXX" />
                                                </p>
                                                <asp:Panel ID="pnlAdditionalID" runat="server">
                                                    <p>
                                                        <b>OR</b>
                                                    </p>
                                                    <p>
                                                        <%--<asp:TextBox ID="txtPremera1" runat="server" Text="XXXXXXXXX" alt="XXXXXXXXX" name="premera1"
                                                    MaxLength="9" CssClass="boxed boxed-pid1 prefilled" ClientIDMode="Static" />
                                                <asp:TextBox ID="txtPremera2" runat="server" Text="XX" alt="XX" name="premera2" MaxLength="2"
                                                    CssClass="boxed boxed-pid2 prefilled" ClientIDMode="Static" />--%>
                                                        <cch:CustomMemberIDField ID="cmidfWelcome" runat="server" />
                                                    </p>
                                                </asp:Panel>
                                                <p>
                                                    <div>
                                                        <asp:CustomValidator ID="cvSSNorID" runat="server" Display="Dynamic" 
                                                            ErrorMessage="Please enter a valid number for either your SSN{0}." 
                                                            ForeColor="Red" OnServerValidate="ValidateSSNorID" SetFocusOnError="true" 
                                                            ToolTip="Please enter a valid number for either your SSN{0}." 
                                                            ValidateEmptyText="true" ValidationGroup="upWelcome" />
                                                    </div>
                                                    <p>
                                                    </p>
                                                </p>
                                            </p>
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
                                    If you have questions or concerns, you can contact us at {0}.
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
                                    <asp:Literal ID="ltlUnavailable" runat="server">
                                        <p>
                                            Thank you for your interest in ClearCost Health. We are not able to confirm your eligibility at this time.
                                            This may be for one of the following reasons:
                                            <ul style="padding-left: 20px;margin-top:10px;">
                                                <li style="list-style-type:square;">
                                                    If you recently enrolled in health benefits, it takes some time for us to receive that information.
                                                    Please leave us your email address below and we will contact you as soon as your account is available.
                                                </li>
                                                <li style="list-style-type:square;">
                                                    There may have been an error in your previous submission. Please try clicking the back button, selecting "Register Now",
                                                    and entering your information again so that your name matches the information on your Medical ID card.
                                                </li>
                                                <li style="list-style-type:square;">
                                                    You may not have a Social Security Number on file with your health plan. If you think this is the case, please contact
                                                    your benefits administrator to update this information in their system.
                                                </li>
                                                <li style="list-style-type:square;">
                                                    If you have tried all of the above and are still having trouble, please contact ClearCost Health at {0} or leave us your email
                                                    address below.
                                                </li>
                                            </ul>
                                        </p>
                                    </asp:Literal>
                                    <%--<p>
                                        We're sorry, ClearCost Health is available only to partners currently enrolled in
                                        a Premera medical plan. If you recently enrolled, your account will become active
                                        on October 1st. Please leave us your email and we will contact you when your account
                                        is ready.
                                    </p>--%>
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
                                        Thank You
                                    </h1>
                                    <br />
                                    <p class="center">
                                        We will contact you when your account is ready.
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
