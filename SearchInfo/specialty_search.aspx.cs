using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.SearchInfo
{
    public partial class specialty_search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(ThisSession.SpecialtyNetworkText))
                abSpecialtyNetworks.StaticText = ThisSession.SpecialtyNetworkText;
            else
                abSpecialtyNetworks.Visible = false;

            if (!Page.IsPostBack)
            {

                if (ThisSession.ServiceEntered != string.Empty)
                    lblServiceName.Text = Encoder.HtmlEncode( ThisSession.ServiceEntered );
                else
                    lblServiceName.Text = Encoder.HtmlEncode( ThisSession.ServiceName );

                // If they alread entered what type of office visit they want, don't have they reselect
                if (ThisSession.ServiceEntered.ToLower().Contains("-"))
                {
                    //dont need to select visit type, and don't need to show step number image.
                    imgStep1.Visible = false;
                }
                else
                {

                    //Office Visit Types
                    ListItem li = new ListItem();
                    li.Text = @"<b>New patient</b> - what does it cost to see this provider for the first time?";
                    li.Value = "Office Visit - For new patient";
                    rblOfficeVisitTypes.Items.Add(li);
                    li = new ListItem();
                    li.Text = @"<b>Established patient</b> - what does it cost after the first visit?";
                    li.Value = "Office Visit - For established patient";
                    rblOfficeVisitTypes.Items.Add(li);
                    li = new ListItem();
                    li.Text = @"<b>Preventive visit</b> - what does it cost for a physical examination?";
                    li.Value = "Office Visit - For preventive care";//TODO: NOT SURE ABOUT THIS ONE.
                    rblOfficeVisitTypes.Items.Add(li);
                }
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void ddlSpecialty_DataBound(object sender, EventArgs e)
        {
            ddlSpecialty.Items.Insert(0, "Select from list:");

        }
        protected void ddlSpecialty_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TODO: This will probably actually be table driven. But last minute change request from Syam/Peter:
            //          Do not list preventive care IF they selected ob/gyn
            if (int.Parse(ddlSpecialty.SelectedValue) == 17)
            {
                rblOfficeVisitTypes.Items.Clear();

                ListItem li = new ListItem();
                li.Text = @"<b>New patient</b> - what does it cost to see this provider for the first time?";
                li.Value = "Office Visit - For new patient";
                rblOfficeVisitTypes.Items.Add(li);
                li = new ListItem();
                li.Text = @"<b>Established patient</b> - what does it cost after the first visit?";
                li.Value = "Office Visit - For established patient";
                rblOfficeVisitTypes.Items.Add(li);
                li = new ListItem();
                li.Text = @"<b>Physical exam</b> - what does it cost for a physical exam?";
                li.Value = "Office Visit - For new patient";
                rblOfficeVisitTypes.Items.Add(li);
            }
            // If they alread entered what type of office visit they want, don't have they reselect
            if (ThisSession.ServiceEntered.ToLower().Contains("-"))
            {
                //Just go to results.
                ThisSession.ServiceName = lblServiceName.Text;
                goToResults();
            }
            else
            {
                imgStep2.Visible = true;
                lblSelectVisitType.Visible = true;
                rblOfficeVisitTypes.Visible = true;
            }
        }
        protected void rblOfficeVisitTypes_SelectedIndexChanged(object sender, EventArgs e)
        {

            ThisSession.ServiceName = rblOfficeVisitTypes.SelectedItem.Value;

            goToResults();
        }
        private void goToResults()
        {
            ThisSession.Specialty = ddlSpecialty.SelectedItem.Text; ;
            ThisSession.SpecialtyID = int.Parse(ddlSpecialty.SelectedValue);
            ThisSession.ServiceEnteredFrom = "DropDowns";

            //go to results
            //  lam, 20130319, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
            //Response.Redirect("results_specialty.aspx#tabcare");
            Response.Redirect("results_care.aspx");
            //  lam, 20130311, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
            //Response.Redirect("results_specialty.aspx#tabdoc"); 
        }

    }
}