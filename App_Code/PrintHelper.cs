using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using System.Web.SessionState;

namespace ClearCostWeb
{
    public class PrintHelper
    {
        public PrintHelper()
        { }

        public static void PrintWebControl(Control ctrl)
        {
            PrintWebControl(ctrl, string.Empty);
        }
        public static void PrintWebControl(Control ctrl, string Script)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
            if (ctrl is WebControl)
            {
                Unit w = new Unit(100, UnitType.Percentage); ((WebControl)ctrl).Width = w;
            }
            //ctrl.RenderControl(hw);
            String strHTML = sb.ToString();
            Page pg = new Page();
            pg.EnableEventValidation = false;
            if (Script != string.Empty)
            {
                pg.ClientScript.RegisterStartupScript(pg.GetType(), "PrintJavaScript", Script);
            }
            HtmlForm frm = new HtmlForm();
            frm.Attributes.Add("runat", "server");
            frm.Controls.Add(ctrl);
            pg.Controls.Add(frm);
            pg.DesignerInitialize();
            pg.RenderControl(hw);
            //string strHTML = stringWrite.ToString();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(strHTML);
            HttpContext.Current.Response.Write("<script>window.print();</script>");
            HttpContext.Current.Response.End();
        }
    }
}