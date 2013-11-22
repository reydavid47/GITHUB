using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.ClientPages
{
    public partial class FAQ : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                using (GetFAQItemsForWeb gfiw = new GetFAQItemsForWeb())
                {
                    if (null != Request.QueryString["e"])
                        gfiw.EmployerID = Convert.ToInt32(Request.QueryString["e"]);
                    if (gfiw.Tables.Count == 0)
                        gfiw.GetFrontEndData();
                    if (!gfiw.HasErrors)
                    {
                        rptFAQs.DataSource = gfiw.Tables[0];
                        rptFAQs.DataBind();
                        if (gfiw.Tables.Count == 2)
                        {
                            ClientSpecificFAQ.DataSource = gfiw.Tables[1];
                            ClientSpecificFAQ.DataBind();
                        }
                    }
                }
            }
        }
    }
}