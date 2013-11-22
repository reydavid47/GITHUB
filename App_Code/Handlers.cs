using System;
using System.Web;
using System.Web.SessionState;
using System.Web.Script.Serialization;
using System.Text;
using System.Collections.Generic;
using System.Data;

namespace ClearCostWeb.Handlers
{
    public struct QuerySettings
    {
        public String Latitude;
        public String Longitude;
        public Int32 FromRow;
        public Int32 ToRow;
        public Boolean LastResult;
        public String CurrentSort;
        public String CurrentDirection;
        public String Distance;
    }

    public class AutoCompleteResult
    {
        private List<String> results = new List<String>();
        public String[] Results { get { return results.ToArray(); } }
        public void AddMatch(string match)
        {
            this.results.Add(match.ToUpper());
        }
    }
    
    public class Results_Specialty : IHttpHandler, IReadOnlySessionState
    {
        public struct QuerySettings
        {
            public String Lat;
            public String Lng;
        }
        public struct ResultData
        {
            public String ServiceName;
            public String TaxID;
            public String NPI;
            public String PracticeName;
            public String ProviderName;
            public String RangeMin;
            public String RangeMax;
            public String YourCostMin;
            public String YourCostMax;
            public String Latitude;
            public String Longitude;
            public Int32 OrganizationLocationID;
            public String LocationAddress1;
            public String LocationCity;
            public String LocationState;
            public String LocationZip;
            public String LocationTelephone;
            public String RowNumber;
            public String Distance;
            public String NumericDistance;
            public Boolean FairPrice;
            public Int32 HGRecognized;
            public String LatLong;
            public Double HGOverallRating;
            public int HGPatientCount;
        }
        public class ResultsBack
        {
            public struct MainResults
            {
                public String[] TaxID;
                public String[] NPI;
                public String PracticeName;
                public String[] ProviderName;
                public String PracticeRangeMin;
                public String PracticeRangeMax;
                public String[] RangeMin;
                public String[] RangeMax;
                public String PracticeYourCostMin;
                public String PracticeYourCostMax;
                public String[] YourCostMin;
                public String[] YourCostMax;
                public String Latitude;
                public String Longitude;
                public Int32 OrganizationLocationID;
                public String LocationAddress1;
                public String LocationCity;
                public String LocationState;
                public String LocationZip;
                public String Distance;
                public String NumericDistance;
                public Boolean PracticeFairPrice;
                public Boolean[] FairPrice;
                public String PracticeHGRecognized;
                public Int32[] HGRecognized;
                public Double PracticeAvgRating;
                public Double[] HGOverallRating;
                public int[] HGPatientCount;
            }
            private List<MainResults> results = new List<MainResults>();
            public MainResults[] Results { get { return results.ToArray(); } }
            public void AddResult(ResultData r)
            {
                Boolean match = false;
                int indx = -1;
                foreach (MainResults MaRe in results)
                {
                    //match = (MaRe.Latitude == r.Latitude && MaRe.Longitude == r.Longitude);
                    match = (MaRe.OrganizationLocationID == r.OrganizationLocationID);
                    if (match)
                    {
                        indx = results.IndexOf(MaRe);
                        break;
                    }
                }
                if (!match)
                {
                    MainResults mr = new MainResults();
                    mr.TaxID = new String[] { r.TaxID };
                    mr.NPI = new String[] { r.NPI };
                    mr.PracticeName = r.PracticeName;
                    mr.ProviderName = new String[] { r.ProviderName };
                    mr.PracticeRangeMin = String.Format("{0:c0}", decimal.Parse(r.RangeMin));
                    mr.RangeMin = new String[] { r.RangeMin };
                    mr.PracticeRangeMax = String.Format("{0:c0}", decimal.Parse(r.RangeMax));
                    mr.RangeMax = new String[] { r.RangeMax };
                    mr.PracticeYourCostMin = String.Format("{0:c0}", decimal.Parse(r.YourCostMin));
                    mr.YourCostMin = new String[] { r.YourCostMin };
                    mr.PracticeYourCostMax = String.Format("{0:c0}", decimal.Parse(r.YourCostMax));
                    mr.YourCostMax = new String[] { r.YourCostMax };
                    mr.Latitude = r.Latitude;
                    mr.Longitude = r.Longitude;
                    mr.OrganizationLocationID = r.OrganizationLocationID;
                    mr.LocationAddress1 = r.LocationAddress1;
                    mr.LocationCity = r.LocationCity;
                    mr.LocationState = r.LocationState;
                    mr.LocationZip = r.LocationZip;
                    mr.Distance = r.Distance;
                    mr.NumericDistance = r.NumericDistance;
                    mr.PracticeFairPrice = r.FairPrice;
                    mr.FairPrice = new Boolean[] { r.FairPrice };
                    mr.HGRecognized = new Int32[] { r.HGRecognized };
                    switch (r.HGRecognized)
                    {
                        case -1:
                            mr.PracticeHGRecognized = "N/A";
                            break;
                        case 0:
                            mr.PracticeHGRecognized = "0/1 Physicians";
                            break;
                        case 1:
                            mr.PracticeHGRecognized = "1/1 Physicians";
                            break;
                        default:
                            mr.PracticeHGRecognized = "N/A";
                            break;
                    }
                    mr.PracticeAvgRating = r.HGOverallRating;
                    mr.HGOverallRating = new Double[] { r.HGOverallRating };
                    mr.HGPatientCount = new int[] { r.HGPatientCount };
                    results.Add(mr);
                }
                else
                {
                    MainResults mr = results[indx];

                    String[] s = new String[mr.TaxID.Length + 1];
                    mr.TaxID.CopyTo(s, 0);
                    s[s.Length - 1] = r.TaxID;
                    mr.TaxID = (String[])s.Clone();
                    mr.NPI.CopyTo(s, 0);
                    s[s.Length - 1] = r.NPI;
                    mr.NPI = (String[])s.Clone();
                    mr.ProviderName.CopyTo(s, 0);
                    s[s.Length - 1] = r.ProviderName;
                    mr.ProviderName = (String[])s.Clone();

                    mr.RangeMin.CopyTo(s, 0);
                    s[s.Length - 1] = r.RangeMin;
                    mr.RangeMin = (String[])s.Clone();
                    mr.PracticeRangeMin = String.Format("{0:c0}", ((double.Parse(mr.PracticeRangeMin.Replace("$", "")) + double.Parse(r.RangeMin)) / 2.0));
                    mr.RangeMax.CopyTo(s, 0);
                    s[s.Length - 1] = r.RangeMax;
                    mr.RangeMax = (String[])s.Clone();
                    mr.PracticeRangeMax = String.Format("{0:c0}", ((double.Parse(mr.PracticeRangeMax.Replace("$", "")) + double.Parse(r.RangeMax)) / 2.0));

                    mr.YourCostMin.CopyTo(s, 0);
                    s[s.Length - 1] = r.YourCostMin;
                    mr.YourCostMin = (String[])s.Clone();
                    mr.PracticeYourCostMin = String.Format("{0:c0}", ((double.Parse(mr.PracticeYourCostMin.Replace("$", "")) + double.Parse(r.YourCostMin)) / 2.0));
                    mr.YourCostMax.CopyTo(s, 0);
                    s[s.Length - 1] = r.YourCostMax;
                    mr.YourCostMax = (String[])s.Clone();
                    mr.PracticeYourCostMax = String.Format("{0:c0}", ((double.Parse(mr.PracticeYourCostMax.Replace("$", "")) + double.Parse(r.YourCostMax)) / 2.0));

                    Boolean[] b = new Boolean[mr.FairPrice.Length + 1];
                    mr.FairPrice.CopyTo(b, 0);
                    b[b.Length - 1] = r.FairPrice;
                    mr.FairPrice = (Boolean[])b.Clone();
                    if (!mr.PracticeFairPrice && r.FairPrice) { mr.PracticeFairPrice = r.FairPrice; }

                    Int32[] i32 = new Int32[mr.HGRecognized.Length + 1];
                    mr.HGRecognized.CopyTo(i32, 0);
                    i32[i32.Length - 1] = r.HGRecognized;
                    mr.HGRecognized = (Int32[])i32.Clone();
                    switch (r.HGRecognized)
                    {
                        case -1:
                            //Do Nothing
                            break;
                        case 0:
                            if (mr.PracticeHGRecognized == "N/A") { mr.PracticeHGRecognized = "0/0 Physicians"; }
                            String[] str0 = mr.PracticeHGRecognized.Replace("Physicians", "").Trim().Split('/');
                            mr.PracticeHGRecognized = String.Format("{0}/{1} Physicians",
                                str0[0],
                                (Convert.ToInt32(str0[1]) + 1).ToString());
                            break;
                        case 1:
                            if (mr.PracticeHGRecognized == "N/A") { mr.PracticeHGRecognized = "0/0 Physicians"; }
                            String[] str1 = mr.PracticeHGRecognized.Replace("Physicians", "").Trim().Split('/');
                            mr.PracticeHGRecognized = String.Format("{0}/{1} Physicians",
                                (Convert.ToInt32(str1[0]) + 1).ToString(),
                                (Convert.ToInt32(str1[1]) + 1).ToString());
                            break;
                        default:
                            break;
                    }

                    Double[] d = new Double[mr.HGOverallRating.Length + 1];
                    mr.HGOverallRating.CopyTo(d, 0);
                    d[d.Length - 1] = r.HGOverallRating;
                    mr.HGOverallRating = (Double[])d.Clone();
                    mr.PracticeAvgRating = ((mr.PracticeAvgRating + r.HGOverallRating) / 2.0);

                    int[] i = new int[mr.HGPatientCount.Length + 1];
                    mr.HGPatientCount.CopyTo(i, 0);
                    i[i.Length - 1] = r.HGPatientCount;
                    mr.HGPatientCount = (int[])i.Clone();

                    results[indx] = mr;
                }
            }
        }
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            // The Request QueryString here is protected against XSS attacks by Request Validation    
            QuerySettings settings = CCHSerializer.DeserializeSettings<QuerySettings>(context.Request.QueryString);
            if (settings.Lat == null) { settings.Lat = context.GetLatitude<String>(); }
            if (settings.Lng == null) { settings.Lng = context.GetLongitude<String>(); }

            ResultsBack rb = new ResultsBack();
            using (GetDoctorsForService gdfs = new GetDoctorsForService())
            {
                gdfs.ServiceName = context.GetServiceName<String>();
                gdfs.Latitude = settings.Lat;
                gdfs.Longitude = settings.Lng;
                gdfs.SpecialtyID = context.GetSpecialtyID<Int32>();
                //gdfs.MemberMedicalID = ThisSession.SubscriberMedicalID;
                gdfs.CCHID = context.GetCCHID<Int32>();
                gdfs.UserID = context.GetUserLogginID<String>();
                gdfs.SessionID = context.GetSessionID<String>();
                gdfs.Domain = context.GetDomain<String>();
                gdfs.GetData();
                if (!gdfs.HasErrors)
                {
                    ResultData rd = new ResultData();
                    gdfs.ForEachResult(delegate(object Result)
                    {
                        rd = CCHSerializer.DeserializeDataRow<ResultData>(Result);
                        rd.Distance = String.Format("{0:#0.0 mi}", Convert.ToDouble(rd.NumericDistance));
                        rb.AddResult(rd);
                    });
                }
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            String jsonBack = jss.Serialize(rb);

            context.Response.Write(jsonBack);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
    public class Results_rx : IHttpHandler, IReadOnlySessionState
    {
        //public struct QuerySettings
        //{
        //    public String Lat;
        //    public String Lng;
        //    public Int32 FromRow;
        //    public Int32 ToRow;
        //    public String CurrentSort;
        //}
        public struct ResultData
        {
            public String PharmacyName;
            public String WebURL;
            public String Address1;
            public String Address2;
            public String City;
            public String State;
            public String Zipcode;
            public String Telephone;
            public String Email;
            public Double Latitude;
            public Double Longitude;
            public String Price;
            public String YourCost;
            public String Distance;
            public Int32 PharmacyID;
            public Int32 PharmacyLocationID;
            public String Mail_Retail_ind;
            public String CurrentPharmText;
            public String BestPriceText;
            public bool Specialty_ind;  //  lam, 20130429, CI-59
        }
        public class ResultsBack
        {
            private List<ResultData> results = new List<ResultData>();
            public ResultData[] Results { get { return results.ToArray(); } }
            public void AddResult(ResultData r)
            { this.results.Add(r); }
        }
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            // The Request QueryString here is protected against XSS attacks by Request Validation    
            QuerySettings settings = CCHSerializer.DeserializeSettings<QuerySettings>(context.Request.QueryString);
            if (settings.Latitude == null) { settings.Latitude = context.GetLatitude<String>(); }
            if (settings.Longitude == null) { settings.Longitude = context.GetLongitude<String>(); }
            if (String.IsNullOrWhiteSpace(settings.CurrentSort)) settings.CurrentSort = ThisSession.DefaultSort;
            if (String.IsNullOrWhiteSpace(settings.Distance)) settings.Distance = "20";
            if (settings.ToRow == 0) { settings.ToRow = 49; }

            ResultsBack rb = new ResultsBack();
            if (context.GetChosenDrugs<DataTable>() == null || context.GetChosenDrugs<DataTable>().CheckForRows())
            {
                using (GetDrugPricingResults gdpr = new GetDrugPricingResults())
                {
                    gdpr.Distance = settings.Distance.To<Int32>();
                    gdpr.Latitude = settings.Latitude;
                    gdpr.Longitude = settings.Longitude;
                    gdpr.FromIndex = settings.FromRow;
                    gdpr.ToIndex = settings.ToRow;
                    gdpr.OrderByField = settings.CurrentDirection;
                    //gdpr.MemberRXID = ThisSession.SubscriberRXID;
                    gdpr.CCHID = context.GetCCHID<Int32>();
                    gdpr.UserID = context.GetUserLogginID<String>(); // System.Web.Security.Membership.GetUser(ThisSession.PatientEmail).ProviderUserKey.ToString();
                    if (context.GetChosenDrugs<DataTable>() == null)
                    {
                        gdpr.DrugID = context.GetDrugID<String>();// ThisSession.DrugID;
                        gdpr.GPI = context.GetDrugGPI<String>();// ThisSession.DrugGPI;
                        gdpr.Quantity = context.GetDrugQuantity<String>();// ThisSession.DrugQuantity;
                        if (!String.IsNullOrWhiteSpace(context.GetPastCareID<String>()))
                            gdpr.PastCareID = context.GetPastCareID<String>();
                    }
                    else
                    {
                        gdpr.DrugID = context.GetDrugID<String>(0);// ThisSession.ChosenDrugs.Rows[0]["DrugID"].ToString();
                        gdpr.GPI = context.GetDrugGPI<String>(0);// ThisSession.ChosenDrugs.Rows[0]["GPI"].ToString();
                        gdpr.Quantity = context.GetDrugQuantity<String>(0);// ThisSession.ChosenDrugs.Rows[0]["Quantity"].ToString();
                        gdpr.PastCareID = context.GetPastCareID<String>(0);// ThisSession.ChosenDrugs.Rows[0]["PastCareID"].ToString();
                    }
                    gdpr.SessionID = context.GetSessionID<String>();
                    gdpr.Domain = context.GetDomain<String>();
                    gdpr.GetData();

                    if (gdpr.RawResults.TableName != "EmptyTable")
                    {
                        foreach (DataRow dr in gdpr.RawResults.Rows)
                        {
                            ResultData rd = CCHSerializer.DeserializeDataRow<ResultData>(dr);
                            rd.Price = String.Format("{0:C2}", Convert.ToDouble(rd.Price));
                            rd.YourCost = String.Format("{0:C2}", Convert.ToDouble(rd.YourCost));
                            rd.Distance = String.Format("{0:#0.0 mi}", Convert.ToDouble(rd.Distance));
                            rb.AddResult(rd);
                        }
                    }
                }
            }
            else
            {
                using (GetDrugMultiPricingResults gdmpr = new GetDrugMultiPricingResults())
                {
                    using (DataView dv = new DataView(ThisSession.ChosenDrugs))
                        gdmpr.DrugList = dv.ToTable("DrugInput", false, new String[] { "DrugID", "GPI", "Quantity", "PastCareID" });
                    gdmpr.Latitude = settings.Latitude;
                    gdmpr.Longitude = settings.Longitude;
                    gdmpr.GetData();
                    if (gdmpr.RawResults.TableName != "EmptyTable")
                    {
                        foreach (DataRow dr in gdmpr.RawResults.Rows)
                        {
                            ResultData rd = CCHSerializer.DeserializeDataRow<ResultData>(dr);
                            rd.Price = String.Format("{0:C2}", Convert.ToDouble(rd.Price));
                            rd.YourCost = String.Format("{0:C2}", Convert.ToDouble(rd.YourCost));
                            rd.Distance = String.Format("{0:#0.0 mi}", Convert.ToDouble(rd.Distance));
                            rb.AddResult(rd);
                        }
                    }
                }
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            String jsonBack = jss.Serialize(rb);

            context.Response.Write(jsonBack);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
    public class Results_Past_Care : IHttpHandler, IReadOnlySessionState
    {
        public struct QuerySettings
        {
            public Double Lat;
            public Double Lng;
        }
        public struct ResultData
        {
            String ServiceName;
            Int32 TaxID;
            Int32 NPI;
            String PracticeName;
            Int32 OrganizationLocationID;
            String ProviderName;
            Int32 AllowedAmount;
            Double Latitude;
            Double Longitude;
            String LocationAddress1;
            Int32 YourCost;
            String LocationCity;
            String LocationState;
            String LocationZip;
            String LocationTelephone;
            String Distance;
            Double NumericDistance;
            Boolean FairPrice;
            Int32 HGRecognized;
            Int32 HGOverallRating;
            Int32 HGPatientCount;
            Boolean FindAService;
            Int32 HGRecognizedDocCount;
            Int32 HGDocCOunt;
        }
        public class ResultsBack
        {
            private List<ResultData> results = new List<ResultData>();
            public ResultData[] Results { get { return results.ToArray(); } }
            public void AddResult(ResultData r)
            { this.results.Add(r); }
        }
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            // The Request QueryString here is protected against XSS attacks by Request Validation    
            QuerySettings settings = CCHSerializer.DeserializeSettings<QuerySettings>(context.Request.QueryString);
            if (settings.Lat == 0.0) { settings.Lat = context.GetLatitude<Double>(); }// Convert.ToDouble(context.GetLatitude<String>()); }
            if (settings.Lng == 0.0) { settings.Lng = context.GetLongitude<Double>(); }// Convert.ToDouble(ThisSession.PatientLongitude); }

            ResultsBack rb = new ResultsBack();
            using (GetFacilitiesForServicePastCare gffspc = new GetFacilitiesForServicePastCare())
            {
                gffspc.ServiceID = context.GetServiceID<Int32>();//ThisSession.ServiceID;
                gffspc.ProcedureCode = context.GetPastCareProcedureCode<String>();// ThisSession.PastCareProcedureCode;
                gffspc.Latitude = settings.Lat;
                gffspc.Longitude = settings.Lng;
                gffspc.SpecialtyID = context.GetSpecialtyID<Int32>();//ThisSession.SpecialtyID;
                gffspc.PastCareID = context.GetPastCareID<Int32>();//Convert.ToInt32(ThisSession.PastCareID);
                //gffspc.MemberMedicalID = ThisSession.SubscriberMedicalID;
                gffspc.CCHID = context.GetCCHID<Int32>();//ThisSession.CCHID;
                gffspc.UserID = context.GetUserLogginID<String>();//System.Web.Security.Membership.GetUser(ThisSession.PatientEmail).ProviderUserKey.ToString();
                gffspc.SessionID = context.GetSessionID<String>();
                gffspc.Domain = context.GetDomain<String>();
                gffspc.GetData();

                if (!gffspc.HasErrors)
                {
                    //ResultData rd;
                    gffspc.ForEachResult<DataRow>(delegate(DataRow dr)
                    {
                        //rd = new ResultData();

                    });
                }
            }
        }

        public bool IsReusable { get { return false; } }
    }
}