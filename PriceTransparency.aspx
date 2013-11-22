<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="PriceTransparency.aspx.cs" Inherits="ClearCostWeb.Public.PriceTransparency" %>

<%@ Register Src="~/controls/unavplaceholder.ascx" TagPrefix="uc" TagName="unavPlaceHolder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="clearboth"></div>
    <h1>
        Price Transparency</h1>
    <div class='intcontent'>
        <p>
            The lack of price transparency allows surprisingly large price differentials to
            exist between in-network providers.
        </p>
        <p>
            For years, benefits professionals have been told that the key to cost containment
            was simply to encourage in-network utilization. Yet, even when in-network utilization
            rates exceed 90%, plan and patient out-of-pocket costs continue to rise unabated.
        </p>
        <p>
            While encouraging in-network utilization is generally good practice, it does not
            address a very real problem: The lack of price transparency allows price differentials
            of 2x, 3x, even up to 55x to exist between in-network providers for the same service,
            in the same geography and with the same objective quality ratings.
        </p>
        <p>
            Higher deductibles and co-insurance mean that patients have more skin in the game.
            Lack of price transparency means that they can't save their own skin.
        </p>
        <p>
            The ClearCost service was developed to address this problem.
        </p>
    </div>
    <div class='intzone'>
        <br />
        <table cellspacing="1" cellpadding="0" border="0" class="transparencygrid">
            <tr>
                <th class="desc">
                    Description
                </th>
                <th>
                    Highest Cost<br />
                    In-Network
                </th>
                <th>
                    Lowest Cost<br />
                    In-Network
                </th>
                <th>
                    &nbsp;Multiple&nbsp;
                </th>
            </tr>
            <tr class="odd">
                <td class="desc">
                    Urinalysis
                </td>
                <td class="fig">
                    $165
                </td>
                <td class="fig">
                    $3
                </td>
                <td>
                    55x
                </td>
            </tr>
            <tr class="even">
                <td class="desc">
                    Cholesterol tests
                </td>
                <td class="fig">
                    $232
                </td>
                <td class="fig">
                    $13
                </td>
                <td>
                    18x
                </td>
            </tr>
            <tr class="odd">
                <td class="desc">
                    Cardiovascular<br />
                    stress test
                </td>
                <td class="fig">
                    $1,164
                </td>
                <td class="fig">
                    $89
                </td>
                <td>
                    13x
                </td>
            </tr>
            <tr class="even">
                <td class="desc">
                    CT scan, pelvis
                </td>
                <td class="fig">
                    $3,071
                </td>
                <td class="fig">
                    $348
                </td>
                <td>
                    8.8x
                </td>
            </tr>
            <tr class="odd">
                <td class="desc">
                    Office visit,<br />
                    new patient
                </td>
                <td class="fig">
                    $186
                </td>
                <td class="fig">
                    $36
                </td>
                <td>
                    5.2x
                </td>
            </tr>
            <tr class="even">
                <td class="desc">
                    Removal of skin tags
                </td>
                <td class="fig">
                    $207
                </td>
                <td class="fig">
                    $80
                </td>
                <td>
                    2.6x
                </td>
            </tr>
        </table>
    </div>
    <div class="clearboth"></div>
</asp:Content>
