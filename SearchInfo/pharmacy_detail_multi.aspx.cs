using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.SearchInfo
{
    public partial class pharmacy_detail_multi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                lblDistance.Text = "2.8";

                //Google work
                GenerateGoogleMap();

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
        //    GenerateGoogleMap();
        //}

        protected void GenerateGoogleMap()
        {
            //TODO: Will be database driven. Hard coded for now.
            ThisSession.FacilityLatitude = @"42.37744319999999";
            ThisSession.FacilityLongitude = @"-71.08967640000003";

            string facilityAddress = ThisSession.FacilityLatitude.ToString().Trim() +
                "," + ThisSession.FacilityLongitude.ToString().Trim();//ThisSession.FacilityAddressForGoogle;

            string patientAddress = ThisSession.PatientLatitude.ToString().Trim() +
                "," + ThisSession.PatientLongitude.ToString().Trim();//ThisSession.FacilityAddressForGoogle;

            //string JScript = "GenerateMapAndDirections('" + patientAddress + "','" + facilityAddress + "');";
            string JScript = "var tempStart = '" + patientAddress + "';var tempEnd = '" + facilityAddress + "';";

            ClientScriptManager CSManager = Page.ClientScript;
            CSManager.RegisterStartupScript(Page.GetType(), "GenerateMap", JScript, true);
        }
    }
}