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
    public partial class AddLab : System.Web.UI.Page
    {
        #region Private Variables
        private DataTable dtLabs
        {
            get
            {
                return (ViewState["dtLabs"] == null ? new DataTable("Empty") : ((DataTable)ViewState["dtLabs"]));
            }
            set { ViewState["dtLabs"] = value; }
        }
        #endregion

        #region Protected Properties
        protected String ShowLetters { get { return (ThisSession.ServiceEnteredFrom == "Letters" ? "" : "display:none;"); } }
        protected String ShowText { get { return (ThisSession.ServiceEnteredFrom == "Letters" ? "display:none;" : ""); } }
        #endregion

        #region GUI Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (ThisSession.ChosenLabs == null)
                {
                    dtLabs = new DataTable();
                    dtLabs.Columns.Add(new DataColumn("ServiceID"));
                    dtLabs.Columns.Add(new DataColumn("ServiceName"));
                    dtLabs.Columns.Add(new DataColumn("Description"));

                    AddLabToList(ThisSession.ServiceID.ToString(), ThisSession.ServiceName);
                }
                else
                {
                    rptLabTests.DataSource = dtLabs;
                    rptLabTests.DataBind();
                }
                switch (ThisSession.ServiceEnteredFrom)
                {
                    case "Letters":
                        SetupLetters();
                        break;
                    default:
                        break;
                }
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void rptTestLetters_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            { return; }

            DataRowView drvRow = (DataRowView)e.Item.DataItem;
            LinkButton lbtnLetter = (LinkButton)e.Item.FindControl("lbtnLetter");
            lbtnLetter.Text = drvRow.Row[0].ToString();
            if (Convert.ToBoolean(drvRow.Row["Available"]))
            {
                lbtnLetter.CommandName = "Select";
                lbtnLetter.CommandArgument = drvRow.Row[0].ToString();
            }
            else //We want to replace the link button with the unavailable character
            {
                lbtnLetter.Style.Add("cursor", "default");
                lbtnLetter.Style.Add("color", "#808080");
                lbtnLetter.Enabled = false;
            }
        }
        protected void SelectLetter(object sender, EventArgs e)
        {
            //Which letter were they on?
            LinkButton lbtnLetter = (LinkButton)sender;
            string selectedLetter = lbtnLetter.CommandArgument;

            using (GetLabTestLetterMenuOptions LetterOptions = new GetLabTestLetterMenuOptions(selectedLetter))
            {
                if (LetterOptions.Tables.Count >= 1)
                {
                    rptTestList.DataSource = (LetterOptions.HasErrors ? new DataSet() : LetterOptions);
                    rptTestList.DataBind();

                    rptTestList.Visible = !LetterOptions.HasErrors;
                }
            }
        }
        protected void lbtnTest_OnDataBinding(object sender, EventArgs e)
        {
            LinkButton bindingButton = (LinkButton)sender;
            RepeaterItem container = (RepeaterItem)bindingButton.NamingContainer;
            bindingButton.Text = ((DataRowView)container.DataItem)["ServiceName"].ToString();
            bindingButton.CommandArgument = ((DataRowView)container.DataItem)["ServiceID"].ToString();
            bindingButton.CommandName = "Select";
        }
        protected void SelectOption(object sender, EventArgs e)
        {
            //Which letter were they on?
            LinkButton selectedBtn = (LinkButton)sender;

            //Add the lab to the list
            AddLabToList(selectedBtn.CommandArgument, selectedBtn.Text);

            //Collapse the service list
            rptTestList.Visible = false;

            lnkBtnContinue.Visible = true;
            lblAddMoreServices.Visible = false;
        }
        protected void ContinueSearch(object sender, EventArgs e)
        {
            if (dtLabs.Rows.Count == 1)
            {
                ThisSession.ServiceID = Convert.ToInt16(dtLabs.Rows[0]["ServiceID"].ToString());
                ThisSession.ServiceName = dtLabs.Rows[0]["ServiceName"].ToString();
                ThisSession.ChosenLabs = null;
            }
            else
            {
                ThisSession.ChosenLabs = dtLabs;
            }

            Response.Redirect("results_care.aspx");
        }
        #endregion

        #region Supporting Methods
        public void SetupLetters()
        {
            using (GetLabTestLetterMenu Letters = new GetLabTestLetterMenu())
            {
                if (!Letters.HasErrors)
                {
                    rptTestLetters.DataSource = Letters.TableWithAvailable;
                    rptTestLetters.DataBind();
                }
            }
        }
        private void AddLabToList(String labID, String labName)
        {
            DataRow dr = dtLabs.NewRow();
            dr["ServiceID"] = labID;
            dr["ServiceName"] = labName;
            dr["Description"] = "";
            dtLabs.Rows.Add(dr);

            rptLabTests.DataSource = dtLabs;
            rptLabTests.DataBind();
        }
        protected void RemoveLabTest(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            RepeaterItem container = (RepeaterItem)cb.NamingContainer;
            int indx = 0;
            foreach (RepeaterItem i in rptLabTests.Items)
            {
                if (i != container) { indx++; }
                else { break; }
            }
            dtLabs.Rows.RemoveAt(indx);

            if (dtLabs.Rows.Count == 0)
            {
                lnkBtnContinue.Visible = false;
                lblAddMoreServices.Visible = true;
            }

            rptLabTests.DataSource = dtLabs;
            rptLabTests.DataBind();
        }
        #endregion

        #region Data Classes
        private class GetLabTestLetterMenu : DataSet
        {
            private const String Procedure = "GetLabTestLetterMenu";
            private const CommandType ProcedureType = CommandType.StoredProcedure;
            private String sqlException;
            private String genException;
            private Boolean hasErrors;

            public String SqlException { get { return sqlException; } }
            public String GenException { get { return genException; } }
            public Boolean HasErrors { get { return hasErrors; } }
            public DataTable TableWithAvailable
            {
                get
                {
                    DataTable dtOut = new DataTable("Letters");
                    if (this.Tables.Count >= 1)
                    {
                        dtOut = this.Tables[0].Clone();
                        dtOut.Columns.Add("Available", Type.GetType("System.Boolean"));

                        using (DataTableReader dtrLetters = this.Tables[0].CreateDataReader())
                        {
                            dtrLetters.Read();
                            for (int ascii = 65; ascii <= 90; ascii++)
                            {
                                DataRow newLetter = dtOut.NewRow();
                                newLetter[0] = Convert.ToChar(ascii).ToString();
                                bool avail = (dtrLetters.HasRows ? (Convert.ToChar(dtrLetters[0]) == Convert.ToChar(ascii)) : false);
                                newLetter["Available"] = avail;
                                dtOut.Rows.Add(newLetter);

                                if (avail) { dtrLetters.Read(); }
                            }
                        }
                    }
                    return dtOut;
                }
            }

            public GetLabTestLetterMenu()
            {
                hasErrors = false;
                using (SqlConnection conn = new SqlConnection(ThisSession.CnxString))
                {
                    using (SqlCommand comm = new SqlCommand(Procedure, conn))
                    {
                        comm.CommandType = ProcedureType;
                        using (SqlDataAdapter da = new SqlDataAdapter(comm))
                        {
                            try
                            { da.Fill(this); }
                            catch (SqlException sqEx)
                            { sqlException = sqEx.Message; hasErrors = true; }
                            catch (Exception ex)
                            { genException = ex.Message; hasErrors = true; }
                            finally
                            { }
                        }
                    }
                }
            }
        }
        private class GetLabTestLetterMenuOptions : DataSet
        {
            private const String Procedure = "GetLabTestLetterMenuOptions";
            private const CommandType ProcedureType = CommandType.StoredProcedure;
            private String sqlException;
            private String genException;
            private Boolean hasErrors;
            private SqlParameter letter = new SqlParameter("Letter", SqlDbType.NVarChar, 2);

            public String SqlException { get { return sqlException; } }
            public String GenException { get { return genException; } }
            public Boolean HasErrors { get { return hasErrors; } }
            public String Letter { set { letter.Value = value; } }

            public GetLabTestLetterMenuOptions()
            {
                //We get nothing we do nothing
            }
            public GetLabTestLetterMenuOptions(String Letter)
            {
                hasErrors = false;
                this.Letter = Letter;
                RefreshData();
            }
            public void RefreshData()
            {
                using (SqlConnection conn = new SqlConnection(ThisSession.CnxString))
                {
                    using (SqlCommand comm = new SqlCommand(Procedure, conn))
                    {
                        comm.CommandType = ProcedureType;
                        comm.Parameters.Add(letter);
                        using (SqlDataAdapter da = new SqlDataAdapter(comm))
                        {
                            try
                            { da.Fill(this); }
                            catch (SqlException sqEx)
                            { sqlException = sqEx.Message; hasErrors = true; }
                            catch (Exception ex)
                            { genException = ex.Message; hasErrors = true; }
                            finally
                            { }
                        }
                        comm.Parameters.Clear();
                    }
                }
            }
        }
        #endregion
    }
}