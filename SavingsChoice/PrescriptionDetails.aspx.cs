using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace ClearCostWeb.SavingsChoice
{
    public partial class PrescriptionDetails : System.Web.UI.Page
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
                //return 77501;
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
        protected String Category = "Rx";
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
        protected String UsedProviderName, UsedAddress1, UsedCity, UsedState, UsedFairPriceImage, UsedDrugName, UsedOrganizationLocationID, UsedOrgID, UsedProviderID;
        protected int UsedSatisfaction, UsedServiceCount;
        protected string PrepUsedLabsData(RepeaterItem i) {
            DataRow dr = (i.DataItem as DataRowView).Row;
            UsedProviderName = dr["ProviderName"].ToString();
            UsedAddress1 = dr["Address1"].ToString();            
            UsedCity = dr["city"].ToString();
            UsedState = dr["state"].ToString();
            UsedSatisfaction = Convert.ToInt32(dr["MemberRating"]);
            UsedServiceCount = Convert.ToInt32(dr["ServiceCount"]);
            UsedDrugName = dr["DrugName"].ToString();
            UsedOrganizationLocationID = dr["OrganizationLocationID"].ToString();
            UsedOrgID = dr["OrganizationID"].ToString();
            if (dr["ProviderID"] != DBNull.Value)
            { UsedProviderID = dr["ProviderID"].ToString();  }
            else
            { UsedProviderID = "0"; }     
            if (dr["IsFairPrice"] != DBNull.Value && Convert.ToInt32(dr["IsFairPrice"]) == 1)
            {
                UsedFairPriceImage = "<img width=23 height=23 src=" + ResolveUrl("~/images/check_green.png") + ">";
            }
            else if (dr["IsFairPrice"] != DBNull.Value && Convert.ToInt32(dr["IsFairPrice"]) == 0)
            {
                UsedFairPriceImage = "";
            }
            else {

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
        DataTable SubCategories;
        protected String SubCategory, DrugName, DrugStrength, DrugQuantity, GPI, GenericIndicator, LoadedGPI;
        protected int LoadedGenericIndicator;
        protected string PrepSubcategories(RepeaterItem i)
        {
            DataRow dr = (i.DataItem as DataRowView).Row;
            DrugName = dr["DrugName"].ToString();
            DrugStrength = dr["Strength"].ToString();
            DrugQuantity = dr["Quantity"].ToString();
            GPI = dr["GPI"].ToString();
            GenericIndicator = "1";// dr["GenericIndicator"].ToString();
            SubCategory = DrugName + " " + DrugStrength.Trim() + "mg " + DrugQuantity;
            return "";
        }
        protected void prepData()
        {
            
            SC_GetRecentProvidersRX grp = new SC_GetRecentProvidersRX();
            {
                grp.CCHID = this.PrimaryCCHID;
                grp.EmployerID = this.EmployerID;
                grp.GetData();

                if (grp.Tables.Count >= 1 && grp.Tables[0].Rows.Count > 0)
                {
                    UsedProviders = grp.Tables[0];
                    recentProviders.DataSource = UsedProviders;
                    recentProviders.DataBind();
                }

            }            

            SC_GetFairPriceAlternativesRX fpa = new SC_GetFairPriceAlternativesRX();
            {
                fpa.CCHID = this.PrimaryCCHID;
                fpa.GPI = LoadedGPI = UsedProviders.Rows[0]["GPI"].ToString();
                fpa.GenericIndicator = LoadedGenericIndicator = Convert.ToInt32(UsedProviders.Rows[0]["GenericIndicator"]);
                fpa.RowMax = 5;
                fpa.GetData();

                if (fpa.Tables.Count >= 1 && fpa.Tables[0].Rows.Count > 0)
                {
                    Providers = fpa.Tables[0];
                    fairPriceProviders.DataSource = Providers;
                    fairPriceProviders.DataBind();
                }
            }

            //SC_GetSubCategories sc = new SC_GetSubCategories();
            //{
            //    sc.Subcategory = "Rx";
            //    sc.GetData();
            //    if (sc.Tables.Count >= 1 && sc.Tables[0].Rows.Count > 0)
            //    {
            //        SubCategories = sc.Tables[0];
            //    }
            //}
            //subCategorySelections.DataSource = SubCategories;
            //subCategorySelections.DataBind();

            SC_GetMedication gm = new SC_GetMedication();
            {
                gm.CCHID = this.PrimaryCCHID;
                gm.GetData();
                if (gm.Tables.Count > 0) {
                    SubCategories = gm.Tables[0];
                    subCategorySelections.DataSource = SubCategories;
                    subCategorySelections.DataBind();
                }
            }
        }

        protected void prepUI() { 
        
        }

        protected string getOverAllScore()
        {
            return "<div class='overallDivider'>&nbsp;</div><div class='overallText'>Your Savings Choice Score: </div><div class='overallScore'>62%</div>";
        }
    }
}