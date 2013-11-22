<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="results_facility.aspx.cs" Inherits="ClearCostWeb.SearchInfo.results_facility" %>

<asp:Content ID="results_facility_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <h1>
        Search Results</h1>
    <p>
        You searched for "Pediatrician - initial office visit" from "Rockman Pediatrics".
        Which "Rockman Pediatrics" did you mean?
    </p>
    <p>
        <a href="facility_detail.aspx" class="readmore">Rockman Pediatrics - 1242 L Road, Naperville,
            IL</a><br />
        <a href="facility_detail.aspx" class="readmore">Rockman Pediatrics - 808 California,
            Rockford, IL</a><br />
        <a href="facility_detail.aspx" class="readmore">Rockman Pediatrics - 232 Devan Avenue,
            Chicago, IL</a>
    </p>
    <p>
        <b>OR</b>
    </p>
    <p>
        <a href="search.aspx" class="back">Edit search</a>
    </p>
</asp:Content>

