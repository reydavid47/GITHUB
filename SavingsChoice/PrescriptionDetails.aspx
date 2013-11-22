<%@ Page Title="" Language="C#" MasterPageFile="~/SavingsChoice/Details.master" AutoEventWireup="true" CodeFile="PrescriptionDetails.aspx.cs" Inherits="ClearCostWeb.SavingsChoice.PrescriptionDetails " %>
<%@ Register TagName="starRating" TagPrefix="cchUIControl" Src="~/Controls/StarRating.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" Runat="Server">
    <div style="float: left; width: 489px;">
        <h1>Prescription Drugs</h1>
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
        .cch_SCtable.recurring thead tr th:first-child
        {
        	width:25%;
        	}
        .cch_SCtable.recurring thead tr th:nth-child(2)
        {
        	width:25%;
        	text-align:left !important;
        	font-size:12px;
        	}        	
        .cch_SCtable.recurring tbody tr td:nth-child(2)
        {
        	text-align:left !important;
        	}        	        	
        #fairprice_providers 
        {
        	width:73%;
        	float:right;	
        	}
        .seeMoreSplit
        {
        	position:relative;
        	left:227px;
        	}	
        #fairprice_providers.cch_SCtable thead tr th:first-child
        {
        	width:45%;
        	}	
    </style>   
    <h3 class="savingschoice emphasis center">RECURRING PRESCRIPTION DRUGS USED RECENTLY</h3>
    <table class="cch_SCtable recurring">
    <thead>
        <tr>
            <th><a>Medication</a></th>
            <th><a href="#">Facility</a></th>
            <th><a href="#">Number of Fills</a></th>
            <th>Fair Price</th>
            <th><a href="#">Your Satisfaction</a></th>
        </tr>    
    </thead>
    <tbody>
        <asp:Repeater ID="recentProviders" runat="server">
            <ItemTemplate>
                <%# PrepUsedLabsData(Container)%>
                <tr>
                <td><%# UsedDrugName%></td>
                <td><a class="b"><%# UsedProviderName%></a><br><%# UsedAddress1 %>, <%# UsedCity %>, <%# UsedState %></td>
                <td><%# UsedServiceCount %></td>
                <td><%# UsedFairPriceImage %></td>
                <td>    
                    <div class="userRatings <%= StarType %> miniStar" existingRating="<%# UsedSatisfaction %>" pid="<%# UsedProviderID %>" oid="<%# UsedOrgID %>">
                        <cchUIControl:starRating ID="StarRating3" runat="server" />
                    </div>                                                 
                </td>
                </tr>            
            </ItemTemplate>
        </asp:Repeater> 
    </tbody>       
    </table>
    </br>
    <hr class="savingschoice" />    
    <h3 class="savingschoice emphasis center">FAIR PRICE OPTIONS FOR PRESCRIPTION DRUGS</h3>        
    <div id="subCategoriesWrapper" style="float: left; width: 220px;">
        <p><b>Your medications:</b></p>
        <asp:Repeater ID="subCategorySelections" runat="server">
            <ItemTemplate>
                <%# PrepSubcategories(Container)%>
                <p>
                <input type="radio" name="radsubcat" class="subcategories" cchdata="GPI:<%# GPI %>,GenericIndicator:<%#GenericIndicator %>"  value="<%# SubCategory%>"> <%# SubCategory%>
                </p>            
            </ItemTemplate>
        </asp:Repeater>
    </div>
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
    <div class="clear"></div>
    <p class="seeMoreSplit"></br><a class="readmore b">See more options</a></p>
    </br>
    <div style="text-align: right;">
        <div class="button-gray-white-arrow-lg displayinline"><a href="<%=ResolveUrl("switch.aspx") %>?scswitch_category=<%=Category %>&scswitch_step=1" id="prescriptionjourney" rel="prescriptionjourney" class="triggeroverlay">Select a Fair Price Pharmacy</a></div>
    </div>
    <div id="ajaxTemplates" style="display:none">
        <div id="category-rx" style="display:none">
            <table>
            <tr class="tablerow" style="display: table-row;">
            <td><a href="" class="b">~providername~</a><br>~provideraddress~</td>
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
    </div>
    <!-- css order required : begin -->
    <link href="../Styles/starRatings.css" rel="stylesheet" type="text/css" />   
    <link href="../Styles/SavingsChoiceDetail.css" rel="stylesheet" type="text/css" />   
    <!-- css order required : end -->
    <script src="../Scripts/cch_global.js" type="text/javascript"></script>            
    <script src="../Scripts/cch_starRating.js" type="text/javascript"></script>
    <script src="../Scripts/cch_showMore.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        _cchGlobal.pageData["LoadedGPI"] = '<%=LoadedGPI %>';
        _cchGlobal.pageData["LoadedGenericIndicator"] = '<%=LoadedGenericIndicator %>';
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
        }
        function loadHandlers() {
            var _showMore = function () {
                $('.readmore.b').on('click', function () {
                    $('#fairprice_providers').fadeTo('slow', 0);
                    var oData;
                    oData = [
                        'CCHID:<%=PrimaryCCHID %>',
                        'GPI:' + _cchGlobal.pageData.LoadedGPI,
                        'GenericIndicator:' + _cchGlobal.pageData.LoadedGenericIndicator,
                        'returnType:json',
                        'latitude:' + $("#PATIENTLATITUDE").val(),
                        'longitude:' + $("#PATIENTLONGITUDE").val(),
                        'startIndex:' + $('#fairprice_providers tbody tr').length,
                        'stopIndex:' + parseInt(parseInt($('#fairprice_providers tbody tr').length) + parseInt(5))
                    ];
                    console.log(oData);
                    cch_showMore({
                        action: 'categoryDetail-RX',
                        data: oData,
                        callback: function (iData) {
                            console.log('iData');
                            console.log(iData);
                            loadNewData({
                                data:iData,
                                callback: function () {
                                    setTimeout(function () {
                                        $('#fairprice_providers').fadeTo('fast', 1);
                                    }, 250);
                                }
                            });
                        }
                    });
                });
            }
            var _selectableSubcategories = function () {
                $('#subCategoriesWrapper p input').on('click', function () {
                    
                    var oData;
                    oData = [
                        'CCHID:<%=PrimaryCCHID %>',
                        'GPI:90850060104005',
                        'GenericIndicator:1',
                        'returnType:json',
                        'startIndex:' + $('#fairprice_providers tbody tr').length,
                        'stopIndex:' + parseInt(parseInt($('#fairprice_providers tbody tr').length) + parseInt(5))
                    ];
                    cch_showMore({
                        action: 'categoryDetail-RX',
                        data: oData,
                        callback: function (iData) {
                            loadNewData({
                                data: iData,
                                callback: function () {
                                    setTimeout(function () {
                                        $('#fairprice_providers').fadeTo('fast', 1);
                                    }, 250);                                
                                }
                            });
                        }
                    });
                });
            }
            _showMore();
            _selectableSubcategories();
        }

        function loadNewData(iData) {
            console.log(iData.data.jsonItems);
            /*
            -- iData Structure --
            iData.data
            iData.callback
            */
            var dataItems = iData.data.jsonItems;
            for (each in dataItems) {
                /* -- rendered html:clone -- */
                var newrow = $('#ajaxTemplates #category-rx').find('tr').clone();
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
                console.log(dataItems[each]);
            }
            if (dataItems == undefined) {
                $('.readmore.b').text('Showing all providers').unbind('click');
            }
            if (iData.callback) {iData.callback();}       
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

