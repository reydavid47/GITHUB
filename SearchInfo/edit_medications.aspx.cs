using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace ClearCostWeb.SearchInfo
{
    public partial class edit_medications : System.Web.UI.Page
    {
        #region Private Variables

        private const string PastCareRxProcName = "GetPastCareRX";
        private Dictionary<String, Guid> attributeMasks
        {
            get
            {
                return (ViewState["attributeMasks"] == null ? new Dictionary<String, Guid>() : ((Dictionary<String, Guid>)ViewState["attributeMasks"]));
            }
            set { ViewState["attributeMasks"] = value; }
        }
        private DataTable myDT
        {
            get
            {
                return (ViewState["myDT"] == null ? new DataTable("Empty") : ((DataTable)ViewState["myDT"]));
            }
            set { ViewState["myDT"] = value; }
        }
        #endregion

        #region GUI Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SqlParameter empId = new SqlParameter("EmployeeID", SqlDbType.NVarChar, 50);
                empId.Value = ThisSession.EmployeeID;

                using (DataSet ds = GetPastCareRxFromDB(empId, ThisSession.CnxString))
                {
                    if (ds.Tables.Count >= 1)
                    {
                        myDT = ds.Tables[0];
                        SetupFamilyMembers();
                    }
                }
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void SearchClickedDrug(object sender, EventArgs e)
        {
            LinkButton lbtnMed = (LinkButton)sender;
            ThisSession.DrugGPI = lbtnMed.Attributes[attributeMasks["GPI"].ToString()].ToString();
            ThisSession.PastCareID = lbtnMed.Attributes[attributeMasks["PastCareID"].ToString()].ToString();
            ThisSession.DrugID = lbtnMed.Attributes[attributeMasks["DrugID"].ToString()].ToString();

            String MedText = lbtnMed.Text;
            ThisSession.DrugName = (MedText.Split(' '))[0].ToString();
            ThisSession.DrugStrength = (MedText.Split(' '))[1].ToString();
            ThisSession.DrugQuantity = (MedText.Split(' '))[3].ToString();

            Response.Redirect("results_rx.aspx");
        }
        protected void DeleteClickedDrug(object sender, EventArgs e)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            LinkButton s = (LinkButton)sender;
            String[] args = s.CommandArgument.Split('|');

            SqlParameter prm = new SqlParameter("EmployeeID", SqlDbType.NVarChar, 50);
            prm.Value = args[0];
            parms.Add(prm);

            prm = new SqlParameter("CCHID", SqlDbType.Int);
            prm.Value = args[1];
            parms.Add(prm);

            prm = new SqlParameter("GPI", SqlDbType.NVarChar, 20);
            prm.Value = args[2];
            parms.Add(prm);

            prm = new SqlParameter("Email", SqlDbType.NVarChar, 150);
            prm.Value = args[3];
            parms.Add(prm);

            DeleteMedFromMember(parms, ThisSession.CnxString);
        }
        protected void BindFamilyItem(Object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                RepeaterItem riMember = e.Item;
                DataRowView drv = (DataRowView)riMember.DataItem;

                //Setup the dataview we'll use for the special bindings based off of our static table
                DataView dv = new DataView(myDT);
                //Setup a list to hold the columns we'll be interested in for each binding
                List<String> Cols = new List<String>();
                //Filter the dataview so that we're only looking at the employee we're binding to in this repeater item
                dv.RowFilter = "CCHID = " + drv["CCHID"].ToString();

                //Bind the Employee Name
                Cols.Add("FirstName");
                Cols.Add("LastName");
                Label lblRXEmployeeName = (Label)riMember.FindControl("lblRXEmployeeName");
                DataTable tblForLabel = dv.ToTable(true, Cols.ToArray());
                lblRXEmployeeName.Text = String.Format("{0} {1}",
                                            tblForLabel.Rows[0]["FirstName"].ToString(),
                                            tblForLabel.Rows[0]["LastName"].ToString());
                //END EMPLOYEE BIND

                //Bind the meds for that employee
                Cols.Clear();
                Cols.Add("GPI");
                Cols.Add("CCHID");
                Cols.Add("DrugName");
                Cols.Add("DrugID");
                Cols.Add("Strength");
                Cols.Add("PastCareID");
                Cols.Add("Quantity");
                Cols.Add("PharmacyName");
                Cols.Add("City");
                Cols.Add("QuantityUOM");
                Repeater rptMemberMeds = (Repeater)riMember.FindControl("rptMemberMeds");
                DataTable tblForMeds = dv.ToTable(true, Cols.ToArray());

                rptMemberMeds.ItemDataBound += new RepeaterItemEventHandler(BindMed);

                rptMemberMeds.DataSource = tblForMeds;
                rptMemberMeds.DataBind();
                //END MED BIND
            }
        }
        private void BindMed(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RepeaterItem riMed = e.Item;
                DataRowView drv = (DataRowView)riMed.DataItem;
                LinkButton lbtnMed = (LinkButton)riMed.FindControl("lbtnMed");
                LinkButton lbtnDeleteMed = (LinkButton)riMed.FindControl("lbtnDeleteMed");
                Label lblPharmacy = (Label)riMed.FindControl("lblPharmacy");


                lbtnMed.Text = String.Format("{0} {1} - {2:f0} {3}",
                    drv["DrugName"].ToString(),
                    drv["Strength"].ToString(),
                    Decimal.Parse(drv["Quantity"].ToString()),
                    drv["QuantityUOM"].ToString());
                lbtnMed.Attributes.Add(attributeMasks["GPI"].ToString(), drv["GPI"].ToString());
                lbtnMed.Attributes.Add(attributeMasks["PastCareID"].ToString(), drv["PastCareID"].ToString());
                lbtnMed.Attributes.Add(attributeMasks["DrugID"].ToString(), drv["DrugID"].ToString());

                lbtnDeleteMed.CommandArgument = String.Format("{0}|{1}|{2}|{3}",
                    ThisSession.EmployeeID,
                    drv["CCHID"].ToString(),
                    drv["GPI"].ToString(),
                    "");

                lblPharmacy.Text = String.Format("{0} ({1})", drv["PharmacyName"].ToString(), drv["City"].ToString());
            }
        }
        #endregion

        #region Helper Methods
        public void SetupFamilyMembers()
        {
            DataView dvFamily = new DataView(myDT);

            List<String> neededCols = new List<string>();
            neededCols.Add("CCHID");

            //JM 3/22/2012
            //Due to the number of nested repeaters and post back bugs, I've created a custom postback java call for the search button
            //Because the arguments that are posted back come directly from the client side of things at the time of click
            //I'm masking the custom attributes we need with GUIDs that are randomly generated upon page visit to prevent tampering/hacking
            //NOTE:This issue should go away when we move to Check boxes instead of radio buttons for Multi-Rx
            attributeMasks = new Dictionary<string, Guid>();
            attributeMasks.Add("GPI", Guid.NewGuid());
            attributeMasks.Add("PastCareID", Guid.NewGuid());
            attributeMasks.Add("DrugID", Guid.NewGuid());

            rptFamilyMeds.DataSource = dvFamily.ToTable(true, neededCols.ToArray());
            rptFamilyMeds.DataBind();
        }
        private DataSet GetPastCareRxFromDB(SqlParameter EmployeeID, String CnxString)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(CnxString))
            {
                using (SqlCommand comm = new SqlCommand(PastCareRxProcName, conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.Add(EmployeeID);
                    comm.Parameters.Add(new SqlParameter("CCHID", ThisSession.CCHID));

                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try
                        {
                            da.Fill(ds);
                        }
                        catch (SqlException sqEx)
                        { }
                        catch (Exception ex)
                        { }
                        finally
                        { }
                    }

                    comm.Parameters.Clear();
                }
            }
            return ds;
        }
        private int DeleteMedFromMember(List<SqlParameter> inpParms, String CnxString)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(CnxString))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand("DeleteRX", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    inpParms.ForEach(delegate(SqlParameter prm)
                    {
                        comm.Parameters.Add(prm);
                    });

                    try
                    {
                        rowsAffected = comm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    { }
                    finally
                    { }

                    comm.Parameters.Clear();
                }
                conn.Close();
            }
            return rowsAffected;
        }
        #endregion
    }
}