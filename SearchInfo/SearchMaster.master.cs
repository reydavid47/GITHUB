using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;

namespace ClearCostWeb.SearchInfo
{
    public partial class SearchMaster : System.Web.UI.MasterPage
    {
        public int SessionLengthSeconds { get { return (Session.Timeout * 60); } }
        public String SessionExpirationDestinationUrl { get { return ResolveUrl(String.Format("~/{0}/Sign_in.aspx?timeout=true", ThisSession.EmployerName)); } }
        protected String SecurityQuestion { get { return Membership.GetUser(ThisSession.PatientEmail).PasswordQuestion; } }
        private String CurrentQuestion
        {
            get
            {
                return (ViewState["CurrentQuestion"] == null ? String.Empty : ((String)ViewState["CurrentQuestion"]));
            }
            set { ViewState["CurrentQuestion"] = value; }
        }
        protected String FormattedPatientPhone
        {
            get
            {
                String phoneOut = ThisSession.PatientPhone;
                int phoneAsInt = 0;
                if (int.TryParse(ThisSession.PatientPhone, out phoneAsInt)) //Phone number looks like 5555555555
                {
                    int areaCode = phoneAsInt / 10000000;
                    int lastFour = phoneAsInt % 10000;
                    int mid = (phoneAsInt / 10000) % 1000;
                    phoneOut = String.Format("({0}) {1}-{2}", areaCode, mid, lastFour);
                }
                return phoneOut;
            }
        }
        protected Boolean ShowAccessQuestions { get { return ThisSession.Dependents.ShowAccessQuestionSave; } }

        protected override void OnPreRender(EventArgs e)
        {
            //VERY IMPORT FOR CHANGE CONTROL
            base.OnPreRender(e);
            //JavaScriptHelper.IncludeJQueryTabs(Page.ClientScript);
            JavaScriptHelper.IncludeJQuery(Page.ClientScript);
            JavaScriptHelper.IncludeDropdown(Page.ClientScript);
            JavaScriptHelper.IncludeProduction(Page.ClientScript);            
            JavaScriptHelper.IncludeSearchMaster(Page.ClientScript);
            if ((bool?)HttpContext.Current.Session["benchmark"] ?? false)
            {
                HtmlMeta m = new HtmlMeta();
                m.Attributes["http-equiv"] = "X-UA-Compatible";
                m.Attributes["content"] = "IE=edge";
                Page.Header.Controls.AddAt(0, m);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //Page.AddCSSToHeader("SearchMaster.css");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetNoServerCaching();

            internalLogo.Visible = ThisSession.InternalLogo;
            if (ThisSession.InternalLogo)
                internalLogo.ImageUrl = ResolveUrl(String.Concat("~/Images/", ThisSession.LogoImageName));
            amSearchMaster.ContactText = ThisSession.ContactText;

            ltlAboutUs.Text = String.Format("<a href=\"{0}\">About Us</a>",
                ResolveUrl(String.Format("../{0}/AboutUs.aspx",
                    ThisSession.EmployerName)
                )
            );
            ltlContactUs.Text = String.Format("<a href=\"{0}\">Contact Us</a>",
                ResolveUrl(String.Format("../{0}/Contact_Us.aspx",
                    ThisSession.EmployerName)
                )
            );
            ltlFAQ.Text = String.Format("<a href=\"{0}\">FAQs</a>",
                ResolveUrl(String.Format("../{0}/FAQ.aspx",
                    ThisSession.EmployerName)
                )
            );
            if (!Page.IsPostBack)
            {
                lblEmployeeName.Text = ThisSession.FirstName.ToUpper() + " " + ThisSession.LastName.ToUpper();

                //The LargerMap page also uses the Search Master template but we don't want to clear all session vars there so only do it if we're on the search page
                if (!Request.Url.PathAndQuery.ToLower().Contains("largermap"))
                    ThisSession.ClearSearchSessionVariables();


            }

            prepUI();

            //if (!Page.ClientScript.IsClientScriptIncludeRegistered("Timeout"))
            //{
            //    if (Request.Url.Host.ToLower().Contains("localhost"))
            //        Page.ClientScript.RegisterClientScriptInclude("Timeout", ResolveUrl("~/Scripts/Timeout.js?Rev=") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //    else
            //        Page.ClientScript.RegisterClientScriptInclude("Timeout", ResolveUrl("~/Scripts/Timeout.min.js?Rev=") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //}

            //String startScript = ("$('body').TimeoutTimer({ TimeTilTimeout: {0}, TimeTilNotify: 120, NavUrl: '{1}' });").Replace("{0}", (Session.Timeout * 60).ToString()).Replace("{1}", ResolveUrl("~/Sign_in.aspx"));
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "SetTimeout", startScript, true);
        }
        protected void AuditIQRestart(object sender, EventArgs e)
        {
            using (CreateSCIQAuditTrail SCIQAudit = new CreateSCIQAuditTrail())
            {
                SCIQAudit.CCHID = ThisSession.CCHID;
                SCIQAudit.SessionID = Session.SessionID;
                SCIQAudit.Action = "restart";
                SCIQAudit.Category = null;
                SCIQAudit.URL = null;
                SCIQAudit.SCIQ_FLG = true;
                SCIQAudit.PostData();

                var btn = (LinkButton)sender;
                Response.Redirect("~/SavingsChoice/" + btn.CommandArgument);
            }
        }

        //protected string PrimaryCCHID, EmployerID, SessionID;
        protected void prepUI() {
            if (ThisSession.SavingsChoiceEnabled) {
                //PrimaryCCHID = ThisSession.CCHID.ToString();
                //EmployerID = ThisSession.EmployerID.ToString();
                //SessionID = Session.SessionID.ToString();
                using (SC_GetLastSCIQUrl glsu = new SC_GetLastSCIQUrl())
                {
                    glsu.CCHID = ThisSession.CCHID;
                    glsu.GetData();
                    if (glsu.Tables.Count > 0 && glsu.Tables[0].Rows.Count > 0 && glsu.Tables[0].Rows[0][0].ToString().Trim() != string.Empty)
                    {
                        restartSCIQURL.CommandArgument = glsu.Tables[0].Rows[0][0].ToString();
                        restartSCIQWrapper.Visible = true;
                    }
                }
            }
        }
        protected string getOverAllScore()
        {
            return "<div class='overallDivider'>&nbsp;</div><div class='overallText'>Your Savings Choice Score: </div><div class='overallScore'>62%</div>";
        }
        protected string getSCIQURL()
        {
            string returnURL = "";
            using (SC_GetLastSCIQUrl sciqURL = new SC_GetLastSCIQUrl())
            {
                sciqURL.CCHID = ThisSession.CCHID;
                sciqURL.GetData();
                if ((sciqURL.getLastSCIQURL != null) && (sciqURL.getLastSCIQURL.Rows.Count > 0))
                {
                    DataRow dr = sciqURL.getLastSCIQURL.Rows[0];
                    returnURL = dr["url"].ToString();
                }
            }
            //returnURL = "test";
            return returnURL;
        }

        //Very important!
        protected void Logout(object sender, EventArgs e)
        {
            //Do not delete
            HttpContext.Current.Session.Abandon();
        }
    }

    public static class LiteralControlExtensions
    {
        public static void AddRevisionControl(this LiteralControl lc)
        {
            String rv = String.Concat("?Rev=", HttpContext.Current.Session["Version"].ToString(), "\"");
            lc.Text = lc.Text.Replace(".js\"", ".js" + rv).Replace(".css\"", ".css" + rv);
        }
    }
}