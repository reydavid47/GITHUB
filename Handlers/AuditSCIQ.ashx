<%@ WebHandler Language="C#" Class="ClearCostWeb.Handlers.AuditSCIQ" %>
using System;
using System.Web;
using System.Collections.Specialized;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace ClearCostWeb.Handlers
{
    public class AuditSCIQ : IHttpHandler, IReadOnlySessionState
    {
        //ensure no random calls are consumed[security/sever resources] - methods available 
        private string[] availableMethods = { "started", "restart", "completed", "quit" };
        private NameValueCollection iData = HttpContext.Current.Request.Form;
        private JavaScriptSerializer jserialize = new JavaScriptSerializer();
        private SortedDictionary<string, object> returnData = new SortedDictionary<string, object>();
        CreateSCIQAuditTrail SCIQAudit = new CreateSCIQAuditTrail();
        public void ProcessRequest(HttpContext context)
        {
            bool methodAccepted = false;
            foreach (string methods in availableMethods)
            {
                if (iData["method"] == methods)
                {
                    methodAccepted = true;
                }
            }
            if (methodAccepted == false)
            {
                returnData.Add("ok", "false");
                returnData.Add("reason", "method");
                returnSuccess(jserialize.Serialize(returnData));
            }
            else
            {
                beginProcess();
            }
        }

        private void beginProcess()
        {
            switch (iData["method"])
            {
                case "started":                    
                    SCIQAudit.CCHID = Convert.ToInt32(iData["cchid"]);
                    SCIQAudit.SessionID = iData["sid"];
                    SCIQAudit.Action = "started";
                    SCIQAudit.Category = iData["category"];
                    SCIQAudit.URL = iData["url"];
                    SCIQAudit.SCIQ_FLG = true;
                    SCIQAudit.PostData();               
                    returnData.Add("ok", true);                                                                                                                                                        
                    returnSuccess(jserialize.Serialize(returnData));
                    break;
                case "restart":
                    SCIQAudit.CCHID = Convert.ToInt32(iData["cchid"]);
                    SCIQAudit.SessionID = iData["sid"];
                    SCIQAudit.Action = "restart";
                    SCIQAudit.Category = null;
                    SCIQAudit.URL = null;
                    SCIQAudit.SCIQ_FLG = true;
                    SCIQAudit.PostData();
                    returnData.Add("ok", true);
                    returnSuccess(jserialize.Serialize(returnData));
                    break;                    
                case "quit":
                    SCIQAudit.CCHID = Convert.ToInt32(iData["cchid"]);
                    SCIQAudit.SessionID = iData["sid"];
                    SCIQAudit.Action = "quit";
                    SCIQAudit.Category = iData["category"];
                    SCIQAudit.URL = iData["url"];
                    SCIQAudit.SCIQ_FLG = true;
                    SCIQAudit.PostData();
                    returnData.Add("ok", true);                                                      
                    returnSuccess(jserialize.Serialize(returnData));
                    break;                                    
            }
        }

        private void returnSuccess(object data)
        {
            HttpContext.Current.Response.AddHeader("content-type", "text/json");
            HttpContext.Current.Response.StatusCode = 200;
            HttpContext.Current.Response.Write(data);
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