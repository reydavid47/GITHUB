<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="ClearCostWeb.Admin.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link rel="Shortcut Icon" href="../Images/favicon.ico" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="smAdmin" runat="server">
        <CompositeScript>
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/jquery-1.5.1.js" />
                <%--<asp:ScriptReference Path="~/Scripts/Notify.js" />--%>
            </Scripts>
        </CompositeScript>
    </asp:ScriptManager>
    <div style="float: right; width: 1150px; height: 800px;">
        <a style="cursor: pointer;" onclick="document.getElementById('iCC').src = '../CallCenter/Default.aspx';">
            Back To Call Center</a>
        <iframe id="iCC" src="../CallCenter/Default.aspx" frameborder="0" marginheight="0"
            marginwidth="0" scrolling="auto" style="width: 100%; height: 100%;border: 2px solid blue;"></iframe>
    </div>
    <asp:LoginStatus ID="lsLoginStatus" runat="server" LogoutAction="RedirectToLoginPage" />
    <div id="message">
    </div>
    <a href="Reports.aspx">Reporting Page</a>&nbsp;|&nbsp;<a href="ContentManagement.aspx">Content Management</a>
    <div>
        <asp:CreateUserWizard ID="CreateUserWizard1" runat="server">
            <WizardSteps>
                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
    </div>
    <asp:UpdatePanel ID="upUpdateUser" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Table ID="tblUpdateUser" runat="server" Width="360px">
                <asp:TableHeaderRow HorizontalAlign="Center">
                    <asp:TableCell ColumnSpan="2">
                        <h2>Update User</h2>
                    </asp:TableCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Left">
                        <asp:Label runat="server" AssociatedControlID="ddlUsers" Text="Users:" />
                        <%--<asp:SqlDataSource ID="dsUsers" runat="server" ConnectionString="<%$ ConnectionStrings:CCH_FrontEnd %>"
                            SelectCommand="SELECT [UserName], [UserId] FROM [vw_aspnet_Users]" />--%>
                        <asp:DropDownList ID="ddlUsers" runat="server" DataTextField="UserName"
                            DataValueField="UserId" AutoPostBack="true" OnSelectedIndexChanged="GetUserInfo" OnDataBound="UpdateDDL" />
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label runat="server" AssociatedControlID="ddlOrphans" Text="Potential Orphans:" />
                        <asp:DropDownList ID="ddlOrphans" runat="server" DataTextField="UserName"
                            DataValueField="UserId"  OnDataBound="UpdateOrphans" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <h3>Set Password</h3>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <asp:Label ID="Label1" runat="server" Text="New Password:" /><br />
                        <asp:TextBox ID="txtNewPassword" runat="server" /><br />
                        <asp:Button ID="btnSetPassword" runat="server" Text="Set Password" OnClick="btnSetPassword_Click" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <asp:Button ID="btnUnlock" runat="server" Text="Unlock User" Enabled="false" OnClick="UnlockUser" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <h3>
                            Set User Role</h3>
                        <asp:Literal ID="ltlUserRoles" runat="server" Text="" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <asp:CheckBoxList ID="cblRoles" runat="server" AutoPostBack="false" />
                        <asp:Button ID="btnSaveRoles" runat="server" Text="Update Roles" OnClick="UpdateUserRoles" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <h3>Delete User (Meant For Test Users)</h3>
                        <h4>Note: Does not remove from Employer Enrollments</h4>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <asp:CheckBox ID="cbAllRelated" runat="server" Text="And <u>ALL</u> related data"
                            Checked="false" AutoPostBack="false" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <div>
                            <img src="../Images/buttonDS.PNG" style="width: 140px; height: 25px; position: relative;
                                z-index: 1; float: left; top: 6px; left: 6px;" alt="" />
                            <asp:Button ID="btnConfirmDelete" runat="server" Text="Confirm and Delete" OnClick="ConfirmDelete"
                                Width="140px" Height="25px" Style="position: relative; top: 0px; left: -140px;
                                z-index: 2; float: left;" />
                        </div>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableFooterRow>
                    <asp:TableCell ColumnSpan="2">
                        <asp:Label ID="lblMessage" runat="server" Text="" Visible="true" />
                    </asp:TableCell>
                </asp:TableFooterRow>
            </asp:Table>
        </ContentTemplate>
    </asp:UpdatePanel>
</form>
</body>
</html>