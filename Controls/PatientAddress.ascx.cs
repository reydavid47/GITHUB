using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Microsoft.Security.Application;

namespace ClearCostWeb.Controls
{
    public partial class PatientAddress : System.Web.UI.UserControl
    {
        public delegate void OnSubmit(bool isSubmitted);
        public event OnSubmit loadPage;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            JavaScriptHelper.IncludePatientAddress(Page.ClientScript);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //Page.AddCSSToHeader("PatientAddress.css");
            Page.Header.Controls.Add(new LiteralControl("<link href=\"Styles\\PatientAddress.css\" rel=\"Stylesheet\" type=\"text/css\" />"));
            //Page.AddScriptToHeader("PatientAddress.js");

            if (ThisSession.MedicalPlanType != null)
            {
                String cssText = "<style type=\"text/css\">#";
                cssText += lblInsurerName.ClientID;
                cssText += ":hover:before{content:\"";
                cssText += ThisSession.MedicalPlanType;
                cssText += "\";position:absolute;background-color:rgb(255,255,255);padding:3px 5px;border:1px solid black;margin-top:-30px;";
                cssText += "margin-left:5px;border-radius:6px;box-shadow:1px 1px 3px black;color:#693;}#";
                cssText += lblInsurerName.ClientID;
                cssText += "{cursor: default;}</style>";
                Page.Header.Controls.Add(new LiteralControl(cssText));
            }

            if (!Page.IsPostBack)
            {
                WireClientJava();

                PATIENTLATITUDE.Value = ThisSession.PatientLatitude;
                PATIENTLONGITUDE.Value = ThisSession.PatientLongitude;
                lblPatientAddress.Text = ThisSession.PatientAddressTwoLines;
                lblInsurerName.Text = ThisSession.Insurer;
                lblRxProviderName.Text = ThisSession.RXProvider;

                dsSavedAddresses.ConnectionString = ThisSession.CnxString;

                chgloc.DataBind();
            }
            else
            {
                if (StateRequired.IsValid && PATIENTLATITUDE.Value != ThisSession.PatientLatitude || PATIENTLONGITUDE.Value != ThisSession.PatientLongitude)
                {
                    ThisSession.PatientLatitude = PATIENTLATITUDE.Value;
                    ThisSession.PatientLongitude = PATIENTLONGITUDE.Value;
                    SaveNewLocation(PATIENTLATITUDE.Value, PATIENTLONGITUDE.Value);
                    lblPatientAddress.Text = ThisSession.PatientAddressTwoLines;
                }
                if (!String.IsNullOrWhiteSpace(LOCATIONHASH.Value))
                {
                    Page
                        .ClientScript
                        .RegisterStartupScript(
                            this.GetType(),
                            "changeHash",
                            "<script type='text/javascript' language='javascript'>window.location.hash = '" + LOCATIONHASH.Value + "';</script>");
                }
                if (loadPage != null)
                {
                    loadPage(true);
                }
            }
        }
        private void ClearLocationFields()
        {
            ddlState.DataBind();
            ddlSavedAddresses.DataBind();
            txtChgAddress.Text = "Enter street address";
            txtChgCity.Text = "Enter city";
            txtChgZipCode.Text = "Zip Code";
            cbSaveAddress.Checked = false;
        }
        protected void ddlState_DataBound(object sender, EventArgs e)
        {
            //Add "State" to top of state list
            ddlState.Items.Insert(0, new ListItem("State", ""));
        }
        protected void ddlSavedAddresses_DataBound(object sender, EventArgs e)
        {
            //Add "Select from saved locations" to top of state list
            ddlSavedAddresses.Items.Insert(0, new ListItem(ThisSession.DefaultPatientAddress,"Orig"));
            ddlSavedAddresses.Items.Insert(0, new ListItem("Select from saved locations", ""));
        }

        private void WireClientJava()
        {
            String javaOnBlur, javaOnFocus, javaOnClick;
            javaOnBlur = javaOnFocus = javaOnClick = String.Empty;

            //Add required java to txtChgAddress fields
            javaOnBlur = "this.value=!this.value?'Enter street address':this.value;";
            javaOnFocus = "this.select()";
            javaOnClick = "if(this.value=='Enter street address'){this.value='';};" + rbUseNew.ClientID + ".checked=true;";
            txtChgAddress.Attributes.Add("onblur", javaOnBlur); //Add client side onblur handling
            txtChgAddress.Attributes.Add("onfocus", javaOnFocus); //Add client side onfocus handling
            txtChgAddress.Attributes.Add("onclick", javaOnClick); //Add client side onclick handling

            //Add required java to txtChgCity
            javaOnBlur = "this.value=!this.value?'Enter city':this.value;";
            javaOnFocus = "this.select()";
            javaOnClick = "if(this.value=='Enter city'){this.value='';};" + rbUseNew.ClientID + ".checked=true;";
            txtChgCity.Attributes.Add("onblur", javaOnBlur); //Add client side onblur handling
            txtChgCity.Attributes.Add("onfocus", javaOnFocus); //Add client side onfocus handling
            txtChgCity.Attributes.Add("onclick", javaOnClick); //Add client side onclick handling

            //Add required java to txtChgZipCode
            javaOnBlur = "this.value=!this.value?'Zip Code':this.value;";
            javaOnFocus = "this.select()";
            javaOnClick = "if(this.value=='Zip Code'){this.value='';};" + rbUseNew.ClientID + ".checked=true;";
            txtChgZipCode.Attributes.Add("onblur", javaOnBlur); //Add client side onblur handling
            txtChgZipCode.Attributes.Add("onfocus", javaOnFocus); //Add client side onfocus handling
            txtChgZipCode.Attributes.Add("onclick", javaOnClick); //Add client side onclick handling

            //Add required java to txtChgZipCode
            javaOnBlur = "this.value=!this.value?'Zip Code':this.value;";
            javaOnFocus = "this.select()";
            javaOnClick = "if(this.value=='Zip Code'){this.value='';};" + rbUseNew.ClientID + ".checked=true;";
            txtChgZipCode.Attributes.Add("onblur", javaOnBlur); //Add client side onblur handling
            txtChgZipCode.Attributes.Add("onfocus", javaOnFocus); //Add client side onfocus handling
            txtChgZipCode.Attributes.Add("onclick", javaOnClick); //Add client side onclick handling

            //Add required java to ddlSavedAddresses
            javaOnClick = rbUseSaved.ClientID + ".checked=true;";
            ddlSavedAddresses.Attributes.Add("onclick", javaOnClick); //Add client side onclick handling

            //Add required java to ddlState
            javaOnClick = rbUseNew.ClientID + ".checked=true;";
            ddlState.Attributes.Add("onclick", javaOnClick); //Add client side onclick handling

            //Add required java to cbSaveAddress
            javaOnClick = rbUseNew.ClientID + ".checked=true;";
            cbSaveAddress.Attributes.Add("onclick", javaOnClick); //Add client side onclick handling
        }
        private void SaveNewLocation(String newLatitude, String newLongitude)
        {
            dsSavedAddresses.ConnectionString = ThisSession.CnxString;
            if (rbUseSaved.Checked)
            {
                String RawQuery = "";
                if (ddlSavedAddresses.SelectedValue != "Orig")
                {
                    RawQuery = "Select Address1, Address2, City, State, Zip From SavedAltAddresses Where AddressID = '" + ddlSavedAddresses.SelectedValue + "'";
                }
                else
                {
                    RawQuery = "Select Address1, Address2, City, State, Zipcode as [Zip] from Enrollments Where CCHID = '" + ThisSession.CCHID + "'";
                }
                using (BaseCCHData b = new BaseCCHData(RawQuery, true))
                {
                    b.GetData();
                    if (b.Tables.Count > 0 && b.Tables[0].Rows.Count > 0)
                    {
                        using (DataTable dt = b.Tables[0])
                        {
                            DataRow dr = dt.Rows[0];
                            ThisSession.PatientAddress1 = dr.Field<String>("Address1");
                            ThisSession.PatientAddress2 = dr.Field<String>("Address2");
                            ThisSession.PatientCity = dr.Field<String>("City");
                            ThisSession.PatientState = dr.Field<String>("State");
                            ThisSession.PatientZipCode = dr.Field<String>("Zip");
                        }
                    }
                }
            }
            else if (cbSaveAddress.Checked)
            {
                using (SaveAddress sa = new SaveAddress())
                {
                    sa.CCHID = ThisSession.CCHID;
                    sa.Address1 = Encoder.HtmlEncode(txtChgAddress.Text);
                    sa.Address2 = String.Empty;
                    sa.City = Encoder.HtmlEncode(txtChgCity.Text);
                    sa.State = ddlState.SelectedValue;
                    if (txtChgZipCode.Text.ToLower().StartsWith("zip"))
                        sa.Zip = String.Empty;
                    else
                        sa.Zip = Encoder.HtmlEncode(txtChgZipCode.Text);
                    sa.PostData();
                }
                ThisSession.PatientAddress1 = Encoder.HtmlEncode(txtChgAddress.Text);
                ThisSession.PatientAddress2 = "";
                ThisSession.PatientCity = Encoder.HtmlEncode(txtChgCity.Text);
                ThisSession.PatientState = ddlState.SelectedValue;
                ThisSession.PatientZipCode = Encoder.HtmlEncode(txtChgZipCode.Text);
            }
            else
            {
                ThisSession.PatientAddress1 = Encoder.HtmlEncode(txtChgAddress.Text);
                ThisSession.PatientAddress2 = "";
                ThisSession.PatientCity = Encoder.HtmlEncode(txtChgCity.Text);
                ThisSession.PatientState = ddlState.SelectedValue;
                ThisSession.PatientZipCode = Encoder.HtmlEncode(txtChgZipCode.Text);
            }
            ddlSavedAddresses.Items.Clear();
            dsSavedAddresses.Select(new DataSourceSelectArguments());
            ddlSavedAddresses.DataBind();

            ClearLocationFields();
        }
   }
}