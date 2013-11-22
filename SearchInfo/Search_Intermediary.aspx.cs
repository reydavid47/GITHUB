using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using Microsoft.Security.Application;

namespace ClearCostWeb.SearchInfo
{
    public partial class Search_Intermediary : System.Web.UI.Page
    {
        private bool isServiceNotFound;  //  lam, 20130604, MSF-377
        protected bool IsServiceNotFound  //  lam, 20130604, MSF-377
        {
            get { return isServiceNotFound; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                tsIntermediary.SearchTerm = Encoder.HtmlEncode(ThisSession.ServiceEntered); //Set the text we see in the search box
                if (ThisSession.ServiceEnteredFrom == "Entry") //If we didn't recognize the service they typed in
                {
                    isServiceNotFound = true;
                    ThisSession.ServiceEnteredFrom = string.Empty; //Do this to prevent the "you entered..." text on the results_care page
                    SetupNoService();
                }
                else //If we did but we have multiple matches
                {
                    SetupMultipleServices();
                }
            }

            if (isServiceNotFound || lblBetween.Text.IndexOf("possible result") >=0  || (lblBetween.Text.Trim() != "" && Convert.ToInt32(lblBetween.Text) <= 0))  //  lam, 20130604, MSF-377
            {
                String ContactUs = String.Format("<a href=\"{0}\">contact us</a>",
                    ResolveUrl(String.Format("../{0}/Contact_Us.aspx",
                        ThisSession.EmployerName)
                    )
                );

                isServiceNotFound = true;
                String s = "<i style=\"color: #662D91;\">\"" + ThisSession.ServiceEntered + "\"</i>&nbsp;is not a service included in ClearCost Health.";
                if (ThisSession.ServiceNotFoundDisclaimerText != "")
                {
                    lblServiceNotFoundDisclaimerTitle.Text = "<p class=\"displayinline larger\" style=\"color: #662D91;font-weight:bold;\">" + s + "</p>";
                    lblServiceNotFoundDisclaimerText.Text = ThisSession.ServiceNotFoundDisclaimerText.Replace("contact us", ContactUs);
                }

                SetupNoService();

                lblResultCount.Text = (lblResultCount.Text == "0" ? "" : lblResultCount.Text);
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void lbtnSelectSearch_Click(object sender, EventArgs e)
        {
            LinkButton lbSelected = (LinkButton)sender;

            //  lam, 20130307, MSF-177, "Office Visit" should direct to a page in Find a Service not Find a Doc"
            if (lbSelected.Text.ToLower().Contains("office visit"))
            {
                ThisSession.ServiceEntered = lbSelected.Text;
            }
            else
            {
                ThisSession.ServiceName = lbSelected.Text;
                ThisSession.ServiceID = int.Parse(lbSelected.CommandArgument.ToString());
            }
            //  lam, 20130311, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
            if (lbSelected.Text.ToLower().Contains("office visit"))
            {
                ThisSession.ServiceEntered = lbSelected.Text;
                //  lam, 20130319, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
                ThisSession.ServiceID = int.Parse(lbSelected.CommandArgument.ToString());
                Response.Redirect("specialty_search.aspx");
                //  -------------------------------------------------------------------------------------------------
            }
            else
            {
                ThisSession.ServiceName = lbSelected.Text;
                ThisSession.ServiceID = int.Parse(lbSelected.CommandArgument.ToString());
                Response.Redirect("results_care.aspx");
            }
            //  old code  --------------------------------------------------------------------------------------
            //if (lbSelected.Text.ToLower().Contains("office visit"))
            //{
            //    ThisSession.ServiceEntered = lbSelected.Text;
            //    Response.Redirect("specialty_search.aspx#tabdoc");
            //}
            //else
            //{
            //    ThisSession.ServiceName = lbSelected.Text;
            //    ThisSession.ServiceID = int.Parse(lbSelected.CommandArgument.ToString());
            //    Response.Redirect("results_care.aspx#tabcare");
            //}
            //  ------------------------------------------------------------------------------------------------
        }
        protected void lbOldSearch_Click(object sender, EventArgs e)
        {
            ThisSession.ServiceEntered = lbOldSearch.Text;
            Response.Redirect("results_care.aspx");
        }

        private void SetupNoService()
        {
            lbOldSearch.Text = string.Format("\"{0}\"", ThisSession.ServiceEntered); //Set the string asking if we want to search for what we meant instead

            DataSet comparedText = TextCompare(ThisSession.ServiceEntered); //Do compare against what we have for services and get the best match
            if (comparedText.Tables[1].Rows.Count > 0 && comparedText.Tables[1].Rows[0]["NewSearchString"].ToString().Trim() != String.Empty)
            {
                lblSearchTerm.Text = "\"" + comparedText.Tables[1].Rows[0]["NewSearchString"].ToString() + "\""; //Set the string stating that we are displaying possible searches for what we meant

                DataSet newSearches = NewSearchData(comparedText.Tables[1].Rows[0]["NewSearchString"].ToString());
                rptSearchList.DataSource = newSearches;
                rptSearchList.DataBind();

                lblResultCount.Text = newSearches.Tables[0].Rows.Count.ToString();
                lblBetween.Text = BetweenText(newSearches.Tables[0].Rows.Count);

                pnlInstead.Visible = false;  //  lam, 20130703, MSF-377
                //  lam, 20130703, MSF-377
                if (newSearches.Tables[0].Rows.Count <= 0)  
                    lblNoResults.Visible = lblBetween.Visible = lblSearchTerm.Visible = false;
                else
                    lblBetween.Visible = lblResultCount.Visible = lblSearchTerm.Visible = true;
            }
            else
            {
                //  lam, 20130703, MSF-377
                lblSearchTerm.Text = "";
                lblNoResults.Visible = false;
                lblBetween.Visible = false;

                //  lam, 20130703, MSF-377
                //lblSearchTerm.Text = ThisSession.ServiceEntered;
                //lblNoResults.Visible = true;
            }
        }
        private void SetupMultipleServices()
        {
            String term = ThisSession.ServiceEntered;
            lblSearchTerm.Text = "\"" + Encoder.HtmlEncode( term ) + "\""; //Set the string stating that we are displaying possible searches for what we meant

            DataSet newSearches = NewSearchData(term);
            rptSearchList.DataSource = newSearches;
            rptSearchList.DataBind();

            lblResultCount.Text = newSearches.Tables[0].Rows.Count.ToString();
            lblBetween.Text = BetweenText(newSearches.Tables[0].Rows.Count);

            pnlInstead.Visible = false;
        }

        private DataSet TextCompare(String searchedFor)
        {
            //Add SQL Data here
            //GetServiceList_VerifySearchString

            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ThisSession.CnxString))
                {
                    using (SqlCommand com = new SqlCommand("GetServiceList_VerifySearchString", con))
                    {
                        com.CommandType = CommandType.StoredProcedure;
                        SqlParameter parm = new SqlParameter("@SearchString", SqlDbType.NVarChar, 100);
                        parm.SqlValue = searchedFor.Trim();
                        com.Parameters.Add(parm);
                        using (SqlDataAdapter da = new SqlDataAdapter(com))
                        {
                            da.Fill(ds);
                        }
                    }
                }
            }
            catch
            { }

            return ds;
        }
        private DataSet NewSearchData(String newTerm)
        {
            DataSet ds = new DataSet();

            using (SqlConnection con = new SqlConnection(ThisSession.CnxString))
            {
                using (SqlCommand com = new SqlCommand("GetServiceList_AutoComplete", con))
                {
                    com.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@SearchString", SqlDbType.NVarChar, 100);
                    parm.SqlValue = newTerm;
                    com.Parameters.Add(parm);
                    using (SqlDataAdapter da = new SqlDataAdapter(com))
                    {
                        da.Fill(ds);
                    }
                }
            }

            return ds;
        }
        private String BetweenText(int ResultCount)
        {
            String retTxt = "possible result";
            retTxt += ((ResultCount > 1) ? "s " : " ");
            retTxt += "for ";
            return retTxt;
        }

        [WebMethod]
        public static string Autocomplete(string userIn)
        {
            String autoRes = "";

            SqlConnection conn = new SqlConnection(ThisSession.CnxString);
            SqlCommand sqlCmd;
            sqlCmd = new SqlCommand("GetServiceList_AutoComplete", conn);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            SqlParameter userParm = new SqlParameter("@SearchString", SqlDbType.NVarChar, 100);
            userParm.Value = userIn;
            sqlCmd.Parameters.Add(userParm);

            SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                adp.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    for (Int32 i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        object[] test = ds.Tables[0].Rows[i].ItemArray;
                        autoRes += test[1].ToString() + '|';
                    }
                }
            }
            catch (Exception ex)
            {
                string testing = ex.Message;
            }
            finally
            { conn.Close(); }

            return autoRes;
        }
    }
}