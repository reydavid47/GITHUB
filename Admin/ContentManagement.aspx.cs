using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.Admin
{
    public partial class ContentManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (GetEmployersForCallCenter gefcc = new GetEmployersForCallCenter())
                {
                    if (!gefcc.HasErrors)
                    {
                        ddlEmployers.DataSource = gefcc.Tables[0];
                        ddlEmployers.DataBind();
                    }
                }
            }
        }
        protected void lbtnShowFAQCM_Click(object sender, EventArgs e)
        {
            pnlFAQ.Visible = pnlChoseEmployer.Visible = true;
            using (GetFAQItemsForWeb gfiw = new GetFAQItemsForWeb())
            {
                if (Convert.ToInt32(ddlEmployers.SelectedItem.Value) == 0)
                {
                    gfiw.GetFrontEndData();
                    //if (!gfiw.HasErrors)
                        //gvFAQItems.DataSource = gfiw.Tables[0];
                }
                else
                {
                    gfiw.EmployerID = Convert.ToInt32(ddlEmployers.SelectedItem.Value);
                    gfiw.GetFrontEndData();
                    //if (!gfiw.HasErrors)
                        //gvFAQItems.DataSource = gfiw.Tables[1];
                }
                //gvFAQItems.DataBind();
            }
        }
    }
}