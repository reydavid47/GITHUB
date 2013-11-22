using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.SavingsChoice
{
    public partial class Details : System.Web.UI.MasterPage
    {
        public event EventHandler submitEvent;

        protected void Page_Load(object sender, EventArgs e)
        {
            ((ClearCostWeb.SearchInfo.Results)this.Master).SubmitEvent += new EventHandler(Details_SubmitEvent);
        }

        void Details_SubmitEvent(object sender, EventArgs e)
        {
            if (submitEvent != null)
            {
                submitEvent(this, EventArgs.Empty);
            }
        }

        protected string getOverAllScore()
        {
            return "<div class='overallDivider'>&nbsp;</div><div class='overallText'>Your Savings Choice Score: </div><div class='overallScore'>62%</div>";
        }
    }
}