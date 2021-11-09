using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using autouniv;


using OldColor;



public partial class frmClient : System.Web.UI.Page
{
    private static Permis per;
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
                            //Session["dept"] = dReader["dept"].ToString();
                            wnot = dReader["description"].ToString();
                            Session["userID"] = dReader["ID"].ToString();
                        }
                        Session["wnote"] = wnot;

                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type,ShortName from gl_set_of_books where book_name='" + Session["book"].ToString() + "' ";
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
                                Session["ShotName"] = dReader["ShortName"].ToString();
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
            dgClient.DataSource = clsClientInfoManager.GetClientInfosGrid();
            dgClient.DataBind();

            string query3 = "select '' [COUNTRY_CODE],'' [COUNTRY_DESC]  union select [COUNTRY_CODE] ,[COUNTRY_DESC] from [COUNTRY_INFO] order by 1";
            util.PopulationDropDownList(ddlCountry, "COUNTRY_INFO", query3, "COUNTRY_DESC", "COUNTRY_CODE");
            ddlCountry.SelectedValue = "131";
            dgClient.Visible = true;
            txtClientName.Focus();
        }
    }
    protected void dgClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        clsClientInfo ci = clsClientInfoManager.GetClientInfo(dgClient.SelectedRow.Cells[5].Text.Trim());
        if (ci != null)
        {
            lbLId.Text = dgClient.SelectedRow.Cells[5].Text.Trim();
            lblGlCoa.Text = ci.GlCoa;
            txtGlCoa.Text = ci.GlCoa;
            txtClientId.Text = ci.Code;
            txtClientName.Text = ci.CustomerName;
            txtNationalId.Text = ci.NationalId;
            txtAddress1.Text = ci.Address1;
            txtAddress2.Text = ci.Address2;
            txtPhone.Text = ci.Phone;
            txtMobile.Text = ci.Mobile;
            txtFax.Text = ci.Fax;
            txtEmail.Text = ci.Email;
            txtPostalCode.Text = ci.PostalCode;
            ddlCountry.SelectedValue = ci.Country;
            if (ci.Active == "True") { CheckBox1.Checked = true; } else { CheckBox1.Checked = false; }
            if (ci.CommonCus == "1") { CheckBox2.Checked = true; } else { CheckBox2.Checked = false; }
            dgClient.Visible = false;
        }      
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        clearFields();
    }

    private void clearFields()
    {
        txtGlCoa.Text = "";
        txtClientId.Text = "";
        txtClientName.Text = "";
        txtNationalId.Text = "";
        txtAddress1.Text = "";
        txtAddress2.Text = "";
        txtPhone.Text = "";
        txtMobile.Text = "";
        txtFax.Text = "";
        txtEmail.Text = "";
        txtPostalCode.Text = "";
        ddlCountry.SelectedValue = "131";
        CheckBox2.Checked = false;
        dgClient.DataSource = clsClientInfoManager.GetClientInfosGrid();
        dgClient.DataBind();
        dgClient.Visible = true;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {

        string IdGlCoa = "";
        string Desc = "";
        string User = "";
        string glCode = "";



        if (ddlCountry.SelectedValue == "1")
        {
            Desc = "BD-Accounts Receivable from-Customer-" + txtClientName.Text;
            User = "Ban";
            glCode = "1044000";
        }
        else if (ddlCountry.SelectedValue == "2")
        {
            Desc = "PH_Accounts Receivable from-Customer-" + txtClientName.Text;
            User = "Man";
            glCode = "1046000";
        }
        else
        {
            Desc = "Accounts Receivable from-Customer-" + txtClientName.Text;
            User = "All";
        }
        if (string.IsNullOrEmpty(ddlCountry.SelectedItem.Text))
        {

            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select Country Name..!!');", true);
            return;
        }
        clsClientInfo ci = clsClientInfoManager.GetClientInfo(lbLId.Text);
        if (ci != null)
        {
            if (per.AllowEdit == "Y")
            {
                int CheckCode = IdManager.GetShowSingleValueInt("COUNT(*)", "SEG_COA_CODE", "GL_SEG_COA", txtGlCoa.Text);
                if (CheckCode <= 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Rong Segment code check this code.\\n Try again ...!!');", true);
                    return;
                }
                ci.ID = lbLId.Text;
                ci.Code = txtClientId.Text;
                ci.CustomerName = txtClientName.Text;
                ci.NationalId = txtNationalId.Text;
                ci.Address1 = txtAddress1.Text;
                ci.Address2 = txtAddress2.Text;
                ci.Phone = txtPhone.Text;
                ci.Mobile = txtMobile.Text;
                ci.Fax = txtFax.Text;
                ci.Email = txtEmail.Text;
                ci.PostalCode = txtPostalCode.Text;
                ci.Country = ddlCountry.SelectedValue;
                ci.GlCoa = txtGlCoa.Text;
                ci.LoginBy = Session["user"].ToString();
                string NM = Session["ShotName"].ToString();
                ci.GlCoaDesc = NM + "," + Desc;
                if (CheckBox1.Checked == true)
                { ci.Active = "True"; }
                else { ci.Active = "False"; }
                if (CheckBox2.Checked == true)
                { ci.CommonCus = "1"; }
                else { ci.CommonCus = "0"; }
                clsClientInfoManager.UpdateClientInfo(ci);
                clearFields();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are update suceessfullly..!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
            }
        }
        else
        {
            if (txtClientName.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter customer name!!');", true);
            }
            else
            {
                if (per.AllowAdd == "Y")
                {

                    ci = new clsClientInfo();
                    ci.CustomerName = txtClientName.Text;
                    ci.NationalId = txtNationalId.Text;
                    ci.Address1 = txtAddress1.Text;
                    ci.Address2 = txtAddress2.Text;
                    ci.Phone = txtPhone.Text;
                    ci.Mobile = txtMobile.Text;
                    ci.Fax = txtFax.Text;
                    ci.Email = txtEmail.Text;
                    ci.PostalCode = txtPostalCode.Text;
                    ci.Country = ddlCountry.SelectedValue;
                    ci.Code = IdManager.GetNextID("Customer", "Code").ToString().PadLeft(7, '0');
                    txtClientId.Text = ci.Code;
                    if (CheckBox1.Checked == true)
                    { ci.Active = "True"; }
                    else { ci.Active = "False"; }
                    if (CheckBox2.Checked == true)
                    { ci.CommonCus = "1"; }
                    else { ci.CommonCus = "0"; }
                    ci.LoginBy = Session["user"].ToString();

                    //*********** Auto Voucher generate off **********//
                    //IdGlCoa = IdManager.getAutoIdWithParameter("1044", "GL_SEG_COA", "SEG_COA_CODE", glCode, "000", "3");
                    //ci.GlCoa = txtGlCoa.Text;                 
                    clsClientInfoManager.CreateClientInfo(ci);

                    //*********** Auto Voucher generate off **********//
                    //SegCoa sg = new SegCoa();
                    //sg.GlSegCode = IdGlCoa;
                    //sg.SegCoaDesc = "Accounts Receivable from-Customer-" + txtClientName.Text;
                    //sg.LvlCode = "02";
                    //sg.ParentCode = glCode;
                    //sg.BudAllowed = "Y";
                    //sg.PostAllowed = "N";
                    //sg.AccType = "A";
                    //sg.OpenDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    //sg.RootLeaf = "L";
                    //sg.Status = "A";
                    //sg.Taxable = "N";
                    //sg.BookName = "AMB";
                    //sg.EntryUser = Session["user"].ToString();
                    //sg.EntryDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    //sg.AuthoDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    //sg.AuthoUser = "ACC";
                    //SegCoaManager.CreateSegCoa(sg);
                    ////string dept = SegCoaManager.GetSegCoaDesc(Session["dept"].ToString());
                    //GlCoa gl = new GlCoa();
                    //gl.GlCoaCode = "1-" + IdGlCoa;
                    //gl.CoaEnabled = "Y";
                    //gl.BudAllowed = "N";
                    //gl.PostAllowed = "Y";
                    //gl.Taxable = "N";
                    //gl.AccType = "A";
                    //gl.Status = "A";
                    //gl.BookName = "AMB";
                    //string NM = Session["ShotName"].ToString();
                    //gl.CoaDesc = NM + "," + Desc;
                    //gl.CoaCurrBal = "0.00";
                    //gl.CoaNaturalCode = IdGlCoa;
                    //GlCoaManager.CreateGlCoa(gl);
                    clearFields();
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are saved suceessfullly..!!');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                }
            }
        }
        btnSave.Enabled = true;
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            clsClientInfo ci = clsClientInfoManager.GetClientInfo(lbLId.Text);
            if (ci != null)
            {
                clsClientInfoManager.DeleteClientInfo(ci);
                clearFields();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are delete suceessfullly..!!');", true);
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }
    protected void dgClient_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgClient.DataSource = clsClientInfoManager.GetClientInfosGrid();
        dgClient.PageIndex = e.NewPageIndex;
        dgClient.DataBind();
    }
    protected void dgClient_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[5].Attributes.Add("style", "display:none");
        }
    }
}
