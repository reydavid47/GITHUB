using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ClearCostWeb.Controls
{
    public partial class ServiceTypeSearch : System.Web.UI.UserControl
    {
        #region GUI Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            ((Image)upLoading.FindControl("imgLoader")).Visible = !ddlSpecialties.SelectedValue.ToString().StartsWith("Lab");
            if (!IsPostBack)
            {
                using (GetSpecialtiesForWeb gsfw = new GetSpecialtiesForWeb())
                {
                    if (!gsfw.HasErrors && gsfw.RowsBack > 0)
                    {
                        ddlSpecialties.DataSource = gsfw.SpecialtyResults;
                        ddlSpecialties.DataBind();
                        ddlSpecialties.Items.Insert(0, "Select from list:");
                    }
                }
            }
        }
        protected void ddlSpecialties_SelectedIndexChanged(object sender, EventArgs e)
        {
            String selectedSpecialty = ddlSpecialties.SelectedValue.ToString();
            ClearDropDowns();
            ClearSessionEntryInfo();
            ThisSession.IsShotVaccine = (selectedSpecialty.ToLower().Contains("shots") || selectedSpecialty.ToLower().Contains("vaccine"));
            if (selectedSpecialty.StartsWith("Select")) //"Select from list:"
            {
                lnkBtnSearch2.Visible = false;
            }
            else
            {
                if (selectedSpecialty.StartsWith("Lab")) //"Lab Test"
                {
                    lnkBtnSearch2.Visible = false;

                    lsLabSearch.SetupLetters();

                    pnlLabLetters.Visible = true;
                }
                else
                {
                    using (GetSpecialtySubCategoriesForWeb gsscfw = new GetSpecialtySubCategoriesForWeb(selectedSpecialty))
                    {
                        if (!gsscfw.HasErrors && gsscfw.RowsBack > 0)
                        {
                            //Per Syam's request, we are skipping the specialty_search page and just continuing with the drop-downs
                            //ThisSession.ServiceEntered = selectedSpecialty;
                            //ThisSession.ServiceName = "Office visit - For new patient";
                            //Response.Redirect("specialty_search.aspx");
                            ddlSubCategories.DataSource = gsscfw.SubSpecialties;
                            ddlSubCategories.DataBind();
                            ddlSubCategories.Items.Insert(0, "Select from list:");
                            ddlSubCategories.Visible = true;
                        }
                        lnkBtnSearch2.Visible = (!gsscfw.HasErrors && gsscfw.RowsBack == 0);
                    }
                }
            }
        }
        protected void ddlSubCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            String selectedSpecialty = ddlSpecialties.SelectedValue.ToString();
            String selectedSubSpecialty = ddlSubCategories.SelectedValue.ToString();

            if (selectedSubSpecialty.StartsWith("Select"))//"Select from list:"
            {
                ddlLastCategory.Visible = false;
            }
            else
            {
                using (GetLastCategoriesForWeb glcfw = new GetLastCategoriesForWeb())
                {
                    glcfw.Specialty = selectedSpecialty;
                    glcfw.SubCategory = selectedSubSpecialty;
                    glcfw.GetFrontEndData();
                    if (!glcfw.HasErrors && glcfw.RowsBack > 0)
                    {
                        ddlLastCategory.DataSource = glcfw.Categories;
                        ddlLastCategory.DataBind();
                        ddlLastCategory.Items.Insert(0, "Select from list:");
                        ddlLastCategory.Visible = true;
                    }
                    lnkBtnSearch2.Visible = (!glcfw.HasErrors && glcfw.RowsBack == 0);
                }
            }
        }
        protected void lnkBtnSearch2_Click(object sender, EventArgs e)
        {
            String selectedSpecialty = ddlSpecialties.SelectedValue.ToString();
            String selectedSubSpecialty = ddlSubCategories.SelectedValue.ToString();
            String selectedLastCategory = ddlLastCategory.SelectedValue.ToString();

            using (GetServiceListByCategoryForWeb gslbcfw = new GetServiceListByCategoryForWeb())
            {
                gslbcfw.Specialty = selectedSpecialty;
                gslbcfw.SubCategory = selectedSubSpecialty;
                gslbcfw.CategoryLvl4 = selectedLastCategory;
                gslbcfw.GetData();
                if (!gslbcfw.HasErrors && gslbcfw.RowsBack > 0)
                {
                    ThisSession.ServiceName = gslbcfw.ServiceName;
                    ThisSession.SpecialtyID = gslbcfw.SpecialtyID;
                    ThisSession.ServiceID = gslbcfw.ServiceID;
                }
            }
            ThisSession.ServiceEnteredFrom = "DropDowns";
            //  lam, 20130311, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
            if (selectedSpecialty.ToLower().Contains("office"))
            {
                ThisSession.Specialty = selectedSubSpecialty;  //  lam, 20130311, MSF-272
                //  lam, 20130319, MSF-177, "Office Visit" should stay on "Find a Service" tab but not "Find a Doctor"
                //Response.Redirect("results_specialty.aspx#tabcare");
                Response.Redirect("results_care.aspx");
                //  -------------------------------------------------------------------------------------------------
            }
            else
                Response.Redirect("results_care.aspx");
            //  old code  --------------------------------------------------------------------------------------
            //if (selectedSpecialty.ToLower().Contains("office"))
            //    Response.Redirect("results_specialty.aspx#tabdoc");
            //else
            //    Response.Redirect("results_care.aspx#tabcare");
            //  ------------------------------------------------------------------------------------------------
        }
        protected void ddlLastCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            lnkBtnSearch2.Visible = true;
        }
        protected void lbCareSearchResults2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ThisSession.ServiceEnteredFrom = "DropDowns";
            ThisSession.ServiceName = lbCareSearchResults2.SelectedValue.ToString();
            Response.Redirect("results_care.aspx");
        }
        #endregion

        #region Supporting Methods
        private void ClearSessionEntryInfo()
        {
            ThisSession.ServiceEnteredFrom = string.Empty;
            ThisSession.ServiceEntered = string.Empty;
            ThisSession.SpecialtyID = -1;
            ThisSession.Specialty = string.Empty;
        }
        private void ClearDropDowns()
        {
            ddlSubCategories.Visible = false;
            ddlLastCategory.Visible = false;
            ddlLastCategory.Visible = false;

            //Make Lab Test Search by Letter invisible
            pnlLabLetters.Visible = false;

            lnkBtnSearch2.Visible = false;
        }
        #endregion
    }
}