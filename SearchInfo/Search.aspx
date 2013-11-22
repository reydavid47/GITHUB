<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/SearchMaster.master" 
    AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="ClearCostWeb.SearchInfo.Search" %>

<%@ Register Src="~/Controls/savingsChoiceDashboard.ascx" TagPrefix="cch" TagName="SCDashboard" %>
<%@ Register Src="~/Controls/FindADoctor.ascx" TagPrefix="cch" TagName="FindADoctor" %>
<%@ Register Src="~/Controls/FindRx.ascx" TagPrefix="cch" TagName="FindRx" %>
<%@ Register Src="~/Controls/FindAService.ascx" TagPrefix="cch" TagName="FindAService" %>
<%@ Register Src="~/Controls/FindPastCare.ascx" TagPrefix="cch" TagName="FindPastCare" %>
<%@ Register Src="~/Controls/TextSearch.ascx" TagPrefix="cch" TagName="TextSearch" %>
<%@ Register Src="~/Controls/AlertBar.ascx" TagPrefix="cch" TagName="AlertBar" %>

<asp:Content ID="cMain" ContentPlaceHolderID="MainBody" runat="Server">
    <script type="text/javascript">
        var jDashboard, jTabCare, jTabRx, jTabDoc, jTabPast;
        $(document).ready(function () {
            //            $('a#dashboard').click(function () { // bind click event to link
            //                $("#tabs").tabs('select', 0); // switch to first tab
            //                return false;
            //            });
            //            $('a#tabcare').click(function () { // bind click event to link
            //                $("#tabs").tabs('select', 1); // switch to first tab
            //                return false;
            //            });
            //            $('a#tabrx').click(function () { // bind click event to link
            //                $("#tabs").tabs('select', 2); // switch to second tab
            //                return false;
            //            });
            //            $('a#tabdoc').click(function () { // bind click event to link
            //                $("#tabs").tabs('select', 3); // switch to second tab
            //                return false;
            //            });
            //            $('a#tabpast').click(function () { // bind click event to link
            //                $.ajax("../Handlers/AuditTabHit.ashx");
            //                $("#tabs").tabs('select', 4); // switch to third tab                
            //                return false;
            //            });

            /* for the letter-based prescription nav */
            //$(".letternav").bind('click', letternav);

            $('#tabs ul li').mouseover(function (a) {
                if (!$(a.currentTarget).hasClass("ui-state-active")) {
                    //$(".ui-state-hover").removeClass("ui-state-hover");
                    $(a.currentTarget).addClass("ui-state-hover");
                }
            });
            $('#tabs ul li').mouseout(function (a) {
                if (!$(a.currentTarget).hasClass("ui-state-active")) {
                    $(".ui-state-hover").removeClass("ui-state-hover");
                    //$(a.currentTarget).addClass("ui-state-hover");
                }
            });

            // see http://stackoverflow.com/questions/3199130/updatepanel-breaks-jquery-script
            // and http://api.jquery.com/live/ and http://api.jquery.com/delegates/
            // for more info on how to handle jquery bindings with partial postbacks. 
            // Review the deprecation section for live() implementation! 
            $(document).delegate(".letternav", "click", letternav);
        });

        function letternav() {
            $("div.letterbox").hide();
            $("a.letternav").removeClass("callout");
            var letter = $(this).attr("id");
            $("div#letter" + letter).show();
            $("a#" + letter).addClass("callout");
        }
    </script>
    <div class="ui-tabs ui-widget ui-widget-content ui-corner-all" id="tabs">
        <ul class="ui-tabs-nav ui-helper-clearfix ui-helper-reset ui-widget-header ui-corner-all">
            <li runat="server" id="liSavingsChoice" class="ui-state-default ui-corner-all" visible="false">
                <asp:LinkButton ID="lbtnDasboard" runat="server" Text="Savings Choice" OnClick="ChangeTab" tab="dashboard" />
            </li>
            <li runat="server" id="liFindAService" class="ui-state-default ui-corner-all">
                <asp:LinkButton ID="lbtnFindAService" runat="server" Text="Find A Service" OnClick="ChangeTab" tab="tabcare" />
            </li>
            <li runat="server" id="liFindRx" class="ui-state-default ui-corner-all" tab="tabrx">
                <asp:LinkButton ID="lbtnFindRX" runat="server" Text="Find Rx" OnClick="ChangeTab" tab="tabrx" />
            </li>
            <li runat="server" id="liFindADoc" class="ui-state-default ui-corner-all" tab="tabdoc">
                <asp:LinkButton ID="lbtnFindADoc" runat="server" Text="Find A Doc" OnClick="ChangeTab" tab="tabdoc" />
            </li>
            <li runat="server" id="liPastCare" class="ui-state-default ui-corner-all" tab="tabpast">
                <asp:LinkButton ID="lbtnPastCare" runat="server" Text="Past Care" OnClick="ChangeTab" tab="tabpast" />
            </li>
        </ul>
        <div id="ctop"></div>
        <div id="cmid">
            <div id="cbotdemo">
                <div class="intwide">
                    <div runat="server" id="dashboard" visible="false">
                        <cch:SCDashboard ID="usSCDashboard" runat="server"/>
                    </div>
                    <div runat="server" id="tabcare" visible="false">
                        <cch:FindAService runat="server" ID="ucFindAService" />
                        <cch:AlertBar ID="abSearch" runat="server" SaveTotal="">
                            <MessageTemplate>
                                Our records suggest that you and your employer could have saved <b><label id="lblYouCouldHaveSaved"><%# Container.SaveTotal %></label></b>
                                in the last twelve months by using our service to select fair price providers.  To learn more, click on <a href="<%# Container.NavigateTo %>">
                                Past Care</a>
                            </MessageTemplate>
                        </cch:AlertBar>
                    </div>
                    <div runat="server" id="tabrx" visible="false">
                        <%--<cch:FindRx runat="server" ID="ucFindRx" OnCommitTransaction="lbtnFindRx_click" />--%>
                        <cch:FindRx runat="server" ID="ucFindRx" />
                        <cch:AlertBar ID="abRX" runat="server" SaveTotal="" NavigateTo="switch_rx.aspx">
                            <MessageTemplate>
                                Based on your prescriptions last year, our records suggest you and your employer could save up to <b>
                                    <%# Container.SaveTotal %></b> a year on two drugs in your family's medication
                                list by switching to less expensive alternatives. Click <b><a href="<%# Container.NavigateTo %>">here</a>
                                    </b> to find out more.
                            </MessageTemplate>
                        </cch:AlertBar>
                    </div>
                    <div runat="server" id="tabdoc" visible="false">
                        <cch:FindADoctor runat="server" ID="ucFindADoctor" />
                    </div>
                    <div runat="server" id="tabpast" visible="false">
                        <cch:FindPastCare ID="ucFindPastCare" runat="server" />
                    </div>
                </div>
            </div>
        </div>
        <div class="clearboth"></div>
    </div>
    <%--<div class="ui-tabs ui-widget ui-widget-content ui-corner-all" id="tabs">--%>
        <%--<ul class="ui-tabs-nav ui-helper-clearfix ui-helper-reset ui-widget-header ui-corner-all">--%>
            <%-- if (Boolean.Parse(HttpContext.Current.Session["SavingsChoiceEnabled"].ToString()))
               { --%>
            <%--<li class="ui-state-default ui-corner-all ui-selected ui-state-active"><a href="#dashboard"><asp:LinkButton ID="lbtnDashboard" runat="server" OnClick="lbtnDashboard_click" PostBackUrl="#dashboard">Dashboard</asp:LinkButton></a></li>--%>
            <%-- } --%>
            <%--<li class="ui-state-default ui-corner-all"><a href="#tabcare"><asp:LinkButton ID="lbtnFindAService" runat="server" OnClick="lbtnFindAService_click" PostBackUrl="#tabcare">Find a Service</asp:LinkButton></a></li>
            <li class="ui-state-default ui-corner-all"><a href="#tabrx"><asp:LinkButton ID="lbtnFindRx" runat="server" OnClick="lbtnFindRx_click" PostBackUrl="#tabrx">Find Rx</asp:LinkButton></a></li>
            <li class="ui-state-default ui-corner-all"><a href="#tabdoc"><asp:LinkButton ID="lbtnFindADoc" runat="server" OnClick="lbtnFindADoc_click" PostBackUrl="#tabdoc">Find a Doctor</asp:LinkButton></a></li>
            <li class="ui-state-default ui-corner-all"><a href="#tabpast" onclick="$.ajax('../Handlers/AuditTabHit.ashx');"><asp:LinkButton ID="lbtnPastCare" runat="server" OnClick="lbtnPastCare_click" OnClientClick="$.ajax('../Handlers/AuditTabHit.ashx');" PostBackUrl="#tabpast">Past Care</asp:LinkButton></a></li>
        </ul>--%> 
        
        
        <%--<div id="cmid">
            <div id="cbotdemo">
                <div class="intwide">--%>
                    <%-- if (Boolean.Parse(HttpContext.Current.Session["SavingsChoiceEnabled"].ToString()))
                       { --%>
                    <%--<div id="dashboard">
                        <cch:SCDashboard ID="usSCDashboard" runat="server" />
                    </div>--%>
                    <%-- } --%>
                    <%--<div id="tabcare">
                        <cch:FindAService runat="server" ID="ucFindAService" />
                        <cch:AlertBar ID="abSearch" runat="server" SaveTotal="" NavigateTo="Search.aspx?t=4#tabpast">
                            <MessageTemplate>
                                Our records suggest that you and your employer could have saved <b><label id="lblYouCouldHaveSaved"><%# Container.SaveTotal %></label></b> in the last twelve months by using our service to select fair price providers. To learn more, click on <a href="<%# Container.NavigateTo %>">
                                Past Care</a>.
                            </MessageTemplate>
                        </cch:AlertBar>
                    </div>--%>
                    <%--<div id="tabrx">
                        <cch:FindRx runat="server" ID="ucFindRx" OnCommitTransaction="lbtnFindRx_click" />
                        <cch:AlertBar ID="abRX" runat="server" SaveTotal="" NavigateTo="switch_rx.aspx#tabrx">
                            <MessageTemplate>
                                Based on your prescriptions last year, our records suggest you and your employer could save up to <b>
                                    <%# Container.SaveTotal %></b> a year on two drugs in your family's medication
                                list by switching to less expensive alternatives. Click <b><a href="<%# Container.NavigateTo %>">here</a>
                                    </b> to find out more.
                            </MessageTemplate>
                        </cch:AlertBar>
                    </div>--%>
                    <%--<div id="tabdoc">
                        <cch:FindADoctor runat="server" ID="ucFindADoctor" />
                    </div>
                    <div id="tabpast">
                        <cch:FindPastCare ID="ucFindPastCare" runat="server" />
                    </div>--%>
                <%--</div>
            </div>
        </div>
        <div class="clearboth"></div>
    </div>--%>
    
</asp:Content>
