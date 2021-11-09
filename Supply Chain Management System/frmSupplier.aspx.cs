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
using OldColor;
using System.Globalization;
using autouniv;

using OldColor;


public partial class frmSupplier : System.Web.UI.Page
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
                Response.Redirect("Home.aspx?sid=sam");
            }
        }
        catch
        {
            Response.Redirect("Default.aspx?sid=sam");
        }  
        if (!IsPostBack)
        {
            DataTable dt = SupplierManager.GetSuppliers();
            dgSup.DataSource = dt;
            Session["Sup"] = dt;
            dgSup.DataBind();

            string query2 = "select '' [ID],'' [Name]  union select [ID] ,[Name] from [SupplierGroup] where Active='True' order by 1";
            util.PopulationDropDownList(ddlSupplierGroup, "SupplierGroup", query2, "Name", "ID");

            string query3 = "select '' [COUNTRY_CODE],'' [COUNTRY_DESC]  union select [COUNTRY_CODE] ,[COUNTRY_DESC] from [COUNTRY_INFO] order by 1";
            util.PopulationDropDownList(ddlCountry, "COUNTRY_INFO", query3, "COUNTRY_DESC", "COUNTRY_CODE");
            ddlCountry.SelectedIndex = -1;

            dgSup.Visible = true;
            txtCompanyName.Focus();
        }
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        clearField();
    }
    
    protected void btnSave_Click(object sender, EventArgs e)
    {
        
        if (txtSupplierName.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Supplier Name..!!');", true);
            return;
        }
        if(string.IsNullOrEmpty(ddlCountry.SelectedItem.Text))
        {

            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select Country Name..!!');", true);
            return;
        }
        Supplier sup = SupplierManager.GetSupplier(lbLID.Text);
        if (sup != null)
        {
            if (per.AllowEdit == "Y")
            {
                int CheckCode = IdManager.GetShowSingleValueInt("COUNT(*)", "SEG_COA_CODE", "GL_SEG_COA", txtGlCoa.Text);
                if (CheckCode <= 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Rong Segment code check this code.\\n Try again ...!!');", true);
                    return;
                }
                sup.ID = lbLID.Text;
                sup.SupCode = txtSupCode.Text;
                sup.ComName = txtCompanyName.Text;
                sup.SupAddr1 = txtAddress1.Text;
                sup.SupName = txtSupplierName.Text;
                sup.SupAddr2 = txtAddress2.Text;
                sup.Designation = txtDesignation.Text;
                sup.City = txtCity.Text;
                sup.SupMobile = txtMobile.Text;
                sup.State = txtState.Text;
                sup.SupPhone = txtPhone.Text;
                sup.PostCode = txtPostalCode.Text;
                sup.Fax = txtFax.Text;
                sup.Country = ddlCountry.SelectedValue;
                sup.Email = txtEmail.Text;
                sup.SupGroup = ddlSupplierGroup.SelectedValue;
                if (CheckBox1.Checked)
                { sup.Active = "True"; }
                else { sup.Active = "False"; }
                sup.LoginBy = Session["userID"].ToString();
                SupplierManager.UpdateSupplier(sup);
                clearField();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are update suceessfullly..!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
            }
        }
        else
        {
            if (per.AllowAdd == "Y")
            {
                sup = new Supplier();
                sup.SupCode = txtSupCode.Text;
                sup.ComName = txtCompanyName.Text; 
                sup.SupAddr1 = txtAddress1.Text;
                sup.SupName = txtSupplierName.Text;
                sup.SupAddr2 = txtAddress2.Text;
                sup.Designation = txtDesignation.Text;
                sup.City = txtCity.Text;
                sup.SupMobile = txtMobile.Text;
                sup.State = txtState.Text;
                sup.SupPhone = txtPhone.Text;
                sup.PostCode = txtPostalCode.Text;
                sup.Fax = txtFax.Text;
                sup.Country = ddlCountry.SelectedValue;
                sup.Email = txtEmail.Text;
                sup.SupGroup = ddlSupplierGroup.SelectedValue;
                if (CheckBox1.Checked)
                { sup.Active = "True"; }
                else { sup.Active = "False"; }
                sup.SupCode = IdManager.GetNextID("supplier", "Code").ToString().PadLeft(7, '0');
                sup.LoginBy = Session["userID"].ToString();
                SupplierManager.CreateSupplier(sup);
                clearField();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are saved suceessfullly...!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
            }
            btnSave.Enabled = true;
        }
    }    
    public void clearField()
    {
        txtSupCode.Text = "";
        txtCompanyName.Text = "";
        txtAddress1.Text = "";
        txtSupplierName.Text = "";
        txtAddress2.Text = "";
        txtDesignation.Text = "";
        txtCity.Text = "";
        txtMobile.Text = "";
        txtState.Text = "";
        txtPhone.Text = "";
        txtPostalCode.Text = "";
        txtFax.Text = "";
       // ddlCountry.SelectedValue = "131";
        txtEmail.Text = "";
        ddlSupplierGroup.SelectedIndex = -1;
        dgSup.DataSource = SupplierManager.GetSuppliers();
        dgSup.DataBind();
        dgSup.Visible = true;
        txtCompanyName.Focus();
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {

            Supplier sup = SupplierManager.GetSupplier(lbLID.Text);
            sup.LoginBy = Session["userID"].ToString();
            if (sup != null)
            {
                SupplierManager.DeleteSupplier(sup);
                clearField();
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }
    protected void dgSup_SelectedIndexChanged(object sender, EventArgs e)
    {
        Supplier sup = SupplierManager.GetSupplier(dgSup.SelectedRow.Cells[5].Text.Trim());
        if (sup != null)
        {
            lbLID.Text=sup.ID;
            txtSupCode.Text=sup.SupCode;
            txtCompanyName.Text=sup.ComName;
            txtAddress1.Text=sup.SupAddr1;
            txtSupplierName.Text=sup.SupName;
            txtAddress2.Text=sup.SupAddr2;
            txtDesignation.Text=sup.Designation;
            txtCity.Text=sup.City;
            txtMobile.Text=sup.SupMobile;
            txtState.Text=sup.State;
            txtPhone.Text=sup.SupPhone;
            txtPostalCode.Text=sup.PostCode;
            txtFax.Text=sup.Fax;
            ddlCountry.SelectedValue=sup.Country;
            txtEmail.Text=sup.Email;
            ddlSupplierGroup.SelectedValue = sup.SupGroup;
            if ( sup.Active == "True")
            { CheckBox1.Checked = true; }
            else { CheckBox1.Checked = true; }
            dgSup.Visible = false;
        }
    }
    protected void dgSup_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgSup.DataSource = Session["Sup"];
        dgSup.PageIndex = e.NewPageIndex;
        dgSup.DataBind();
    }
    protected void dgSup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[5].Attributes.Add("style", "display:none");
        }
    }
}
