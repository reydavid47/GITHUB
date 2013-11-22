using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.ClientPages
{
    public partial class landing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsMobileBrowser()) { Response.Redirect("https://mobile.clearcosthealth.com"); }
            if (Request.Browser.Browser.ToLower() == "ie" && (Request.Browser.MajorVersion == 6 || Request.Browser.MajorVersion == 7))
                ltlCompatabilityWarning.Text = "<center><div class=\"compatWarn\">It appears you are using Internet Explorer " + Request.Browser.MajorVersion + ".  We do not actively support this older browser.<br />We suggest you upgrade your browser for the best experience using ClearCost Health.</div></center>";
            if (!Page.IsPostBack)
            {
                if (null != Request.QueryString["e"])
                {
                    using (GetEmployerContent gec = new GetEmployerContent(Convert.ToInt32(Request.QueryString["e"])))
                    {
                        if (!gec.HasErrors)
                        {
                            imgLogo.ImageUrl = "../Images/" + gec.LogoImageName;
                            imgLogo.Visible = (gec.LogoImageName != String.Empty);
                            loginregister.Visible = ltlDisclamer.Visible = (gec.CanSignIn || gec.CanRegister);
                            ltlBreaker.Visible = (gec.CanSignIn && gec.CanRegister);
                            pnlSignIn.Visible = gec.CanSignIn;
                            pnlRegister.Visible = pnlRegNow.Visible = pnlRegNow2.Visible = gec.CanRegister;
                            if (ltlDisclamer.Visible)
                                ltlDisclamer.Text = String.Format(ltlDisclamer.Text, gec.InsurerName, gec.Tables[0].Rows[0]["EmployerName"].ToString());

                            if (!gec.CanRegister && !gec.CanSignIn)
                                pnlSignInNow.Visible = pnlRegNow.Visible = false;
                            else if (gec.CanRegister && !gec.CanSignIn)
                            {
                                pnlSignInNow.Visible = false;
                                pnlRegNow.Visible = true;
                            }
                            else if (gec.CanRegister && gec.CanSignIn)
                            {
                                pnlRegNow.Visible = pnlSignInNow.Visible = false;
                                if (gec.OverrideRegisterButton)
                                {
                                    pnlSignIn.CssClass += " centric";
                                    pnlSignInNow.Visible = true;
                                }
                                else
                                {
                                    pnlRegister.CssClass += " centric";
                                    pnlRegNow.Visible = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}