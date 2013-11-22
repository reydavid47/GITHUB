<%@ Page Title="" Language="C#" MasterPageFile="~/SignIn.master" AutoEventWireup="true"
    CodeFile="ResetPassword.aspx.cs" Inherits="ClearCostWeb.ClientPages.ResetPassword" %>

<asp:Content ID="ResetPassword_Content" ContentPlaceHolderID="ContentPlaceHolder1"
    runat="Server">
    <asp:ScriptManager ID="smResetPassword" runat="server">
        <CompositeScript>
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/jquery-1.8.3.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery.ui.core.js" />
            </Scripts>
        </CompositeScript>
    </asp:ScriptManager>
    <h1>
        Password Reset
    </h1>
    <asp:Image ID="imgLogo" runat="server" AlternateText="logo" Visible="false" Style="max-width: 168px; top: 0px; right: 0px; position: absolute;" />
    <asp:UpdatePanel runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlVerify" runat="server" DefaultButton="VerifySubmitButton">
                <asp:Table ID="tblVerify" runat="server" CellPadding="1" CellSpacing="1" Style="border-collapse: collapse;">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell HorizontalAlign="Center" ColumnSpan="2">
                            <h3 style="color:Black;max-width:550px;">
                                Enter your Email Address and the last 4 of your SSN 
                                <asp:Label ID="lblAndMemberID" runat="server" Text="(or full Member ID) " Visible="false" />
                                to change your password.
                            </h3>
                        </asp:TableHeaderCell></asp:TableHeaderRow><asp:TableRow>
                        <asp:TableCell HorizontalAlign="Right" Style="vertical-align: middle;">
                            <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">
                                <h4 style="color:Black;">
                                    Email:
                                </h4>
                            </asp:Label>
                        </asp:TableCell><asp:TableCell>
                            <asp:TextBox ID="Email" runat="server" CssClass="boxed"></asp:TextBox>
                        </asp:TableCell></asp:TableRow><asp:TableRow ID="trSSN">
                        <asp:TableCell HorizontalAlign="Right" Style="vertical-align: middle;">
                            <asp:Label ID="SSNLabel" runat="server" AssociatedControlID="SSN">
                                <h4 style="color:Black;">
                                    Last 4 of SSN:
                                </h4>
                            </asp:Label>
                        </asp:TableCell><asp:TableCell>
                            <asp:TextBox ID="SSN" runat="server" CssClass="boxed" MaxLength="4"></asp:TextBox>
                        </asp:TableCell></asp:TableRow><asp:TableRow ID="trOR">
                        <asp:TableCell ColumnSpan="2" align="center">
                            <h4>
                                OR
                            </h4>
                        </asp:TableCell></asp:TableRow><asp:TableRow ID="trMemberID">
                        <asp:TableCell HorizontalAlign="Right" Style="vertical-align: middle;">
                            <asp:Label ID="MemberIDLabel" runat="server" AssociatedControlID="MemberID">
                                <h4 style="color:Black;">
                                    Member ID:
                                </h4>
                            </asp:Label>
                        </asp:TableCell><asp:TableCell>
                            <asp:TextBox ID="MemberID" runat="server" CssClass="boxed"></asp:TextBox>
                        </asp:TableCell></asp:TableRow><asp:TableRow>
                        <asp:TableCell HorizontalAlign="Center" ColumnSpan="2" ForeColor="Red">
                            <asp:Literal ID="VerifyFailureText" runat="server" EnableViewState="false"></asp:Literal>
                        </asp:TableCell></asp:TableRow><asp:TableFooterRow>
                        <asp:TableCell HorizontalAlign="Right" ColumnSpan="2">
                            <asp:LinkButton ID="VerifySubmitButton" runat="server" Text="Continue" CssClass="submitlink" ValidationGroup="tblVerify" OnClick="ValidateInput" OnClientClick="document.body.style.cursor = 'wait'; return true;" />
                        </asp:TableCell></asp:TableFooterRow></asp:Table></asp:Panel><asp:Panel ID="pnlReset" runat="server" Visible="false" DefaultButton="ChangeLinkButton">
                <asp:Table ID="tblReset" runat="server" Visible="false" CellPadding="1" CellSpacing="1" Style="border-collapse: collapse;">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell HorizontalAlign="Center" ColumnSpan="2">
                            <h3>Please choose a new password.</h3>
                        </asp:TableHeaderCell></asp:TableHeaderRow><asp:TableRow>
                        <asp:TableCell HorizontalAlign="Right" Style="vertical-align: middle;">
                            <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">
                                <h4>New Password:</h4>
                            </asp:Label>
                        </asp:TableCell><asp:TableCell>
                            <asp:TextBox ID="NewPassword" runat="server" TextMode="Password" CssClass="boxed"></asp:TextBox>
                        </asp:TableCell></asp:TableRow><asp:TableRow>
                        <asp:TableCell HorizontalAlign="Right" Style="vertical-align: middle;">
                            <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">
                                <h4>Confirm New Password:</h4>
                            </asp:Label>
                        </asp:TableCell><asp:TableCell>
                            <asp:TextBox ID="ConfirmNewPassword" runat="server" TextMode="Password" CssClass="boxed"></asp:TextBox>
                        </asp:TableCell></asp:TableRow><asp:TableRow>
                        <asp:TableCell HorizontalAlign="Left" ColumnSpan="2" Style="vertical-align: middle;">
                            <h4>
                                <asp:Label ID="lblQuestion" runat="server" Text="PasswordQuestion" />
                            </h4>
                        </asp:TableCell></asp:TableRow><asp:TableRow>
                        <asp:TableCell>
                        </asp:TableCell><asp:TableCell>
                            <asp:TextBox ID="PasswordQuestionAnswer" runat="server" CssClass="boxed"></asp:TextBox>
                        </asp:TableCell></asp:TableRow><asp:TableRow>
                        <asp:TableCell HorizontalAlign="Center" ColumnSpan="2" ForeColor="Red">
                            <asp:Literal ID="ChangeFailureText" runat="server" EnableViewState="False"></asp:Literal>
                        </asp:TableCell></asp:TableRow><asp:TableFooterRow>
                        <asp:TableCell HorizontalAlign="Right" ColumnSpan="2">
                            <%--<a style="cursor:pointer;" onclick="promptForEmail();">
                                <div>
                                    <img src="../Images/lg_arrow_square.png" style="position:relative;width:12px;height:12px;vertical-align:middle;" alt="" />
                                    <span style="color:#662D91;font-size:11px;position:relative;margin-left:2px;">
                                        I forgot my answer
                                    </span>
                                </div>
                            </a><br />--%>
                            <asp:LinkButton ID="ChangeLinkButton" runat="server" Text="Change Password" ValidationGroup="tblReset" CssClass="submitlink" OnClick="ChangePassword" OnClientClick="document.body.style.cursor = 'wait'; return true;" />
                        </asp:TableCell></asp:TableFooterRow></asp:Table></asp:Panel><asp:Table ID="tblSuccess" runat="server" Visible="false" CellPadding="1" CellSpacing="1" Style="border-collapse: collapse;">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell HorizontalAlign="Center" ColumnSpan="2">
                        Password Change Successful
                    </asp:TableHeaderCell></asp:TableHeaderRow><asp:TableRow>
                    <asp:TableCell>
                        Your password was successfully changed.
                    </asp:TableCell></asp:TableRow><asp:TableFooterRow>
                    <asp:TableCell HorizontalAlign="Right" ColumnSpan="2">
                        <asp:LinkButton ID="SuccessContinue" runat="server" CausesValidation="false" Text="Continue To Sign In Page" CssClass="submitlink" OnClick="ContinueToSearch" />
                    </asp:TableCell></asp:TableFooterRow></asp:Table></ContentTemplate></asp:UpdatePanel><div class="resetLightBox" align="center" style="padding:0px;margin:0px;top:0;left:0;right:0;bottom:0;width:100%;height:100%;background-color:rgba(0,0,0,0.5);position:fixed;display:none;">
        <div class="rlbContent" align="left" style="background-color:rgb(255,255,255);margin:0px auto;position:relative;top:50%;border:1px solid black;display:inline-block;padding:10px;width:425px;margin-top:-142px;">
            <h1>Password Reset Via Email Emailheight:100%;background-color:rgba(0,0,0,0.5);position:fixed;display:none;"> 
        <div class="rlbContent" align="left" style="background-color:rgb(255,255,255);margin:0px auto;position:relative;top:50%;border:1px solid black;display:inline-block;padding:10px;width:425px;margin-top:-142px;">
            <h1>Password Reset Via Email Email</h1><p>
                <h2 style="font-size:18px;">                
                    By clicking OK below, you confirm that you wish to receive an email to the address we have on file for you with a link allowing you to reset your email.
                    This link is specific to you and should not be shared with anyone else.
                    As a security measure, this link will expire 3 minutes after it is sent.
                    If you choose to receive this email, please delete it from your inbox after you've successfully reset your password. </h2></p><a style="cursor:pointer;float:left;" onclick="cancelEmailPrompt();"><div style="display:inline-block;">
                    <img src="../Images/lg_arrow_square.png" style="position:relative;vertical-align:middle;margin-bottom:2px;" alt="" /> <span style="color:#662D91;position:relative;margin-left:2px;font-weight:bold;">Cancel </span></div></a><a style="cursor:pointer;float:right;" onclick="sendResetEmail();"><div style="display:inline-block;">
                    <img src="../Images/lg_arrow_square.png" style="position:relative;vertical-align:middle;margin-bottom:2px;" alt="" /> <span style="color:#662D91;position:relative;margin-left:2px;font-weight:bold;">Ok </span></div></a></div></div><script type="text/javascript">
        function promptForEmail() {
            $(".resetLightBox").show();
        }
        function cancelEmailPrompt() {
            $(".resetLightBox").hide();
        }
        function sendResetEmail() {
            var theForm = document.forms['form1'];
            if (!theForm) {
                theForm = document.form1;
            }
            if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
                theForm.__EVENTTARGET.value = "SendEmail";
                theForm.__EVENTARGUMENT.value = "";
                theForm.submit();
            }
        }
    </script></asp:Content>