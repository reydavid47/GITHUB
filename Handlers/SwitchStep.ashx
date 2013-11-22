<%@ WebHandler Language="C#" Class="ClearCostWeb.Handlers.SwitchStep" %>

using System;
using System.Web;
using System.Collections.Specialized;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace ClearCostWeb.Handlers
{
    public class SwitchStep : IHttpHandler, IReadOnlySessionState
    {
        //ensure no random calls are consumed[security/sever resources] - methods available 
        private string[] availableMethods = { "savechoice", "savechoiceemail", "endsession", "selectedproviders" };
        private NameValueCollection iData = HttpContext.Current.Request.Form;
        private JavaScriptSerializer jsonp_serialize = new JavaScriptSerializer();
                
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
                SortedDictionary<string, string> denied = new SortedDictionary<string, string>();
                denied.Add("ok", "false");
                denied.Add("reason", "method");
                returnSuccess(jsonp_serialize.Serialize(denied));
            }
            else
            {
                beginProcess();
            }
        }

        private void beginProcess() {
            SortedDictionary<string, object> returnJSONP = new SortedDictionary<string, object>();
            switch (iData["method"])
            {
                case "savechoice":
                    SC_ssInsSwitchWorkFlow ssFlow = new SC_ssInsSwitchWorkFlow();
                    ssFlow.CCHID = Convert.ToInt32(iData["cchid"]);
                    ssFlow.SSID = Convert.ToInt32(iData["sessionid"]);
                    ssFlow.Stepnum = Convert.ToInt32(iData["stepnum"]);
                    ssFlow.DecisionID = Convert.ToInt32(iData["decisionid"]);
                    ssFlow.EmailDate = null;
                    ssFlow.PostData();
                    
                    returnJSONP.Add("ok", true);
                    returnSuccess(jsonp_serialize.Serialize(returnJSONP));
                    break;
                    
                case "savechoiceemail":
                    SC_ssInsSwitchWorkFlow ssFlowEmail = new SC_ssInsSwitchWorkFlow();
                    ssFlowEmail.CCHID = Convert.ToInt32(iData["cchid"]);
                    ssFlowEmail.SSID = Convert.ToInt32(iData["sessionid"]);
                    ssFlowEmail.Stepnum = Convert.ToInt32(iData["stepnum"]);
                    ssFlowEmail.DecisionID = Convert.ToInt32(iData["decisionid"]);
                    ssFlowEmail.EmailDate = iData["emaildate"].ToString();
                    ssFlowEmail.PostData();

                    returnJSONP.Add("ok", true);
                    returnSuccess(jsonp_serialize.Serialize(returnJSONP));
                    break;
                    
                case "endsession":
                    SC_ssEndSession ssEndSession = new SC_ssEndSession();
                    ssEndSession.sessionID = Convert.ToInt32(iData["sessionid"]);
                    ssEndSession.state = Convert.ToInt32(iData["state"]);
                    ssEndSession.PostData();

                    returnJSONP.Add("ok", true);
                    returnSuccess(jsonp_serialize.Serialize(returnJSONP));
                    break;

                case "selectedproviders":
                    SC_ssInsSwitchWorkFlow ssSelectedProviders = new SC_ssInsSwitchWorkFlow();
                    ssSelectedProviders.CCHID = Convert.ToInt32(iData["cchid"]);
                    ssSelectedProviders.SSID = Convert.ToInt32(iData["sessionid"]);
                    ssSelectedProviders.Stepnum = Convert.ToInt32(iData["stepnum"]);
                    ssSelectedProviders.DecisionID = Convert.ToInt32(iData["decisionid"]);
                    ssSelectedProviders.ProviderList = iData["providerlist"].ToString();
                    ssSelectedProviders.PostData();

                    returnJSONP.Add("ok", true);
                    returnSuccess(jsonp_serialize.Serialize(returnJSONP));
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
                return false;
            }
        }

    }
}