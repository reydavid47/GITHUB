using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Data;
using System.Web.Security;
using QSEncryption.QSEncryption;
using System.Web.UI.HtmlControls;
using Microsoft.Security.Application;

namespace ClearCostWeb.CallCenter
{
    public partial class Default : System.Web.UI.Page
    {
        public int SessionLengthSeconds { get { return (Session.Timeout * 60); } }
        public String SessionExpirationDestinationUrl { get { return ResolveUrl("~/Sign_in.aspx?timeout=true"); } }
        private GetEmployersForCallCenter Employers
        {
            get
            {
                return (HttpContext.Current.Session["Employers"] == null ?
                    new GetEmployersForCallCenter() :
                    (GetEmployersForCallCenter)HttpContext.Current.Session["Employers"]);
            }
            set { HttpContext.Current.Session["Employers"] = value; }
        }
        private GetEmployeesByLastNameForCallCenter Employees
        {
            get
            {
                return (HttpContext.Current.Session["Employees"] == null ?
                    new GetEmployeesByLastNameForCallCenter() :
                    (GetEmployeesByLastNameForCallCenter)HttpContext.Current.Session["Employees"]);
            }
            set { HttpContext.Current.Session["Employees"] = value; }
        }
        private GetEmployeeByCCHIDForCallCenter Employee
        {
            get
            {
                return (HttpContext.Current.Session["Employee"] == null ?
                    new GetEmployeeByCCHIDForCallCenter(0) :
                    (GetEmployeeByCCHIDForCallCenter)HttpContext.Current.Session["Employee"]);
            }
            set { HttpContext.Current.Session["Employee"] = value; }
        }

        private String passedCCHID = String.Empty;
        private String passedEmployerID = String.Empty;

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
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["srch"] != null)
                    SetupShortSearch();
                else
                    SetupLongSearch();
            }
        }
        private void SetupShortSearch()
        {
            pnlLongSearch.Visible = false;
            pnlShortSearch.Visible = true;

            try
            {
                QueryStringEncryption qse = new QueryStringEncryption(
                    Encoder.HtmlEncode(Request.QueryString["srch"].ToString()), 
                    (Guid)Membership.GetUser().ProviderUserKey);

                passedCCHID = qse["CCHID"];
                passedEmployerID = qse["EmployerID"];

                lblEmployerFromSrch.Text = String.Format("{0}", passedEmployerID);
                lblEmployeeIDFromSrch.Text = String.Format("{0}", passedCCHID);

                DataRow dr = (from employer in Employers.Tables[0].AsEnumerable()
                              where employer.Field<int>("EmployerID") == int.Parse(passedEmployerID)
                              select employer).FirstOrDefault();
                ThisSession.CnxString = dr[2].ToString();
                ThisSession.EmployerID = dr[0].ToString();
                ThisSession.EmployerName = dr[1].ToString();
                ThisSession.Insurer = dr[3].ToString();
                ThisSession.RXProvider = dr[4].ToString();
                ThisSession.ShowYourCostColumn = Convert.ToBoolean(dr[5].ToString());
                using (GetEmployerContent gec = new GetEmployerContent(int.Parse(passedEmployerID)))
                    gec.PutInSession();

                Employee = new GetEmployeeByCCHIDForCallCenter(Convert.ToInt32(passedCCHID));
                Employee.GetData();
                if (!Employee.HasErrors)
                {
                    gvUsers.DataSource = Employee.Employee;
                    gvUsers.DataBind();                    
                }
                else
                { SetupLongSearch(); }
            }
            catch (Exception ex)
            { SetupLongSearch(); ltlMessage.Text = "<div>" + ex.Message + "</div>"; }
        }
        private void SetupLongSearch()
        {
            pnlLongSearch.Visible = true;
            pnlShortSearch.Visible = false;

            if (Roles.IsUserInRole(Membership.GetUser().UserName, "Admin")) { lblListTitle.Text = "List of employees with that last name:"; }
            else { lblListTitle.Text = "List of registered users:"; }

            if (!Employers.HasErrors)
            {
                ddlEmployers.Items.Clear();
                ddlEmployers.DataSource = Employers.Tables[0];
                ddlEmployers.DataBind();
                ddlEmployers.Items.Insert(0, "Select an Employer");
            }
        }
        protected void SelectLetter(object sender, EventArgs e)
        {
            gvUsers.SelectedIndex = -1;
            gvUsers.DataSource = null;
            gvUsers.DataBind();
            lbConfirm.Enabled = false; lbConfirm.Visible = false; lbConfirm.Text = String.Empty;

            String cnString = String.Empty;
            Employers.ForeEachEmployer(delegate(Int32 EmployerID, String ConnectionString)
            {
                if (EmployerID == Convert.ToInt32(ddlEmployers.SelectedItem.Value))
                    cnString = ConnectionString;
            });

            Employees = new GetEmployeesByLastNameForCallCenter();
            Employees.Letter = ((LinkButton)sender).Text;
            if (Roles.IsUserInRole(Membership.GetUser().UserName, "Admin")) { Employees.Admin = true; }
            Employees.GetData(cnString);
            if (!Employees.HasErrors)
            {

                ddlEmployees.Items.Clear();
                ddlEmployees.DataSource = Employees.DistinctLastNames;
                ddlEmployees.DataBind();
                ddlEmployees.Items.Insert(0, "Select Employee Last Name");
            }
        }
        protected void ChooseLastName(object sender, EventArgs e)
        {
            gvUsers.DataSource = Employees.ByLastName(ddlEmployees.SelectedItem.Text);
            gvUsers.DataBind();
            gvUsers.SelectedIndex = -1;
            lbConfirm.Enabled = false; lbConfirm.Visible = false; lbConfirm.Text = String.Empty;
        }
        protected void ChooseEmployer(object sender, EventArgs e)
        {
            gvUsers.SelectedIndex = -1;
            gvUsers.DataSource = null;
            gvUsers.DataBind();
            ddlEmployees.DataSource = null;
            ddlEmployees.DataBind();
            lbConfirm.Enabled = false; lbConfirm.Visible = false; lbConfirm.Text = String.Empty;

            pnlLetters.Enabled = (ddlEmployers.SelectedItem.Text != "Select Employer");
            DataRow dr = Employers.Tables[0].Rows[ddlEmployers.SelectedIndex - 1];
            ThisSession.CnxString = dr[2].ToString();
            ThisSession.EmployerID = dr[0].ToString();
            ThisSession.EmployerName = dr[1].ToString();
            ThisSession.Insurer = dr[3].ToString();
            ThisSession.RXProvider = dr[4].ToString();
            ThisSession.ShowYourCostColumn = Convert.ToBoolean(dr[5].ToString());
            using (GetEmployerContent gec = new GetEmployerContent(int.Parse(ThisSession.EmployerID)))
            {
                gec.PutInSession();
            }
        }
        protected void SelectEmployee(object sender, EventArgs e)
        {
            lbConfirm.Visible = true;
            if (gvUsers.SelectedIndex > -1)
            {
                IOrderedDictionary vals = gvUsers.SelectedDataKey.Values;
                ThisSession.PatientEmail = vals[0].ToString();
                String FN = vals[1].ToString(); String LN = vals[2].ToString(); String MMId = vals[3].ToString();

                Boolean Enrolled = false;
                if (Employee == null || Employee.Tables.Count == 0)
                    Enrolled = Employees.IsSelectedRegistered(MMId);
                else
                    Enrolled = Employee.IsSelectedRegistered(MMId);

                lbConfirm.Enabled = Enrolled;
                if (Enrolled)
                    lbConfirm.Text = String.Format("Confirm selection and continue as {0} {1} ({2})", FN, LN, MMId);
                else
                    lbConfirm.Text = String.Format("User {0} {1} ({2}) is not currently registered.", FN, LN, MMId);
            }
            else { lbConfirm.Enabled = false; lbConfirm.Text = "Please Choose an employee to continue."; }
        }
        protected void ContinueAsEmployee(object sender, EventArgs e)
        {
            if (gvUsers.SelectedIndex > -1)
            {
                LoadUserEmployerSessionInfo();

                using (GetPasswordQuestions gpq = new GetPasswordQuestions())
                {
                    if (!gpq.PutInSession())
                    {
                        ThisSession.CurrentAvailableSecurityQuestions = new[] { "none" };
                    }
                }

                using (InsertUserLoginHistory iulh = new InsertUserLoginHistory())
                {
                    iulh.UserName = Membership.GetUserNameByEmail(ThisSession.PatientEmail);
                    iulh.CallCenterID = Membership.GetUser().ProviderUserKey.ToString();
                    iulh.Domain = Request.Url.Host;
                    iulh.PostData();
                    if (!iulh.HasErrors && iulh.RowsBack != 1)
                    {
                        //In the event this fails or either 0 or more than one row is effected
                        //NOTIFY LAURA :)
                    }
                    ThisSession.UserLogginID = Membership.GetUser().ProviderUserKey.ToString();
                    ThisSession.LoggedIn = true;
                }
                Response.Redirect("~/SearchInfo/Search.aspx#tabcare");
            }
        }
        protected void LoadUserEmployerSessionInfo()
        {
            //Which employer database?
            GetKeyEmployeeInfo gkei = new GetKeyEmployeeInfo(ThisSession.PatientEmail);
            if (!gkei.HasErrors)
            {
                gkei.PutInSession(ThisSession.PatientEmail);
            }
        }
    }
}