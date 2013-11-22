<%@ Page Title="" Language="C#" MasterPageFile="~/SignIn.master" AutoEventWireup="true"
    CodeFile="ResetPassword.aspx.cs" Inherits="ClearCostWeb.Public.ResetPassword" %>

<asp:Content ID="ResetPassword_Content" ContentPlaceHolderID="ContentPlaceHolder1"
    runat="Server">
    <asp:ScriptManager ID="smResetPassword" runat="server">
        <CompositeScript>
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/jquery-1.5.1.js" />
                <asp:ScriptReference Path="~/Scripts/jquery.ui.core.js" />
            </Scripts>
        </CompositeScript>
    </asp:ScriptManager>
    <h1>
        Password Reset
    </h1>
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
                    </asp:TableCell></asp:TableFooterRow></asp:Table></ContentTemplate></asp:UpdatePanel>
</asp:Content>
