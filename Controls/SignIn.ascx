<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SignIn.ascx.cs" Inherits="ClearCostWeb.Controls.SignIn" %>

<asp:Login ID="MainLogin" runat="server" OnLoggedIn="MainLogin_LoggedIn" 
    onloginerror="MainLogin_LoginError">
    <LayoutTemplate>
        <table cellpadding="10" cellspacing="0" border="0" class="formtable" style="position:relative;top:100px;">
            <tr>
                <td>
                    <h3 style="float:left;">
                        Sign In</h3>
                    <div class="clearboth"></div>
                    <asp:TextBox ID="UserName" runat="server" Text="Enter email address" Width="200px" TabIndex="1" autocomplete="off"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                        ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="ctl00$MainLogin">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtPasswordWatermark" runat="server" Text="Enter Password" Width="200px" TabIndex="2" autocomplete="off"></asp:TextBox>
                    <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="200px" TabIndex="3" autocomplete="off"></asp:TextBox>
                    <%--<asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                        ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="ctl00$MainLogin">*</asp:RequiredFieldValidator>--%>                    
                    <div style="width:100px; position: relative; float:left; padding-left:109px;">
                        <a href="ResetPassword.aspx">
                            <div>
                                <asp:Image runat="server" ImageUrl="~/Images/lg_arrow_square.png" AlternateText="" style="position:relative; float:left; width:12px; height:12px ;margin-top:3px;" />
                                <asp:Label runat="server" Text="Forgot Password" ForeColor="#662D91" style="font-size:11px; position:relative; margin-left:2px;" />
                            </div>
                        </a>
                    </div>        
                </td>
            </tr>
            <tr>
                <td align="center" style="color: Red;">
                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:ImageButton ID="LoginButton" runat="server" CommandName="Login" ImageUrl="~/Images/btnContinue.PNG"
                        ValidationGroup="ctl00$MainLogin" ImageAlign="Left" TabIndex="4" />            
                </td>
            </tr>
        </table>        
    </LayoutTemplate>
</asp:Login>
<asp:LinkButton ID="lbtnRegister" runat="server" Text="Register Here" CssClass="submitlink" OnClientClick="window.location.href='Welcome.aspx';return false;" TabIndex="5" 
            Visible="false" style="margin-left: 5px;position:relative;top:100px;" />
