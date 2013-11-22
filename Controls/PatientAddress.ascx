<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PatientAddress.ascx.cs" Inherits="ClearCostWeb.Controls.PatientAddress" %>

<asp:HiddenField ID="PATIENTLATITUDE" runat="server" ClientIDMode="Static" Value="" />
<asp:HiddenField ID="PATIENTLONGITUDE" runat="server" ClientIDMode="Static" Value="" />
<asp:HiddenField ID="LOCATIONHASH" runat="server" ClientIDMode="Static" Value="" />
<div id="locationbar">
    <div id="location">
        <span class="callout floatleft">Patient Location:&nbsp;&nbsp;</span>
        <div class="floatleft">
            <b>
                <asp:Label ID="lblPatientAddress" runat="server" Text="" CssClass="patientAddress" />                
            </b>
        </div>
        <div class="floatleft">
            &nbsp;&nbsp; [ <a class="changelocation" id="aChange">Change Location</a> ]
        </div>
    </div>
    <div id="insurer">
        <span class="callout">Health Plan:</span>
        <asp:Label ID="lblInsurerName" runat="server" Text="" Font-Bold="true" />
        <br />
        <span class="callout">Rx Provider:</span>
        <asp:Label ID="lblRxProviderName" runat="server" Text="" Font-Bold="true" />
    </div>
    <div class="locationform">
        <hr />
        <h3>
            Alternate Patient Location
        </h3>
        <a id="cancellocation" class="xclose">X</a>
        <asp:Panel ID="chgloc" ClientIDMode="Static" runat="server">
            <table cellspacing="0" cellpadding="5" border="0" width="100%">
                <tbody>
                    <tr>
                        <td valign="top" align="left">
                            <asp:RadioButton runat="server" ID="rbUseSaved" Text="" Checked="true" GroupName="rbAddressGroup" CssClass="rbUseSaved" ClientIDMode="Static" />
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSavedAddresses" runat="server" Width="347px" DataSourceID="dsSavedAddresses" ClientIDMode="Static"
                                DataTextField="DisplayText" DataValueField="AddressID" OnDataBound="ddlSavedAddresses_DataBound" />
                            <asp:SqlDataSource ID="dsSavedAddresses" runat="server" SelectCommand="GetSavedAddresses" SelectCommandType="StoredProcedure">
                                <SelectParameters>
                                    <asp:SessionParameter Name="CCHID" SessionField="CCHID" Type="Int32" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="right">
                        </td>
                        <td>
                            <b>OR</b>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="left">
                            <asp:RadioButton runat="server" ID="rbUseNew" Text="" Checked="false" GroupName="rbAddressGroup" ClientIDMode="Static" />
                        </td>
                        <td>
                            <table id="tbFields" cellspacing="1" cellpadding="0" border="0">
                                <tbody>
                                    <tr>
                                        <td colspan="3">
                                            <div class="addressField floatleft">
                                                <asp:TextBox ID="txtChgAddress" runat="server" Text="Enter street address" ValidationGroup="chgLoc" CssClass="addressField" ClientIDMode="Static" />
                                                <asp:CustomValidator ID="AddressRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="txtChgAddress" ForeColor="Red" Display="Dynamic"
                                                    SetFocusOnError="true" ClientValidationFunction="PatientAddress.CheckAddress">
                                                    <br />Address is Required    
                                                </asp:CustomValidator>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="cityField floatleft">
                                                <asp:TextBox ID="txtChgCity" runat="server" Text="Enter city" CssClass="cityField" ValidationGroup="chgLoc" ClientIDMode="Static" />
                                            </div>
                                        </td>
                                        <td>
                                            <div class="stateField floatleft">
                                                <asp:DropDownList ID="ddlState" runat="server" DataSourceID="dsStates" DataTextField="State"
                                                    DataValueField="State" OnDataBound="ddlState_DataBound" CssClass="sateField" ValidationGroup="chgLoc" ClientIDMode="Static" />
                                            </div>
                                        </td>
                                        <td>
                                            <div class="zipField floatleft">
                                                <asp:TextBox ID="txtChgZipCode" runat="server" Text="Zip Code" CssClass="zipField" ValidationGroup="chgLoc" ClientIDMode="Static" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <asp:CustomValidator ID="CityRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="txtChgCity" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                                ClientValidationFunction="PatientAddress.CheckCity">
                                                City is Required    
                                            </asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <%--<asp:RequiredFieldValidator ID="StateRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="ddlState" ForeColor="Red" Display="Dynamic" InitialValue="State">
                                                State is Required
                                            </asp:RequiredFieldValidator>--%>
                                            <asp:CustomValidator ID="StateRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="ddlState" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                                ClientValidationFunction="PatientAddress.CheckState">
                                                State is Required
                                            </asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <%--<asp:RequiredFieldValidator ID="ZipRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="txtChgZipCode" ForeColor="Red" Display="Dynamic" InitialValue="Zip Code" SetFocusOnError="true">
                                                Zip Code is Required
                                            </asp:RequiredFieldValidator>--%>
                                            <asp:CustomValidator ID="ZipRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="txtChgZipCode" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                                ClientValidationFunction="PatientAddress.CheckZip">
                                                Zip Code is Required
                                            </asp:CustomValidator>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="clearboth" />
                            <asp:SqlDataSource ID="dsStates" runat="server" ConnectionString="<%$ ConnectionStrings:CCH_FrontEnd %>"
                                ProviderName="<%$ ConnectionStrings:CCH_FrontEnd.ProviderName %>" SelectCommand="Select * from states" />
                            <asp:CheckBox ID="cbSaveAddress" runat="server" Text="Store address for future searches" Checked="false" />
                            <div class="clearboth" />
                        </td>
                    </tr>
                </tbody>
            </table>
            <a id="lbtnSaveButton" class="submitlink">Save</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id="lblCancel" class="submitlink">Cancel</a>
        </asp:Panel>
    </div>
</div>

<%--<script src="../Scripts/PatientAddress.js" type="text/javascript"></script>
<div id="locationbar" style="height:44px;">
    <div id="location">
        <span class="callout floatleft">Patient Location:&nbsp;&nbsp;</span>
        <div class="floatleft">
            <b>
                <asp:Label ID="lblPatientAddress" runat="server" Text="" CssClass="patientAddress" />
            </b>
        </div>
        <div class="floatleft">
            &nbsp;&nbsp; [ <a class="changelocation" id="aChange">Change Location</a> ]
        </div>
    </div>
    <div id="insurer">
        <span class="callout">Health Plan:</span>
        <asp:Label ID="lblInsurerName" runat="server" Text="" Font-Bold="True" style="cursor:default;"/><br />
        <span class="callout">Rx Provider:</span>
        <asp:Label ID="lblRxProviderName" runat="server" Text="" Font-Bold="True" />
    </div>
    <div class="clearboth" />
    <div class="locationform">
        <hr />
        <h3>
            Alternate Patient Location
        </h3>
        <a id="cancellocation" class="xclose">X</a>
        <asp:Panel ID="chgloc" ClientIDMode="Static" runat="server">
            <table cellspacing="0" cellpadding="5" border="0" width="100%">
                <tr>
                    <td valign="top" align="left">
                        <asp:RadioButton runat="server" ID="rbUseSaved" Text="" Checked="true" GroupName="rbAddressGroup" CssClass="rbUseSaved" ClientIDMode="Static" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlSavedAddresses" runat="server" Width="347px" DataSourceID="dsSavedAddresses" ClientIDMode="Static"
                            DataTextField="DisplayText" DataValueField="AddressID" OnDataBound="ddlSavedAddresses_DataBound">
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="dsSavedAddresses" runat="server" SelectCommand="GetSavedAddresses"
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter Name="CCHID" SessionField="CCHID" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
                <tr>
                    <td valign="top" align="right">
                    </td>
                    <td>
                        <b>OR</b>
                    </td>
                </tr>
                <tr>
                    <td valign="top" align="left">
                        <asp:RadioButton runat="server" ID="rbUseNew" Text="" Checked="false" GroupName="rbAddressGroup" ClientIDMode="Static" />
                    </td>
                    <td>
                        <table id="tblFields" cellspacing="1" cellpadding="0" border="0">
                            <tr>
                                <td colspan="3">
                                    <div class="addressField floatleft">
                                        <asp:TextBox ID="txtChgAddress" runat="server" Text="Enter street address" ValidationGroup="chgLoc" CssClass="addressField" ClientIDMode="Static"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="AddressRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="txtChgAddress" ForeColor="Red" Display="Dynamic" InitialValue="Enter street address" SetFocusOnError="true">
                                            <br />Address is Required
                                        </asp:RequiredFieldValidator>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="cityField floatleft">
                                        <asp:TextBox ID="txtChgCity" runat="server" Text="Enter city" CssClass="cityField" ValidationGroup="chgLoc" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </td>
                                <td>
                                    <div class="stateField floatleft">
                                        <asp:DropDownList ID="ddlState" runat="server" DataSourceID="dsStates" DataTextField="State"
                                            DataValueField="State" OnDataBound="ddlState_DataBound" CssClass="stateField" ValidationGroup="chgLoc" ClientIDMode="Static">
                                        </asp:DropDownList>
                                    </div>
                                </td>
                                <td>
                                    <div class="zipField floatleft">
                                        <asp:TextBox ID="txtChgZipCode" runat="server" Text="Zip Code" CssClass="zipField" ValidationGroup="chgLoc" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:RequiredFieldValidator ID="CityRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="txtChgCity" ForeColor="Red" Display="Dynamic" InitialValue="Enter city" SetFocusOnError="true">
                                        City is Required
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:RequiredFieldValidator ID="StateRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="ddlState" ForeColor="Red" Display="Dynamic" InitialValue="State">
                                        State is Required
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:RequiredFieldValidator ID="ZipRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="txtChgZipCode" ForeColor="Red" Display="Dynamic" InitialValue="Zip Code" SetFocusOnError="true">
                                        Zip Code is Required
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>                        
                        <div class="clearboth" />
                        <asp:SqlDataSource ID="dsStates" runat="server" ConnectionString="<%$ ConnectionStrings:CCH_FrontEnd %>"
                            ProviderName="<%$ ConnectionStrings:CCH_FrontEnd.ProviderName %>" SelectCommand="select * from states">
                        </asp:SqlDataSource>
                        <asp:CheckBox ID="cbSaveAddress" runat="server" Text="Store address for future searches"
                            Checked="false" />
                        <div class="clearboth" />
                    </td>
                </tr>
            </table>            
            <a id="lbtnSaveButton" class="submitlink">Save</a>
            &nbsp&nbsp&nbsp <a id="lblCancel" class="submitlink">Cancel</a>
        </asp:Panel>
    </div>
</div>--%>
<%--<div id="locationbar" style="height:44px;">
    <asp:HiddenField ID="uaLatitude" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="uaLongitude" runat="server" ClientIDMode="Static" />
    <div id="location">
        <span class="callout floatleft">Patient Location:&nbsp;&nbsp;</span>
        <div class="floatleft">
            <b>
                <asp:Label ID="lblPatientAddress" runat="server" Text="" CssClass="patientAddress" />
            </b>
        </div>
        <div class="floatleft">
            &nbsp;&nbsp;[&nbsp;<a class="changelocation" id="aChange">Change Location</a>&nbsp;]
        </div>
    </div>
    <div id="insurer">
        <span class="callout">Health Plan:</span>
        <asp:Label ID="lblInsurerName" runat="server" Text="" Font-Bold="true" /><br />
        <span class="callout">Rx Provider:</span>
        <asp:Label ID="lblRxProviderName" runat="server" Text="" Font-Bold="true" />
    </div>
    <div class="clearboth"></div>
    <div class="locationform">
        <hr />
        <h3>
            Alternate Patient Location
        </h3>
        <a id="cancellocation" class="xclose">X</a>
        <div id="chgloc">
            <table cellspacing="0" cellpadding="5" border="0" width="100%">
                <tr>
                    <td valign="top" align="left">
                        <input type="radio" checked="checked" name="chgLoc" id="rbUseSaved" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlSavedAddresses" runat="server" Width="347px" ClientIDMode="Static"
                            DataTextField="DisplayText" DataValueField="AddressID" OnDataBound="ddlSavedAddresses_DataBound">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td valign="top" align="right">
                    </td>
                    <td>
                        <b>OR</b>
                    </td>
                </tr>
                <tr>
                    <td valign="top" align="left">
                        <input type="radio" name="chgLoc" id="rbUseNew" />
                    </td>
                    <td>
                        <table id="tblFields" cellspacing="1" cellpadding="0" border="0">
                            <tr>
                                <td colspan="3">
                                    <div class="addressField floatleft">
                                        <asp:TextBox ID="txtChgAddress" runat="server" Text="Enter Street Address" ValidationGroup="chgLoc" CssClass="addressField" ClientIDMode="Static" />
                                        <asp:RequiredFieldValidator ID="AddressRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="txtChgAddress" ForeColor="Red" Display="Dynamic"
                                            InitialValue="Enter Street Address" SetFocusOnError="true">
                                            <br />Address is Required    
                                        </asp:RequiredFieldValidator>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="cityField floatleft">
                                        <asp:TextBox ID="txtChgCity" runat="server" Text="Enter City" CssClass="cityField" ValidationGroup="chgLoc" ClientIDMode="Static" />
                                    </div>
                                </td>
                                <td>
                                    <div class="stateField floatleft">
                                        <asp:DropDownList ID="ddlState" runat="server" AutoPostBack="false" CssClass="stateField" ValidationGroup="chgLoc" ClientIDMode="Static">
                                            <asp:ListItem Selected="True" Text="State:" Value="" />
                                            <asp:ListItem Text="AL" Value="AL" />
                                            <asp:ListItem Text="AK" Value="AK" />
                                            <asp:ListItem Text="AZ" Value="AZ" />
                                            <asp:ListItem Text="AR" Value="AR" />
                                            <asp:ListItem Text="CA" Value="CA" />
                                            <asp:ListItem Text="CO" Value="CO" />
                                            <asp:ListItem Text="CT" Value="CT" />
                                            <asp:ListItem Text="DE" Value="DE" />
                                            <asp:ListItem Text="DC" Value="DC" Enabled="false" />
                                            <asp:ListItem Text="FL" Value="FL" />
                                            <asp:ListItem Text="GA" Value="GA" />
                                            <asp:ListItem Text="HI" Value="HI" />
                                            <asp:ListItem Text="ID" Value="ID" />
                                            <asp:ListItem Text="IL" Value="IL" />
                                            <asp:ListItem Text="IN" Value="IN" />
                                            <asp:ListItem Text="IA" Value="IA" />
                                            <asp:ListItem Text="KS" Value="KS" />
                                            <asp:ListItem Text="KY" Value="KY" />
                                            <asp:ListItem Text="LA" Value="LA" />
                                            <asp:ListItem Text="ME" Value="ME" />
                                            <asp:ListItem Text="MD" Value="MD" />
                                            <asp:ListItem Text="MA" Value="MA" />
                                            <asp:ListItem Text="MI" Value="MI" />
                                            <asp:ListItem Text="MN" Value="MN" />
                                            <asp:ListItem Text="MS" Value="MS" />
                                            <asp:ListItem Text="MO" Value="MO" />
                                            <asp:ListItem Text="MT" Value="MT" />
                                            <asp:ListItem Text="NE" Value="NE" />
                                            <asp:ListItem Text="NV" Value="NV" />
                                            <asp:ListItem Text="NH" Value="NH" />
                                            <asp:ListItem Text="NJ" Value="NJ" />
                                            <asp:ListItem Text="NM" Value="NM" />
                                            <asp:ListItem Text="NY" Value="NY" />
                                            <asp:ListItem Text="NC" Value="NC" />
                                            <asp:ListItem Text="ND" Value="ND" />
                                            <asp:ListItem Text="OH" Value="OH" />
                                            <asp:ListItem Text="OK" Value="OK" />
                                            <asp:ListItem Text="OR" Value="OR" />
                                            <asp:ListItem Text="PA" Value="PA" />
                                            <asp:ListItem Text="RI" Value="RI" />
                                            <asp:ListItem Text="SC" Value="SC" />
                                            <asp:ListItem Text="SD" Value="SD" />
                                            <asp:ListItem Text="TN" Value="TN" />
                                            <asp:ListItem Text="TX" Value="TX" />
                                            <asp:ListItem Text="UT" Value="UT" />
                                            <asp:ListItem Text="VT" Value="VT" />
                                            <asp:ListItem Text="VA" Value="VA" />
                                            <asp:ListItem Text="WA" Value="WA" />
                                            <asp:ListItem Text="WV" Value="WV" />
                                            <asp:ListItem Text="WI" Value="WI" />
                                            <asp:ListItem Text="WY" Value="WY" />
                                        </asp:DropDownList>
                                    </div>
                                </td>
                                <td>
                                    <div class="zipField floatleft">
                                        <asp:TextBox ID="txtChgZipCode" runat="server" Text="Zip Code" CssClass="zipField" ValidationGroup="chgLoc" ClientIDMode="Static" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:RequiredFieldValidator ID="CityRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="txtChgCity" ForeColor="Red" Display="Dynamic"
                                        InitialValue="Enter City" SetFocusOnError="true">
                                        City is Required    
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:RequiredFieldValidator ID="StateRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="ddlState" ForeColor="Red" Display="Dynamic" InitialValue="State:">
                                        State is Required
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:RequiredFieldValidator ID="ZipRequired" runat="server" ValidationGroup="chgLoc" ControlToValidate="txtChgZipCode" ForeColor="Red" Display="Dynamic" InitialValue="Zip Code"
                                        SetFocusOnError="true">
                                        Zip Code is Required    
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                        <a id="lbtnSaveButton" class="submitlink">Save</a>
                        &nbsp;&nbsp;&nbsp;
                        <a id="lblCancel" class="submitlink">Cancel</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>--%>