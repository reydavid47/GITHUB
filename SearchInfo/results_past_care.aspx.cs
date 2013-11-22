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

namespace ClearCostWeb.SearchInfo
{
    public partial class results_past_care : System.Web.UI.Page
    {
        private const String IMAGE_ASC = "~/Images/icon_arrow_down.gif";
        private const String IMAGE_DESC = "~/Images/icon_arrow_up.gif";

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "RigJava", "rigJava();", true);

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowingYCTest",
                "var showingYC = " + (ThisSession.DefaultYourCostOn ? "true;" : "false;"),
                true);

            if (!Page.IsPostBack)
            {
                hfdSortDirection.Value = "DESC";  //  lam, 20130625, MSB-168
                hfdCurrentSort.Value = "AllowedAmount";  //  lam, 20130625, MSB-168

                lblServiceName.Text = ThisSession.ServiceName;
                // Additional text for those scenarios where the user entered or selected different spellings of the service found.
                if (ThisSession.ServiceEnteredFrom == "Letters")
                {
                    // For now, this will probably just be fine - new Lipid Panel name seems to cover this.
                    //      Will want to expand on it sometime.
                    // not a misspelling, just another alias.
                    //lblServiceVerification.Text = "(" + ThisSession.ServiceEntered + ")";
                    //lblServiceVerification.Visible = true;
                }
                else
                {
                    if (ThisSession.ServiceEnteredFrom == "Entry")
                    {
                        string quote = @"""";
                        // Entered by user, could be misspelling
                        lblServiceVerification.Text = "(You entered: " + quote + ThisSession.ServiceEntered + quote + ". Click <b><a href='search.aspx'>here</a></b> if you'd like to search for different service.)";
                        lblServiceVerification.Visible = true;
                    }
                    else
                    { lblServiceVerification.Visible = false; }
                }


                //If we're looking at past care, we'll have a Past Care query string. Populate Facility text fields.
                if (ThisSession.PastCareFacilityName != string.Empty)
                {
                    lblFacilityTitle.Visible = true;
                    lblFacilityName.Text = ThisSession.PastCareFacilityName;
                    lblFacilityName.Visible = true;

                    lblFacilityNameDetails.Text = ThisSession.PastCareFacilityName;
                    lblFacilityNameDetails.Visible = true;
                    pnlPastCareDetails.Visible = true;
                }
                else
                {
                    lblFacilityTitle.Visible = false;
                    lblFacilityName.Visible = false;
                    pnlPastCareDetails.Visible = false;
                }
                GetFacilities(); // Load grid later

                lblAllResult1DisclaimerText.Text = ThisSession.AllResult1DisclaimerText;  //  lam, 20130425, MSF-295 move disclaimer text to content manager
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        //protected void HandleGeoCode(object sender, GeoCodeEventArgs e)
        //{
        //    GetFacilities();
        //}
        protected void GetFacilities()
        {
            DataTable FacilityList = SearchForFacilities();

            //TODO: For demo, just get first 25 if there are more than that.
            int rowCount = FacilityList.Rows.Count;
            if (rowCount > 25) { rowCount = 25; }

            if (rowCount > 0)
            {
                DataColumn googleColumn = new DataColumn("GoogleDistance");
                FacilityList.Columns.Add(googleColumn);

                //Modified to render prior to the page rendering
                string originString = string.Format("{0},{1}", ThisSession.PatientLatitude, ThisSession.PatientLongitude);
                List<string> destinations = new List<string>();
                for (int i = 0; i < FacilityList.Rows.Count; i++)
                {
                    destinations.Add(FacilityList.Rows[i]["LatLong"].ToString());
                }

                //string[] distances = GoogleHelper.GetDistances(originString, destinations.ToArray());

                for (int i = 0; i < FacilityList.Rows.Count; i++)
                {
                    //if (distances.Length > i)
                    //{
                    //    FacilityList.Rows[i]["Distance"] = distances[i];
                    //    FacilityList.Rows[i]["NumericDistance"] = Convert.ToDouble(distances[i].Replace(" mi", string.Empty).Trim());
                    //}
                    //else
                    //{
                    FacilityList.Rows[i]["Distance"] = string.Format("{0:##0.0}", FacilityList.Rows[i]["NumericDistance"]) + " mi";
                    //}
                }

                LoadFacilityGrid(FacilityList);
            }
            //ThisSession.FacilityList = FacilityList;

            //string JScript = GenerateGoogleRequest(FacilityList);
            //ClientScriptManager CSManager = Page.ClientScript;
            //CSManager.RegisterStartupScript(Page.GetType(), "GetDistances", JScript, true);

            //rptResults.DataSource = FacilityList;
            //rptResults.DataBind();
        }
        protected void LoadFacilityGrid(DataTable FacilityList)
        {
            //DataTable FacilityList = SearchForFacilities();
            DataView dv = new DataView(FacilityList);
            Boolean isShowYC = (hfdShowYC.Value.Trim() != "" && hfdShowYC.Value.ToLower() == "true");  //  lam, 20130717, MSF-443
            String SortDirection = "";

            if (rblSort.SelectedIndex > -1)  //  lam, 20130625, MSB-168
            {
                if (rblSort.SelectedValue != hfdCurrentSort.Value)
                {
                    hfdCurrentSort.Value = rblSort.SelectedValue;
                    SortDirection = hfdSortDirection.Value = "ASC";
                }
                else
                {
                    SortDirection = hfdSortDirection.Value = (hfdSortDirection.Value == "ASC" ? "DESC" : "ASC");
                }

                if (rblSort.SelectedValue == "PracticeName")
                {
                    dv.Sort = "PracticeName " + SortDirection + ", NumericDistance ASC, AllowedAmount ASC, FairPrice DESC";
                }
                else
                {
                    if (rblSort.SelectedValue == "YourCost")
                    {
                        dv.Sort = "YourCost " + SortDirection + ", NumericDistance ASC, FairPrice DESC, PracticeName ASC";
                    }
                    else
                    {
                        if (rblSort.SelectedValue == "NumericDistance")
                        {
                            dv.Sort = "NumericDistance " + SortDirection + ", AllowedAmount ASC, FairPrice DESC, PracticeName ASC";
                        }
                        else
                        {
                            //  default sort, "AllowedAmount" (TotalCost)
                            dv.Sort = "AllowedAmount " + SortDirection + ", NumericDistance ASC, FairPrice DESC, PracticeName ASC";
                        }
                    }
                }
            }
            /*
            if (rblSort.SelectedIndex > -1)
            {
                if (rblSort.SelectedValue == "AllowedAmount" || rblSort.SelectedValue == "YourCost")
                {
                    //LB 9/20/11 - Not sure of the syntax here - test.
                    dv.Sort = rblSort.SelectedValue + ", NumericDistance";
                }
                else
                {
                    dv.Sort = rblSort.SelectedValue;
                }
                isShowYC = (hfdShowYC.Value.ToString().Equals("true") || rblSort.SelectedValue == "YourCost");
                //isShowYC = (rblSort.Items[3].Attributes.CssStyle.Value.ToLower() != "display:none;") || (rblSort.SelectedValue == "YourCost");  //  lam, 20130321, MSF-274
            }
            else
            {
                // sort by amount then distance for this past care results page.
                dv.Sort = "AllowedAmount, NumericDistance";
            }
            */

            rptResults.DataSource = dv;
            rptResults.DataBind();

            ThisSession.FacilityList = dv.ToTable(); //store the sorted dataview in session to keep the appropriate indexing later

            //UpdatePanel1.Update(); // not working?!

            //  lam, 20130321, MSF-274, make sure showYC is initialized accordingly
            string nJ = (isShowYC ? " showingYC = true;" : " showingYC = false;") + "if(showingYC){$(\".YC\").toggle();$(\"td.PHARM\").toggleClass(\"expanded\");$(\"td.DIST\").toggleClass(\"expanded\");$(\"td.PRICE\").toggleClass(\"expanded\");}";
            //string nJ = "if(showingYC){$(\".YC\").toggle();$(\"td.PHARM\").toggleClass(\"expanded\");$(\"td.DIST\").toggleClass(\"expanded\");$(\"td.PRICE\").toggleClass(\"expanded\");}";
            //  -------------------------------------------------------------------
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowYC", nJ, true);

            //  lam, 20130321, MSF-274, new script segment
            nJ = "function toggleYC(setOn) {showingYC=setOn;" + hfdShowYC.ClientID + ".value=setOn;if(!setOn)changeSelection();}";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "toggleYC", nJ, true);
            //  ------------------------------------------
        }
        protected DataTable SearchForFacilities()
        {
            DataTable FacilityList = new DataTable();

            //Which employer database?
            SqlConnection conn = new SqlConnection(ThisSession.CnxString);
            SqlCommand sqlCmd;
            sqlCmd = new SqlCommand("GetFacilitiesForServicePastCare", conn);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            SqlParameter userParm = new SqlParameter("@ServiceID", SqlDbType.Int);
            userParm.Value = ThisSession.ServiceID;
            sqlCmd.Parameters.Add(userParm);
            userParm = new SqlParameter("@ProcedureCode", SqlDbType.VarChar, 5);
            userParm.Value = ThisSession.PastCareProcedureCode;
            sqlCmd.Parameters.Add(userParm);
            userParm = new SqlParameter("@CCHID", SqlDbType.NVarChar, 50);
            userParm.Value = ThisSession.CCHID;
            sqlCmd.Parameters.Add(userParm);
            userParm = new SqlParameter("@UserID", SqlDbType.NVarChar, 36);
            userParm.Value = Membership.GetUser().ProviderUserKey.ToString();

            //TODO: How will professional component play in to imaging etc.
            userParm = new SqlParameter("@ProfessionalComponent", SqlDbType.Money);
            if (ThisSession.ServiceName.ToLower().StartsWith("mri"))
                userParm.Value = 150.69;
            else
                userParm.Value = 0;
            sqlCmd.Parameters.Add(userParm);

            userParm = new SqlParameter("@Latitude", SqlDbType.Float);
            userParm.Value = ThisSession.PatientLatitude;
            sqlCmd.Parameters.Add(userParm);
            userParm = new SqlParameter("@Longitude", SqlDbType.Float);
            userParm.Value = ThisSession.PatientLongitude;
            sqlCmd.Parameters.Add(userParm);
            userParm = new SqlParameter("@SpecialtyID", SqlDbType.Int);
            userParm.Value = ThisSession.SpecialtyID;
            sqlCmd.Parameters.Add(userParm);
            userParm = new SqlParameter("@PastCareID", SqlDbType.Int);
            userParm.Value = ThisSession.PastCareID;
            sqlCmd.Parameters.Add(userParm);
            userParm = new SqlParameter("@SessionID", SqlDbType.NVarChar, 36);
            userParm.Value = HttpContext.Current.Session.SessionID;
            sqlCmd.Parameters.Add(userParm);
            userParm = new SqlParameter("@Domain", SqlDbType.NVarChar, 30);
            userParm.Value = Request.Url.Host;
            sqlCmd.Parameters.Add(userParm);
            userParm = new SqlParameter("@Distance", SqlDbType.Int);
            userParm.Value = this.distance;
            sqlCmd.Parameters.Add(userParm);

            SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                adp.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    FacilityList = ds.Tables[0];

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
                    }
                    //JM 12/3/12
                    // Separated this from the previous IF statement as we ran into an issue on 11/30/12 with this piece having information but being skipped due to no results
                    //  being sent in the "Learn More" table.
                    if (ds.Tables.Count > 2) // did we get specialty info?
                    {
                        //Specialty Title
                        if (ds.Tables[2].Rows[0]["Title"].ToString() != String.Empty)
                        {
                            lblServiceName.Text = ds.Tables[2].Rows[0]["Title"].ToString() + ": " + ThisSession.ServiceName;
                        }
                        if (ds.Tables.Count > 3) // Did we get savings info
                        {
                            lblAllowedAmount.Text = string.Format("{0:c0}", decimal.Parse(ds.Tables[3].Rows[0]["AllowedAmount"].ToString()));
                            lblYouCouldHaveSaved.Text = string.Format("{0:c0}", decimal.Parse(ds.Tables[3].Rows[0]["YouCouldHaveSaved"].ToString()));
                            lblDifference.Text = string.Format("{0:c0}",
                                (decimal.Parse(ds.Tables[3].Rows[0]["AllowedAmount"].ToString()) - decimal.Parse(ds.Tables[3].Rows[0]["YouCouldHaveSaved"].ToString())));
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

            return FacilityList;
        }
        protected void rptResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                // set the visibility of the down arrow on the headers to show which it's sorted by.
                Image imgFacilitySort = (Image)e.Item.FindControl("imgFacilitySort");
                Image imgDistanceSort = (Image)e.Item.FindControl("imgDistanceSort");
                Image imgEstimatedCostSort = (Image)e.Item.FindControl("imgEstimatedCostSort");
                Image imgYourCostSort = (Image)e.Item.FindControl("imgYourCostSort");

                // default to not visible
                imgFacilitySort.Visible = false;
                imgDistanceSort.Visible = false;
                imgEstimatedCostSort.Visible = false;
                imgYourCostSort.Visible = false;

                switch (rblSort.SelectedIndex)
                {
                    case -1:
                        imgDistanceSort.Visible = true;
                        imgDistanceSort.ImageUrl = (hfdSortDirection.Value == "ASC" ? IMAGE_ASC : IMAGE_DESC);
                        break;
                    case 0:
                        imgFacilitySort.Visible = true;
                        imgFacilitySort.ImageUrl = (hfdSortDirection.Value == "ASC" ? IMAGE_ASC : IMAGE_DESC);
                        break;
                    case 1:
                        imgDistanceSort.Visible = true;
                        imgDistanceSort.ImageUrl = (hfdSortDirection.Value == "ASC" ? IMAGE_ASC : IMAGE_DESC);
                        break;
                    case 2:
                        imgEstimatedCostSort.Visible = true;
                        imgEstimatedCostSort.ImageUrl = (hfdSortDirection.Value == "ASC" ? IMAGE_ASC : IMAGE_DESC);
                        break;
                    case 3:
                        imgYourCostSort.Visible = true;
                        imgYourCostSort.ImageUrl = (hfdSortDirection.Value == "ASC" ? IMAGE_ASC : IMAGE_DESC);
                        break;
                }

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
                Label lblHGCount = (Label)e.Item.FindControl("lblHGCount");
                if (drv["FindAService"].ToString() == "True")
                {
                    imgHGRecognizedFalse.Visible = false; imgHGRecognizedTrue.Visible = false;
                    if (Convert.ToInt32(drv["HGDocCount"].ToString()) == 0)
                    {
                        lblHGCount.Text = "N/A";
                    }
                    else
                    {
                        lblHGCount.Text = String.Format("{0}/{1} Physicians",
                                drv["HGRecognizedDocCount"],
                                drv["HGDocCount"]);
                    }
                    lblHGCount.Visible = true;
                }
                else
                {
                    if (Convert.ToInt32(drv["HGRecognized"].ToString()) == 1)
                    { imgHGRecognizedTrue.Visible = true; imgHGRecognizedFalse.Visible = false; lblHGCount.Visible = false; }
                    else if (Convert.ToInt32(drv["HGRecognized"].ToString()) == 0)
                    { imgHGRecognizedTrue.Visible = false; imgHGRecognizedFalse.Visible = true; lblHGCount.Visible = false; }
                    else //if (Convert.ToInt32(drv["HGRecognized"].ToString()) == -1)
                    {
                        imgHGRecognizedFalse.Visible = false; imgHGRecognizedTrue.Visible = false;
                        lblHGCount.Text = "N/A";
                        lblHGCount.Visible = true;
                    }
                }

                //Format amount with commas
                Label lblAllowedAmount = (Label)e.Item.FindControl("lblAllowedAmount");
                Image imgLearnMore = (Image)e.Item.FindControl("imgLearnMore");  //  lam, 20130530, MSB-299
                if (drv["AntiTransparency"].ToString().ToLower() == "true")  //  lam, 20130530, MSB-299
                {
                    lblAllowedAmount.Text = "Undisclosed";
                    imgLearnMore.Visible = true;
                }
                else
                {
                    lblAllowedAmount.Text = string.Format("${0:#,##0}", int.Parse(drv["AllowedAmount"].ToString()));
                    imgLearnMore.Visible = false;
                }

                Label lblYourCost = (Label)e.Item.FindControl("lblYourCost");
                lblYourCost.Text = string.Format("${0:#,##0}", int.Parse(drv["YourCost"].ToString()));

                //Add command arg to Practice Name
                LinkButton lbtnSelectFacility = (LinkButton)e.Item.FindControl("lbtnSelectFacility");
                lbtnSelectFacility.CommandName = "Select";
                //NOTE: not sure how this will work out in long run. Put both in here for now.
                lbtnSelectFacility.CommandArgument = drv["PracticeName"].ToString() + "|" + drv["NPI"].ToString() + "|" + drv["OrganizationLocationID"].ToString();

            }
        }
        protected void rblSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFacilityGrid(ThisSession.FacilityList);
        }
        protected void FacilitySort(object sender, EventArgs e)
        {
            rblSort.SelectedValue = "PracticeName";

            //Image img = (Image)((LinkButton)sender).Parent.FindControl("imgFacilitySort");
            
            LoadFacilityGrid(ThisSession.FacilityList);
        }
        protected void DistanceSort(object sender, EventArgs e)
        {
            rblSort.SelectedValue = "NumericDistance";

            LoadFacilityGrid(ThisSession.FacilityList);
        }
        protected void EstimatedCostSort(object sender, EventArgs e)
        {
            rblSort.SelectedValue = "AllowedAmount";

            LoadFacilityGrid(ThisSession.FacilityList);
        }
        protected void YourCostSort(object sender, EventArgs e)
        {
            rblSort.SelectedValue = "YourCost";

            LoadFacilityGrid(ThisSession.FacilityList);
        }
        protected void SelectFacility(object sender, EventArgs e)
        {
            //Whic facility line were they on?
            LinkButton lbtnSelectFacility = (LinkButton)sender;
            string[] args = lbtnSelectFacility.CommandArgument.Split('|');

            // Put facility key information in Session
            ThisSession.PracticeName = args[0];
            ThisSession.PracticeNPI = args[1];
            ThisSession.OrganizationLocationID = int.Parse(args[2]);

            foreach (DataRow dr in ThisSession.FacilityList.Rows)
            {
                if (dr["PracticeName"].ToString() == ThisSession.PracticeName &&
                    dr["NPI"].ToString() == ThisSession.PracticeNPI)
                { ThisSession.FacilityDistance = dr["Distance"].ToString(); }
            }
            if (ThisSession.FacilityDistance.EndsWith(" mi"))
            { ThisSession.FacilityDistance = ThisSession.FacilityDistance.Substring(0, ThisSession.FacilityDistance.Length - 3); }


            // Go to the Care Detail page for the selected Facility.
            Response.Redirect("results_care_detail.aspx");
        }
        protected void ibtnPrintResults_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("print.aspx");

        }

        protected int distance = 25;
        protected void updateDistance(object sender, EventArgs e)
        {
            distance = sFindPastCare.Value;
            lblSliderValue.Text = String.Concat(" ", sFindPastCare.Value, " mile(s)");
        }
        // Used to set class on first row of repeater. Not called on Alt items.
        protected string GetItemClass(int itemIndex, object practiceName)
        {
            string PracticeName = practiceName.ToString();
            string setClass = string.Empty;
            if (PracticeName.ToLower() == ThisSession.PastCareFacilityName.ToLower())
                setClass = "rowbold ";

            if (itemIndex == 0)
            {
                return setClass + "roweven graydiv graytop";
            }
            else
            {
                if ((itemIndex % 2) == 0)
                    return setClass + "roweven graydiv";
                else
                    return setClass + "graydiv";
            }
        }


        [ScriptMethod, WebMethod]
        public static string GetPatientCenter()
        {
            string retStr = new JavaScriptSerializer().Serialize(string.Format("{0},{1}", ThisSession.PatientLatitude, ThisSession.PatientLongitude));
            return retStr;
        }
        [ScriptMethod, WebMethod]
        public static string GetMarker(int index)
        {
            string retJava = "";
            string infoHtml = "";
            char quote = '"';

            DataTable dt = ThisSession.FacilityList;

            string facLat = dt.Rows[index]["Latitude"].ToString();
            string facLng = dt.Rows[index]["Longitude"].ToString();
            string pracName = dt.Rows[index]["PracticeName"].ToString().Replace("'", string.Empty);
            string fp = dt.Rows[index]["FairPrice"].ToString();
            string min = dt.Rows[index]["AllowedAmount"].ToString();
            string hg = dt.Rows[index]["HGRecognized"].ToString();

            infoHtml += string.Format("<p class={0}smaller{0}><a href={0}#{0} class={0}readmore{0}>{1}</a><br/>Total Estimated cost: <b> ${2} </b>", quote, pracName, min, index);
            if (Convert.ToBoolean((fp == string.Empty ? "false" : fp)))
            {
                infoHtml += string.Format("<br /><img src={0}../Images/check_green.png{0} alt={0}X{0} class={0}checkmark{0} width={0}23{0} height={0}23{0} border={0}{0} />&nbsp;Fair Price", quote);
            }
            if (Convert.ToBoolean((hg == string.Empty ? "false" : (hg == "1" ? "true" : "false"))))
            {
                infoHtml += string.Format("<br /><img src={0}../Images/check_purple.png{0} alt={0}X{0} class={0}checkmark{0} width={0}23{0} height={0}23{0} border={0}{0} />&nbsp;Healthgrades Recognized Provider", quote);
            }
            infoHtml += "</p>";

            retJava += string.Format("var point = new google.maps.LatLng({0},{1});\r\n", facLat.ToString(), facLng.ToString());
            retJava += "var marker = createMarker(";
            if (Convert.ToBoolean(fp))
            {
                retJava += "icon_FP,";
            }
            else
            {
                retJava += "icon,";
            }
            retJava += string.Format("map,infoWindow,point,'{0}',{2}{1}{2});", infoHtml, string.Format("{0:00}", index + 1), quote);

            return retJava;
        }
        [ScriptMethod, WebMethod]
        public static string GetMarkerCount()
        {
            DataTable dt = ThisSession.FacilityList;
            Int32 rows = dt.Rows.Count;
            return rows.ToString();
        }
    }
}