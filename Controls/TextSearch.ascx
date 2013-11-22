<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TextSearch.ascx.cs" Inherits="ClearCostWeb.Controls.TextSearch" %>

<%--<link href="../SearchInfo/Styles/skin.css" rel="stylesheet" type="text/css" />--%>
<asp:ScriptManagerProxy ID="smpTextSearch" runat="server">
        <Scripts>
            <%--<asp:ScriptReference Path="~/Scripts/jquery.ui.core.js" />
            <asp:ScriptReference Path="~/Scripts/jquery.ui.widget.js" />
            <asp:ScriptReference Path="~/Scripts/jquery.ui.position.js" />
            <asp:ScriptReference Path="~/Scripts/jquery.ui.autocomplete.js" />--%>
        </Scripts>
</asp:ScriptManagerProxy>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%= txtSearch.ClientID %>").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "../Handlers/AutoComplete.ashx",
                    cache: false,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: { 
                        userInput: request.term,
                        type: $("#<%= txtSearch.ClientID %>").attr("ACType")
                        },
                    responseType: "json",
                    success: function (result) {
                        response(result.Results);
                    },
                    error: function (result) {
                        
                    }
                });
                return false;
            },
            minLength: 3
        });
    });
</script>
<asp:Panel runat="server" ID="pnlTextSearch" DefaultButton="lnkBtnSearch">
    <div>
        <asp:Label ID="lblSearchDescription" runat="server" Text=""></asp:Label>
    </div>
    <div>
        <div>
            <asp:TextBox ID="txtSearch" runat="server" CssClass="boxed" Width="500px"></asp:TextBox>
            &nbsp;
            <asp:LinkButton ID="lnkBtnSearch" runat="server" CssClass="submitlink" OnClick="lnkBtnSearch_Click">Search</asp:LinkButton>
        </div>
        <div>
            <asp:Label ID="lblCareSearchNote" runat="server" Text=""></asp:Label>
        </div>
    </div>
    <div>
        <asp:ListBox ID="lbCareSearchResults" runat="server" Visible="false" CssClass="boxed" Width="500px" AutoPostBack="True">
        </asp:ListBox>
    </div>
</asp:Panel>