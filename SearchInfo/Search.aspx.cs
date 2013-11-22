using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;

namespace ClearCostWeb.SearchInfo
{
    public partial class Search : System.Web.UI.Page
    {
        #region GeoCoding Handlers REQUIRED
        private String PostBackControl { get { return Request.Params["__EVENTTARGET"].ToString(); } }
        private String PostBackArgument { get { return Request.Params["__EVENTARGUMENT"].ToString(); } }
        private String[] PostBackLatLng { get { return (PostBackControl == "Geocoder" ? PostBackArgument.Split('|') : null); } }
        #endregion

        //protected int RequestedTab
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session["requestedTab"] != null)
        //        {
        //            switch ((string)HttpContext.Current.Session["requestedTab"])
        //            {
        //                case "SCDashboard":
        //                    return 1;
        //                case "FindRx":
        //                    return 2;
        //                case "FindADoc":
        //                    return 3;
        //                case "PastCare":
        //                    return 4;
        //            }
        //        }
                
        //        return 0;
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            abSearch.NavigateTo = String.Format("javascript:__doPostBack('{0}','')",lbtnPastCare.UniqueID);
            if (!Page.IsPostBack)
            {
                liSavingsChoice.Visible = ThisSession.SavingsChoiceEnabled && ThisSession.ShowSCIQTab;  //  lam, 20130816, SCIQ-77
                //liSavingsChoice.Visible = ThisSession.SavingsChoiceEnabled;  lam, 20130816, SCIQ-77
                //  lam, 20130719, MSF-448 block the following 2 lines
                //if (ThisSession.SavingsChoiceEnabled)
                //    ThisSession.SelectedTab = ThisSession.AvailableTab.SavingsChoiceDashboard;
                //  lam, 20130719, MSF-448 add the following "if/else" block
                if (HttpContext.Current.Session["SelectedTab"] == null)
                {
                    //if (ThisSession.SavingsChoiceEnabled)  lam, 20130816, SCIQ-77
                    if (ThisSession.SavingsChoiceEnabled && ThisSession.ShowSCIQTab)  //  lam, 20130816, sCIQ-77
                        ThisSession.SelectedTab = ThisSession.AvailableTab.SavingsChoiceDashboard;
                    else
                        ThisSession.SelectedTab = ThisSession.AvailableTab.FindAService;
                }
                else
                    ThisSession.SelectedTab = (ThisSession.AvailableTab)HttpContext.Current.Session["SelectedTab"];

                MakeVisible(ThisSession.SelectedTab);

                abSearch.SaveTotal = ThisSession.YouCouldHaveSavedDisplay;

                SetupFamilyMemberSection();

                // Clear out information on how user entered select from session.
                ClearSessionEntryInfo();
            }
            else if (PostBackControl == "Geocoder") { abSearch.SaveTotal = ThisSession.YouCouldHaveSavedDisplay; }
            else
            {
                // Postbacks losing java script
                string myScript = @"         
                $('tr.category').toggle(showrows, hiderows);

                function showrows() {
                    $(this).nextAll('tr').each(function() {
                        if ($(this).hasClass('category') || $(this).hasClass('trfooter')) {
                            return false;
                        }
                        $(this).show();
                    });
                    if ($(this).hasClass('rowclosed')) {
                        $(this).removeClass('rowclosed');
                        $(this).addClass('rowopen');
                    }
                }
                function hiderows() {
                    $(this).nextAll('tr').each(function() {
                        if ($(this).hasClass('category') || $(this).hasClass('trfooter')) {
                            return false;
                        }
                        $(this).hide();
                    });
                    if ($(this).hasClass('rowopen')) {
                        $(this).removeClass('rowopen');
                        $(this).addClass('rowclosed');
                    }
                } ";

                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), myScript, true);
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void ClearSessionEntryInfo()
        {
            ThisSession.ServiceEnteredFrom = string.Empty;
            ThisSession.ServiceEntered = string.Empty;
            ThisSession.SpecialtyID = -1;
            ThisSession.Specialty = string.Empty;
        }
        protected void lbtnYouCouldHaveSaved_Click(object sender, EventArgs e)
        {
            LinkButton lbtnYouCouldHaveSaved = (LinkButton)sender;

            string[] args = lbtnYouCouldHaveSaved.CommandArgument.Split('|');

            ThisSession.ServiceName = args[0];
            ThisSession.PastCareFacilityName = args[1];
            ThisSession.PastCareProcedureCode = args[3];

            //set the latitude and longitude if they changed locations.
            //SetLatLong();

            //go to results
            if (args[2] == "rptPrescriptionDrugs")
            { Response.Redirect("search.aspx"); }
            else
            { Response.Redirect("results_past_care.aspx"); }

        }
        protected void lbtnSavedSearch_KneeMRI_Click(object sender, EventArgs e)
        {
            ClearSessionEntryInfo();

            ThisSession.ServiceName = "MRI - Knee, ankle, leg, foot, hip (lower extremity)";
            ClearSessionEntryInfo();

            //set the latitude and longitude if they changed locations.
            //SetLatLong();

            //go to results
            Response.Redirect("results_care.aspx");

        }
        protected void lbtnSelectSpecialty_Click(object sender, EventArgs e)
        {
            //clear any note fields
            //ClearNoteFields(); ClearDropDowns(); ClearSessionEntryInfo();

            //Which specialty did they select?
            LinkButton lbtnSelectSpecialty = (LinkButton)sender;

            string specialty = lbtnSelectSpecialty.Text;

            ThisSession.ServiceName = "Office visit - For new patient";
            ThisSession.Specialty = specialty;
            ThisSession.SpecialtyID = int.Parse(lbtnSelectSpecialty.CommandArgument);

            //set the latitude and longitude if they changed locations.
            //SetLatLong();

            //go to results
            //  lam, 20130319, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
            //Response.Redirect("results_specialty.aspx");
            Response.Redirect("results_care.aspx");

        }

        private void SetupFamilyMemberSection()
        {
            GetPastCareRX gpcrx = new GetPastCareRX(ThisSession.EmployeeID, ThisSession.CCHID);
            if (!gpcrx.HasErrors)
            {
                if (gpcrx.FamilyMedTable.Rows.Count > 0)
                {
                    ucFindRx.FamilyMedDataTable = gpcrx.FamilyMedTable;
                    ucFindRx.SetupFamilyMembers();
                }
                if (gpcrx.SaveTable.Rows.Count > 0)
                {
                    if (gpcrx.SaveTotal != "$0")
                    {
                        abRX.SaveTotal = gpcrx.SaveTotal;
                    }
                    else
                    {
                        abRX.Visible = false;
                    }
                }
            }
            else
            {
                abRX.Visible = false;
            }
        }

        protected void MakeVisible(ThisSession.AvailableTab at)
        {
            //Setup our default settings for the new CSS Classes the control will receive
            String inactiveTab = "ui-state-default ui-corner-all",
                activeTab = "ui-state-default ui-corner-top ui-tabs-selected ui-state-active";

            //Set everything to inactive and hidden
            liSavingsChoice.Attributes["class"] =
                liFindAService.Attributes["class"] =
                liFindRx.Attributes["class"] =
                liFindADoc.Attributes["class"] =
                liPastCare.Attributes["class"] = inactiveTab;
            dashboard.Visible =
                tabcare.Visible =
                tabrx.Visible =
                tabdoc.Visible =
                tabpast.Visible = false;
                
            //Make visible the panel we need and add the active class to only to the tab we want
            switch (at)
            {
                case ThisSession.AvailableTab.SavingsChoiceDashboard:
                    ThisSession.SelectedTab = ThisSession.AvailableTab.SavingsChoiceDashboard;
                    liSavingsChoice.Attributes["class"] = activeTab;
                    dashboard.Visible = true;
                    break;
                case ThisSession.AvailableTab.FindAService:
                    ThisSession.SelectedTab = ThisSession.AvailableTab.FindAService;
                    liFindAService.Attributes["class"] = activeTab;
                    tabcare.Visible = true;
                    break;
                case ThisSession.AvailableTab.FindRx:
                    ThisSession.SelectedTab = ThisSession.AvailableTab.FindRx;
                    liFindRx.Attributes["class"] = activeTab;
                    tabrx.Visible = true;
                    break;
                case ThisSession.AvailableTab.FindADoc:
                    ThisSession.SelectedTab = ThisSession.AvailableTab.FindADoc;
                    liFindADoc.Attributes["class"] = activeTab;
                    tabdoc.Visible = true;
                    break;
                case ThisSession.AvailableTab.FindPastCare:
                    ThisSession.SelectedTab = ThisSession.AvailableTab.FindPastCare;
                    liPastCare.Attributes["class"] = activeTab;
                    tabpast.Visible = true;
                    break;
            }
        }
        protected void ChangeTab(object sender, EventArgs e)
        {
            switch (((LinkButton)sender).ID)
            {
                case "lbtnDashboard":
                    MakeVisible(ThisSession.AvailableTab.SavingsChoiceDashboard);
                    break;
                case "lbtnFindAService":
                    MakeVisible(ThisSession.AvailableTab.FindAService);
                    break;
                case "lbtnFindRX":
                    MakeVisible(ThisSession.AvailableTab.FindRx);
                    break;
                case "lbtnFindADoc":
                    MakeVisible(ThisSession.AvailableTab.FindADoc);
                    break;
                case "lbtnPastCare":
                    using (CreateAuditTrail cat = new CreateAuditTrail())
                    {
                        cat.CCHID = ThisSession.CCHID;
                        cat.SessionID = HttpContext.Current.Session.SessionID;
                        cat.Domain = Request.Url.Host;
                        cat.Latitude = double.Parse(ThisSession.PatientLatitude);
                        cat.Longitude = double.Parse(ThisSession.PatientLongitude);
                        cat.SearchType = "ClickedPastCareTab";
                        cat.PostData();
                    }
                    MakeVisible(ThisSession.AvailableTab.FindPastCare);
                    break;
            }
        }
    }
}