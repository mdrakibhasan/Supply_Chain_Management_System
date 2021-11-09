using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.Data.SqlClient;
using autouniv;
using OldColor;

public partial class SalesReturn : System.Web.UI.Page
{
    private static DataTable dtmsr = new DataTable();
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
            ViewState["PVID"] = "";
            Session["purdtl"] =null;
            getEmptyDtl();
            txtReturnDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
            Session["Cash_Code"] = IdManager.GetShowSingleValueString("CASH_CODE", "BOOK_NAME", "GL_SET_OF_BOOKS", "AMB");
            txtGoodsReceiveNo.Enabled = txtSupplier.Enabled = txtReturnNO.Enabled = txtRemarks.Enabled = false;           
            dgPRNMst.Visible = true;
            btnSave.Enabled = true;
            PVIesms_UP.Visible = false;
            DataTable dt = SaleReturnManager.GetShowSalesReturnItems();
            ViewState["mst"] = dt;
            dgPRNMst.DataSource = dt;
            dgPRNMst.DataBind();
            btnNew.Visible = true;
            VisiblePayment(false, false, false, false, false, false, false, false);
            txtTotal.Text = txtTotPayment.Text =  txtDue.Text = "0";

            string query2 = "select '' [bank_id],'' [bank_name]  union select [bank_id] ,[bank_name] from [bank_info] order by 1";
            util.PopulationDropDownList(ddlBank, "bank_info", query2, "bank_name", "bank_id");
        }
    }
    private void getEmptyDtl()
    {
        dgPODetailsDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ID", typeof(string));
        dtDtlGrid.Columns.Add("item_code", typeof(string));
        dtDtlGrid.Columns.Add("item_desc", typeof(string));
        dtDtlGrid.Columns.Add("msr_unit_code", typeof(string));
        dtDtlGrid.Columns.Add("item_rate", typeof(string));
        dtDtlGrid.Columns.Add("qnty", typeof(string));
        dtDtlGrid.Columns.Add("Quantity", typeof(string));
        DataRow dr = dtDtlGrid.NewRow();
        dr["qnty"] = "0";
        dr["Quantity"] = "0";
        dtDtlGrid.Rows.Add(dr);
        dgPODetailsDtl.DataSource = dtDtlGrid;
        Session["purdtl"] = dtDtlGrid;
        dgPODetailsDtl.DataBind();
    }
    protected void txtGoodsReceiveNo_TextChanged(object sender, EventArgs e)
    {
        ViewState["PVID"] = "";
        lblGlCoa.Text = "";
        DataTable dt = SaleReturnManager.GetShowSLMasterInfo(txtGoodsReceiveNo.Text);
        if (dt.Rows.Count > 0)
        {
            txtSupplier.Text = dt.Rows[0]["ContactName"].ToString();
            ViewState["PVID"] = dt.Rows[0]["ID"].ToString();
            lblPVID.Text = dt.Rows[0]["ID"].ToString();
            //lblGlCoa.Text = dt.Rows[0]["Gl_CoaCode"].ToString();
            lblSupID.Text = dt.Rows[0]["CustomerID"].ToString();
            dgPODetailsDtl.DataSource = Session["purdtl"];
            dgPODetailsDtl.DataBind();
            PVIesms_UP.Update();
        }
    }
   
    public DataTable PopulatePayType()
    {
        DataTable dt = SaleReturnManager.GetSalesReturnItems(ViewState["PVID"].ToString());
        DataRow dr = dt.NewRow();
        dt.Rows.InsertAt(dr, 0);        
        return dt;
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {
        dgPRNMst.Visible = false;
        PVIesms_UP.Visible = true;
        txtGoodsReceiveNo.Text =txtSupplier.Text=txtReturnNO.Text=txtRemarks.Text= "";
        txtGoodsReceiveNo.Enabled = txtSupplier.Enabled = txtRemarks.Enabled = true;
        txtReturnNO.Text = IdManager.GetDateTimeWiseSerial("IVRN", "Return_No", "[OrderReturn]");
        txtGoodsReceiveNo.Focus();
        btnNew.Visible = false;
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (txtGoodsReceiveNo.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Select Goods Receive No...!!');", true);
        }
        else if (Session["purdtl"] == null)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There are no items in list.!!');", true);
        }
        else if (ddlPaymentMethord.SelectedValue == "Q" && Convert.ToDouble(txtTotPayment.Text) > 0 && ddlChequeStatus.SelectedValue == "P")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Incorrect Check Status.!!');", true);
        }
        else
        {
            SaleReturn rtn = SaleReturnManager.getShowRetirnItems(lbLId.Text);
            if (rtn != null)
            {
                if (per.AllowEdit == "Y")
                {
                    rtn.ReturnDate = txtReturnDate.Text;
                    rtn.Remarks = txtRemarks.Text;
                    rtn.LogonBy = Session["user"].ToString();
                    rtn.TotalAmount = txtTotal.Text.Replace(",", "");
                    rtn.Pay_Amount = txtTotPayment.Text.Replace(",", "");
                    rtn.PaymentMethod = ddlPaymentMethord.SelectedValue.Trim();
                    rtn.BankName = ddlBank.SelectedValue.Trim();
                    rtn.ChequeNo = txtChequeNo.Text.Trim();
                    rtn.ChequeDate = txtChequeDate.Text;
                    rtn.Chk_Status = ddlChequeStatus.SelectedValue.Trim();
                    DataTable dt = (DataTable)Session["purdtl"];
                    SaleReturnManager.UpdateSalesReturn(rtn, dt);
                    //PV_Acc_JurnalVoucher_Update();
                    //PV_Acc_Debit_Voucher_Update();
                    btnSave.Enabled = false;
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been update successfully...!!');", true);
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
                    rtn = new SaleReturn();
                    rtn.GRN = lblPVID.Text;
                    rtn.Return_No = txtReturnNO.Text;
                    rtn.ReturnDate = txtReturnDate.Text;
                    rtn.Remarks = txtRemarks.Text;
                    rtn.LogonBy = Session["user"].ToString();
                    rtn.TotalAmount = txtTotal.Text.Replace(",", "");
                    rtn.Pay_Amount = txtTotPayment.Text.Replace(",", "");
                    rtn.SupplierID = lblSupID.Text;
                    rtn.PaymentMethod = ddlPaymentMethord.SelectedValue.Trim();
                    rtn.BankName = ddlBank.SelectedValue.Trim();
                    rtn.ChequeNo = txtChequeNo.Text.Trim();
                    rtn.ChequeDate = txtChequeDate.Text;
                    rtn.Chk_Status = ddlChequeStatus.SelectedValue.Trim();
                    DataTable dt = (DataTable)Session["purdtl"];
                    SaleReturnManager.SaveInvoiceReturn(rtn, dt);
                    //PV_Acc_JurnalVoucher_Save();
                    //PV_Acc_Debit_Voucher_Save();
                    btnSave.Enabled = false;
                    lbLId.Text = IdManager.GetShowSingleValueIntNotParameter("top(1)ID", "OrderReturn order by ID desc").ToString();
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been save successfully...!!');", true);
                    
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                }
            }
        }

        btnSave.Enabled = true;
    }
    //string SalesCode = "1-5010001";
    //private void PV_Acc_JurnalVoucher_Save()
    //{
    //    if (Convert.ToDouble(txtTotal.Text) > 0)
    //    {
    //        VouchMst vmst = new VouchMst();
    //        vmst.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
    //        vmst.ValueDate = txtReturnDate.Text;
    //        vmst.VchCode = "03";
    //        vmst.RefFileNo = "";
    //        vmst.VolumeNo = "";
    //        vmst.SerialNo = txtReturnNO.Text.Trim();
    //        vmst.Particulars = "Sales Return Payable Amount for. - (" + txtReturnNO.Text + "-" + txtSupplier.Text + ")";
    //        vmst.ControlAmt = txtTotal.Text.Replace(",", "");
    //        vmst.Payee = "IR";
    //        vmst.CheckNo = txtChequeNo.Text;
    //        vmst.CheqDate = txtChequeDate.Text;
    //        vmst.CheqAmnt = txtTotal.Text.Replace(",", "");
    //        vmst.MoneyRptNo = "";
    //        vmst.MoneyRptDate = "";
    //        vmst.TransType = "R";
    //        vmst.BookName = "AMB";
    //        vmst.EntryUser = Session["user"].ToString();
    //        vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //        vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
    //        vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
    //        vmst.Status = "U";
    //        vmst.AuthoUserType = Session["userlevel"].ToString();
    //        VouchManager.CreateVouchMst(vmst);
    //        VouchDtl vdtl;
    //        for (int j = 0; j < 2; j++)
    //        {
    //            if (j == 0)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtReturnDate.Text;
    //                vdtl.LineNo = "1";
    //                vdtl.GlCoaCode = "1-" + lblGlCoa.Text;
    //                vdtl.Particulars = "On Customer - " + txtSupplier.Text + "";
    //                vdtl.AccType = VouchManager.getAccType("1-" + lblGlCoa.Text);
    //                vdtl.AmountDr = "0";
    //                vdtl.AmountCr = txtTotal.Text.Replace(",", "");
    //                vdtl.Status = "U";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //            else if (j == 1)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtReturnDate.Text;
    //                vdtl.LineNo = "2";
    //                vdtl.GlCoaCode = SalesCode; //**** AdditionalCharge Code *******//
    //                vdtl.Particulars = "Sales Return Payable Amount";
    //                vdtl.AccType = VouchManager.getAccType(SalesCode); //**** AdditionalCharge Code *******//
    //                vdtl.AmountDr = txtTotal.Text.Replace(",", "");
    //                vdtl.AmountCr = "0";
    //                vdtl.Status = "U";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //        }
    //    }
    //}
    //private void PV_Acc_JurnalVoucher_Update()
    //{
    //    string SRSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='IR' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtReturnNO.Text);
    //    if (Convert.ToDouble(txtTotal.Text) > 0)
    //    {
    //        VouchMst vmst = VouchManager.GetVouchMst(SRSerial.Trim());
    //        if (vmst != null)
    //        {

    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
    //            vmst.ValueDate = txtReturnDate.Text;
    //            vmst.VchCode = "03";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtReturnNO.Text.Trim();
    //            vmst.Particulars = "Sales Return Payable Amount for. - (" + txtReturnNO.Text + "-" + txtSupplier.Text + ")";
    //            vmst.ControlAmt = txtTotal.Text.Replace(",", "");           
    //            vmst.CheckNo = txtChequeNo.Text;
    //            vmst.CheqDate = txtChequeDate.Text;
    //            vmst.CheqAmnt = txtTotal.Text.Replace(",", "");                
    //            vmst.UpdateUser = Session["user"].ToString().ToUpper();
    //            vmst.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
    //            vmst.AuthoUserType = Session["userlevel"].ToString();
    //            VouchManager.UpdateVouchMst(vmst);
    //            VouchManager.DeleteVouchDtl(vmst.VchSysNo);
    //            VouchDtl vdtl;
    //            for (int j = 0; j < 2; j++)
    //            {
    //                if (j == 0)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtReturnDate.Text;
    //                    vdtl.LineNo = "1";
    //                    vdtl.GlCoaCode = "1-" + lblGlCoa.Text;
    //                    vdtl.Particulars = "On Customer - " + txtSupplier.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + lblGlCoa.Text);
    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = txtTotal.Text.Replace(",", "");
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = "AMB";
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                else if (j == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtReturnDate.Text;
    //                    vdtl.LineNo = "2";
    //                    vdtl.GlCoaCode = SalesCode; //**** Sales  Code *******//
    //                    vdtl.Particulars = "Sales Return Payable Amount";
    //                    vdtl.AccType = VouchManager.getAccType(SalesCode); //**** Sales Code *******//
    //                    vdtl.AmountDr = txtTotal.Text.Replace(",", "");
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = "AMB";
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }
    //}

    //private void PV_Acc_Debit_Voucher_Update()
    //{
    //    string SVSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='IR' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtReturnNO.Text);
    //    if (Convert.ToDouble(txtTotPayment.Text) > 0)
    //    {
    //        if ((ddlPaymentMethord.SelectedValue == "C" && Convert.ToDouble(txtTotPayment.Text) > 0) || (ddlPaymentMethord.SelectedValue == "Q" && ddlChequeStatus.SelectedValue == "A" && Convert.ToDouble(txtTotPayment.Text) > 0))
    //        {
    //            VouchMst vmst = VouchManager.GetVouchMst(SVSerial.Trim());
    //            if (vmst != null)
    //            {
    //                vmst.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
    //                vmst.ValueDate = txtReturnDate.Text;                    
    //                vmst.SerialNo = txtReturnNO.Text.Trim();
    //                vmst.Particulars = "Taka Payment For Items Return in Customer .- (" + txtReturnNO.Text + "-" + txtSupplier.Text + ")";
    //                vmst.ControlAmt = txtTotPayment.Text.Replace(",", "");                   
    //                vmst.CheckNo = txtChequeNo.Text;
    //                vmst.CheqDate = txtChequeDate.Text;
    //                vmst.CheqAmnt = txtTotPayment.Text.Replace(",", "");                    
    //                vmst.UpdateUser = Session["user"].ToString().ToUpper();
    //                vmst.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
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
    //                        vdtl.ValueDate = txtReturnDate.Text;
    //                        vdtl.LineNo = "1";
    //                        vdtl.GlCoaCode = "1-" + lblGlCoa.Text;
    //                        vdtl.Particulars = "On Customer" + txtSupplier.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + lblGlCoa.Text);
    //                        vdtl.AmountDr = txtTotPayment.Text.Replace(",", "");
    //                        vdtl.AmountCr = "0";
    //                        vdtl.Status = vmst.Status;
    //                        vdtl.BookName = Session["book"].ToString();
    //                        VouchManager.CreateVouchDtl(vdtl);
    //                    }
    //                    if (j == 1)
    //                    {
    //                        vdtl = new VouchDtl();
    //                        vdtl.VchSysNo = vmst.VchSysNo;
    //                        vdtl.ValueDate = txtReturnDate.Text;
    //                        vdtl.LineNo = "2";
    //                        if (ddlPaymentMethord.SelectedValue != "C")
    //                        {
    //                            string BankCOA = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    //                            vdtl.GlCoaCode = "1-" + BankCOA;
    //                            vdtl.Particulars = "Cash at Bank : Purchase Items Payment On Supplier -(" + txtReturnNO.Text + "-" + txtSupplier.Text + ")";
    //                            vdtl.AccType = VouchManager.getAccType("1-" + BankCOA);
    //                        }
    //                        else
    //                        {

    //                            vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                            vdtl.Particulars = "Cash at Hand :  Purchase Items Payment On Supplier -(" + txtReturnNO.Text + "-" + txtSupplier.Text + ")";
    //                            vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());
    //                        }

    //                        vdtl.AmountDr = "0";
    //                        vdtl.AmountCr = txtTotPayment.Text.Replace(",", "");
    //                        vdtl.Status = vmst.Status;
    //                        vdtl.BookName = Session["book"].ToString();
    //                        VouchManager.CreateVouchDtl(vdtl);
    //                    }
    //                }
    //            }
    //        }
    //    }  
    //}
    //private void PV_Acc_Debit_Voucher_Save()
    //{
    //    if (Convert.ToDouble(txtTotPayment.Text) > 0)
    //    {
    //        if ((ddlPaymentMethord.SelectedValue == "C" && Convert.ToDouble(txtTotPayment.Text) > 0) || (ddlPaymentMethord.SelectedValue == "Q" && ddlChequeStatus.SelectedValue == "A" && Convert.ToDouble(txtTotPayment.Text) > 0))
    //        {
    //            VouchMst vmst = new VouchMst();
    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
    //            vmst.ValueDate = txtReturnDate.Text;
    //            vmst.VchCode = "01";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtReturnNO.Text.Trim();
    //            vmst.Particulars = "Taka Payment For Items Return in Customer .- (" + txtReturnNO.Text + "-" + txtSupplier.Text + ")";
    //            vmst.ControlAmt = txtTotPayment.Text.Replace(",", "");
    //            vmst.Payee = "IR";
    //            vmst.CheckNo = txtChequeNo.Text;
    //            vmst.CheqDate = txtChequeDate.Text;
    //            vmst.CheqAmnt = txtTotPayment.Text.Replace(",", "");
    //            vmst.MoneyRptNo = "";
    //            vmst.MoneyRptDate = "";
    //            vmst.TransType = "R";
    //            vmst.BookName = Session["book"].ToString();
    //            vmst.EntryUser = Session["user"].ToString();
    //            vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //            vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
    //            vmst.VchRefNo = "DV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
    //            vmst.Status = "U";
    //            vmst.AuthoUserType = Session["userlevel"].ToString();
    //            VouchManager.CreateVouchMst(vmst);
    //            VouchDtl vdtl;
    //            for (int j = 0; j < 2; j++)
    //            {
    //                if (j == 0)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtReturnDate.Text;
    //                    vdtl.LineNo = "1";
    //                    vdtl.GlCoaCode = "1-" + lblGlCoa.Text;
    //                    vdtl.Particulars = "On Customer" + txtSupplier.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + lblGlCoa.Text);
    //                    vdtl.AmountDr = txtTotPayment.Text.Replace(",", "");
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = "U";
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                if (j == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtReturnDate.Text;
    //                    vdtl.LineNo = "2";
    //                    if (ddlPaymentMethord.SelectedValue != "C")
    //                    {
    //                        string BankCOA = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    //                        vdtl.GlCoaCode = "1-" + BankCOA;
    //                        vdtl.Particulars = "Cash at Bank : Purchase Items Payment On Supplier -(" + txtReturnNO.Text + "-" + txtSupplier.Text + ")";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + BankCOA);
    //                    }
    //                    else
    //                    {

    //                        vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                        vdtl.Particulars = "Cash at Hand :  Purchase Items Payment On Supplier -(" + txtReturnNO.Text + "-" + txtSupplier.Text + ")";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());
    //                    }

    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = txtTotPayment.Text.Replace(",", "");
    //                    vdtl.Status = "U";
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }           
    //}

    protected void Delete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            SaleReturn rtn = SaleReturnManager.getShowRetirnItems(lbLId.Text);
            if (rtn != null)
            {
                SaleReturnManager.DeleteItemsReturn(rtn);
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been delete successfully...!!');", true);
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }

    protected void dgPRNMst_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgPRNMst.DataSource = ViewState["mst"];
        dgPRNMst.PageIndex = e.NewPageIndex;
        dgPRNMst.DataBind();
    }
    protected void dgPRNMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[5].Attributes.Add("style", "display:none");
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
    protected void dgPRNMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtRemarks.Enabled = txtReturnDate.Enabled = true;
        lbLId.Text = dgPRNMst.SelectedRow.Cells[5].Text.Trim();
        SaleReturn rtn = SaleReturnManager.getShowRetirnItems(lbLId.Text);
        if (rtn != null)
        {
            txtGoodsReceiveNo.Text = IdManager.GetShowSingleValueString("t2.InvoiceNo", "t1.ID", "OrderReturn t1 inner join [Order] t2 on t2.ID=t1.InvoiceNo", lbLId.Text);
           ViewState["PVID"] = "";
           lblGlCoa.Text = "";


           DataTable dt = SaleReturnManager.GetShowSLMasterInfo(txtGoodsReceiveNo.Text);
           if (dt.Rows.Count > 0)
           {
               txtSupplier.Text = dt.Rows[0]["ContactName"].ToString();
               ViewState["PVID"] = dt.Rows[0]["ID"].ToString();
               lblPVID.Text = dt.Rows[0]["ID"].ToString();
               lblGlCoa.Text = dt.Rows[0]["Gl_CoaCode"].ToString();
               lblSupID.Text = dt.Rows[0]["CustomerID"].ToString();
               
               txtReturnDate.Text = rtn.ReturnDate;
               txtReturnNO.Text = rtn.Return_No;
               txtRemarks.Text = rtn.Remarks;

               DataTable dtItems = SaleReturnManager.ItemsDetails(lbLId.Text);
               Session["purdtl"] = dtItems;
               dgPODetailsDtl.DataSource = dtItems;
               dgPODetailsDtl.DataBind();
               ShowFooterTotal(dtItems);
               dgPRNMst.Visible =btnNew.Visible= false;
               PVIesms_UP.Visible = true;
               PVIesms_UP.Update();
           }
           ddlPaymentMethord.SelectedValue = rtn.PaymentMethod.Trim();
           txtTotPayment.Text = rtn.Pay_Amount;
             txtDue.Text=( Convert.ToDouble(txtTotal.Text)-Convert.ToDouble(txtTotPayment.Text)).ToString("N2");
           if (ddlPaymentMethord.SelectedValue != "C")
           {
               txtChequeDate.Text = rtn.ChequeDate;
               txtChequeNo.Text=rtn.ChequeNo;
               ddlBank.SelectedValue = rtn.BankName;
               ddlChequeStatus.SelectedValue = rtn.Chk_Status;
           }
        }
    }
    protected void txtQnty_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dt = (DataTable)Session["purdtl"];
        // DataTable dt = ItemManager.GetItems(((TextBox)gvr.FindControl("txtItemDesc")).Text);
        if (dt.Rows.Count > 0)
        {
            
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            try
            {
                if (Convert.ToDecimal(dr["Quantity"].ToString()) < Convert.ToDecimal(((TextBox)gvr.FindControl("txtQnty")).Text))
                {
                    ((TextBox)gvr.FindControl("txtQnty")).Text="0";
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Return Qty Over then Sale Qty...!!');", true);
                    return;
                }
            }
            catch
            {

            }
            dr["ID"] = dr["ID"].ToString();
            dr["item_desc"] = dr["item_desc"].ToString();
            dr["item_code"] = dr["item_code"].ToString();
            dr["msr_unit_code"] = dr["msr_unit_code"].ToString();
            dr["item_rate"] = ((TextBox)gvr.FindControl("txtItemRate")).Text;
           
            dr["qnty"] = ((TextBox)gvr.FindControl("txtQnty")).Text;

        }
        string found = "";
        foreach (DataRow drd in dt.Rows)
        {
            if (drd["item_code"].ToString() == "" && drd["item_desc"].ToString() == "")
            {
                found = "Y";
            }
        }
        if (found == "")
        {
            DataRow drd = dt.NewRow();
            dt.Rows.Add(drd);
        }
        dgPODetailsDtl.DataSource = dt;
        dgPODetailsDtl.DataBind();
        ShowFooterTotal(dt);
        ((DropDownList)dgPODetailsDtl.Rows[dgPODetailsDtl.Rows.Count - 1].FindControl("DropDownList1")).Focus();
        PVIesms_UP.Update();
    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((DropDownList)sender).NamingContainer;
        DataTable dtdtl = (DataTable)Session["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        DataTable dt = SaleReturnManager.GetIVItems(((DropDownList)gvr.FindControl("DropDownList1")).SelectedValue, ViewState["PVID"].ToString());
        if (dt.Rows.Count > 0)
        {
            dtdtl.Rows.Remove(dr);
            dr = dtdtl.NewRow();
            dr["ID"] = ((DataRow)dt.Rows[0])["ID"].ToString();
            dr["item_desc"] = ((DataRow)dt.Rows[0])["item_desc"].ToString();
            dr["item_code"] = ((DataRow)dt.Rows[0])["item_code"].ToString();
            dr["msr_unit_code"] = ((DataRow)dt.Rows[0])["msr_unit_code"].ToString();
            dr["item_rate"] = ((DataRow)dt.Rows[0])["item_rate"].ToString();
            dr["qnty"] = "0";
            dr["Quantity"] = ((DataRow)dt.Rows[0])["Quantity"].ToString();
            dtdtl.Rows.InsertAt(dr, gvr.DataItemIndex);
        }
        dgPODetailsDtl.DataSource = dtdtl;
        Session["purdtl"]=dtdtl;
        dgPODetailsDtl.DataBind();
        ShowFooterTotal(dtdtl);
        ((TextBox)dgPODetailsDtl.Rows[dgPODetailsDtl.Rows.Count - 1].FindControl("txtQnty")).Focus();
        PVIesms_UP.Update();
    }
    private void ShowFooterTotal(DataTable DT1)
    {
        decimal tot = 0;
        foreach (DataRow dr in DT1.Rows)
        {
            if (dr["item_desc"].ToString() != "")
            {
                tot += Convert.ToDecimal(dr["item_rate"]) * Convert.ToDecimal(dr["qnty"]);
            }
        }
        txtTotal.Text = tot.ToString();      
    }
    protected void Clear_Click(object sender, EventArgs e)
    {
        Response.Redirect("SalesReturn.aspx?mno=5.19");
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {

        string filename = txtReturnNO.Text;
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
        cell = new PdfPCell(new Phrase("Sales/Invoice Return", FontFactory.GetFont(FontFactory.TIMES_BOLD, 11, iTextSharp.text.Font.BOLD)));
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
        cell = new PdfPCell(FormatHeaderPhrase("I.Return No "));
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        pdtclient.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtReturnNO.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        pdtclient.AddCell(cell);
        string Phone = IdManager.GetShowSingleValueString("Mobile", "ID", "Customer", lblSupID.Text);
        cell = new PdfPCell(FormatHeaderPhrase("Customer Name "));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtclient.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtSupplier.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        pdtclient.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Remarks "));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtclient.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtRemarks.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        pdtclient.AddCell(cell);


        PdfPTable pdtpur = new PdfPTable(2);
        pdtpur.WidthPercentage = 100;
        cell = new PdfPCell(FormatHeaderPhrase("Invoice No. "));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtGoodsReceiveNo.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Return Date"));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtReturnDate.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 2;
        pdtpur.AddCell(cell);

        cell = new PdfPCell(pdtclient);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);

        cell = new PdfPCell(pdtpur);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 8f;
        cell.Colspan = 2;
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
        //DataTable DT1 = SalesManager.GetSalesDetails(lblInvNo.Text);
        int Serial = 1;
        decimal totQty = 0;
        decimal tot = 0;
        DataTable DT1 = SaleReturnManager.ItemsDetails(lbLId.Text);
        foreach (DataRow dr in DT1.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell = new PdfPCell(FormatPhrase(dr["des_name"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["qnty"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["item_rate"].ToString()));
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
    public DataTable PopulateMeasure()
    {
        dtmsr = ItemManager.GetMeasure();
        DataRow dr = dtmsr.NewRow();
        dtmsr.Rows.InsertAt(dr, 0);
        return dtmsr;
    }
    protected void dgPurDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((DataRowView)e.Row.DataItem)["qnty"].ToString() != "" && ((DataRowView)e.Row.DataItem)["item_rate"].ToString() != "")
                {
                    decimal total = decimal.Parse(((DataRowView)e.Row.DataItem)["item_rate"].ToString()) *
                                   decimal.Parse(((DataRowView)e.Row.DataItem)["qnty"].ToString());
                    ((Label)e.Row.FindControl("lblTotal")).Text = total.ToString("N2");

                }
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[3].Attributes.Add("style", "display:none");
                e.Row.Cells[7].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[3].Attributes.Add("style", "display:none");
                e.Row.Cells[7].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[3].Attributes.Add("style", "display:none");
                e.Row.Cells[7].Attributes.Add("style", "display:none");
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
    protected void dgPurDtl_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

    }
    protected void ddlPaymentMethord_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlPaymentMethord.SelectedValue == "C")
        {
            VisiblePayment(false, false, false, false, false, false, false, false);
           // lblAmount.Text = "Cash Amount ";
            ddlChequeStatus.SelectedIndex = -1;
        }
        else if (ddlPaymentMethord.SelectedValue == "Q")
        {
            VisiblePayment(true, true, true, true, true, true, true, true);
           // lblAmount.Text = "Cheque Amount ";
            ddlChequeStatus.SelectedIndex = 1;
        }
        else if (ddlPaymentMethord.SelectedValue == "CR")
        {
            VisiblePayment(false, false, true, true, true, true, true, true);
           // lblAmount.Text = "Card Amount ";
        }
        else
        {
            VisiblePayment(false, false, false, false, false, false, false, false);
            //lblAmount.Text = "Cash Amount ";
            ddlChequeStatus.SelectedIndex = -1;
        }
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
    }
    protected void txtTotPayment_TextChanged(object sender, EventArgs e)
    {
        txtDue.Text = (Convert.ToDouble(txtTotal.Text) - Convert.ToDouble(txtTotPayment.Text)).ToString("N2");
        ddlPaymentMethord.Focus();
        UPPaymentMtd.Update();
    }
}