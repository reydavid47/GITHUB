using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.Scripts
{
    public partial class SavingsChoice_JS : System.Web.UI.Page
    {
        protected String Category
        {
            get
            {
                return ViewState["Category"].ToString();
            }
            set
            {
                ViewState["Category"] = value;
            }
        }
        protected int EmployerID
        {
            get
            {
                return int.Parse(ThisSession.EmployerID);
            }
        }
        protected int PrimaryCCHID
        {
            get
            {
                return ThisSession.CCHID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.QueryString["cat"] != null &&
                HttpContext.Current.Request.QueryString["cat"].ToString().Trim() != string.Empty)
            {
                switch (HttpContext.Current.Request.QueryString["cat"].ToString())
                {
                    case "Rx":
                        Category = HttpContext.Current.Request.QueryString["cat"].ToString();
                        break;
                }
            }
        }
    }
}