using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.Controls
{
    public partial class unavPlaceHolder : System.Web.UI.UserControl
    {
        private String[] pageNames = { "home", "pricetransparency", "expertise", "services", "aboutus", "contact_us" };
        private String[] pageLinks = { "../Index.aspx", "../PriceTransparency.aspx", "../Expertise.aspx", "../Services.aspx", "../AboutUs.aspx", "../contact_us.aspx" };
        private String[] linkText = { "Home", "Price Transparency", "Our Expertise", "Our Service", "Our Team", "Contact Us" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.Request.Url.ToString().ToLower().Contains("sign_in"))
            {
                SetupLinks();
            }
        }

        private void SetupLinks()
        {
            Panel pnlContent = unavContent;
            HyperLink linkToAdd;

            Image navDiv;

            Int32 controlsToAdd = 0;
            if (Page.Request.Url.ToString().ToLower().Contains("index")) { controlsToAdd++; } //We don't want to show the Home link if we are on home
            for (Int32 i = controlsToAdd; i < pageNames.Length; i++)
            {
                linkToAdd = new HyperLink();
                linkToAdd.NavigateUrl = pageLinks[i];
                linkToAdd.Text = linkText[i];
                if (Page.Request.Url.ToString().ToLower().Contains(pageNames[i]))
                {
                    linkToAdd.Attributes.Add("class", "unavon");
                }
                pnlContent.Controls.Add(linkToAdd);

                if (i < (pageNames.Length - 1))
                {
                    navDiv = new Image();
                    navDiv.ImageUrl = "../Images/nav_div.gif";
                    navDiv.AlternateText = string.Empty;
                    navDiv.Width = 1;
                    navDiv.Height = 17;
                    navDiv.BorderWidth = 0;
                    pnlContent.Controls.Add(navDiv);
                }
            }
        }
    }
}