<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ContentManagement.aspx.cs" Inherits="ClearCostWeb.Admin.ContentManagement" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Shortcut Icon" href="../Images/favicon.ico" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="smCM" runat="server"></asp:ScriptManager>
    <div>
        <div>
            <asp:LoginStatus ID="lsLoggedIn" runat="server" />
            &nbsp;|&nbsp;
            <asp:LinkButton ID="lbtnAdmin" runat="server" Text="Back To Admin Page" PostBackUrl="~/Admin/Default.aspx" />
        </div>
        <hr />
        <div>
            <asp:LinkButton ID="lbtnShowFAQCM" runat="server" Text="Manage FAQ's" 
                onclick="lbtnShowFAQCM_Click" />
        </div>
        <hr />
        <asp:UpdatePanel ID="upCM" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <asp:Panel ID="pnlChoseEmployer" runat="server" Visible="false">
                    Manage 
                    <asp:DropDownList ID="ddlEmployers" runat="server" AppendDataBoundItems="true" DataTextField="EmployerName" DataValueField="EmployerID" AutoPostBack="true">
                        <asp:ListItem Text="Generic" Value="0" Selected="True" />
                    </asp:DropDownList>
                     content
                </asp:Panel>
                <asp:Panel ID="pnlFAQ" runat="server" Visible="false">
                    <%--<asp:GridView ID="gvFAQItems" runat="server" AutoGenerateColumns="False" 
                        onrowediting="gvFAQItems_RowEditing" AutoGenerateEditButton="true" 
                        onrowcancelingedit="gvFAQItems_RowCancelingEdit">
                        <Columns>                                   
                            <asp:BoundField HeaderText="Title" DataField="Title" />
                            <asp:BoundField HeaderText="Text" DataField="Text" />
                        </Columns>
                    </asp:GridView>--%>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
