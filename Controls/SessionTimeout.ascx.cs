using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace ClearCostWeb.Controls
{
    public partial class SessionTimeout : System.Web.UI.UserControl
    {
        protected String PingURL
        {
            get
            {
                return ResolveUrl("~/Handlers/KeepAlive.ashx");
            }
        }

        private String timeoutText = "You will be signed out due to inactivity in:";
        public String TimeOutText { get { return timeoutText; } set { timeoutText = value; } }
        private String navURL = "~/Sign_In.aspx?timeout";
        public String NavURL { get { return Page.ResolveUrl(navURL); } set { navURL = value; } }
        private Int32 timeoutOverride = 0;
        public Int32 TimeoutLength { get { return timeoutOverride; } set { timeoutOverride = value; } }
        private Boolean publicFacing = false;
        public Boolean PublicFacing { get { return publicFacing; } set { publicFacing = value; } }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            JavaScriptHelper.IncludeSessionTimeout(Page.ClientScript);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.AddCSSToHeader("SessionTimeout.css", publicFacing);

            ltlTimeoutText.Text = timeoutText;

            if (timeoutOverride > 0)
            {
                TIMEOUT.Value = ((timeoutOverride * 60) - 60).ToString();
                TIMETILNOTIFY.Value = "60";
            }
            else
            {
                TIMEOUT.Value = ((Session.Timeout * 60) - 60).ToString();
                TIMETILNOTIFY.Value = "120";
            }
        }
    }
}