using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCostWeb.Account
{
    public partial class Review : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //Load account key information.
                lblFirstName.Text = ThisSession.FirstName;
                lblLastName.Text = ThisSession.LastName;
                DateTime dateOfBirth = DateTime.Parse(ThisSession.PatientDateOfBirth);
                string dob = dateOfBirth.ToLongDateString();
                dob = dob.Substring(dob.IndexOf(",") + 1); // remove day of week from string.
                lblDateOfBirth.Text = dob;
                lblAddress.Text = ThisSession.PatientAddress1;
                if (ThisSession.PatientAddress2 != String.Empty)
                { lblAddress.Text += "<br />" + ThisSession.PatientAddress2; }
                lblAddress.Text += "<br />" + ThisSession.PatientCity + ", " + ThisSession.PatientState + " " + ThisSession.PatientZipCode;
                lblEmail.Text = ThisSession.PatientEmail;

                //Display list of adult dependents
                rptAdultDependents.DataSource = ThisSession.Dependents.GetAdultDependents();
                rptAdultDependents.DataBind();

            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void lbtnAccessAccount_Click(object sender, EventArgs e)
        {
            lblTandC.Visible = false; lblTandC.Text = string.Empty;

            //TODO: Validations
            if (cbxTandC.Checked)
            {

                Response.Redirect("../SearchInfo/search.aspx");
            }
            else
            { lblTandC.Text = "<br />You must agree to the terms and conditions above."; lblTandC.Visible = true; }
        }
    }
}