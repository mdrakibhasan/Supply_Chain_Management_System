using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using autouniv;
using OldColor;
using OldColor;

public partial class UserInfo : System.Web.UI.Page
{
    public static Permis per;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null)
        {
            if (Session.SessionID != "" | Session.SessionID != null)
            {
                clsSession ses = clsSessionManager.getSession(Session.SessionID);
                if (ses != null)
                {
                    Session["user"] = ses.UserId;
                    Session["book"] = "AMB";
                    string connectionString = DataManager.OraConnString();
                    SqlDataReader dReader;
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = connectionString;
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Select ID,user_grp,description from utl_userinfo where upper(user_name)=upper('" + Session["user"].ToString().ToUpper() + "') and status='A'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                    string wnot = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            wnot = dReader["description"].ToString();
                            Session["userID"] = dReader["ID"].ToString();
                        }
                        Session["wnote"] = wnot;

                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type from gl_set_of_books where book_name='" + Session["book"].ToString() + "' ";
                        if (dReader.IsClosed == false)
                        {
                            dReader.Close();
                        }
                        dReader = cmd.ExecuteReader();
                        if (dReader.HasRows == true)
                        {
                            while (dReader.Read())
                            {
                                Session["septype"] = dReader["separator_type"].ToString();
                                Session["org"] = dReader["book_desc"].ToString();
                                Session["add1"] = dReader["company_address1"].ToString();
                                Session["add2"] = dReader["company_address2"].ToString();
                            }
                        }
                    }
                    dReader.Close();
                    conn.Close();
                }
            }
        }
        try
        {
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('" + Session.SessionID + "');", true);
            string pageName = DataManager.GetCurrentPageName();
            string modid = PermisManager.getModuleId(pageName);
            per = PermisManager.getUsrPermis(Session["user"].ToString().Trim().ToUpper(), modid);
            if (per != null && per.AllowView == "Y")
            {
                ((Label)Page.Master.FindControl("lblLogin")).Text = Session["wnote"].ToString();
                ((LinkButton)Page.Master.FindControl("lbLogout")).Visible = true;
            }
            else
            {
                Response.Redirect("~/Home.aspx");                
            }
        }
        catch (Exception ex)
        {
            Response.Redirect("Default.aspx?sid=sam");
        }  
        if (!Page.IsPostBack)
        {
            clearFields();
        }
    }
    private void clearFields()
    {
        txtUserId.Text = txtPassword.Text = txtDescription.Text = "";
        ddlUsrGrp.SelectedIndex = -1;
        ddlStatus.SelectedIndex = -1;
        txtEmpNo.Text = "";
        lblEmpID.Text = "";
        txtEmpNo.Text = "";
        txtEmail.Text =txtSearch.Text= "";
        //************** Campus List show For that **************//
        ddlBranch.Items.Clear();
        string queryCampus = "SELECT ID,IsNull(BranchName,'') BranchName FROM BranchInfo where Status='1' and DeleteBy Is Null order by 1 asc";
        util.PopulationDropDownList(ddlBranch, "BranchInfo", queryCampus, "BranchName", "ID");
        ddlBranch.Items.Insert(0,"");

        //************** User Group **************//
        ddlUsrGrp.Items.Clear();
        string query = "SELECT USER_GRP,IsNull(GROUP_DESC,'') GROUP_DESC FROM UTL_GROUPINFO  order by 1 asc";
        util.PopulationDropDownList(ddlUsrGrp, "UTL_GROUPINFO", query, "GROUP_DESC", "USER_GRP");
        ddlUsrGrp.Items.Insert(0,"");
        
        DataTable dt = null;
        dt = UsersManager.GetShowUser("1");
        GridView1.DataSource = dt;
        GridView1.DataBind();
        
        txtUserId.Focus();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        clearFields();

    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        string Name = txtDescription.Text;
        string EmployeeID = lblEmpID.Text;
        if (rbUserType.SelectedValue.Equals("2"))
        {
            int StdID = IdManager.GetShowSingleValueInt("COUNT(*)", "STUDENT_ID", "STUDENT_INFO", txtUserId.Text);
            if (StdID <= 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('this student are not correct.please check this ID..!!!')", true);
                return;
            }
            Name = IdManager.GetShowSingleValueString("ISNULL(t1.F_NAME,'')+' '+ISNULL(t1.M_NAME,'')+' '+ISNULL(t1.L_NAME,'')", "t1.STUDENT_ID", "STUDENT_INFO t1", txtUserId.Text);
            EmployeeID = IdManager.GetShowSingleValueString("t1.ID", "t1.STUDENT_ID", "STUDENT_INFO t1", txtUserId.Text);
        }
        if (string.IsNullOrEmpty(txtUserId.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Enter User Name...!!!')", true);
            txtUserId.Focus();
            return;
        }
        //if (txtPassword.Text == "")
        //{
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Enter Password...!!!')", true);
        //    txtPassword.Focus();
        //    return;
        //}
        if (string.IsNullOrEmpty(txtDescription.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Enter Name..!!!')", true);
            txtDescription.Focus();
            return;
        }
        if (string.IsNullOrEmpty(ddlUsrGrp.SelectedItem.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Enter User Group..!!!')", true);
            ddlUsrGrp.Focus();
            return;
        }
        if (string.IsNullOrEmpty(ddlStatus.SelectedItem.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Enter Status.!!!')", true);
            ddlStatus.Focus();
            return;
        }
        //if (string.IsNullOrEmpty(ddlBranch.SelectedItem.Text))
        //{
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Enter Campus.!!!')", true);
        //    ddlStatus.Focus();
        //    return;
        //}
        if (txtUserId.Text.ToString().Trim() != String.Empty)
        {
            Users usr = UsersManager.getUser(txtUserId.Text.ToString().ToUpper());
            if (usr != null)
            {
                usr.Description = Name;
                usr.Password = txtPassword.Text;
                usr.UserGrp = ddlUsrGrp.SelectedValue;
                usr.Status = ddlStatus.SelectedValue;
                usr.EmpNo = EmployeeID;
                usr.BranchID = ddlBranch.SelectedValue;
                usr.Email = txtEmail.Text;

                if (rbUserType.SelectedValue.Equals("1"))
                {
                    usr.Type = "1";
                }
                else
                {
                    usr.Type = "2";
                }
                UsersManager.UpdateUser(usr);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage",
                    "alert('Record update Successfully.!!')", true);
                clearFields();
            }
            else
            {
                usr = new Users();
                usr.UserName = txtUserId.Text;
                usr.Description = Name;
                usr.Password = txtPassword.Text;
                usr.UserGrp = ddlUsrGrp.SelectedValue;
                usr.Status = ddlStatus.SelectedValue;
                usr.EmpNo = EmployeeID;
                usr.BranchID = ddlBranch.SelectedValue;
                usr.Email = txtEmail.Text;
                if (rbUserType.SelectedValue.Equals("1"))
                {
                    usr.Type = "1";
                }
                else
                {
                    usr.Type = "2";
                }
                UsersManager.CreateUser(usr);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage",
                    "alert('Record Inserted Successfully.!!')", true);
                clearFields();
            }
        }
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (txtUserId.Text.ToString().Trim() != String.Empty)
        {
            Users usr = UsersManager.getUser(txtUserId.Text.ToString().ToUpper());
            if (usr != null)
            {
                UsersManager.DeleteUser(usr);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record delete Successfully')", true);
            }
        }
    }
    protected void btnFind_Click(object sender, EventArgs e)
    {
        if (txtUserId.Text.ToString().Trim() != String.Empty)
        {
            Users usr = UsersManager.getUser(txtUserId.Text.ToString().ToUpper());
            if (usr != null)
            {
                txtUserId.Text = usr.UserName;
                txtDescription.Text = usr.Description;
                txtPassword.Text = usr.Password;
                ddlUsrGrp.SelectedValue = usr.UserGrp;
                ddlStatus.SelectedValue = usr.Status;
                lblEmpID.Text = usr.EmpNo;
                string Name = IdManager.GetShowSingleValueString("dbo.InitCap(isnull([F_NAME],'')+' '+isnull([M_NAME],'')+' '+isnull([L_NAME],''))",
                    "EMP_NO", "PMIS_PERSONNEL", usr.EmpNo);
                txtEmpNo.Text = Name;
                rbUserType.SelectedValue = usr.Type;
                UP1.Update();
                UP2.Update();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Not search...!!')", true);
            }
        }
    }
    protected void txtEmpNo_TextChanged(object sender, EventArgs e)
    {
        string ID = IdManager.GetShowSingleValueString("EMP_NO", "upper(Code+' - '+dbo.InitCap(isnull(F_NAME,'')+' '+isnull(M_NAME,'')+' '+isnull(L_NAME,'')))", "PMIS_PERSONNEL", txtEmpNo.Text.ToUpper());
        if (ID != "")
        {
            string EmpName = IdManager.GetShowSingleValueString("dbo.InitCap(isnull(F_NAME,'')+' '+isnull(M_NAME,'')+' '+isnull(L_NAME,''))", "upper(Code+' - '+dbo.InitCap(isnull(F_NAME,'')+' '+isnull(M_NAME,'')+' '+isnull(L_NAME,'')))", "PMIS_PERSONNEL", txtEmpNo.Text.ToUpper());
            lblEmpID.Text = ID;
            txtEmpNo.Text = EmpName;
            UP1.Update();
            UP2.Update();
        }
    }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        Users usr = UsersManager.getUser(GridView1.SelectedRow.Cells[1].Text.ToString().ToUpper());
        if (usr != null)
        {
            txtUserId.Text = usr.UserName;
            txtDescription.Text = usr.Description;
            txtPassword.Text = usr.Password;
            ddlUsrGrp.SelectedValue = usr.UserGrp;
            ddlStatus.SelectedValue = usr.Status;
            lblEmpID.Text = usr.EmpNo;
            txtEmail.Text = usr.Email;
            string Name = IdManager.GetShowSingleValueString("dbo.InitCap(isnull([F_NAME],'')+' '+isnull([M_NAME],'')+' '+isnull([L_NAME],''))",
                "EMP_NO", "PMIS_PERSONNEL", usr.EmpNo);
            txtEmpNo.Text = Name;
            rbUserType.SelectedValue = usr.Type;
            txtPassword.Text = "";
            UP1.Update();
            UP2.Update();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Not search...!!')", true);
        }
    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Reset")
        {
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            string UserName = gvr.Cells[1].Text.ToString().Trim();
            Users usr = UsersManager.getUser(UserName.ToUpper());
            if (usr != null)
            {
                usr.LoginBy = Session["userID"].ToString();
                usr.Password = "123";
                UsersManager.GetResetPassword(usr);
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Password reset sucessfully. \\n default password (123) ..!!');", true);                
            }
        }
    }
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = UsersManager.GetShowUser(" where t1.[status]='A'  and upper(t1.[USER_NAME]+' - '+t1.[DESCRIPTION]) = upper('" + txtSearch.Text + "') order by [user_name] ");
        if (dt.Rows.Count == 1)
        {
            txtUserId.Text = dt.Rows[0]["USER_NAME"].ToString();
            btnFind_Click(sender,e);
        }
        else
        {
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
    }
    //protected void dgStudent_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    Users usr = UsersManager.getUser(dgStudent.SelectedRow.Cells[1].Text.ToString().ToUpper());
    //    if (usr != null)
    //    {
    //        txtUserId.Text = usr.UserName;
    //        txtDescription.Text = usr.Description;
    //        txtPassword.Text = usr.Password;
    //        ddlUsrGrp.SelectedValue = usr.UserGrp;
    //        ddlStatus.SelectedValue = usr.Status;
    //        lblEmpID.Text = usr.EmpNo;
    //        txtEmail.Text = usr.Email;
    //        //string Name = IdManager.GetShowSingleValueString("dbo.InitCap(isnull([F_NAME],'')+' '+isnull([M_NAME],'')+' '+isnull([L_NAME],''))",
    //        //    "EMP_NO", "PMIS_PERSONNEL", usr.EmpNo);
    //        txtEmpNo.Text = usr.Description;
    //        rbUserType.SelectedValue = usr.Type;
    //        txtPassword.Text = "";
    //        UP1.Update();
    //        UP2.Update();
    //    }
    //    else
    //    {
    //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Not search...!!')", true);
    //    }
    //}
}
