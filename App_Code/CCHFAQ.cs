using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for CCHContentManagement
/// </summary>
namespace ClearCostWeb
{
    [Serializable()] 
    public class FAQContent
    {
        public Int32 FAQContentID { get; set; }
        public Int32 EmployerID { get; set; }
        public String FAQID { get; set; }
        public String FAQText { get; set; }
        public Boolean NeedUserInput { get; set; }
    }
    [System.ComponentModel.DesignerCategory("")]
    public sealed class GetEmployerFAQCategory : BaseCCHData
    {
        public GetEmployerFAQCategory()
            : base("GetEmployerFAQCategory")
        {
        }

        public override void GetFrontEndData()
        {
            base.GetFrontEndData();
        }
    }

    public sealed class GetEmployerFAQContent : BaseCCHData
    {
        public Int32 EmployerID { set { this.Parameters["EmployerID"].Value = value; } }
        public String FAQID { set { this.Parameters["FAQID"].Value = value; } }

        public List<FAQContent> FAQContentList
        {
            get
            {
                if (this.Tables.Count < 1) return new List<FAQContent>();
                using (DataTable dt = this.Tables[0])
                {
                    return (from fc in dt.AsEnumerable()
                            orderby fc.GetData<Int32>("FAQContentID") ascending  
                            select new FAQContent
                            {
                                FAQContentID = fc.Field<Int32>("FAQContentID"),
                                EmployerID = fc.Field<Int32>("EmployerID"),
                                FAQID = fc.Field<String>("FAQID"),
                                FAQText = fc.Field<String>("FAQText"),
                                NeedUserInput = fc.Field<Boolean>("NeedUserInput")
                            }).ToList<FAQContent>();
                }
            }
        }

        public GetEmployerFAQContent(Int32 inputEmployerID, String inputFAQID)
            : base("GetEmployerFAQContent")
        {
            this.Parameters.New("EmployerID", SqlDbType.Int, Size: 0, Value: inputEmployerID);
            this.Parameters.New("FAQID", SqlDbType.VarChar, Size: 50, Value: inputFAQID);
        }

        public override void GetFrontEndData()
        {
            base.GetFrontEndData();
        }
    }

    public sealed class EmployerFAQContent : BaseCCHData
    {
        public void UpdateEmployerFAQContent(Int32 EID, String FID, List<FAQContent> faqcList)
        {
            SqlParameter EmployerID = new SqlParameter("EmployerID", SqlDbType.Int);
            SqlParameter FAQID = new SqlParameter("FAQID", SqlDbType.VarChar, 50);
            SqlParameter EmployerFAQContent = new SqlParameter("EmployerFAQContentList", SqlDbType.Structured);

            DataTable dtEmployerFAQContent = new DataTable("EmployerFAQContent");
            dtEmployerFAQContent.Columns.Add("FAQContentID", typeof(int));
            dtEmployerFAQContent.Columns.Add("EmployerID", typeof(int));
            dtEmployerFAQContent.Columns.Add("FAQID", typeof(string));
            dtEmployerFAQContent.Columns.Add("FAQText", typeof(string));
            dtEmployerFAQContent.Columns.Add("NeedUserInput", typeof(bool));

            if (faqcList != null && faqcList.Count > 0)
            {
                
                for(int i = 0; i < faqcList.Count; i++)
                {
                    DataRow drEmployerFAQContentRow = dtEmployerFAQContent.NewRow();

                    drEmployerFAQContentRow["FAQContentID"] = 0;  //  just a dummy id
                    drEmployerFAQContentRow["EmployerID"] = faqcList[i].EmployerID;
                    drEmployerFAQContentRow["FAQID"] = faqcList[i].FAQID;
                    drEmployerFAQContentRow["FAQText"] = faqcList[i].FAQText;
                    drEmployerFAQContentRow["NeedUserInput"] = faqcList[i].NeedUserInput;

                    dtEmployerFAQContent.Rows.Add(drEmployerFAQContentRow);
                }
                EmployerFAQContent.Value = dtEmployerFAQContent;
            }

            EmployerID.Value = EID;
            FAQID.Value = FID;

            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand("UpdateEmployerFAQContent", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.Add(EmployerID);
                    comm.Parameters.Add(FAQID);
                    comm.Parameters.Add(EmployerFAQContent);
                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    { }
                    finally
                    { conn.Close(); }
                }
            }
        }
    }

    public sealed class UpdateHearCCH : BaseCCHData
    {
        public String HearCCH { set { this.Parameters["HearCCH"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }

        public Int32 ReturnStatus { get { return Int32.Parse(this.Parameters["retVal"].Value.ToString()); } }

        public UpdateHearCCH()
            : base("UpdateHearCCH")
        {
        }

        public void UpdateClientSide(String HearCCH, Int32 CCHID)
        {
            if (this.Parameters.Count > 0) this.Parameters.Clear();

            this.Parameters.New("CCHID", SqlDbType.Int, Value: CCHID);
            this.Parameters.New("HearCCH", SqlDbType.VarChar, Size: 2600, Value: HearCCH);
            this.Parameters.New("retVal", SqlDbType.Int);
            this.Parameters["retVal"].Direction = ParameterDirection.ReturnValue;
            this.PostData();
        }
    }
}