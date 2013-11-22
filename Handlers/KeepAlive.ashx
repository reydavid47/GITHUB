<%@ WebHandler Language="C#" Class="ClearCostWeb.Handlers.KeepAlive" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;

namespace ClearCostWeb.Handlers
{
    public class KeepAlive : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            context.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Expires", "0");
            DateTime d = DateTime.UtcNow;
            
            MembershipUser mu = Membership.GetUser(new Guid(ThisSession.UserLogginID));
            if (mu != null && mu.IsOnline)
            {
                //ASP.NET Memberships are in UTC time
                mu.LastActivityDate = d;
                context.Session["Heartbeat"] = d;
            }
            else
            {
                context.Response.StatusCode = 404;
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