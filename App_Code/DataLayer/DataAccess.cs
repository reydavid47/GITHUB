using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using System.Web.Configuration;

namespace ClearCostWeb
{
    public partial class DataAccess
    {
        private String Procedure = string.Empty;
        private CommandType ProcedureType = CommandType.StoredProcedure;
        private List<Type> RequiredEntities = null;
        private Dictionary<Type, List<object>> Results = null;
        private List<SqlParameter> Parameters = null;

        public List<T> BuildCollection<T>(SqlDataReader rdr)
        {
            Type typ = typeof(T);
            List<T> ret = new List<T>();
            T entity;

            PropertyInfo[] props = typ.GetProperties();

            while (rdr.Read())
            {
                entity = Activator.CreateInstance<T>();

                foreach (PropertyInfo col in props)
                {
                    if (rdr[col.Name].Equals(DBNull.Value))
                        col.SetValue(entity, null, null);
                    else
                        col.SetValue(entity, rdr[col.Name], null);
                }

                ret.Add(entity);
            }

            return ret;
        }

        public DataAccess(String ProcedureName)
        {
            this.Procedure = ProcedureName;
        }
        public void AddEntityRequirement(Type typ)
        {
            if (RequiredEntities == null)
                RequiredEntities = new List<Type>();
            RequiredEntities.Add(typ);
        }
        public void AddParameter(String Name, object Value)
        {
            if (Parameters == null)
                Parameters = new List<SqlParameter>();
            Parameters.Add(new SqlParameter(Name, Value));
        }
        public virtual void GetFrontEndData()
        {
            Results = new Dictionary<Type, List<object>>();
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand(Procedure, conn))
                {
                    comm.Parameters.AddRange(Parameters.ToArray());
                    conn.Open();
                    try
                    {
                        using (SqlDataReader rdr = comm.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            foreach (Type typ in RequiredEntities)
                            {
                                
                            }
                        }
                    }
                    catch (Exception) { }
                    finally { if (conn.State != ConnectionState.Closed) conn.Close(); }
                }
            }
        }
    }
}