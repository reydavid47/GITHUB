<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="results_care_share.aspx.cs" Inherits="ClearCostWeb.SearchInfo.results_care_share" %>

<asp:Content ID="results_care_share_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <h1>
        Share Search Results</h1>
    <h2>
        It's easy to share these search results with your doctor.<sup>
            <div class="learnmore">
                <a title="Learn more">
                    <img src="../Images/icon_question_mark.png" width="12" height="13" border="0" alt="Learn More" /></a>
                <div class="moreinfo">
                    <img src="../Images/icon_x_sm.png" alt="" width="14" height="14" border="0" align="right"
                        style="cursor: pointer;" />
                    <p>
                        <b class="upper">Sharing These Results</b><br />
                        Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec
                        arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante.
                        Etiam aliquet massa et lorem.</p>
                </div>
                <!-- end moreinfo -->
            </div>
            <!-- end learnmore -->
        </sup>
    </h2>
    <table cellspacing="0" cellpadding="6" border="0">
        <tr>
            <td valign="top">
                <img src="../Images/1.gif" alt="1" width="18" height="18" border="0">
            </td>
            <td valign="top">
                Enter your doctor's name and email or fax number.
            </td>
        </tr>
        <tr>
            <td valign="top">
                <img src="../Images/2.gif" alt="2" width="18" height="18" border="0">
            </td>
            <td valign="top">
                Click "Share my search results."
            </td>
        </tr>
        <tr>
            <td valign="top">
                <img src="../Images/3.gif" alt="3" width="18" height="18" border="0">
            </td>
            <td valign="top">
                We will send a copy of the results to your provider, along with information about
                how ClearCost Health works.
            </td>
        </tr>
    </table>
    <hr />
    <h3>
        Share Results with your doctor</h3>
    <table cellspacing="0" cellpadding="4" border="0">
        <tr>
            <td>
                Physician's Name:
            </td>
            <td>
                <input type="text" class="boxed" name="doctor" value="" />
            </td>
        </tr>
        <tr>
            <td>
                Physician's Email:
            </td>
            <td>
                <input type="text" class="boxed" name="email" value="" />
            </td>
        </tr>
        <tr>
            <td>
                <b>OR</b>
            </td>
        </tr>
        <tr>
            <td>
                Fax
            </td>
            <td>
                <input type="text" class="boxed" name="fax" value="" />
            </td>
        </tr>
        <tr>
            <td>
                Save information for this doctor:
            </td>
            <td>
                <input type="checkbox" />
                Yes
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <a class="submitlink" onclick="document.caresearch.submit();return false;">Share my
                    search results</a>
            </td>
        </tr>
    </table>
</asp:Content>

