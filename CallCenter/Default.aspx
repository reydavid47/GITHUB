<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="ClearCostWeb.CallCenter.Default" MaintainScrollPositionOnPostback="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .letter 
        {
            padding: 0px 8px;
            font-weight: bold;
        }
        .grid
        {
            margin:0px auto;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:LoginStatus ID="lsCallCenter" runat="server" LogoutText="Sign Out of the Call Center" />
    <asp:ScriptManager ID="smCC" runat="server" ScriptMode="Release" />
    <% if (Roles.IsUserInRole("UserAdmin"))
       { %>
            <a href="../Security">User Admin</a>
    <% } %>
    <script type="text/javascript">
        var yPos;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        function BeginRequestHandler(sender, args) {
            if ($get('<%= pnlNames.ClientID %>') != null) {
                yPos = $get('<%= pnlNames.ClientID %>').scrollTop;
            }
        }
        function EndRequestHandler(sender, args) {
            if ($get('<%= pnlNames.ClientID %>') != null) {
                $get('<%= pnlNames.ClientID %>').scrollTop = yPos;
            }
        }
        prm.add_beginRequest(BeginRequestHandler);
        prm.add_endRequest(EndRequestHandler);
    </script>
    <% HttpContext.Current.Session["FromCallCenter"] = true; //Adding code here due to the code behind being checked out.  MOVE TO CODE BEHIND ASAP (JM 4/17) %>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div style="width:600px;margin:10px auto;">        
                <div style="height:32px;">
                    <asp:UpdateProgress runat="server">
                        <ProgressTemplate>
                            <center><img src="../Images/ajax-loader-AltCircle.gif" alt="" /></center>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                </div>
                <asp:Panel ID="pnlShortSearch" runat="server" Visible="false">
                    <div>
                        <div>Employer Passed in:&nbsp<asp:Label ID="lblEmployerFromSrch" runat="server" Text="" /></div>
                        <div>EmployeeID Passed in:&nbsp<asp:Label ID="lblEmployeeIDFromSrch" runat="server" Text="" /></div>
                    </div>
                <hr />
                </asp:Panel>
                <asp:Panel ID="pnlLongSearch" runat="server" Visible="true">
                    <div>
                        <div>Choose an Employer:</div>
                        <asp:DropDownList ID="ddlEmployers" runat="server" AutoPostBack="true"
                            DataTextField="EmployerName" DataValueField="EmployerID" OnSelectedIndexChanged="ChooseEmployer" />
                    </div>
                    <br />
                    <div>            
                        <div>First Letter Of Last Name:</div>
                        <asp:Panel ID="pnlLetters" runat="server" Enabled="false" Visible="true">
                            <asp:LinkButton runat="server" Text="A" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="B" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="C" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="D" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="E" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="F" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="G" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="H" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="I" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="J" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="K" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="L" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="M" OnClick="SelectLetter" class="letter" />
                            <br />
                            <asp:LinkButton runat="server" Text="N" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="O" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="P" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="Q" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="R" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="S" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="T" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="U" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="V" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="W" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="X" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="Y" OnClick="SelectLetter" class="letter" />
                            <asp:LinkButton runat="server" Text="Z" OnClick="SelectLetter" class="letter" />
                        </asp:Panel>
                    </div>
                    <br />
                    <div>
                        <div>Choose the last name of an employee:</div>
                        <asp:DropDownList ID="ddlEmployees" runat="server" AutoPostBack="true"
                            DataTextField="LastName" OnSelectedIndexChanged="ChooseLastName" />
                    </div>
                    <br />               
                    <hr />
                </asp:Panel>
            </div>
                <center>
                    <div style="width:800px;">
                        <asp:label ID="lblListTitle" runat="server" Text="" style="float:left;" />
                        <asp:LinkButton ID="lbConfirm" runat="server" Enabled="false" Visible="true" Text="" OnClick="ContinueAsEmployee" style="float:right;" />
                    </div>
                </center>
                <br style="clear:both;" />
                <br />
                <asp:Panel ID="pnlNames" runat="server" ScrollBars="Vertical" Height="400px">
                    <asp:GridView ID="gvUsers" runat="server" AllowPaging="false" AutoGenerateColumns="true"
                        AutoGenerateSelectButton="true" CellPadding="5" CellSpacing="3" CssClass="grid"
                        HeaderStyle-BackColor="#946CB2" HeaderStyle-ForeColor="White"
                        AlternatingRowStyle-BackColor="#F0EAF4" SelectedRowStyle-BackColor="#689B35"
                        OnSelectedIndexChanged="SelectEmployee" AllowSorting="false" DataKeyNames="Email,FirstName,LastName,MemberMedicalID" />
                </asp:Panel>
                <br />
                <asp:Literal ID="ltlMessage" runat="server" Text="" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
