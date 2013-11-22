<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Contact_us.aspx.cs" Inherits="ClearCostWeb.Public.Contact_us" %>

<asp:Content ID="Contact_us_Content" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h1>
        Contact Us</h1>
    <p>
        For inquiries about our service or to schedule a free market analysis of projected
        savings and of high- and low-cost provider utilization by your group's participants,
        please contact:</p>
    <p>
        Mark Agnew<br />
        Chief Marketing Officer
    </p>
    <p>
        800-934-2167, direct<br />
        650-473-3951, fax<br />
        <a href="mailto:mwagnew@clearcosthealth.com">mwagnew@clearcosthealth.com</a>
    </p>
    <p>
        If you'd prefer, provide us with the following information and we will contact you
        &mdash;
    </p>
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
</asp:Content>

