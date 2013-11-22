<%@ WebHandler Language="C#" Class="ClearCostWeb.Handlers.AutoComplete" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Web.Script.Serialization;
using System.Text;
namespace ClearCostWeb.Handlers
{
    public class AutoComplete : IHttpHandler, IReadOnlySessionState
    {
        public struct QuerySettings
        {
            public String type;
            public String userInput;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            // The Request QueryString here is protected against XSS attacks by Request Validation    
            QuerySettings settings = CCHSerializer.DeserializeSettings<QuerySettings>(context.Request.QueryString);
            AutoCompleteResult acr = new AutoCompleteResult();

            if (null != settings.userInput)
            {
                if (settings.userInput.Trim() != String.Empty)
                {                    
                    settings.userInput = settings.userInput.Trim();
                    if (settings.type == "AutoComplete")
                    {
                        using (GetServiceList_AutoComplete gslac = new GetServiceList_AutoComplete(settings.userInput))
                            if (!gslac.HasErrors && gslac.RowsBack > 0)
                                gslac.ForEachMatch(delegate(string match) { acr.AddMatch(match); });
                    }
                    else if (settings.type == "DrugAutoComplete")
                    {
                        using (GetDrugList_AutoComplete gdlac = new GetDrugList_AutoComplete(settings.userInput))
                            if (!gdlac.HasErrors && gdlac.RowsBack > 0)
                                gdlac.ForEachMatch(delegate(string match) { acr.AddMatch(match); });
                    }
                }

                JavaScriptSerializer jss = new JavaScriptSerializer();
                String jsonBack = jss.Serialize(acr);
                context.Response.Write(jsonBack);
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