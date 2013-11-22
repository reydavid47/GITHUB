using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Text.RegularExpressions;

namespace ClearCostWeb.SearchInfo
{
    public partial class doctor_specialty_detail : System.Web.UI.Page
    {
        private Boolean isAntiTransparency = false;  //  lam, 20130308, MSF-141 antitransparency
        private Boolean isTeleMedicine = false;  //  lam, 20130528, CI-142
        protected Boolean IsAntiTransparency { get { return isAntiTransparency; } }  //  lam, 20130308, MSF-141 antitransparency
        protected Boolean IsTeleMedicine { get { return isTeleMedicine; } }  //  lam, 20130528, CI-142

        private String returnTab = "";  //  lam, 20130313, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
        protected String ReturnTab { get { return returnTab; } }  //  lam, 20130313, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                returnTab = (ThisSession.ServiceEntered.ToLower().Contains("office") ? "tabcare" : "tabdoc");  //  lam, 20130313, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
                lblSpecialty.Text = ThisSession.Specialty.ToString();
                lblSpecialty_MoreInfoTitle.Text = ThisSession.Specialty.ToString();
                lblPrimarySpecialty.Text = ThisSession.Specialty.ToString();
                lblSpecialtyMoreInfo.Text = "Additional information for this specialty is not currently available.";

                //lblDistance.Text = ThisSession.FacilityDistance;
                PopulateFacilityinfo();
            }
            //Google work
            hfStartLL.Value = String.Format("{0},{1}",
                ThisSession.PatientLatitude.ToString().Trim(),
                ThisSession.PatientLongitude.ToString().Trim()
            );
            hfEndLL.Value = String.Format("{0},{1}",
                ThisSession.FacilityLatitude.ToString().Trim(),
                ThisSession.FacilityLongitude.ToString().Trim()
            );
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }

        protected void PopulateFacilityinfo()
        {
            DataTable FacilityInfo = GetSelectedFacilityDetails();

            if (FacilityInfo.Rows.Count > 0)
            {
                //Save to session - probably needed elsewhere
                SaveSelectedFacilityInSession(FacilityInfo);

                //Populate fields on page
                //if (FacilityInfo.Rows[0]["FairPrice"].ToString() == "True")  lam, 20130528, CI-142
                if (FacilityInfo.Rows[0]["FairPrice"].ToString() == "True")
                { imgFairPriceTrue.Visible = true; imgFairPriceFalse.Visible = false; lblFairPriceTitle.Visible = true; imgFPLearnMore.Visible = true; }
                else
                { imgFairPriceTrue.Visible = false; imgFairPriceFalse.Visible = true; lblFairPriceTitle.Visible = false; imgFPLearnMore.Visible = false; }

                if (FacilityInfo.Rows[0]["HGRecognized"].ToString() == "True")
                { imgHGRecognizedTrue.Visible = true; imgHGRecognizedFalse.Visible = false; lblHGRecTitle.Visible = true; imgHGLearnMore.Visible = true; }
                else
                { imgHGRecognizedTrue.Visible = false; imgHGRecognizedFalse.Visible = true; lblHGRecTitle.Visible = false; imgHGLearnMore.Visible = false; }

                isTeleMedicine = (FacilityInfo.Rows[0]["PracticeName"].ToString().ToLower() == "mdlive");  //  lam, 20130528, CI-142
                isAntiTransparency = (FacilityInfo.Rows[0].GetData<int>("AntiTransparency") == 1); //Added onto logic from Lam, 20130308 to handle the event this column doesn't exist
                //isAntiTransparency = (Int32.Parse(FacilityInfo.Rows[0]["AntiTransparency"].ToString()) == 1);  //  lam, 20130308, MSF-141 antitransparency
                if (!IsAntiTransparency)  //  lam, 20130308, MSF-141 antitransparency
                {
                    lblRangeMin.Text = FacilityInfo.Rows[0]["RangeMin"].ToString();
                    lblRangeMax.Text = FacilityInfo.Rows[0]["RangeMax"].ToString();
                }
                if (!IsTeleMedicine)  //  lam, 20130528, CI-142
                {
                    lblAddressLine1.Text = ThisSession.FacilityAddress1;
                    lblAddressLine2.Text = ThisSession.FacilityCity + ", " + ThisSession.FacilityState + " " + ThisSession.FacilityZipCodeFormatted;
                }
                else  //  lam, 20130528, CI-142
                {
                    lblAddressLine1.Text = "N/A";
                    lblPracticeName.Text = "";  //  lam, 20130528, CI-142
                    imgFairPriceTrue.Visible = true; imgFairPriceFalse.Visible = false; lblFairPriceTitle.Visible = true; imgFPLearnMore.Visible = true;
                    lblFairPriceTitle.Text = "MD Live is a Fair Price Practice";
                }

                if (ThisSession.FacilityTelephoneFormatted != string.Empty)
                {
                    lblPhoneTitle.Visible = true;
                    lblPhone.Text = ThisSession.FacilityTelephoneFormatted;
                }
                //TODO: Will eventually have additional information for the practice, like website, etc.
                //      Place those here like the phone item above, and set the title to visible.

                // Provider Educational info
                string HGProviderID = FacilityInfo.Rows[0]["HGProviderID"].ToString();
                GetKeyDoctorDetails(HGProviderID);


                ////GenerateGoogleDirections();
            }
            else
            {
                //TODO: Will need to go through app and watch for this kind of thing. shouldn't happen, but will need to add error handling.
            }
        }
        protected DataTable GetSelectedFacilityDetails()
        {
            DataTable FacilityInfo = new DataTable();

            //Which employer database?
            SqlConnection conn = new SqlConnection(ThisSession.CnxString);
            SqlCommand sqlCmd;
            sqlCmd = new SqlCommand("GetSelectedProviderDetailsForSpecialty", conn);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            SqlParameter userParm = new SqlParameter("@SpecialtyID", SqlDbType.Int);
            userParm.Value = ThisSession.SpecialtyID;
            sqlCmd.Parameters.Add(userParm); userParm = new SqlParameter("@ProviderName", SqlDbType.VarChar, 150);
            userParm.Value = ThisSession.ProviderName;
            sqlCmd.Parameters.Add(userParm); userParm = new SqlParameter("@NPI", SqlDbType.VarChar, 50);
            userParm.Value = ThisSession.PracticeNPI;
            sqlCmd.Parameters.Add(userParm); userParm = new SqlParameter("@TaxID", SqlDbType.NVarChar, 15);
            userParm.Value = ThisSession.TaxID;
            sqlCmd.Parameters.Add(userParm); userParm = new SqlParameter("@ServiceID", SqlDbType.Int);
            userParm.Value = ThisSession.ServiceID;
            sqlCmd.Parameters.Add(userParm); userParm = new SqlParameter("@OrganizationLocationID", SqlDbType.Int);
            userParm.Value = ThisSession.OrganizationLocationID;
            sqlCmd.Parameters.Add(userParm);

            SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                adp.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    FacilityInfo = ds.Tables[0];

                    //Populate "Learn More" and less verbose Title.
                    //      NOTE: not sure this is the best place, but don't want to store a ton of text in session...
                    if (ds.Tables.Count > 1)
                    {
                        //Learn More:
                        if (ds.Tables[1].Rows[0][0].ToString() != String.Empty)
                        {
                            lblSpecialtyMoreInfo.Text = ds.Tables[1].Rows[0][0].ToString();
                        }
                        else
                        {
                            imgSpcLearnMore.Visible = false;
                            lblSpecialty_MoreInfoTitle.Text = string.Empty;
                        }
                        //Specialty Title
                        if (ds.Tables[1].Rows[0]["Title"].ToString() != String.Empty)
                        {
                            //  -------------------------------------------------
                            //  lam, 20130304, MSF-10, make this text conditional
                            String serviceName = ds.Tables[0].Rows[0]["ServiceName"].ToString();
                            Match match = Regex.Match(serviceName.Substring(0, 1), @"[aeiou]", RegexOptions.IgnoreCase);
                            serviceName = (match.Success ? "an " : "a ") + serviceName;
                            lblServiceName.Text = serviceName;
                            //  -------------------------------------------------
                            lblSpecialty.Text = ds.Tables[1].Rows[0]["Title"].ToString();
                            lblPrimarySpecialty.Text = ds.Tables[1].Rows[0]["Title"].ToString();
                            lblSpecialty_MoreInfoTitle.Text = ds.Tables[1].Rows[0]["Title"].ToString();
                        }

                        string docName = FacilityInfo.Rows[0]["ProviderName"].ToString();
                        lblProviderName.Text = docName;
                        lblPracticeName.Text = FacilityInfo.Rows[0]["PracticeName"].ToString();

                        //Practice level fair price settings.
                        //      NOTE: not sure this is the best place, but don't want to store a ton of text in session...
                        if (ds.Tables.Count > 2)
                        {
                            // rld, 20131115, SCIQ-189, removed FP table
                        }
                        else
                        {
                            //TODO: For demo, this will just keep default guys checked. For long term, may want to add text to that area if we don't find records/values.
                        }
                    }// end of If more than one table
                }
            }
            catch (Exception ex)
            {
                string testing = ex.Message;
            }
            finally
            { conn.Close(); }

            return FacilityInfo;
        }
        protected void GetKeyDoctorDetails(string HGProviderID)
        {
            DataTable EducationInfo = new DataTable();

            //Which employer database?
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CCH_Healthgrades"].ConnectionString);
            SqlCommand sqlCmd;
            sqlCmd = new SqlCommand("GetKeyProviderInfo", conn);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            SqlParameter userParm = new SqlParameter("@ProviderID", SqlDbType.VarChar, 25);
            userParm.Value = HGProviderID;
            sqlCmd.Parameters.Add(userParm);

            SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                adp.Fill(ds);

                // Returning 1 or 2 tables depending on whether the doctor has data for each.
                int currentTable = 0;

                //Education Data:
                if (ds.Tables.Count > 0)
                {
                    EducationInfo = ds.Tables[0];
                    if (EducationInfo.Rows.Count > 0)
                    {
                        rptEducation.DataSource = EducationInfo;
                        rptEducation.DataBind();
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), Guid.NewGuid().ToString(), "$(\".hgEdu\").css({ \"display\":\"none\",\"visibility\":\"hidden\" });", true);
                    }
                }
                //Patient Rating Data:
                if (ds.Tables.Count > 1)
                {
                    //Populate Overall rating line:
                    if (ds.Tables[1].Rows.Count == 0)
                    {
                        lblPatientCount.Text = "0 patient surveys";
                        lblStar3.Text = @"N/A";
                    }
                    else
                    {
                        //Overall area: survey count.
                        if (ds.Tables[1].Rows[0]["PatientCount"].ToString() == "1")
                            lblPatientCount.Text = "(based on 1 review)";
                        else
                            lblPatientCount.Text = "(based on " + ds.Tables[1].Rows[0]["PatientCount"].ToString() + " reviews)";

                        //Overall rating:
                        double rating = double.Parse(ds.Tables[1].Rows[0]["OverallRating"].ToString());
                        if (rating >= 1) { lblStar1.CssClass = "star_full"; }
                        else
                        {
                            if (rating >= .5) lblStar1.CssClass = "star_half";
                        }
                        if (rating >= 2) { lblStar2.CssClass = "star_full"; }
                        else
                        {
                            if (rating >= 1.5) lblStar2.CssClass = "star_half";
                        }
                        if (rating >= 3) { lblStar3.CssClass = "star_full"; }
                        else
                        {
                            if (rating >= 2.5) lblStar3.CssClass = "star_half";
                        }
                        if (rating >= 4) { lblStar4.CssClass = "star_full"; }
                        else
                        {
                            if (rating >= 3.5) lblStar4.CssClass = "star_half";
                        }
                        if (rating >= 5) { lblStar5.CssClass = "star_full"; }
                        else
                        {
                            if (rating >= 4.5) lblStar5.CssClass = "star_half";
                        }

                        //Populate survey details
                        rptRatings.DataSource = ds.Tables[1];
                        rptRatings.DataBind();
                    }
                }

            }
            catch (Exception ex)
            {
                string testing = ex.Message;
            }
            finally
            { conn.Close(); }
        }

        protected void SaveSelectedFacilityInSession(DataTable FacilityInfo)
        {
            //Capture Facility info in session:
            ThisSession.FacilityAddress1 = FacilityInfo.Rows[0]["LocationAddress1"].ToString();
            //TODO: missed address line 2.
            ThisSession.FacilityCity = FacilityInfo.Rows[0]["LocationCity"].ToString();
            ThisSession.FacilityState = FacilityInfo.Rows[0]["LocationState"].ToString();
            ThisSession.FacilityZipCode = FacilityInfo.Rows[0]["LocationZip"].ToString();
            ThisSession.FacilityTelephone = FacilityInfo.Rows[0]["LocationTelephone"].ToString();
            ThisSession.FacilityLatitude = FacilityInfo.Rows[0]["Latitude"].ToString();
            ThisSession.FacilityLongitude = FacilityInfo.Rows[0]["Longitude"].ToString();

            //Capture Facility/patient/service specific info in session
            ThisSession.FacilityFairPrice = FacilityInfo.Rows[0]["FairPrice"].ToString();
            ThisSession.FacilityHGRecognized = FacilityInfo.Rows[0]["HGRecognized"].ToString();
        }
        protected void rptEducation_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            { return; }

            // Populate details on education item (adding line breaks and commas)
            DataRowView drv = (DataRowView)e.Item.DataItem;
            Label lblEducationInfo = (Label)e.Item.FindControl("lblEducationInfo");
            lblEducationInfo.Text = drv["InstitutionName"].ToString();
            if (drv["InstitutionCity"].ToString() != string.Empty)
            { lblEducationInfo.Text += ", " + drv["InstitutionCity"].ToString(); }
            if (drv["CompletionYear"].ToString() != "0")
            { lblEducationInfo.Text += "<br />" + drv["YearCompletedTitle"].ToString() + ": " + drv["CompletionYear"].ToString(); }

        }
        protected void rptRatings_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            { return; }

            //Health Grade Rating stars:
            DataRowView drv = (DataRowView)e.Item.DataItem;

            //Get Star Labels
            Label lblStar1 = (Label)e.Item.FindControl("lblStar1");
            Label lblStar2 = (Label)e.Item.FindControl("lblStar2");
            Label lblStar3 = (Label)e.Item.FindControl("lblStar3");
            Label lblStar4 = (Label)e.Item.FindControl("lblStar4");
            Label lblStar5 = (Label)e.Item.FindControl("lblStar5");

            //Set Star Label display based on average rating.
            double rating = double.Parse(drv["SurveyScore"].ToString());
            if (rating >= 1) { lblStar1.CssClass = "star_full"; }
            else
            {
                if (rating >= .5) lblStar1.CssClass = "star_half";
            }
            if (rating >= 2) { lblStar2.CssClass = "star_full"; }
            else
            {
                if (rating >= 1.5) lblStar2.CssClass = "star_half";
            }
            if (rating >= 3) { lblStar3.CssClass = "star_full"; }
            else
            {
                if (rating >= 2.5) lblStar3.CssClass = "star_half";
            }
            if (rating >= 4) { lblStar4.CssClass = "star_full"; }
            else
            {
                if (rating >= 3.5) lblStar4.CssClass = "star_half";
            }
            if (rating >= 5) { lblStar5.CssClass = "star_full"; }
            else
            {
                if (rating >= 4.5) lblStar5.CssClass = "star_half";
            }

        }
    }
}