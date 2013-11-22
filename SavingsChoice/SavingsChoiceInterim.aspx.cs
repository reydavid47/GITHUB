using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.SavingsChoice
{
    public partial class SavingsChoiceInterim : System.Web.UI.Page
    {
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

        protected void Page_Load(object sender, EventArgs e)
        {
            auditSCIQCompletion();
            loadEmployerContent();
        }

        private void loadEmployerContent() {
            using (GetEmployerContent gec = new GetEmployerContent()) {
                gec.EmployerID = Convert.ToInt32(ThisSession.EmployerID);
                gec.GetFrontEndData();
                lblStartingText.Text = gec.SCIQStartText;
                ltlPhoneLM.Text = ltlPhoneQ.Text = gec.FormattedPhoneNumber;
                ltlEmployerName.Text = gec.EmployerName.Trim();
            }
        }

        private void auditSCIQCompletion() {
            CreateSCIQAuditTrail SCIQAudit = new CreateSCIQAuditTrail();
            SCIQAudit.CCHID = PrimaryCCHID;
            SCIQAudit.SessionID = ThisSession.UserLogginID;
            SCIQAudit.Action = "completed";
            SCIQAudit.SCIQ_FLG = true;
            SCIQAudit.Category = null;
            SCIQAudit.URL = null;
            SCIQAudit.PostData();        
        }
    }
}