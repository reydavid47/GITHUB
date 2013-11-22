<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SavingsChoiceIQAddProvider.ascx.cs" Inherits="ClearCostWeb.SavingsChoice.Controls_SavingsChoiceIQAddProvider" %>
<%@ Register TagName="starRating" TagPrefix="cchUIControl" Src="~/Controls/StarRating.ascx" %>

<style type="text/css">
    #body_addProviderControl
    {
    	display:none;
    	}
</style>

<div class="ui-helper-hidden" id="newProviderInput" style="display: block;">
	<div class="new"></div>
	<div class="satisfaction inputHeaders">
		<h2 class="avatar">Patient</h2>
		<h2 class="provider">Provider Information</h2>
		<h2 class="slider">Satisfaction</h2>
	</div>
	<div class="satisfaction row added">
		<div class="avatar pointer">
            <img width="60" height="60" alt="outline" src="<%=ResolveUrl("~/images/avatars/outline.png") %>"> 
		</div>
        <div class="provider"> 
            <div id="stateWrapper" style="opacity:0">
                <p>Please select the State </p>
                <select id="states"></select>
            </div>
            <div id="cityWrapper" style="display:none; opacity:0;">
                <p>Please select the City</p>
                <select id="cities"></select>
            </div>
            <div id="providerWrapper" style="display:none; opacity:0">
                <div class="ui-widget">
                    <label for="providers_autocomplete">Please select Provider</label></br>
                    <input id="providers_autocomplete" />
                </div>                
            </div>
			<div class="done">
                <a id="addProviderBtn" href="#" class="btn">Done</a>
			</div>
		</div>        
        <div class="userRatings">
                <cchUIControl:starRating ID="StarRating1" runat="server" />
        </div>
        <div class="userAddedProvider">
            <div class="ui-state-default ui-corner-all delete">
                <span class="ui-icon ui-icon-closethick"></span>
                <label>delete</label>
            </div>
            <div class="ui-state-default ui-corner-all review">
                <span class="ui-icon ui-icon-comment"></span>
                <label>write a review</label>
                <textarea></textarea>
            </div>        
        </div>
	</div>
</div>

<div id="addProviderAvatar" title="Select your Avatar" style="display:none">
    Select one or more family members that use this provider.</br>
    <asp:Repeater ID="avatarListRepeater" runat="server">
        <ItemTemplate>
            <div>
                <img
                    class="newAvatars" 
                    title="<%#DataBinder.Eval(Container.DataItem, "FirstName") %>" 
                    cid="<%#DataBinder.Eval(Container.DataItem, "cchid") %>" 
                    aid="<%#DataBinder.Eval(Container.DataItem, "avatarid") %>" 
                    src="<%#ResolveUrl("~/Images/Avatars/"+DataBinder.Eval(Container.DataItem, "AvatarFileName"))%>" />
                </br>
                <input type="checkbox" />
            </div>
        </ItemTemplate>
    </asp:Repeater>
    </br>
    <a id="addAvatars" href="#">
	        <img width="20" height="20" src="../images/buttons/expand.png" alt="expand" class="expand">
	        Add family member(s)
    </a>
</div>        

<script type="text/javascript">
    $(document).ready(function () {
        buildStates();    
    });
</script>