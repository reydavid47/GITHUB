using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.SearchInfo
{
    public partial class LargerMap : System.Web.UI.Page
    {
        private String Latitude { get { return this.ViewState["Latitude"].ToString(); } set { this.ViewState["Latitude"] = value; } }
        private String Longitude { get { return this.ViewState["Longitude"].ToString(); } set { this.ViewState["Longitude"] = value; } }
        private String PostBackControl { get { return Request.Params["__EVENTTARGET"].ToString(); } }
        private String PostBackArgument { get { return Request.Params["__EVENTARGUMENT"].ToString(); } }
        private String[] PostBackLatLng { get { return (PostBackControl == "Geocoder" ? PostBackArgument.Split('|') : null); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Latitude = ThisSession.FacilityLatitude;
                Longitude = ThisSession.FacilityLongitude;
            }
            else if (PostBackControl == "Geocoder")
            {

            }
            map_canvas.Attributes.Add("pLat", Latitude);
            map_canvas.Attributes.Add("pLng", Longitude);
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
    }
}