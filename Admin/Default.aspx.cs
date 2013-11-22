using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.IO;
using QSEncryption.QSEncryption;

namespace ClearCostWeb.Admin
{
    public partial class Default : System.Web.UI.Page
    {
        #region Private session variables for use only on the admin page
        private Dictionary<String, EmployeeData> Employees
        {
            get
            {
                return (HttpContext.Current.Session["AdminEmployees"] as Dictionary<String, EmployeeData>);
            }
            set
            {
                HttpContext.Current.Session["AdminEmployees"] = value;
            }
        }
        private Dictionary<String, EmployeeData> Orphans
        {
            get
            {
                return (HttpContext.Current.Session["AdminOrphans"] as Dictionary<String, EmployeeData>);
            }
            set
            {
                HttpContext.Current.Session["AdminOrphans"] = value;
            }
        }
        private ILookup<String, RoleData> EmployeesInRoles
        {
            get
            {
                return (HttpContext.Current.Session["EmployeesInRoles"] as ILookup<String, RoleData>);
            }
            set
            {
                HttpContext.Current.Session["EmployeesInRoles"] = value;
            }
        }
        #endregion

        #region private data structures
        private struct EmployeeData
        {
            public object UserId;
            public object UserName;
            public object Locked;
            public object EmployerID;
            public object ConnectionString;
            public object DataBase;
            public object CCHID;
        }
        private struct RoleData
        {
            public object UserId;
            public object RoleName;
            public object UserName;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" href=\"" + ResolveUrl("~/Styles/Notify.css") + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\" type=\"text/css\" />"));
                ThisSession.UserLogginID = Membership.GetUser().ProviderUserKey.ToString();

                using (BaseCCHData employees = new BaseCCHData("GetBNCUserInfoForAdmin"))
                {
                    employees.GetFrontEndData();
                    if (employees.Tables.Count >= 1 && employees.Tables[0].Rows.Count > 0)
                    {
                        this.Employees = (from employee in employees.Tables[0].AsEnumerable()
                                          select new EmployeeData()
                                          {
                                              UserId = employee.Field<object>("userid"),
                                              UserName = employee.Field<object>("username"),
                                              Locked = employee.Field<object>("IsLockedOut"),
                                              EmployerID = employee.Field<object>("employerid"),
                                              ConnectionString = employee.Field<object>("connectionstring"),
                                              DataBase = employee.Field<object>("DataBase"),
                                              CCHID = employee.Field<object>("CCHID")
                                          }).ToDictionary(k => k.UserId.ToString(), v => v);
                    }
                    var t = this.Employees.Select(Em => new { Em.Value.UserName, Em.Value.UserId }).OrderBy(Em => Em.UserName).ToList();
                    ddlUsers.DataBind(t);
                    if (employees.Tables.Count >= 2 && employees.Tables[1].Rows.Count > 0)
                    {
                        this.EmployeesInRoles = (from role in employees.Tables[1].AsEnumerable()
                                                 select new RoleData()
                                                 {
                                                     UserId = role.Field<object>("userid"),
                                                     RoleName = role.Field<object>("rolename"),
                                                     UserName = role.Field<object>("username")
                                                 }).ToLookup(k => k.UserId.ToString(), v => v);
                    }
                    if (employees.Tables.Count >= 3 && employees.Tables[2].Rows.Count > 0)
                    {
                        this.Orphans = (from orphan in employees.Tables[2].AsEnumerable()
                                        select new EmployeeData()
                                        {
                                            UserId = orphan.Field<object>("userid"),
                                            UserName = orphan.Field<object>("username")
                                        }).ToDictionary(k => k.UserId.ToString(), v => v);
                    }
                    t = this.Orphans.Select(Or => new { Or.Value.UserName, Or.Value.UserId }).ToList();
                    ddlOrphans.DataBind(t);
                }
                
            }
        }
        protected void Page_Error(object sender, EventArgs e)
        {
            ThisSession.AppException = Server.GetLastError().GetBaseException();
            Server.ClearError();

            Response.Redirect(ResolveUrl("~/Err.aspx"));
        }
        protected void btnSetPassword_Click(object sender, EventArgs e)
        {
            lblMessage.Text = String.Empty;
            try
            {
                lblMessage.Text = ddlUsers.SelectedItem.Text + ": ";

                if (Membership.GetUser(ddlUsers.SelectedItem.Text).IsLockedOut)
                {
                    Membership.GetUser(ddlUsers.SelectedItem.Text).UnlockUser();
                    lblMessage.Text += "Unlocked, ";
                }

                string s = Membership.GetUser(ddlUsers.SelectedItem.Text).ResetPassword();
                Membership.GetUser(ddlUsers.SelectedItem.Text).ChangePassword(s, txtNewPassword.Text.Trim());
                lblMessage.Text += "Password Changed";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error changing password: " + ex.Message;
            }

            lblMessage.Visible = true;
        }
        protected void UpdateDDL(object sender, EventArgs e)
        {
            ddlUsers.Items.Insert(0, "Select User");
        }
        protected void ConfirmDelete(object sender, EventArgs e)
        {
            bool isOrphan = (ddlOrphans.SelectedItem.Text != "Select User");
            lblMessage.Text = String.Empty;
            try
            {
                String uID = (isOrphan ? ddlOrphans.SelectedItem.Value : ddlUsers.SelectedItem.Value);
                lblMessage.Text = (isOrphan ? ddlOrphans.SelectedItem.Text : ddlUsers.SelectedItem.Text) + ":<br />";
                //Remove from Asp.Net membership provider
                if (Membership.DeleteUser(
                    (isOrphan ? ddlOrphans.SelectedItem.Text : ddlUsers.SelectedItem.Text)
                    , cbAllRelated.Checked))
                    lblMessage.Text += "Removed from Membership Successfully<br />";
                else
                    lblMessage.Text += "Error removing from Membership<br />";

                if(!isOrphan)
                {
                    //Remove from UserProfiles table
                    using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString))
                    {
                        String Query = "DELETE FROM userprofile WHERE userid = '" + uID + "'";
                        using (SqlCommand comm = new SqlCommand(Query, conn))
                        {
                            conn.Open();
                            int Recs = comm.ExecuteNonQuery();
                            lblMessage.Text += Recs + " row(s) removed from profile table (should be 1)";
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            { lblMessage.Text += "Failure: " + ex.Message; }
            if (isOrphan)
                ddlOrphans.DataBind();
            else
                ddlUsers.DataBind();
        }
        protected void GetUserInfo(object sender, EventArgs e)
        {
            EmployeeData selectedEmployee = this.Employees[ddlUsers.SelectedItem.Value];

            btnUnlock.Enabled = (Boolean)selectedEmployee.Locked;

            object[] userRoles = this.EmployeesInRoles[ddlUsers.SelectedItem.Value].Select(R => R.RoleName).ToArray();
            String[] roles = Roles.GetAllRoles();

            StringWriter sw = new StringWriter();
            using (HtmlTextWriter htmlOut = new HtmlTextWriter(sw))
            {
                if (roles.Length > 0)
                {
                    htmlOut.RenderBeginTag(HtmlTextWriterTag.Div);
                    htmlOut.Write("This user currently belongs to the following Roles.");
                    htmlOut.WriteBreak();
                    htmlOut.Write("After updating click \"Update Roles\".");
                    htmlOut.RenderEndTag();
                }
                else
                {
                    htmlOut.RenderBeginTag(HtmlTextWriterTag.Div);
                    htmlOut.Write("User is not currently set to any roles.");
                    htmlOut.WriteBreak();
                    htmlOut.Write("Please choose from the following to add them to and click save");
                    htmlOut.RenderEndTag();
                }
            }

            cblRoles.Items.Clear();
            foreach (String s in roles)
            {
                cblRoles.Items.Add(s);
                cblRoles.Items[cblRoles.Items.Count - 1].Selected = userRoles.Contains(s);
            }
            ltlUserRoles.Text = sw.ToString();

            QueryStringEncryption qse = new QueryStringEncryption();
            qse.UserKey = new Guid(ThisSession.UserLogginID);
            qse["EmployerID"] = selectedEmployee.EmployerID.ToString();
            ThisSession.CnxString = selectedEmployee.ConnectionString.ToString();
            qse["CCHID"] = selectedEmployee.CCHID.ToString();

            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "ChangeIframe", "document.getElementById('iCC').src ='" + ResolveUrl("~/CallCenter/Default.aspx?srch=" + qse.ToString()) + "';", true);
        }
        protected void UpdateOrphans(object sender, EventArgs e)
        {
            ddlOrphans.Items.Insert(0, "Select User");
        }
        protected void UpdateUserRoles(object sender, EventArgs e)
        {
            foreach (ListItem li in cblRoles.Items)
            {
                if (li.Selected && !Roles.IsUserInRole(ddlUsers.SelectedItem.Text, li.Text))
                    Roles.AddUserToRole(ddlUsers.SelectedItem.Text, li.Text);
                if (!li.Selected && Roles.IsUserInRole(ddlUsers.SelectedItem.Text, li.Text))
                    Roles.RemoveUserFromRole(ddlUsers.SelectedItem.Text, li.Text);
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertRoles", "alert('Roles Updated Successfully');", true);
        }
        protected void UnlockUser(object sender, EventArgs e)
        {
            Membership.GetUser(ddlUsers.SelectedItem.Text).UnlockUser();
            ScriptManager.RegisterStartupScript(btnUnlock, btnUnlock.GetType(), "Unlock Complete", "alert('" + ddlUsers.SelectedItem.Text + " has been unlocked');", true);
        }
    }
}