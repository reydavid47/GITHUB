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
using Microsoft.Security.Application;

namespace ClearCostWeb.AnalogDevices
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
            ltlStyleSheets.Text = CssStyleSheets;

            if (!Page.IsPostBack)
            {
                ////Load account key information.
                //DateTime dateOfBirth = DateTime.Parse(ThisSession.PatientDateOfBirth);
                //string dob = dateOfBirth.ToLongDateString();
                //dob = dob.Substring(dob.IndexOf(",") + 1); // remove day of week from string.
                //lblDateOfBirth.Text = dob;
                //lblAddress.Text = ThisSession.PatientAddress1;
                //if (ThisSession.PatientAddress2 != String.Empty)
                //{ lblAddress.Text += "<br />" + ThisSession.PatientAddress2; }
                //lblAddress.Text += "<br />" + ThisSession.PatientCity + ", " + ThisSession.PatientState + " " + ThisSession.PatientZipCode;
                cuwReview.UserName = ThisSession.PatientEmail;

                //if (ThisSession.Insurer != string.Empty)
                //    trHP.Visible = true;

                //if (ThisSession.HealthPlanType != string.Empty)
                //    trHPT.Visible = true;

                SetupPasswordQuestions();

                //Display list of adult dependents
                Repeater r = (Repeater)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("rptOtherMembers");
                if (ThisSession.Dependents.Count == 0)
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
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand("GetPasswordQuestions", conn))
                {
                    //Setup securty questions
                    comm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            try
                            {
                                da.Fill(ds);
                            }
                            catch (Exception ex)
                            {

                            }
                            ((DropDownList)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("ddlQuestion")).DataSource = ds.Tables[0];
                            ((DropDownList)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("ddlQuestion")).DataBind();
                        }
                    }
                }
            }
        }
        private void SaveAlerts()
        {
            CheckBox cbEmailAlerts = (CheckBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("cbEmailAlerts");
            CheckBox cbTextAlerts = (CheckBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("cbTextAlerts");
            CheckBox cbConciergeAlerts = (CheckBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("cbConciergeAlerts");
            TextBox txtMobileAlert = (TextBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("txtMobilePhone");
            using (SqlConnection conn = new SqlConnection(ThisSession.CnxString))
            {
                using (SqlCommand comm = new SqlCommand("UpdateUserNotificationSettings", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("CCHID", ThisSession.CCHID);
                    comm.Parameters.AddWithValue("OptInEmailAlerts", cbEmailAlerts.Checked);
                    comm.Parameters.AddWithValue("OptInTextMsgAlerts", cbTextAlerts.Checked);
                    comm.Parameters.AddWithValue("MobilePhone",  Encoder.HtmlEncode(txtMobileAlert.Text));
                    comm.Parameters.AddWithValue("OptInPriceConcierge", cbConciergeAlerts.Checked);
                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    { }
                    finally
                    { conn.Close(); }
                }
            }
        }
        private void UpdateEmailAddress()
        {
            //Successfully updated front end
            using (SqlConnection conn = new SqlConnection(ThisSession.CnxString))
            {
                using (SqlCommand comm = new SqlCommand("UpdateUserEmail", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("CCHID", ThisSession.CCHID);
                    comm.Parameters.AddWithValue("Email", ThisSession.PatientEmail);
                    comm.Parameters.Add("retVal", SqlDbType.Int);
                    comm.Parameters["retVal"].Direction = ParameterDirection.ReturnValue;
                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    { }
                    finally
                    { conn.Close(); }
                }
            }
        }
        private void UpdateAccess()
        {
            Repeater rptOtherMembers = (Repeater)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("rptOtherMembers");
            SqlParameter UserID = new SqlParameter("UserID", SqlDbType.UniqueIdentifier);
            UserID.Value = Guid.Parse(Membership.GetUser(ThisSession.PatientEmail).ProviderUserKey.ToString());

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
                    drPastCareRequestRow["RequestAccessFromDependent"] = (d.DepToUserGranted ? false : ((CheckBox)ri.FindControl("cbRequestToSee")).Checked);
                    //If you've already given access, set to value of DisallowCheckbox, otherwise set to value of AllowCheckbox
                    drPastCareRequestRow["GrantAccessToDependent"] = (d.UserToDepGranted ?
                                                                     ((CheckBox)ri.FindControl("cbDisallowSeeMy")).Checked :
                                                                     ((CheckBox)ri.FindControl("cbAllowSeeMy")).Checked);
                    drPastCareRequestRow["DependentEmail"] = d.Email;
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
                    { }
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
                Membership.GetUser(ThisSession.PatientEmail).ChangePasswordQuestionAndAnswer(cuwReview.Password, ddl.SelectedValue, cuwReview.Answer);
            }
            catch (Exception ex)
            {

            }
        }
        private void SetupUserProfile()
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString))
            {
                String query = @"INSERT INTO [CCH_FrontEnd].[dbo].[UserProfile]([UserID],[EmployerID],[UnregisteredEmpID],[FirstName],[LastName],[Email]) "
                    + @"VALUES('{0}',{1},'','{2}','{3}','{4}')";
                query = string.Format(query,
                    Membership.GetUser(cuwReview.UserName).ProviderUserKey,
                    ThisSession.EmployerID,
                    ThisSession.FirstName,
                    ThisSession.LastName,
                    cuwReview.Email);
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.CommandType = CommandType.Text;
                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    { }
                    finally
                    { conn.Close(); }
                }
            }
        }

        protected void cuwReview_CreatedUser(object sender, EventArgs e)
        {
            Roles.AddUserToRole(cuwReview.UserName, "Customer");
            UpdateEmailAddress();
            UpdateAccess();
            SaveAlerts();
            UpdateSecurityQuestion();
            SetupUserProfile();
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
                    cuwReview.DuplicateUserNameErrorMessage = "Our records show you've already registered.<br />Please try logging in via the login page.<br />If you've forgotten your password please click \"Forgot Password\"";
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidAnswer:
                    cuwReview.InvalidAnswerErrorMessage = "The answer you've provided to the Security Question is invalid.<br />Please check your answer and try again.";
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidEmail:
                    cuwReview.InvalidEmailErrorMessage = "The email address you've provided is invalid.<br />Please check the email address you supplied and try again.";
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidPassword:
                    cuwReview.InvalidPasswordErrorMessage = "The password you've provided is invalid.<br />Please retype the password you supplied and try again.";
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidProviderUserKey:
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidQuestion:
                    break;
                case System.Web.Security.MembershipCreateStatus.InvalidUserName:
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
            CheckBox c = (CheckBox)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("cbxTandC");
            Label l = (Label)cuwReview.CreateUserStep.ContentTemplateContainer.FindControl("lblPleaseAgree");
            e.Cancel = !c.Checked;
            if (!c.Checked)
            {
                c.Focus();
                l.Visible = true;
            }
            else
                l.Visible = false;
        }
    }
}