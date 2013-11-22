using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;

namespace ClearCostWeb.SearchInfo
{
    public partial class results_rx : System.Web.UI.Page
    {
        #region Private Variables
        private SqlParameter prmDrugID = new SqlParameter("DrugID", SqlDbType.Int);
        private SqlParameter prmLat = new SqlParameter("Latitude", SqlDbType.Float);
        private SqlParameter prmLng = new SqlParameter("Longitude", SqlDbType.Float);
        private SqlParameter prmDistance = new SqlParameter("Distance", SqlDbType.Int); //Defaults to 20
        private SqlParameter prmGPI = new SqlParameter("GPI", SqlDbType.NVarChar, 50);
        private SqlParameter prmQuantity = new SqlParameter("Quantity", SqlDbType.Decimal);
        private SqlParameter prmPastCareID = new SqlParameter("PastCareID", SqlDbType.Int);
        private SqlParameter prmDrugList = new SqlParameter("DrugList", SqlDbType.Structured);
        private SqlParameter prmMemberRXID = new SqlParameter("MemberRXID", SqlDbType.NVarChar, 50);
        private SqlParameter prmUserID = new SqlParameter("UserID", SqlDbType.NVarChar, 36);

        private enum PharmacyProcs { GetDrugPricingResults, GetDrugMultiPricingResults }
        private static Dictionary<PharmacyProcs, String> AvailablePharmProcs = new Dictionary<PharmacyProcs, String>{
        { PharmacyProcs.GetDrugPricingResults, "GetDrugPricingResults" },
        { PharmacyProcs.GetDrugMultiPricingResults, "GetDrugMultiPricingResults" }
    };
        private String ReturnHTML
        {
            get
            {
                String tOut = "";
                switch (ThisSession.PrevPage)
                {
                    case "FindRx":
                        tOut = "<a href=\"Search.aspx\" class=\"back\">Return To Search</a>";
                        break;
                    case "FindPastCare":
                        tOut = "<a href=\"Search.aspx\" class=\"back\">Return To Past Care</a>";
                        break;
                    case "Results_Rx_Name":
                        tOut = "<a href=\"Results_Rx_Name.aspx\" class=\"back\">Return To Dose and Quantity</a>";
                        break;
                    default:
                        tOut = "<a href=\"Search.aspx\" class=\"back\">Return To Search</a>";
                        break;
                }
                return tOut;
            }
        }
        #endregion

        #region Public Properties
        //Used to determine if "Add this drug to Med List" is visible depending on if we got here from past care
        protected String IsHidden { get { return (ChosenDrugsTable == null ? ((ThisSession.PastCareID == "") ? "" : " hidden") : " hidden"); } }
        /*  lam, 20130430, MSF-294
        protected String RxDisclaimerForAvayaCaesars
        {
            get
            {
                if (ThisSession.EmployerID == 9.ToString() || ThisSession.EmployerID == 11.ToString())
                    return @"<i class=""smaller"">Note: After two fills at a retail pharmacy, prescriptions obtained from pharmacies other than mail order will not apply to your deductible.  Select “Your Estimated Cost” to see your out-of-pocket cost for each option.</i>";
                else
                    return "";
                
            }
        }
        */
        /*  lam, 20130430, MSF-294
        protected String MailOrderDisclaimerForSanminaStarbucks
        {
            get
            {
                if (ThisSession.EmployerID == 7.ToString() || ThisSession.EmployerID == 8.ToString() || ThisSession.EmployerID == 10.ToString())
                    return @"<i class=""smaller"">Note: for fills greater than 30 days, prescriptions obtained from pharmacies other than mail order will not apply to your deductible. See ""Your Estimated Cost"" to see your out-of-pocket cost for each option.</i>";
                else
                    return "";

            }
        }
        */ 
        private DataTable ChosenDrugsTable { get { return (DataTable)ViewState["ChosenDrugs"]; } set { ViewState["ChosenDrugs"] = (DataTable)value; } }
        private Boolean SingleDrugResults { get { return (ChosenDrugsTable == null ? true : (ChosenDrugsTable.Rows.Count == 1 ? true : false)); } }

        private String PostBackControl { get { return Request.Params["__EVENTTARGET"].ToString(); } }
        private String PostBackArgument { get { return Request.Params["__EVENTARGUMENT"].ToString(); } }
        private String[] PostBackLatLng { get { return (PostBackControl == "Geocoder" ? PostBackArgument.Split('|') : null); } }
        #endregion

        #region GUI Methods
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            JavaScriptHelper.IncludeFindRx(Page.ClientScript);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ltlReturn.Text = this.ReturnHTML;

            prmQuantity.Precision = 18;
            prmQuantity.Scale = 2;

            if (ThisSession.DefaultYourCostOn)
            {
                Page.Header.Controls.Add(new LiteralControl("<script type=\"text/javascript\">var YourCostDefault = true;</script>"));
            }
            else
            {
                Page.Header.Controls.Add(new LiteralControl("<script type=\"text/javascript\">var YourCostDefault = false;</script>"));
            }

            if (ThisSession.DefaultSort != "Distance")
                Page.Header.Controls.Add(new LiteralControl("<script type=\"text/javascript\">var globDefSort = \"" + ThisSession.DefaultSort + "\";</script>"));

            if (!IsPostBack)
            {
                //lblDrugName.Text = ThisSession.DrugName;
                //lblDrugDose.Text = ThisSession.DrugStrength;
                //lblDrugQuantity.Text = string.Format("{0:f0} {1}", Convert.ToDecimal(ThisSession.DrugQuantity), ThisSession.DrugQuantityUOM);

                SetupHeaders();
                SetupFamilyMembers();
                SetupGrid(ThisSession.PatientLatitude, ThisSession.PatientLongitude);

                lblRxResultDisclaimerText.Text = ThisSession.RxResultDisclaimerText;  //  lam, 20130418, MSF-294
                lblSpecialtyDrugDisclaimer.Text = ThisSession.SpecialtyDrugDisclaimerText;  //  lam, 20130429, CI-59
            }
            else if (PostBackControl == "Result")
            {
                String[] pharmArgs = PostBackArgument.Split('|');
                ThisSession.PharmacyID = pharmArgs[0];
                ThisSession.PharmacyLocationID = pharmArgs[1];
                Response.Redirect("pharmacy_detail.aspx");
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void SaveMed(object sender, EventArgs e)
        {
            String[] lbArgs = ddlMembers.SelectedItem.Value.Split('|');
            List<SqlParameter> inpParms = new List<SqlParameter>();

            SqlParameter prm = new SqlParameter("EmployeeID", SqlDbType.NVarChar, 50);
            prm.Value = lbArgs[0];
            inpParms.Add(prm);

            prm = new SqlParameter("CCHID", SqlDbType.Int);
            prm.Value = lbArgs[1];
            inpParms.Add(prm);

            prm = new SqlParameter("GPI", SqlDbType.NVarChar, 20);
            prm.Value = ThisSession.DrugGPI;
            inpParms.Add(prm);

            prm = new SqlParameter("Quantity", SqlDbType.Decimal);
            prm.Precision = 18;
            prm.Scale = 2;
            prm.Value = ThisSession.DrugQuantity;
            inpParms.Add(prm);

            prm = new SqlParameter("Email", SqlDbType.NVarChar, 100);
            prm.Value = lbArgs[2];
            inpParms.Add(prm);

            AddMedToMember(inpParms, ThisSession.CnxString);
        }
        protected void DoSort(object sender, EventArgs e)
        {

        }
        //protected void NavNew(object sender, EventArgs e)
        //{
        //    ThisSession.DrugEntered = string.Empty;
        //    ThisSession.DrugEnteredFrom = string.Empty;
        //    ThisSession.DrugGPI = string.Empty;
        //    ThisSession.DrugID = string.Empty;
        //    ThisSession.DrugName = string.Empty;
        //    ThisSession.DrugQuantity = string.Empty;
        //    ThisSession.DrugStrength = string.Empty;
        //    ThisSession.PastCareID = string.Empty;
        //    ThisSession.DrugQuantityUOM = string.Empty;
        //    Response.Redirect("Search.aspx#tabrx");
        //}
        protected void SelectPharmacy(object sender, EventArgs e)
        {
            //Whic facility line were they on?
            LinkButton lbtnSelectPharmacy = (LinkButton)sender;

            String[] args = lbtnSelectPharmacy.CommandArgument.Split('|');
            ThisSession.PharmacyID = args[0];
            ThisSession.PharmacyLocationID = args[1];

            // Go to the Care Detail page for the selected Facility.
            Response.Redirect("pharmacy_detail.aspx");
        }
        protected void rptResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                ((LinkButton)e.Item.FindControl("lbtnSelectPharmacy")).Text = String.Format("{0}{1}{2}",
                    drv.Row["PharmacyName"].ToString(),
                    ((drv.Row["CurrentPharmText"].ToString() != "") ? String.Format("&nbsp;<b>{0}</b>", drv.Row["CurrentPharmText"].ToString()) : string.Empty),
                    ((drv.Row["BestPriceText"].ToString() != "") ? String.Format("&nbsp;<b>{0}</b>", drv.Row["BestPriceText"].ToString()) : string.Empty)
                );

                ((LinkButton)e.Item.FindControl("lbtnSelectPharmacy")).CommandArgument = String.Format("{0}|{1}",
                    drv.Row["PharmacyID"].ToString(), drv.Row["PharmacyLocationID"].ToString());
                ((Label)e.Item.FindControl("lblLocation")).Text = drv.Row["City"].ToString();
                ((Label)e.Item.FindControl("lblDistance")).Text = string.Format("{0:f1} mi", Convert.ToDouble(drv.Row["Distance"]));
                ((Label)e.Item.FindControl("lblDistance")).Attributes.Add("Lat", drv.Row["Latitude"].ToString());
                ((Label)e.Item.FindControl("lblDistance")).Attributes.Add("Lng", drv.Row["Longitude"].ToString());
                ((Label)e.Item.FindControl("lblEstimatedCost")).Text = string.Format("${0:f2}", Convert.ToDouble(drv.Row[(this.SingleDrugResults ? "Price" : "TotalCost")]));

            }

        }
        protected String IsTopRow(int itemIndex) { return ((itemIndex == 0) ? " graytop" : ""); }
        protected int distance = 25;
        protected void updateDistance(object sender, EventArgs e)
        {
            distance = sFindRx.Value;
            lblSliderValue.Text = String.Concat(" ", sFindRx.Value, " mile(s)");
        }
        #endregion

        #region Helper Methods
        private void SetupHeaders()
        {
            this.ChosenDrugsTable = ThisSession.ChosenDrugs;
            DataView dv = new DataView(this.ChosenDrugsTable);
            if (this.SingleDrugResults)
            {
                if (this.ChosenDrugsTable == null)
                {
                    pnlSingleDrug.Visible = true;
                    pnlMultiDrug.Visible = false;

                    lblSingleDrugName.Text = ThisSession.DrugName;
                    ltlSingleDrugDose.Text = (ThisSession.DrugStrength.Trim() == String.Empty ? String.Empty : String.Format("&nbsp; &nbsp; &nbsp; Searched Dose:<b>{0}</b>", ThisSession.DrugStrength));
                    //lblSingleDrugDose.Text = ThisSession.DrugStrength;
                    lblSingleDrugQuantity.Text = String.Format("{0:#0} {1}",
                        Convert.ToDecimal(ThisSession.DrugQuantity),
                        ThisSession.DrugQuantityUOM);
                }
                else
                {
                    pnlSingleDrug.Visible = true;
                    pnlMultiDrug.Visible = false;

                    lblSingleDrugName.Text = dv[0].Row["DrugName"].ToString();
                    ltlSingleDrugDose.Text = (dv[0].Row["DrugStrength"].ToString().Trim() == String.Empty ? String.Empty : String.Format("&nbsp; &nbsp; &nbsp; Searched Dose:<b>{0}</b>", dv[0].Row["DrugStrength"].ToString()));
                    //lblSingleDrugDose.Text = dv[0].Row["DrugStrength"].ToString();
                    lblSingleDrugQuantity.Text = String.Format("{0:#0} {1}",
                        Convert.ToDecimal(dv[0].Row["Quantity"].ToString()),
                        dv[0].Row["QuantityUOM"].ToString());
                }
            }
            else
            {
                pnlSingleDrug.Visible = false;
                pnlMultiDrug.Visible = true;

                string[] headercols = { "DrugName", "DrugStrength", "Quantity", "QuantityUOM" };
                DataTable dtHeaders = dv.ToTable("DrugHeaders", false, headercols);
                rptMultiDrugTable.DataSource = dtHeaders;
                rptMultiDrugTable.DataBind();
            }

        }
        private void SetupFamilyMembers()
        {
            DataTable members = new DataTable("Members");
            members.Columns.Add(new DataColumn("Name"));
            members.Columns.Add(new DataColumn("DVF"));
            DataRow dr = members.NewRow();
            dr["Name"] = String.Format("{0} {1}",
                ThisSession.FirstName, ThisSession.LastName);

            dr["DVF"] = String.Format("{0}|{1}|{2}",
                ThisSession.EmployeeID,
                ThisSession.CCHID,
                ThisSession.PatientEmail);
            members.Rows.Add(dr);
            foreach (Dependent d in ThisSession.Dependents)
            {
                dr = members.NewRow();
                dr["Name"] = String.Format("{0} {1}",
                    d.FirstName, d.LastName);
                dr["DVF"] = String.Format("{0}|{1}|{2}",
                    ThisSession.EmployeeID,
                    d.CCHID,
                    d.Email);
                members.Rows.Add(dr);
            }
            ddlMembers.DataSource = members;
            ddlMembers.DataBind();
        }
        private void SetupGrid(String Lat, String Lng)
        {
            String currentPharmacyName = String.Empty;

            if (ChosenDrugsTable == null || ChosenDrugsTable.Rows.Count == 1)
            {
                using (GetDrugPricingResults gdpr = new GetDrugPricingResults())
                {
                    gdpr.Latitude = Lat;
                    gdpr.Longitude = Lng;
                    if (this.ChosenDrugsTable == null)
                    {
                        gdpr.DrugID = ThisSession.DrugID;
                        gdpr.GPI = ThisSession.DrugGPI;
                        gdpr.Quantity = ThisSession.DrugQuantity;
                        if (ThisSession.PastCareID != "")
                            gdpr.PastCareID = ThisSession.PastCareID;
                    }
                    else
                    {
                        gdpr.DrugID = this.ChosenDrugsTable.Rows[0]["DrugID"].ToString();
                        gdpr.GPI = this.ChosenDrugsTable.Rows[0]["GPI"].ToString();
                        gdpr.Quantity = this.ChosenDrugsTable.Rows[0]["Quantity"].ToString();
                        gdpr.PastCareID = this.ChosenDrugsTable.Rows[0]["PastCareID"].ToString();
                    }
                    //gdpr.MemberRXID = ThisSession.SubscriberRXID;
                    gdpr.CCHID = ThisSession.CCHID;
                    gdpr.UserID = Membership.GetUser().ProviderUserKey.ToString();
                    gdpr.GetData();

                    //If there were details returned from the database
                    if (gdpr.RawResults.TableName != "EmptyTable")
                    {
                        //Bind the details to the repeater
                        //rptResults.DataSource = gdpr.RawResults;
                        //rptResults.DataBind();

                        if (gdpr.CurrentPharmacyTable.Rows.Count > 0)
                        {
                            if (ThisSession.CurrentPharmacyID == String.Empty)
                                ThisSession.CurrentPharmacyID = gdpr.CurrentPharmacyTable.Rows[0]["PharmacyID"].ToString();
                            if (ThisSession.CurrentPharmacyLocationID == String.Empty)
                                ThisSession.CurrentPharmacyLocationID = gdpr.CurrentPharmacyTable.Rows[0]["PharmacyLocationID"].ToString();
                            if (ThisSession.CurrentPharmacyPrice == String.Empty)
                                ThisSession.CurrentPharmacyPrice = gdpr.CurrentPharmacyTable.Rows[0]["Price"].ToString();

                            currentPharmacyName = gdpr.CurrentPharmacyTable.Rows[0]["PharmacyName"].ToString();
                        }
                    }
                    //If there were results return for Past Care
                    if (gdpr.PastCare.TableName != "EmptyTable" && gdpr.PastCare.Rows.Count > 0)
                    {
                        //Set the visibility of the alert bars based off of having any pertinent past care data

                        abCurrentPrice.Visible = gdpr.HasPastCare;
                        abCouldSave.Visible = gdpr.HasPastCare;
                        //Additional PastCare work
                        if (gdpr.HasPastCare)
                        {
                            //Update the alert bar savings total with the data from the database
                            abCurrentPrice.SaveTotal = String.Format("${0:f2}", gdpr.CurrentPrice);
                            abCurrentPrice.PharmacyName = currentPharmacyName;

                            //lbNew.Text = "New Past Care";
                            //lbNew.PostBackUrl = "search.aspx#tabpast";
                        }
                    }
                    //If there were results returned for Pricing
                    if (gdpr.Pricingtable.TableName != "EmptyTable" && gdpr.Pricingtable.Rows.Count > 0)
                    {
                        //Assign the difference to the total savings property to drive the visibility of certain controls
                        abCouldSave.TotalSavings = Math.Round((gdpr.CurrentPrice - gdpr.BestPrice), 2);
                        //Set the savings total to the difference (even if 0)
                        //  lam, 20130401, MSF-260, remove .ToString() at the end so that the decimal point displays correctly
                        //abCouldSave.SaveTotal = String.Format("${0:f2}", Math.Round((gdpr.CurrentPrice - gdpr.BestPrice), 2).ToString());
                        abCouldSave.SaveTotal = String.Format("${0:f2}", Math.Round((gdpr.CurrentPrice - gdpr.BestPrice), 2));
                        abCouldSave.PharmacyName = currentPharmacyName;
                    }
                    //If we received a drug display name back from the database
                    if (gdpr.DisplayName != String.Empty)
                    {
                        lblSingleDrugName.Text = gdpr.DisplayName;
                        ltlSingleDrugDose.Text = (gdpr.DisplayStrength.Trim() == String.Empty ? String.Empty : String.Format("&nbsp; &nbsp; &nbsp; Searched Dose:<b>{0}</b>", gdpr.DisplayStrength));
                        //lblSingleDrugDose.Text = gdpr.DisplayStrength;
                        ThisSession.DrugStrength = gdpr.DisplayStrength;
                    }
                }
            }
            else
            {
                using (GetDrugMultiPricingResults gdmpr = new GetDrugMultiPricingResults())
                {
                    using (DataView dv = new DataView(this.ChosenDrugsTable))
                        gdmpr.DrugList = dv.ToTable("DrugInput", false, new String[] { "DrugID", "GPI", "Quantity", "PastCareID" });

                    gdmpr.Latitude = Lat;
                    gdmpr.Longitude = Lng;
                    gdmpr.GetData();

                    if (gdmpr.RawResults.TableName != "EmptyTable")
                    {
                        //Bind the details to the repeater
                        //rptResults.DataSource = gdpr.RawResults;
                        //rptResults.DataBind();

                        if (gdmpr.CurrentPharmacyTable.TableName != "EmptyTable" && gdmpr.CurrentPharmacyTable.Rows.Count > 0)
                        {
                            if (ThisSession.CurrentPharmacyID == String.Empty)
                                ThisSession.CurrentPharmacyID = gdmpr.CurrentPharmacyTable.Rows[0]["PharmacyID"].ToString();
                            if (ThisSession.CurrentPharmacyLocationID == String.Empty)
                                ThisSession.CurrentPharmacyLocationID = gdmpr.CurrentPharmacyTable.Rows[0]["PharmacyLocationID"].ToString();
                            if (ThisSession.CurrentPharmacyPrice == String.Empty)
                                ThisSession.CurrentPharmacyPrice = gdmpr.CurrentPharmacyTable.Rows[0]["Price"].ToString();

                            currentPharmacyName = gdmpr.CurrentPharmacyTable.Rows[0]["PharmacyName"].ToString();
                        }
                    }
                    //If there were results return for Past Care
                    if (gdmpr.PastCare.TableName != "EmptyTable" && gdmpr.PastCare.Rows.Count > 0)
                    {
                        //Set the visibility of the alert bars based off of having any pertinent past care data

                        abCurrentPrice.Visible = gdmpr.HasPastCare;
                        abCouldSave.Visible = gdmpr.HasPastCare;
                        //Additional PastCare work
                        if (gdmpr.HasPastCare)
                        {
                            //Update the alert bar savings total with the data from the database
                            abCurrentPrice.SaveTotal = String.Format("${0:f2}", gdmpr.CurrentPrice);
                            abCurrentPrice.PharmacyName = currentPharmacyName;
                        }
                    }
                    abCurrentPrice.Visible = (currentPharmacyName.Trim() != String.Empty);
                    //If there were results returned for Pricing
                    if (gdmpr.PricingTable.TableName != "EmptyTable" && gdmpr.PricingTable.Rows.Count > 0)
                    {
                        //Assign the difference to the total savings property to drive the visibility of certain controls
                        abCouldSave.TotalSavings = Math.Round((gdmpr.CurrentPrice - gdmpr.BestPrice), 2);
                        //Set the savings total to the difference (even if 0)
                        abCouldSave.SaveTotal = String.Format("${0:f2}",
                            Math.Round((gdmpr.CurrentPrice - gdmpr.BestPrice), 2).ToString());
                        abCouldSave.PharmacyName = currentPharmacyName;
                    }
                    abCouldSave.Visible = (currentPharmacyName.Trim() != String.Empty);
                }
            }


            //List<SqlParameter> prms = new List<SqlParameter>();
            //DataView dv = new DataView(this.ChosenDrugsTable);
            //DataSet results = null;

            //prmLat.Value = Convert.ToDouble(Lat);
            //prms.Add(prmLat);
            //prmLng.Value = Convert.ToDouble(Lng);
            //prms.Add(prmLng);

            //if (this.SingleDrugResults)
            //{
            //    if (this.ChosenDrugsTable == null)
            //    {
            //        prmDrugID.Value = ThisSession.DrugID;
            //        prms.Add(prmDrugID);
            //        prmGPI.Value = ThisSession.DrugGPI;
            //        prms.Add(prmGPI);
            //        prmQuantity.Value = Convert.ToDouble(ThisSession.DrugQuantity);
            //        prms.Add(prmQuantity);
            //        if (ThisSession.PastCareID != "")
            //        {
            //            prmPastCareID.Value = Convert.ToInt32(ThisSession.PastCareID);
            //            prms.Add(prmPastCareID);
            //        }
            //    }
            //    else
            //    {
            //        prmDrugID.Value = dv[0].Row["DrugID"];
            //        prms.Add(prmDrugID);
            //        prmGPI.Value = dv[0].Row["GPI"];
            //        prms.Add(prmGPI);
            //        prmQuantity.Value = Convert.ToDouble(dv[0].Row["Quantity"]);
            //        prms.Add(prmQuantity);
            //        prmPastCareID.Value = Convert.ToInt32(dv[0].Row["PastCareID"]);
            //        prms.Add(prmPastCareID);
            //    }

            //    results = QueryPharmaciesFor(PharmacyProcs.GetDrugPricingResults, prms, ThisSession.CnxString);
            //}
            //else
            //{
            //    string[] contentcols = { "DrugID", "GPI", "Quantity", "PastCareID" };
            //    prmDrugList.Value = dv.ToTable("DrugInput", false, contentcols);
            //    prms.Add(prmDrugList);

            //    results = QueryPharmaciesFor(PharmacyProcs.GetDrugMultiPricingResults, prms, ThisSession.CnxString);
            //}

            ////If there is details table returned from the database
            //if (results.Tables.Count > 0)
            //{
            //    DataView fullView = new DataView(results.Tables[0]);
            //    //Bind the details to the repeater
            //    rptResults.DataSource = fullView; //results.Tables[0];
            //    rptResults.DataBind();

            //    List<String> neededCols = new List<string>();
            //    neededCols.Add("PharmacyName");
            //    neededCols.Add("PharmacyID");
            //    neededCols.Add("PharmacyLocationID");
            //    neededCols.Add((this.SingleDrugResults ? "Price" : "TotalCost"));
            //    fullView.RowFilter = "CurrentPharmText <> ''";
            //    String pharmName = "";
            //    DataTable tblPharm = fullView.ToTable("CurrentPharm", true, neededCols.ToArray<String>());
            //    if (tblPharm.Rows.Count > 0)
            //    {
            //        if(ThisSession.CurrentPharmacyID == string.Empty)
            //            ThisSession.CurrentPharmacyID = tblPharm.Rows[0]["PharmacyID"].ToString();
            //        if(ThisSession.CurrentPharmacyLocationID == string.Empty)
            //            ThisSession.CurrentPharmacyLocationID = tblPharm.Rows[0]["PharmacyLocationID"].ToString();
            //        if (ThisSession.CurrentPharmacyPrice == string.Empty)
            //            ThisSession.CurrentPharmacyPrice = tblPharm.Rows[0][(this.SingleDrugResults ? "Price" : "TotalCost")].ToString();

            //        pharmName = tblPharm.Rows[0]["PharmacyName"].ToString();
            //    }            

            //    //If there is a second table with past care info
            //    if (results.Tables.Count > 1)
            //    {
            //        DataTable visibilityTable = results.Tables[1];
            //        //If there are any rows in the past care table
            //        if (visibilityTable.Rows.Count > 0)
            //        {
            //            //Get the Yes or No text from the first column
            //            String pharmShow = visibilityTable.Rows[0]["ShowCurrentPharmacy"].ToString();
            //            Boolean hasPast = (pharmShow.ToLower() == "yes" && (visibilityTable.Rows[0]["CurrentPrice"].ToString() != ""));

            //            //Bind the visiblity of the two alert bars to whether or not there is any past care data
            //            abCurrentPrice.Visible = hasPast;
            //            abCouldSave.Visible = hasPast;

            //            //Additional past care work
            //            if (hasPast)
            //            {
            //                //Store the current price for use with the next table ("Best Price")
            //                Double currentPrice = Double.Parse(visibilityTable.Rows[0]["CurrentPrice"].ToString());
            //                //Get the name of the current pharmacy
            //                //String currentPharm = pharmName;//visibilityTable.Rows[0]["PharmName"].ToString();
            //                //Update the alert bar savings total with the data from the database
            //                abCurrentPrice.SaveTotal = String.Format("${0:f2}",currentPrice);
            //                abCurrentPrice.PharmacyName = pharmName;

            //                //If there is a third table returned
            //                if (results.Tables.Count > 2)
            //                {
            //                    DataTable priceTable = results.Tables[2];
            //                    //If that table has any rows
            //                    if (priceTable.Rows.Count > 0)
            //                    {
            //                        //Capture the best price total 
            //                        Double bestPrice = Double.Parse(priceTable.Rows[0]["BestPrice"].ToString());
            //                        //Compare it to the current price to find savings
            //                        Double savings = currentPrice - bestPrice;
            //                        //Assign the difference to the total savings property do drive the visibilty of certain controls
            //                        abCouldSave.TotalSavings = Math.Round(savings, 2);
            //                        //Set the savings total to the difference (even if 0)
            //                        abCouldSave.SaveTotal = String.Format("${0:f2}", Math.Round(savings, 2).ToString());
            //                        abCouldSave.PharmacyName = pharmName;

            //                        ////If the savings are more than Zero, make visible the text "You could be saving...."
            //                        //((Label)abCouldSave.FindControl("lblCouldSave")).Visible = (savings != 0.0);
            //                        ////If the savings are equal to Zero, make visible the text "You are saving the most!"
            //                        //((Label)abCouldSave.FindControl("lblMaxSave")).Visible = (savings == 0.0);
            //                    }
            //                }
            //            }
            //        }

            //    }
            //}
        }
        private DataSet QueryPharmaciesFor(PharmacyProcs ProcToRun, List<SqlParameter> inpParms, String CnxString)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(CnxString))
            {
                using (SqlCommand comm = new SqlCommand(AvailablePharmProcs[ProcToRun], conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    inpParms.ForEach(delegate(SqlParameter prm)
                    {
                        comm.Parameters.Add(prm);
                    });

                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try
                        {
                            da.Fill(ds);
                        }
                        catch (Exception)
                        { }
                        finally
                        { }
                    }

                    comm.Parameters.Clear();
                }
            }
            return ds;
        }
        private int AddMedToMember(List<SqlParameter> inpParms, String CnxString)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(CnxString))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand("AddRX", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    inpParms.ForEach(delegate(SqlParameter prm)
                    {
                        comm.Parameters.Add(prm);
                    });

                    try
                    {
                        rowsAffected = comm.ExecuteNonQuery();
                    }
                    catch (Exception)
                    { }
                    finally
                    { }

                    comm.Parameters.Clear();
                }
                conn.Close();
            }
            return rowsAffected;
        }
        #endregion
    }
}