using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace ClearCostWeb.SavingsChoice
{
    public partial class LabDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((Details)this.Master).submitEvent += PatientLocationSubmit;
            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        protected void LoadPage()
        {
            prepData();
            prepUI();
        }

        protected void PatientLocationSubmit(object sender, EventArgs e)
        {
            LoadPage();
        }

        protected int EmployerID
        {
            get
            {
                //return 7;
                //if ((Request.QueryString["e"] ?? "").Trim() != string.Empty)
                //    return int.Parse(Request.QueryString["e"]);
                //else
                //    if (Debugger.IsAttached)
                //        return 7;
                //    else
                return int.Parse(ThisSession.EmployerID);
            }
        }
        protected int PrimaryCCHID
        {
            get
            {
                //return 77502; // 91266;
                //if ((request.querystring["cchid"] ?? "").trim() != string.empty)
                //    return int.parse(request.querystring["cchid"]);
                //else
                //    if (debugger.isattached)
                //        return 110733;
                //    else
                return ThisSession.CCHID;
            }
        }
        DataTable Providers;
        protected String ProviderName, Address1, City, State, StarType;
        protected String Category = "Lab";
        protected int Rating, Satisfaction;
        protected double Distance;
        protected String PrepFairPriceProviders(RepeaterItem i)
        {
            DataRow dr = (i.DataItem as DataRowView).Row;
            ProviderName = dr["ProviderName"].ToString();
            Address1 = dr["Address1"].ToString();
            Distance = Convert.ToDouble(dr["distance"]);
            City = dr["City"].ToString();
            State = dr["State"].ToString();
            if (dr["PatientRatings"] == DBNull.Value)
            {
                Rating = 0;
            }
            else
            {
                Rating = Convert.ToInt32(dr["PatientRatings"]);
            }            
            Satisfaction = Convert.ToInt32(dr["AvgPatientSatisfaction"]);
            StarType = "inactiveStar";
            if (Satisfaction < 0){
                Satisfaction = 0;
            }
            return "";
        }
        DataTable UsedProviders;
        protected String UsedProviderName, UsedAddress1, UsedCity, UsedState, UsedFairPriceImage, UsedOrganizationLocationID, UsedOrgID, UsedProviderID;
        protected int UsedSatisfaction, UsedServiceCount;
        protected string PrepUsedLabsData(RepeaterItem i) {
            DataRow dr = (i.DataItem as DataRowView).Row;
            UsedProviderName = dr["ProviderName"].ToString();
            UsedAddress1 = dr["Address1"].ToString();            
            UsedCity = dr["city"].ToString();
            UsedState = dr["state"].ToString();
            UsedSatisfaction = Convert.ToInt32(dr["MemberRating"]);
            UsedServiceCount = Convert.ToInt32(dr["ServiceCount"]);
            UsedOrganizationLocationID = dr["OrganizationLocationID"].ToString();
            UsedOrgID = dr["OrganizationID"].ToString();
            if (dr["ProviderID"] != DBNull.Value)
            { UsedProviderID = dr["ProviderID"].ToString(); }
            else { UsedProviderID = "0"; }  

            if (dr["IsFairPrice"] != DBNull.Value && Convert.ToInt32(dr["IsFairPrice"]) == 1)
            {
                UsedFairPriceImage = "<img width=23 height=23 src=" + ResolveUrl("~/images/check_green.png") + ">";
            }
            else if (dr["IsFairPrice"] != DBNull.Value && Convert.ToInt32(dr["IsFairPrice"]) == 0)
            {
                UsedFairPriceImage = "";
            }
            else if (dr["IsFairPrice"] != DBNull.Value && Convert.ToInt32(dr["IsFairPrice"]) == -1)
            {

                UsedFairPriceImage = "No data available";
            }

            StarType = "inactiveStar";
            if (UsedSatisfaction < 0)
            {
                UsedSatisfaction = 0;
                StarType = "ratingRequired";
            }
            return "";
        }
        protected void prepData() {
            SC_GetFairPriceAlternatives fpa = new SC_GetFairPriceAlternatives();
            {
                fpa.CCHID = this.PrimaryCCHID;
                fpa.Category = Category;
                fpa.SubCategory = DBNull.Value;
                fpa.Lat = double.Parse(ThisSession.PatientLatitude);
                fpa.Lon = double.Parse(ThisSession.PatientLongitude);
                fpa.RowMax = 5;
                fpa.GetData();

                if (fpa.Tables.Count >= 1 && fpa.Tables[0].Rows.Count > 0)
                {
                    Providers = fpa.Tables[0];                    
                }                

            }
            fairPriceProviders.DataSource = Providers;
            fairPriceProviders.DataBind();

            SC_GetRecentProviders grp = new SC_GetRecentProviders();
            {
                grp.CCHID = this.PrimaryCCHID;
                grp.Category = Category;
                grp.EmployerID = this.EmployerID;
                grp.GetData();

                if (grp.Tables.Count >= 1 && grp.Tables[0].Rows.Count > 0)
                {
                    UsedProviders = grp.Tables[0];
                }

            }
            recentProviders.DataSource = UsedProviders;
            recentProviders.DataBind();
        }

        protected void prepUI() { 
        
        }

        protected string getOverAllScore()
        {
            return "<div class='overallDivider'>&nbsp;</div><div class='overallText'>Your Savings Choice Score: </div><div class='overallScore'>62%</div>";
        }
    }
}