using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using QSEncryption.QSEncryption;

namespace ClearCostWeb.Controls
{
    public partial class FindRx : System.Web.UI.UserControl
    {
        private DataView myDV;
        public DataTable FamilyMedDataTable { set { myDV = new DataView(value); } }
        private String PostBackControl { get { return Request.Params["__EVENTTARGET"].ToString(); } }
        private String PostBackArgument { get { return Request.Params["__EVENTARGUMENT"].ToString(); } }
        private String[] PostBackLatLng { get { return (PostBackControl == "Geocoder" ? PostBackArgument.Split('|') : null); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lsRxSearch.SetupLetters();
            }
            else
            {
                if (hfChosenDrugs.Value != String.Empty)
                {
                    ThisSession.PrevPage = "FindRX"; SearchManagedMed(sender, hfChosenDrugs.Value);
                }
                else
                {
                    if (PostBackControl == "Geocoder") { lsRxSearch.SetupLetters(PostBackLatLng[0], PostBackLatLng[1]); }
                }
            }

        }
        protected void BindFamilyItem(Object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                RepeaterItem riMember = e.Item;
                DataRowView drv = (DataRowView)riMember.DataItem;

                //Setup a list to hold the columns we'll be interested in for each binding
                List<String> Cols = new List<String>();
                //Filter the dataview so that we're only looking at the employee we're binding to in this repeater item
                myDV.RowFilter = "CCHID = " + drv["CCHID"].ToString();

                myDV.Sort = "DrugName ASC";

                //Bind the Employee Name
                Cols.Add("FirstName");
                Cols.Add("LastName");
                Label lblRXEmployeeName = (Label)riMember.FindControl("lblRXEmployeeName");
                DataTable tblForLabel = myDV.ToTable(true, Cols.ToArray());
                lblRXEmployeeName.Text = String.Format("{0} {1}",
                                            tblForLabel.Rows[0]["FirstName"].ToString(),
                                            tblForLabel.Rows[0]["LastName"].ToString());
                //END EMPLOYEE BIND

                //Bind the meds for that employee
                Cols.Clear();
                Cols.Add("GPI");
                Cols.Add("DrugName");
                Cols.Add("DrugID");
                Cols.Add("Strength");
                Cols.Add("Quantity");
                Cols.Add("QuantityUOM");
                Cols.Add("PastCareID");
                Repeater rptMemberMeds = (Repeater)riMember.FindControl("rptMemberMeds");
                DataTable tblForMeds = myDV.ToTable(true, Cols.ToArray());
                DataColumn dispCol = new DataColumn("DisplayText");
                tblForMeds.Columns.Add(dispCol);

                foreach (DataRow dr in tblForMeds.Rows)
                {
                    dr["DisplayText"] = dr["DrugName"].ToString().Trim();
                    if (dr["Strength"].ToString().Trim() != String.Empty)
                        dr["DisplayText"] += ", " + dr["Strength"].ToString().Trim();
                    if (dr["Quantity"].ToString().Trim() != String.Empty)
                        dr["DisplayText"] += ", " + String.Format("{0:#0}", Convert.ToDecimal(dr["Quantity"].ToString().Trim()));
                    if (dr["QuantityUOM"].ToString().Trim() != String.Empty)
                        dr["DisplayText"] += " " + dr["QuantityUOM"].ToString().Trim();
                }

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
                //CheckBox cb = (CheckBox)riMed.FindControl("cbMemberMed");
                RadioButton rb = (RadioButton)riMed.FindControl("rbMemberMed");

                rb.Text = drv["DisplayText"].ToString();
                //cb.Text = drv["DisplayText"].ToString();

                QueryStringEncryption qse = new QueryStringEncryption();
                qse.UserKey = new Guid(ThisSession.UserLogginID);
                qse["GPI"] = drv["GPI"].ToString().Trim();
                qse["PastCareID"] = drv["PastCareID"].ToString().Trim();
                qse["DrugID"] = drv["DrugID"].ToString().Trim();
                qse["Quantity"] = drv["Quantity"].ToString().Trim();
                qse["QuantityUOM"] = drv["QuantityUOM"].ToString().Trim();
                qse["DrugName"] = drv["DrugName"].ToString().Trim();
                qse["Strength"] = drv["Strength"].ToString().Trim();

                rb.Attributes.Add("n", qse.ToString());
                //cb.Attributes.Add("n", qse.ToString());
            }
        }
        protected void SearchManagedMed(object sender, String e)
        {
            if (!e.ToLower().Contains("undefined"))
            {
                String[] drugs = e.Split('|');

                DataTable dt = new DataTable("MultiRX");
                dt.Columns.Add(new DataColumn("DrugID", Type.GetType("System.Int32")));
                dt.Columns.Add(new DataColumn("GPI", Type.GetType("System.String")));
                dt.Columns.Add(new DataColumn("Quantity", Type.GetType("System.Double")));
                dt.Columns.Add(new DataColumn("PastCareID", Type.GetType("System.Int32")));
                dt.Columns.Add(new DataColumn("QuantityUOM", Type.GetType("System.String")));
                dt.Columns.Add(new DataColumn("DrugName", Type.GetType("System.String")));
                dt.Columns.Add(new DataColumn("DrugStrength", Type.GetType("System.String")));
                for (int i = 0; i < (drugs.Length - 1); i++)
                {
                    QueryStringEncryption qse = new QueryStringEncryption(drugs[i], new Guid(ThisSession.UserLogginID));
                    DataRow dr = dt.NewRow();
                    dr["GPI"] = qse["GPI"].ToString();
                    dr["PastCareID"] = qse["PastCareID"].ToString();
                    dr["DrugID"] = qse["DrugID"].ToString();
                    dr["Quantity"] = qse["Quantity"].ToString();
                    dr["QuantityUOM"] = qse["QuantityUOM"].ToString();
                    dr["DrugName"] = qse["DrugName"].ToString();
                    dr["DrugStrength"] = qse["Strength"].ToString();
                    dt.Rows.Add(dr);
                }
                ThisSession.ChosenDrugs = dt;
                Response.Redirect("results_rx.aspx");
            }
        }
        public void SetupFamilyMembers()
        {
            rptFamilyMeds.DataSource = myDV.ToTable(true, "CCHID");
            rptFamilyMeds.DataBind();
        }
    }
}