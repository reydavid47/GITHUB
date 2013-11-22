using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace ClearCostWeb.Public
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        private String[] pageNames = { "home", "pricetransparency", "expertise", "services", "aboutus", "contact_us" };
        private String[] pageLinks = { "Index.aspx", "PriceTransparency.aspx", "Expertise.aspx", "Services.aspx", "AboutUs.aspx", "contact_us.aspx" };
        private String[] linkText = { "Home", "Price Transparency", "Our Expertise", "Our Service", "Our Team", "Contact Us" };

        protected override void OnPreRender(EventArgs e)
        {
            //VERY IMPORT FOR CHANGE CONTROL
            //Adds the compile version to the end of any links to css or javascript files forcing the browser to get the latest version upon first visit
            foreach (var ctrl in Page.Header.Controls)
                if (ctrl is HtmlLink) //Updates all .css files with .css?Rev=<VERSION>
                    ((HtmlLink)ctrl).Href = String.Concat(
                        ((HtmlLink)ctrl).Href,
                        "?Rev=",
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                else if (ctrl is LiteralControl)//Updates all .js files with .js?Rev=<VERSION>
                    ((LiteralControl)ctrl).Text = ((LiteralControl)ctrl).Text.Replace(".js\"", String.Format(".js?Rev={0}\"", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));


            base.OnPreRender(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            SetupLinks();
        }
        private void SetupLinks()
        {
            Panel pnlContent = botnavcontent;
            HyperLink linkToAdd;
            LiteralControl space;

            for (Int32 i = 0; i < pageNames.Length; i++)
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
                    space = new LiteralControl();
                    space.Text = "&nbsp;|&nbsp;";
                    pnlContent.Controls.Add(space);
                }
            }
        }
    }
}