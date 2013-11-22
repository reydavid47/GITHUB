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
    public partial class Controls_SavingsChoiceIQAddProvider : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            getData();
        }

        protected DataTable AvatarList;

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

        private void getData()
        {
            using (sc_GetMemberAvatars gma = new sc_GetMemberAvatars())
            {
                gma.CCHID = this.PrimaryCCHID;
                gma.GetData();

                if (gma.Tables.Count >= 1 && gma.Tables[0].Rows.Count > 0)
                {
                    AvatarList = gma.Tables[0];
                    avatarListRepeater.DataSource = AvatarList;
                    avatarListRepeater.DataBind();
                }
            }
        }
    }
}