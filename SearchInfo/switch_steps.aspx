<%@ Page Title="" Language="C#" MasterPageFile="~/SearchInfo/Results.master" AutoEventWireup="true" CodeFile="switch_steps.aspx.cs" Inherits="ClearCostWeb.SearchInfo.switch_steps" %>
<%@ Register Src="~/Controls/AlertBar.ascx" TagPrefix="cch" TagName="AlertBar" %>
<%@ Register Src="~/Controls/Print.ascx" TagPrefix="cch" TagName="Print" %>

<asp:Content ID="switch_steps_Content" ContentPlaceHolderID="ResultsContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("p#therasublink a").click(function () {
                $(this).hide();
                $("div#therasub2").show();
            });
        });
    </script>
    <cch:Print runat="server" Visible="false" />
    <%--<script type="text/javascript">
        function printNote(divId) {
            var DocumentContainer = document.getElementById(divId);
            var html = "<html><head><link href='/Styles/old/style.css' rel='stylesheet' type='text/css' /></head><body>" +
	        DocumentContainer.innerHTML +
	        "</body></html>";
            var WindowObject = window.open("", "PrintWindow",
	        "width=800,height=600,top=50,left=50,toolbars=no,scrollbars=yes,status=no,resizable=yes");
            WindowObject.document.writeln(html);
            WindowObject.document.close();
            WindowObject.focus();
            WindowObject.print();
        }
    </script>--%>
    <h1>
        Save Money on Rx</h1>
    <cch:AlertBar runat="server" ID="abCouldSave" TypeOfAlert="Small" SaveTotal="" Visible="true">
        <MessageTemplate>
            Estimated Annual Savings for you and your employer: <b><%# Container.SaveTotal %></b>
        </MessageTemplate>
    </cch:AlertBar>
    <hr class="heavy" />
    <p class="step1">
        <b>Select the prescriptions you would like to switch</b><br />
        This will require a new prescription. But don't worry, our two-step process will
        make it easy for you to request a new prescription or just share this information
        with your doctor.
    </p>
    <div id="switchrxform">
        <asp:Repeater ID="rptMemberSubs" runat="server" OnItemDataBound="BindMed">
            <ItemTemplate>
                <p><b><%# String.Format("{0} {1}", Eval("FirstName"), Eval("LastName")) %></b></p>
                <div class="membermedvisible">
                    <asp:Repeater ID="rptMeds" runat="server">
                        <ItemTemplate>
                            <p>
                                <input type="checkbox" name='<%# Eval("DrugName").ToString().Trim() %>' value='<%# Eval("DrugName").ToString().Trim() %>' checked="checked" onchange='<%# String.Format("$(\"#tr{0}\").toggle();", Eval("DrugName").ToString().Trim()) %>' />
                                <%# String.Format("{0} {1} - {2:f0} {3}", Eval("DrugName").ToString().Trim(), Eval("Strength"), Double.Parse(Eval("Quantity").ToString()), Eval("QuantityUOM"))%>
                                &nbsp; &rarr; &nbsp;
                                <%# String.Format("{0} {1} - {2:f0} {3}", Eval("ReplacementDrugName"), Eval("ReplacementStrength"), Double.Parse(Eval("Quantity").ToString()), Eval("ReplacementQuantityUOM")) %>
                            </p>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <p id="therasublink">
        <a class="submitlink" onclick="return true;">Next Step</a>
    </p>
    <asp:Panel ID="therasub2" ClientIDMode="Static" runat="server">
        <hr />
        <p class="step2">
            <b>Select how you want to share this with your doctor</b><br />
            <br />
        </p>
        <div id="shareswitch">
            <p>
                <input type="radio" name="ShareChoice" onchange="$('#FaxOrEmail').show();$('#lbPrint').hide();$('#lbShare').show();" />
                I would like to print this information to share with my doctor during my next appointment
                <%--<asp:RadioButton ID="rbPrint" runat="server" GroupName="ShareChoice" Text="I would like to print this information to share with my doctor during my next appointment"
                 onchange="$('#FaxOrEmail').hide();$('#lbPrint').show();$('#lbShare').hide();" />--%>
            </p>
            <p>
                <input type="radio" name="ShareChoice" onchange="$('#FaxOrEmail').hide();$('#lbPrint').show();$('#lbShare').hide();" checked="checked" />
                I would like to request a new prescription from my doctor
                <%--<asp:RadioButton ID="rbSend" runat="server" GroupName="ShareChoice" Text="I would like to request a new prescription from my doctor" Checked="true" 
                 onchange="$('#FaxOrEmail').show();$('#lbPrint').hide();$('#lbShare').show();" />   --%>             
            </p>
            <div id="FaxOrEmail">
                <p>
                    <b>Enter the fax number of your doctor to request a new prescription</b>
                </p>
                <p>
                    Prescribing doctor: <%# String.Format("{0} {1}", DocFirstName, DocLastName) %>
                </p>
                <table cellspacing="0" cellpadding="4" border="0" class="">
                    <%--<tr>
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
                    </tr>--%>
                    <tr>
                        <td>
                            Fax
                        </td>
                        <td>
                            <input type="text" class="boxed" name="fax" value="" />
                        </td>
                    </tr>
                </table>
                <p>
                    Your doctor will receive the fax shown below
                </p>
            </div>
            <p>
                <a id="lbPrint" onclick="printResults('docnote');" class="submitlink" style="display:none;cursor:pointer;">Share my search results</a>
                <asp:LinkButton ID="lbShare" runat="server" CssClass="submitlink" Text="Share my search results" OnClick="ShareResults" ClientIDMode="Static" />
            </p>
            <div id="docnote" style="display: block;">
                <div style="width: 710px; border: 2px #ccc solid; margin: 40px auto; padding: 25px;">
                    <img src="../Images/clearcosthealth_logo_email.gif" alt="ClearCost Health" width="200"
                        height="30" border="0" /><br />
                    <table cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td>
                                From:
                            </td>
                            <td>
                                ClearCost Health
                            </td>
                        </tr>
                        <tr>
                            <td>
                                To:
                            </td>
                            <td>
                                <%# PrescribingDoctor %>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-right: 15px;">
                                Subject:
                            </td>
                            <td>
                                Patient <%# PatientName %>
                            </td>
                        </tr>
                    </table>
                    <p>
                        Dear <%# String.Format("{0} {1}", DocTitle, DocLastName) %>,
                    </p>
                    <p>
                        Your patient, <%# PatientName %>, used ClearCost Health to find therapeutic substitutes
                        for two current medications. These alternatives could save <%# PatientName %> and her employer
                        up to <%# SaveTotal %> per year. In your medical opinion, if you feel these are acceptable
                        alternatives, then please write a new prescription for these drugs.
                    </p>
                    <p>
                        For more information on ClearCost Health, please visit us at <a href="http://www.clearcosthealth.com"
                            target="_new">www.clearcosthealth.com</a>
                    </p>
                    <p>
                        Thanks,<br />
                        The ClearCost Health team
                    </p>
                    <hr />
                    <asp:Repeater ID="rptSearchQuery" runat="server">
                        <HeaderTemplate>
                            <table cellspacing="0" cellpadding="4" border="0" class="searchquery">
                                <tr>
                                    <td class="alignleft">
                                        <b>Current Drug</b>
                                    </td>
                                    <td>
                                        <b>Therapeutic Alternative</b>
                                    </td>
                                    <td>
                                        <b>Current Pharmacy</b>
                                    </td>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr class="graydiv graytop" id='<%# String.Format("tr{0}", Eval("DrugName").ToString().Trim()) %>'>
                                <td class="graydiv alignleft">
                                    <%# String.Format("{0} {1}", Eval("DrugName").ToString().Trim(), Eval("Strength")) %>
                                </td>
                                <td class="graydiv">
                                    <%# String.Format("{0} {1}", Eval("ReplacementDrugName"), Eval("ReplacementStrength")) %>
                                </td>
                                <td class="graydiv">
                                    <%# Eval("PharmacyName") %><br />
                                    <%# Eval("Address1") %><br />
                                    <%# ((Eval("Address2").ToString().Trim() == "") ? "" : String.Format("{0}<br />", Eval("Address2"))) %>
                                    <%# String.Format("{0}, {1} {2}", Eval("City"), Eval("State"), Eval("ZipCode")) %><br />
                                    <b>Phone: <%# Eval("Telephone") %></b>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <!-- end docnote -->
        </div>
    </asp:Panel>
    <!-- end therasub2 -->

</asp:Content>

