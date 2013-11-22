using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace ClearCostWeb.Controls
{
    public partial class CustomMemberIDField : System.Web.UI.UserControl
    {
        private const Char DigitIndicator = '1';
        private const Char LetterIndicator = 'A';
        private const Char EitherIndicator = '*';
        [Serializable]
        protected struct BindData
        {
            public Int32 Length;
            public String ToolTip;
            public String Style;
            public String Alt;
            public String Text;
        }
        private List<BindData> PageData
        {
            get { return (List<BindData>)ViewState["PageData"]; }
            set { ViewState["PageData"] = value; }
        }

        protected String InsurerName = String.Empty;

        public Boolean IsValid
        {
            get
            {
                Boolean v = true;
                foreach (RepeaterItem i in rptFields.Items)
                {
                    TextBox tb = (TextBox)i.FindControl("MemberID");
                    if (String.IsNullOrWhiteSpace(tb.Text))
                        v = false;
                    if (!v) break;
                }
                return v;
            }
        }
        public String MemberIDText
        {
            get
            {
                String v = "";
                foreach (RepeaterItem i in rptFields.Items)
                {
                    TextBox tb = (TextBox)i.FindControl("MemberID");
                    v += tb.Text.Trim();
                }
                return v;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                String employerFormat = String.Empty;
                using (GetEmployerContent gecs = new GetEmployerContent(Convert.ToInt16(Request.QueryString["e"])))
                {
                    this.InsurerName = gecs.InsurerName;
                    employerFormat = gecs.MemberIDFormat;
                }
                String leftOver = employerFormat;
                //Int32 tbCount = 0;
                PageData = new List<BindData>();
                while (leftOver.Length > 0)
                {
                    BindData newBd = new BindData() { Length = 0 };

                    foreach (char c in leftOver.ToCharArray())
                    {
                        if (c == DigitIndicator || c == LetterIndicator || c == EitherIndicator)
                        {
                            newBd.Text += "X";
                            newBd.Alt += "X";
                            newBd.Length++;
                            newBd.ToolTip = newBd.Length + " character(s)";
                        }
                        else
                            break;
                    }
                    newBd.Style = String.Format("width:{0}px;padding:10px 15px;margin:10px 0px;", newBd.Length * 11);

                    leftOver = leftOver.Remove(0, newBd.Length);
                    if (leftOver.Length > 0)
                    {
                        leftOver = leftOver.Remove(0, 1);
                    }
                    PageData.Add(newBd);
                }
                rptFields.DataBind(PageData);
            }
        }

        protected void rptFields_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            TextBox tb = (TextBox)e.Item.FindControl("MemberID");
            BindData b = PageData[e.Item.ItemIndex];

            tb.MaxLength = b.Length;
            tb.ToolTip = b.ToolTip;
            tb.Attributes["alt"] = b.Alt;
            tb.Attributes["style"] = b.Style;
            tb.Text = b.Text;
        }
    }
}