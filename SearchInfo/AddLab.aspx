<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="AddLab.aspx.cs" Inherits="ClearCostWeb.SearchInfo.AddLab" %>
<%@ Register Src="~/Controls/LetterSearch.ascx" TagName="LetterSearch" TagPrefix="cch" %>
<%@ Register Src="~/Controls/TextSearch.ascx" TagName="TextSearch" TagPrefix="cch" %>
<asp:Content ID="AddLab_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <h1>Search Lab Tests</h1>
    <h3>Your search will show cost and quality information for:</h3>
    <asp:UpdatePanel runat="server" ID="upMain">
        <ContentTemplate>
            <asp:Repeater ID="rptLabTests" runat="server">
                <HeaderTemplate><p></HeaderTemplate>
                <ItemTemplate>
                    <span>
                        <asp:CheckBox ID="cbLabTest" runat="server" Checked="true" Text='<%# Eval("ServiceName") %>' OnCheckedChanged="RemoveLabTest" AutoPostBack="true" />
                    </span>
                </ItemTemplate>
                <SeparatorTemplate><br /></SeparatorTemplate>
                <FooterTemplate></p></FooterTemplate>
            </asp:Repeater>
            <div style="<%= ShowLetters %> height:auto;" class="switchbar">
                <asp:Repeater ID="rptTestLetters" runat="server" OnItemDataBound="rptTestLetters_ItemDataBound">
                    <HeaderTemplate>
                        <h3 style="float:left; height:23px; padding-top:10px;">
                            Choose a lab test to add:
                        </h3>
                        <asp:UpdateProgress ID="upLetterSpin" runat="server" DisplayAfter="0" AssociatedUpdatePanelID="upMain">
                            <ProgressTemplate>
                                <asp:Image ID="Image1" runat="server" AlternateText="" ImageUrl="~/Images/CCHSpinLoader.gif" 
                                    style="z-index: 1002; width: 35px; height: 35px; float:left;" />
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <br class="clearboth" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnLetter" Text="" runat="server" OnClick="SelectLetter" CssClass="letternav" />                        
                    </ItemTemplate>
                    <SeparatorTemplate>&nbsp;</SeparatorTemplate>
                </asp:Repeater>
                <asp:Repeater ID="rptTestList" runat="server" >
                    <HeaderTemplate>
                        <br />
                        <hr />
                        <div class="letterbox" style="overflow:auto; max-height:300px; text-transform:capitalize;">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnTest" runat="server" CssClass="readmore" OnClick="SelectOption" Text="" OnDataBinding="lbtnTest_OnDataBinding" />
                    </ItemTemplate>
                    <SeparatorTemplate>                    
                        <br />
                    </SeparatorTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div style="<%= ShowText %>" class="switchbar">
                <cch:TextSearch ID="tsAddLab" runat="server" runMethod="SearchPage" />
            </div>
            <asp:LinkButton ID="lnkBtnContinue" runat="server" CssClass="submitlink" OnClick="ContinueSearch" Visible="true">Search</asp:LinkButton>
            <asp:Label ID="lblAddMoreServices" runat="server" Text="Please add at least one lab to search for before continuing" visible="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

