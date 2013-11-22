<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SavingsChoiceIQAddProviderNoAddress.ascx.cs" Inherits="ClearCostWeb.SavingsChoice.Controls_SavingsChoiceIQAddProviderNoAddress" %>

<div class="noAddressProviderDetails" style="display: block;">
    <div class="noAddressProvider">         
        <div class="stateWrapper" style="opacity:0">
            <div class="displayText">Please specify location.</div>
            <%--<p>Please select the State </p>--%>
            <select class="states"></select>
        </div>
        <div class="cityWrapper" style="display:none; opacity:0;">
            <p>Please select the City</p>
            <select class="cities"></select>
        </div>
        <div class="providerWrapper" style="display:none; opacity:0">
            <p>Please select Provider</p>
            <select class="providers"></select>
        </div>
	</div>        
</div>      