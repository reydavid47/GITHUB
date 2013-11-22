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
    public partial class switch_steps : System.Web.UI.Page
    {
        #region Private Variables
        private GetTheraSubPageContent gtspc = new GetTheraSubPageContent();
        #endregion

        #region Protected Properties
        protected String PrescribingDoctor { get; set; }
        protected String SaveTotal { get; set; }
        protected String DocTitle { get { String[] DocSegs = PrescribingDoctor.Split(' '); return (DocSegs.Length == 1 ? "Dr." : DocSegs[0]); } }
        protected String DocFirstName { get { String[] DocSegs = PrescribingDoctor.Split(' '); return (DocSegs.Length == 1 ? "" : DocSegs[1]); } }
        protected String DocLastName { get { String[] DocSegs = PrescribingDoctor.Split(' '); return (DocSegs.Length == 1 ? DocSegs[0] : DocSegs[2]); } }
        protected String PatientName { get; set; }
        #endregion

        #region GUI Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                abCouldSave.SaveTotal = SaveTotal = ThisSession.YouCouldHaveSavedDisplay;

                gtspc.PastCareIDList = ThisSession.PastCareID;
                gtspc.GetData();
                if (!gtspc.HasErrors)
                {
                    rptMemberSubs.DataSource = gtspc.MembersTable;
                    rptMemberSubs.DataBind();

                    PrescribingDoctor = gtspc.PrescribingDoctor;
                    PatientName = gtspc.PatientName;

                    rptSearchQuery.DataSource = gtspc.SubDetailsTable;

                    therasub2.DataBind();
                }
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void BindMed(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                RepeaterItem member = (RepeaterItem)e.Item;
                Repeater rptMeds = (Repeater)member.FindControl("rptMeds");

                rptMeds.DataSource = gtspc.GetMemberMed(((DataRowView)e.Item.DataItem)["CCHID"].ToString());
                rptMeds.DataBind();
            }
        }
        protected void ShareResults(object sender, EventArgs e)
        {
            //We need to setup a method for emailing and do that here

            Response.Redirect("Search.aspx#tabrx");
        }
        #endregion
    }
}