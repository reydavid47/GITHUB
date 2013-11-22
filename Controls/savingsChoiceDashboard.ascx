<%@ Control Language="C#" AutoEventWireup="true" CodeFile="savingsChoiceDashboard.ascx.cs" Inherits="ClearCostWeb.Controls.SavingsChoice_Dashboard" %>
<style type="text/css">
.btn
{
    background: url("../Images/buttons/short.png") no-repeat scroll -10px -10px transparent !important;
    border: 1px solid #BBBBBB;
    border-radius: 3px;
    box-shadow: 0 0 2px #CCCCCC;
    line-height: 30px;
    margin: 10px !important;
    padding: 8px 20px 8px 30px !important;
	}		
.btn:hover
{
	background-position:-10px -71px !important;
	}
.completeIQ
{
	font-weight:bold;
	position:relative;
	top:-3px;
	}	
.readmore-savingschoice li:hover
{
	background-color:transparent;
	height:16px;
	text-decoration:underline;
	color:#662D91 !important;
	}
.readmore-savingschoice a:hover	
{
    color:#662D91 !important;	
    background-color:transparent;
	}
</style>
<div id="tabchoice" class="ui-tabs-panel ui-widget-content ui-corner-bottom">
    <div style="width: 840px;">
        <div style="float: left; width: 489px; padding-top: 10px;">
            <h1>Savings Choice</h1>
            <div id="restartSCIQWrapper" style="display:none">                
                <div class='overallDivider'>&nbsp;</div>
                <div class="completeIQ">
                    <asp:HyperLink ID="restarSCIQURL" class="btn" runat="server">Complete IQ</asp:HyperLink>
                </div>                                
            </div>
        </div><!-- end float left -->

        <div style="float: right; width: 350px;">

            <table border="0" cellspacing="0" cellpadding="0" style="width: 300px;">
            <tbody>
                <tr><th class="goal" style="display:none">Goal: <asp:Literal ID="userGoal" runat="server"></asp:Literal>%</th></tr>
                <tr>
                    <td>
                    <div class="yellowbar" style="width: 176px; position: relative;"><img width="300" height="22" src="../images/savings_bar.png"><b style="position:absolute; left: 151px; top: -1px;">62%</b><div style="position: absolute; left: 230px; top: 1px; border-left: 2px black solid; width: 1px; height: 18px; display:none;"></div></div>
                    <%--<p class="alignright"><a href="">Learn more</a></p>--%>
                    </td>
                </tr>
            </tbody>
            </table>

        </div>


        <div class="clearboth"></div>
    </div>

    </br>
    <h3 class="savingschoice"><asp:Literal ID="startText" runat="server"></asp:Literal></h3>
    <p>
    <asp:Repeater ID="repeaterQuickSuggestions" runat="server">
        <ItemTemplate>
            <%# loadQuickSuggestions(Container) %>
            <a style="font-weight: normal;" class="submitlink" href="<%# QuickSuggestionPath %>"><%# QuickSuggestionText %></a><br>
        </ItemTemplate>
    </asp:Repeater>
    <%--<a id="specials" rel="specials" class="submitlink triggeroverlay">Try MDLive, Direct Dermatology, or your local Onsite Clinic to earn a 25% Savings Choice bonus.</a>--%>
    </p>

    </br>
    <h3 class="savingschoice">Savings Choice scores for medical providers used recently by your family</h3>
    </br>

    <table id="tblCategoryFlow" border="0" cellspacing="0" cellpadding="8" style="width: 740px; margin-left: 35px;">
    <tbody>
        <tr category="imaging">
        <td width="75"><a class="pointer" rel="radiology" href="<%=ResolveUrl("~/SavingsChoice/radiologydetails.aspx") %>"><img width="60" border="0" height="56" src="../images/icon_imaging.png" alt="Radiology" class="choiceicon" /></a></td>
        <td width="385">
        <b><a class="pointer" rel="radiology" href="<%=ResolveUrl("~/SavingsChoice/radiologydetails.aspx") %>">Radiology</a></b><br>
        <div 
            class="cch_ui progress_main" 
            cch_type="progress_main" 
            cch_defaultvalue="<%= ratingRadiology %>%"
            ratingColor="<%=ratingColorRadiology %>"
            ></div>
        <%= visitsRadiology %> services<br>
        </td>
        <td class="valigntop"><ul class="readmore-savingschoice">
	        <li><a class="b" href="<%=ResolveUrl("~/SavingsChoice/radiologydetails.aspx") %>">Find Fair Price Options</a></li>
	        </ul>
	        </td>
        </tr>
        <tr category="lab">
            <td width="75"><a class="pointer" rel="lab" href="<%=ResolveUrl("~/SavingsChoice/labdetails.aspx") %>"><img width="60" border="0" height="56" src="../images/icon_lab.png" alt="Lab" class="choiceicon"></a></td>
            <td width="385">
            <b><a class="pointer" rel="lab" href="<%=ResolveUrl("~/SavingsChoice/labdetails.aspx") %>">Lab</a></b><br>

            <div 
            class="cch_ui progress_main" 
            cch_type="progress_main" 
            cch_defaultvalue="<%= ratingLab %>%"             
            ></div>
            
            </td>
            <td class="valigntop"><ul class="readmore-savingschoice">
	            <li><a class="b" href="<%=ResolveUrl("~/SavingsChoice/labdetails.aspx") %>">Find Fair Price Options</a></li>
	            </ul>
	            </td>
        </tr>
        <tr category="rx">
            <td width="75"><a class="pointer" rel="medical" href="<%=ResolveUrl("~/SavingsChoice/prescriptiondetails.aspx") %>"><img width="60" border="0" height="56" class="choiceicon" alt="Medical visits and procedures" src="../images/icon_visits.png"></a></td>
            <td width="385">
            <b><a class="pointer" rel="medical" href="<%=ResolveUrl("~/SavingsChoice/prescriptiondetails.aspx") %>">Prescription Drugs</a></b><br>
            <div 
                class="cch_ui progress_main" 
                cch_type="progress_main" 
                cch_defaultvalue="<%= ratingPrescription %>%"             
                ></div>
            <%= visitsPrescription %> services<br>
            </td>
            <td class="valigntop"><ul class="readmore-savingschoice">
	            <li><a class="b" href="<%=ResolveUrl("~/SavingsChoice/prescriptiondetails.aspx") %>">Find Fair Price Options</a></li>
	            </ul>
	            </td>
        </tr>
        <tr category="mvp">
            <td width="75"><a class="pointer" rel="medical" href="<%=ResolveUrl("~/SavingsChoice/medicaldetails.aspx") %>"><img width="60" border="0" height="56" class="choiceicon" alt="Medical visits and procedures" src="../images/icon_visits.png"></a></td>
            <td width="385">
            <b><a class="pointer" rel="medical" href="<%=ResolveUrl("~/SavingsChoice/medicaldetails.aspx") %>">Medical visits and procedures</a></b><br>
            <div 
                class="cch_ui progress_main" 
                cch_type="progress_main" 
                cch_defaultvalue="<%= ratingMedical %>%"             
                ></div>
            <%= visitsMedical %> services<br>
            </td>
            <td class="valigntop"><ul class="readmore-savingschoice">
	            <li><a class="b" href="<%=ResolveUrl("~/SavingsChoice/medicaldetails.aspx") %>">Find Fair Price Options</a></li>
	            </ul>
	        </td>
        </tr>
    </tbody>
    </table>

    <br>
    <p class="center">
    <b>Questions? Our Health Shoppers are here to help! Call us at (800) 390-6855.</b>
    </p>

    <p style="font-size: 10px;">
    <br>
    * Measurement period: <asp:Literal ID="startMonth" runat="server"></asp:Literal>, <asp:Literal ID="startYear" runat="server"></asp:Literal>-<asp:Literal ID="endMonth" runat="server"></asp:Literal>, <asp:Literal ID="endYear" runat="server"></asp:Literal>
    </p>


</div>
<script src="<%=ResolveUrl("/Scripts/cch_global.js") %>" type="text/javascript"></script>        
<script src="<%=ResolveUrl("/Scripts/cch_savingsChoiceUI.js") %>" type="text/javascript"></script>        
<script type="text/javascript">
    /* -- holds available categoires : *** only availabe during user control, js object re-initialized during page load *** -- */
    _cchGlobal.pageData["availableCategories"] = '<%= availableCategories %>';
    
    /* -- updates ui to match avilabile categories -- */
    $('table#tblCategoryFlow').find('tr').each(function () {    
        var ac = _cchGlobal.pageData.availableCategories.toLowerCase();
        var cc = $(this).attr('category').toLowerCase();
        if (ac.indexOf(cc) < 0) {
            $(this).remove();
        }
    });
    
    /* -- builds progress for all categories -- */    
    _cchGlobal.actions.buildUI({ uiType: 'progress_main' });              
</script>