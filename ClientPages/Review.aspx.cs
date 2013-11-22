using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using Microsoft.Security.Application;

namespace ClearCostWeb.ClientPages
{
    public partial class Review : System.Web.UI.Page
    {
        private Boolean NotificationsVisible
        {
            get { return Boolean.Parse(ViewState["NotificationsVisible"].ToString()); }
            set { ViewState["NotificationsVisible"] = value; }
        }
        private Boolean OtherMembersVisible
        {
            get { return Boolean.Parse(ViewState["OtherPeopleVisible"] as String); }
            set { ViewState["OtherPeopleVisible"] = value; }
        }

        private String CssStyleSheets
        {
            get
            {
                String template = @"<link href=""{0}"" rel=""Stylesheet"" type=""text/css"" /><link href=""{1}"" rel=""Stylesheet"" type=""text/css"" />";

                return String.Format(template,
                    ResolveUrl("~/Styles/skin.css") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    ResolveUrl("~/Styles/old/style.css") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }
        }
        private String MobileStyles
        {
            get
            {
                String template = @"<link href=""{0}"" rel=""Stylesheet"" type=""text/css"" media=""screen"" />";
                return String.Format(template,
                    ResolveUrl("~/Styles/cch-mobile.css?Rev=") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }
        }
        private String LandingStyles
        {
            get
            {
                String template = @"<link href=""{0}"" rel=""stylesheet"" type=""text/css"" />";
                return String.Format(template,
                    ResolveUrl("~/Styles/landing.css?Rev=") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            //VERY IMPORT FOR CHANGE CONTROL
            //Adds the compile version to the end of any links to css or javascript files forcing the browser to get the latest version upon first visit
            foreach (var ctrl in Page.Header.Controls)
                if (ctrl is HtmlLink) //Updates all .css files with .css?Rev=<VERSION>
                    ((HtmlLink)ctrl).Href = String.Concat(
                        ((HtmlLink)ctrl).Href,
                        "?Rev=",
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                else if (ctrl is LiteralControl)//Updates all .js files with .js?Rev=<VERSION>
                    ((LiteralControl)ctrl).Text = ((LiteralControl)ctrl).Text.Replace(".js\"", String.Format(".js?Rev={0}\"", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            base.OnPreRender(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsMobileBrowser() || (Request.QueryString.AllKeys.Contains("asMob") && Request.QueryString["asMob"] == "true"))
                Page.Header.Controls.Add(new LiteralControl(MobileStyles));
            else
                Page.Header.Controls.Add(new LiteralControl(LandingStyles));

            if (Request.Browser.Browser.ToLower() == "ie" && (Request.Browser.MajorVersion == 6 || Request.Browser.MajorVersion == 7))
                ltlCompatabilityWarning.Text = "<center><div class=\"compatWarn\">It appears you are using Internet Explorer " + Request.Browser.MajorVersion + ".  We do not actively support this older browser.<br />We suggest you upgrade your browser for the best experience using ClearCost Health.</div></center>";
            ScriptManager.RegisterStartupScript(cuwReview, cuwReview.GetType(), "BindThePage", "SetPage()", true);

            if (!Page.IsPostBack)
            {   //If the page is not a post back
                if (Request.QueryString["e"] != null)
                {   //If there is an employer tied to the 'e' query string parameter, process employer content
                    using ( GetEmployerContent gec = new GetEmployerContent(int.Parse(Request.QueryString["e"])))
                    {   //Check the object and store session info as needed.
                        if (gec.PutInSession())
                        {   //If the data object was clean and had data continue to finish the page with the rest of the data
                            Boolean LoginAvailable = gec.CanSignIn;
                            NotificationsVisible = gec.HasNotificationSection;
                            OtherMembersVisible = gec.HasOtherPeopleSection;

                            loginregister.Visible = LoginAvailable;
                            //Do everything required for the Create User Step
                            using (Control create = cuwReview.CreateUserStep.ContentTemplateContainer)
                            {
                                using (Image i = (create.FindControl("imgLogo") as Image))
                                {
                                    i.ImageUrl = ResolveUrl(String.Concat("~/images/", ThisSession.LogoImageName));
                                    i.Visible = !String.IsNullOrWhiteSpace(gec.LogoImageName);
                                }
                                (create.FindControl("pnlOtherMembers") as Panel).Visible = gec.HasOtherPeopleSection;
                                (create.FindControl("pnlNotificationSettings") as Panel).Visible = gec.HasNotificationSection;
                                (create.FindControl("pnlTCVisible") as Panel).Visible = gec.TandCVisible;
                                (create.FindControl("pnlTCHidden") as Panel).Visible = !gec.TandCVisible;
                                using (Repeater r = (create.FindControl("rptOtherMembers") as Repeater))
                                {
                                    if (ThisSession.Dependents == null || ThisSession.Dependents.Count == 0)
                                        r.Visible = false;
                                    else
                                    {
                                        r.DataSource = ThisSession.Dependents.AsDataTable();
                                        r.DataBind();
                                    }
                                }
                            }
                            //Do everyting required for the Complete Step
                            using (Control complete = cuwReview.CompleteStep.ContentTemplateContainer)
                            {
                                (complete.FindControl("imgLogo") as Image).ImageUrl = ThisSession.LogoImageName;
                                (complete.FindControl("pnlStartSearching") as Panel).Visible = LoginAvailable;
                                (complete.FindControl("ltlRegisterComplete") as Literal).Text = gec.CheckBackText;
                                if (LoginAvailable)
                                {
                                    using (Literal l = (complete.FindControl("ltlStartSearching") as Literal))
                                    {
                                        l.Text = String.Format(l.Text, ResolveUrl("~/SavingsChoice/SavingsChoiceWelcome.aspx"));
                                        //l.Text = string.Format(l.Text, Page.ClientScript.GetPostBackClientHyperlink(cuwReview, "StartSearching"));
                                    }
                                    (complete.FindControl("ltlRegisterComplete") as Literal).Text = "You are now registered.";
                                }
                            }
                            //Everything that is non-step related
                            cuwReview.DisableCreatedUser = !LoginAvailable;
                            cuwReview.LoginCreatedUser = LoginAvailable;
                            //if (LoginAvailable)
                            //    cuwReview.DuplicateUserNameErrorMessage = "<div style=\"text-align:left;color:red;\">Our records show you've already registered.<br />Please try logging in via the <a href=\"Sign_In.aspx\">Sign In</a> page.<br />If you've forgotten your password please click <a href=\"../../ResetPassword.aspx\">Forgot Password</a></div>";
                            //else
                            //    cuwReview.DuplicateUserNameErrorMessage = "<div style=\"text-align:left;color:red;\">Our records show you've already registered.<br />You will be able to log in as soon as the new plan year begins.</div>";
                            if (LoginAvailable)
                                cuwReview.DuplicateUserNameErrorMessage = "<div style=\"text-align:left;color:red;\">Our records indicate that this email address is associated with an account already.<br />Please try registering with a different email address or call us at " + ThisSession.EmployerPhone + " for assistance.<br />You can also try logging in via the <a href=\"Sign_In.aspx\">Sign In</a> page.<br />If you've forgotten your password please click <a href=\"../../ResetPassword.aspx\">Forgot Password</a></div>";
                            else
                                cuwReview.DuplicateUserNameErrorMessage = "<div style=\"text-align:left;color:red;\">Our records show you've already registered.<br />You will be able to log in as soon as the new plan year begins.</div>";
                        }
                    }                    
                }

                SetupPasswordQuestions();

                SetupHearCCH();  //  lam, 20130411, MSF-290

                
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        private void SetupPasswordQuestions()
        {
            using (GetPasswordQuestions gpq = new GetPasswordQuestions())
            {
                if (!gpq.HasErrors &&
                    gpq.Tables.Count > 0 &&
                    gpq.Tables[0].Rows.Count > 0)
                {
                    using (DropDownList Qs = (cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("ddlQuestion") as DropDownList))
                    {
                        Qs.DataSource = gpq.QuestionsTable;
                        Qs.DataBind();
                    }
                }
            }
        }
        private void SaveAlerts()
        {
            if (NotificationsVisible)
            {   //If the employer content dictated that notifications should be visible
                using (Control create = (cuwReview.CreateUserStep.ContentTemplateContainer))
                {   //Setup a temporary object to access the create user step content template container
                    using (CheckBox cbTextAlerts = (create.FindControl("cbTextAlerts") as CheckBox))
                    {   //Setup a temporary object to references as the alerts checkbox
                        if (cbTextAlerts.Checked)
                        {   //If the user checked that they wish to receive alerts
                            ThisSession.PatientPhone = String.Concat(
                                Encoder.HtmlEncode((create.FindControl("txtAreaCode") as TextBox).Text),
                                Encoder.HtmlEncode((create.FindControl("txtFirstThree") as TextBox).Text),
                                Encoder.HtmlEncode((create.FindControl("txtLastFour") as TextBox).Text));

                            CheckBox cbEmailAlerts = (create.FindControl("cbEmailAlerts") as CheckBox),
                                cbConciergeAlerts = (create.FindControl("cbConciergeAlerts") as CheckBox);

                            using (UpdateUserNotificationSettings uuns = new UpdateUserNotificationSettings())
                            {
                                uuns.CCHID = ThisSession.CCHID;
                                uuns.OptInEmailAlerts = cbEmailAlerts.Checked;
                                uuns.OptInTextMsgAlerts = cbTextAlerts.Checked;
                                uuns.OptInConciergeAlerts = cbConciergeAlerts.Checked;
                                uuns.MobilePhone = ThisSession.PatientPhone;
                                uuns.PostData();
                            }
                        }
                    }
                }
            }
        }
        private void UpdateEmailAddress()
        {
            using (UpdateUserEmail uea = new UpdateUserEmail())
            {
                //Must run the update on the customer database first
                uea.UpdateClientSide(cuwReview.UserName, ThisSession.CCHID);
                if (!uea.HasErrors)
                { }
            }
        }
        //  lam, 20130227, add UpdatePhone for MSB-142
        private void UpdateRegistrationPhone()
        {
            using (UpdateUserPhone uup = new UpdateUserPhone())
            {
                //Must run the update on the customer database first
                String RegistrationPhoneNumber = "";

                using (Control c = cuwReview.CreateUserStep.ContentTemplateContainer)
                {
                    RegistrationPhoneNumber = Encoder.HtmlEncode((c.FindControl("RegistrationAreaCode") as TextBox).Text) + 
                        Encoder.HtmlEncode((c.FindControl("RegistrationFirstThree") as TextBox).Text) + 
                        Encoder.HtmlEncode((c.FindControl("RegistrationLastFour") as TextBox).Text);
                }

                uup.UpdateClientSide(RegistrationPhoneNumber, ThisSession.CCHID);
                if (!uup.HasErrors)
                { }
            }
        }
        private void UpdateAccess()
        {
            if (((Panel)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("pnlOtherMembers")).Visible)
            {
                Repeater rptOtherMembers = (Repeater)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("rptOtherMembers");
                SqlParameter UserID = new SqlParameter("UserID", SqlDbType.UniqueIdentifier);
                UserID.Value = Guid.Parse(Membership.GetUser(cuwReview.UserName).ProviderUserKey.ToString());

                SqlParameter PastCareRequests = new SqlParameter("PastCareRequests", SqlDbType.Structured);
                DataTable dtPastCareRequest = new DataTable("PastCareRequest");
                dtPastCareRequest.Columns.Add("CCHID", typeof(int));
                dtPastCareRequest.Columns.Add("CCHID_Dependent", typeof(int));
                dtPastCareRequest.Columns.Add("RequestAccessFromDependent", typeof(bool));
                dtPastCareRequest.Columns.Add("GrantAccessToDependent", typeof(bool));
                dtPastCareRequest.Columns.Add("DependentEmail", typeof(string));

                DataRow drPastCareRequestRow = dtPastCareRequest.NewRow();
                foreach (RepeaterItem ri in rptOtherMembers.Items)
                {
                    drPastCareRequestRow["CCHID"] = ThisSession.CCHID;
                    Dependent d = ThisSession.Dependents[ri.ItemIndex];
                    if (d.ShowAccessQuestions)
                    {
                        drPastCareRequestRow["CCHID_Dependent"] = d.CCHID;
                        //If you've already been given access, set to false as we don't want to request again, otherwise set to value of checkbox
                        drPastCareRequestRow["RequestAccessFromDependent"] = ((CheckBox)ri.FindControl("cbRequestToSee")).Checked;
                        //If you've already given access, set to value of DisallowCheckbox, otherwise set to value of AllowCheckbox
                        drPastCareRequestRow["GrantAccessToDependent"] = ((CheckBox)ri.FindControl("cbAllowSeeMy")).Checked;
                        drPastCareRequestRow["DependentEmail"] = Encoder.HtmlEncode(((TextBox)ri.FindControl("txtDepEmail")).Text);
                        dtPastCareRequest.Rows.Add(drPastCareRequestRow);
                        drPastCareRequestRow = dtPastCareRequest.NewRow();
                    }
                }
                PastCareRequests.Value = dtPastCareRequest;

                using (SqlConnection conn = new SqlConnection(ThisSession.CnxString))
                {
                    using (SqlCommand comm = new SqlCommand("UpdateUserAccessRequest", conn))
                    {
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.Parameters.Add(UserID);
                        comm.Parameters.Add(PastCareRequests);
                        try
                        {
                            conn.Open();
                            comm.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            using (BaseCCHData errorHandler = new BaseCCHData())
                            {
                                errorHandler.CaptureError(ex);
                            }
                        }
                        finally
                        { conn.Close(); }
                    }
                }
            }
        }
        private void UpdateSecurityQuestion()
        {
            DropDownList ddl = (DropDownList)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("ddlQuestion");
            try
            {
                Membership.GetUser(cuwReview.UserName).ChangePasswordQuestionAndAnswer(cuwReview.Password, ddl.SelectedValue, cuwReview.Answer);
            }
            catch (Exception ex)
            {
                using (BaseCCHData errorHandler = new BaseCCHData())
                {
                    errorHandler.CaptureError(ex, true);
                }
            }
        }
        private void SetupUserProfile()
        {
            using (InsertUserProfile iup = new InsertUserProfile())
            {
                iup.UserID = (Guid)Membership.GetUser(cuwReview.UserName).ProviderUserKey;
                iup.EmployerID = (ThisSession.EmployerID == String.Empty ? 0 : Convert.ToInt32(ThisSession.EmployerID));
                iup.FirstName = ThisSession.FirstName;
                iup.LastName = ThisSession.LastName;
                iup.Email = cuwReview.Email;
                iup.PostFrontEndData();
            }
        }
        private void CaptureRegistrationLogin()
        {
            using (InsertUserLoginHistory iulh = new InsertUserLoginHistory())
            {
                iulh.UserName = cuwReview.UserName;
                iulh.Domain = Request.Url.Host;
                iulh.PostData();
                if (!iulh.HasErrors && iulh.RowsBack != 1)
                {
                    //In the event this fails or either 0 or more than one row is effected
                    //NOTIFY LAURA :)
                }
            }
        }

        #region Validation Methods
        protected void ValidateEmail(object sender, ServerValidateEventArgs args)
        {
            using (CustomValidator cv = sender as CustomValidator)
            {
                if (cv.ID == "cvDepEmail")
                {
                    using (Control cvP = cv.Parent)
                        using (CheckBox cb = (cvP.FindControl("cbRequestToSee") as CheckBox))
                            if (cb != null && cb.Checked)
                                using (TextBox tb = (cvP.FindControl("txtDepEmail") as TextBox))
                                {
                                    args.IsValid = (tb.Text.Trim() != String.Empty);
                                    if (args.IsValid) args.IsValid = (tb.Text.Contains("@") && tb.Text.Contains("."));
                                }
                }
                else
                {
                    args.IsValid = (args.Value.Trim() != String.Empty);
                    if (args.IsValid) args.IsValid = (args.Value.Contains("@") && args.Value.Contains("."));
                }
            }
        }
        protected void CompareEmail(object sender, ServerValidateEventArgs args)
        {
            using (Control c = cuwReview.CreateUserStep.ContentTemplateContainer)
                args.IsValid = ((c.FindControl("Email") as TextBox).Text == (c.FindControl("ConfirmEmail") as TextBox).Text);
        }
        protected void ComparePassword(object sender, ServerValidateEventArgs args)
        {
            using (Control c = cuwReview.CreateUserStep.ContentTemplateContainer)
                args.IsValid = ((c.FindControl("Password") as TextBox).Text == (c.FindControl("ConfirmPassword") as TextBox).Text);
        }
        protected void ValidateEmpty(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = (args.Value.Trim() != String.Empty);
        }
        protected void ValidateTnC(object sender, ServerValidateEventArgs args)
        {
            if (Request.IsMobileBrowser()) { args.IsValid = true; return; }
            using (Control c = cuwReview.CreateUserStep.ContentTemplateContainer)   
                if ((c.FindControl("pnlTCVisible") as Panel).Visible)
                    args.IsValid = ((c.FindControl("cbxTandCV") as CheckBox).Checked);
                else
                    args.IsValid = ((c.FindControl("cbxTandCH") as CheckBox).Checked);
        }
        protected void ValidatePhone(object sender, ServerValidateEventArgs args)
        {
           using (Control c = cuwReview.CreateUserStep.ContentTemplateContainer)
            {
                if ((c.FindControl("cbTextAlerts") as CheckBox).Checked)
                {
                    int DigitalValidator = 0;
                    String area = (c.FindControl("txtAreaCode") as TextBox).Text,
                        firstThree = (c.FindControl("txtFirstThree") as TextBox).Text,
                        lastFour = (c.FindControl("txtLastFour") as TextBox).Text;

                    if (args.IsValid && int.TryParse(area, out DigitalValidator))
                        args.IsValid = (DigitalValidator > 0);
                    if (args.IsValid && int.TryParse(firstThree, out DigitalValidator))
                        args.IsValid = (DigitalValidator > 0);
                    if (args.IsValid && int.TryParse(lastFour, out DigitalValidator))
                        args.IsValid = (DigitalValidator > 0);
                }
            }
        }
        //  lam, 20130227, new phone validation for MSB-142
        protected void ValidateRegistrationPhone(object sender, ServerValidateEventArgs args)
        {
            using (Control c = cuwReview.CreateUserStep.ContentTemplateContainer)
            {
                    int DigitalValidator = 0;
                    String area = (c.FindControl("RegistrationAreaCode") as TextBox).Text,
                        firstThree = (c.FindControl("RegistrationFirstThree") as TextBox).Text,
                        lastFour = (c.FindControl("RegistrationLastFour") as TextBox).Text;

                    args.IsValid = (area != String.Empty && firstThree != String.Empty && lastFour != String.Empty);
                    if (args.IsValid)
                    {
                        args.IsValid = (area.Length == 3 && firstThree.Length == 3 && lastFour.Length == 4);
                    }

                    if (args.IsValid)
                        args.IsValid = (int.TryParse(area, out DigitalValidator) && int.TryParse(firstThree, out DigitalValidator) && int.TryParse(lastFour, out DigitalValidator));
            }
        }
        #endregion

        #region User Creation Methods
        protected void cuwReview_CreatedUser(object sender, EventArgs e)
        {
            //FormsAuthentication.SetAuthCookie(cuwReview.UserName, false);
            ThisSession.PatientEmail = cuwReview.UserName;
            ThisSession.CurrentSecurityQuestion = cuwReview.Question; 
            try //(JM 9-10-12)Wrap this in a try catch as we were having errors with users already in a role when the registered. This was failing and causing everything else to be skipped
            { Roles.AddUserToRole(cuwReview.UserName, "Customer"); }
            catch (System.Configuration.Provider.ProviderException pEx)
            { //Capture the error
                using (BaseCCHData errorHandler = new BaseCCHData())
                {
                    errorHandler.CaptureError(pEx, true);
                }
            }
            SetupUserProfile();
            UpdateEmailAddress();
            UpdateRegistrationPhone();  //  lam, 20130227, add UpdatePhone for MSB-142
            UpdateAccess();
            UpdateHearCCH();  //  lam, 20130411, MSF-290
            SaveAlerts();

            //UpdateSecurityQuestion(); //JM 10/23/12 - Native registration does this, no need to do it a second time.  Was causing Orphans             
            LoadUserSessionInfo(); //MUST BE DONE LAST
            LoadEmployerContent();
            CaptureRegistrationLogin(); //Can come after as it's just an insert and not a retrieval

            // JM 2-6-13
            // Forced user into the site after successfull registration in order to properly audit the registration login
            if (Membership.ValidateUser(cuwReview.UserName, cuwReview.Password))
            {
                FormsAuthentication.SetAuthCookie(cuwReview.UserName, false);
                if (ThisSession.SavingsChoiceEnabled)
                    Response.Redirect(ResolveUrl("~/SavingsChoice/SavingsChoiceWelcome.aspx"));
                else
                    FormsAuthentication.RedirectFromLoginPage(cuwReview.UserName, false);
            }
        }
        protected void cuwReview_CreateUserError(object sender, CreateUserErrorEventArgs e)
        {
            switch (e.CreateUserError)
            {
                case System.Web.Security.MembershipCreateStatus.DuplicateEmail:
                    cuwReview.DuplicateEmailErrorMessage = "The email address you've specified is already in use.<br />If you've already registered with ClearCost Health please try logging in or using \"Forgot Password\"";
                    break;
                case System.Web.Security.MembershipCreateStatus.DuplicateProviderUserKey:
                    break;
                case System.Web.Security.MembershipCreateStatus.DuplicateUserName:

                    //cuwReview.DuplicateUserNameErrorMessage = "<div style=\"text-align:left;color:red;\">Our records show you've already registered.<br />Please try logging in via the <a href=\"Sign_In.aspx\">Sign In</a> page.<br />If you've forgotten your password please click <a href=\"../../ResetPassword.aspx\">Forgot Password</a></div>";
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidAnswer:
                    cuwReview.InvalidAnswerErrorMessage = "The answer you've provided to the Security Question is invalid.<br />Please check your answer and try again.";
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidEmail:
                    cuwReview.InvalidEmailErrorMessage = "The email address you've provided is invalid.<br />Please check the email address you supplied and try again.";
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidPassword:
                    cuwReview.InvalidPasswordErrorMessage = "The password you've provided is invalid.<br />Please choose a different password of at least {0} characters.";
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidProviderUserKey:
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidQuestion:
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidUserName:
                    cuwReview.InvalidPasswordErrorMessage = "The email address you entered was invalid.<br />Please check the email address you supplied and try again.";
                    break;
                case System.Web.Security.MembershipCreateStatus.ProviderError:
                    break;
                case System.Web.Security.MembershipCreateStatus.Success:
                    break;
                case System.Web.Security.MembershipCreateStatus.UserRejected:
                    break;
                default:
                    break;
            }
        }
        protected void cuwReview_CreatingUser(object sender, LoginCancelEventArgs e)
        {
            Page.Validate();
            if(Page.IsValid)
            {
                using (Control c = cuwReview.CreateUserStep.ContentTemplateContainer)
                {
                    cuwReview.UserName = Encoder.HtmlEncode((c.FindControl("Email") as TextBox).Text);
                    cuwReview.Question = (c.FindControl("ddlQuestion") as DropDownList).SelectedItem.Text;
                }
            }
            e.Cancel = !Page.IsValid;
        }
        #endregion

        protected void LoadUserSessionInfo()
        {
            String sUN = (cuwReview.UserName).Trim();
            MembershipUser mu = Membership.GetUser(sUN);
            String puk = mu.ProviderUserKey.ToString();

            using(GetKeyUserInfo gkui = new GetKeyUserInfo(puk))
            {
                gkui.PutInSession();
            }
            using (GetKeyEmployeeInfo gkei = new GetKeyEmployeeInfo(sUN))
            {
                gkei.PutInSession(cuwReview.UserName.Trim());
            }

            ThisSession.UserLogginID = puk;
            ThisSession.LoggedIn = true;
        }
        protected void LoadEmployerContent()
        {
            using (GetEmployerContent gec = new GetEmployerContent(int.Parse(ThisSession.EmployerID)))
            {
                gec.PutInSession();
            }
        }

        protected override void LoadViewState(object savedState)  //  lam, 20130411, MSF-290
        {
            base.LoadViewState(savedState);
            if (ViewState["controlsadded"] == null)
                SetupHearCCH();
            else
                DisplayHearCCH((List<FAQContent>)ViewState["controlsadded"]);
        }

        private void DisplayHearCCH(List<FAQContent> FAQContentList)  //  lam, 20130411, MSF-290
        {
            if (FAQContentList != null && FAQContentList.Count > 0)
            {
                Panel pnlHearCCH = (Panel)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("pnlHearCCH");

                for (int i = 0; i < FAQContentList.Count; i++)
                {
                    CheckBox chk = new CheckBox();
                    //rbt.GroupName = "HearCCH";
                    chk.ID = "chkQuestion" + FAQContentList[i].FAQContentID.ToString();
                    chk.Text = "&nbsp;&nbsp;" + FAQContentList[i].FAQText + "</br>";

                    if (FAQContentList[i].NeedUserInput)
                    {
                        TextBox txt = new TextBox();
                        txt.ID = "txtQuestion" + FAQContentList[i].FAQContentID.ToString();
                        txt.MaxLength = 50;
                        txt.CssClass = "boxed";

                        pnlHearCCH.Controls.Add(chk);
                        pnlHearCCH.Controls.Add(txt);
                    }
                    else
                    {
                        chk.Text += "</br>";
                        pnlHearCCH.Controls.Add(chk);
                    }
                }

                //ViewState["controlsadded"] = FAQContentList;

            }
        }

        private void SetupHearCCH()  //  lam, 20130411, MSF-290
        {
            //  CategoryID is hardcoded since we know the value from db
            String CategoryID = "6fe85dba-3430-49e7-9baa-f80267e833c6";
            using (GetEmployerFAQContent gefc = new GetEmployerFAQContent(Convert.ToInt32(Request.QueryString["e"], 10), CategoryID))
            {
                gefc.GetFrontEndData();
                List<FAQContent> FAQContentList = gefc.FAQContentList;

                ViewState["controlsadded"] = FAQContentList;
                DisplayHearCCH(FAQContentList);
            }
        }

        protected void ValidateRegistrationHearCCH(object sender, ServerValidateEventArgs args)  //  lam, 20130411, MSF-290
        {
            using (Control c = cuwReview.CreateUserStep.ContentTemplateContainer)
            {
                Control pnl = c.FindControl("pnlHearCCH");

                for (int i = 0; i < pnl.Controls.Count; i++)
                {
                    Control cc = pnl.Controls[i];
                    if (cc.GetType().FullName.ToLower() == "system.web.ui.webcontrols.checkbox")
                    {
                        CheckBox chk = (CheckBox)cc;
                        if (chk.Checked)
                        {
                            TextBox txt = (TextBox)pnl.FindControl(chk.ID.Replace("chk", "txt"));
                            args.IsValid = (txt == null || txt.Text.Trim() != "");
                        }
                    }
                }
            }
        }

        private void UpdateHearCCH()  //  lam, 20130411, MSF-290
        {
            using (UpdateHearCCH uhcch = new UpdateHearCCH())
            {
                //Must run the update on the customer database first
                String HearCCH = "";

                using (Control c = cuwReview.CreateUserStep.ContentTemplateContainer)
                {
                    Control pnl = c.FindControl("pnlHearCCH");

                    for (int i = 0; i < pnl.Controls.Count; i++)
                    {
                        Control cc = pnl.Controls[i];
                        if (cc.GetType().FullName.ToLower() == "system.web.ui.webcontrols.checkbox")
                        {
                            CheckBox chk = (CheckBox)cc;
                            if (chk.Checked)
                            {
                                TextBox txt = (TextBox)pnl.FindControl(chk.ID.Replace("chk", "txt"));
                                HearCCH += (HearCCH == "" ? "" : "|") + (txt != null ? txt.Text.Trim() : chk.Text.Replace("&nbsp;", "").Replace("</br>", "").Trim());
                            }
                        }
                    }
                }

                if (HearCCH != "")
                {
                    uhcch.UpdateClientSide(HearCCH, ThisSession.CCHID);
                    if (!uhcch.HasErrors)
                    { }
                }
            }
        }
    }
}