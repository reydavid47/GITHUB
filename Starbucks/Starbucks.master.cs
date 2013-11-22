using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Security;

namespace ClearCostWeb.Starbucks
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Browser.Browser.ToLower() == "ie" && Request.Browser.MajorVersion == 6)
                ltlCompatabilityWarning.Text = "<center><div class=\"compatWarn\">It appears you are using Internet Explorer 6.  We do not actively support this older browser.<br />We suggest you upgrade your browser for the best experience using ClearCost Health.</div></center>";
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
                    htw.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl("AboutUs.aspx"));
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
                    htw.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl("Contact_Us.aspx"));
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
                        htw.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl("FAQ.aspx"));
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
