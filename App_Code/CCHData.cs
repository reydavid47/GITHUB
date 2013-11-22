using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using QSEncryption.QSEncryption;
using System.Diagnostics;

namespace ClearCostWeb
{
    [System.ComponentModel.DesignerCategory("")]
    public static class CCHSerializer
    {
        [DebuggerStepThrough]
        public static T DeserializeSettings<T>(NameValueCollection nvc) where T : struct
        {
            T result = new T();
            Type type = typeof(T);
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                string key = field.Name;
                string stringValue = nvc[key];
                if (stringValue != null)
                {
                    object value;
                    var baseType = Nullable.GetUnderlyingType(field.FieldType);
                    if (baseType != null)
                        value = Convert.ChangeType(stringValue, baseType);
                    else
                        value = Convert.ChangeType(stringValue, field.FieldType);
                    field.SetValueDirect(__makeref(result), value);
                }
            }
            return result;
        }
        public static T DeserializeDataRow<T>(object dr) where T : struct
        {
            T result = new T();
            if (typeof(DataRow) == dr.GetType())
            {
                DataRow asRow = (DataRow)dr;
                Type type = typeof(T);
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    string key = field.Name;
                    string stringValue = (asRow.Table.Columns.Contains(key) ? asRow[key].ToString() : null);
                    if (stringValue != null)
                    {
                        object value;
                        var baseType = Nullable.GetUnderlyingType(field.FieldType);
                        if (baseType != null)
                            value = Convert.ChangeType(stringValue, baseType);
                        else
                            value = Convert.ChangeType(stringValue, field.FieldType);
                        field.SetValueDirect(__makeref(result), value);
                    }
                }
            }
            return result;
        }
    }

    public class CCHParamList : CollectionBase, IDisposable
    {
        /// <summary>
        /// Retrieve a Parameter in the list by list index
        /// </summary>
        /// <param name="Index">Index in the collection to retrieve</param>
        /// <returns>System.Data.SqlClient.SqlParameter</returns>
        /// <example>this[1].Value = "Test"</example>
        public SqlParameter this[int Index]
        {
            get { return (SqlParameter)List[Index]; }
        }
        /// <summary>
        /// Retrieve a Parameter in the list by the Parameter Name
        /// </summary>
        /// <param name="ParameterName">Name of the SqlParameter to retrieve</param>
        /// <returns>System.Data.SqlClient.SqlParameter</returns>
        /// <example>this["CCHID"].Value = 1</example>
        public SqlParameter this[String ParameterName]
        {
            get
            {
                SqlParameter outParam = null;
                foreach (SqlParameter p in List) { if (p.ParameterName == ParameterName) { outParam = p; break; } }
                return outParam;
            }
        }
        /// <summary>
        /// Adds an already created System.Data.SqlClient.SqlParameter to the list.  If it already exists it is updated
        /// </summary>
        /// <param name="Parameter">System.Data.SqlClient.SqlParameter</param>
        //[DebuggerStepThrough]
        public void Add(SqlParameter Parameter)
        {
            if (!List.Contains(Parameter))
                List.Add(Parameter);
            else
                this[Parameter.ParameterName].Value = Parameter.Value;
        }
        /// <summary>
        /// Creates a new System.Data.SqlClient.SqlParameter and adds it to the list
        /// </summary>
        /// <param name="ParameterName"></param>
        /// <param name="Type"></param>
        /// <param name="Value"></param>
        /// <param name="Size"></param>
        //[DebuggerStepThrough]
        public void New(String ParameterName, SqlDbType Type, int Size = 0, Object Value = null)
        {
            SqlParameter spTemp = null;
            if (Type == SqlDbType.NVarChar)
                spTemp = new SqlParameter(ParameterName, Type, Size);
            else
                spTemp = new SqlParameter(ParameterName, Type);
            spTemp.Value = Value;
            List.Add(spTemp);
        }
        public Boolean Has(String ParamName)
        {
            return (this[ParamName] != null);
        }
        /// <summary>
        /// ForEach SqlParameter in the list, do the specified action
        /// </summary>
        /// <typeparam name="SqlParameter"></typeparam>
        /// <param name="action"></param>
        //[DebuggerStepThrough]
        public void ForEach<SqlParameter>(Action<SqlParameter> action)
        { foreach (SqlParameter item in List) action(item); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~CCHParamList()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this != null)
                this.Dispose();
        }
    }
    public class BaseCCHData : DataSet
    {
        #region Variables
        private String Procedure = String.Empty;
        private CommandType ProcedureType = CommandType.StoredProcedure;
        protected String sqlException = String.Empty;
        protected String genException = String.Empty;
        protected Boolean hasErrors = false;
        private Int32 rowsBack = 0;
        protected CCHParamList Parameters = new CCHParamList();
        #endregion

        #region properties
        public String SqlException { get { return sqlException; } }
        public String GenException { get { return genException; } }
        public Boolean HasErrors { get { return this.hasErrors; } }
        public Int32 RowsBack { get { return rowsBack; } }
        public Boolean HasParameters { get { return this.Parameters.Count > 0; } }
        public String OrderDirection
        {
            set
            {
                if (this.Parameters["OrderDirection"] == null)
                    this.Parameters.New("OrderDirection", SqlDbType.NVarChar, 4, value);
                else
                    this.Parameters["OrderDirection"].Value = value;
            }
        }
        public String OrderByField
        {
            set
            {
                if (this.Parameters["OrderByField"] == null)
                    this.Parameters.New("OrderByField", SqlDbType.NVarChar, 50, value);
                else
                    this.Parameters["OrderByField"].Value = value;
            }
        }
        #endregion

        public BaseCCHData() { }
        public BaseCCHData(String ProcedureName, Boolean IsRawQuery = false)
            : base(ProcedureName)
        { this.Procedure = ProcedureName; if (IsRawQuery) ProcedureType = CommandType.Text; }
        public virtual void GetData()
        {
            using (SqlConnection conn = new SqlConnection(ThisSession.CnxString))
            {
                using (SqlCommand comm = new SqlCommand(Procedure, conn))
                {
                    comm.CommandType = ProcedureType;
                    if (this.HasParameters)
                        this.Parameters.ForEach(delegate(SqlParameter Parameter)
                        {
                            comm.Parameters.Add(Parameter);
                        });
                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try
                        { rowsBack = da.Fill(this); }
                        catch (SqlException sqEx)
                        { CaptureError(sqEx); }
                        catch (Exception Ex)
                        { CaptureError(Ex); }
                        finally
                        { if (conn != null && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    }
                    comm.Parameters.Clear();
                }
            }
        }
        public virtual void GetData(String ConnectionString)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand(Procedure, conn))
                {
                    comm.CommandType = ProcedureType;
                    if (this.HasParameters)
                        this.Parameters.ForEach(delegate(SqlParameter Parameter)
                        {
                            comm.Parameters.Add(Parameter);
                        });
                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try
                        { rowsBack = da.Fill(this); }
                        catch (SqlException sqEx)
                        { CaptureError(sqEx); }
                        catch (Exception Ex)
                        { CaptureError(Ex); }
                        finally
                        { if (conn != null && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    }
                    comm.Parameters.Clear();
                }
            }
        }
        public virtual void GetFrontEndData()
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand(Procedure, conn))
                {
                    comm.CommandType = ProcedureType;
                    if (this.HasParameters)
                        this.Parameters.ForEach(delegate(SqlParameter Parameter)
                        { comm.Parameters.Add(Parameter); });
                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try
                        { rowsBack = da.Fill(this); }
                        catch (SqlException sqEx)
                        { CaptureError(sqEx); }
                        catch (Exception ex)
                        { CaptureError(ex); }
                        finally
                        { if (conn != null && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    }
                    comm.Parameters.Clear();
                }
            }
        }
        public virtual void PostData()
        {
            using (SqlConnection conn = new SqlConnection(ThisSession.CnxString))
            {
                using (SqlCommand comm = new SqlCommand(Procedure, conn))
                {
                    comm.CommandType = ProcedureType;
                    if (this.HasParameters)
                        this.Parameters.ForEach(delegate(SqlParameter Parameter) { comm.Parameters.Add(Parameter); });
                    try
                    { conn.Open(); rowsBack = comm.ExecuteNonQuery(); }
                    catch (SqlException sqEx)
                    { CaptureError(sqEx); }
                    catch (Exception Ex)
                    { CaptureError(Ex); }
                    finally
                    { if (conn != null && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    comm.Parameters.Clear();
                }
            }
        }
        public virtual void PostFrontEndData()
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand(Procedure, conn))
                {
                    comm.CommandType = ProcedureType;
                    if (this.HasParameters)
                        this.Parameters.ForEach(delegate(SqlParameter Parameter) { comm.Parameters.Add(Parameter); });
                    try
                    { conn.Open(); rowsBack = comm.ExecuteNonQuery(); }
                    catch (SqlException sqEx)
                    { CaptureError(sqEx); }
                    catch (Exception ex)
                    { CaptureError(ex); }
                    finally
                    { if (conn != null && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    comm.Parameters.Clear();
                }
            }
        }

        public void CaptureError(Exception exception, Boolean IsFrontEnd = false)
        {
            String Source = exception.Source,
                StackTrace = exception.StackTrace,
                Message = exception.Message,
                Procedure = "", Server = "",
                Cnx;

            if (exception.GetType() == typeof(SqlException))
            {
                this.sqlException = exception.Message;
                Procedure = ((SqlException)exception).Procedure;
                Server = ((SqlException)exception).Server;
            }
            else
            {
                this.genException = exception.Message;
            }

            Cnx = (IsFrontEnd ? WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString : ThisSession.CnxString);
            using (SqlConnection conn = new SqlConnection(Cnx))
            {
                using (SqlCommand comm = new SqlCommand("CaptureUIError", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("Source", Source);
                    comm.Parameters.AddWithValue("StackTrace", StackTrace);
                    comm.Parameters.AddWithValue("Message", Message);
                    comm.Parameters.AddWithValue("Procedure", Procedure);
                    comm.Parameters.AddWithValue("Server", Server);
                    try
                    { conn.Open(); comm.ExecuteNonQuery(); }
                    catch (SqlException)
                    { }
                    catch (Exception)
                    { }
                    finally
                    { if (conn != null && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    comm.Parameters.Clear();
                }
            }

            this.hasErrors = true;
        }
    }

    public sealed class CreateAuditTrail : BaseCCHData
    {
        public int? CCHID
        {
            get { int o = default(int); if (this.Parameters.Has("CCHID") && int.TryParse(this.Parameters["CCHID"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("CCHID")) { this.Parameters["CCHID"].Value = value; } else { this.Parameters.New("CCHID", SqlDbType.Int, Value: value); } }
        }
        public string SessionID
        {
            get { if (this.Parameters.Has("SessionID")) { return this.Parameters["SessionID"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("SessionID")) { this.Parameters["SessionID"].Value = value; } else { this.Parameters.New("SessionID", SqlDbType.NVarChar, 36, value); } }
        }
        public string SearchType
        {
            get { if (this.Parameters.Has("SearchType")) { return this.Parameters["SearchType"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("SearchType")) { this.Parameters["SearchType"].Value = value; } else { this.Parameters.New("SearchType", SqlDbType.NVarChar, 100, value); } }
        }
        public string Domain
        {
            get { if (this.Parameters.Has("Domain")) { return this.Parameters["Domain"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("Domain")) { this.Parameters["Domain"].Value = value; } else { this.Parameters.New("Domain", SqlDbType.NVarChar, 30, value); } }
        }
        public Double? Latitude
        {
            get { Double o = default(Double); if (this.Parameters.Has("Latitude") && Double.TryParse(this.Parameters["Latitude"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("Latitude")) { this.Parameters["Latitude"].Value = value; } else { this.Parameters.New("Latitude", SqlDbType.Float, Value: value); } }
        }
        public Double? Longitude
        {
            get { Double o = default(Double); if (this.Parameters.Has("Longitude") && Double.TryParse(this.Parameters["Longitude"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("Longitude")) { this.Parameters["Longitude"].Value = value; } else { this.Parameters.New("Longitude", SqlDbType.Float, Value: value); } }
        }

        public CreateAuditTrail()
            : base("CreateAuditTrail")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("SessionID", SqlDbType.NVarChar, 36);
            this.Parameters.New("SearchType", SqlDbType.NVarChar, 100);
            this.Parameters.New("Domain", SqlDbType.NVarChar, 30);
            this.Parameters.New("Latitude", SqlDbType.Float);
            this.Parameters.New("Longitude", SqlDbType.Float);
        }
    }
    public sealed class CreateSCIQAuditTrail : BaseCCHData
    {
        public int? CCHID
        {
            get { int o = default(int); if (this.Parameters.Has("CCHID") && int.TryParse(this.Parameters["CCHID"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("CCHID")) { this.Parameters["CCHID"].Value = value; } else { this.Parameters.New("CCHID", SqlDbType.Int, Value: value); } }
        }
        public string SessionID
        {
            get { if (this.Parameters.Has("SessionID")) { return this.Parameters["SessionID"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("SessionID")) { this.Parameters["SessionID"].Value = value; } else { this.Parameters.New("SessionID", SqlDbType.NVarChar, 36, value); } }
        }
        public Boolean SCIQ_FLG
        {
            set { this.Parameters["SCIQ_FLG"].Value = true; }
            get { return true; }
        }
        public string Action 
        {
            set { this.Parameters["Action"].Value = value; }
        }
        public string URL
        {
            set { this.Parameters["URL"].Value = value; }
        }
        public string Category
        {
            set { this.Parameters["Category"].Value = value; }
        }
        public CreateSCIQAuditTrail()
            : base("CreateAuditTrail")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("SessionID", SqlDbType.NVarChar, 36);
            this.Parameters.New("SCIQ_FLG", SqlDbType.Bit);
            this.Parameters.New("Action", SqlDbType.VarChar, 200);
            this.Parameters.New("URL", SqlDbType.VarChar, 500);
            this.Parameters.New("Category", SqlDbType.VarChar, 255);
        }
    }
    public sealed class GetDoctorsForService : BaseCCHData
    {
        #region Parameter Set properties
        public String ServiceName { set { this.Parameters["ServiceName"].Value = value; } }
        public String Latitude { set { this.Parameters["Latitude"].Value = Convert.ToDouble(value); } }
        public String Longitude { set { this.Parameters["Longitude"].Value = Convert.ToDouble(value); } }
        public int SpecialtyID { set { this.Parameters["SpecialtyID"].Value = value; } }
        public String MemberMedicalID { set { this.Parameters["MemberMedicalID"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        public String UserID { set { this.Parameters["UserID"].Value = value; } }
        public String SessionID { set { this.Parameters["SessionID"].Value = value; } }
        public String Domain { set { this.Parameters["Domain"].Value = value; } }
        #endregion

        public DataTable RawResults { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("EmptyTable")); } }

        public void ForEachResult(Action<object> action)
        {
            DataRow[] results = this.Tables[0].Select();
            foreach (DataRow dr in results) { action(dr); }
        }

        public GetDoctorsForService()
            : base("GetDoctorsForService")
        {
            this.Parameters.New("ServiceName", SqlDbType.NVarChar, Size: 200);
            this.Parameters.New("Latitude", SqlDbType.Float);
            this.Parameters.New("Longitude", SqlDbType.Float);
            this.Parameters.New("SpecialtyID", SqlDbType.Int);
            //this.Parameters.New("MemberMedicalID", SqlDbType.NVarChar, Size: 50);
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("UserID", SqlDbType.NVarChar, Size: 36);
            this.Parameters.New("SessionID", SqlDbType.NVarChar, Size: 36);
            this.Parameters.New("Domain", SqlDbType.NVarChar, Size: 30);
        }
    }
    public sealed class GetDoctorsForServiceAPI : BaseCCHData
    {
        #region SQL Parameter Properties
        public Double? Latitude
        {
            get { Double o = default(Double); if (this.Parameters.Has("Latitude") && Double.TryParse(this.Parameters["Latitude"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("Latitude")) { this.Parameters["Latitude"].Value = value; } else { this.Parameters.New("Latitude", SqlDbType.Float, Value: value); } }
        }
        public Double? Longitude
        {
            get { Double o = default(Double); if (this.Parameters.Has("Longitude") && Double.TryParse(this.Parameters["Longitude"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("Longitude")) { this.Parameters["Longitude"].Value = value; } else { this.Parameters.New("Longitude", SqlDbType.Float, Value: value); } }
        }
        public Int32? SpecialtyID
        {
            get { Int32 o = default(Int32); if (this.Parameters.Has("SpecialtyID") && Int32.TryParse(this.Parameters["SpecialtyID"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("SpecialtyID")) { this.Parameters["SpecialtyID"].Value = value; } else { this.Parameters.New("SpecialtyID", SqlDbType.Int, Value: value); } }
        }
        public Int32? ServiceID
        {
            get { Int32 o = default(Int32); if (this.Parameters.Has("ServiceID") && Int32.TryParse(this.Parameters["ServiceID"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("ServiceID")) { this.Parameters["ServiceID"].Value = value; } else { this.Parameters.New("ServiceID", SqlDbType.Int, Value: value); } }
        }
        public String ServiceName
        {
            get { if (this.Parameters.Has("ServiceName")) { return this.Parameters["ServiceName"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("ServiceName")) { this.Parameters["ServiceName"].Value = value; } else { this.Parameters.New("ServiceName", SqlDbType.NVarChar, 200, value); } }
        }
        public Int32? Distance
        {
            get { Int32 o = default(Int32); if (this.Parameters.Has("Distance") && Int32.TryParse(this.Parameters["Distance"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("Distance")) { this.Parameters["Distance"].Value = value; } else { this.Parameters.New("Distance", SqlDbType.Int, Value: value); } }
        }
        public String MemberMedicalID
        {
            get { if (this.Parameters.Has("MemberMedicalID")) { return this.Parameters["MemberMedicalID"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("MemberMedicalID")) { this.Parameters["MemberMedicalID"].Value = value; } else { this.Parameters.New("MemberMedicalID", SqlDbType.NVarChar, 50, value); } }
        }
        public Int32? CCHID
        {
            get { Int32 o = default(Int32); if (this.Parameters.Has("CCHID") && Int32.TryParse(this.Parameters["CCHID"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("CCHID")) { this.Parameters["CCHID"].Value = value; } else { this.Parameters.New("CCHID", SqlDbType.Int, Value: value); } }
        }
        public String UserID
        {
            get { if (this.Parameters.Has("UserID")) { return this.Parameters["UserID"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("UserID")) { this.Parameters["UserID"].Value = value; } else { this.Parameters.New("UserID", SqlDbType.NVarChar, 36, value); } }
        }
        public String Domain
        {
            get { if (this.Parameters.Has("Domain")) { return this.Parameters["Domain"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("Domain")) { this.Parameters["Domain"].Value = value; } else { this.Parameters.New("Domain", SqlDbType.NVarChar, 30, value); } }
        }
        public String SessionID
        {
            get { if (this.Parameters.Has("SessionID")) { return this.Parameters["SessionID"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("SessionID")) { this.Parameters["SessionID"].Value = value; } else { this.Parameters.New("SessionID", SqlDbType.NVarChar, 36, value); } }
        }
        #endregion

        #region Results
        private List<dynamic> resultData = null;
        private List<dynamic> learnMoreResults = null;
        private List<dynamic> thinData = null;
        private List<dynamic> preferredData = null;

        public dynamic Results { get { return this.resultData; } }
        public dynamic LearnMore { get { return this.learnMoreResults.First(); } }
        public dynamic ThinData { get { return this.thinData.First(); } }
        public dynamic PreferredData { get { return this.preferredData; } }
        public String EmptySet
        {
            get
            {
                StringWriter sw = new StringWriter();
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "resultrow graydiv graytop");
                    htw.RenderBeginTag(HtmlTextWriterTag.Tr);
                    htw.AddStyleAttribute("border-right", "#ccc 1px solid");
                    htw.RenderBeginTag(HtmlTextWriterTag.Td);
                    htw.Write("We apologize but were not able to retrieve any results as it doesn't appear we have services matching your search criteria available in your area.");
                    htw.RenderEndTag();
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "resultrow graydiv");
                    htw.RenderBeginTag(HtmlTextWriterTag.Tr);
                    htw.AddStyleAttribute("border-right", "#ccc 1px solid");
                    htw.RenderBeginTag(HtmlTextWriterTag.Td);
                    htw.Write("Please revise your search and try again");
                    htw.RenderEndTag();
                    htw.RenderEndTag();
                }
                return sw.ToString();
            }
        }
        #endregion

        public GetDoctorsForServiceAPI()
            : base("GetDoctorsForService")
        { }
        public override void GetData()
        {
            base.GetData();

            if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0)
            {
                resultData = (from result in this.Tables[0].AsEnumerable()
                              group result by result.Field<dynamic>("OrganizationLocationID") into groupedResult
                              select new
                              {
                                  PracticeName = groupedResult.First().Field<dynamic>("PracticeName"),
                                  Latitude = groupedResult.First().Field<dynamic>("Latitude"),
                                  Longitude = groupedResult.First().Field<dynamic>("Longitude"),
                                  Distance = String.Format("{0:#.0} mi", groupedResult.First().Field<dynamic>("NumericDistance")),
                                  FairPrice = groupedResult.First().Field<dynamic>("FairPrice"),
                                  MapMarker = (groupedResult.First().Field<dynamic>("FairPrice") ?
                                    "../Images/icon_map_green.png" :
                                    "../Images/icon_map_blue.png"),
                                  Docs = ExtractDocs(groupedResult.First().Field<dynamic>("OrganizationLocationID")),
                                  RowHTML = PracticeHTML(groupedResult.First().Field<dynamic>("OrganizationLocationID")),
                                  InfoHTML = groupedResult.First().GetInfoHTML()
                              }).ToList<dynamic>();
                //Process Learn More
                if (this.Tables.Count >= 3 && this.Tables[2].Rows.Count > 0)
                {
                    learnMoreResults = (from result in this.Tables[2].AsEnumerable()
                                        select new
                                        {
                                            Title = result.Field<dynamic>("Title"),
                                            LearnMore = result.Field<dynamic>("LearnMore")
                                        }).ToList<dynamic>();
                }
                //Process Thin Data
                if (this.Tables.Count >= 4 && this.Tables[3].Rows.Count > 0)
                {
                    thinData = (from result in this.Tables[3].AsEnumerable()
                                select new
                                {
                                    ThinData = result.Field<dynamic>("ThinData")
                                }).ToList<dynamic>();
                }
            }
        }
        public void GetData(Int32 FromRow, Int32 ToRow)
        {
            base.GetData();

            if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0)
            {
                resultData = (from result in this.Tables[0].AsEnumerable()
                              group result by result.Field<dynamic>("OrganizationLocationID") into groupedResult
                              select new
                              {
                                  PracticeName = groupedResult.First().Field<dynamic>("PracticeName"),
                                  Latitude = groupedResult.First().Field<dynamic>("Latitude"),
                                  Longitude = groupedResult.First().Field<dynamic>("Longitude"),
                                  Distance = String.Format("{0:#.0} mi", groupedResult.First().Field<dynamic>("NumericDistance")),
                                  FairPrice = groupedResult.First().Field<dynamic>("FairPrice"),
                                  MapMarker = (groupedResult.First().Field<dynamic>("FairPrice") ?
                                    "../Images/icon_map_green.png" :
                                    "../Images/icon_map_blue.png"),
                                  Docs = ExtractDocs(groupedResult.First().Field<dynamic>("OrganizationLocationID")),
                                  RowHTML = PracticeHTML(groupedResult.First().Field<dynamic>("OrganizationLocationID")),
                                  InfoHTML = groupedResult.First().GetInfoHTML()
                              }).ToList<dynamic>();
                if (ToRow > (resultData as List<dynamic>).Count)
                    ToRow = (resultData as List<dynamic>).Count;
                if (ToRow > FromRow)  //  20130604, lam, MSF-405
                    resultData = (resultData as List<dynamic>).GetRange(FromRow, (ToRow - FromRow));
                else
                    //  the above statement "(resultData as List<dynamic>).GetRange(FromRow, (ToRow - FromRow));" will fail if ToRow < FromRow
                    //  therefore, set resultData back to null and don't let the handler think that there are more data to result
                    resultData = null;  //  20130604, lam, MSF-405

                //Process Learn More
                if (this.Tables.Count >= 3 && this.Tables[2].Rows.Count > 0)
                {
                    learnMoreResults = (from result in this.Tables[2].AsEnumerable()
                                        select new
                                        {
                                            Title = result.Field<dynamic>("Title"),
                                            LearnMore = result.Field<dynamic>("LearnMore")
                                        }).ToList<dynamic>();
                }
                //Process Thin Data
                if (this.Tables.Count >= 4 && this.Tables[3].Rows.Count > 0)
                {
                    thinData = (from result in this.Tables[3].AsEnumerable()
                                select new
                                {
                                    ThinData = result.Field<dynamic>("ThinData")
                                }).ToList<dynamic>();
                }
                //Process Preferred Data
                if (this.Tables.Count >= 5 && this.Tables[4].Rows.Count > 0)
                {
                    preferredData = (from result in this.Tables[4].AsEnumerable()
                                        group result by result.Field<dynamic>("OrganizationLocationID") into groupedResult
                                        select new
                                        {
                                            PracticeName = groupedResult.First().Field<dynamic>("PracticeName"),
                                            Latitude = groupedResult.First().Field<dynamic>("Latitude"),
                                            Longitude = groupedResult.First().Field<dynamic>("Longitude"),
                                            Distance = String.Format("{0:#.0} mi", groupedResult.First().Field<dynamic>("NumericDistance")),
                                            FairPrice = groupedResult.First().Field<dynamic>("FairPrice"),
                                            MapMarker = (groupedResult.First().Field<dynamic>("FairPrice") ?
                                            "../Images/icon_map_green.png" :
                                            "../Images/icon_map_blue.png"),
                                            Docs = ExtractPreferredDocs(groupedResult.First().Field<dynamic>("OrganizationLocationID")),
                                            RowHTML = PreferredPracticeHTML(groupedResult.First().Field<dynamic>("OrganizationLocationID")),
                                            InfoHTML = groupedResult.First().GetInfoHTML(true),
                                            TeleMedicine = groupedResult.First().GetData<bool>("TeleMedicine"),  //  lam, 20130528, CI-142
                                            LocationTelephone = groupedResult.First().Field<dynamic>("LocationTelephone")  //  lam, 20130528, CI-142
                                        }).ToList<dynamic>();
                }
            }
        }
        private object ExtractDocs(Int32 orgID)
        {
            return (from doc in this.Tables[0].AsEnumerable()
                    where doc.Field<dynamic>("OrganizationLocationID") == orgID
                    select new
                    {
                        Nav = doc.GetDocNavInfo(),
                        HGOverallRating = doc.Field<dynamic>("HGOverallRating"),
                        HGPatientCount = doc.Field<dynamic>("HGPatientCount")
                    }).ToList<dynamic>();
        }
        private String PracticeHTML(Int32 orgID)
        {
            var workingSet = (from r in this.Tables[0].AsEnumerable()
                              where r.Field<dynamic>("OrganizationLocationID") == orgID
                              select new
                              {
                                  ProviderName = r.Field<String>("ProviderName"),
                                  PracticeName = r.Field<String>("PracticeName"),
                                  LocationAddress1 = r.Field<String>("LocationAddress1"),
                                  LocationCity = r.Field<String>("LocationCity"),
                                  RangeMin = r.Field<Int32>("RangeMin"),
                                  RangeMax = r.Field<Int32>("RangeMax"),
                                  YourCostMin = r.Field<Int32>("YourCostMin"),
                                  YourCostMax = r.Field<Int32>("YourCostMax"),
                                  HGOverallRating = r.Field<Double>("HGOverallRating"),
                                  HGPatientCount = r.Field<Int32>("HGPatientCount"),
                                  FairPrice = r.Field<Boolean>("FairPrice"),
                                  HGRecognized = (r.Field<Int32>("HGRecognized") == 1 ? true : false),
                                  AntiTransparency = r.GetData<Boolean>("AntiTransparency")
                              });
            int docCount = workingSet.Count(),
                hgDocCount = (from ws in workingSet where ws.HGRecognized == true select ws).Count();
            Boolean IsSingleDoc = (docCount == 1);
            String htmlBack = "";
            if (IsSingleDoc)
            {
                var relavantData = workingSet.First();
                htmlBack = SingleDocPractice(
                    relavantData.ProviderName,
                    relavantData.PracticeName,
                    relavantData.LocationAddress1,
                    relavantData.LocationCity,
                    relavantData.RangeMin,
                    relavantData.RangeMax,
                    relavantData.YourCostMin,
                    relavantData.YourCostMax,
                    relavantData.FairPrice,
                    relavantData.HGRecognized,
                    relavantData.HGOverallRating,
                    relavantData.HGPatientCount,
                    relavantData.AntiTransparency);
            }
            else
            {
                var DocListings = (from ws in workingSet
                                   select new
                                   {
                                       ws.ProviderName,
                                       ws.FairPrice,
                                       ws.HGOverallRating,
                                       ws.HGPatientCount,
                                       ws.HGRecognized
                                   }).ToList<dynamic>();
                var PracticeInfo = (from ws in workingSet
                                    select new
                                    {
                                        ws.PracticeName,
                                        ws.LocationAddress1,
                                        ws.LocationCity,
                                        ws.RangeMin,
                                        ws.RangeMax,
                                        ws.YourCostMin,
                                        ws.YourCostMax,
                                        ws.FairPrice,
                                        ws.AntiTransparency
                                    }).FirstOrDefault();
                htmlBack = MultiDocPractice(
                    PracticeInfo.PracticeName,
                    docCount, hgDocCount,
                    PracticeInfo.LocationAddress1,
                    PracticeInfo.LocationCity,
                    PracticeInfo.RangeMin,
                    PracticeInfo.RangeMax,
                    PracticeInfo.YourCostMin,
                    PracticeInfo.YourCostMax,
                    PracticeInfo.FairPrice,
                    DocListings,
                    PracticeInfo.AntiTransparency);
            }
            return htmlBack;
        }
        private object ExtractPreferredDocs(Int32 orgID)
        {
            return (from doc in this.Tables[4].AsEnumerable()
                    where doc.Field<dynamic>("OrganizationLocationID") == orgID
                    select new
                    {
                        Nav = doc.GetDocNavInfo(),
                        HGOverallRating = doc.Field<dynamic>("HGOverallRating"),
                        HGPatientCount = doc.Field<dynamic>("HGPatientCount")
                    }).ToList<dynamic>();
        }
        private String PreferredPracticeHTML(Int32 orgID)
        {
            var workingSet = (from r in this.Tables[4].AsEnumerable()
                              where r.Field<dynamic>("OrganizationLocationID") == orgID
                              select new
                              {
                                  //ProviderName = r.Field<String>("ProviderName"),  lam, 20130528, CI-142
                                  ProviderName = r.Field<bool>("TeleMedicine") ? r.Field<String>("NPI") : r.Field<String>("ProviderName"),
                                  //PracticeName = r.Field<String>("PracticeName"),  lam, 20130528, CI-142
                                  PracticeName = r.Field<bool>("TeleMedicine") ? r.Field<String>("LocationTelephone") : r.Field<String>("PracticeName"),  //  lam, 20130528, CI-142
                                  //LocationAddress1 = r.Field<String>("LocationAddress1"),  lam, 20130528, CI-142
                                  LocationAddress1 = r.Field<bool>("TeleMedicine") ? "" : r.Field<String>("LocationAddress1"),  //  lam, 20130528, CI-142
                                  //LocationCity = r.Field<String>("LocationCity"),  lam, 20130528, CI-142
                                  LocationCity = r.Field<bool>("TeleMedicine") ? "" : r.Field<String>("LocationCity"),  //  lam, 20130528, CI-142
                                  RangeMin = r.Field<Int32>("RangeMin"),
                                  RangeMax = r.Field<Int32>("RangeMax"),
                                  YourCostMin = r.Field<Int32>("YourCostMin"),
                                  YourCostMax = r.Field<Int32>("YourCostMax"),
                                  HGOverallRating = r.Field<Double>("HGOverallRating"),
                                  HGPatientCount = r.Field<Int32>("HGPatientCount"),
                                  FairPrice = r.Field<Boolean>("FairPrice"),
                                  HGRecognized = (r.Field<Int32>("HGRecognized") == 1 ? true : false),
                                  AntiTransparency = r.GetData<bool>("AntiTransparency"),
                                  TeleMedicine = r.GetData<bool>("TeleMedicine")  //  lam, 20130528, CI-142
                              });
            int docCount = workingSet.Count(),
                hgDocCount = (from ws in workingSet where ws.HGRecognized == true select ws).Count();
            Boolean IsSingleDoc = (docCount == 1);
            String htmlBack = "";
            if (IsSingleDoc)
            {
                var relavantData = workingSet.First();
                htmlBack = SingleDocPractice(
                    relavantData.ProviderName,
                    relavantData.PracticeName,
                    relavantData.LocationAddress1,
                    relavantData.LocationCity,
                    relavantData.RangeMin,
                    relavantData.RangeMax,
                    relavantData.YourCostMin,
                    relavantData.YourCostMax,
                    relavantData.FairPrice,
                    relavantData.HGRecognized,
                    relavantData.HGOverallRating,
                    relavantData.HGPatientCount,
                    relavantData.AntiTransparency,
                    true, 
                    relavantData.TeleMedicine);  //  lam, 20130528, CI-142
            }
            else
            {
                var DocListings = (from ws in workingSet
                                   select new
                                   {
                                       ws.ProviderName,
                                       ws.FairPrice,
                                       ws.HGOverallRating,
                                       ws.HGPatientCount,
                                       ws.HGRecognized
                                   }).ToList<dynamic>();
                var PracticeInfo = (from ws in workingSet
                                    select new
                                    {
                                        ws.PracticeName,
                                        ws.LocationAddress1,
                                        ws.LocationCity,
                                        ws.RangeMin,
                                        ws.RangeMax,
                                        ws.YourCostMin,
                                        ws.YourCostMax,
                                        ws.FairPrice,
                                        ws.AntiTransparency,
                                        ws.TeleMedicine  //  lam, 20130528, CI-142
                                    }).First();
                htmlBack = MultiDocPractice(
                    PracticeInfo.PracticeName,
                    docCount, hgDocCount,
                    PracticeInfo.LocationAddress1,
                    PracticeInfo.LocationCity,
                    PracticeInfo.RangeMin,
                    PracticeInfo.RangeMax,
                    PracticeInfo.YourCostMin,
                    PracticeInfo.YourCostMax,
                    PracticeInfo.FairPrice,
                    DocListings,
                    PracticeInfo.AntiTransparency,
                    true,
                    PracticeInfo.TeleMedicine);  //  lam, 20130528, CI-142
            }
            return htmlBack;
        }
        private String SingleDocPractice(String ProviderName, String PracticeName, String Address1, String City, Int32 RangeMin, Int32 RangeMax, Int32 YourCostMin, Int32 YourCostMax, Boolean FairPrice, Boolean HGRecognized, Double Rating, Int32 Surveys, Boolean AntiTransparency, Boolean IsPreferred = false, Boolean IsTeleMedicine = false)
        {
            StringWriter sw = new StringWriter();
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                #region TR
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv");
                htw.RenderBeginTag(HtmlTextWriterTag.Tr); //Table Row definition
                #region TR Doctor TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdfirst graydiv NameLoc");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "32%");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); //Doctor Name TD
                #region TR Doctor TD Link A
                htw.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "pointer");
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "navLink");
                htw.RenderBeginTag(HtmlTextWriterTag.A); //Link Doctor Link button
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "readmore result");
                #region TR Doctor TD Link A Div
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Doctor link DIV                    
                htw.Write(ProviderName.Trim());
                htw.RenderEndTag(); // End Doctor link DIV
                #endregion
                htw.RenderEndTag(); //End Doctor Name Link
                #endregion
                #region TR Doctor TD Practice Div
                htw.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); //Begin Practice Div
                htw.Write(PracticeName.Trim());
                htw.RenderEndTag(); //End Practice Div
                #endregion
                #region TR Doctor TD Address and City Div
                htw.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); //Address and City Div                    
                //htw.Write(String.Format("{0}, {1}", Address1.Trim(), City.Trim()));  lam, 20130528, CI-142
                htw.Write(Address1.Trim() != "" || City.Trim() != "" ? String.Format("{0}, {1}", Address1.Trim(), City.Trim()) : "");  //  lam, 20130528, CI-142
                htw.RenderEndTag(); //End Address and City Span
                #endregion
                htw.RenderEndTag(); // End Doctor Name TD
                #endregion
                #region TR Distance TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv Dist");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "10%");
                htw.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); //Distance TD
                #region TR Distance TD Div
                htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Distance DIV
                #region TR Distanct TD Div Span
                htw.RenderBeginTag(HtmlTextWriterTag.Span); //Distance Span
                htw.RenderEndTag(); // End Distance SPAN
                #endregion
                htw.RenderEndTag(); // End Distance DIV
                #endregion
                htw.RenderEndTag(); // End Distance TD
                #endregion
                #region TR Estimated Initial Office Visit Cost TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv EC");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "12%");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // Total Estimated Cost TD
                #region TR EIOVC TD Div
                htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Total Estimated Cost DIV
                #region TR EIOVC TD Div RangeMin B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                htw.Write(String.Format("{0:c0}", RangeMin));
                htw.RenderEndTag(); // End RangeMin B
                #endregion
                #region TR EIOVC TD Div DashCol B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "dashcol");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                htw.Write("-");
                htw.RenderEndTag(); // End DashCol B
                #endregion
                #region TR EIOVC TD Div RangeMax B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                htw.Write(String.Format("{0:c0}", RangeMax));
                htw.RenderEndTag(); // End RangeMax B
                #endregion
                htw.RenderEndTag(); // End RangeMax DIV
                #endregion
                htw.RenderEndTag(); // End RangeMax TD
                #endregion
                #region TR Your Estimated Cost TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv YC");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "0px");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // Your Estimated Cost TD
                #region TR YEC TD Div
                htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Your Estimated Cost DIV
                #region TR YEC TD Div RangeMin B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                htw.Write(String.Format("{0:c0}", YourCostMin));
                htw.RenderEndTag(); // End RangeMin B
                #endregion
                #region TR YEC TD Div DashCol B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "dashcol");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                htw.Write("-");
                htw.RenderEndTag(); // End DashCol B
                #endregion
                #region TR YEC TD Div RangeMax B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                htw.Write(String.Format("{0:c0}", YourCostMax));
                htw.RenderEndTag(); // End RangeMax B
                #endregion
                htw.RenderEndTag(); // End RangeMax DIV
                #endregion
                htw.RenderEndTag(); // End RangeMax TD
                #endregion
                #region TR FairPrice TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv FP");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "12%");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // Fair Price TD
                if (AntiTransparency)
                {
                    htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                    htw.RenderBeginTag(HtmlTextWriterTag.Div); // FairPrice Div
                    //using (LiteralControl lc = new LiteralControl("<b>Undisclosed</b>&nbsp;"))  lam, 20130828, MSF-285
                    using (LiteralControl lc = new LiteralControl("<b>N/A</b>&nbsp;"))  //  lam, 20130828, MSF-285
                        lc.RenderControl(htw);
                    using (Panel learnMore = new Panel())
                    {
                        learnMore.CssClass = "learnmore";
                        learnMore.RenderBeginTag(htw);

                        htw.AddAttribute(HtmlTextWriterAttribute.Title, "Learn More");
                        htw.RenderBeginTag(HtmlTextWriterTag.A);

                        using (Image i = new Image())
                        {
                            i.AlternateText = "Learn More";
                            i.Width = new Unit(12, UnitType.Pixel);
                            i.Height = new Unit(13, UnitType.Pixel);
                            i.BorderWidth = new Unit(0, UnitType.Pixel);
                            i.ImageUrl = "~/Images/icon_question_mark.png";
                            i.RenderControl(htw); //Render Learnmore Question Mark Image
                        }

                        htw.RenderEndTag();

                        using (Panel moreInfo = new Panel())
                        {
                            moreInfo.CssClass = "moreinfo";
                            moreInfo.Style[HtmlTextWriterStyle.ZIndex] = "1031";
                            moreInfo.RenderBeginTag(htw);

                            using (Image i = new Image())
                            {
                                i.AlternateText = "Close";
                                i.Width = new Unit(14, UnitType.Pixel);
                                i.Height = new Unit(14, UnitType.Pixel);
                                i.BorderWidth = new Unit(0, UnitType.Pixel);
                                i.ImageAlign = ImageAlign.Right;
                                i.Style[HtmlTextWriterStyle.Cursor] = "pointer";
                                i.ImageUrl = "~/Images/icon_x_sm.png";
                                i.RenderControl(htw); //Render MoreInfo close image
                            }

                            htw.RenderBeginTag(HtmlTextWriterTag.P); //Render MoreInfo Title Paragraph
                            htw.AddAttribute(HtmlTextWriterAttribute.Class, "upper");
                            htw.RenderBeginTag(HtmlTextWriterTag.B); //Render MoreInfo Title Bold
                            htw.Write("Anti-Transparency Providers"); //Add MoreInfo Title text
                            htw.RenderEndTag(); //Close MoreInfo Title Bold
                            htw.RenderEndTag(); //Close MoreInfo Title Paragraph
                            htw.RenderBeginTag(HtmlTextWriterTag.P); //Render MoreInfo Text Paragraph
                            htw.Write("This provider has chosen not to show prices to patients.  Please note that in some instances, refusal to show this information can be an indication of high prices."); //Write Text
                            htw.RenderEndTag(); //Close MoreInfo Text Paragraph

                            moreInfo.RenderEndTag(htw);
                        }

                        learnMore.RenderEndTag(htw);
                    }
                }
                else
                {
                    #region TR FairPrice TD Img
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                    htw.AddAttribute(HtmlTextWriterAttribute.Alt, "FairPrice?");
                    if (FairPrice)
                        htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/check_green.png");
                    else
                        htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/s.gif");
                    htw.RenderBeginTag(HtmlTextWriterTag.Img); //Fair Price Images
                    htw.RenderEndTag(); // End Fair Price Img
                    #endregion
                }
                htw.RenderEndTag(); //End Fair Price TD
                #endregion
                #region TR HGRecognized TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv HG");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "17%");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // HGRecognized TD
                #region TR HGRecognized TD Img
                if (HGRecognized)
                {
                    htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/check_purple.png");
                    htw.RenderBeginTag(HtmlTextWriterTag.Img); //Begin HG IMage
                    htw.RenderEndTag(); //End HG Image
                }
                else
                    if (!IsPreferred)
                    {
                        htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/s.gif");
                        htw.RenderBeginTag(HtmlTextWriterTag.Img); //Begin HG IMage
                        htw.RenderEndTag(); //End HG Image
                    }
                    else
                        htw.Write("N/A");
                #endregion
                htw.RenderEndTag(); // End HGRecognized TD
                #endregion
                #region TR Ratings TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv ratingTD");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "17%");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); //Ratings TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "ratingRow");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); //Begin rating row div
                #region TR Ratings TD Star Divs
                if (IsPreferred && Rating == 0)
                { 
                    if (IsPreferred && IsTeleMedicine)
                        {
                            htw.RenderBeginTag(HtmlTextWriterTag.Span); //Begin ratings span
                            //htw.Write(String.Format("{0} patient survey{1}", workingDoc.HGPatientCount, (workingDoc.HGPatientCount == 1 ? "" : "s")));  lam, 20130528, CI-142
                            htw.Write("N/A");  //  lam, 20130528, CI-142
                            htw.RenderEndTag(); // End ratings span
                        }
                }
                else
                {
                    Boolean HasHalfStar = (Rating % 1 > 0);
                    Double FullStars = (Rating - (Rating % 1));
                    String starClass = "";
                    for (int i = 1; i <= 5; i++)
                    {
                        if (i > FullStars)
                            if (i == FullStars + 1 && HasHalfStar)
                                starClass = "star_half";// Set the image to a half star
                            else
                                starClass = "star_none";// Set the image to a blank star
                        else
                            starClass = "star_full";//Set the image to a full star
                        htw.AddAttribute(HtmlTextWriterAttribute.Class, starClass);
                        htw.RenderBeginTag(HtmlTextWriterTag.Div); //Begin Star Div
                        htw.RenderEndTag(); //End star div
                    }
                    #region TR Ratings TD Survey P
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "ratings");
                    htw.RenderBeginTag(HtmlTextWriterTag.P);//Begin ratings P
                    #region TR Ratings TD Survey P Span
                    htw.RenderBeginTag(HtmlTextWriterTag.Span); //Begin ratings span
                    //htw.Write(String.Format("{0} patient survey{1}", Surveys, (Surveys == 1 ? "" : "s")));  lam, 20130528, CI-142
                    htw.Write(IsTeleMedicine ? "N/A" : String.Format("{0} patient survey{1}", Surveys, (Surveys == 1 ? "" : "s")));  //  lam, 20130528, CI-142
                    htw.RenderEndTag(); // End ratings span
                    #endregion
                    htw.RenderEndTag(); // End ratings p
                    #endregion
                }

                htw.RenderEndTag(); //End ratings Div
                #endregion
                htw.RenderEndTag(); //End ratings td
                #endregion

                htw.RenderEndTag(); //End Table Row
                #endregion
            }
            return sw.ToString()
                .Replace("\r", String.Empty)
                .Replace("\n", String.Empty)
                .Replace("\t", String.Empty);
        }
        private String MultiDocPractice(String PracticeName, Int32 DoctorCount, Int32 HGDocCount, String Address1, String City, Int32 RangeMin, Int32 RangeMax, Int32 YourCostMin, Int32 YourCostMax, Boolean FairPrice, List<dynamic> docsInfo, Boolean AntiTransparency, Boolean IsPreferred = false, Boolean IsTeleMedicine = false)
        {
            StringWriter sw = new StringWriter();
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                #region Practice TR
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv");
                htw.RenderBeginTag(HtmlTextWriterTag.Tr); //Table Row definition
                #region TR Practice TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdfirst graydiv NameLoc");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "32%");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); //Doctor Name TD
                #region TR Practice TD Link A
                htw.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "pointer");
                htw.RenderBeginTag(HtmlTextWriterTag.A); //Link Doctor Link button
                #region TR Practice TD Link A Div
                htw.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "default");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Color, "black");
                htw.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "result");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Doctor link DIV                    
                htw.Write(PracticeName.Trim());
                htw.RenderEndTag(); // End Doctor link DIV
                #endregion
                htw.RenderEndTag(); //End Doctor Name Link
                #endregion
                #region TR Doctor TD HideShow Div
                htw.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); //Begin Practice Div
                #region TR Practice TD HideShow Div Show A
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "AshowDoc");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Display, (IsPreferred ? "inline" : "none"));
                htw.RenderBeginTag(HtmlTextWriterTag.A);
                htw.Write(String.Format("(see {0} physicians)", DoctorCount));
                htw.RenderEndTag();
                #endregion
                #region TR Practice TD HideShow Div Hide A
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "AhideDoc");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Display, (IsPreferred ? "none" : "inline"));
                htw.RenderBeginTag(HtmlTextWriterTag.A);
                htw.Write("(hide physicians)");
                htw.RenderEndTag();
                #endregion
                htw.RenderEndTag(); //End Practice Div
                #endregion
                #region TR Doctor TD Address and City Div
                htw.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); //Address and City Div                    
                htw.Write(String.Format("{0}, {1}", Address1.Trim(), City.Trim()));
                htw.RenderEndTag(); //End Address and City Span
                #endregion
                htw.RenderEndTag(); // End Doctor Name TD
                #endregion
                #region TR Distance TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv Dist");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "10%");
                htw.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); //Distance TD
                #region TR Distance TD Div
                htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Distance DIV
                #region TR Distance TD Div Span
                htw.RenderBeginTag(HtmlTextWriterTag.Span); //Distance Span
                htw.RenderEndTag(); // End Distance SPAN
                #endregion
                htw.RenderEndTag(); // End Distance DIV
                #endregion
                htw.RenderEndTag(); // End Distance TD
                #endregion
                #region TR Estimated Initial Office Visit Cost TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv EC");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "12%");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // Total Estimated Cost TD
                #region TR EIOVC TD Div
                htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Total Estimated Cost DIV
                #region TR EIOVC TD Div RangeMin B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                htw.Write(String.Format("{0:c0}", RangeMin));
                htw.RenderEndTag(); // End RangeMin B
                #endregion
                #region TR EIOVC TD Div DashCol B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "dashcol");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                htw.Write("-");
                htw.RenderEndTag(); // End DashCol B
                #endregion
                #region TR EIOVC TD Div RangeMax B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                htw.Write(String.Format("{0:c0}", RangeMax));
                htw.RenderEndTag(); // End RangeMax B
                #endregion
                htw.RenderEndTag(); // End RangeMax DIV
                #endregion
                htw.RenderEndTag(); // End RangeMax TD
                #endregion
                #region TR Your Estimated Cost TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv YC");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "0px");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // Your Estimated Cost TD
                #region TR YEC TD Div
                htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Your Estimated Cost DIV
                #region TR YEC TD Div RangeMin B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                htw.Write(String.Format("{0:c0}", YourCostMin));
                htw.RenderEndTag(); // End RangeMin B
                #endregion
                #region TR YEC TD Div DashCol B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "dashcol");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                htw.Write("-");
                htw.RenderEndTag(); // End DashCol B
                #endregion
                #region TR YEC TD Div RangeMax B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                htw.Write(String.Format("{0:c0}", YourCostMax));
                htw.RenderEndTag(); // End RangeMax B
                #endregion
                htw.RenderEndTag(); // End RangeMax DIV
                #endregion
                htw.RenderEndTag(); // End RangeMax TD
                #endregion
                #region TR FairPrice TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv FP");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "12%");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // Fair Price TD
                if (AntiTransparency)
                {
                    htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                    htw.RenderBeginTag(HtmlTextWriterTag.Div); // FairPrice Div
                    using (LiteralControl lc = new LiteralControl("<b>Undisclosed</b>&nbsp;"))
                        lc.RenderControl(htw);
                    using (Panel learnMore = new Panel())
                    {
                        learnMore.CssClass = "learnmore";
                        learnMore.RenderBeginTag(htw);

                        htw.AddAttribute(HtmlTextWriterAttribute.Title, "Learn More");
                        htw.RenderBeginTag(HtmlTextWriterTag.A);

                        using (Image i = new Image())
                        {
                            i.AlternateText = "Learn More";
                            i.Width = new Unit(12, UnitType.Pixel);
                            i.Height = new Unit(13, UnitType.Pixel);
                            i.BorderWidth = new Unit(0, UnitType.Pixel);
                            i.ImageUrl = "~/Images/icon_question_mark.png";
                            i.RenderControl(htw); //Render Learnmore Question Mark Image
                        }

                        htw.RenderEndTag();

                        using (Panel moreInfo = new Panel())
                        {
                            moreInfo.CssClass = "moreinfo";
                            moreInfo.Style[HtmlTextWriterStyle.ZIndex] = "1031";
                            moreInfo.RenderBeginTag(htw);

                            using (Image i = new Image())
                            {
                                i.AlternateText = "Close";
                                i.Width = new Unit(14, UnitType.Pixel);
                                i.Height = new Unit(14, UnitType.Pixel);
                                i.BorderWidth = new Unit(0, UnitType.Pixel);
                                i.ImageAlign = ImageAlign.Right;
                                i.Style[HtmlTextWriterStyle.Cursor] = "pointer";
                                i.ImageUrl = "~/Images/icon_x_sm.png";
                                i.RenderControl(htw); //Render MoreInfo close image
                            }

                            htw.RenderBeginTag(HtmlTextWriterTag.P); //Render MoreInfo Title Paragraph
                            htw.AddAttribute(HtmlTextWriterAttribute.Class, "upper");
                            htw.RenderBeginTag(HtmlTextWriterTag.B); //Render MoreInfo Title Bold
                            htw.Write("Anti-Transparency Providers"); //Add MoreInfo Title text
                            htw.RenderEndTag(); //Close MoreInfo Title Bold
                            htw.RenderEndTag(); //Close MoreInfo Title Paragraph
                            htw.RenderBeginTag(HtmlTextWriterTag.P); //Render MoreInfo Text Paragraph
                            htw.Write("This provider has chosen not to show prices to patients.  Please note that in some instances, refusal to show this information can be an indication of high prices."); //Write Text
                            htw.RenderEndTag(); //Close MoreInfo Text Paragraph

                            moreInfo.RenderEndTag(htw);
                        }

                        learnMore.RenderEndTag(htw);
                    }
                }
                else
                {
                    #region TR FairPrice TD Img
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                    htw.AddAttribute(HtmlTextWriterAttribute.Alt, "FairPrice?");
                    if (FairPrice)
                        htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/check_green.png");
                    else
                        htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/s.gif");
                    htw.RenderBeginTag(HtmlTextWriterTag.Img); //Fair Price Images
                    htw.RenderEndTag(); // End Fair Price Img
                    #endregion
                }
                htw.RenderEndTag(); //End Fair Price TD
                #endregion
                #region TR HGRecognized TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv HG");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "17%");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // HGRecognized TD
                if (IsPreferred && HGDocCount == 0)
                {
                    htw.Write("N/A");
                }
                else
                {
                    htw.Write(String.Format("{0}/{1} Physicians", HGDocCount, DoctorCount));
                }
                htw.RenderEndTag(); // End HGRecognized TD
                #endregion
                #region TR Ratings TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv ratingTD");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "17%");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); //Ratings TD
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "ratingRow");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); //Begin rating row div
                #region TR Ratings TD Div
                htw.RenderBeginTag(HtmlTextWriterTag.Div);
                #region TR Ratings TD Div Show A
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "AresShow");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline");
                htw.RenderBeginTag(HtmlTextWriterTag.A);
                htw.Write("Click for Ratings");
                htw.RenderEndTag();
                #endregion
                #region TR Ratings TD Div Hide A
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "AresHide");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                htw.RenderBeginTag(HtmlTextWriterTag.A);
                htw.Write("Hide Ratings");
                htw.RenderEndTag();
                #endregion
                htw.RenderEndTag(); //End ratings Div
                #endregion
                htw.RenderEndTag(); //End ratings td
                #endregion
                htw.RenderEndTag(); //End Table Row
                #endregion
                #region Doctor Rows
                for (int i = 0; i < DoctorCount; i++)
                {
                    var workingDoc = docsInfo[i];
                    #region Individual Doc TR
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "docRow graydiv");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Display, (IsPreferred ? "none" : "table-row"));
                    htw.RenderBeginTag(HtmlTextWriterTag.Tr);
                    #region Doc TR Provider Name TD
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdfirst graydiv NameLoc");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "32%");
                    htw.RenderBeginTag(HtmlTextWriterTag.Td);
                    #region Doc TR Provider Name TD Link A
                    htw.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline-block");
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "navLink");
                    htw.RenderBeginTag(HtmlTextWriterTag.A);
                    #region Doc TR Provider Name TD Link A Name Div
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "readmore result");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                    htw.AddStyleAttribute("background-position-y", "50%");
                    htw.RenderBeginTag(HtmlTextWriterTag.Div);
                    htw.Write(workingDoc.ProviderName.Trim());
                    htw.RenderEndTag();
                    #endregion
                    htw.RenderEndTag();
                    #endregion
                    htw.RenderEndTag();
                    #endregion
                    #region TR Distance TD
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv Dist");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "10%");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                    htw.RenderBeginTag(HtmlTextWriterTag.Td); //Distance TD
                    #region TR Distance TD Div
                    htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                    htw.RenderBeginTag(HtmlTextWriterTag.Div); // Distance DIV
                    #region TR Distance TD Div Span
                    htw.RenderBeginTag(HtmlTextWriterTag.Span); //Distance Span
                    htw.RenderEndTag(); // End Distance SPAN
                    #endregion
                    htw.RenderEndTag(); // End Distance DIV
                    #endregion
                    htw.RenderEndTag(); // End Distance TD
                    #endregion
                    #region TR Estimated Initial Office Visit Cost TD
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv EC");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "12%");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                    htw.RenderBeginTag(HtmlTextWriterTag.Td); // Total Estimated Cost TD
                    #region TR EIOVC TD Div
                    htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                    htw.RenderBeginTag(HtmlTextWriterTag.Div); // Total Estimated Cost DIV
                    #region TR EIOVC TD Div RangeMin B
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                    htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                    htw.Write(String.Format("{0:c0}", RangeMin));
                    htw.RenderEndTag(); // End RangeMin B
                    #endregion
                    #region TR EIOVC TD Div DashCol B
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "dashcol");
                    htw.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                    htw.Write("-");
                    htw.RenderEndTag(); // End DashCol B
                    #endregion
                    #region TR EIOVC TD Div RangeMax B
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                    htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                    htw.Write(String.Format("{0:c0}", RangeMax));
                    htw.RenderEndTag(); // End RangeMax B
                    #endregion
                    htw.RenderEndTag(); // End RangeMax DIV
                    #endregion
                    htw.RenderEndTag(); // End RangeMax TD
                    #endregion
                    #region TR Your Estimated Cost TD
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv YC");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "0px");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                    htw.RenderBeginTag(HtmlTextWriterTag.Td); // Your Estimated Cost TD
                    #region TR YEC TD Div
                    htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                    htw.RenderBeginTag(HtmlTextWriterTag.Div); // Your Estimated Cost DIV
                    #region TR YEC TD Div RangeMin B
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                    htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                    htw.Write(String.Format("{0:c0}", YourCostMin));
                    htw.RenderEndTag(); // End RangeMin B
                    #endregion
                    #region TR YEC TD Div DashCol B
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "dashcol");
                    htw.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                    htw.Write("-");
                    htw.RenderEndTag(); // End DashCol B
                    #endregion
                    #region TR YEC TD Div RangeMax B
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                    htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                    htw.Write(String.Format("{0:c0}", YourCostMax));
                    htw.RenderEndTag(); // End RangeMax B
                    #endregion
                    htw.RenderEndTag(); // End RangeMax DIV
                    #endregion
                    htw.RenderEndTag(); // End RangeMax TD
                    #endregion
                    #region TR FairPrice TD
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv FP");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "12%");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                    htw.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                    htw.RenderBeginTag(HtmlTextWriterTag.Td); // Fair Price TD
                    if (AntiTransparency)
                    {
                        htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                        htw.RenderBeginTag(HtmlTextWriterTag.Div); // FairPrice Div
                        using (LiteralControl lc = new LiteralControl("<b>Undisclosed</b>&nbsp;"))
                            lc.RenderControl(htw);
                        using (Panel learnMore = new Panel())
                        {
                            learnMore.CssClass = "learnmore";
                            learnMore.RenderBeginTag(htw);

                            htw.AddAttribute(HtmlTextWriterAttribute.Title, "Learn More");
                            htw.RenderBeginTag(HtmlTextWriterTag.A);

                            using (Image img = new Image())
                            {
                                img.AlternateText = "Learn More";
                                img.Width = new Unit(12, UnitType.Pixel);
                                img.Height = new Unit(13, UnitType.Pixel);
                                img.BorderWidth = new Unit(0, UnitType.Pixel);
                                img.ImageUrl = "~/Images/icon_question_mark.png";
                                img.RenderControl(htw); //Render Learnmore Question Mark Image
                            }

                            htw.RenderEndTag();

                            using (Panel moreInfo = new Panel())
                            {
                                moreInfo.CssClass = "moreinfo";
                                moreInfo.Style[HtmlTextWriterStyle.ZIndex] = "1031";
                                moreInfo.RenderBeginTag(htw);

                                using (Image img = new Image())
                                {
                                    img.AlternateText = "Close";
                                    img.Width = new Unit(14, UnitType.Pixel);
                                    img.Height = new Unit(14, UnitType.Pixel);
                                    img.BorderWidth = new Unit(0, UnitType.Pixel);
                                    img.ImageAlign = ImageAlign.Right;
                                    img.Style[HtmlTextWriterStyle.Cursor] = "pointer";
                                    img.ImageUrl = "~/Images/icon_x_sm.png";
                                    img.RenderControl(htw); //Render MoreInfo close image
                                }

                                htw.RenderBeginTag(HtmlTextWriterTag.P); //Render MoreInfo Title Paragraph
                                htw.AddAttribute(HtmlTextWriterAttribute.Class, "upper");
                                htw.RenderBeginTag(HtmlTextWriterTag.B); //Render MoreInfo Title Bold
                                htw.Write("Anti-Transparency Providers"); //Add MoreInfo Title text
                                htw.RenderEndTag(); //Close MoreInfo Title Bold
                                htw.RenderEndTag(); //Close MoreInfo Title Paragraph
                                htw.RenderBeginTag(HtmlTextWriterTag.P); //Render MoreInfo Text Paragraph
                                htw.Write("This provider has chosen not to show prices to patients.  Please note that in some instances, refusal to show this information can be an indication of high prices."); //Write Text
                                htw.RenderEndTag(); //Close MoreInfo Text Paragraph

                                moreInfo.RenderEndTag(htw);
                            }

                            learnMore.RenderEndTag(htw);
                        }
                    }
                    else
                    {
                        #region TR FairPrice TD Img
                        htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                        htw.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                        htw.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                        htw.AddAttribute(HtmlTextWriterAttribute.Alt, "FairPrice?");
                        if (workingDoc.FairPrice)
                            htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/check_green.png");
                        else
                            htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/s.gif");
                        htw.RenderBeginTag(HtmlTextWriterTag.Img); //Fair Price Images
                        htw.RenderEndTag(); // End Fair Price Img
                        #endregion
                    }
                    htw.RenderEndTag(); //End Fair Price TD
                    #endregion
                    #region TR HGRecognized TD
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv HG");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "17%");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                    htw.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                    htw.RenderBeginTag(HtmlTextWriterTag.Td); // HGRecognized TD
                    #region TR HGRecognized TD Img
                    if (workingDoc.HGRecognized)
                    {
                        htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/check_purple.png");
                        htw.RenderBeginTag(HtmlTextWriterTag.Img); //Begin HG IMage
                        htw.RenderEndTag(); //End HG Image
                    }
                    else
                        if (!IsPreferred)
                        {
                            htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/s.gif");
                            htw.RenderBeginTag(HtmlTextWriterTag.Img); //Begin HG IMage
                            htw.RenderEndTag(); //End HG Image
                        }
                        else
                            htw.Write("N/A");

                    #endregion
                    htw.RenderEndTag(); // End HGRecognized TD
                    #endregion
                    #region TR Ratings TD
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv ratingTD");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "17%");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                    htw.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                    htw.RenderBeginTag(HtmlTextWriterTag.Td); //Ratings TD
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "ratingRow");
                    htw.RenderBeginTag(HtmlTextWriterTag.Div); //Begin rating row div
                    #region TR Ratings TD Star Divs
                    if (IsPreferred && !workingDoc.HGRecognized)
                    {
                        if (IsPreferred && IsTeleMedicine)
                        {
                            htw.RenderBeginTag(HtmlTextWriterTag.Span); //Begin ratings span
                            //htw.Write(String.Format("{0} patient survey{1}", workingDoc.HGPatientCount, (workingDoc.HGPatientCount == 1 ? "" : "s")));  lam, 20130528, CI-142
                            htw.Write("N/A");  //  lam, 20130528, CI-142
                            htw.RenderEndTag(); // End ratings span
                        }
                    }
                    else
                    {
                        Boolean HasHalfStar = (workingDoc.HGOverallRating % 1 > 0);
                        Double FullStars = (workingDoc.HGOverallRating - (workingDoc.HGOverallRating % 1));
                        String starClass = "";
                        for (int sc = 1; sc <= 5; sc++)
                        {
                            if (sc > FullStars)
                                if (sc == FullStars + 1 && HasHalfStar)
                                    starClass = "star_half";// Set the image to a half star
                                else
                                    starClass = "star_none";// Set the image to a blank star
                            else
                                starClass = "star_full";//Set the image to a full star
                            htw.AddAttribute(HtmlTextWriterAttribute.Class, starClass);
                            htw.RenderBeginTag(HtmlTextWriterTag.Div); //Begin Star Div
                            htw.RenderEndTag(); //End star div
                        }
                        #region TR Ratings TD Survey P
                        htw.AddAttribute(HtmlTextWriterAttribute.Class, "ratings");
                        htw.RenderBeginTag(HtmlTextWriterTag.P);//Begin ratings P
                        #region TR Ratings TD Survey P Span
                        htw.RenderBeginTag(HtmlTextWriterTag.Span); //Begin ratings span
                        //htw.Write(String.Format("{0} patient survey{1}", workingDoc.HGPatientCount, (workingDoc.HGPatientCount == 1 ? "" : "s")));  lam, 20130528, CI-142
                        htw.Write(IsTeleMedicine ? "N/A" : String.Format("{0} patient survey{1}", workingDoc.HGPatientCount, (workingDoc.HGPatientCount == 1 ? "" : "s")));  //  lam, 20130528, CI-142
                        htw.RenderEndTag(); // End ratings span
                        #endregion
                        htw.RenderEndTag(); // End ratings p
                        #endregion
                    }
                    htw.RenderEndTag(); //End ratings Div
                    #endregion
                    htw.RenderEndTag(); //End ratings td
                    #endregion
                    htw.RenderEndTag();
                    #endregion
                }
                #endregion
            }
            return sw.ToString()
                .Replace("\r", String.Empty)
                .Replace("\n", String.Empty)
                .Replace("\t", String.Empty);
        }
        private String InfoHTML(Int32 orgID)
        {
            var workingSet = (from r in this.Tables[0].AsEnumerable()
                              where r.Field<dynamic>("OrganizationLocationID") == orgID
                              select new
                              {
                                  ProviderName = r.Field<String>("ProviderName"),
                                  PracticeName = r.Field<String>("PracticeName"),
                                  LocationAddress1 = r.Field<String>("LocationAddress1"),
                                  LocationCity = r.Field<String>("LocationCity"),
                                  RangeMin = r.Field<Int32>("RangeMin"),
                                  RangeMax = r.Field<Int32>("RangeMax"),
                                  YourCostMin = r.Field<Int32>("YourCostMin"),
                                  YourCostMax = r.Field<Int32>("YourCostMax"),
                                  HGOverallRating = r.Field<Double>("HGOverallRating"),
                                  HGPatientCount = r.Field<Int32>("HGPatientCount"),
                                  FairPrice = r.Field<Boolean>("FairPrice"),
                                  HGRecognized = (r.Field<Int32>("HGRecognized") == 1 ? true : false),
                                  AntiTransparency = r.GetData<Boolean>("AntiTransparency")
                              });
            int docCount = workingSet.Count(),
                hgDocCount = (from ws in workingSet where ws.HGRecognized == true select ws).Count();
            Boolean IsSingleDoc = (docCount == 1);
            String htmlBack = "";
            if (IsSingleDoc)
            {
                var relavantData = workingSet.First();
                htmlBack = SingleDocPractice(
                    relavantData.ProviderName,
                    relavantData.PracticeName,
                    relavantData.LocationAddress1,
                    relavantData.LocationCity,
                    relavantData.RangeMin,
                    relavantData.RangeMax,
                    relavantData.YourCostMin,
                    relavantData.YourCostMax,
                    relavantData.FairPrice,
                    relavantData.HGRecognized,
                    relavantData.HGOverallRating,
                    relavantData.HGPatientCount,
                    relavantData.AntiTransparency);
            }
            else
            {
                var DocListings = (from ws in workingSet
                                   select new
                                   {
                                       ws.ProviderName,
                                       ws.FairPrice,
                                       ws.HGOverallRating,
                                       ws.HGPatientCount,
                                       ws.HGRecognized
                                   }).ToList<dynamic>();
                var PracticeInfo = (from ws in workingSet
                                    select new
                                    {
                                        ws.PracticeName,
                                        ws.LocationAddress1,
                                        ws.LocationCity,
                                        ws.RangeMin,
                                        ws.RangeMax,
                                        ws.YourCostMin,
                                        ws.YourCostMax,
                                        ws.FairPrice,
                                        ws.AntiTransparency
                                    }).FirstOrDefault();
                htmlBack = MultiDocPractice(
                    PracticeInfo.PracticeName,
                    docCount, hgDocCount,
                    PracticeInfo.LocationAddress1,
                    PracticeInfo.LocationCity,
                    PracticeInfo.RangeMin,
                    PracticeInfo.RangeMax,
                    PracticeInfo.YourCostMin,
                    PracticeInfo.YourCostMax,
                    PracticeInfo.FairPrice,
                    DocListings,
                    PracticeInfo.AntiTransparency);
            }
            return htmlBack;
        }
        private String GetInfoHTML(String ProviderName, String PracticeName, Int32 RangeMin, Int32 RangeMax, Boolean HealthGrades, Boolean FairPrice)
        {
            StringWriter sw = new StringWriter();
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "smaller infoWin");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "220px");
                htw.RenderBeginTag(HtmlTextWriterTag.P); //Info Window Paragraph definition

                //htw.AddAttribute("nav", this.Nav);
                htw.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "pointer");
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "readmore");
                htw.RenderBeginTag(HtmlTextWriterTag.A); //Link Facility Link button                
                htw.Write(ProviderName);
                htw.RenderEndTag(); //End Facility Name Link
                htw.WriteBreak();

                htw.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                htw.RenderBeginTag(HtmlTextWriterTag.Div);
                htw.Write(PracticeName);
                htw.RenderEndTag();

                htw.WriteEncodedText("Estimated Initial Office Visit Cost: ");
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                htw.Write(String.Format("{0:c0}", RangeMin));
                htw.RenderEndTag(); // End RangeMin B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "dashcol");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                htw.Write("-");
                htw.RenderEndTag(); // End DashCol B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                htw.Write(String.Format("{0:c0}", RangeMax));
                htw.RenderEndTag(); // End RangeMax B
                htw.WriteBreak();


                if (FairPrice)
                {
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                    htw.AddAttribute(HtmlTextWriterAttribute.Alt, "FairPrice?");
                    htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/check_green.png");
                    htw.RenderBeginTag(HtmlTextWriterTag.Img); //Fair Price Images
                    htw.RenderEndTag(); // End Fair Price TD
                    htw.WriteEncodedText(" Fair Price");
                }
                htw.RenderEndTag(); //End Table Row
            }
            return sw.ToString();
        }
    }
    public sealed class GetDrugDetailOptions : BaseCCHData
    {
        #region Parameter Set properties
        public String DrugID { set { this.Parameters["DrugID"].Value = Convert.ToInt32(value); } }
        public String Latitude { set { this.Parameters["Latitude"].Value = Convert.ToDouble(value); } }
        public String Longitude { set { this.Parameters["Longitude"].Value = Convert.ToDouble(value); } }
        #endregion

        public DataTable Drugs { get { return (this.Tables.Count > 0 ? this.Tables[0] : null); } }

        public GetDrugDetailOptions()
            : base("GetDrugDetailOptions")
        {
            this.Parameters.New("Latitude", SqlDbType.Float);
            this.Parameters.New("Longitude", SqlDbType.Float);
            this.Parameters.New("DrugID", SqlDbType.Int);
        }
    }
    public sealed class GetDrugList_AutoComplete : BaseCCHData
    {
        #region Parameter Set Properties
        public String SearchString { set { this.Parameters["SearchString"].Value = value; } }
        #endregion

        public GetDrugList_AutoComplete()
            : base("GetDrugList_AutoComplete")
        {
            this.Parameters.New("SearchString", SqlDbType.NVarChar, 100);
        }
        public GetDrugList_AutoComplete(String SearchString)
            : base("GetDrugList_AutoComplete")
        {
            this.Parameters.New("SearchString", SqlDbType.NVarChar, 100, SearchString);
            this.GetData();
        }

        public void ForEachMatch(Action<string> action)
        {
            DataRow[] AutoCompleteResults = this.Tables[0].Select();
            foreach (DataRow dr in AutoCompleteResults) action(dr[1].ToString());
        }
    }
    public sealed class GetDrugMultiPricingResults : BaseCCHData
    {
        #region Property Set Parameters
        public String Latitude { set { this.Parameters["Latitude"].Value = Convert.ToDouble(value); } }
        public String Longitude { set { this.Parameters["Longitude"].Value = Convert.ToDouble(value); } }
        public DataTable DrugList { set { this.Parameters["DrugList"].Value = value; } }
        #endregion

        #region Regular Properties
        public DataTable RawResults { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("EmptyTable")); } }
        public DataTable CurrentPharmacyTable
        {
            get
            {
                using (DataView dv = new DataView(RawResults))
                {
                    dv.RowFilter = "CurrentPharmText <> ''";
                    DataTable dt = dv.ToTable("CurrentPharm",
                        true,
                        new String[] { "PharmacyName", "PharmacyID", "PharmacyLocationID", "Price" });
                    return (dt.Rows.Count > 0 ? dt : new DataTable("EmptyTable"));
                }
            }
        }
        public DataTable PastCare { get { return (this.Tables.Count > 1 ? this.Tables[1] : new DataTable("EmptyTable")); } }
        public Boolean HasPastCare
        {
            get
            {
                return (PastCare.Rows[0]["ShowCurrentPharmacy"].ToString().ToLower() == "yes"
                    && PastCare.Rows[0]["CurrentPrice"].ToString() != "");
            }
        }
        public Double CurrentPrice { get { return Double.Parse(PastCare.Rows[0]["CurrentPrice"].ToString()); } }
        public DataTable PricingTable { get { return (this.Tables.Count > 2 ? this.Tables[2] : new DataTable("EmptyTable")); } }
        public Double BestPrice { get { return Double.Parse(PricingTable.Rows[0]["BestPrice"].ToString()); } }
        #endregion

        public GetDrugMultiPricingResults()
            : base("GetDrugMultiPricingResults")
        {
            this.Parameters.New("Latitude", SqlDbType.Float);
            this.Parameters.New("Longitude", SqlDbType.Float);
            this.Parameters.New("DrugList", SqlDbType.Structured);
        }
    }
    public sealed class GetDrugPricingResults : BaseCCHData
    {
        #region Property Set Parameters
        public Int32 Distance { set { this.Parameters["Distance"].Value = Convert.ToInt32(value); } }
        public String DrugID { set { this.Parameters["DrugID"].Value = Convert.ToInt32(value); } }
        public String GPI { set { this.Parameters["GPI"].Value = value; } }
        public String Quantity { set { this.Parameters["Quantity"].Value = Convert.ToDouble(value); } }
        public String Latitude { set { this.Parameters["Latitude"].Value = Convert.ToDouble(value); } }
        public String Longitude { set { this.Parameters["Longitude"].Value = Convert.ToDouble(value); } }
        public String PastCareID { set { this.Parameters["PastCareID"].Value = Convert.ToInt32(value); } }
        public String MemberRXID { set { this.Parameters["MemberRXID"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        public String UserID { set { this.Parameters["UserID"].Value = value; } }
        public Int32 FromIndex { set { if (this.Parameters["FromRow"] == null) this.Parameters.New("FromRow", SqlDbType.Int, Value: value); else this.Parameters["FromRow"].Value = value; } }
        public Int32 ToIndex { set { if (this.Parameters["ToRow"] == null) this.Parameters.New("ToRow", SqlDbType.Int, Value: value); else this.Parameters["ToRow"].Value = value; } }
        public String Domain { set { this.Parameters["Domain"].Value = value; } }
        public String SessionID { set { this.Parameters["SessionID"].Value = value; } }
        #endregion

        #region Regular Properties
        public DataTable RawResults { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("EmptyTable")); } }
        public DataTable CurrentPharmacyTable
        {
            get
            {
                using (DataView dv = new DataView(RawResults))
                {
                    dv.RowFilter = "CurrentPharmText <> ''";
                    return dv.ToTable("CurrentPharm",
                        true,
                        new String[] { "PharmacyName", "PharmacyID", "PharmacyLocationID", "Price" });
                }
            }
        }
        public DataTable PastCare { get { return (this.Tables.Count > 1 ? this.Tables[1] : new DataTable("EmptyTable")); } }
        public Boolean HasPastCare
        {
            get
            {
                return (PastCare.Rows[0]["ShowCurrentPharmacy"].ToString().ToLower() == "yes"
                    && PastCare.Rows[0]["CurrentPrice"].ToString() != "");
            }
        }
        public Double CurrentPrice { get { return Double.Parse((PastCare.Rows[0]["CurrentPrice"].ToString() == String.Empty ? "0.0" : PastCare.Rows[0]["CurrentPrice"].ToString())); } }
        public DataTable Pricingtable { get { return (this.Tables.Count > 2 ? this.Tables[2] : new DataTable("EmptyTable")); } }
        public Double BestPrice { get { return Double.Parse((Pricingtable.Rows[0]["BestPrice"].ToString() == String.Empty ? "0.0" : Pricingtable.Rows[0]["BestPrice"].ToString())); } }
        public DataTable DisplayTable { get { return (this.Tables.Count > 3 ? this.Tables[3] : new DataTable("EmptyTable")); } }
        public String DisplayName { get { return (this.DisplayTable.TableName == "EmptyTable" ? String.Empty : this.DisplayTable.Rows[0]["DrugDisplayName"].ToString().ToUpper()); } }
        public String DisplayStrength { get { return (this.DisplayTable.TableName == "EmptyTable" ? string.Empty : this.DisplayTable.Rows[0][1].ToString()); } }
        #endregion

        public GetDrugPricingResults()
            : base("GetDrugPricingResults")
        {
            this.Parameters.New("Distance", SqlDbType.Int);
            this.Parameters.New("DrugID", SqlDbType.Int);
            this.Parameters.New("GPI", SqlDbType.NVarChar, Size: 50);
            this.Parameters.New("Quantity", SqlDbType.Decimal);
            this.Parameters.New("Latitude", SqlDbType.Float);
            this.Parameters.New("Longitude", SqlDbType.Float);
            this.Parameters.New("PastCareID", SqlDbType.Int);
            // this.Parameters.New("MemberRXID", SqlDbType.NVarChar, Size: 50);
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("UserID", SqlDbType.NVarChar, Size: 36);
            this.Parameters.New("Domain", SqlDbType.NVarChar, Size: 30);
            this.Parameters.New("SessionID", SqlDbType.NVarChar, 36);
        }
    }
    public sealed class GetEmployeeByCCHIDForCallCenter : BaseCCHData
    {
        public DataTable Employee
        {
            get
            {
                using (DataView dv = new DataView(this.Tables[0]))
                {
                    //  lam, 20130319, MSF-235, add CCHID for callcenter app
                    return dv.ToTable(true,
                        new String[] { "MemberMedicalID", "CCHID", "FirstName", "LastName", "Middle", "DateOfBirth",
                    "Address1","Address2","City","State", "Email" });
                }
            }
        }

        public GetEmployeeByCCHIDForCallCenter(int CCHID)
            : base("GetEmployeeByCCHIDForCallCenter")
        {
            this.Parameters.New("CCHID", SqlDbType.Int, Value: CCHID);
        }

        public Boolean IsSelectedRegistered(String MedicalMemberID)
        {
            DataRow[] MemberRow = this.Tables[0].Select("MemberMedicalID = '" + MedicalMemberID + "'");
            if (MemberRow.Length == 0) { return false; }
            return Convert.ToBoolean(MemberRow[0]["Enrolled"]);
        }
    }
    public sealed class GetEmployerConnString : BaseCCHData
    {
        #region Parameter Set Properties
        public int EmployerID { get { return int.Parse(this["EmployerID"].ToString()); } set { this.Parameters["EmployerID"].Value = value; } }
        public String EmployerName { set { this.Parameters["EmployerName"].Value = value; } }
        #endregion

        #region Regular Properties
        public DataTable EmployerTable { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("Empty")); } }
        public DataRow[] EmployerRows { get { return (this.EmployerTable.Select()); } }
        public DataRow EmployerRow { get { return (this.EmployerTable.Rows.Count > 0 ? this.EmployerTable.Rows[0] : this.EmployerTable.NewRow()); } }
        public String this[String ColumnName] { get { return this.EmployerRow[ColumnName].ToString(); } }

        public String ConnectionString { get { return this["ConnectionString"]; } }
        #endregion

        public GetEmployerConnString()
            : base("GetEmployerConnString")
        {
            this.Parameters.New("EmployerID", SqlDbType.Int);
            this.Parameters.New("EmployerName", SqlDbType.VarChar, Size: 100);
        }
        public GetEmployerConnString(int EmployerID)
            : base("GetEmployerConnString")
        {
            this.Parameters.New("EmployerID", SqlDbType.Int, Value: EmployerID);
            this.GetFrontEndData();
        }
        public GetEmployerConnString(String EmployerName)
            : base("GetEmployerConnString")
        {
            this.Parameters.New("EmployerName", SqlDbType.VarChar, Size: 100, Value: EmployerName);
            this.GetFrontEndData();
        }

        public void ForEach(Action<object[]> act)
        {
            foreach (DataRow dr in EmployerRows)
            {
                act(dr.ItemArray);
            }
        }
    }
    public sealed class GetEmployerContent : BaseCCHData
    {
        #region Access Properties
        private object this[String Column]
        {
            get
            {
                if (this.HasErrors || this.Tables.Count == 0)
                    return null;
                else if (this.Tables[0].Rows.Count == 0)
                    return null;
                else if (!this.Tables[0].Columns.Contains(Column))
                    return null;
                else
                    return this.Tables[0].Rows[0][Column];
            }
        }
        public Boolean SSNOnly
        {
            get
            {
                return Convert.ToBoolean(this["SSNOnly"] ?? true);
            }
        }
        public String InsurerName
        {
            get
            {
                return (this["InsurerName"] ?? String.Empty).ToString();
            }
        }
        public String LogoImageName
        {
            get
            {
                return (this["LogoImageName"] ?? String.Empty).ToString();
            }
        }
        public Int32 LogoWidth
        {
            get
            {
                return Convert.ToInt32(this["LogoWidth"] ?? 0);
            }
        }
        public Int32 LogoHeight
        {
            get
            {
                return Convert.ToInt32(this["LogoHeight"] ?? 0);
            }
        }
        public Boolean HasOtherPeopleSection
        {
            get
            {
                return Convert.ToBoolean(this["HasOtherPeopleSection"] ?? false);
            }
        }
        public Boolean HasNotificationSection
        {
            get
            {
                return Convert.ToBoolean(this["HasNotificationSection"] ?? false);
            }
        }
        public Boolean TandCVisible
        {
            get
            {
                return Convert.ToBoolean(this["TandCVisible"] ?? false);
            }
        }
        public String PhoneNumber
        {
            get
            {
                return (this["PhoneNumber"] ?? String.Empty).ToString();
            }
        }
        public String FormattedPhoneNumber
        {
            get
            {
                if (!this.PhoneNumber.Contains('-'))
                {
                    return string.Format("{0}-{1}-{2}",
                        this.PhoneNumber.Substring(0, 3),
                        this.PhoneNumber.Substring(3, 3),
                        this.PhoneNumber.Substring(6, 4));
                }
                else return this.PhoneNumber;
            }
        }
        public Boolean CanSignIn
        {
            get
            {
                return Convert.ToBoolean(this["CanSignIn"] ?? false);
            }
        }
        public Boolean CanRegister
        {
            get
            {
                return Convert.ToBoolean(this["CanRegister"] ?? false);
            }
        }
        public Boolean InternalLogo
        {
            get
            {
                return Convert.ToBoolean(this["InternalLogo"] ?? false);
            }
        }
        public String MemberIDFormat
        {
            get
            {
                return (this["MemberIDFormat"] ?? String.Empty).ToString();
            }
        }
        public String SpecialtyNetworkText
        {
            get
            {
                return (this["SpecialtyNetworkText"] ?? String.Empty).ToString();
            }
        }
        public String PastCareDisclaimerText  //  lam, 20130418, MSF-299
        {
            get
            {
                return (this["PastCareDisclaimerText"] ?? String.Empty).ToString();
            }
        }
        public String RxResultDisclaimerText  //  lam, 20130418, MSF-294
        {
            get
            {
                return (this["RxResultDisclaimerText"] ?? String.Empty).ToString();
            }
        }
        public String AllResult1DisclaimerText  //  lam, 20130425, MSF-295
        {
            get
            {
                return (this["AllResult1DisclaimerText"] ?? String.Empty).ToString();
            }
        }
        public String AllResult2DisclaimerText  //  lam, 20130425, MSF-295
        {
            get
            {
                return (this["AllResult2DisclaimerText"] ?? String.Empty).ToString();
            }
        }
        public String SpecialtyDrugDisclaimerText  //  lam, 20130418, CI-59
        {
            get
            {
                return (this["SpecialtyDrugDisclaimerText"] ?? String.Empty).ToString();
            }
        }
        public String MentalHealthDisclaimerText  //  lam, 20130508, CI-144
        {
            get
            {
                return (this["MentalHealthDisclaimerText"] ?? String.Empty).ToString();
            }
        }
        public String ServiceNotFoundDisclaimerText  //  lam, 20130604, MSF-377
        {
            get
            {
                return (this["ServiceNotFoundDisclaimerText"] ?? String.Empty).ToString();
            }
        }
        public Boolean DefaultYourCostOn
        {
            get
            {
                return Convert.ToBoolean(this["DefaultYourCostOn"] ?? false);
            }
        }
        public String DefaultSort
        {
            get
            {
                return (this["DefaultSort"] ?? String.Empty).ToString();
            }
        }
        public Boolean AllowFairPriceSort  //  lam, 20130618, MSB-324
        {
            get
            {
                return Convert.ToBoolean(this["AllowFairPriceSort"] ?? false);
            }
        }
        public Boolean ShowSCIQTab  //  lam, 20130816, SCIQ-77
        {
            get
            {
                return Convert.ToBoolean(this["ShowSCIQTab"] ?? false);
            }
        }
        public String EmployerName
        {
            get
            {
                return (this["EmployerName"] ?? String.Empty).ToString();
            }
        }
        public String SCIQStartText
        {
            get
            {
                return (this["SCIQStartText"] ?? String.Empty).ToString();
            }
        }
        public String SCUserGoalToHit
        {
            get
            {
                return (this["SCUserGoalToHit"] ?? String.Empty).ToString();
            }
        }
        public String SCDashboardStartText
        {
            get
            {
                return (this["SCDashboardStartText"] ?? String.Empty).ToString();
            }
        }        
        public String CheckBackText { get { return (this["CheckBackText"] ?? String.Empty).ToString(); } }
        public Boolean OverrideRegisterButton { get { return Boolean.Parse((this["OverrideRegisterButton"].ToString())); } }
        public String ContactText { get { return (this["ContactText"] ?? String.Empty).ToString(); } }
        public Boolean SavingsChoiceEnabled { get { return Boolean.Parse((this["SavingsChoiceEnabled"].ToString())); } }
        #endregion
        #region Parameter Set Properties
        public Int32 EmployerID { set { this.Parameters["EmployerID"].Value = value; } }
        #endregion
        public GetEmployerContent(Int32 EmployerID = 0)
            : base("GetEmployerContent")
        {
            this.Parameters.New("EmployerID", SqlDbType.Int);
            if (EmployerID != 0)
            {
                this.Parameters["EmployerID"].Value = EmployerID;
                this.GetFrontEndData();
            }
        }
    }
    public sealed class GetEmployeesByLastNameForCallCenter : BaseCCHData
    {
        public String Letter { set { this.Parameters["Letter"].Value = value; } }
        public Boolean Admin { set { if (this.Parameters["Admin"] == null) { this.Parameters.New("Admin", SqlDbType.Bit, Value: value); } else { this.Parameters["Admin"].Value = value; } } }

        public DataTable DistinctLastNames
        {
            get
            {
                using (DataView dv = new DataView(this.Tables[0]))
                {
                    dv.Sort = "Lastname";
                    return dv.ToTable(true, new string[] { "LastName" });
                }
            }
        }

        public GetEmployeesByLastNameForCallCenter()
            : base("GetEmployeesByLastNameForCallCenter")
        {
            this.Parameters.New("Letter", SqlDbType.NVarChar, Size: 1);
        }
        public DataTable ByLastName(String LastName)
        {
            using (DataView dv = new DataView(this.Tables[0]))
            {
                dv.RowFilter = "LastName = '" + LastName.Replace("'", "''") + "'";
                dv.Sort = "FirstName";
                //  lam, 20130319, MSF-235, add CCHID for callcenter app
                return dv.ToTable(true,
                    new String[] { "MemberMedicalID", "CCHID", "FirstName", "LastName", "Middle", "DateOfBirth",
                    "Address1","Address2","City","State", "Email", "HearCCH" });  //  lam, 20130419, MSF-290, add "HearCCH"
            }
        }
        public Boolean IsSelectedRegistered(String MedicalMemberID)
        {
            DataRow[] MemberRow = this.Tables[0].Select("MemberMedicalID = '" + MedicalMemberID + "'");
            if (MemberRow.Length == 0) { return false; }
            return Convert.ToBoolean(
                Convert.ToInt32(
                    MemberRow[0]["Enrolled"].ToString()) > 0);
        }
    }
    public sealed class GetEmployersForCallCenter : BaseCCHData
    {
        public GetEmployersForCallCenter()
            : base("GetEmployersForCallCenter")
        {
            this.GetFrontEndData();
        }
        public void ForeEachEmployer(Action<Int32, String> action)
        {
            DataRow[] employers = this.Tables[0].Select();
            foreach (DataRow dr in employers)
                action(Convert.ToInt32(dr["EmployerID"].ToString()),
                    dr["ConnectionString"].ToString());
        }
    }
    public sealed class GetEmployeeEnrollment : BaseCCHData
    {
        #region Parameter Set Properties
        //public String Firstname { get { return this["FirstName"]; } set { this.Parameters["Firstname"].Value = value; } }
        public String LastName { get { return this["LastName"]; } set { this.Parameters["Lastname"].Value = value; } }
        public String MemberID { set { this.Parameters["MemberID"].Value = value; } }
        public String SSN { set { this.Parameters["SSN"].Value = value; } }
        public String DOB { get { return this["DateOfBirth"]; } set { this.Parameters["DOB"].Value = value; } }
        #endregion

        #region Regular Properties
        public DataTable EmployeeTable { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("EmptyTable")); } }
        public DataRow EmployeeRow { get { return (this.EmployeeTable.Rows.Count > 0 ? this.EmployeeTable.Rows[0] : this.EmployeeTable.NewRow()); } }
        public DataTable DependentTable { get { return (this.Tables.Count > 1 ? this.Tables[1] : new DataTable("EmptyTable")); } }
        public DataTable YouCouldHaveSavedTable { get { return (this.Tables.Count > 2 ? this.Tables[2] : new DataTable("EmptyTable")); } }
        public DataRow YouCouldHaveSavedRow { get { return (this.YouCouldHaveSavedTable.Rows.Count > 0 ? this.YouCouldHaveSavedTable.Rows[0] : this.YouCouldHaveSavedTable.NewRow()); } }
        public String this[String ColumnName] { get { return this.EmployeeRow[ColumnName].ToString(); } }
        public DataTable AlternateTable { get { return (this.Tables.Count > 3 ? this.Tables[3] : new DataTable("EmptyTable")); } }
        public DataRow AlternateRow { get { return (this.AlternateTable.Rows.Count > 0 ? this.AlternateTable.Rows[0] : this.AlternateTable.NewRow()); } }

        public int CCHID { get { return int.Parse(this["CCHID"]); } }
        public String EmployeeID { get { return this["EmployeeID"]; } }
        public String FirstName { get { return this["FirstName"]; } }
        public String SubscriberMedicalID { get { return this["SubscriberMedicalID"]; } }
        public String SubscriberRXID { get { return this["SubscriberRXID"]; } }
        public String Address1 { get { return this["Address1"]; } }
        public String Address2 { get { return this["Address2"]; } }
        public String City { get { return this["City"]; } }
        public String State { get { return this["State"]; } }
        public String ZipCode { get { return this["ZipCode"]; } }
        public String Latitude { get { return this["Latitude"]; } }
        public String Longitude { get { return this["Longitude"]; } }
        public String Phone { get { return this["Phone"]; } }
        public String HealthPlanType { get { return this["HealthPlanType"]; } }
        public String MedicalPlanType { get { return this["MedicalPlanType"]; } }
        public String RxPlanType { get { return this["RxPlanType"]; } }
        public String Gender { get { return this["Gender"]; } }
        public Boolean Parent { get { return Boolean.Parse(this["Parent"]); } }
        public Boolean Adult { get { return Boolean.Parse(this["Adult"]); } }
        public String Insurer { get { return this["Insurer"]; } }
        public String RXProvider { get { return this["RXProvider"]; } }
        public Decimal YouCouldHaveSaved
        {
            get
            {
                return (YouCouldHaveSavedRow["YouCouldHaveSaved"].ToString().Trim() == String.Empty ?
                    (Decimal)0.0 : Decimal.Parse(YouCouldHaveSavedRow["YouCouldHaveSaved"].ToString()));
            }
        }
        public String Email { get { return this["Email"]; } }
        public int AlternateEmployerID { get { return (AlternateRow.Field<int>("EmployerID")); } }
        public String AlternateEmployerName { get { return (AlternateRow.Field<string>("EmployerName")); } }
        public String AlternateConnectionString { get { return (AlternateRow.Field<string>("ConnectionString")); } }
        public String AlternateInsurer { get { return (AlternateRow.Field<string>("Insurer")); } }
        #endregion

        public GetEmployeeEnrollment()
            : base("GetEmployeeEnrollment")
        {
            //this.Parameters.New("Firstname", SqlDbType.NVarChar, 100);
            this.Parameters.New("Lastname", SqlDbType.NVarChar, 100);
            this.Parameters.New("MemberID", SqlDbType.NVarChar, 50);
            this.Parameters.New("SSN", SqlDbType.NVarChar, 4);
            this.Parameters.New("DOB", SqlDbType.DateTime);
        }
        //[DebuggerStepThrough]
        public void ForEachDependent<DataRow>(Action<DataRow> action)
        { foreach (DataRow dr in DependentTable.Rows) action(dr); }
    }
    public sealed class GetEmployeeWelcomeInfo : BaseCCHData
    {
        #region Parameter Set Properties
        public String UnRegisteredID { set { this.Parameters["UnRegisteredID"].Value = value; } }
        #endregion

        #region Regular Properties
        public DataTable EmployeeTable { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("EmptyTable")); } }
        public DataRow EmployeeRow { get { return (this.EmployeeTable.Rows.Count > 0 ? this.EmployeeTable.Rows[0] : this.EmployeeTable.NewRow()); } }

        public String EmployeeFirstName { get { return this.EmployeeRow["FirstName"].ToString(); } }
        public String EmployeeLastname { get { return this.EmployeeRow["LastName"].ToString(); } }
        public String EmployeeEmail { get { return this.EmployeeRow["Email"].ToString(); } }
        #endregion

        public GetEmployeeWelcomeInfo()
            : base("GetEmployeeWelcomeInfo")
        {
            this.Parameters.New("UnRegisteredID", SqlDbType.NVarChar, Size: 12);
        }
        public GetEmployeeWelcomeInfo(String EmpId)
            : base("GetEmployeeWelcomeInfo")
        {
            this.Parameters.New("UnRegisteredID", SqlDbType.NVarChar, Size: 12, Value: EmpId);
            this.GetFrontEndData();
        }
    }
    public sealed class GetEnrollmentsForAllEmployers : BaseCCHData
    {
        public String LastName
        {
            get { return this.Parameters["LastName"].Value.ToString(); }
            set { this.Parameters["LastName"].Value = value; }
        }
        public String LastFour
        {
            get { return this.Parameters["LastFour"].Value.ToString(); }
            set { this.Parameters["LastFour"].Value = value; }
        }
        public DateTime DateOfBirth
        {
            get { return (DateTime)this.Parameters["DateTime"].Value; }
            set { this.Parameters["DateTime"].Value = value; }
        }
        public String Email
        {
            get { return this.Parameters["Email"].Value.ToString(); }
            set { this.Parameters["Email"].Value = value; }
        }

        public GetEnrollmentsForAllEmployers()
            : base("GetEnrollmentsForAllEmployers")
        {
            this.Parameters.New("LastName", SqlDbType.NVarChar, 40);
            this.Parameters.New("LastFour", SqlDbType.NVarChar, 4);
            this.Parameters.New("DateOfBirth", SqlDbType.DateTime);
            this.Parameters.New("Email", SqlDbType.NVarChar, 200);
        }
    }
    public sealed class GetFacilitiesForMultiLab : BaseCCHData
    {
        private Boolean reachedEnd = false;

        #region Parameter Set Properties
        public String Distance { set { this.Parameters["Distance"].Value = Convert.ToInt16(value); } }
        public String Latitude { set { this.Parameters["Latitude"].Value = Convert.ToDecimal(value); } }
        public String Longitude { set { this.Parameters["Longitude"].Value = Convert.ToDecimal(value); } }
        public DataTable ServiceList { set { this.Parameters["ServiceList"].Value = value; } }
        #endregion

        #region Regular Properties
        public DataTable RawResults { get { if (this.Tables.Count >= 1) { return this.Tables[0]; } else { return new DataTable("Results"); } } }
        public DataTable LearnMoreResults { get { if (this.Tables.Count >= 2) { return this.Tables[1]; } else { return new DataTable("LearnMore"); } } }
        public Boolean ReachedEnd { get { return reachedEnd; } }
        #endregion

        public GetFacilitiesForMultiLab()
            : base("GetFacilitiesForMultiLab")
        {
            this.Parameters.New("Distance", SqlDbType.Int);
            this.Parameters.New("Latitude", SqlDbType.Float);
            this.Parameters.New("Longitude", SqlDbType.Float);
            this.Parameters.New("ServiceList", SqlDbType.Structured);
        }
        public override void GetData()
        {
            base.GetData();
            this.reachedEnd = (this.RowsBack < 25);
        }
        public void GetData(Int32 From, Int32 To)
        {
            //Need to add two new parameters for the event that we need a result subset
            this.Parameters.New("FromRow", SqlDbType.Int, Value: From);
            this.Parameters.New("ToRow", SqlDbType.Int, Value: To);
            //Do the normal GetData
            base.GetData();
            //Check to see if we've hit the end of the results
            this.reachedEnd = !(this.RowsBack == (To - From));
        }
    }
    public sealed class GetFacilitiesForService : BaseCCHData
    {
        private Boolean reachedEnd = false;

        #region Parameter Set Properties
        public String ServiceName { set { this.Parameters["ServiceName"].Value = value; } }
        public String Latitude { set { this.Parameters["Latitude"].Value = Convert.ToDecimal(value); } }
        public String Longitude { set { this.Parameters["Longitude"].Value = Convert.ToDecimal(value); } }
        public String SpecialtyID { set { this.Parameters["SpecialtyID"].Value = Convert.ToInt16(value); } }
        public String ServiceID { set { this.Parameters["ServiceID"].Value = Convert.ToInt32(value); } }
        public String Distance { set { this.Parameters["Distance"].Value = Convert.ToInt32(value); } }
        public String MemberMedicalID { set { this.Parameters["MemberMedicalID"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        public String UserID { set { this.Parameters["UserID"].Value = value; } }
        public String Domain { set { this.Parameters["Domain"].Value = value; } }
        public String SessionID { set { this.Parameters["SessionID"].Value = value; } }
        #endregion

        #region Regular Properties
        public DataTable RawResults { get { if (this.Tables.Count >= 1) { return this.Tables[0]; } else { return new DataTable("Results"); } } }
        public DataTable LearnMoreResults { get { if (this.Tables.Count >= 2) { return this.Tables[1]; } else { return new DataTable("LearnMore"); } } }
        public Boolean DataIsThin
        {
            get
            {
                Boolean bOut = false;

                int rowCount, fpCount;
                using (DataView dv = new DataView(RawResults))
                {
                    rowCount = dv.Count;
                    dv.RowFilter = "FairPrice = 'True'";
                    fpCount = dv.Count;
                }
                if (rowCount == 3) { bOut = (fpCount == 0); }
                else { bOut = (rowCount < 3); }

                return bOut;
            }

        }
        public Boolean ReachedEnd { get { return reachedEnd; } }
        #endregion

        public GetFacilitiesForService()
            : base("GetFacilitiesForService")
        {
            this.Parameters.New("Latitude", SqlDbType.Float);
            this.Parameters.New("Longitude", SqlDbType.Float);
            this.Parameters.New("SpecialtyID", SqlDbType.Int);
            this.Parameters.New("ServiceID", SqlDbType.Int);
            this.Parameters.New("Distance", SqlDbType.Int);
            this.Parameters.New("MemberMedicalID", SqlDbType.NVarChar, Size: 50);
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("UserID", SqlDbType.NVarChar, Size: 36);
            this.Parameters.New("Domain", SqlDbType.NVarChar, Size: 30);
            this.Parameters.New("SessionID", SqlDbType.NVarChar, Size: 36);
        }
        public override void GetData()
        {
            base.GetData();
            this.reachedEnd = (this.RowsBack < 25);
        }
        public void GetData(Int32 From, Int32 To)
        {
            //Need to add two new parameters for the event that we need a result subset
            this.Parameters.New("FromRow", SqlDbType.Int, Value: From);
            this.Parameters.New("ToRow", SqlDbType.Int, Value: To);
            //Do the normal GetData
            base.GetData();
            //Check to see if we've hit the end of the results
            this.reachedEnd = (this.RowsBack < (To - From));
        }
    }
    public sealed class GetFacilitiesForServiceAPI : BaseCCHData
    {
        #region SQL Parameter Properties
        public Double? Latitude
        {
            get { Double o = default(Double); if (this.Parameters.Has("Latitude") && Double.TryParse(this.Parameters["Latitude"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("Latitude")) { this.Parameters["Latitude"].Value = value; } else { this.Parameters.New("Latitude", SqlDbType.Float, Value: value); } }
        }
        public Double? Longitude
        {
            get { Double o = default(Double); if (this.Parameters.Has("Longitude") && Double.TryParse(this.Parameters["Longitude"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("Longitude")) { this.Parameters["Longitude"].Value = value; } else { this.Parameters.New("Longitude", SqlDbType.Float, Value: value); } }
        }
        public Int32? SpecialtyID
        {
            get { Int32 o = default(Int32); if (this.Parameters.Has("SpecialtyID") && Int32.TryParse(this.Parameters["SpecialtyID"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("SpecialtyID")) { this.Parameters["SpecialtyID"].Value = value; } else { this.Parameters.New("SpecialtyID", SqlDbType.Int, Value: value); } }
        }
        public Int32? ServiceID
        {
            get { Int32 o = default(Int32); if (this.Parameters.Has("ServiceID") && Int32.TryParse(this.Parameters["ServiceID"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("ServiceID")) { this.Parameters["ServiceID"].Value = value; } else { this.Parameters.New("ServiceID", SqlDbType.Int, Value: value); } }
        }
        public Int32? Distance
        {
            get { Int32 o = default(Int32); if (this.Parameters.Has("Distance") && Int32.TryParse(this.Parameters["Distance"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("Distance")) { this.Parameters["Distance"].Value = value; } else { this.Parameters.New("Distance", SqlDbType.Int, Value: value); } }
        }
        public String MemberMedicalID
        {
            get { if (this.Parameters.Has("MemberMedicalID")) { return this.Parameters["MemberMedicalID"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("MemberMedicalID")) { this.Parameters["MemberMedicalID"].Value = value; } else { this.Parameters.New("MemberMedicalID", SqlDbType.NVarChar, 50, value); } }
        }
        public Int32? CCHID
        {
            get { Int32 o = default(Int32); if (this.Parameters.Has("CCHID") && Int32.TryParse(this.Parameters["CCHID"].Value.ToString(), out o)) { return o; } else { return null; } }
            set { if (this.Parameters.Has("CCHID")) { this.Parameters["CCHID"].Value = value; } else { this.Parameters.New("CCHID", SqlDbType.Int, Value: value); } }
        }
        public String UserID
        {
            get { if (this.Parameters.Has("UserID")) { return this.Parameters["UserID"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("UserID")) { this.Parameters["UserID"].Value = value; } else { this.Parameters.New("UserID", SqlDbType.NVarChar, 36, value); } }
        }
        public String Domain
        {
            get { if (this.Parameters.Has("Domain")) { return this.Parameters["Domain"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("Domain")) { this.Parameters["Domain"].Value = value; } else { this.Parameters.New("Domain", SqlDbType.NVarChar, 30, value); } }
        }
        public String SessionID
        {
            get { if (this.Parameters.Has("SessionID")) { return this.Parameters["SessionID"].Value.ToString(); } else { return null; } }
            set { if (this.Parameters.Has("SessionID")) { this.Parameters["SessionID"].Value = value; } else { this.Parameters.New("SessionID", SqlDbType.NVarChar, 36, value); } }
        }
        #endregion

        #region Results
        private List<dynamic> resultData = null;
        private List<dynamic> learnMoreResults = null;
        private List<dynamic> thinData = null;
        private List<dynamic> preferredData = null;

        public dynamic Results { get { return this.resultData; } }
        public dynamic LearnMore { get { return this.learnMoreResults.First(); } }
        public dynamic ThinData { get { return this.thinData.First(); } }
        public dynamic PreferredData { get { return this.preferredData; } }
        public String EmptySet
        {
            get
            {
                StringWriter sw = new StringWriter();
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "resultrow graydiv graytop");
                    htw.RenderBeginTag(HtmlTextWriterTag.Tr);
                    htw.AddStyleAttribute("border-right", "#ccc 1px solid");
                    htw.RenderBeginTag(HtmlTextWriterTag.Td);
                    htw.Write("We apologize but were not able to retrieve any results as it doesn't appear we have services matching your search criteria available in your area.");
                    htw.RenderEndTag();
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "resultrow graydiv");
                    htw.RenderBeginTag(HtmlTextWriterTag.Tr);
                    htw.AddStyleAttribute("border-right", "#ccc 1px solid");
                    htw.RenderBeginTag(HtmlTextWriterTag.Td);
                    htw.Write("Please revise your search and try again");
                    htw.RenderEndTag();
                    htw.RenderEndTag();
                }
                return sw.ToString();
            }
        }
        #endregion

        public GetFacilitiesForServiceAPI()
            : base("GetFacilitiesForService")
        { }
        public override void GetData()
        {
            base.GetData();

            //Process Results
            if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0)
            {
                resultData = (from result in this.Tables[0].AsEnumerable()
                              select new
                              {
                                  PracticeName = result.Field<dynamic>("PracticeName"),
                                  NPI = result.Field<dynamic>("NPI"),
                                  LocationAddress1 = result.Field<dynamic>("LocationAddress1"),
                                  LocationCity = result.Field<dynamic>("LocationCity"),
                                  LocationState = result.Field<dynamic>("LocationState"),
                                  LocationZip = result.Field<dynamic>("LocationZip"),
                                  Nav = "",
                                  Latitude = result.Field<dynamic>("Latitude"),
                                  Longitude = result.Field<dynamic>("Longitude"),
                                  Distance = String.Format("{0:#.0} mi", result.Field<dynamic>("NumericDistance")),
                                  RangeMin = result.Field<dynamic>("RangeMin"),
                                  RangeMax = result.Field<dynamic>("RangeMax"),
                                  YourCostMin = result.Field<dynamic>("YourCostMin"),
                                  YourCostMax = result.Field<dynamic>("YourCostMax"),
                                  FairPrice = result.Field<dynamic>("FairPrice"),
                                  HGRecognized = result.Field<dynamic>("HGRecognized"),
                                  HGOverallRating = result.Field<dynamic>("HGOverallRating"),
                                  HGPatientCount = result.Field<dynamic>("HGPatientCount"),
                                  HGRecognizedDocCount = result.Field<dynamic>("HGRecognizedDocCount"),
                                  HGDocCount = result.Field<dynamic>("HGDocCount"),
                                  OrganizationLocationID = result.Field<dynamic>("OrganizationLocationID"), 
                                  AntiTransparency = result.Field<dynamic>("AntiTransparency")  //  lam, 20130308, MSF-141
                              }).ToList<dynamic>();
            }
            //Process Learn More
            if (this.Tables.Count >= 2 && this.Tables[1].Rows.Count > 0)
            {
                learnMoreResults = (from result in this.Tables[1].AsEnumerable()
                                    select new
                                    {
                                        Description = result.Field<dynamic>("Description")
                                    }).ToList<dynamic>();
            }
            //Process Thin Data
            if (this.Tables.Count >= 3 && this.Tables[2].Rows.Count > 0)
            {
                thinData = (from result in this.Tables[2].AsEnumerable()
                            select new
                            {
                                ThinData = result.Field<dynamic>("ThinData")
                            }).ToList<dynamic>();
            }
        }
        public void GetData(Int32 From, Int32 To)
        {
            //Need to add two new parameters for the event that we need a result subset
            this.Parameters.New("FromRow", SqlDbType.Int, Value: From);
            this.Parameters.New("ToRow", SqlDbType.Int, Value: To);
            //Do the normal GetData
            base.GetData();

            //Process Results
            if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0)
            {
                resultData = (from result in this.Tables[0].AsEnumerable()
                              select new
                              {
                                  PracticeName = result.Field<dynamic>("PracticeName"),
                                  NPI = result.Field<dynamic>("NPI"),
                                  LocationAddress1 = result.Field<dynamic>("LocationAddress1"),
                                  LocationCity = result.Field<dynamic>("LocationCity"),
                                  LocationState = result.Field<dynamic>("LocationState"),
                                  LocationZip = result.Field<dynamic>("LocationZip"),
                                  Nav = result.GetNavInfo(),
                                  Latitude = result.Field<dynamic>("Latitude"),
                                  Longitude = result.Field<dynamic>("Longitude"),
                                  Distance = String.Format("{0:#.0} mi", result.Field<dynamic>("NumericDistance")),
                                  RangeMin = result.Field<dynamic>("RangeMin"),
                                  RangeMax = result.Field<dynamic>("RangeMax"),
                                  YourCostMin = result.Field<dynamic>("YourCostMin"),
                                  YourCostMax = result.Field<dynamic>("YourCostMax"),
                                  FairPrice = result.Field<dynamic>("FairPrice"),
                                  HGRecognized = result.Field<dynamic>("HGRecognized"),
                                  HGOverallRating = result.Field<dynamic>("HGOverallRating"),
                                  HGPatientCount = result.Field<dynamic>("HGPatientCount"),
                                  HGRecognizedDocCount = result.Field<dynamic>("HGRecognizedDocCount"),
                                  HGDocCount = result.Field<dynamic>("HGDocCount"),
                                  OrganizationLocationID = result.Field<dynamic>("OrganizationLocationID"),
                                  MapMarker = (result.GetData<bool>("FairPrice") ?  //result.Field<dynamic>("FairPrice") ?  lam, 20130308, MSF-278, 279
                                        "../Images/icon_map_green.png" :
                                        "../Images/icon_map_blue.png"),
                                  AntiTransparency = result.Field<dynamic>("AntiTransparency"),
                                  RowHTML = result.GetFASRowHTML(),
                                  InfoHTML = result.GetInfoHTML()
                              }).ToList<dynamic>();
            }
            //Process Learn More
            if (this.Tables.Count >= 2 && this.Tables[1].Rows.Count > 0)
            {
                learnMoreResults = (from result in this.Tables[1].AsEnumerable()
                                    select new
                                    {
                                        Description = result.Field<dynamic>("Description")
                                    }).ToList<dynamic>();
            }
            //Process Thin Data
            if (this.Tables.Count >= 3 && this.Tables[2].Rows.Count > 0)
            {
                thinData = (from result in this.Tables[2].AsEnumerable()
                            select new
                            {
                                ThinData = result.Field<dynamic>("ThinData")
                            }).ToList<dynamic>();
            }
            //Process Preferred Data
            if (this.Tables.Count >= 5 && this.Tables[4].Rows.Count > 0)
            {
                preferredData = (from result in this.Tables[4].AsEnumerable()
                                 select new
                                 {
                                     PracticeName = result.Field<dynamic>("PracticeName"),
                                     NPI = result.Field<dynamic>("NPI"),
                                     LocationAddress1 = result.Field<dynamic>("LocationAddress1"),
                                     LocationCity = result.Field<dynamic>("LocationCity"),
                                     LocationState = result.Field<dynamic>("LocationState"),
                                     LocationZip = result.Field<dynamic>("LocationZip"),
                                     Nav = result.GetNavInfo(),
                                     Latitude = result.Field<dynamic>("Latitude"),
                                     Longitude = result.Field<dynamic>("Longitude"),
                                     Distance = String.Format("{0:#.0} mi", result.Field<dynamic>("NumericDistance")),
                                     RangeMin = result.Field<dynamic>("RangeMin"),
                                     RangeMax = result.Field<dynamic>("RangeMax"),
                                     YourCostMin = result.Field<dynamic>("YourCostMin"),
                                     YourCostMax = result.Field<dynamic>("YourCostMax"),
                                     FairPrice = result.Field<dynamic>("FairPrice"),
                                     HGRecognized = result.Field<dynamic>("HGRecognized"),
                                     HGOverallRating = result.Field<dynamic>("HGOverallRating"),
                                     HGPatientCount = result.Field<dynamic>("HGPatientCount"),
                                     HGRecognizedDocCount = result.Field<dynamic>("HGRecognizedDocCount"),
                                     HGDocCount = result.Field<dynamic>("HGDocCount"),
                                     OrganizationLocationID = result.Field<dynamic>("OrganizationLocationID"),
                                     MapMarker = (result.GetData<bool>("FairPrice") ?  //  lam, MSF-304, use GetData instead of Field<dynamic>
                                           "../Images/icon_map_green.png" :
                                           "../Images/icon_map_blue.png"),
                                     AntiTransparency = result.Field<dynamic>("AntiTransparency"),
                                     RowHTML = result.GetFASRowHTML(true),
                                     InfoHTML = result.GetInfoHTML(true)
                                 }).ToList<dynamic>();
            }
        }
    }
    public sealed class GetFacilitiesForServicePastCare : BaseCCHData
    {
        #region Parameter Set Properties
        public Int32 ServiceID { set { this.Parameters["ServiceID"].Value = value; } }
        public String ProcedureCode { set { this.Parameters["ProcedureCode"].Value = value; } }
        public Double ProfessionalComponent { set { this.Parameters["ProfessionalComponent"].Value = value; } }
        public Double Latitude { set { this.Parameters["Latitude"].Value = value; } }
        public Double Longitude { set { this.Parameters["Longitude"].Value = value; } }
        public Int32 SpecialtyID { set { this.Parameters["SpecialtyID"].Value = value; } }
        public Int32 PastCareID { set { this.Parameters["PastCareID"].Value = value; } }
        public String MemberMedicalID { set { this.Parameters["MemberMedicalID"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        public String UserID { set { this.Parameters["UserID"].Value = value; } }
        public String SessionID { set { this.Parameters["SessionID"].Value = value; } }
        public String Domain { set { this.Parameters["Domain"].Value = value; } }
        #endregion

        public GetFacilitiesForServicePastCare()
            : base("GetFacilitiesForServicePastCare")
        {
            this.Parameters.New("ServiceID", SqlDbType.Int);
            this.Parameters.New("ProcedureCode", SqlDbType.NVarChar, Size: 5);
            this.Parameters.New("ProfessionalComponent", SqlDbType.Money);
            this.Parameters.New("Latitude", SqlDbType.Float);
            this.Parameters.New("Longitude", SqlDbType.Float);
            this.Parameters.New("SpecialtyID", SqlDbType.Int);
            this.Parameters.New("PastCareID", SqlDbType.Int);
            //this.Parameters.New("MemberMedicalID", SqlDbType.NVarChar, Size: 50);
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("UserID", SqlDbType.NVarChar, Size: 30);
            this.Parameters.New("SessionID", SqlDbType.NVarChar, 36);
            this.Parameters.New("Domain", SqlDbType.NVarChar, 30);
        }
        public void ForEachResult<DataRow>(Action<DataRow> action)
        { foreach (DataRow row in this.Tables[0].Rows) action(row); }
    }
    public sealed class GetFAQItemsForWeb : BaseCCHData
    {
        public Int32 EmployerID { set { this.Parameters["EmployerID"].Value = value; } }
        public GetFAQItemsForWeb(Int32 EmployerID = 0)
            : base("GetFAQItemsForWeb")
        {
            this.Parameters.New("EmployerID", SqlDbType.Int);
            if (EmployerID != 0)
            {
                this.Parameters["EmployerID"].Value = EmployerID;
                this.GetFrontEndData();
            }
        }
    }
    public sealed class GetKeyEmployeeInfo : BaseCCHData
    {
        #region Parameter Set Properties
        public String Email { set { this.Parameters["Email"].Value = value; } }
        #endregion

        #region Regular Properties
        public DataTable EmployeeTable { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("EmptyTable")); } }
        public DataRow EmployeeRow { get { return (this.EmployeeTable.Rows.Count > 0 ? this.EmployeeTable.Rows[0] : this.EmployeeTable.NewRow()); } }
        public DataTable DependentTable { get { return (this.Tables.Count > 1 ? this.Tables[1] : new DataTable("EmptyTable")); } }
        public DataTable YouCouldHaveSavedTable { get { return (this.Tables.Count > 2 ? this.Tables[2] : new DataTable("EmptyTable")); } }
        public DataRow YouCouldHaveSavedRow { get { return (this.YouCouldHaveSavedTable.Rows.Count > 0 ? this.YouCouldHaveSavedTable.Rows[0] : this.YouCouldHaveSavedTable.NewRow()); } }
        public object this[String ColumnName] { get { return (this.EmployeeRow.Table.Columns.Contains(ColumnName) ? this.EmployeeRow[ColumnName] : null); } }

        public int CCHID { get { return Convert.ToInt32(this["CCHID"]); } }
        public String EmployeeID { get { return this["EmployeeID"].ToString(); } }
        public String SubscriberMedicalID { get { return this["SubscriberMedicalID"].ToString(); } }
        public String SubscriberRXID { get { return this["SubscriberRXID"].ToString(); } }
        public String LastName { get { return this["LastName"].ToString(); } }
        public String FirstName { get { return this["FirstName"].ToString(); } }
        public String Address1 { get { return this["Address1"].ToString(); } }
        public String Address2 { get { return this["Address2"].ToString(); } }
        public String City { get { return this["City"].ToString(); } }
        public String State { get { return this["State"].ToString(); } }
        public String ZipCode { get { return this["ZipCode"].ToString(); } }
        public String Latitude { get { return this["Latitude"].ToString(); } }
        public String Longitude { get { return this["Longitude"].ToString(); } }
        public String DateOfBirth { get { return this["DateOfBirth"].ToString(); } }
        public String Phone { get { return this["Phone"].ToString(); } }
        public String HealthPlanType { get { return this["HealthPlanType"].ToString(); } }
        public String MedicalPlanType { get { return this["MedicalPlanType"].ToString(); } }
        public String RxPlanType { get { return this["RxPlanType"].ToString(); } }
        public String Gender { get { return this["Gender"].ToString(); } }
        public Boolean Parent { get { return Convert.ToBoolean(this["Parent"]); } }
        public Boolean Adult { get { return Convert.ToBoolean(this["Adult"]); } }
        public String Insurer { get { return this["Insurer"].ToString(); } }
        public String RXProvider { get { return this["RXProvider"].ToString(); } }
        public Boolean OptInIncentiveProgram { get { return Convert.ToBoolean(this["OptInIncentiveProgram"]); } }
        public Boolean OptInEmailAlerts { get { return Convert.ToBoolean(this["OptInEmailAlerts"]); } }
        public Boolean OptInTextMsgAlerts { get { return Convert.ToBoolean(this["OptInTextMsgAlerts"]); } }
        public String MobilePhone { get { return (this["MobilePhone"] ?? "").ToString(); } }
        public Boolean OptInPriceConcierge { get { return Convert.ToBoolean(this["OptInPriceConcierge"]); } }
        public Decimal YouCouldHaveSaved
        {
            get
            {
                return Decimal.Parse((YouCouldHaveSavedRow["YouCouldHaveSaved"].ToString().Trim() == String.Empty ?
                    "0.0" : YouCouldHaveSavedRow["YouCouldHaveSaved"].ToString()));
            }
        }
        #endregion

        public GetKeyEmployeeInfo()
            : base("GetKeyEmployeeInfo")
        {
            this.Parameters.New("Email", SqlDbType.NVarChar, Size: 150);
        }
        public GetKeyEmployeeInfo(String Email)
            : base("GetKeyEmployeeInfo")
        {
            this.Parameters.New("Email", SqlDbType.NVarChar, Size: 150, Value: Email);
            this.GetData();
        }
        public void ForEachDependent<DataRow>(Action<DataRow> action)
        { foreach (DataRow dr in DependentTable.Rows) action(dr); }
    }
    public sealed class GetKeyProviderInfo : BaseCCHData
    {
        public String ProviderID { set { this.Parameters["ProviderID"].Value = value; } }

        public DataTable Education { get { return (this.Tables[0] != null ? this.Tables[0] : new DataTable()); } }
        public DataTable Ratings { get { return (this.Tables[1] != null ? this.Tables[1] : new DataTable()); } }

        public Int32 PatientCount { get { return Convert.ToInt32(Ratings.Rows[0]["PatientCount"].ToString()); } }
        public Double Rating { get { return Convert.ToDouble(Ratings.Rows[0]["OverallRating"].ToString()); } }

        public GetKeyProviderInfo()
            : base("GetKeyProviderInfo")
        {
            this.Parameters.New("ProviderID", SqlDbType.NVarChar, Size: 25);
        }
    }
    public sealed class GetKeyUserInfo : BaseCCHData
    {
        #region Parameter Set Properties
        public String UserName { set { this.Parameters["UserName"].Value = value; } }
        public String UserID { set { this.Parameters["UserID"].Value = value; } }
        #endregion

        #region Regular Properties
        public DataTable EmployeeTable { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("EmptyTable")); } }
        public DataRow EmployeeRow { get { return (this.EmployeeTable.Rows.Count > 0 ? this.EmployeeTable.Rows[0] : this.EmployeeTable.NewRow()); } }
        public String this[String ColumnName] { get { return this.EmployeeRow[ColumnName].ToString(); } }

        public String ConnectionString { get { return this["ConnectionString"].ToString(); } }
        public String EmployerID { get { return this["EmployerID"].ToString(); } }
        public String EmployerName { get { return this["EmployerName"].ToString(); } }
        public String Insurer { get { return this["Insurer"].ToString(); } }
        public String RXProvider { get { return this["RXProvider"].ToString(); } }
        public Boolean ShowYourCostColumn { get { return Boolean.Parse((this["ShowYourCostColumn"].ToString().Trim() == "" ? "false" : this["ShowYourCostColumn"].ToString())); } }
        #endregion

        public GetKeyUserInfo()
            : base("GetKeyUserInfo")
        {
            this.Parameters.New("UserName", SqlDbType.NVarChar, Size: 250);
        }
        public GetKeyUserInfo(String UserID)
            : base("GetKeyUserInfo")
        {
            this.Parameters.New("UserID", SqlDbType.NVarChar, Size: 36, Value: UserID);
            this.GetFrontEndData();
        }
    }
    public sealed class GetLabTestLetterMenu : BaseCCHData
    {
        public DataTable AvailableLetters
        {
            get
            {
                char[] alpha = new char[] { 'a','b','c','d','e','f','g','h','i','j','k','l','m',
                                        'n','o','p','q','r','s','t','u','v','w','x','y','z' };
                DataTable dtOut = new DataTable("Letters");
                if (this.Tables.Count > 0)
                {
                    dtOut = this.Tables[0].Clone();
                    dtOut.Columns.Add("Available", typeof(Boolean));
                    this.ForEachLetter(delegate(String letter, Int32 index)
                    {
                        DataRow newLetter = dtOut.NewRow();

                    });
                }
                return dtOut;
            }
        }
        public GetLabTestLetterMenu()
            : base("GetLabTestLetterMenu")
        {
            GetData();
        }
        public void ForEachLetter(Action<string, Int32> action)
        {
            DataRow[] Letters = this.Tables[0].Select(); Int32 i = 0;
            foreach (DataRow dr in Letters) { action(dr[0].ToString(), i); i++; }
        }
    }
    public sealed class GetLastCategoriesForWeb : BaseCCHData
    {
        #region Parameter Set properties
        public String Specialty { set { this.Parameters["Specialty"].Value = value; } }
        public String SubCategory { set { this.Parameters["SubCategory"].Value = value; } }
        #endregion

        public DataTable Categories { get { return (this.Tables.Count > 0 ? this.Tables[0] : null); } }

        public GetLastCategoriesForWeb()
            : base("GetLastCategoriesForWeb")
        {
            this.Parameters.New("Specialty", SqlDbType.NVarChar, Size: 70);
            this.Parameters.New("SubCategory", SqlDbType.NVarChar, Size: 150);
        }
    }
    public sealed class GetPastCare : BaseCCHData
    {
        public class ServiceInfo
        {
            public String Category { get; set; }
            public String ServiceDate { get; set; }
            public String PatientName { get; set; }
            public String ServiceName { get; set; }
            public String FacilityName { get; set; }
            public String PracticeName { get; set; }
            public String AllowedAmount { get; set; }
            public String YouCouldHaveSaved { get; set; }
            public String ProcedureCode { get; set; }
            public Int32 SpecialtyID { get; set; }
            public Int32 PastCareID { get; set; }
            public Int32 ServiceID { get; set; }
        }
        public class DrugInfo : ServiceInfo
        {
            public dynamic GPI { get; set; }
            public dynamic Quantity { get; set; }
            public dynamic DrugID { get; set; }
            public dynamic PharmacyLocationID { get; set; }
        }

        /// <summary>
        /// The Member Medical ID of the Subscriber (Or the CCHID for now)
        /// </summary>
        public String SubscriberMedicalID { set { this.Parameters["SubscriberMedicalID"].Value = value; } }
        /// <summary>
        /// The CCHID of the chosen employee or dependant from the drop down.
        /// </summary>
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }

        /// <summary>
        /// A Read-Only Tuple containing an overall total of what the person Spent and what they could have Saved
        /// <para>Item1 = You Spent</para>
        /// <para>Item2 = You Could Have Saved</para>
        /// <para>Returns Null if there is no data</para>
        /// </summary>
        public Tuple<Int32, Int32> YouCouldHaveSaved
        {
            get
            {
                if (this.Tables.Count < 1) return null;
                if (this.Tables[0] == null) return null;
                using (DataTable dt = this.Tables[0])
                {
                    if (dt.Rows.Count == 0) return null;
                    dynamic d = (from ychs in dt.AsEnumerable()
                                 select new
                                 {
                                     TotalSpent = (int)ychs.Field<Decimal>("YouSpent"),
                                     TotalSaved = (int)ychs.Field<Decimal>("YouCouldHaveSaved")
                                 }).First();
                    return new Tuple<int, int>(d.TotalSpent, d.TotalSaved);
                }
            }
        }
        /// <summary>
        /// A Read-Only Tuple containing an overall total of what the person Spent and what they could have Saved for Office Visits
        /// <para>Item1 = You Spent</para>
        /// <para>Item2 = You Could Have Saved</para>
        /// <para>Returns Null if there is no data</para>
        /// </summary>
        public Tuple<Int32, Int32> OfficeVisitSavings
        {
            get
            {
                if (this.Tables.Count < 2) return null;
                if (this.Tables[1] == null) return null;
                using (DataTable dt = this.Tables[1])
                {
                    if (dt.Rows.Count == 0) return null;
                    dynamic d = (from ov in dt.AsEnumerable()
                                 select new
                                 {
                                     TotalSpent = (int)ov.Field<Decimal>("TotalYouSpent"),
                                     TotalSaved = (int)ov.Field<Decimal>("TotalYouCouldHaveSaved")
                                 }).Distinct().First();
                    return new Tuple<int, int>(d.TotalSpent, d.TotalSaved);
                }
            }
        }
        /// <summary>
        /// Returns: Read-Only List of type GetPastCare.ServiceInfo objects containing formatted Office Visit Data
        /// </summary>
        public List<ServiceInfo> OfficeVisits
        {
            get
            {
                if (this.Tables.Count < 2) return new List<ServiceInfo>();
                if (this.Tables[1] == null) return new List<ServiceInfo>();
                using (DataTable dt = this.Tables[1])
                {
                    return (from ov in dt.AsEnumerable()
                            orderby ov.GetData<DateTime>("ServiceDate") descending  //  ascending, lam, 20130401, MSF-311
                            select new ServiceInfo
                            {
                                Category = ov.Field<String>("Category"),
                                ServiceDate = ov.Field<DateTime>("ServiceDate").ToShortDateString(),
                                PatientName = ov.Field<String>("PatientName"),
                                ServiceName = ov.Field<String>("ServiceName"),
                                FacilityName = ov.Field<String>("PracticeName"),
                                PracticeName = ov.Field<String>("PracticeName"),
                                AllowedAmount = String.Format("{0:c0}", (int)ov.GetData<Decimal>("AllowedAmount")),
                                YouCouldHaveSaved = String.Format("{0:c0}", (int)ov.GetData<Decimal>("YouCouldHaveSaved")),
                                ProcedureCode = ov.Field<String>("ProcedureCode"),
                                SpecialtyID = (Int32)(ov.Field<dynamic>("SpecialtyID") ?? -1),
                                PastCareID = ov.Field<Int32>("PastCareID"),
                                ServiceID = ov.Field<Int32>("ServiceID")
                            }).ToList<ServiceInfo>();
                }
            }
        }
        /// <summary>
        /// A Read-Only Tuple containing an overall total of what the person Spent and what they could have Saved for Laboratory Services
        /// <para>Item1 = You Spent</para>
        /// <para>Item2 = You Could Have Saved</para>
        /// <para>Returns Null if there is no data</para>
        /// </summary>
        public Tuple<Int32, Int32> LabServiceSavings
        {
            get
            {
                if (this.Tables.Count < 3) return null;
                if (this.Tables[2] == null) return null;
                using (DataTable dt = this.Tables[2])
                {
                    if (dt.Rows.Count == 0) return null;
                    dynamic d = (from ls in dt.AsEnumerable()
                                 select new
                                 {
                                     TotalSpent = (int)ls.Field<Decimal>("TotalYouSpent"),
                                     TotalSaved = (int)ls.Field<Decimal>("TotalYouCouldHaveSaved")
                                 }).Distinct().First();
                    return new Tuple<int, int>(d.TotalSpent, d.TotalSaved);
                }
            }
        }
        /// <summary>
        /// Returns: Read-Only List of type GetPastCare.ServiceInfo objects containing formatted Laboratory Service data
        /// </summary>
        public List<ServiceInfo> LabServices
        {
            get
            {
                if (this.Tables.Count < 3) return new List<ServiceInfo>();
                if (this.Tables[2] == null) return new List<ServiceInfo>();
                using (DataTable dt = this.Tables[2])
                {
                    return (from ov in dt.AsEnumerable()
                            orderby ov.GetData<DateTime>("ServiceDate") descending  //  ascending, lam, 20130401, MSF-311
                            select new ServiceInfo
                            {
                                Category = ov.Field<dynamic>("Category"),
                                ServiceDate = ov.Field<dynamic>("ServiceDate").ToShortDateString(),
                                PatientName = ov.Field<dynamic>("PatientName"),
                                ServiceName = ov.Field<dynamic>("ServiceName"),
                                FacilityName = ov.Field<dynamic>("PracticeName"),
                                PracticeName = ov.Field<dynamic>("PracticeName"),
                                AllowedAmount = String.Format("{0:c0}", ov.Field<dynamic>("AllowedAmount")),
                                YouCouldHaveSaved = String.Format("{0:c0}", ov.GetData<decimal>("YouCouldHaveSaved")),
                                ProcedureCode = ov.Field<dynamic>("ProcedureCode"),
                                SpecialtyID = ov.Field<dynamic>("SpecialtyID"),
                                PastCareID = ov.Field<dynamic>("PastCareID"),
                                ServiceID = ov.Field<dynamic>("ServiceID")
                            }).ToList<ServiceInfo>();
                }
            }
        }
        /// <summary>
        /// A Read-Only Tuple containing an overall total of what the person Spent and what they could have Saved for Outpatient Proceedures
        /// <para>Item1 = You Spent</para>
        /// <para>Item2 = You Could Have Saved</para>
        /// <para>Returns Null if there is no data</para>
        /// </summary>
        public Tuple<Int32, Int32> OutpatientProceedureSavings
        {
            get
            {
                if (this.Tables.Count < 4) return null;
                if (this.Tables[3] == null) return null;
                using (DataTable dt = this.Tables[3])
                {
                    if (dt.Rows.Count == 0) return null;
                    dynamic d = (from ls in dt.AsEnumerable()
                                 select new
                                 {
                                     TotalSpent = (int)((ls.Field<Decimal?>("TotalYouSpent")).HasValue ? ls.Field<Decimal>("TotalYouSpent") : 0.0m),
                                     TotalSaved = (int)((ls.Field<Decimal?>("TotalYouCouldHaveSaved")).HasValue ? ls.Field<Decimal>("TotalYouCouldHaveSaved") : 0.0m)
                                 }).Distinct().First();
                    return new Tuple<int, int>(d.TotalSpent, d.TotalSaved);
                }
            }
        }
        /// <summary>
        /// Returns: Read-Only List of type GetPastCare.ServiceInfo objects containing formatted Outpatient Proceedure data
        /// </summary>
        public List<ServiceInfo> OutpatientProceedures
        {
            get
            {
                if (this.Tables.Count < 4) return new List<ServiceInfo>();
                if (this.Tables[3] == null) return new List<ServiceInfo>();
                using (DataTable dt = this.Tables[3])
                {
                    return (from ov in dt.AsEnumerable()
                            orderby ov.GetData<DateTime>("ServiceDate") descending  //  ascending, lam, 20130401, MSF-311
                            select new ServiceInfo
                            {
                                Category = ov.Field<String>("Category"),
                                ServiceDate = ov.Field<DateTime>("ServiceDate").ToShortDateString(),
                                PatientName = ov.Field<String>("PatientName"),
                                ServiceName = ov.Field<String>("ServiceName"),
                                FacilityName = ov.Field<String>("PracticeName"),
                                PracticeName = ov.Field<String>("PracticeName"),
                                AllowedAmount = String.Format("{0:c0}", (Int32)ov.Field<Decimal>("AllowedAmount")),
                                YouCouldHaveSaved = String.Format("{0:c0}", (Int32)ov.GetData<Decimal>("YouCouldHaveSaved")),
                                ProcedureCode = ov.Field<String>("ProcedureCode"),
                                SpecialtyID = ov.Field<Int32>("SpecialtyID"),
                                PastCareID = ov.Field<Int32>("PastCareID"),
                                ServiceID = ov.Field<Int32>("ServiceID")
                            }).ToList<ServiceInfo>();
                }
            }
        }
        /// <summary>
        /// A Read-Only Tuple containing an overall total of what the person Spent and what they could have Saved for Imaging Services
        /// <para>Item1 = You Spent</para>
        /// <para>Item2 = You Could Have Saved</para>
        /// <para>Returns Null if there is no data</para>
        /// </summary>
        public Tuple<Int32, Int32> ImagingSavings
        {
            get
            {
                if (this.Tables.Count < 5) return null;
                if (this.Tables[4] == null) return null;
                using (DataTable dt = this.Tables[4])
                {
                    if (dt.Rows.Count == 0) return null;
                    dynamic d = (from ls in dt.AsEnumerable()
                                 select new
                                 {
                                     TotalSpent = (int)ls.GetData<Decimal>("TotalYouSpent"),
                                     TotalSaved = (int)ls.GetData<Decimal>("TotalYouCouldHaveSaved")
                                 }).Distinct().First();
                    return new Tuple<int, int>(d.TotalSpent, d.TotalSaved);
                }
            }
        }
        /// <summary>
        /// Returns: Read-Only List of type GetPastCare.ServiceInfo objects containing formatted Imaging data
        /// </summary>
        public List<ServiceInfo> Imaging
        {
            get
            {
                if (this.Tables.Count < 5) return new List<ServiceInfo>();
                if (this.Tables[4] == null) return new List<ServiceInfo>();
                using (DataTable dt = this.Tables[4])
                {
                    return (from ov in dt.AsEnumerable()
                            orderby ov.GetData<DateTime>("ServiceDate") descending  //  ascending, lam, 20130401, MSF-311
                            select new ServiceInfo
                            {
                                Category = ov.Field<String>("Category"),
                                ServiceDate = ov.Field<DateTime>("ServiceDate").ToShortDateString(),
                                PatientName = ov.Field<String>("PatientName"),
                                ServiceName = ov.Field<String>("ServiceName"),
                                FacilityName = ov.Field<String>("PracticeName"),
                                PracticeName = ov.Field<String>("PracticeName"),
                                AllowedAmount = String.Format("{0:c0}", (Int32)ov.Field<Decimal>("AllowedAmount")),
                                YouCouldHaveSaved = String.Format("{0:c0}", (Int32)ov.GetData<Decimal>("YouCouldHaveSaved")),
                                ProcedureCode = ov.Field<String>("ProcedureCode"),
                                SpecialtyID = ov.Field<Int32>("SpecialtyID"),
                                PastCareID = ov.Field<Int32>("PastCareID"),
                                ServiceID = ov.Field<Int32>("ServiceID")
                            }).ToList<ServiceInfo>();
                }
            }
        }
        /// <summary>
        /// A Read-Only Tuple containing an overall total of what the person Spent and what they could have Saved on Prescription Drugs
        /// <para>Item1 = You Spent</para>
        /// <para>Item2 = You Could Have Saved</para>
        /// <para>Returns Null if there is no data</para>
        /// </summary>
        public Tuple<Int32, Int32> DrugSavings
        {
            get
            {
                if (this.Tables.Count < 6) return null;
                if (this.Tables[5] == null) return null;
                using (DataTable dt = this.Tables[5])
                {
                    if (dt.Rows.Count == 0) return null;
                    dynamic d = (from ls in dt.AsEnumerable()
                                 select new
                                 {
                                     TotalSpent = (int)ls.GetData<Decimal>("TotalYouSpent"),  //  lam, 20130322, MSF-300 switch to GetData
                                     //TotalSpent = (int)ls.Field<Decimal>("TotalYouSpent"),
                                     TotalSaved = (int)ls.GetData<Decimal>("TotalYouCouldHaveSaved")  //  lam, 20130322, MSF-300 switch to GetData
                                     //TotalSaved = (int)ls.Field<Decimal?>("TotalYouCouldHaveSaved") //JM 3-6-13: Updated to handle nulls in this field
                                 }).Distinct().First();
                    return new Tuple<int, int>(d.TotalSpent, d.TotalSaved);
                }
            }
        }
        /// <summary>
        /// Returns: Read-Only List of type GetPastCare.DrugInfo objects containing formatted Prescription Drug data
        /// </summary>
        public List<DrugInfo> Drugs
        {
            get
            {
                if (this.Tables.Count < 6) return new List<DrugInfo>();
                if (this.Tables[5] == null) return new List<DrugInfo>();
                using (DataTable dt = this.Tables[5])
                {
                    return (from ov in dt.AsEnumerable()
                            orderby ov.GetData<DateTime>("ServiceDate") descending  //  ascending, lam, 20130401, MSF-311
                            select new DrugInfo
                            {
                                Category = ov.Field<String>("Category"),
                                ServiceDate = ov.Field<DateTime>("ServiceDate").ToShortDateString(),
                                PatientName = ov.Field<String>("PatientName"),
                                ServiceName = ov.Field<String>("ServiceName"),
                                FacilityName = ov.Field<String>("PracticeName"),
                                PracticeName = ov.Field<String>("PracticeName"),
                                AllowedAmount = String.Format("{0:c0}", (Int32)(ov.Field<dynamic>("AllowedAmount") ?? 0)),
                                YouCouldHaveSaved = String.Format("{0:c0}", (Int32)ov.GetData<decimal>("YouCouldHaveSaved")),
                                ProcedureCode = ov.Field<String>("ProcedureCode"),
                                PastCareID = ov.Field<Int32>("PastCareID"),
                                GPI = ov.Field<dynamic>("GPI"),
                                DrugID = ov.Field<dynamic>("DrugID"),
                                PharmacyLocationID = ov.Field<dynamic>("PharmacyLocationID"),
                                Quantity = ov.Field<dynamic>("Quantity")
                            }).ToList<DrugInfo>();
                }
            }
        }

        public Tuple<String> AsOfDate  //  lam, 20130729, MSF-420  add new AsOfDate
        {
            get
            {
                if (this.Tables.Count < 7) return null;
                if (this.Tables[6] == null) return null;
                using (DataTable dt = this.Tables[6])
                {
                    if (dt.Rows.Count == 0) return null;
                    if (dt.Rows[0][0].ToString().Trim() == "") return null;
                    dynamic d = (from aod in dt.AsEnumerable()
                                 select new
                                 {
                                     AsOfDate = aod.Field<DateTime>("AsOfDate").ToShortDateString()
                                 }).First();
                    return new Tuple<String>(d.AsOfDate);
                }
            }
        }

        /// <summary>
        /// Instantiates a new instance of the <code>GetPastCare</code> class
        /// </summary>
        public GetPastCare()
            : base("GetPastCare")
        {
            this.Parameters.New("SubscriberMedicalID", SqlDbType.NVarChar, 50);
            this.Parameters.New("CCHID", SqlDbType.Int);
        }
    }
    public sealed class GetPastCareRX : BaseCCHData
    {
        #region Parameter Set Properties
        public String EmployeeID { set { this.Parameters["EmployeeID"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        #endregion

        #region Regular Properties
        public DataTable FamilyMedTable { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("Empty")); } }
        public DataTable SaveTable { get { return (this.Tables.Count > 1 ? this.Tables[1] : new DataTable("Empty")); } }
        public DataTable TheraSubTable { get { return (this.Tables.Count > 2 ? this.Tables[2] : new DataTable("Empty")); } }
        public String SaveTotal
        {
            get
            {
                return (this.SaveTable.TableName == "Empty" ?
                    String.Empty
                    : String.Format("{0:C0}", Double.Parse((this.SaveTable.Rows[0][0].ToString().Trim() == String.Empty ? "0.0"
                        : this.SaveTable.Rows[0][0].ToString()))));
            }
        }
        public DataTable DistinctTherasubMembers
        {
            get
            {
                using (DataView dvMembers = new DataView(this.TheraSubTable))
                {
                    return dvMembers.ToTable("Members", true,
                        new String[] { "CCHID", "FirstName", "LastName" });
                }
            }
        }
        #endregion

        public GetPastCareRX()
            : base("GetPastCareRX")
        {
            this.Parameters.New("EmployeeID", SqlDbType.NVarChar, Size: 50);
            this.Parameters.New("CCHID", SqlDbType.Int);
        }
        public GetPastCareRX(String EmployeeID, Int32 CCHID)
            : base("GetPastCareRX")
        {
            this.Parameters.New("EmployeeID", SqlDbType.NVarChar, Size: 50, Value: EmployeeID);
            this.Parameters.New("CCHID", SqlDbType.Int, Value: CCHID);
            this.GetData();
        }
        public void ForEachTheraSubMemberPastCareID(Action<string> action)
        {
            DataRow[] TheraSubMemberPastCareIDs = this.TheraSubTable.Select();
            foreach (DataRow dr in TheraSubMemberPastCareIDs) { action(dr["PastCareID"].ToString()); }
        }
    }
    public sealed class GetPasswordQuestions : BaseCCHData
    {
        public DataTable QuestionsTable { get { return (this.Tables.Count > 0 ? this.Tables[0] : new DataTable("Empty")); } }
        public String CurrentUserQuestion
        {
            get
            {
                if (this.Tables.Count > 1)
                {
                    if (this.Tables[1].Rows.Count > 0)
                        return this.Tables[1].Rows[0][0].ToString();
                    else
                        return String.Empty;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public GetPasswordQuestions()
            : base("GetPasswordQuestions")
        {
            this.GetFrontEndData();
        }
        public GetPasswordQuestions(String Email)
            : base("GetPasswordQuestions")
        {
            this.Parameters.New("Email", SqlDbType.NVarChar, Size: 256, Value: Email);
            this.GetFrontEndData();
        }
    }
    public sealed class GetProviderSpecialtyList : BaseCCHData
    {
        #region Public Properties
        public DataTable Specialties
        {
            get
            {
                this.Tables[0].Columns["Specialty"].ColumnName = "SubCategory";
                return this.Tables[0];
            }
        }
        public DataTable SubSpecialties
        {
            get
            {
                this.Tables[1].Columns["DisplayName"].ColumnName = "CategoryLvl4";
                return this.Tables[1];
            }
        }
        #endregion

        public GetProviderSpecialtyList()
            : base("GetProviderSpecialtyList")
        {
            this.GetFrontEndData();
        }
    }
    public sealed class GetRegisterCounts : BaseCCHData
    {
        public DataTable Counts { get { return this.Tables[0] ?? null; } }

        public GetRegisterCounts()
            : base("GetRegisterCounts")
        {

        }
    }
    public sealed class GetSelectedFacilityDetailsForMultiLab : BaseCCHData
    {
        #region Property Set Parameters
        public String OrganizationID { set { this.Parameters["OrganizationID"].Value = Convert.ToInt16(value); } }
        public String OrganizationLocationID { set { this.Parameters["OrganizationLocationID"].Value = Convert.ToInt16(value); } }
        public DataTable ServiceList { set { this.Parameters["ServiceList"].Value = value; } }
        #endregion

        public GetSelectedFacilityDetailsForMultiLab()
            : base("GetSelectedFacilityDetailsForService")
        {
            this.Parameters.New("OrganizationID", SqlDbType.Int);
            this.Parameters.New("OrganizationLocationID", SqlDbType.Int);
            this.Parameters.New("ServiceList", SqlDbType.Structured);
        }
    }
    public sealed class GetSelectedFacilityDetailsForService : BaseCCHData
    {
        #region Property Set Parameters
        public String ServiceName { set { this.Parameters["ServiceName"].Value = value; } }
        public String PracticeName { set { this.Parameters["PracticeName"].Value = value; } }
        public String NPI { set { this.Parameters["NPI"].Value = value; } }
        public int SpecialtyID { set { this.Parameters["SpecialtyID"].Value = value; } }
        public int ServiceID { set { this.Parameters["ServiceID"].Value = value; } }
        public int OrganizationLocationID { set { this.Parameters["OrganizationLocationID"].Value = value; } }
        #endregion

        public GetSelectedFacilityDetailsForService()
            : base("GetSelectedFacilityDetailsForService")
        {
            this.Parameters.New("ServiceName", SqlDbType.NVarChar, Size: 200);
            this.Parameters.New("PracticeName", SqlDbType.NVarChar, Size: 150);
            this.Parameters.New("NPI", SqlDbType.NVarChar, Size: 50);
            this.Parameters.New("SpecialtyID", SqlDbType.Int);
            this.Parameters.New("ServiceID", SqlDbType.Int);
            this.Parameters.New("OrganizationLocationID", SqlDbType.Int);
        }
    }
    public sealed class GetSelectedProviderDetailsForSpecialty : BaseCCHData
    {
        #region Public Set Properties
        public Int32 SpecialtyID { set { this.Parameters["SpecialtyID"].Value = value; } }
        public String ProviderName { set { this.Parameters["ProviderName"].Value = value; } }
        public String NPI { set { this.Parameters["NPI"].Value = value; } }
        public String TaxID { set { this.Parameters["TaxID"].Value = value; } }
        public Int32 ServiceID { set { this.Parameters["ServiceID"].Value = value; } }
        public Int32 OrganizationLocationID { set { this.Parameters["OrganizationLocationID"].Value = value; } }
        #endregion

        public String LearnMoreInfo
        {
            get
            {
                return (this.Tables[1] != null && this.Tables[1].Rows[0][0] != null ?
                        this.Tables[1].Rows[0][0].ToString() :
                        String.Empty);
            }
        }
        public String SpecialtyTitle
        {
            get
            {
                return (this.Tables[1] != null && this.Tables[1].Rows[0]["Title"] != null ?
                        this.Tables[1].Rows[0]["Title"].ToString() :
                        String.Empty);
            }
        }
        public Boolean OfficeVisible
        {
            get
            {
                return (this.Tables[2] != null && this.Tables[2].Rows[0] != null ?
                        Convert.ToBoolean(this.Tables[2].Rows[0]["Office"].ToString().ToLower()) :
                        false);
            }
        }
        public Boolean ProceduresVisible
        {
            get
            {
                return (this.Tables[2] != null && this.Tables[2].Rows[0] != null ?
                        Convert.ToBoolean(this.Tables[2].Rows[0]["Procedures"].ToString().ToLower()) :
                        false);
            }
        }
        public Boolean LabVisible
        {
            get
            {
                return (this.Tables[2] != null && this.Tables[2].Rows[0] != null ?
                        Convert.ToBoolean(this.Tables[2].Rows[0]["Lab"].ToString().ToLower()) :
                        false);
            }
        }
        public Boolean ImagingVisible
        {
            get
            {
                return (this.Tables[2] != null && this.Tables[2].Rows[0] != null ?
                        Convert.ToBoolean(this.Tables[2].Rows[0]["Imaging"].ToString().ToLower()) :
                        false);
            }
        }
        public String SpecialtyReferalsVisible
        {
            get
            {
                return (this.Tables[2] != null && this.Tables[2].Rows[0] != null ?
                        this.Tables[2].Rows[0]["SpecialtyReferals"].ToString().ToLower().Trim() :
                        String.Empty);
            }
        }
        public String DoctorName
        {
            get
            {
                return (this.Tables[0] != null && this.Tables[0].Rows[0] != null ?
                    this.Tables[0].Rows[0]["ProviderName"].ToString() :
                    String.Empty);
            }
        }
        public String PracticeName
        {
            get
            {
                return (this.Tables[0] != null && this.Tables[0].Rows[0] != null ?
                    this.Tables[0].Rows[0]["PracticeName"].ToString() :
                    String.Empty);
            }
        }
        public Boolean IsFairPrice
        {
            get
            {
                return (this.Tables[0] != null && this.Tables[0].Rows[0] != null ?
                    Convert.ToBoolean(this.Tables[0].Rows[0]["FairPrice"].ToString().ToLower()) :
                    false);
            }
        }
        public Boolean IsHGRecognized
        {
            get
            {
                return (this.Tables[0] != null && this.Tables[0].Rows[0] != null ?
                    Convert.ToBoolean(this.Tables[0].Rows[0]["HGRecognized"].ToString().ToLower()) :
                    false);
            }
        }
        public String RangeMin
        {
            get
            {
                return (this.Tables[0] != null && this.Tables[0].Rows[0] != null ?
                    this.Tables[0].Rows[0]["RangeMin"].ToString() :
                    String.Empty);
            }
        }
        public String RangeMax
        {
            get
            {
                return (this.Tables[0] != null && this.Tables[0].Rows[0] != null ?
                    this.Tables[0].Rows[0]["RangeMax"].ToString() :
                    String.Empty);
            }
        }
        public String HGProviderID
        {
            get
            {
                return (this.Tables[0] != null && this.Tables[0].Rows[0] != null ?
                    this.Tables[0].Rows[0]["HGProviderID"].ToString() :
                    String.Empty);
            }
        }

        public GetSelectedProviderDetailsForSpecialty()
            : base("GetSelectedProviderDetailsForSpecialty")
        {
            this.Parameters.New("SpecialtyID", SqlDbType.Int);
            this.Parameters.New("ProviderName", SqlDbType.NVarChar, Size: 150);
            this.Parameters.New("NPI", SqlDbType.NVarChar, Size: 50);
            this.Parameters.New("TaxID", SqlDbType.NVarChar, Size: 15);
            this.Parameters.New("ServiceID", SqlDbType.Int);
            this.Parameters.New("OrganizationLocationID", SqlDbType.Int);
        }
    }
    public sealed class GetServiceList_AutoComplete : BaseCCHData
    {
        #region Parameter Set Properties
        public String SearchString { set { this.Parameters["SearchString"].Value = value; } }
        #endregion

        public GetServiceList_AutoComplete()
            : base("GetServiceList_AutoComplete")
        {
            this.Parameters.New("SearchString", SqlDbType.NVarChar, 100);
        }
        public GetServiceList_AutoComplete(String SearchString)
            : base("GetServiceList_AutoComplete")
        {
            this.Parameters.New("SearchString", SqlDbType.NVarChar, 100, SearchString);
            this.GetData();
        }
        public void ForEachMatch(Action<string> action)
        {
            DataRow[] AutoCompleteResults = this.Tables[0].Select();
            foreach (DataRow dr in AutoCompleteResults) action(dr[1].ToString());
        }
    }
    public sealed class GetServiceListByCategoryForWeb : BaseCCHData
    {
        #region Parameter Set Properties
        public String Specialty { set { this.Parameters["Specialty"].Value = value; } }
        public String SubCategory { set { this.Parameters["SubCategory"].Value = value; } }
        public String CategoryLvl4 { set { this.Parameters["CategoryLvl4"].Value = value; } }
        #endregion

        public DataTable ServiceResults { get { return (this.Tables.Count > 0 ? this.Tables[0] : null); } }
        public String this[String ColumnName] { get { return (ServiceResults != null ? ServiceResults.Rows[0][ColumnName].ToString() : String.Empty); } }
        public String ServiceName { get { return this["ServiceName"]; } }
        public Int32 ServiceID { get { return (this["ServiceID"] == String.Empty ? -1 : Convert.ToInt32(this["ServiceID"])); } }
        public Int32 SpecialtyID { get { return (this["SpecialtyID"] == String.Empty ? -1 : Convert.ToInt32(this["SpecialtyID"])); } }

        public GetServiceListByCategoryForWeb()
            : base("GetServiceListByCategoryForWeb")
        {
            this.Parameters.New("Specialty", SqlDbType.NVarChar, Size: 70);
            this.Parameters.New("SubCategory", SqlDbType.NVarChar, Size: 150);
            this.Parameters.New("CategoryLvl4", SqlDbType.NVarChar, Size: 150);
        }
    }
    public sealed class GetSpecialtiesForWeb : BaseCCHData
    {
        public DataTable SpecialtyResults { get { return (this.Tables.Count > 0 ? this.Tables[0] : null); } }

        public GetSpecialtiesForWeb()
            : base("GetSpecialtiesForWeb")
        {
            base.GetFrontEndData();
        }
    }
    public sealed class GetSpecialtySubCategoriesForWeb : BaseCCHData
    {
        #region Parameter Set properties
        public String Specialty { set { this.Parameters["Specialty"].Value = value; } }
        #endregion

        public DataTable SubSpecialties { get { return (this.Tables.Count > 0 ? this.Tables[0] : null); } }

        public GetSpecialtySubCategoriesForWeb()
            : base("GetSpecialtySubCategoriesForWeb")
        {
            this.Parameters.New("Specialty", SqlDbType.NVarChar, Size: 70);
        }
        public GetSpecialtySubCategoriesForWeb(String Specialty)
            : base("GetSpecialtySubCategoriesForWeb")
        {
            this.Parameters.New("Specialty", SqlDbType.NVarChar, Size: 70, Value: Specialty);
            this.GetFrontEndData();
        }
    }
    public sealed class GetTheraSubPageContent : BaseCCHData
    {
        public DataTable MembersTable
        {
            get
            {
                using (DataView dv = new DataView(this.Tables[0]))
                {
                    return dv.ToTable("Members", true,
                        new String[] { "CCHID", "FirstName", "LastName" });
                }
            }
        }
        public DataTable SubDetailsTable
        {
            get
            {
                using (DataView dv = new DataView(this.Tables[0]))
                {
                    return dv.ToTable("SubDetails", false,
                        new String[] { "DrugName", "Strength", "ReplacementDrugName", "ReplacementStrength", "PharmacyName", "Address1", "Address2", "City", "State", "ZipCode", "Telephone" });
                }
            }
        }
        public String PrescribingDoctor { get { return this.Tables[0].Rows[0]["PrescribingProviderName"].ToString(); } }
        public String PatientName { get { return String.Format("{0} {1}", this.Tables[0].Rows[0]["FirstName"].ToString(), this.Tables[0].Rows[0]["LastName"].ToString()); } }

        public String PastCareIDList { set { this.Parameters["PastCareIDList"].Value = value; } }

        public GetTheraSubPageContent()
            : base("GetTheraSubPageContent")
        {
            this.Parameters.New("PastCareIDList", SqlDbType.NVarChar, Size: 200);
        }
        public GetTheraSubPageContent(String IDList)
            : base("GetTheraSubPageContent")
        {
            this.Parameters.New("PastCareIDList", SqlDbType.NVarChar, Size: 200, Value: IDList);
            this.GetData();
        }
        public DataTable GetMemberMed(String CCHID)
        {
            using (DataView dv = new DataView(this.Tables[0]))
            {
                dv.RowFilter = String.Format("CCHID = '{0}'", CCHID);
                return dv.ToTable("membermeds", false,
                    new String[] { "DrugName", "Strength", "Quantity", "QuantityUOM", "ReplacementDrugName", "ReplacementStrength", "ReplacementQuantityUOM" });
            }
        }
    }
    public sealed class GetThinDataResults : BaseCCHData
    {
        private Boolean reachedEnd = false;

        #region Parameter Set Properties
        public String ZipCode { set { this.Parameters["ZipCode"].Value = value; } }
        public String ServiceID { set { this.Parameters["ServiceID"].Value = Convert.ToInt16(value); } }
        public String Latitude { set { this.Parameters["Latitude"].Value = Convert.ToDecimal(value); } }
        public String Longitude { set { this.Parameters["Longitude"].Value = Convert.ToDecimal(value); } }
        public String SpecialtyId { set { this.Parameters["SpecialtyID"].Value = Convert.ToInt16(value); } }
        #endregion

        #region Regular Properties
        public DataTable RawResults { get { if (this.Tables.Count >= 2) { return this.Tables[1]; } else { return new DataTable("Results"); } } }
        public DataTable CMSResults { get { if (this.Tables.Count >= 1) { return this.Tables[0]; } else { return new DataTable("CMS"); } } }
        public Boolean ReachedEnd { get { return reachedEnd; } }
        #endregion

        public GetThinDataResults()
            : base("GetThinDataResults")
        {
            this.Parameters.New("ZipCode", SqlDbType.NVarChar, 10);
            this.Parameters.New("ServiceID", SqlDbType.Int);
            this.Parameters.New("Latitude", SqlDbType.Float);
            this.Parameters.New("Longitude", SqlDbType.Float);
            this.Parameters.New("SpecialtyID", SqlDbType.Int);
        }
        public override void GetData()
        {
            base.GetData();
            this.reachedEnd = (this.RowsBack < 25);
        }
    }
    public sealed class GetUIErrorList : BaseCCHData
    {
        public GetUIErrorList()
            : base("GetUIErrorList")
        {
            this.GetFrontEndData();
        }
        public DataTable GetGridViewData()
        {
            return this.Tables[0];
        }
    }
    public sealed class InsertEnrollmentRequest : BaseCCHData
    {
        #region Parameter Set Properties
        public String LastName
        {
            set { this.Parameters["LastName"].Value = value; }
        }
        public DateTime DateOfBirth
        {
            set { this.Parameters["DateOfBirth"].Value = value; }
        }
        public String Email
        {
            set { this.Parameters["Email"].Value = value; }
        }
        public String SSN
        {
            set { if (this.Parameters["SSN"] == null) this.Parameters.New("SSN", SqlDbType.NChar, Size: 4, Value: value); else this.Parameters["SSN"].Value = value; }
        }
        public String MemberID
        {
            set { if (this.Parameters["MemberID"] == null) this.Parameters.New("MemberID", SqlDbType.NChar, Size: 11, Value: value); else this.Parameters["MemberID"].Value = value; }
        }
        #endregion

        public InsertEnrollmentRequest()
            : base("InsertEnrollmentRequest")
        {
            this.Parameters.New("LastName", SqlDbType.NVarChar, Size: 100);
            this.Parameters.New("DateOfBirth", SqlDbType.Date);
            this.Parameters.New("Email", SqlDbType.NVarChar, Size: 256);
        }
    }
    public sealed class InsertUserLoginHistory : BaseCCHData
    {
        public String UserName { set { this.Parameters["Username"].Value = value; } }
        public String CallCenterID
        {
            set
            {
                if (this.Parameters["CallCenterID"] == null)
                    this.Parameters.New("CallCenterID", SqlDbType.NVarChar, Size: 36, Value: value);
                else
                    this.Parameters["CallCenterID"].Value = value;
            }
        }
        public String Domain
        {
            set
            {
                if (this.Parameters["Domain"] == null) this.Parameters.New("Domain", SqlDbType.NVarChar, Size: 30, Value: value);
                else this.Parameters["Domain"].Value = value;
            }
        }

        public InsertUserLoginHistory()
            : base("InsertUserLoginHistory")
        {
            this.Parameters.New("Username", SqlDbType.NVarChar, Size: 50);
        }
    }
    public sealed class InsertUserProfile : BaseCCHData
    {
        public Guid UserID { set { this.Parameters["UserID"].Value = value; } }
        public Int32 EmployerID { set { this.Parameters["EmployerID"].Value = value; } }
        public String FirstName { set { this.Parameters["FirstName"].Value = value; } }
        public String LastName { set { this.Parameters["LastName"].Value = value; } }
        public String Email { set { this.Parameters["Email"].Value = value; } }
        public InsertUserProfile()
            : base("InsertUserProfile")
        {
            this.Parameters.New("UserID", SqlDbType.UniqueIdentifier);
            this.Parameters.New("EmployerID", SqlDbType.Int);
            this.Parameters.New("FirstName", SqlDbType.NVarChar, Size: 100);
            this.Parameters.New("LastName", SqlDbType.NVarChar, Size: 100);
            this.Parameters.New("Email", SqlDbType.NVarChar, Size: 256);
        }
    }
    public sealed class UpdateReferral : BaseCCHData
    {
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        public Int32 ReffCCHID { set { this.Parameters["Refferal_CCHID"].Value = value; } }

        public UpdateReferral()
            : base("UpdateWithRefferal")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("Refferal_CCHID", SqlDbType.Int);

        }
    }
    public sealed class UpdateUserEmail : BaseCCHData
    {
        #region Parameter Set Properties
        public String UserName { set { this.Parameters["UserName"].Value = value; } }
        public String Email { set { this.Parameters["Email"].Value = value; } }
        public String UserID { set { this.Parameters["UserID"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        #endregion

        public Int32 ReturnStatus { get { return Int32.Parse(this.Parameters["retVal"].Value.ToString()); } }

        public UpdateUserEmail()
            : base("UpdateUserEmail")
        {
            this.Parameters.New("UserName", SqlDbType.NVarChar, Size: 256);
            this.Parameters.New("Email", SqlDbType.NVarChar, Size: 256);
            this.Parameters.New("UserID", SqlDbType.NVarChar, Size: 36);
            this.Parameters.New("retVal", SqlDbType.Int);
            this.Parameters["retVal"].Direction = ParameterDirection.ReturnValue;
        }
        public void UpdateClientSide(String Email, Int32 CCHID)
        {
            //String tempEmail = this.Parameters["Email"].Value.ToString();
            if (this.Parameters.Count > 0) this.Parameters.Clear();

            this.Parameters.New("Email", SqlDbType.NVarChar, Size: 256, Value: Email);
            this.Parameters.New("CCHID", SqlDbType.Int, Value: CCHID);
            this.Parameters.New("retVal", SqlDbType.Int);
            this.Parameters["retVal"].Direction = ParameterDirection.ReturnValue;
            //base.GetData();
            this.PostData();
        }
    }
    //  lam, 20130227, add UpdateUserPhone for MSB-142
    public sealed class UpdateUserPhone : BaseCCHData
    {
        public String Phone { set { this.Parameters["Phone"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }

        public Int32 ReturnStatus { get { return Int32.Parse(this.Parameters["retVal"].Value.ToString()); } }

        public UpdateUserPhone()  
            : base("UpdateUserPhone")
        {
            //this.Parameters.New("CCHID", SqlDbType.Int);
            //this.Parameters.New("Phone", SqlDbType.NVarChar, Size: 50);
            //this.PostData();
        }
        public void UpdateClientSide(String Phone, Int32 CCHID)
        {
            //String tempEmail = this.Parameters["Email"].Value.ToString();
            if (this.Parameters.Count > 0) this.Parameters.Clear();
            
            this.Parameters.New("CCHID", SqlDbType.Int, Value: CCHID);
            this.Parameters.New("Phone", SqlDbType.VarChar, Size: 50, Value: Phone);
            this.Parameters.New("retVal", SqlDbType.Int);
            this.Parameters["retVal"].Direction = ParameterDirection.ReturnValue;
            //base.GetData();
            this.PostData();
        }
    }
    public sealed class UpdateUserNotificationSettings : BaseCCHData
    {
        #region Sql Parameter Properties
        public Int32? CCHID
        {
            get
            {
                Int32 o = default(Int32);
                using (CCHParamList cpl = this.Parameters)
                    if (cpl.Has("CCHID") && Int32.TryParse(cpl["CCHID"].Value.ToString(), out o))
                        return o;
                    else
                        return null;
            }
            set
            {
                using (CCHParamList cpl = this.Parameters)
                    if (cpl.Has("CCHID"))
                        cpl["CCHID"].Value = value;
                    else
                        cpl.New("CCHID", SqlDbType.Int, Value: value);
            }
        }
        public Boolean? OptInEmailAlerts
        {
            get
            {
                Boolean b = default(Boolean);
                using (CCHParamList cpl = this.Parameters)
                    if (cpl.Has("OptInEmailAlerts") && Boolean.TryParse(cpl["OptInEmailAlerts"].Value.ToString(), out b))
                        return b;
                    else
                        return null;
            }
            set
            {
                using (CCHParamList cpl = this.Parameters)
                    if (cpl.Has("OptInEmailAlerts"))
                        cpl["OptInEmailAlerts"].Value = value;
                    else
                        cpl.New("OptInEmailAlerts", SqlDbType.Bit, Value: value);
            }
        }
        public Boolean? OptInTextMsgAlerts
        {
            get
            {
                Boolean b = default(Boolean);
                using (CCHParamList cpl = this.Parameters)
                    if (cpl.Has("OptInTextMsgAlerts") && Boolean.TryParse(cpl["OptInTextMsgAlerts"].Value.ToString(), out b))
                        return b;
                    else
                        return null;
            }
            set
            {
                using (CCHParamList cpl = this.Parameters)
                    if (cpl.Has("OptInTextMsgAlerts"))
                        cpl["OptInTextMsgAlerts"].Value = value;
                    else
                        cpl.New("OptInTextMsgAlerts", SqlDbType.Bit, Value: value);
            }
        }
        public Boolean? OptInConciergeAlerts
        {
            get
            {
                Boolean b = default(Boolean);
                using (CCHParamList cpl = this.Parameters)
                    if (cpl.Has("OptInConciergeAlerts") && Boolean.TryParse(cpl["OptInConciergeAlerts"].Value.ToString(), out b))
                        return b;
                    else
                        return null;
            }
            set
            {
                using (CCHParamList cpl = this.Parameters)
                    if (cpl.Has("OptInConciergeAlerts"))
                        cpl["OptInConciergeAlerts"].Value = value;
                    else
                        cpl.New("OptInConciergeAlerts", SqlDbType.Bit, Value: value);
            }
        }
        public String MobilePhone
        {
            get
            {
                using (CCHParamList cpl = this.Parameters)
                {
                    if (cpl.Has("MobilePhone"))
                        return cpl["MobilePhone"].Value.ToString();
                    else
                        return null;
                }
            }
            set
            {
                using (CCHParamList cpl = this.Parameters)
                {
                    if (cpl.Has("MobilePhone"))
                        cpl["MobilePhone"].Value = value;
                    else
                        cpl.New("MobilePhone", SqlDbType.NVarChar, 36, value);
                }
            }
        }
        #endregion

        public UpdateUserNotificationSettings()
            : base("UpdateUserNotificationSettings")
        { }
    }

    public sealed class SaveAddress : BaseCCHData
    {
        public int CCHID { get { return (int)this.Parameters["CCHID"].Value; } set { this.Parameters["CCHID"].Value = value; } }
        public String Address1 { get { return (String)this.Parameters["Address1"].Value; } set { this.Parameters["Address1"].Value = value; } }
        public String Address2 { get { return (String)this.Parameters["Address2"].Value; } set { this.Parameters["Address2"].Value = value; } }
        public String City { get { return (String)this.Parameters["City"].Value; } set { this.Parameters["City"].Value = value; } }
        public String State { get { return (String)this.Parameters["State"].Value; } set { this.Parameters["State"].Value = value; } }
        public String Zip { get { return (String)this.Parameters["Zip"].Value; } set { this.Parameters["Zip"].Value = value; } }

        public SaveAddress()
            : base("SaveAddress")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("Address1", SqlDbType.VarChar, 100);
            this.Parameters.New("Address2", SqlDbType.VarChar, 100);
            this.Parameters.New("City", SqlDbType.VarChar, 50);
            this.Parameters.New("State", SqlDbType.VarChar, 2);
            this.Parameters.New("Zip", SqlDbType.VarChar, 15);
        }
    }
    public sealed class sc_GetAvatars : BaseCCHData
    {
        public sc_GetAvatars()
            : base("sc_GetAvatars")
        { }
    }
    public sealed class sc_GetMemberAvatars : BaseCCHData
    {
        public int CCHID
        {
            get
            {
                return (int)this.Parameters["CCHID"].Value;
            }
            set
            {
                this.Parameters["CCHID"].Value = value;
            }
        }

        public sc_GetMemberAvatars()
            : base("sc_getmemberavatars")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
        }
    }
    public sealed class sc_GetProviderRatings : BaseCCHData
    {
        public int CCHID
        {
            get
            {
                return int.Parse(this.Parameters["CCHID"].Value.ToString());
            }
            set
            {
                this.Parameters["CCHID"].Value = value;
            }
        }
        public string Category
        {
            get
            {
                return this.Parameters["Category"].Value.ToString();
            }
            set
            {
                this.Parameters["Category"].Value = value;
            } 
        }
        public int EmployerID
        {
            get
            {
                return int.Parse(this.Parameters["EmployerID"].Value.ToString());
            }
            set
            {
                this.Parameters["EmployerID"].Value = value;
            }
        }

        public sc_GetProviderRatings()
            : base("sc_GetProviderRatings")
        {
            this.Parameters.New("CCHID", System.Data.SqlDbType.Int);
            this.Parameters.New("Category", System.Data.SqlDbType.NVarChar, 255);
            this.Parameters.New("EmployerID", System.Data.SqlDbType.Int);
        }
    }
    public sealed class sc_InsertMemberAvatar : BaseCCHData
    {
        public int CCHID
        {
            get
            {
                return (int)this.Parameters["CCHID"].Value;
            }
            set
            {
                this.Parameters["CCHID"].Value = value; 
            }
        }
        public int AvatarID
        {
            get
            {
                return (int)this.Parameters["AvatarID"].Value;
            }
            set
            {
                this.Parameters["AvatarID"].Value = value;
            }
        }

        public sc_InsertMemberAvatar()
            : base("sc_InsertMemberAvatar")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("AvatarID", SqlDbType.Int);
        }
    }
    public sealed class sc_InsertUpdateProviderRating : BaseCCHData
    {
        public int CCHID
        {
            get { return (int)(this.Parameters["CCHID"].Value ?? 0); }
            set { this.Parameters["CCHID"].Value = value; }
        }
        public int ProviderID
        {
            get { return (int)(this.Parameters["ProviderID"].Value ?? 0); }
            set { this.Parameters["ProviderID"].Value = value; }
        }
        public string Category
        {
            get { return this.Parameters["Category"].Value.ToString(); }
            set { this.Parameters["Category"].Value = value; }
        }
        public int Score
        {
            get { return (int)(this.Parameters["Score"].Value ?? 0); }
            set { this.Parameters["Score"].Value = value; }
        }
        public string Review
        {
            get { return this.Parameters["Review"].Value.ToString(); }
            set { this.Parameters["Review"].Value = value; }
        }
        public int EmployerID
        {
            get { return (int)(this.Parameters["EmployerID"].Value ?? 0); }
            set { this.Parameters["EmployerID"].Value = value; }
        }

        public sc_InsertUpdateProviderRating() 
            : base("sc_InsertUpdateProviderRating")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("ProviderID", SqlDbType.Int);
            this.Parameters.New("Category", SqlDbType.NVarChar, 50);
            this.Parameters.New("Score", SqlDbType.Int);
            this.Parameters.New("Review", SqlDbType.Text);
            this.Parameters.New("EmployerID", SqlDbType.Int);
        }
    }

    public sealed class SC_AddNewProviderRating : BaseCCHData
    {
        public int CCHID
        {
            get { return (int)(this.Parameters["CCHID"].Value ?? 0); }
            set { this.Parameters["CCHID"].Value = value; }
        }
        public DataTable CCHIDs
        {
            set { this.Parameters["CCHIDs"].Value = value; }
        }
        public int ProviderID
        {
            get { return (int)(this.Parameters["ProviderID"].Value ?? 0); }
            set { this.Parameters["ProviderID"].Value = value; }
        }
        public int OrganizationID
        {
            get { return (int)(this.Parameters["OrganizationID"].Value ?? 0); }
            set { this.Parameters["OrganizationID"].Value = value; }
        }
        public string Category
        {
            get { return this.Parameters["Category"].Value.ToString(); }
            set { this.Parameters["Category"].Value = value; }
        }
        public int Rating
        {
            get { return (int)(this.Parameters["Rating"].Value ?? 0); }
            set { this.Parameters["Rating"].Value = value; }
        }
        public string Review
        {
            get { return this.Parameters["Review"].Value.ToString(); }
            set { this.Parameters["Review"].Value = value; }
        }
        public int EmployerID
        {
            get { return (int)(this.Parameters["EmployerID"].Value ?? 0); }
            set { this.Parameters["EmployerID"].Value = value; }
        }

        public SC_AddNewProviderRating()
            : base("SC_AddNewProviderRating")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("CCHIDs", SqlDbType.Structured);
            this.Parameters.New("ProviderID", SqlDbType.Int);
            this.Parameters.New("OrganizationID", SqlDbType.Int);
            this.Parameters.New("Category", SqlDbType.NVarChar, 50);
            this.Parameters.New("Rating", SqlDbType.Int);
            this.Parameters.New("Review", SqlDbType.Text);
            this.Parameters.New("EmployerID", SqlDbType.Int);
        }
    }
    
    public sealed class sc_GetProviderLookup : BaseCCHData
    {
        public string Category
        {
            set { this.Parameters["Category"].Value = value; }
        }
        public string State
        {
            set { this.Parameters["State"].Value = value; }
        }
        public string City
        {
            set { this.Parameters["City"].Value = value; }
        }

        public object OrgID {
            set {
                this.Parameters["OrgID"].Value = (value == null) ? null : value;
                //working
                //if (value == null)
                //{
                //    this.Parameters["OrgID"].Value = null;
                //}
                //else
                //{
                //    this.Parameters["OrgID"].Value = value;
                //}
            }
        }

        public DataTable providers {
            set { }
            get { return this.Tables[0]; }
        }

        public sc_GetProviderLookup()
            : base("sc_GetProviderLookup")
        {
            this.Parameters.New("Category", SqlDbType.VarChar, 50);
            this.Parameters.New("State", SqlDbType.Char, 2);
            this.Parameters.New("City", SqlDbType.VarChar, 50);
            this.Parameters.New("OrgID", SqlDbType.Int);
        }
    }
    public sealed class sc_AddNewProviderRating : BaseCCHData {
        public int CCHID {
            set { this.Parameters["CCHID"].Value = value; }
        }
        public int ProviderID
        {
            set { this.Parameters["ProviderID"].Value = value; }
        }
        public int EmployerID {
            set { this.Parameters["EmployerID"].Value = value; }
        }
        public string Category
        {
            set { this.Parameters["Category"].Value = value; }
        }
        public int Rating
        {
            set { this.Parameters["Rating"].Value = value; }
        }
        public string Review
        {
            set { this.Parameters["Review"].Value = (value ?? string.Empty); }
        }

        public sc_AddNewProviderRating()
            :base ("sc_AddNewProviderRating")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("ProviderID", SqlDbType.Int);
            this.Parameters.New("EmployerID", SqlDbType.Int);
            this.Parameters.New("Category", SqlDbType.VarChar, 255);
            this.Parameters.New("Rating", SqlDbType.Int);
            this.Parameters.New("Review", SqlDbType.Text);
        }
    }

    public sealed class sc_GetThermometerValue : BaseCCHData {
        public int CCHID {
            set { this.Parameters["CCHID"].Value = value; }
        }
        public string Category
        {
            set { this.Parameters["Category"].Value = value; }
        }
        public sc_GetThermometerValue() 
            : base("sc_GetThermometerValue") 
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("Category", SqlDbType.VarChar, 50);
        }
        public DataTable ThermometerValues
        {
            set { }
            get { return this.Tables[0]; }
        }
    }

    public sealed class SC_GetRecentProviders : BaseCCHData
    {
        public int CCHID
        {
            set { this.Parameters["CCHID"].Value = value; }
        }
        public string Category
        {
            set { this.Parameters["Category"].Value = value; }
        }
        public int EmployerID
        {
            set { this.Parameters["EmployerID"].Value = value; }
        }
        public SC_GetRecentProviders()
            : base("SC_GetRecentProviders")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("Category", SqlDbType.VarChar, 50);
            this.Parameters.New("EmployerID", SqlDbType.Int);
        }
        public DataTable Providers
        {
            set { }
            get { return this.Tables[0]; }
        }
    }

    public sealed class SC_GetRecentProvidersRX : BaseCCHData
    {
        public int CCHID
        {
            set { this.Parameters["CCHID"].Value = value; }
        }
        public int EmployerID
        {
            set { this.Parameters["EmployerID"].Value = value; }
        }
        public SC_GetRecentProvidersRX()
            : base("SC_GetRecentProvidersRX")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("EmployerID", SqlDbType.Int);
        }
        public DataTable Providers
        {
            set { }
            get { return this.Tables[0]; }
        }
    }

    public sealed class SC_GetMedication : BaseCCHData 
    {
        public int CCHID {
            set { this.Parameters["CCHID"].Value = value; }
        }
        public SC_GetMedication()
            : base("SC_GetMedication")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
        }
        public DataTable Providers
        {
            set { }
            get { return this.Tables[0]; }
        }
    }

    public sealed class SC_GetFairPriceAlternatives : BaseCCHData
    {
        public int CCHID
        {
            set { this.Parameters["CCHID"].Value = value; }
        }
        public string Category
        {
            set { this.Parameters["Category"].Value = value; }
        }
        public object SubCategory
        {            
            set { this.Parameters["SubCategory"].Value = value; }
        }
        public int SpecialtyID
        {
            set { this.Parameters["SpecialtyID"].Value = value; }
        }
        public int RowMin
        {
            set { this.Parameters["RowMin"].Value = value; }
        }
        public int RowMax 
        {
            set { this.Parameters["RowMax"].Value = value; }
        }
        public double Lat
        {
            set { this.Parameters["Lat"].Value = value; }
        }
        public double Lon
        {
            set { this.Parameters["Lon"].Value = value; }
        }

        public SC_GetFairPriceAlternatives()
            : base("SC_GetFairPriceAlternatives")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("Category", SqlDbType.VarChar, 50);
            this.Parameters.New("SubCategory", SqlDbType.VarChar, 50);
            this.Parameters.New("SpecialtyID", SqlDbType.Int);
            this.Parameters.New("RowMin", SqlDbType.Int);
            this.Parameters.New("RowMax", SqlDbType.Int);
            this.Parameters.New("Lat", SqlDbType.Float);
            this.Parameters.New("Lon", SqlDbType.Float);
        }
        public DataTable Providers
        {
            set { }
            get { return this.Tables[0]; }
        }
    }

    public sealed class SC_GetFairPriceAlternativesRX : BaseCCHData
    {
        public int CCHID
        {
            set { this.Parameters["CCHID"].Value = value; }
        }
        public string GPI
        {
            set { this.Parameters["GPI"].Value = value; }
        }
        public int GenericIndicator
        {
            set { this.Parameters["GenericIndicator"].Value = value; }
        }
        public int RowMin
        {
            set { this.Parameters["RowMin"].Value = value; }
        }
        public int RowMax
        {
            set { this.Parameters["RowMax"].Value = value; }
        }
        public double Lat
        {
            set { this.Parameters["Lat"].Value = value; }
        }
        public double Lon
        {
            set { this.Parameters["Lon"].Value = value; }
        }

        public SC_GetFairPriceAlternativesRX()
            : base("SC_GetFairPriceAlternativesRX")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("GPI", SqlDbType.VarChar, 50);
            this.Parameters.New("GenericIndicator", SqlDbType.Int);
            this.Parameters.New("RowMin", SqlDbType.Int);
            this.Parameters.New("RowMax", SqlDbType.Int);
            this.Parameters.New("Lat", SqlDbType.Float);
            this.Parameters.New("Lon", SqlDbType.Float);
        }
        public DataTable Providers
        {
            set { }
            get { return this.Tables[0]; }
        }
    }

    public sealed class GetSavingsChoicePastCare : BaseCCHData
    {
        public int CCHID
        {
            set { this.Parameters["CCHID"].Value = value; }
        }
        public GetSavingsChoicePastCare()
            : base("GetSavingsChoicePastCare")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);            
        }
        public DataTable PastCare
        {
            set { }
            get { return this.Tables[0]; }
        }
    }

    public sealed class SC_GetOptDecisionList : BaseCCHData
    {
        public string CatID {
            set { this.Parameters["CatID"].Value = value; }
        }
        public SC_GetOptDecisionList()
            : base("SC_GetOptDecisionList")
        {
            this.Parameters.New("CatID", SqlDbType.VarChar);
        }
        public DataTable DecisionList
        {
            set { }
            get { return this.Tables[0]; }
        }
    }

    public sealed class SC_ssInitSession : BaseCCHData
    {
        public int CCHID
        {
            set { this.Parameters["CCHID"].Value = value; }
        }
        public string Category
        {
            set { this.Parameters["categoryid"].Value = value; }
        }
        public SC_ssInitSession()
            : base("SC_ssInitSession")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("categoryid", SqlDbType.VarChar);
        }
        public DataTable dbSessionData
        {
            set { }
            get { return this.Tables[0]; }
        }
    }
    
    public sealed class SC_ssInsSwitchWorkFlow : BaseCCHData
    {
        public int CCHID
        {
            set { this.Parameters["CCHID"].Value = value; }
        }
        public int SSID
        {
            set { this.Parameters["sessionid"].Value = value; }
        }
        public int Stepnum
        {
            set { this.Parameters["stepnum"].Value = value; }
        }
        public int DecisionID
        {
            set { this.Parameters["decisionid"].Value = value; }
        }
        public string EmailDate
        {
            set
            {
                if (value == null)
                {
                    this.Parameters["emaildate"].Value = null;
                }
                else
                {
                    this.Parameters["emaildate"].Value = value;
                }
            }
        }
        public string ProviderList {
            set {
                if (value == null)
                {
                    this.Parameters["providerlist"].Value = null;
                }
                else
                {
                    this.Parameters["providerlist"].Value = value;
                }
            }
        }
        public SC_ssInsSwitchWorkFlow()
            : base("SC_ssInsSwitchWorkFlow")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters.New("sessionid", SqlDbType.Int);
            this.Parameters.New("stepnum", SqlDbType.Int);
            this.Parameters.New("decisionid", SqlDbType.Int);
            this.Parameters.New("emaildate", SqlDbType.Date);
            this.Parameters.New("providerlist", SqlDbType.VarChar);
        }
    }

    public sealed class SC_ssEndSession : BaseCCHData
    {
        public int sessionID
        {
            set { this.Parameters["sessionID"].Value = value; }
        }
        public int state
        {
            set { this.Parameters["state"].Value = value; }
        }
        public SC_ssEndSession()
            : base("SC_ssEndSession")
        {
            this.Parameters.New("sessionID", SqlDbType.Int);
            this.Parameters.New("state", SqlDbType.Int);
        }
    }

    public sealed class SC_GetSubCategories : BaseCCHData
    {
        public string Subcategory
        {
            set { this.Parameters["subcategory"].Value = value; }
        }
        public SC_GetSubCategories()
            : base("SC_GetSubCategories")
        {
            this.Parameters.New("subcategory", SqlDbType.VarChar);
        }
        public DataTable Subcategories
        {
            set { }
            get { return this.Tables[0]; }
        }
    }

    public sealed class SC_GetLastSCIQUrl : BaseCCHData
    {
        public int CCHID
        {
            set { this.Parameters["cchid"].Value = value; }
        }
        public SC_GetLastSCIQUrl()
            : base("SC_GetLastSCIQUrl")
        {
            this.Parameters.New("cchid", SqlDbType.VarChar);
        }
        public DataTable getLastSCIQURL
        {
            set { }
            get {
                if (this.Tables.Count > 0)
                {
                    return this.Tables[0];
                }
                else {
                    return null;
                }
            }
        }
    }

    public sealed class SC_QuickSuggestions : BaseCCHData
    {
        public int CCHID
        {
            set { this.Parameters["cchid"].Value = value; }
        }
        public int EmployerID
        {
            set { this.Parameters["EmployerID"].Value = value; }
        }
        public SC_QuickSuggestions()
            : base("SC_QuickSuggestions")
        {
            this.Parameters.New("cchid", SqlDbType.VarChar);
            this.Parameters.New("EmployerID", SqlDbType.Int);
        }
        public DataTable getQuickSuggestions
        {
            set { }
            get
            {
                if (this.Tables.Count > 0)
                {
                    return this.Tables[0];
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public sealed class sc_getAvailableCategories : BaseCCHData
    {
        public sc_getAvailableCategories()
            : base("sc_getAvailableCategories")
        {
            
        }
        public DataTable AvailableCategories
        {
            set { }
            get { return this.Tables[0]; }
        }
    }
    
    public sealed class SC_GetCurrentMeasurementPeriod : BaseCCHData
    {
        public SC_GetCurrentMeasurementPeriod()
            : base("SC_GetCurrentMeasurementPeriod")
        {
            
        }
        public DataTable MeasurementPeriod
        {
            set { }
            get { return this.Tables[0]; }
        }
    }
}