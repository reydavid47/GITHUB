<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FindADoctor.ascx.cs" Inherits="ClearCostWeb.Controls.FindADoctor" %>
<%@ Register Src="~/Controls/AlertBar.ascx" TagPrefix="cch" TagName="AlertBar" %>
<h1>
    Search by Specialty
</h1>
<cch:AlertBar ID="abSpecialtyNetworks" runat="server" TypeOfAlert="Normal">
    <MessageTemplate>
        <%# Container.StaticText %>
    </MessageTemplate>
</cch:AlertBar>
<h3>
    Select the specialty of the doctor you are looking for</h3>
<br />
<asp:SqlDataSource ID="dsSpecialtyList" runat="server" ConnectionString="<%$ ConnectionStrings:CCH_FrontEnd %>"
    SelectCommand="SELECT * FROM [CCHSpecialties] where InFindADoc = 1 and SpecialtyID not in (33, 40, 44, 36, 43, 38,39, 32) ORDER BY [Specialty]"></asp:SqlDataSource>
<asp:Repeater ID="rptSpecialtyList" runat="server" DataSourceID="dsSpecialtyList">
    <HeaderTemplate>
        <table class="searchresults doctypes" id="finddoc" border="0" cellspacing="0" cellpadding="0">
            <thead>
                <tr>
                    <td class="tdfirst">
                        <b>Specialty</b>
                    </td>
                    <td class="tdfirst">
                        <b>This is the practice of medicine related to</b>
                    </td>
                </tr>
            </thead>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="graydiv roweven">
            <td class="tdfirst graydiv">
                <asp:LinkButton ID="lbtnSelectSpecialty" runat="server" CssClass="readmore" Text='<%# Eval("Title") %>'
                    CommandArgument='<%# Eval("SpecialtyID") %>' CommandName="Select" OnClick="lbtnSelectSpecialty_Click"></asp:LinkButton>
            </td>
            <td class="graydiv">
                <%# Eval("relatedToText") %>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="graydiv">
            <td class="tdfirst graydiv">
                <asp:LinkButton ID="lbtnSelectSpecialty" runat="server" CssClass="readmore" Text='<%# Eval("Title") %>'
                    CommandArgument='<%# Eval("SpecialtyID") %>' CommandName="Select" OnClick="lbtnSelectSpecialty_Click"></asp:LinkButton>
            </td>
            <td class="graydiv">
                <%# Eval("relatedToText") %>
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
