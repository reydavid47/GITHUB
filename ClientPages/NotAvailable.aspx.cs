using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Security.Application;

namespace ClearCostWeb.ClientPages
{
    public partial class NotAvailable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (null != Request.QueryString["Client"] && Request.QueryString["Client"].Trim() != String.Empty)
            {
                lblClient.Text = Encoder.HtmlEncode(Request.QueryString["Client"].ToString());
                lblFor.Visible = true;
            }
        }
    }
}