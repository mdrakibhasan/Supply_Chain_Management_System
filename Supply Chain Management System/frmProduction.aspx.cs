using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using autouniv;
using OldColor;
using System.Data.SqlClient;

public partial class frmProduction : System.Web.UI.Page
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
        if (!IsPostBack)
        {
            
            RefreshAll();
        }
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {
       // ClearFields();
        PanelHistory.Visible = btnNew.Visible  = false;
        
        btnSave.Visible = true;
        tabVch.Visible = true;
        Session["prodtl"] = null;
        getEmptyDtl();
        txtReceiveDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
       txtReceiveDate.Enabled =   txtRemarks.Enabled =txtRemarks.Enabled=txtBatchNo.Enabled = true;
     
    }
    private void ClearFields()
    {
        Session["prodtl"] = null;
        txtProductionNo.Text = "";
       
        txtRemarks.Text = "";
        // txtSiftment.Text = "";
        txtReceiveDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
      
        txtID.Text = "";
      
        VisiblePayment(false, false, false, false, false, false, false, false);
    }
    private void getEmptyDtl()
    {
        dgProductionDtl.Visible = true;
         DataTable dtDtlGrid = new DataTable();      
       
            dtDtlGrid.Columns.Add("ID", typeof(string));
            dtDtlGrid.Columns.Add("ItemID", typeof(string));
            dtDtlGrid.Columns.Add("item_code", typeof(string));
            dtDtlGrid.Columns.Add("item_desc", typeof(string));
            dtDtlGrid.Columns.Add("msr_unit_code", typeof(string));
            dtDtlGrid.Columns.Add("item_rate", typeof(decimal));
            dtDtlGrid.Columns.Add("qnty", typeof(string));
            dtDtlGrid.Columns.Add("ExpireDate", typeof(string));
            dtDtlGrid.Columns.Add("UMO", typeof(string));
            dtDtlGrid.Columns.Add("BrandName", typeof(string));

            DataRow dr = dtDtlGrid.NewRow();

            dtDtlGrid.Rows.Add(dr);
            dgProductionDtl.DataSource = dtDtlGrid;
            Session["prodtl"] = dtDtlGrid;
            dgProductionDtl.DataBind();
     
       
       
      
       
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            ProductionInfo productmst = ProductionManager.GetProductMst(txtID.Text.Trim());
            if (productmst != null)
                {
                    if (string.IsNullOrEmpty(txtReceiveDate.Text))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Input Receive Date...!!');", true);
                    }
                    //else if (string.IsNullOrEmpty(txtBatchNo.Text))
                    // {
                    //        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Input Batch NO...!!');", true);
                    //}
                   else
                   {

                            productmst.ID = txtID.Text;
                            productmst.GoodsReceiveNo = txtProductionNo.Text.Trim();
                            productmst.ReceiveDate = txtReceiveDate.Text;
                            productmst.Remarks = txtRemarks.Text;
                            productmst.BatchNo = txtBatchNo.Text;
                            productmst.ShiftmentID = "";
                            productmst.LoginBy = Session["userID"].ToString();
                            DataTable dt = (DataTable)Session["prodtl"];

                            if (dt.Rows.Count <= 1)
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('No Item Received....!!');", true);
                                return;
                            }
                            DataTable dtOldStk = (DataTable)ViewState["OldStock"];

                         string VCH_SYS_NO = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                            "t1.PAYEE='PD' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                            productmst.ID);
                        VouchMst vmst = VouchManager.GetVouchMst(VCH_SYS_NO.Trim());
                        if (vmst != null)
                        {
                            vmst.FinMon = FinYearManager.getFinMonthByDate(productmst.ReceiveDate);
                            vmst.ValueDate = productmst.ReceiveDate;
                            vmst.VchCode = "03";
                            vmst.RefFileNo = "";
                            vmst.VolumeNo = "";
                            vmst.SerialNo = productmst.GoodsReceiveNo;
                            if (string.IsNullOrEmpty(txtRemarks.Text))
                            {
                                vmst.Particulars = "Closing Stock for Production No. :" + txtProductionNo.Text;
                            }
                            else
                            {
                                vmst.Particulars = "Closing Stock for Production No. " + txtProductionNo.Text + " : " + txtRemarks.Text.Replace("'", "");
                            }
                            vmst.ControlAmt ="";
                            vmst.Payee = "PD";
                            vmst.CheckNo = "";
                            vmst.CheqDate ="";
                            vmst.CheqAmnt = "0";
                            vmst.UpdateUser = Session["user"].ToString().ToUpper();
                            vmst.UpdateDate = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
                            vmst.AuthoUserType = Session["userlevel"].ToString();
                        }

                        ProductionManager.UpdateProduction(productmst, dt, dtOldStk, vmst);
                            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been update successfully...!!');", true);
                            RefreshAll();
                    }
                }
                else
                {
                if (string.IsNullOrEmpty(txtReceiveDate.Text))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Input Receive Date...!!');", true);
                    }
                    //else if (string.IsNullOrEmpty(txtBatchNo.Text))
                    // {
                    //        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Input Batch NO...!!');", true);
                    //}
                   else
                   {

                    productmst = new ProductionInfo();
                        productmst.ReceiveDate = txtReceiveDate.Text;                    
                        productmst.Remarks = txtRemarks.Text;
                        productmst.BatchNo = txtBatchNo.Text;                        
                        txtProductionNo.Text = IdManager.GetDateTimeWiseSerial("PN", "PN", "ProductionMst");
                        productmst.GoodsReceiveNo = txtProductionNo.Text.Trim();                       
                        productmst.ShiftmentID = "";
                        productmst.LoginBy = Session["userID"].ToString();
                        DataTable dt = (DataTable)Session["prodtl"];
                    if(dt.Rows.Count<=1)
                    {
                         ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('No Item Received....!!');", true);
                         return;
                    }
                        //*************** Jurnal Voucher ******//
                        VouchMst vmst = new VouchMst();
                        vmst.FinMon = FinYearManager.getFinMonthByDate(txtReceiveDate.Text);
                        vmst.ValueDate = txtReceiveDate.Text;
                        vmst.VchCode = "03";
                        vmst.RefFileNo = "";
                        vmst.VolumeNo = "";
                        vmst.SerialNo = productmst.GoodsReceiveNo;
                        if (string.IsNullOrEmpty(txtRemarks.Text))
                        {
                            vmst.Particulars = "Closing Stock for Production No. :" + txtProductionNo.Text;
                        }
                        else
                        {
                            vmst.Particulars = "Closing Stock for Production No. " + txtProductionNo.Text + " : " + txtRemarks.Text.Replace("'", "");
                        }
                        //vmst.ControlAmt = txtTotalAmount.Text.Replace(",", "");
                        vmst.Payee = "PD";
                        vmst.CheckNo = "";
                        vmst.CheqDate = "";
                        vmst.CheqAmnt = "0";
                        vmst.MoneyRptNo = "";
                        vmst.MoneyRptDate = "";
                        vmst.TransType = "R";
                        vmst.BookName = "AMB";
                        vmst.EntryUser = Session["user"].ToString();
                        vmst.EntryDate = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
                        vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
                        vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
                        vmst.Status = "A";
                        vmst.AuthoUserType = Session["userlevel"].ToString();

                        ProductionManager.SaveProduct(productmst, dt, lblOrNo.Text,vmst);
                        //PV_Acc_JurnalVoucher_Save();
                        //if (ddlParty.SelectedItem.Text == "")
                        //{ PV_Acc_Debit_Voucher_Save(); }
                        //else { PV_Acc_Debit_Voucher_For_Party_Save(); }
                        RefreshAll();
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been saved successfully...!!');", true);
                       
                       // int ID = IdManager.GetShowSingleValueIntNotParameter("TOP(1)[ID]", "[ProductionMst] order by ID desc");
                       // Session["prodtl"] = ProductionManager.GetPurchaseItemsDetails(ID.ToString());
                    }
                }

            btnSave.Enabled = true;
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
    string PurchaseCode = "1-6010001";
    string OtherLabureCode = "1-4050000";
    string OtherCarriageCode = "1-4060000";
    string Other = "1-4070000";
    string AdditionalCharge = "1-6010002";

    
    

    protected void Delete_Click(object sender, EventArgs e)
    {

        ProductionInfo productmst = ProductionManager.GetProductMst(txtID.Text.Trim());
        if (productmst != null)
        {
            productmst.ID = txtID.Text;
            productmst.GoodsReceiveNo = txtProductionNo.Text.Trim();
           DataTable dt = (DataTable)Session["prodtl"];
           productmst.LoginBy = Session["userID"].ToString();
            ProductionManager.DeleteProduction(productmst, dt);
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been delete successfully...!!');", true);
            RefreshAll();
           
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Please Select Production No ...!!');", true);
        }

      
    }
    protected void Find_Click(object sender, EventArgs e)
    {

    }
    protected void Clear_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmProduction.aspx?mno=0.0");
        //RefreshAll();
    }

    private void RefreshAll()
    {
        //txtReceiveDate.Attributes.Add("onBlur", "formatdate('" + txtReceiveDate.ClientID + "')");
        PanelHistory.Visible = true;
        tabVch.Visible = false;
        //Session["Cash_Code"] = IdManager.GetShowSingleValueString("CASH_CODE", "BOOK_NAME", "GL_SET_OF_BOOKS", "AMB");
        DataTable dt = ProductionManager.GetShowProductionMst();
        dgProMst.DataSource = dt;
        dgProMst.DataBind();
        Session["PRoMst"] = dt;
        btnSave.Visible = false;
        Session["prodtl"] = null;
        
        dgProductionDtl.DataSource = null;
        dgProductionDtl.DataBind();
        txtProductionNo.Enabled = txtReceiveDate.Enabled = false;
        txtRemarks.Enabled = txtBatchNo.Enabled = false;
        //dgProductionDtl.DataSource = null;
        //dgProductionDtl.DataBind();
        txtID.Text = "";
        txtProductionNo.Text=txtBatchNo.Text= txtReceiveDate.Text=txtRemarks.Text= "";
        btnNew.Visible = true;
        PVI_UP.Update();
        //txtProductionNo.Focus();
    }
    //************* Pv Items Details ******//
    protected void dgProDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((DataRowView)e.Row.DataItem)["qnty"].ToString() != "" && ((DataRowView)e.Row.DataItem)["item_rate"].ToString() != "")
                {
                    decimal total = decimal.Parse(((DataRowView)e.Row.DataItem)["item_rate"].ToString()) *
                                   decimal.Parse(((DataRowView)e.Row.DataItem)["qnty"].ToString());
                    //((Label)e.Row.FindControl("lblTotal")).Text = total.ToString("N2");

                    //decimal totAdd = decimal.Parse(((Label)e.Row.FindControl("lblTotal")).Text) + ((decimal.Parse(((Label)e.Row.FindControl("lblTotal")).Text)));
                    ((Label)e.Row.FindControl("lblAddTotal")).Text = total.ToString("N2");
                   
                    //e.Row.Cells[6].Attributes.Add("style", "display:none");
                    //e.Row.Cells[7].Attributes.Add("style", "display:none");
                }
                e.Row.Cells[8].Attributes.Add("style", "display:none");
                e.Row.Cells[9].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[8].Attributes.Add("style", "display:none");
                e.Row.Cells[9].Attributes.Add("style", "display:none");
              //  e.Row.Cells[6].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[8].Attributes.Add("style", "display:none");
                e.Row.Cells[9].Attributes.Add("style", "display:none");
               // e.Row.Cells[6].Attributes.Add("style", "display:none");
            }
            e.Row.Cells[6].Attributes.Add("style", "display:none");

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
    protected void dgProDtl_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (Session["prodtl"] != null)
        {
            DataTable dtDtlGrid = (DataTable)Session["prodtl"];
            dtDtlGrid.Rows.RemoveAt(dgProductionDtl.Rows[e.RowIndex].DataItemIndex);
            if (dtDtlGrid.Rows.Count > 0)
            {
                string found = "";
                foreach (DataRow drf in dtDtlGrid.Rows)
                {
                    if (drf["item_code"].ToString() == "" && drf["item_desc"].ToString() == "")
                    {
                        found = "Y";
                    }
                }
                if (found == "")
                {
                    DataRow dr = dtDtlGrid.NewRow();
                    dtDtlGrid.Rows.Add(dr);
                }
                dgProductionDtl.DataSource = dtDtlGrid;
                dgProductionDtl.DataBind();
            }
            else
            {
                getEmptyDtl();
            }
            ShowFooterTotal();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Your session is over. Try it again!!');", true);
        }
    }

    // *************** PV History **************//

    protected void dgProMst_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgProMst.DataSource = Session["PRoMst"];
        dgProMst.PageIndex = e.NewPageIndex;
        dgProMst.DataBind();
    }
    protected void dgProMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtID.Text = dgProMst.SelectedRow.Cells[1].Text;
        ProductionInfo aProductionInfo = ProductionManager.GetProductMst(txtID.Text);

        if (aProductionInfo != null)
        {
            txtReceiveDate.Enabled = txtBatchNo.Enabled = txtRemarks.Enabled = true;
            //txtID.Text = dgPVMst.SelectedRow.Cells[7].Text;
            txtProductionNo.Text = aProductionInfo.GoodsReceiveNo;
            txtReceiveDate.Text = aProductionInfo.ReceiveDate;
            txtRemarks.Text = aProductionInfo.Remarks;
            txtBatchNo.Text = aProductionInfo.BatchNo;
            DataTable dt = ProductionManager.GetPurchaseItemsDetails(txtID.Text);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                dgProductionDtl.DataSource = dt;
                Session["prodtl"] = dt;
                ViewState["OldStock"] = dt;
                dgProductionDtl.DataBind();
                ShowFooterTotal();
            }
            else
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                dgProductionDtl.DataSource = dt;
                Session["prodtl"] = dt;
                ViewState["OldStock"] = dt;
                dgProductionDtl.DataBind();
                ShowFooterTotal();
            }

            tabVch.Visible = true;
            PanelHistory.Visible = btnNew.Visible = false;
            PVI_UP.Update();
            PVIesms_UP.Update();
            //UPPaymentMtd.Update();
            btnSave.Visible = true;
        }
    }
    protected void dgProMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
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
    //*********************** PV Details ********************************//
   
  
    protected void txtItemDesc_TextChanged(object sender, EventArgs e) 
    
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        if (!string.IsNullOrEmpty(((TextBox)gvr.FindControl("txtItemDesc")).Text))
        {
            DataTable dtdtl = (DataTable)Session["prodtl"];
            DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
            DataTable dt = ItemManager.GetItems(((TextBox)gvr.FindControl("txtItemDesc")).Text);
            if (dt.Rows.Count > 0)
            {
                //string flag = "";
                //foreach (DataRow Dr2 in dtdtl.Rows)
                //{
                //    if (Dr2["ItemID"].ToString() == dt.Rows[0]["ID"].ToString())
                //    {


                //        flag = "Y";
                //        ((TextBox)dgProductionDtl.Rows[dgProductionDtl.Rows.Count - 1].FindControl("txtItemDesc")).Focus();
                //        //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Already This Item Add...!!');", true);
                //    }

                //}


                //if (flag != "Y")
                //{

                bool IsCheck = false;
                foreach (DataRow ddr in dtdtl.Rows)
                {
                    if (!string.IsNullOrEmpty(ddr["ItemID"].ToString()))
                    {
                        if (ddr["ItemID"].ToString().Equals(((DataRow)dt.Rows[0])["ID"].ToString()))
                        {
                            IsCheck = true;
                            break;
                        }
                    }
                }
                if (IsCheck == true)
                {
                    dgProductionDtl.DataSource = dtdtl;
                    dgProductionDtl.DataBind();
                    ShowFooterTotal();
                    // ((TextBox)gvr.FindControl("txtItemRate")).Focus();
                    ((TextBox)gvr.FindControl("txtItemDesc")).Text = "";
                    ((TextBox)gvr.FindControl("txtItemDesc")).Focus();
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('This items already added...!!!');", true);
                    return;
                }
                    dtdtl.Rows.Remove(dr);
                    dr = dtdtl.NewRow();
                    dr["ItemID"] = ((DataRow)dt.Rows[0])["ID"].ToString();
                    dr["item_desc"] = ((DataRow)dt.Rows[0])["item_desc"].ToString();
                    dr["item_code"] = ((DataRow)dt.Rows[0])["item_code"].ToString();
                    dr["msr_unit_code"] = ((DataRow)dt.Rows[0])["msr_unit_code"].ToString();
                    dr["item_rate"] = Convert.ToDecimal(((DataRow)dt.Rows[0])["UnitPrice"].ToString()) ;
                    dr["qnty"] = "0.00";

                    dr["BrandName"] = ((DataRow)dt.Rows[0])["BrandName"].ToString();
                    dtdtl.Rows.InsertAt(dr, gvr.DataItemIndex);
                


            }


            string found = "";
            foreach (DataRow drd in dtdtl.Rows)
            {
                if (drd["ItemID"].ToString() == "" && drd["item_desc"].ToString() == "")
                {
                    found = "Y";
                }
            }
            if (found == "")
            {
                DataRow drd = dtdtl.NewRow();
                dtdtl.Rows.Add(drd);
            }
            dgProductionDtl.DataSource = dtdtl;
            dgProductionDtl.DataBind();
            ShowFooterTotal();
            Session["prodtl"]=dtdtl;
            // ((TextBox)gvr.FindControl("txtItemRate")).Focus();
            ((TextBox)dgProductionDtl.Rows[gvr.DataItemIndex].FindControl("txtQnty")).Focus();
        }
    }
    private void ShowFooterTotal()
    {
        decimal ctot = decimal.Zero;
        decimal totAddi = 0;
        decimal totRat = 0;
        decimal totQty = 0;
        decimal totItemsP = 0;
        decimal totA = 0;
        decimal Total = 0;

        if (Session["prodtl"] != null)
        {
            DataTable dt = (DataTable)Session["prodtl"];
            foreach (DataRow drp in dt.Rows)
            {
                if (drp["ItemID"].ToString() != "" && drp["item_rate"].ToString() != "" && drp["qnty"].ToString() != "")
                {
                    totRat += decimal.Parse(drp["item_rate"].ToString());
                    totQty += decimal.Parse(drp["qnty"].ToString());
                    totItemsP += decimal.Parse(drp["item_rate"].ToString()) * decimal.Parse(drp["qnty"].ToString());
                   // totA += decimal.Parse(drp["ExpireDate"].ToString());

                    //totAddi += (totItemsP * decimal.Parse(drp["Additional"].ToString())) / 100;
                    Total += decimal.Parse(drp["item_rate"].ToString()) * decimal.Parse(drp["qnty"].ToString()); ;
                }
            }
         
        }

        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
        TableCell cell;
        cell = new TableCell();
        cell.Text = "Total";
        cell.ColumnSpan = 4;
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.Text = "";
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        cell = new TableCell();
        
        cell.Text = totQty.ToString("N2");
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        //cell = new TableCell();
        //cell.Text = totItemsP.ToString("N0");
        //cell.HorizontalAlign = HorizontalAlign.Right;
        //row.Cells.Add(cell);
        //cell = new TableCell();
        //cell.Text = totA.ToString("N2");
        //cell.HorizontalAlign = HorizontalAlign.Right;
        //row.Cells.Add(cell);
        cell = new TableCell();
       // cell.ColumnSpan = 2;
        cell.Text = Total.ToString("N0");
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        row.Font.Bold = true;
        row.BackColor = System.Drawing.Color.LightGray;
        if (dgProductionDtl.Rows.Count > 0)
        {
            dgProductionDtl.Controls[0].Controls.Add(row);
        }
       
        //row.Attributes.Add("style", "display:none");
    }



    //*************************  txtItemsRate_TextChanged *******************//

    protected void txtItemsRate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)Session["prodtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                dr["ItemID"] = dr["ItemID"].ToString();
                dr["item_desc"] = dr["item_desc"].ToString();
                dr["item_code"] = dr["item_code"].ToString();
                dr["msr_unit_code"] = ((DropDownList)gvr.FindControl("ddlMeasure")).SelectedValue;
                dr["item_rate"] = Convert.ToDecimal(((TextBox)gvr.FindControl("txtItemRate")).Text);
                if (((TextBox)gvr.FindControl("txtQnty")).Text == "") { dr["qnty"] = "0"; }
                dr["qnty"] = Convert.ToDecimal(((TextBox)gvr.FindControl("txtQnty")).Text);
                string date=Convert.ToString(((TextBox)gvr.FindControl("txtExpireDate")).Text);
                if (!string.IsNullOrEmpty(date))
                { dr["ExpireDate"] = date; }
            }
            string found = "";
            foreach (DataRow drd in dt.Rows)
            {
                if (drd["ItemID"].ToString() == "" && drd["item_desc"].ToString() == "")
                {
                    found = "Y";
                }
            }
            if (found == "")
            {
                DataRow drd = dt.NewRow();
                dt.Rows.Add(drd);
            }
            dgProductionDtl.DataSource = dt;
            dgProductionDtl.DataBind();
            ShowFooterTotal();
            ((TextBox)dgProductionDtl.Rows[dgProductionDtl.Rows.Count - 2].FindControl("txtQnty")).Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }

    //*************************  txtQnty_TextChanged *******************//

    protected void txtQnty_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)Session["prodtl"];

           

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];

                //DataTable dtStock = ProductionManager.GetConsumtionItemStock(dr["ItemID"].ToString());
                //foreach (DataRow drCon in dtStock.Rows )
                //{
                    //string count = (Convert.ToDouble(((Convert.ToDouble(drCon["ItemQuantity"].ToString()) * Convert.ToDouble(((TextBox)gvr.FindControl("txtQnty")).Text))) - (Convert.ToDouble(dr["qnty"].ToString()) * Convert.ToDouble(drCon["ItemQuantity"].ToString())))).ToString();

                    //if (Convert.ToDouble(((Convert.ToDouble(drCon["ItemQuantity"].ToString()) * Convert.ToDouble(((TextBox)gvr.FindControl("txtQnty")).Text))) - (Convert.ToDouble(dr["qnty"].ToString()) * Convert.ToDouble(drCon["ItemQuantity"].ToString()))) > Convert.ToDouble(drCon["ClosingStock"].ToString()))
                    //{
                    //    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Warning :\\n Item : " + drCon["RMItem"].ToString() + " is Out of Closing Stock.!!!');", true);
                    //    ((TextBox)gvr.FindControl("txtQnty")).Text = "";
                    //    ((TextBox)gvr.FindControl("txtQnty")).Focus();
                    //    return;
                    //}
                //}
                dr["ItemID"] = dr["ItemID"].ToString();
                    dr["item_desc"] = dr["item_desc"].ToString();
                    dr["item_code"] = dr["item_code"].ToString();
                    dr["msr_unit_code"] = ((DropDownList)gvr.FindControl("ddlMeasure")).SelectedValue;
                    dr["item_rate"] = Convert.ToDecimal(((TextBox)gvr.FindControl("txtItemRate")).Text);
                    if (((TextBox)gvr.FindControl("txtQnty")).Text == "") { dr["qnty"] = "0"; }
                    dr["qnty"] = Convert.ToDecimal(((TextBox)gvr.FindControl("txtQnty")).Text);
                    string date = Convert.ToString(((TextBox)gvr.FindControl("txtExpireDate")).Text);
                    if (!string.IsNullOrEmpty(date))
                    { dr["ExpireDate"] = date; }
               
            }
            string found = "";
            foreach (DataRow drd  in dt.Rows)
            {
                if (drd["ItemID"].ToString() == "" && drd["item_desc"].ToString() == "")
                {
                    found = "Y";
                }
            }
            if (found == "")
            {
                DataRow drd = dt.NewRow();
                dt.Rows.Add(drd);
            }
            dgProductionDtl.DataSource = dt;
            dgProductionDtl.DataBind();
            ShowFooterTotal();


            
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
    //*************************  txtAdditional_TextChanged *******************//

    protected void txtExpireDate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)Session["prodtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                string date = Convert.ToString(((TextBox)gvr.FindControl("txtExpireDate")).Text);
                 dr["ExpireDate"] = date; 
            }
            dgProductionDtl.DataSource = dt;
            dgProductionDtl.DataBind();
            ShowFooterTotal();
            
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }


    protected void ddlPaymentMethord_SelectedIndexChanged(object sender, EventArgs e)
    {
      
    }
    public void VisiblePayment(bool lblBank, bool Bank, bool lblChkNo, bool ChkNo, bool lblChkDate, bool chkdate, bool lblChkStatus, bool chkStatus)
    {
       
    }
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = ProductionManager.GetShowPVMasterInfo("");
        if (dt.Rows.Count > 0)
        {
            txtID.Text = dt.Rows[0]["ID"].ToString();
            //txtID.Text = dgPVMst.SelectedRow.Cells[7].Text;
            btnFind_Click(sender, e);

        }
    }
    protected void btnFind_Click(object sender, EventArgs e)
    {

       
    }
    private static DataTable dtmsr = new DataTable();
    public DataTable PopulateMeasure()
    {
        dtmsr = ItemManager.GetMeasure();
        DataRow dr = dtmsr.NewRow();
        dtmsr.Rows.InsertAt(dr, 0);
        return dtmsr;
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
           //CashReport();
       
           // BankReport();
        
    }

    //*************************  Check Popup  *******************//


 



    protected void ddlMeasure_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((DropDownList)sender).NamingContainer;
            DataTable dt = (DataTable)Session["prodtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                dr["ID"] = dr["ID"].ToString();
                dr["item_desc"] = dr["item_desc"].ToString();
                dr["item_code"] = dr["item_code"].ToString();
                dr["msr_unit_code"] = ((DropDownList)gvr.FindControl("ddlMeasure")).SelectedValue;
                dr["item_rate"] = Convert.ToDecimal(((TextBox)gvr.FindControl("txtItemRate")).Text);
                if (((TextBox)gvr.FindControl("txtQnty")).Text == "") {dr["qnty"] = "0.00"; }
                dr["qnty"] = Convert.ToDecimal(((TextBox)gvr.FindControl("txtQnty")).Text);
                string date = Convert.ToString(((TextBox)gvr.FindControl("txtExpireDate")).Text);
                if (!string.IsNullOrEmpty(date))
                { dr["ExpireDate"] = date; }
            }
       
            
            dgProductionDtl.DataSource = dt;
            dgProductionDtl.DataBind();
            ShowFooterTotal();
          
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
}