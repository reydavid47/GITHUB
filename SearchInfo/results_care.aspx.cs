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
    public partial class results_care : System.Web.UI.Page
    {
        private ListItem[] sortOptions
        {
            get
            {
                return (ViewState["sortOptions"] == null ?
                    new ListItem[] { new ListItem(), new ListItem(), new ListItem(), new ListItem() } :
                    ((ListItem[])ViewState["sortOptions"]));
            }
            set { ViewState["sortOptions"] = value; }
        }

        protected Boolean IsCaesarsOphthalmology { get { return ((ThisSession.SpecialtyID == 18) && (ThisSession.EmployerID == "11")); } }  //  lam, 20130426, CI-51
        protected Boolean IsMammogram { get { return ((new List<String>(new String[] { "464", "214", "216", "465" })).Contains(ThisSession.ServiceID.ToString())); } }
        protected Boolean IsMentalHealth { get { return ((new List<String>(new String[] { "306", "307", "308" })).Contains(ThisSession.ServiceID.ToString())); } }  //  lam, 20130508, CI-144
        private Boolean IsMultiLab { get { return (ThisSession.ChosenLabs != null); } }
        protected String ShowSingleSearch { get { return (!IsMultiLab ? "" : "display:none;"); } }
        protected String ShowMultiSearch { get { return (!IsMultiLab ? "display:none;" : ""); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //rblSort.Items.CopyTo(sortOptions, 0);
                if (ThisSession.ChosenLabs == null)
                {
                    lblServiceName.Text = ThisSession.ServiceName.ToString();

                    // For office visits, add the specialty in the display name;
                    if (ThisSession.ServiceName.ToLower().StartsWith("office"))  //  lam, 20130319, MSF-177, need to add ToLower() and make "office" lower case for matching purpose
                        lblServiceName.Text = ThisSession.Specialty + ": " + lblServiceName.Text;

                    lblServiceName_MoreInfoTitle.Text = ThisSession.ServiceName.ToString();
                }
                else
                {
                    rptLabTests.DataSource = ThisSession.ChosenLabs;
                    rptLabTests.DataBind();
                }

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

                //Set Learn More
                if (IsMultiLab)
                {
                    //using (GetFacilitiesForMultiLab gffml = new GetFacilitiesForMultiLab())
                    //{
                    //    //Set the parameters
                    //    using (DataView dv = new DataView(ThisSession.ChosenLabs))
                    //        gffml.ServiceList = dv.ToTable("ChosenLabs", true, new String[] { "ServiceID" });
                    //    gffml.Distance = "20";
                    //    gffml.Latitude = ThisSession.PatientLatitude;
                    //    gffml.Longitude = ThisSession.PatientLongitude;
                    //    gffml.OrderByField = "Distance";
                    //    gffml.OrderDirection = "ASC";

                    //    //Populate the object with data from the database
                    //    gffml.GetData();

                    //    //If we got anything back and had no errors...
                    //    if (!gffml.HasErrors)
                    //    {
                    //        //Set the LearnMore table for use later
                    //        if (gffml.LearnMoreResults.Rows[0][0].ToString().Trim() != String.Empty)
                    //        {
                    //            lblServiceMoreInfo.Text = gffml.LearnMoreResults.Rows[0][0].ToString();
                    //        }
                    //        else
                    //        {
                    //            pServiceLM.Visible = false;
                    //        }
                    //    }
                    //}
                }
                else
                {
                    //using (GetFacilitiesForService gffs = new GetFacilitiesForService())
                    //{
                    //    //Set the parameters
                    //    gffs.ServiceID = ThisSession.ServiceID.ToString();
                    //    gffs.Latitude = ThisSession.PatientLatitude;
                    //    gffs.Longitude = ThisSession.PatientLongitude;
                    //    gffs.SpecialtyID = ThisSession.SpecialtyID.ToString();
                    //    gffs.Distance = "20";
                    //    gffs.OrderByField = "Distance";
                    //    gffs.OrderDirection = "ASC";
                    //    //gffs.MemberMedicalID = ThisSession.SubscriberMedicalID;
                    //    gffs.CCHID = ThisSession.CCHID;
                    //    gffs.UserID = Membership.GetUser().ProviderUserKey.ToString();
                    //    gffs.Domain = Request.Url.Host;
                    //    gffs.SessionID = HttpContext.Current.Session.SessionID;

                    //    //Populate the object with data from the database
                    //    gffs.GetData();

                    //    //Is this thin data or no?
                    //    if (!gffs.HasErrors)
                    //    {
                    //        //Set the LearnMore table for use later
                    //        if (gffs.LearnMoreResults.Rows[0][0].ToString().Trim() != String.Empty)
                    //        {
                    //            lblServiceMoreInfo.Text = gffs.LearnMoreResults.Rows[0][0].ToString();
                    //        }
                    //        else
                    //        {
                    //            pServiceLM.Visible = false;
                    //        }
                    //    }
                    //}
                }
                lblAllResult1DisclaimerText.Text = ThisSession.AllResult1DisclaimerText;  //  lam, 20130425, MSF-295 move disclaimer text to content manager
                lblAllResult2DisclaimerText.Text = ThisSession.AllResult2DisclaimerText;  //  lam, 20130425, MSF-295 move disclaimer text to content manager
                lblMentalHealthDisclaimerText.Text = ThisSession.MentalHealthDisclaimerText;  //  lam, 20130508, CI-144
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }

        private void SetCMSData(DataTable CMSData)
        {
            if (CMSData.Rows.Count > 0 && CMSData.Rows[0][0].ToString() != String.Empty)
            {
                String cmsRate = CMSData.Rows[0][0].ToString();
                Double dCMSRate = 0.0;
                if (Double.TryParse(cmsRate, out dCMSRate))
                {
                    //We received just one CMS value
                    lblCMSRate.Text = String.Format("{0:c0}", dCMSRate);
                }
                else
                {
                    //We received a CMS range
                    String[] range = cmsRate.Split('-');
                    Double cmsMin = Double.Parse(range[0].Trim());
                    Double cmsMax = Double.Parse(range[1].Trim());
                    lblCMSRate.Text = String.Format("{0:c0} - {1:c0}", cmsMin, cmsMax);
                }
                medRef.Visible = true;
                //rblSort.Items.RemoveAt(2);
                pnlThinPriceLegend.Visible = true;
            }
            else
            {
                medRef.Visible = false;

                //rblSort.Items.Clear();
                //rblSort.Items.AddRange(sortOptions);
                pnlThinPriceLegend.Visible = false;
            }
        }
    }
}
