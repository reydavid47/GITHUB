using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.Controls
{
    public partial class GoogleMap : System.Web.UI.UserControl, ICallbackEventHandler
    {
        private string latitude = "44.49075";
        private string longitude = "-73.111095";

        public string Latitude { get { return latitude; } }
        public string Longitude { get { return longitude; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScriptManager cm = Page.ClientScript;
            String cbRef = cm.GetCallbackEventReference(this, "arg", "ReceiveServerData", "");
            String callbackScript = "function CallServer(arg, context) {" + cbRef + "; }";
            cm.RegisterClientScriptBlock(this.GetType(), "CallServer", callbackScript, true);
        }

        #region ICallbackEventHandler Members

        public string GetCallbackResult()
        {
            return latitude + ',' + longitude;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {

        }

        #endregion
    }
}