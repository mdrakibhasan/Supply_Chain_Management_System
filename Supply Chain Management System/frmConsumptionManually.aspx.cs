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
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.draw;

public partial class frmConsumptionManually : System.Web.UI.Page
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
            ReFreshAll();
        }
    }

    private void ReFreshAll()
    {
        pnlConMn.Visible = false;
        getEmptyDtl();
        lblmstID.Text = "";
        txtRemarks.Enabled = txtConsumptionDate.Enabled = false;        
        dgConsumptionManualyDtl.DataBind();
        btnNew.Visible = true;
        btnSave.Visible = false;
        txtConsumptionNo.Text=txtRemarks.Text=txtConsumptionDate.Text = "";
       DataTable dt= ConsumptionManualyManager.GetConsumtionMAnualyMstInfo("");
       dgMst.DataSource = dt;
       dgMst.DataBind();
       dgMst.Visible = true;
       Session["ItemDtl"] = null;
       txtConsumptionDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        pnlConMn.Visible = true;
        txtRemarks.Enabled = txtConsumptionDate.Enabled = true;
        txtConsumptionDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
        getEmptyDtl();
        btnSave.Visible = true;
        btnNew.Visible = false;
        dgMst.Visible = false;
       string ConsumptionNo= ConsumptionManualyManager.GetMstAutoID();
       txtConsumptionNo.Text = ConsumptionNo;
       btnSave.Visible = true;

            
    }
    public DataTable PopulateMeasure()
    {
        DataTable dtmsr = new DataTable();
        dtmsr = ItemManager.GetMeasure();
        DataRow dr = dtmsr.NewRow();
        dtmsr.Rows.InsertAt(dr, 0);
        return dtmsr;
    }
    private void getEmptyDtl()
    {
        dgConsumptionManualyDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ID", typeof(string));
        dtDtlGrid.Columns.Add("item_code", typeof(string));
        dtDtlGrid.Columns.Add("item_desc", typeof(string));
        dtDtlGrid.Columns.Add("msr_unit_code", typeof(string));
        dtDtlGrid.Columns.Add("item_rate", typeof(Decimal));
        dtDtlGrid.Columns.Add("qnty", typeof(Decimal));
        dtDtlGrid.Columns.Add("ItemID", typeof(string));
        dtDtlGrid.Columns.Add("BrandName", typeof(string));
        dtDtlGrid.Columns.Add("DtlsID", typeof(string));
        DataRow dr = dtDtlGrid.NewRow();

        dtDtlGrid.Rows.Add(dr);
        dgConsumptionManualyDtl.DataSource = dtDtlGrid;
        dgConsumptionManualyDtl.DataBind();
        Session["ItemDtl"] = dtDtlGrid;
        
    }
    protected void txtItemDesc_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;

        DataTable dtdtl = (DataTable)Session["ItemDtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        DataTable dt = ItemManager.GetItems(((TextBox)gvr.FindControl("txtItemDesc")).Text);
        string flag = "";



        if (dt.Rows.Count > 0)
        {
            //     foreach (DataRow Dr2 in dtdtl.Rows)
            //     {
            //         if (Dr2["ItemID"].ToString() == dt.Rows[0]["ID"].ToString())
            //         {
            //             flag = "Y";
            //             ((TextBox)dgConsumptionManualyDtl.Rows[dgConsumptionManualyDtl.Rows.Count - 1].FindControl("txtItemDesc")).Focus();
            //             //((TextBox)dgConsumptionManualyDtl.Rows[dgConsumptionManualyDtl.Rows.Count - 1].FindControl("txtItemDesc")).Focus();
            //             //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Already This Item Add...!!');", true);
            //         }

            //     }
            //if (flag != "Y")
            //  {

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
                dgConsumptionManualyDtl.DataSource = dtdtl;
                dgConsumptionManualyDtl.DataBind();
                Session["ItemDtl"] = dtdtl;
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('This items already added...!!!');", true);
                ((TextBox)gvr.FindControl("txtItemDesc")).Text = "";
                ((TextBox)gvr.FindControl("txtItemDesc")).Focus();
                return;
            }
            dtdtl.Rows.Remove(dr);
            dr = dtdtl.NewRow();
            //dr["ID"] = ((DataRow)dt.Rows[0])["ID"].ToString();
            dr["ItemID"] = ((DataRow)dt.Rows[0])["ID"].ToString();
            dr["item_desc"] = ((DataRow)dt.Rows[0])["item_desc"].ToString();
            dr["item_code"] = ((DataRow)dt.Rows[0])["item_code"].ToString();
            dr["msr_unit_code"] = ((DataRow)dt.Rows[0])["msr_unit_code"].ToString();
            dr["item_rate"] = ((DataRow)dt.Rows[0])["UnitPrice"].ToString(); ;
            dr["qnty"] = "0.00";
            dr["BrandName"] = ((DataRow)dt.Rows[0])["BrandName"].ToString();
            dtdtl.Rows.InsertAt(dr, gvr.DataItemIndex);


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
        }
        dgConsumptionManualyDtl.DataSource = dtdtl;
        dgConsumptionManualyDtl.DataBind();
        Session["ItemDtl"] = dtdtl;
        ShowFooterTotal();
        // ((TextBox)gvr.FindControl("txtItemRate")).Focus();
        ((TextBox)dgConsumptionManualyDtl.Rows[gvr.DataItemIndex].FindControl("txtItemRate")).Focus();

    }
    protected void txtQnty_TextChanged(object sender, EventArgs e)
    {
       
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)Session["ItemDtl"];
            if (!string.IsNullOrEmpty(((TextBox)gvr.FindControl("txtQnty")).Text))
            {
               
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[gvr.DataItemIndex];
                    object ClosingStock = ConsumptionManualyManager.GetClosingStock(dr["ItemID"].ToString());
                    //if (Convert.ToDouble(Convert.ToDouble(((TextBox)gvr.FindControl("txtQnty")).Text) - (Convert.ToDouble(dr["qnty"].ToString()))) > Convert.ToDouble(ClosingStock))
                    //{
                    //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('This Item Quantity More Than Closing Stock ...!!')", true);
                    //    ((TextBox)gvr.FindControl("txtQnty")).Focus();
                    //}
                    //else
                    //{
                        //dr["ID"] = dr["ID"].ToString();
                        dr["ItemID"] = dr["ItemID"].ToString();
                        dr["item_desc"] = dr["item_desc"].ToString();
                        dr["item_code"] = dr["item_code"].ToString();
                        dr["msr_unit_code"] = ((DropDownList)gvr.FindControl("ddlMeasure")).SelectedValue;
                        dr["item_rate"] = ((TextBox)gvr.FindControl("txtItemRate")).Text;
                        if (((TextBox)gvr.FindControl("txtQnty")).Text == "") { dr["qnty"] = "0.00"; }
                        dr["qnty"] = decimal.Parse(((TextBox)gvr.FindControl("txtQnty")).Text);
                   // }
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
                Session["ItemDtl"] = dt;
                dgConsumptionManualyDtl.DataSource = dt;
                dgConsumptionManualyDtl.DataBind();
                ShowFooterTotal();
                // ((TextBox)gvr.FindControl("txtItemRate")).Focus();
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

        if (Session["ItemDtl"] != null)
        {
            DataTable dt = (DataTable)Session["ItemDtl"];
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
        cell.ColumnSpan = 2;
        cell.Text = Total.ToString("N0");
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        row.Font.Bold = true;
        row.BackColor = System.Drawing.Color.LightGray;
        if (dgConsumptionManualyDtl.Rows.Count > 0)
        {
            dgConsumptionManualyDtl.Controls[0].Controls.Add(row);
        }

        //row.Attributes.Add("style", "display:none");
    }

    
    protected void ddlMeasure_SelectedIndexChanged(object sender, EventArgs e)
    {

        

            GridViewRow gvr = (GridViewRow)((DropDownList)sender).NamingContainer;
            DataTable dt = (DataTable)Session["ItemDtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];

                if (dr["ItemID"].ToString() != "" && dr["item_desc"].ToString()!="")
                {
                
                    dr["ItemID"] = dr["ItemID"].ToString();
                    dr["item_desc"] = dr["item_desc"].ToString();
                    dr["item_code"] = dr["item_code"].ToString();
                    dr["msr_unit_code"] = ((DropDownList)gvr.FindControl("ddlMeasure")).SelectedValue;
                    dr["item_rate"] = decimal.Parse(dr["item_rate"].ToString());
                    if (((TextBox)gvr.FindControl("txtQnty")).Text == "") { dr["qnty"] = "0"; }
                    dr["qnty"] = Convert.ToDecimal(((TextBox)gvr.FindControl("txtQnty")).Text);
                }


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
            Session["ItemDtl"] = dt;
            dgConsumptionManualyDtl.DataSource = dt;
            dgConsumptionManualyDtl.DataBind();
            ShowFooterTotal();
            // ((TextBox)gvr.FindControl("txtItemRate")).Focus();
            ((TextBox)dgConsumptionManualyDtl.Rows[dgConsumptionManualyDtl.Rows.Count - 1].FindControl("txtItemDesc")).Focus();
        }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        DataTable dtitemDtl = (DataTable)Session["ItemDtl"];
        ConsumptionManualyInfo aConsumptionManualyInfo = new ConsumptionManualyInfo();
            if (!string.IsNullOrEmpty(lblmstID.Text))
            {
                if (dtitemDtl.Rows.Count > 0)
                {
                    aConsumptionManualyInfo.ConsumtionDate = txtConsumptionDate.Text;
                    aConsumptionManualyInfo.Remarks = txtRemarks.Text;
                    aConsumptionManualyInfo.LogineBy = Session["userID"].ToString();
                    DataTable OldStock = (DataTable)ViewState["OldStock"];
                    aConsumptionManualyInfo.MstID = lblmstID.Text;

                    string VCH_SYS_NO = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                           "t1.PAYEE='CM' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                           aConsumptionManualyInfo.MstID);
                    

                    ConsumptionManualyManager.UpdateConsumptionManualy(aConsumptionManualyInfo, dtitemDtl, OldStock);
                    ReFreshAll();
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been Update successfully...!!');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Rae Matrals Item Empty...!!');", true);
                }
            }

            else
            {
                if(!string.IsNullOrEmpty(txtConsumptionNo.Text))
                {
                    if (dtitemDtl.Rows.Count > 0)
                    {
                        aConsumptionManualyInfo.ConsumtionDate = txtConsumptionDate.Text;
                        aConsumptionManualyInfo.Remarks = txtRemarks.Text;
                        aConsumptionManualyInfo.LogineBy = Session["userID"].ToString();

                        //*************** Jurnal Voucher ******//
                        VouchMst vmst = new VouchMst();
                        vmst.FinMon = FinYearManager.getFinMonthByDate(txtConsumptionDate.Text);
                        vmst.ValueDate = txtConsumptionDate.Text;
                        vmst.VchCode = "03";
                        vmst.RefFileNo = "";
                        vmst.VolumeNo = "";
                        vmst.SerialNo ="";
                        vmst.Particulars = "Closing Stock : " + txtRemarks.Text.Replace("'", "");
                        //vmst.ControlAmt = txtTotalAmount.Text.Replace(",", "");
                        vmst.Payee = "CM";
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

                        ConsumptionManualyManager.SaveConsumptionManualy(aConsumptionManualyInfo, dtitemDtl, vmst);
                        ReFreshAll();
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been Save successfully...!!');", true);
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Rae Matrals Item Empty...!!');", true);
                }
        
            }
            btnSave.Enabled = true;
       

    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        ReFreshAll();
    }

    protected void dgMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {


        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {

                e.Row.Cells[3].Attributes.Add("style", "display:none");
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
    protected void dgMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblmstID.Text = dgMst.SelectedRow.Cells[3].Text;
        DataTable dt= ConsumptionManualyManager.GetConsumtionMAnualyMstInfo(lblmstID.Text);
        if(dt.Rows.Count>0)
        {
            pnlConMn.Visible = true;
            txtRemarks.Enabled = txtConsumptionDate.Enabled = true;            
            getEmptyDtl();
            btnSave.Visible = true;
            btnNew.Visible = false;
            dgMst.Visible = false;
            txtConsumptionDate.Text = dt.Rows[0]["ConsumptionDate"].ToString();
            txtConsumptionNo.Text = dt.Rows[0]["ID"].ToString();
            txtRemarks.Text = dt.Rows[0]["Remarks"].ToString();
            DataTable dtDtlGrid=ConsumptionManualyManager.GetItemDtls(lblmstID.Text);
            if (dtDtlGrid.Rows.Count>0)
            {
                btnPrint.Visible = true;
                DataRow drd = dtDtlGrid.NewRow();
                dtDtlGrid.Rows.Add(drd);
                dgConsumptionManualyDtl.DataSource = dtDtlGrid;
                dgConsumptionManualyDtl.DataBind();
                Session["ItemDtl"] = dtDtlGrid;
                ViewState["OldStock"] = dtDtlGrid;
            }
        }
    }

    protected void dgConsumptionManualyDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        
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
                e.Row.Cells[7].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[7].Attributes.Add("style", "display:none");
                //e.Row.Cells[7].Attributes.Add("style", "display:none");
                //  e.Row.Cells[6].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[7].Attributes.Add("style", "display:none");
                //e.Row.Cells[7].Attributes.Add("style", "display:none");
                // e.Row.Cells[6].Attributes.Add("style", "display:none");
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
    }
   
    protected void dgConsumptionManualyDtl_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (Session["ItemDtl"] != null)
        {
            DataTable dt = (DataTable)Session["ItemDtl"];
            DataRow dr1 = dt.Rows[dgConsumptionManualyDtl.Rows[e.RowIndex].DataItemIndex];

            //txtTotalCost.Text = (Convert.ToDouble(lblTotalCost.Text) - Convert.ToDouble(dr1["Total"].ToString())).ToString();
            //lblTotalCost.Text = txtTotalCost.Text;
            dt.Rows.Remove(dr1);
            dgConsumptionManualyDtl.DataSource = dt;
            Session["ItemDtl"] = dt;            
            dgConsumptionManualyDtl.DataBind();

            UP2.Update();
            //UP1.Update();

        }
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(lblmstID.Text))
        {
            ConsumptionManualyInfo aConsumptionManualyInfo = new ConsumptionManualyInfo();
            aConsumptionManualyInfo.LogineBy =  Session["userID"].ToString();
            ConsumptionManualyManager.DeleteConsumptionManualyInfo(aConsumptionManualyInfo, lblmstID.Text);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Record Delete Succesfully ...!!');", true);
            ReFreshAll();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Please Select Item ...!!');", true);
        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        DataTable dt = ConsumptionManualyManager.GetConsumptionManualyInfo(lblmstID.Text);
        if(dt.Rows.Count>0)
        {
            CreateConsumptionPDFReport(dt);
        }
    }
    private readonly itextFormatHeaderPhrase _itextFormatHeaderPhrase = new itextFormatHeaderPhrase();
    private void CreateConsumptionPDFReport(DataTable dt)
    {
       // string filename = ;
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=Consumption.pdf");
        Document document = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 0, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(30f);

        float[] titwidth = new float[2] { 10, 200 };
        

        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 3;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(_itextFormatHeaderPhrase.FormatHeader14_BOLD(Session["org"].ToString()));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 15f;
        dth.AddCell(cell);
        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader14_BOLD("Consumption Information"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 15f;
        dth.AddCell(cell);
        document.Add(dth);
        //****************************************

        PdfPTable dth2 = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;
        cell = new PdfPCell();
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        dth2.AddCell(cell);
        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader14_BOLD(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 15f;
        dth2.AddCell(cell);
        //**********************************************
              

        document.Add(dth2);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] titwidthDtl = new float[8] { 10, 40, 40, 40, 40, 30, 40,40 };
        PdfPTable dtl = new PdfPTable(titwidthDtl);
        dtl.WidthPercentage = 100;
        dtl.HeaderRows = 1;
        ///DataTable ddtDays = IdManager.GetShowDataTable("Select * from Days");

        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("SL."));
        // cell.Border = 1;
        cell.VerticalAlignment = 0;
        cell.HorizontalAlignment = 1;
        dtl.AddCell(cell);
        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("Consumption Date"));
        // cell.Border = 1;
        cell.VerticalAlignment = 0;
        cell.HorizontalAlignment = 1;
        dtl.AddCell(cell);

        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("Item Name"));
        // cell.Border = 1;
        cell.VerticalAlignment = 0;
        cell.HorizontalAlignment = 0;
        dtl.AddCell(cell);

        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("Code"));
        // cell.Border = 1;
        cell.VerticalAlignment = 0;
        cell.HorizontalAlignment = 1;
        dtl.AddCell(cell);
        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("Quantity"));
        // cell.Border = 1;
        cell.VerticalAlignment = 0;
        cell.HorizontalAlignment = 1;
        dtl.AddCell(cell);

        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("UOM"));
        // cell.Border = 1;
        cell.VerticalAlignment = 0;
        cell.HorizontalAlignment = 1;
        dtl.AddCell(cell);

        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("Unite Price"));
        // cell.Border = 1;
        cell.VerticalAlignment = 0;
        cell.HorizontalAlignment = 1;
        dtl.AddCell(cell);



        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("BrandName"));
        // cell.Border = 1;
        cell.VerticalAlignment = 0;
        cell.HorizontalAlignment = 1;
        dtl.AddCell(cell);
        int SL = 1;

        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(SL.ToString()));
            //cell.Border = 1;
            cell.VerticalAlignment = 0;
            cell.HorizontalAlignment = 1;
            dtl.AddCell(cell);
            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["ConsumptionDate"].ToString()));
            //cell.Border = 1;
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 0;
            dtl.AddCell(cell);
            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["item_desc"].ToString()));
            // cell.Border = 1;
            cell.VerticalAlignment = 0;
            cell.HorizontalAlignment = 1;
            dtl.AddCell(cell);
            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["item_code"].ToString()));
            // cell.Border = 1;
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["qnty"].ToString()));
            // cell.Border = 1;
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["UOM"].ToString()));
            // cell.Border = 1;
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["item_rate"].ToString()));
            // cell.Border = 1;
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["BrandName"].ToString()));
            // cell.Border = 1;
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 0;
            dtl.AddCell(cell);

            SL++;
        }


        document.Add(dtl);

        PdfPTable pdt1 = new PdfPTable(1);
        cell.FixedHeight = 10f;
        cell.Border = 0;
        pdt1.AddCell(cell);

        cell = SignatureFormat(document, cell);
        document.Close();
        Response.Flush();
        Response.End();
    }
    private static PdfPCell SignatureFormat(Document document, PdfPCell cell)
    {
        float[] widtl = new float[5] { 20, 20, 20, 20, 20 };
        PdfPTable pdtsig = new PdfPTable(widtl);
        pdtsig.WidthPercentage = 100;
        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 5;
        cell.FixedHeight = 40f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);


        cell = new PdfPCell(FormatPhrase("Prepared by"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Checked by"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Authorised by"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);
        document.Add(pdtsig);
        return cell;
    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
}