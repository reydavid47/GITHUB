using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.Configuration;
using Microsoft.Security.Application;

namespace ClearCostWeb.Controls
{
    public partial class SignIn : System.Web.UI.UserControl
    {
        public String ClientUrl { get; set; }
        private String Destination { get { return "~/SearchInfo/Search.aspx"; } }  //Always start a new search as we are also expiring the session 
        private Boolean IsTimeout { get { return (Request.QueryString.ToString().Contains("timeout")); } }

        protected override void OnPreRender(EventArgs e)
        {
            if (Request.QueryString["ReturnUrl"] != null) { Response.Redirect("sign_in.aspx"); }
            else { base.OnPreRender(e); }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            MainLogin.DestinationPageUrl = this.Destination;
            if (!Page.IsPostBack)
            {
                //Uri u = Request.Url;
                //String[] uSegs = u.Segments.TakeWhile(seg => seg != u.Segments.Last()).ToArray();

                //((HyperLink)MainLogin.FindControl("hlReset")).NavigateUrl = ResolveUrl("ResetPassword.aspx");

                if (ClientUrl != null && ClientUrl != String.Empty) { lbtnRegister.Visible = true; }

                WorkWithSignInTextBoxes();

                if (IsTimeout)
                {
                    Literal lFail = (Literal)MainLogin.FindControl("FailureText");
                    lFail.Text = "<div style=\"text-align:left;color:red;\">For your security we logged you out after being idle for 15 minutes.<br /><b style=\"color:red;\">Please sign in again.</b></div>";
                    FormsAuthentication.SignOut();
                    ThisSession.ClearSessionVariables();
                }
                else if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                    ThisSession.ClearSessionVariables();
            }
            else
            {

            }
        }

        protected void MainLogin_LoggedIn(object sender, EventArgs e)
        {
            String mlUN = MainLogin.UserName.Trim();
            Boolean IsCustomer = Roles.IsUserInRole(mlUN, "Customer"),
                IsCallCenter = Roles.IsUserInRole(mlUN, "CallCenter"),
                IsAdmin = Roles.IsUserInRole(mlUN, "Admin"),
                IsManagement = Roles.IsUserInRole(mlUN, "Management"),
                IsDebugUser = Roles.IsUserInRole(mlUN, "DebugUser");

            bool iqComplete; string lastIQUrl;

            if (IsCustomer)
            {
                using (TextBox userName = (MainLogin.FindControl("UserName") as TextBox))
                {
                    String sUN = Encoder.HtmlEncode(userName.Text.Trim());

                    ThisSession.UserLogginID = Membership.GetUser(sUN).ProviderUserKey.ToString();
                    ThisSession.LoggedIn = true;

                    LoadUserSessionInfo();
                    //LoadUserEmployerSessionInfo();
                    iqComplete = LoadUserEmployerSessionInfo(out lastIQUrl);
                    LoadEmployerContent();

                    using (GetPasswordQuestions gpq = new GetPasswordQuestions())
                    {
                        if (!gpq.PutInSession())
                        {
                            ThisSession.CurrentAvailableSecurityQuestions = new[] { "none" };
                        }
                        ThisSession.CurrentSecurityQuestion = Membership.GetUser(sUN).PasswordQuestion;
                    }
                    using (InsertUserLoginHistory iulh = new InsertUserLoginHistory())
                    {
                        iulh.UserName = Membership.GetUserNameByEmail(ThisSession.PatientEmail);
                        iulh.Domain = Request.Url.Host;
                        if (IsDebugUser) { iulh.CallCenterID = Guid.Empty.ToString(); }
                        iulh.PostData();
                    }
                    if (ThisSession.SavingsChoiceEnabled)
                    {
                        if (!iqComplete)
                            if (lastIQUrl == null || lastIQUrl == "null" || lastIQUrl.Trim() == "" || lastIQUrl == "error")
                                Response.Redirect("~/SavingsChoice/SavingsChoiceWelcome.aspx");
                            else
                                Response.Redirect("~/SavingsChoice/" + lastIQUrl);
                    }
                }
                if (Request.QueryString.AllKeys.Contains("dest"))
                    HttpContext.Current.Session["requestedTab"] = Encoder.HtmlEncode( Request.QueryString["dest"] );
            }
            else if (IsCallCenter)
            {
                Response.Redirect(ResolveUrl("~/CallCenter/Default.aspx"));
            }
            else if (IsAdmin && !IsManagement)
            {
                using (InsertUserLoginHistory iulh = new InsertUserLoginHistory())
                {
                    iulh.UserName = Membership.GetUserNameByEmail(ThisSession.PatientEmail);
                    iulh.Domain = Request.Url.Host;
                    iulh.PostData();
                    if (!iulh.HasErrors && iulh.RowsBack != 1)
                    { }
                }
                Response.Redirect("~/Admin/Default.aspx");
            }
            else if (IsManagement && !IsAdmin)
            {
                Response.Redirect("~/ContentManagement/Default.aspx");
            }
            else if (IsManagement && IsAdmin)
            {
                Response.Redirect("~/AdminPortal/Default.aspx");
            }
        }
        protected void WorkWithSignInTextBoxes()
        {
            using (TextBox userName = (MainLogin.FindControl("UserName") as TextBox))
            {   //Add client side scripting to default the username text to 'Enter email address' if the user doesn't enter anything
                userName.Attributes.Add("onclick", "if(this.value=='Enter email address'){this.value='';}");
                userName.Attributes.Add("onblur", "this.value=!this.value?'Enter email address':this.value;");
            }
            using (TextBox pwdWatermark = (MainLogin.FindControl("txtPasswordWatermark") as TextBox))
            {   //Add client side scripting to allow for password hashing and default the text to 'Enter Password' if the user doesn't enter anything
                using (TextBox Password = (MainLogin.FindControl("Password") as TextBox))
                {
                    pwdWatermark.Attributes.Add("onfocus", "this.style.display = 'none';" +
                        "document.getElementById('" + Password.ClientID + "').style.display = 'block';" +
                        "document.getElementById('" + Password.ClientID + "').focus();");

                    Password.Style.Clear();
                    Password.Style.Add("display", "none");
                    Password.Attributes.Add("onblur", "if(this.value==''){this.style.display='none';" +
                        "document.getElementById('" + pwdWatermark.ClientID + "').style.display = 'block';}");
                }
            }
        }
        protected void LoadUserSessionInfo()
        {
            //Which employer database?
            GetKeyUserInfo gkui = new GetKeyUserInfo(ThisSession.UserLogginID);
            if (!gkui.PutInSession())
            {
                Literal failText = (Literal)MainLogin.FindControl("FailureText");
                failText.Text = gkui.SqlException;
                failText.Visible = true;
            }
        }
        protected void LoadUserEmployerSessionInfo()
        {
            //Which employer database?
            GetKeyEmployeeInfo gkei = new GetKeyEmployeeInfo(MainLogin.UserName.Trim());
            if (!gkei.PutInSession(MainLogin.UserName))
            {
                Literal failText = (Literal)MainLogin.FindControl("FailureText");
                failText.Text = gkei.SqlException;
                failText.Visible = true;
            }
        }
        protected bool LoadUserEmployerSessionInfo(out String lastUrl)
        {
            //Overloaded method to return whether or not the user completed IQ, no need to store in session
            using (GetKeyEmployeeInfo gkei = new GetKeyEmployeeInfo(MainLogin.UserName.Trim()))
            {
                if (!gkei.PutInSession(MainLogin.UserName))
                {
                    Literal failText = (Literal)MainLogin.FindControl("FailureText");
                    failText.Text = gkei.SqlException;
                    failText.Visible = true;
                    lastUrl = "error";
                    return false;
                }
                if (gkei.Tables.Count < 4) { lastUrl = "error"; return false; }
                if (gkei.Tables[3].Rows.Count < 1) { lastUrl = "error"; return false; }
                if (gkei.Tables[3].Rows[0].Field<string>("action") == "completed") { lastUrl = ""; return true; }
                lastUrl = gkei.Tables[3].Rows[0].Field<string>("url");
                return false;
            }
        }
        protected void LoadEmployerContent()
        {
            using (GetEmployerContent gec = new GetEmployerContent(int.Parse(ThisSession.EmployerID)))
            {
                gec.PutInSession();
            }
        }
        protected void MainLogin_LoginError(object sender, EventArgs e)
        {
            MembershipUser mu = Membership.GetUser(MainLogin.UserName) ?? null;

            if (mu != null)
            {
                if (mu.IsLockedOut) MainLogin.FailureText = "You have exceeded the number of attempts to sign in.  Please click on \"Forgot Password\" to set a new password.";
                else if (!mu.IsApproved) MainLogin.FailureText = "You are not currently setup to sign in at this time.";
            }
        }
    }
}