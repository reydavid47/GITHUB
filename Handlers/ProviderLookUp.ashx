<%@ WebHandler Language="C#" Class="ClearCostWeb.Handlers.ProviderLookUp" %>

using System;
using System.Web;
using System.Collections.Specialized;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace ClearCostWeb.Handlers
{
    public class ProviderLookUp : IHttpHandler, IReadOnlySessionState
    {
        //ensure no random calls are consumed[security/sever resources] - methods available 
        private string[] availableMethods = { "getProviderStates", "getProviderCities", "getProviders", "addProvider", "getProvidersByORG" };
        private NameValueCollection iData = HttpContext.Current.Request.Form;
        private JavaScriptSerializer jserialize = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            bool methodAccepted = false;
            foreach (string methods in availableMethods) {
                if (iData["method"] == methods) {
                    methodAccepted = true;
                }
            }
            if (methodAccepted == false)
            {
                SortedDictionary<string, string> denied = new SortedDictionary<string, string>();
                denied.Add("ok", "false");
                denied.Add("reason", "method");
                returnSuccess(jserialize.Serialize(denied));                
            }
            else {
                beginProcess();
            }
        }

        private void beginProcess() {
            switch (iData["method"]) { 
                case "getProviderStates":
                    List<string[]> states = new List<string[]>();
                    sc_GetProviderLookup statesOb = new sc_GetProviderLookup();
                    statesOb.Category = iData["category"].ToString();                   
                    statesOb.GetData();
                    System.Data.DataTable statesTable = statesOb.providers;
                    foreach (System.Data.DataRow row in statesTable.Rows)
                    {
                        string[] tempList = { row["state"].ToString(), row["state"].ToString() };
                        states.Add(tempList);
                    }
                    SortedDictionary<string, object> states_json = new SortedDictionary<string, object>();
                    SortedDictionary<string, object> returnData_state = new SortedDictionary<string, object>();
                    states_json.Add("states", states);

                    returnData_state.Add("ok", true);
                    returnData_state.Add("data", states_json);
                    returnSuccess(jserialize.Serialize(returnData_state));
                    break;
                case "getProviderCities":
                    List<string[]> cities = new List<string[]>();
                    sc_GetProviderLookup citiesOb = new sc_GetProviderLookup();
                    citiesOb.Category = iData["category"].ToString();                    
                    citiesOb.State = iData["state"].ToString();
                    citiesOb.GetData();
                    System.Data.DataTable citiesTable = citiesOb.providers;
                    foreach (System.Data.DataRow row in citiesTable.Rows)
                    {
                        string[] tempList = { row["city"].ToString(), row["city"].ToString() };
                        cities.Add(tempList);
                    }                    
                    SortedDictionary<string, object> cities_json = new SortedDictionary<string, object>();
                    SortedDictionary<string, object> returnData_city = new SortedDictionary<string, object>();
                    cities_json.Add("cities", cities);

                    returnData_city.Add("ok", true);
                    returnData_city.Add("data", cities_json);
                    returnSuccess(jserialize.Serialize(returnData_city));
                    break;
                case "getProviders":
                    List<string[]> providers = new List<string[]>();
                    sc_GetProviderLookup providersOb = new sc_GetProviderLookup();
                    providersOb.Category = iData["category"].ToString();
                    providersOb.State = iData["state"].ToString();
                    providersOb.City = iData["city"].ToString();
                    providersOb.GetData();
                    System.Data.DataTable providertsTable = providersOb.providers;
                    foreach (System.Data.DataRow row in providertsTable.Rows)
                    {
                        string[] tempList = { row["ProviderID"].ToString(), row["location"].ToString() };
                        providers.Add(tempList);
                    }                    
                    SortedDictionary<string, object> providers_json = new SortedDictionary<string, object>();
                    SortedDictionary<string, object> returnData_provider = new SortedDictionary<string, object>();
                    providers_json.Add("providers", providers);

                    returnData_provider.Add("ok", true);
                    returnData_provider.Add("data", providers_json);
                    returnSuccess(jserialize.Serialize(returnData_provider));
                    break;
                case "getProvidersByORG":
                    List<string[]> providersByORG = new List<string[]>();
                    sc_GetProviderLookup providersObByORG = new sc_GetProviderLookup();
                    providersObByORG.Category = iData["category"].ToString();
                    providersObByORG.State = iData["state"].ToString();
                    providersObByORG.City = iData["city"].ToString();
                    providersObByORG.OrgID = Convert.ToInt32(iData["orgid"]);
                    providersObByORG.GetData();
                    System.Data.DataTable providersByORGTable = providersObByORG.providers;
                    foreach (System.Data.DataRow row in providersByORGTable.Rows)
                    {
                        string[] tempList = { row["ProviderID"].ToString(), row["location"].ToString() };
                        providersByORG.Add(tempList);
                    }
                    SortedDictionary<string, object> providersByORG_json = new SortedDictionary<string, object>();
                    SortedDictionary<string, object> returnData_providerByORG = new SortedDictionary<string, object>();
                    providersByORG_json.Add("providers", providersByORG);

                    returnData_providerByORG.Add("ok", true);
                    returnData_providerByORG.Add("data", providersByORG_json);
                    returnSuccess(jserialize.Serialize(returnData_providerByORG));
                    break;                     
                case "addProvider":
                    SortedDictionary<string, object> returnData_addProvider = new SortedDictionary<string, object>();
                    sc_AddNewProviderRating newProvider = new sc_AddNewProviderRating();
                    newProvider.CCHID = Convert.ToInt32(iData["cchid"]);
                    newProvider.Category = iData["category"].ToString();
                    newProvider.Review = iData["pcomment"];
                    newProvider.Rating = Convert.ToInt32(iData["prating"]);
                    newProvider.ProviderID = Convert.ToInt32(iData["pid"]);
                    newProvider.EmployerID = Convert.ToInt32(iData["eid"]);
                    newProvider.PostData();
                    returnData_addProvider.Add("ok", true);
                    returnData_addProvider.Add("data", new string[] { "comment", iData["pcomment"] });
                    returnSuccess(jserialize.Serialize(returnData_addProvider));
                    break;               
            }
        }
        
        private void returnSuccess(object data){
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