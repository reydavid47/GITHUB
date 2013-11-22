<%@ Page Title="" Language="C#" MasterPageFile="~/SavingsChoice/Details.master" AutoEventWireup="true" CodeFile="LabDetails.aspx.cs" Inherits="ClearCostWeb.SavingsChoice.LabDetails" %>
<%@ Register TagName="starRating" TagPrefix="cchUIControl" Src="~/Controls/StarRating.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" Runat="Server">
    <div style="float: left; width: 489px;">
        <h1>Labs</h1>
        <p><asp:LinkButton runat="server" CssClass="back" PostBackUrl="~/SearchInfo/Search.aspx" Text="Return to summary" /></p>
        </div> 
        <div style="float: right; width: 350px; padding-top: 15px;">

        <table cellspacing="0" cellpadding="0" border="0" style="width: 300px;">
        <tbody><tr>
        <td>
	        <div class="redbar" style="width: 140px; position: relative;"><img width="300" height="22" src="<%=ResolveUrl("~/images/savings_bar.png") %>"><b style="position:absolute; left: 111px; top: -1px; color: white;">50%</b></div>
        </td>
        </tr>
        </tbody></table>
        </div>
        <div class="clearboth"></div>
            
    <hr class="savingschoice"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphSideBar" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContentBar" Runat="Server">
    <style type="text/css">
        .cch_SCtable thead tr th:first-child
        {
        	width:45%;
        	}       
    </style>   
    <h3 class="savingschoice emphasis center">Labs used recently</h3>
    <table class="cch_SCtable recurring">
    <thead>
        <tr>
            <th><a href="#">Facility</a></th>
            <th><a href="#">Services</a></th>
            <th>Fair Price</th>
            <th><a href="#">Your Satisfaction</a></th>
        </tr>    
    </thead>
    <tbody>
        <asp:Repeater ID="recentProviders" runat="server">
            <ItemTemplate>
                <%# PrepUsedLabsData(Container)%>
                <tr>
                <td><a class="b"><%# UsedProviderName%></a><br><%# UsedAddress1 %>, <%# UsedCity %>, <%# UsedState %></td>
                <td><%# UsedServiceCount %></td>
                <td><%# UsedFairPriceImage %></td>
                <td>
                    <div class="userRatings <%= StarType %> miniStar" existingRating="<%# UsedSatisfaction %>" pid="<%# UsedProviderID %>" oid="<%# UsedOrgID %>">
                        <cchUIControl:starRating ID="StarRating1" runat="server" />
                    </div>                                       
                </td>
                </tr>            
            </ItemTemplate>
        </asp:Repeater> 
    </tbody>       
    </table>
    </br></br>   
    <h3 class="savingschoice emphasis center">Fair Price Labs</h3>    
    <table id="fairprice_providers" class="cch_SCtable">
    <thead>
        <tr>
            <th><a href="#">Facility</a></th>
            <th><a href="#">Distance</a></th>
            <th>Fair Price</th>
            <th><a href="#">Average Patient<br />Satisfaction</a></th>
        </tr>    
    </thead>
    <tbody>
        <asp:Repeater ID="fairPriceProviders" runat="server">
            <ItemTemplate>
                <%# PrepFairPriceProviders(Container) %>
                <tr class="tablerow">
                <td><a class="b"><%# ProviderName%></a><br><%# Address1 %>, <%# City %>, <%# State %></td>
                <td><%# (float)(Math.Round((double)Distance, 1)) %> mi</td>
                <td><img width="23" height="23" border="" alt="X" src="<%=ResolveUrl("~/images/check_green.png")%>"></td>
                <td>
                    <div class="userRatings inactiveStar miniStar" existingRating="<%# Satisfaction %>">
                        <cchUIControl:starRating ID="StarRating2" runat="server" />
                    </div>
                    <label><%# Rating %> ratings</label>
                </td>
                </tr>            
            </ItemTemplate>
        </asp:Repeater> 
    </tbody>       
    </table>
    <div class="clearboth"></div>
    <p></br><a class="readmore b">See more options</a></p>
    <div style="text-align: right;">
        <div class="button-gray-white-arrow-lg displayinline"><a href="<%=ResolveUrl("switch.aspx") %>?scswitch_category=<%=Category %>&scswitch_step=1" id="beginSwitchStep" rel="beginSwitchStep" class="triggeroverlay">Select a Fair Price Lab</a></div>
    </div>
    <%-- ajax templpates : using post-rendered output : cloned then used with ajax data --%>
    <div id="ajaxTemplates" style="display:none">
        <%-- specify tempate : begin --%>
        <div id="category-imaging" style="display:none">
            <table>
            <tr class="tablerow" style="display: table-row;">
            <td><a class="b">~providername~</a><br>~provideraddress~</td>
            <td>~providerdistance~ mi</td>
            <td><img width="23" height="23" border="" alt="X" src="<%=ResolveUrl("~/images/check_green.png")%>"></td>
            <td>
                <div class="userRatings inactiveStar miniStar" existingRating="~providerexistingrating~">
                    <cchUIControl:starRating ID="StarRating2" runat="server" />
                </div>
                <label>~providerratings~ ratings</label>
            </td>
            </tr>
            </table>
        </div>
        <%-- specify tempate : end --%>
    </div>
    <!-- css order required : begin -->
    <link href="../Styles/starRatings.css" rel="stylesheet" type="text/css" />   
    <link href="../Styles/SavingsChoiceDetail.css" rel="stylesheet" type="text/css" />   
    <link href="../Styles/tipsy.css" rel="stylesheet" type="text/css" />
    <!-- css order required : end -->
    <script src="../Scripts/cch_global.js" type="text/javascript"></script>            
    <script src="../Scripts/cch_starRating.js" type="text/javascript"></script>
    <script src="../Scripts/cch_SavingsChoiceCategoryDetails.js" type="text/javascript"></script>
    <script src="../Scripts/cch_showMore.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.tipsy.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        /*cch_ui - page*/
        _cchGlobal.pageData["cchid"] = '<%= PrimaryCCHID %>';
        _cchGlobal.pageData["eid"] = '<%= EmployerID %>';
        _cchGlobal.pageData["category"] = 'lab';       
        function prepUI() {
            /*preps table to show first 5*/
            $('#fairprice_providers .tablerow').each(function () {
                if ($(this).index() < 5) {
                    $(this).show();
                }
            });

            /*hide showmore button if not needed*/
            if ($('#fairprice_providers .tablerow').length < 5) {
                $('a.readmore.b').hide();
            }

            /* -- recent provider table, checks records, if none remove/hide table*/
            recentProviderTable({
                action: 'checkLength'
            });
            /* -- fair price provider table, checks records, if none remove/hide table*/
            fairPriceProviderTable({
                action: 'checkLength',
                category: _cchGlobal.pageData.category
            });
            /*-- sets up funtionality to save stars(if available) and continue to next page -- */
            continueBtn({
                action: 'init',
                element: '#beginSwitchStep'
            });
            /*-- addition clean up needed --*/
            additionalCleanUP({
                action: 'tempStarRemove',
                callback: function () {
                    additionalCleanUP({
                        action: 'removeProviderLink'
                    });
                }
            });
        }
        function loadHandlers() {
            var _showMore = function () {
                $('.readmore.b').on('click', function () {
                    var oData;
                    oData = [
                        'CCHID:<%=PrimaryCCHID %>',
                        'returnType:json',
                        'latitude:' + $("#PATIENTLATITUDE").val(),
                        'longitude:' + $("#PATIENTLONGITUDE").val(),
                        'startIndex:' + parseInt($('#fairprice_providers tbody tr').length + 1),
                        'stopIndex:' + parseInt(parseInt($('#fairprice_providers tbody tr').length) + parseInt(5))
                    ];
                    cch_showMore({
                        action: 'categoryDetail-Lab',
                        data: oData,
                        callback: function (iData) {
                            loadNewData({
                                data: iData,
                                callback: function () {

                                }
                            });
                        }
                    });
                });
            }
            _showMore();
        }
        function additionalCleanUP(iData) {
            /*
            -- iData Structure --
            iData.action
            iData.callback
            */
            switch (iData.action) {                
                case 'tempStarRemove':
                    $('.cch_SCtable.recurring tbody td:last-child').each(function () {
                        if ($(this).find('.userRatings').attr('existingrating') == 0) {
                            $(this).html('');
                        }
                    });
                    break;
                case 'removeProviderLink':
                    $('.cch_SCtable.recurring tbody tr').each(function () {
                        if ($(this).find('td:last-child').text().trim() == '' || $(this).find('td:last-child div.userRatings').attr('pid') == 0) {
                            var providerName = $(this).find('td:first-child a');
                            console.log(providerName);
                            var PNLMessage = 'Insufficient data on ' + $(providerName).text() + '  to show details';
                            $(this).find('td:first-child').html(providerName).find('a').css({ 'color': '#333', 'cursor': 'default' }).attr('original-title', PNLMessage).tipsy().on('click', function (e) {
                                e.preventDefault();
                            });
                        }
                    });
                    break;
            }
            if (iData.callback != undefined) {
                iData.callback();
            }
        };
        function loadNewData(iData) {
            /*
            -- iData Structure --
            iData.data
            iData.callback
            */
            var dataItems = iData.data.jsonItems;
            function divider() {
                var itemsFound = 0;
                var dividerHTML = '<tr><td colspan="4"><hr></td></tr>';
                for (item in dataItems) {
                    itemsFound++;
                }
                if (itemsFound > 0) {
                    $('#fairprice_providers tbody').append(dividerHTML);
                }
            };
            //divider();
            for (each in dataItems) {
                /* -- rendered html:clone -- */
                var newrow = $('#ajaxTemplates #category-imaging').find('tr').clone();
                /*updating provider - name*/
                $(newrow).find('td:first-child a').text(dataItems[each].providername);
                /*updating provider - address*/
                var providerAddress = dataItems[each].address1 + ', ' + dataItems[each].city + ', ' + dataItems[each].state;
                var updateAddress = $(newrow).find('td:first-child').html();
                updateAddress = updateAddress.replace('~provideraddress~', providerAddress);
                $(newrow).find('td:first-child').html(updateAddress)
                /*updating provider - distance*/
                $(newrow).find('td:nth-child(2)').text(parseFloat(dataItems[each].distance).toFixed(1) + ' mi');
                /*update provider ratings count*/
                $(newrow).find('td:nth-child(4) div.userRatings').attr('existingrating', dataItems[each].patientratings);
                /*updating provider ratings text below stars*/
                $(newrow).find('td:nth-child(4)').find('label').text(dataItems[each].patientratings + ' ratings');
                /* -- append new item -- */
                $('#fairprice_providers tbody').append(newrow);
            }
            if (dataItems == undefined) {
                $('.readmore.b').text('Showing all providers').unbind('click');
            }
            if (iData.callback) { iData.callback(); }
        };
        //$(document).ready(function () {
            prepUI();
            loadHandlers();
            /* -- builds all star elements, functionality, and loads original value if exists -- */
            starRating({
                action: 'init',
                callback: function () {
                    starRating({
                        action: 'showOriginalRating'
                    })
                }
            });
        //}); 
    </script>
</asp:Content>

