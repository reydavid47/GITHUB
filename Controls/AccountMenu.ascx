<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountMenu.ascx.cs" Inherits="ClearCostWeb.Controls.AccountMenu" %>

<%@ Register Src="~/Controls/LearnMore.ascx" TagPrefix="cch" TagName="LearnMore" %>
<script type="text/javascript">
    function StartAccountMenu() {
        $("a#acctlink").AccountMenu({
            AccountMenuElement: "div#acctmenu",
            CloseSelector: "a.xclose",
            EditSelector: "a.display-edit-acct-settings",
            SaveSelector: "a.acct-save",
            CancelSelector: "a.acct-cancel",
            HeaderSelector: "tr.edit-acct-header"
        });
    }
</script>
<div id="demounav">
    <a id="acctlink">My Account</a><img src='../Images/nav_div.gif' alt='' width='1' height='17' border='0' />
    <asp:LoginStatus ID="lsMain" runat="server" LoginText="Sign in" LogoutAction="RedirectToLoginPage" LogoutPageUrl="~/sign_in.aspx" LogoutText="Sign Out"
        OnLoggedOut="Logout" />
</div>
<div id="acctmenu" style="visibility:hidden;">
    <b><a id="acctmenu-personal" ref="acct-personal">My Personal Information</a></b><br />
    <b><a id="acctmenu-settings" ref="acct-settings">My Account Settings</a></b><br />
    <b><a id="acctmenu-members" ref="acct-members">Other People on My Plan</a></b><br />
    <b><a id="acctmenu-notifications" ref="acct-notifications">Notification Settings</a></b><%-- <br />
    <b><a id="acctmenu-plan">My Health Plan</a></b><br /> --%>
</div>
<div id="acct-personal" class="overlay">
    <a class="xclose"></a>
    <h1>
        My Personal Information
    </h1>
    <p>
        Some of these items cannot be edited to be consistent with enrollment information.
        If any of the information below is incorrect, please contact <%= Microsoft.Security.Application.Encoder.HtmlEncode( this.contactText.Trim() ) %>.
    </p>
    <table cellspacing="0" cellpadding="6" border="0">
        <tr>
            <td>
                <b>Name:</b>
            </td>
            <td>
                <asp:Label ID="lblFullName" runat="server" Text="" />
            </td>
        </tr>
        <tr>
            <td>
                <b>Date of birth:</b>
            </td>
            <td>
                <asp:Label ID="lblDateOfBirth" runat="server" Text="" />
            </td>
        </tr>
        <tr>
            <td>
                <b>Health Plan:</b>
            </td>
            <td>
                <asp:Label ID="lblInsurer" runat="server" Text="" />
            </td>
        </tr>
        <% if (ClearCostWeb.ThisSession.MedicalPlanType != String.Empty)
           { %>
        <tr>
            <td>
                <b>Plan Type:</b>
            </td>
            <td>
                <asp:Label ID="lblMedicalPlan" runat="server" Text="" />
            </td>
        </tr>
        <% } %>
        <tr class="edit-acct-header">
            <td>
                <b>Home Address:</b>
            </td>
            <td>
                <asp:Label ID="lblAddress3Line" runat="server" Text="" />
            </td>
        </tr>
        <tr class="edit-acct-settings edit-acct-rowfirst acct-settings-address">
            <td>
                Address 1:
            </td>
            <td>
                <input type="text" class="boxed" name="addr1" />
            </td>
        </tr>
        <tr class="edit-acct-settings acct-settings-address">
            <td>
                Address 2:
            </td>
            <td>
                <input type="text" class="boxed" name="addr2" />
            </td>
        </tr>
        <tr class="edit-acct-settings acct-settings-address">
            <td>
                City:
            </td>
            <td>
                <input type="text" class="boxed" name="city" />
            </td>
        </tr>
        <tr class="edit-acct-settings acct-settings-address">
            <td>
                State:
            </td>
            <td>
                <input type="text" class="boxed" name="state" />
            </td>
        </tr>
        <tr class="edit-acct-settings acct-settings-address">
            <td>
                Zip:
            </td>
            <td>
                <input type="text" class="boxed" name="zip" />
            </td>
        </tr>
        <tr class="edit-acct-settings edit-acct-rowlast acct-settings-address">
            <td>
            </td>
            <td>
                <a class="submitlink">Save</a> &nbsp;&nbsp;&nbsp;<a class="submitlink acct-cancel">Cancel</a>
            </td>
        </tr>
        <tr class="edit-acct-header">
            <td>
                <b>Phone Number:</b>
            </td>
            <td>
                <asp:UpdatePanel ID="upPhone" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="lbtnPhoneUpdate" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Label ID="lblFormattedPhone" runat="server" Text="" />&nbsp;&nbsp;[&nbsp;<a class="display-edit-acct-settings">Edit</a>&nbsp;]
                    </ContentTemplate>
                </asp:UpdatePanel>                
            </td>
        </tr>
        <tr class="edit-acct-settings edit-acct-rowfirst acct-settings-address">
                <td>
                    Phone:
                </td>
                <td>                            
                    <asp:TextBox ID="txtPhoneUpdate" runat="server" name="phone" CssClass="boxed" style="margin-right:40px;" />
                </td>
            </tr>
        <tr class="edit-acct-settings edit-acct-rowlast acct-settings-address">
            <td>
            </td>
            <td>
                <asp:LinkButton ID="lbtnPhoneUpdate" runat="server" Text="Save" OnClick="UpdatePhoneNumber" CssClass="submitlink acct-save" ClientIDMode="Static" />
                &nbsp;&nbsp;&nbsp;<a class="submitlink acct-cancel">Cancel</a>
            </td>
        </tr>  
    </table>
</div>
<div id="acct-settings" class="overlay">
    <a class="xclose"></a>
    <h1>
        My Account Settings
    </h1>
    <table cellspacing="0" cellpadding="6" border="0">
        <tr class="edit-acct-header">
            <td>
                <b>Email:</b>
            </td>
            <td>
                <asp:UpdatePanel ID="upEmail" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="lnkbtnUpdateEmail" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Label ID="lblPatientEmail" runat="server" Text="" />&nbsp;&nbsp;[&nbsp;<a class="display-edit-acct-settings">Edit</a>&nbsp]
                    </ContentTemplate>
                </asp:UpdatePanel>                    
            </td>
        </tr>
        <tr class="edit-acct-settings acct-settings-email edit-acct-rowfirst">
            <td>
                Preferred Login Email:
            </td>
            <td>
                <asp:TextBox ID="txtUpdateEmail" runat="server" CssClass="boxed" name="preferredemail" ValidationGroup="vEmail" CausesValidation="true" />
                <%--<asp:RegularExpressionValidator ID="ValidEmail" runat="server" SetFocusOnError="true" ControlToValidate="txtUpdateEmail" Display="Dynamic"
                    ErrorMessage="<br />Must be a valid email address." ForeColor="Red" ValidationGroup="vEmail" />--%>
                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" SetFocusOnError="true" ControlToValidate="txtUpdateEmail" Display="Dynamic"
                    ErrorMessage="<br />A valid email address is required when attempting to Save." ForeColor="Red" ValidationGroup="vEmail" />                         
            </td>
        </tr>
        <tr class="edit-acct-settings acct-settings-email edit-acct-rowlast">
            <td>
            </td>
            <td>
                <asp:LinkButton id="lnkbtnUpdateEmail" runat="server" CssClass="submitlink acct-save" Text="Save" OnClick="UpdateEmailAddress" ValidationGroup="vEmail" CausesValidation="true" ClientIDMode="Static" />
                &nbsp;&nbsp;&nbsp;<a class="submitlink acct-cancel">Cancel</a>
            </td>
        </tr>
        <tr class="edit-acct-header">                        
            <td>
                <b>Password:</b>
            </td>
            <td>
                ********&nbsp;&nbsp;[&nbsp;<a class="display-edit-acct-settings">Change</a>&nbsp;]
            </td>
        </tr>
        <tr class="edit-acct-settings acct-settings-password edit-acct-rowfirst">
            <td colspan="2">
                <asp:UpdatePanel ID="upPassword" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" ClientIDMode="Static">
                    <ContentTemplate>
                        <asp:ChangePassword ID="cpChangePassword" runat="server" RenderOuterTable="false"
                            ChangePasswordFailureText="The password you entered does not meet the current requirements set.<br />Your password must be at least 1 character in length." 
                            OnChangedPassword="SetTimeout" >
                            <ChangePasswordTemplate>
                                <table style="margin-right:40px;">
                                    <tr>
                                        <td>
                                            <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">Current Password:</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="CurrentPassword" runat="server" TextMode="Password" CssClass="boxed"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server"
                                                ControlToValidate="CurrentPassword" ErrorMessage="Password is required."
                                                ToolTip="Password is required." ValidationGroup="<%= cpChangePassword.ClientID %>">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">New Password:</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="NewPassword" runat="server" TextMode="Password" CssClass="boxed"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server"
                                                ControlToValidate="NewPassword" ErrorMessage="New Password is required."
                                                ToolTip="New Password is required." ValidationGroup="<%= cpChangePassword.ClientID %>">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="boxed" TextMode="Password"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server"
                                                ControlToValidate="ConfirmNewPassword" ErrorMessage="Confirm New Password is required."
                                                ToolTip="Confirm New Password is required." ValidationGroup="<%= cpChangePassword.ClientID %>">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" align="center">
                                            <asp:CompareValidator ID="NewPasswordCompare" runat="server"
                                                ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword"
                                                Display="Dynamic"
                                                ErrorMessage="The Confirm New Password must match the New Password entry."
                                                ValidationGroup="<%= cpChangePassword.ClientID %>"></asp:CompareValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2" style="color:Red;">
                                            <asp:Literal ID="FailureText" runat="server" EnableViewState="false"></asp:Literal>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:LinkButton ID="ChangePasswordPushButton" runat="server" CssClass="submitlink acct-save"
                                                CommandName="ChangePassword" Text="Save" ValidationGroup="<%= cpChangePassword.ClientID %>" />
                                            &nbsp;&nbsp;&nbsp;
                                            <a class="submitlink acct-cancel">Cancel</a>
                                        </td>
                                    </tr>
                                </table>
                            </ChangePasswordTemplate>
                            <SuccessTemplate>
                                <table style="margin-bottom:40px;">
                                    <tr>
                                        <td colspan="2" align="center">
                                            Change Password Complete
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" align="center">
                                            Your password has been changed!
                                        </script>
                                        </td>
                                    </tr>
                                </table>
                            </SuccessTemplate>
                        </asp:ChangePassword>  
                    </ContentTemplate>                
                </asp:UpdatePanel>  
            </td>
        </tr>           
        <tr class="edit-acct-header">
            <td>
                <b>Security Question:</b>
            </td>
            <td>
                <asp:UpdatePanel ID="upQuestion" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False">
                    <Triggers>                            
                        <asp:AsyncPostBackTrigger ControlID="lbtnUpdateSecurityAnswer" EventName="Click" />                            
                    </Triggers>
                    <ContentTemplate>
                        <asp:Label ID="lblCurrentQuestion" runat="server" Text="" />&nbsp;&nbsp;[&nbsp;<a class="display-edit-acct-settings">Change</a>&nbsp;]
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr class="edit-acct-settings acct-settings-question edit-acct-rowfirst">
            <td>
                Current Question:
            </td>
            <td>
                <asp:DropDownList ID="ddlQuestion" runat="server" name="question">                                
                </asp:DropDownList>
            </td>
        </tr>
        <tr class="edit-acct-settings acct-settings-question">
            <td>
                Answer:
            </td>
            <td>
                <asp:TextBox ID="txtUpdateSecurityAnswer" runat="server" CssClass="boxed" name="answer" />
            </td>
        </tr>
        <tr class="edit-acct-settings acct-settings-password edit-acct-rowfirst">
            <td>
                Current Password:
            </td>
            <td>
                <asp:TextBox ID="txtUpdateSecurityPassword" runat="server" CssClass="boxed" name="currentpassword" TextMode="Password" />
            </td>
        </tr>
        <tr class="edit-acct-settings acct-settings-question edit-acct-rowlast">
            <td>
            </td>
            <td>
                <asp:LinkButton ID="lbtnUpdateSecurityAnswer" runat="server" CssClass="submitlink acct-save" OnClick="UpdateSecurityQuestion" Text="Save" ClientIDMode="Static" />
                    &nbsp;&nbsp;&nbsp;<a class="submitlink acct-cancel">Cancel</a>
            </td>
        </tr>
    </table>
</div>
<div id="acct-members" class="overlay">
    <a class="xclose"></a>
    <h1>
        Other People on My Plan
    </h1>
    <p>
        The members on your plan cannot be edited in order to be consistent with enrollment
        information. If any of the information below is incorrect, please contact <%= Microsoft.Security.Application.Encoder.HtmlEncode( this.contactText.Trim() ) %>.
    </p>
    <asp:Label id="lblOnlyMember" runat="server" Text="You are currently the only person covered under your plan." Visible="false" />
    <asp:Repeater ID="rptMemberAccess" runat="server">
        <ItemTemplate>
            <p>
                <b><%# Eval("FullName") %></b><%# (Eval("RelationShip").ToString() == String.Empty ? "" : (" - " + Eval("RelationShip"))) %>
            </p>
            <asp:Table CellSpacing="0" CellPadding="6" BorderWidth="0" Visible='<%# Boolean.Parse(Eval("ShowQuestions").ToString()) %>' runat="server">
                <asp:TableRow Visible='<%# !Boolean.Parse(Eval("UserToDep").ToString()) %>'>
                    <asp:TableCell>
                         <%  //  lam, 20130228, change Checked="false" --> Boolean.Parse(Eval("UserToDep").ToString())  %>
                        <asp:CheckBox ID="cbAllowSeeMy" runat="server" Text="" AutoPostBack="false" Checked='<%# Boolean.Parse(Eval("UserToDep").ToString()) %>' />
                    </asp:TableCell>
                    <asp:TableCell>
                        Allow <%# Eval("FullName") %>&nbsp;to see all my recent medical and prescription drug history.
                        <cch:LearnMore runat="server">
                            <TitleTemplate>
                                Sharing access with other members
                            </TitleTemplate>
                            <TextTemplate>
                                Checking this box will send a request to this family member for 
                                access to see all of his/her past medical history for the last 
                                twelve months. You can change this option at any time in the 
                                future by signing into your account and altering your settings 
                                in "My Account".
                            </TextTemplate>
                        </cch:LearnMore>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow Visible='<%# Boolean.Parse(Eval("UserToDep").ToString()) %>'>
                    <asp:TableCell>
                        <%  //  lam, 20130228, change Checked="false" --> Boolean.Parse(Eval("UserToDep").ToString())  %>
                        <asp:CheckBox ID="cbDisallowSeeMy" runat="server" Text="" AutoPostBack="false" Checked='<%# Boolean.Parse(Eval("UserToDep").ToString()) %>' />
                    </asp:TableCell>
                    <asp:TableCell>
                        You are currently allowing <%# Eval("FullName") %> to see your past medical care.
                        <cch:LearnMore runat="server">
                            <TitleTemplate>Sharing access with other members</TitleTemplate>
                            <TextTemplate>
                                By checking this box, you are agreeing to allow <%# Eval("FullName") %> to use the 
                                ClearCost Health service to see all of your past medical history for 
                                the last two years, including sensitive claims.  Sensitive claims 
                                include many medical services and drug prescriptions related to 
                                reproductive health, mental health, HIV/AIDs, and other sexually 
                                trasnmitted diseases.  You can change this option at any time in 
                                the future by logging into your account and altering your settings 
                                in "My Account".
                            </TextTemplate>
                        </cch:LearnMore>
                        <br /><b>Uncheck to disallow.</b>                            
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow Visible='<%# !Boolean.Parse(Eval("DepToUser").ToString()) %>'>
                    <asp:TableCell>
                        <asp:CheckBox ID="cbRequestToSee" runat="server" Text="" AutoPostBack="false" Checked="false" />
                    </asp:TableCell>
                    <asp:TableCell>
                        Request access to view all of <%# Eval("FullName") %>'s recent medical and prescription drug history.
                        <cch:LearnMore runat="server">
                            <TitleTemplate>Requesting Access To Other Members</TitleTemplate>
                            <TextTemplate>
                                Request access to view all of <%# Eval("FullName") %>'s recent medical and prescription drug history.
                            </TextTemplate>
                        </cch:LearnMore>                        
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow Visible='<%# !Boolean.Parse(Eval("DepToUser").ToString()) %>'>
                    <asp:TableCell>
                        
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="txtRequesteeEmail" runat="server" Text="" CssClass="boxed" CausesValidation="true" />
                        <cch:LearnMore ID="LearnMore1" runat="server">
                            <TitleTemplate>Requesting Access To Other Members</TitleTemplate>
                            <TextTemplate>
                                INVITE <%# Eval("FullName") %> TO USE CLEARCOST HEALTH If you enter an email address here, we will send your family member an invitation to register for ClearCost Health.
                            </TextTemplate>
                        </cch:LearnMore>    
                        <div>
                            <asp:CustomValidator ID="cvRequesteeEmail" runat="server" Display="Dynamic"
                                    ErrorMessage="Please enter a valid email address if you wish to view this dependent's recent medical history."
                                    ToolTip="Please enter a valid email address if you wish to view this dependent's recent medical history."
                                    ForeColor="Red" SetFocusOnError="true" ValidateEmptyText="true"
                                    OnServerValidate="ValidateEmail" ControlToValidate="txtRequesteeEmail" />
                        </div>                    
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow Visible='<%# Boolean.Parse(Eval("DepToUser").ToString()) %>'>
                    <asp:TableCell HorizontalAlign="Center">
                        <img src="../Images/StaticCheck.png" alt="" height="8" width="8" style="padding-top:6px;" />
                    </asp:TableCell>
                    <asp:TableCell>
                        You have been given access to view <%# Eval("FullName") %>'s past medical care.
                        <cch:LearnMore runat="server">
                            <TitleTemplate>Requesting access to other members</TitleTemplate>
                            <TextTemplate>
                                Request access to view all of <%# Eval("FullName") %>'s recent medical and prescription drug history.
                            </TextTemplate>
                        </cch:LearnMore>                           
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow 
                    Visible='<%# !(Boolean.Parse(Eval("UserToDep").ToString())||Boolean.Parse(Eval("DepToUser").ToString())) %>'>
                    <asp:TableCell ColumnSpan="2">
                        <br />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </ItemTemplate>
    </asp:Repeater>    
    <% if (ClearCostWeb.ThisSession.Dependents.ShowAccessQuestionSave)
        { %>
    <asp:LinkButton CssClass="submitlink acct-save" Text="Save" OnClick="UpdateAccess" runat="server" />
    &nbsp;&nbsp;&nbsp;<a class="submitlink acct-cancel" <%if(!HasValidationError) {%>onclick="cancelOverlay()"<%} %>>Cancel</a>
        <% if (HasValidationError) { %>
        <script language="javascript" type="text/javascript">
            document.getElementById("acct-members").style.display = 'block';
        </script>
        <% } %>
    <% }
        else
        { %>           
    &nbsp;&nbsp;&nbsp;<a class="submitlink acct-cancel" onclick="cancelOverlay()">Close</a>
    <% } %>        
</div>
<div id="acct-notifications" class="overlay">
    <a class="xclose"></a>
    <h1>
        Notification Settings
    </h1>
    <table cellspacing="0" cellpadding="6" border="0">
        <tr>
            <td colspan="2">
                <br />
            </td>
        </tr>
        <tr>
            <td>                           
            </td>
            <td>
                <asp:CheckBox ID="cbEmailAlerts" runat="server" Text="I would like to receive savings alerts by email." AutoPostBack="false" />
                <cch:LearnMore runat="server">
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
        <tr>
            <td colspan="2">
                <br />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <asp:CheckBox ID="cbTextAlerts" runat="server" Text="I would like to receive savings alerts by text message." AutoPostBack="false" />
                <cch:LearnMore runat="server">
                    <TitleTemplate>
                        Text Alerts
                    </TitleTemplate>
                    <TextTemplate>
                        If this option is selected, than we will reach out to you by text message to 
                        let you know when you have opportunities to save on your medical and prescription drug costs.
                    </TextTemplate>
                </cch:LearnMore>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                Preferred Contact Mobile Phone: &nbsp; &nbsp;
                <asp:TextBox ID="txtMobileAlert" runat="server" CssClass="boxed" name="mobile" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <asp:CheckBox ID="cbConciergeAlerts" runat="server" Text="If I qualify, I would like to participate in the ClearCost Health Shopper program to identify all the ways I can save." AutoPostBack="false" />
                <cch:LearnMore runat="server">
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
        <tr>
            <td colspan="2">
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:LinkButton ID="lnkbtnSaveAlerts" runat="server" Text="Save" CssClass="submitlink acct-save" OnClick="SaveAlerts" ClientIDMode="Static" />
                    &nbsp;&nbsp;&nbsp;<a class="submitlink" onclick="cancelOverlay();">Cancel</a>
            </td>
        </tr>
    </table>
</div>
<script type="text/javascript">
    StartAccountMenu();
</script>