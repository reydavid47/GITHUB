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
using Microsoft.Security.Application;

namespace ClearCostWeb.AnalogDevices
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
            ltlStyleSheets.Text = CssStyleSheets;

            if (!Page.IsPostBack)
            {
                HoldUser = string.Empty;

                //Get the ID from the query string.
                string unregisteredEmpID = Encoder.HtmlEncode( Request.QueryString["emp"] );

                GetUserInfoFromEmployerDB(unregisteredEmpID);
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
            //Get the connection string for the specific database if it isn't already in the view state
            if (ConnString == String.Empty)
                using (GetEmployerConnString gecs = new GetEmployerConnString("AnalogDevices")) //2 = Analog Devices
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
                gee.MemberID = Encoder.HtmlEncode(txtMemID.Text);
                gee.DOB = Encoder.HtmlEncode(txtDOB.Text);
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
                        //ThisSession.FirstName = gee.Firstname;
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
                    { lblNotFound.Visible = true; }
                }
                else
                { lblError.Visible = true; }
            }
        }


        protected void GetUserInfoFromEmployerDB(string unregisteredEmpID)
        {
            GetEmployeeWelcomeInfo gewi = new GetEmployeeWelcomeInfo(unregisteredEmpID);
            if (!gewi.HasErrors)
            {
                //lblLastName.Text = gewi.EmployeeLastname;
                //lblFirstName.Text = gewi.EmployeeFirstName;
                //TextBox userName = (TextBox)CheckEmpIDLogin.FindControl("UserName");
                //userName.Text = gewi.EmployeeEmail;
                HoldUser = gewi.EmployeeEmail;
            }
            else
            {
                //lblLastName.Text = "User";
                //lblFirstName.Text = "Unrecognized";
                //Literal failText = (Literal)CheckEmpIDLogin.FindControl("FailureText");
                //failText.Text = "There was an error retrieving the employee ID you entered.  Please try again."; //gewi.SqlException;
                //failText.Visible = true;
            }
        }

        protected void CheckEmpIDLogin_LoggedIn(object sender, EventArgs e)
        {
            //Validate employee number. Log the user in? We will start with their Employee ID being their password.

            //Go to the review page.

            //if (Roles.IsUserInRole(CheckEmpIDLogin.UserName.Trim(), "Customer"))
            if (Roles.IsUserInRole(HoldUser, "Customer"))
            {
                string serverName = Encoder.UrlEncode(Request.ServerVariables["SERVER_NAME"]);
                string vdirName = Request.ApplicationPath;

                ThisSession.UserLogginID = Membership.GetUser(HoldUser).ProviderUserKey.ToString();

                LoadUserSessionInfo();
                LoadUserEmployerSessionInfo();

                String dest = "";
                if (ThisSession.EmployerName.ToLower().Contains("starbucks"))
                    dest += "/Starbucks";
                else if (ThisSession.EmployerName.ToLower().Contains("analog") && ThisSession.EmployerName.ToLower().Contains("devices"))
                    dest += "/AnalogDevices";

                dest += "/Account/review.aspx";

                if (serverName == "localhost")
                {
                    Response.Redirect("~" + dest);
                }
                else
                { Response.Redirect("https://" + serverName + vdirName + dest); }
            }

        }
        protected void LoadUserSessionInfo()
        {
            //Which employer database?
            GetKeyUserInfo gkui = new GetKeyUserInfo(ThisSession.UserLogginID);
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
        protected void LoadUserEmployerSessionInfo()
        {
            //Which employer database?
            GetKeyEmployeeInfo gkei = new GetKeyEmployeeInfo(HoldUser);
            if (!gkei.HasErrors)
            {
                ThisSession.CCHID = gkei.CCHID;
                ThisSession.EmployeeID = gkei.EmployeeID;
                ThisSession.SubscriberMedicalID = gkei.SubscriberMedicalID;
                ThisSession.SubscriberRXID = gkei.SubscriberRXID;
                ThisSession.LastName = gkei.LastName;
                ThisSession.FirstName = gkei.FirstName;
                ThisSession.PatientAddress1 = gkei.Address1;
                ThisSession.PatientAddress2 = gkei.Address2;
                ThisSession.PatientCity = gkei.City;
                ThisSession.PatientState = gkei.State;
                ThisSession.PatientZipCode = gkei.ZipCode;
                ThisSession.PatientLatitude = gkei.Latitude;
                ThisSession.PatientLongitude = gkei.Longitude;
                ThisSession.PatientDateOfBirth = gkei.DateOfBirth;
                ThisSession.PatientPhone = gkei.Phone;
                ThisSession.HealthPlanType = gkei.HealthPlanType;
                ThisSession.MedicalPlanType = gkei.MedicalPlanType;
                ThisSession.RxPlanType = gkei.RxPlanType;
                ThisSession.PatientGender = gkei.Gender;
                ThisSession.Parent = gkei.Parent;
                ThisSession.Adult = gkei.Adult;
                ThisSession.PatientEmail = HoldUser;

                if (gkei.Insurer != String.Empty)
                    ThisSession.Insurer = gkei.Insurer;
                if (gkei.RXProvider != String.Empty)
                    ThisSession.RXProvider = gkei.RXProvider;

                if (gkei.DependentTable.TableName != "EmptyTable")
                {
                    Dependents deps = new Dependents();
                    Dependent dep = null;

                    gkei.ForEachDependent(delegate(DataRow dr)
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

                if (gkei.YouCouldHaveSavedTable.TableName != "EmptyTable")
                    ThisSession.YouCouldHaveSaved = (int)gkei.YouCouldHaveSaved;
            }
            else
            {
                //Literal failText = (Literal)CheckEmpIDLogin.FindControl("FailureText");
                //failText.Text = gkei.SqlException;
                //failText.Visible = true;
            }
        }
    }
}