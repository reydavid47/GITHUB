<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="Review.aspx.cs"
    Inherits="ClearCostWeb.ClientPages.Review" %>

<%@ Register Src="~/Controls/LearnMore.ascx" TagPrefix="cch" TagName="LearnMore" %>
<%@ Register Src="~/Controls/SessionTimeout.ascx" TagPrefix="cch" TagName="SessionTimeout" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="robots" content="noindex,nofollow" /><meta name="viewport" content="initial-scale=1, maximum-scale=1" />
    <title>ClearCost Health</title>
    <%--<link href="../Styles/landing.css" rel="stylesheet" type="text/css" />--%>
    <%--lam, 20130311, MSF-273, include skin.css for style--%>
    <link href="../Styles/skin.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        div.overlayAlert
        {
            border:1px solid gray;
            width:175px;
            border-radius:10px;
            padding:10px;
            display:none;
            position:absolute;
            background-color:White;
            left:-195px;
            box-shadow:1px 1px 3px #fff;
            cursor:pointer;
            font-weight:bold;
            color:Red;
        }
        div.compatWarn 
        {
            margin-bottom: 30px;
            background-color: #fcb21f;
            font-size: 18px;
            font-weight: bold;
            color: #fff;
            display:inline-block;
            padding:5px 10px;
        }
    </style>
    <script src="../Scripts/jquery-1.5.1.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.zclip.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.bxSlider.min.js" type="text/javascript"></script>
    <script src="../Scripts/Timeout.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var showingText = true,
            showingEmailAlert = false,
            showingConcAlert = false;
        function SetPage() {
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
            if (showingText) { $("tr.textAlerts").show(); } else { $("tr.textAlerts").hide(); }
            if (showingConcAlert) { $("#cbConciergeAlerts").siblings(".overlayAlert").show(); } else { $("#cbConciergeAlerts").siblings(".overlayAlert").hide(); }
            if (showingEmailAlert) { $("#cbEmailAlerts").siblings(".overlayAlert").show(); } else { $("#cbEmailAlerts").siblings(".overlayAlert").hide(); }
            // open overlay when email form submitted
            $(".opensuccessoverlay").click(function () {
                $('<div />').addClass('whitescreen').appendTo('body').show();
                var layername = $(this).attr("rel");
                $('div#' + layername + 'overlay').show();
                setTimeout('document.location="landing.html"', 5000);
            });

            // assign copy functionality to "copylink" anchor tag; input#friendlink is copied
//            $('a#copylink').zclip({
//                path: 'js/ZeroClipboard.swf',
//                copy: $('input#friendlink').val()
//            });

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

            $("#cbTextAlerts").bind("change", function () {
                showingText = !showingText;
                $("tr.textAlerts").toggle();
            });
            $("#cbEmailAlerts").bind("change", function () {
                showingEmailAlert = !showingEmailAlert;
                $(this).siblings(".overlayAlert").css("display", ($(this).attr("checked") == "" ? "block" : "none"));
            });
            $("#cbConciergeAlerts").bind("change", function () {
                showingConcAlert = !showingConcAlert;
                $(this).siblings(".overlayAlert").css("display", ($(this).attr("checked") == "" ? "block" : "none"));
            });
            $(".overlayAlert").bind("click", function () { $(this).hide(); });

            $("<div>").addClass("button-gray-white-arrow-lg").attr("id", "cba").css("float", "left").appendTo($(".continueButtonAlt").parent("td"));
            $(".continueButtonAlt").appendTo("#cba");

            window.createWait = function() {
                $(".submitlink").toggle();
                __doPostBack('cuwReview$__CustomNav0$StepNextButtonLinkButton', '');
            }
            function setupOverride() {
                var elm = $(".submitlink"), elmP = elm.parent("td"),
                    newA = $("<a>").addClass("submitlink").append("Creating your account, please wait...").hide();
                    newA.css({ "cursor": "default", "backgroundImage": "url('../Images/reviewLoader.gif')" }).attr("id","aOverride");
                newA[0].onclick = function () { };
                elmP.append(newA);
                elm.attr("href", "javascript:window.createWait()");
                Sys.WebForms.PageRequestManager.getInstance().remove_pageLoaded(setupOverride);
            }
            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(setupOverride);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" autocomplete="off">
    <div id="main">
        <div id="topbar">
            <asp:Literal ID="ltlCompatabilityWarning" runat="server" Text="" />
            <div id="logo">
                <a href="landing.aspx">
                    <img src="../Images/clearcosthealth_logo.gif" alt="ClearCost Health" width="469" height="70"
                        border="0" /></a>
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
                    <%--
                      lam, 20130311, MSF-273, remove this from after <asp:CreateUserWizard ID="cuwReview" />
                      <CreateUserButtonStyle CssClass="continueButtonAlt" />
                    --%>
                    <asp:ScriptManager ID="smReview" runat="server" >
                    </asp:ScriptManager>                    
                    <asp:UpdatePanel ID="upReview" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:CreateUserWizard ID="cuwReview" runat="server" OnCreatedUser="cuwReview_CreatedUser"
                                OnCreateUserError="cuwReview_CreateUserError" LoginCreatedUser="true" OnCreatingUser="cuwReview_CreatingUser"
                                CreateUserButtonType="Link" CreateUserButtonStyle-CssClass="submitlink" CreateUserButtonText="Submit" DisableCreatedUser="false" >
                                <WizardSteps>
                                    <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                                        <ContentTemplate>                                            
                                            <img src="../Images/button_register.png" vspace="31" alt="Register" width="220"
                                                height="55" border="0"/>
                                            <img src="../Images/button_account_on.png" vspace="31" alt="Create an Account"
                                                width="220" height="55" border="0"/>
                                            <img src="../Images/button_save.png" vspace="31" alt="Begin to Shop and Save" width="220"
                                                height="55" border="0"/>
                                            <%--<img src="../Images/logo.png" style="margin-left: 25px;" alt="Starbucks"
                                                width="119" height="118" border="0"/>--%>
                                            <asp:Image ID="imgLogo" runat="server" AlternateText="logo" Visible="false" style="max-width:156px;" />
                                            <h2>
                                                Now create an account to finish registering.
                                            </h2>
                                            <br />
                                            <asp:TextBox ID="UserName" runat="server" Visible="false" />
                                            <table class="formtable">
                                                <tr>
                                                    <td colspan="2">
                                                        <h3>
                                                            Your Account Settings:</h3>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="Email" runat="server" CssClass="boxed" ValidationGroup="upReview" CausesValidation="true" AutoCompleteType="Disabled" style="float:left;" />
                                                        <div style="clear:both;">
                                                            <asp:CustomValidator ID="cvEmail" runat="server" ControlToValidate="Email" Display="Dynamic"
                                                                ErrorMessage="Please enter a valid email address" ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                ToolTip="Please enter a valid email address." ValidationGroup="upReview" OnServerValidate="ValidateEmail" />
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="ConfirmEmailLabel" runat="server" AssociatedControlID="ConfirmEmail">Confirm E-mail:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="ConfirmEmail" runat="server" CssClass="boxed" CausesValidation="true" ValidationGroup="upReview" AutoCompleteType="Disabled" style="float:left;" />
                                                        <div style="clear:both;">
                                                            <asp:CustomValidator ID="cvConfirmEmail" runat="server" ControlToValidate="ConfirmEmail" Display="Dynamic"
                                                                ErrorMessage="Please enter a valid email address" ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                ToolTip="Please enter a valid email address." ValidationGroup="upReview" OnServerValidate="ValidateEmail" />
                                                        </div>
                                                        <div style="clear:both;">
                                                            <asp:CustomValidator ID="cvEmailCompare" runat="server" Display="Dynamic"
                                                                ErrorMessage="Your email and confirmation must match." ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                ToolTip="Your email and confirmation must match." ValidationGroup="upReview" OnServerValidate="CompareEmail" />
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="boxed" CausesValidation="true" AutoCompleteType="Disabled" ValidationGroup="upReview" />
                                                        <span style="color:Gray;font-size:14px;font-weight:bold;vertical-align:top;">Minimum of 8 characters</span>
                                                        <div style="clear:both;">
                                                            <asp:CustomValidator ID="cvPassword" runat="server" ControlToValidate="Password" Display="Dynamic"
                                                                ErrorMessage="Please enter a valid password" ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                ToolTip="Please enter a valid password" ValidationGroup="upReview" OnServerValidate="ValidateEmpty" />
                                                        </div>                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">Confirm Password:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" CssClass="boxed" CausesValidation="true" AutoCompleteType="Disabled" ValidationGroup="upReview" />
                                                        <div style="clear:both;">
                                                            <asp:CustomValidator ID="cvConfirmPassword" runat="server" ControlToValidate="ConfirmPassword" Display="Dynamic"
                                                                ErrorMessage="Please enter a valid password" ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                ToolTip="Please enter a valid password" ValidationGroup="upReview" OnServerValidate="ValidateEmpty" />
                                                        </div>
                                                        <div style="clear:both;">
                                                            <asp:CustomValidator ID="cvPasswordCompare" runat="server" Display="Dynamic"
                                                                ErrorMessage="Your password and confirmation must match." ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                ToolTip="Your password and confirmation must match." ValidationGroup="upReview" OnServerValidate="ComparePassword" />
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="QuestionLabel" runat="server" AssociatedControlID="ddlQuestion">Security Question:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="Question" runat="server" Text="" Visible="false" Enabled="false" Width="0" Height="0" />
                                                        <asp:DropDownList ID="ddlQuestion" runat="server" name="question" autocomplete="off" DataTextField="PasswordQuestion">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer">Security Answer:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="Answer" runat="server" CssClass="boxed" CausesValidation="true" ValidationGroup="upReview" ToolTip="Case Sensitive" AutoCompleteType="Disabled" />
                                                        <span style="color:Gray;font-size:14px;font-weight:bold;vertical-align:top;">Case Sensitive</span>
                                                        <div style="clear:both;">
                                                            <asp:CustomValidator ID="cvAnswer" runat="server" ControlToValidate="Answer" Display="Dynamic"
                                                                ErrorMessage="Please enter an CASE SENSITIVE answer to your selected security question." ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                ToolTip="Please enter an CASE SENSITIVE answer to your selected security question." ValidationGroup="upReview" OnServerValidate="ValidateEmpty" />
                                                        </div>
                                                    </td>
                                                </tr>
                                                <%--  lam, 20130227, add RegistrationAreaCode, RegistrationFirstThree, and RegistrationLastFour for MSB-142  --%>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="PhoneLabel" runat="server" AssociatedControlID="RegistrationAreaCode">Phone Number:</asp:Label>
                                                    </td>
                                                    <td>
                                                        (&nbsp;<asp:TextBox ID="RegistrationAreaCode" runat="server" CssClass="boxed" MaxLength="3" Text="" alt="" style="width:35px;" ToolTip="3 Digit Area Code" AutoCompleteType="Disabled" />&nbsp;)&nbsp;
                                                        <asp:TextBox ID="RegistrationFirstThree" runat="server" CssClass="boxed" MaxLength="3" Text="" alt="" style="width:35px;" ToolTip="First 3 Digits" AutoCompleteType="Disabled" />&nbsp;-&nbsp;
                                                        <asp:TextBox ID="RegistrationLastFour" runat="server" CssClass="boxed" MaxLength="4" Text="" alt="" style="width:50px;" ToolTip="Last 4 Digits" AutoCompleteType="Disabled" />
                                                        <span style="color:Gray;font-size:14px;font-weight:bold;vertical-align:top;">(###) ###-####</span>
                                                        <div style="clear:both;">
                                                            
                                                            <asp:CustomValidator ID="cvRegistrationPhone" runat="server" Display="Dynamic"
                                                                    ErrorMessage="Please enter a valid phone number in this format (###) ###-####."
                                                                    ToolTip="Please enter a valid phone number in this format (###) ###-####."
                                                                    ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                    ValidationGroup="upReview" OnServerValidate="ValidateRegistrationPhone" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                            <hr />
                                            <asp:Panel ID="pnlOtherMembers" runat="server" Visible="false">
                                                <asp:Repeater ID="rptOtherMembers" runat="server">
                                                    <HeaderTemplate>
                                                        <table class="formtable" cellspacing="0" cellpadding="0" border="0">
                                                            <tr>
                                                                <td>
                                                                    <h3>
                                                                        Other People on Your Plan*
                                                                    </h3>
                                                                </td>
                                                            </tr>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td>
                                                                <b>
                                                                    <%# Eval("FirstName") %>&nbsp;<%# Eval("LastName") %>
                                                                </b>
                                                                <%# (((String)Eval("RelationShip")).Trim() == String.Empty ? String.Empty : " - " + Eval("RelationShip")) %>
                                                            </td>
                                                        </tr>
                                                        <tr style='<%# (Eval("ShowQuestions").ToString() == "True" ? "": "display:none;") %>'>
                                                            <td class="leftpadded">
                                                                <asp:Label ID="lblDepEmail" runat="server" Style="margin-left: 15px;"><%# Eval("FirstName") %>'s Email:</asp:Label>
                                                                <asp:TextBox ID="txtDepEmail" runat="server" Text="" CssClass="boxed" CausesValidation="true" ValidationGroup="upReview" AutoCompleteType="Disabled" />
                                                                <cch:LearnMore ID="lmDepEmail" runat="server">
                                                                    <TitleTemplate>
                                                                        Invite
                                                                        <%# Eval("FirstName") %>&nbsp;to use ClearCost Health
                                                                    </TitleTemplate>
                                                                    <TextTemplate>
                                                                        If you enter an email address here, we will send your family member an invitation to register for the ClearCost Health service.
                                                                    </TextTemplate>
                                                                </cch:LearnMore>
                                                                <div style="clear:both;">
                                                                    <asp:CustomValidator ID="cvDepEmail" runat="server" Display="Dynamic"
                                                                        ErrorMessage="Please enter a valid email address if you wish to view this dependent's recent medical history."
                                                                        ToolTip="Please enter a valid email address if you wish to view this dependent's recent medical history."
                                                                        ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                        ValidationGroup="upReview" OnServerValidate="ValidateEmail" />
                                                                </div>                                                            
                                                            </td>
                                                        </tr>
                                                        <tr style='<%# (Eval("ShowQuestions").ToString() == "True" ? "": "display:none;") %>'>
                                                            <td class="leftpadded">
                                                                <asp:CheckBox ID="cbAllowSeeMy" runat="server" Style="margin-left: 15px;" autocomplete="off" />
                                                                Allow <%# Eval("FullName") %>&nbsp;to see all my recent medical and prescription drug history.
                                                                <cch:LearnMore ID="lmAllowSeeMy" runat="server">
                                                                    <TitleTemplate>
                                                                        Sharing Access With Other Members
                                                                    </TitleTemplate>
                                                                    <TextTemplate>
                                                                        Checking this box will send a request to this family member for 
                                                                        access to see all of his/her past medical history for the last 
                                                                        twelve months. You can change this option at any time in the 
                                                                        future by signing into your account and altering your settings 
                                                                        in "My Account".
                                                                    </TextTemplate>
                                                                </cch:LearnMore>
                                                            </td>
                                                        </tr>
                                                        <tr style='<%# (Eval("ShowQuestions").ToString() == "True" ? "": "display:none;") %>'>
                                                            <td class="leftpadded">
                                                                <asp:CheckBox ID="cbRequestToSee" runat="server" Style="margin-left: 15px;" autocomplete="off" />
                                                                Request access to view all of <%# Eval("FullName") %>'s recent medical and prescription drug history.
                                                                <cch:LearnMore ID="lmRequestToSee" runat="server" Visible="false">
                                                                    <TitleTemplate>
                                                                        Requesting Access To Other Members
                                                                    </TitleTemplate>
                                                                    <TextTemplate>
                                                                        If you check this box, we will send an email to  <%# Eval("FullName") %> requesting that they allow
                                                                        you to see their past medical history for the last two years. Once they agree, you
                                                                        will be able to view their medical history through the ClearCost Health service.
                                                                    </TextTemplate>
                                                                </cch:LearnMore>
                                                            </td>
                                                        </tr>                                                    
                                                    </ItemTemplate>
                                                    <SeparatorTemplate>
                                                        <tr>
                                                            <td class="leftpadded">
                                                                <hr class="heavy" />
                                                            </td>
                                                        </tr>
                                                    </SeparatorTemplate>                                                
                                                    <FooterTemplate>
                                                        <tr>
                                                            <td>
                                                                <p class="disclaimer">
                                                                    * If any of the information above is incorrect, please contact your HR department.</p>
                                                            </td>
                                                        </tr>
                                                        </table>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                                <hr />
                                            </asp:Panel>
                                            <asp:Panel ID="pnlNotificationSettings" runat="server" Visible="false">
                                                <table class="formtable" cellspacing="0" cellpadding="0" border="0">
                                                    <tr>
                                                        <td>
                                                            <h3>
                                                                Your Notification Settings:
                                                            </h3>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <div class="overlayAlert">
                                                                Are you sure you don't want this?
                                                                If you unselect this option, we will be less able to help you save money on your health care expenses.
                                                            </div>
                                                            <asp:CheckBox ID="cbEmailAlerts" runat="server" Text="I would like to receive savings alerts by email."
                                                                AutoPostBack="false" Checked="true" ClientIDMode="Static" autocomplete="off" />
                                                            <cch:LearnMore ID="LearnMore1" runat="server">
                                                                <TitleTemplate>
                                                                    Email Alerts
                                                                </TitleTemplate>
                                                                <TextTemplate>
                                                                    If this option is selected, than we will reach out to you by email to let you know when you 
                                                                    have opportunities to save on your medical and prescription drug costs.
                                                                </TextTemplate>
                                                            </cch:LearnMore>
                                                        </td>
                                                    </tr>
                                                    <tr class="">
                                                        <td>
                                                            <asp:CheckBox ID="cbTextAlerts" runat="server" Text="I would like to receive savings alerts by text message."
                                                                AutoPostBack="false" Checked="true" ClientIDMode="Static" autocomplete="off" />
                                                            <cch:LearnMore ID="lmTextAlerts" runat="server">
                                                                <TitleTemplate>
                                                                    Text Message
                                                                </TitleTemplate>
                                                                <TextTemplate>
                                                                    If this option is selected, then we will reach out to you by text message, 
                                                                    in addition to email, to let you know when you have opportunities to save 
                                                                    on your medical and prescription drug costs. If its unchecked, we will only reach out to you by email.
                                                                </TextTemplate>
                                                            </cch:LearnMore>
                                                        </td>
                                                    </tr>
                                                    <tr class="textAlerts">
                                                        <td>
                                                            <asp:Label ID="Label1" runat="server" Style="margin-left: 15px;" Text="Preferred Contact Mobile Phone:" />
                                                        </td>
                                                    </tr>
                                                    <tr class="textAlerts">
                                                        <td>
                                                            (&nbsp;<asp:TextBox ID="txtAreaCode" runat="server" CssClass="boxed" MaxLength="3" Text="" alt="" style="width:35px;" AutoCompleteType="Disabled" />&nbsp;)
                                                            &nbsp;<asp:TextBox ID="txtFirstThree" runat="server" CssClass="boxed" MaxLength="3" Text="" alt="" style="width:35px;" AutoCompleteType="Disabled" />&nbsp;-&nbsp;
                                                            <asp:TextBox ID="txtLastFour" runat="server" CssClass="boxed" MaxLength="4" Text="" alt="" style="width:50px;" AutoCompleteType="Disabled" />
                                                            <div style="clear:both;">
                                                                <asp:CustomValidator ID="cvPhone" runat="server" Display="Dynamic"
                                                                    ErrorMessage="Please enter a valid phone number if you wish to receive savings alerts via text message."
                                                                    ToolTip="Please enter a valid phone number if you wish to receive savings alerts via text message."
                                                                    ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                    ValidationGroup="upReview" OnServerValidate="ValidatePhone" />
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <div class="overlayAlert">
                                                                Are you sure you don't want this?
                                                                If you unselect this option, we will be less able to help you save money on your health care expenses.
                                                            </div>
                                                            <asp:CheckBox ID="cbConciergeAlerts" runat="server" Text="If I qualify, I would like to participate in the ClearCost Health Shopper program to identify all the ways I can save."
                                                                AutoPostBack="false" Checked="true" ClientIDMode="Static" autocomplete="off" />
                                                            <cch:LearnMore ID="LearnMore2" runat="server">
                                                                <TitleTemplate>
                                                                    Health Shopper Program
                                                                </TitleTemplate>
                                                                <TextTemplate>
                                                                    If you are eligible, our Health Shopper Associates will reach out to you to identify 
                                                                    opportunities for you by phone to save big bucks on your medical and prescription drug costs.
                                                                </TextTemplate>
                                                            </cch:LearnMore>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <hr />
                                            </asp:Panel>
                                            <table class="formtable" cellspacing="0" cellpadding="0" border="0">
                                                <tr>
                                                    <td>
                                                        <h3 class="terms">
                                                            Terms of Use
                                                        </h3>
                                                        <asp:Panel ID="pnlTCVisible" runat="server" Visible="true">
                                                            <iframe src="TermsAndConditions.htm" style="width:750px;height:175px; overflow-y: scroll;border:1px solid rgb(102,102,102);"></iframe>
                                                            <p class="AcceptAgreement">
                                                                <asp:CheckBox ID="cbxTandCV" CssClass="AcceptAgreement" runat="server" style="display:block;"
                                                                    Text="I have read and agree to the Terms of Use listed here." />
                                                            </p>
                                                        </asp:Panel>
                                                        <asp:Panel ID="pnlTCHidden" runat="server" Visible="false">
                                                            <p class="AcceptAgreement">
                                                                <asp:CheckBox ID="cbxTandCH" CssClass="AcceptAgreement" runat="server" style="display:block;"
                                                                        Text="I have read and agree to the Terms of Use listed <a href='TermsAndConditions.htm'>here</a>." autocomplete="off" />
                                                            </p>
                                                        </asp:Panel>
                                                        <%--<asp:Literal ID="ltlTC" runat="server">
                                                            <iframe src="TermsAndConditions.htm" style="width:750px;height:175px; overflow-y: scroll;border:1px solid rgb(102,102,102);"></iframe>
                                                        </asp:Literal>
                                                        <p class="AcceptAgreement">
                                                            <asp:CheckBox ID="cbxTandC" CssClass="AcceptAgreement" runat="server" style="display:block;"
                                                                Text="I have read and agree to the Terms of Use listed here." />
                                                        </p>--%>
                                                        <div style="clear:both;">
                                                            <asp:CustomValidator ID="cvTnC" runat="server" Display="Dynamic"
                                                                    ErrorMessage="Please read and agree to the Terms of Use."
                                                                    ToolTip="Please read and agree to the Terms of Use."
                                                                    ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                    ValidationGroup="upReview" OnServerValidate="ValidateTnC" />
                                                            </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" style="color: Red;">
                                                        <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                            <br />   
                                            <table class="formtable">
                                                <tr>
                                                    <td>
                                                        <h3>How did you first hear about ClearCost Health?<i>(Optional)</i></h3>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Panel ID="pnlHearCCH" runat="server"></asp:Panel>
                                                        <asp:CustomValidator ID="cvRegistrationHearCCH" runat="server" Display="Dynamic"
                                                                    ErrorMessage="Please specify."
                                                                    ToolTip="Please specify."
                                                                    ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                                                    ValidationGroup="upReview" OnServerValidate="ValidateRegistrationHearCCH" />
                                                    </td>
                                                </tr>
                                           </table>                                         
                                        </ContentTemplate>
                                    </asp:CreateUserWizardStep>
                                    <asp:CompleteWizardStep ID="CompleteUserStep1" runat="server">
                                        <ContentTemplate>
                                            <img src="../Images/button_register.png" vspace="31" alt="Register" width="220"
                                                height="55" border="0"/>
                                            <img src="../Images/button_account.png" vspace="31" alt="Create an Account"
                                                width="220" height="55" border="0"/>
                                            <img src="../Images/button_save_on.png" vspace="31" alt="Begin to Shop and Save" width="220"
                                                height="55" border="0"/>
                                            <%--<img src="Images/logo.png" style="margin-left: 25px;" alt="Starbucks"
                                                width="119" height="118" border="0"/>--%>
                                            <asp:Image ID="imgLogo" runat="server" AlternateText="logo" Visible="false" style="max-width:156px;" />
                                            <h1>
                                                Success!</h1>
                                            <h2 class="smallerh2">
                                                <asp:Literal ID="ltlRegisterComplete" runat="server"
                                                    Text="" />
                                            </h2>
                                            <asp:Panel ID="pnlStartSearching" runat="server" Visible="false" style="margin-bottom:15px;">
                                                <div class="aligncenter">
                                                    <div class="button-gray-white-arrow-lg">
                                                        <asp:Literal ID="ltlStartSearching" runat="server">
                                                            <a href="{0}">
                                                                Start searching for health care
                                                            </a>
                                                        </asp:Literal>
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                            <div style="width:575px;height:323px;border:1px solid black;margin:0;padding:0">
                                                <%--<iframe width="575" height="323" src="https://www.youtube-nocookie.com/embed/E0G5dIpPH4I?rel=0" frameborder="0" allowfullscreen></iframe>--%>
                                                <div id="movieframe">
                                                    <script type="text/javascript" language="javascript" src="../Scripts/jwplayer.js"></script>
                                                    <div id="mediaspace" style="width:450px;height:254px;"></div>
                                                    <script type="text/javascript" language="javascript">
                                                        jwplayer("mediaspace").setup({
                                                            flashplayer: '<%= ResolveUrl("~/Images/player.swf") %>',
                                                            file: "http://www.youtube.com/watch?v=eMRf6NAooXU",
                                                            image: '<%= ResolveUrl("~/Images/screen.jpg") %>',
                                                            skin: '<%= ResolveUrl("~/Images/glow.zip") %>',
                                                            width: 450,
                                                            height: 254,
                                                            "controlbar.idlehide": true
                                                        });
                                                    </script>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:CompleteWizardStep>                                    
                                </WizardSteps>
                            </asp:CreateUserWizard> 
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cuwReview" EventName="ContinueButtonClick" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <div id="botbar">
        <div style="float: left;" id="demobotnav">
            <a href="AboutUs.aspx">About Us</a>
            <img src="../Images/nav_div.gif" alt="" width="1" height="17" border="0" />
            <a href="contact_us.aspx">Contact Us</a>
            <img src="../Images/nav_div.gif" alt="" width="1" height="17" border="0" />
            <a href="FAQ.aspx">FAQs</a>
        </div>
        <p class="copyright">
            &copy; ClearCost Health. All Rights Reserved.
        </p>
        <div class="clearboth">
        </div>
    </div>
    <%--<cch:SessionTimeout ID="smRegister" runat="server" NavURL="Welcome.aspx?timeout" PublicFacing="true"
        TimeOutText="Your registration will be reset due to inactivity in:" />--%>
    </form>
</body>
</html>
