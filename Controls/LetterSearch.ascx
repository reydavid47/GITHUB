<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LetterSearch.ascx.cs" Inherits="ClearCostWeb.Controls.LetterSearch" %>

<asp:Repeater ID="rptTestLetters" runat="server" OnItemDataBound="rptTestLetters_ItemDataBound">
    <ItemTemplate>
        <asp:LinkButton ID="lbtnLetter" Text="" runat="server" OnClick="SelectLetter"
            CssClass="letternav" />
        &nbsp;
    </ItemTemplate>
</asp:Repeater>

<asp:Repeater ID="rptTestList" runat="server" OnItemDataBound="rptTestList_ItemDataBound">
    <HeaderTemplate>
        <br />
        <hr />
        <div class="letterbox" style="overflow:auto; max-height:300px; text-transform:capitalize;">
    </HeaderTemplate>
    <ItemTemplate>
        <asp:LinkButton ID="lbtnTest" runat="server" CssClass="readmore" OnClick="SelectOption"
            Text="" OnDataBinding="lbtnTest_OnDataBinding" />
        <br />
    </ItemTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
</asp:Repeater>

