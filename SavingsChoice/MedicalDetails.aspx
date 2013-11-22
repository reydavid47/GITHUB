<%@ Page Title="" Language="C#" MasterPageFile="~/SavingsChoice/Details.master" AutoEventWireup="true" CodeFile="MedicalDetails.aspx.cs" Inherits="ClearCostWeb.SavingsChoice.MedicalDetails" %>
<%@ Register TagName="starRating" TagPrefix="cchUIControl" Src="~/Controls/StarRating.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" Runat="Server">
    <div style="float: left; width: 489px;">
        <h1>Medical Visits & Procedures</h1>
        <p><asp:LinkButton ID="LinkButton1" runat="server" CssClass="back" PostBackUrl="~/SearchInfo/Search.aspx" Text="Return to summary" /></p>
        </div> 
        <div style="float: right; width: 350px; padding-top: 15px;">

        <table cellspacing="0" cellpadding="0" border="0" style="width: 300px;">
        <tbody><tr>
        <td>
	        <div class="redbar" style="width: 140px; position: relative;"><img width="300" height="22" src="<%=ResolveUrl("/images/savings_bar.png") %>"><b style="position:absolute; left: 111px; top: -1px; color: white;">50%</b></div>
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
        	}        	
        #fairprice_providers 
        {
        	width:73%;
        	float:right;	
        	}
        .seeMoreSplit
        {
        	position:relative;
        	top:0px;
        	left:228px;
        	clear:both;
        	}	
        #fairprice_providers.cch_SCtable thead tr th:first-child
        {
        	width:45%;
        	}	
    </style>    
    <h3 class="savingschoice emphasis center">MEDICAL PROVIDERS USED RECENTLY BY YOUR FAMILY</h3>
    <table class="cch_SCtable recurring">
    <thead>
        <tr>
            <th><a href="#">Provider</a></th>
            <th><a href="#">Specialty</a></th>
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
                <td><a class="b viewableprovider" orglocid="<%# UsedOrganizationLocationID %>"><%# UsedProviderName%></a><br><%# UsedAddress1 %>, <%# UsedCity %>, <%# UsedState %></td>
                <td><%# UsedSpecialtyName %></td>
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
    </br>
    <hr class="savingschoice" />    
    <h3 class="savingschoice emphasis center updatableFairPrice">FAIR PRICE FACILITIES <hr style="margin:0px !important" /><em><asp:Label runat="server" ID="lblFairPriceAlternativeCategory"></asp:Label></em></h3>    
    <div id="subcategoryOptions" style="float: left; width: 220px;">
        <p><b>Select a recent specialty:</b></p>
        <asp:Repeater ID="medicalRecentSubcategory" runat="server">
            <ItemTemplate>
                <%# PrepRecentSubcategories(Container)%>
                <p>
                <input type="radio" name="recentSubCat" class="recentSubCategoryList" value="<%# RecentSubCategoryID%>"> <label><%# RecentSubCategoryName%></label>
                </p>            
            </ItemTemplate>
        </asp:Repeater>
        <p><b>Select other specialty</b></p>
        <select id="selAllSubCategoryList">
        <option class="allSubCategoryList" >select specialty</option>
        <asp:Repeater ID="medicalSubcategories" runat="server">
            <ItemTemplate>
                <%# PrepSubcategories(Container)%>
                <option class="allSubCategoryList" value="<%# SubCategory%>" subcategory="<%# SubCategoryID %>"> <%# SubCategory%></option>            
            </ItemTemplate>
        </asp:Repeater>      
        </select>
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
                <td><a class="b viewableprovider" href="javascript:__doPostBack('aProvider','linkProvider')"><%# ProviderName%></a><br><%# Address1 %>, <%# City %>, <%# State %></td>
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
    <p class="seeMoreSplit"></br><a class="readmore b">See more options</a></p>
    </br>
    <div style="text-align: right;">
        <div class="button-gray-white-arrow-lg displayinline"><a href="<%=ResolveUrl("switch.aspx") %>?scswitch_category=<%=Category %>&scswitch_step=1" id="beginSwitchStep" rel="beginSwitchStep" class="triggeroverlay">Select a Fair Price Facility</a></div>
    </div>
    <%-- ajax templpates : using post-rendered output : cloned then used with ajax data --%>
    <div id="ajaxTemplates" style="display:none">
        <%-- specify tempate : begin --%>
        <div id="category-mvp" style="display:none">
            <table>
            <tr class="tablerow" style="display: table-row;">
            <td><a class="b viewableprovider">~providername~</a><br>~provideraddress~</td>
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
    <!-- view provider details : ui triggers postback to create session variables needed when provider name (link) clicked -->
    <asp:Panel ID="postBackPanel" runat="server" style="display:none">
        <!-- input data as required -->        
        <asp:TextBox id="postProviderName" runat="server"></asp:TextBox>
        <asp:TextBox ID="postOrganizationLocationID" runat="server"></asp:TextBox>
        <asp:Button ID="btn_triggerPostBack" OnClick="btn_triggerPostBack_OnClick" runat="server" style="display:none" />    
    </asp:Panel>

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
        _cchGlobal.pageData["category"] = 'mvp';           
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

            /*-- sets up funtionality to view provider details -- */
            viewProvider({
                action: 'init'
            });

            /*-- sets up funtionality to save stars(if available) and continue to next page -- */
            continueBtn({
                action: 'init',
                element: '#beginSwitchStep'
            });

            /* -- addition cleanup, not categorized but required -- */
            /*page load*/
            additionalCleanUP({
                action: 'removeRecentFromList',
                callback: function () {
                    additionalCleanUP({
                        action: 'selectFirstRecentRadio',
                        callback: function () {
                            additionalCleanUP({
                                action: 'letsPlaySelect',
                                callback:function(){
                                    additionalCleanUP({
                                        action: 'tempStarRemove',
                                        callback: function () {
                                            additionalCleanUP({
                                                action: 'removeProviderLink'
                                            });
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
            });
            
        }

        function additionalCleanUP(iData) {
            /*
            -- iData Structure --
            iData.action
            iData.callback
            */
            switch (iData.action) {
                case 'removeRecentFromList':
                    /* cleans up subcategory list, removes if displayed as radio button (recent)*/
                    $('.recentSubCategoryList').each(function () {
                        var recentText = $(this).closest('p').text().trim();
                        $('#selAllSubCategoryList').children('option').each(function () {
                            if ($(this).text().trim() == recentText) {
                                $(this).remove();
                            }
                        });
                    });
                    break;
                case 'selectFirstRecentRadio':
                    /* -- auto selects first radio if available --*/
                    $('.recentSubCategoryList').eq(0).prop('checked', true);
                    break;
                case 'letsPlaySelect':
                    /*-- modify select : keeping in case addition ui is needed --*/
                    var thisSelect = $('#selAllSubCategoryList');
                    $(thisSelect).attr('oWidth', $(this).width());
                    $(thisSelect).width('190px');
                    break;
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
        function showMoreProviders() {
            $('.readmore.b').unbind('click');
            $('.readmore.b').on('click', function () {
                var oData;
                oData = [
                        'CCHID:<%=PrimaryCCHID %>',
                        'returnType:json',
                        'startIndex:' + parseInt($('#fairprice_providers tbody tr').length + 1),
                        'stopIndex:' + parseInt(parseInt($('#fairprice_providers tbody tr').length) + parseInt(5)),
                        'SubCategory:' + _cchGlobal.pageData.selectedSubCategory
                    ];
                cch_showMore({
                    action: 'categoryDetail-MVP',
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
        };
        function loadHandlers() {
            var _showMore = function () {
                showMoreProviders();
            }
            var _selectableSubcategories = function () {
                /*-- recent subcategory visits (radio button) --*/
                $('.recentSubCategoryList').on('click', function () {
                    /* -- sets pageData subcategory for see more functionality -- */
                    _cchGlobal.pageData['selectedSubCategory'] = $(this).val();
                    /* -- clears table contents -- */
                    $('#fairprice_providers tbody tr').remove();
                    /* -- hide see more option until table populated -- */
                    $('.readmore.b').hide();
                    var oData;
                    oData = [
                        'CCHID:<%=PrimaryCCHID %>',
                        'returnType:json',
                        'latitude:' + $("#PATIENTLATITUDE").val(),
                        'longitude:' + $("#PATIENTLONGITUDE").val(),
                        'startIndex:0',
                        'stopIndex:5',
                        'SubCategory:' + $(this).val()
                    ];
                    cch_showMore({
                        action: 'categoryDetail-MVP',
                        data: oData,
                        callback: function (iData) {
                            loadNewData({
                                data: iData,
                                callback: function () {
                                    fairPriceProviderTable({
                                        action: 'checkLength'
                                    });
                                }
                            });
                        }
                    });

                    /* - updates fair price header based on input text - */
                    $('.updatableFairPrice em').text($(this).parent().text().trim());

                    /* - resets dropdown state when radio selected - */
                    $('#selAllSubCategoryList').children('option').each(function () {
                        $(this).removeAttr('selected');
                    });
                });
                /*-- all avilable subcategories (select) --*/
                $('#selAllSubCategoryList').on('change', function () {
                    /* -- sets pageData subcategory for see more functionality -- */
                    _cchGlobal.pageData['selectedSubCategory'] = $('#selAllSubCategoryList').children('option:selected').attr('subcategory');
                    /* -- clears table contents -- */
                    $('#fairprice_providers tbody tr').remove();
                    /* -- hide see more option until table populated -- */
                    $('.readmore.b').hide();
                    var oData;
                    oData = [
                        'CCHID:<%=PrimaryCCHID %>',
                        'returnType:json',
                        'startIndex:0',
                        'stopIndex:5',
                        'SubCategory:' + $('#selAllSubCategoryList').children('option:selected').attr('subcategory')
                    ];
                    cch_showMore({
                        action: 'categoryDetail-MVP',
                        data: oData,
                        callback: function (iData) {
                            loadNewData({
                                data: iData,
                                callback: function () {
                                    fairPriceProviderTable({
                                        action: 'checkLength'
                                    });
                                }
                            });
                        }
                    });

                    /* - updates fair price header based on selected option text - */
                    $('.updatableFairPrice em').text($('#selAllSubCategoryList').children('option:selected').text().trim());

                    /* - resets radio state when dropdown altered - */
                    $('.recentSubCategoryList').each(function () {
                        $(this).removeAttr('checked');
                    });
                });
            }
            _showMore();
            _selectableSubcategories();
        }
        function loadNewData(iData) {
            /*
            -- iData Structure --
            iData.data
            iData.callback
            */
            var dataItems = iData.data.jsonItems;
            var dataItemsFound = 0;
            for (each in dataItems) {
                /* -- rendered html:clone -- */
                var newrow = $('#ajaxTemplates #category-mvp').find('tr').clone();
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
                dataItemsFound++;
            }
            if (dataItemsFound == 0) {
                $('.readmore.b').text('Showing all providers').unbind('click');
            }
            else {
                $('.readmore.b').text('See more options');
                /* -- re-init functionality -- */
                showMoreProviders();
            }

                if ($('#fairprice_providers .tablerow').length < 5) {
                    $('a.readmore.b').hide();
                }
                else {
                    $('.readmore.b').show();                
                }
            
            if (iData.callback!=undefined) { iData.callback(); }
        };
        function viewProvider(iData) {
            /*
            --iData Structure--
            iData.action
            */            
            switch(iData.action){
                case 'init':
                    $('.viewableprovider:visible').each(function () {
                        $(this).unbind('click');
                        $(this).on('click', function () {
                            /*gather and place ui data*/
                            $('#ResultsContent_cphContentBar_postProviderName').val($(this).text());
                            $('#ResultsContent_cphContentBar_postOrganizationLocationID').val($(this).attr('orglocid'));
                            /*send post*/
                            $('#ResultsContent_cphContentBar_btn_triggerPostBack').trigger('click');
                        });
                    });
                    break;
            }
        }
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

