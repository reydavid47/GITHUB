using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Security.Application;

public partial class SavingsChoice_switch : System.Web.UI.Page
{
    protected string SCSwitch_category = Encoder.HtmlEncode( HttpContext.Current.Request.QueryString["scswitch_category"] );
    
    //add new categories for switch steps here, URL passed categories -- required for time being
    protected string SCSwitch_options = "Lab, Imaging, MVP, Rx";

    private void loadSwitchStepPanel() {
        switch (SCSwitch_category.ToLower()) { 
            case "imaging":
                //this is fix for name change, points to page based on category.  was radiology->imaging
                SCSwitch_category = "Radiology";
                break;
            case "rx":
                SCSwitch_category = "PrescriptionDrugs";
                break;
        }
        //if (SCSwitch_category == "Imaging") {
        //    //this is fix for name change, points to page based on category.  was radiology->imaging
        //    SCSwitch_category = "Radiology";
        //}
        Control switchContent = Page.LoadControl(ResolveUrl("~/Controls/SavingsChoiceSwitchSteps_" + SCSwitch_category.ToLower()+ ".ascx"));
        //get category, load user content based on category : filename requires category
        if (switchContent != null)
        {
            switchContentWrapper.Controls.Clear();
            switchContentWrapper.Controls.Add(switchContent);
        }
    }

    protected void page_init(object sender, EventArgs e) {
        //confirming incoming params match categories
        if ( SCSwitch_options.ToLower().IndexOf(SCSwitch_category.ToLower()) >= 0) {
            //load category switch step -- Lab, Radiology, Etc
            loadSwitchStepPanel();            
        }        
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}