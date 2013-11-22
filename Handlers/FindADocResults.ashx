<%@ WebHandler Language="C#" Class="ClearCostWeb.Handlers.FindADocResults" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Text;
using System.Dynamic;
namespace ClearCostWeb.Handlers
{
    public class FindADocResults : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            QuerySettings settings = CCHSerializer.DeserializeSettings<QuerySettings>(context.Request.Form);
            if (String.IsNullOrWhiteSpace(settings.Latitude)) settings.Latitude = context.GetLatitude<String>();
            if (String.IsNullOrWhiteSpace(settings.Longitude)) settings.Longitude = context.GetLongitude<String>();
            if (String.IsNullOrWhiteSpace(settings.CurrentSort)) settings.CurrentSort = "Distance";
            if (String.IsNullOrWhiteSpace(settings.CurrentDirection)) settings.CurrentDirection = "ASC";
            if (String.IsNullOrWhiteSpace(settings.Distance)) settings.Distance = "25";  //  lam, 20130618, MSB-324, change to 25 as default from 20
            if (settings.ToRow == 0) settings.ToRow = 25;

            dynamic resBack = new ExpandoObject();
            using (GetDoctorsForServiceAPI gdfs = new GetDoctorsForServiceAPI())
            {
                gdfs.ServiceName = context.GetServiceName<String>();
                gdfs.Latitude = settings.Latitude.To<Double>();
                gdfs.Longitude = settings.Longitude.To<Double>();
                gdfs.SpecialtyID = context.GetSpecialtyID<Int32>();
                gdfs.Distance = settings.Distance.To<Int32>();
                gdfs.OrderByField = settings.CurrentSort;
                gdfs.OrderDirection = settings.CurrentDirection;
                gdfs.CCHID = context.GetCCHID<Int32>();
                gdfs.UserID = context.GetUserLogginID<String>();
                gdfs.SessionID = context.GetSessionID<String>();
                gdfs.Domain = context.GetDomain<String>();
                gdfs.GetData(settings.FromRow, settings.ToRow);
                if (gdfs.Results != null && gdfs.Results.Count > 0)
                {
                    resBack.Results = gdfs.Results;
                    resBack.LearnMore = gdfs.LearnMore;
                    resBack.ThinData = gdfs.ThinData;
                    if (gdfs.PreferredData != null) { resBack.PreferredResults = gdfs.PreferredData; }
                }
                else
                    resBack.EmptyResults = gdfs.EmptySet;

                String jsonBack = (resBack as ExpandoObject).ToJson();
                if (context.Request.QueryString["callback"] != null)
                    jsonBack = String.Format("{0}({1})",
                        context.Request.QueryString["callback"].ToString(),
                        jsonBack);
                context.Response.Write(jsonBack);
            }
        }
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
