using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.IO;
using Microsoft.Security.Application;

namespace ClearCostWeb.Public
{
    public partial class Contact_us : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void lbtnSubmit_Click(object sender, EventArgs e)
        {
            string ToAddress = "info@clearcosthealth.com";

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