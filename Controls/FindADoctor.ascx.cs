using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.Controls
{
    public partial class FindADoctor : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(ThisSession.SpecialtyNetworkText))
                abSpecialtyNetworks.StaticText = ThisSession.SpecialtyNetworkText;
            else
                abSpecialtyNetworks.Visible = false;
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
            Response.Redirect("results_specialty.aspx");

        }
    }
}