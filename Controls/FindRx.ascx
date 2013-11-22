<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FindRx.ascx.cs" Inherits="ClearCostWeb.Controls.FindRx" %>
<%@ Register Src="~/Controls/TextSearch.ascx" TagPrefix="cch" TagName="TextSearch" %>
<%@ Register Src="~/Controls/LetterSearch.ascx" TagPrefix="cch" TagName="LetterSearch" %>

<asp:HiddenField ID="hfChosenDrugs" runat="server" ClientIDMode="Static" />
<h1>
    Find Prescriptions</h1>
<% if (rptFamilyMeds.HasControls())
   { %>
<h3>
    Select a prescription from your family members' list of medications:</h3>
<%--<p>
    Click family member's name to see full list of medications.
</p>--%>

<asp:Panel runat="server" ClientIDMode="Static" ID="familymedform" >
    <asp:Repeater runat="server" ID="rptFamilyMeds" OnItemDataBound="BindFamilyItem">
        <HeaderTemplate>
            <table cellspacing="0" cellpadding="4" border="0" width="100%">
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td width="50%" valign="top" class="medlistcontainer">
                    <b><a class="member listclosed pointer">
                        <asp:Label runat="server" ID="lblRXEmployeeName" Text="" ForeColor="#662D91" />
                    </a></b><%--<span class="managemember" id="mmb">[<a href="../SearchInfo/edit_medications.aspx#tabrx">Manage
                        List</a>] </span>--%>
                    <asp:Panel runat="server" CssClass="membermed" ID="membermedcontainer">
                        <asp:Repeater runat="server" ID="rptMemberMeds">
                            <ItemTemplate>
                                <asp:RadioButton ID="rbMemberMed" runat="server" Checked="false" CssClass="rbMed" />
                                <%--<asp:CheckBox ID="cbMemberMed" runat="server" Checked="false" CssClass="rbMed" />--%>
                            </ItemTemplate>
                            <SeparatorTemplate>
                                <br />
                            </SeparatorTemplate>
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
    
    <%-- Temporarily rerouting from results_rx_multi.aspx#tabrx to results_rx.aspx#tabrx  as for April 1 we aren't using Multil RX --%>
    <%--<a id="managemedsubmit" class="submitlink" href="../SearchInfo/results_rx_multi.aspx#tabrx"
    style="display: none">Search</a>--%>
    <asp:HyperLink CssClass="submitlink" runat="server" ID="managemedsubmit" Text="Search" ClientIDMode="Static" style="display: none;" />
    <script type="text/javascript">
        $(document).ready(function () {
            $("#managemedsubmit").click(function () {
                var theForm = document.forms['form1'];
                if (!theForm) { theForm = document.form1; }
                if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
                    theForm.hfChosenDrugs.value = '';
                    $("input[name='selectMed']:checked").each(function (index, med) {
                        theForm.hfChosenDrugs.value += $(med).parent("span").attr("n") + "|";
                    });
                    theForm.submit();
                }
            });
        });
    </script>
</asp:Panel>


<hr />
<p>
    <b>OR</b>
</p>
<% } %>

    <cch:TextSearch ID="tsRX" runat="server" SearchDescription="Search for a prescription drug by name:" runMethod="RXPage" />
<p>
    <b>OR</b>
</p>
<h3>
    Search drug name by first letter:</h3>
<asp:UpdatePanel ID="upRxSearch" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <cch:LetterSearch ID="lsRxSearch" runat="server" runMethod="RXPage" />
    </ContentTemplate>
</asp:UpdatePanel>
