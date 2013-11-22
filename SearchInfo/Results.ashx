<%@ WebHandler Language="C#" Class="ClearCostWeb.Handlers.Results" %>

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Script.Serialization;

namespace ClearCostWeb.Handlers
{
    public class Results : IHttpHandler, IReadOnlySessionState
    {
        DataTable tblResults = null;

        public struct QuerySettings
        {
            public String ToDo;
            public Int32 ResCount;
            public String Sort;
            public String Direction;
            public Boolean EndOfResults;
            public Boolean IsThin;
            public Boolean IsMultiLab;
            public String Lat;
            public String Lng;
        }
        QuerySettings settings;

        public struct GoogleFields
        {
            public Int32 Index;
            public String Lat;
            public String Lng;
        }
        public class GoogleDistance
        {
            private List<GoogleFields> results = new List<GoogleFields>();
            public GoogleFields[] Results { get { return results.ToArray(); } }
            public void AddMatch(GoogleFields match)
            { this.results.Add(match); }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            // The Request QueryString here is protected against XSS attacks by Request Validation    
            settings = CCHSerializer.DeserializeSettings<QuerySettings>(context.Request.QueryString);

            switch (settings.ToDo)
            {
                case "GetFirstResults":
                    GetFirstResults();
                    break;
                case "ChangeSort":
                    //if (!this.endOfResults)
                    //ChangeSortResults();
                    break;
                case "GetNextResults":
                    //GetNextResults();
                    break;
                default:
                    break;
            }
            GoogleDistance gd = new GoogleDistance();
            foreach (DataRow dr in tblResults.Rows)
            {
                GoogleFields gf = new GoogleFields();
                gf.Index = tblResults.Rows.IndexOf(dr);
                gf.Lat = dr["Latitude"].ToString();
                gf.Lng = dr["Longitude"].ToString();
                gd.AddMatch(gf);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            String jsonBack = jss.Serialize(gd);
            context.Response.Write(jsonBack);
        }

        private void GetFirstResults()
        {
            using (GetFacilitiesForService gffs = new GetFacilitiesForService())
            {
                gffs.ServiceID = ThisSession.ServiceID.ToString();
                gffs.Latitude = settings.Lat;
                gffs.Longitude = settings.Lng;
                gffs.SpecialtyID = ThisSession.SpecialtyID.ToString();
                gffs.Distance = "20";
                gffs.OrderByField = "Distance";
                gffs.OrderDirection = "ASC";
                gffs.MemberMedicalID = ThisSession.SubscriberMedicalID;
                gffs.UserID = System.Web.Security.Membership.GetUser(ThisSession.PatientEmail).ProviderUserKey.ToString();

                gffs.GetData();

                if (!gffs.DataIsThin)
                {
                    if (!gffs.HasErrors && gffs.RowsBack > 0)
                    {
                        settings.EndOfResults = gffs.ReachedEnd;
                        tblResults = gffs.RawResults;
                    }
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}