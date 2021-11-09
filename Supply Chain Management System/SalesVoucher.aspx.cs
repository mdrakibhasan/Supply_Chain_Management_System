using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using autouniv;
using OldColor;

public partial class SalesVoucher : System.Web.UI.Page
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
                    cmd.CommandText = "Select user_grp,description from utl_userinfo where upper(user_name)=upper('" + Session["user"].ToString().ToUpper() + "') and status='A'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                    string wnot = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            //Session["dept"] = dReader["dept"].ToString();
                            wnot = "Welcome Mr. " + dReader["description"].ToString();
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
        if (!IsPostBack)
        {
            txtDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
            txtInvoiceNo.Text = IdManager.GetDateTimeWiseSerial("INV", "InvoiceNo", "[Order]");
            Panel1.Visible = txtItemsCode.Enabled = false;
            dgSVMst.Visible = true;
            
            string queryLoc = "SELECT [ID],[ContactName] FROM [Customer] order by ID";
            util.PopulationDropDownList(ddlCustomer, "Customer", queryLoc, "ContactName", "ID");
            ddlCustomer.Items.Insert(0,"");
            string query2 = "select '' [bank_id],'' [bank_name]  union select [bank_id] ,[bank_name] from [bank_info] order by 1";
            util.PopulationDropDownList(ddlBank, "bank_info", query2, "bank_name", "bank_id");

            Session["Cash_Code"] = IdManager.GetShowSingleValueString("CASH_CODE", "BOOK_NAME", "GL_SET_OF_BOOKS", "AMB");
            DataTable dt= SalesManager.GetShowSalesDetails();
            dgSVMst.DataSource =dt;
            Session["SvMst"] = dt;
            dgSVMst.DataBind();
            VisiblePayment(false, false, false, false, false, false, false, false);
            btnDelete.Enabled = btnSave.Enabled = true;
            btnNew.Visible = true;
        }
    }
    private void getEmptyDtl()
    {
       
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ID", typeof(string));
        dtDtlGrid.Columns.Add("Code", typeof(string));
        dtDtlGrid.Columns.Add("Name", typeof(string));
        dtDtlGrid.Columns.Add("Tax", typeof(string));
        dtDtlGrid.Columns.Add("DiscountAmount", typeof(string));
        dtDtlGrid.Columns.Add("SPrice", typeof(string));
        dtDtlGrid.Columns.Add("Qty", typeof(string));
        dtDtlGrid.Columns.Add("Total", typeof(string));
        dtDtlGrid.Columns.Add("ClosingStock", typeof(string));
        DataRow dr = dtDtlGrid.NewRow();
        dr["Tax"] = 0;
        dr["DiscountAmount"] = 0;
        dr["SPrice"] = 0;
        dr["Qty"] = 0;
        dr["Total"] = 0;
        dr["ClosingStock"] = 0;
        dtDtlGrid.Rows.Add(dr);
        dgSV.DataSource = dtDtlGrid;        
        dgSV.DataBind();        
    }
    protected void txtCode_TextChanged(object sender, EventArgs e)
    {
        bool IsChk = false;
        DataTable dt = SalesManager.GetShowItemsInformation(txtItemsCode.Text.Trim());
        DataTable DT1 = new DataTable();        
        DT1 =(DataTable) ViewState["SV"];
        if (DT1 == null)
        {
            DT1 = dt;
        }
        else
        {
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in DT1.Rows)
                {
                    if (dr["Code"].ToString() == dt.Rows[0]["Code"].ToString())
                    {
                        IsChk = true;
                        break;
                    }                  
                }
                if (IsChk == false)
                { DT1.ImportRow(dt.Rows[0]); }
            }
        }
        dgSV.DataSource = DT1;
        ViewState["SV"] = DT1;
        dgSV.DataBind();
        ShowFooterTotal( DT1);
        txtItemsCode.Text = "";
        UPCustomer.Update();
        UpSearch.Update();
        txtItemsCode.Focus();
    }
    private void ShowFooterTotal(DataTable DT1)
    {
        decimal totVat = 0; decimal totDis = 0; decimal Qty = 0; decimal tot = 0;
        foreach (DataRow dr in DT1.Rows)
        {
            totVat += Convert.ToDecimal(txtVat.Text);
            totDis += Convert.ToDecimal(dr["DiscountAmount"]);
            Qty += Convert.ToDecimal(dr["Qty"]);
            tot += Convert.ToDecimal(dr["Total"]);
        }
        txtSubTotal.Text = tot.ToString();
        txtVat.Text = totVat.ToString();     
        txtDue.Text = (tot + ((tot * totVat) / 100)).ToString("F");
    }
    protected void dgPVMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Attributes.Add("style", "display:none");
                e.Row.Cells[9].Attributes.Add("style", "display:none");
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
    protected void dgSV_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["SV"] != null)
        {
            DataTable dtDtlGrid = (DataTable)ViewState["SV"];
            dtDtlGrid.Rows.RemoveAt(dgSV.Rows[e.RowIndex].DataItemIndex);
            if (dtDtlGrid.Rows.Count == 0)
            { DataRow dr = dtDtlGrid.NewRow();
                dr["Tax"]=0;
                dr["DiscountAmount"]=0;
                dr["Qty"]=0;
                dr["Total"]=0;
                dtDtlGrid.Rows.Add(dr); }
            dgSV.DataSource = dtDtlGrid;
            dgSV.DataBind();
            ShowFooterTotal(dtDtlGrid);          
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Your session is over. Try it again!!');", true);
        }
    }
    protected void txtQty_TextChanged(object sender, EventArgs e)
    {
        try
        {
            Quantity(sender);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
    protected void txtSalesPrice_TextChanged(object sender, EventArgs e)
    {
        try
        {
            Quantity(sender);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
    private void Quantity(object sender)
    {        
        double tot = 0;
        double dis = 0;
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dt = (DataTable)ViewState["SV"];
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            if (Convert.ToDouble(dr["ClosingStock"]) < Convert.ToDouble(((TextBox)gvr.FindControl("txtQty")).Text))
            {
                string Mgs = "Items Quantity Over This Closing Quantity.\\n Tolat Closing Qiantity:-(" + dr["ClosingStock"] + ")..!!";
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + Mgs + "');", true);
            }
            else
            {
                dr["SPrice"] = ((TextBox)gvr.FindControl("txtSalesPrice")).Text;
                dr["Qty"] = ((TextBox)gvr.FindControl("txtQty")).Text;
                dr["DiscountAmount"] = ((TextBox)gvr.FindControl("txtDiscount")).Text;
                tot = Convert.ToDouble(((TextBox)gvr.FindControl("txtSalesPrice")).Text) * Convert.ToDouble(((TextBox)gvr.FindControl("txtQty")).Text);
                dis = ((Convert.ToDouble(((TextBox)gvr.FindControl("txtSalesPrice")).Text) * (Convert.ToDouble(((TextBox)gvr.FindControl("txtDiscount")).Text) * Convert.ToDouble(((TextBox)gvr.FindControl("txtQty")).Text))) / 100);
                dr["Total"] = (tot - dis).ToString("N2");
            }
        }
        dgSV.DataSource = dt;
        dgSV.DataBind();
        ShowFooterTotal(dt);
        ((TextBox)dgSV.Rows[dgSV.Rows.Count - 1].FindControl("txtQty")).Focus();       
        
    }
    protected void txtDiscount_TextChanged(object sender, EventArgs e)
    {
        try
        {
            Quantity(sender);
            ((TextBox)dgSV.Rows[dgSV.Rows.Count - 1].FindControl("txtQty")).Focus();
        
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {
        txtDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
        txtInvoiceNo.Text = IdManager.GetDateTimeWiseSerial("INV", "InvoiceNo", "[Order]");
        Panel1.Visible = txtItemsCode.Enabled = false;
        dgSVMst.Visible = true;
        txtVat.Text = IdManager.GetShowSingleValueString("Rate", "ID", "TaxCategory", "1");

        string queryLoc = "SELECT [ID],[ContactName] FROM [Customer] order by ID";
        util.PopulationDropDownList(ddlCustomer, "CostType", queryLoc, "ContactName", "ID");
        ddlCustomer.Items.Insert(0, "");
        string query2 = "select '' [bank_id],'' [bank_name]  union select [bank_id] ,[bank_name] from [bank_info] order by 1";
        util.PopulationDropDownList(ddlBank, "bank_info", query2, "bank_name", "bank_id");

        Session["Cash_Code"] = IdManager.GetShowSingleValueString("CASH_CODE", "BOOK_NAME", "GL_SET_OF_BOOKS", "AMB");
        dgSVMst.DataSource = SalesManager.GetShowSalesDetails();
        dgSVMst.DataBind();
        Clear();
        btnNew.Visible = false;
        UPCustomer.Update();
    }
    protected void btnFind_Click(object sender, EventArgs e)
    {
       
        //lblInvNo.Text = dgSVMst.SelectedRow.Cells[6].Text.Trim();
        Sales aSales = SalesManager.GetShowSalesInfo(lblInvNo.Text);
        if (aSales != null)
        {
            Clear();            
            txtInvoiceNo.Text = aSales.Invoice;
            txtDate.Text = aSales.Date;
            txtSubTotal.Text = aSales.Total;
            txtVat.Text = aSales.Tax;
            txtDiscount.Text = aSales.Disount;
            txtSubTotal.Text = aSales.GTotal;
            txtPayment.Text = aSales.CReceive;
            //ddlCustomer.SelectedValue = aSales.Customer;
            Session["Customer_COA"] = IdManager.GetShowSingleValueString("Code", "ID", "Customer", ddlCustomer.SelectedValue);
            ddlDelevery.SelectedValue = aSales.DvStatus;
            txtDeleveryDate.Text = aSales.DvDate;
            txtRemarks.Text = aSales.Remarks;
            ddlPaymentMethord.SelectedValue = aSales.PMethod;
            if (aSales.PMethod != "C")
            {
                txtChequeNo.Text = aSales.PMNumber;
                ddlBank.SelectedValue = aSales.BankId;
                txtChequeDate.Text = aSales.ChequeDate;
                txtChequeAmount.Text = aSales.ChequeAmount;
            }
            DataTable DT1 = SalesManager.GetSalesDetails(lblInvNo.Text);
            if (DT1.Rows.Count > 0)
            {
                dgSV.DataSource = DT1;
                ViewState["SV"] = DT1;
                dgSV.DataBind();
                ShowFooterTotal(DT1);
            }
            txtDue.Text = Convert.ToDouble(aSales.Due).ToString("N2");
            btnNew.Visible = false;
        }
    }
    private void Clear()
    {
        Panel1.Visible = txtItemsCode.Enabled = true;
        txtDeleveryDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
        dgSVMst.Visible = false;
        txtSubTotal.Text =  txtDiscount.Text = txtPayment.Text = txtDue.Text = txtChequeAmount.Text = "0";
        ddlCustomer.SelectedIndex = ddlDelevery.SelectedIndex = ddlBank.SelectedIndex = ddlPaymentMethord.SelectedIndex = -1;
        txtChequeNo.Text = txtChequeDate.Text = txtRemarks.Text = "";
        getEmptyDtl();
        ViewState["SV"] = null;
        txtItemsCode.Focus();
    }
    //protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    Session["Customer_COA"] = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Customer", ddlCustomer.SelectedValue);
    //}
    protected void btnSave_Click(object sender, EventArgs e)
    {
        double Dis = 0;
        if (Panel1.Visible == false) { }
        else
        {
            Sales aSales = SalesManager.GetShowSalesInfo(lblInvNo.Text);
            Dis = (Convert.ToDouble(txtSubTotal.Text) * Convert.ToDouble(txtDiscount.Text)) / 100;
            UPPaymentMtd.Update();
            if (aSales != null)
            {
                if (per.AllowEdit == "Y")
                {
                    aSales.ID = lblInvNo.Text;
                    aSales.Invoice = txtInvoiceNo.Text;
                    aSales.Date = txtDate.Text;
                    aSales.Total = txtSubTotal.Text.Replace("'", "");
                    aSales.Tax = txtVat.Text.Replace(",", "");
                    aSales.Disount = txtDiscount.Text.Replace(",", "");
                    aSales.GTotal = (Convert.ToDouble(txtSubTotal.Text) - Dis).ToString().Replace(",", "");
                    aSales.CReceive = txtPayment.Text.Replace(",", "");
                    aSales.Due = txtDue.Text.Replace(",", "");
                    aSales.Customer = ddlCustomer.SelectedValue;
                    aSales.DvStatus = ddlDelevery.SelectedValue;
                    aSales.DvDate = txtDate.Text;
                    aSales.Remarks = txtRemarks.Text;
                    aSales.PMethod = ddlPaymentMethord.SelectedValue;
                    aSales.PMNumber = txtChequeNo.Text;
                    aSales.BankId = ddlBank.SelectedValue;
                    aSales.ChequeDate = txtChequeDate.Text;
                    aSales.ChequeAmount = txtChequeAmount.Text.Replace(",", "");
                    aSales.Chk_Status = ddlChequeStatus.SelectedValue;
                    aSales.LoginBy = Session["user"].ToString();
                    if (string.IsNullOrEmpty(ddlCustomer.SelectedValue))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Please Select Customer...!!');", true);
                    }
                    else
                    {
                        DataTable dt = (DataTable)ViewState["SV"];
                        SalesManager.UpdateSalesInfo(aSales, dt);
                        //SV_Acc_JurnalVoucher_Update();
                        //CV_Acc_CreaditVoucher_Update();
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been update successfully...!!');", true);
                        btnSave.Enabled = false;
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                }
            }
            else
            {
                if (ViewState["SV"] == null)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('No Items In this list...!!');", true);
                }
                else if (ddlPaymentMethord.SelectedValue == "Q" && ddlChequeStatus.SelectedItem.Text == "") { ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Select Cheque Status..!!');", true); }
                else
                {
                    if (per.AllowAdd == "Y")
                    {
                        aSales = new Sales();
                        aSales.Invoice = txtInvoiceNo.Text;
                        aSales.Date = txtDate.Text;
                        aSales.Total = txtSubTotal.Text.Replace(",", "");
                        aSales.Tax = txtVat.Text.Replace(",", "");
                        aSales.Disount = txtDiscount.Text.Replace(",", "");
                        aSales.GTotal = (Convert.ToDouble(txtSubTotal.Text) - Dis).ToString().Replace(",", "");
                        aSales.CReceive = txtPayment.Text.Replace(",", "");
                        aSales.Due = txtDue.Text.Replace(",", "");
                        try
                        {
                            if (ddlCustomer.SelectedItem.Text == "")
                            {
                                DataTable dt1 = clsClientInfoManager.GetCommonClient();
                                ViewState["ComDt"] = dt1;
                                aSales.Customer = dt1.Rows[0]["ID"].ToString();
                            }
                            else { aSales.Customer = ddlCustomer.SelectedValue; }
                        }
                        catch
                        {
                        }
                        aSales.DvStatus = ddlDelevery.SelectedValue;
                        aSales.DvDate = txtDate.Text;
                        aSales.Remarks = txtRemarks.Text;
                        aSales.PMethod = ddlPaymentMethord.SelectedValue;
                        aSales.PMNumber = txtChequeNo.Text;
                        aSales.BankId = ddlBank.SelectedValue;
                        aSales.ChequeDate = txtChequeDate.Text;
                        aSales.ChequeAmount = txtChequeAmount.Text.Replace(",", "");
                        aSales.Chk_Status = ddlChequeStatus.SelectedValue;
                        aSales.LoginBy = Session["user"].ToString();
                        DataTable dt = (DataTable)ViewState["SV"];

                        if (string.IsNullOrEmpty(ddlCustomer.SelectedValue))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Please Select Customer...!!');", true);
                        }
                        else
                        {
                            SalesManager.SaveSalesInfo(aSales, dt);
                            //SV_Acc_JurnalVoucher_Save();
                            //CV_Acc_CreaditVoucher_Save();
                            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been saved successfully...!!');", true);
                            btnSave.Enabled = false;
                            lblInvNo.Text = IdManager.GetShowSingleValueIntNotParameter("top(1)[ID]", "[Order] order by ID desc").ToString();
                        }
                        //try
                        //{
                        //    if (txtMobilenumber.Text != "")
                        //    {
                        //        if (txtMobilenumber.Text.Length > 10)
                        //        {
                        //            SMSGEtWay aa = new SMSGEtWay();
                        //            string Year = DateTime.Now.Year.ToString("");
                        //            string TotalAmount = (Convert.ToDouble(txtPayment.Text) * 1 + 0).ToString().Replace(" ", "").Replace(",", "");
                        //            string Invoice = "Dear Sir/Madam,Thanks for shopping with Dorjibari.Invoice:" + txtInvoiceNo.Text + "  Amount :" + TotalAmount + " Thank you for Shopping With Us. ";
                        //            int ln = (int)Invoice.Length;

                        //            aa.SampleTestHttpApi(txtMobilenumber.Text, Invoice, "netsoft", "Netsoft001$");
                        //        }
                        //    }
                        //}
                        //catch
                        //{
                        //}
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                    }
                }
            }

        }

        btnSave.Enabled = true;
    }
    string SalesCode = "1-5010001";
    string VatCoa = "1-5010002";
    double Dis = 0;

    //private void SV_Acc_JurnalVoucher_Update()
    //{
    //    string SVSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='SV' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtInvoiceNo.Text);
    //    VouchMst vmst = VouchManager.GetVouchMst(SVSerial.Trim());
    //    if (vmst != null)
    //    {
    //        if (Convert.ToDouble(txtSubTotal.Text) > 0 && ddlCustomer.SelectedItem.Text != "")
    //        {
    //            Dis = (Convert.ToDouble(txtSubTotal.Text) * Convert.ToDouble(txtDiscount.Text)) / 100;
    //            int count = IdManager.GetShowSingleValueIntTowParameter("COUNT(*)", "ID", "Customer", "ID=" + ddlCustomer.SelectedValue + " and CommonCus='1'");
    //            if (count <= 0)
    //            {                    
    //                vmst.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
    //                vmst.ValueDate = txtDate.Text;
    //                vmst.VchCode = "03";
    //                vmst.RefFileNo = "";
    //                vmst.VolumeNo = "";
    //                vmst.SerialNo = txtInvoiceNo.Text.Trim();
    //                vmst.Particulars = "Sales Payable Amount. - (" + txtInvoiceNo.Text + "-" + ddlCustomer.SelectedItem.Text + ")";
    //                vmst.ControlAmt = (Convert.ToDouble(txtSubTotal.Text) - Dis).ToString().Replace(",","");
    //                vmst.Payee = "SV";
    //                vmst.CheckNo = txtChequeNo.Text;
    //                vmst.CheqDate = txtChequeDate.Text;
    //                vmst.CheqAmnt = txtChequeAmount.Text.Replace(",","");
    //                vmst.MoneyRptNo = "";
    //                vmst.MoneyRptDate = "";                                                    
    //                vmst.UpdateUser = Session["user"].ToString();
    //                vmst.UpdateDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");                   
    //                vmst.AuthoUserType = Session["userlevel"].ToString();
    //                VouchManager.UpdateVouchMst(vmst);
    //                VouchManager.DeleteVouchDtl(vmst.VchSysNo);
    //                VouchDtl vdtl;
    //                for (int j = 0; j < 2; j++)
    //                {
    //                    if (j == 0)
    //                    {
    //                        vdtl = new VouchDtl();
    //                        vdtl.VchSysNo = vmst.VchSysNo;
    //                        vdtl.ValueDate = txtDate.Text;
    //                        vdtl.LineNo = "1";
    //                        vdtl.GlCoaCode = "1-" + Session["Customer_COA"].ToString();
    //                        vdtl.Particulars = "On Customer - " + ddlCustomer.SelectedItem.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Customer_COA"].ToString());
    //                        vdtl.AmountDr = (Convert.ToDouble(txtSubTotal.Text) - Dis).ToString().Replace(",","");
    //                        vdtl.AmountCr = "0";
    //                        vdtl.Status = vmst.Status;
    //                        vdtl.BookName = "AMB";
    //                        VouchManager.CreateVouchDtl(vdtl);
    //                    }
    //                    else if (j == 1)
    //                    {
    //                        vdtl = new VouchDtl();
    //                        vdtl.VchSysNo = vmst.VchSysNo;
    //                        vdtl.ValueDate = txtDate.Text;
    //                        vdtl.LineNo = "2";
    //                        vdtl.GlCoaCode = SalesCode; //**** Sales Code *******//
    //                        vdtl.Particulars = "Accounts Receivable Amount(Sales)";
    //                        vdtl.AccType = VouchManager.getAccType(SalesCode); //**** Sales Code *******//
    //                        vdtl.AmountDr = "0";
    //                        vdtl.AmountCr = (Convert.ToDouble(txtSubTotal.Text) - Dis).ToString().Replace(",","");
    //                        vdtl.Status = vmst.Status;
    //                        vdtl.BookName = "AMB";
    //                        VouchManager.CreateVouchDtl(vdtl);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
    //private void CV_Acc_CreaditVoucher_Update()
    //{
    //    double totVat = 0;
    //    string CusName = "";
    //    string GlCoa = "";
    //    totVat = (Convert.ToDouble(txtSubTotal.Text) * Convert.ToDouble(txtVat.Text)) / 100;
    //    if (Convert.ToDouble(txtSubTotal.Text) > 0)
    //    {
    //        if ((ddlPaymentMethord.SelectedValue == "C" && Convert.ToDouble(txtPayment.Text) > 0) || (ddlPaymentMethord.SelectedValue == "Q" && ddlChequeStatus.SelectedValue == "A" && Convert.ToDouble(txtPayment.Text) > 0))
    //        {
    //            if (ddlCustomer.SelectedItem.Text == "")
    //            {
    //                DataTable dt = (DataTable)ViewState["ComDt"];
    //                CusName = dt.Rows[0]["ContactName"].ToString();
    //                GlCoa = dt.Rows[0]["Gl_CoaCode"].ToString();
    //            }
    //            else
    //            {
    //                CusName = ddlCustomer.SelectedItem.Text;
    //                GlCoa = Session["Customer_COA"].ToString();
    //            }
    //            string SVSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='SV' and SUBSTRING(t1.VCH_REF_NO,1,2)='CV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtInvoiceNo.Text);
    //            VouchMst vmst = VouchManager.GetVouchMst(SVSerial.Trim());
    //            if (vmst != null)
    //            {
    //                vmst.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
    //                vmst.ValueDate = txtDate.Text;
    //                vmst.VchCode = "02";
    //                vmst.RefFileNo = "";
    //                vmst.VolumeNo = "";
    //                vmst.SerialNo = txtInvoiceNo.Text;
    //                vmst.Particulars = "Amount receive for selling of Customer - " + CusName;
    //                vmst.ControlAmt = (Convert.ToDouble(txtPayment.Text) - totVat).ToString("N2").Replace(",", "");
    //                vmst.Payee = "SV";
    //                vmst.CheckNo = txtChequeNo.Text;
    //                vmst.CheqDate = txtChequeDate.Text;
    //                vmst.CheqAmnt = txtChequeAmount.Text.Replace(",", "");
    //                vmst.BookName = Session["book"].ToString();
    //                vmst.UpdateUser = Session["user"].ToString();
    //                vmst.UpdateDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //                //vmst.Status = "A";
    //                vmst.AuthoUserType = Session["userlevel"].ToString();
    //                VouchManager.UpdateVouchMst(vmst);
    //                VouchManager.DeleteVouchDtl(vmst.VchSysNo);
    //                VouchDtl vdtl;
    //                for (int i = 0; i < 2; i++)
    //                {
    //                    if (i == 0)
    //                    {
    //                        vdtl = new VouchDtl();
    //                        vdtl.VchSysNo = vmst.VchSysNo;
    //                        vdtl.ValueDate = txtDate.Text;
    //                        vdtl.LineNo = "1";
    //                        if (ddlPaymentMethord.SelectedValue != "C")
    //                        {
    //                            string BankCOA = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    //                            vdtl.GlCoaCode = "1-" + BankCOA;
    //                            vdtl.Particulars = "Cash at Bank: Sales Amount Receive of -" + txtInvoiceNo.Text + "";
    //                            vdtl.AccType = VouchManager.getAccType("1-" + BankCOA);
    //                        }
    //                        else
    //                        {
    //                            vdtl.GlCoaCode = "1-" + Session["Cash_Code"];
    //                            vdtl.Particulars = "Cash in Hand: Sales Amount Receive of -" + txtInvoiceNo.Text + "";
    //                            vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"]);
    //                        }

    //                        vdtl.AmountDr = (Convert.ToDouble(txtPayment.Text) - totVat).ToString("N2").Replace(",", "");
    //                        vdtl.AmountCr = "0";
    //                        vdtl.Status = vmst.Status;
    //                        vdtl.BookName = Session["book"].ToString();
    //                        VouchManager.CreateVouchDtl(vdtl);
    //                    }
    //                    else if (i == 1)
    //                    {
    //                        vdtl = new VouchDtl();
    //                        vdtl.VchSysNo = vmst.VchSysNo;
    //                        vdtl.ValueDate = txtDate.Text;
    //                        vdtl.LineNo = "2";
    //                        vdtl.GlCoaCode = "1-" + GlCoa;
    //                        vdtl.Particulars = "On Customer Bill pay (" + GlCoa + "-" + CusName + ")";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + GlCoa);
    //                        vdtl.AmountDr = "0";
    //                        vdtl.AmountCr = (Convert.ToDouble(txtPayment.Text) - totVat).ToString("N2").Replace(",", "");
    //                        vdtl.Status = vmst.Status;
    //                        vdtl.BookName = Session["book"].ToString();
    //                        VouchManager.CreateVouchDtl(vdtl);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    //**************************************** Start Vat Charge ***************************//

    //    if (Convert.ToDouble(txtVat.Text) > 0)
    //    {
    //        double tVat = 0;
    //        tVat = (Convert.ToDouble(txtSubTotal.Text) * Convert.ToDouble(txtVat.Text)) / 100;
    //        string SVSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='Vat' and SUBSTRING(t1.VCH_REF_NO,1,2)='CV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtInvoiceNo.Text);
    //        VouchMst vmst = VouchManager.GetVouchMst(SVSerial.Trim());
    //        if (vmst != null)
    //        {                
    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
    //            vmst.ValueDate = txtDate.Text;
    //            vmst.VchCode = "02";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtInvoiceNo.Text;
    //            vmst.Particulars = "Vat Amount Receive of -(" + txtInvoiceNo.Text + ")";
    //            vmst.ControlAmt = tVat.ToString().Replace(",","");
    //            vmst.Payee = "Vat";               
    //            vmst.BookName = Session["book"].ToString();
    //            vmst.EntryUser = Session["user"].ToString();
    //            vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");                
    //            //vmst.Status = "A";
    //            vmst.AuthoUserType = Session["userlevel"].ToString();
    //            VouchManager.UpdateVouchMst(vmst);
    //            VouchManager.DeleteVouchDtl(vmst.VchSysNo);
    //            VouchDtl vdtl;
    //            for (int i = 0; i < 2; i++)
    //            {
    //                if (i == 0)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtDate.Text;
    //                    vdtl.LineNo = "1";
    //                    if (ddlPaymentMethord.SelectedValue != "C")
    //                    {
    //                        string BankCOA = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    //                        vdtl.GlCoaCode = "1-" + BankCOA;
    //                        vdtl.Particulars = "Cash at Bank: Vat Amount Receive of -" + txtInvoiceNo.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + BankCOA);
    //                    }
    //                    else
    //                    {
    //                        vdtl.GlCoaCode = "1-" + Session["Cash_Code"];
    //                        vdtl.Particulars = "Cash in Hand: Vat Amount Receive of -" + txtInvoiceNo.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"]);
    //                    }

    //                    vdtl.AmountDr = tVat.ToString().Replace(",",""); ;
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                else if (i == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtDate.Text;
    //                    vdtl.LineNo = "2";
    //                    vdtl.GlCoaCode = VatCoa;
    //                    vdtl.Particulars = "On Vat pay (" + VatCoa + ")";
    //                    vdtl.AccType = VouchManager.getAccType(VatCoa);
    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = tVat.ToString().Replace(",",""); ;
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }
    //}
    //private void SV_Acc_JurnalVoucher_Save()
    //{
    //    if (Convert.ToDouble(txtSubTotal.Text) > 0 && ddlCustomer.SelectedItem.Text != "")
    //    {            
    //        Dis = (Convert.ToDouble(txtSubTotal.Text) * Convert.ToDouble(txtDiscount.Text)) / 100;         
    //        int count = IdManager.GetShowSingleValueIntTowParameter("COUNT(*)", "ID", "Customer", "ID=" + ddlCustomer.SelectedValue + " and CommonCus='1'");
    //        if (count <= 0)
    //        {
    //            VouchMst vmst = new VouchMst();
    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
    //            vmst.ValueDate = txtDate.Text;
    //            vmst.VchCode = "03";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtInvoiceNo.Text.Trim();
    //            vmst.Particulars = "Sales Payable Amount. - (" + txtInvoiceNo.Text + "-" + ddlCustomer.SelectedItem.Text + ")";
    //            vmst.ControlAmt = (Convert.ToDouble(txtSubTotal.Text) - Dis).ToString().Replace(",","");
    //            vmst.Payee = "SV";
    //            vmst.CheckNo = txtChequeNo.Text;
    //            vmst.CheqDate = txtChequeDate.Text;
    //            vmst.CheqAmnt = txtChequeAmount.Text.Replace(",","");
    //            vmst.MoneyRptNo = "";
    //            vmst.MoneyRptDate = "";
    //            vmst.TransType = "R";
    //            vmst.BookName = "AMB";
    //            vmst.EntryUser = Session["user"].ToString();
    //            vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //            vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
    //            vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
    //            vmst.Status = "A";
    //            vmst.AuthoUserType = Session["userlevel"].ToString();
    //            VouchManager.CreateVouchMst(vmst);               
    //            VouchDtl vdtl;
    //            for (int j = 0; j < 2; j++)
    //            {
    //                if (j == 0)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtDate.Text;
    //                    vdtl.LineNo = "1";
    //                    vdtl.GlCoaCode = "1-" + Session["Customer_COA"].ToString();
    //                    vdtl.Particulars = "On Customer - " + ddlCustomer.SelectedItem.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + Session["Customer_COA"].ToString());
    //                    vdtl.AmountDr = (Convert.ToDouble(txtSubTotal.Text) - Dis).ToString().Replace(",","");
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = "A";
    //                    vdtl.BookName = "AMB";
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                else if (j == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtDate.Text;
    //                    vdtl.LineNo = "2";
    //                    vdtl.GlCoaCode = SalesCode; //**** Sales Code *******//
    //                    vdtl.Particulars = "Accounts Receivable Amount(Sales)";
    //                    vdtl.AccType = VouchManager.getAccType(SalesCode); //**** Sales Code *******//
    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = (Convert.ToDouble(txtSubTotal.Text) - Dis).ToString().Replace(",","");
    //                    vdtl.Status = "A";
    //                    vdtl.BookName = "AMB";
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }
    //}
    //private void CV_Acc_CreaditVoucher_Save()
    //{
    //    double totVat = 0;
    //    string CusName = "";
    //    string GlCoa = "";

    //    if (Convert.ToDouble(txtSubTotal.Text) > 0)
    //    {
    //        totVat = (Convert.ToDouble(txtSubTotal.Text) * Convert.ToDouble(txtVat.Text)) / 100;
    //        if ((ddlPaymentMethord.SelectedValue == "C" && Convert.ToDouble(txtPayment.Text) > 0) || (ddlPaymentMethord.SelectedValue == "Q" && ddlChequeStatus.SelectedValue == "A" && Convert.ToDouble(txtPayment.Text) > 0))
    //        {
    //            if (ddlCustomer.SelectedItem.Text == "")
    //            {
    //                DataTable dt = (DataTable)ViewState["ComDt"];
    //                CusName = dt.Rows[0]["ContactName"].ToString();
    //                GlCoa = dt.Rows[0]["Gl_CoaCode"].ToString();
    //            }
    //            else
    //            {
    //                CusName = ddlCustomer.SelectedItem.Text;
    //                GlCoa = Session["Customer_COA"].ToString();
    //            }
    //            VouchMst vmst = new VouchMst();
    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
    //            vmst.ValueDate = txtDate.Text;
    //            vmst.VchCode = "02";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtInvoiceNo.Text;
    //            vmst.Particulars = "Amount receive for selling of Customer - " + CusName;
    //            vmst.ControlAmt = (Convert.ToDouble(txtPayment.Text) - totVat).ToString("N2").Replace("'", "");
    //            vmst.Payee = "SV";
    //            vmst.CheckNo = txtChequeNo.Text;
    //            vmst.CheqDate = txtChequeDate.Text;
    //            vmst.CheqAmnt = txtChequeAmount.Text.Replace(",", "");
    //            vmst.MoneyRptNo = "";
    //            vmst.MoneyRptDate = "";
    //            vmst.TransType = "R";
    //            vmst.BookName = Session["book"].ToString();
    //            vmst.EntryUser = Session["user"].ToString();
    //            vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //            vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
    //            vmst.VchRefNo = "CV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
    //            vmst.Status = "U";
    //            vmst.AuthoUserType = Session["userlevel"].ToString();
    //            VouchManager.CreateVouchMst(vmst);
    //            VouchDtl vdtl;
    //            for (int i = 0; i < 2; i++)
    //            {
    //                if (i == 0)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtDate.Text;
    //                    vdtl.LineNo = "1";
    //                    if (ddlPaymentMethord.SelectedValue != "C")
    //                    {
    //                        string BankCOA = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    //                        vdtl.GlCoaCode = "1-" + BankCOA;
    //                        vdtl.Particulars = "Cash at Bank: Sales Amount Receive of -" + txtInvoiceNo.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + BankCOA);
    //                    }
    //                    else
    //                    {
    //                        vdtl.GlCoaCode = "1-" + Session["Cash_Code"];
    //                        vdtl.Particulars = "Cash in Hand: Sales Amount Receive of -" + txtInvoiceNo.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"]);
    //                    }

    //                    vdtl.AmountDr = (Convert.ToDouble(txtPayment.Text) - totVat).ToString("N2").Replace(",", "");
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = "U";
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                else if (i == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtDate.Text;
    //                    vdtl.LineNo = "2";
    //                    vdtl.GlCoaCode = "1-" + GlCoa;
    //                    vdtl.Particulars = "On Customer Bill pay (" + GlCoa + "-" + CusName + ")";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + GlCoa);
    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = (Convert.ToDouble(txtPayment.Text) - totVat).ToString("N2").Replace(",", "");
    //                    vdtl.Status = "U";
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }

    //    //**************************************** Start Vat Charge ***************************//
        
    //    if (Convert.ToDouble(txtVat.Text) > 0)
    //    {
    //        double tVat = 0;
    //        tVat = (Convert.ToDouble(txtSubTotal.Text) * Convert.ToDouble(txtVat.Text)) / 100;
    //        VouchMst vmst = new VouchMst();
    //        vmst.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
    //        vmst.ValueDate = txtDate.Text;
    //        vmst.VchCode = "02";
    //        vmst.RefFileNo = "";
    //        vmst.VolumeNo = "";
    //        vmst.SerialNo = txtInvoiceNo.Text;
    //        vmst.Particulars = "Vat Amount Receive of -(" + txtInvoiceNo.Text + ")";
    //        vmst.ControlAmt = tVat.ToString().Replace(",","");
    //        vmst.Payee = "Vat";
    //        vmst.CheckNo = "";
    //        vmst.CheqDate = "";
    //        vmst.CheqAmnt = "";
    //        vmst.MoneyRptNo = "";
    //        vmst.MoneyRptDate = "";
    //        vmst.TransType = "R";
    //        vmst.BookName = Session["book"].ToString();
    //        vmst.EntryUser = Session["user"].ToString();
    //        vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //        vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
    //        vmst.VchRefNo = "CV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
    //        vmst.Status = "A";
    //        vmst.AuthoUserType = "4";
    //        VouchManager.CreateVouchMst(vmst);
           
    //        VouchDtl vdtl;
    //        for (int i = 0; i < 2; i++)
    //        {
    //            if (i == 0)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtDate.Text;
    //                vdtl.LineNo = "1";
    //                if (ddlPaymentMethord.SelectedValue != "C")
    //                {
    //                    string BankCOA = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    //                    vdtl.GlCoaCode = "1-" + BankCOA;
    //                    vdtl.Particulars = "Cash at Bank: Vat Amount Receive of -" + txtInvoiceNo.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + BankCOA);
    //                }
    //                else
    //                {
    //                    vdtl.GlCoaCode = "1-" + Session["Cash_Code"];
    //                    vdtl.Particulars = "Cash in Hand: Vat Amount Receive of -" + txtInvoiceNo.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"]);
    //                }

    //                vdtl.AmountDr = tVat.ToString().Replace(",",""); ;
    //                vdtl.AmountCr = "0";
    //                vdtl.Status = "A";
    //                vdtl.BookName = Session["book"].ToString();
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //            else if (i == 1)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtDate.Text;
    //                vdtl.LineNo = "2";
    //                vdtl.GlCoaCode = VatCoa;
    //                vdtl.Particulars = "On Vat pay (" + VatCoa + ")";
    //                vdtl.AccType = VouchManager.getAccType(VatCoa);
    //                vdtl.AmountDr = "0";
    //                vdtl.AmountCr = tVat.ToString().Replace(",",""); ;
    //                vdtl.Status = "A";
    //                vdtl.BookName = Session["book"].ToString();
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //        }
    //    }
    //}
    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect("SalesVoucher.aspx?mno=5.20");
    }

    protected void dgSVMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[4].Attributes.Add("style", "display:none");
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
    protected void dgSVMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblInvNo.Text = dgSVMst.SelectedRow.Cells[4].Text.Trim();
        btnFind_Click(sender,e);
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            Sales aSales = SalesManager.GetShowSalesInfo(lblInvNo.Text);
            if (aSales != null)
            {
                aSales.ID = lblInvNo.Text;
                aSales.Invoice = txtInvoiceNo.Text;
                SalesManager.DeleteSalesVoucher(aSales);
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been delete successfully...!!');", true);
                btnDelete.Enabled = false;
                btnSave.Enabled = false;
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }
    protected void dgSVMst_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgSVMst.DataSource = Session["SvMst"];
        dgSVMst.PageIndex = e.NewPageIndex;
        dgSVMst.DataBind();
    }
    protected void ddlPaymentMethord_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlPaymentMethord.SelectedValue == "C")
        {
            VisiblePayment(false, false, false, false, false, false, false, false);
            lblAmount.Text = "Cash Amount ";
            ddlChequeStatus.SelectedIndex = -1;
        }
        else if (ddlPaymentMethord.SelectedValue == "Q")
        {
            VisiblePayment(true, true, true, true, true, true, true, true);
            lblAmount.Text = "Cheque Amount ";
            ddlChequeStatus.SelectedIndex = 1;
        }
        else if (ddlPaymentMethord.SelectedValue == "CR")
        {
            VisiblePayment(false, false, true, true, true, true, true, true);
            lblAmount.Text = "Card Amount ";
        }
        else
        {
            VisiblePayment(false, false, false, false, false, false, false, false);
            lblAmount.Text = "Cash Amount ";
            ddlChequeStatus.SelectedIndex = -1;
        }
        UPPaymentMtd.Update();
    }
    public void VisiblePayment(bool lblBank, bool Bank, bool lblChkNo, bool ChkNo, bool lblChkDate, bool chkdate, bool lblChkStatus, bool chkStatus)
    {
        lblBankName.Visible = lblBank;
        ddlBank.Visible = Bank;
        lblChequeNo.Visible = lblChkNo;
        txtChequeNo.Visible = ChkNo;
        lblChequeDate.Visible = lblChkDate;
        txtChequeDate.Visible = chkdate;
        lblChequeStatus.Visible = lblChkStatus;
        ddlChequeStatus.Visible = chkStatus;
        ddlBank.SelectedIndex = -1;
        txtChequeDate.Text = txtChequeNo.Text = "";
        txtChequeAmount.Text = "0";
        txtChequeAmount.Focus();
    }


    protected void btnPrint_Click(object sender, EventArgs e)
    {
        string filename = txtInvoiceNo.Text;
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        Document document = new Document(PageSize.A4, 50f, 50f, 40f, 40f);
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
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

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
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
       
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
       
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 23f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase("INVOICE/BILL", FontFactory.GetFont(FontFactory.TIMES_BOLD, 11, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
       
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] titW = new float[2] { 80, 60 };
        PdfPTable pdtm = new PdfPTable(titW);
        pdtm.WidthPercentage = 100;

        PdfPTable pdtclient = new PdfPTable(4);
        pdtclient.WidthPercentage = 100;
        cell = new PdfPCell(FormatHeaderPhrase("Delivered/Sold to"));
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        pdtclient.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + ddlCustomer.SelectedItem.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        pdtclient.AddCell(cell);
        string Phone = IdManager.GetShowSingleValueString("Mobile", "ID", "Customer", ddlCustomer.SelectedValue);
        cell = new PdfPCell(FormatHeaderPhrase("Phone Number :"));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtclient.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + Phone));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        pdtclient.AddCell(cell);


        PdfPTable pdtpur = new PdfPTable(2);
        pdtpur.WidthPercentage = 100;
        cell = new PdfPCell(FormatHeaderPhrase("Invoice/Bill No."));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtInvoiceNo.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Invoice/Bill Date"));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtDate.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);


        cell = new PdfPCell(pdtclient);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);

        cell = new PdfPCell(pdtpur);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);
        document.Add(pdtm);

        //document.Add(dtempty);     

        float[] widthdtl = new float[6] { 15, 60, 20, 20, 20, 25 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Particulars"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Unit Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        DataTable DT1 = SalesManager.GetSalesDetails(lblInvNo.Text);
        int Serial = 1;
        decimal totQty = 0;
        decimal tot = 0;
        foreach (DataRow dr in DT1.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell = new PdfPCell(FormatPhrase(dr["Name"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Qty"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["SPrice"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Total"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            tot += Convert.ToDecimal(dr["Total"]);
        }

        cell = new PdfPCell(FormatPhrase("Total"));
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 5;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(tot.ToString("N2")));
        //cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        //cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 6;
        pdtdtl.AddCell(cell);


        //PdfPTable dtempty1 = new PdfPTable(1);
        //dtempty1.WidthPercentage = 100;
        cell = new PdfPCell(FormatPhrase("In word: " + DataManager.GetLiteralAmt(tot.ToString()).Replace("  ", " ").Replace("  ", " ")));
        cell.VerticalAlignment = 1;
        cell.HorizontalAlignment = 0;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        cell.Colspan = 6;    
        pdtdtl.AddCell(cell);      

        //PdfPTable dtempty1 = new PdfPTable(1);
        //dtempty1.WidthPercentage = 100;
        //cell = new PdfPCell(FormatHeaderPhrase("Comments :"));         
        //cell.FixedHeight = 20f;
        //cell.HorizontalAlignment = 0;
        //cell.VerticalAlignment = 1;
        //cell.Border = 0;
        
        //dtempty1.AddCell(cell);
        //document.Add(dtempty1);
        document.Add(pdtdtl);

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

    private static Phrase FormatHeaderTopPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
    protected void btnChallan_Click(object sender, EventArgs e)
    {
        string filename = txtInvoiceNo.Text;
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        Document document = new Document(PageSize.A4, 50f, 50f, 40f, 40f);
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
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

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
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase("CHALLAN", FontFactory.GetFont(FontFactory.TIMES_BOLD, 11, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] titW = new float[2] { 60, 50 };
        PdfPTable pdtm = new PdfPTable(titW);
        pdtm.WidthPercentage = 100;

        PdfPTable pdtclient = new PdfPTable(4);
        pdtclient.WidthPercentage = 100;
        cell = new PdfPCell(FormatHeaderPhrase("Delivery to"));
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        pdtclient.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + ddlCustomer.SelectedItem.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        pdtclient.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Phone Number :"));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtclient.AddCell(cell);
        string Phone = IdManager.GetShowSingleValueString("Mobile", "ID", "Customer", ddlCustomer.SelectedValue);
        cell = new PdfPCell(FormatPhrase(": " + Phone));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        pdtclient.AddCell(cell);


        PdfPTable pdtpur = new PdfPTable(2);
        pdtpur.WidthPercentage = 100;
        cell = new PdfPCell(FormatHeaderPhrase("Challan No."));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtInvoiceNo.Text.Replace("INV", "CH")));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Challan Date"));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtDate.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);


        cell = new PdfPCell(pdtclient);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);

        cell = new PdfPCell(pdtpur);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);
        document.Add(pdtm);

        //document.Add(dtempty);       
        float[] widthdtl = new float[5] { 15, 60, 20, 20, 20 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Particulars"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Unit Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        DataTable DT1 = SalesManager.GetSalesDetails(lblInvNo.Text);
        int Serial = 1;
        decimal totQty = 0;
        decimal tot = 0;
        foreach (DataRow dr in DT1.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell = new PdfPCell(FormatPhrase(dr["Name"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["SPrice"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Qty"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

        }
        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);


        document.Close();
        Response.Flush();
        Response.End();
    }

    protected void txtInvoiceNo_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = SaleReturnManager.GetShowSLMasterInfo(txtInvoiceNo.Text);
        if (dt.Rows.Count > 0)
        {
            lblInvNo.Text = dt.Rows[0]["ID"].ToString();
            btnFind_Click(sender, e);            
        }
    }
    protected void btnClientSave_Click(object sender, EventArgs e)
    {
        string IdGlCoa = "";
        clsClientInfo ci = new clsClientInfo();
        ci.CustomerName = txtvalue.Text;
        ci.Mobile = txtMobile.Text;
        ci.Email = txtEmail.Text;
        ci.NationalId = ci.Address1 = ci.Address2 = ci.Phone = ci.Fax = ci.PostalCode = ci.Country = "";
        ci.Code = IdManager.GetNextID("Customer", "Code").ToString().PadLeft(7, '0');       
        ci.Active = "True"; ci.CommonCus = "0";
        ci.LoginBy = Session["userID"].ToString();
        //IdGlCoa = IdManager.getAutoIdWithParameter("1044", "GL_SEG_COA", "SEG_COA_CODE", "1044000", "000", "3");
        //ci.GlCoa = IdGlCoa;
        clsClientInfoManager.CreateClientInfo(ci);
        //SegCoa sg = new SegCoa();
        //sg.GlSegCode = IdGlCoa;
        //sg.SegCoaDesc = "Accounts Receivable from-Customer-" + txtvalue.Text;
        //sg.LvlCode = "02";
        //sg.ParentCode = "1044000";
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
        //string NM = Session["org"].ToString().Substring(0, 5);
        //gl.CoaDesc = NM + ",Accounts Receivable from-Customer-" + txtvalue.Text;
        //gl.CoaCurrBal = "0.00";
        //gl.CoaNaturalCode = IdGlCoa;
        //GlCoaManager.CreateGlCoa(gl);
        txtvalue.Text = txtMobile.Text = txtEmail.Text = "";
        string queryLoc = "SELECT [ID],[ContactName] FROM [Customer] order by ID";
        util.PopulationDropDownList(ddlCustomer, "Customer", queryLoc, "ContactName", "ID");
        ddlCustomer.Items.Insert(0, "");
    }
    protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string Mobile = IdManager.GetShowSingleValueString("Mobile", "ID", "dbo.Customer", ddlCustomer.SelectedValue);
            txtMobilenumber.Text = Mobile;
            UpItemsDetails.Update();
        }
        catch
        {

        }
    }
}