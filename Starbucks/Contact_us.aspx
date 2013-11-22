<%@ Page Title="" Language="C#" MasterPageFile="Starbucks.master" AutoEventWireup="true" CodeFile="Contact_us.aspx.cs" Inherits="ClearCostWeb.Starbucks.Contact_us" %>

<asp:Content ID="Contact_us_Content" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h1>
        Contact Us
    </h1>
    <b>Please leave us your contact information and let us know how we can help you:</b>
    <br />
    <% if (Membership.GetUser() != null && Membership.GetUser().IsOnline)
       { %><a class="back" href="javascript:history.back();">Return</a><% } %>
    <br />
    <table cellspacing="0" cellpadding="0" border="0" class="formtable">
        <tr>
            <td>
                Your Name
            </td>
            <td>
                <asp:TextBox ID="txtYourName" runat="server" CssClass="boxed"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Name of Organization
            </td>
            <td>
                <asp:TextBox ID="txtNameOfOrganization" runat="server" CssClass="boxed"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Email
            </td>
            <td>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="boxed"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Phone
            </td>
            <td>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="boxed"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                How may we help you?
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtUsersBody" TextMode="MultiLine" style="width: 300px;" Rows="10"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <asp:LinkButton ID="lbtnSubmit" runat="server" 
                style="font-family: arial, helvetica, sans-serif; padding: 3px;  background-color: #662D91; color: #fff; text-align:center"
                    onclick="lbtnSubmit_Click" Width="100px" BorderColor="#662D91" 
                    BorderStyle="Solid" BorderWidth="1px">Send</asp:LinkButton>
            </td>
        </tr>
    </table>
    <br />
    <br />
    <b>Phone number: </b>1-800-390-6855
</asp:Content>

