using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

namespace ClearCostWeb.SearchInfo
{
    public partial class pharmacy_detail : System.Web.UI.Page
    {
        #region Private Variables
        private DataTable dtPharmacyDetails
        {
            get
            {
                return (ViewState["dtPharmacyDetails"] == null ? new DataTable("Empty") : ((DataTable)ViewState["dtPharmacyDetails"]));
            }
            set { ViewState["dtPharmacyDetails"] = value; }
        }
        private DataTable dtTransferDetails
        {
            get
            {
                return (ViewState["dtTransferDetails"] == null ? new DataTable("Empty") : ((DataTable)ViewState["dtTransferDetails"]));
            }
            set { ViewState["dtTransferDetails"] = value; }
        }
        private DataTable dtCurrentPharmacyDetails
        {
            get
            {
                return (ViewState["dtCurrentPharmacyDetails"] == null ? new DataTable("Empty") : ((DataTable)ViewState["dtCurrentPharmacyDetails"]));
            }
            set { ViewState["dtCurrentPharmacyDetails"] = value; }
        }
        private SqlParameter prmDrugID = new SqlParameter("DrugID", SqlDbType.Int);
        private SqlParameter prmGPI = new SqlParameter("GPI", SqlDbType.NVarChar, 50);
        private SqlParameter prmQuantity = new SqlParameter("Quantity", SqlDbType.Decimal);
        private SqlParameter prmPharmacyID = new SqlParameter("PharmacyID", SqlDbType.Int);
        private SqlParameter prmPharmacyLocationID = new SqlParameter("PharmacyLocationID", SqlDbType.Int);
        private SqlParameter prmPastCareID = new SqlParameter("PastCareID", SqlDbType.Int);
        private SqlParameter prmDrugList = new SqlParameter("DrugList", SqlDbType.Structured);

        private enum PharmacyProcs { GetSelectedPharmacyDetails, GetSelectedPharmacyDetailsMultiRX }
        private static Dictionary<PharmacyProcs, String> AvailablePharmProcs = new Dictionary<PharmacyProcs, String>{
        { PharmacyProcs.GetSelectedPharmacyDetails, "GetSelectedPharmacyDetails" },
        { PharmacyProcs.GetSelectedPharmacyDetailsMultiRX, "GetSelectedPharmacyDetailsMultiRX" }
    };
        //private enum PharmDetailColumns { Details = 0, PharmacyName, WebURL, Address1, Address2, City, State, Zipcode, Telephone, Email, Latitude, Longitude, Fax, Hours, Price, PharmacyID, PharmacyLocationID }
        //private enum TransferDetailColumns { Show = 0 }
        //private enum CurrentPharmacyColumns { Details = 0, PharmacyName, WebURL, Address1, Address2, City, State, Zipcode, Telephone, Email, Latitude, Longitude, Fax, Hours, Price, PrescriptionNum, PharmacyID, PharmacyLocationID }
        #endregion

        #region Public Properties
        //Used to determine if "Add this drug to Med List" is visible depending on if we got here from past care
        protected String IsHidden { get { return ((ThisSession.PastCareID == "") ? "" : " hidden"); } }

        private String PharmacyName { get { return dtPharmacyDetails.Rows[0]["PharmacyName"].ToString(); } }
        private String PharmacyID { get { return dtPharmacyDetails.Rows[0]["PharmacyID"].ToString(); } }
        private String PharmacyLocationID { get { return dtPharmacyDetails.Rows[0]["PharmacyLocationID"].ToString(); } }
        private String WebURL { get { return dtPharmacyDetails.Rows[0]["WebURL"].ToString(); } }
        private Boolean HasWeb { get { return (WebURL.Trim() != ""); } }
        private String Address1 { get { return dtPharmacyDetails.Rows[0]["Address1"].ToString(); } }
        private String Address2 { get { return dtPharmacyDetails.Rows[0]["Address2"].ToString(); } }
        private String City { get { return dtPharmacyDetails.Rows[0]["City"].ToString(); } }
        private String State { get { return dtPharmacyDetails.Rows[0]["State"].ToString(); } }
        private String ZipCode { get { return dtPharmacyDetails.Rows[0]["Zipcode"].ToString(); } }
        private String Telephone { get { return dtPharmacyDetails.Rows[0]["Telephone"].ToString(); } }
        private Boolean HasPhone { get { return (Telephone.Trim() != ""); } }
        private String Email { get { return dtPharmacyDetails.Rows[0]["Email"].ToString(); } }
        private Boolean HasEmail { get { return (Email.Trim() != ""); } }
        private String Latitude { get { return dtPharmacyDetails.Rows[0]["Latitude"].ToString(); } }
        private String Longitude { get { return dtPharmacyDetails.Rows[0]["Longitude"].ToString(); } }
        private String Fax { get { return dtPharmacyDetails.Rows[0]["Fax"].ToString(); } }
        private Boolean HasFax { get { return (Fax != ""); } }
        private String Hours { get { return dtPharmacyDetails.Rows[0]["Hours"].ToString(); } }
        private Boolean HasHours { get { return (Hours != ""); } }
        private String Price { get { return dtPharmacyDetails.Rows[0]["Price"].ToString(); } }

        private Boolean ShowTransfer { get { return (dtTransferDetails.Rows[0]["ShowTransferInfo"].ToString() != "0"); } }
        protected Boolean UITransferVisible { get { return (ViewState["UITransferVisible"] == null ? false : Convert.ToBoolean(ViewState["UITransferVisible"])); } set { ViewState["UITransferVisible"] = value; } }

        private String CurrentPharmacyName { get { return dtCurrentPharmacyDetails.Rows[0]["PharmacyName"].ToString(); } }
        private String CurrentPharmacyID { get { return dtCurrentPharmacyDetails.Rows[0]["PharmacyID"].ToString(); } }
        private String CurrentPharmacyLocationID { get { return dtCurrentPharmacyDetails.Rows[0]["PharmacyLocationID"].ToString(); } }
        private String CurrentPharmacyPhone { get { return dtCurrentPharmacyDetails.Rows[0]["Telephone"].ToString(); } }
        private String CurrentPharmacyAddress1 { get { return dtCurrentPharmacyDetails.Rows[0]["Address1"].ToString(); } }
        private String CurrentPharmacyAddress2 { get { return dtCurrentPharmacyDetails.Rows[0]["Address2"].ToString(); } }
        private String CurrentPharmacyCity { get { return dtCurrentPharmacyDetails.Rows[0]["City"].ToString(); } }
        private String CurrentPharmacyState { get { return dtCurrentPharmacyDetails.Rows[0]["State"].ToString(); } }
        private String CurrentPharmacyZip { get { return dtCurrentPharmacyDetails.Rows[0]["Zipcode"].ToString(); } }
        private String CurrentPrescriptionNumber { get { return dtCurrentPharmacyDetails.Rows[0]["PrescriptionNumber"].ToString(); } }
        private String CurrentPharmacyPrice { get { return dtCurrentPharmacyDetails.Rows[0]["Price"].ToString(); } }

        private DataTable ChosenDrugsTable { get { if ((DataTable)ViewState["ChosenDrugs"] == null) { ViewState["ChosenDrugs"] = ThisSession.ChosenDrugs; } return (DataTable)ViewState["ChosenDrugs"]; } set { ViewState["ChosenDrugs"] = (DataTable)value; } }
        private Boolean SingleDrugResults { get { return (ChosenDrugsTable == null ? true : (ChosenDrugsTable.Rows.Count == 1 ? true : false)); } }

        private String PostBackControl { get { return Request.Params["__EVENTTARGET"].ToString(); } }
        private String PostBackArgument { get { return Request.Params["__EVENTARGUMENT"].ToString(); } }
        private String[] PostBackLatLng { get { return (PostBackControl == "Geocoder" ? PostBackArgument.Split('|') : null); } }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                prmQuantity.Precision = 18;
                prmQuantity.Scale = 2;

                dtPharmacyDetails = new DataTable();

                SetupHeaders();

                GetSelectedFacilityDetails();

                lblPharmacyName.Attributes.Add("pLat", Latitude);
                lblPharmacyName.Attributes.Add("pLng", Longitude);

                ThisSession.FacilityLatitude = Latitude;
                ThisSession.FacilityLongitude = Longitude;

                lblAllResult1DisclaimerText.Text = ThisSession.AllResult1DisclaimerText;  //  lam, 20130425, MSF-295 move disclaimer text to content manager
            }
            else if (PostBackControl == "Geocoder")
            {
                abCurrentPrice.PharmacyName = this.CurrentPharmacyName;
                abCurrentPrice.SaveTotal = string.Format("{0:c2}", double.Parse(this.CurrentPharmacyPrice));
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void NavNew(object sender, EventArgs e)
        {
            ThisSession.DrugEntered = string.Empty;
            ThisSession.DrugEnteredFrom = string.Empty;
            ThisSession.DrugGPI = string.Empty;
            ThisSession.DrugID = string.Empty;
            ThisSession.DrugName = string.Empty;
            ThisSession.DrugQuantity = string.Empty;
            ThisSession.DrugStrength = string.Empty;
            ThisSession.PastCareID = string.Empty;
            ThisSession.DrugQuantityUOM = string.Empty;
            Response.Redirect("Search.aspx");
        }

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
                    lblSingleDrugDose.Text = ThisSession.DrugStrength;
                    lblSingleDrugQuantity.Text = String.Format("{0:#0} {1}",
                        Convert.ToDecimal(ThisSession.DrugQuantity),
                        ThisSession.DrugQuantityUOM);
                }
                else
                {
                    pnlSingleDrug.Visible = true;
                    pnlMultiDrug.Visible = false;

                    lblSingleDrugName.Text = dv[0].Row["DrugName"].ToString();
                    lblSingleDrugDose.Text = dv[0].Row["DrugStrength"].ToString();
                    lblSingleDrugQuantity.Text = String.Format("{0:d0} {1}",
                        dv[0].Row["Quantity"].ToString(),
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
        private void GetSelectedFacilityDetails()
        {
            List<SqlParameter> prms = new List<SqlParameter>();
            DataView dv = new DataView(this.ChosenDrugsTable);
            DataSet results = null;

            prmPharmacyID.Value = ThisSession.PharmacyID;
            prms.Add(prmPharmacyID);
            prmPharmacyLocationID.Value = ThisSession.PharmacyLocationID;
            prms.Add(prmPharmacyLocationID);

            if (this.SingleDrugResults)
            {
                if (this.ChosenDrugsTable == null)
                {
                    prmDrugID.Value = ThisSession.DrugID;
                    prms.Add(prmDrugID);
                    prmGPI.Value = ThisSession.DrugGPI;
                    prms.Add(prmGPI);
                    prmQuantity.Value = Convert.ToDouble(ThisSession.DrugQuantity);
                    prms.Add(prmQuantity);
                    if (ThisSession.PastCareID != "")
                    {
                        prmPastCareID.Value = Convert.ToInt32(ThisSession.PastCareID);
                        prms.Add(prmPastCareID);
                    }
                }
                else
                {
                    prmDrugID.Value = Convert.ToInt16(dv[0].Row["DrugID"]);
                    prms.Add(prmDrugID);
                    prmGPI.Value = dv[0].Row["GPI"];
                    prms.Add(prmGPI);
                    prmQuantity.Value = Convert.ToDouble(dv[0].Row["Quantity"]);
                    prms.Add(prmQuantity);
                    prmPastCareID.Value = Convert.ToInt32(dv[0].Row["PastCareID"]);
                    prms.Add(prmPastCareID);
                }

                results = QueryPharmaciesFor(PharmacyProcs.GetSelectedPharmacyDetails, prms, ThisSession.CnxString);
            }
            else
            {
                string[] contentcols = { "DrugID", "GPI", "Quantity", "PastCareID" };
                prmDrugList.Value = dv.ToTable("DrugInput", false, contentcols);
                prms.Add(prmDrugList);

                results = QueryPharmaciesFor(PharmacyProcs.GetSelectedPharmacyDetailsMultiRX, prms, ThisSession.CnxString);
            }

            //DataSet results = QueryPharmaciesFor(PharmacyProcs.GetSelectedPharmacyDetails, prms, ThisSession.CnxString);
            //If there is details table returned from the database
            if (results.Tables.Count >= 1)
            {
                dtPharmacyDetails = results.Tables[0];

                if (results.Tables[0].Rows.Count > 0)
                {

                    lblPharmacyName.Text = PharmacyName;
                    if (ThisSession.CurrentPharmacyID == this.PharmacyID && ThisSession.CurrentPharmacyLocationID == this.PharmacyLocationID)
                    {
                        abCurrentPrice.Visible = false;
                        pnlSavingsTitle.Visible = false;
                        lblSavings.Visible = false;
                        lblPharmacyName.Text += " (Current Pharmacy)";
                    }

                    lblPrice.Text = string.Format("{0:c2}", double.Parse(this.Price));

                    double savings = ((double.Parse(ThisSession.CurrentPharmacyPrice)) - (double.Parse(this.Price)));
                    pnlSavingsTitle.Visible = (savings > 0);
                    if (savings >= (double)0)
                        lblSavings.Text = string.Format("{0:c2}", savings);

                    lblAddressLine1.Text = Address1;
                    lblAddressLine2.Text = Address2;

                    lblPhoneTitle.Visible = HasPhone;
                    if (HasPhone)
                        lblPhone.Text = Telephone;

                    lblFaxTitle.Visible = HasFax;
                    if (HasFax)
                        lblFax.Text = Fax;

                    lblHoursTitle.Visible = HasHours;
                    if (HasHours)
                        lblHours.Text = Hours.Replace("|", "<br />");

                    lblEmailTitle.Visible = HasEmail;
                    if (HasEmail)
                        lblEmail.Text = Email;

                    lblWebSiteTitle.Visible = HasWeb;
                    if (HasWeb)
                        ltlURL.Text = String.Format("<a href=\"http://{0}\" target=\"_new\">{0}</a>", WebURL);
                }
            }
            //If there is a "ShowTransfer" table returned from the database
            if (results.Tables.Count >= 2 && results.Tables[1].Rows.Count > 0)
            {
                dtTransferDetails = results.Tables[1];

                //ltlShowTransfer.Text = (ShowTransfer ? @"Transfer Prescription to this Pharmacy" : @"<script>$(""#transfer"").toggleClass(""hidden"");$(""#transferDetails"").hide();</script>");
                UITransferVisible = ShowTransfer;
            }
            else
            {
                //ltlShowTransfer.Text = @"<script>$(""#transfer"").toggleClass(""hidden"");$(""#transferDetails"").hide();</script>";
                UITransferVisible = false;
            }
            //If there is a "CurrentPharmacy" table returned from the database
            if (results.Tables.Count >= 3)
            {
                dtCurrentPharmacyDetails = results.Tables[2];
                if (dtCurrentPharmacyDetails.Rows.Count > 0)
                {
                    abCurrentPrice.PharmacyName = this.CurrentPharmacyName;
                    abCurrentPrice.SaveTotal = string.Format("{0:c2}", double.Parse(this.CurrentPharmacyPrice));
                    pnlSavingsTitle.Visible = true;
                    double savings = ((double.Parse(this.CurrentPharmacyPrice) - (double.Parse(this.Price))));
                    if (savings >= 0.0)
                        lblSavings.Text = string.Format("{0:c2}", savings);

                    if (ShowTransfer)
                    {
                        DataTable dtCurrentForRepeater = dtCurrentPharmacyDetails.Clone();
                        dtCurrentForRepeater.Merge(dtCurrentPharmacyDetails);
                        dtCurrentForRepeater.Columns.Add(new DataColumn("OldPharmText", Type.GetType("System.String")));
                        if (this.SingleDrugResults)
                            dtCurrentForRepeater.Columns.Add(new DataColumn("DrugName", Type.GetType("System.String")));
                        if (this.SingleDrugResults)
                            dtCurrentForRepeater.Columns.Add(new DataColumn("Strength", Type.GetType("System.String")));
                        if (this.SingleDrugResults)
                            dtCurrentForRepeater.Columns.Add(new DataColumn("Quantity", Type.GetType("System.String")));

                        dtCurrentForRepeater.Columns.Add(new DataColumn("QuantityUOM", Type.GetType("System.String")));
                        foreach (DataRow dr in dtCurrentForRepeater.Rows)
                        {
                            dr["OldPharmText"] = string.Format("{0}<br />{1}{2}<br />{3}, {4} {5}",
                                                            dr["PharmacyName"].ToString(),
                                                            dr["Address1"].ToString(),
                                                            (dr["Address2"].ToString() != string.Empty ? "<br />" + dr["Address2"].ToString() : ""),
                                                            dr["City"].ToString(),
                                                            dr["State"].ToString(),
                                                            dr["Zipcode"].ToString());
                            if (this.SingleDrugResults)
                            {
                                dr["DrugName"] = ThisSession.DrugName;
                                dr["Strength"] = ThisSession.DrugStrength;
                                dr["Quantity"] = ThisSession.DrugQuantity;
                                dr["QuantityUOM"] = ThisSession.DrugQuantityUOM;
                            }
                            else
                            {
                                dr["DrugName"] = this.ChosenDrugsTable.Rows[dtCurrentForRepeater.Rows.IndexOf(dr)]["DrugName"].ToString();
                                dr["Strength"] = this.ChosenDrugsTable.Rows[dtCurrentForRepeater.Rows.IndexOf(dr)]["DrugStrength"].ToString();
                                dr["Quantity"] = this.ChosenDrugsTable.Rows[dtCurrentForRepeater.Rows.IndexOf(dr)]["Quantity"].ToString();
                                dr["QuantityUOM"] = this.ChosenDrugsTable.Rows[dtCurrentForRepeater.Rows.IndexOf(dr)]["QuantityUOM"].ToString();
                            }
                        }
                        DataView dvTransfer = new DataView(dtCurrentForRepeater);
                        string[] transfercols = { "DrugName", "Quantity", "QuantityUOM", "Strength", "OldPharmText", "PrescriptionNumber" };
                        rptTransfer.DataSource = dvTransfer.ToTable("Transfer", false, transfercols);
                        rptTransfer.DataBind();
                        //lblTransferDrug.Text = ThisSession.DrugName;
                        //lblTransferQuantity.Text = string.Format("{0} {1}", ThisSession.DrugQuantity, ThisSession.DrugQuantityUOM);
                        //lblTransferDosage.Text = ThisSession.DrugStrength;
                        //ltlOldPharmacy.Text = string.Format("{0}<br />{1}{2}<br />{3}, {4} {5}",
                        //                                    this.CurrentPharmacyName,
                        //                                    this.CurrentPharmacyAddress1,
                        //                                    (this.CurrentPharmacyAddress2 != string.Empty ?  "<br />" +  this.CurrentPharmacyAddress2 : ""),
                        //                                    this.CurrentPharmacyCity,
                        //                                    this.CurrentPharmacyState,
                        //                                    this.CurrentPharmacyZip);
                        //lblTransferPhone.Text = this.CurrentPharmacyPhone;
                        //lblPerscriptionNumber.Text = this.CurrentPrescriptionNumber;
                    }
                }
                else
                {
                    pnlSavingsTitle.Visible = false;
                    abCurrentPrice.Visible = false;
                    lblSavings.Visible = false;
                    UITransferVisible = false;
                }
            }
            else
            {
                abCurrentPrice.Visible = false;
                pnlSavingsTitle.Visible = false;
                lblSavings.Visible = false;
                //ltlShowTransfer.Text = @"<script>$(""#transfer"").toggleClass(""hidden"");$(""#transferDetails"").hide();</script>";
                UITransferVisible = false;
            }
        }
        protected String IsTopRow(int itemIndex) { return ((itemIndex == 0) ? " graytop" : ""); }
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
        #endregion
    }
}