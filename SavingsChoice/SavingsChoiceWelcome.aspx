<%@ Page Title="" Language="C#" 
    MasterPageFile="~/SavingsChoice/SavingsChoice.master" 
    AutoEventWireup="true" CodeFile="SavingsChoiceWelcome.aspx.cs" Inherits="ClearCostWeb.SavingsChoice.SavingsChoiceWelcome"
    EnableEventValidation="false" ViewStateMode="Enabled" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" Runat="Server">
    <link href="../Styles/overlay.css" rel="Stylesheet" />
    <script src="../Scripts/jquery-1.8.3.min.js" type="text/javascript" language="javascript"></script>
    <script src="../Scripts/cch_global.js" type="text/javascript"></script>        
    <script src="../Scripts/cch_savingsChoiceUI.js" type="text/javascript"></script>   
    <script type="text/javascript" language="javascript">
        function ToggleOverlay(event) {
            if (event == null || event.target.id == "overlayBackground" || $("#overlayBackground").is(":visible") == false) {
                $("#overlayBackground").fadeToggle();
            }
        }
        function ToggleAvatarOverlay(event) {
            if (event == null || event.target.id == "avatarOverlayBackground" || $("#avatarOverlayBackground").is(":visible") == false) {
                $("#avatarOverlayBackground").fadeToggle();
            }
        }
    </script>
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="body" Runat="Server">
    <h3>You're getting there!</h3>
    <p>
        Rate your experience with your medical providers. We'll use this information to help identify where you can save money on health care.
    </p>
    <p>
        First, select an icon for each of your family members.
    </p>
    <div class="family">
        <table>
            <tr>
                <% for (int i = 0; i < MemberCount; i++)
                   { %>
                    <td class="member" onclick='hfFamElem.value = <%= MemberCCHIDs[i] %>; ToggleAvatarOverlay();'>
                        <img src='<%= GetAvatarForMember(i) %>' width="100" height="100" alt='<%= GetMemberName(i) %>' />
                        <br />
                        <%= GetMemberName(i) %>
                    </td>
                    <% if((i > 5 && i <= 10) || (i > 10 && i <= 15)) { %> </tr><tr> <% } %>
                <% } %>
            </tr>
        </table>
        <asp:HiddenField ID="hfChosenMember" runat="server" Value="" />
        <script type="text/javascript" language="javascript">var hfFamElem = document.getElementById('<%= hfChosenMember.ClientID %>');</script>
    </div>
    <div class="next">
        <asp:HyperLink ID="startSCIQ" runat="server" NavigateUrl="SavingsChoicePharmacies.aspx" Text="Start Now" />
        <script type="text/javascript">
            $(document).ready(function () {
                $('#body_startSCIQ').on('click', function (e) {
                    var _t = this;
                    e.preventDefault();
                    auditSCIQ({
                        method: 'started',
                        cchid: '<%= PrimaryCCHID %>',
                        sid: '<%= sessionID %>',
                        callback: function () {
                            window.location.href = $(_t).attr('href');
                        }
                    });
                });
            });
        </script>
    </div>
    <div class="more">
        <a onclick="ToggleOverlay();">Learn More</a>
    </div>
    <div id="avatarOverlayBackground" class="ui-helper-hidden" style="display: none;">
        <div id="avatarOverlay">
            <!-- Top Rounded Rectangle -->
			<div id="avatarOverlayTop">
                <img src="../Images/buttons/close.png" alt="X" width="35px" height="35px" onclick="ToggleAvatarOverlay();" />
			</div>
			
			<!-- End Main Content -->
			<div id="avatarOverlayContent">
                <div style="overflow-y: scroll;max-height: 300px;">
			        <asp:Repeater ID="rptAvailableAvatars" runat="server">
                        <ItemTemplate>
                            <asp:ImageButton ID="ibAvatar" runat="server" Width="100" Height="100" CssClass="avatarImage"
                                ImageUrl='<%# ResolveUrl("~/Images/Avatars/" + Eval("AvatarFileName").ToString()) %>' 
                                CommandArgument='<%# Eval("AvatarID") %>' 
                                OnClick="UpdateMemberAvatar"
                                onmouseover="this.src = this.src.replace('.png','_Glow.png');"
                                onmouseout="this.src = this.src.replace('_Glow.png','.png');" />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
			</div>
			<!-- End Main Content -->

			<!-- Bottom Rounded Rectangle -->
			<div id="avatarOverlayBottom">
			</div>
        </div>	
	</div>
    <div id="overlayBackground" class="ui-helper-hidden" style="display: none;">
        <div id="overlay">
            <!-- Top Rounded Rectangle -->
			<div id="overlayTop">
                <img src="../Images/buttons/close.png" alt="X" width="35px" height="35px" onclick="ToggleOverlay();" />
			</div>
			
			<!-- End Main Content -->
			<div id="overlayContent">
			<div class="questionContent">
				<!-- Start Question -->
				<div class="question">
					<div class="overlayQ">
						What is ClearCost?
					</div>
					<div class="overlayA">
						ClearCost Health is a free service that Caesars offers which allows you to shop for in-network health care providers based on cost, quality and convenience. You can search for services like doctors’ office visits, lab tests, radiology procedures (like x-rays and MRIs), prescription drugs and basic services (like colonoscopies and sleep studies) to find the best priced provider so you can save on your out-of-pocket health care expenses.
					</div>
				</div>
				<!-- End Question -->
				
				<!-- Start Question -->
				<div class="question">	
					<div class="overlayQ">
						How do I avoid the penalty for 2014?
					</div>
					<div class="overlayA">
						You just need to register for ClearCost, which you have successfully done. If you have a spouse or domestic partner that is on your Caesars health plan, they also need to register for ClearCost. 
					</div>
				</div>
				<!-- End Question -->
				
				
				<!-- Start Question -->
				<div class="question">
					<div class="overlayQ">
						What services can I search for with ClearCost?
					</div>
					<div class="overlayA">
						ClearCost provides cost and quality information for basic, non-urgent health care services. These include doctors’ office visits, lab tests, radiology procedures (like x-rays and MRIs), prescription drugs and basic procedures (like colonoscopies and sleep studies). Visit the “Past Care” tab on the website to see how you could have saved money on the medical services that you received in the last year. 	
					</div>
				</div>
				<!-- End Question -->
				
				
				<!-- Start Question -->
				<div class="question">	
					<div class="overlayQ">
						I already have health insurance, why would I need ClearCost?
					</div>
					<div class="overlayA">
						ClearCost allows you to search for health care providers within your health care network. Few people realize that there can be substantial cost differences even among in-network health care providers. As a result, many individuals overpay for basic health care services. ClearCost helps you identify areas in your medical spending where you can save money. For instance, you might be able to save simply by switching your pharmacy! Use ClearCost to learn how you can save. 
					</div>
				</div>
				<!-- End Question -->

                <!-- Start Question -->
				<div class="question">	
					<div class="overlayQ">
						Who can use ClearCost?
					</div>
					<div class="overlayA">
						Caesars offers ClearCost for free to all over-18 employees and dependents on a Caesars medical plan. 
					</div>
				</div>
				<!-- End Question -->

                <!-- Start Question -->
				<div class="question">	
					<div class="overlayQ">
						How can I contact ClearCost?
					</div>
					<div class="overlayA">
						You can call ClearCost Health at 800-318-4168, M-F, 6am – 5pm Pacific or email us at caesars@clearcosthealth.com.
					</div>
				</div>
				<!-- End Question -->
			</div>
			</div>
			<!-- End Main Content -->

			<!-- Bottom Rounded Rectangle -->
			<div id="overlayBottom">
			</div>
        </div>	
	</div>
</asp:Content>

