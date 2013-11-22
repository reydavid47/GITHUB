using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI.HtmlControls;

namespace ClearCostWeb.Public
{
    public partial class SignIn : System.Web.UI.MasterPage
    {
        protected string cssSkin { get { return CssHelper.IncludeSkinStyle(this.Page); } }
        //protected override void OnPreRender(EventArgs e)
        //{
        //    //VERY IMPORT FOR CHANGE CONTROL
        //    //Adds the compile version to the end of any links to css or javascript files forcing the browser to get the latest version upon first visit
        //    foreach (var ctrl in Page.Header.Controls)
        //        if (ctrl is HtmlLink) //Updates all .css files with .css?Rev=<VERSION>
        //            ((HtmlLink)ctrl).Href = String.Concat(
        //                ((HtmlLink)ctrl).Href,
        //                "?Rev=",
        //                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        //        else if (ctrl is LiteralControl)//Updates all .js files with .js?Rev=<VERSION>
        //            ((LiteralControl)ctrl).Text = ((LiteralControl)ctrl).Text.Replace(".js\"", String.Format(".js?Rev={0}\"", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));


        //    base.OnPreRender(e);
        //}
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}