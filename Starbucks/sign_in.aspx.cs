﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.Starbucks
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string test = Request.Browser.Platform;
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
    }
}