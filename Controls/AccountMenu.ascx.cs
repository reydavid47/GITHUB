using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;
using System.Text.RegularExpressions;
using Microsoft.Security.Application;

namespace ClearCostWeb.Controls
{
    public partial class AccountMenu : System.Web.UI.UserControl
    {
        private Boolean _hasValidationError = false;  //  lam, 20130322, MSF-223, add this property
        protected Boolean HasValidationError
        {
            get { return _hasValidationError; }
        }
        //  ---------------------------------------------------------------------------------------

        protected String contactText { get; set; }
        public String ContactText { set { this.contactText = value; } }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            JavaScriptHelper.IncludeAccountMenu(Page.ClientScript);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScriptManager csm = Page.ClientScript;

            if (!Page.IsPostBack)
            {
                lblFullName.Text = String.Format("{0} {1}", ThisSession.FirstName, ThisSession.LastName);
                lblDateOfBirth.Text = String.Format("{0:MMMM dd, yyyy}",
                    Convert.ToDateTime(ThisSession.PatientDateOfBirth.Substring(0, ThisSession.PatientDateOfBirth.IndexOf(" ")))
                    );
                lblInsurer.Text = ThisSession.Insurer;
                lblMedicalPlan.Text = ThisSession.MedicalPlanType;
                lblAddress3Line.Text = String.Format("{0}{1}{2}<br />",
                    ThisSession.PatientAddress1,
                    (ThisSession.PatientAddress2.Trim() != String.Empty ? "" : ThisSession.PatientAddress2.Trim() + "<br />"),
                    String.Format("{0}, {1} {2}",
                        ThisSession.PatientCity,
                        ThisSession.PatientState,
                        ThisSession.PatientZipCode
                        )
                    );
                lblFormattedPhone.Text = FormatedPatientPhone();
                lblPatientEmail.Text = ThisSession.PatientEmail;
                lblCurrentQuestion.Text = ThisSession.CurrentSecurityQuestion;
                if (ThisSession.CurrentAvailableSecurityQuestions == null || ThisSession.CurrentAvailableSecurityQuestions.Length == 0)
                    ddlQuestion.DataSource = GetListOfQuestions();
                else
                    ddlQuestion.DataSource = ThisSession.CurrentAvailableSecurityQuestions;

                ddlQuestion.DataBind();

                if (ThisSession.Dependents.Count > 0)
                {
                    rptMemberAccess.DataSource = ThisSession.Dependents.AsDataTable();
                    rptMemberAccess.DataBind();
                }
                else
                {
                    rptMemberAccess.Visible = false;
                    lblOnlyMember.Visible = true;
                }

                // - Rig Up the Opt In UI elements to the data
                cbEmailAlerts.Checked = ThisSession.OptInEmailAlerts;
                cbTextAlerts.Checked = ThisSession.OptInTextMsgAlerts;
                if (ThisSession.OptInTextMsgAlerts)
                    txtMobileAlert.Text = ThisSession.MobilePhone;
                cbConciergeAlerts.Checked = ThisSession.OptInPriceConcierge;
            }
        }
        protected void UpdatePhoneNumber(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ThisSession.CnxString))
            {
                using (SqlCommand comm = new SqlCommand("UpdateUserPhone", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("CCHID", ThisSession.CCHID);
                    comm.Parameters.AddWithValue("Phone", Encoder.HtmlEncode(txtPhoneUpdate.Text));
                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                        ThisSession.PatientPhone = Encoder.HtmlEncode(txtPhoneUpdate.Text);
                    }
                    catch (Exception)
                    { }
                    finally
                    {
                        lblFormattedPhone.Text = FormatedPatientPhone();
                        txtPhoneUpdate.Text = "";
                        conn.Close();
                    }
                }
            }
        }
        protected void UpdateEmailAddress(object sender, EventArgs e)
        {
            using (UpdateUserEmail uue = new UpdateUserEmail())
            {
                uue.UserName = System.Web.Security.Membership.GetUserNameByEmail(ThisSession.PatientEmail);
                uue.Email = Encoder.HtmlEncode(txtUpdateEmail.Text);//ThisSession.PatientEmail;
                uue.UserID = ThisSession.UserLogginID;
                uue.PostFrontEndData();
                if (!uue.HasErrors)
                {
                    switch (uue.ReturnStatus)
                    {
                        case 0:
                            uue.UpdateClientSide(Encoder.HtmlEncode(txtUpdateEmail.Text), ThisSession.CCHID);
                            if (!uue.HasErrors)
                            {
                                ThisSession.PatientEmail = Encoder.HtmlEncode(txtUpdateEmail.Text);
                                lblPatientEmail.Text = Encoder.HtmlEncode(txtUpdateEmail.Text);
                                txtUpdateEmail.Text = "";
                            }
                            break;
                        case 1:
                            //Unable to find that username in the front end
                            break;
                        case 7:
                            //Duplicate email found in the front end for user id
                            break;
                        case -1:
                            //Other failure
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        protected void UpdateMe(object sender, EventArgs e)
        {
            upPassword.Update();
            //((Literal)((ChangePassword)(upPassword.ContentTemplateContainer.FindControl("cpChangePassword"))).ChangePasswordTemplateContainer.FindControl("FailureText")).Text = "The password you entered does not meet the current requirements set.<br />Your password must be at least 1 character in length.";
        }
        protected void SetTimeout(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript((ChangePassword)sender, sender.GetType(), "ClearPasswordBox",
                "setTimeout(function () { $(\"div#upPassword\").closest(\"tr\").fadeOut(1000); }, 1000);", true);
        }
        protected void UpdateSecurityQuestion(object sender, EventArgs e)
        {
            try
            {
                if (Membership.GetUser(ThisSession.PatientEmail).ChangePasswordQuestionAndAnswer(txtUpdateSecurityPassword.Text, ddlQuestion.SelectedItem.Text, txtUpdateSecurityAnswer.Text))
                {
                    //CurrentQuestion = ddlQuestion.SelectedItem.Text;
                    lblCurrentQuestion.Text = ddlQuestion.SelectedItem.Text;
                    ThisSession.CurrentSecurityQuestion = ddlQuestion.SelectedItem.Text;
                    txtUpdateSecurityAnswer.Text = txtUpdateSecurityPassword.Text = "";
                }
                else
                {

                }
            }
            catch (Exception)
            { }
        }
        protected void Logout(object sender, EventArgs e)
        {
            //Do not delete
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
        }
        protected void SaveAlerts(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ThisSession.CnxString))
            {
                using (SqlCommand comm = new SqlCommand("UpdateUserNotificationSettings", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("CCHID", ThisSession.CCHID);
                    comm.Parameters.AddWithValue("OptInEmailAlerts", cbEmailAlerts.Checked);
                    comm.Parameters.AddWithValue("OptInTextMsgAlerts", cbTextAlerts.Checked);
                    comm.Parameters.AddWithValue("MobilePhone", Encoder.HtmlEncode(txtMobileAlert.Text));
                    comm.Parameters.AddWithValue("OptInPriceConcierge", cbConciergeAlerts.Checked);
                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                    catch (Exception)
                    { }
                    finally
                    { conn.Close(); }
                }
            }

            // - Update the session information with the new OptIn settings so we don't have to go get them from the database again
            ThisSession.OptInEmailAlerts = cbEmailAlerts.Checked;
            ThisSession.OptInTextMsgAlerts = cbEmailAlerts.Checked;
            if (cbEmailAlerts.Checked)
                ThisSession.MobilePhone = Encoder.HtmlEncode(txtMobileAlert.Text);
            ThisSession.OptInPriceConcierge = cbConciergeAlerts.Checked;
        }
        protected void UpdateAccess(object sender, EventArgs e)
        {
            if (!HasValidationError)
            {  //  lam, 20130322, MSF-223, allow saving only pass validation
                SqlParameter UserID = new SqlParameter("UserID", SqlDbType.UniqueIdentifier);
                UserID.Value = Guid.Parse(ThisSession.UserLogginID);

                SqlParameter PastCareRequests = new SqlParameter("PastCareRequests", SqlDbType.Structured);
                DataTable dtPastCareRequest = new DataTable("PastCareRequest");
                dtPastCareRequest.Columns.Add("CCHID", typeof(int));
                dtPastCareRequest.Columns.Add("CCHID_Dependent", typeof(int));
                dtPastCareRequest.Columns.Add("RequestAccessFromDependent", typeof(bool));
                dtPastCareRequest.Columns.Add("GrantAccessToDependent", typeof(bool));
                dtPastCareRequest.Columns.Add("DependentEmail", typeof(string));

                DataRow drPastCareRequestRow = dtPastCareRequest.NewRow();
                foreach (RepeaterItem ri in rptMemberAccess.Items)
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
                        //  lam, 20130322, MSF-223
                        //  drPastCareRequestRow["DependentEmail"] = d.Email;
                        drPastCareRequestRow["DependentEmail"] = (((CheckBox)ri.FindControl("cbRequestToSee")).Checked ? ((TextBox)ri.FindControl("txtRequesteeEmail")).Text.Trim() : d.Email);
                        //  ----------------------------------------------------------
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
                        { String t = ex.Message; }
                        finally
                        { conn.Close(); }
                    }
                }
            }
        }

        private String FormatedPatientPhone()
        {
            String phoneOut = ThisSession.PatientPhone;
            Regex regexObj = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");

            if (regexObj.IsMatch(ThisSession.PatientPhone))
            {
                phoneOut = regexObj.Replace(phoneOut, "($1) $2-$3");
            }
            return phoneOut;
        }
        private String[] GetListOfQuestions()
        {
            using (GetPasswordQuestions gpq = new GetPasswordQuestions())
            {
                if (!gpq.HasErrors)
                {
                    if (gpq.QuestionsTable.TableName != "Empty")
                    {
                        List<String> Qs = new List<string>();
                        foreach (DataRow dr in gpq.QuestionsTable.Rows)
                            Qs.Add(dr[0].ToString());
                        ThisSession.CurrentAvailableSecurityQuestions = Qs.ToArray<String>();
                        return Qs.ToArray<String>();
                    }
                    else
                    {
                        ThisSession.CurrentAvailableSecurityQuestions = new[] { "none" };
                        return new String[] { "none" };
                    }
                }
                else { return null; }
            }
        }
        //  lam, 20130322, MSF-223, add this validation method
        protected void ValidateEmail(object sender, ServerValidateEventArgs args)
        {
            using (CustomValidator cv = sender as CustomValidator)
            {
                if (cv.ID == "cvRequesteeEmail")
                {
                    using (Control cvP = cv.Parent)
                    using (CheckBox cb = (cvP.FindControl("cbRequestToSee") as CheckBox))
                        if (cb != null) {
                            if (cb.Checked)
                            {
                                using (TextBox tb = (cvP.FindControl("txtRequesteeEmail") as TextBox))
                                {
                                    args.IsValid = (tb.Text.Trim() != String.Empty);
                                    if (args.IsValid) args.IsValid = (tb.Text.Contains("@") && tb.Text.Contains("."));
                                    _hasValidationError = !args.IsValid;
                                }
                            }
                            else
                                ((TextBox)cvP.FindControl("txtRequesteeEmail")).Text = "";
                        }
                }
                else
                {
                    args.IsValid = (args.Value.Trim() != String.Empty);
                    if (args.IsValid) args.IsValid = (args.Value.Contains("@") && args.Value.Contains("."));
                }
            }
            //args.IsValid = (args.Value.Trim() != string.Empty); //Validate that they entered something and didn't leave it blank
            //if (args.IsValid) args.IsValid = (args.Value.Contains('.') && args.Value.Contains("@")); //Validate that the input they entered has at least an @ and a . symbol in it
        }
    }
}