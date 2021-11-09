using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OldColor;
using autouniv;
using System.Data.SqlClient;

public partial class frmConsumption : System.Web.UI.Page
{
 private byte[] ItemsPhoto;
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
        if(!IsPostBack)
        {
            RefeshAll();
        }
    }

    private void RefeshAll()
    {
        PnlDtl.Visible = false;
        btnPrint.Visible = false;
        txtConsumptionNo.Enabled = txtFinishGoodItem.Enabled = ddlStatus.Enabled = txtDeclareDate.Enabled = false;
        BtnSave.Visible = false;
        txtFinishGoodItem.Text = "";
        ddlStatus.SelectedValue = null;
        txtDeclareDate.Text = "";
        txtConsumptionNo.Text = "";
        btnNew.Visible = true;
        DataTable dt = ConsumptionManager.GetConsumptionMst("");
        dgConsumptionMst.DataSource = dt;
        dgConsumptionMst.DataBind();
        ViewState["ConsumptionMst"] = dt;
        dgConsumptionMst.Visible = true;
        txtTotalCost.Text = txtTotal.Text = "";
        dgItemDetails.DataSource = null;
        ViewState["ConsumptionDtl"] = null;
        dgItemDetails.DataBind();
        lblTotalCost.Text = "0";
        lblTotal.Text = "0";
        lblMstID.Text = "";
        txtDeclareDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
        txtFinishGoodItem.Focus();
    }
    protected void BtnSave_Click(object sender, EventArgs e)
    {
       
        DataTable dt =(DataTable) ViewState["ConsumptionDtl"]  ;
         
        if(!string.IsNullOrEmpty(lblMstID.Text))
        {
           

                if (dt.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(txtFinishGoodItem.Text))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Select F.G Item...!!')", true);
                        txtFinishGoodItem.Focus();
                    }
                    else if (string.IsNullOrEmpty(ddlStatus.SelectedValue))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Select Item Status...!!')", true);
                        ddlStatus.Focus();
                    }
                    else if (string.IsNullOrEmpty(txtDeclareDate.Text))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Select item Date...!!')", true);
                        txtDeclareDate.Focus();
                    }
                    else
                    {

                        ConsumptionInfo aConsumptionInfo = new ConsumptionInfo();
                        aConsumptionInfo.FGItemIdMst = lblFinishGoodItemID.Text;
                        aConsumptionInfo.StatusMst = ddlStatus.SelectedValue;
                        aConsumptionInfo.DeclareDate = txtDeclareDate.Text;
                        aConsumptionInfo.TotalCost = lblTotalCost.Text;
                        aConsumptionInfo.FGQuantity = txtFGQuantity.Text;

                        aConsumptionInfo.LogineBy = Session["userID"].ToString();
                        ConsumptionManager.UpdateConsumptionInfo(aConsumptionInfo, dt, lblMstID.Text);
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Update Succesfully...!!')", true);
                       
                        RefeshAll();
                    }
                }
        
            }
        
        else
        {
                      
            if (dt.Rows.Count > 0)
            {
                if (string.IsNullOrEmpty(txtFinishGoodItem.Text))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Select F.G Item...!!')", true);
                    txtFinishGoodItem.Focus();
                }
                else if (string.IsNullOrEmpty(ddlStatus.SelectedValue))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Select Item Status...!!')", true);
                    ddlStatus.Focus();
                }
                else if (string.IsNullOrEmpty(txtDeclareDate.Text))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Select item Date...!!')", true);
                    txtDeclareDate.Focus();
                }
                else
                    
                {

                    ConsumptionInfo aConsumptionInfo = new ConsumptionInfo();
                    aConsumptionInfo.FGItemIdMst = lblFinishGoodItemID.Text;
                    aConsumptionInfo.StatusMst = ddlStatus.SelectedValue;
                    aConsumptionInfo.DeclareDate = txtDeclareDate.Text;
                    aConsumptionInfo.TotalCost = lblTotalCost.Text;
                    aConsumptionInfo.FGQuantity = txtFGQuantity.Text;


                    aConsumptionInfo.LogineBy = Session["userID"].ToString();
                    ConsumptionManager.SaveConsumptionInfo(aConsumptionInfo, dt);
                    
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Save Succesfully...!!')", true);
                    RefeshAll();
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Raw Materials Item Add...!!')", true);
            }
        }
        BtnSave.Enabled = true;
        
        
        
    }
    protected void txtRawMaterialItem_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();

        dt = (DataTable)ViewState["ConsumptionDtl"];

        DataTable dtitem = ConsumptionManager.GetItemInformation(txtRawMaterialItem.Text);
        if (dtitem.Rows.Count > 0)
        {
            //string flag = "";
            //if(dt !=null)
            //{
            //    foreach(DataRow dr in dt.Rows)
            //    {
            //        if (dr["ItemID"].ToString() == dtitem.Rows[0]["ID"].ToString())
            //        {
            //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Already This Item Add...!!')", true);
            //            flag = "Y";
            //            txtRawMaterialItem.Text = "";
            //            txtRawMaterialItem.Focus();
            //        }

               
            //    }
            //}

            bool IsCheck = false;
            if(dt !=null)
            {
                
                foreach (DataRow ddr in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(ddr["ItemID"].ToString()))
                    {
                        if (ddr["ItemID"].ToString().Equals((dtitem.Rows[0]["ID"].ToString())))
                        {
                            IsCheck = true;
                            break;
                        }
                    }
                }
            }
            if (IsCheck == true)
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('This items already added...!!!');", true);
                txtRawMaterialItem.Text = "";
                txtRawMaterialItem.Focus();
                return;
            }

           
                txtRawMaterialItem.Text = dtitem.Rows[0]["Name"].ToString();
                lblRawMaterialsItemID.Text = dtitem.Rows[0]["ID"].ToString();
                txtUnitePrice.Text = dtitem.Rows[0]["UnitPrice"].ToString();
            ddlUniteType.SelectedValue = dtitem.Rows[0]["UmoID"].ToString();

            // UP2.Update();
        }
    }
    protected void txtFinishGoodItem_TextChanged(object sender, EventArgs e)
    {
        DataTable dtitem = ConsumptionManager.GetItemInformation(txtFinishGoodItem.Text);
        if (dtitem.Rows.Count > 0)
        {
            DataTable ActiveItem = ConsumptionManager.CheckActiveItem(dtitem.Rows[0]["ID"].ToString(), ddlStatus.SelectedValue);
            if (ActiveItem.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('This Item Already Active...!!');", true);
                txtFinishGoodItem.Text = "";
                txtFinishGoodItem.Focus();
            }
            else
            {

                txtFinishGoodItem.Text = dtitem.Rows[0]["Name"].ToString();
                lblFinishGoodItemID.Text = dtitem.Rows[0]["ID"].ToString();
                //lblUnitPrice.Text = dtitem.Rows[0]["UnitPrice"].ToString();
                //lblClosingStock.Text = dtitem.Rows[0]["ClosingStock"].ToString();
                UP1.Update();
            }
        }
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();
        dt = (DataTable) ViewState["ConsumptionDtl"] ;
        if (dt == null)
        {
            dt = new DataTable();
            dt.Columns.Add("ItemID", typeof(int));
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("UnitePrice", typeof(decimal));
            dt.Columns.Add("UniteTypeId", typeof(int));
            dt.Columns.Add("dtlID", typeof(string));
            dt.Columns.Add("Total", typeof(decimal));
            lblTotalCost.Text = "0";
            if (string.IsNullOrEmpty(txtRawMaterialItem.Text))
            {
               // ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Select Raw Metrial Itemm...!!');", true);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Select Raw Metrial Itemm...!!')", true);
            
            }

            else if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage",
                    "alert('Please Input Quantity ...!!')", true);
            }
            else
            {
                DataRow dr = dt.NewRow();
                dr["ItemID"] = lblRawMaterialsItemID.Text;
                dr["ItemName"] = txtRawMaterialItem.Text;
                dr["Quantity"] = Convert.ToDecimal(txtQuantity.Text);
                dr["UnitePrice"] = Convert.ToDecimal(txtUnitePrice.Text);
                dr["UniteTypeId"] = ddlUniteType.SelectedValue;
                dr["dtlID"] = "0";
                dr["total"] = Convert.ToDecimal(txtTotal.Text);
                dt.Rows.Add(dr);
                ViewState["ConsumptionDtl"] = dt;
                dgItemDetails.DataSource = dt;
                dgItemDetails.DataBind();
                txtRawMaterialItem.Text = "";
                lblRawMaterialsItemID.Text = "";
                txtUnitePrice.Text = "0";
                txtQuantity.Text = "0";
                txtTotal.Text = "0";
                UP2.Update();
            }
            string TotalCost = (Convert.ToDecimal(lblTotalCost.Text)+ Convert.ToDecimal(lblTotal.Text)).ToString();
            lblTotalCost.Text = TotalCost.ToString();
            txtTotalCost.Text = TotalCost.ToString();
            lblTotal.Text = "0";
            txtTotal.Text = "0";
            ddlUniteType.SelectedIndex = -1;
            UP1.Update();
        }
        else 
        {
            if (string.IsNullOrEmpty(txtRawMaterialItem.Text))
            {
              //  ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Select Raw Metrial Itemm...!!');", true);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Select Raw Metrial Itemm...!!...!!')", true);
            }

            else if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                ScriptManager.RegisterStartupScript(this,this.GetType(), "alertMessage", "alert('Please Input Quantity ...!!');", true);
            }

            else
            {
              
                    DataRow dr = dt.NewRow();
                    dr["ItemID"] = lblRawMaterialsItemID.Text;
                    dr["ItemName"] = txtRawMaterialItem.Text;
                    dr["Quantity"] = Convert.ToDecimal(txtQuantity.Text); 
                    dr["UnitePrice"] = Convert.ToDecimal(txtUnitePrice.Text);
                   dr["UniteTypeId"] = ddlUniteType.SelectedValue;
                    dr["dtlID"] = "0";
                    dr["total"] = Convert.ToDecimal(txtTotal.Text);
                    dt.Rows.Add(dr);
                    ViewState["ConsumptionDtl"] = dt;
                    dgItemDetails.DataSource = dt;
                    dgItemDetails.DataBind();

                    txtRawMaterialItem.Text = "";
                    lblRawMaterialsItemID.Text = "";
                    txtUnitePrice.Text = "";
                    txtQuantity.Text = "";
                   // UP2.Update();
                    string TotalCost = (Convert.ToDecimal(lblTotalCost.Text) + Convert.ToDecimal(lblTotal.Text)).ToString();
                    txtTotal.Text = "";

                    lblTotalCost.Text = TotalCost.ToString();
                    txtTotalCost.Text = TotalCost.ToString();
                    UP1.Update();
                }
            }
           
        
    }
    protected void BtnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(lblMstID.Text))
        {
            ConsumptionInfo aConsumptionInfo = new ConsumptionInfo();
            aConsumptionInfo.LogineBy = Session["userID"].ToString();
            ConsumptionManager.DeleteConsumptionInfo(lblMstID.Text, aConsumptionInfo);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Record Delete Succesfully ...!!');", true);
            RefeshAll();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Please Select Item ...!!');", true);
        }
    }
    protected void BtnReset_Click(object sender, EventArgs e)
    {
        RefeshAll();
    }
   
    protected void txtQuantity_TextChanged(object sender, EventArgs e)
    {

       // object ClosingStock = ConsumptionManualyManager.GetClosingStock(lblRawMaterialsItemID.Text);
        //if (Convert.ToDouble(txtQuantity.Text) > Convert.ToDouble(ClosingStock))
        //{
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('This Item Quantity More Than Closing Stock ...!!')", true);
        //    txtQuantity.Text = "0";
        //    txtQuantity.Focus();
        //}
            lblTotal.Text = (Convert.ToDecimal(txtUnitePrice.Text) * Convert.ToDecimal(txtQuantity.Text)).ToString();
            txtTotal.Text = (Convert.ToDecimal(txtUnitePrice.Text) * Convert.ToDecimal(txtQuantity.Text)).ToString();
            UP2.Update();
       
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {
        txtFinishGoodItem.Enabled = ddlStatus.Enabled = txtDeclareDate.Enabled = true;
        BtnSave.Visible = true;
        PnlDtl.Visible = true;
        btnNew.Visible = false;
        dgConsumptionMst.Visible = false;
       string Consumption= ConsumptionManager.GetConsumptionNo();
       txtConsumptionNo.Text = Consumption;
       txtDeclareDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
       string queryLoc = "select ID,Name from UOM";
       util.PopulationDropDownList(ddlUniteType, "UOM", queryLoc, "Name", "ID");
        ddlUniteType.Items.Insert(0,"");
       //DataTable dt = ConsumptionManager.GetMeasure();     
       //ddlUniteType.DataSource = dt;
       //ddlUniteType.DataBind();
        


    }

    protected void dgConsumptionMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblMstID.Text = dgConsumptionMst.SelectedRow.Cells[1].Text;
        DataTable dtMst = ConsumptionManager.GetConsumptionMst(lblMstID.Text);
        if (dtMst.Rows.Count > 0)
        {
            txtDeclareDate.Text = dtMst.Rows[0]["DeclareDate"].ToString();
            ddlStatus.SelectedValue = dtMst.Rows[0]["Status"].ToString();
            lblFinishGoodItemID.Text = dtMst.Rows[0]["FGItemID"].ToString();
            txtFinishGoodItem.Text = dtMst.Rows[0]["ItemNamae"].ToString();
            lblTotalCost.Text = dtMst.Rows[0]["TotalCost"].ToString();
            txtConsumptionNo.Text = dtMst.Rows[0]["ID"].ToString();
            txtTotalCost.Text = lblTotalCost.Text;
        }

        DataTable dtDtl = ConsumptionManager.GetConsumptiondtlInfoDetails(lblMstID.Text);
        if (dtDtl.Rows.Count > 0)
        {
            string queryLoc = "select ID,Name from UOM";
            util.PopulationDropDownList(ddlUniteType, "UOM", queryLoc, "Name", "ID");
            txtFinishGoodItem.Enabled = ddlStatus.Enabled = txtDeclareDate.Enabled = true;
            BtnSave.Visible = true;
            PnlDtl.Visible = true;
            btnNew.Visible = false;
            dgItemDetails.Visible = true;

            ViewState["ConsumptionDtl"] = dtDtl;
            dgItemDetails.DataSource = dtDtl;
            dgItemDetails.DataBind();
            //UP2.Update();
            dgConsumptionMst.Visible = false;
            //UP1.Update();

        }
    }
    protected void dgItemDetails_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void dgItemDetails_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["ConsumptionDtl"] != null)
        {
            DataTable dt = (DataTable)ViewState["ConsumptionDtl"];
            DataRow dr1 = dt.Rows[dgItemDetails.Rows[e.RowIndex].DataItemIndex];

            txtTotalCost.Text = (Convert.ToDouble(lblTotalCost.Text) - Convert.ToDouble(dr1["Total"].ToString())).ToString();
            lblTotalCost.Text =txtTotalCost.Text;
            dt.Rows.Remove(dr1);
            dgItemDetails.DataSource = dt;
            ViewState["ConsumptionDtl"] = dt;
            dgItemDetails.DataBind();        
          
             UP2.Update();
            UP1.Update();
            
        }
    }
    protected void txtGRNO_TextChanged(object sender, EventArgs e)
    {

    }
    protected void dgItemDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if ( e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Footer )
        {
            
            
            e.Row.Cells[2].Attributes.Add("style", "display:none");
        }
    }


    protected void dgConsumptionMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Footer)
        {

            e.Row.Cells[1].Attributes.Add("style", "display:none");
            e.Row.Cells[5].Attributes.Add("style", "display:none");
        }
    }
    protected void lbClear_Click(object sender, EventArgs e)
    {
        txtRawMaterialItem.Text = txtUnitePrice.Text = txtTotal.Text = txtQuantity.Text = string.Empty;
        ddlUniteType.SelectedIndex = -1;
        txtRawMaterialItem.Focus();
        UP2.Update();
    }
    protected void dgItemDetails_RowDataBound1(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Footer)
        {


            e.Row.Cells[1].Attributes.Add("style", "display:none");
        }
    }
}