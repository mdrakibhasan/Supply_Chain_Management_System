using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using autouniv;
using System.Data.SqlClient;


public partial class UserPermis : System.Web.UI.Page
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
                    cmd.CommandText = "Select user_grp,description from utl_userinfo where upper(user_name)=upper('" + Session["user"].ToString().ToUpper() + "') and status='A'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                    string wnot = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            wnot =  dReader["description"].ToString();
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
            if (per != null & per.AllowView == "Y")
            {
                ((Label)Page.Master.FindControl("lblLogin")).Text = Session["wnote"].ToString();
                ((LinkButton)Page.Master.FindControl("lbLogout")).Visible = true;
            }
            else
            {
                Response.Redirect("Home.aspx?sid=sam");
            }
        }
        catch
        {
            Response.Redirect("Default.aspx?sid=sam");
        }         
        if (!Page.IsPostBack)
        {            
            dgUserMst.DataSource = UsersManager.GetUsers();
            dgUserMst.DataBind();
            Search.Visible = true;
        }
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtUserName.Text = "";
        txtDesc.Text = "";
        dgPermis.Visible = false;
        dgUserMst.Visible = true;
        lblTranStatus.Visible = false;
        dgUserMst.DataSource = UsersManager.GetUsers();
        dgUserMst.DataBind();
        Session["perm"] = null;
        txtSearch.Text = "";
        Search.Visible = true;
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (txtUserName.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Nothing to save!!');", true);
        }
        else
        {
            Permis per;
            DataTable dtPermis = (DataTable)Session["perm"];
            foreach (DataRow dr in dtPermis.Rows)
            {
                per = PermisManager.getPermis(txtUserName.Text.ToString().Trim(), dr["mod_id"].ToString());
                if (per == null)
                {
                    per = new Permis();
                    per.UserName = txtUserName.Text.ToString().Trim();
                    per.ModId = dr["mod_id"].ToString();
                    per.AllowAdd = dr["allow_add"].ToString();
                    per.AllowEdit = dr["allow_Edit"].ToString();
                    per.AllowView = dr["allow_View"].ToString();
                    per.AllowDelete = dr["allow_delete"].ToString();
                    per.AllowPrint = dr["allow_print"].ToString();
                    per.AllowAutho = dr["allow_autho"].ToString();
                    PermisManager.CreatePermis(per);
                }
                else
                {
                    if (per.AllowAdd != dr["allow_add"].ToString()| per.AllowEdit != dr["allow_Edit"].ToString() |
                    per.AllowView != dr["allow_View"].ToString() | per.AllowDelete != dr["allow_delete"].ToString() |
                    per.AllowPrint != dr["allow_print"].ToString() | per.AllowAutho != dr["allow_autho"].ToString())
                    {
                        per.AllowAdd = dr["allow_add"].ToString();
                        per.AllowEdit = dr["allow_Edit"].ToString();
                        per.AllowView = dr["allow_View"].ToString();
                        per.AllowDelete = dr["allow_delete"].ToString();
                        per.AllowPrint = dr["allow_print"].ToString();
                        per.AllowAutho = dr["allow_autho"].ToString();
                        PermisManager.UpdatePermis(per);
                    }
                }
            }
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Saved successfully!!');", true);
        }
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {

    }
    protected void btnFind_Click(object sender, EventArgs e)
    {

    }
    protected void dgUserMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["userGroupID"] = null;
        dgUserMst.Visible = false;
        dgPermis.Visible = true;
        Search.Visible = false;
        txtUserName.Text = dgUserMst.SelectedRow.Cells[1].Text.ToString().Trim();
        txtDesc.Text = dgUserMst.SelectedRow.Cells[2].Text.ToString().Trim();
        DataTable dtPermis = new DataTable();
        dtPermis.Columns.Add("mod_id", typeof(string));
        dtPermis.Columns.Add("mod_name", typeof(string));
        dtPermis.Columns.Add("allow_add", typeof(string));
        dtPermis.Columns.Add("allow_edit", typeof(string));
        dtPermis.Columns.Add("allow_view", typeof(string));
        dtPermis.Columns.Add("allow_delete", typeof(string));
        dtPermis.Columns.Add("allow_print", typeof(string));
        dtPermis.Columns.Add("allow_autho", typeof(string));
        DataTable dtModules = PermisManager.getModules();
        DataTable dtUserPermis = PermisManager.GetPermiss(dgUserMst.SelectedRow.Cells[1].Text.Trim());
        DataTable dtUserGroup = PermisManager.getShowGroupPriviledge(" where t1.USER_GRP='" + dgUserMst.SelectedRow.Cells[4].Text.Trim() + "' ");
        if (dtUserPermis.Rows.Count > 0)
        {
           string add = "", edit = "", view = "", delete = "", print = "", autho = "";
           foreach (DataRow drModel in dtModules.Rows)
           {
               GetPermission(dtPermis, dtUserPermis, add, edit, view, delete, print, autho, drModel);        
           }
        }
        else
        {
            //dtUserGroup = PermisManager.getShowGroupPriviledge(" where t1.USER_GRP='" + dgUserMst.SelectedRow.Cells[4].Text.Trim() + "' ");
            if (dtUserGroup.Rows.Count > 0)
            {
                string add = "", edit = "", view = "", delete = "", print = "", autho = "";
                dtUserGroup.Columns["11ID"].ColumnName = "MOD_ID";
                dtUserGroup.Columns["ModelName"].ColumnName = "mod_name";
                dtUserGroup.Columns["Add"].ColumnName = "allow_add";
                dtUserGroup.Columns["Edit"].ColumnName = "allow_edit";
                dtUserGroup.Columns["View"].ColumnName = "allow_view";
                dtUserGroup.Columns["Delete"].ColumnName = "allow_delete";
                dtUserGroup.Columns["Print"].ColumnName = "allow_print";
                dtUserGroup.Columns["Authoriz"].ColumnName = "allow_autho";                
                foreach (DataRow drModel in dtModules.Rows)
                {

                    GetPermission(dtPermis, dtUserGroup, add, edit, view, delete, print, autho, drModel);
                }
            }
        }
        if (dtUserPermis.Rows.Count.Equals(0) && dtUserGroup.Rows.Count.Equals(0))
        {
            string add = "", edit = "", view = "", delete = "", print = "", autho = "";
            foreach (DataRow drModel in dtModules.Rows)
            {
                GetPermission(dtPermis, dtUserPermis, add, edit, view, delete, print, autho, drModel);
            }
        }
        //DataTable dtModule = PermisManager.getModulesUser(dgUserMst.SelectedRow.Cells[1].Text.Trim());
        //DataTable dt = IdManager.GetShowDataTable("Select * from UTL_GROUPGRANT");
        //if (dtPermis.Rows.Count == 0 | dtModules.Rows.Count != dtPermis.Rows.Count)
        //{            
        //    DataRow drPermis;
        //    string add = "", edit = "", view = "", delete = "", print = "", autho = ""; 
        //    foreach (DataRow dr in dtModule.Rows)
        //    {
        //        add = "N"; edit = "N"; view = "Y"; delete = "N"; print = "Y"; autho = "N";
        //        drPermis = dtPermis.NewRow();
        //        drPermis["user_name"] = dgUserMst.SelectedRow.Cells[1].Text.Trim();
        //        drPermis["mod_id"] = dr["mod_id"].ToString();
        //        drPermis["mod_name"] = dr["description"].ToString();
        //        if (dgUserMst.SelectedRow.Cells[3].Text.ToString().Trim() == "Operator")
        //        {
        //            add = "N"; edit = "N"; view = "Y"; delete = "N"; print = "N"; autho = "N";
        //        }
        //        else if (dgUserMst.SelectedRow.Cells[3].Text.ToString().Trim() == "Supervisor")
        //        {
        //            add = "N"; edit = "N"; view = "Y"; delete = "N"; print = "Y"; autho = "Y";
        //        }
        //        else if (dgUserMst.SelectedRow.Cells[3].Text.ToString().Trim() == "Evaluator")
        //        {
        //            add = "N"; edit = "N"; view = "Y"; delete = "N"; print = "Y"; autho = "Y";
        //        }
        //        else if (dgUserMst.SelectedRow.Cells[3].Text.ToString().Trim() == "Administrator")
        //        {
        //            add = "Y"; edit = "Y"; view = "Y"; delete = "Y"; print = "Y"; autho = "Y";
        //        }
        //        else 
        //        {
        //            DataRow[] row = dt.Select("SUB_MOD_ID ='" + dr["MOD_ID"].ToString() + "'");                     
        //            if (row.Length > 0)
        //            {
        //                if (row[0]["ALLOW_ADD"].ToString() == "Y")
        //                    add = "Y";
        //                else
        //                    add = "N";
        //                    if (row[0]["ALLOW_EDIT"].ToString() == "Y")
        //                edit = "Y";
        //                else
        //                        edit = "N";
        //                    if (row[0]["ALLOW_VIEW"].ToString() == "Y")
        //                  view = "Y";
        //                    else
        //                        view = "N";
        //                    if (row[0]["ALLOW_DELETE"].ToString() == "Y")
        //                delete = "Y";
        //                    else
        //                        delete = "N";
        //                    if (row[0]["ALLOW_PRINT"].ToString() == "Y")
        //                print = "Y";
        //                    else
        //                        print = "N";
        //                    if (row[0]["ALLOW_AUTHO"].ToString() == "Y")
        //                autho = "Y";
        //                    else
        //                        autho = "N";
        //            }
        //            else
        //            {
        //                add = "N"; edit = "N"; view = "N"; delete = "N"; print = "N"; autho = "N";
        //            }
        //        }
                
        
        //    }
        //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('No or Some permission was created previously!!');", true);
        //}
        dgPermis.Visible = true;
        dgPermis.DataSource = dtPermis;
        dgPermis.DataBind();
        Session["perm"] = dtPermis;
    }

    private void GetPermission(DataTable dtPermis, DataTable dtUserPermis, string add, string edit, string view, string delete, string print, string autho, DataRow drModel)
    {
        if (ViewState["userGroupID"] == null)
        {
          int  userlevel = IdManager.GetShowSingleValueInt("USER_GRP", "UPPER(USER_NAME)", "UTL_USERINFO",
                txtUserName.Text.ToUpper());
            ViewState["userGroupID"] = userlevel.ToString();
        }
        add = edit = view = delete = print = autho = "N";
        string ModelName = "",ModID="";
        DataRow[] row = dtUserPermis.Select("mod_id ='" + drModel["mod_id"].ToString() + "'");
        ModelName = drModel["DESCRIPTION"].ToString();
        ModID = drModel["MOD_ID"].ToString();
        if (row != null)
        {
            if (row.Length > 0)
            {
                dtPermis.NewRow();
                ModelName = row[0]["mod_name"].ToString();
                if (row[0]["ALLOW_ADD"].ToString() == "Y")
                {
                    add = "Y";
                }
                else
                {
                    add = "N";
                }
                if (row[0]["ALLOW_EDIT"].ToString() == "Y")
                {
                    edit = "Y";
                }
                else
                {
                    edit = "N";
                }
                if (row[0]["ALLOW_VIEW"].ToString() == "Y")
                {
                    view = "Y";
                }
                else
                {
                    view = "N";
                }
                if (row[0]["ALLOW_DELETE"].ToString() == "Y")
                {
                    delete = "Y";
                }
                else
                {
                    delete = "N";
                }
                if (row[0]["ALLOW_PRINT"].ToString() == "Y")
                {
                    print = "Y";
                }
                else
                {
                    print = "N";
                }
                if (row[0]["ALLOW_AUTHO"].ToString() == "Y")
                {
                    autho = "Y";
                }
                else
                {
                    autho = "N";
                }

                dtPermis.Rows.Add(ModID, ModelName, add, edit, view, delete, print, autho);
            }
            else
            {
                //string userlevel = Session["userlevel"].ToString();
                if (ViewState["userGroupID"].ToString().Equals("4"))
                {
                    add = edit = view = delete = print = autho = "Y";
                }
                dtPermis.Rows.Add(ModID, ModelName, add, edit, view, delete, print, autho);
            }
        }
        else
        {
            if (ViewState["userGroupID"].ToString().Equals("4"))
            {
                add = edit = view = delete = print = autho = "Y";
            }
            dtPermis.Rows.Add(ModID, ModelName, add, edit, view, delete, print, autho);
        }
    }
    protected void dgPermis_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {

    }
    protected void dgPermis_CancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        DataTable dtPermis = (DataTable)Session["perm"];
        dgPermis.DataSource = dtPermis;
        dgPermis.EditIndex = -1;
        dgPermis.DataBind();
    }
    protected void dgPermis_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }
    protected void dgBudget_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DataTable dtPermis = (DataTable)Session["perm"];
        string add = ((Label)dgPermis.Rows[e.NewEditIndex].FindControl("lblAllowAdd")).Text;
        string edit = ((Label)dgPermis.Rows[e.NewEditIndex].FindControl("lblAllowEdit")).Text;
        string view = ((Label)dgPermis.Rows[e.NewEditIndex].FindControl("lblAllowView")).Text;
        string delete = ((Label)dgPermis.Rows[e.NewEditIndex].FindControl("lblAllowDelete")).Text;
        string print = ((Label)dgPermis.Rows[e.NewEditIndex].FindControl("lblAllowPrint")).Text;
        string autho = ((Label)dgPermis.Rows[e.NewEditIndex].FindControl("lblAllowAutho")).Text;
        dgPermis.DataSource = dtPermis;
        dgPermis.EditIndex = e.NewEditIndex;
        dgPermis.DataBind();
        ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowAdd")).SelectedIndex = ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowAdd")).Items.IndexOf(((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowAdd")).Items.FindByValue(add));
        ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowEdit")).SelectedIndex = ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowEdit")).Items.IndexOf(((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowEdit")).Items.FindByValue(edit));
        ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowView")).SelectedIndex = ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowView")).Items.IndexOf(((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowView")).Items.FindByValue(view));
        ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowDelete")).SelectedIndex = ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowDelete")).Items.IndexOf(((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowDelete")).Items.FindByValue(delete));
        ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowPrint")).SelectedIndex = ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowPrint")).Items.IndexOf(((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowPrint")).Items.FindByValue(print));
        ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowAutho")).SelectedIndex = ((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowAutho")).Items.IndexOf(((DropDownList)dgPermis.Rows[e.NewEditIndex].FindControl("ddlAllowAutho")).Items.FindByValue(autho));
    }
    protected void dgPermis_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataTable dtPermis = (DataTable)Session["perm"];
        DataRow dr = dtPermis.Rows[dgPermis.Rows[e.RowIndex].DataItemIndex];
        dr["allow_add"] = ((DropDownList)dgPermis.Rows[e.RowIndex].FindControl("ddlAllowAdd")).SelectedValue;
        dr["allow_edit"] = ((DropDownList)dgPermis.Rows[e.RowIndex].FindControl("ddlAllowEdit")).SelectedValue;
        dr["allow_view"] = ((DropDownList)dgPermis.Rows[e.RowIndex].FindControl("ddlAllowView")).SelectedValue;
        dr["allow_delete"] = ((DropDownList)dgPermis.Rows[e.RowIndex].FindControl("ddlAllowDelete")).SelectedValue;
        dr["allow_print"] = ((DropDownList)dgPermis.Rows[e.RowIndex].FindControl("ddlAllowPrint")).SelectedValue;
        dr["allow_autho"] = ((DropDownList)dgPermis.Rows[e.RowIndex].FindControl("ddlAllowAutho")).SelectedValue;
        dgPermis.EditIndex = -1;
        dgPermis.DataSource = dtPermis;
        dgPermis.DataBind();        
    }
    protected void dgPermis_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {

    }
    protected void dgPermis_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }
    protected void dgUserMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[4].Attributes.Add("style", "display:none");
        }
    }
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        dgUserMst.DataSource = UsersManager.GetUsersSetSearchUser(txtSearch.Text);
        dgUserMst.DataBind();
    }
}
