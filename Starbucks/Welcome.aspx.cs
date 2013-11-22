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
using System.Security.Cryptography;
using System.IO;
using QSEncryption.QSEncryption;
using Microsoft.Security.Application;

namespace ClearCostWeb.Starbucks
{
    public partial class Welcome : System.Web.UI.Page
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

        private String HoldUser { get { return (ViewState["HoldUser"] == null ? String.Empty : ViewState["HoldUser"].ToString()); } set { ViewState["HoldUser"] = value; } }
        private String ConnString { get { return (ViewState["CnxString"] == null ? String.Empty : ViewState["CnxString"].ToString()); } set { ViewState["CnxString"] = value; } }
        private String EmployerID { get { return (ViewState["EmployerID"] == null ? String.Empty : ViewState["EmployerID"].ToString()); } set { ViewState["EmployerID"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Browser.Browser.ToLower() == "ie" && Request.Browser.MajorVersion == 6)
                ltlCompatabilityWarning.Text = "<center><div class=\"compatWarn\">It appears you are using Internet Explorer 6.  We do not actively support this older browser.<br />We suggest you upgrade your browser for the best experience using ClearCost Health.</div></center>";
            if (!Page.IsPostBack)
            {
                HoldUser = string.Empty;
                GetReferral(); //Check for Refferel and set session Values
            }

        }

        protected void GetReferral()
        {
            string strReq = "";
            strReq = Request.RawUrl;
            if (Request.QueryString != null && Request.QueryString.Count > 0)
            {
                strReq = strReq.Substring(strReq.IndexOf('?') + 1);
                try
                {
                    strReq = Decrypt(strReq, "r0b1nr0y");
                    //Parse the value... this is done is very raw format..
                    //you can add loops or so to get the values out of the query string...
                    string[] arrMsgs = strReq.Split('&');
                    string[] arrIndMsg;
                    int ReffralID;
                    arrIndMsg = arrMsgs[0].Split('='); //Get the Name
                    ReffralID = Convert.ToInt32(arrIndMsg[1].ToString().Trim()); //Convert.ToInt32(Request.QueryString["CCHID"]);
                    ThisSession.ReffralCCHID = ReffralID;
                }
                catch (Exception)
                {

                }
            }
            else
            {
                ThisSession.ReffralCCHID = 0;
            }
        }
        private byte[] key = { };
        private byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
        public string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {

            byte[] inputByteArray = new byte[stringToDecrypt.Length + 1];

            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(sEncryptionKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms,
                  des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
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
            //Get the connection string for the specific database if it isn't already in the view state
            if (ConnString == String.Empty)
                using (GetEmployerConnString gecs = new GetEmployerConnString("Starbucks"))
                    if (!gecs.HasErrors)
                    {
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
                gee.SSN = "zzzz";
                if (txtPremera1.Text != "XXXXXXX" && txtPremera2.Text != "XX") //From this point validation has already passed so this indicates they are using the member id
                    gee.MemberID = Encoder.HtmlEncode(txtPremera1.Text) + Encoder.HtmlEncode(txtPremera2.Text);
                else
                    gee.SSN = Encoder.HtmlEncode(txtSSN.Text);
                gee.DOB = String.Format("{2}-{0}-{1}", txtMonth.Text, txtDay.Text, txtYear.Text);
                gee.GetData(ConnString);

                if (!gee.HasErrors)
                {
                    if (gee.EmployeeTable.TableName != "Empty" && gee.EmployeeTable.Rows.Count > 0)
                    {
                        lblNotFound.Visible = false;
                        lblError.Visible = false;

                        ThisSession.CCHID = gee.CCHID;
                        ThisSession.EmployeeID = gee.EmployeeID;
                        ThisSession.SubscriberMedicalID = gee.SubscriberMedicalID;
                        ThisSession.SubscriberRXID = gee.SubscriberRXID;
                        ThisSession.LastName = gee.LastName;
                        ThisSession.FirstName = gee.FirstName;
                        ThisSession.PatientAddress1 = gee.Address1;
                        ThisSession.PatientAddress2 = gee.Address2;
                        ThisSession.PatientCity = gee.City;
                        ThisSession.PatientState = gee.State;
                        ThisSession.PatientZipCode = gee.ZipCode;
                        ThisSession.PatientLatitude = gee.Latitude;
                        ThisSession.PatientLongitude = gee.Longitude;
                        ThisSession.PatientDateOfBirth = gee.DOB;
                        ThisSession.PatientPhone = gee.Phone;
                        ThisSession.HealthPlanType = gee.HealthPlanType;
                        ThisSession.MedicalPlanType = gee.MedicalPlanType;
                        ThisSession.RxPlanType = gee.RxPlanType;
                        ThisSession.PatientGender = gee.Gender;
                        ThisSession.Parent = gee.Parent;
                        ThisSession.Adult = gee.Adult;
                        ThisSession.PatientEmail = gee.Email;

                        if (gee.Insurer != String.Empty)
                            ThisSession.Insurer = gee.Insurer;
                        if (gee.RXProvider != String.Empty)
                            ThisSession.RXProvider = gee.RXProvider;

                        if (gee.DependentTable.TableName != "EmptyTable")
                        {
                            Dependents deps = new Dependents();
                            Dependent dep = null;

                            gee.ForEachDependent(delegate(DataRow dr)
                            {
                                dep = new Dependent();
                                dep.CCHID = int.Parse(dr["CCHID"].ToString());
                                dep.FirstName = dr["FirstName"].ToString();
                                dep.LastName = dr["LastName"].ToString();
                                dep.DateOfBirth = DateTime.Parse(dr["DateOfBirth"].ToString());
                                dep.Age = int.Parse(dr["Age"].ToString());
                                dep.IsAdult = int.Parse(dr["Adult"].ToString()) == 1 ? true : false;
                                dep.ShowAccessQuestions = int.Parse(dr["ShowAccessQuestions"].ToString()) == 1 ? true : false;
                                dep.RelationshipText = dr["RelationshipText"].ToString();
                                dep.DepToUserGranted = int.Parse(dr["DepToUserGranted"].ToString()) == 1 ? true : false;
                                dep.UserToDepGranted = int.Parse(dr["UserToDepGranted"].ToString()) == 1 ? true : false;
                                dep.Email = dr["Email"].ToString();

                                deps.Add(dep);
                            });
                            ThisSession.Dependents = deps;
                        }
                        if (gee.YouCouldHaveSavedTable.TableName != "EmptyTable")
                            ThisSession.YouCouldHaveSaved = (int)gee.YouCouldHaveSaved;
                        Response.Redirect("Review.aspx");
                    }
                    else
                    { pnlReg.Visible = false; pnlCapture.Visible = true; } //Membership not found
                }
                else
                { lblError.Visible = true; }//General error validating
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
                if (txtPremera1.Text != "XXXXXXX" && txtPremera2.Text != "XX") ier.MemberID = txtPremera1.Text + txtPremera2.Text;
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
            String tPremera1 = txtPremera1.Text.ToLower().Trim(); int iPremera1 = 0; Boolean bPremera1 = false;
            String tPremera2 = txtPremera2.Text.ToLower().Trim(); int iPremera2 = 0; Boolean bPremera2 = false;

            bSSN = int.TryParse(tSSN, out iSSN);
            if (bSSN) bSSN = iSSN > 0;
            bPremera1 = int.TryParse(tPremera1, out iPremera1);
            if (bPremera1) bPremera1 = iPremera1 > 0;
            bPremera2 = int.TryParse(tPremera2, out iPremera2);
            if (bPremera2) bPremera2 = iPremera2 > 0;

            if (args.IsValid && (bSSN && bPremera1 && bPremera2))
            {
                //If they entered both an SSN and a Premera ID
                //Default to the member ID and validate that it is long enough
                args.IsValid = (tPremera1.Length == 9 && tPremera2.Length == 2);
                if (!args.IsValid) args.IsValid = (tSSN.Length == 4); //Fail back to the ssn if they entered both but the member id was not a full length
            }
            else if (args.IsValid && (bSSN && (!bPremera1 || !bPremera2)))
            {
                //If they entered an SSN and either/both of the Premera ID fields are still text incase they started with the Premera ID
                //Validate that the ssn is at least 4 characters
                args.IsValid = (tSSN.Length == 4);
            }
            else if (args.IsValid && (!bSSN && (bPremera1 && bPremera2)))
            {
                //If they left the ssn as text and entered something in both the Premera fields other than text
                //Validate that the member id is long enough
                args.IsValid = (tPremera1.Length == 9 && tPremera2.Length == 2);
            }
            else { args.IsValid = false; } //They didn't enter valid information in any of the 3 fields
        }
        protected void ValidateDOB(object sender, ServerValidateEventArgs args)
        {
            int DigitalValidator = 0; DateTime DateValidator = new DateTime();
            String Month = txtMonth.Text;
            String Day = txtDay.Text;
            String Year = txtYear.Text;

            args.IsValid = int.TryParse(Month, out DigitalValidator);
            if (args.IsValid) args.IsValid = (DigitalValidator > 0);
            if (args.IsValid) args.IsValid = int.TryParse(Day, out DigitalValidator);
            if (args.IsValid) args.IsValid = (DigitalValidator > 0);
            if (args.IsValid) args.IsValid = int.TryParse(Year, out DigitalValidator);
            if (args.IsValid) args.IsValid = (DigitalValidator > 0);

            if (args.IsValid) args.IsValid = DateTime.TryParse(String.Format("{0}/{1}/{2}", Month, Day, Year), out DateValidator);
            if (args.IsValid) args.IsValid = ((new DateTime(1800, 1, 1)) < DateValidator && DateValidator < DateTime.Now);
        }
        protected void ValidateEmail(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = (args.Value.Trim() != string.Empty); //Validate that they entered something and didn't leave it blank
            if (args.IsValid) args.IsValid = (args.Value.Contains('.') && args.Value.Contains("@")); //Validate that the input they entered has at least an @ and a . symbol in it
        }
    }
}