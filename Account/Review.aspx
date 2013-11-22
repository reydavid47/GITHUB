<%@ Page Title="" Language="C#" MasterPageFile="~/SignIn.master" AutoEventWireup="true" CodeFile="Review.aspx.cs" Inherits="ClearCostWeb.Account.Review" %>

<asp:Content ID="Review_Content" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:ScriptManager ID="smReview" runat="server" ></asp:ScriptManager>
<h1>
                        Review</h1>
                    <h2>
                        Success! Your account has been validated. Please review the information below; if
                        any of the information is incorrect, please contact your HR department:
                    </h2>
                    <br />
                    <h3>
                        Your Personal Information</h3>
                    <table class="formtable" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td width="200px">
                                Name:
                            </td>
                            <td>
                                <asp:Label ID="lblFirstName" runat="server" Text=""></asp:Label>&nbsp;
                                <asp:Label ID="lblLastName" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Date of Birth
                            </td>
                            <td>
                                <asp:Label ID="lblDateOfBirth" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Health Plan:
                            </td>
                            <td>
                                Blue Cross Blue Shield of Illinois &nbsp; BCBS PPO
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Home Address
                            </td>
                            <td>
                                <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Email:
                            </td>
                            <td>
                                <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <hr />
                            </td>
                        </tr>
                    </table>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:Repeater ID="rptAdultDependents" runat="server">
                                <HeaderTemplate>
                                    <table class="formtable" cellspacing="0" cellpadding="0" border="0">
                                        <tr>
                                            <td colspan="2">
                                                <h3>
                                                    Adult Dependent Information</h3>
                                            </td>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td width="120px">
                                            Name:
                                        </td>
                                        <td>
                                            <%--Need to change this section into a list.  For the demo, just populate it.--%>
                                            <asp:Label ID="lblADFirstName" runat="server" Text='<%# Eval("FirstName") %>'></asp:Label>&nbsp;
                                            <asp:Label ID="lblADLastName" runat="server" Text='<%# Eval("LastName") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Date of Birth
                                        </td>
                                        <td>
                                            <asp:Label ID="lblADDateOfBirth" runat="server" Text='<%# Eval("DOBDisplay") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            Because
                                            <asp:Label ID="lblADFname2" runat="server" Text='<%# Eval("FirstName") %>'></asp:Label>
                                            <asp:Label ID="lblADLname2" runat="server" Text='<%# Eval("LastName") %>'></asp:Label>
                                            is over the age of 18, you are not able to see her medical information unless she
                                            grants access to you. To send
                                            <asp:Label ID="lblADFname3" runat="server" Text='<%# Eval("FirstName") %>'></asp:Label>
                                            a link to authenticate her identification and allow you access to her medical records,
                                            we need
                                            <asp:Label ID="lblADFname4" runat="server" Text='<%# Eval("FirstName") %>'></asp:Label>'s
                                            email address.
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblADFname5" runat="server" Text='<%# Eval("FirstName") %>'></asp:Label>'s
                                            email:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtADEmail" runat="server" CssClass="boxed"></asp:TextBox>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                            <table class="formtable" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td colspan="2">
                                        <hr />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <h3>
                                            Create a personalized account password</h3>
                                    </td>
                                </tr>
                                <tr>
                                    <td width="200px">
                                        Password:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="boxed"
                                            ValidationGroup="ChangePassword1"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvPassword" ControlToValidate="txtPassword" runat="server"
                                            ErrorMessage="Password Required" ForeColor="Red" ValidationGroup="ChangePassword1"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td width="200px">
                                        Confirm Password:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtConfirm" runat="server" TextMode="Password" CssClass="boxed"
                                            ValidationGroup="ChangePassword1"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvConfirm" ControlToValidate="txtConfirm" runat="server"
                                            ErrorMessage="Confirm Password Required" ForeColor="Red" ValidationGroup="ChangePassword1"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="txtPassword"
                                            ControlToValidate="txtConfirm" Display="Dynamic" ErrorMessage="The Confirm Password must match the Password entry."
                                            ValidationGroup="ChangePassword1"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <hr />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <h3>
                                            Terms and Conditions</h3>
                                        <div style="width: 415px; height: 150px; overflow: scroll; border: 1px #666 solid;">
                                            <div style="width: 360px; margin: 10px">
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
                                        <asp:CheckBox ID="cbxTandC" runat="server" Text="I have read and agree to the Terms and Conditions listed here." />
                                        <asp:Label ID="lblTandC" runat="server" Visible="False" ForeColor="Red"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:LinkButton ID="lbtnAccessAccount" runat="server" CssClass="submitlink" OnClick="lbtnAccessAccount_Click"
                                            ValidationGroup="ChangePassword1">Access Account</asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
</asp:Content>

