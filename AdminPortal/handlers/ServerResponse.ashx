<%@ WebHandler Language="C#" Class="ServerResponse" %>

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Script.Serialization;
using System.Web.Configuration;



class Metric
{
    public string Name;
    public int Value;
}


public class ServerResponse : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "application/json";
        context.Response.ContentEncoding = Encoding.UTF8;

        /*****************************************
         * PARSE PARAMS
         * is there a more general way to do this?
         *  QuerySettings settings = CCHSerializer.DeserializeSettings<QuerySettings>(context.Request.QueryString);
         ****************************************/


        string Date = Microsoft.Security.Application.Encoder.HtmlEncode( context.Request.QueryString["query-date"] );
        string Employer = Microsoft.Security.Application.Encoder.HtmlEncode( context.Request.QueryString["employer"]);

        string Registered = context.Request.QueryString["registered"];
        string Visited = context.Request.QueryString["visited"];
        string SavingsAlerts = context.Request.QueryString["savings-alerts"];
        string HealthShopper = context.Request.QueryString["health-shopper"];
        string RetailHourly = context.Request.QueryString["retail-hourly"];
        string RetailSalary = context.Request.QueryString["retail-salary"];
        string RoastingPlant = context.Request.QueryString["roasting-plant"];
        string Regional = context.Request.QueryString["regional"];
        
       

        /*****************************************
         * DO SERVER SIDE WORK
         * GO TO THE DATABASE WITH THE PARAMS
         ****************************************/
        //System.Diagnostics.Debug.WriteLine(ThisSession.CnxString);

        string connString = WebConfigurationManager.ConnectionStrings["CCH_StarbucksWeb"].ConnectionString;

        using (ClearCostWeb.GetRegisterCounts grc = new ClearCostWeb.GetRegisterCounts())
        {
            System.Diagnostics.Debug.WriteLine(connString);
            grc.GetData(connString);

            System.Diagnostics.Debug.WriteLine(grc.Tables);
            var table = new List<Dictionary<string, object>>();
            

            DataRow[] results = grc.Tables[0].Select();
            foreach (DataRow dr in results)
            {
                //row.Clear();
                var row = new Dictionary<string, object> { };
                foreach (DataColumn column in grc.Tables[0].Columns)
                {
                    System.Diagnostics.Debug.WriteLine(column.ColumnName);
                    System.Diagnostics.Debug.WriteLine(dr[column.ColumnName]);
                    row.Add(column.ColumnName, dr[column.ColumnName]);
                }
                table.Add(row);
            }
        
        
        /*****************************************
         * RETURN JSON TO THE CLIENT
         ****************************************/

        JavaScriptSerializer jss = new JavaScriptSerializer();
        String jsonBack = jss.Serialize(table);
        System.Diagnostics.Debug.WriteLine(jsonBack);

                    
        context.Response.Write(jsonBack);
        }//using basecchdata
        
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}