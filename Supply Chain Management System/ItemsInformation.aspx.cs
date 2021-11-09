using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using autouniv;
using OldColor;
using OldColor;
using System.Data;
using System.Configuration;

public partial class ItemsInformation : System.Web.UI.Page
{
    private byte[] ItemsPhoto;
    private static Permis per;
    private  readonly  ClsItemDetailsManager _aItemDetailsManager=new ClsItemDetailsManager();
    private  readonly  ClsItemDetailsManager _aClsItemDetailsManager=new ClsItemDetailsManager();
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
                Response.Redirect("Default.aspx?sid=sam");
            }
        }
        catch
        {
            Response.Redirect("Default.aspx?sid=sam");
        } 
        if (!IsPostBack)
        {
            try
            {
                
                RefreshAll();
                UPOPAmount.Update();
                UPOPStock.Update();
                UPanel1Cat.Update();
                UpdatePanel2.Update();
                UPCode.Update();
                UPItemType.Update();
                
            }
            catch (FormatException fex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Database"))
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
                else
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
            }
           
        }
    }
    private void getColor()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ID", typeof(string));
        dt.Columns.Add("ColorName", typeof(string));
        //DataRow dr = dt.NewRow();
        //dt.Rows.Add(dr);
        ViewState["ColorInfo"] = dt;
    }
    private void getSize()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ID", typeof(string));
        dt.Columns.Add("SizeName", typeof(string));
        ViewState["SizeInfo"] = dt;
    }
    private void RefreshAll()
    {
        rdbItemType.Enabled = true;
        txtCode.Text = "";
        imgEmp.ImageUrl = "~/Signature/T-Shart.jpg";
        imgEmp.DataBind();
        txtDiscountAmount.Text = "0";
        ddlCatagory.DataSource = MajorCategoryManager.GetMajorCats();
        ddlCatagory.DataTextField = "mjr_desc";
        ddlCatagory.DataValueField = "mjr_code";
        ddlCatagory.DataBind();
        lblID.Text = "";
        ddlCatagory.Items.Insert(0, "");
        CheckBox1.Checked = dgHistory.Visible =  true;
        txtName.Text ="";
        txtOpeningStock.Text =
            txtOpeningAmount.Text =
                txtClosingStock.Text =
                    txtClosingAmount.Text =
                        txtDiscountAmount.Text =
                            txtUnitPrice.Text = txtDiscountAmount.Text = txtBranchSalesPrice.Text = "0";        
                ddlCurrency.SelectedIndex = ddlTextCatagory.SelectedIndex = ddlUmo.SelectedIndex = -1;
        txtOpeningStock.Enabled = txtOpeningAmount.Enabled = true;
        DataTable dtHistory = ClsItemDetailsManager.getShowItemsHistoryDetails();
        dgHistory.DataSource = dtHistory;
        dgHistory.Caption = "<h1>Total Items : " + dtHistory.Rows.Count.ToString() + "</h1>";
        dgHistory.DataBind();        
        ddlTextCatagory.DataSource = ClsItemDetailsManager.ShowTextCatagory();
        ddlTextCatagory.DataValueField = "ID";
        ddlTextCatagory.DataTextField = "Name";
        ddlTextCatagory.DataBind();
        ddlTextCatagory.Items.Insert(0,new ListItem(""));

        ddlCategoryName.DataSource = MajorCategoryManager.GetMajorCats();
        ddlCategoryName.DataTextField = "mjr_desc";
        ddlCategoryName.DataValueField = "mjr_code";
        ddlCategoryName.DataBind();
        lblID.Text = "";
      
        
        //txtCode.Text = ClsItemDetailsManager.GetShowItemsDetailsInformation().ToString().PadLeft(6, '0');
        txtDiscountAmount.Enabled=CheckBox2.Checked= false;
        txtDescription.Text = "";        
        ddlUmo.Items.Clear();
        string query2 = "SELECT  [ID] ,[Name] FROM [View_UOM]";
        util.PopulationDropDownList(ddlUmo, "UOM", query2, "Name", "ID");

        string query3 = "select * from View_BrandInfo  order by 2 ";
        util.PopulationDropDownList(ddlBrand, "View_BrandInfo", query3, "BrandName", "ID");

        DataTable dt = IdManager.GetShowDataTable("select * from View_ColorInfo");
        chkColor.DataSource = dt; chkColor.DataTextField = "ColorName"; chkColor.DataValueField = "ID";
        chkColor.DataBind();
        DataTable dt1 = IdManager.GetShowDataTable(" select * from View_SizeName ");
        chkSize.DataSource = dt1; chkSize.DataTextField = "SizeName"; chkSize.DataValueField = "ID";
        chkSize.DataBind();
        txtStName.Text =txtSecurityCode.Text=txtStyleNo.Text=lblID.Text=txtCode.Text= "";
        //txtCode.Text = IdManager.GetDateTimeWiseSerial("", "Code", "Item");

        AutoID();        
        txtName.Focus();
        UPCode.Update();
        UPItemType.Update();
    }
    protected void ddlCatagory_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            ddlSubCatagory.DataSource = SubMajorCategoryManager.GetSubMajorCategories(ddlCatagory.SelectedValue);
            ddlSubCatagory.DataTextField = "Name";
            ddlSubCatagory.DataValueField = "ID";
            ddlSubCatagory.DataBind();
           // ddlSubCatagory.Items.Insert(0, "");
            UpdatePanel2.Update();
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }
    protected void BtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if(rdbItemType.SelectedValue!="1")
            {
                if (string.IsNullOrEmpty(txtName.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input Items Name...!!');", true);
                    return;
                }

                else if (string.IsNullOrEmpty(txtStyleNo.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input Code Number...!!');", true);
                    return;
                }

               
            }
                     


           

            if (string.IsNullOrEmpty(txtName.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input Items Name...!!');", true);
                return;
            }
            //if (string.IsNullOrEmpty(txtBranchSalesPrice.Text))
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select Catagory.if you not set catagory.\\n please set none catagory.!!');", true);
            //    return;
            //}
            //if (string.IsNullOrEmpty(ddlSubCatagory.SelectedItem.Text))
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select Sub-Catagory.if you not set sub-catagory.\\n please set none sub-catagory.!!');", true);
            //    return;
            //}
            //if (string.IsNullOrEmpty(txtDescription.Text))
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input Items Description.!!');", true);
            //    return;
            //}
            //if (string.IsNullOrEmpty(txtCode.Text))
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input Security Code No.!!');", true);
            //    return;
            //}
            else
            {

                
                
                ClsItemDetailsInfo aClsItemDetailsInfoObj = ClsItemDetailsManager.GetShowDetails(lblID.Text.Trim());
                if (aClsItemDetailsInfoObj == null)
                {
                    AutoID();

                    if (!string.IsNullOrEmpty(txtStyleNo.Text))
                    {
                        int count = IdManager.GetShowSingleValueInt("Count(*)", "StyleNo", "Item", txtStyleNo.Text);

                        if (count > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Alredy Add This Code Number...!!');", true);
                            return;
                        }
                    }
                    if (per.AllowAdd == "Y")
                    {
                        int count1 = IdManager.GetShowSingleValueInt("Count(*)",
                            "t1.Code", "Item t1", txtCode.Text);
                        if (count1 > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                "alert('Already this Code is Exist...!!');", true);
                            return;
                        }
                        aClsItemDetailsInfoObj = new ClsItemDetailsInfo();
                       // txtCode.Text = IdManager.GetDateTimeWiseSerial("", "Code", "Item");
                        aClsItemDetailsInfoObj.ItemsCode = txtCode.Text;
                        
                        if (CheckBox1.Checked == true)
                        {
                            aClsItemDetailsInfoObj.Active = true;
                        }
                        else
                        {
                            aClsItemDetailsInfoObj.Active = false;
                        }
                        aClsItemDetailsInfoObj.ItemsName = txtName.Text.Replace("'","");
                        aClsItemDetailsInfoObj.Umo = ddlUmo.SelectedValue;
                        aClsItemDetailsInfoObj.UnitPrice = txtUnitPrice.Text;

                        aClsItemDetailsInfoObj.BranchSalesPrice = txtBranchSalesPrice.Text.Replace(",", "");
                        if (string.IsNullOrEmpty(aClsItemDetailsInfoObj.BranchSalesPrice))
                        {
                            aClsItemDetailsInfoObj.BranchSalesPrice = "0";
                        }
                        aClsItemDetailsInfoObj.Currency = ddlCurrency.SelectedValue;
                        aClsItemDetailsInfoObj.OpeningStock = txtOpeningStock.Text.Replace(",", "");
                        aClsItemDetailsInfoObj.OpeningAmount = txtOpeningAmount.Text.Replace(",", "");
                        aClsItemDetailsInfoObj.ClosingStock = txtOpeningStock.Text.Replace(",", "");
                        aClsItemDetailsInfoObj.ClosingAmount = txtOpeningAmount.Text.Replace(",", "");
                        // Catagory.
                        if (string.IsNullOrEmpty(ddlCatagory.SelectedItem.Text))
                        {
                            aClsItemDetailsInfoObj.Catagory = ConfigurationManager.AppSettings["CatagoryID"];
                        }
                        else
                        {
                            aClsItemDetailsInfoObj.Catagory = ddlCatagory.SelectedValue;
                        }
                        //**** Sub Cat
                        if (string.IsNullOrEmpty(ddlCatagory.SelectedItem.Text))
                        {
                            aClsItemDetailsInfoObj.SubCatagory = ConfigurationManager.AppSettings["SubCatagoryID"];
                        }
                        else
                        {
                            aClsItemDetailsInfoObj.SubCatagory = ddlSubCatagory.SelectedValue;
                        }

                        if (string.IsNullOrEmpty(ddlTextCatagory.SelectedValue))
                        {
                            aClsItemDetailsInfoObj.Text = "0";
                        }
                        else
                        {
                            aClsItemDetailsInfoObj.Text = ddlTextCatagory.SelectedValue;
                        }
                        if (CheckBox2.Checked == true)
                        {
                            aClsItemDetailsInfoObj.DiscountCheck = true;
                        }
                        else
                        {
                            aClsItemDetailsInfoObj.DiscountCheck = false;
                        }
                        aClsItemDetailsInfoObj.Discount = txtDiscountAmount.Text;
                        if (Session["empPhoto"] != null )
                        {
                            aClsItemDetailsInfoObj.ItemsImage = (byte[])Session["empPhoto"];
                        }
                        aClsItemDetailsInfoObj.LoginBy = Session["userID"].ToString();
                        aClsItemDetailsInfoObj.Description = txtDescription.Text.Replace("'", "");
                        aClsItemDetailsInfoObj.Brand = ddlBrand.SelectedValue;
                        aClsItemDetailsInfoObj.ShortName = txtStName.Text.Replace("'", "");
                        aClsItemDetailsInfoObj.StyleNo = txtStyleNo.Text.Replace("'", "");
                        aClsItemDetailsInfoObj.SecurityCode = txtSecurityCode.Text.Replace("'", "");
                        //*********************** Color & Size Entry *******************//
                        getColor();
                        getSize();
                        foreach (System.Web.UI.WebControls.ListItem item in chkColor.Items)
                        {
                            if (item.Selected)
                            {
                                string Name = "";
                                DataTable dt = (DataTable)ViewState["ColorInfo"];
                                Name = IdManager.GetShowSingleValueString("ColorName", "ID", "ColorInfo", item.Value);
                                dt.Rows.Add(item.Value, Name);
                                ViewState["ColorInfo"] = dt;
                            }
                        }
                        DataTable dtt = new DataTable();
                        foreach (System.Web.UI.WebControls.ListItem item in chkSize.Items)
                        {
                            if (item.Selected)
                            {
                                DataTable dt = (DataTable)ViewState["SizeInfo"];
                                string Name = IdManager.GetShowSingleValueString("SizeName", "ID", "SizeInfo", item.Value);
                                dt.Rows.Add(item.Value, Name);
                                ViewState["SizeInfo"] = dt;
                            }
                        }
                        aClsItemDetailsInfoObj.ItemsType = rdbItemType.SelectedValue;
                        DataTable dtColor = (DataTable) ViewState["ColorInfo"];
                         DataTable dtSize = (DataTable) ViewState["SizeInfo"];
                         
                         ClsItemDetailsManager.SaveItemsInformation(aClsItemDetailsInfoObj, dtColor, dtSize);

                        RefreshAll();
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('Record is/are saved saved successfully....!!');", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('You are not Permitted this Step...!!');", true);
                    }

                }

                else
                {
                    if (per.AllowEdit == "Y")
                    {
                       

                        int count1 = IdManager.GetShowSingleValueInt("Count(*)",
                            "t1.Code", "Item t1", txtCode.Text,"t1.ID",Convert.ToInt32(lblID.Text));
                        if (count1 > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                "alert('Already this Code is Exist...!!');", true);
                            return;
                        }

                        aClsItemDetailsInfoObj.ItemsCode = txtCode.Text;
                        if (CheckBox1.Checked == true)
                        {
                            aClsItemDetailsInfoObj.Active = true;
                        }
                        else
                        {
                            aClsItemDetailsInfoObj.Active = false;
                        }
                        aClsItemDetailsInfoObj.ID = lblID.Text.Trim();
                        aClsItemDetailsInfoObj.ItemsName = txtName.Text.Replace("'", "");
                        aClsItemDetailsInfoObj.Umo = ddlUmo.SelectedValue;
                        aClsItemDetailsInfoObj.UnitPrice = txtUnitPrice.Text;
                        aClsItemDetailsInfoObj.BranchSalesPrice = txtBranchSalesPrice.Text;
                        aClsItemDetailsInfoObj.Currency = ddlCurrency.SelectedValue;
                        aClsItemDetailsInfoObj.OpeningStock = txtOpeningStock.Text.Replace(",", "");
                        aClsItemDetailsInfoObj.OpeningAmount = txtOpeningAmount.Text.Replace(",", "");
                        //aClsItemDetailsInfoObj.BranchSalesPrice = txtBranchSalesPrice.Text;
                        // Catagory.
                        if (string.IsNullOrEmpty(ddlCatagory.SelectedItem.Text))
                        {
                            aClsItemDetailsInfoObj.Catagory = ConfigurationManager.AppSettings["CatagoryID"];
                        }
                        else
                        {
                            aClsItemDetailsInfoObj.Catagory = ddlCatagory.SelectedValue;
                        }
                        //**** Sub Cat
                        if (string.IsNullOrEmpty(ddlCatagory.SelectedItem.Text))
                        {
                            aClsItemDetailsInfoObj.SubCatagory = ConfigurationManager.AppSettings["SubCatagoryID"];
                        }
                        else
                        {
                            aClsItemDetailsInfoObj.SubCatagory = ddlSubCatagory.SelectedValue;
                        }

                        aClsItemDetailsInfoObj.Text = ddlTextCatagory.SelectedValue;
                        if (CheckBox2.Checked == true)
                        {
                            aClsItemDetailsInfoObj.DiscountCheck = true;
                            aClsItemDetailsInfoObj.Discount = txtDiscountAmount.Text;
                        }
                        else
                        {
                            aClsItemDetailsInfoObj.DiscountCheck = false;
                            aClsItemDetailsInfoObj.Discount = txtDiscountAmount.Text;
                        }
                        if (Session["empPhoto"]!=null)
                        {
                            aClsItemDetailsInfoObj.ItemsImage = (byte[]) Session["empPhoto"];
                        }

                        aClsItemDetailsInfoObj.LoginBy = Session["userID"].ToString();
                        aClsItemDetailsInfoObj.Description = txtDescription.Text.Replace("'", "");
                        aClsItemDetailsInfoObj.Brand = ddlBrand.SelectedValue;
                        aClsItemDetailsInfoObj.ShortName = txtStName.Text.Replace("'", "");
                        aClsItemDetailsInfoObj.StyleNo = txtStyleNo.Text.Replace("'", "");
                        aClsItemDetailsInfoObj.SecurityCode = txtSecurityCode.Text.Replace("'", "");

                        //*********************** Color & Size Entry *******************//
                        getColor();
                        getSize();
                        foreach (System.Web.UI.WebControls.ListItem item in chkColor.Items)
                        {
                            if (item.Selected)
                            {
                                string Name = "";
                                DataTable dt = (DataTable)ViewState["ColorInfo"];
                                Name = IdManager.GetShowSingleValueString("ColorName", "ID", "ColorInfo", item.Value);
                                dt.Rows.Add(item.Value, Name);
                                ViewState["ColorInfo"] = dt;
                            }
                        }
                        DataTable dtt = new DataTable();
                        foreach (System.Web.UI.WebControls.ListItem item in chkSize.Items)
                        {
                            if (item.Selected)
                            {
                                DataTable dt = (DataTable)ViewState["SizeInfo"];
                                string Name = IdManager.GetShowSingleValueString("SizeName", "ID", "SizeInfo", item.Value);
                                dt.Rows.Add(item.Value, Name);
                                ViewState["SizeInfo"] = dt;
                            }
                        }
                        DataTable dtColor = (DataTable)ViewState["ColorInfo"];
                        DataTable dtSize = (DataTable)ViewState["SizeInfo"];
                        aClsItemDetailsInfoObj.ItemsType = rdbItemType.SelectedValue;
                        ClsItemDetailsManager.UpdateItemsInformation(aClsItemDetailsInfoObj, dtColor, dtSize);
                        RefreshAll();
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Update Sucessfully....!!');",
                            true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('You are not Permitted this Step...!!');", true);
                    }
                }
            }
            BtnSave.Enabled = true;
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                    "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                    "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }

    protected void BtnFind_Click(object sender, EventArgs e)
    {
        int ID = 0;

        ID = IdManager.GetShowSingleValueInt("ID", "UPPER(Code)", "Item", txtCode.Text.ToUpper());
        if (ID.Equals(0))
        {
            ID = IdManager.GetShowSingleValueInt("ID", "UPPER(StyleNo)", "Item", txtStyleNo.Text.ToUpper());
        }
        if (ID.Equals(0))
        {
            ID = IdManager.GetShowSingleValueInt("ID", "UPPER(SecurityCode)", "Item", txtSecurityCode.Text.ToUpper());
        }
        if (!ID.Equals(0))
        {
            lblID.Text = ID.ToString();
            GetItemInformationDetails(lblID.Text);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                    "alert('Not Found.!!');", true);
        }

    }

    protected void BtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            if (per.AllowDelete == "Y")
            {
                ClsItemDetailsInfo aClsItemDetailsInfoObj = new ClsItemDetailsInfo();
                aClsItemDetailsInfoObj.ItemsCode = txtCode.Text;
                aClsItemDetailsInfoObj.ItemsName = txtName.Text;
                aClsItemDetailsInfoObj.Umo = ddlUmo.SelectedValue;
                aClsItemDetailsInfoObj.LoginBy = Session["userID"].ToString();
                aClsItemDetailsInfoObj.ID = lblID.Text.Trim();
                ClsItemDetailsManager.DeleteItemsInformation(aClsItemDetailsInfoObj);
                RefreshAll();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Delete Sucessfully....!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
            }
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }
    protected void BtnReset_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
   

    protected void dgHistory_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
          
            lblID.Text = dgHistory.SelectedRow.Cells[1].Text.Trim();
            
            GetItemInformationDetails(lblID.Text);
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }

    private void GetItemInformationDetails(string ID)
    {
        ClsItemDetailsInfo Items = ClsItemDetailsManager.GetShowDetails(ID);
        if (Items != null)
        {
            rdbItemType.Enabled = false;
            Session["empPhoto"] = "";
            rdbItemType.SelectedValue = Items.ItemsType;
            txtCode.Text = Items.ItemsCode;
            txtName.Text = Items.ItemsName;
            ddlUmo.SelectedValue = Items.Umo;
            txtUnitPrice.Text = Items.UnitPrice;
            ddlCurrency.SelectedValue = Items.Currency;
            txtOpeningStock.Text = Items.OpeningStock.Replace(",", "");
            txtOpeningAmount.Text = Items.OpeningAmount.Replace(",", "");
            txtClosingStock.Text = Items.ClosingStock.Replace(",", "");
            txtClosingAmount.Text = Items.ClosingAmount.Replace(",", "");
            txtStName.Text = Items.ShortName;
            txtStyleNo.Text = Items.StyleNo;
            txtSecurityCode.Text = Items.SecurityCode;
            txtBranchSalesPrice.Text = Items.BranchSalesPrice;

            ddlCatagory.DataSource = MajorCategoryManager.GetMajorCats();
            ddlCatagory.DataTextField = "mjr_desc";
            ddlCatagory.DataValueField = "mjr_code";
            ddlCatagory.DataBind();
            ddlCatagory.Items.Insert(0, new ListItem(""));
            if (Items.Catagory != "0")
            {
                ddlCatagory.SelectedValue = Items.Catagory;
            }
            if (Items.Brand != "0")
            {
                ddlBrand.SelectedValue = Items.Brand;
            }
            ddlSubCatagory.DataSource = SubMajorCategoryManager.GetSubMajorCategories(ddlCatagory.SelectedValue);
            ddlSubCatagory.DataTextField = "Name";
            ddlSubCatagory.DataValueField = "ID";
            ddlSubCatagory.DataBind();
            ddlSubCatagory.Items.Insert(0, new ListItem(""));
            if (Items.SubCatagory != "0")
            {
                ddlSubCatagory.SelectedValue = Items.SubCatagory;
            }
            txtDescription.Text = Items.Description;
            if (Items.DiscountCheck == true)
            {
                CheckBox2.Checked = true;
            }

            if (Items.Discount == "")
            { txtDiscountAmount.Text = "0"; }
            else { txtDiscountAmount.Text = Items.Discount; }
           
            if (Items.Text != "0")
            {
                ddlTextCatagory.SelectedValue = Items.Text;
            }
            if (Items.Active == true)
            {
                CheckBox1.Checked = true;
            }
            ItemsPhoto = (byte[])Items.ItemsImage;
            Session["empPhoto"] = ItemsPhoto;
            if (ItemsPhoto != null)
            {
                string base64String = Convert.ToBase64String(ItemsPhoto, 0, ItemsPhoto.Length);
                imgEmp.ImageUrl = "data:image/png;base64," + base64String;
            }
            DataTable dtColor = _aItemDetailsManager.getItemsColor(lblID.Text);
            DataTable dtSize = _aItemDetailsManager.getItemsSize(lblID.Text);
            int Check = 0;
            foreach (System.Web.UI.WebControls.ListItem item in chkColor.Items)
            {
                int Val = Convert.ToInt32(item.Value);
                DataRow rw = dtColor.AsEnumerable().FirstOrDefault(tt => tt.Field<int>("ID") == Val);
                if (rw != null)
                {
                    chkColor.Items[Check].Selected = true;
                }
                Check++;
            }
            int Check1 = 0;
            foreach (System.Web.UI.WebControls.ListItem item in chkSize.Items)
            {
                int Val = Convert.ToInt32(item.Value);
                DataRow rw = dtSize.AsEnumerable().FirstOrDefault(tt => tt.Field<int>("ID") == Val);
                if (rw != null)
                {
                    chkSize.Items[Check1].Selected = true;
                }
                Check1++;
            }
            txtOpeningStock.Enabled = txtOpeningAmount.Enabled = txtClosingStock.Enabled = txtClosingAmount.Enabled = false;
            UPOPAmount.Update();
            UPOPStock.Update();
            UPanel1Cat.Update();
            UpdatePanel2.Update();
        }
    }
    protected void lbImgUpload_Click(object sender, EventArgs e)
    {
        try
        {
            if (imgUpload.HasFile)
            {
                int width = 145;
                int height = 165;
                using (System.Drawing.Bitmap img = DataManager.ResizeImage(new System.Drawing.Bitmap(imgUpload.PostedFile.InputStream), width, height, DataManager.ResizeOptions.ExactWidthAndHeight))
                {
                    imgUpload.PostedFile.InputStream.Close();
                    ItemsPhoto = DataManager.ConvertImageToByteArray(img, System.Drawing.Imaging.ImageFormat.Png);
                    Session["empPhoto"] = ItemsPhoto;
                    img.Dispose();
                }
                string base64String = Convert.ToBase64String(ItemsPhoto, 0, ItemsPhoto.Length);
                imgEmp.ImageUrl = "data:image/png;base64," + base64String;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please input employee first name, birth date, and then browse a photograph image!!');", true);
            }
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }   
    protected void dgHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgHistory.DataSource = ClsItemDetailsManager.getShowItemsHistoryDetails();
        dgHistory.PageIndex = e.NewPageIndex;
        dgHistory.DataBind();
    }
    protected void dgHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
            }
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }   
    protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckBox2.Checked == true) { txtDiscountAmount.Enabled = true; } 
        else
        { txtDiscountAmount.Enabled = false; }
    }
    protected void txtOpeningStock_TextChanged(object sender, EventArgs e)
    {
        try
        {
            txtOpeningAmount.Text = (Convert.ToDouble(txtUnitPrice.Text) * Convert.ToDouble(txtOpeningStock.Text)).ToString("N2");
            UPOPAmount.Update();
        }
        catch (FormatException fx)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fx.Message + "');", true);
        }
    }
    protected void lbSearch_Click(object sender, EventArgs e)
    {
        int ID = 0;
        string[] words = txtItemsSearch.Text.Trim().Split('-');
        if (words.Length > 2)
        {
            //ID = _aClsItemDetailsManager.getShowItemsHistoryDetails(lblItemIDSearch.Text, "", chkItemsType.SelectedValue);
            //lblID.Text = ID.ToString();
            //if (!ID.Equals(0))
            //{
            //    dgHistory_SelectedIndexChanged(sender, e);
            //}
            //else
            //{
            DataTable dtItems = _aClsItemDetailsManager.getShowItemsHistoryDetailsSearch(lblItemIDSearch.Text,txtItemsSearch.Text, "1",
                    chkItemsType.SelectedValue);
                dgHistory.Caption = "<h1>Total Items : " + dtItems.Rows.Count.ToString() + "</h1>";
                dgHistory.DataSource = dtItems;
                dgHistory.DataBind();
            //}
        }
        else
        {
            DataTable dtItems = _aClsItemDetailsManager.getShowItemsHistoryDetailsSearch(lblItemIDSearch.Text, txtItemsSearch.Text, "1",
                chkItemsType.SelectedValue);
            dgHistory.Caption = "<h1>Total Items : " + dtItems.Rows.Count.ToString() + "</h1>";
            dgHistory.DataSource = dtItems;
            dgHistory.DataBind();
        }
    }
    protected void lbClear_Click(object sender, EventArgs e)
    {
        txtItemsSearch.Text = lblItemIDSearch.Text = string.Empty;
        RefreshAll();
        chkItemsType.SelectedIndex = - 1;
        txtItemsSearch.Focus();
    }
    protected void btnClientSave_Click(object sender, EventArgs e)
    {

        
                    if (per.AllowAdd == "Y")
                    {
                        Brand msr = new Brand();
                        if (string.IsNullOrEmpty(txtBrandName.Text))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input Brand Name...!!');", true);

                        }
                        else
                        {

                            msr.CompanyDesc = txtBrandName.Text;
                            msr.Active = "true";
                            msr.LoginBy = Session["userID"].ToString();
                            BrandManage.CreateCompany(msr);

                            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Records are successfully saved!!');", true);
                            string query3 = "select * from View_BrandInfo  order by 2 ";
                            util.PopulationDropDownList(ddlBrand, "View_BrandInfo", query3, "BrandName", "ID");
                        }
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
               
                    }
              
    }

    protected void btnClientCategorySave0_Click(object sender, EventArgs e)
    {
        if (txtCategoryName.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Catagory Name..!!');", true);
        }
        else
        {
            MajorCategory aMajorCategory = MajorCategoryManager.GetMajorCat(lblID.Text);
            if (aMajorCategory == null)
            {
                if (per.AllowAdd == "Y")
                {
                    if (string.IsNullOrEmpty(txtSubcategory.Text))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Input Catagory ..!!');", true);

                    }
                    else
                    {
                        txtCategoryCode.Text = IdManager.GetTransSl1("Category", "Code");
                        aMajorCategory = new MajorCategory();
                        aMajorCategory.Code = txtCategoryCode.Text;
                        aMajorCategory.ID = lblID.Text;
                        aMajorCategory.Name = txtCategoryName.Text;
                        aMajorCategory.Description = txtCatageryDescription.Text;
                        if (CheckBox1.Checked)
                        {
                            aMajorCategory.Active = "1";
                        }
                        else { aMajorCategory.Active = "0"; }
                        aMajorCategory.LoginBy = Session["userID"].ToString();
                        MajorCategoryManager.CreateMajorCat(aMajorCategory);
                        RefreshAll();
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Catagory saved Sucessfully....!!');", true);
                    }

                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                }
            }
        }
    }

   

    protected void btnSubcategory_Click(object sender, EventArgs e)
    {
        SubMajorCategory aSubMajorCategory = SubMajorCategoryManager.GetSubMajorCat(lblID.Text);
                   
                        if (per.AllowAdd == "Y")
                        {
                            if (string.IsNullOrEmpty(txtSubcategory.Text))
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Sub Catagory Saved Sucessfully....!!');", true);
                    
                            }
                            else
                            {

                            txtSubCategoryCode.Text = IdManager.GetTransSl1("SubCategory", "Code");
                            aSubMajorCategory = new SubMajorCategory();
                            aSubMajorCategory.Code = txtSubCategoryCode.Text;
                            aSubMajorCategory.Code = txtSubCategoryCode.Text;
                            aSubMajorCategory.ID = lblID.Text;
                            aSubMajorCategory.Name = txtSubcategory.Text;
                            aSubMajorCategory.Catagory = ddlCategoryName.SelectedValue;
                            aSubMajorCategory.Description = txtSubcategory.Text;
                            if (CheckBox1.Checked)
                            {
                                aSubMajorCategory.Active = "1";
                            }
                            else { aSubMajorCategory.Active = "0"; }
                            aSubMajorCategory.LoginBy = Session["userID"].ToString();
                            SubMajorCategoryManager.CreateSubMajorCat(aSubMajorCategory);
                            RefreshAll();
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Sub Catagory Saved Sucessfully....!!');", true);
                        }
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                        }
    }



    protected void txtItemsSearch_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = DataManager.GetItemDataTable(" select ID,Code from Item where upper(Code+ ' - '+Name)=Upper('" + txtItemsSearch.Text + "') ", "Item", Session["user"].ToString());
        if (dt.Rows.Count > 0)
        {
            lblItemIDSearch.Text = dt.Rows[0]["ID"].ToString();

        }
        else
        {
            lblItemIDSearch.Text ="";
        }
    }
    public void rdbItemType_SelectedIndexChanged(object sender, EventArgs e)
    {
        AutoID();

        UPCode.Update();
        UPItemType.Update();
    }

    private void AutoID()
    {
        DataTable dtFixValue = IdManager.GetShowDataTable("select * from FixValue");

        if (rdbItemType.SelectedValue.ToString().Equals("1"))
        {
            string a ="0000" + dtFixValue.Rows[0]["RowMateriaID"].ToString();
            txtStyleNo.Text = "R-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + a.Substring(a.Length - 4, 4) ;
            txtCode.Text = "R-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + a.Substring(a.Length - 4, 4);
        }
        else if (rdbItemType.SelectedValue.ToString().Equals("2"))
        {
            string a = "0000" + dtFixValue.Rows[0]["FinishedGoodID"].ToString();
             txtStyleNo.Text = "F-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +  a.Substring(a.Length - 4, 4);
             txtCode.Text = "F-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + a.Substring(a.Length - 4, 4);
        }
        else if (rdbItemType.SelectedValue.ToString().Equals("3"))
        {
            string a = "0000" + dtFixValue.Rows[0]["CommonID"].ToString();
            txtStyleNo.Text = "C-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + a.Substring(a.Length - 4, 4);
            txtCode.Text = "C-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + a.Substring(a.Length - 4, 4);

        }
        else if (rdbItemType.SelectedValue.ToString().Equals("4"))
        {
            string a = "0000" + dtFixValue.Rows[0]["DamagedID"].ToString();
            txtStyleNo.Text = "D-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + a.Substring(a.Length - 4, 4);
            txtCode.Text = "D-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + a.Substring(a.Length - 4, 4);
        }
    }
}
