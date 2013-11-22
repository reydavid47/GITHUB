using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

namespace ClearCostWeb.Controls
{
    public partial class FindPastCare : System.Web.UI.UserControl
    {
        #region GUI Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Populate dependents list
                ListItem li = new ListItem("All", "0");
                ddlMembers.Items.Add(li);
                li = new ListItem(
                    String.Concat(ThisSession.FirstName, " ", ThisSession.LastName),
                    ThisSession.CCHID.ToString());
                ddlMembers.Items.Add(li);
                foreach (Dependent dep in ThisSession.Dependents)
                {
                    li = new ListItem(
                        String.Concat(dep.FirstName, " ", dep.LastName),
                        dep.CCHID.ToString());
                    ddlMembers.Items.Add(li);
                }

                lblPastCareDisclaimerText.Text = ThisSession.PastCareDisclaimerText.Trim();  //  lam, 20130418, MSF-299

                //Populate Past Care area
                SetupPastCare(0);
            }
        }
        protected void ddlMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupPastCare(int.Parse(ddlMembers.SelectedValue));
        }
        protected void lbtnYouCouldHaveSaved_Click(object sender, EventArgs e)
        {
            ThisSession.PrevPage = "FindPastCare";
            LinkButton lbtnYouCouldHaveSaved = (LinkButton)sender;

            string[] args = lbtnYouCouldHaveSaved.CommandArgument.Split('|');
            if (args[2] == "rptPrescriptionDrugs")
            {
                ThisSession.DrugName = args[0];

                string[] drugInfo = args[3].Split('~');
                ThisSession.DrugGPI = drugInfo[0];
                ThisSession.DrugQuantity = drugInfo[1];
                ThisSession.DrugID = drugInfo[2];
                ThisSession.PharmacyLocationID = drugInfo[3];
                ThisSession.PastCareID = args[5];
                Response.Redirect("results_rx.aspx");
            }
            else
            {
                ThisSession.ServiceName = args[0];
                ThisSession.PastCareFacilityName = args[1];
                ThisSession.PastCareProcedureCode = args[3];
                ThisSession.SpecialtyID = int.Parse((args[4] == string.Empty ? "-1" : args[4]));
                ThisSession.PastCareID = args[5];
                ThisSession.ServiceID = int.Parse(args[6]);
                Response.Redirect("results_past_care.aspx");
            }

            //set the latitude and longitude if they changed locations.
            //SetLatLong();

        }
        protected void rptOfficeVisits_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            { return; }

            Repeater rpt = (Repeater)sender;

            //Could be either a Service or a drug
            dynamic d = e.Item.DataItem;
            //Is this a drug item or not
            Boolean drugItem = d.Category.ToLower().Contains("drug");

            LinkButton lbtnYouCouldHaveSaved = (LinkButton)e.Item.FindControl("lbtnYouCouldHaveSaved");
            Label lblYouCouldHaveSavedNothing = (Label)e.Item.FindControl("lblYouCouldHaveSavedNothing");

            if (d.YouCouldHaveSaved == "$0")
            { lblYouCouldHaveSavedNothing.Visible = true; lbtnYouCouldHaveSaved.Visible = false; }
            else
            {
                lblYouCouldHaveSavedNothing.Visible = false; lbtnYouCouldHaveSaved.Visible = true;
                lbtnYouCouldHaveSaved.CommandArgument = String.Format("{0}|{1}|{2}|", d.ServiceName, d.FacilityName, rpt.ID);
                if (drugItem)
                    lbtnYouCouldHaveSaved.CommandArgument = String.Concat(
                        lbtnYouCouldHaveSaved.CommandArgument,
                        String.Format("{0}~{1}~{2}~{3}", d.GPI, d.Quantity, d.DrugID, d.PharmacyLocationID));
                else
                    lbtnYouCouldHaveSaved.CommandArgument = String.Concat(
                        lbtnYouCouldHaveSaved.CommandArgument,
                        d.ProcedureCode);
                lbtnYouCouldHaveSaved.CommandArgument = String.Concat(
                    lbtnYouCouldHaveSaved.CommandArgument,
                    String.Format("|{0}|{1}|{2}", d.SpecialtyID, d.PastCareID, d.ServiceID));
                lbtnYouCouldHaveSaved.CommandName = "Select";
            }
        }
        #endregion

        #region Helper Methods
        private void SetupPastCare(int CCHID)
        {
            using (GetPastCare gpc = new GetPastCare())
            {
                gpc.CCHID = ThisSession.CCHID;
                gpc.SubscriberMedicalID = CCHID.ToString();
                gpc.GetData();
                if (!gpc.HasErrors)
                {
                    //Setup You Could Have Saved
                    if (gpc.YouCouldHaveSaved != null)
                    {
                        lblTotalYouSpent.Text = String.Format("{0:c0}", gpc.YouCouldHaveSaved.Item1);
                        lblTotalYouCouldHaveSaved.Text = String.Format("{0:c0}", gpc.YouCouldHaveSaved.Item2);
                    }
                    else
                        lblTotalYouSpent.Text = lblTotalYouCouldHaveSaved.Text = "--";

                    String ssHTML = "<div style=\"text-align:left;color:white;float:left;position:relative;left:5px\">Smart Shopper!</div>";

                    //Setup Office Visits
                    if (gpc.OfficeVisitSavings != null)
                    {
                        //Spent
                        if (gpc.OfficeVisitSavings.Item1 == 0) lblOfficeVisitsTotalYouSpent.Text = "--";
                        else lblOfficeVisitsTotalYouSpent.Text = String.Format("{0:c0}", gpc.OfficeVisitSavings.Item1);

                        //Saved
                        if (gpc.OfficeVisitSavings.Item2 == 0) lblOfficeVisitsTotalYouCouldHaveSaved.Text = ssHTML;
                        else lblOfficeVisitsTotalYouCouldHaveSaved.Text = String.Format("{0:c0}", gpc.OfficeVisitSavings.Item2);

                        //Repeater
                        rptOfficeVisits.DataBind(gpc.OfficeVisits);
                    }
                    else
                    {
                        lblOfficeVisitsTotalYouSpent.Text = lblOfficeVisitsTotalYouCouldHaveSaved.Text = "--";
                        rptOfficeVisits.DataBind(null);
                    }
                    //Visibility
                    rptOfficeVisits.Visible = ltlEnabledOfficeVisits.Visible = (gpc.OfficeVisits.Count >= 1);
                    ltlDisabledOfficeVisits.Visible = (gpc.OfficeVisits.Count == 0);

                    //Setup Laboratory Services
                    if (gpc.LabServiceSavings != null)
                    {
                        //Spent
                        if (gpc.LabServiceSavings.Item1 == 0) lblLaboratoryServicesTotalYouSpent.Text = "--";
                        else lblLaboratoryServicesTotalYouSpent.Text = String.Format("{0:c0}", gpc.LabServiceSavings.Item1);

                        //Saved
                        if (gpc.LabServiceSavings.Item2 == 0) lblLaboratoryServicesTotalYouCouldHaveSaved.Text = ssHTML;
                        else lblLaboratoryServicesTotalYouCouldHaveSaved.Text = String.Format("{0:c0}", gpc.LabServiceSavings.Item2);

                        //Repeater
                        rptLaboratoryServices.DataBind(gpc.LabServices);
                    }
                    else
                    {
                        lblLaboratoryServicesTotalYouSpent.Text = lblLaboratoryServicesTotalYouCouldHaveSaved.Text = "--";
                        rptLaboratoryServices.DataBind(null);
                    }
                    //Visibility
                    rptLaboratoryServices.Visible = ltlEnableLabServices.Visible = (gpc.LabServices.Count >= 1);
                    ltlDisableLabServices.Visible = (gpc.LabServices.Count == 0);

                    //Setup Outpatient Proceedures
                    if (gpc.OutpatientProceedureSavings != null)
                    {
                        //Spent
                        if (gpc.OutpatientProceedureSavings.Item1 == 0) lblOutpatientProceduresTotalYouSpent.Text = "--";
                        else lblOutpatientProceduresTotalYouSpent.Text = String.Format("{0:c0}", gpc.OutpatientProceedureSavings.Item1);

                        //Saved
                        if (gpc.OutpatientProceedureSavings.Item2 == 0) lblOutpatientProceduresTotalYouCouldHaveSaved.Text = ssHTML;
                        else lblOutpatientProceduresTotalYouCouldHaveSaved.Text = String.Format("{0:c0}", gpc.OutpatientProceedureSavings.Item2);

                        //Repeater
                        rptOutpatientProcedures.DataBind(gpc.OutpatientProceedures);
                    }
                    else
                    {
                        lblOutpatientProceduresTotalYouSpent.Text = lblOutpatientProceduresTotalYouCouldHaveSaved.Text = "--";
                        rptOutpatientProcedures.DataBind(null);
                    }
                    //Visibility
                    rptOutpatientProcedures.Visible = ltlEnableOutpatient.Visible = (gpc.OutpatientProceedures.Count >= 1);
                    ltlDisableOutpatient.Visible = (gpc.OutpatientProceedures.Count == 0);

                    //Setup Imaging
                    if (gpc.ImagingSavings != null)
                    {
                        //Spent
                        if (gpc.ImagingSavings.Item1 == 0) lblImagingTotalYouSpent.Text = "--";
                        else lblImagingTotalYouSpent.Text = String.Format("{0:c0}", gpc.ImagingSavings.Item1);

                        //Saved
                        if (gpc.ImagingSavings.Item2 == 0) lblImagingTotalYouCouldHaveSaved.Text = ssHTML;
                        else lblImagingTotalYouCouldHaveSaved.Text = String.Format("{0:c0}", gpc.ImagingSavings.Item2);

                        //Repeater
                        rptImaging.DataBind(gpc.Imaging);
                    }
                    else
                    {
                        lblImagingTotalYouSpent.Text = lblImagingTotalYouCouldHaveSaved.Text = "--";
                        rptImaging.DataBind(null);
                    }
                    //Visibility
                    rptImaging.Visible = ltlEnableImaging.Visible = (gpc.Imaging.Count >= 1);
                    ltlDisableImaging.Visible = (gpc.Imaging.Count == 0);

                    //Setup Drugs
                    if (gpc.DrugSavings != null)
                    {
                        //Spent
                        if (gpc.DrugSavings.Item1 == 0) lblPrescriptionDrugsTotalYouSpent.Text = "--";
                        else lblPrescriptionDrugsTotalYouSpent.Text = String.Format("{0:c0}", gpc.DrugSavings.Item1);

                        //Saved
                        if (gpc.DrugSavings.Item2 == 0) lblPrescriptionDrugsTotalYouCouldHaveSaved.Text = ssHTML;
                        else lblPrescriptionDrugsTotalYouCouldHaveSaved.Text = String.Format("{0:c0}", gpc.DrugSavings.Item2);

                        //Repeater
                        rptPrescriptionDrugs.DataBind(gpc.Drugs);
                    }
                    else
                    {
                        lblPrescriptionDrugsTotalYouSpent.Text = lblPrescriptionDrugsTotalYouCouldHaveSaved.Text = "--";
                        rptPrescriptionDrugs.DataBind(null);
                    }

                    ltlAsOfDate.Text = (gpc.AsOfDate != null && gpc.AsOfDate.Item1 != "" ? gpc.AsOfDate.Item1 : "N/A");  //  lam, 20130729, MSF-420

                    //Visibility
                    rptPrescriptionDrugs.Visible = ltlEnableDrugs.Visible = (gpc.Drugs.Count >= 1);
                    ltlDisableDrugs.Visible = (gpc.Drugs.Count == 0);
                }
            }
        }
        #endregion
    }
}