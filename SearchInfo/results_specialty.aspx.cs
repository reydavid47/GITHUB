using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.IO;
using QSEncryption.QSEncryption;

namespace ClearCostWeb.SearchInfo
{
    public partial class results_specialty : System.Web.UI.Page
    {
        #region Private Variables
        private DataSet Results
        {
            get
            {
                return (ViewState["Results"] == null ? new DataSet() : ((DataSet)ViewState["Results"]));
            }
            set { ViewState["Results"] = value; }
        }
        private Boolean fromFindADoc
        {
            get
            {
                return (ViewState["fromFindADoc"] == null ? false : ((Boolean)ViewState["fromFindADoc"]));
            }
            set { ViewState["fromFindADoc"] = value; }
        }

        protected String InitialSortOption  //  lam, 20130618, MSB-324
        {
            get
            {
                return ThisSession.DefaultSort == "" ? "Distance" : ThisSession.DefaultSort;
            }
        }

        protected Boolean AllowFairPriceSort
        {
            get
            {
                return ThisSession.AllowFairPriceSort;
            }
        }

        private SqlParameter prmServiceName = new SqlParameter("ServiceName", SqlDbType.NVarChar, 200);
        private SqlParameter prmLatitude = new SqlParameter("Latitude", SqlDbType.Float);
        private SqlParameter prmLongitude = new SqlParameter("Longitude", SqlDbType.Float);
        private SqlParameter prmSpecialtyID = new SqlParameter("SpecialtyID", SqlDbType.Int);
        private SqlParameter prmMemberMedicalID = new SqlParameter("MemberMedicalID", SqlDbType.NVarChar, 50);
        private SqlParameter prmUserID = new SqlParameter("UserID", SqlDbType.NVarChar, 36);
        private const String FindADocProc = "GetDoctorsForService";

        private enum PostBacksIHandle { Geocoder, Doctor, Fault }
        private Dictionary<String, PostBacksIHandle> AcceptablePostBackControls = new Dictionary<String, PostBacksIHandle>
    {
        {"Geocoder", PostBacksIHandle.Geocoder},
        {"Result", PostBacksIHandle.Doctor}
    };
        #endregion

        #region Callback Parts
        private ResultsInfo resOut;
        public void RaiseCallbackEvent(String eventArgument)
        {
            try
            {
                //Page.ClientScript.ValidateEvent(this.pnlServices.UniqueID);

                JavaScriptSerializer js = new JavaScriptSerializer();
                CallbackActionRequest car = js.Deserialize<CallbackActionRequest>(eventArgument);
                car.ResultSet = ResultsTable;
                //car.Act();
                //car.SortResults();

                resOut = car.RetrieveResultsInfo();
            }
            catch (Exception ex)
            { }
        }
        public string GetCallbackResult()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (resOut == null) { resOut = new ResultsInfo(); }
            return js.Serialize(resOut);
        }
        #endregion

        #region Properties
        private DataTable ResultsTable { get { return ((Results.Tables.Count >= 1) ? (Results.Tables[0]) : (new DataTable())); } }
        private DataTable DescriptionTable { get { return ((Results.Tables.Count >= 2) ? (Results.Tables[1]) : (new DataTable())); } }
        private DataTable LearnMoreTable { get { return ((Results.Tables.Count >= 3) ? (Results.Tables[2]) : (new DataTable())); } }

        public Boolean FromFindADoc { get { return fromFindADoc; } }
        private String PostBackControl { get { return Request.Params["__EVENTTARGET"].ToString(); } }
        private PostBacksIHandle AcceptablePostBackControl { get { return (AcceptablePostBackControls.ContainsKey(PostBackControl) ? AcceptablePostBackControls[PostBackControl] : PostBacksIHandle.Fault); } }
        private String PostBackArgument { get { return Request.Params["__EVENTARGUMENT"].ToString(); } }
        private String[] PostBackLatLng { get { return (PostBackControl == "Geocoder" ? PostBackArgument.Split('|') : null); } }

        protected Boolean IsCaesarsOphthalmology { get { return ((ThisSession.SpecialtyID == 18) && (ThisSession.EmployerID == "11")); } }  //  lam, 20130426, CI-51
        protected Boolean IsMentalHealth { get { return ((new List<String>(new String[] { "24", "42" })).Contains(ThisSession.SpecialtyID.ToString())); } }  //  lam, 20130508, CI-144

        #endregion

        #region GUI Methods
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            JavaScriptHelper.IncludeFindADocResults(Page.ClientScript);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //ltlCallbackScript.Text = @"<script type=""text/javascript"">function CallServer(arg, context) { " + Page.ClientScript.GetCallbackEventReference(this, "arg", "ReceiveServerData", "context") + "; }</script>";
            //Page.Header.Controls.Add(
            //        new LiteralControl(
            //            String.Format(@"<script src=""{0}?Rev={1}"" type=""text/javascript""></script>",
            //                ResolveUrl("~/Scripts/FindADocResults.js"),
            //                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString())));

            if (!Page.IsPostBack)
            {
                lblSpecialty.Text = lblSpecialty_MoreInfoTitle.Text = ThisSession.Specialty;
                fromFindADoc = !(Request.UrlReferrer == null ? true : Request.UrlReferrer.ToString().ToLower().Contains("specialty_search"));
                //GetDocsForSpecialty(ThisSession.PatientLatitude, ThisSession.PatientLongitude);           

                lblAllResult1DisclaimerText.Text = ThisSession.AllResult1DisclaimerText;  //  lam, 20130425, MSF-295 move disclaimer text to content manager
                lblMentalHealthDisclaimerText.Text = ThisSession.MentalHealthDisclaimerText;  //  lam, 20130508, CI-144
            }
            else
            {
                if (POSTDIST.Value.Replace("undefined", "") != "" && POSTNAV.Value.Replace("undefined", "") != "")
                {
                    ThisSession.FacilityDistance = POSTDIST.Value;
                    QueryStringEncryption qs = new QueryStringEncryption(POSTNAV.Value, new Guid(ThisSession.UserLogginID));
                    ThisSession.PracticeName = qs["PracticeName"];
                    ThisSession.ProviderName = qs["ProviderName"];
                    ThisSession.PracticeNPI = qs["PracticeNPI"];
                    ThisSession.TaxID = qs["TaxID"];
                    ThisSession.OrganizationLocationID = Convert.ToInt32(qs["OrganizationLocationID"]);
                    //  lam, 20130313, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
                    Response.Redirect("doctor_specialty_detail.aspx#" + (ThisSession.ServiceEntered.ToLower().Contains("office") ? "tabcare" : "tabdoc"));
                    //Response.Redirect("doctor_specialty_detail.aspx#tabdoc");
                }
                //switch (AcceptablePostBackControl)
                //{
                //    case PostBacksIHandle.Geocoder:
                //        //Handle the event that the user has changed their address
                //        GetDocsForSpecialty(PostBackLatLng[0], PostBackLatLng[1]);
                //        break;
                //    case PostBacksIHandle.Doctor:
                //        //Handle the event that the user has selected a doctor result
                //        SelectDoctor();
                //        break;
                //    case PostBacksIHandle.Fault:
                //        //The post back was not recognized so same as "default" switch
                //        break;
                //    default:
                //        //Do Nothing
                //        break;
                //}
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected override void Render(HtmlTextWriter writer)
        {
            //Page.ClientScript.RegisterForEventValidation(this.pnlServices.UniqueID);
            base.Render(writer);
        }

        protected int distance = 25;
        protected void updateDistance(object sender, EventArgs e)
        {
            distance = sFindADoc.Value;
            lblSliderValue.Text = String.Concat(" ", sFindADoc.Value, " mile(s)");
        }
        #endregion

        #region Helper Methods
        private void SelectDoctor()
        {
            //Store session variables from postback
            string[] args = PostBackArgument.Split('|');
            ThisSession.PracticeName = args[0];
            ThisSession.ProviderName = args[1];
            ThisSession.PracticeNPI = args[2];
            ThisSession.FacilityDistance = args[3];
            ThisSession.TaxID = args[4];
            ThisSession.OrganizationLocationID = Convert.ToInt32(args[5]);

            //Navigate forward
            //  lam, 20130313, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
            Response.Redirect("doctor_specialty_detail.aspx#" + (ThisSession.ServiceEntered.ToLower().Contains("office") ? "tabcare" : "tabdoc"));
            //Response.Redirect("doctor_specialty_detail.aspx#tabdoc");
        }
        protected void GetDocsForSpecialty(String Lat, String Lng)
        {
            List<SqlParameter> FindADocParms = new List<SqlParameter>();
            prmServiceName.Value = ThisSession.ServiceName;
            FindADocParms.Add(prmServiceName);
            prmLatitude.Value = Lat;
            FindADocParms.Add(prmLatitude);
            prmLongitude.Value = Lng;
            FindADocParms.Add(prmLongitude);
            prmSpecialtyID.Value = ThisSession.SpecialtyID;
            FindADocParms.Add(prmSpecialtyID);
            prmMemberMedicalID.Value = ThisSession.SubscriberMedicalID;
            FindADocParms.Add(prmMemberMedicalID);
            prmUserID.Value = Membership.GetUser().ProviderUserKey.ToString();
            FindADocParms.Add(prmUserID);

            Results = QueryDBFor(FindADocParms, ThisSession.CnxString);
            if (LearnMoreTable.Rows.Count > 0)
            {
                if (LearnMoreTable.Rows[0]["Specialty"].ToString() != string.Empty)
                {
                    //lblSpecialtyMoreInfo.Text = LearnMoreTable.Rows[0]["LearnMore"].ToString();
                    //lblSpecialty_MoreInfoTitle.Text = LearnMoreTable.Rows[0]["Specialty"].ToString();
                }
                else
                {
                    imgSpcLearnMore.Visible = false;
                    lblSpecialty_MoreInfoTitle.Text = string.Empty;
                }
                if (LearnMoreTable.Rows[0]["Title"].ToString() != string.Empty)
                {
                    lblSpecialty.Text = LearnMoreTable.Rows[0]["Title"].ToString();
                    lblSpecialty_MoreInfoTitle.Text = LearnMoreTable.Rows[0]["Title"].ToString();
                }
            }

            ApplyGoogleDistances(Lat, Lng);
        }
        private void ApplyGoogleDistances(String Lat, String Lng)
        {
            DataColumn googleColumn = new DataColumn("GoogleDistance");
            Results.Tables[ResultsTable.TableName, ResultsTable.Namespace].Columns.Add(googleColumn);

            string originString = string.Format("{0},{1}", Lat, Lng);
            List<string> destinations = new List<string>();

            foreach (DataRow dr in ResultsTable.Rows)
                destinations.Add(String.Format("{0},{1}", dr["Latitude"].ToString(), dr["Longitude"].ToString()));

            string[] distances = GoogleHelper.GetDistances(originString, destinations.ToArray());
            for (int i = 0; i < ResultsTable.Rows.Count; i++)
            {
                if ((i + 1) <= distances.Length)
                {
                    if (distances[i] != "")
                    {
                        Results.Tables[ResultsTable.TableName, ResultsTable.Namespace].Rows[i]["Distance"] = distances[i];
                        Results.Tables[ResultsTable.TableName, ResultsTable.Namespace].Rows[i]["NumericDistance"] = Convert.ToDouble(distances[i].Replace(" mi", string.Empty).Trim());
                    }
                    else
                    {
                        Results.Tables[ResultsTable.TableName, ResultsTable.Namespace].Rows[i]["Distance"] = string.Format("{0:##0.0}", ResultsTable.Rows[i]["NumericDistance"]) + " mi";
                    }
                }
                else
                {
                    Results.Tables[ResultsTable.TableName, ResultsTable.Namespace].Rows[i]["Distance"] = string.Format("{0:##0.0}", ResultsTable.Rows[i]["NumericDistance"]) + " mi";
                }
            }
        }
        private DataSet QueryDBFor(List<SqlParameter> InpParms, String CnxString)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(CnxString))
                {
                    using (SqlCommand comm = new SqlCommand(FindADocProc, conn))
                    {
                        comm.CommandType = CommandType.StoredProcedure;
                        InpParms.ForEach(delegate(SqlParameter prm)
                        {
                            comm.Parameters.Add(prm);
                        });
                        using (SqlDataAdapter da = new SqlDataAdapter(comm))
                        {
                            da.Fill(ds);
                        }
                        comm.Parameters.Clear();
                    }
                }
            }
            catch (SqlException sqEx)
            { }
            catch (Exception ex)
            { }
            finally
            { }
            return ds;
        }
        #endregion

        #region Other To Be Removed
        //protected void GetDocsForSpecialty(String Lat, String Lng)
        //{
        //    DataTable docsDT = SearchForDocs(Lat, Lng);


        //    DataColumn googleColumn = new DataColumn("GoogleDistance");
        //    docsDT.Columns.Add(googleColumn);

        //    //Modified to render prior to the page rendering
        //    string originString = string.Format("{0},{1}", Lat, Lng);
        //    List<string> destinations = new List<string>();
        //    for (int i = 0; i < docsDT.Rows.Count; i++)
        //    {
        //        destinations.Add(docsDT.Rows[i]["LatLong"].ToString());
        //    }

        //    string[] distances = GoogleHelper.GetDistances(originString, destinations.ToArray());

        //    for (int i = 0; i < docsDT.Rows.Count; i++)
        //    {
        //        if (distances.Length > (i + 1))
        //        {
        //            docsDT.Rows[i]["Distance"] = distances[i];
        //            docsDT.Rows[i]["NumericDistance"] = Convert.ToDouble(distances[i].Replace(" mi", string.Empty).Trim());
        //        }
        //        else
        //        {
        //            docsDT.Rows[i]["Distance"] = string.Format("{0:##0.0}", docsDT.Rows[i]["NumericDistance"]) + " mi";
        //        }
        //    }

        //    LoadDoctorGrid(docsDT);

        //    //ThisSession.FacilityList = docsDT;

        //    //TODO: Get the distance working!
        //    //string JScript = GenerateGoogleRequest(docsDT);
        //    //ClientScriptManager CSManager = Page.ClientScript;
        //    //CSManager.RegisterStartupScript(Page.GetType(), "GetDistances", JScript, true);

        //    //rptResults.DataSource = docsDT;
        //    //rptResults.DataBind();
        //}


        //protected DataTable SearchForDocs(String Lat, String Lng)
        //{
        //    DataTable FacilityList = new DataTable();

        //    //Which employer database?
        //    SqlConnection conn = new SqlConnection(ThisSession.CnxString);
        //    SqlCommand sqlCmd;
        //    sqlCmd = new SqlCommand("GetDoctorsForService", conn);
        //    sqlCmd.CommandType = CommandType.StoredProcedure;
        //    SqlParameter userParm = new SqlParameter("@ServiceName", SqlDbType.VarChar, 200);
        //    userParm.Value = ThisSession.ServiceName;
        //    sqlCmd.Parameters.Add(userParm);
        //    userParm = new SqlParameter("@Latitude", SqlDbType.Float);
        //    userParm.Value = Lat;
        //    sqlCmd.Parameters.Add(userParm);
        //    userParm = new SqlParameter("@Longitude", SqlDbType.Float);
        //    userParm.Value = Lng;
        //    sqlCmd.Parameters.Add(userParm);
        //    userParm = new SqlParameter("@SpecialtyID", SqlDbType.Int);
        //    userParm.Value = ThisSession.SpecialtyID;
        //    sqlCmd.Parameters.Add(userParm);

        //    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
        //    DataSet ds = new DataSet();

        //    try
        //    {
        //        conn.Open();
        //        adp.Fill(ds);
        //        if (ds.Tables.Count > 0)
        //        {
        //            FacilityList = ds.Tables[0];

        //            //Populate "Learn More" and less verbose Title.
        //            //      NOTE: not sure this is the best place, but don't want to store a ton of text in session...
        //            if (ds.Tables.Count > 2)
        //            {
        //                //Learn More:
        //                if (ds.Tables[2].Rows[0][0].ToString() != String.Empty)
        //                {
        //                    lblSpecialtyMoreInfo.Text = ds.Tables[2].Rows[0]["LearnMore"].ToString();
        //                    lblSpecialty_MoreInfoTitle.Text = ds.Tables[2].Rows[0]["Specialty"].ToString();
        //                }
        //                else
        //                {
        //                    imgSpcLearnMore.Visible = false;
        //                    lblSpecialty_MoreInfoTitle.Text = string.Empty;
        //                }
        //                //Specialty Title
        //                if (ds.Tables[2].Rows[0]["Title"].ToString() != String.Empty)
        //                {
        //                    lblSpecialty.Text = ds.Tables[2].Rows[0]["Title"].ToString();
        //                    lblSpecialty_MoreInfoTitle.Text = ds.Tables[2].Rows[0]["Title"].ToString();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string testing = ex.Message;
        //    }
        //    finally
        //    { conn.Close(); }

        //    return FacilityList;
        //}

        protected void LoadDoctorGrid(DataTable FacilityList)
        {
            //DataTable FacilityList = SearchForFacilities();
            DataView dv = new DataView(FacilityList);

            //if (rblSort.SelectedIndex > -1)
            //{
            //if (rblSort.SelectedValue == "HGOverallRating")
            //{
            //LB 9/20/11 - Not sure of the syntax here - test.
            //dv.Sort = rblSort.SelectedValue + " desc, NumericDistance";
            //}
            //else
            //{
            //dv.Sort = rblSort.SelectedValue;
            //}
            //}
            //else
            //{
            //dv.Sort = "NumericDistance";
            //}

            //rptResults.DataSource = dv;
            //rptResults.DataBind();

            ThisSession.FacilityList = dv.ToTable();
            //UpdatePanel1.Update(); // not working?!

        }

        protected void ibtnPrintResults_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("print.aspx");

        }
        protected void rblSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDoctorGrid(ThisSession.FacilityList);
        }
        protected void FacilitySort(object sender, EventArgs e)
        {
            //rblSort.SelectedValue = "ProviderName";
            LoadDoctorGrid(ThisSession.FacilityList);
        }
        protected void DistanceSort(object sender, EventArgs e)
        {
            //rblSort.SelectedValue = "NumericDistance";
            LoadDoctorGrid(ThisSession.FacilityList);
        }

        protected void RatingSort(object sender, EventArgs e)
        {
            //rblSort.SelectedValue = "HGOverallRating";
            LoadDoctorGrid(ThisSession.FacilityList);
        }
        protected void SelectFacility(object sender, EventArgs e)
        {
            //Whic facility line were they on?
            LinkButton lbtnSelectFacility = (LinkButton)sender;
            string[] args = lbtnSelectFacility.CommandArgument.Split('|');

            // Put facility key information in Session
            ThisSession.PracticeName = args[0];
            ThisSession.ProviderName = args[1];
            ThisSession.PracticeNPI = args[2];

            foreach (DataRow dr in ThisSession.FacilityList.Rows)
            {
                if (dr["ProviderName"].ToString() == ThisSession.ProviderName)
                { ThisSession.FacilityDistance = dr["Distance"].ToString(); }
            }
            if (ThisSession.FacilityDistance.EndsWith(" mi"))
            { ThisSession.FacilityDistance = ThisSession.FacilityDistance.Substring(0, ThisSession.FacilityDistance.Length - 3); }


            // Go to the Care Detail page for the selected Facility.
            Response.Redirect("doctor_specialty_detail.aspx");
        }

        protected void rptResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                // set the visibility of the down arrow on the headers to show which it's sorted by.
                Image imgFacilitySort = (Image)e.Item.FindControl("imgFacilitySort");
                Image imgDistanceSort = (Image)e.Item.FindControl("imgDistanceSort");
                Image imgRatingSort = (Image)e.Item.FindControl("imgRatingSort");

                // default to not visible
                imgFacilitySort.Visible = false;
                imgDistanceSort.Visible = false;
                imgRatingSort.Visible = false;

                //switch (rblSort.SelectedIndex)
                //{
                //    case -1:
                //        imgDistanceSort.Visible = true;
                //        break;
                //   case 0:
                //        imgFacilitySort.Visible = true;
                //        break;
                //    case 1:
                //        imgDistanceSort.Visible = true;
                //        break;
                //    case 2:
                //        imgRatingSort.Visible = true;
                //        break;
                // }

            }
            else
            {
                if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                { return; }

                // Visibility of Fair Price and Health Grades check images.
                DataRowView drv = (DataRowView)e.Item.DataItem;

                Image imgFairPriceTrue = (Image)e.Item.FindControl("imgFairPriceTrue");
                Image imgFairPriceFalse = (Image)e.Item.FindControl("imgFairPriceFalse");
                //Label lblTest = (Label)e.Item.FindControl("lblTest");
                if (drv["FairPrice"].ToString() == "True")
                { imgFairPriceTrue.Visible = true; imgFairPriceFalse.Visible = false; }
                else
                { imgFairPriceTrue.Visible = false; imgFairPriceFalse.Visible = true; }

                Image imgHGRecognizedTrue = (Image)e.Item.FindControl("imgHGRecognizedTrue");
                Image imgHGRecognizedFalse = (Image)e.Item.FindControl("imgHGRecognizedFalse");
                if (drv["HGRecognized"].ToString() == "True")
                { imgHGRecognizedTrue.Visible = true; imgHGRecognizedFalse.Visible = false; }
                else
                { imgHGRecognizedTrue.Visible = false; imgHGRecognizedFalse.Visible = true; }

                //Add command arg to Practice Name
                LinkButton lbtnSelectFacility = (LinkButton)e.Item.FindControl("lbtnSelectFacility");
                lbtnSelectFacility.CommandName = "Select";
                //NOTE: not sure how this will work out in long run. Put both in here for now.
                lbtnSelectFacility.CommandArgument = drv["PracticeName"].ToString() + "|" +
                    drv["ProviderName"].ToString() + "|" + drv["NPI"].ToString();

                lbtnSelectFacility.Attributes.Add("Lat", drv["Latitude"].ToString());
                lbtnSelectFacility.Attributes.Add("Lng", drv["Longitude"].ToString());
                lbtnSelectFacility.Attributes.Add("FP", drv["FairPrice"].ToString());
                lbtnSelectFacility.Attributes.Add("HG", drv["HGRecognized"].ToString());

                //Literal for Star Rating
                Literal ltlRating = (Literal)e.Item.FindControl("ltlRating");

                //Survey count text
                Label lblSurveyCountText = (Label)e.Item.FindControl("lblSurveyCountText");

                //Set Star Label display based on average rating.
                double rating = double.Parse(drv["HGOverallRating"].ToString());
                int patientCount = int.Parse(drv["HGPatientCount"].ToString());
                String sRating = "";
                if (patientCount == 0)
                {
                    sRating += "<div>N/A<div>";
                    lblSurveyCountText.Text = "0 patient surveys";
                }
                else
                {
                    //Loop through all of the whole star ratings (i.e. draw 4 whole stars if rating 4.5)
                    for (int i = 1; i <= (int)rating; i++)
                    {
                        sRating += @"<div class=""star_full""></div>";
                    }
                    //Add any half stars that may be needed
                    if ((rating % 1) > 0.0)
                    {
                        sRating += @"<div class=""star_half""></div>";
                    }
                    //Finish up the last of the 5 stars with empties
                    for (int i = 1; i <= (5 - (((int)rating) + ((rating % 1) > 0.0 ? 1 : 0))); i++)
                    {
                        sRating += @"<div class=""star_none""></div>";
                    }

                    // Survey singular if only one patient.
                    if (patientCount == 1)
                    { lblSurveyCountText.Text = "1 patient survey"; }
                    else { lblSurveyCountText.Text = patientCount.ToString() + " patient surveys"; }

                }//End of If HG Patient Count <> 0
                ltlRating.Text = sRating;
            }
        }

        protected string GetItemClass(int itemIndex)
        {
            if (itemIndex == 0)
            {
                return "roweven graydiv graytop";
            }
            else
            {
                return "roweven graydiv";
            }
        }
        #endregion

        private class ResultsInfo : results_specialty
        {
            public int ResultCount;
            public Boolean EndOfResults;
            public Result[] Results;
        }
        private class Result : results_specialty
        {
            private int index;
            public Practice Practice;
            public Provider[] Providers;
            private double avgProvMin
            {
                get
                {
                    double avg = 0.0;
                    foreach (Provider p in this.Providers)
                    {
                        avg += double.Parse(p.RangeMin);
                    }
                    return (avg / this.Providers.Length);
                }
            }
            private double avgProvMax
            {
                get
                {
                    double avg = 0.0;
                    foreach (Provider p in this.Providers)
                    {
                        avg += double.Parse(p.RangeMax);
                    }
                    return (avg / this.Providers.Length);
                }
            }
            private bool hasCCDoc { get { bool anyCC = false; foreach (Provider p in Providers) { anyCC = p.IsFairPrice; if (anyCC) { break; } }; return anyCC; } }
            private bool hasHGDoc { get { bool anyHG = false; foreach (Provider p in Providers) { anyHG = p.IsHGRecognized; if (anyHG) { break; } }; return anyHG; } }

            public string HTML
            {
                get
                {
                    String retHTML = "";
                    retHTML = this.practiceHTML;
                    if (this.Providers.Length > 1)
                    {
                        foreach (Provider p in this.Providers)
                        {
                            retHTML += p.GetHTML();
                        }
                    }
                    return retHTML;
                }
            }
            public string InfoHTML
            {
                get
                {
                    StringWriter stringWriter = new StringWriter();
                    using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
                    {
                        //Paragraph
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "smaller infoWin");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "220px");
                        writer.RenderBeginTag(HtmlTextWriterTag.P);

                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        //Only 1 provider?
                        if (this.Providers.Length == 1)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Id, "Result" + this.index);
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "readmore result");
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            writer.AddAttribute("nav", this.Providers[0].Nav);
                            writer.AddAttribute("onclick", "SelectResult($(this).attr('nav'));");
                            writer.RenderBeginTag(HtmlTextWriterTag.A); //Provider Name Link
                            writer.Write(this.Providers[0].Name);
                            writer.RenderEndTag(); //End Provider name A link

                            writer.WriteBreak();//New Line

                            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                            writer.RenderBeginTag(HtmlTextWriterTag.Span); //Practice Name Span
                            writer.Write(this.Practice.Name);
                            writer.RenderEndTag();//End Practice Span

                            writer.WriteBreak(); //New Line

                            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                            writer.RenderBeginTag(HtmlTextWriterTag.Span); //Address Span
                            writer.Write(this.Practice.Address1 + ", " + this.Practice.City);
                            writer.RenderEndTag(); //End Address span
                        }
                        else //More than 1
                        {
                            //Result Facility Name
                            writer.AddAttribute(HtmlTextWriterAttribute.Id, "Result" + this.index);
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "readmore result");
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            writer.AddAttribute("nav", this.Practice.Nav);
                            writer.AddAttribute("onclick", "SelectResult($(this).attr('nav'));");
                            writer.RenderBeginTag(HtmlTextWriterTag.A);
                            writer.Write(this.Practice.Name);
                            writer.RenderEndTag(); //End A

                            writer.WriteBreak();//New Line

                            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                            writer.RenderBeginTag(HtmlTextWriterTag.Span); //Expand Span
                            writer.Write("(");

                            writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            writer.AddAttribute("onclick", "ShowDocs(" + this.index + "); $('a.table-map').click();");
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "AshowDoc");
                            writer.AddAttribute("parent", this.index.ToString());
                            writer.RenderBeginTag(HtmlTextWriterTag.A); //Expand Link
                            writer.Write("see all " + this.Providers.Length + " physicians");
                            writer.RenderEndTag(); //End expand link

                            writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            writer.AddAttribute("onclick", "HideDocs(" + this.index + "); $('a.table-map').click();");
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "AhideDoc");
                            writer.AddAttribute("parent", this.index.ToString());
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                            writer.RenderBeginTag(HtmlTextWriterTag.A); //Hide Link
                            writer.Write("hide physicians");
                            writer.RenderEndTag(); //End Hide link

                            writer.Write(")");
                            writer.RenderEndTag();//End Practice Span

                            writer.WriteBreak(); //New Line

                            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                            writer.RenderBeginTag(HtmlTextWriterTag.Span); //Address Span
                            writer.Write(this.Practice.Address1 + ", " + this.Practice.City);
                            writer.RenderEndTag(); //End Address span
                        }
                        writer.RenderEndTag();

                        //Clear Choice
                        if (this.hasCCDoc)
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Div);
                            writer.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/check_green.png");
                            writer.AddAttribute(HtmlTextWriterAttribute.Alt, "ClearChoice Doc?");
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                            writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                            writer.RenderBeginTag(HtmlTextWriterTag.Img); //BEGIN IMAGE
                            writer.RenderEndTag(); //END IMAGE
                            writer.Write("Clear Choice Doc");
                            writer.RenderEndTag();
                        }

                        //Healthgrades Recognized Physician
                        if (this.hasHGDoc)
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Div);
                            writer.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/check_purple.png");
                            writer.AddAttribute(HtmlTextWriterAttribute.Alt, "Health Grades Recognized?");
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                            writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                            writer.RenderBeginTag(HtmlTextWriterTag.Img); //BEGIN IMAGE
                            writer.RenderEndTag(); //END IMAGE
                            writer.Write("Healthgrades Recognized Provider");
                            writer.RenderEndTag();
                        }
                        //Rating
                        if (this.Providers.Length > 1)
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Div);

                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "AresShow");
                            writer.AddAttribute("parent", this.index.ToString());
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            writer.AddAttribute("onclick", "ShowDocs(" + this.index + "); $('a.table-map').click();");
                            writer.RenderBeginTag(HtmlTextWriterTag.A); //Provider Name Link
                            writer.Write("Click for Ratings");
                            writer.RenderEndTag(); //End Provider name A link

                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "AresHide");
                            writer.AddAttribute("parent", this.index.ToString());
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            writer.AddAttribute("onclick", "HideDocs(" + this.index + "); $('a.table-map').click();");
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                            writer.RenderBeginTag(HtmlTextWriterTag.A); //Provider Name Link
                            writer.Write("Hide Ratings");
                            writer.RenderEndTag(); //End Provider name A link

                            writer.RenderEndTag(); //End DIV
                        }
                        else
                        {
                            //Rating
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ratingRow");
                            writer.RenderBeginTag(HtmlTextWriterTag.Div);
                            double rat = Convert.ToDouble(this.Providers[0].HGRating);
                            for (int i = 1; i <= (int)rat; i++)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "star_full");
                                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                                writer.RenderEndTag();
                            }
                            if ((rat % 1) > 0.0)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "star_half");
                                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                                writer.RenderEndTag();
                            }
                            if (rat == -1.0) { rat = 0.0; }
                            for (int i = 1; i <= (5 - (((int)rat) + ((rat % 1) > 0.0 ? 1 : 0))); i++)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "star_none");
                                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                                writer.RenderEndTag();
                            }
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ratings");
                            writer.RenderBeginTag(HtmlTextWriterTag.P);
                            writer.RenderBeginTag(HtmlTextWriterTag.Span);
                            writer.Write(string.Format("{0} patient survey{1}", this.Providers[0].HGRatingCount, (Convert.ToInt32(this.Providers[0].HGRatingCount) == 1 ? "" : "s")));
                            writer.RenderEndTag(); //End Span
                            writer.RenderEndTag(); //End P
                            writer.RenderEndTag(); //End DIV
                        }

                        writer.RenderEndTag(); //END P
                    }
                    return stringWriter.ToString();
                }
            }
            private string practiceHTML
            {
                get
                {
                    StringWriter stringWriter = new StringWriter();
                    using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
                    {
                        //Table Row
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "resultRow graydiv" + ((this.index % 2) == 0 ? "" : " roweven") + ((this.index == 1) ? " graytop" : ""));
                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                        //Result Name and Location
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "tdfirst graydiv NameLoc");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "32%");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //Column 1
                        //Only 1 provider?
                        if (this.Providers.Length == 1)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Id, "Result" + this.index);
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "readmore result");
                            //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            writer.AddAttribute("nav", this.Providers[0].Nav);
                            writer.AddAttribute("onclick", "SelectResult($(this).attr('nav'));");
                            writer.RenderBeginTag(HtmlTextWriterTag.A); //Provider Name Link
                            writer.Write(this.Providers[0].Name);
                            writer.RenderEndTag(); //End Provider name A link

                            writer.WriteBreak();//New Line

                            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                            writer.RenderBeginTag(HtmlTextWriterTag.Span); //Practice Name Span
                            writer.Write(this.Practice.Name);
                            writer.RenderEndTag();//End Practice Span

                            writer.WriteBreak(); //New Line

                            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                            writer.RenderBeginTag(HtmlTextWriterTag.Span); //Address Span
                            writer.Write(this.Practice.Address1 + ", " + this.Practice.City);
                            writer.RenderEndTag(); //End Address span
                        }
                        else //More than 1
                        {
                            //Result Facility Name
                            writer.AddAttribute(HtmlTextWriterAttribute.Id, "Result" + this.index);
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "readmore result");
                            //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            //writer.AddAttribute("nav", this.Practice.Nav);
                            //writer.AddAttribute("onclick", "SelectResult($(this).attr('nav'));");
                            writer.AddAttribute("onclick", "HideDocs(" + this.index + ")");
                            writer.RenderBeginTag(HtmlTextWriterTag.A);
                            writer.Write(this.Practice.Name);
                            writer.RenderEndTag(); //End A

                            writer.WriteBreak();//New Line

                            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                            writer.RenderBeginTag(HtmlTextWriterTag.Span); //Expand Span
                            writer.Write("(");

                            //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            writer.AddAttribute("onclick", "HideDocs(" + this.index + ")");
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "AhideDoc");
                            writer.AddAttribute("parent", this.index.ToString());
                            writer.RenderBeginTag(HtmlTextWriterTag.A); //Expand Link
                            writer.Write("hide physicians");
                            writer.RenderEndTag(); //End expand link

                            //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            writer.AddAttribute("onclick", "ShowDocs(" + this.index + ")");
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "AshowDoc");
                            writer.AddAttribute("parent", this.index.ToString());
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                            writer.RenderBeginTag(HtmlTextWriterTag.A); //Hide Link
                            writer.Write("see all " + this.Providers.Length + " physicians");
                            writer.RenderEndTag(); //End Hide link

                            writer.Write(")");
                            writer.RenderEndTag();//End Practice Span

                            writer.WriteBreak(); //New Line

                            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                            writer.RenderBeginTag(HtmlTextWriterTag.Span); //Address Span
                            writer.Write(this.Practice.Address1 + ", " + this.Practice.City);
                            writer.RenderEndTag(); //End Address span
                        }
                        writer.RenderEndTag(); //End Column 1 TD

                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv Dist");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "10%");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //Distance TD

                        writer.AddAttribute(HtmlTextWriterAttribute.Id, "dvDistance");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div); //Distance DIV                    
                        writer.RenderBeginTag(HtmlTextWriterTag.Span); //Distance Span
                        writer.Write(this.Practice.Distance);
                        writer.RenderEndTag(); //End Distance Span
                        writer.RenderEndTag(); //End Distance Div
                        writer.RenderEndTag(); //End Distance TD

                        //Estimated Initial Office Visit Cost
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv EIOVC");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "12%");
                        if (fromFindADoc)
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //TotalCost TD
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "totalCostRow");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div); //TotalCost Div
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                        writer.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                        writer.Write(string.Format("{0:c0}", this.avgProvMin));
                        writer.RenderEndTag(); //End Range Min B
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "dascol");
                        writer.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                        writer.Write("-");
                        writer.RenderEndTag(); //End DashCol B
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                        writer.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                        writer.Write(string.Format("{0:c0}", this.avgProvMax));
                        writer.RenderEndTag(); //End Range Max B
                        writer.RenderEndTag(); //End TotalCost Div
                        writer.RenderEndTag(); //End TotalCost TD

                        //Clear Choice
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv FP");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "12%");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //BEGIN TD
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, (this.hasCCDoc ? "../Images/check_green.png" : "../Images/s.gif"));
                        writer.AddAttribute(HtmlTextWriterAttribute.Alt, "ClearChoice Doc?");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                        writer.RenderBeginTag(HtmlTextWriterTag.Img); //BEGIN IMAGE
                        writer.RenderEndTag(); //END IMAGE
                        writer.RenderEndTag(); //END TD                    

                        //Healthgrades Recognized Physician
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv HG");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "17%");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //BEGIN TD
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, (this.hasHGDoc ? "../Images/check_purple.png" : "../Images/s.gif"));
                        writer.AddAttribute(HtmlTextWriterAttribute.Alt, "Health Grades Recognized?");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                        writer.RenderBeginTag(HtmlTextWriterTag.Img); //BEGIN IMAGE
                        writer.RenderEndTag(); //END IMAGE
                        writer.RenderEndTag(); //END TD

                        //Rating
                        if (this.Providers.Length > 1)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv ratingTD");
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "17%");
                            writer.RenderBeginTag(HtmlTextWriterTag.Td); //Column 1  
                            writer.RenderBeginTag(HtmlTextWriterTag.Div);

                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "AresHide");
                            writer.AddAttribute("parent", this.index.ToString());
                            //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            writer.AddAttribute("onclick", "HideDocs(" + this.index + ")");
                            writer.RenderBeginTag(HtmlTextWriterTag.A); //Provider Name Link
                            writer.Write("Hide Ratings");
                            writer.RenderEndTag(); //End Provider name A link

                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "AresShow");
                            writer.AddAttribute("parent", this.index.ToString());
                            //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                            writer.AddAttribute("onclick", "ShowDocs(" + this.index + ")");
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                            writer.RenderBeginTag(HtmlTextWriterTag.A); //Provider Name Link
                            writer.Write("Click for Ratings");
                            writer.RenderEndTag(); //End Provider name A link

                            writer.RenderEndTag(); //End DIV
                            writer.RenderEndTag(); //End Column 1 TD
                        }
                        else
                        {
                            //Rating
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv ratingTD");
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "17%");
                            writer.RenderBeginTag(HtmlTextWriterTag.Td); //Column 1  
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ratingRow");
                            writer.RenderBeginTag(HtmlTextWriterTag.Div);
                            double rat = Convert.ToDouble(this.Providers[0].HGRating);
                            for (int i = 1; i <= (int)rat; i++)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "star_full");
                                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                                writer.RenderEndTag();
                            }
                            if ((rat % 1) > 0.0)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "star_half");
                                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                                writer.RenderEndTag();
                            }
                            if (rat == -1.0) { rat = 0.0; }
                            for (int i = 1; i <= (5 - (((int)rat) + ((rat % 1) > 0.0 ? 1 : 0))); i++)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "star_none");
                                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                                writer.RenderEndTag();
                            }
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ratings");
                            writer.RenderBeginTag(HtmlTextWriterTag.P);
                            writer.RenderBeginTag(HtmlTextWriterTag.Span);
                            writer.Write(string.Format("{0} patient survey{1}", this.Providers[0].HGRatingCount, (Convert.ToInt32(this.Providers[0].HGRatingCount) == 1 ? "" : "s")));
                            writer.RenderEndTag(); //End Span
                            writer.RenderEndTag(); //End P
                            writer.RenderEndTag(); //End DIV
                            writer.RenderEndTag(); //End Column 1 TD
                        }

                        writer.RenderEndTag(); //END TR
                    }
                    return stringWriter.ToString();
                }
            }
            public Result(int index)
            {
                this.index = index;
            }
        }
        private class Practice : results_specialty
        {
            public string Name;
            public string Lat;
            public string Lng;
            public string Address1;
            public string City;
            public string State;
            public string Zip;
            public string Distance;
            public string Nav;
        }
        private class Provider : results_specialty
        {
            private int PracIndex;
            public string Name;
            public string NPI;
            public string HGRating;
            public string HGRatingCount;
            public string RangeMin;
            public string RangeMax;
            public Boolean IsFairPrice;
            public Boolean IsHGRecognized;
            public string Nav;
            private string HTML
            {
                get
                {
                    StringWriter stringWriter = new StringWriter();
                    using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
                    {
                        //Table Row
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "docRow graydiv" + ((this.PracIndex % 2) == 0 ? "" : " roweven"));
                        writer.AddAttribute("parent", this.PracIndex.ToString());
                        //writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                        //Result Name and Location
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "tdfirst graydiv NameLoc");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "32%");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //Column 1
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "readmore");
                        //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void();");
                        writer.AddAttribute("onclick", "SelectResult($(this).attr('nav'));");
                        writer.AddAttribute("nav", this.Nav);
                        writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px;");
                        writer.RenderBeginTag(HtmlTextWriterTag.A); //Provider Name Link
                        writer.Write(this.Name);
                        writer.RenderEndTag(); //End Provider name A link

                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv Dist");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "10%");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //Distance TD
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, "dvDistance");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div); //Distance DIV                    
                        writer.RenderBeginTag(HtmlTextWriterTag.Span); //Distance Span
                        writer.Write("");
                        writer.RenderEndTag(); //End Distance Span
                        writer.RenderEndTag(); //End Distance Div
                        writer.RenderEndTag(); //End Distance TD

                        //Estimated Initial Office Visit Cost
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv EIOVC");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "12%");
                        if (fromFindADoc)
                            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //TotalCost TD
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "totalCostRow");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div); //TotalCost Div
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                        writer.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                        writer.Write(string.Format("{0:c0}", double.Parse(this.RangeMin)));
                        writer.RenderEndTag(); //End Range Min B
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "dascol");
                        writer.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                        writer.Write("-");
                        writer.RenderEndTag(); //End DashCol B
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                        writer.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                        writer.Write(string.Format("{0:c0}", double.Parse(this.RangeMax)));
                        writer.RenderEndTag(); //End Range Max B
                        writer.RenderEndTag(); //End TotalCost Div
                        writer.RenderEndTag(); //End TotalCost TD

                        //Clear Choice
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv FP");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "12%");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //BEGIN TD
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, (this.IsFairPrice ? "../Images/check_green.png" : "../Images/s.gif"));
                        writer.AddAttribute(HtmlTextWriterAttribute.Alt, "ClearChoice Doc?");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                        writer.RenderBeginTag(HtmlTextWriterTag.Img); //BEGIN IMAGE
                        writer.RenderEndTag(); //END IMAGE
                        writer.RenderEndTag(); //END TD                    

                        //Healthgrades Recognized Physician
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv HG");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "17%");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //BEGIN TD
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, (this.IsHGRecognized ? "../Images/check_purple.png" : "../Images/s.gif"));
                        writer.AddAttribute(HtmlTextWriterAttribute.Alt, "Health Grades Recognized?");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                        writer.RenderBeginTag(HtmlTextWriterTag.Img); //BEGIN IMAGE
                        writer.RenderEndTag(); //END IMAGE
                        writer.RenderEndTag(); //END TD

                        //Rating
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv ratingTD");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "17%");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //Column 1  
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "ratingRow");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        double rat = Convert.ToDouble(this.HGRating);
                        for (int i = 1; i <= (int)rat; i++)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "star_full");
                            writer.RenderBeginTag(HtmlTextWriterTag.Div);
                            writer.RenderEndTag();
                        }
                        if ((rat % 1) > 0.0)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "star_half");
                            writer.RenderBeginTag(HtmlTextWriterTag.Div);
                            writer.RenderEndTag();
                        }
                        if (rat == -1.0) { rat = 0.0; }
                        for (int i = 1; i <= (5 - (((int)rat) + ((rat % 1) > 0.0 ? 1 : 0))); i++)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "star_none");
                            writer.RenderBeginTag(HtmlTextWriterTag.Div);
                            writer.RenderEndTag();
                        }
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "ratings");
                        writer.RenderBeginTag(HtmlTextWriterTag.P);
                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        writer.Write(string.Format("{0} patient survey{1}", this.HGRatingCount, (Convert.ToInt32(this.HGRatingCount) == 1 ? "" : "s")));
                        writer.RenderEndTag(); //End Span
                        writer.RenderEndTag(); //End P
                        writer.RenderEndTag(); //End DIV
                        writer.RenderEndTag(); //End Column 1 TD

                        writer.RenderEndTag(); //END TR
                    }
                    return stringWriter.ToString();
                }
            }
            public Provider(int pracIndex)
            {
                this.PracIndex = pracIndex;
            }

            public String GetHTML()
            {
                return this.HTML;
            }
        }
        private class CallbackActionRequest : results_specialty
        {
            #region Private Variables
            private DataTable _results;

            //private Dictionary<String, OrderOptions> inCallbackSortOptions = new Dictionary<string, OrderOptions>
            //{
            //    {"NameLoc", OrderOptions.PracticeName},
            //    {"Dist", OrderOptions.Distance},
            //    {"TC", OrderOptions.TotalCost},
            //    {"EC", OrderOptions.YourCost}
            //};
            //private Dictionary<String, OrderDirections> inCallbackSortDirection = new Dictionary<string, OrderDirections>
            //{
            //    {"ASC", OrderDirections.Ascending},
            //    {"DESC", OrderDirections.Descending}
            //};
            #endregion

            #region Properties
            public DataTable ResultSet { set { this._results = value; } }
            public int ResultCount { get { return this._results.Rows.Count; } }
            #endregion

            public void SortResults()
            {
                DataView dv = new DataView(this._results);
                dv.Sort = "NumericDistance";
                this._results = dv.ToTable();
            }
            public ResultsInfo RetrieveResultsInfo()
            {
                string[] pracCols = { "PracticeName", "Latitude", "Longitude", "LocationAddress1", "LocationCity", "LocationState", "LocationZip", "Distance" };
                string[] provCols = { "ProviderName", "NPI", "RangeMin", "RangeMax", "FairPrice", "HGRecognized", "HGOverallRating", "HGPatientCount", "PracticeName" };

                ResultsInfo ri = new ResultsInfo();
                ri.ResultCount = this._results.Rows.Count;
                ri.EndOfResults = true; //We aren't buffering these results so for now just load the whole set and don't try again.

                List<Result> lResult = new List<Result>();
                using (DataView dv = new DataView(this._results))
                {
                    string[] UniquePractCol = { "PracticeName" };
                    using (DataTable uniquePractices = dv.ToTable("ByPractice", true, UniquePractCol))
                    {
                        foreach (DataRow dr in uniquePractices.Rows)
                        {
                            Result r = new Result(lResult.Count + 1);
                            using (DataView byPractice = new DataView(dv.ToTable("PracticeInfo", true, pracCols)))
                            {
                                byPractice.RowFilter = "PracticeName = '" + dr[0].ToString().Replace("'", "''") + "'";
                                Practice pr = new Practice();
                                pr.Name = byPractice[0].Row[pracCols[0].ToString()].ToString();
                                pr.Lat = byPractice[0].Row[pracCols[1].ToString()].ToString();
                                pr.Lng = byPractice[0].Row[pracCols[2].ToString()].ToString();
                                pr.Address1 = byPractice[0].Row[pracCols[3].ToString()].ToString();
                                pr.City = byPractice[0].Row[pracCols[4].ToString()].ToString();
                                pr.State = byPractice[0].Row[pracCols[5].ToString()].ToString();
                                pr.Zip = byPractice[0].Row[pracCols[6].ToString()].ToString();
                                pr.Distance = byPractice[0].Row[pracCols[7].ToString()].ToString();

                                FormsAuthenticationTicket tk = new FormsAuthenticationTicket(string.Format("{0}|{1}|{2}|{3}"
                                    , pr.Name
                                    , ""
                                    , ""
                                    , pr.Distance)
                                    , false, 5);
                                pr.Nav = FormsAuthentication.Encrypt(tk);

                                r.Practice = pr;
                            }
                            List<Provider> provsAtPrac = new List<Provider>();
                            using (DataView byProvider = new DataView(dv.ToTable("ProviderInfo", false, provCols)))
                            {
                                byProvider.RowFilter = "PracticeName = '" + dr[0].ToString().Replace("'", "''") + "'";
                                for (int i = 0; i < byProvider.Count; i++)
                                {
                                    Provider pr = new Provider(lResult.Count + 1);
                                    pr.Name = byProvider[i].Row[provCols[0].ToString()].ToString();
                                    pr.NPI = byProvider[i].Row[provCols[1].ToString()].ToString();
                                    pr.RangeMin = byProvider[i].Row[provCols[2].ToString()].ToString();
                                    pr.RangeMax = byProvider[i].Row[provCols[3].ToString()].ToString();
                                    pr.IsFairPrice = Boolean.Parse(byProvider[i].Row[provCols[4].ToString()].ToString());
                                    pr.IsHGRecognized = Boolean.Parse(byProvider[i].Row[provCols[5].ToString()].ToString());
                                    pr.HGRating = byProvider[i].Row[provCols[6].ToString()].ToString();
                                    pr.HGRatingCount = byProvider[i].Row[provCols[7].ToString()].ToString();

                                    FormsAuthenticationTicket tk = new FormsAuthenticationTicket(string.Format("{0}|{1}|{2}|{3}"
                                    , r.Practice.Name
                                    , pr.Name
                                    , pr.NPI
                                    , r.Practice.Distance)
                                    , false, 5);
                                    pr.Nav = FormsAuthentication.Encrypt(tk);

                                    provsAtPrac.Add(pr);
                                }
                            }
                            r.Providers = provsAtPrac.ToArray<Provider>();
                            lResult.Add(r);
                        }
                    }
                }
                ri.Results = lResult.ToArray<Result>();

                return ri;
            }
        }
    }
}