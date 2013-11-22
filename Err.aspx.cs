using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace ClearCostWeb.Public
{
    public partial class Err : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                    {
                        htw.RenderBeginTag(HtmlTextWriterTag.H2);
                        htw.Write("We regret to report that something went wrong processing your request.");
                        htw.WriteBreak();
                        htw.Write("Please feel free to try what you were doing again in a few minutes.");
                        htw.RenderEndTag();
                        htw.WriteBreak();
                        htw.RenderBeginTag(HtmlTextWriterTag.H3);

                        MembershipUser mu = Membership.GetUser();

                        if (mu != null)
                            if (mu.IsOnline)
                                htw.Write("Click here to log back in and try again.");
                            else
                                htw.Write("Click here to try logging in again.");
                        else
                            htw.Write("Click here to continue to the sign-in page.");

                        htw.RenderEndTag();
                        htw.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl("~/Sign_in.aspx"));
                        htw.AddAttribute(HtmlTextWriterAttribute.Class, "submitlink");
                        htw.RenderBeginTag(HtmlTextWriterTag.A);
                        htw.Write("Continue");
                        htw.RenderEndTag();

                        if (Roles.IsUserInRole("DebugUser"))
                        {
                            htw.WriteBreak();
                            htw.WriteBreak();
                            htw.RenderBeginTag(HtmlTextWriterTag.Table);
                            htw.RenderBeginTag(HtmlTextWriterTag.Tr);
                            htw.AddAttribute(HtmlTextWriterAttribute.Width, "110px");
                            htw.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "Bold");
                            htw.RenderBeginTag(HtmlTextWriterTag.Td);
                            htw.Write("From Page:");
                            htw.RenderEndTag(); //TD
                            htw.RenderBeginTag(HtmlTextWriterTag.Td);
                            htw.Write(Request.UrlReferrer.PathAndQuery);
                            htw.RenderEndTag(); //TD
                            htw.RenderEndTag(); //TR
                            htw.RenderBeginTag(HtmlTextWriterTag.Tr);
                            htw.AddAttribute(HtmlTextWriterAttribute.Width, "110px");
                            htw.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "Bold");
                            htw.RenderBeginTag(HtmlTextWriterTag.Td);
                            htw.Write("Error Message:");
                            htw.RenderEndTag(); //TD
                            htw.RenderBeginTag(HtmlTextWriterTag.Td);
                            htw.Write(ThisSession.AppException.Message);
                            htw.RenderEndTag(); //TD
                            htw.RenderEndTag(); //TR
                            htw.RenderBeginTag(HtmlTextWriterTag.Tr);
                            htw.AddAttribute(HtmlTextWriterAttribute.Width, "110px");
                            htw.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "Bold");
                            htw.RenderBeginTag(HtmlTextWriterTag.Td);
                            htw.Write("Error Source:");
                            htw.RenderEndTag(); //TD
                            htw.RenderBeginTag(HtmlTextWriterTag.Td);
                            htw.Write(ThisSession.AppException.Source);
                            htw.RenderEndTag(); //TD
                            htw.RenderEndTag(); //TR
                            htw.RenderBeginTag(HtmlTextWriterTag.Tr);
                            htw.AddAttribute(HtmlTextWriterAttribute.Width, "110px");
                            htw.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "Bold");
                            htw.RenderBeginTag(HtmlTextWriterTag.Td);
                            htw.Write("Error Stack:");
                            htw.RenderEndTag(); //TD
                            htw.RenderBeginTag(HtmlTextWriterTag.Td);
                            htw.Write(ThisSession.AppException.StackTrace);
                            htw.RenderEndTag(); //TD
                            htw.RenderEndTag(); //TR
                            htw.RenderEndTag(); //Table
                        }
                        using (BaseCCHData errorHandler = new BaseCCHData())
                        {
                            errorHandler.CaptureError(ThisSession.AppException, (mu != null && !mu.IsOnline));
                        }
                    }

                    ltlErrorMessage.Text = sw.ToString();
                }
                HttpContext.Current.Session.Abandon();
                FormsAuthentication.SignOut();
            }
        }
    }
}