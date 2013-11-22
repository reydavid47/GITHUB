<%@ WebHandler Language="C#" Class="ClearCostWeb.Handlers.ShowMore" %>

using System;
using System.Web;
using System.Collections.Specialized;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Data;

namespace ClearCostWeb.Handlers
{
    public class ShowMore : IHttpHandler, IReadOnlySessionState
    {
        //ensure no random calls are consumed[security/sever resources] - methods available 
        private string[] availableMethods = { "categoryDetail-RX", "categoryDetail-Imaging", "categoryDetail-Lab", "categoryDetail-MVP" };
        private DataTable dbData;
        private NameValueCollection iData = HttpContext.Current.Request.Form;
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
                returnSuccess(new JavaScriptSerializer().Serialize(denied));
            }
            else
            {
                beginProcess();
            }
        }

        private void beginProcess() {
            SortedDictionary<string, object> success = new SortedDictionary<string, object>();
            switch (iData["method"]) {
                case "categoryDetail-RX":
                    dbData = null;
                    SC_GetFairPriceAlternativesRX fpa = new SC_GetFairPriceAlternativesRX();
                    {
                        fpa.CCHID = Convert.ToInt32(iData["cchid"]);
                        fpa.GPI = iData["gpi"];
                        fpa.GenericIndicator = 1;
                        fpa.RowMin = Convert.ToInt32(iData["startIndex"]);
                        fpa.RowMax = Convert.ToInt32(iData["stopIndex"]);
                        fpa.GetData();

                        if (fpa.Tables.Count >= 1 && fpa.Tables[0].Rows.Count > 0)
                        { dbData = fpa.Tables[0]; }
                        else { dbData = null; }
                    }                    
                    success.Add("ok", "true");
                    switch(iData["returnType"]){
                        case"json":
                            SortedDictionary<string, object> jsonItems = new SortedDictionary<string,object>();
                            int i = 0;
                            if (dbData != null)
                            {
                                foreach (DataRow dr in dbData.Rows)
                                {
                                    SortedDictionary<string, string> jsonData = new SortedDictionary<string, string>();
                                    jsonData.Add("providername", dr["providername"].ToString());
                                    jsonData.Add("address1", dr["address1"].ToString());
                                    jsonData.Add("city", dr["city"].ToString());
                                    jsonData.Add("state", dr["state"].ToString());
                                    jsonData.Add("zipcode", dr["zipcode"].ToString());
                                    jsonData.Add("distance", dr["distance"].ToString());
                                    jsonData.Add("avgpatientsatisfaction", dr["avgpatientsatisfaction"].ToString());
                                    jsonData.Add("patientratings", dr["patientratings"].ToString());
                                    jsonItems.Add(i.ToString(), jsonData);
                                    i++;
                                }
                                success.Add("jsonItems", jsonItems);                            
                            }
                            else 
                            {
                                SortedDictionary<string, string> jsonData = new SortedDictionary<string, string>();                          
                                success.Add("jsonItems", jsonItems);                            
                            }                  
                            
                            break;
                        case"html":
                            /* 
                            -- example usage of return html if needed -- 
                            List<string> htmlItems = new List<string>();
                            htmlItems.Add("<div>1</div>");
                            htmlItems.Add("<div>2</div>");
                            htmlItems.Add("<div>3</div>");
                            success.Add("htmlItems", htmlItems.ToArray());                            
                            */
                            break;
                    }                    
                    returnSuccess(new JavaScriptSerializer().Serialize(success));        
                    break;
                case "categoryDetail-Imaging":
                    dbData = null;
                    SC_GetFairPriceAlternatives fpaImaging = new SC_GetFairPriceAlternatives();
                    {
                        fpaImaging.CCHID = Convert.ToInt32(iData["cchid"]);
                        fpaImaging.Category = "imaging";
                        fpaImaging.Lat = Convert.ToDouble(iData["latitude"]);
                        fpaImaging.Lon = Convert.ToDouble(iData["longitude"]);
                        fpaImaging.RowMin = Convert.ToInt32(iData["startIndex"]);
                        fpaImaging.RowMax = Convert.ToInt32(iData["stopIndex"]);
                        fpaImaging.GetData();

                        if (fpaImaging.Tables.Count >= 1 && fpaImaging.Tables[0].Rows.Count > 0)
                        { dbData = fpaImaging.Tables[0]; }
                        else { dbData = null; }
                    }
                    success.Add("ok", "true");
                    switch (iData["returnType"])
                    {
                        case "json":
                            SortedDictionary<string, object> jsonItems = new SortedDictionary<string, object>();
                            int i = 0;
                            if (dbData != null)
                            {
                                foreach (DataRow dr in dbData.Rows)
                                {
                                    SortedDictionary<string, string> jsonData = new SortedDictionary<string, string>();
                                    jsonData.Add("providername", dr["providername"].ToString());
                                    jsonData.Add("address1", dr["address1"].ToString());
                                    jsonData.Add("city", dr["city"].ToString());
                                    jsonData.Add("state", dr["state"].ToString());
                                    jsonData.Add("zipcode", dr["zipcode"].ToString());
                                    jsonData.Add("distance", dr["distance"].ToString());
                                    jsonData.Add("avgpatientsatisfaction", dr["avgpatientsatisfaction"].ToString());
                                    jsonData.Add("patientratings", dr["patientratings"].ToString());
                                    jsonItems.Add(i.ToString(), jsonData);
                                    i++;
                                }
                                success.Add("jsonItems", jsonItems);
                            }
                            else
                            {
                                SortedDictionary<string, string> jsonData = new SortedDictionary<string, string>();
                                success.Add("jsonItems", jsonItems);
                            }

                            break;
                        case "html":
                            /* 
                            -- example usage of return html if needed -- 
                            List<string> htmlItems = new List<string>();
                            htmlItems.Add("<div>1</div>");
                            htmlItems.Add("<div>2</div>");
                            htmlItems.Add("<div>3</div>");
                            success.Add("htmlItems", htmlItems.ToArray());                            
                            */
                            break;
                    }
                    returnSuccess(new JavaScriptSerializer().Serialize(success));
                    break;
                case "categoryDetail-Lab":
                    dbData = null;
                    SC_GetFairPriceAlternatives fpaLab = new SC_GetFairPriceAlternatives();
                    {
                        fpaLab.CCHID = Convert.ToInt32(iData["cchid"]);
                        fpaLab.Category = "lab";
                        fpaLab.Lat= Convert.ToDouble(iData["latitude"]);
                        fpaLab.Lon = Convert.ToDouble(iData["longitude"]);
                        fpaLab.RowMin = Convert.ToInt32(iData["startIndex"]);
                        fpaLab.RowMax = Convert.ToInt32(iData["stopIndex"]);
                        fpaLab.GetData();

                        if (fpaLab.Tables.Count >= 1 && fpaLab.Tables[0].Rows.Count > 0)
                        { dbData = fpaLab.Tables[0]; }
                        else { dbData = null; }
                    }
                    success.Add("ok", "true");
                    switch (iData["returnType"])
                    {
                        case "json":
                            SortedDictionary<string, object> jsonItems = new SortedDictionary<string, object>();
                            int i = 0;
                            if (dbData != null)
                            {
                                foreach (DataRow dr in dbData.Rows)
                                {
                                    SortedDictionary<string, string> jsonData = new SortedDictionary<string, string>();
                                    jsonData.Add("providername", dr["providername"].ToString());
                                    jsonData.Add("address1", dr["address1"].ToString());
                                    jsonData.Add("city", dr["city"].ToString());
                                    jsonData.Add("state", dr["state"].ToString());
                                    jsonData.Add("zipcode", dr["zipcode"].ToString());
                                    jsonData.Add("distance", dr["distance"].ToString());
                                    jsonData.Add("avgpatientsatisfaction", dr["avgpatientsatisfaction"].ToString());
                                    jsonData.Add("patientratings", dr["patientratings"].ToString());
                                    jsonItems.Add(i.ToString(), jsonData);
                                    i++;
                                }
                                success.Add("jsonItems", jsonItems);
                            }
                            else
                            {
                                SortedDictionary<string, string> jsonData = new SortedDictionary<string, string>();
                                success.Add("jsonItems", jsonItems);
                            }

                            break;
                        case "html":
                            /* 
                            -- example usage of return html if needed -- 
                            List<string> htmlItems = new List<string>();
                            htmlItems.Add("<div>1</div>");
                            htmlItems.Add("<div>2</div>");
                            htmlItems.Add("<div>3</div>");
                            success.Add("htmlItems", htmlItems.ToArray());                            
                            */
                            break;
                    }
                    returnSuccess(new JavaScriptSerializer().Serialize(success));
                    break;
                case "categoryDetail-MVP":
                    dbData = null;
                    SC_GetFairPriceAlternatives fpaMVP = new SC_GetFairPriceAlternatives();
                    {
                        fpaMVP.CCHID = Convert.ToInt32(iData["cchid"]);
                        fpaMVP.Category = "mvp";
                        if (iData["SubCategory"] != null)
                        {
                            Int16 sub = -1;
                            if (Int16.TryParse(iData["subCategory"], out sub))
                            {
                                fpaMVP.SpecialtyID = sub;
                            }
                        }
                        fpaMVP.Lat = Convert.ToDouble(iData["latitude"]);
                        fpaMVP.Lon = Convert.ToDouble(iData["longitude"]);
                        fpaMVP.RowMin = Convert.ToInt32(iData["startIndex"]);
                        fpaMVP.RowMax = Convert.ToInt32(iData["stopIndex"]);
                        fpaMVP.GetData();

                        if (fpaMVP.Tables.Count >= 1 && fpaMVP.Tables[0].Rows.Count > 0)
                        { dbData = fpaMVP.Tables[0]; }
                        else { dbData = null; }
                    }
                    success.Add("ok", "true");
                    switch (iData["returnType"])
                    {
                        case "json":
                            SortedDictionary<string, object> jsonItems = new SortedDictionary<string, object>();
                            int i = 0;
                            if (dbData != null)
                            {
                                foreach (DataRow dr in dbData.Rows)
                                {
                                    SortedDictionary<string, string> jsonData = new SortedDictionary<string, string>();
                                    jsonData.Add("providername", dr["providername"].ToString());
                                    jsonData.Add("address1", dr["address1"].ToString());
                                    jsonData.Add("city", dr["city"].ToString());
                                    jsonData.Add("state", dr["state"].ToString());
                                    jsonData.Add("zipcode", dr["zipcode"].ToString());
                                    jsonData.Add("distance", dr["distance"].ToString());
                                    jsonData.Add("avgpatientsatisfaction", dr["avgpatientsatisfaction"].ToString());
                                    jsonData.Add("patientratings", dr["patientratings"].ToString());
                                    jsonItems.Add(i.ToString(), jsonData);
                                    i++;
                                }
                                success.Add("jsonItems", jsonItems);
                            }
                            else
                            {
                                SortedDictionary<string, string> jsonData = new SortedDictionary<string, string>();
                                success.Add("jsonItems", jsonItems);
                            }

                            break;
                        case "html":
                            /* 
                            -- example usage of return html if needed -- 
                            List<string> htmlItems = new List<string>();
                            htmlItems.Add("<div>1</div>");
                            htmlItems.Add("<div>2</div>");
                            htmlItems.Add("<div>3</div>");
                            success.Add("htmlItems", htmlItems.ToArray());                            
                            */
                            break;
                    }
                    returnSuccess(new JavaScriptSerializer().Serialize(success));
                    break;                                                            
                default:
                    
                    break;
            };                        
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