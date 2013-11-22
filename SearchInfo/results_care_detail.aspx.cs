using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace ClearCostWeb.SearchInfo
{
    public partial class results_care_detail : System.Web.UI.Page
    {
        private String PostBackControl { get { return Request.Params["__EVENTTARGET"].ToString(); } }
        private String PostBackArgument { get { return Request.Params["__EVENTARGUMENT"].ToString(); } }
        private String[] PostBackLatLng { get { return (PostBackControl == "Geocoder" ? PostBackArgument.Split('|') : null); } }
        private Double totalRangeMin = 0.0;
        private Double totalRangeMax = 0.0;
        private Boolean isAntiTransparency = false;  //  lam, 20130308, MSF-141 antitransparency

        private Boolean IsMultiLab { get { return (ThisSession.ChosenLabs == null); } }
        protected String ShowSingleSearch { get { return (IsMultiLab ? "" : "display:none;"); } }
        protected String ShowMultiSearch { get { return (IsMultiLab ? "display:none;" : ""); } }
        protected String TotalRangeMin { get { return String.Format("{0:c0}", totalRangeMin); } }
        protected String TotalRangeMax { get { return String.Format("{0:c0}", totalRangeMax); } }
        protected Boolean IsAntiTransparency { get { return isAntiTransparency; } }  //  lam, 20130308, MSF-141 antitransparency

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                if (ThisSession.ChosenLabs == null)
                {
                    //Populate employee info on page.
                    //lblEmployeeName.Text = ThisSession.FirstName.ToUpper() + " " + ThisSession.LastName.ToUpper();
                    lblServiceName.Text = ThisSession.ServiceName.ToString();
                    lblServiceName_MoreInfoTitle.Text = ThisSession.ServiceName.ToString();
                    lblServiceMoreInfo.Text = "Additional information for this service is not currently available.";
                }
                else
                {
                    rptLabTests.DataSource = ThisSession.ChosenLabs;
                    rptLabTests.DataBind();
                }

                //Populate facility info on page.
                lblPracticeName.Text = ThisSession.PracticeName;
                //lblDistance.Text = ThisSession.FacilityDistance;

                //change return url if came from past care.
                if (ThisSession.PastCareFacilityName != String.Empty)
                    hlReturnToSearchResults.NavigateUrl = "results_past_care.aspx";

                PopulateFacilityinfo();

                lblPracticeName.Attributes.Add("pLat", ThisSession.FacilityLatitude);
                lblPracticeName.Attributes.Add("pLng", ThisSession.FacilityLongitude);

                lblAllResult1DisclaimerText.Text = ThisSession.AllResult1DisclaimerText;  //  lam, 20130425, MSF-295 move disclaimer text to content manager
            }
            else if (PostBackControl == "Geocoder")
            {

            }
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
                isAntiTransparency = (Int32.Parse(FacilityInfo.Rows[0]["AntiTransparency"].ToString()) == 1);  //  lam, 20130308, MSF-141 antitransparency
                if (!IsAntiTransparency)  //  lam, 20130308, MSF-141 antitransparency
                {
                    if (ThisSession.ChosenLabs == null)
                    {
                        lblRangeMin.Text = FacilityInfo.Rows[0]["RangeMin"].ToString();
                        lblRangeMax.Text = FacilityInfo.Rows[0]["RangeMax"].ToString();
                    }
                    else
                    {
                        string[] fCols = { "ServiceName", "RangeMin", "RangeMax" };
                        DataView fDV = new DataView(FacilityInfo);
                        rptMultiLabCost.DataSource = fDV.ToTable("FacilityRangeInfo", false, fCols);
                        rptMultiLabCost.DataBind();
                    }
                }

                lblAddressLine1.Text = ThisSession.FacilityAddress1;
                lblAddressLine2.Text = ThisSession.FacilityCity + ", " + ThisSession.FacilityState + " " + ThisSession.FacilityZipCodeFormatted;

                if (ThisSession.FacilityTelephoneFormatted != string.Empty)
                {
                    lblPhoneTitle.Visible = true;
                    lblPhone.Text = ThisSession.FacilityTelephoneFormatted;
                }
                //TODO: Will eventually have additional information for the practice, like website, etc.
                //      Place those here like the phone item above, and set the title to visible.

                //Tesla rating, or "Other" info. 
                //TODO: For the demo, this is only for the MRI tesla rating. Will be more general.
                try
                {
                    if (FacilityInfo.Rows[0]["OtherKeyItem1"].ToString() != String.Empty)
                    {
                        lblOtherKeyItemTitle.Text = FacilityInfo.Rows[0]["OtherKeyItem1"].ToString();
                        lblOtherKeyItemText.Text = FacilityInfo.Rows[0]["OtherKeyItem1_Text"].ToString();
                        lblOtherKeyItemText.Visible = true; lblOtherKeyItemTitle.Visible = true;
                        imgOtherKeyLearnMoreIcon.Visible = true;
                    }
                }
                catch (Exception ex)
                { }

            }
            else
            {
                //TODO: Will need to go through app and watch for this kind of thing. shouldn't happen, but will need to add error handling.
            }
        }
        protected void BindRangeItem(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            { return; }

            DataRowView drvRow = (DataRowView)e.Item.DataItem;
            totalRangeMin += Convert.ToDouble(drvRow.Row["RangeMin"].ToString());
            totalRangeMax += Convert.ToDouble(drvRow.Row["RangeMax"].ToString());
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
        protected DataTable GetSelectedFacilityDetails()
        {
            DataTable FacilityInfo = new DataTable();

            //Which employer database?
            SqlConnection conn = new SqlConnection(ThisSession.CnxString);
            SqlCommand sqlCmd = new SqlCommand("", conn);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            if (ThisSession.ChosenLabs == null)
            {
                sqlCmd.CommandText = "GetSelectedFacilityDetailsForService";
                SqlParameter userParm = new SqlParameter("@ServiceName", SqlDbType.VarChar, 200);
                userParm.Value = ThisSession.ServiceName;
                sqlCmd.Parameters.Add(userParm);
                userParm = new SqlParameter("@PracticeName", SqlDbType.VarChar, 150);
                userParm.Value = ThisSession.PracticeName;
                sqlCmd.Parameters.Add(userParm);
                userParm = new SqlParameter("@NPI", SqlDbType.VarChar, 50);
                userParm.Value = ThisSession.PracticeNPI;
                sqlCmd.Parameters.Add(userParm);
                userParm = new SqlParameter("@SpecialtyID", SqlDbType.Int);
                userParm.Value = ThisSession.SpecialtyID;
                sqlCmd.Parameters.Add(userParm);
                userParm = new SqlParameter("@ServiceID", SqlDbType.Int);
                userParm.Value = ThisSession.ServiceID;
                sqlCmd.Parameters.Add(userParm);
                userParm = new SqlParameter("@OrganizationLocationID", SqlDbType.Int);
                userParm.Value = ThisSession.OrganizationLocationID;
                sqlCmd.Parameters.Add(userParm);
            }
            else
            {
                sqlCmd.CommandText = "GetSelectedFacilityDetailsForMultiLab";
                SqlParameter userParm = new SqlParameter("OrganizationID", SqlDbType.Int);
                userParm.Value = ThisSession.ChosenLabID;
                sqlCmd.Parameters.Add(userParm);
                userParm = new SqlParameter("OrganizationLocationID", SqlDbType.Int);
                userParm.Value = ThisSession.ChosenLabLocationID;
                sqlCmd.Parameters.Add(userParm);
                userParm = new SqlParameter("ServiceList", SqlDbType.Structured);
                string[] iCols = { "ServiceID" };
                DataView iDV = new DataView(ThisSession.ChosenLabs);
                userParm.Value = iDV.ToTable("ServiceList", true, iCols);
                sqlCmd.Parameters.Add(userParm);
            }

            SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                adp.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    FacilityInfo = ds.Tables[0];

                    //Populate "Learn More".
                    //      NOTE: not sure this is the best place, but don't want to store a ton of text in session...
                    if (ds.Tables.Count > 1)
                    {
                        if (ds.Tables[1].Rows[0][0].ToString() != String.Empty)
                        {
                            lblServiceMoreInfo.Text = ds.Tables[1].Rows[0][0].ToString();
                        }
                        else
                        {
                            imgSvcLearnMore.Visible = false;
                            lblServiceName_MoreInfoTitle.Text = string.Empty;
                        }
                        if (ds.Tables.Count > 2) // did we get specialty info?
                        {
                            //Specialty Title
                            if (ds.Tables[2].Rows[0]["Title"].ToString() != String.Empty)
                            {
                                lblServiceName.Text = ds.Tables[2].Rows[0]["Title"].ToString() + ": " + ThisSession.ServiceName;
                            }
                            else
                            {
                                if (ThisSession.Specialty != string.Empty)
                                    lblServiceName.Text = ThisSession.Specialty + ": " + ThisSession.ServiceName;
                            }
                            //Doctor Title Text (title bar for doctor list area)
                            if (ds.Tables[2].Rows[0]["DoctorTitleText"].ToString() != String.Empty)
                            {
                                ltlShowDocs.Text = ds.Tables[2].Rows[0]["DoctorTitleText"].ToString() + " At This Facility";
                            }
                        }
                        if (ds.Tables.Count > 3) // did we get doctors in specialty?
                        {
                            if (ds.Tables[3].Rows.Count > 0)
                            {
                                //bind results to datalist.
                                dlDoctors.DataSource = ds.Tables[3];
                                dlDoctors.DataBind();

                                ltlShowDocs.Text = (ds.Tables[3].Rows.Count == 1 ? "Doctor At This Facility" : "Doctors At This Facility");
                            }
                            else
                            {
                                ltlShowDocs.Text = @"<script>$(""#docs"").toggleClass(""hidden"");</script>";
                            }
                        }
                        else
                        {
                            ltlShowDocs.Text = @"<script>$(""#docs"").toggleClass(""hidden"");</script>";
                        }
                    }
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
        protected void ibtnPrintResults_Click(object sender, ImageClickEventArgs e)
        {
            //Response.Redirect("print.aspx");
            //Session["ctrl"] = pnlPrint; // this;
            //ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('Print.aspx','PrintMe','height=300px,width=300px,scrollbars=1');</script>");
        }
        protected void dlDoctors_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                //LinkButton lbtnDoctor = (LinkButton)e.Item.FindControl("lbtnDoctor");
                Label lblDoctor = (Label)e.Item.FindControl("lblDoctor");

                //Are we getting Health Grades details, or manually provided details?
                if (drv["DataSource"].ToString() == "Local")
                {
                    //Local manually populated list:
                    //lbtnDoctor.Text = drv["DoctorName"].ToString();
                    //lbtnDoctor.CommandName = "Select";
                    //lbtnDoctor.CommandArgument = drv["DoctorName"].ToString();//Will need something better than this probably.
                    lblDoctor.Text = drv["DoctorName"].ToString();
                }
                else
                {
                    //Health Grades:
                    //lbtnDoctor.Text = drv["LastName"].ToString() + ", " + drv["FirstName"].ToString() + " " + drv["Degree"].ToString();
                    //lbtnDoctor.CommandName = "Select";
                    //lbtnDoctor.CommandArgument = drv["ProviderID"].ToString() + "|" + drv["NPI"].ToString();
                    lblDoctor.Text = string.Format("{0}, {1} {2}",
                        drv["LastName"],
                        drv["FirstName"],
                        drv["Degree"]);
                }
            }
        }
    }
}