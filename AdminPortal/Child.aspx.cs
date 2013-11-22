using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class AdminPortal_Child : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      
        /*
        TableHeaders.DataSource = GetTableHeaders();
        TableHeaders.DataBind();
        TableRows.DataSource = GetTableRows();
        TableRows.DataBind();
         */
    }

    public Array GetTableHeaders()
    {
        System.Diagnostics.Debug.Write("begin function");
        string[] headers = { "Name", "Value", "Date" };
        return headers;
    }

    public Array GetTableRows()
    {
        string[,] rows = new string[,] 
            { 
                {"Registered","1000", "01/01/2012"}, 
                {"Unregistered","41000", "01/01/2012"} 
            };

        return rows;
    }
}