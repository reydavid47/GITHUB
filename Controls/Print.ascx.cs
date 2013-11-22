using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.Controls
{
    public partial class Print : System.Web.UI.UserControl
    {
        public String PrintDiv { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            String ltlText = String.Empty;
            //Setup Script Text
            ltlText = @"<script type=""text/javascript"">
    function printResults(divId) {
        var DocumentContainer = document.getElementById(divId);
        var html = ""<html><head>"";
        html += ""<link href='" + ResolveUrl("~/Styles/old/style.css") + @"' rel='stylesheet' type='text/css'></link>""
        html += ""</head><body>"" + DocumentContainer.innerHTML + ""</body></html>"";
        var WindowObject = window.open("""", ""PrintWindow"",
            ""width=800,height=600,top=50,left=50,toolbars=no,scrollbars=yes,status=no,resizable=yes"");
        WindowObject.document.writeln(html);
            WindowObject.document.close();
            WindowObject.focus();
            WindowObject.print();
        }
    </script>";
            ltlPrintScript.Text = ltlText;

            //Setup Button
            ltlText = @"<input type=""image"" name=""ibtnPrintResults"" id=""ibtnPrintResults"" src="""
                + ResolveUrl("~/Images/PrintResults.PNG")
                + @""" style=""float:right; border-width:0px;"" onclick=""printResults('{0}'); return false;"" />";
            ltlPrintButton.Text = String.Format(ltlText, PrintDiv);
        }
    }
}