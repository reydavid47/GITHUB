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
using System.Security.Cryptography;
using System.IO;
using System.Text;
using QSEncryption.QSEncryption;
using Microsoft.Security.Application;

namespace ClearCostWeb.Starbucks
{
    public partial class Review : System.Web.UI.Page
    {
        private String CssStyleSheets
        {
            get
            {
                String template = @"<link href=""{0}"" rel=""Stylesheet"" type=""text/css"" /><link href=""{1}"" rel=""Stylesheet"" type=""text/css"" />";

                return String.Format(template,
                    ResolveUrl("~/Styles/skin.css"),
                    ResolveUrl("~/Styles/old/style.css"));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Browser.Browser.ToLower() == "ie" && Request.Browser.MajorVersion == 6)
                ltlCompatabilityWarning.Text = "<center><div class=\"compatWarn\">It appears you are using Internet Explorer 6.  We do not actively support this older browser.<br />We suggest you upgrade your browser for the best experience using ClearCost Health.</div></center>";
            ScriptManager.RegisterStartupScript(cuwReview, cuwReview.GetType(), "BindThePage", "SetPage()", true);
            if (!Page.IsPostBack)
            {
                SetupPasswordQuestions();

                //Display list of adult dependents
                Repeater r = (Repeater)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("rptOtherMembers");
                if (ThisSession.Dependents == null || ThisSession.Dependents.Count == 0)
                    r.Visible = false;
                else
                {
                    r.DataSource = ThisSession.Dependents.AsDataTable();
                    r.DataBind();
                }
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
                if (!gpq.HasErrors)
                {
                    DropDownList Qs = (DropDownList)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("ddlQuestion");
                    Qs.DataSource = gpq.QuestionsTable;
                    Qs.DataBind();
                }
            }
        }
        private void SaveAlerts()
        {
            CheckBox cbEmailAlerts = (CheckBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("cbEmailAlerts");
            CheckBox cbTextAlerts = (CheckBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("cbTextAlerts");
            CheckBox cbConciergeAlerts = (CheckBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("cbConciergeAlerts");
            //TextBox txtMobileAlert = (TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("txtMobilePhone");
            if (cbTextAlerts.Checked)
            {
                String FullPhone = String.Format("{0}{1}{2}",
                    ((TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("txtAreaCode")).Text,
                    ((TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("txtFirstThree")).Text,
                    ((TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("txtLastFour")).Text);
                FullPhone = Microsoft.Security.Application.Encoder.HtmlEncode(FullPhone);

                using (SqlConnection conn = new SqlConnection(ThisSession.CnxString))
                {
                    using (SqlCommand comm = new SqlCommand("UpdateUserNotificationSettings", conn))
                    {
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.Parameters.AddWithValue("CCHID", ThisSession.CCHID);
                        comm.Parameters.AddWithValue("OptInEmailAlerts", cbEmailAlerts.Checked);
                        comm.Parameters.AddWithValue("OptInTextMsgAlerts", cbTextAlerts.Checked);
                        comm.Parameters.AddWithValue("MobilePhone", FullPhone);
                        comm.Parameters.AddWithValue("OptInPriceConcierge", cbConciergeAlerts.Checked);
                        try
                        {
                            conn.Open();
                            comm.ExecuteNonQuery();
                            ThisSession.PatientPhone = FullPhone;
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
            //if (cbConciergeAlerts.Checked)
            //{
            //    ((Literal)cuwReview.CompleteStep.ContentTemplateContainer.FindControl("ltlRegisterComplete")).Text = "You have registered and been entered into a raffle for the chance to win one of twenty $500 prizes.";
            //}
            //else
            //{
            ((Literal)cuwReview.CompleteStep.ContentTemplateContainer.FindControl("ltlRegisterComplete")).Text = "You are now registered.";
            //}
        }
        private void UpdateEmailAddress()
        {
            using (UpdateUserEmail uea = new UpdateUserEmail())
            {
                //Must run the update on the customer database first
                uea.UpdateClientSide(cuwReview.UserName, ThisSession.CCHID);
                if (!uea.HasErrors)
                {

                }
            }
        }
        private void UpdateAccess()
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
                    drPastCareRequestRow["DependentEmail"] = Microsoft.Security.Application.Encoder.HtmlEncode(((TextBox)ri.FindControl("txtDepEmail")).Text);
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
            CustomValidator cv = (CustomValidator)sender;
            if (cv.ID == "cvDepEmail")
            {
                CheckBox cb = (CheckBox)cv.Parent.FindControl("cbRequestToSee");
                TextBox tb = (TextBox)cv.Parent.FindControl("txtDepEmail");
                if (cb != null && cb.Checked) //Only validate if they wish to have access
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
        protected void CompareEmail(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = (((TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("Email")).Text == ((TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("ConfirmEmail")).Text);
        }
        protected void ComparePassword(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = (((TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("Password")).Text == ((TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("ConfirmPassword")).Text);
        }
        protected void ValidateEmpty(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = (args.Value.Trim() != String.Empty);
        }
        protected void ValidateTnC(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = (((CheckBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("cbxTandC")).Checked);
        }
        protected void ValidatePhone(object sender, ServerValidateEventArgs args)
        {
            //if (((CheckBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("cbRaffle")).Checked)
            //{
            if (((CheckBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("cbTextAlerts")).Checked)
            {
                int DigitalValidator = 0;
                String area = ((TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("txtAreaCode")).Text;
                String firstThree = ((TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("txtFirstThree")).Text;
                String lastFour = ((TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("txtLastFour")).Text;

                args.IsValid = int.TryParse(area, out DigitalValidator);
                if (args.IsValid) args.IsValid = (DigitalValidator > 0);
                if (args.IsValid) args.IsValid = int.TryParse(firstThree, out DigitalValidator);
                if (args.IsValid) args.IsValid = (DigitalValidator > 0);
                if (args.IsValid) args.IsValid = int.TryParse(lastFour, out DigitalValidator);
                if (args.IsValid) args.IsValid = (DigitalValidator > 0);
            }
            //}
        }
        #endregion

        #region User Creation Methods
        protected void cuwReview_CreatedUser(object sender, EventArgs e)
        {
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
            UpdateAccess();
            SaveAlerts();
            UpdateSecurityQuestion();
            LoadUserSessionInfo(); //MUST BE DONE LAST
            CaptureRegistrationLogin(); //Can come after as it's just an insert and not a retrieval

            GenerateReferralLink(); //Generate Refferel link


        }

        protected void GenerateReferralLink()
        {
            string URL = Request.Url.ToString().Replace("Review", "Welcome"); //"https://www.clearcosthealth.com/Starbucks/Welcome.aspx?";

            TextBox txtLink = ((TextBox)cuwReview.CompleteStep.ContentTemplateContainer.FindControl("txtReffral"));
            string strURLWithData = "";
            if (ThisSession.ReffralCCHID != 0) //Self Regestring
            {
                UpdateReffralInfo(); //Update Referral  ID in Database 
            }
            strURLWithData = URL + "?" + Encrypt(string.Format("CCHID={0}", ThisSession.CCHID), "r0b1nr0y");
            txtLink.Text = strURLWithData;
        }

        private byte[] key = { };
        private byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
        public string Encrypt(string stringToEncrypt, string SEncryptionKey)
        {
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(SEncryptionKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms,
                  des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public void UpdateReffralInfo()
        {
            //Update Referral  to database
            using (UpdateReferral reff = new UpdateReferral())
            {
                reff.ReffCCHID = ThisSession.ReffralCCHID;
                reff.CCHID = ThisSession.CCHID;
                reff.PostData();
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
                    cuwReview.DuplicateUserNameErrorMessage = "<div style=\"text-align:left;color:red;\">Our records show you've already registered.<br />Please try logging in via the <a href=\"Sign_In.aspx\">Sign In</a> page.<br />If you've forgotten your password please click <a href=\"../../ResetPassword.aspx\">Forgot Password</a></div>";
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
            if (Page.IsValid) cuwReview.UserName = Microsoft.Security.Application.Encoder.HtmlEncode(
                ((TextBox)(cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("Email"))).Text);
            if (Page.IsValid) cuwReview.Question = 
                ((DropDownList)(cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("ddlQuestion"))).SelectedItem.Text;
            e.Cancel = !Page.IsValid;
        }
        #endregion

        protected void LoadUserSessionInfo()
        {
            //Which employer database?
            GetKeyUserInfo gkui = new GetKeyUserInfo(Membership.GetUser(cuwReview.UserName).ProviderUserKey.ToString());
            if (!gkui.HasErrors)
            {
                ThisSession.CnxString = gkui.ConnectionString;
                ThisSession.EmployerID = gkui.EmployerID;
                ThisSession.EmployerName = gkui.EmployerName;
                ThisSession.Insurer = gkui.Insurer;
                ThisSession.RXProvider = gkui.RXProvider;
                ThisSession.ShowYourCostColumn = gkui.ShowYourCostColumn;
            }
            else
            {
                //Literal failText = (Literal)CheckEmpIDLogin.FindControl("FailureText");
                //failText.Text = gkui.SqlException;
                //failText.Visible = true;
            }
        }
    }
}