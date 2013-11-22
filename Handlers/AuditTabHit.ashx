<%@ WebHandler Language="C#" Class="ClearCostWeb.Handlers.AuditTabHit" %>
using System;
using System.Web;
using System.Web.SessionState;
using System.Text;
namespace ClearCostWeb.Handlers
{
    public class AuditTabHit : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            //Set the response type
            context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = Encoding.UTF8;

            using (CreateAuditTrail cat = new CreateAuditTrail())
            {
                cat.CCHID = context.GetCCHID<int>();
                cat.SessionID = context.GetSessionID<string>();
                cat.Domain = context.GetDomain<string>();
                cat.Latitude = context.GetLatitude<double>();
                cat.Longitude = context.GetLongitude<double>();
                cat.SearchType = "ClickedPastCareTab";
                cat.PostData();
            }

            context.Response.StatusCode = 200;
        }
        public bool IsReusable
        {
            get { return true; }
        }
    }
}