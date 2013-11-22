<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SavingsChoiceSwitchSteps_Radiology.ascx.cs" Inherits="ClearCostWeb.SavingsChoice.SavingsChoiceSwitchSteps_Radiology" %>
<style type="text/css">
    .cch_SCtable tbody tr td:nth-child(2), .cch_SCtable tbody tr td:nth-child(3)
    {
        padding:5px 0px 0px;
        }
    .togglecontent 
    {
    	display:none;
    	}          
</style>    
<script src="<%=ResolveUrl("/Scripts/zebra_datepicker.js") %>" type="text/javascript"></script>
<script src="<%=ResolveUrl("/Scripts/SavingsChoiceSwitchSteps.js") %>" type="text/javascript"></script>
    <div id="tabchoice" class="ui-tabs-panel ui-widget-content ui-corner-bottom">        
        <h1>Save Money on Radiology</h1>
        <p><asp:HyperLink ID="goBack" runat="server" CssClass="back">Return to previous page</asp:HyperLink></p>
        <div id="switchStepGuIDe" cchid="<%=PrimaryCCHID %>" ssid="<%=dbSessionID %>" style="display:none">&nbsp;</div>
        <asp:Panel ID="step1" runat="server" Visible="false" CssClass="stepWrapper" >           
            <p class="subhead">
                Saving money on radiology is simple! We'll walk you step-by-step through the process. If you have any questions, feel free to call our Health Shoppers at (800) 390-6855.
            </p>
            <div class="subpanel">
                <p class="switchquestions">
                    How do you want to select a new radiology facility?
                </p>
                <asp:Repeater ID="step1DisplayQuestions" runat="server">
                <ItemTemplate>
                    <p class="switchradio">
                        <input type="radio" class="answer" decisionid="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>" stepnum="<%#DataBinder.Eval(Container.DataItem,"stepnum")%>" name="q1" value="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>"><%#DataBinder.Eval(Container.DataItem,"decisionvalue") %>
                    </p>
                </ItemTemplate>
                </asp:Repeater>    
                <p class="switchquestions">
                    Select a type of radiology
                </p>
                <asp:Repeater ID="step2DisplayQuestions" runat="server">
                <ItemTemplate>
                    <p>
                        <input type="radio" class="answer" decisionid="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>" stepnum="<%#DataBinder.Eval(Container.DataItem,"stepnum")%>" name="q2" value="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>" subcattext="<%#DataBinder.Eval(Container.DataItem,"decisionvalue") %>"><%#DataBinder.Eval(Container.DataItem,"decisionvalue") %>
                    </p>
                </ItemTemplate>
                </asp:Repeater>                               
            </div><!-- end subpanel -->    
            <script type="text/javascript">
                function loadHandlers() {
                    var _saveRadiologySubcategory = function () {
                        $('#ResultsContent_cphContentBar_ctl00_switchContinue').on('click', function (e) {
                            e.preventDefault();
                            window.location.href = $(this).attr('href') + '&selectedtype='+$('.answer[name=q2]:checked').attr('subcattext');
                        });
                    };
                    _saveRadiologySubcategory();
                }
                $(document).ready(function () {
                    loadHandlers();
                });
            </script>        
        </asp:Panel>
        <asp:Panel ID="step2" runat="server" Visible="false" CssClass="stepWrapper" >
            <style type="text/css">
            .cch_SCtable
            {
            	width:94%;
            	}            	
            .cch_SCtable tbody tr:nth-child(n+6)
            {
            	display:none;
            	}
            .cch_SCtable tbody tr td:nth-child(n+5)
            {
            	display:none;
            	}
            .cch_SCtable tbody tr td:nth-child(4)
            {
            	border-right:1px solid #DBD1E2
            	}	
            .subhead label
            {
            	font-weight:bold;
            	font-size:1em;
            	}	
            </style>   
            <p class="subhead">
                Nearby Fair Price facilities for <label><%=radiologySubCategory %></label> are listed below. Please uncheck any facilities you are not interested in visiting.
            </p>                                          
            <div class="subpanel">
                <table class="cch_SCtable">
                <thead>
                    <tr>
                        <th><a href="#">Facility</a></th>
                        <th><a href="#">Distance</a></th>
                        <th>Fair Price</th>
                        <th>Select</th>
                        <th><!--placeholder for address--></th>
                        <th><!--placeholder for contact info--></th>
                    </tr>    
                </thead>
                <tbody>
                    <asp:Repeater ID="step2DisplayContent" runat="server">
                        <ItemTemplate>
                            <tr>
                            <td><a class="b"><%#DataBinder.Eval(Container.DataItem,"ProviderName") %></a></td>
                            <td><%#(float)(Math.Round((double)Convert.ToDouble(DataBinder.Eval(Container.DataItem, "Distance")), 1))%>&nbsp; mi</td>
                            <td><img src="<%=ResolveUrl("~/images/check_green.png") %>" /></td>
                            <td><input class="providerlist" provid="<%#DataBinder.Eval(Container.DataItem,"ProviderID") %>" type="checkbox" checked="checked" /></td>
                            <td><%#DataBinder.Eval(Container.DataItem,"Address1") %></br><%#DataBinder.Eval(Container.DataItem,"City") %>, <%#DataBinder.Eval(Container.DataItem,"State") %>&nbsp;<%#DataBinder.Eval(Container.DataItem,"Zipcode").ToString().Substring(0,5) %></td>
                            <td>P: <%#DataBinder.Eval(Container.DataItem,"Telephone") %></td>
                        </ItemTemplate>
                    </asp:Repeater> 
                </tbody>       
                </table>
                <p><br><a class="readmore b">See more options</a></p>
            </div><!-- end subpanel -->    
            <script type="text/javascript">
                function loadHandlers() {
                    //show >5 rows click event
                    var _showMore = function () {
                        $('.readmore.b').on('click', function () {
                            if ($(this).attr('id') == 'ResultsContent_cphContentBar_ctl00_switchContinue') {
                                return;
                            }
                            $('.cch_SCtable tbody tr').each(function () {
                                if ($(this).index() > 4) {
                                    $(this).slideDown();
                                }
                            });
                            $(this).hide();
                        });
                    }
                    //track selected/unselected switch providers
                    var selectedItemsList = {};
                    var _saveSelectedFairProviders = function () {
                        //saves to client session, for re-display later 
                        var displayTable = '.cch_SCtable';
                        var selectableItems = '.cch_SCtable input:checkbox';
                        $(selectableItems).each(function () {
                            var thisIndex = $(this).index(selectableItems)
                            selectedItemsList[thisIndex] = true;
                        });
                        sessionStorage.setItem('switchStepDisplaySelections', JSON.stringify(selectedItemsList));                        
                        $(selectableItems).on('click', function () {
                            var thisIndex = $(this).index(selectableItems);
                            var checked = $(this).prop('checked');
                            if (checked == true) {
                                selectedItemsList[thisIndex] = true;
                            }
                            else {
                                selectedItemsList[thisIndex] = false;
                            }
                            sessionStorage.setItem('switchStepDisplaySelections', JSON.stringify(selectedItemsList));
                        });
                        //saves to web service on continue
                        var closeSessionItem = '#ResultsContent_cphContentBar_ctl00_switchContinue';
                        $(closeSessionItem).on('click', function (e) {
                            e.preventDefault();                    
                            function getProviderList() {
                                var providerList = [];
                                $('.providerlist').each(function () {
                                    providerList.push($(this).attr('provid'));
                                });
                                return providerList.join(',');
                            }
                            switchStep.saveSelectedFairPriceProviders({
                                callback: function () {
                                    window.location.href = $(closeSessionItem).attr('href');
                                },
                                providerlist:getProviderList()
                            });
                        });

                    };
                    _showMore();
                    _saveSelectedFairProviders();
                }
                $(document).ready(function () {
                    loadHandlers();
                });
            </script>                  
        </asp:Panel>
        <asp:Panel ID="step3" runat="server" Visible="false" CssClass="stepWrapper" >      
            <asp:Repeater ID="step3DisplayQuestions" runat="server">
            <ItemTemplate>
                <p>
                    <%--<input type="radio" class="answer" decisionid="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>" stepnum="<%#DataBinder.Eval(Container.DataItem,"stepnum")%>" name="q2" value="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>" subcattext="<%#DataBinder.Eval(Container.DataItem,"decisionvalue") %>"><%#DataBinder.Eval(Container.DataItem,"decisionvalue") %>--%>
                </p>
            </ItemTemplate>
            </asp:Repeater>      
            <p class="subhead">Do you want to contact your doctor now or would you like to wait until the next time you see your doctor?</p>
            <div class="subpanel">
                <asp:Repeater ID="step4DisplayQuestions" runat="server">
                <ItemTemplate>
                    <p class="switchradio">
                        <input type="radio" class="answer" decisionid="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>" stepnum="<%#DataBinder.Eval(Container.DataItem,"stepnum")%>" name="q1" value="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>"><%#DataBinder.Eval(Container.DataItem,"decisionvalue") %>
                    </p>
                </ItemTemplate>
                </asp:Repeater>            

                <div style="margin: 0; padding: 0; display: none;" id="willwait">
                <p class="subhead">Do you want an email reminder closer to a time when you will see your doctor (e.g. your next appointment)?</p>
                    <div class="subpanel">
                    <asp:Repeater ID="step5DisplayQuestions" runat="server">
                    <ItemTemplate>
                        <p class="switchradio">
                        <input type="radio" class="answer" decisionid="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>" stepnum="<%#DataBinder.Eval(Container.DataItem,"stepnum")%>" name="q2" value="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>"><%#DataBinder.Eval(Container.DataItem,"decisionvalue") %>
                        </p>
                    </ItemTemplate>
                    </asp:Repeater>                     
                    </div><!-- end subpanel -->
                </div><!-- end question2 -->

                <div style="margin: 0; padding: 0; display: none;" id="requestEmail">
                    <p class="subhead">
                    When would you like a reminder?
                    </p>
                    <div class="subpanel" style="position: relative;">
                    <input id="reminderdate" value="" class="boxed" style="margin-bottom: 5px;" readonly="" type="text"><%--<button style="left: 335px; top: 7px; display: inline;" type="button" class="Zebra_DatePicker_Icon">Pick a date</button>--%>
                    <span id="remindernote" style="display: none;" class="b green">Great, we'll send you a reminder on </span>
                    </div><!-- end subpanel -->
                </div><!-- end question3 -->

                <div style="margin: 0; padding: 0; display: none;" id="declineEmail">
                    <div class="subpanel">
                    <p class="b green">
                    Ok, just remember to take a look at your account before speaking with your doctor.</p>
                    </div><!-- end subpanel -->
                </div><!-- end question4 -->
            </div><!-- end subpanel -->
            <script type="text/javascript">
                function loadHandlers() {
                    //last checkbox opens content below
                    var _waitDisplay = function () {
                        var clickWait = '#willwait';
                        var waitappointment = $(clickWait).prev().children('input');
                        $(waitappointment).on('click', function () {
                            $(clickWait).show();
                        });
                        $(waitappointment).parent().prev().on('click', function () {
                            $(clickWait).hide();
                        });
                    };
                    var _emailDisplay = function () {
                        var clickEmail = '#willwait .subpanel';
                        var requestEmail = $(clickEmail).children(':first-child');
                        var rejectEmail = $(clickEmail).children(':last-child');
                        $(requestEmail).on('click', function () {
                            $('#requestEmail').show();
                            $('#declineEmail').hide();
                        });
                        $(rejectEmail).on('click', function () {
                            $('#requestEmail').hide();
                            $('#declineEmail').show();
                        });
                    };                    
                    _waitDisplay();
                    _emailDisplay();
                };
                function zebradatePicker() {
                    $('#reminderdate').Zebra_DatePicker({
                        inside: false,
                        direction: true,
                        first_day_of_week: 0,
                        format: 'm/d/Y',
                        offset: [-230, 215],
                        onSelect: function () {
                            var dateStr = document.getElementById('reminderdate').value;
                            $("#remindernote").append(dateStr).append('!');
                            $("#remindernote").show();
                            switchStep.sendEmailReminder();
                        }
                    });
                }
                $(document).ready(function () {
                    loadHandlers();
                    zebradatePicker();
                });
            </script>  
        </asp:Panel>
        <asp:Panel ID="step4" runat="server" Visible="false" CssClass="stepWrapper" >
            <style type="text/css">
            .cch_SCtable
            {
            	width:94%;
            	}  
            .cch_SCtable tbody tr td:nth-child(2), .cch_SCtable tbody tr td:nth-child(3), .cch_SCtable tbody tr td:nth-child(4),
            .cch_SCtable thead
            {
            	display:none;
            	}            	          	            
            .cch_SCtable tbody tr td:nth-child(5)
            {
            	text-align:left;
            	}	
            </style>
            <div class="subpanel">
            <h3 class="savingschoice">How to transfer your lab tests to a Fair Price facility:</h3>

            <div style="margin-top: 20px;">
            <a class="plusminus listclosed">Decide which radiology facility you'd like to visit.</a>
            <div class="togglecontent subpanel">
                <table id="displayTable" class="cch_SCtable"></table>
            </div><!-- end togglecontent subpanel -->
            </div><!-- end margintop20 -->

            <div style="margin-top: 20px;">
            <a class="plusminus listclosed">Call your preferred radiology provider to make an appointment. They will ask for some personal information.</a>
            <div class="togglecontent subpanel">
            <p>
                <b>Make sure that you have the following personal information:</b><br>
                Name<br>Date of birth<br>Insurance member ID number<br>Address<br>Phone number
            </p>
            <p>
                <b>You will also need the following information about the doctor ordering the lab test:</b><br>
                Name<br>Address<br>Phone and fax numbers
            </p>
            </div><!-- end togglecontent subpanel -->
            </div><!-- end margintop20 -->

            <div style="margin-top: 20px;">
                <a class="plusminus listclosed">Make sure you have a radiology requisition form from your doctor.</a>
                <div class="togglecontent subpanel">
                    <p>The requisition form includes your doctor's signature, phone and fax numbers and address, along with details about the procedure.</p>
                    <p>If you do not have this form, request one from your doctor.</p>        
                    <p>You can bring this to your radiology appointment, or send it via fax or email prior to your appointment.</p>
                </div>
            </div>
            </div><!-- end subpanel -->
            <br><br>
            <div>
                <asp:Repeater ID="step6DisplayQuestions" runat="server">
                <ItemTemplate>
                    <div class="button" id="switchstepresults_<%#DataBinder.Eval(Container.DataItem,"decisionvalue") %>"><a class="answer" decisionid="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>" stepnum="<%#DataBinder.Eval(Container.DataItem,"stepnum")%>"><%#DataBinder.Eval(Container.DataItem,"decisionvalue") %></a></div>                        
                    <%--<input type="radio" class="answer" decisionid="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>" stepnum="<%#DataBinder.Eval(Container.DataItem,"stepnum")%>" name="q2" value="<%#DataBinder.Eval(Container.DataItem,"decisionid")%>"><%#DataBinder.Eval(Container.DataItem,"decisionvalue") %>--%>                    
                </ItemTemplate>
                </asp:Repeater>            
<%--                <div class="button" id="switchstepresults_print"><a href="#">Print</a></div>
                <div class="button" id="switchstepresults_email"><a href="#">Email</a></div>--%>
            </div>            
            <script type="text/javascript">
                //manage events
                function loadHandlers() {
                    var _printPage = function () {
                        $('#switchstepresults_Print').on('click', function () {
                            window.print();
                        });
                    };
                    var _closeSession = function () {
                        var closeSessionItem = '#ResultsContent_cphContentBar_ctl00_switchContinue';
                        $(closeSessionItem).on('click', function (e) {
                            e.preventDefault();
                            switchStep.closeSession({
                                callback: function () {
                                    window.location.href = $(closeSessionItem).attr('href');
                                }
                            });
                        });
                    };
                    _printPage();
                    _closeSession();
                }
                //display only previously selected providers by index selected
                function updateSelectedProviders() {
                    var switchStepSelections = JSON.parse(sessionStorage.getItem('switchStepDisplaySelections'));
                    for (index in switchStepSelections) {
                        if (switchStepSelections[index] == false) {
                            var selectedIndex = index;
                            var tableIndex = parseInt(selectedIndex) + 1;
                            $('#displayTable tbody').children(':nth-child(' + tableIndex + ')').hide();
                        }                       
                    }
                }
                $(document).ready(function () {
                    switchStep.bind({
                        event: 'plusminus',
                        element: '.plusminus.listclosed'
                    });
                    //selected fair price providers
                    $('#displayTable').append(switchStep.getDisplayTable());
                    loadHandlers();
                    updateSelectedProviders();
                });
            </script>      
        </asp:Panel>    
        <br><br><br>        
        <div><!-- prev/back nav -->
            <div style="float: right; text-align: right;">
            <asp:HyperLink ID="switchContinue" CssClass="readmore b" runat="server">Continue</asp:HyperLink>
            </div>
        </div>
        <div class="clearboth"></div>    
    </div>