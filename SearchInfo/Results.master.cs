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
using ClearCostWeb.Controls;

namespace ClearCostWeb.SearchInfo
{
    public partial class Results : System.Web.UI.MasterPage
    {
        public event EventHandler SubmitEvent;

        public int SessionLengthSeconds { get { return (Session.Timeout * 60); } }
        public String SessionExpirationDestinationUrl { get { return ResolveUrl(String.Format("~/{0}/Sign_in.aspx?timeout=true", ThisSession.EmployerName)); } }
        protected String SecurityQuestion { get { return Membership.GetUser(ThisSession.PatientEmail).PasswordQuestion; } }
        protected String CurrentQuestion
        {
            get
            {
                return (ViewState["CurrentQuestion"] == null ? String.Empty : ViewState["CurrentQuestion"].ToString());
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
            //Adds the compile version to the end of any links to css or javascript files forcing the browser to get the latest version upon first visit
            //foreach (var ctrl in Page.Header.Controls)
            //    if (ctrl is HtmlLink) //Updates all .css files with .css?Rev=<VERSION>
            //        ((HtmlLink)ctrl).Href = String.Concat(
            //            ((HtmlLink)ctrl).Href,
            //            "?Rev=", Session["Version"].ToString());
            //    else if (ctrl is LiteralControl)//Updates all .js files with .js?Rev=<VERSION>
            //        ((LiteralControl)ctrl).Text = ((LiteralControl)ctrl).Text.Replace(".js\"", String.Format(".js?Rev={0}\"", Session["Version"].ToString()));


            base.OnPreRender(e);
            JavaScriptHelper.IncludeJQuery(Page.ClientScript);
            JavaScriptHelper.IncludeDropdown(Page.ClientScript);
            JavaScriptHelper.IncludeProduction(Page.ClientScript);
            JavaScriptHelper.IncludeResultMaster(Page.ClientScript);
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
            paMaster.loadPage += new PatientAddress.OnSubmit(OnSubmit);

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetNoServerCaching();

            liSavingsChoice.Visible = ThisSession.SavingsChoiceEnabled && ThisSession.ShowSCIQTab;  //  lam, 20130816, SCIQ-77
            //liSavingsChoice.Visible = ThisSession.SavingsChoiceEnabled;  lam, 20130816, SCIQ-77
            MakeVisible(ThisSession.SelectedTab);

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
            }
            prepUI();
            //if (!Page.ClientScript.IsClientScriptIncludeRegistered("Timeout"))
            //{
            //    if (Request.Url.Host.ToLower().Contains("localhost"))
            //        Page.ClientScript.RegisterClientScriptInclude("Timeout", ResolveUrl("~/Scripts/Timeout.js?Rev=") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //    else
            //        Page.ClientScript.RegisterClientScriptInclude("Timeout", ResolveUrl("~/Scripts/Timeout.min.js?Rev=") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //}

            //if (!Page.ClientScript.IsClientScriptIncludeRegistered("Timeout"))
            //{
            //    if (Request.Url.Host.ToLower().Contains("localhost"))
            //        Page.ClientScript.RegisterClientScriptInclude("Timeout", ResolveUrl("~/Scripts/Timeout.js?") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //    else
            //        Page.ClientScript.RegisterClientScriptInclude("Timeout", ResolveUrl("~/Scripts/Timeout.min.js?") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //}

            //String startScript = ("$('body').TimeoutTimer({ TimeTilTimeout: {0}, TimeTilNotify: 120, NavUrl: '{1}' });").Replace("{0}", (Session.Timeout * 60).ToString()).Replace("{1}", ResolveUrl("~/Sign_in.aspx"));
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "SetTimeout", startScript, true);
        }
        protected void ChangeTab(object sender, EventArgs e)
        {
            switch (((LinkButton)sender).ID)
            {
                case "lbtnDashboard":
                    MakeVisible(ThisSession.AvailableTab.SavingsChoiceDashboard);
                    break;
                case "lbtnFindAService":
                    MakeVisible(ThisSession.AvailableTab.FindAService);
                    break;
                case "lbtnFindRX":
                    MakeVisible(ThisSession.AvailableTab.FindRx);
                    break;
                case "lbtnFindADoc":
                    MakeVisible(ThisSession.AvailableTab.FindADoc);
                    break;
                case "lbtnPastCare":
                    MakeVisible(ThisSession.AvailableTab.FindPastCare);
                    break;
            }
            Response.Redirect(ResolveUrl("~/SearchInfo/Search.aspx"));
        }
        protected void MakeVisible(ThisSession.AvailableTab at)
        {            
            //Setup our default settings for the new CSS Classes the control will receive
            String inactiveTab = "ui-state-default ui-corner-all",
                activeTab = "ui-state-default ui-corner-top ui-tabs-selected ui-state-active";

            //Make visible the panel we need and add the active class to only to the tab we want
            switch (at)
            {
                case ThisSession.AvailableTab.SavingsChoiceDashboard:
                    ThisSession.SelectedTab = ThisSession.AvailableTab.SavingsChoiceDashboard;
                    liSavingsChoice.Attributes["class"] = activeTab;
                    break;
                case ThisSession.AvailableTab.FindAService:
                    ThisSession.SelectedTab = ThisSession.AvailableTab.FindAService;
                    liFindAService.Attributes["class"] = activeTab;
                    break;
                case ThisSession.AvailableTab.FindRx:
                    ThisSession.SelectedTab = ThisSession.AvailableTab.FindRx;
                    liFindRx.Attributes["class"] = activeTab;
                    break;
                case ThisSession.AvailableTab.FindADoc:
                    ThisSession.SelectedTab = ThisSession.AvailableTab.FindADoc;
                    liFindADoc.Attributes["class"] = activeTab;
                    break;
                case ThisSession.AvailableTab.FindPastCare:
                    ThisSession.SelectedTab = ThisSession.AvailableTab.FindPastCare;
                    liPastCare.Attributes["class"] = activeTab;
                    break;
            }
        }

        protected string PrimaryCCHID, EmployerID, SessionID;
        protected void prepUI()
        {
            //if (ThisSession.ShowSCIQTab)
            //{
                PrimaryCCHID = ThisSession.CCHID.ToString();
                EmployerID = ThisSession.EmployerID.ToString();
                SessionID = Session.SessionID.ToString();
            //}
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

        protected void OnSubmit(bool isSubmitted)
        {
            if (SubmitEvent != null)
            {
                SubmitEvent(this, EventArgs.Empty);
            }
        }
    }
}
