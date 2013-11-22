<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="edit_medications.aspx.cs" Inherits="ClearCostWeb.SearchInfo.edit_medications" %>

<asp:Content ID="edit_medications_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
<link rel="Start" href="../Styles/old/style.css" type="text/css" />
    <h1>
        Manage Your Family's List of Medications</h1>
    <p>
        <a href="search.aspx" class="back">Return to Search</a>
    </p>
    <asp:Repeater runat="server" ID="rptFamilyMeds" OnItemDataBound="BindFamilyItem">
        <HeaderTemplate>
            <table cellspacing="0" cellpadding="4" border="0" width="100%">
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td width="50%" valign="top" class="medlistcontainer">
                    <b><a class="member listclosed pointer">
                        <asp:Label runat="server" ID="lblRXEmployeeName" Text="" ForeColor="#662D91" />
                    </a></b>
                    <asp:Panel runat="server" CssClass="membermed" ID="membermedcontainer">
                        <asp:Repeater runat="server" ID="rptMemberMeds">
                            <HeaderTemplate>
                                <table cellspacing="0" cellpadding="4" border="0" width="600">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td width="45%">
                                        <asp:LinkButton runat="server" ID="lbtnMed" CssClass="readmore" OnClick="SearchClickedDrug" Text="" />
                                    </td>
                                    <td width="35%">
                                        <asp:Label runat="server" ID="lblPharmacy" Text="" />
                                    </td>
                                    <td width="20%">
                                        <asp:LinkButton runat="server" ID="lbtnDeleteMed" OnClick="DeleteClickedDrug" Text="Delete" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
            <asp:Literal ID="ltlNameJava" runat="server" Text="<script>$('.rbMed input').attr('name','selectMed');</script>" />
        </FooterTemplate>
    </asp:Repeater>
    <%--<asp:Repeater runat="server" ID="rptFamilyMembers">
        <ItemTemplate>
            <div class="medlistcontainer">
                <p>
                    <b>
                        <asp:HyperLink ID="hlEmployeeName" runat="server" CssClass="listclosed pointer membermed" Text="" />
                    </b>
                </p>
                <div class="membermed">
                    <asp:Repeater runat="server" ID="rptMemberMeds">
                        <HeaderTemplate>
                            <table cellspacing="0" cellpadding="4" border="0" width="600">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td width="45%">
                                    <asp:LinkButton runat="server" ID="lbtnMed" CssClass="readmore" OnClick="SearchClickedDrug" Text="" />
                                </td>
                                <td width="35%">
                                    <asp:Label runat="server" ID="lblPharmacy" Text="" />
                                </td>
                                <td width="20%">
                                    <asp:LinkButton runat="server" ID="lbtnDeleteMed" OnClick="DeleteClickedDrug" Text="Delete" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>--%>
</asp:Content>

