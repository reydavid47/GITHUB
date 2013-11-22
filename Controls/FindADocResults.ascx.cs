using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using QSEncryption.QSEncryption;

namespace ClearCostWeb.Controls
{
    public partial class FindADocResults : System.Web.UI.UserControl
    {
        private HtmlLink CssSheet
        {
            get
            {
                HtmlLink cssSheet = new HtmlLink();
                cssSheet.Href = ResolveUrl("~/SearchInfo/Styles/FindADocResults.css") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                cssSheet.Attributes["rel"] = "stylesheet";
                cssSheet.Attributes["type"] = "text/css";
                return cssSheet;
            }
        }
        private const String Spinner = "<img class=\"loadingSpinner\" src=\"{0}\" alt=\"\" style=\"display:none;width:32px;height:32px;\" width=\"32px\" height=\"32px\" />";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            JavaScriptHelper.IncludeFindADocResults(Page.ClientScript);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Header.Controls.Add(this.CssSheet);
//#if DEBUG
//        Page.AddScriptToHeader("FindADocResults.js");
//#else

//            Page.AddScriptToHeader("FindADocResults.min.js");
//#endif

            //For Debugging Purposes only load the non-minified script
            //if (Request.Url.Host.ToLower() == "localhost")
            //    ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "FindADocResults", ResolveUrl("~/Scripts/FindADocResults.js") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //else
            //    ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "FindADocResults", ResolveUrl("~/Scripts/FindADocResults.min.js") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            if (ThisSession.DefaultYourCostOn)
            {
                Page.Header.Controls.Add(new LiteralControl("<script type=\"text/javascript\">var YourCostDefault = true;</script>"));
            }
            //Set the default sort to start with if it's different than Distance
            if (ThisSession.DefaultSort != "Distance")
            {
                String csJava = "var globDefSort = '" + ThisSession.DefaultSort + "';";
                csJava += "$(\"input.sortHeader[sortCol=Distance]\").attr(\"Checked\",false);";
                csJava += "$(\"input.sortHeader[sortCol=" + ThisSession.DefaultSort + "]\").attr(\"Checked\",true);";
                csJava += "$(\"td[sort=Distance]\").children(\"a\").removeClass(\"sortAsc\");";
                csJava += "$(\"td[sort=" + ThisSession.DefaultSort + "]\").children(\"a\").first().addClass(\"sortAsc\");";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ChangeDefaultSort", csJava, true);
            }

            ltlSpinLoader.Text = String.Format(Spinner, ResolveUrl("~/Images/ajax-loader-AltCircle.gif"));
            pnlServices.Style[HtmlTextWriterStyle.Position] = "relative";

            if (Page.IsPostBack)
            {
                if (POSTDIST.Value.Replace("undefined", "") != "" && POSTNAV.Value.Replace("undefined", "") != "")
                {
                    ThisSession.FacilityDistance = POSTDIST.Value;
                    QueryStringEncryption qs = new QueryStringEncryption(POSTNAV.Value, new Guid(ThisSession.UserLogginID));
                    ThisSession.PracticeName = qs["PracticeName"];
                    ThisSession.ProviderName = qs["ProviderName"];
                    ThisSession.PracticeNPI = qs["PracticeNPI"];
                    ThisSession.FacilityDistance = qs["FacilityDistance"];
                    ThisSession.TaxID = qs["TaxID"];
                    ThisSession.OrganizationLocationID = Convert.ToInt32(qs["OrganizationLocationID"]);
                    Response.Redirect("doctor_specialty_detail.asxp");
                }
            }
        }

        protected int distance = 25;
        protected void updateDistance(object sender, EventArgs e)
        {
            distance = sFindADoc.Value;
            lblSliderValue.Text = String.Concat(" ", sFindADoc.Value, " mile(s)");
        }
    }
}