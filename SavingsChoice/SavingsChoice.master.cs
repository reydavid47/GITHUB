using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.SavingsChoice
{
    public partial class SavingsChoice : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
        }
    }
}