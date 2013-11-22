using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.ClientPages
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (GetEmployerContent gec = new GetEmployerContent())
                {
                    if (null != Request.QueryString["e"])
                        gec.EmployerID = Convert.ToInt32(Request.QueryString["e"]);
                    gec.GetFrontEndData();
                    if (!gec.HasErrors)
                    {
                        imgLogo.ImageUrl = "../images/" + gec.LogoImageName;
                        imgLogo.Visible = (gec.LogoImageName != String.Empty);
                        StarbucksSignIn.Visible = gec.CanSignIn;
                        if (!StarbucksSignIn.Visible)
                            lbtnRegister.Visible = gec.CanRegister;
                    }
                }
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