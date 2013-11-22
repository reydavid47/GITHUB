using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.SearchInfo
{
    public partial class search_results_thin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblServiceName.Text = ThisSession.ServiceName.ToString();

                // For office visits, add the specialty in the display name;
                if (ThisSession.ServiceName.StartsWith("Office"))
                    lblServiceName.Text = ThisSession.Specialty + ": " + lblServiceName.Text;

                lblServiceName_MoreInfoTitle.Text = ThisSession.ServiceName.ToString();
                lblServiceMoreInfo.Text = "Magnetic resonance imaging of the knee or any other joint or bone(s) of the lower extremity";
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
    }
}