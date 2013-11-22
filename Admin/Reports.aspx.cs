using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;

namespace ClearCostWeb.Admin
{
    public partial class Reports : System.Web.UI.Page
    {
        private SqlConnectionStringBuilder csb;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                csb = new SqlConnectionStringBuilder();
                csb.DataSource = "199.79.49.163";
                csb.InitialCatalog = "CCH_FrontEnd2";
                csb.IntegratedSecurity = false;
                csb.Password = "de2Aiyoomie5ahw6";
                csb.UserID = "samb01sa";

                using (SqlConnection conn = new SqlConnection(csb.ConnectionString.ToString()))
                {
                    String ddlQuery = @"SELECT EmployerName, ConnectionString FROM [Employers] ORDER BY EmployerName";
                    using (SqlCommand comm = new SqlCommand(ddlQuery, conn))
                    {
                        comm.CommandType = CommandType.Text;
                        using (SqlDataAdapter da = new SqlDataAdapter(comm))
                        {
                            DataSet ds = new DataSet();
                            try
                            { da.Fill(ds); }
                            catch (Exception)
                            { }
                            finally
                            { if (conn != null && conn.State != ConnectionState.Closed) { conn.Close(); } }
                            ddlEmployers.DataSource = ds.Tables[0];
                            ddlEmployers.DataBind();
                        }
                    }
                }
            }
        }
        protected void ShowErrors(object sender, EventArgs e)
        {
            pnlErrors.Visible = true;
            pnlLogins.Visible = false;
            upAdmin.Update();
        }
        protected void ShowLogins(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection())
            {

            }
            pnlErrors.Visible = false;
            pnlErrors.Visible = true;
        }
        protected void UpdateDVErrors(object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            using (GetUIErrorList guel = new GetUIErrorList())
            {
                if (!guel.HasErrors)
                {
                    DataView dv = new DataView(guel.Tables[0]);
                    dv.RowFilter = "table = '" + gv.SelectedDataKey.Values[0] + "' and ErrID='" + gv.SelectedDataKey.Values[1] + "'";
                    dvError.DataSource = dv.ToTable();
                    dvError.DataBind();
                }
            }
        }
        protected void ShowRegistered(object sender, EventArgs e)
        {
            SqlConnectionStringBuilder sbConn = new SqlConnectionStringBuilder(WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString);
            sbConn.InitialCatalog = "CCH_StarbucksWeb";
            using (GetRegisterCounts grc = new GetRegisterCounts())
            {
                grc.GetData(sbConn.ConnectionString);
                if (!grc.HasErrors && grc.RowsBack > 0)
                {
                    gvRegistered.DataSource = grc.Counts;
                    gvRegistered.DataBind();
                    pnlRegistered.Visible = true;
                }
            }
        }
    }
}