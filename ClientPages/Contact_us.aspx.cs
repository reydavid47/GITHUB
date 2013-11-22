using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Security.Application;

namespace ClearCostWeb.ClientPages
{
    public partial class Contact_us : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (GetEmployerContent gec = new GetEmployerContent())
                {
                    if (null != Request.QueryString["e"])
                        gec.EmployerID = Convert.ToInt32(Request.QueryString["e"]);
                    gec.GetFrontEndData();
                    if (!gec.HasErrors)
                    {
                        if (gec.Tables[0].Rows.Count > 0)
                        {
                            txtNameOfOrganization.Text = hfToAddress.Value = gec.Tables[0].Rows[0]["EmployerName"].ToString();
                            ltlPhone.Text = Regex.Replace(gec.PhoneNumber, @"(\d{3})(\d{3})(\d{4})", "$1-$2-$3");
                            hfToAddress.Value += "@clearcosthealth.com";
                        }
                        else
                        {
                            hfToAddress.Value = "info";
                            ltlPhone.Text = "650-473-3950";
                        }
                    }
                }
            }
        }
        protected void lbtnSubmit_Click(object sender, EventArgs e)
        {
            string ToAddress = hfToAddress.Value; //"starbucks@clearcosthealth.com";

            string fromEmail = Encoder.HtmlEncode(txtEmail.Text);
            if (fromEmail == string.Empty) fromEmail = "NoEmailProvided@clearcosthealth.com";

            //(1) Create the MailMessage instance
            MailMessage mm = new MailMessage(fromEmail, ToAddress);

            //(2) Assign the MailMessage's properties
            mm.Subject = "Contact Info";
            mm.Body = "<p><strong>Name:</strong>  " + Encoder.HtmlEncode(txtYourName.Text);
            mm.Body = mm.Body + "</p><p><strong>Organization:</strong>  " + Encoder.HtmlEncode(txtNameOfOrganization.Text);
            mm.Body = mm.Body + "</p><p><strong>Phone:</strong>         " + Encoder.HtmlEncode(txtPhone.Text) + "</p>";
            mm.Body = mm.Body + "</p><p><strong>Comments:</strong>      " + Encoder.HtmlEncode(txtUsersBody.Text) + "</p>";
            mm.IsBodyHtml = true;

            //(3) Create the SmtpClient object
            SmtpClient smtp = new SmtpClient();

            //(4) Send the MailMessage (will use the Web.config settings)
            smtp.Send(mm);

            //(5) Send the user to a Thank You page
            Response.Redirect("thankyou.aspx");

        }
    }
}