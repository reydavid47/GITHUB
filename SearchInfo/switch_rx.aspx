<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="switch_rx.aspx.cs" Inherits="ClearCostWeb.SearchInfo.switch_rx" %>
<%@ Register Src="~/Controls/AlertBar.ascx" TagPrefix="cch" TagName="AlertBar" %>

<asp:Content ID="switch_rx_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <%--<div id="result_buttons">
        <div class="button">
            <a href="print.aspx" target="_blank" onclick="javascript:window.print(); return false;">Print Results</a>
        </div>
    </div>--%>
    <!-- end result buttons -->
    <h1>
        Save Money on Rx</h1>
    <div class="switchbar">
        <p>
            You and your employer could save money on two drugs in your medication list by switching
            to less expensive alternatives. These alternatives are called "therapeutic substitutes"
            because, while not identical, they are very similar to the drugs you take. Because
            they are generic, they can mean savings for you and your employer.
        </p>
    </div>
    <div style="float:left;">
        <cch:AlertBar runat="server" ID="abCouldSave" TypeOfAlert="Small" SaveTotal="" Visible="true">
            <MessageTemplate>
                Estimated Annual Savings for you and your employer: <b><%# Container.SaveTotal %></b>
            </MessageTemplate>
        </cch:AlertBar>
    </div>
    <div class="displayinline" style="margin-left: 6em;">
        <p class="displayinline">
            <a href="switch_steps.aspx#tabrx" class="readmore">See how to switch and save</a>
        </p>
        &nbsp;&nbsp;&nbsp;
        <p class="displayinline">
            <a href="search.aspx#tabrx" class="readmore">Cancel</a>
        </p>
    </div>
    <div class="clearboth"></div>
    <hr class="heavy" />
    <asp:Repeater ID="rptTheraSubData" runat="server" OnItemDataBound="BindMeds">
        <ItemTemplate>
            <asp:Panel ID="pnlMember" runat="server" CssClass="medlistcontainer">
                <p><a class="listclosed pointer member"><%# String.Format("{0} {1}",Eval("FirstName"), Eval("LastName")) %></a></p>
                <asp:Panel ID="pnlMemberMed" runat="server" CssClass="membermed">
                    <asp:Repeater ID="rptMembersMeds" runat="server">
                        <HeaderTemplate>
                            <table cellspacing="0" cellpadding="4" border="0" width="750">
                        </HeaderTemplate>
                        <ItemTemplate>
                                <tr>
                                    <td width="25%">
                                        Current drug:
                                    </td>
                                    <td width="50%">
                                        <%# String.Format("{0} {1}", Eval("DrugName"), Eval("Strength")) %>
                                    </td>
                                    <td width="25%">
                                        Estimated savings: <b><%# String.Format("{0:C2}", Double.Parse(Eval("TotalPotentialSavings").ToString())) %></b>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Alternative:
                                    </td>
                                    <td colspan="2">
                                        <%# String.Format("{0} {1}<br />(or similar drug recommended by your doctor)", Eval("ReplacementDrugName"), Eval("ReplacementStrength")) %>
                                    </td>
                                </tr>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <tr>
                                <td colspan="3">
                                    <hr style="margin: 0;" />
                                </td>
                            </tr>
                        </SeparatorTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </asp:Panel>
            </asp:Panel>
        </ItemTemplate>
        <SeparatorTemplate>
            <hr class="heavy" />
        </SeparatorTemplate>
    </asp:Repeater>
</asp:Content>

