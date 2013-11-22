<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="results_doctor.aspx.cs" Inherits="ClearCostWeb.SearchInfo.results_doctor" %>

<asp:Content ID="results_doctor_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <h1>
        Search Results</h1>
    <p>
        You searched for "Pediatrician - initial office visit" from "Dr. Jan Smith". Which
        "Dr. Jan Smith" did you mean?
    </p>
    <p>
        <a href="doctor_detail.aspx" class="readmore">Dr. Jan Smith (pediatrics) - Pediatric
            Teacher Foundation, Naperville, IL</a><br />
        <a href="doctor_detail.aspx" class="readmore">Dr. Jan Smith (pediatrics) - Chicago Public
            Physicians, Oak Park, IL</a><br />
        <a href="doctor_detail.aspx" class="readmore">Dr. Jan Smith (pediatrics) - Imaging Associates,
            Oak Park, IL</a>
    </p>
    <p>
        <b>OR</b>
    </p>
    <p>
        <a href="search.aspx" class="back">Edit search</a>
    </p>
</asp:Content>

