using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;

namespace ClearCostWeb.Controls
{
    public partial class LetterSearch : System.Web.UI.UserControl
    {
        #region Private Variables
        private SqlParameter prmSelectedLetter = new SqlParameter("Letter", SqlDbType.NVarChar, 2);
        private SqlParameter prmLatForRx = new SqlParameter("Latitude", SqlDbType.Float);
        private SqlParameter prmLngForRx = new SqlParameter("Longitude", SqlDbType.Float);
        private RunMethod _runMethod;
        #endregion

        #region Supporting Enums and Dictionaries
        public enum RunMethod { SearchPage, RXPage }
        private enum LabTestProcs { GetAvailableLetters, GetLetterOptions }
        private static Dictionary<LabTestProcs, String> AvailableLTProcs = new Dictionary<LabTestProcs, String>{
        { LabTestProcs.GetAvailableLetters, "GetLabTestLetterMenu" },
        { LabTestProcs.GetLetterOptions, "GetLabTestLetterMenuOptions" }
    };
        private enum RxProcs { GetDrugLetterMenu, GetDrugLetterMenuOptions }
        private static Dictionary<RxProcs, String> AvailableRxProcs = new Dictionary<RxProcs, string>{
        { RxProcs.GetDrugLetterMenu, "GetDrugLetterMenu" },
        {RxProcs.GetDrugLetterMenuOptions, "GetDrugLetterMenuOptions" }
    };
        #endregion

        #region Public Properties
        [DefaultValue(RunMethod.SearchPage)]
        [Browsable(true)]
        [Description("The Run Method the search control needs to use")]
        public RunMethod runMethod
        {
            get { return _runMethod; }
            set { _runMethod = value; }
        }
        #endregion

        #region GUI Methods
        protected void Page_Load(object sender, EventArgs e)
        { }
        public void SetupLetters()
        {
            SetupLetters(ThisSession.PatientLatitude, ThisSession.PatientLongitude);
        }
        public void SetupLetters(String Latitude, String Longitude)
        {
            DataSet Letters;
            switch (_runMethod)
            {
                case RunMethod.SearchPage:
                    Letters = TestQueryFor(LabTestProcs.GetAvailableLetters, ThisSession.CnxString);
                    break;
                case RunMethod.RXPage:
                    List<SqlParameter> prmInp = new List<SqlParameter>();
                    prmLatForRx.Value = Convert.ToDouble(Latitude);
                    prmInp.Add(prmLatForRx);
                    prmLngForRx.Value = Convert.ToDouble(Longitude);
                    prmInp.Add(prmLngForRx);
                    Letters = TestQueryFor(RxProcs.GetDrugLetterMenu, prmInp, ThisSession.CnxString);
                    break;
                default:
                    Letters = new DataSet();
                    break;
            }
            if (Letters.Tables.Count >= 1)
            {
                DataTable dtOut = Letters.Tables[0].Clone();
                dtOut.Columns.Add("Available", Type.GetType("System.Boolean"));

                using (DataTableReader dtrLetters = Letters.Tables[0].CreateDataReader())
                {
                    dtrLetters.Read(); //Get the reader started
                    for (int ascii = 65; ascii <= 90; ascii++)
                    {
                        DataRow newLetter = dtOut.NewRow();
                        newLetter[0] = Convert.ToChar(ascii).ToString();
                        bool avail = (dtrLetters.HasRows ? (Convert.ToChar(dtrLetters[0]) == Convert.ToChar(ascii)) : false); //Make every letter gray if we have no available services
                        newLetter["Available"] = avail;
                        dtOut.Rows.Add(newLetter);

                        if (avail) { dtrLetters.Read(); }
                    }
                }

                rptTestLetters.DataSource = dtOut;
                rptTestLetters.DataBind();
            }
        }
        protected void lbtnTest_OnDataBinding(object sender, EventArgs e)
        {
            LinkButton lbtnTest = (LinkButton)sender;
            RepeaterItem container = (RepeaterItem)lbtnTest.NamingContainer;
            switch (_runMethod)
            {
                case RunMethod.SearchPage:
                    lbtnTest.Text = ((DataRowView)container.DataItem)["ServiceName"].ToString();
                    break;
                case RunMethod.RXPage:
                    lbtnTest.Text = ((DataRowView)container.DataItem)["DrugName"].ToString().ToUpper();
                    lbtnTest.Attributes.Add("DrugID", ((DataRowView)container.DataItem)["DrugID"].ToString());
                    break;
                default:
                    lbtnTest.Text = string.Empty;
                    break;
            }
        }
        protected void SelectLetter(object sender, EventArgs e)
        {
            //Which letter were they on?
            LinkButton lbtnLetter = (LinkButton)sender;
            string selectedLetter = lbtnLetter.CommandArgument;

            prmSelectedLetter.Value = selectedLetter;
            DataSet letterOptions;
            switch (_runMethod)
            {
                case RunMethod.SearchPage:
                    letterOptions = TestQueryFor(LabTestProcs.GetLetterOptions, prmSelectedLetter, ThisSession.CnxString);
                    break;
                case RunMethod.RXPage:
                    List<SqlParameter> prmInp = new List<SqlParameter>();
                    prmLatForRx.Value = Convert.ToDouble(ThisSession.PatientLatitude);
                    prmInp.Add(prmLatForRx);
                    prmLngForRx.Value = Convert.ToDouble(ThisSession.PatientLongitude);
                    prmInp.Add(prmLngForRx);
                    prmInp.Add(prmSelectedLetter);
                    letterOptions = TestQueryFor(RxProcs.GetDrugLetterMenuOptions, prmInp, ThisSession.CnxString);

                    break;
                default:
                    letterOptions = new DataSet();
                    break;
            }
            if (letterOptions.Tables.Count >= 1)
            {
                //Get List of Lab Tests for that letter
                rptTestList.DataSource = letterOptions;
                rptTestList.DataBind();
            }
        }
        protected void rptTestLetters_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
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
        protected void rptTestList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            { return; }

            DataRowView drv = (DataRowView)e.Item.DataItem;

            LinkButton lbtnTest = (LinkButton)e.Item.FindControl("lbtnTest");
            lbtnTest.CommandName = "Select";
            switch (_runMethod)
            {
                case RunMethod.SearchPage:
                    lbtnTest.CommandArgument = drv["ServiceID"].ToString();
                    break;
                case RunMethod.RXPage:
                    lbtnTest.CommandArgument = drv["DrugName"].ToString();
                    break;
                default:
                    lbtnTest.CommandArgument = string.Empty;
                    break;
            }

        }
        protected void SelectOption(object sender, EventArgs e)
        {
            //Which letter were they on?
            LinkButton lbtnTest = (LinkButton)sender;

            //go to results
            switch (_runMethod)
            {
                case RunMethod.SearchPage:
                    ThisSession.ServiceID = Convert.ToInt16(lbtnTest.CommandArgument);
                    ThisSession.ServiceName = lbtnTest.Text;

                    //if (lbtnTest.Text != lbtnTest.CommandArgument)
                    //{
                    //ThisSession.ServiceEntered = lbtnTest.Text;
                    ThisSession.ServiceEnteredFrom = "Letters";
                    //}
                    //NOTE: 7/14 JM
                    //Temporarily removing multi-lab capability for the starbucks go-live as the CCH believes it isn't working
                    Response.Redirect("results_care.aspx");
                    //Response.Redirect("AddLab.aspx#tabcare");
                    break;
                case RunMethod.RXPage:
                    ThisSession.DrugName = lbtnTest.CommandArgument;
                    ThisSession.DrugEnteredFrom = "Letters";
                    ThisSession.DrugID = lbtnTest.Attributes["DrugID"].ToString();

                    if (lbtnTest.Text != lbtnTest.CommandArgument)
                    {
                        ThisSession.DrugEntered = lbtnTest.Text;
                    }
                    Response.Redirect("results_rx_name.aspx");
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Supporting Methods
        private DataSet TestQueryFor(LabTestProcs ProcToRun, String CnxString)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(CnxString))
            {
                using (SqlCommand comm = new SqlCommand(AvailableLTProcs[ProcToRun], conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try
                        {
                            da.Fill(ds);
                        }
                        catch (Exception)
                        { }
                        finally
                        { }
                    }
                }
            }
            return ds;
        }
        private DataSet TestQueryFor(LabTestProcs ProcToRun, SqlParameter SelectedLetter, String CnxString)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(CnxString))
            {
                using (SqlCommand comm = new SqlCommand(AvailableLTProcs[ProcToRun], conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.Add(SelectedLetter);

                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try
                        {
                            da.Fill(ds);
                        }
                        catch (Exception)
                        { }
                        finally
                        { }
                    }

                    comm.Parameters.Clear();
                }
            }
            return ds;
        }

        private DataSet TestQueryFor(RxProcs ProcToRun, String CnxString)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(CnxString))
            {
                using (SqlCommand comm = new SqlCommand(AvailableRxProcs[ProcToRun], conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try
                        {
                            da.Fill(ds);
                        }
                        catch (Exception)
                        { }
                        finally
                        { }
                    }
                }
            }
            return ds;
        }
        private DataSet TestQueryFor(RxProcs ProcToRun, List<SqlParameter> Parms, String CnxString)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(CnxString))
            {
                using (SqlCommand comm = new SqlCommand(AvailableRxProcs[ProcToRun], conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    Parms.ForEach(delegate(SqlParameter parm)
                    {
                        comm.Parameters.Add(parm);
                    });

                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try
                        {
                            da.Fill(ds);
                        }
                        catch (Exception)
                        { }
                        finally
                        { }
                    }

                    comm.Parameters.Clear();
                }
            }
            return ds;
        }
        #endregion
    }
}