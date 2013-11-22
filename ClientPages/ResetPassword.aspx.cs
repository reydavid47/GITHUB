using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Net.Mail;
using Microsoft.Security.Application;

namespace ClearCostWeb.ClientPages
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        private String sEmail { get { return (ViewState["sEmail"] == null ? String.Empty : ViewState["sEmail"].ToString()); } set { ViewState["sEmail"] = value; } }
        private String sUserName { get { return (ViewState["sUserName"] == null ? String.Empty : ViewState["sUserName"].ToString()); } set { ViewState["sUserName"] = value; } }
        private String sPassword { get { return (ViewState["sPassword"] == null ? String.Empty : ViewState["sPassword"].ToString()); } set { ViewState["sPassword"] = value; } }
        private String sMemberID { get { return (ViewState["sMemberID"] == null ? String.Empty : ViewState["sMemberID"].ToString()); } set { ViewState["sMemberID"] = value; } }
        private String sSSN { get { return (ViewState["sSSN"] == null ? String.Empty : ViewState["sSSN"].ToString()); } set { ViewState["sSSN"] = value; } }
        private Boolean onlySSN { get { return (ViewState["ssnOnly"] == null ? true : Convert.ToBoolean(ViewState["ssnOnly"])); } set { ViewState["ssnOnly"] = value; } }
        private Int32 empID { get { return (ViewState["empID"] == null ? 0 : Convert.ToInt32(ViewState["empID"])); } set { ViewState["empID"] = value; } }
        private String sEmployerName { get { return (ViewState["sEmployerName"] == null ? String.Empty : ViewState["sEmployerName"].ToString()); } set { ViewState["sEmployerName"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (null != Request.QueryString["e"])
                {
                    using (GetEmployerContent gec = new GetEmployerContent(Convert.ToInt32(Request.QueryString["e"])))
                    {
                        if (!gec.HasErrors && gec.Tables[0].Rows.Count > 0)
                        {
                            tblVerify.FindControl("trMemberID").Visible = tblVerify.FindControl("trOR").Visible = !gec.SSNOnly;
                            imgLogo.ImageUrl = ResolveUrl(String.Concat("~/Images/", gec.LogoImageName));
                            imgLogo.Visible = true;
                            onlySSN = gec.SSNOnly;
                            lblAndMemberID.Visible = !onlySSN;
                            empID = Convert.ToInt32(Request.QueryString["e"]);
                            sEmployerName = gec.EmployerName;
                        }
                    }
                }
                //if (Request.QueryString.Count > 0)
                //{
                //    EmailData eData;
                //    if (Request.QueryString["e"] == null)
                //    {
                //        eData = DecryptInfo(Request.QueryString[0]);
                //    }
                //    else
                //    {
                //        eData = DecryptInfo(Request.QueryString["e"] != null);
                //    }
                //    if (eData.Expiration > DateTime.Now)
                //    {

                //    }
                //}
            }
            else
            {
                if (Request.Params["__EVENTTARGET"].ToString() == "SendEmail")
                {
                    String encrypted = EncryptInfo();

                    string ToAddress = "justin@bluenotecomputing.com";// sUserName; 

                    string fromEmail = "info@clearcosthealth.com";

                    //(1) Create the MailMessage instance
                    MailMessage mm = new MailMessage(fromEmail, ToAddress);

                    //(2) Assign the MailMessage's properties
                    mm.Subject = "Password Reset Link";
                    if (Request.QueryString["e"] != null)
                        mm.Body = Request.Url.ToString() + "&" + encrypted;
                    else
                        mm.Body = Request.Url.ToString() + encrypted;
                    mm.IsBodyHtml = true;

                    //(3) Create the SmtpClient object
                    SmtpClient smtp = new SmtpClient();

                    //(4) Send the MailMessage (will use the Web.config settings)
                    //smtp.Send(mm);
                }
            }
        }
        [Serializable]
        private struct EmailData
        {
            public String UserName;
            public Int32 EmployerID;
            public String SSN;
            public String MemberID;
            public DateTime Expiration;
        }
        private String EncryptInfo()
        {
            IFormatter form = new BinaryFormatter();
            EmailData sd = new EmailData()
            {
                UserName = sUserName,
                EmployerID = empID,
                SSN = sSSN,
                MemberID = sMemberID,
                Expiration = DateTime.Now.AddMinutes(3)
            };

            MemoryStream ser = new MemoryStream();
            form.Serialize(ser, (object)sd);
            ser.Position = 0;

            using (RijndaelManaged rijndaelCipher = new RijndaelManaged())
            {
                PasswordDeriveBytes secretKey = new PasswordDeriveBytes(Encoding.ASCII.GetBytes(sEmployerName), Encoding.ASCII.GetBytes("LetMeReset!"));
                using (ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16)))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(ser.ToArray(), 0, (int)ser.Length);
                            cryptoStream.FlushFinalBlock();
                            string base64 = Convert.ToBase64String(memoryStream.ToArray());
                            string urlEncoded = Microsoft.Security.Application.Encoder.UrlEncode(base64);
                            return urlEncoded;
                        }
                    }
                }
            }
        }
        private EmailData DecryptInfo(String enc)
        {
            IFormatter form = new BinaryFormatter();
            byte[] encryptedData = Convert.FromBase64String(enc);
            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(Encoding.ASCII.GetBytes(sEmployerName), Encoding.ASCII.GetBytes("LetMeReset!"));
            byte[] decryptedData;
            using (RijndaelManaged rijndaelCipher = new RijndaelManaged())
            {
                using (ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16)))
                {
                    using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            decryptedData = new byte[encryptedData.Length];
                            cryptoStream.Read(decryptedData, 0, decryptedData.Length);
                        }
                    }
                }
            }
            using (MemoryStream ser = new MemoryStream(decryptedData, 0, decryptedData.Length))
            {
                dynamic ed = (EmailData)form.Deserialize(ser);
                return new EmailData()
                {
                    EmployerID = ed.EmpoyerID,
                    Expiration = ed.Expiration,
                    MemberID = ed.MemberID,
                    SSN = ed.SSN,
                    UserName = ed.UserName
                };
            }
        }

        private class GetEmployerIDFromEmail : DataSet
        {
            private const CommandType ProcedureType = CommandType.Text;
            private const String query = "SELECT EmployerID FROM UserProfile WHERE Email = '{0}'";
            private String sqlException = String.Empty;
            private String genException = String.Empty;
            private Boolean hasErrors = false;
            private Int32 rowsBack = 0;

            public String Email { get; set; }
            private String Query { get { return String.Format(query, Email); } }
            public String SqlException { get { return sqlException; } }
            public String GenException { get { return genException; } }
            public Boolean HasErrors { get { return this.hasErrors; } }
            public Int32 RowsBack { get { return rowsBack; } }
            public DataTable UserTable { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("EmptyTable")); } }
            public DataRow TopUserRow { get { return (this.rowsBack > 0 ? this.UserTable.Rows[0] : this.UserTable.NewRow()); } }
            public String this[String ColumnName] { get { return this.TopUserRow[ColumnName].ToString(); } }

            public Int32 TopUserEmployerID { get { return Int32.Parse(this["EmployerID"]); } }

            public GetEmployerIDFromEmail()
            { Email = String.Empty; }
            public GetEmployerIDFromEmail(String Email)
            { this.Email = Email; GetFrontEndData(); }

            public void GetFrontEndData()
            {
                using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString))
                {
                    using (SqlCommand comm = new SqlCommand(Query, conn))
                    {
                        comm.CommandType = ProcedureType;
                        using (SqlDataAdapter da = new SqlDataAdapter(comm))
                        {
                            try
                            { rowsBack = da.Fill(this); }
                            catch (SqlException sqEx)
                            { sqlException = sqEx.Message; hasErrors = true; }
                            catch (Exception ex)
                            { genException = ex.Message; hasErrors = true; }
                            finally
                            { }
                        }
                        comm.Parameters.Clear();
                    }
                }
            }
        }
        private class GetMemberMedicalID : DataSet
        {
            private const CommandType ProcedureType = CommandType.Text;
            private const String query = "SELECT MemberMedicalID FROM Enrollments WHERE Email = '{0}'";
            private String sqlException = String.Empty;
            private String genException = String.Empty;
            private Boolean hasErrors = false;
            private Int32 rowsBack = 0;

            public String Email { get; set; }
            public String EmployerConnString { get; set; }
            private String Query { get { return String.Format(query, Email); } }
            public String SqlException { get { return sqlException; } }
            public String GenException { get { return genException; } }
            public Boolean HasErrors { get { return this.hasErrors; } }
            public Int32 RowsBack { get { return rowsBack; } }
            public DataTable UserTable { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("EmptyTable")); } }
            public DataRow TopUserRow { get { return (this.rowsBack > 0 ? this.UserTable.Rows[0] : this.UserTable.NewRow()); } }
            public String this[String ColumnName] { get { return this.TopUserRow[ColumnName].ToString(); } }

            public String TopUserMemberID { get { return this["MemberMedicalID"]; } }

            public GetMemberMedicalID()
            {
                Email = String.Empty;
                EmployerConnString = String.Empty;
            }
            public GetMemberMedicalID(String ProviderConnString, String UserEmail)
            {
                this.Email = UserEmail;
                this.EmployerConnString = ProviderConnString;
                GetData();
            }

            public void GetData()
            {
                using (SqlConnection conn = new SqlConnection(EmployerConnString))
                {
                    using (SqlCommand comm = new SqlCommand(Query, conn))
                    {
                        comm.CommandType = ProcedureType;
                        using (SqlDataAdapter da = new SqlDataAdapter(comm))
                        {
                            try
                            { rowsBack = da.Fill(this); }
                            catch (SqlException sqEx)
                            { sqlException = sqEx.Message; hasErrors = true; }
                            catch (Exception ex)
                            { genException = ex.Message; hasErrors = true; }
                            finally
                            { }
                        }
                        comm.Parameters.Clear();
                    }
                }
            }
        }

        protected void ValidateInput(object sender, EventArgs e)
        {
            //Handle no email entered
            if (Email.Text.Trim() == String.Empty)
            {
                VerifyFailureText.Text = "Email is required.";
                Email.Focus();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ResetCursor", "document.body.style.cursor = 'default';", true);
                return;
            }

            //Handle no SSN nor Member ID
            if (SSN.Text.Trim() == String.Empty && MemberID.Text.Trim() == String.Empty)
            {
                if (onlySSN)
                {
                    VerifyFailureText.Text = "Please enter the last 4 digits of your SSN.";
                    SSN.Focus();
                }
                else
                {
                    VerifyFailureText.Text = "Please enter either the last 4 digits of your SSN or you Member ID.";
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ResetCursor", "document.body.style.cursor = 'default';", true);
                return;
            }

            //Get the Employer Connection String to validate the user
            String cnxString = String.Empty;
            using (GetEmployerConnString gecs = new GetEmployerConnString(empID))
            {
                if (!gecs.HasErrors && gecs.Tables[0].Rows.Count > 0)
                {
                    cnxString = gecs.ConnectionString;
                }
                else
                {
                    VerifyFailureText.Text = "There was an error validating your enrollment.";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ResetCursor", "document.body.style.cursor = 'default';", true);
                    return;
                }
            }

            //Always try to use SSN if it has something in the text box
            Boolean ssnSuccess = false;
            if (SSN.Text.Trim() != String.Empty)
            {
                String cleanSSN = Regex.Replace(SSN.Text, "[^0-9]", "");
                if (cleanSSN.Length == 4)
                {
                    String query = String.Concat(
                        "SELECT MemberSSN FROM Enrollments WHERE Email = '",
                        Email.Text.Trim(),
                        "'");
                    using (BaseCCHData b = new BaseCCHData(query, true))
                    {
                        b.GetData(cnxString);
                        if (!b.HasErrors && b.Tables[0].Rows.Count > 0)
                        {
                            Int32 idFromDB = Convert.ToInt32(b.Tables[0].Rows[0]["MemberSSN"].ToString());
                            if (idFromDB == Convert.ToInt32(cleanSSN))
                            {
                                ssnSuccess = true;
                                sSSN = cleanSSN;
                            }
                        }
                    }
                }
            }

            //If nothing was entered into SSN or if SSN validation failed
            Boolean memberIdSuccess = false;
            if (!ssnSuccess)
            {
                if (MemberID.Text.Trim() != String.Empty)
                {
                    String cleanMemberID = Regex.Replace(MemberID.Text, "[^0-9]", "");
                    if (cleanMemberID.Length == 11)
                    {
                        String query = String.Concat(
                            "SELECT MemberMedicalID FROM Enrollments WHERE Email = '",
                            Microsoft.Security.Application.Encoder.HtmlEncode(Email.Text.Trim()),
                            "'");
                        using (BaseCCHData b = new BaseCCHData(query, true))
                        {
                            b.GetData(cnxString);
                            if (!b.HasErrors && b.Tables[0].Rows.Count > 0)
                            {
                                Int64 idFromDB = Convert.ToInt64(b.Tables[0].Rows[0]["MemberMedicalID"].ToString());
                                if (idFromDB == Convert.ToInt64(cleanMemberID))
                                {
                                    memberIdSuccess = true;
                                }
                            }
                        }
                    }
                }
            }

            if (ssnSuccess || memberIdSuccess)
            {
                sUserName = Membership.GetUserNameByEmail(Microsoft.Security.Application.Encoder.HtmlEncode(Email.Text.Trim()));
                if (String.IsNullOrWhiteSpace(sUserName))
                {
                    VerifyFailureText.Text = "User not found.";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ResetCursor", "document.body.style.cursor = 'default';", true);
                }
                else
                {
                    lblQuestion.Text = Membership.GetUser(Microsoft.Security.Application.Encoder.HtmlEncode(Email.Text.Trim())).PasswordQuestion;
                    tblVerify.Visible = pnlVerify.Visible = false;
                    tblReset.Visible = pnlReset.Visible = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ResetCursor", "document.body.style.cursor = 'default';", true);
                }
            }
            else
            {
                VerifyFailureText.Text = "There was an error resetting your password with the information provided.<br />Please double check the information you entered and try again.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ResetCursor", "document.body.style.cursor = 'default';", true);
            }
        }
        protected void ChangePassword(object sender, EventArgs e)
        {
            MembershipUser mu = Membership.GetUser(Email.Text.Trim());//Setup a membership user object and set it to the one resetting the password for faster processing

            if (PasswordQuestionAnswer.Text.Trim() == String.Empty) { ChangeFailureText.Text = "The correct answer to your security question is required."; }
            else
            {
                if (NewPassword.Text.Trim() != ConfirmNewPassword.Text.Trim()) { ChangeFailureText.Text = "The passwords you entered must match."; }
                else
                {
                    //Do this to make sure that if the user locked out their account they'd have to reset their password to get back in
                    if (mu.IsLockedOut) { mu.UnlockUser(); }
                    String newPwd = String.Empty;
                    try
                    {
                        newPwd = mu.ResetPassword(PasswordQuestionAnswer.Text.Trim());

                        if (!mu.ChangePassword(newPwd, NewPassword.Text.Trim()))
                            ChangeFailureText.Text = "There was an error changing your password. Please try again.";
                        else
                        { tblReset.Visible = pnlReset.Visible = false; tblSuccess.Visible = true; }
                    }
                    catch (ArgumentException ae)
                    {
                        if (ae.Message.ToLower().Contains("the length of parameter"))
                            ChangeFailureText.Text = "Your new password must be at least 8 characters in length.";
                        PasswordQuestionAnswer.Text = String.Empty;
                    }
                    catch (MembershipPasswordException mpe)
                    {
                        if (mpe.Message.ToLower().Contains("answer"))
                            ChangeFailureText.Text = "Your answer to the security question is wrong.";
                        PasswordQuestionAnswer.Text = String.Empty;
                    }
                }
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ResetCursor", "document.body.style.cursor = 'default';", true);
        }
        protected void ContinueToSearch(object sender, EventArgs e)
        {
            Response.Redirect("~/sign_in.aspx");
        }
    }
}