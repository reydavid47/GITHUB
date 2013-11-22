using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Data;
using System.IO;

namespace ClearCostWeb.SavingsChoice
{
    public partial class SavingsChoiceImaging : System.Web.UI.Page
    {
        private string[] rateText = {                                
                                "You did not visit any radiology facilities in the past year.",
                                "Please rate your satisfaction with the following radiology facilities that you have visited."
                            };
        protected DataTable Providers, ProviderAvatars;

        protected int EmployerID
        {
            get
            {   
                //return 7;
                //if ((Request.QueryString["e"] ?? "").Trim() != string.Empty)
                //    return int.Parse(Request.QueryString["e"]);
                //else
                //    if (Debugger.IsAttached)
                //        return 7;
                //    else
                return int.Parse(ThisSession.EmployerID);
            }
        }
        protected int PrimaryCCHID
        {
            get
            {
                //return 110733;
                //if ((Request.QueryString["cchid"] ?? "").Trim() != String.Empty)
                //    return int.Parse(Request.QueryString["cchid"]);
                //else
                //    if (Debugger.IsAttached)
                //        return 110733;
                //    else
                return ThisSession.CCHID;
            }
        }
        protected string SessionID
        {
            get
            {
                return ThisSession.UserLogginID;
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            JavaScriptHelper.IncludeJQuerySlider(Page.ClientScript);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetData();
                prepAddProviderUI();
            }
        }

        protected int ProviderID, OrganizationID, UserRating;
        protected String ProviderName, FirstName, Address1, Address2, City, State, Zipcode, avatarClass, avatarHTML, Review;
        protected String ReadyDataForItem(RepeaterItem i)
        {
            DataRow dr = (i.DataItem as DataRowView).Row;
            if (dr["providerid"] == DBNull.Value)
            {
                ProviderID = 0;
                OrganizationID = (dr["OrganizationID"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["organizationID"]);
                ProviderName = dr["ProviderName"].ToString();
                Address1 = dr["Address1"].ToString();
                Address2 = dr["Address2"].ToString();
                City = dr["City"].ToString();
                State = dr["State"].ToString();
                Zipcode = dr["Zipcode"].ToString();
                Review = dr["review"].ToString();
            }
            else
            {
                ProviderID = int.Parse(dr["providerid"].ToString());
                OrganizationID = (dr["OrganizationID"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["organizationID"]);
                ProviderName = dr["ProviderName"].ToString();
                Address1 = dr["Address1"].ToString();
                Address2 = dr["Address2"].ToString();
                City = dr["City"].ToString();
                State = dr["State"].ToString();
                Zipcode = dr["Zipcode"].ToString();
                Review = dr["review"].ToString();
            }

            if (int.Parse(dr["Rating"].ToString()) > -1)
            {
                UserRating = int.Parse(dr["Rating"].ToString());
                if (UserRating > 5)
                {
                    UserRating = 5;
                }
            }

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    foreach (Image imgAvatar in ImgsForProvider(dr.Field<int>("id")))
                    {
                        imgAvatar.RenderControl(htw);
                    }
                }
                return sw.ToString();
            }
        }

        private IEnumerable<Image> ImgsForProvider(int providerId)
        {
            EnumerableRowCollection<DataRow> drs = ProviderAvatars.AsEnumerable().Where(a => a.Field<int>("id") == providerId);
            foreach (DataRow dr in drs)
            {
                avatarClass = dr.Field<string>("Avatar").Replace(".png", "");
                using (Image i = new Image())
                {
                    if (dr.Field<string>("Avatar") != null)
                    {
                        i.ImageUrl = ResolveUrl("~/Images/Avatars/" + dr.Field<string>("Avatar"));
                    }
                    else
                    {
                        i.ImageUrl = ResolveUrl("~/Images/Avatars/outline.png");
                    }
                    i.Width = new Unit(60, UnitType.Pixel);
                    i.Height = new Unit(60, UnitType.Pixel);
                    i.AlternateText = FirstName;
                    i.Attributes.Add("cid", dr.Field<int>("cchid").ToString());
                    yield return i;
                }
            }
        }
        private void GetData()
        {
            ratingText.Text = rateText[0];
            //using (GetEmployerConnString gecs = new GetEmployerConnString(EmployerID))
            //{
            //    gecs.GetFrontEndData();
            //    if (gecs.Tables.Count > 0 && gecs.Tables[0].Rows.Count > 0)
            //    {
            //        ThisSession.CnxString = gecs.ConnectionString;
                    using (sc_GetProviderRatings gpr = new sc_GetProviderRatings())
                    {
                        gpr.Category = "Imaging";
                        gpr.CCHID = this.PrimaryCCHID;
                        gpr.EmployerID = this.EmployerID;
                        gpr.GetData();

                        if (gpr.Tables.Count >= 1 && gpr.Tables[0].Rows.Count > 0) {
                            Providers = gpr.Tables[0];
                            ratingText.Text = rateText[1];
                        }                        
                        if (gpr.Tables.Count >= 2 && gpr.Tables[1].Rows.Count > 0)
                            ProviderAvatars = gpr.Tables[1];

                    }
                //}
                rptSatisfaction.DataSource = Providers;
                rptSatisfaction.DataBind();
            //}
        }

        protected void prepAddProviderUI()
        {
            Control addProvider = Page.LoadControl("~/Controls/SavingsChoiceIQAddProvider.ascx");
            addProviderControl.Controls.Clear();
            addProviderControl.Controls.Add(addProvider);
        }
    }
}