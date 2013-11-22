using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using Microsoft.Security.Application;

namespace ClearCostWeb.Controls
{
    public partial class TextSearch : System.Web.UI.UserControl
    {
        #region Private Variables

        private String searchDescription;

        public enum RunMethod { SearchPage, RXPage, Intermediary }
        private RunMethod _runMethod;

        private static Dictionary<String, String> SearchTexts = new Dictionary<string, string>
    {
      {"NoText","Please enter a service to search for."},
      {"NoneFound", "No services found for "}
    };
        private static Dictionary<String, String> RXTexts = new Dictionary<string, string>
    {
        {"NoText","Please enter an RX to search for."},
        {"NoneFound", "No Drugs were found matching {0}. Try searching by first letter."}
    };
        private static Dictionary<String, String> IntTexts = new Dictionary<string, string>
    {
        {"NoText","Please enter a service to search for."},
        {"NoneFound","No services found for "}
    };
        private static Dictionary<RunMethod, Dictionary<String, String>> Texts = new Dictionary<RunMethod, Dictionary<string, string>>
    {
        {RunMethod.SearchPage, SearchTexts},
        {RunMethod.RXPage, RXTexts},
        {RunMethod.Intermediary, IntTexts}
    };
        private static Dictionary<RunMethod, String> ProcsToRun = new Dictionary<RunMethod, string>
    {
        {RunMethod.SearchPage, "GetServiceListForWeb"},
        {RunMethod.Intermediary, "GetServiceListForWeb"},
        {RunMethod.RXPage, "GetDrugListForWeb"}
    };

        #endregion

        #region Public Properties

        [Description("The text to show just above the search box")]
        public String SearchDescription
        {
            get { return searchDescription.ToString(); }
            set { searchDescription = value.ToString(); }
        }

        [Description("Any text to initially show in the search box")]
        public String SearchTerm { set { txtSearch.Text = Encoder.HtmlEncode( value ); } }

        [DefaultValue(RunMethod.SearchPage)]
        [Browsable(true)]
        [Description("The Run Method the search control needs to use")]
        public RunMethod runMethod
        {
            get { return _runMethod; }
            set { _runMethod = value; }
        }

        private String javaAutoComplete
        {
            get
            {
                String outJava = @"$(document).ready(function () {
$('#" + txtSearch.ClientID.ToString();
                outJava += @"').autocomplete({
source: function(request, response) {
PageMethods.";

                switch (_runMethod)
                {
                    case RunMethod.SearchPage:
                        outJava += "Autocomplete";
                        break;
                    case RunMethod.RXPage:
                        outJava += "DrugAutocomplete";
                        break;
                    case RunMethod.Intermediary:
                        outJava += "Autocomplete";
                        break;
                    default:
                        outJava += "";
                        break;
                }
                outJava += @"(request.term, function(result) {
var tempData = result.split('|');
response(tempData);
});
},
minLength: 3
});
});";
                return outJava;
            }
        }
        #endregion

        #region GUI Methods

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            JavaScriptHelper.IncludeJQueryAutocomplete(Page.ClientScript);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            lblSearchDescription.Text = "<h3>" + searchDescription + "</h3>";

            ClientScriptManager csm = Page.ClientScript;
            Type cstype = Page.GetType();

            //String scriptID = "";
            switch (_runMethod)
            {
                case RunMethod.SearchPage:
                    //scriptID = "AutoComplete";
                    txtSearch.Attributes.Add("ACType", "AutoComplete");
                    break;
                case RunMethod.RXPage:
                    //scriptID = "DrugAutoComplete";
                    txtSearch.Attributes.Add("ACType", "DrugAutoComplete");
                    break;
                case RunMethod.Intermediary:
                    //scriptID = "AutoComplete";
                    txtSearch.Attributes.Add("ACType", "AutoComplete");
                    break;
                default:
                    //scriptID = "";
                    break;
            }

        }

        protected void lnkBtnSearch_Click(object sender, EventArgs e)
        {
            //did they enter something?
            String feedback = string.Empty;
            if (txtSearch.Text == string.Empty)
            {
                if (Texts[_runMethod].TryGetValue("NoText", out feedback))
                {
                    lblCareSearchNote.Text = feedback; //"Please enter a service to search for.";
                    lblCareSearchNote.Style.Add(HtmlTextWriterStyle.Color, "Red");
                }
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ChangeHashForPostback", "window.location.hash = 'tabrx';", true);
            }
            else
            {
                //find out if we can get a match
                DataTable dt = SearchByDescription();

                if (dt.Rows.Count == 0) // If we got no results, display a message.
                {
                    if (_runMethod == RunMethod.RXPage)
                    {
                        if (Texts[_runMethod].TryGetValue("NoneFound", out feedback))
                        {
                            lblCareSearchNote.Text = string.Format(feedback, Encoder.HtmlEncode( txtSearch.Text ));
                            lblCareSearchNote.Style.Add(HtmlTextWriterStyle.Color, "Red");
                        }
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "ChangeHashForPostback", "window.location.hash = 'tabrx';", true);
                    }
                    else
                    {
                        ThisSession.ServiceEnteredFrom = "Entry";
                        ThisSession.ServiceEntered = txtSearch.Text;
                        Response.Redirect("Search_Intermediary.aspx");
                    }
                }
                else if (dt.Rows.Count == 1) // If there was only one result, save it and go to the results page.
                {
                    if (_runMethod == RunMethod.RXPage)
                    {
                        ThisSession.DrugName = dt.Rows[0][0].ToString();
                        ThisSession.DrugID = dt.Rows[0][1].ToString();

                        if (txtSearch.Text != ThisSession.DrugName)
                        {
                            ThisSession.DrugEnteredFrom = "Entry";
                            ThisSession.DrugEntered = txtSearch.Text;
                        }
                        //go to results
                        Response.Redirect("results_rx_name.aspx");
                    }
                    else
                    {
                        ThisSession.ServiceName = dt.Rows[0][0].ToString();
                        ThisSession.ServiceID = int.Parse(dt.Rows[0][1].ToString());
                        //Check if this is a lab and if so head to multi-lab page
                        if (dt.Rows[0][2].ToString() == "Lab")
                        {
                            ThisSession.ServiceEnteredFrom = "Letters";
                            //NOTE: 7/14 JM
                            //Temporarily removing multi-lab capability for the starbucks go-live as the CCH believes it isn't working
                            Response.Redirect("results_care.aspx");
                            //Response.Redirect("AddLab.aspx#tabcare");
                        }
                        else
                        {
                            //set the latitude and longitude if they changed locations.
                            //SetLatLong();

                            //Save information on what the user actually entered.
                            if (txtSearch.Text != ThisSession.ServiceName)
                            {
                                ThisSession.ServiceEnteredFrom = "Entry";
                                ThisSession.ServiceEntered = txtSearch.Text;
                            }

                            // different step if they picked office visit:
                            //  lam, 20130311, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
                            if (ThisSession.ServiceName.ToLower().StartsWith("office visit"))
                            {
                                ThisSession.ServiceEntered = txtSearch.Text;// need to  know if they picked new patient
                                Response.Redirect("specialty_search.aspx");
                            }
                            else
                            {
                                //go to results
                                Response.Redirect("results_care.aspx");
                            }
                            //  old code  --------------------------------------------------------------------------------------
                            //if (ThisSession.ServiceName.ToLower().StartsWith("office visit"))
                            //{
                            //    ThisSession.ServiceEntered = txtSearch.Text;// need to  know if they picked new patient
                            //    Response.Redirect("specialty_search.aspx#tabdoc");
                            //}
                            //else
                            //{
                            //    //go to results
                            //    Response.Redirect("results_care.aspx");
                            //}
                            //  ------------------------------------------------------------------------------------------------
                        }
                    }
                }
                else // If there are multiple results, have them pick one.
                {
                    //lbCareSearchResults.DataSource = dt;
                    //lbCareSearchResults.DataTextField = "ServiceName";
                    //lbCareSearchResults.DataValueField = "ServiceName";
                    //lbCareSearchResults.DataBind();
                    //lbCareSearchResults.Visible = true;
                    //lblCareSearchNote.Text = "<br />";

                    if (_runMethod == RunMethod.RXPage)
                    {
                        //come up with some kind of handling for multiple drug results
                    }
                    else
                    {
                        ThisSession.ServiceEntered = txtSearch.Text;
                        Response.Redirect("Search_Intermediary.aspx");
                    }
                }
            }
        }

        #endregion

        #region Supporting Methods

        protected DataTable SearchByDescription()
        {
            DataTable ServiceList = new DataTable();

            //Which employer database?
            SqlConnection conn = new SqlConnection(ThisSession.CnxString);
            SqlCommand sqlCmd;
            sqlCmd = new SqlCommand(ProcsToRun[_runMethod], conn);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            SqlParameter userParm = new SqlParameter("@SearchWord", SqlDbType.VarChar, 200);
            userParm.Value = txtSearch.Text.Trim();
            sqlCmd.Parameters.Add(userParm);

            SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                adp.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    ServiceList = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                string testing = ex.Message;
            }
            finally
            { conn.Close(); }

            return ServiceList;
        }
        #endregion
    }
}
