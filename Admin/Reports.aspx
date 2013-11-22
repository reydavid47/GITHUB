<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reports.aspx.cs" Inherits="ClearCostWeb.Admin.Reports" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Shortcut Icon" href="../Images/favicon.ico" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager id="smAdmin" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="upAdmin" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>                
                <asp:LinkButton ID="lbtnShowErrors" runat="server" OnClick="ShowErrors" Text="Show Errors" />
                <br />
                <asp:DropDownList ID="ddlEmployers" runat="server" DataTextField="EmployerName" DataValueField="ConnectionString" />
                <br />
                <asp:LinkButton ID="lbtnShowLogins" runat="server" OnClick="ShowLogins" Text="Show Logins" />
                <br />
                <asp:LinkButton ID="lbtnShowRegisteredUsers" runat="server" OnClick="ShowRegistered" Text="Show Registered Users" />
                <br />
                <br />
                <asp:Panel ID="pnlErrors" runat="server" Visible="false">
                    <%--<asp:GridView ID="gvErrors" runat="server" AutoGenerateColumns="true" AutoGenerateSelectButton="true" EmptyDataText="No Data"
                     RowStyle-Height="10px" RowStyle-Wrap="false" OnSelectedIndexChanged="UpdateDetails">
                    </asp:GridView>
                    <asp:DetailsView ID="dvErrors" runat="server" AutoGenerateRows="true" EmptyDataText="No Data" />--%>
                    <asp:GridView runat="server" DataSourceID="gvErrorsDS" ID="gvErrors" DataKeyNames="table,ErrID" AutoGenerateColumns="false" OnSelectedIndexChanged="UpdateDVErrors"
                         CellPadding="5">
                        <HeaderStyle BackColor="Beige" BorderColor="Black" BorderWidth="2" BorderStyle="Solid" />
                        <RowStyle BackColor="White" BorderColor="Black" BorderWidth="1" BorderStyle="Solid" Wrap="false" />
                        <AlternatingRowStyle BackColor="LightGray" BorderColor="Black" BorderWidth="1" BorderStyle="Solid" Wrap="false" />
                        <SelectedRowStyle BackColor="YellowGreen" BorderColor="Black" BorderWidth="1" BorderStyle="Solid" Wrap="false" />
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" ButtonType="Button" />
                            <asp:BoundField DataField="table" HeaderText="Database" />
                            <asp:BoundField DataField="Source" HeaderText="Source" />
                            <asp:BoundField DataField="Message" HeaderText="Message" />
                            <asp:BoundField DataField="Procedure" HeaderText="Procedure" />
                            <asp:BoundField DataField="ErrorDate" HeaderText="Date" />
                        </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="gvErrorsDS" runat="server" 
                        SelectMethod="GetGridViewData" TypeName="GetUIErrorList">
                    </asp:ObjectDataSource>  
                    <br />                  
                    <asp:DetailsView ID="dvError" runat="server" Height="50px" Width="300px" GridLines="Both" >
                        <HeaderStyle BackColor="Beige" BorderColor="Black" BorderWidth="1" />
                    </asp:DetailsView>                    
                </asp:Panel>
                
                <asp:Panel ID="pnlLogins" runat="server" Visible="false">
                    
                </asp:Panel>

                <asp:Panel ID="pnlRegistered" runat="server" Visible="false">
                    <asp:GridView ID="gvRegistered" runat="server">
                    </asp:GridView>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
