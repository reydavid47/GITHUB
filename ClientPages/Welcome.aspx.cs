using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.Security.Application;

namespace ClearCostWeb.ClientPages
{
    public partial class Welcome : System.Web.UI.Page
    {
        private String CssStyleSheets
        {
            get
            {
                String template = @"<link href=""{0}"" rel=""Stylesheet"" type=""text/css"" /><link href=""{1}"" rel=""Stylesheet"" type=""text/css"" />";

                return String.Format(template,
                    ResolveUrl("~/Styles/skin.css?Rev=") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
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

        private String HoldUser { get { return (ViewState["HoldUser"] == null ? String.Empty : ViewState["HoldUser"].ToString()); } set { ViewState["HoldUser"] = value; } }
        private String ConnString { get { return (ViewState["CnxString"] == null ? String.Empty : ViewState["CnxString"].ToString()); } set { ViewState["CnxString"] = value; } }
        private String EmployerID { get { return (ViewState["EmployerID"] == null ? String.Empty : ViewState["EmployerID"].ToString()); } set { ViewState["EmployerID"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsMobileBrowser() || (Request.QueryString.AllKeys.Contains("asMob") && Request.QueryString["asMob"] == "true"))
                Page.Header.Controls.Add(new LiteralControl(MobileStyles));
            else
                Page.Header.Controls.Add(new LiteralControl(LandingStyles));

            ltlCompatabilityWarning.Text = Request.CompatabilityWarning();                    

            if (!Page.IsPostBack)
            {
                Int32 localEmployerID = Request.EmployerID<Int32>();
                if (localEmployerID > 0)
                {
                    using (GetEmployerContent gec = new GetEmployerContent())
                    {
                        gec.EmployerID = localEmployerID;
                        gec.GetFrontEndData();
                        if (!gec.HasErrors &&
                            gec.Tables.Count > 0 &&
                            gec.Tables[0].Rows.Count > 0)
                        {
                            loginregister.Visible = gec.CanSignIn;
                            imgLogo.ImageUrl = ResolveUrl("~/images/" + gec.LogoImageName);
                            imgLogo.Visible = (gec.LogoImageName != String.Empty);
                            ltlAltID.Text = String.Format(ltlAltID.Text, gec.InsurerName);
                            if (!gec.SSNOnly)
                                ltlAltID.Visible = (gec.InsurerName != String.Empty);
                            if (gec.SSNOnly)
                                cvSSNorID.ErrorMessage = cvSSNorID.ToolTip = "Please enter a valid number for your SSN.";
                            else
                                cvSSNorID.ErrorMessage = String.Format(cvSSNorID.ErrorMessage, " or " + gec.InsurerName + " ID.");
                            pnlAdditionalID.Visible = !gec.SSNOnly;
                            hfEmployerIDFromURL.Value = gec.Tables[0].Rows[0]["EmployerName"].ToString();
                            //ltlUnavailable.Text = String.Format(ltlUnavailable.Text,
                            //    gec.InsurerName, gec.Tables[0].Rows[0]["EmployerName"].ToString());
                            ltlUnavailable.Text = string.Format(ltlUnavailable.Text, gec.FormattedPhoneNumber);
                            lblNotFound.Text = String.Format(lblNotFound.Text, gec.PhoneNumber);
                            cvAGE.ErrorMessage = String.Format(cvAGE.ErrorMessage, StandardizedPhoneNumber(gec.PhoneNumber), gec.EmployerName + "@clearcosthealth.com");
                        }
                    }
                }                
                HoldUser = string.Empty;
                if (Request.IsTimeout())
                {
                    ltlFailure.Text = "You've arrived on this page because your previous attempt at registering timed out due to inactivity.<br />Please begin again.";
                    ltlFailure.Visible = true;
                }
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void Continue(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            ////Dictionary<Int32, Tuple<String, String>> connections = null;
            //Get the connection string for the specific database if it isn't already in the view state
            if (ConnString == String.Empty)
                using (GetEmployerConnString gecs = new GetEmployerConnString(hfEmployerIDFromURL.Value))
                    if (!gecs.HasErrors)
                    {
                        ////if (gecs.EmployerRows.Length > 1)
                        ////{
                        ////    connections = new Dictionary<int, Tuple<string, string>>();
                        ////    gecs.ForEach(delegate(object[] row)
                        ////    {
                        ////        connections.Add(Convert.ToInt32(row[0]),
                        ////            new Tuple<string, string>(row[1].ToString(), row[2].ToString()));
                        ////    });
                        ////}
                        ConnString = gecs.ConnectionString;
                        EmployerID = gecs.EmployerID.ToString();
                    }

            if (ConnString != String.Empty)
                ThisSession.CnxString = ConnString;
            if (EmployerID != String.Empty)
                ThisSession.EmployerID = EmployerID;

            //Check if the employee exists and if they do, store session info and move on
            using (GetEmployeeEnrollment gee = new GetEmployeeEnrollment())
            {
                //gee.Firstname = txtFirstName.Text;
                gee.LastName = Encoder.HtmlEncode(txtLastName.Text);
                if (!String.IsNullOrWhiteSpace(cmidfWelcome.MemberIDText.Replace("X","")))
                    gee.MemberID = cmidfWelcome.MemberIDText;
                else
                    gee.SSN = Encoder.HtmlEncode(txtSSN.Text);
                //if(txtPremera1.Text != "XXXXXXX" && txtPremera2.Text != "XX") //From this point validation has already passed so this indicates they are using the member id
                //gee.MemberID = txtPremera1.Text + txtPremera2.Text;
                //else
                //gee.SSN = txtSSN.Text;
                gee.DOB = String.Format("{0}-{2}-{1}", Encoder.HtmlEncode(txtYear.Text), Encoder.HtmlEncode(txtDay.Text), Encoder.HtmlEncode(txtMonth.Text));// txtMonth.Text, txtDay.Text, txtYear.Text));
                gee.GetData(ConnString);

                if (gee.PutInSession())
                {
                    lblNotFound.Visible = lblError.Visible = false;

                    //////JM 11/28/12 (TO BE REMOVED 9/16/13)
                    //////For Sanmina change the employer id and cnxstring to the BCBS version if this employee is not an AETNA insured employee
                    ////if (hfEmployerIDFromURL.Value.Trim().ToLower().Contains("sanmina"))
                    ////{
                    ////    if (ThisSession.MedicalPlanType.ToLower().Contains("bcbs"))
                    ////    {
                    ////        ThisSession.EmployerID = connections.Single(cnn => cnn.Value.Item2.ToLower().Contains("bcbs")).Key.ToString();
                    ////        ThisSession.CnxString = connections[Convert.ToInt32(ThisSession.EmployerID)].Item1;
                    ////    }
                    ////}

                    if (Debugger.IsAttached && Request.QueryString["e"] != null)
                        Response.Redirect("Review.aspx?e=" + Encoder.HtmlEncode( Request.QueryString["e"]) );
                    else
                        Response.Redirect("Review.aspx");
                }
                else
                { // Membership not found
                    pnlReg.Visible = false;
                    pnlCapture.Visible = true;
                }
            }
        }
        protected void CaptureEmail(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            using (InsertEnrollmentRequest ier = new InsertEnrollmentRequest())
            {
                ier.LastName = Encoder.HtmlEncode(txtLastName.Text);
                ier.DateOfBirth = new DateTime(Convert.ToInt16(txtYear.Text), Convert.ToInt16(txtMonth.Text), Convert.ToInt16(txtDay.Text));
                ier.Email = Encoder.HtmlEncode(txtCapEmail.Text);
                //if (txtPremera1.Text != "XXXXXXX" && txtPremera2.Text != "XX") ier.MemberID = txtPremera1.Text + txtPremera2.Text;
                if (txtSSN.Text != "XXXX") ier.SSN = Encoder.HtmlEncode(txtSSN.Text);
                ier.PostData();
                if (!ier.HasErrors)
                    ScriptManager.RegisterStartupScript(pnlCapture,
                        pnlCapture.GetType(),
                        "ShowOverlay",
                        "$('<div />').addClass('whitescreen').appendTo('body').show();$('div#successoverlay').show();setTimeout('document.location=\"landing.aspx\"',5000);",
                        true);
            }
        }
        protected void ValidateEmpty(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = (args.Value.Trim() != string.Empty); //Validate that they entered something and didn't leave it blank
        }
        protected void ValidateSSNorID(object sender, ServerValidateEventArgs args)
        {
            String tSSN = txtSSN.Text.ToLower().Trim(); int iSSN = 0; Boolean bSSN = false;

            bSSN = int.TryParse(tSSN, out iSSN);
            if (bSSN) bSSN = iSSN > 0;

            args.IsValid = (bSSN || this.cmidfWelcome.IsValid);
        }
        protected void ValidateDOB(object sender, ServerValidateEventArgs args)
        {
            int DigitalValidator = 0; 
            DateTime DateValidator = new DateTime();
            String Month = txtMonth.Text, 
                Day = txtDay.Text, 
                Year = txtYear.Text;

            args.IsValid = false;
            if (int.TryParse(Month, out DigitalValidator)) args.IsValid = (DigitalValidator > 0); else return;
            if (int.TryParse(Day, out DigitalValidator)) args.IsValid = (DigitalValidator > 0); else return;
            if (int.TryParse(Year, out DigitalValidator)) args.IsValid = (DigitalValidator > 0); else return;
            if (DateTime.TryParse(String.Format("{0}/{1}/{2}", Month, Day, Year), out DateValidator))
                args.IsValid = (
                    (new DateTime(1800, 1, 1) < DateValidator &&
                    DateValidator < DateTime.Now));
            else return;
            
        }
        protected void ValidateAge(object sender, ServerValidateEventArgs args)
        {
            //JM 6/26/13
            // -- Added a validation step to make sure that the user is 18 or older
            DateTime DateValidator = new DateTime(
                    txtYear.Text.To<int>(), 
                    txtMonth.Text.To<int>(), 
                    txtDay.Text.To<int>());

            Double DifInDays = (DateTime.Today - DateValidator).TotalDays,
                EighteenInDays = (DateTime.Today - DateTime.Today.AddYears(-18)).TotalDays;

            args.IsValid = false;
            if (DifInDays > 0 && EighteenInDays > 0)
                args.IsValid = (DifInDays >= EighteenInDays);
            else
                return;
        }
        protected void ValidateEmail(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = (args.Value.Trim() != string.Empty); //Validate that they entered something and didn't leave it blank
            if (args.IsValid) args.IsValid = (args.Value.Contains('.') && args.Value.Contains("@")); //Validate that the input they entered has at least an @ and a . symbol in it
        }

        private string StandardizedPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber
                .Replace("-", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")",string.Empty)
                .Replace(".",string.Empty)
                .Trim();
            if (phoneNumber.Length < 10) return string.Empty;
            return Regex.Replace(phoneNumber, @"(\d{3})(\d{3})(\d{4})", "$1-$2-$3");
        }
    }
}