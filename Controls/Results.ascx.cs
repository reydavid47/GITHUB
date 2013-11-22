using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace ClearCostWeb.Controls
{
    public partial class Results : System.Web.UI.UserControl, ICallbackEventHandler
    {
        #region Private Variables
        private DataTable resultsData
        {
            get
            {
                return (ViewState["ResultsData"] == null ? new DataTable("Empty") : ((DataTable)ViewState["ResultsData"]));
            }
            set
            {
                ViewState["ResultsData"] = value;
            }
        }
        private DataTable learnMoreTable
        {
            get
            {
                return (ViewState["learnMoreTable"] == null ? new DataTable("Empty") : ((DataTable)ViewState["learnMoreTable"]));
            }
            set
            {
                ViewState["learnMoreTable"] = value;
            }
        }
        private DataTable resultBuffer
        {
            get
            {
                return ((DataTable)HttpContext.Current.Session["resultBuffer"]);
            }
            set
            {
                HttpContext.Current.Session["resultBuffer"] = value;
            }
        }
        private DataTable displayedResults
        {
            get
            {
                return ((DataTable)HttpContext.Current.Session["displayedResults"]);
            }
            set
            {
                HttpContext.Current.Session["displayedResults"] = value;
            }
        }
        private Boolean IsMultiLab
        {
            get
            {
                return (ViewState["IsMultiLab"] == null ? false : ((Boolean)ViewState["IsMultiLab"]));
            }
            set
            {
                ViewState["IsMultiLab"] = value;
            }
        }
        private Boolean IsThin
        {
            get
            {
                return (ViewState["IsThin"] == null ? false : ((Boolean)ViewState["IsThin"]));
            }
            set
            {
                ViewState["IsThin"] = value;
            }
        }
        #endregion

        #region Callback Parts
        ResultsInfo resOut;
        public void RaiseCallbackEvent(String eventArgument)
        {
            try
            {
                //Validate that an appropriate control caused the callback for security reasons
                Page.ClientScript.ValidateEvent(this.UniqueID);

                //Create a serializer to handle the client side info
                JavaScriptSerializer js = new JavaScriptSerializer();

                //Serialize out the info from the callback so we can work with it
                CallbackActionRequest car = js.Deserialize<CallbackActionRequest>(eventArgument);
                Boolean byPassDistances = car.LastResult;
                if (car.ToDo == "ChangeSort") { byPassDistances = false; }

                //Perform the intended action
                car.Act();

                //Setup the distances we'll want to reprort back using Google
                if (!byPassDistances)
                    car.GetDistances();

                //Sort the results including the buffer ontop of what the database already sort as we now have google distance data            
                if (!byPassDistances)
                    car.SortResults();
                else
                    car.SortFullResults();

                //Set the resOut object to an instance of ResultsInfo for serialization
                resOut = car.RetrieveResultsInfo();
            }
            catch (Exception ex)
            { }
        }
        public string GetCallbackResult()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (resOut == null) { resOut = new ResultsInfo(); }
            return js.Serialize(resOut);
        }
        #endregion

        #region Properties
        public DataTable ResultsData { get { return resultsData; } set { resultsData = value; } }

        private String PostBackControl { get { return Request.Params["__EVENTTARGET"].ToString(); } }
        private String PostBackArgument { get { return Request.Params["__EVENTARGUMENT"].ToString(); } }
        private String[] PostBackLatLng { get { return (PostBackControl == "Geocoder" ? PostBackArgument.Split('|') : null); } }
        #endregion

        #region GUI Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            //Setup the callback javascript we'll need to drive the lazy load
            ltlCallbackScript.Text = @"<script type=""text/javascript"">function CallServer(arg, context) { " + Page.ClientScript.GetCallbackEventReference(this, "arg", "ReceiveServerData", "context") + "; }</script>";
            if (Page.IsPostBack)
            {
                IsThin = false;
                //If we've clicked on a result...
                if (PostBackControl == "Result")
                {
                    FormsAuthenticationTicket tk = FormsAuthentication.Decrypt(PostBackArgument);
                    string[] args = tk.Name.Split('|');
                    if (ThisSession.ChosenLabs == null)
                    {
                        ThisSession.PracticeName = args[0];
                        ThisSession.PracticeNPI = args[1];
                        ThisSession.FacilityDistance = args[2];
                        ThisSession.OrganizationLocationID = Int32.Parse(args[3].ToString());
                    }
                    else
                    {
                        ThisSession.PracticeName = args[0];
                        ThisSession.ChosenLabID = args[1];
                        ThisSession.FacilityDistance = args[2];
                        ThisSession.ChosenLabLocationID = args[3];
                    }

                    Response.Redirect("results_care_detail.aspx");
                }
            }
            else
                IsMultiLab = (ThisSession.ChosenLabs != null);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            Page.ClientScript.RegisterForEventValidation(this.UniqueID);
            base.Render(writer);
        }
        #endregion

        protected class ResultsInfo
        {
            public int ResultCount;
            public Boolean EndOfResults;
            public String[] LearnMore;
            public Result[] Results;
        }
        protected class Result
        {
            private Boolean sYC;
            private int Index;
            public string Name;
            public string City;
            public string Distance;
            public string RangeMin;
            public string RangeMax;
            public string YourCostMin;
            public string YourCostMax;
            public Boolean isFairPrice;
            public Boolean isHGRecognized;
            public string LocID;
            public string Lat;
            public string Lng;
            public string HTML
            {
                get
                {
                    //Table Row Definition
                    String htmlOut = string.Format("<tr class=\"resultRow graydiv{0}{1}\">", ((this.Index % 2) == 0 ? "" : " roweven"), ((this.Index == 1) ? " graytop" : ""));
                    //Result Facility/Name and City Definition
                    htmlOut += string.Format("<td class=\"tdfirst graydiv PRAC\" style=\"width:40%;\"><a id=\"Result{2}\" href=\"javascript:void();\" onclick=\"SelectResult(this.id);\"><div class=\"readmore result\">{0}</div></a><div style=\"margin-left: 19px;\" >{1}</div></td>",
                        this.Name,
                        this.City,
                        this.Index);
                    //Result Distance Definition
                    htmlOut += string.Format("<td class=\"graydiv DIST\" style=\"white-space: nowrap; width:" + (this.sYC ? "10%" : "16%") + ";\"><div id=\"dvDistance\" style=\"text-align:center;\"><span>{0}</span></div></td>",
                        this.Distance);
                    //Result Total Estimated Cost
                    htmlOut += string.Format("<td class=\"graydiv EC\" style=\"width:" + (this.sYC ? "10%" : "16%") + ";\"><div class=\"totalCostRow\"><b class=\"alignright\">{0:c0}</b><b class=\"dashcol\">-</b><b class=\"alignleft\">{1:c0}</b></div></td>",
                        decimal.Parse(this.RangeMin),
                        decimal.Parse(this.RangeMax));
                    //Result Your Cost  NOTE: HIDDEN BY DEFAULT
                    if (this.sYC)
                        htmlOut += string.Format("<td class=\"graydiv YC\" style=\"width: 10%;\"><div class=\"YourCostRow\"><b class=\"alignright\">{0:c0}</b><b class=\"dashcol\">-</b><b class=\"alignleft\">{1:c0}</b></div></td>",
                            decimal.Parse(this.YourCostMin),
                            decimal.Parse(this.YourCostMax));
                    else
                        htmlOut += string.Format("<td class=\"graydiv YC\" style=\"width: 0%;Display: none;\"><div class=\"YourCostRow\"><b class=\"alignright\">{0:c0}</b><b class=\"dashcol\">-</b><b class=\"alignleft\">{1:c0}</b></div></td>",
                            decimal.Parse(this.YourCostMin),
                            decimal.Parse(this.YourCostMax));
                    //Result Fair Price
                    htmlOut += string.Format("<td class=\"tdcheck graydiv FP\" style=\"width:10%;\"><img src=\"../Images/{0}\" alt=\"FairPrice?\" style=\"width:23px; height:23px; border-width:0px;\" /></td>",
                        (this.isFairPrice ? "check_green.png" : "s.gif"));
                    //Result Healthgrades
                    //htmlOut += string.Format("<td class=\"graydiv HG\" style=\"width: 17%;\"><img src=\"../Images/{0}\" alt=\"Health Grades Recognized?\" style=\"width: 23px; height: 23px; border-width: 0px;\" /></td>", 
                    //    (this.isHGRecognized ? "check_purple.png" : "s.gif"));
                    if (Convert.ToInt16(this.HGDocCount) == 0)
                    {
                        htmlOut += "<td class=\"tdcheck graydiv HG\" style=\"width:18%;\">N/A</td>";
                    }
                    else
                    {
                        htmlOut += string.Format("<td class=\"tdcheck graydiv HG\" style=\"width:18%;\">{0}/{1} physicians</td>",
                            this.HGRecognizedDocCount,
                            this.HGDocCount);
                    }
                    htmlOut += string.Format("</tr>");
                    return htmlOut;
                }
            }
            public string InfoHTML
            {
                get
                {
                    String htmlOut = "<p class=\"smaller infoWin\" style=\"width:220px;\">";
                    htmlOut += String.Format("<a id=\"Result{1}\" class=\"readmore\" href=\"javascript:void();\" onclick=\"SelectResult(this.id);\">{0}</a><br />",
                        this.Name,
                        this.Index);
                    htmlOut += string.Format("Total cost: <b class=\"alignright\">{0:c0}</b><b class=\"dashcol\">-</b><b class=\"alignleft\">{1:c0}</b>",
                        decimal.Parse(this.RangeMin),
                        decimal.Parse(this.RangeMax));
                    if (this.isFairPrice)
                    {
                        htmlOut += "<br /><img src=\"../Images/check_green.png\" alt=\"X\" class=\"checkmark\" width=\"23\" height=\"23\" border=\"\" />&nbsp;Fair Price";
                    }
                    if (this.isHGRecognized)
                    {
                        htmlOut += "<br /><img src=\"../Images/check_purple.png\" alt=\"X\" class=\"checkmark\" width=\"23\" height=\"23\" border=\"\" />&nbsp;Healthgrades Recognized Provider";
                    }
                    htmlOut += "</p>";
                    return htmlOut;
                }
            }
            public string Nav;
            public string HGRecognizedDocCount = String.Empty;
            public String HGDocCount = String.Empty;
            public Result(int index, Boolean showYC)
            {
                this.Index = index;
                this.sYC = showYC;
            }
        }
        protected class EmptyResult : Result
        {
            public string Name = String.Empty;
            public string City = String.Empty;
            public string Distance = String.Empty;
            public string RangeMin = String.Empty;
            public string RangeMax = String.Empty;
            public Boolean isFairPrice = false;
            public Boolean isHGRecognized = false;
            public string LocID = String.Empty;
            public string Lat = String.Empty;
            public string Lng = String.Empty;
            public string HTML
            {
                get
                {
                    //Table Row Definition
                    String htmlOut = "<tr class=\"resultRow graydiv graytop\">";
                    htmlOut += "<td class=\"tdfirst graydiv\">We apologize but we were not able to locate any results within 50 miles of your area.</td>";
                    htmlOut += string.Format("</tr>");
                    return htmlOut;
                }
            }
            public string InfoHTML
            {
                get
                {
                    String htmlOut = "<p class=\"smaller infoWin\" style=\"width:220px;\">";
                    htmlOut += String.Empty;
                    htmlOut += "</p>";
                    return htmlOut;
                }
            }
            public string Nav = String.Empty;
            public string HGRecognizedDocCount = String.Empty;
            public String HGDocCount = String.Empty;
            public EmptyResult()
                : base(0, false)
            { }
        }
        protected class CallbackActionRequest : Results
        {
            #region Private Variables
            private DataTable tblResults = null;

            //Input vars
            private String toDo = String.Empty;
            private String resCount = String.Empty;
            private String lat = String.Empty;
            private String lng = String.Empty;
            private String sort = String.Empty;
            private String dir = String.Empty;
            private Boolean sYC = false;

            //Output vars
            private Boolean lastResult = false;
            #endregion

            #region Public Properties
            //Input properties
            public String ToDo { set { toDo = value; } get { return toDo; } }
            public String ResCount { set { resCount = value; } }
            public String Lat { set { lat = value; } }
            public String Lng { set { lng = value; } }
            public String Sort { set { sort = value; } }
            public String Direction { set { dir = value; } }
            public Boolean showYC { set { sYC = value; } }
            public Boolean EndOfResults { set { lastResult = value; } }

            //Output properties
            public Boolean LastResult { get { return lastResult; } }
            #endregion

            public void Act()
            {
                switch (this.toDo)
                {
                    case "GetFirstResults":
                        GetFirstResults();
                        break;
                    case "ChangeSort":
                        ChangeSortResults();
                        break;
                    case "GetNextResults":
                        GetNextResults();
                        break;
                    default:
                        break;
                }
            }
            public void GetFirstResults()
            {
                if (IsMultiLab)
                {
                    using (GetFacilitiesForMultiLab gffml = new GetFacilitiesForMultiLab())
                    {
                        //Set the parameters
                        using (DataView dv = new DataView(ThisSession.ChosenLabs))
                            gffml.ServiceList = dv.ToTable("ChosenLabs", true, new String[] { "ServiceID" });
                        gffml.Distance = "20";
                        gffml.Latitude = this.lat;
                        gffml.Longitude = this.lng;
                        gffml.OrderByField = "Distance";
                        gffml.OrderDirection = "ASC";

                        //Populate the object with data from the database
                        gffml.GetData();

                        //If we got anything back and had no errors...
                        if (!gffml.HasErrors && gffml.RowsBack > 0)
                        {
                            //If we've reached the end of the result set it even on the first pass
                            lastResult = gffml.ReachedEnd;

                            //Save the specific results for sorting and geocoding for when we leave the Using
                            tblResults = gffml.RawResults;

                            //Set the LearnMore table for use later
                            learnMoreTable = gffml.LearnMoreResults;
                        }
                    }
                }
                else
                {
                    if (!IsThin)
                    {
                        using (GetFacilitiesForService gffs = new GetFacilitiesForService())
                        {
                            //Set the parameters
                            gffs.ServiceID = ThisSession.ServiceID.ToString();
                            gffs.Latitude = this.lat;
                            gffs.Longitude = this.lng;
                            gffs.SpecialtyID = ThisSession.SpecialtyID.ToString();
                            gffs.Distance = "20";
                            gffs.OrderByField = "Distance";
                            gffs.OrderDirection = "ASC";
                            //gffs.MemberMedicalID = ThisSession.SubscriberMedicalID;
                            gffs.CCHID = ThisSession.CCHID;
                            gffs.UserID = Membership.GetUser().ProviderUserKey.ToString();

                            //Populate the object with data from the database
                            gffs.GetData();

                            //Is this thin data or no?
                            if (!gffs.DataIsThin)
                            {
                                //If we got anything back and had no errors...
                                if (!gffs.HasErrors && gffs.RowsBack > 0)
                                {
                                    //If we've reached the end of the result set it even on the first pass
                                    lastResult = gffs.ReachedEnd;

                                    //Save the specific results for sorting and geocoding for when we leave the Using
                                    tblResults = gffs.RawResults;

                                    //Set the LearnMore table for use later
                                    learnMoreTable = gffs.LearnMoreResults;
                                }
                            }
                            else
                            { IsThin = true; }
                        }
                    }
                    if (IsThin)
                    {
                        using (GetThinDataResults gtdr = new GetThinDataResults())
                        {
                            //Set the parameters
                            gtdr.ZipCode = ThisSession.PatientZipCode;
                            gtdr.ServiceID = ThisSession.ServiceID.ToString();
                            gtdr.Latitude = this.lat;
                            gtdr.Longitude = this.lng;
                            gtdr.SpecialtyId = ThisSession.SpecialtyID.ToString();

                            //Populate the object with data from the database
                            gtdr.GetData();

                            if (gtdr.HasErrors && gtdr.RowsBack > 0)
                            {
                                lastResult = gtdr.ReachedEnd;
                                tblResults = gtdr.RawResults;

                            }
                        }
                    }
                }
                //Prepare the result buffer to begin with a clean slate
                if (tblResults != null && tblResults.Rows.Count > 0)
                    if (resultBuffer == null)
                        resultBuffer = tblResults.Clone();
                    else
                        resultBuffer.Clear();
            }
            public void ChangeSortResults()
            {
                if (IsMultiLab)
                {
                    using (GetFacilitiesForMultiLab gffml = new GetFacilitiesForMultiLab())
                    {
                        //Set the parameters
                        using (DataView dv = new DataView(ThisSession.ChosenLabs))
                            gffml.ServiceList = dv.ToTable("ChosenLabs", true, new String[] { "ServiceID" });
                        gffml.Distance = "20";
                        gffml.Latitude = this.lat;
                        gffml.Longitude = this.lng;
                        gffml.OrderByField = sort;
                        gffml.OrderDirection = dir;

                        //Populate the object with data from the databdase
                        gffml.GetData();

                        //If we got anything back and had no errors...
                        if (!gffml.HasErrors && gffml.RowsBack > 0)
                        {
                            //If we've reached the end of the result set it even on the first pass
                            lastResult = gffml.ReachedEnd;

                            //Save the specific results for sorting and geocoding for when we leave the Using
                            tblResults = gffml.RawResults;
                        }
                    }
                }
                else
                {
                    using (GetFacilitiesForService gffs = new GetFacilitiesForService())
                    {
                        //Set the Parameters
                        gffs.ServiceID = ThisSession.ServiceID.ToString();
                        gffs.Latitude = this.lat;
                        gffs.Longitude = this.lng;
                        gffs.SpecialtyID = ThisSession.SpecialtyID.ToString();
                        gffs.Distance = "20";
                        gffs.OrderByField = sort;
                        gffs.OrderDirection = dir;
                        //gffs.MemberMedicalID = ThisSession.SubscriberMedicalID;
                        gffs.CCHID = ThisSession.CCHID;
                        gffs.UserID = Membership.GetUser().ProviderUserKey.ToString();

                        //Populate the object with data from the database
                        gffs.GetData();

                        //If we got anything back and had no errors...
                        if (!gffs.HasErrors && gffs.RowsBack > 0)
                        {
                            //If we've reached the end of the result set it even on the first pass
                            lastResult = gffs.ReachedEnd;

                            //Save the specific results for sorting and geocoding for when we leave the Using
                            tblResults = gffs.RawResults;
                        }
                    }
                }

                //Prepare the result buffer to begin with a clean slate
                if (resultBuffer == null)
                    resultBuffer = tblResults.Clone();
                else
                    resultBuffer.Clear();
            }
            public void GetNextResults()
            {
                if (IsMultiLab)
                {
                    using (GetFacilitiesForMultiLab gffml = new GetFacilitiesForMultiLab())
                    {
                        //Set the parameters
                        using (DataView dv = new DataView(ThisSession.ChosenLabs))
                            gffml.ServiceList = dv.ToTable("ChosenLabs", true, new String[] { "ServiceID" });
                        gffml.Distance = "20";
                        gffml.Latitude = this.lat;
                        gffml.Longitude = this.lng;
                        gffml.OrderByField = sort;
                        gffml.OrderDirection = dir;

                        //Populate the object with data from the database
                        Int32 calcFrom = Int32.Parse(this.resCount) + resultBuffer.Rows.Count + 1;
                        gffml.GetData(calcFrom, (calcFrom + 20));

                        //If we got anything back and had no errors...
                        if (!gffml.HasErrors && gffml.RowsBack > 0)
                        {
                            //If we've reached the end of the result, set it even on the first pass
                            lastResult = gffml.ReachedEnd;

                            //Save the specific results for sorting and geocoding for when we leave the Using
                            tblResults = gffml.RawResults;

                            //DO NOT CLEAR THE RESULT BUFFER HERE AS WE'LL NEED IT'S CONTENTS IN THIS SITUATION
                        }
                    }
                }
                else
                {
                    using (GetFacilitiesForService gffs = new GetFacilitiesForService())
                    {
                        //Set the Parameters
                        gffs.ServiceID = ThisSession.ServiceID.ToString();
                        gffs.Latitude = this.lat;
                        gffs.Longitude = this.lng;
                        gffs.SpecialtyID = ThisSession.SpecialtyID.ToString();
                        gffs.Distance = "20";
                        gffs.OrderByField = sort;
                        gffs.OrderDirection = dir;
                        //gffs.MemberMedicalID = ThisSession.SubscriberMedicalID;
                        gffs.CCHID = ThisSession.CCHID;
                        gffs.UserID = Membership.GetUser().ProviderUserKey.ToString();

                        //Populate the object with data from the database
                        Int32 calcFrom = Int32.Parse(this.resCount) + resultBuffer.Rows.Count + 1;
                        gffs.GetData(calcFrom, (calcFrom + 20));

                        //If we got anything back and had no errors...
                        if (!gffs.HasErrors && gffs.RowsBack > 0)
                        {
                            //If we've reached the end of the result, set it even on the first pass
                            lastResult = gffs.ReachedEnd;

                            //Save the specific results for sorting and geocoding for when we leave the Using
                            tblResults = gffs.RawResults;

                            //DO NOT CLEAR THE RESULT BUFFER HERE AS WE'LL NEED IT'S CONTENTS IN THIS SITUATION
                        }
                    }
                }
            }
            public void GetDistances()
            {
                if (tblResults != null && tblResults.Rows.Count > 0)
                {
                    //Add a new column to the table specific to Google so we don't step on any of the original data
                    DataColumn googleColumn = new DataColumn("GoogleDistance");
                    tblResults.Columns.Add(googleColumn);

                    //Setup the string we'll use as the origin Lat/Lng for Google
                    String originString = String.Format("{0},{1}", lat, lng);

                    //Setup a list of Lat/Lng destinations to loop through based off of the data from the databse
                    List<String> destinations = new List<String>();
                    for (Int32 i = 0; i < tblResults.Rows.Count; i++)
                        destinations.Add(String.Format("{0},{1}",
                            tblResults.Rows[i]["Latitude"].ToString(),
                            tblResults.Rows[i]["Longitude"].ToString()));

                    //Call the GoogleHelper with what we've put together and store the results in a String array
                    String[] distances = GoogleHelper.GetDistances(originString, destinations.ToArray());

                    //Loop through the rows in the data table again and update with what we received from Google
                    for (Int32 i = 0; i < tblResults.Rows.Count; i++)
                    {
                        if ((i + 1) <= distances.Length)
                        {
                            //If we received a distance for this destination...
                            if (distances[i] != "")
                            {
                                //Update our Distance with Google's for Display
                                tblResults.Rows[i]["Distance"] = distances[i].ToString();
                                //Update our Numeric Distance with Google's for sorting
                                tblResults.Rows[i]["NumericDistance"] = Convert.ToDouble(distances[i].Replace(" mi", String.Empty).Trim());
                            }
                            else
                            {
                                //Update our Distance with our calculated Numeric Distance as a fallback
                                tblResults.Rows[i]["Distance"] = String.Format("{0:##0.0} mi",
                                    tblResults.Rows[i]["NumericDistance"]);
                            }
                        }
                        else
                        {
                            //Update our Distance with our calculated Numeric Distance as a fallback
                            tblResults.Rows[i]["Distance"] = String.Format("{0:##0.0} mi",
                                tblResults.Rows[i]["NumericDistance"]);
                        }
                    }
                }
            }
            public void SortResults()
            {
                if (tblResults != null && tblResults.Rows.Count > 0)
                {
                    //If there are any buffered rows
                    if (resultBuffer.Rows.Count > 0)
                    {
                        //Add any rows that are in the buffer to the result set so they are included in the sort
                        //// Loop should suffice as the max should be 5
                        foreach (DataRow dr in resultBuffer.Rows)
                            tblResults.ImportRow(dr);

                        //Clear the buffer in preparation to receive new data rows
                        resultBuffer.Clear();
                    }

                    //Create a Data View of the Results + Buffer to Sort With
                    using (DataView dv = new DataView((tblResults == null ? displayedResults : tblResults)))
                    {
                        //Sort the results based off of what the callback is indicating
                        //NOTE: Because the boolean columns FairPrice and HGRecognize are True or False but treated as Text, we need to reverse the direction as F comes before T but True comes before False
                        //NOTE: 7-11-12 Sorting rules based off of a spreadsheet provided by ARTHI 
                        switch (sort)
                        {
                            case "Distance":
                                dv.Sort = String.Format("NumericDistance {0}, PracticeName ASC", dir);
                                break;
                            case "TotalCost":
                                dv.Sort = String.Format("RangeMin {0}, RangeMax ASC, FairPrice DESC, NumericDistance ASC, PracticeName ASC", dir);
                                break;
                            case "YourCost":
                                dv.Sort = String.Format("YourCostMin {0}, YourCostMax ASC, FairPrice DESC, NumericDistance ASC, PracticeName ASC", dir);
                                break;
                            case "FairPrice":
                                dv.Sort = String.Format("FairPrice {0}, RangeMin ASC, RangeMax ASC, NumericDistance ASC, PracticeName ASC", (dir == "ASC" ? "DESC" : "ASC"));
                                break;
                            case "PracticeName":
                                dv.Sort = String.Format("PracticeName {0}, NumericDistance ASC", dir);
                                break;
                            default:
                                dv.Sort = String.Format("NumericDistance {0}, PracticeName ASC", dir);
                                break;
                        }

                        //If we've recognized this to be the last of the data from the database...
                        if (lastResult)
                        {
                            //Add the whole thing to the results we're passing back as we won't need to come here again
                            //// Even if over 20; we'll no longer need to buffer as we wont get any more data
                            tblResults = dv.ToTable();

                            if (displayedResults == null) { displayedResults = tblResults.Clone(); }
                            displayedResults.Merge(tblResults);
                        }
                        else
                        {
                            //Cast the sorted dataview as a table to work with
                            using (DataTable dt = dv.ToTable())
                            {
                                //Clear our results to return as this will change after buffer processing
                                tblResults.Clear();

                                //Add the first 20 rows to the output results
                                for (Int32 i = 0; i < 20; i++)
                                    if (dt.Rows.Count > i)
                                        tblResults.ImportRow(dt.Rows[i]);
                                    else
                                        break;

                                //Add the last 5 rows to the buffer for next time around
                                for (Int32 i = 20; i < 25; i++)
                                    if (dt.Rows.Count > i)
                                        resultBuffer.ImportRow(dt.Rows[i]);
                                    else
                                        break;
                                if (displayedResults == null) { displayedResults = tblResults.Clone(); }
                                displayedResults.Merge(tblResults);
                            }
                        }
                    }
                }
            }
            public void SortFullResults()
            {
                using (DataView dv = new DataView(displayedResults))
                {
                    dv.Sort = String.Format("{0} {1}",
                        sort,
                        dir);
                    tblResults = dv.ToTable();
                }
            }
            public ResultsInfo RetrieveResultsInfo()
            {
                ResultsInfo ri = new ResultsInfo();
                ri.ResultCount = (tblResults == null ? 0 : tblResults.Rows.Count);
                List<Result> lResult = new List<Result>();

                if (ri.ResultCount > 0)
                {
                    if (!IsMultiLab)
                    {
                        ri.LearnMore = new String[1];
                        if (learnMoreTable.Rows.Count > 0 && learnMoreTable.Rows[0][0].ToString().Trim() != String.Empty)
                            ri.LearnMore[0] = learnMoreTable.Rows[0][0].ToString().Trim();
                        else
                            ri.LearnMore[0] = "Additional information about this service could not be retrieved at this time.";
                    }
                    else
                    {
                        ri.LearnMore = new String[(learnMoreTable.Rows.Count)];
                        if (learnMoreTable.Rows.Count > 0)
                            for (int i = 0; i < learnMoreTable.Rows.Count; i++)
                                if (learnMoreTable.Rows[i]["Description"].ToString().Trim() != String.Empty && learnMoreTable.Rows[i]["Description"].ToString() != "NULL")
                                    ri.LearnMore[i] = learnMoreTable.Rows[i]["Description"].ToString().Trim();
                                else
                                    ri.LearnMore[i] = "Additional information about this service could not be retrieved at this time.";
                    }

                    Result r = null;
                    DataRow dr = null;
                    for (Int32 i = 1; i <= ri.ResultCount; i++)
                    {
                        dr = tblResults.Rows[i - 1];

                        r = new Result(int.Parse(resCount) + i, this.sYC);
                        r.Name = dr["PracticeName"].ToString();
                        r.City = (tblResults.Columns.Contains("LocationCity") ? dr["LocationCity"].ToString() : dr["City"].ToString());
                        r.Distance = dr["Distance"].ToString();
                        r.RangeMin = dr["RangeMin"].ToString();
                        r.RangeMax = dr["RangeMax"].ToString();
                        r.YourCostMin = dr["YourCostMin"].ToString();
                        r.YourCostMax = dr["YourCostMax"].ToString();
                        r.LocID = dr["OrganizationLocationID"].ToString();

                        Int32 iFP = 0;
                        if (Int32.TryParse(dr["FairPrice"].ToString(), out iFP))
                            r.isFairPrice = Convert.ToBoolean(iFP);
                        else
                            r.isFairPrice = Boolean.Parse((dr["FairPrice"].ToString().Trim() == String.Empty ? "False" : dr["FairPrice"].ToString().Trim()));

                        if (dr["HGRecognized"].ToString().Trim() == "")
                            r.isHGRecognized = false;
                        else
                            r.isHGRecognized = Boolean.Parse((dr["HGRecognized"].ToString().Trim() == String.Empty ? "False" : dr["HGRecognized"].ToString().Trim()));

                        r.Lat = dr["Latitude"].ToString();
                        r.Lng = dr["Longitude"].ToString();

                        //Encrypt the Navigation Elements to prevent potential security risk(s)
                        FormsAuthenticationTicket tk = new FormsAuthenticationTicket(String.Format("{0}|{1}|{2}|{3}",
                            r.Name,
                            (ThisSession.ChosenLabs == null ? dr["NPI"].ToString() : dr["OrganizationID"].ToString()),
                            r.Distance,
                            r.LocID), false, 5);
                        r.Nav = FormsAuthentication.Encrypt(tk);
                        r.HGRecognizedDocCount = dr["HGRecognizedDocCount"].ToString();
                        r.HGDocCount = dr["HGDocCount"].ToString();

                        lResult.Add(r);
                    }

                    ri.EndOfResults = this.lastResult;
                }
                else
                {

                    lResult.Add(new EmptyResult());
                    ri.EndOfResults = true;
                }

                ri.Results = lResult.ToArray<Result>();
                return ri;
            }
        }
    }
}