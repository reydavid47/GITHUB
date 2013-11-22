using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Security;
using System.Web.UI.HtmlControls;

namespace ClearCostWeb.ClientPages
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
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
            if (Request.Browser.Browser.ToLower() == "ie" && (Request.Browser.MajorVersion == 6 || Request.Browser.MajorVersion == 7))
                ltlCompatabilityWarning.Text = "<center><div class=\"compatWarn\">It appears you are using Internet Explorer " + Request.Browser.MajorVersion + ".  We do not actively support this older browser.<br />We suggest you upgrade your browser for the best experience using ClearCost Health.</div></center>";
            if (!Page.IsPostBack)
            {
                //IE8 doesn't support the target="_newtab" so just draw navigation links for now as there is the posibility for pop up blockers at Starbucks
                StringWriter sw = new StringWriter();
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.AddAttribute(HtmlTextWriterAttribute.Id, "botbar");
                    htw.RenderBeginTag(HtmlTextWriterTag.Div);                          //<div id="botbar">
                    htw.AddAttribute(HtmlTextWriterAttribute.Id, "demobotnav");
                    htw.AddStyleAttribute("float", "left");
                    htw.RenderBeginTag(HtmlTextWriterTag.Div);                          //  <div style="float:left;" id="demobotnav">
                    htw.AddAttribute(HtmlTextWriterAttribute.Href, "AboutUs.aspx");
                    htw.RenderBeginTag(HtmlTextWriterTag.A);                            //      <a href="AboutUs.aspx" target="_newtab">
                    htw.Write("About Us");                                              //          About Us
                    htw.RenderEndTag();                                                 //      </a>
                    htw.AddAttribute(HtmlTextWriterAttribute.Src, ResolveUrl("~/Images/nav_div.gif"));
                    htw.AddAttribute(HtmlTextWriterAttribute.Alt, String.Empty);
                    htw.AddAttribute(HtmlTextWriterAttribute.Width, "1");
                    htw.AddAttribute(HtmlTextWriterAttribute.Height, "17");
                    htw.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                    htw.RenderBeginTag(HtmlTextWriterTag.Img);                          //      <img src="../Images/nav_div.gif" alt="" width="1" height="17" border="0">
                    htw.RenderEndTag();                                                 //      </img>
                    htw.AddAttribute(HtmlTextWriterAttribute.Href, "Contact_Us.aspx");
                    htw.RenderBeginTag(HtmlTextWriterTag.A);                            //      <a href="contact_us.aspx" target="_newtab">
                    htw.Write("Contact Us");                                            //          Contact Us
                    htw.RenderEndTag();                                                 //      </a>
                    if (Membership.GetUser() != null && Membership.GetUser().IsOnline)
                    {
                        htw.AddAttribute(HtmlTextWriterAttribute.Src, ResolveUrl("~/Images/nav_div.gif"));
                        htw.AddAttribute(HtmlTextWriterAttribute.Alt, String.Empty);
                        htw.AddAttribute(HtmlTextWriterAttribute.Width, "1");
                        htw.AddAttribute(HtmlTextWriterAttribute.Height, "17");
                        htw.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                        htw.RenderBeginTag(HtmlTextWriterTag.Img);                          //      <img src="../Images/nav_div.gif" alt="" width="1" height="17" border="0">
                        htw.RenderEndTag();                                                 //      </img>
                        htw.AddAttribute(HtmlTextWriterAttribute.Href, "FAQ.aspx");
                        htw.RenderBeginTag(HtmlTextWriterTag.A);                            //      <a href="FAQ.aspx" target="_newtab">
                        htw.Write("FAQs");                                                  //          FAQs
                        htw.RenderEndTag();                                                 //      </a>
                    }
                    htw.RenderEndTag();                                                 //  </div>
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "copyright");
                    htw.RenderBeginTag(HtmlTextWriterTag.P);                            //  <p class="copyright">
                    htw.Write("&copy;&nbsp;ClearCost Health. All Rights Reserved.");    // &copy; ClearCost Health. All Rights Reserved.
                    htw.RenderEndTag();                                                 //  </p>
                    htw.RenderEndTag();                                                 //</div>
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "clearboth");
                    htw.RenderBeginTag(HtmlTextWriterTag.Div);                          //<div class="clearboth">
                    htw.RenderEndTag();                                                 //</div>
                }
                ltlBotBar.Text = sw.ToString();
            }
        }
    }
}
