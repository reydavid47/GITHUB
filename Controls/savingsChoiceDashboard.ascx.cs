using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace ClearCostWeb.Controls
{
    public partial class SavingsChoice_Dashboard : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // rld, this should only execute if the employer supports Savings Choice
            if (ThisSession.SavingsChoiceEnabled && ThisSession.ShowSCIQTab)
            {
                prepData();
                prepUI();
                loadEmployerContent();
            }
        }
        protected string    ratingLab, visitsLab, ratingColorLab, 
                            ratingRadiology, visitsRadiology, ratingColorRadiology,
                            ratingMedical, visitsMedical, ratingColorMedical,
                            ratingPrescription, visitsPrescription, ratingColorPrescription,
                            availableCategories;

        // protected string ratingLab, visitsLab, ratingRadiology, visitsRadiology, ratingMedical, visitsMedical, ratingPrescription, visitsPrescription, availableCategories;
        protected void prepData() {
            getAllThermomoterValues();
            ratingLab = getThermometerValue("Lab").ToString();
            visitsLab = getVisitCount("lab");
            ratingRadiology = getThermometerValue("Imaging").ToString();
            visitsRadiology = getVisitCount("imaging");
            ratingMedical = getThermometerValue("MVP").ToString();
            visitsMedical = getVisitCount("mvp");
            ratingPrescription = getThermometerValue("rx").ToString();
            visitsPrescription = getVisitCount("rx");
            getQuickSuggestions();
            getAvailableCategories();
            getMeasurementPeriod();
        }

        protected void prepUI() {
            setupMeasurementPeriod();
        }

        /*measurement period related*/
        DataTable measurementPeriod;
        protected void setupMeasurementPeriod() {
            if (measurementPeriod != null) {
                startMonth.Text = string.Format("{0:MMMM}", measurementPeriod.Rows[0]["StartDate"]);
                startYear.Text = string.Format("{0:yyyy}", measurementPeriod.Rows[0]["StartDate"]);
                endMonth.Text = string.Format("{0:MMMM}", measurementPeriod.Rows[0]["EndDate"]);
                endYear.Text = string.Format("{0:yyyy}", measurementPeriod.Rows[0]["EndDate"]);
            }
        }
        protected void getMeasurementPeriod() {
            using (SC_GetCurrentMeasurementPeriod mp = new SC_GetCurrentMeasurementPeriod())
            {
                mp.GetData();
                if (mp.Tables.Count > 0)
                {
                    measurementPeriod = mp.Tables[0];
                }
                else {
                    measurementPeriod = null;
                }
            }
        }

        /*available categories*/
        protected void getAvailableCategories() {
            using (sc_getAvailableCategories ac = new sc_getAvailableCategories()) {
                ac.GetData();
                List<string> dbCategories = new List<string>();
                foreach (DataRow dr in ac.AvailableCategories.Rows) {
                    dbCategories.Add(dr["ConfigValue"].ToString());
                }
                availableCategories = string.Join(",", dbCategories);
            }
        }
        /*quick suggestions*/
        public string QuickSuggestionText, QuickSuggestionPath;
        protected void getQuickSuggestions() { 
            using (SC_QuickSuggestions gs = new SC_QuickSuggestions()){
                gs.CCHID = this.PrimaryCCHID;
                gs.EmployerID = this.EmployerID;
                gs.GetData();
                if (gs.Tables.Count > 0) {
                    repeaterQuickSuggestions.DataSource = gs.Tables[0];
                    repeaterQuickSuggestions.DataBind();
                }                
            }
        }
        private string getQuickSuggestionURL(string category) {
            string returnURL = "";
            switch(category.ToLower()){
                case "rx":
                    returnURL = ResolveUrl("~/SavingsChoice/prescriptiondetails.aspx");
                    break;
                case "mvp":
                    returnURL = ResolveUrl("~/SavingsChoice/medicaldetails.aspx");
                    break;
                case "imaging":
                    returnURL = ResolveUrl("~/SavingsChoice/radiologydetails.aspx");
                    break;
                case "lab":
                    returnURL = ResolveUrl("~/SavingsChoice/labdetails.aspx");
                    break;
            };
            return returnURL;
        }
        protected String loadQuickSuggestions(RepeaterItem i) {
            DataRow dr = (i.DataItem as DataRowView).Row;
            QuickSuggestionText = dr["SuggestionText"].ToString();
            QuickSuggestionPath = getQuickSuggestionURL(dr["Category"].ToString());
            return "";
        }
        protected int EmployerID
        {
            get
            {
                //return 7;
                //if ((Request.QueryString["e"] ?? "").Trim() != string.Empty)
                //    return int.Parse(Request.QueryString["e"]);
                //else
                //    if (Debugger.IsAttached)
                //        return 7;
                //    else
                return int.Parse(ThisSession.EmployerID);
            }
        }
        protected int PrimaryCCHID
        {
            get
            {
                //return 77501;
                //if ((request.querystring["cchid"] ?? "").trim() != string.empty)
                //    return int.parse(request.querystring["cchid"]);
                //else
                //    if (debugger.isattached)
                //        return 110733;
                //    else
                return ThisSession.CCHID;
            }
        }
        protected string SessionID
        {
            get
            {
                return ThisSession.UserLogginID;
            }
        }

        DataTable dbThermometerValues;
        protected void getAllThermomoterValues() {
            sc_GetThermometerValue gtv = new sc_GetThermometerValue();
            gtv.CCHID = this.PrimaryCCHID;
            gtv.GetData();
            if (gtv.Tables.Count > 0)
            {
                dbThermometerValues = gtv.ThermometerValues;
            }
            else {
                dbThermometerValues = null;
            }
        }
        protected string getThermometerValue(string section) {
            string thermometorValue = "0";
            if(dbThermometerValues!=null){
                foreach (DataRow dr in dbThermometerValues.Rows) {
                    if (dr["category"].ToString().ToLower() == section.ToLower()) {
                        thermometorValue = dr["pctscore"].ToString();
                    }
                }
            }                                       
            
            return thermometorValue;
        }

        protected string getVisitCount(string section)
        {
            string visitCount = "0";
            if (dbThermometerValues != null)
            {
                foreach (DataRow dr in dbThermometerValues.Rows)
                {
                    if (dr["category"].ToString().ToLower() == section.ToLower())
                    {
                        visitCount = dr["total"].ToString();
                    }
                }
            }
            return visitCount;
        }

        protected string getRatingColor(string section) {
            string ratingColor = "white";
            if (dbThermometerValues != null) {
                foreach (DataRow dr in dbThermometerValues.Rows) { 
                    
                }
            }
            return ratingColor;
        }

        private void loadEmployerContent()
        {
            using (GetEmployerContent gec = new GetEmployerContent())
            {
                gec.EmployerID = Convert.ToInt32(ThisSession.EmployerID);
                gec.GetFrontEndData();
                userGoal.Text = gec.SCUserGoalToHit;
                startText.Text = gec.SCDashboardStartText;
            }
        }
    }
}