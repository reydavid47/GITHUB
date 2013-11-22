<%@ Page Title="" Language="C#" MasterPageFile="~/AdminPortal/MasterAdmin.master" AutoEventWireup="true" CodeFile="EmployerMetrics.aspx.cs" Inherits="AdminPortal_EmployerMetrics" %>


<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" Runat="Server">
    <%-- TOP BAR  --%>
    <div class="toolbar">
        <div class="toolbar-button" id="heading">
            <label> ClearCost Health Metrics Interface </label>   
        </div>

        <div class="toolbar-button">
            <label> Usage Metrics </label>
        </div>

        <div class="toolbar-button">
            <label> Other Metrics </label>
        </div>   
    </div>
    
    
    <div>

    <%-- LEFT TOOLBAR  --%>

    <div id="metrics-options"> 
        <form id="metrics-options-form" action="">
            <div id="submit-options">
                <div class="query-option"><input type="submit" name="submit" class="button" id="submit-metrics" value="Send" /></div>
                
            </div>

            <div id="employer-select">
                <div class="query-option">
                <select name="employer">
                        <option value="starbucks">Starbucks</option>
                </select>
                </div>
            </div>

            <div id="date-range-lower">
                <div class="query-option"><input type="text" class="date-select" id="from" name="lower-date"/></div>
            </div>
            <div id="date-range-upper">
                <div class="query-option"><input type="text" class="date-select" id="to" name="upper-date"/></div>
            </div>

            <div id="query-options" class="query-options">
                <div class="query-option panel"> General Criteria </div>
                <div>
                    <label> <input type="checkbox" name="registered" value="1"/> Registered </label>
                    <label> <input type="checkbox" name="registered" value="0"/> Non-registered </label>
                    <label> <input type="checkbox" name="visited" value="1"/> Visited the site </label>
                    <label> <input type="checkbox" name="visited" value="0"/> Have not visited the site </label>
                    <label> <input type="checkbox" name="savings-alerts" value="1"/> Accept Savings Alerts </label>
                    <label> <input type="checkbox" name="savings-alerts" value="0"/> Did not accept Saving Alerts </label>
                    <label> <input type="checkbox" name="health-shopper" value="1"/> Accept Health Shopper </label>
                    <label> <input type="checkbox" name="health-shopper" value="0"/> Did not accept CC Health Shopper </label>
                </div>
            </div>

            <div id="employer-query-option" class="query-options">
                <div class="query-option panel"> Employer Specific Criteria </div>
                <div>
                    <label> <input type="checkbox" name="retail-hourly" value="1"/> Retail hourly </label>
                    <label> <input type="checkbox" name="retail-salary" value="1"/> Retail Salary </label>
                    <label> <input type="checkbox" name="roasting-plant" value="1"/> Roasting Plant </label>
                    <label> <input type="checkbox" name="regional" value="1"/> Regional/Support HQ </label>
                </div>
            </div>
            

        </form>
    </div>
 



    <%-- CONTENT (RIGHT)  --%>

    <div id="metrics-results">
        <div id="results-header">
            ClearCost Health Metrics
        </div>
        <div class="content-divider"></div>
        <div id="results-content">
            <p>First name: <strong data-bind="text: firstName"></strong></p>
            <button data-bind="text: firstName, click: $root.getData"></button>
            <div id="table-div"></div>
        
        <div id="table-div">
            <table>
                <thead class="table-header">
                <tr>
                    <td>Metric</td>
                    <td>Value</td>       
                </tr> 
                </thead>
                <tbody data-bind="foreach: data">
                <tr class="table-row">
                    <td data-bind="text: name"></td>
                    <td data-bind="text: value"></td>
                </tr>
                </tbody>
            </table>
        </div>
        </div>
    </div>


    </div>
</asp:Content>

