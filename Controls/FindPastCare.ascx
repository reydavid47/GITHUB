<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FindPastCare.ascx.cs" Inherits="ClearCostWeb.Controls.FindPastCare" %>

<asp:UpdatePanel ID="upnlPast" runat="server">
    <ContentTemplate>
        <h1>Past Care for services targeted by ClearCost Health</h1>
        <b>As of:&nbsp;<asp:Literal ID="ltlAsOfDate" runat="server" Text=""></asp:Literal></b>
        <p>
            Choose a family member:
            <asp:DropDownList ID="ddlMembers" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlMembers_SelectedIndexChanged">
            </asp:DropDownList>
        </p>
        <br />
        <table id="tablepastcare" cellspacing="0" cellpadding="4" border="0" width="100%">
            <tr>
                <th class="tdfirst whitediv" colspan="4" width="605px" align="left">
                    Medical Service
                </th>
                <th width="109px" class="whitediv" align="center">
                    Total spent
                </th>
                <th width="116px" align="center">
                    Potential savings
                </th>
            </tr>
            <asp:Literal ID="ltlEnabledOfficeVisits" runat="server" Visible="true">
                <tr class="category rowclosed">
                    <td class="tdfirst whitediv" colspan="4">
            </asp:Literal>
            <asp:Literal ID="ltlDisabledOfficeVisits" runat="server" Visible="false">
                <tr class="category rowclosed" style="background-color:Gray; cursor:default;">
                    <td class="tdfirst whitediv" colspan="4" style="background-image: none;">
            </asp:Literal>
                    Office Visits
                </td>
                <td class="whitediv costcol">
                    <asp:Label ID="lblOfficeVisitsTotalYouSpent" runat="server" Text="--" ForeColor="White"></asp:Label>
                </td>
                <td class="savecol">
                    <asp:Label ID="lblOfficeVisitsTotalYouCouldHaveSaved" runat="server" Text="--" ForeColor="White"></asp:Label>
                </td>
            </tr>
            <asp:Repeater ID="rptOfficeVisits" runat="server" OnItemDataBound="rptOfficeVisits_ItemDataBound">
                <HeaderTemplate>
                    <tr class="careinstance subhead">
                        <td class="tdfirst whitediv">
                            Date
                        </td>
                        <td class="whitediv">
                            Patient
                        </td>
                        <td class="whitediv">
                            Service
                        </td>
                        <td class="whitediv">
                            Provider Name
                        </td>
                        <%--<td class="whitediv" align="center">
                            Fair Price
                        </td>--%>
                        <td class="whitediv" align="center">
                            Total spent
                        </td>
                        <td align="center">
                            Potential savings
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="careinstance roweven">
                        <td class="tdfirst whitediv">
                            <%# Eval("ServiceDate") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PatientName") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("ServiceName").ToString().ToUpper() %> 
                        </td>
                        <td class="whitediv">
                            <%# Eval("PracticeName") %>
                        </td>
                        <%--<td class="whitediv">
                            <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/icon_fp_checkmark.png" Width="23"
                                Height="23" border="" AlternateText="FairPrice?" runat="server" />
                            <asp:Image ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23"
                                border="" AlternateText="FairPrice?" runat="server" />
                        </td>--%>
                        <td class="whitediv costcol">
                            <%# Eval("AllowedAmount") %>
                        </td>
                        <td class="savecol">
                            <asp:LinkButton ID="lbtnYouCouldHaveSaved" runat="server" CssClass="readmore smaller"
                                OnClick="lbtnYouCouldHaveSaved_Click"><%# Eval("YouCouldHaveSaved") %></asp:LinkButton>
                            <%-- <asp:HyperLink ID="hlYouCouldHaveSaved" NavigateUrl="~/SearchInfo/facility_detail.aspx"
                            runat="server" CssClass="readmore smaller"><%# Eval("YouCouldHaveSaved") %></asp:HyperLink>--%>
                            <asp:Label ID="lblYouCouldHaveSavedNothing" runat="server" Text="--" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="careinstance">
                        <td class="tdfirst whitediv">
                            <%# Eval("ServiceDate") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PatientName") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("ServiceName").ToString().ToUpper() %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PracticeName") %>
                        </td>
                        <%--<td class="whitediv">
                            <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/icon_fp_checkmark.png" Width="23"
                                Height="23" border="" AlternateText="FairPrice?" runat="server" />
                            <asp:Image ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23"
                                border="" AlternateText="FairPrice?" runat="server" />
                        </td>--%>
                        <td class="whitediv costcol">
                            <%# Eval("AllowedAmount") %>
                        </td>
                        <td class="savecol">
                            <asp:LinkButton ID="lbtnYouCouldHaveSaved" runat="server" CssClass="readmore smaller"
                                OnClick="lbtnYouCouldHaveSaved_Click"><%# Eval("YouCouldHaveSaved") %></asp:LinkButton>
                            <%-- <asp:HyperLink ID="hlYouCouldHaveSaved" NavigateUrl="~/SearchInfo/facility_detail.aspx"
                            runat="server" CssClass="readmore smaller"><%# Eval("YouCouldHaveSaved") %></asp:HyperLink>--%>
                            <asp:Label ID="lblYouCouldHaveSavedNothing" runat="server" Text="--" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Literal ID="ltlEnableLabServices" runat="server" Visible="true">
                <tr class="category rowclosed">
                    <td class="tdfirst whitediv" colspan="4">
            </asp:Literal>
            <asp:Literal ID="ltlDisableLabServices" runat="server" Visible="false">
                <tr class="category rowclosed" style="background-color:Gray; cursor:default;">
                    <td class="tdfirst whitediv" colspan="4" style="background-image: none;">
            </asp:Literal>
                    Laboratory Services
                </td>
                <td class="whitediv costcol">
                    <asp:Label ID="lblLaboratoryServicesTotalYouSpent" runat="server" Text="--" ForeColor="White"></asp:Label>
                </td>
                <td class="savecol">
                    <asp:Label ID="lblLaboratoryServicesTotalYouCouldHaveSaved" runat="server" Text="--"
                        ForeColor="White"></asp:Label>
                </td>
            </tr>
            <asp:Repeater ID="rptLaboratoryServices" runat="server" OnItemDataBound="rptOfficeVisits_ItemDataBound">
                <HeaderTemplate>
                    <%--<table>
    <tr class="subhead">--%>
                    <tr class="careinstance subhead">
                        <td class="tdfirst whitediv">
                            Date
                        </td>
                        <td class="whitediv">
                            Patient
                        </td>
                        <td class="whitediv">
                            Service
                        </td>
                        <td class="whitediv">
                            Provider Name
                        </td>
                        <%--<td class="whitediv" align="center">
                            Fair Price
                        </td>--%>
                        <td class="whitediv" align="center">
                            Total spent
                        </td>
                        <td align="center">
                            Potential savings
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="careinstance roweven">
                        <td class="tdfirst whitediv">
                            <%# Eval("ServiceDate") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PatientName") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("ServiceName").ToString().ToUpper() %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PracticeName") %>
                        </td>
                        <%--<td class="whitediv" align="center">
                            <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/icon_fp_checkmark.png" Width="23"
                                Height="23" border="" AlternateText="FairPrice?" runat="server" />
                            <asp:Image ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23"
                                border="" AlternateText="FairPrice?" runat="server" />
                        </td>--%>
                        <td class="whitediv costcol">
                            <%# Eval("AllowedAmount") %>
                        </td>
                        <td class="savecol">
                            <asp:LinkButton ID="lbtnYouCouldHaveSaved" runat="server" CssClass="readmore smaller"
                                OnClick="lbtnYouCouldHaveSaved_Click"><%# Eval("YouCouldHaveSaved") %></asp:LinkButton>
                            <%-- <asp:HyperLink ID="hlYouCouldHaveSaved" NavigateUrl="~/SearchInfo/facility_detail.aspx"
                            runat="server" CssClass="readmore smaller"><%# Eval("YouCouldHaveSaved") %></asp:HyperLink>--%>
                            <asp:Label ID="lblYouCouldHaveSavedNothing" runat="server" Text="--" Visible="false"></asp:Label>
                            <%--<a href="facility_detail.aspx" class="readmore smaller">
                    <%# Eval("YouCouldHaveSaved") %></a>--%>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="careinstance">
                        <td class="tdfirst whitediv">
                            <%# Eval("ServiceDate") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PatientName") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("ServiceName").ToString().ToUpper() %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PracticeName") %>
                        </td>
                        <%--<td class="whitediv" align="center">
                            <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/icon_fp_checkmark.png" Width="23"
                                Height="23" border="" AlternateText="FairPrice?" runat="server" />
                            <asp:Image ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23"
                                border="" AlternateText="FairPrice?" runat="server" />
                        </td>--%>
                        <td class="whitediv costcol">
                            <%# Eval("AllowedAmount") %>
                        </td>
                        <td class="savecol">
                            <asp:LinkButton ID="lbtnYouCouldHaveSaved" runat="server" CssClass="readmore smaller"
                                OnClick="lbtnYouCouldHaveSaved_Click"><%# Eval("YouCouldHaveSaved") %></asp:LinkButton>
                            <%-- <asp:HyperLink ID="hlYouCouldHaveSaved" NavigateUrl="~/SearchInfo/facility_detail.aspx"
                            runat="server" CssClass="readmore smaller"><%# Eval("YouCouldHaveSaved") %></asp:HyperLink>--%>
                            <asp:Label ID="lblYouCouldHaveSavedNothing" runat="server" Text="--" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    <%--</table>--%></FooterTemplate>
            </asp:Repeater>
            <asp:Literal ID="ltlEnableOutpatient" runat="server" Visible="true">
                <tr class="category rowclosed">
                    <td class="tdfirst whitediv" colspan="4">
            </asp:Literal>
            <asp:Literal ID="ltlDisableOutpatient" runat="server" Visible="false">
                <tr class="category rowclosed" style="background-color:Gray; cursor:default;">
                    <td class="tdfirst whitediv" colspan="4" style="background-image: none;">
            </asp:Literal>
                    Outpatient Procedures
                </td>
                <td class="whitediv costcol">
                    <asp:Label ID="lblOutpatientProceduresTotalYouSpent" runat="server" Text="--" ForeColor="White"></asp:Label>
                </td>
                <td class="savecol">
                    <asp:Label ID="lblOutpatientProceduresTotalYouCouldHaveSaved" runat="server" Text="--"
                        ForeColor="White"></asp:Label>
                </td>
            </tr>
            <asp:Repeater ID="rptOutpatientProcedures" runat="server" OnItemDataBound="rptOfficeVisits_ItemDataBound">
                <HeaderTemplate>
                    <tr class="careinstance subhead">
                        <td class="tdfirst whitediv">
                            Date
                        </td>
                        <td class="whitediv">
                            Patient
                        </td>
                        <td class="whitediv">
                            Service
                        </td>
                        <td class="whitediv">
                            Provider Name
                        </td>
                        <%--<td class="whitediv" align="center">
                            Fair Price
                        </td>--%>
                        <td class="whitediv" align="center">
                            Total spent
                        </td>
                        <td align="center">
                            Potential savings
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="careinstance roweven">
                        <td class="tdfirst whitediv">
                            <%# Eval("ServiceDate") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PatientName") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("ServiceName").ToString().ToUpper() %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PracticeName") %>
                        </td>
                        <%--<td class="whitediv">
                            <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/icon_fp_checkmark.png" Width="23"
                                Height="23" border="" AlternateText="FairPrice?" runat="server" />
                            <asp:Image ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23"
                                border="" AlternateText="FairPrice?" runat="server" />
                        </td>--%>
                        <td class="whitediv costcol">
                            <%# Eval("AllowedAmount") %>
                        </td>
                        <td class="savecol">
                            <asp:LinkButton ID="lbtnYouCouldHaveSaved" runat="server" CssClass="readmore smaller"
                                OnClick="lbtnYouCouldHaveSaved_Click"><%# Eval("YouCouldHaveSaved") %></asp:LinkButton>
                            <%-- <asp:HyperLink ID="hlYouCouldHaveSaved" NavigateUrl="~/SearchInfo/facility_detail.aspx"
                            runat="server" CssClass="readmore smaller"><%# Eval("YouCouldHaveSaved") %></asp:HyperLink>--%>
                            <asp:Label ID="lblYouCouldHaveSavedNothing" runat="server" Text="--" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="careinstance">
                        <td class="tdfirst whitediv">
                            <%# Eval("ServiceDate") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PatientName") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("ServiceName").ToString().ToUpper() %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PracticeName") %>
                        </td>
                        <%--<td class="whitediv">
                            <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/icon_fp_checkmark.png" Width="23"
                                Height="23" border="" AlternateText="FairPrice?" runat="server" />
                            <asp:Image ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23"
                                border="" AlternateText="FairPrice?" runat="server" />
                        </td>--%>
                        <td class="whitediv costcol">
                            <%# Eval("AllowedAmount") %>
                        </td>
                        <td class="savecol">
                            <asp:LinkButton ID="lbtnYouCouldHaveSaved" runat="server" CssClass="readmore smaller"
                                OnClick="lbtnYouCouldHaveSaved_Click"><%# Eval("YouCouldHaveSaved") %></asp:LinkButton>
                            <%-- <asp:HyperLink ID="hlYouCouldHaveSaved" NavigateUrl="~/SearchInfo/facility_detail.aspx"
                            runat="server" CssClass="readmore smaller"><%# Eval("YouCouldHaveSaved") %></asp:HyperLink>--%>
                            <asp:Label ID="lblYouCouldHaveSavedNothing" runat="server" Text="--" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Literal ID="ltlEnableImaging" runat="server" Visible="true">
                <tr class="category rowclosed">
                    <td class="tdfirst whitediv" colspan="4">
            </asp:Literal>
            <asp:Literal ID="ltlDisableImaging" runat="server" Visible="false">
                <tr class="category rowclosed" style="background-color:Gray; cursor:default;">
                    <td class="tdfirst whitediv" colspan="4" style="background-image: none;">
            </asp:Literal>
                    Imaging
                </td>
                <td class="whitediv costcol">
                    <asp:Label ID="lblImagingTotalYouSpent" runat="server" Text="--" ForeColor="White"></asp:Label>
                </td>
                <td class="savecol">
                    <asp:Label ID="lblImagingTotalYouCouldHaveSaved" runat="server" Text="--" ForeColor="White"></asp:Label>
                </td>
            </tr>
            <asp:Repeater ID="rptImaging" runat="server" OnItemDataBound="rptOfficeVisits_ItemDataBound">
                <HeaderTemplate>
                    <tr class="careinstance subhead">
                        <td class="tdfirst whitediv">
                            Date
                        </td>
                        <td class="whitediv">
                            Patient
                        </td>
                        <td class="whitediv">
                            Service
                        </td>
                        <td class="whitediv">
                            Provider Name
                        </td>
                        <%--<td class="whitediv" align="center">
                            Fair Price
                        </td>--%>
                        <td class="whitediv" align="center">
                            Total spent
                        </td>
                        <td align="center">
                            Potential savings
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="careinstance roweven">
                        <td class="tdfirst whitediv">
                            <%# Eval("ServiceDate") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PatientName") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("ServiceName").ToString().ToUpper() %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PracticeName") %>
                        </td>
                        <%--<td class="whitediv">
                            <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/icon_fp_checkmark.png" Width="23"
                                Height="23" border="" AlternateText="FairPrice?" runat="server" />
                            <asp:Image ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23"
                                border="" AlternateText="FairPrice?" runat="server" />
                        </td>--%>
                        <td class="whitediv costcol">
                            <%# Eval("AllowedAmount") %>
                        </td>
                        <td class="savecol">
                            <asp:LinkButton ID="lbtnYouCouldHaveSaved" runat="server" CssClass="readmore smaller"
                                OnClick="lbtnYouCouldHaveSaved_Click"><%# Eval("YouCouldHaveSaved") %></asp:LinkButton>
                            <%-- <asp:HyperLink ID="hlYouCouldHaveSaved" NavigateUrl="~/SearchInfo/facility_detail.aspx"
                            runat="server" CssClass="readmore smaller"><%# Eval("YouCouldHaveSaved") %></asp:HyperLink>--%>
                            <asp:Label ID="lblYouCouldHaveSavedNothing" runat="server" Text="--" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="careinstance">
                        <td class="tdfirst whitediv">
                            <%# Eval("ServiceDate") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PatientName") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("ServiceName").ToString().ToUpper() %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PracticeName") %>
                        </td>
                       <%-- <td class="whitediv">
                            <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/icon_fp_checkmark.png" Width="23"
                                Height="23" border="" AlternateText="FairPrice?" runat="server" />
                            <asp:Image ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23"
                                border="" AlternateText="FairPrice?" runat="server" />
                        </td>--%>
                        <td class="whitediv costcol">
                            <%# Eval("AllowedAmount") %>
                        </td>
                        <td class="savecol">
                            <asp:LinkButton ID="lbtnYouCouldHaveSaved" runat="server" CssClass="readmore smaller"
                                OnClick="lbtnYouCouldHaveSaved_Click"><%# Eval("YouCouldHaveSaved") %></asp:LinkButton>
                            <%-- <asp:HyperLink ID="hlYouCouldHaveSaved" NavigateUrl="~/SearchInfo/facility_detail.aspx"
                            runat="server" CssClass="readmore smaller"><%# Eval("YouCouldHaveSaved") %></asp:HyperLink>--%>
                            <asp:Label ID="lblYouCouldHaveSavedNothing" runat="server" Text="--" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Literal ID="ltlEnableDrugs" runat="server" Visible="true">
                <tr class="category rowclosed">
                    <td class="tdfirst whitediv" colspan="4">
            </asp:Literal>
            <asp:Literal ID="ltlDisableDrugs" runat="server" Visible="false">
                <tr class="category rowclosed" style="background-color:Gray; cursor:default;">
                    <td class="tdfirst whitediv" colspan="4" style="background-image: none;">
            </asp:Literal>
                    Prescription Drugs
                </td>
                <td class="whitediv costcol">
                    <asp:Label ID="lblPrescriptionDrugsTotalYouSpent" runat="server" Text="--" ForeColor="White"></asp:Label>
                </td>
                <td class="savecol">
                    <asp:Label ID="lblPrescriptionDrugsTotalYouCouldHaveSaved" runat="server" Text="--"
                        ForeColor="White"></asp:Label>
                </td>
            </tr>
            <asp:Repeater ID="rptPrescriptionDrugs" runat="server" OnItemDataBound="rptOfficeVisits_ItemDataBound">
                <HeaderTemplate>
                    <tr class="careinstance subhead">
                        <td class="tdfirst whitediv">
                            Date
                        </td>
                        <td class="whitediv">
                            Patient
                        </td>
                        <td class="whitediv">
                            Drug
                        </td>
                        <td class="whitediv">
                            Pharmacy
                        </td>
                        <%--<td class="whitediv" align="center">
                            Fair Price
                        </td>--%>
                        <td class="whitediv" align="center">
                            Total spent
                        </td>
                        <td align="center">
                            Potential savings
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="careinstance roweven">
                        <td class="tdfirst whitediv">
                            <%# Eval("ServiceDate") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PatientName") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("ServiceName").ToString().ToUpper() %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PracticeName") %>
                        </td>
                        <%--<td class="whitediv">
                            <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/icon_fp_checkmark.png" Width="23"
                                Height="23" border="" AlternateText="FairPrice?" runat="server" />
                            <asp:Image ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23"
                                border="" AlternateText="FairPrice?" runat="server" />
                        </td>--%>
                        <td class="whitediv costcol">
                            <%# Eval("AllowedAmount") %>
                        </td>
                        <td class="savecol">
                            <asp:LinkButton ID="lbtnYouCouldHaveSaved" runat="server" CssClass="readmore smaller"
                                OnClick="lbtnYouCouldHaveSaved_Click"><%# Eval("YouCouldHaveSaved") %></asp:LinkButton>
                            <asp:Label ID="lblYouCouldHaveSavedNothing" runat="server" Text="--" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="careinstance">
                        <td class="tdfirst whitediv">
                            <%# Eval("ServiceDate") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PatientName") %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("ServiceName").ToString().ToUpper() %>
                        </td>
                        <td class="whitediv">
                            <%# Eval("PracticeName") %>
                        </td>
                        <%--<td class="whitediv">
                            <asp:Image ID="imgFairPriceTrue" ImageUrl="../Images/icon_fp_checkmark.png" Width="23"
                                Height="23" border="" AlternateText="FairPrice?" runat="server" />
                            <asp:Image ID="imgFairPriceFalse" ImageUrl="~/Images/s.gif" Width="23" Height="23"
                                border="" AlternateText="FairPrice?" runat="server" />
                        </td>--%>
                        <td class="whitediv costcol">
                            <%# Eval("AllowedAmount") %>
                        </td>
                        <td class="savecol">
                            <asp:LinkButton ID="lbtnYouCouldHaveSaved" runat="server" CssClass="readmore smaller"
                                OnClick="lbtnYouCouldHaveSaved_Click"><%# Eval("YouCouldHaveSaved") %></asp:LinkButton>
                            <asp:Label ID="lblYouCouldHaveSavedNothing" runat="server" Text="--" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
            <tr class="trfooter">
                <td class="tdfirst whitediv" colspan="4">
                    Total
                </td>
                <td class="whitediv costcol">
                    <asp:Label ID="lblTotalYouSpent" runat="server" Text="--"></asp:Label>
                </td>
                <td class="savecol">
                    <asp:Label ID="lblTotalYouCouldHaveSaved" runat="server" Text="--"></asp:Label>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:Label ID="lblPastCareDisclaimerText" runat="server" CssClass="smaller" Font-Italic="True"></asp:Label>
