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
    public partial class results_rx_name : System.Web.UI.Page
    {
        #region Private Variables
        private DataTable Drugs
        {
            get
            {
                return (ViewState["DrugTable"] == null ? new DataTable("Empty") : ((DataTable)ViewState["DrugTable"]));
            }
            set
            {
                ViewState["DrugTable"] = value;
            }
        }
        #endregion

        #region GUI Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblDrugName.Text = ThisSession.DrugName.ToString();
                if (ThisSession.DrugEnteredFrom == "Letters")
                {

                }
                else
                {
                    if (ThisSession.DrugEnteredFrom == "Entry")
                    {
                        string quote = @"""";
                        lblDrugVerification.Text = "(You entered: " + quote + ThisSession.DrugEntered + quote + ". Click <b><a href='search.aspx'>here</a></b> if you'd like to search for a different drug.)";
                        lblDrugVerification.Visible = true;
                    }
                    else
                    { lblDrugVerification.Visible = false; }
                }
                SetupDrugList();
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void rptDrugDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //Set the name, dosage, and unit of delivery for the radio buttons
                DataRowView drvRow = (DataRowView)e.Item.DataItem;
                //Label lblRefineDosage = (Label)e.Item.FindControl("lblRefineDosage");
                RadioButton rbRefineDosage = (RadioButton)e.Item.FindControl("rbRefineDosage");
                DropDownList ddlOptions = (DropDownList)e.Item.FindControl("ddlOptions");

                string rbText = String.Format("{0} {1} {2}",
                        ThisSession.DrugName,
                        drvRow.Row["Strength"].ToString(),
                        drvRow.Row["QuantityUOM"].ToString().Replace("\n","").Replace("\r",""));
                rbRefineDosage.Text = rbText;
                rbRefineDosage.InputAttributes.Add("class", "refinedosage");
                rbRefineDosage.Attributes.Add("GPI", drvRow.Row["GPI"].ToString()); //Add a GPI attribute to the link button in order to extract it later
                rbRefineDosage.Attributes.Add("QUOM", drvRow.Row["QuantityUOM"].ToString().Replace("\n", "").Replace("\r", ""));

                //If there is a drug strength in session (as if comming from family meds) disable all the others we aren't using
                //if (ThisSession.DrugStrength != "" && Request.UrlReferrer.Segments[Request.UrlReferrer.Segments.Length - 1].ToString() != "results_rx.aspx")
                //{
                //    rbRefineDosage.Visible = (drvRow.Row["Strength"].ToString() == ThisSession.DrugStrength);
                //rbRefineDosage.Checked = (drvRow.Row["Strength"].ToString() == ThisSession.DrugStrength);
                rbRefineDosage.Checked = ((drvRow.Row["GPI"].ToString() == ThisSession.DrugGPI) &&
                    (drvRow.Row["QuantityUOM"].ToString().Replace("\n", "").Replace("\r", "") == ThisSession.DrugQuantityUOM));
                //}

                //Get a new list of drugs filtered by the GPI for this row
                using (DataView dv = new DataView(this.Drugs))
                {
                    dv.RowFilter = "GPI = '" + drvRow.Row["GPI"].ToString() + "'";
                    //Set the quantities in the drop down based off of the doseage we're working with
                    using (DataTable dt = dv.ToTable())
                    {
                        ddlOptions.DataSource = dt;
                        ddlOptions.DataBind();
                        if (ThisSession.DrugQuantity == "")
                        {
                            if (ddlOptions.Items.FindByText("30 pills") != null)
                                ddlOptions.Items.FindByText("30 pills").Selected = true;
                            else if (ddlOptions.Items.FindByText("10 pills") != null)
                                ddlOptions.Items.FindByText("10 pills").Selected = true;
                        }
                        else
                        {
                            if (ddlOptions.Items.FindByText(String.Format("{0:0} pills", Decimal.Parse(ThisSession.DrugQuantity))) != null)
                                ddlOptions.Items.FindByText(String.Format("{0:0} pills", Decimal.Parse(ThisSession.DrugQuantity))).Selected = true;
                            else
                            {
                                if (ddlOptions.Items.FindByText("30 pills") != null)
                                    ddlOptions.Items.FindByText("30 pills").Selected = true;
                                else if (ddlOptions.Items.FindByText("10 pills") != null)
                                    ddlOptions.Items.FindByText("10 pills").Selected = true;
                            }
                        }
                    }
                }

                //Setup the command aspects of the link button to extract and save to session later
                LinkButton lbSearch = (LinkButton)e.Item.FindControl("lbtnSearch");
                lbSearch.CommandName = "Select";
                lbSearch.CommandArgument = drvRow.Row["Strength"].ToString();
            }
        }
        protected String IsHidden(RepeaterItem item)
        {
            DataRowView drv = (DataRowView)item.DataItem;
            return (((drv.Row["GPI"].ToString() == ThisSession.DrugGPI) &&
               (drv.Row["QuantityUOM"].ToString().Replace("\n","").Replace("\r","") == ThisSession.DrugQuantityUOM)) ? 
               "" : "hidden");
        }
        protected void SelectDrug(object sender, EventArgs e)
        {
            ThisSession.PrevPage = "Results_Rx_Name";
            //Get an instance of which link button was clicked in the repeater
            LinkButton lbSearch = (LinkButton)sender;
            //Set the drug strength to session
            ThisSession.DrugStrength = lbSearch.CommandArgument;

            //Get the repeater row the user clicked on in order to find the ddl associated with it and the label to extract info from the controls
            RepeaterItem riRow = (RepeaterItem)lbSearch.NamingContainer;

            //Capture the ddl from the repeater item to extract the quantity VALUE from it's datavaluefield
            DropDownList ddlQuantity = (DropDownList)riRow.FindControl("ddlOptions");
            //Set the drug quantity to session
            ThisSession.DrugQuantity = ddlQuantity.SelectedValue.ToString();

            //if (ThisSession.DrugGPI == "")
            //{
            //Capture the label from the repeater to get it's GPI attribute
            RadioButton lblRefineDosage = (RadioButton)riRow.FindControl("rbRefineDosage");
            //Add the GPI to the session for the stored proceedure on the next page
            ThisSession.DrugGPI = lblRefineDosage.Attributes["GPI"].ToString();
            ThisSession.DrugQuantityUOM = lblRefineDosage.Attributes["QUOM"].ToString().Replace("\n", "").Replace("\r", "");
            //}

            //Move to results page
            Response.Redirect("results_rx.aspx");
        }
        #endregion

        #region Supporting Methods
        private void SetupDrugList()
        {
            using (GetDrugDetailOptions gddo = new GetDrugDetailOptions())
            {
                gddo.DrugID = ThisSession.DrugID;
                gddo.Latitude = ThisSession.PatientLatitude;
                gddo.Longitude = ThisSession.PatientLongitude;
                gddo.GetData();
                //gddo.GetReaderData();
                if (!gddo.HasErrors && gddo.RowsBack > 0)
                {
                    this.Drugs = gddo.Drugs;
                    using (DataView dv = new DataView(this.Drugs))
                    {
                        rptDrugDetails.DataSource = dv.ToTable("DistinctDrugs",
                            true,
                            new String[] { "GPI", "Strength", "QuantityUOM" });
                        rptDrugDetails.DataBind();
                    }
                }
            }
        }
        #endregion
    }
}