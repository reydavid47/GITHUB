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
    public partial class switch_rx : System.Web.UI.Page
    {
        private GetPastCareRX gpcr = new GetPastCareRX();

        #region GUI Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                gpcr.EmployeeID = ThisSession.EmployeeID;
                gpcr.CCHID = ThisSession.CCHID;
                gpcr.GetData();
                if (!gpcr.HasErrors)
                {
                    if (gpcr.SaveTable.Rows.Count > 0)
                        abCouldSave.SaveTotal = gpcr.SaveTotal;

                    rptTheraSubData.DataSource = gpcr.DistinctTherasubMembers;
                    rptTheraSubData.DataBind();

                    ThisSession.PastCareID = String.Empty;
                    gpcr.ForEachTheraSubMemberPastCareID(delegate(String PastCareID)
                    {
                        ThisSession.PastCareID += PastCareID + "|";
                    });
                    ThisSession.PastCareID = ThisSession.PastCareID.TrimEnd('|');
                }
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void BindMeds(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                RepeaterItem member = (RepeaterItem)e.Item;
                Panel pnlMember = (Panel)member.FindControl("pnlMember");
                Panel pnlMemberMed = (Panel)pnlMember.FindControl("pnlMemberMed");
                Repeater rptMembersMeds = (Repeater)pnlMember.FindControl("rptMembersMeds");

                using (DataView dv = new DataView(gpcr.TheraSubTable))
                {
                    dv.RowFilter = String.Format("CCHID = '{0}'", ((DataRowView)e.Item.DataItem)["CCHID"].ToString());
                    rptMembersMeds.DataSource = dv.ToTable("Membermeds", false,
                        new String[] { "DrugName", "Strength", "ReplacementDrugName", "ReplacementStrength", "TotalPotentialSavings" });
                    rptMembersMeds.DataBind();
                }
            }
        }
        #endregion
    }
}