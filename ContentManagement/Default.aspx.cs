using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.IO;

namespace ClearCostWeb.ContentManagement
{
    public partial class Default : System.Web.UI.Page
    {
        private Dictionary<String, object> emptyChanges
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    {"employerid",null},
                    {"EmployerName",null},
                    {"Insurer",null},
                    {"RXProvider",null},
                    {"ShowYourCostColumn",null},
                    {"ContentManagementEnabled",null},
                    {"SSNOnly",null},
                    {"InsurerName",null},
                    {"LogoImageName",null},
                    {"HasOtherPeopleSection",null},
                    {"HasNotificationSection",null},
                    {"TandCVisible",null},
                    {"PhoneNumber",null},
                    {"CanSignIn",null},
                    {"CanRegister",null},
                    {"InternalLogo",null},
                    {"MemberIDFormat",null},
                    {"ContactText",null},
                    {"SpecialtyNetworkText",null},
                    {"PastCareDisclaimerText",null},  //  lam, 20130418, MSF-299
                    {"RxResultDisclaimerText",null},  //  lam, 20130418, MSF-294
                    {"AllResult1DisclaimerText",null},  //  lam, 20130425, MSF-295
                    {"AllResult2DisclaimerText",null},  //  lam, 20130425, MSF-295
                    {"SpecialtyDrugDisclaimerText",null},  //  lam, 20130418, CI-59
                    {"MentalHealthDisclaimerText",null},  //  lam, 20130508, CI-144
                    {"ServiceNotFoundDisclaimerText",null},  //  lam, 20130604, MSF-377
                    {"OverrideRegisterButton",null},
                    {"DefaultYourCostOn",null},
                    {"DefaultSort",null},
                    {"AllowFairPriceSort", null}, //  lam, 20130618, MSB-324
                    {"SavingsChoiceEnabled", null}, // JRM, 7/10/13, Added ability to turn SC on or off
                    {"ShowSCIQTab", null},  //  lam, 20130816, SCIQ-77
                    {"SCIQStartText", null}
                };
            }
        }
        private static Dictionary<String, String> DefaultSorts = new Dictionary<String, String>() { 
            {"rbDistance","Distance"},
            {"rbTotalCost","TotalCost"},
            {"rbFairPrice","FairPrice"}
        };
        private String feedback = String.Empty;
        private DataTable content
        {
            get
            {
                return (DataTable)this.ViewState["ContentData"];
            }
            set
            {
                this.ViewState["ContentData"] = value;
            }
        }
        private Dictionary<String, object> Changes
        {
            get
            {
                return (Dictionary<String, object>)this.ViewState["Changes"];
            }
            set
            {
                this.ViewState["Changes"] = value;
            }
        }
        private Boolean ContentChanged { get { return (Changes.Any(chg => chg.Value != null) || FAQChanges); } }  //  lam, 20130411, MSF-290

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Changes = emptyChanges;
                FAQChanges = false;  //  lam, 20130411, MSF-290

                Page.Header.Controls.Add(
                    new Literal()
                    {
                        Text = @"<style type='text/css'>
        input[type='text'] {padding:1px 5px;width:190px;border-width:1px;}
        input[type='text']:enabled  {background-color:rgb(223, 200, 223);border:1px solid purple;}
        input[type='text']:enabled:focus {border-color:Yellow;}
        textarea {border-width:1px;padding:2px 5px;}
        textarea:enabled {background-color:rgb(223,200,223);border:1px solid purple;}
        textarea:enabled:focus {border-color:Yellow;}
        .StaticContent {font-weight:bold;color:Purple;cursor:default;}
    </style>"
                    });

                SetContent();

                if (content != null)
                {
                    ddlEmployers.DataBind(from eT in content.AsEnumerable()
                                          select new
                                          {
                                              EmployerID = eT.Field<Int32>("EmployerID"),
                                              EmployerName = eT.Field<String>("EmployerName")
                                          });
                    ddlEmployers.Items.Insert(0, new ListItem() { Text = "Select Employer", Value = "0", Selected = true });

                    SetFAQCategory();  //  lam, 20130411, MSF-290
                }
            }
        }
        protected void selectEmployer(object sender, EventArgs e)
        {
            if (Convert.ToInt32(ddlEmployers.SelectedValue) > 0)
            {
                cbEnabled.Enabled = true;
                dynamic dynContent = (from ec in content.AsEnumerable()
                                      where ec.Field<Int32>("EmployerID") == Convert.ToInt32(ddlEmployers.SelectedValue)
                                      select ec).First().ToDyn(emptyChanges);

                //cbEnabled.Checked = pContentWrapper.Enabled = (dynContent.ContentManagementEnabled == 1 ? true : false);  //  lam, 20130411, MSF-290, block
                cbEnabled.Checked = pContentWrapper.Enabled = pFAQ.Enabled = SCIQInterim.Enabled = (dynContent.ContentManagementEnabled == 1 ? true : false);  //  lam, 20130411, MSF-290

                lblEmployer.Text = dynContent.EmployerName;
                lblInsurer.Text = dynContent.Insurer;
                lblRXProvider.Text = dynContent.RXProvider;
                cbYCEnabled.Checked = dynContent.ShowYourCostColumn;

                if ((dynContent.ContentManagementEnabled == 1 ? true : false))
                {
                    rbFairPrice.Enabled = dynContent.AllowFairPriceSort;  //  lam, 20130613, MSB-324
                    
                    cbSSNOnly.Checked = dynContent.SSNOnly;
                    txtInsurerName.Text = dynContent.InsurerName;
                    txtLogoFilename.Text = dynContent.LogoImageName;
                    cbOtherPeople.Checked = dynContent.HasOtherPeopleSection;
                    cbNotifications.Checked = dynContent.HasNotificationSection;
                    cbTandC.Checked = dynContent.TandCVisible;
                    txtPhone.Text = dynContent.PhoneNumber;
                    cbSignIn.Checked = dynContent.CanSignIn;
                    cbRegister.Checked = dynContent.CanRegister;
                    cbInternalLogo.Checked = dynContent.InternalLogo;
                    txtMemberIDFormat.Text = dynContent.MemberIDFormat;
                    txtContactText.Text = dynContent.ContactText;
                    txtSpecialtyNetworkText.Text = dynContent.SpecialtyNetworkText;
                    txtPastCareDisclaimerText.Text = dynContent.PastCareDisclaimerText;  //  lam, 20130418, MSF-299
                    txtRxResultDisclaimerText.Text = dynContent.RxResultDisclaimerText;  //  lam, 20130418, MSF-294
                    txtAllResult1DisclaimerText.Text = dynContent.AllResult1DisclaimerText;  //  lam, 20130425, MSF-295
                    txtAllResult2DisclaimerText.Text = dynContent.AllResult2DisclaimerText;  //  lam, 20130425, MSF-295
                    txtSpecialtyDrugDisclaimerText.Text = dynContent.RxResultDisclaimerText;  //  lam, 20130418, CI-59
                    txtMentalHealthDisclaimerText.Text = dynContent.MentalHealthDisclaimerText;  //  lam, 20130508, CI-144
                    txtServiceNotFoundDisclaimerText.Text = dynContent.ServiceNotFoundDisclaimerText;  //  lam, 20130604, MSF-377
                    cbSignInCentric.Checked = dynContent.OverrideRegisterButton;
                    cbYCOn.Checked = dynContent.DefaultYourCostOn;
                    cbSCEnabled.Checked = dynContent.SavingsChoiceEnabled;
                    cbShowSCIQTab.Checked = dynContent.ShowSCIQTab;  //  lam, 20130816, SCIQ-77

                    rbTotalCost.Checked = (dynContent.DefaultSort == "TotalCost");
                    rbDistance.Checked = (dynContent.DefaultSort == "Distance");
                    rbFairPrice.Checked = (dynContent.DefaultSort == "FairPrice");

                    ddlEmployerFAQCategory.Enabled = true;  //  lam, 20130411, MSF-290
                    SetFAQContent(Convert.ToInt32(ddlEmployers.SelectedValue, 10), ddlEmployerFAQCategory.SelectedValue);  //  lam, 20130411, MSF-290

                    txtSciqStartText.Text = dynContent.SCIQStartText;
                }
                else
                {
                    cbSSNOnly.Checked =
                        cbOtherPeople.Checked =
                        cbNotifications.Checked =
                        cbTandC.Checked =
                        cbSignIn.Checked =
                        cbRegister.Checked =
                        cbInternalLogo.Checked =
                        cbSignInCentric.Checked =
                        cbYCOn.Checked =
                        cbSCEnabled.Checked =
                        pnlDefaultSort.Enabled =
                            false;
                    txtInsurerName.Text =
                        txtLogoFilename.Text =
                        txtPhone.Text =
                        txtMemberIDFormat.Text =
                        txtContactText.Text =
                        txtSpecialtyNetworkText.Text =
                        txtPastCareDisclaimerText.Text =  //  lam, 20130418, MSF-299
                        txtRxResultDisclaimerText.Text =  //  lam, 20130418, MSF-294
                        txtAllResult1DisclaimerText.Text =  //  lam, 20130425, MSF-295
                        txtAllResult2DisclaimerText.Text =  //  lam, 20130425, MSF-295
                        txtSpecialtyDrugDisclaimerText.Text =  //  lam, 20130418, CI-59
                        txtMentalHealthDisclaimerText.Text =  //  lam, 20130508, CI-144
                        txtServiceNotFoundDisclaimerText.Text =  //  lam, 20130604, MSF-377
                        txtSciqStartText.Text =
                            String.Empty;

                    ddlEmployerFAQCategory.Enabled = false;  //  lam, 20130411, MSF-290
                    ClearFAQContent();  //  lam, 20130411, MSF-290
                }
            }
            else
            {
                pContentWrapper.Enabled = pFAQ.Enabled = false;
                cbEnabled.Enabled = false;
                ClearUI();
                lblEmployer.Text =
                    lblInsurer.Text =
                    lblRXProvider.Text =
                        String.Empty;
                cbYCEnabled.Checked =
                    cbEnabled.Checked =
                        false;

                ddlEmployerFAQCategory.Enabled = false;  //  lam, 20130411, MSF-290
                ClearFAQContent();  //  lam, 20130411, MSF-290
            }
            btnSave.Enabled = false;
            Changes = emptyChanges;
            FAQChanges = false;  //  lam, 20130411, MSF-290
            ddlEmployers.Focus();
        }
        protected void cbEnabled_CheckedChanged(object sender, EventArgs e)
        {
            ValidateChange(sender, e);
            cbSSNOnly.Checked = cbTandC.Checked = cbEnabled.Checked;
            cbOtherPeople.Checked =
                cbNotifications.Checked =
                cbSignIn.Checked =
                cbRegister.Checked =
                cbInternalLogo.Checked =
                cbSignInCentric.Checked =
                cbYCOn.Checked =
                cbSCEnabled.Checked =
                pnlDefaultSort.Enabled =
                    (!cbEnabled.Checked);

            //  lam, 20130411, MSF-290
            if (cbEnabled.Checked)
                SetFAQContent(Convert.ToInt32(ddlEmployers.SelectedValue, 10), ddlEmployerFAQCategory.SelectedValue);
            else
                ClearFAQContent();
            pContentWrapper.Enabled = pFAQ.Enabled = ddlEmployerFAQCategory.Enabled = cbEnabled.Checked;
            //pContentWrapper.Enabled = cbEnabled.Checked;  //  lam, 20130411, MSF-290, block
            //  -----------------------
        }
        protected void ValidateChange(object sender, EventArgs e)
        {
            WebControl wc = (WebControl)sender;
            String fKey = wc.Attributes["fID"].ToString();
            dynamic fVal = (from ec in content.AsEnumerable()
                            where ec.Field<Int32>("EmployerID") == Convert.ToInt32(ddlEmployers.SelectedValue)
                            select new { oldValue = ec.Field<dynamic>(wc.Attributes["fID"].ToString()) }).First();

            //If they're switching content management we need to convert 1's and 0's to Boolean
            if (fKey == "ContentManagementEnabled") fVal = new { oldValue = (fVal.oldValue == 1 ? true : false) };

            if (wc.GetType() == typeof(CheckBox))
            {
                CheckBox cb = (CheckBox)wc;
                Changes[fKey] = (cb.Checked == fVal.oldValue ? null : (object)cb.Checked);
            }
            else if (wc.GetType() == typeof(TextBox))
            {
                TextBox tb = (TextBox)wc;
                Changes[fKey] = (tb.Text == fVal.oldValue ? null : (object)tb.Text);
            }
            else if (wc.GetType() == typeof(RadioButton))
            {
                Changes[fKey] = DefaultSorts[wc.ID];
            }

            btnSave.Enabled = ContentChanged;
        }
        protected void SaveChanges(object sender, EventArgs e)
        {
            if (ContentChanged)
            {
                if (Changes["ContentManagementEnabled"] != null) //If they've enabled or disabled content management
                {
                    if (((Boolean)Changes["ContentManagementEnabled"])) //If Adding Content Management
                    {
                        EnableCM();
                        Changes["ContentManagementEnabled"] = null;
                        SaveContent();
                        SaveFAQContent();  //  lam, 20130411, MSF-290
                    }
                    else //If Removing Content Management
                    {
                        DisableCM();
                        ClearUI();
                        //pContentWrapper.Enabled = false;  //  lam, 20130411, MSF-290, block
                        pContentWrapper.Enabled = pFAQ.Enabled = false;  //  lam, 20130411, MSF-290
                    }
                }
                else
                {
                    SaveContent();
                    SaveFAQContent();  //  lam, 20130411, MSF-290
                }

                SetContent();
                SetFAQContent(Convert.ToInt32(ddlEmployers.SelectedValue, 10), ddlEmployerFAQCategory.SelectedValue);  //  lam, 20130411, MSF-290
                btnSave.Enabled = false;

                if (feedback != String.Empty)
                {
                    ScriptManager.RegisterStartupScript(this,
                        this.GetType(),
                        "Feedback",
                        String.Concat(
                            "$('<span>').text('",
                            feedback,
                            "!!!').css({ 'fontWeight': 'bolder', 'color': 'rgb(0,130,0)' }).append('<br>').appendTo('.feedback').delay(",
                            feedback.Length * 40,
                            ").fadeOut(1000);"),
                        true);
                }
            }
        }

        private void SetContent()
        {
            String cmQuery =
                "select " +
                String.Concat<String>(
                    emptyChanges
                        .Keys
                        .Where<String>(key =>
                            key == "employerid" ||
                            key == "EmployerName" ||
                            key == "Insurer" ||
                            key == "RXProvider" ||
                            key == "ShowYourCostColumn")
                        .Select<String, String>(key =>
                            "e." + key + ", ")) +
                String.Concat<String>(
                    emptyChanges
                        .Keys
                        .Where(k =>
                            k != "ContentManagementEnabled" &&
                            k != "employerid" &&
                            k != "EmployerName" &&
                            k != "Insurer" &&
                            k != "RXProvider" &&
                            k != "ShowYourCostColumn")
                        .Select<String, String>(k =>
                            "ec." + k + ", ")) +
                "CASE WHEN ec.cmid is null THEN 0 ELSE 1 END as ContentManagementEnabled " +
                    "from employers e " +
                    "left join employercontent ec on e.contentid = ec.cmid";

            using (BaseCCHData b = new BaseCCHData(cmQuery, true))
            {
                b.GetFrontEndData();

                if(b.Tables.Count > 0)
                    this.content = b.Tables[0];
            }
        }
        private void EnableCM()
        {
            String newID = Guid.NewGuid().ToString();
            String qAddCMEntry = String.Concat(
                "INSERT INTO [EmployerContent] (CMID, SSNOnly, HasOtherPeopleSection, HasNotificationSection, TandCVisible, CanSignIn, Canregister, InternalLogo, MemberIDFormat, ContactText, SpecialtyNetworkText, PastCareDisclaimerText, RxResultDisclaimerText, SpecialtyDrugDisclaimerText, OverrideRegisterButton, SCIQInMonth, SCIQUserEarnings, SCIQButtonText, SCIQDashboardName) ",  //  lam, 20130418, MSF-299
                "VALUES ('", newID, "', 1, 0, 0, 1, 0, 0, 0, '','','','','','',0,'','','','')");
            String qUpdateEmpEntry = String.Concat(
                "UPDATE [Employers] SET ContentID = '", newID, "' WHERE EmployerID = ", ddlEmployers.SelectedValue);

            try
            {
                using (BaseCCHData b = new BaseCCHData(qAddCMEntry, true))
                {
                    b.PostFrontEndData();
                    if (b.HasErrors) throw new Exception("Error adding new entry to content management table");
                }
                using (BaseCCHData b = new BaseCCHData(qUpdateEmpEntry, true))
                {
                    b.PostFrontEndData();
                    if (b.HasErrors) throw new Exception("Error updating employers with new content");
                }
                this.feedback = feedback.Append(
                    String.Concat("Content Management Added to ",
                        ddlEmployers.SelectedItem.Text,
                        " Successfully"));
            }
            catch (Exception ex)
            {
            }
        }
        private void DisableCM()
        {
            String oldID = String.Empty;
            String qUpdateEmpEntry = String.Concat(
                "UPDATE [Employers] SET ContentID = NULL OUTPUT Deleted.ContentID WHERE EmployerID = ", ddlEmployers.SelectedValue);
            String qRemCMEntry = "DELETE FROM [EmployerContent] WHERE CMID = '{0}'";

            try
            {
                using (BaseCCHData b = new BaseCCHData(qUpdateEmpEntry, true))
                {
                    b.GetFrontEndData();
                    if (b.HasErrors) throw new Exception("Error  unlinking content from employer");
                    oldID = b.Tables[0].Rows[0][0].ToString();
                }
                if (oldID == String.Empty) throw new Exception("Could not find previous content ID");
                using (BaseCCHData b = new BaseCCHData(String.Format(qRemCMEntry, oldID), true))
                {
                    b.PostFrontEndData();
                    if (b.HasErrors) throw new Exception("Error removing old content from table");
                }
                this.feedback = feedback.Append(
                    String.Concat(
                        "Content Management Removed from ",
                        ddlEmployers.SelectedItem.Text,
                        " Successfully"));
            }
            catch (Exception ex)
            {
            }
        }
        private void SaveContent()
        {
            if (!ContentChanged) return;

            String cID = String.Empty;
            String sQuery = String.Concat("SELECT ContentID FROM [Employers] WHERE EmployerID = ", ddlEmployers.SelectedValue);
            String uQuery = "UPDATE [EmployerContent] SET ";
            try
            {
                using (BaseCCHData b = new BaseCCHData(sQuery, true))
                {
                    b.GetFrontEndData();
                    if (b.HasErrors) throw new Exception("Error finding Content in database");
                    cID = b.Tables[0].Rows[0][0].ToString();
                }

                List<KeyValuePair<String, object>> l = Changes.Where(kvp => kvp.Value != null).ToList<KeyValuePair<String, object>>();
                l.ForEach(delegate(KeyValuePair<String, object> kvp)
                {
                    uQuery = String.Concat(uQuery, kvp.Key, " = '", kvp.Value, "'");
                    if (kvp.Key != l.Last().Key) uQuery = String.Concat(uQuery, ", ");
                });
                if (cID == String.Empty) throw new Exception("Could not find content ID in the table");
                uQuery = String.Concat(uQuery, " WHERE CMID = '", cID, "'");
                using (BaseCCHData b = new BaseCCHData(uQuery, true))
                {
                    b.PostFrontEndData();
                    if (b.HasErrors) throw new Exception("Error updating content with changes");
                }
                this.feedback = feedback.Append("Changes Saved Successfully");
            }
            catch (Exception ex)
            {
            }
        }
        private void ClearUI()
        {
            cbSSNOnly.Checked =
                cbOtherPeople.Checked =
                cbNotifications.Checked =
                cbTandC.Checked =
                cbInternalLogo.Checked =
                cbSignIn.Checked =
                cbRegister.Checked =
                cbSignInCentric.Checked =
                cbYCOn.Checked =
                cbSCEnabled.Checked = 
                pnlDefaultSort.Enabled =
                    false;

            txtInsurerName.Text =
                txtLogoFilename.Text =
                txtPhone.Text =
                txtMemberIDFormat.Text =
                txtContactText.Text =
                txtSpecialtyNetworkText.Text =
                txtPastCareDisclaimerText.Text =  //  lam, 20130418, MSF-299
                txtRxResultDisclaimerText.Text =  //  lam, 20130418, MSF-294
                txtAllResult1DisclaimerText.Text =  //  lam, 20130425, MSF-295
                txtAllResult2DisclaimerText.Text =  //  lam, 20130425, MSF-295
                txtSpecialtyDrugDisclaimerText.Text =  //  lam, 20130418, CI-59
                txtMentalHealthDisclaimerText.Text =  //  lam, 20130508, CI-144
                txtServiceNotFoundDisclaimerText.Text =  //  lam, 20130604, MSF-377
                txtSciqStartText.Text =
                    String.Empty;
        }

        //protected class ContentControl : Control
        //{
        //    private Label myLabel = null;
        //    private CheckBox myCheckBox = null;
        //    private TextBox myTextBox = null;

        //    public enum ControlTypes { Static = 0, CheckBox = 1, TextBox = 2 }

        //    public override string ID 
        //    { 
        //        get { return base.ID; } 
        //        set 
        //        { 
        //            myTextBox.ID += value; 
        //            myTextBox.Attributes["onchange"] = @"javascript:setTimeout('__doPostBack(\'" + myTextBox.ID + @"\',\'\')', 0)"; 
        //            myCheckBox.ID += value; 
        //            myLabel.ID += value;                 
        //            base.ID = value; 
        //        } 
        //    }
        //    public String LabelText { get; set; }
        //    public String ControlText { set { myLabel.Text = value; myTextBox.Text = value; myCheckBox.Text = value; } }
        //    public String ControlField { set { myTextBox.Attributes["fID"] = value; myCheckBox.Attributes["fID"] = value; } }
        //    public ControlTypes ControlType { get; set; }

        //    protected override void Render(HtmlTextWriter writer)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        StringWriter sw = new StringWriter(sb);
        //        using(HtmlTextWriter htw = new HtmlTextWriter(sw))
        //        {
        //            htw.RenderBeginTag(HtmlTextWriterTag.Div);

        //            switch (ControlType)
        //            {
        //                case ControlTypes.Static:
        //                    htw.AddAttribute(HtmlTextWriterAttribute.For, myLabel.ID);
        //                    htw.RenderBeginTag(HtmlTextWriterTag.Label);
        //                    htw.Write(LabelText);
        //                    htw.RenderEndTag();
        //                    myLabel.RenderControl(htw);
        //                    htw.RenderEndTag();
        //                    break;
        //                case ControlTypes.CheckBox:
        //                    htw.AddAttribute(HtmlTextWriterAttribute.For, myCheckBox.ID);
        //                    htw.RenderBeginTag(HtmlTextWriterTag.Label);
        //                    htw.Write(LabelText);
        //                    htw.RenderEndTag();
        //                    myCheckBox.RenderControl(htw);
        //                    htw.RenderEndTag();
        //                    break;
        //                case ControlTypes.TextBox:
        //                    htw.AddAttribute(HtmlTextWriterAttribute.For, myTextBox.ID);
        //                    htw.RenderBeginTag(HtmlTextWriterTag.Label);
        //                    htw.Write(LabelText);
        //                    htw.RenderEndTag();
        //                    myTextBox.RenderControl(htw);
        //                    htw.RenderEndTag();
        //                    break;
        //            }
        //        }
        //        writer.Write(sb.ToString());
        //    }
        //    public ContentControl()
        //    {
        //        myTextBox = new TextBox() 
        //        { 
        //            AutoPostBack = true,
        //            AutoCompleteType = System.Web.UI.WebControls.AutoCompleteType.Disabled,
        //            ID = "txt" + this.ID
        //        };
        //        myTextBox.Style.Add(HtmlTextWriterStyle.Cursor, "default");
        //        myTextBox.Attributes["onkeypress"] = "if (WebForm_TextBoxKeyHandler(event) == false) return false;";
        //        myTextBox.TextChanged += new EventHandler(ValidateChange);
        //        myLabel = new Label()
        //        {
        //            ID = "lbl" + this.ID,
        //            CssClass = "StaticContent",
        //            ToolTip = "*Property is Read-Only but displayed to better identify the employer you're working with.",
        //            Enabled = false
        //        };
        //        myCheckBox = new CheckBox()
        //        {
        //            ID = "cb" + this.ID,
        //            AutoPostBack = true,
        //            Enabled = false
        //        };
        //        myCheckBox.CheckedChanged += new EventHandler(ValidateChange);
        //    }
        //    private void ValidateChange(object sender, EventArgs e)
        //    {
        //        String test = "TEST";
        //    }
        //}

        private List<FAQContent> contentFAQ  //  lam, 20130411, MSF-290
        {
            get
            {
                return (List<FAQContent>)this.ViewState["ContentDataFAQ"];
            }
            set
            {
                this.ViewState["ContentDataFAQ"] = value;
            }
        }

        private bool FAQChanges  //  lam, 20130411, MSF-290
        {
            get
            {
                return (bool)this.ViewState["FAQChanges"];
            }
            set
            {
                this.ViewState["FAQChanges"] = value;
            }
        }

        protected void ClearFAQContent()  //  lam, 20130411, MSF-290
        {
            txtContent1.Text = txtContent2.Text = txtContent3.Text = txtContent4.Text = txtContent5.Text = txtContent6.Text = txtContent7.Text = txtContent8.Text = txtContent9.Text = txtContent10.Text = "";
            chkNeedUserInput1.Checked = chkNeedUserInput2.Checked = chkNeedUserInput3.Checked = chkNeedUserInput4.Checked = chkNeedUserInput5.Checked = chkNeedUserInput6.Checked = chkNeedUserInput7.Checked = chkNeedUserInput8.Checked = chkNeedUserInput9.Checked = chkNeedUserInput10.Checked = false;
            FAQChanges = false;
        }

        protected void SetFAQCategory()  //  lam, 20130411, MSF-290
        {
            using (GetEmployerFAQCategory gefc = new GetEmployerFAQCategory())
            {
                gefc.GetFrontEndData();
                if (gefc.Tables.Count > 0)
                {
                    ddlEmployerFAQCategory.DataSource = gefc.Tables[0];
                    ddlEmployerFAQCategory.DataBind();
                }
            }
        }

        protected void SetFAQContent(Int32 EmployerID, String CategoryID)  //  lam, 20130411, MSF-290
        {
            List<FAQContent> currentFAQContent = new List<FAQContent>();
            ClearFAQContent();
            using (GetEmployerFAQContent gefc = new GetEmployerFAQContent(EmployerID, CategoryID))
            {
                gefc.GetFrontEndData();
                if (gefc.FAQContentList.Count > 0)
                {
                    List<FAQContent> l = gefc.FAQContentList;
                    for (Int32 i = 0; i < l.Count; i++)
                    {
                        FAQContent item = new FAQContent();
                        item.FAQText = l[i].FAQText;
                        item.NeedUserInput = l[i].NeedUserInput;
                        currentFAQContent.Add(item);

                        ((TextBox)pFAQ.FindControl("txtContent" + (i + 1).ToString())).Text = l[i].FAQText;
                        ((CheckBox)pFAQ.FindControl("chkNeedUserInput" + (i + 1).ToString())).Checked = l[i].NeedUserInput;
                    }
                }
            }
            while (currentFAQContent.Count < 10)
                currentFAQContent.Add(new FAQContent() {FAQText = "", NeedUserInput = false});

            this.contentFAQ = currentFAQContent;
            FAQChanges = false;
        }

        protected void ddlEmployerFAQCategory_SelectedIndexChanged(object sender, EventArgs e)  //  lam, 20130411, MSF-290
        {
            SetFAQContent(Convert.ToInt32(ddlEmployers.SelectedValue, 10), ddlEmployerFAQCategory.SelectedValue);
        }

        private void SaveFAQContent()  //  lam, 20130411, MSF-290
        {
            List<FAQContent> l = new List<FAQContent>();
            for (Int32 i = 0; i < 10; i++)
            {
                if (((TextBox)pFAQ.FindControl("txtContent" + (i + 1).ToString())).Text.Trim() != "")
                    l.Add(new FAQContent { EmployerID = Convert.ToInt32(ddlEmployers.SelectedValue), FAQID = ddlEmployerFAQCategory.SelectedValue, FAQText = ((TextBox)pFAQ.FindControl("txtContent" + (i + 1).ToString())).Text.Trim(), NeedUserInput = ((CheckBox)pFAQ.FindControl("chkNeedUserInput" + (i + 1).ToString())).Checked });
            }
            EmployerFAQContent uefc = new EmployerFAQContent();
            uefc.UpdateEmployerFAQContent(Convert.ToInt32(ddlEmployers.SelectedValue), ddlEmployerFAQCategory.SelectedValue, l);
            while (l.Count < 10)
                l.Add(new FAQContent { FAQText = "", NeedUserInput = false } );
            contentFAQ = l;
            FAQChanges = false;
        }

        protected void ValidateFAQChange(object sender, EventArgs e)  //  lam, 20130411, MSF-290
        {
            FAQChanges = false;
            List<FAQContent> l = contentFAQ;
            for (int i = 0; i < l.Count; i++)
            {
                if (((TextBox)pFAQ.FindControl("txtContent" + (i + 1).ToString())).Text.Trim() != l[i].FAQText.Trim() || ((CheckBox)pFAQ.FindControl("chkNeedUserInput" + (i + 1).ToString())).Checked != l[i].NeedUserInput)
                    FAQChanges = true;
            }
            btnSave.Enabled = ContentChanged;
        }
    }
}
