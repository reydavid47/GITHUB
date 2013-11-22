<%@ WebHandler Language="C#" Class="ClearCostWeb.Handlers.FindAServiceResults" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Text;
using System.Dynamic;
namespace ClearCostWeb.Handlers
{
    public class FindAServiceResults : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            //Set the response type
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            //Extract and save the incomming request details
            QuerySettings settings = CCHSerializer.DeserializeSettings<QuerySettings>(context.Request.Form);
            //Set the defaults if we received blanks
            if (String.IsNullOrWhiteSpace(settings.Latitude)) settings.Latitude = context.GetLatitude<String>();
            if (String.IsNullOrWhiteSpace(settings.Longitude)) settings.Longitude = context.GetLongitude<String>();
            if (String.IsNullOrWhiteSpace(settings.CurrentSort)) settings.CurrentSort = ThisSession.DefaultSort;
            if (String.IsNullOrWhiteSpace(settings.CurrentDirection)) settings.CurrentDirection = "ASC";
            //if (String.IsNullOrWhiteSpace(settings.Distance)) settings.Distance = "20";  //  lam, 20130613, 20 mile is not correct for default distance
            if (String.IsNullOrWhiteSpace(settings.Distance)) settings.Distance = "25";  //  lam, 20130613, MSB-327, change default distance from 20 to 25
            if (settings.ToRow == 0) settings.ToRow = 25;

            dynamic resBack = new ExpandoObject();
            using (GetFacilitiesForServiceAPI gffs = new GetFacilitiesForServiceAPI())
            {
                gffs.ServiceID = context.GetServiceID<Int32>();
                gffs.Latitude = settings.Latitude.To<Double>();
                gffs.Longitude = settings.Longitude.To<Double>();
                gffs.SpecialtyID = context.GetSpecialtyID<Int32>();
                gffs.Distance = settings.Distance.To<Int32>();
                gffs.OrderByField = settings.CurrentSort;
                gffs.OrderDirection = settings.CurrentDirection;
                gffs.CCHID = context.GetCCHID<Int32>();
                gffs.UserID = context.GetUserLogginID<String>();
                gffs.SessionID = context.GetSessionID<String>();
                gffs.Domain = context.GetDomain<String>();
                gffs.GetData(settings.FromRow, settings.ToRow);

                if (gffs.Results != null && gffs.Results.Count > 0)
                    resBack.Results = gffs.Results;
                else
                    resBack.EmptyResults = gffs.EmptySet;

                // rld, these could throw null exceptions
                resBack.ThinData = gffs.ThinData;
                resBack.LearnMore = gffs.LearnMore;
                if (gffs.PreferredData != null) { resBack.PreferredResults = gffs.PreferredData; }
            }

            String jsonBack = (resBack as ExpandoObject).ToJson();
            //Add support for JsonP
            if (context.Request.QueryString["callback"] != null)
            {
                jsonBack = string.Format("{0}({1})",
                    context.Request.QueryString["callback"].ToString(),
                    jsonBack);
            }

            context.Response.Write(jsonBack);
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