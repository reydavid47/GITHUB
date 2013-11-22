<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LearnMore.ascx.cs" Inherits="ClearCostWeb.Controls.LearnMore" %>
<asp:Panel ID="pnlLearnMore" runat="server" CssClass="learnmore">
    <a title="Learn more">
        <asp:Image ID="imgLearnMore" runat="server" Width="12" Height="13" BorderWidth="0" AlternateText="Learn More" style="z-index: 1030;width:13px;height:13px;" />
    </a>
    <asp:Panel ID="MoreInfo" runat="server" CssClass="moreinfo" style="z-index: 1031;">
        <asp:Image ID="imgMoreInfo" runat="server" Width="14" Height="14" BorderWidth="0" AlternateText="" ImageAlign="Right"
            style="cursor:pointer;width:14px;height:14px;" />
        <p>
            <b class="upper">
                <asp:PlaceHolder ID="phTitle" runat="server" />
            </b>
        </p>
        <p>
            <asp:PlaceHolder ID="phText" runat="server" />
        </p>
    </asp:Panel>
</asp:Panel>