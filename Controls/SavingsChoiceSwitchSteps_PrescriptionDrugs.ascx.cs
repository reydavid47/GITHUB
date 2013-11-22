using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace ClearCostWeb.SavingsChoice
{
    public partial class SavingsChoiceSwitchSteps_PrescriptionDrugs : System.Web.UI.UserControl
    {
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
                //return 91266;
                //if ((request.querystring["cchid"] ?? "").trim() != string.empty)
                //    return int.parse(request.querystring["cchid"]);
                //else
                //    if (debugger.isattached)
                //        return 110733;
                //    else
                return ThisSession.CCHID;
            }
        }
        //holds all category questions Lab, Radiology, etc
        private DataTable Questions;
        //holds question count, 4, etc
        private int QuestionCount;
        //--requires page customization--
        private string category = "MVP",
                         goToDashboard = "Return to Savings Choice Dashboard",
                         gotoContinue = "Continue",
                         dashboardURL = "/SearchInfo/Search.aspx";
        //db related data being stored
        protected int dbSessionID;
        private int currentStep = Convert.ToInt16(HttpContext.Current.Request.QueryString["scswitch_step"].ToString());
        protected void setupGoBackLink()
        {
            if (Request.UrlReferrer != null)
            {
                goBack.NavigateUrl = Request.UrlReferrer.ToString();
            }
            else
            {
                goBack.Text = this.goToDashboard;
                goBack.NavigateUrl = ResolveUrl(this.dashboardURL);
            }
        }
        //display panel based on current step 
        protected void setupDisplayStep()
        {
            Control displayPanel = FindControl("step" + currentStep);
            displayPanel.Visible = true;
        }
        protected void setupQuestions()
        {
            SC_GetOptDecisionList godl = new SC_GetOptDecisionList();
            {
                godl.CatID = this.category;
                godl.GetData();

                if (godl.Tables.Count >= 1 && godl.Tables[0].Rows.Count > 0)
                {
                    this.Questions = godl.Tables[0];
                    DataTable tempTable = Questions.Clone();
                    tempTable.ImportRow(Questions.Rows[Questions.Rows.Count - 1]);
                    this.QuestionCount = Convert.ToInt16(tempTable.Rows[0][2]);
                }

            }
        }
        //loads all related questions per matching current step to db stepnum
        protected void loadQuestions() {
            Repeater displayQuestions = (Repeater)FindControl("step"+currentStep+"DisplayQuestions");
            DataTable currentStepQuestions = new DataTable();
            currentStepQuestions.Clear();
            currentStepQuestions.Columns.Add("stepnum");
            currentStepQuestions.Columns.Add("Optiondesc");
            currentStepQuestions.Columns.Add("decisionid");
            currentStepQuestions.Columns.Add("decisionvalue");
            foreach(DataRow row in Questions.Rows){
                if (Convert.ToInt16(row["stepnum"]) == currentStep) {
                    object[] rowValues = { 
                                             row["stepnum"],
                                             row["Optiondesc"],
                                             row["decisionid"], 
                                             row["decisionvalue"]                                             
                                         };
                    currentStepQuestions.Rows.Add(rowValues);
                }
            }
            displayQuestions.DataSource = currentStepQuestions;
            displayQuestions.DataBind();
        }
        //loads single question matching question number to stepnum in db
        private void loadQuestion(int QuestionNumber) {
            Repeater displayQuestions = (Repeater)FindControl("step" + QuestionNumber + "DisplayQuestions");
            DataTable currentStepQuestions = new DataTable();
            currentStepQuestions.Clear();
            currentStepQuestions.Columns.Add("stepnum");
            currentStepQuestions.Columns.Add("Optiondesc");
            currentStepQuestions.Columns.Add("decisionid");
            currentStepQuestions.Columns.Add("decisionvalue");
            foreach (DataRow row in Questions.Rows)
            {
                if (Convert.ToInt16(row["stepnum"]) == QuestionNumber)
                {
                    object[] rowValues = { 
                                             row["stepnum"],
                                             row["Optiondesc"],
                                             row["decisionid"], 
                                             row["decisionvalue"]                                             
                                         };
                    currentStepQuestions.Rows.Add(rowValues);
                }
            }
            displayQuestions.DataSource = currentStepQuestions;
            displayQuestions.DataBind();            
        }
        private void loadFairPriceProviderList()
        {
            Repeater displayContent = (Repeater)FindControl("step2DisplayContent");
            DataTable UsedProviders;
            SC_GetFairPriceAlternatives fpa = new SC_GetFairPriceAlternatives();
            {
                fpa.CCHID = this.PrimaryCCHID;
                fpa.Category = category;
                fpa.SubCategory = DBNull.Value;
                fpa.RowMax = 10;
                fpa.GetData();

                if (fpa.Tables.Count >= 1 && fpa.Tables[0].Rows.Count > 0)
                {
                    UsedProviders = fpa.Tables[0];
                    displayContent.DataSource = UsedProviders;
                    displayContent.DataBind(); 
                }
            }                  
        }
        private void loadSubcategories()
        {
            Repeater subCategoriesList = (Repeater)FindControl("medicalSubcategories");
            SC_GetSubCategories gsc = new SC_GetSubCategories(); {
                gsc.Subcategory = category;
                gsc.GetData();
                if (gsc.Tables.Count >= 1 && gsc.Tables[0].Rows.Count > 0) {
                    subCategoriesList.DataSource = gsc.Tables[0];
                    subCategoriesList.DataBind();
                }
            }
        }
        //--requires page customization--
        private void loadAdditionQuestions() { 
            //addional case where we want to load additional questions per page
            switch (currentStep) {
                case 1:
                    loadQuestion(2);
                    break;
                case 2:
                    loadQuestion(3);
                    break;
                case 3:
                    loadQuestion(4);
                    loadQuestion(5);
                    break;
                case 4:
                    loadQuestion(6);
                    break;
            }
        }
        //--requires page customization--
        private void loadAdditionalContent() {
            //addional case where we want to load additional questions per page
            switch (currentStep)
            {
                case 1:
                    loadSubcategories();
                    break;
                case 2:
                    loadFairPriceProviderList();
                    break;
            }
        }
        //sets up continue url
        private string nextURL() {
            string goToUrl;
            string currentURL = HttpContext.Current.Request.Url.AbsoluteUri;
            if (currentStep >= QuestionCount)
            {
                goToUrl = ResolveUrl(dashboardURL);                 
            }
            else {
                goToUrl = currentURL.Replace("scswitch_step=" + currentStep, "scswitch_step=" + (currentStep + 1));
            }
            
            return goToUrl;
        }
        //setups continue link
        protected void setupContinueLink() {
            if (currentStep >= QuestionCount)
            {
                switchContinue.NavigateUrl = dashboardURL;
                switchContinue.Text = goToDashboard;
                switchContinue.CssClass = "readmore b completeSwitchStep";
            }
            else {
                switchContinue.NavigateUrl = nextURL();
                switchContinue.Text = gotoContinue;
            }
        }
        //preps all ui needed
        protected void prepUI()
        {
            setupDisplayStep();
            setupGoBackLink();
            loadAdditionalContent();
        }
        //preps all data needed
        protected void prepData()
        {
            setupQuestions();
            loadQuestions();
            loadAdditionQuestions();
            setupContinueLink();
        }
        //manages switch step - data, display, db session init, etc
        private void manageSwitchStep() {
            switch (currentStep) { 
                // init db session
                case 1:
                    SC_ssInitSession ssInit = new SC_ssInitSession();
                    ssInit.CCHID = this.PrimaryCCHID;
                    ssInit.Category = this.category;
                    ssInit.GetData();
                    if (ssInit.dbSessionData.Rows.Count > 0)
                    {                        
                        dbSessionID = Convert.ToInt32(ssInit.dbSessionData.Rows[0]["mbrSessID"]);
                        HttpContext.Current.Session["dbSwitchStepSessionID"] = dbSessionID;
                    }
                    else {
                        dbSessionID = 0;
                        HttpContext.Current.Session["dbSwitchStepSessionID"] = dbSessionID;
                    }    
                    break;
                default:
                    dbSessionID = Convert.ToInt32(HttpContext.Current.Session["dbSwitchStepSessionID"]);
                    break;
            }
        }
        protected void page_init(object sender, EventArgs e)
        {
            prepUI();
            prepData();
            manageSwitchStep();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }       
    }
}