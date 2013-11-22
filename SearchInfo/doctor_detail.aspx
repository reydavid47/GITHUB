<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="doctor_detail.aspx.cs" Inherits="ClearCostWeb.SearchInfo.doctor_detail" %>

<asp:Content ID="doctor_detail_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <h1>
        Search by Specialty</h1>
    <h3>
        Select the specialty of the doctor you are looking for</h3>
    <table cellspacing="0" cellpadding="5" border="0" class="nobr" width="100%">
        <tr>
            <td>
                <input type="radio" name="specialty">
                Allergy and Immunology
            </td>
            <td>
                <input type="radio" name="specialty">
                Gastroenterology (Stomach/Digestion)
            </td>
            <td>
                <input type="radio" name="specialty">
                Obstetrics/Gynecology (Women's Health)
            </td>
        </tr>
        <tr>
            <td>
                <input type="radio" name="specialty">
                Cardiology (Heart)
            </td>
            <td>
                <input type="radio" name="specialty">
                Infectious Disease
            </td>
            <td>
                <input type="radio" name="specialty">
                Ophthalmology (Eyes)
            </td>
        </tr>
        <tr>
            <td>
                <input type="radio" name="specialty">
                Dermatology (Skin)
            </td>
            <td>
                <input type="radio" name="specialty">
                Internal Medicine
            </td>
            <td>
                <input type="radio" name="specialty">
                Pulmonary Medicine (Lungs)
            </td>
        </tr>
        <tr>
            <td>
                <input type="radio" name="specialty">
                Family Medicine
            </td>
            <td>
                <input type="radio" name="specialty">
                Neurology (Brain)
            </td>
            <td>
                <input type="radio" name="specialty">
                Sleep Medicine
            </td>
        </tr>
    </table>
    <hr />
    <input type="radio" name="specialty" value="other">
    <select>
        <option>Select other specialty</option>
    </select>
    <hr />
    <a class="submitlink">Submit</a>
</asp:Content>

