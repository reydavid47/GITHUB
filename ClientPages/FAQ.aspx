<%@ Page Title="" Language="C#" MasterPageFile="~/ClientPages/ClientPages.master" AutoEventWireup="true" CodeFile="FAQ.aspx.cs" Inherits="ClearCostWeb.ClientPages.FAQ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h1>
        Frequently Asked Questions
    </h1>
    <% if (Membership.GetUser() != null && Membership.GetUser().IsOnline)
       { %><a class="back" href="javascript:history.back();" style="background-position-y:center;background-image: url('../Images/sm_arrow_square_back.png');">Return</a><% } %>
    <div id="faq">
        <asp:Repeater ID="rptFAQs" runat="server">
            <ItemTemplate>
                <p class="q q-closed"><%# Eval("Title") %></p>
                <p class="answer"><%# Eval("Text") %></p>
            </ItemTemplate>
            <SeparatorTemplate>
                <hr />
            </SeparatorTemplate>
        </asp:Repeater>
        <asp:Repeater ID="ClientSpecificFAQ" runat="server">
            <HeaderTemplate>
                <hr />
            </HeaderTemplate>
            <ItemTemplate>
                <p class="q q-closed"><%# Eval("Title") %></p>
                <p class="answer"><%# Eval("Text") %></p>
            </ItemTemplate>
            <SeparatorTemplate>
                <hr />
            </SeparatorTemplate>
        </asp:Repeater>
    </div>
    <script type="text/javascript">
        (function ($) {
            $("html").css("overflowY", "scroll");
            $("p.q").click(function () {
                var displayIs = $(this).nextUntil("hr").css("display");
                if (displayIs.match(/block/)) {
                    $(this).removeClass("q-open").addClass("q-closed");
                } else {
                    $(this).removeClass("q-closed").addClass("q-open");
                }
                $(this).nextUntil("hr").slideToggle("slow");
            });
        })(jQuery);
    </script>
</asp:Content>
