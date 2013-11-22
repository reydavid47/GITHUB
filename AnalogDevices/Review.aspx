<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="Review.aspx.cs" Inherits="ClearCostWeb.AnalogDevices.Review" %>
<%@ Register Src="~/Controls/LearnMore.ascx" TagPrefix="cch" TagName="LearnMore" %>

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
                    <asp:ScriptManager ID="smReview" runat="server" >
                        <Scripts>
                            <asp:ScriptReference Path="~/Scripts/jquery-1.5.1.js" />
                            <asp:ScriptReference Path="~/Scripts/Review.js" />
                        </Scripts>
                    </asp:ScriptManager>
                    <asp:Image ID="iLogo" runat="server" AlternateText="Analog Devices Logo" ImageUrl="~/AnalogDevices/Images/analog_devices_logo.gif"
                        ImageAlign="Right" />
                    <h1>
                        Review
                    </h1>
                    <h2>
                        Success! Your account has been validated. Please review the information below; if
                        any of the information is incorrect, please contact your HR department:
                    </h2>
                    <br />
                    <%--<h3>
                        Your Personal Information
                    </h3>
                    <asp:Table runat="server" CssClass="formtable" CellPadding="0" CellSpacing="0" BorderWidth="0">
                        <asp:TableRow>
                            <asp:TableCell Width="200px">
                                Name:
                            </asp:TableCell>
                            <asp:TableCell>
                                <%= ThisSession.FirstName %>&nbsp;<%= ThisSession.LastName %>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell>
                                Date Of Birth:
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:Label ID="lblDateOfBirth" runat="server" Text="" />
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow ID="trHP" Visible="false">
                            <asp:TableCell>
                                Health Plan:
                            </asp:TableCell><asp:TableCell>
                                <%= ThisSession.Insurer %>
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow ID="trHPT" Visible="false">
                            <asp:TableCell>
                                Health Plan Type:
                            </asp:TableCell><asp:TableCell>
                                <%= ThisSession.HealthPlanType %>
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell>
                                Home Address:
                            </asp:TableCell><asp:TableCell>
                                <asp:Label ID="lblAddress" runat="server" Text="" />
                            </asp:TableCell></asp:TableRow>
                        <asp:TableFooterRow>
                            <asp:TableCell ColumnSpan="2">
                                <hr />
                            </asp:TableCell></asp:TableFooterRow>
                    </asp:Table>--%>
                    <asp:UpdatePanel ID="upReview" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:CreateUserWizard ID="cuwReview" runat="server" OnCreatedUser="cuwReview_CreatedUser"
                                OnCreateUserError="cuwReview_CreateUserError" LoginCreatedUser="true" OnCreatingUser="cuwReview_CreatingUser"
                                ContinueButtonType="Link" ContinueButtonStyle-CssClass="submitlink" ContinueButtonText="Access Account"
                                ContinueDestinationPageUrl="~/SearchInfo/Search.aspx" CreateUserButtonType="Link"
                                CreateUserButtonStyle-CssClass="submitlink" CreateUserButtonText="Register">
                                <ContinueButtonStyle CssClass="submitlink" />
                                <CreateUserButtonStyle CssClass="submitlink" />
                                <WizardSteps>
                                    <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                                        <ContentTemplate>
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
                                                        <asp:TextBox ID="Email" runat="server" CssClass="boxed" />
                                                        <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                                            ErrorMessage="E-mail is required." ToolTip="E-mail is required." ValidationGroup="cuwReview"
                                                            SetFocusOnError="true">*</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="ConfirmEmailLabel" runat="server" AssociatedControlID="ConfirmEmail">Confirm E-mail:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="ConfirmEmail" runat="server" CssClass="boxed" />
                                                        <asp:RequiredFieldValidator ID="ConfirmEmailRequired" runat="server" ControlToValidate="ConfirmEmail"
                                                            ErrorMessaged="Confirm E-mail is required." ToolTip="Confirm E-mail is required."
                                                            ValidationGroup="cuwReview" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="boxed" />
                                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                                            ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="cuwReview"
                                                            SetFocusOnError="true" Text="Password is required." ForeColor="Red" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">Confirm Password:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" CssClass="boxed" />
                                                        <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword"
                                                            ErrorMessage="Confirm Password is required." ToolTip="Confirm Password is required."
                                                            ValidationGroup="cuwReview" SetFocusOnError="true" Text="Password is required."
                                                            ForeColor="Red"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="QuestionLabel" runat="server" AssociatedControlID="ddlQuestion">Security Question:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlQuestion" runat="server" name="question" DataTextField="PasswordQuestion">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer">Security Answer:</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="Answer" runat="server" CssClass="boxed" />
                                                        <asp:RequiredFieldValidator ID="AnswerRequired" runat="server" ControlToValidate="Answer"
                                                            ErrorMessage="Security answer is required." ToolTip="Security answer is required."
                                                            ValidationGroup="cuwReview" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="2">
                                                        <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                                                            ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match."
                                                            ValidationGroup="cuwReview" SetFocusOnError="true"></asp:CompareValidator><asp:CompareValidator
                                                                ID="EmailCompare" runat="server" ControlToCompare="Email" ControlToValidate="ConfirmEmail"
                                                                Display="Dynamic" ErrorMessage="The E-mail and Confirmation E-mail must match."
                                                                ValidationGroup="cuwReview" SetFocusOnError="true"></asp:CompareValidator>
                                                    </td>
                                                </tr>
                                                <tfoot>
                                                    <tr>
                                                        <td colspan="2">
                                                            <hr />
                                                        </td>
                                                    </tr>
                                                </tfoot>
                                            </table>
                                            <asp:Repeater ID="rptOtherMembers" runat="server">
                                                <HeaderTemplate>
                                                    <table class="formtable" cellspacing="0" cellpadding="0" border="0">
                                                        <tr>
                                                            <td colspan="2">
                                                                <h3>
                                                                    Other Members on Your Plan:
                                                                </h3>
                                                            </td>
                                                        </tr>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td colspan="2">
                                                            <span>
                                                                <b>
                                                                    <%# Eval("FirstName") %>&nbsp;<%# Eval("LastName") %></b><%# (((String)Eval("RelationShip")).Trim() == String.Empty ? String.Empty : " - " + Eval("RelationShip")) %></span></td>
                                                    </tr>
                                                    <%--<tr>
                                                        <td width="120px">
                                                            Name:
                                                        </td>
                                                        <td>
                                                            <%# Eval("FirstName") %>&nbsp;<%# Eval("LastName") %></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Relationship:
                                                        </td>
                                                        <td>
                                                            <%# Eval("RelationShip") %>
                                                        </td>
                                                    </tr>--%>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblDepEmail" runat="server" Style="margin-left: 15px;"><%# Eval("FirstName") %>'s Email:</asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtDepEmail" runat="server" Text="" CssClass="boxed" />   
                                                            <cch:LearnMore ID="lmDepEmail" runat="server">
                                                                <TitleTemplate>
                                                                    Invite
                                                                    <%# Eval("FirstName") %>&nbsp;to use ClearCost Health
                                                                </TitleTemplate>
                                                                <TextTemplate>
                                                                    Proin elit arcu, rutrum commodo,
                                                                    vehicula tempus, commodo a, risus.
                                                                    Curabitur new arcu. Donec sollicitudin mi
                                                                    sit amet mauris. Nam elementum quam
                                                                    ullamcorper ante. Etiam aliquet massa et
                                                                    lorem.
                                                                </TextTemplate>
                                                            </cch:LearnMore>   
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:CheckBox ID="cbAllowSeeMy" runat="server" Style="margin-left: 15px;" />
                                                            Allow
                                                            <%# Eval("FullName") %>&nbsp;to see my past medical care
                                                            <cch:LearnMore ID="lmAllowSeeMy" runat="server">
                                                                <TitleTemplate>
                                                                    Sharing Access With Other Members
                                                                </TitleTemplate>
                                                                <TextTemplate>
                                                                    Proin elit arcu, rutrum commodo,
                                                                    vehicula tempus, commodo a, risus.
                                                                    Curabitur new arcu. Donec sollicitudin mi
                                                                    sit amet mauris. Nam elementum quam
                                                                    ullamcorper ante. Etiam aliquet massa et
                                                                    lorem.
                                                                </TextTemplate>
                                                            </cch:LearnMore>
                                                        </td>
                                                    </tr>
                                                    <tr style='<%# (Eval("Adult").ToString() == "True" ? "": "display:none;") %>'>
                                                        <td colspan="2">
                                                            <asp:CheckBox ID="cbRequestToSe" runat="server" Style="margin-left: 15px;" />
                                                            Request access to view
                                                            <%# Eval("FullName") %>'s past medical care
                                                            <cch:LearnMore ID="lmRequestToSee" runat="server">
                                                                <TitleTemplate>
                                                                    Requesting Access To Other Members
                                                                </TitleTemplate>
                                                                <TextTemplate>
                                                                    Proin elit arcu, rutrum commodo,
                                                                    vehicula tempus, commodo a, risus.
                                                                    Curabitur new arcu. Donec sollicitudin mi
                                                                    sit amet mauris. Nam elementum quam
                                                                    ullamcorper ante. Etiam aliquet massa et
                                                                    lorem.
                                                                </TextTemplate>
                                                            </cch:LearnMore>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <SeparatorTemplate>
                                                    <tr>
                                                        <td colspan="2">
                                                            <hr class="heavy" />
                                                        </td>
                                                    </tr>                                                    
                                                </SeparatorTemplate>
                                                <FooterTemplate>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                            <hr />
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
                                                        <asp:CheckBox ID="cbEmailAlerts" runat="server" Text="I would like to receive savings alerts by email."
                                                            AutoPostBack="false" Checked="true" />                                                        
                                                        <cch:LearnMore ID="lmEmailAlerts" runat="server">
                                                            <TitleTemplate>
                                                                Email Alerts
                                                            </TitleTemplate>
                                                            <TextTemplate>
                                                                Proin elit arcu, rutrum commodo,
                                                                vehicula tempus, commodo a, risus.
                                                                Curabitur new arcu. Donec sollicitudin mi
                                                                sit amet mauris. Nam elementum quam
                                                                ullamcorper ante. Etiam aliquet massa et
                                                                lorem.
                                                            </TextTemplate>
                                                        </cch:LearnMore>                                                            
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:CheckBox ID="cbTextAlerts" runat="server" Text="I would like to receive savings alerts by text message."
                                                            AutoPostBack="false" Checked="true" />
                                                        <cch:LearnMore ID="lmTextAlerts" runat="server">
                                                            <TitleTemplate>
                                                                Text Message
                                                            </TitleTemplate>
                                                            <TextTemplate>
                                                                Proin elit arcu, rutrum commodo,
                                                                vehicula tempus, commodo a, risus.
                                                                Curabitur new arcu. Donec sollicitudin mi
                                                                sit amet mauris. Nam elementum quam
                                                                ullamcorper ante. Etiam aliquet massa et
                                                                lorem.                                                            
                                                            </TextTemplate>
                                                        </cch:LearnMore>                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label runat="server" Style="margin-left: 15px;" Text="Preferred Contact Mobile Phone:" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtMobilePhone" runat="server" Style="margin-left: 15px;" CssClass="boxed" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:CheckBox ID="cbConciergeAlerts" runat="server" Text="I would like to participate in the Price Concierge program to identify all the ways I can save."
                                                            AutoPostBack="false" Checked="true" />
                                                        <cch:LearnMore ID="lmConciergeAlerts" runat="server">
                                                            <TitleTemplate>
                                                                Price Concierge Program
                                                            </TitleTemplate>
                                                            <TextTemplate>
                                                                Proin elit arcu, rutrum commodo,
                                                                vehicula tempus, commodo a, risus.
                                                                Curabitur new arcu. Donec sollicitudin mi
                                                                sit amet mauris. Nam elementum quam
                                                                ullamcorper ante. Etiam aliquet massa et
                                                                lorem.
                                                            </TextTemplate>
                                                        </cch:LearnMore>                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <hr />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <h3>
                                                            Terms and Conditions
                                                        </h3>
                                                        <div style="width: 415px; height: 150px; overflow: scroll; border: 1px #666 solid;">
                                                            <div style="width: 360px; margin: 10px;">
                                                                <p>
                                                                    Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor
                                                                    incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud
                                                                    exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute
                                                                    irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla
                                                                    pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia
                                                                    deserunt mollit anim id est laborum.
                                                                </p>
                                                            </div>
                                                        </div>
                                                        <asp:CheckBox ID="cbxTandC" CssClass="AcceptAgreement" runat="server" Text="Please read and agree to the Terms and Conditions listed here." />
                                                        <asp:Label ID="lblPleaseAgree" runat="server" Text="<br />Agreement to our Terms and Conditions is required."
                                                            Visible="false" Style="color: Red;" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" style="color: Red;">
                                                        <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:CreateUserWizardStep>
                                    <asp:CompleteWizardStep runat="server">
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
