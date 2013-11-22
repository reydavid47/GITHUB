using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

public partial class AdminPortal_EmployerMetrics : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        /*

        string sqlstr = "SELECT TOP 10 * FROM BenefitPlans";
        BaseCCHData b = new BaseCCHData(sqlstr, true);
        b.GetData();
        
        System.Diagnostics.Debug.WriteLine("DONE");
        System.Diagnostics.Debug.WriteLine(b.GetType());
        System.Diagnostics.Debug.WriteLine(b.HasErrors);
        System.Diagnostics.Debug.WriteLine(b.RowsBack);
        System.Diagnostics.Debug.WriteLine(b.SqlException);
        System.Diagnostics.Debug.WriteLine(b.GenException);
        System.Diagnostics.Debug.WriteLine(b.Tables[0].GetType());
        System.Diagnostics.Debug.WriteLine(b.Tables[0].Columns);

        

        DataRow[] results = b.Tables[0].Select();
        foreach (DataRow dr in results) {
            System.Diagnostics.Debug.WriteLine(dr["PlanType"]); 
        }


        foreach (DataColumn column in b.Tables[0].Columns)
        {
            System.Diagnostics.Debug.WriteLine(column.ColumnName);
            System.Diagnostics.Debug.WriteLine(column.DataType);
       
        }

        var table = new List<Dictionary<string, object>>();
        var row = new Dictionary<string, object> { };

        foreach (DataRow dr in results){
            row.Clear();
            foreach (DataColumn column in b.Tables[0].Columns){             
                row.Add(column.ColumnName, dr[column.ColumnName]);
            }
            table.Add(row);
        }

        

        JavaScriptSerializer jss = new JavaScriptSerializer();
        String jsonBack = jss.Serialize(table);

        System.Diagnostics.Debug.WriteLine(jsonBack);
        */

    }
}

