using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Diagnostics;

namespace ClearCostWeb.SavingsChoice
{
    public partial class SavingsChoiceWelcome : System.Web.UI.Page
    {
        private static DataTable Avatars;
        //{
        //    get { return (DataTable)HttpContext.Current.Session["Avatars"]; }
        //    set { HttpContext.Current.Session["Avatars"] = value; }
        //}
        private DataTable MemberAvatars
        {
            get
            {
                return (DataTable)HttpContext.Current.Session["SCWelcomeMemberAvatar"];
            }
            set
            {
                HttpContext.Current.Session["SCWelcomeMemberAvatar"] = value;
            }
        }

        protected int MemberCount { get { return MemberAvatars.Rows.Count; } }
        protected string[] MemberPictures { get { return (from a in MemberAvatars.AsEnumerable() select a.Field<string>("AvatarFileName")).ToArray<string>(); } }
        protected int[] MemberAvatarIDs { get { return (from a in MemberAvatars.AsEnumerable() select a.Field<int>("AvatarID")).ToArray<int>(); } }
        protected int[] MemberCCHIDs { get { return (from a in MemberAvatars.AsEnumerable() select a.Field<int>("CCHID")).ToArray<int>(); } }

        protected Dictionary<int, string> MemberNames
        {
            get { return (from m in MemberAvatars.AsEnumerable() select m).ToDictionary(m => m.Field<int>("CCHID"), m => m.Field<string>("FirstName") + " " + m.Field<string>("LastName")); }
        }
        protected Dictionary<int, string> AvatarFilesByID
        {
            get { return (from a in Avatars.AsEnumerable() select a).ToDictionary(a => a.Field<int>("AvatarID"), a => a.Field<string>("AvatarFileName")); }
        }

        private int EmployerID
        {
            get
            {
                //if ((Request.QueryString["e"] ?? "").Trim() != string.Empty)
                //    return int.Parse(Request.QueryString["e"]);
                //else
                //    if (Debugger.IsAttached)
                //        return 7;
                //    else
                        return int.Parse(ThisSession.EmployerID);
            }
        }
        protected string sessionID {
            get {
                return ThisSession.UserLogginID;
            }
        }
        protected int PrimaryCCHID
        {
            get
            {
                //if ((Request.QueryString["cchid"] ?? "").Trim() != String.Empty) //Allow for a cchid flag
                //    return int.Parse(Request.QueryString["cchid"]);
                //else
                //    if (Debugger.IsAttached)
                //        return 110733;
                //    else
                        return ThisSession.CCHID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using (sc_GetAvatars ga = new sc_GetAvatars())
                {
                    ga.GetFrontEndData();
                    if (ga.Tables.Count > 0 && ga.Tables[0].Rows.Count > 0)
                        Avatars = ga.Tables[0];
                }
                rptAvailableAvatars.DataSource = Avatars;
                rptAvailableAvatars.DataBind();

                //using (GetEmployerConnString gecs = new GetEmployerConnString(EmployerID))
                //{
                //    gecs.GetFrontEndData();
                //    if (gecs.Tables.Count > 0 && gecs.Tables[0].Rows.Count > 0)
                //    {
                //        ThisSession.CnxString = gecs.ConnectionString;
                using (sc_GetMemberAvatars gma = new sc_GetMemberAvatars())
                {
                    gma.CCHID = PrimaryCCHID;
                    gma.GetData(ThisSession.CnxString);
                    if (gma.Tables.Count > 0 && gma.Tables[0].Rows.Count > 0)
                    {
                        MemberAvatars = gma.Tables[0];
                    }
                }

                //    }
                //}
            }
        }
        protected void UpdateMemberAvatar(object sender, ImageClickEventArgs e)
        {
            int cchid = 0,
                avatarID = 0;
            if (int.TryParse(hfChosenMember.Value, out cchid) &&
                int.TryParse((sender as ImageButton).CommandArgument, out avatarID))
            {
                using (sc_InsertUpdateMemberAvatar ima = new sc_InsertUpdateMemberAvatar())
                {
                    ima.CCHID = cchid;
                    ima.AvatarID = avatarID;
                    ima.PostData();
                    MemberAvatars.AsEnumerable().Where(ma => ma.Field<int>("CCHID") == cchid).First().SetField("AvatarID", avatarID);
                    MemberAvatars.AsEnumerable().Where(ma => ma.Field<int>("CCHID") == cchid).First().SetField("AvatarFileName", AvatarFilesByID[avatarID]);
                }
            }

            hfChosenMember.Value = "";
        }

        protected string GetAvatarForMember(int MemberIndex)
        {
            if (MemberAvatarIDs[MemberIndex] > -1)
                return ResolveUrl("~/Images/Avatars/" + MemberPictures[MemberIndex]);
            else
                return ResolveUrl("~/Images/Avatars/outline.png");
        }
        protected string GetMemberName(int MemberIndex)
        {
            int cchid = MemberCCHIDs[MemberIndex];
            return MemberNames[cchid];
        }

        public sealed class sc_InsertUpdateMemberAvatar : BaseCCHData
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

            public sc_InsertUpdateMemberAvatar()
                : base("sc_InsertUpdateMemberAvatar")
            {
                this.Parameters.New("CCHID", SqlDbType.Int);
                this.Parameters.New("AvatarID", SqlDbType.Int);
            }
        }
    }
}