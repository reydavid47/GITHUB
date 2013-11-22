<%@ Page Title="" Language="C#" MasterPageFile="Starbucks.master" AutoEventWireup="true"
    CodeFile="AboutUs.aspx.cs" Inherits="ClearCostWeb.Starbucks.AboutUs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h1>About Us</h1>
    <br />
    <% if (Membership.GetUser() != null && Membership.GetUser().IsOnline)
       { %><a class="back" href="javascript:history.back();">Return</a><% } %>
    <p>
    <b>ClearCost Health</b> allows you to search for doctors, lab test, imaging studies, procedures, and presciption
    drugs based on cost, quality, and convenience.  With our service, you'll be able to make more informed
    decisions about your health care and save money.
    </p>
    <br />
    <p>
        Did you know there are big differences in price for routine medical services, depending on which "in-network" 
        provider you go to?  Some providers (such as large hospitals and big groups of doctors) can demand
        higher prices:
        <br />
        <br />
        <img src="../Images/chart_knee.png" alt="Cost of a Knee MRI" style="margin-left:70px;margin-right:35px;" />
        <img src="../Images/chart_office.png" alt="Cost of an Office Visit" style="margin-right:70px;margin-left:35px;" />
    </p>
    <br />
    <p>
        Studies show there is little correlation between cost and quality in healthcare*, so higher cost does not mean
        better quality.
    </p>
    <br />
    <p>
        With access to this information, you could save a lot of money on your health care by shopping for medical
        services and prescription drugs, without sacrificing quality.
    </p>
    <br />
    <br />
    <p>
        <i class="smaller">* "Examination of Health Care Cost Trends and Cost Drivers" Report for Annual Public Hearing issues by Massachusetts Attorney General Marth Coakley, June 22, 2011</i>
    </p>
</asp:Content>
