<%@ Page Title="" Language="C#" MasterPageFile="~/ContentManagement/ContentManagement.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="ClearCostWeb.ContentManagement.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <asp:ScriptManager ID="smContentManagement" runat="server" />
    <div align="center">
        <h1>
            Content Management
        </h1>
    </div>
    <br />
    <div>            
        <asp:UpdatePanel ID="upEmployer" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>
                <div align="center">
                    <asp:Label ID="lblEmployers" runat="server" Text="Available Employers: " AssociatedControlID="ddlEmployers" />
                    <asp:DropDownList ID="ddlEmployers" runat="server" DataValueField="EmployerID" 
                        DataTextField="EmployerName" AutoPostBack="True" OnSelectedIndexChanged="selectEmployer" />
                    <br />
                    <div style="height:22px;">
                        <asp:UpdateProgress ID="upWorking" runat="server" AssociatedUpdatePanelID="upEmployer" DisplayAfter="0">
                            <ProgressTemplate>
                                Working...
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>             
                    <asp:CheckBox ID="cbEnabled" runat="server" Text="Enable Content Management" 
                        AutoPostBack="true" oncheckedchanged="cbEnabled_CheckedChanged" Enabled="false" fID="ContentManagementEnabled" />
                </div>
                
                <asp:Panel ID="pContentWrapper" runat="server" Enabled="false" BorderStyle="Solid" BorderColor="Gray" Width="400px" style="padding: 10px;margin: 5px auto 20px auto;">
                    <div>
                        <asp:Label ID="Label1" runat="server" AssociatedControlID="lblEmployer" Text="Employer: " />
                        <asp:Label ID="lblEmployer" runat="server" CssClass="StaticContent" 
                            ToolTip="*Property is Read-Only but displayed to better identify the employer you're working with." />
                    </div>
                    <div>
                        <asp:Label ID="Label2" runat="server" AssociatedControlID="lblInsurer" Text="Insurer: " />
                        <asp:Label ID="lblInsurer" runat="server" CssClass="StaticContent" 
                            ToolTip="*Property is Read-Only but displayed to better identify the employer you're working with." />
                    </div>
                    <div>
                        <asp:Label ID="Label3" runat="server" AssociatedControlID="lblRXProvider" Text="RX Provider: " />
                        <asp:Label ID="lblRXProvider" runat="server" CssClass="StaticContent" 
                            ToolTip="*Property is Read-Only but displayed to better identify the employer you're working with." />
                    </div>
                    <div>
                        <asp:Label ID="Label4" runat="server" AssociatedControlID="cbYCEnabled" Text="Your Cost Enabled: " />
                        <asp:CheckBox ID="cbYCEnabled" runat="server" Enabled="false" Text="" 
                            ToolTip="*Property is Read-Only but displayed to better identify the employer you're working with." />
                    </div>
                    <div>
                        <asp:Label ID="Label5" runat="server" AssociatedControlID="cbYCOn" Text="Show Your Cost by default: " />
                        <asp:CheckBox ID="cbYCOn" runat="server" Text="" OnCheckedChanged="ValidateChange" AutoPostBack="true" fID="DefaultYourCostOn" />
                    </div>
                    <div>
                        <asp:Label ID="Label28" runat="server" AssociatedControlID="cbSCEnabled" Text="Savings Choice Enabled: " />
                        <asp:CheckBox ID="cbSCEnabled" runat="server" Text="" OnCheckedChanged="ValidateChange" AutoPostBack="true" fID="SavingsChoiceEnabled" />
                    </div>
                    <div>
                        <asp:Label ID="Label29" runat="server" AssociatedControlID="cbShowSCIQTab" Text="Show SCIQ Tab: " />
                        <asp:CheckBox ID="cbShowSCIQTab" runat="server" Text="" OnCheckedChanged="ValidateChange" AutoPostBack="true" fID="ShowSCIQTab" />
                    </div>
                    <div>
                        <asp:Panel runat="server" ID="pnlDefaultSort">
                            <asp:Label ID="Label6" runat="server" Text="Default Sort: " />
                            <asp:RadioButton ID="rbDistance" runat="server" Text="Distance" GroupName="defaultSort" Checked="true" AutoPostBack="true" OnCheckedChanged="ValidateChange" fID="DefaultSort" />
                            <asp:RadioButton ID="rbTotalCost" runat="server" Text="Total Cost" GroupName="defaultSort" Checked="false" AutoPostBack="true" OnCheckedChanged="ValidateChange" fID="DefaultSort" />
                            <asp:RadioButton ID="rbFairPrice" runat="server" Text="Fair Price" GroupName="defaultSort" Checked="false" AutoPostBack="true" OnCheckedChanged="ValidateChange" fID="DefaultSort" />
                        </asp:Panel>
                    </div>
                    <div>
                        <asp:Label ID="Label7" runat="server" AssociatedControlID="cbSSNOnly" Text="Only SSN Registration: " />
                        <asp:CheckBox ID="cbSSNOnly" runat="server" Text="" AutoPostBack="true" OnCheckedChanged="ValidateChange" fID="SSNOnly" />
                    </div>
                    <div>
                        <asp:Label ID="Label8" runat="server" AssociatedControlID="txtMemberIDFormat" Text="Member ID Format: " />
                        <asp:TextBox ID="txtMemberIDFormat" runat="server" Text="" OnTextChanged="ValidateChange" AutoPostBack="true" fID="MemberIDFormat" />
                    </div>
                    <div>
                        <asp:Label ID="Label9" runat="server" AssociatedControlID="txtInsurerName" Text="Insurer Display Name: " />
                        <asp:TextBox ID="txtInsurerName" runat="server" Text="" OnTextChanged="ValidateChange" AutoPostBack="true" fID="InsurerName"  />
                    </div>
                    <div>
                        <asp:Label ID="Label10" runat="server" AssociatedControlID="txtLogoFilename" Text="Logo Image File: " />
                        <asp:TextBox ID="txtLogoFilename" runat="server" Text="" OnTextChanged="ValidateChange" AutoPostBack="true" fID="LogoImageName"  />
                    </div>
                    <div>
                        <asp:Label ID="Label11" runat="server" AssociatedControlID="cbOtherPeople" Text="Show &quot;Other People&quot; Section During Registration: " />
                        <asp:CheckBox ID="cbOtherPeople" runat="server" Text="" OnCheckedChanged="ValidateChange" AutoPostBack="true" fID="HasOtherPeopleSection" />
                    </div>
                    <div>
                        <asp:Label ID="Label12" runat="server" AssociatedControlID="cbNotifications" Text="Show &quot;Notifications&quot; Section During Registration: " />
                        <asp:CheckBox ID="cbNotifications" runat="server" Text="" OnCheckedChanged="ValidateChange" AutoPostBack="true" fID="HasNotificationSection" />
                    </div>
                    <div>
                        <asp:Label ID="Label13" runat="server" AssociatedControlID="txtContactText" Text="Account Menu Contact Text:" />
                        <br />
                        <asp:TextBox runat="server" ID="txtContactText" Text="" OnTextChanged="ValidateChange" AutoPostBack="true" fID="ContactText" TextMode="MultiLine"
                            Width="375px" Height="50px" style="margin-left:10px;" />
                    </div>
                    <div>
                        <asp:Label ID="Label14" runat="server" AssociatedControlID="txtSpecialtyNetworkText" Text="Specialty Network Text:" />
                        <br />
                        <asp:TextBox runat="server" ID="txtSpecialtyNetworkText" OnTextChanged="ValidateChange" AutoPostBack="true" fID="SpecialtyNetworkText" TextMode="MultiLine"
                            Width="375px" Height="50px" style="margin-left:10px;" />
                    </div>
                    <div>
                        <asp:Label ID="Label21" runat="server" AssociatedControlID="txtPastCareDisclaimerText" Text="PastCare Disclaimer Text:" />
                        <br />
                        <asp:TextBox runat="server" ID="txtPastCareDisclaimerText" OnTextChanged="ValidateChange" AutoPostBack="true" fID="PastCareDisclaimerText" TextMode="MultiLine"
                            MaxLength="500" Width="375px" Height="50px" style="margin-left:10px;" />
                    </div>
                    <div>
                        <asp:Label ID="Label22" runat="server" AssociatedControlID="txtRxResultDisclaimerText" Text="Rx Result Disclaimer Text:" />
                        <br />
                        <asp:TextBox runat="server" ID="txtRxResultDisclaimerText" OnTextChanged="ValidateChange" AutoPostBack="true" fID="RxResultDisclaimerText" TextMode="MultiLine"
                            MaxLength="500" Width="375px" Height="50px" style="margin-left:10px;" />
                    </div>
                    <div>
                        <asp:Label ID="Label24" runat="server" AssociatedControlID="txtAllResult1DisclaimerText" Text="All Result (1) Disclaimer Text:" />
                        <br />
                        <asp:TextBox runat="server" ID="txtAllResult1DisclaimerText" OnTextChanged="ValidateChange" AutoPostBack="true" fID="AllResult1DisclaimerText" TextMode="MultiLine"
                            MaxLength="500" Width="375px" Height="50px" style="margin-left:10px;" />
                    </div>
                    <div>
                        <asp:Label ID="Label25" runat="server" AssociatedControlID="txtAllResult2DisclaimerText" Text="All Result (2) Disclaimer Text:" />
                        <br />
                        <asp:TextBox runat="server" ID="txtAllResult2DisclaimerText" OnTextChanged="ValidateChange" AutoPostBack="true" fID="AllResult2DisclaimerText" TextMode="MultiLine"
                            MaxLength="500" Width="375px" Height="50px" style="margin-left:10px;" />
                    </div>
                    <div>
                        <asp:Label ID="Label23" runat="server" AssociatedControlID="txtSpecialtyDrugDisclaimerText" Text="Specialty Drug Disclaimer Text:" />
                        <br />
                        <asp:TextBox runat="server" ID="txtSpecialtyDrugDisclaimerText" OnTextChanged="ValidateChange" AutoPostBack="true" fID="SpecialtyDrugDisclaimerText" TextMode="MultiLine"
                            MaxLength="500" Width="375px" Height="50px" style="margin-left:10px;" />
                    </div>
                    <div>
                        <asp:Label ID="Label26" runat="server" AssociatedControlID="txtMentalHealthDisclaimerText" Text="Mental Health Disclaimer Text:" />
                        <br />
                        <asp:TextBox runat="server" ID="txtMentalHealthDisclaimerText" OnTextChanged="ValidateChange" AutoPostBack="true" fID="MentalHealthDisclaimerText" TextMode="MultiLine"
                            MaxLength="500" Width="375px" Height="50px" style="margin-left:10px;" />
                    </div>
                    <div>
                        <asp:Label ID="Label27" runat="server" AssociatedControlID="txtServiceNotFoundDisclaimerText" Text="Service Not Found Disclaimer Text:" />
                        <br />
                        <asp:TextBox runat="server" ID="txtServiceNotFoundDisclaimerText" OnTextChanged="ValidateChange" AutoPostBack="true" fID="ServiceNotFoundDisclaimerText" TextMode="MultiLine"
                            MaxLength="500" Width="375px" Height="50px" style="margin-left:10px;" />
                    </div>
                    <div>
                        <asp:Label ID="Label15" runat="server" AssociatedControlID="cbTandC" Text="Show Terms And Conditions window During Registration: " />
                        <asp:CheckBox ID="cbTandC" runat="server" Text="" OnCheckedChanged="ValidateChange" AutoPostBack="true" fID="TandCVisible" />
                    </div>
                    <div>
                        <asp:Label ID="Label16" runat="server" AssociatedControlID="txtPhone" Text="Phone Number(as xxxxxxxxxx): " />
                        <asp:TextBox ID="txtPhone" runat="server" Text="" OnTextChanged="ValidateChange" AutoPostBack="true" fID="PhoneNumber" />
                    </div>
                    <div>
                        <asp:Label ID="Label17" runat="server" AssociatedControlID="cbInternalLogo" Text="Show Logo on Search and Results Pages: " />
                        <asp:CheckBox ID="cbInternalLogo" runat="server" Text="" OnCheckedChanged="ValidateChange" AutoPostBack="true" fID="InternalLogo" />
                    </div>
                    <br />
                    <div>
                        <asp:Label ID="Label18" runat="server" AssociatedControlID="cbSignIn" Text="Enable Sign-In: " />
                        <asp:CheckBox ID="cbSignIn" runat="server" Text="" AutoPostBack="true" OnCheckedChanged="ValidateChange" fID="CanSignIn" />
                    </div>
                    <div>
                        <asp:Label ID="Label19" runat="server" AssociatedControlID="cbRegister" Text="Enable Registration: " />
                        <asp:CheckBox ID="cbRegister" runat="server" Text="" OnCheckedChanged="ValidateChange" AutoPostBack="true" fID="CanRegister" />
                    </div>
                    <div>
                        <asp:Label ID="Label20" runat="server" AssociatedControlID="cbSignInCentric" Text="Sign In Oriented Landing Page: " />
                        <asp:CheckBox ID="cbSignInCentric" runat="server" Text="" OnCheckedChanged="ValidateChange" AutoPostBack="true" fID="OverrideRegisterButton" />
                    </div>
                </asp:Panel>
                <asp:Panel ID="pFAQ" runat="server" Enabled="false" BorderStyle="Solid" BorderColor="Gray" Width="400px" style="padding: 10px;margin: 5px auto 20px auto;">
                    <div>
                        <asp:Label ID="lblEmployerFAQCategory" runat="server" Text="Edit Employer Specific FAQ Category: " AssociatedControlID="ddlEmployerFAQCategory" />
                        <asp:DropDownList ID="ddlEmployerFAQCategory" runat="server" DataValueField="FAQID" 
                            DataTextField="Title" AutoPostBack="True" 
                            onselectedindexchanged="ddlEmployerFAQCategory_SelectedIndexChanged" />
                        <br>
                        </br>
                    </div>
                    <div>
                        <asp:Label ID="lblContent1" runat="server" Text="Content 1: " AssociatedControlID="txtContent1" style="vertical-align:top" />&nbsp;&nbsp;
                        <asp:TextBox ID="txtContent1" runat="server" Height="40px" MaxLength="255" Width="300px" TextMode="MultiLine" OnTextChanged="ValidateFAQChange" AutoPostBack="true"></asp:TextBox>
                        <asp:CheckBox ID="chkNeedUserInput1" runat="server" Text="Need Additional User Input: " OnCheckedChanged="ValidateFAQChange" AutoPostBack="true" />
                        <br></br>
                    </div>
                    <div>
                        <asp:Label ID="lblContent2" runat="server" Text="Content 2: " AssociatedControlID="txtContent2" style="vertical-align:top" />&nbsp;&nbsp;
                        <asp:TextBox ID="txtContent2" runat="server" Height="40px" MaxLength="255" Width="300px" TextMode="MultiLine" OnTextChanged="ValidateFAQChange" AutoPostBack="true"></asp:TextBox>
                        <asp:CheckBox ID="chkNeedUserInput2" runat="server" Text="Need Additional User Input: " OnCheckedChanged="ValidateFAQChange" AutoPostBack="true" />
                        <br></br>
                    </div>
                    <div>
                        <asp:Label ID="lblContent3" runat="server" Text="Content 3: " AssociatedControlID="txtContent3" style="vertical-align:top" />&nbsp;&nbsp;
                        <asp:TextBox ID="txtContent3" runat="server" Height="40px" MaxLength="255" Width="300px" TextMode="MultiLine" OnTextChanged="ValidateFAQChange" AutoPostBack="true"></asp:TextBox>
                        <asp:CheckBox ID="chkNeedUserInput3" runat="server" Text="Need Additional User Input: " OnCheckedChanged="ValidateFAQChange" AutoPostBack="true" />
                        <br></br>
                    </div>
                    <div>
                        <asp:Label ID="lblContent4" runat="server" Text="Content 4: " AssociatedControlID="txtContent4" style="vertical-align:top" />&nbsp;&nbsp;
                        <asp:TextBox ID="txtContent4" runat="server" Height="40px" MaxLength="255" Width="300px" TextMode="MultiLine" OnTextChanged="ValidateFAQChange" AutoPostBack="true"></asp:TextBox>
                        <asp:CheckBox ID="chkNeedUserInput4" runat="server" Text="Need Additional User Input: " OnCheckedChanged="ValidateFAQChange" AutoPostBack="true" />
                        <br></br>
                    </div>
                    <div>
                        <asp:Label ID="lblContent5" runat="server" Text="Content 5: " AssociatedControlID="txtContent5" style="vertical-align:top" />&nbsp;&nbsp;
                        <asp:TextBox ID="txtContent5" runat="server" Height="40px" MaxLength="255" Width="300px" TextMode="MultiLine" OnTextChanged="ValidateFAQChange" AutoPostBack="true"></asp:TextBox>
                        <asp:CheckBox ID="chkNeedUserInput5" runat="server" Text="Need Additional User Input: " OnCheckedChanged="ValidateFAQChange" AutoPostBack="true" />
                        <br></br>
                    </div>
                    <div>
                        <asp:Label ID="lblContent6" runat="server" Text="Content 6: " AssociatedControlID="txtContent6" style="vertical-align:top" />&nbsp;&nbsp;
                        <asp:TextBox ID="txtContent6" runat="server" Height="40px" MaxLength="255" Width="300px" TextMode="MultiLine" OnTextChanged="ValidateFAQChange" AutoPostBack="true"></asp:TextBox>
                        <asp:CheckBox ID="chkNeedUserInput6" runat="server" Text="Need Additional User Input: " OnCheckedChanged="ValidateFAQChange" AutoPostBack="true" />
                        <br></br>
                    </div>
                    <div>
                        <asp:Label ID="lblContent7" runat="server" Text="Content 7: " AssociatedControlID="txtContent7" style="vertical-align:top" />&nbsp;&nbsp;
                        <asp:TextBox ID="txtContent7" runat="server" Height="40px" MaxLength="255" Width="300px" TextMode="MultiLine" OnTextChanged="ValidateFAQChange" AutoPostBack="true"></asp:TextBox>
                        <asp:CheckBox ID="chkNeedUserInput7" runat="server" Text="Need Additional User Input: " OnCheckedChanged="ValidateFAQChange" AutoPostBack="true" />
                        <br></br>
                    </div>
                    <div>
                        <asp:Label ID="lblContent8" runat="server" Text="Content 8: " AssociatedControlID="txtContent8" style="vertical-align:top" />&nbsp;&nbsp;
                        <asp:TextBox ID="txtContent8" runat="server" Height="40px" MaxLength="255" Width="300px" TextMode="MultiLine" OnTextChanged="ValidateFAQChange" AutoPostBack="true"></asp:TextBox>
                        <asp:CheckBox ID="chkNeedUserInput8" runat="server" Text="Need Additional User Input: " OnCheckedChanged="ValidateFAQChange" AutoPostBack="true" />
                        <br></br>
                    </div>
                    <div>
                        <asp:Label ID="lblContent9" runat="server" Text="Content 9: " AssociatedControlID="txtContent9" style="vertical-align:top" />&nbsp;&nbsp;
                        <asp:TextBox ID="txtContent9" runat="server" Height="40px" MaxLength="255" Width="300px" TextMode="MultiLine" OnTextChanged="ValidateFAQChange" AutoPostBack="true"></asp:TextBox>
                        <asp:CheckBox ID="chkNeedUserInput9" runat="server" Text="Need Additional User Input: " OnCheckedChanged="ValidateFAQChange" AutoPostBack="true" />
                        <br></br>
                    </div>
                    <div>
                        <asp:Label ID="lblContent10" runat="server" Text="Content 10: " AssociatedControlID="txtContent10" style="vertical-align:top" />&nbsp;
                        <asp:TextBox ID="txtContent10" runat="server" Height="40px" MaxLength="255" Width="300px" TextMode="MultiLine" OnTextChanged="ValidateFAQChange" AutoPostBack="true"></asp:TextBox>
                        <asp:CheckBox ID="chkNeedUserInput10" runat="server" Text="Need Additional User Input: " OnCheckedChanged="ValidateFAQChange" AutoPostBack="true" />
                        <br></br>
                    </div>
                </asp:Panel>
                <asp:Panel ID="SCIQInterim" runat="server" Enabled="false" BorderStyle="Solid" BorderColor="Gray" Width="400px" style="padding: 10px;margin: 5px auto 20px auto;">                    
                    <h3>SCIQ Interim Page <em style="font-size:.8em; font-weight:normal;"> - only applies when "Savings Choice Enabled" - </em></h3>
                    <div>
                        <asp:Label ID="Label38" runat="server" Text="Starting Text: " AssociatedControlID="txtSciqStartText" />&nbsp;&nbsp;
                        <asp:TextBox ID="txtSciqStartText" runat="server" Height="40px" MaxLength="255" Width="300px" TextMode="MultiLine" OnTextChanged="ValidateChange" AutoPostBack="true" fID="SCIQStartText"></asp:TextBox>
                        <br></br>
                    </div>                    
                </asp:Panel>
                <div align="right" style="width:400px;margin:0px auto;">
                    <div align="left" class="feedback" style="display:inline-block;float:left;max-width:345px;"></div>
                    <asp:Button ID="btnSave" runat="server" Text="Save" Enabled="false" OnClick="SaveChanges" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>        
    </div>
</asp:Content>