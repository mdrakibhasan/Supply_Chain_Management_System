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

public partial class PurchaseVoucher : System.Web.UI.Page
{
    private static DataTable dtsup = new DataTable();
    private static DataTable dtmsr = new DataTable();
    public static decimal priceDr = 0;
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
            Session["Cash_Code"] = "";
            ViewState["OldStock"] = null;
            DropDownListValue();

            txtGRNODate.Attributes.Add("onBlur", "formatdate('" + txtGRNODate.ClientID + "')");
            txtPODate.Attributes.Add("onBlur", "formatdate('" + txtPODate.ClientID + "')");
            txtChallanDate.Attributes.Add("onBlur", "formatdate('" + txtChallanDate.ClientID + "')");

            txtGRNODate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
            txtPODate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
            txtChallanDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");

            PanelHistory.Visible = true;      
            tabVch.Visible = false;
            Session["Cash_Code"] = IdManager.GetShowSingleValueString("CASH_CODE", "BOOK_NAME", "GL_SET_OF_BOOKS", "AMB");
            DataTable dt = PurchaseVoucherManager.GetShowPurchaseMst();
            dgPVMst.DataSource = dt;
            Session["PvMst"] = dt;
            dgPVMst.DataBind();
            btnDelete.Enabled = btnSave.Enabled =btnNew.Visible= true;
            txtID.Text = txtSearchChallanNo.Text = txtFromDate.Text = txtToDAte.Text = txtSearchGrNO.Text = txtSearchSuplier.Text = lblSupplierID.Text = string.Empty;
            txtGRNO.Enabled = txtChallanNo.Enabled = txtPO.Enabled = txtGRNODate.Enabled = txtChallanDate.Enabled = ddlSupplier.Enabled = txtRemarks.Enabled = HyperLink1.Visible =btnSave.Visible= txtShiftmentNo.Enabled = ddlParty.Enabled = false;
            txtAddTot.Text = "0";
            txtID.Text = "";
            ddlPaymentMethord.SelectedIndex = -1;
            btnNew.Visible = true;
            DataTable dtFixCode = VouchManager.GetAllFixGlCode("");

            lblCashValue.Text = "10000000";
            //if (Blance > 0)
            //{
            //    btnNew.Visible = true;
            //}
            //else
            //{
            //    btnNew.Visible = false;
            //}

            txtGRNO.Focus();
        }
    }

    private void DropDownListValue()
    {
        string queryLoc = "select * from View_Supplier order by 2";
        util.PopulationDropDownList(ddlSupplier, "View_Supplier", queryLoc, "ContactName", "ID");

        string query = "select '' ID,'' ContactName  union select ID ,ContactName from Labure where SupplierGroupID='CP' order by 1";
        util.PopulationDropDownList(ddlCarriagePerson, "CostType", query, "ContactName", "ID");

        string query1 = "select '' ID,'' ContactName  union select ID ,ContactName from Labure where SupplierGroupID='LP' order by 1";
        util.PopulationDropDownList(ddlLaburePerson, "CostType", query1, "ContactName", "ID");

        string query2 = "select '' [bank_id],'' [bank_name]  union select [bank_id] ,[bank_name] from [bank_info] order by 1";
        util.PopulationDropDownList(ddlBank, "bank_info", query2, "bank_name", "bank_id");

        string query3 = "select '' ID,'' PartyName  union select  ID,PartyName from  PartyInfo order by 1";
        util.PopulationDropDownList(ddlParty, "PartyInfo", query3, "PartyName", "ID");
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {
        ClearFields();
        PanelHistory.Visible = btnNew.Visible = btnNew.Visible = false;       
        getEmptyDtl();      
        ddlSupplier.SelectedIndex = -1;
        ddlPaymentMethord.SelectedIndex = -1;
        tabVch.Visible = HyperLink1.Visible = btnSave.Visible = true;
        txtChallanNo.Enabled = txtPO.Enabled = txtGRNODate.Enabled = txtChallanDate.Enabled = ddlSupplier.Enabled = txtRemarks.Enabled = dgPVDetailsDtl.Enabled = txtShiftmentNo.Enabled = ddlParty.Enabled = true;
        txtChallanNo.Focus();
    }
    private void ClearFields()
    {
        Session["purdtl"] = null;
        txtGRNO.Text = "";
        txtPO.Text = "";
        txtChallanNo.Text = "";
        txtRemarks.Text = "";
       // txtSiftment.Text = "";
        txtGRNODate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
        txtPODate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
        txtChallanDate.Text = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
        txtTotalAmount.Text = "0";
        txtOtherCharge.Text = "0";
        txtCarriageCharge.Text = "0";
        txtLabureCharge.Text = "0";
        txtTotPayment.Text = "0";
        txtDue.Text = "0";
        //ddlPaymentMethord.SelectedIndex = 1;
        ddlBank.SelectedIndex = -1;
        txtChequeDate.Text = "";
        txtChequeNo.Text = "";
        txtChequeAmount.Text = "0";
        txtID.Text = "";
        txtTotItems.Text = txtAddTot.Text = "0";
        VisiblePayment(false, false, false, false, false, false, false, false);
    }
    private void getEmptyDtl()
    {      
        dgPVDetailsDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ID", typeof(string));
        dtDtlGrid.Columns.Add("item_code", typeof(string));
        dtDtlGrid.Columns.Add("item_desc", typeof(string));
        dtDtlGrid.Columns.Add("msr_unit_code", typeof(string));
        dtDtlGrid.Columns.Add("item_rate", typeof(string));
        dtDtlGrid.Columns.Add("qnty", typeof(string));
        dtDtlGrid.Columns.Add("Additional", typeof(string));
        dtDtlGrid.Columns.Add("UMO", typeof(string));
        dtDtlGrid.Columns.Add("BrandName", typeof(string));

        DataRow dr = dtDtlGrid.NewRow();
        dr["Additional"] = "0";
        dtDtlGrid.Rows.Add(dr);
        dgPVDetailsDtl.DataSource = dtDtlGrid;
        Session["purdtl"] = dtDtlGrid;
        dgPVDetailsDtl.DataBind();
    }

    protected void ddlBank_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["GlCode"] = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (ddlSupplier.SelectedItem.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Enter Supplier...!!');", true);
            }
            else if (ddlPaymentMethord.SelectedValue == "Q" && ddlChequeStatus.SelectedItem.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Select Cheque Status..!!');", true);
            }
            else
            {
                if (ddlPaymentMethord.SelectedValue.Equals("C"))
                {
                    ViewState["GlCode"] = IdManager.GetShowSingleValueString("MainOffCashCode", "ID", "FixValue",
                        "1");
                }
                PurchaseVoucherInfo purmst = PurchaseVoucherManager.GetPurchaseMst(txtID.Text.Trim());

               // if (Convert.ToDecimal(lblCashValue.Text) > Convert.ToDecimal(txtTotPayment.Text.Replace(",", "")))
                {

                    if (purmst != null)
                    {
                        if (per.AllowEdit == "Y")
                        {

                            purmst.ID = txtID.Text;
                            purmst.GoodsReceiveNo = txtGRNO.Text.Trim();
                            purmst.GoodsReceiveDate = txtGRNODate.Text;
                            purmst.PurchaseOrderNo = txtPO.Text;
                            purmst.PurchaseOrderDate = txtPODate.Text;
                            purmst.ChallanNo = txtChallanNo.Text;
                            purmst.ChallanDate = txtChallanDate.Text;
                            purmst.Supplier = ddlSupplier.SelectedValue;
                            purmst.Remarks = txtRemarks.Text;
                            purmst.TotalAmount = txtTotalAmount.Text.Replace(",", "");
                            purmst.TotalPayment = txtTotPayment.Text.Replace(",", "");
                            purmst.CarriagePerson = ddlCarriagePerson.SelectedValue;
                            purmst.CarriageCharge = txtCarriageCharge.Text.Replace(",", "");
                            purmst.LaburePerson = ddlLaburePerson.SelectedValue;
                            purmst.LabureCharge = txtLabureCharge.Text.Replace(",", "");
                            purmst.OtherCharge = txtOtherCharge.Text.Replace(",", "");
                            purmst.PaymentMethord = ddlPaymentMethord.SelectedValue;
                            purmst.BankId = ddlBank.SelectedValue;
                            purmst.ChequeNo = txtChequeNo.Text;
                            purmst.ChequeDate = txtChequeDate.Text;
                            purmst.ChequeAmount = txtChequeAmount.Text.Replace(",", "");
                            if (ddlParty.SelectedItem.Text == "")
                            {
                                purmst.PartyID = "1";
                            }
                            else
                            {
                                purmst.PartyID = ddlParty.SelectedValue;
                            }
                            purmst.ShiftmentID = "";
                            purmst.LoginBy = Session["user"].ToString();
                            purmst.ChkStatus = ddlChequeStatus.SelectedValue;
                            purmst.SupplierGlCode = Session["Supplier_COA"].ToString();
                            purmst.SupplierName = ddlSupplier.SelectedItem.Text;
                            DataTable dt = (DataTable)Session["purdtl"];
                            DataTable dtOldStk = (DataTable)ViewState["OldStock"];

                            
                            PurchaseVoucherManager.UpdatePurchaseVoucher(purmst, dt, dtOldStk,  ViewState["GlCode"].ToString());
                            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                                "alert('Record has been update successfully...!!');", true);
                            btnSave.Visible = false;

                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                "alert('You are not Permitted this Step...!!');", true);
                        }
                    }

                    else
                    {
                        if (per.AllowAdd == "Y")
                        {
                            purmst = new PurchaseVoucherInfo();
                            purmst.GoodsReceiveDate = txtGRNODate.Text;
                            purmst.PurchaseOrderNo = txtPO.Text;
                            purmst.PurchaseOrderDate = txtPODate.Text;
                            purmst.ChallanNo = txtChallanNo.Text;
                            purmst.ChallanDate = txtChallanDate.Text;
                            purmst.Supplier = ddlSupplier.SelectedValue;
                            purmst.Remarks = txtRemarks.Text;
                            purmst.TotalAmount = txtTotalAmount.Text.Replace(",", "");
                            purmst.TotalPayment = txtTotPayment.Text.Replace(",", "");
                            purmst.CarriagePerson = ddlCarriagePerson.SelectedValue;
                            purmst.CarriageCharge = txtCarriageCharge.Text.Replace(",", "");
                            purmst.LaburePerson = ddlLaburePerson.SelectedValue;
                            purmst.LabureCharge = txtLabureCharge.Text.Replace(",", "");
                            purmst.OtherCharge = txtOtherCharge.Text.Replace(",", "");
                            purmst.PaymentMethord = ddlPaymentMethord.SelectedValue;
                            purmst.BankId = ddlBank.SelectedValue;
                            purmst.ChequeNo = txtChequeNo.Text;
                            purmst.ChequeDate = txtChequeDate.Text;
                            purmst.ChequeAmount = txtChequeAmount.Text.Replace(",", "");
                            purmst.LoginBy = Session["user"].ToString();
                            txtGRNO.Text = IdManager.GetDateTimeWiseSerial("GRN", "GRN", "[ItemPurchaseMst]");
                            purmst.GoodsReceiveNo = txtGRNO.Text.Trim();
                            if (ddlParty.SelectedItem.Text == "")
                            {
                                purmst.PartyID = "1";
                            }
                            else
                            {
                                purmst.PartyID = ddlParty.SelectedValue;
                            }
                            purmst.ShiftmentID = "";
                            purmst.ChkStatus = ddlChequeStatus.SelectedValue;
                            purmst.SupplierGlCode = Session["Supplier_COA"].ToString();
                            purmst.SupplierName = ddlSupplier.SelectedItem.Text;
                            DataTable dt = (DataTable)Session["purdtl"];

                           
                            int ID = PurchaseVoucherManager.SavePurchaseVoucher(purmst, dt, lblOrNo.Text, ViewState["GlCode"].ToString());
                            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                                "alert('Record has been saved successfully...!!');", true);
                            btnSave.Visible = false;
                            //int ID = IdManager.GetShowSingleValueIntNotParameter("TOP(1)[ID]", "[ItemPurchaseMst] order by ID desc");
                            Session["purdtl"] = PurchaseVoucherManager.GetPurchaseItemsDetails(ID.ToString());
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                "alert('You are not Permitted this Step...!!');", true);
                        }
                    }
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

    //private void PV_Acc_JurnalVoucher_Save()
    //{       
    //    if (Convert.ToDouble(txtTotalAmount.Text) > 0)
    //    {
    //        VouchMst vmst = new VouchMst();
    //        vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //        vmst.ValueDate = txtGRNODate.Text;
    //        vmst.VchCode = "03";
    //        vmst.RefFileNo = "";
    //        vmst.VolumeNo = "";
    //        vmst.SerialNo = txtGRNO.Text.Trim();
    //        if (ddlParty.SelectedItem.Text != "")                        
    //        { vmst.Particulars = "Items Purchase Payable Amount Party. - (" + txtGRNO.Text + "-" +ddlParty.SelectedItem.Text + ")"; }
    //        else { vmst.Particulars = "Items Purchase Payable Amount Supplier. - (" + txtGRNO.Text + "-" + ddlSupplier.SelectedItem.Text + ")"; }
    //        vmst.ControlAmt = txtTotItems.Text.Replace(",", "");
    //        vmst.Payee = "PV";
    //        vmst.CheckNo = txtChequeNo.Text;
    //        vmst.CheqDate = txtChequeDate.Text;
    //        vmst.CheqAmnt = txtChequeAmount.Text.Replace(",","");
    //        vmst.MoneyRptNo = "";
    //        vmst.MoneyRptDate = "";
    //        vmst.TransType = "R";
    //        vmst.BookName = "AMB";
    //        vmst.EntryUser = Session["user"].ToString();
    //        vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //        vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
    //        vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
    //        vmst.Status = "A";
    //        vmst.AuthoUserType = Session["userlevel"].ToString();
    //        VouchManager.CreateVouchMst(vmst);            
    //        VouchDtl vdtl;
    //        for (int j = 0; j < 2; j++)
    //        {
    //            if (j == 0)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "1";
    //                if (ddlParty.SelectedItem.Text != "")
    //                { 
    //                    vdtl.GlCoaCode = "1-" + Session["Party_COA"].ToString(); vdtl.Particulars = "On Party - " + ddlParty.SelectedItem.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + Session["Party_COA"].ToString());
    //                }
    //                else
    //                { 
    //                    vdtl.GlCoaCode = "1-" + Session["Supplier_COA"].ToString(); vdtl.Particulars = "On Supplier - " + ddlSupplier.SelectedItem.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + Session["Supplier_COA"].ToString());
    //                }                          
    //                vdtl.AmountDr = "0";
    //                vdtl.AmountCr = txtTotItems.Text.Replace(",", "");
    //                vdtl.Status = "A";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //            else if (j == 1)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "2";
    //                vdtl.GlCoaCode = AdditionalCharge; //**** AdditionalCharge Code *******//
    //                vdtl.Particulars = "Purchase Payable Amount";
    //                vdtl.AccType = VouchManager.getAccType(AdditionalCharge); //**** AdditionalCharge Code *******//
    //                vdtl.AmountDr = txtTotItems.Text.Replace(",", "");
    //                vdtl.AmountCr = "0";
    //                vdtl.Status = "A";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //        }
    //    }
    //    //************************************** Additional  Charge *********************

    //    if (Convert.ToDouble(txtAddTot.Text) > 0)
    //    {
    //        VouchMst vmst = new VouchMst();
    //        vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //        vmst.ValueDate = txtGRNODate.Text;
    //        vmst.VchCode = "03";
    //        vmst.RefFileNo = "";
    //        vmst.VolumeNo = "";
    //        vmst.SerialNo = txtGRNO.Text.Trim();
    //        vmst.Particulars = "Additional Service Charge Payable  in Supplier. - (" + txtGRNO.Text + "-" + ddlSupplier.SelectedItem.Text + ")";
    //        vmst.ControlAmt = txtAddTot.Text.Replace(",", "");
    //        vmst.Payee = "AddC";
    //        vmst.CheckNo = txtChequeNo.Text;
    //        vmst.CheqDate = txtChequeDate.Text;
    //        vmst.CheqAmnt = txtChequeAmount.Text.Replace(",", "");
    //        vmst.MoneyRptNo = "";
    //        vmst.MoneyRptDate = "";
    //        vmst.TransType = "R";
    //        vmst.BookName = "AMB";
    //        vmst.EntryUser = Session["user"].ToString();
    //        vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //        vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
    //        vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
    //        vmst.Status = "A";
    //        vmst.AuthoUserType = Session["userlevel"].ToString();
    //        VouchManager.CreateVouchMst(vmst);
    //        VouchDtl vdtl;
    //        for (int j = 0; j < 2; j++)
    //        {
    //            if (j == 0)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "1";
    //                vdtl.GlCoaCode = "1-" + Session["Supplier_COA"].ToString();
    //                vdtl.Particulars = "On Supplier - " + ddlSupplier.SelectedItem.Text + "";
    //                vdtl.AccType = VouchManager.getAccType("1-" + Session["Supplier_COA"].ToString());
    //                vdtl.AmountDr = "0";
    //                vdtl.AmountCr = txtAddTot.Text.Replace(",", "");
    //                vdtl.Status = "A";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //            else if (j == 1)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "2";
    //                vdtl.GlCoaCode = PurchaseCode; //**** Purchase Code *******//
    //                vdtl.Particulars = "Purchase Payable Amount";
    //                vdtl.AccType = VouchManager.getAccType(PurchaseCode); //**** Purchase Code *******//
    //                vdtl.AmountDr = txtAddTot.Text.Replace(",", "");
    //                vdtl.AmountCr = "0";
    //                vdtl.Status = "A";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //        }
    //    }

    //    //************************************** END  Additional  Charge *********************

    //    //************************************** Carriage  Charge *********************
    //    if (Convert.ToDouble(txtCarriageCharge.Text) > 0)
    //    {
    //        VouchMst vmst = new VouchMst();
    //        vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //        vmst.ValueDate = txtGRNODate.Text;
    //        vmst.VchCode = "03";
    //        vmst.RefFileNo = "";
    //        vmst.VolumeNo = "";
    //        vmst.SerialNo = txtGRNO.Text.Trim();
    //        string CPerson = "";
    //        if (ddlCarriagePerson.SelectedItem.Text != "") { CPerson = ddlCarriagePerson.SelectedItem.Text; } else{CPerson="Delve";}
    //        vmst.Particulars = "Carriage Cost Payable Amount. - (" + txtGRNO.Text + "-" + CPerson + ")";
    //        vmst.ControlAmt = txtCarriageCharge.Text.Replace(",","");
    //        vmst.Payee = "CRC";
    //        vmst.CheckNo = "";
    //        vmst.CheqDate = "";
    //        vmst.CheqAmnt = "";
    //        vmst.MoneyRptNo = "";
    //        vmst.MoneyRptDate = "";
    //        vmst.TransType = "R";
    //        vmst.BookName = "AMB";
    //        vmst.EntryUser = Session["user"].ToString();
    //        vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //        vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
    //        vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
    //        vmst.Status = "A";
    //        vmst.AuthoUserType = Session["userlevel"].ToString();
    //        VouchManager.CreateVouchMst(vmst);               
    //        VouchDtl vdtl;
    //        for (int j = 0; j < 2; j++)
    //        {
    //            if (j == 0)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "1";
    //                if (ddlCarriagePerson.SelectedItem.Text != "")
    //                {
    //                    string CG_COA = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Labure", ddlCarriagePerson.SelectedValue);
    //                    vdtl.GlCoaCode = "1-" +CG_COA;
    //                    vdtl.Particulars = "On Carriage Person - " + ddlSupplier.SelectedItem.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + CG_COA);
    //                }
    //                else
    //                {
    //                    vdtl.GlCoaCode = OtherCarriageCode;// carriage charge Common COA
    //                    vdtl.Particulars = "On Carriage Person -Delve";
    //                    vdtl.AccType = VouchManager.getAccType(OtherCarriageCode); // carriage charge Common COA
    //                }
    //                vdtl.AmountDr = "0";
    //                vdtl.AmountCr = txtCarriageCharge.Text.Replace(",","");
    //                vdtl.Status = "A";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //            else if (j == 1)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "2";
    //                vdtl.GlCoaCode = PurchaseCode; //**** Purchase Code *******//
    //                vdtl.Particulars = "Carriage Cost Payable Amount";
    //                vdtl.AccType = VouchManager.getAccType(PurchaseCode);  //**** Purchase Code *******//
    //                vdtl.AmountDr = txtCarriageCharge.Text.Replace(",","");
    //                vdtl.AmountCr = "0";
    //                vdtl.Status = "A";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //        }
    //    }
    //    //************************************** End  Carriage  Charge *********************//

    //    //************************************** Labour charges *********************
    //    if (Convert.ToDouble(txtLabureCharge.Text) > 0)
    //    {
    //        VouchMst vmst = new VouchMst();
    //        vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //        vmst.ValueDate = txtGRNODate.Text;
    //        vmst.VchCode = "03";
    //        vmst.RefFileNo = "";
    //        vmst.VolumeNo = "";
    //        vmst.SerialNo = txtGRNO.Text.Trim();
    //        string LPerson = "";
    //        if (ddlLaburePerson.SelectedItem.Text != "") { LPerson = ddlLaburePerson.SelectedItem.Text; } else { LPerson = "Delve"; }
    //        vmst.Particulars = "Labour Cost Payable Amount. - (" + txtGRNO.Text + "-" + LPerson + ")";
    //        vmst.ControlAmt = txtLabureCharge.Text.Replace(",", "");
    //        vmst.Payee = "LBC";
    //        vmst.CheckNo = "";
    //        vmst.CheqDate = "";
    //        vmst.CheqAmnt = "";
    //        vmst.MoneyRptNo = "";
    //        vmst.MoneyRptDate = "";
    //        vmst.TransType = "R";
    //        vmst.BookName = "AMB";
    //        vmst.EntryUser = Session["user"].ToString();
    //        vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //        vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
    //        vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
    //        vmst.Status = "A";
    //        vmst.AuthoUserType = Session["userlevel"].ToString();
    //        VouchManager.CreateVouchMst(vmst);
    //        VouchDtl vdtl;
    //        for (int j = 0; j < 2; j++)
    //        {
    //            if (j == 0)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "1";
    //                if (ddlCarriagePerson.SelectedItem.Text != "")
    //                {
    //                    string CG_COA = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Labure", ddlCarriagePerson.SelectedValue);
    //                    vdtl.GlCoaCode = "1-" + CG_COA;
    //                    vdtl.Particulars = "On Labour Person - " + ddlSupplier.SelectedItem.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + CG_COA);
    //                }
    //                else
    //                {
    //                    vdtl.GlCoaCode = OtherLabureCode;// Labour charge Common COA
    //                    vdtl.Particulars = "On Labour Person -Delve";
    //                    vdtl.AccType = VouchManager.getAccType(OtherLabureCode); // Labour charge Common COA
    //                }
    //                vdtl.AmountDr = "0";
    //                vdtl.AmountCr = txtLabureCharge.Text.Replace(",","");
    //                vdtl.Status = "A";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //            else if (j == 1)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "2";
    //                vdtl.GlCoaCode = PurchaseCode; //**** Purchase Code *******//
    //                vdtl.Particulars = "Labour Cost Payable Amount";
    //                vdtl.AccType = VouchManager.getAccType(PurchaseCode); //**** Purchase Code *******//
    //                vdtl.AmountDr = txtLabureCharge.Text.Replace(",","");
    //                vdtl.AmountCr = "0";
    //                vdtl.Status = "A";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //        }
    //    }
    //    //************************************** End  Labour charges *********************

    //        //************************************** Other charges *********************
    //    if (Convert.ToDouble(txtOtherCharge.Text) > 0)
    //    {
    //        VouchMst vmst = new VouchMst();
    //        vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //        vmst.ValueDate = txtGRNODate.Text;
    //        vmst.VchCode = "03";
    //        vmst.RefFileNo = "";
    //        vmst.VolumeNo = "";
    //        vmst.SerialNo = txtGRNO.Text.Trim();              
    //        vmst.Particulars = "Other Cost Payable Amount. - (" + txtGRNO.Text + ")";
    //        vmst.ControlAmt = txtOtherCharge.Text.Replace(",","");
    //        vmst.Payee = "OC";
    //        vmst.CheckNo = "";
    //        vmst.CheqDate = "";
    //        vmst.CheqAmnt = "";
    //        vmst.MoneyRptNo = "";
    //        vmst.MoneyRptDate = "";
    //        vmst.TransType = "R";
    //        vmst.BookName = "AMB";
    //        vmst.EntryUser = Session["user"].ToString();
    //        vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //        vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
    //        vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
    //        vmst.Status = "A";
    //        vmst.AuthoUserType = Session["userlevel"].ToString();
    //        VouchManager.CreateVouchMst(vmst);
    //        VouchDtl vdtl;
    //        for (int j = 0; j < 2; j++)
    //        {
    //            if (j == 0)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "1";
    //                vdtl.GlCoaCode = Other;// Other Charge COA
    //                vdtl.Particulars = "On Other Cost";
    //                vdtl.AccType = VouchManager.getAccType(Other); // Other Charge COA                        
    //                vdtl.AmountDr = "0";
    //                vdtl.AmountCr = txtOtherCharge.Text.Replace(",","");
    //                vdtl.Status = "A";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //            else if (j == 1)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "2";
    //                vdtl.GlCoaCode = PurchaseCode; //**** Purchase Code *******//
    //                vdtl.Particulars = "Other Cost Payable Amount";
    //                vdtl.AccType = VouchManager.getAccType(PurchaseCode); //**** Purchase Code *******//
    //                vdtl.AmountDr = txtOtherCharge.Text.Replace(",","");
    //                vdtl.AmountCr = "0";
    //                vdtl.Status = "A";
    //                vdtl.BookName = "AMB";
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //        }
    //    }
    //        //************************************** End  Other charges *********************

      
    //}
    //private void PV_Acc_JurnalVoucher_Update()
    //{
    //    string PVSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='PV' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);
    //    string CRCSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='CRC' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);
    //    string LBCSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='LBC' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);
    //    string OCSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='OC' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);
    //    string AddSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='AddC' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);

    //    if (Convert.ToDouble(txtTotalAmount.Text) > 0)
    //    {
    //        VouchMst vmst = VouchManager.GetVouchMst(PVSerial.Trim());
    //        if (vmst != null)
    //        {

    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //            vmst.ValueDate = txtGRNODate.Text;
    //            vmst.VchCode = "03";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtGRNO.Text.Trim();
    //            if (ddlParty.SelectedItem.Text != "")
    //            { vmst.Particulars = "Items Purchase Payable Amount Party. - (" + txtGRNO.Text + "-" + ddlParty.SelectedItem.Text + ")"; }
    //            else { vmst.Particulars = "Items Purchase Payable Amount Supplier. - (" + txtGRNO.Text + "-" + ddlSupplier.SelectedItem.Text + ")"; }
    //            vmst.ControlAmt = txtTotItems.Text.Replace(",", "");
    //            vmst.Payee = "PV";
    //            vmst.CheckNo = txtChequeNo.Text;
    //            vmst.CheqDate = txtChequeDate.Text;
    //            vmst.CheqAmnt = txtChequeAmount.Text.Replace(",","");               
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
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "1";
    //                    //vdtl.GlCoaCode = "1-" + Session["Supplier_COA"].ToString();
    //                    //vdtl.Particulars = "On Supplier - " + ddlSupplier.SelectedItem.Text + "";
    //                    //vdtl.AccType = VouchManager.getAccType("1-" + Session["Supplier_COA"].ToString());
    //                    if (ddlParty.SelectedItem.Text != "")
    //                    {
    //                        vdtl.GlCoaCode = "1-" + Session["Party_COA"].ToString();
    //                        vdtl.Particulars = "On Party - " + ddlParty.SelectedItem.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Party_COA"].ToString());
    //                    }
    //                    else
    //                    {
    //                        vdtl.GlCoaCode = "1-" + Session["Supplier_COA"].ToString();
    //                        vdtl.Particulars = "On Supplier - " + ddlSupplier.SelectedItem.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Supplier_COA"].ToString());
    //                    }

    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = txtTotItems.Text.Replace(",", "");
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = "AMB";
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                else if (j == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "2";
    //                    vdtl.GlCoaCode = PurchaseCode; //**** Purchase Code *******//
    //                    vdtl.Particulars = "Purchase Payable Amount";
    //                    vdtl.AccType = VouchManager.getAccType(PurchaseCode); //**** Purchase Code *******//
    //                    vdtl.AmountDr = txtTotItems.Text.Replace(",", "");
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = "AMB";
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }

    //    //************************************** Additional  Charge *********************

    //    if (Convert.ToDouble(txtAddTot.Text) > 0)
    //    {
    //        VouchMst vmst = VouchManager.GetVouchMst(AddSerial.Trim());
    //        if (vmst != null)
    //        {
    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //            vmst.ValueDate = txtGRNODate.Text;
    //            vmst.VchCode = "03";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtGRNO.Text.Trim();
    //            vmst.Particulars = "Additional Charge Payable  in Supplier. - (" + txtGRNO.Text + "-" + ddlSupplier.SelectedItem.Text + ")";
    //            vmst.ControlAmt = txtAddTot.Text.Replace(",", "");
    //            vmst.Payee = "AddC";
    //            vmst.CheckNo = txtChequeNo.Text;
    //            vmst.CheqDate = txtChequeDate.Text;
    //            vmst.CheqAmnt = txtChequeAmount.Text.Replace(",", "");
    //            vmst.UpdateUser = Session["user"].ToString();
    //            vmst.UpdateDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");                 
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
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "1";
    //                    vdtl.GlCoaCode = "1-" + Session["Supplier_COA"].ToString();
    //                    vdtl.Particulars = "On Supplier - " + ddlSupplier.SelectedItem.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + Session["Supplier_COA"].ToString());
    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = txtAddTot.Text.Replace(",", "");
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = "AMB";
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                else if (j == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "2";
    //                    vdtl.GlCoaCode = AddSerial; //**** Additional Charge Code *******//
    //                    vdtl.Particulars = "Additional Charge";
    //                    vdtl.AccType = VouchManager.getAccType(AddSerial); //**** Additional Charge Code *******//
    //                    vdtl.AmountDr = txtAddTot.Text.Replace(",", "");
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = "AMB";
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }
    //    //************************************** END  Additional  Charge *********************

    //    //************************************** Carriage  Charge *********************
    //    if (Convert.ToDouble(txtCarriageCharge.Text) > 0)
    //    {

    //        VouchMst vmst = VouchManager.GetVouchMst(CRCSerial.Trim());
    //         if (vmst != null)
    //         {
    //             vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //             vmst.ValueDate = txtGRNODate.Text;
    //             vmst.VchCode = "03";
    //             vmst.RefFileNo = "";
    //             vmst.VolumeNo = "";
    //             vmst.SerialNo = txtGRNO.Text.Trim();
    //             string CPerson = "";
    //             if (ddlCarriagePerson.SelectedItem.Text != "") { CPerson = ddlCarriagePerson.SelectedItem.Text; } else { CPerson = "Delve"; }
    //             vmst.Particulars = "Carriage Cost Payable Amount. - (" + txtGRNO.Text + "-" + CPerson + ")";
    //             vmst.ControlAmt = txtCarriageCharge.Text.Replace(",","");
    //             vmst.Payee = "CRC";           
    //             vmst.UpdateUser = Session["user"].ToString().ToUpper();
    //             vmst.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
    //             vmst.AuthoUserType = Session["userlevel"].ToString();
    //             VouchManager.UpdateVouchMst(vmst);
    //             VouchManager.DeleteVouchDtl(vmst.VchSysNo);
    //             VouchDtl vdtl;
    //             for (int j = 0; j < 2; j++)
    //             {
    //                 if (j == 0)
    //                 {
    //                     vdtl = new VouchDtl();
    //                     vdtl.VchSysNo = vmst.VchSysNo;
    //                     vdtl.ValueDate = txtGRNODate.Text;
    //                     vdtl.LineNo = "1";
    //                     if (ddlCarriagePerson.SelectedItem.Text != "")
    //                     {
    //                         string CG_COA = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Labure", ddlCarriagePerson.SelectedValue);
    //                         vdtl.GlCoaCode = "1-" + CG_COA;
    //                         vdtl.Particulars = "On Carriage Person - " + ddlSupplier.SelectedItem.Text + "";
    //                         vdtl.AccType = VouchManager.getAccType("1-" + CG_COA);
    //                     }
    //                     else
    //                     {
    //                         vdtl.GlCoaCode = OtherCarriageCode;// carriage charge Common COA
    //                         vdtl.Particulars = "On Carriage Person -Delve";
    //                         vdtl.AccType = VouchManager.getAccType(OtherCarriageCode); // carriage charge Common COA
    //                     }
    //                     vdtl.AmountDr = "0";
    //                     vdtl.AmountCr = txtCarriageCharge.Text.Replace(",","");
    //                     vdtl.Status = vmst.Status;
    //                     vdtl.BookName = "AMB";
    //                     VouchManager.CreateVouchDtl(vdtl);
    //                 }
    //                 else if (j == 1)
    //                 {
    //                     vdtl = new VouchDtl();
    //                     vdtl.VchSysNo = vmst.VchSysNo;
    //                     vdtl.ValueDate = txtGRNODate.Text;
    //                     vdtl.LineNo = "2";
    //                     vdtl.GlCoaCode = PurchaseCode; //**** Purchase Code *******//
    //                     vdtl.Particulars = "Carriage Cost Payable Amount";
    //                     vdtl.AccType = VouchManager.getAccType(PurchaseCode);  //**** Purchase Code *******//
    //                     vdtl.AmountDr = txtCarriageCharge.Text.Replace(",","");
    //                     vdtl.AmountCr = "0";
    //                     vdtl.Status = vmst.Status;
    //                     vdtl.BookName = "AMB";
    //                     VouchManager.CreateVouchDtl(vdtl);
    //                 }
    //             }
    //         }
    //    }
    //    //************************************** End  Carriage  Charge *********************//

    //    //************************************** Labour charges *********************
    //    if (Convert.ToDouble(txtLabureCharge.Text) > 0)
    //    {
    //        VouchMst vmst = VouchManager.GetVouchMst(LBCSerial.Trim());
    //         if (vmst != null)
    //         {
    //             vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //             vmst.ValueDate = txtGRNODate.Text;
    //             vmst.VchCode = "03";
    //             vmst.RefFileNo = "";
    //             vmst.VolumeNo = "";
    //             vmst.SerialNo = txtGRNO.Text.Trim();
    //             string LPerson = "";
    //             if (ddlLaburePerson.SelectedItem.Text != "") { LPerson = ddlLaburePerson.SelectedItem.Text; } else { LPerson = "Delve"; }
    //             vmst.Particulars = "Labour Cost Payable Amount. - (" + txtGRNO.Text + "-" + LPerson + ")";
    //             vmst.ControlAmt = txtLabureCharge.Text.Replace(",","");
    //             vmst.Payee = "LBC";                
    //             vmst.UpdateUser = Session["user"].ToString().ToUpper();
    //             vmst.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
    //             vmst.AuthoUserType = Session["userlevel"].ToString();
    //             VouchManager.UpdateVouchMst(vmst);
    //             VouchManager.DeleteVouchDtl(vmst.VchSysNo);
    //             VouchDtl vdtl;
    //             for (int j = 0; j < 2; j++)
    //             {
    //                 if (j == 0)
    //                 {
    //                     vdtl = new VouchDtl();
    //                     vdtl.VchSysNo = vmst.VchSysNo;
    //                     vdtl.ValueDate = txtGRNODate.Text;
    //                     vdtl.LineNo = "1";
    //                     if (ddlCarriagePerson.SelectedItem.Text != "")
    //                     {
    //                         string CG_COA = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Labure", ddlCarriagePerson.SelectedValue);
    //                         vdtl.GlCoaCode = "1-" + CG_COA;
    //                         vdtl.Particulars = "On Labour Person - " + ddlSupplier.SelectedItem.Text + "";
    //                         vdtl.AccType = VouchManager.getAccType("1-" + CG_COA);
    //                     }
    //                     else
    //                     {
    //                         vdtl.GlCoaCode = OtherLabureCode;// Labour charge Common COA
    //                         vdtl.Particulars = "On Labour Person -Delve";
    //                         vdtl.AccType = VouchManager.getAccType(OtherLabureCode); // Labour charge Common COA
    //                     }
    //                     vdtl.AmountDr = "0";
    //                     vdtl.AmountCr = txtLabureCharge.Text.Replace(",","");
    //                     vdtl.Status = vmst.Status;
    //                     vdtl.BookName = "AMB";
    //                     VouchManager.CreateVouchDtl(vdtl);
    //                 }
    //                 else if (j == 1)
    //                 {
    //                     vdtl = new VouchDtl();
    //                     vdtl.VchSysNo = vmst.VchSysNo;
    //                     vdtl.ValueDate = txtGRNODate.Text;
    //                     vdtl.LineNo = "2";
    //                     vdtl.GlCoaCode = PurchaseCode; //**** Purchase Code *******//
    //                     vdtl.Particulars = "Labour Cost Payable Amount";
    //                     vdtl.AccType = VouchManager.getAccType(PurchaseCode); //**** Purchase Code *******//
    //                     vdtl.AmountDr = txtLabureCharge.Text.Replace(",","");
    //                     vdtl.AmountCr = "0";
    //                     vdtl.Status = vmst.Status;
    //                     vdtl.BookName = "AMB";
    //                     VouchManager.CreateVouchDtl(vdtl);
    //                 }
    //             }
    //         }
    //    }
    //    //************************************** End  Labour charges *********************

    //    //************************************** Other charges *********************
    //    if (Convert.ToDouble(txtOtherCharge.Text) > 0)
    //    {           
    //         VouchMst vmst = VouchManager.GetVouchMst(LBCSerial.Trim());
    //         if (vmst != null)
    //         {
    //             vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //             vmst.ValueDate = txtGRNODate.Text;
    //             vmst.VchCode = "03";
    //             vmst.RefFileNo = "";
    //             vmst.VolumeNo = "";
    //             vmst.SerialNo = txtGRNO.Text.Trim();
    //             vmst.Particulars = "Other Cost Payable Amount. - (" + txtGRNO.Text + ")";
    //             vmst.ControlAmt = txtOtherCharge.Text.Replace(",","");
    //             vmst.Payee = "OC";                 
    //             vmst.UpdateUser = Session["user"].ToString().ToUpper();
    //             vmst.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
    //             vmst.AuthoUserType = Session["userlevel"].ToString();
    //             VouchManager.UpdateVouchMst(vmst);
    //             VouchManager.DeleteVouchDtl(vmst.VchSysNo);
    //             VouchDtl vdtl;
    //             for (int j = 0; j < 2; j++)
    //             {
    //                 if (j == 0)
    //                 {
    //                     vdtl = new VouchDtl();
    //                     vdtl.VchSysNo = vmst.VchSysNo;
    //                     vdtl.ValueDate = txtGRNODate.Text;
    //                     vdtl.LineNo = "1";
    //                     vdtl.GlCoaCode = Other;// Other Charge COA
    //                     vdtl.Particulars = "On Other Cost";
    //                     vdtl.AccType = VouchManager.getAccType(Other); // Other Charge COA                        
    //                     vdtl.AmountDr = "0";
    //                     vdtl.AmountCr = txtOtherCharge.Text.Replace(",","");
    //                     vdtl.Status = vmst.Status;
    //                     vdtl.BookName = "AMB";
    //                     VouchManager.CreateVouchDtl(vdtl);
    //                 }
    //                 else if (j == 1)
    //                 {
    //                     vdtl = new VouchDtl();
    //                     vdtl.VchSysNo = vmst.VchSysNo;
    //                     vdtl.ValueDate = txtGRNODate.Text;
    //                     vdtl.LineNo = "2";
    //                     vdtl.GlCoaCode = PurchaseCode; //**** Purchase Code *******//
    //                     vdtl.Particulars = "Other Cost Payable Amount";
    //                     vdtl.AccType = VouchManager.getAccType(PurchaseCode); //**** Purchase Code *******//
    //                     vdtl.AmountDr = txtOtherCharge.Text.Replace(",","");
    //                     vdtl.AmountCr = "0";
    //                     vdtl.Status = vmst.Status;
    //                     vdtl.BookName = "AMB";
    //                     VouchManager.CreateVouchDtl(vdtl);
    //                 }
    //             }
    //         }
    //    }
    //    //************************************** End  Other charges *********************
    //}

    //private void PV_Acc_Debit_Voucher_For_Party_Save()
    //{
    //    if (Convert.ToDouble(txtTotalAmount.Text) > 0)
    //    {
    //        if ((ddlPaymentMethord.SelectedValue == "C" && Convert.ToDouble(txtChequeAmount.Text) > 0) || (ddlPaymentMethord.SelectedValue == "Q" && ddlChequeStatus.SelectedValue == "A" && Convert.ToDouble(txtChequeAmount.Text) > 0))
    //        {
    //            VouchMst vmst = new VouchMst();
    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //            vmst.ValueDate = txtGRNODate.Text;
    //            vmst.VchCode = "01";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtGRNO.Text.Trim();
    //            vmst.Particulars = "Taka Payment For Party. - (" + txtGRNO.Text + "-" + ddlParty.SelectedItem.Text + ")";
    //            vmst.ControlAmt = txtChequeAmount.Text.Replace(",", "");
    //            vmst.Payee = "PV";
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
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "1";
    //                    vdtl.GlCoaCode = "1-" + Session["Party_COA"].ToString();
    //                    vdtl.Particulars = "On Party Pay " +ddlParty.SelectedItem.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + Session["Party_COA"].ToString());
    //                    vdtl.AmountDr = txtChequeAmount.Text.Replace(",", "");
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = "U";
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                if (j == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "2";
    //                    if (ddlPaymentMethord.SelectedValue != "C")
    //                    {
    //                        string BankCOA = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    //                        vdtl.GlCoaCode = "1-" + BankCOA;
    //                        vdtl.Particulars = "Cash at Bank : Purchase Items Payment On Party -(" + txtGRNO.Text + "-" + ddlParty.SelectedItem.Text + ")";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + BankCOA);
    //                    }
    //                    else
    //                    {

    //                        vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                        vdtl.Particulars = "Cash at Hand :  Purchase Items Payment On Party -(" + txtGRNO.Text + "-" + ddlParty.SelectedItem.Text + ")";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());
    //                    }

    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = txtChequeAmount.Text.Replace(",", "");
    //                    vdtl.Status = "U";
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }         
    //}

    //private void PV_Acc_Debit_Voucher_For_Party_Update()
    //{
    //    string PVSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='PV' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);
    //    string CRCSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='CRC' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);
    //    string LBCSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='LBC' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);
    //    string OCSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='OC' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);

    //    if (Convert.ToDouble(txtTotalAmount.Text) > 0)
    //    {
    //        if ((ddlPaymentMethord.SelectedValue == "C" && Convert.ToDouble(txtTotPayment.Text) > 0) || (ddlPaymentMethord.SelectedValue == "Q" && ddlChequeStatus.SelectedValue == "A" && Convert.ToDouble(txtChequeAmount.Text) > 0))
    //        {
    //            VouchMst vmst = VouchManager.GetVouchMst(PVSerial.Trim());
    //            if (vmst != null)
    //            {
    //                vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //                vmst.ValueDate = txtGRNODate.Text;
    //                vmst.VchCode = "01";
    //                vmst.RefFileNo = "";
    //                vmst.VolumeNo = "";
    //                vmst.SerialNo = txtGRNO.Text.Trim();
    //                vmst.Particulars = "Taka Payment For Party .- (" + txtGRNO.Text + "-" + ddlParty.SelectedItem.Text + ")";
    //                vmst.ControlAmt = txtTotItems.Text.Replace(",", "");
    //                vmst.Payee = "PV";
    //                vmst.CheckNo = txtChequeNo.Text;
    //                vmst.CheqDate = txtChequeDate.Text;
    //                vmst.CheqAmnt = txtChequeAmount.Text.Replace(",", "");
    //                vmst.BookName = Session["book"].ToString();
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
    //                        vdtl.ValueDate = txtGRNODate.Text;
    //                        vdtl.LineNo = "1";
    //                        vdtl.GlCoaCode = "1-" + Session["Party_COA"].ToString();
    //                        vdtl.Particulars = "On Party Pay" + ddlParty.SelectedItem.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Party_COA"].ToString());
    //                        vdtl.AmountDr = txtTotItems.Text.Replace(",", "");
    //                        vdtl.AmountCr = "0";
    //                        vdtl.Status = vmst.Status;
    //                        vdtl.BookName = Session["book"].ToString();
    //                        VouchManager.CreateVouchDtl(vdtl);
    //                    }
    //                    if (j == 1)
    //                    {
    //                        vdtl = new VouchDtl();
    //                        vdtl.VchSysNo = vmst.VchSysNo;
    //                        vdtl.ValueDate = txtGRNODate.Text;
    //                        vdtl.LineNo = "2";
    //                        if (ddlPaymentMethord.SelectedValue != "C")
    //                        {
    //                            string BankCOA = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    //                            vdtl.GlCoaCode = "1-" + BankCOA;
    //                            vdtl.Particulars = "Cash at Bank : Purchase Items Payment On Party -(" + txtGRNO.Text + "-" + ddlParty.SelectedItem.Text + ")";
    //                            vdtl.AccType = VouchManager.getAccType("1-" + BankCOA);
    //                        }
    //                        else
    //                        {

    //                            vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                            vdtl.Particulars = "Cash at Hand :  Purchase Items Payment On Party -(" + txtGRNO.Text + "-" + ddlParty.SelectedItem.Text + ")";
    //                            vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());
    //                        }

    //                        vdtl.AmountDr = "0";
    //                        vdtl.AmountCr = txtTotItems.Text.Replace(",", "");
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
    //    if (Convert.ToDouble(txtTotalAmount.Text) > 0)
    //    {
    //        if ((ddlPaymentMethord.SelectedValue == "C" && Convert.ToDouble(txtChequeAmount.Text) > 0) || (ddlPaymentMethord.SelectedValue == "Q" && ddlChequeStatus.SelectedValue == "A" && Convert.ToDouble(txtChequeAmount.Text) > 0))
    //        {
    //            VouchMst vmst = new VouchMst();
    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //            vmst.ValueDate = txtGRNODate.Text;
    //            vmst.VchCode = "01";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtGRNO.Text.Trim();
    //            vmst.Particulars = "Taka Payment For Purchase Items in Supplier .- (" + txtGRNO.Text + "-" + ddlSupplier.SelectedItem.Text + ")";
    //            vmst.ControlAmt = txtChequeAmount.Text.Replace(",", "");
    //            vmst.Payee = "PV";
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
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "1";
    //                    vdtl.GlCoaCode = "1-" + Session["Supplier_COA"].ToString();
    //                    vdtl.Particulars = "On Supplier" + ddlSupplier.SelectedItem.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + Session["Supplier_COA"].ToString());
    //                    vdtl.AmountDr = txtChequeAmount.Text.Replace(",", "");
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = "U";
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                if (j == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "2";
    //                    if (ddlPaymentMethord.SelectedValue != "C")
    //                    {
    //                        string BankCOA = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    //                        vdtl.GlCoaCode = "1-" + BankCOA;
    //                        vdtl.Particulars = "Cash at Bank : Purchase Items Payment On Supplier -(" + txtGRNO.Text + "-" + ddlSupplier.SelectedItem.Text + ")";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + BankCOA);
    //                    }
    //                    else
    //                    {

    //                        vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                        vdtl.Particulars = "Cash at Hand :  Purchase Items Payment On Supplier -(" + txtGRNO.Text + "-" + ddlSupplier.SelectedItem.Text + ")";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());
    //                    }

    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = txtChequeAmount.Text.Replace(",", "");
    //                    vdtl.Status = "U";
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }      

    //    //************************************** Carriage  Charge *********************

    //    if (Convert.ToDouble(txtCarriageCharge.Text) > 0)
    //    {
    //        VouchMst vmst = new VouchMst();
    //        vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //        vmst.ValueDate = txtGRNODate.Text;
    //        vmst.VchCode = "01";
    //        vmst.RefFileNo = "";
    //        vmst.VolumeNo = "";
    //        vmst.SerialNo = txtGRNO.Text.Trim();
    //        string Person = "";
    //        if (ddlCarriagePerson.SelectedItem.Text != "") { Person = ddlLaburePerson.SelectedItem.Text; } else { Person = "Delve"; }
    //        vmst.Particulars = "Taka Payment For Carriage Cost .- (" + txtGRNO.Text + "-" + Person + ")";
    //        vmst.ControlAmt = txtCarriageCharge.Text.Replace(",","");
    //        vmst.Payee = "CRC";
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
    //        vmst.VchRefNo = "DV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
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
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "1";
    //                if (ddlCarriagePerson.SelectedItem.Text != "")
    //                {
    //                    string CG_COA = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Labure", ddlCarriagePerson.SelectedValue);
    //                    vdtl.GlCoaCode = "1-" + CG_COA;
    //                    vdtl.Particulars = "On Carriage Person - " + ddlSupplier.SelectedItem.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + CG_COA);
    //                }
    //                else
    //                {
    //                    vdtl.GlCoaCode = OtherCarriageCode;// carriage charge Common COA
    //                    vdtl.Particulars = "On Carriage Person - Delve";
    //                    vdtl.AccType = VouchManager.getAccType(OtherCarriageCode); // carriage charge Common COA
    //                }
    //                vdtl.AmountDr = txtCarriageCharge.Text.Replace(",","");
    //                vdtl.AmountCr = "0";
    //                vdtl.Status = "U";
    //                vdtl.BookName = Session["book"].ToString();
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //            if (j == 1)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "2";
    //                vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                vdtl.Particulars = "Cash at Hand :  Carriage Cost Payment On -(" + txtGRNO.Text + "-" +Person + ")";
    //                vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());                   
    //                vdtl.AmountDr = "0";
    //                vdtl.AmountCr = txtCarriageCharge.Text.Replace(",","");
    //                vdtl.Status = "U";
    //                vdtl.BookName = Session["book"].ToString();
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //        }
    //    }

    //    //************************************** End  Carriage  Charge *********************//

    //    //************************************** Labour  Charge *********************

    //    if (Convert.ToDouble(txtLabureCharge.Text) > 0)
    //    {
    //        VouchMst vmst = new VouchMst();
    //        vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //        vmst.ValueDate = txtGRNODate.Text;
    //        vmst.VchCode = "01";
    //        vmst.RefFileNo = "";
    //        vmst.VolumeNo = "";
    //        vmst.SerialNo = txtGRNO.Text.Trim();
    //        string Person = "";
    //        if (ddlLaburePerson.SelectedItem.Text != "") { Person = ddlLaburePerson.SelectedItem.Text; } else { Person = "Delve"; }
    //        vmst.Particulars = "Taka Payment For Labour Cost .- (" + txtGRNO.Text + "-" + Person + ")";
    //        vmst.ControlAmt = txtLabureCharge.Text.Replace(",","");//
    //        vmst.Payee = "LBC";
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
    //        vmst.VchRefNo = "DV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
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
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "1";
    //                if (ddlCarriagePerson.SelectedItem.Text != "")
    //                {
    //                    string CG_COA = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Labure", ddlCarriagePerson.SelectedValue);
    //                    vdtl.GlCoaCode = "1-" + CG_COA;
    //                    vdtl.Particulars = "On Labour Person - " + ddlSupplier.SelectedItem.Text + "";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + CG_COA);
    //                }
    //                else
    //                {
    //                    vdtl.GlCoaCode = OtherLabureCode;// Labour charge Common COA
    //                    vdtl.Particulars = "On Labour Person -Delve";
    //                    vdtl.AccType = VouchManager.getAccType(OtherLabureCode);// Labour charge Common COA
    //                }
    //                vdtl.AmountDr = txtLabureCharge.Text.Replace(",","");
    //                vdtl.AmountCr = "0";
    //                vdtl.Status = "U";
    //                vdtl.BookName = Session["book"].ToString();
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //            if (j == 1)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "2";
    //                vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                    vdtl.Particulars = "Cash at Hand :  Labour Cost Payment On -(" + txtGRNO.Text + "-" + Person + ")";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());
    //                vdtl.AmountDr = "0";
    //                vdtl.AmountCr = txtLabureCharge.Text.Replace(",","");
    //                vdtl.Status = "U";
    //                vdtl.BookName = Session["book"].ToString();
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //        }
    //    }

    //    //************************************** End  Labour  Charge *********************//

    //    //************************************** Other  Charge *********************

    //    if (Convert.ToDouble(txtOtherCharge.Text) > 0)
    //    {
    //        VouchMst vmst = new VouchMst();
    //        vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //        vmst.ValueDate = txtGRNODate.Text.Trim();
    //        vmst.VchCode = "01";
    //        vmst.RefFileNo = "";
    //        vmst.VolumeNo = "";
    //        vmst.SerialNo = txtGRNO.Text;
    //        string Person = "";
    //        if (ddlLaburePerson.SelectedItem.Text != "") { Person = ddlLaburePerson.SelectedItem.Text; } else { Person = "Delve"; }
    //        vmst.Particulars = "Taka Payment For Other Cost .- (" + txtGRNO.Text + "-" + Person + ")";
    //        vmst.ControlAmt = txtOtherCharge.Text.Replace(",","");//
    //        vmst.Payee = "OC";
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
    //        vmst.VchRefNo = "DV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
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
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "1";
    //                vdtl.GlCoaCode = Other;// Other charge Common COA
    //                vdtl.Particulars = "On Other Cost -Delve";
    //                vdtl.AccType = VouchManager.getAccType(Other);// Other charge Common COA                       
    //                vdtl.AmountDr = txtOtherCharge.Text.Replace(",","");
    //                vdtl.AmountCr = "0";
    //                vdtl.Status = "U";
    //                vdtl.BookName = Session["book"].ToString();
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //            if (j == 1)
    //            {
    //                vdtl = new VouchDtl();
    //                vdtl.VchSysNo = vmst.VchSysNo;
    //                vdtl.ValueDate = txtGRNODate.Text;
    //                vdtl.LineNo = "2";
    //                vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                    vdtl.Particulars = "Cash at Hand :  Other Cost Payment On -(" + txtGRNO.Text + ")";
    //                    vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());
    //                vdtl.AmountDr = "0";
    //                vdtl.AmountCr = txtOtherCharge.Text.Replace(",","");
    //                vdtl.Status = "U";
    //                vdtl.BookName = Session["book"].ToString();
    //                VouchManager.CreateVouchDtl(vdtl);
    //            }
    //        }
    //    }

    //        //************************************** End  Other  Charge *********************//       
    //}
    //private void PV_Acc_Debit_Voucher_Update()
    //{
    //    string PVSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='PV' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);
    //    string CRCSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='CRC' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);
    //    string LBCSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='LBC' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);
    //    string OCSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO", "t1.PAYEE='OC' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1", txtGRNO.Text);

    //    if (Convert.ToDouble(txtTotalAmount.Text) > 0)
    //    {
    //        if ((ddlPaymentMethord.SelectedValue == "C" && Convert.ToDouble(txtTotPayment.Text) > 0) || (ddlPaymentMethord.SelectedValue == "Q" && ddlChequeStatus.SelectedValue == "A" && Convert.ToDouble(txtChequeAmount.Text) > 0))
    //        {
    //            VouchMst vmst = VouchManager.GetVouchMst(PVSerial.Trim());
    //            if (vmst != null)
    //            {
    //                vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //                vmst.ValueDate = txtGRNODate.Text;
    //                vmst.VchCode = "01";
    //                vmst.RefFileNo = "";
    //                vmst.VolumeNo = "";
    //                vmst.SerialNo = txtGRNO.Text.Trim();
    //                vmst.Particulars = "Taka Payment For Purchase Items in Supplier .- (" + txtGRNO.Text + "-" + ddlSupplier.SelectedItem.Text + ")";
    //                vmst.ControlAmt = txtTotItems.Text.Replace(",", "");
    //                vmst.Payee = "PV";
    //                vmst.CheckNo = txtChequeNo.Text;
    //                vmst.CheqDate = txtChequeDate.Text;
    //                vmst.CheqAmnt = txtChequeAmount.Text.Replace(",", "");
    //                vmst.BookName = Session["book"].ToString();
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
    //                        vdtl.ValueDate = txtGRNODate.Text;
    //                        vdtl.LineNo = "1";
    //                        vdtl.GlCoaCode = "1-" + Session["Supplier_COA"].ToString();
    //                        vdtl.Particulars = "On Supplier" + ddlSupplier.SelectedItem.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Supplier_COA"].ToString());
    //                        vdtl.AmountDr = txtTotItems.Text.Replace(",", "");
    //                        vdtl.AmountCr = "0";
    //                        vdtl.Status = vmst.Status;
    //                        vdtl.BookName = Session["book"].ToString();
    //                        VouchManager.CreateVouchDtl(vdtl);
    //                    }
    //                    if (j == 1)
    //                    {
    //                        vdtl = new VouchDtl();
    //                        vdtl.VchSysNo = vmst.VchSysNo;
    //                        vdtl.ValueDate = txtGRNODate.Text;
    //                        vdtl.LineNo = "2";
    //                        if (ddlPaymentMethord.SelectedValue != "C")
    //                        {
    //                            string BankCOA = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    //                            vdtl.GlCoaCode = "1-" + BankCOA;
    //                            vdtl.Particulars = "Cash at Bank : Purchase Items Payment On Supplier -(" + txtGRNO.Text + "-" + ddlSupplier.SelectedItem.Text + ")";
    //                            vdtl.AccType = VouchManager.getAccType("1-" + BankCOA);
    //                        }
    //                        else
    //                        {

    //                            vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                            vdtl.Particulars = "Cash at Hand :  Purchase Items Payment On Supplier -(" + txtGRNO.Text + "-" + ddlSupplier.SelectedItem.Text + ")";
    //                            vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());
    //                        }

    //                        vdtl.AmountDr = "0";
    //                        vdtl.AmountCr = txtTotItems.Text.Replace(",", "");
    //                        vdtl.Status = vmst.Status;
    //                        vdtl.BookName = Session["book"].ToString();
    //                        VouchManager.CreateVouchDtl(vdtl);
    //                    }
    //                }
    //            }
    //        }
    //    }        

    //    //************************************** Carriage  Charge *********************

    //    if (Convert.ToDouble(txtCarriageCharge.Text) > 0)
    //    {
    //        VouchMst vmst = VouchManager.GetVouchMst(CRCSerial.Trim());
    //        if (vmst != null)
    //        {
    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //            vmst.ValueDate = txtGRNODate.Text;
    //            vmst.VchCode = "01";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtGRNO.Text.Trim();
    //            string Person = "";
    //            if (ddlCarriagePerson.SelectedItem.Text != "") { Person = ddlLaburePerson.SelectedItem.Text; } else { Person = "Delve"; }
    //            vmst.Particulars = "Taka Payment For Carriage Cost .- (" + txtGRNO.Text + "-" + Person + ")";
    //            vmst.ControlAmt = txtCarriageCharge.Text.Replace(",","");
    //            vmst.Payee = "CRC";
    //            vmst.BookName = Session["book"].ToString();
    //            vmst.UpdateUser = Session["user"].ToString().ToUpper();
    //            vmst.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
    //            vmst.AuthoUserType = Session["userlevel"].ToString();
    //            VouchManager.UpdateVouchMst(vmst);
    //            VouchManager.DeleteVouchDtl(vmst.VchSysNo);
    //            //clsPaymentDtl paydtl;
    //            VouchDtl vdtl;
    //            for (int j = 0; j < 2; j++)
    //            {
    //                if (j == 0)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "1";
    //                    if (ddlCarriagePerson.SelectedItem.Text != "")
    //                    {
    //                        string CG_COA = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Labure", ddlCarriagePerson.SelectedValue);
    //                        vdtl.GlCoaCode = "1-" + CG_COA;
    //                        vdtl.Particulars = "On Carriage Person - " + ddlSupplier.SelectedItem.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + CG_COA);
    //                    }
    //                    else
    //                    {
    //                        vdtl.GlCoaCode = OtherCarriageCode;// carriage charge Common COA
    //                        vdtl.Particulars = "On Carriage Person -Delve";
    //                        vdtl.AccType = VouchManager.getAccType(OtherCarriageCode); // carriage charge Common COA
    //                    }
    //                    vdtl.AmountDr = txtCarriageCharge.Text.Replace(",","");
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                if (j == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "2";
    //                    vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                        vdtl.Particulars = "Cash at Hand :  Carriage Cost Payment On -(" + txtGRNO.Text + "-" + Person + ")";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());
    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = txtCarriageCharge.Text.Replace(",","");
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }

    //    //************************************** End  Carriage  Charge *********************//

    //    //************************************** Labour  Charge *********************

    //    if (Convert.ToDouble(txtLabureCharge.Text) > 0)
    //    {          
    //        VouchMst vmst = VouchManager.GetVouchMst(LBCSerial.Trim());
    //        if (vmst != null)
    //        {
    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //            vmst.ValueDate = txtGRNODate.Text;
    //            vmst.VchCode = "01";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtGRNO.Text.Trim();
    //            string Person = "";
    //            if (ddlLaburePerson.SelectedItem.Text != "") { Person = ddlLaburePerson.SelectedItem.Text; } else { Person = "Delve"; }
    //            vmst.Particulars = "Taka Payment For Labour Cost .- (" + txtGRNO.Text + "-" + Person + ")";
    //            vmst.ControlAmt = txtLabureCharge.Text.Replace(",","");//
    //            vmst.Payee = "LBC";             
    //            vmst.BookName = Session["book"].ToString();
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
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "1";
    //                    if (ddlCarriagePerson.SelectedItem.Text != "")
    //                    {
    //                        string CG_COA = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Labure", ddlCarriagePerson.SelectedValue);
    //                        vdtl.GlCoaCode = "1-" + CG_COA;
    //                        vdtl.Particulars = "On Labour Person - " + ddlSupplier.SelectedItem.Text + "";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + CG_COA);
    //                    }
    //                    else
    //                    {
    //                        vdtl.GlCoaCode = OtherLabureCode;// Labour charge Common COA
    //                        vdtl.Particulars = "On Labour Person -Delve";
    //                        vdtl.AccType = VouchManager.getAccType(OtherLabureCode);// LABOUR charge Common COA
    //                    }
    //                    vdtl.AmountDr = txtLabureCharge.Text.Replace(",","");
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                if (j == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "2";
    //                    vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                        vdtl.Particulars = "Cash at Hand :  Labour Cost Payment On -(" + txtGRNO.Text + "-" + Person + ")";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());
    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = txtLabureCharge.Text.Replace(",","");
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }
    //    //************************************** End  Labour  Charge *********************//

    //    //************************************** Other  Charge *********************

    //    if (Convert.ToDouble(txtOtherCharge.Text) > 0)
    //    {           
    //        VouchMst vmst = VouchManager.GetVouchMst(OCSerial.Trim());
    //        if (vmst != null)
    //        {
    //            vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
    //            vmst.ValueDate = txtGRNODate.Text.Trim();
    //            vmst.VchCode = "01";
    //            vmst.RefFileNo = "";
    //            vmst.VolumeNo = "";
    //            vmst.SerialNo = txtGRNO.Text;
    //            string Person = "";
    //            if (ddlLaburePerson.SelectedItem.Text != "") { Person = ddlLaburePerson.SelectedItem.Text; } else { Person = "Delve"; }
    //            vmst.Particulars = "Taka Payment For Other Cost .- (" + txtGRNO.Text + "-" + Person + ")";
    //            vmst.ControlAmt = txtOtherCharge.Text.Replace(",","");//
    //            vmst.Payee = "OC";                
    //            vmst.BookName = Session["book"].ToString();
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
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "1";
    //                    vdtl.GlCoaCode = Other;// Other charge Common COA
    //                    vdtl.Particulars = "On Other Cost -Delve";
    //                    vdtl.AccType = VouchManager.getAccType(Other);// Other charge Common COA                       
    //                    vdtl.AmountDr = txtOtherCharge.Text.Replace(",","");
    //                    vdtl.AmountCr = "0";
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //                if (j == 1)
    //                {
    //                    vdtl = new VouchDtl();
    //                    vdtl.VchSysNo = vmst.VchSysNo;
    //                    vdtl.ValueDate = txtGRNODate.Text;
    //                    vdtl.LineNo = "2";
    //                    //if (ddlPaymentMethord.SelectedValue != "C")
    //                    //{
    //                    //    string BankCOA = IdManager.GetShowSingleValueString("gl_coa_code", "bank_id", "bank_info", ddlBank.SelectedValue);
    //                    //    vdtl.GlCoaCode = "1-" + BankCOA;
    //                    //    vdtl.Particulars = "Cash at Bank : Other Cost Payment On -(" + txtGRNO.Text + ")";
    //                    //    vdtl.AccType = VouchManager.getAccType("1-" + BankCOA);
    //                    //}
    //                    //else
    //                    //{

    //                        vdtl.GlCoaCode = "1-" + Session["Cash_Code"].ToString();
    //                        vdtl.Particulars = "Cash at Hand :  Other Cost Payment On -(" + txtGRNO.Text + ")";
    //                        vdtl.AccType = VouchManager.getAccType("1-" + Session["Cash_Code"].ToString());
    //                    //}

    //                    vdtl.AmountDr = "0";
    //                    vdtl.AmountCr = txtOtherCharge.Text.Replace(",","");
    //                    vdtl.Status = vmst.Status;
    //                    vdtl.BookName = Session["book"].ToString();
    //                    VouchManager.CreateVouchDtl(vdtl);
    //                }
    //            }
    //        }
    //    }

    //    //************************************** End  Other  Charge *********************//       
    //}

    protected void Delete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            PurchaseVoucherInfo purmst = PurchaseVoucherManager.GetPurchaseMst(txtID.Text.Trim());
            if (purmst != null)
            {
                purmst.ID = txtID.Text;
                purmst.GoodsReceiveNo = txtGRNO.Text.Trim();
                DataTable dtOldStk = (DataTable) ViewState["OldStock"];
                PurchaseVoucherManager.DeletePurchaseVoucher(purmst, dtOldStk);
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been delete successfully...!!');", true);
                btnDelete.Enabled = false;
                RefreshAll();
                btnSave.Enabled = false;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Select Item...!!');", true);
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }
    protected void Find_Click(object sender, EventArgs e)
    {

    }
    protected void Clear_Click(object sender, EventArgs e)
    {
        Response.Redirect("PurchaseVoucher.aspx?mno=5.18");
        //RefreshAll();
    }

    private void RefreshAll()
    {
        ClearFields();
        txtID.Text = txtSearchChallanNo.Text = txtFromDate.Text = txtToDAte.Text = txtSearchGrNO.Text = txtSearchSuplier.Text = lblSupplierID.Text = string.Empty;
        Session["Cash_Code"] = "";
        DropDownListValue();
        dgPVDetailsDtl.DataSource = null;
        dgPVDetailsDtl.DataBind();
        PanelHistory.Visible = btnNew.Visible = true;
        tabVch.Visible = false;
        Session["Cash_Code"] = IdManager.GetShowSingleValueString("CASH_CODE", "BOOK_NAME", "GL_SET_OF_BOOKS", "AMB");
        DataTable dt = PurchaseVoucherManager.GetShowPurchaseMst();
        dgPVMst.DataSource = dt;
        Session["PvMst"] = dt;
        lblOrNo.Text = ""; txtID.Text = "";
        dgPVMst.DataBind();      
        btnDelete.Enabled =btnSave.Enabled = true;
        txtGRNO.Enabled = txtChallanNo.Enabled = txtPO.Enabled = txtGRNODate.Enabled = txtChallanDate.Enabled = ddlSupplier.Enabled = txtRemarks.Enabled = false;
        txtGRNO.Focus();
    }
    //************* Pv Items Details ******//
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

                    decimal totAdd = decimal.Parse(((Label)e.Row.FindControl("lblTotal")).Text)+((decimal.Parse(((Label)e.Row.FindControl("lblTotal")).Text) * decimal.Parse(((DataRowView)e.Row.DataItem)["Additional"].ToString())) / 100);
                    ((Label)e.Row.FindControl("lblAddTotal")).Text = totAdd.ToString("N2");
                   
                }
                e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[7].Attributes.Add("style", "display:none");
                e.Row.Cells[6].Attributes.Add("style", "display:none");   
            }       
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[7].Attributes.Add("style", "display:none");
                e.Row.Cells[6].Attributes.Add("style", "display:none");  
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[7].Attributes.Add("style", "display:none");
                e.Row.Cells[6].Attributes.Add("style", "display:none");  
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
        if (Session["purdtl"] != null)
        {
            DataTable dtDtlGrid = (DataTable)Session["purdtl"];
            dtDtlGrid.Rows.RemoveAt(dgPVDetailsDtl.Rows[e.RowIndex].DataItemIndex);
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
                dgPVDetailsDtl.DataSource = dtDtlGrid;
                dgPVDetailsDtl.DataBind();
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

    protected void dgPurMst_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgPVMst.DataSource = Session["PvMst"];
        dgPVMst.PageIndex = e.NewPageIndex;
        dgPVMst.DataBind();
    }
    protected void dgPurMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtID.Text = dgPVMst.SelectedRow.Cells[9].Text;
        btnFind_Click(sender, e);
    }
    protected void dgPVMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[9].Attributes.Add("style", "display:none");
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
    //*********************** PV Details ********************************//
    public DataTable PopulateMeasure()
    {
        dtmsr = ItemManager.GetMeasure();
        DataRow dr = dtmsr.NewRow();
        dtmsr.Rows.InsertAt(dr, 0);
        return dtmsr;
    }
    protected void txtItemCode_TextChanged(object sender, EventArgs e)
    {

    }
    protected void txtItemDesc_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dtdtl = (DataTable)Session["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        DataTable dt = ItemManager.GetItems(((TextBox)gvr.FindControl("txtItemDesc")).Text);
        if (dt.Rows.Count > 0)
        {
            dtdtl.Rows.Remove(dr);
            dr = dtdtl.NewRow();
            dr["ID"] = ((DataRow)dt.Rows[0])["ID"].ToString();
            dr["item_desc"] = ((DataRow)dt.Rows[0])["item_desc"].ToString();
            dr["item_code"] = ((DataRow)dt.Rows[0])["item_code"].ToString();
            dr["msr_unit_code"] = ((DataRow)dt.Rows[0])["msr_unit_code"].ToString();
            dr["item_rate"] = "0";
            dr["qnty"] = "0";
            dr["Additional"] = "0";
            dr["UMO"] = ((DataRow)dt.Rows[0])["UMO"].ToString();
            dr["BrandName"] = ((DataRow)dt.Rows[0])["BrandName"].ToString();
            dtdtl.Rows.InsertAt(dr, gvr.DataItemIndex);
        }
        //string found = "";
        //foreach (DataRow drd in dtdtl.Rows)
        //{
        //    if (drd["item_code"].ToString() == "" && drd["item_desc"].ToString() == "")
        //    {
        //        found = "Y";
        //    }
        //}
        //if (found == "")
        //{
        //    DataRow drd = dtdtl.NewRow();
        //    dtdtl.Rows.Add(drd);
        //}
        dgPVDetailsDtl.DataSource = dtdtl;
        dgPVDetailsDtl.DataBind();
        ShowFooterTotal();
        //Session["purdtl"] = dtdtl;
       // ((TextBox)gvr.FindControl("txtItemRate")).Focus();
        ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 1].FindControl("txtItemRate")).Focus();
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
        
        if (Session["purdtl"] != null)
        {
            DataTable dt = (DataTable)Session["purdtl"];    
            foreach (DataRow drp in dt.Rows)
            {
                if (drp["ID"].ToString() != "" && drp["item_rate"].ToString() != "" && drp["qnty"].ToString() != "")
                {
                    totRat += decimal.Parse(drp["item_rate"].ToString());
                    totQty += decimal.Parse(drp["qnty"].ToString());
                    totItemsP += decimal.Parse(drp["item_rate"].ToString()) * decimal.Parse(drp["qnty"].ToString());
                    totA += decimal.Parse(drp["Additional"].ToString());

                    //totAddi += (totItemsP * decimal.Parse(drp["Additional"].ToString())) / 100;
                    Total += decimal.Parse(drp["item_rate"].ToString()) * decimal.Parse(drp["qnty"].ToString()); ;
                }
            }
            txtAddTot.Text = totAddi.ToString("N2");
            txtTotItems.Text = totItemsP.ToString("N2");
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
        priceDr = Total;
        cell.Text = Total.ToString("N0");
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        row.Font.Bold = true;
        row.BackColor = System.Drawing.Color.LightGray;
        if (dgPVDetailsDtl.Rows.Count > 0)
        {
            dgPVDetailsDtl.Controls[0].Controls.Add(row);
        }
        txtTotalAmount.Text = Total.ToString("N0").Replace(",", "");
        txtDue.Text = Total.ToString("N0").Replace(",", "");
        // while using Total Paid field separately
        //row.Attributes.Add("style", "display:none");
    }



    //*************************  txtItemsRate_TextChanged *******************//

    protected void txtItemsRate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)Session["purdtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                dr["ID"] = dr["ID"].ToString();
                dr["item_desc"] = dr["item_desc"].ToString();
                dr["item_code"] = dr["item_code"].ToString();
                dr["msr_unit_code"] = dr["msr_unit_code"].ToString();
                dr["item_rate"] = ((TextBox)gvr.FindControl("txtItemRate")).Text;
                if (((TextBox)gvr.FindControl("txtQnty")).Text == "") { dr["qnty"] = "0"; }
                dr["qnty"] = ((TextBox)gvr.FindControl("txtQnty")).Text;

            }
            string found = "";
            foreach (DataRow drd in dt.Rows)
            {
                if (drd["ID"].ToString() == "" && drd["item_desc"].ToString() == "")
                {
                    found = "Y";
                }
            }
            if (found == "")
            {
                DataRow drd = dt.NewRow();
                dt.Rows.Add(drd);
            }
            dgPVDetailsDtl.DataSource = dt;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 2].FindControl("txtQnty")).Focus();
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
            DataTable dt = (DataTable)Session["purdtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                dr["ID"] = dr["ID"].ToString();
                dr["item_desc"] = dr["item_desc"].ToString();
                dr["item_code"] = dr["item_code"].ToString();
                dr["msr_unit_code"] = dr["msr_unit_code"].ToString();
                dr["item_rate"] = ((TextBox)gvr.FindControl("txtItemRate")).Text;
                if (((TextBox)gvr.FindControl("txtQnty")).Text == "") { dr["qnty"] = "0"; }
                dr["qnty"] = ((TextBox)gvr.FindControl("txtQnty")).Text;

            }
            string found = "";
            foreach (DataRow drd in dt.Rows)
            {
                if (drd["ID"].ToString() == "" && drd["item_desc"].ToString() == "")
                {
                    found = "Y";
                }
            }
            if (found == "")
            {
                DataRow drd = dt.NewRow();
                dt.Rows.Add(drd);
            }
            dgPVDetailsDtl.DataSource = dt;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 1].FindControl("txtItemDesc")).Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
    //*************************  txtAdditional_TextChanged *******************//

    protected void txtAdditional_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)Session["purdtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                if (((TextBox)gvr.FindControl("txtAdditional")).Text == "") { dr["Additional"] = "0"; }
                dr["Additional"] = ((TextBox)gvr.FindControl("txtAdditional")).Text;
            }
            dgPVDetailsDtl.DataSource = dt;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 1].FindControl("txtItemDesc")).Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
    protected void ddlSupplier_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["Address"] = IdManager.GetShowSingleValueString("Address1", "ID", "Supplier", ddlSupplier.SelectedValue);
        ViewState["Phone"] = IdManager.GetShowSingleValueString("Mobile", "ID", "Supplier", ddlSupplier.SelectedValue);
        Session["Supplier_COA"] = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Supplier", ddlSupplier.SelectedValue);
        lblPhoneNo.Text = ViewState["Phone"].ToString();
    }
    protected void ddlParty_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["AddressParty"] = IdManager.GetShowSingleValueString("Address", "ID", "PartyInfo", ddlParty.SelectedValue);
        ViewState["PhoneParty"] = IdManager.GetShowSingleValueString("Phone", "ID", "PartyInfo", ddlParty.SelectedValue);
        Session["Party_COA"] = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "PartyInfo", ddlParty.SelectedValue); 
    }
    protected void txtPO_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = PurchaseOrderManager.GetShowOrder(txtPO.Text);
        if (dt.Rows.Count > 0)
        {
            txtPO.Text = dt.Rows[0]["PO"].ToString();
            txtPODate.Text =Convert.ToDateTime(dt.Rows[0]["PODate"]).ToString("dd/MM/yyyy");
            ddlSupplier.SelectedValue = dt.Rows[0]["SupplierID"].ToString();
            //txtSupplierPhone.Text = IdManager.GetShowSingleValueString("Phone", "ID", "Supplier", ddlSupplier.SelectedValue);
            Session["Supplier_COA"] = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Supplier", ddlSupplier.SelectedValue);
            lblOrNo.Text = dt.Rows[0]["ID"].ToString();
            DataTable dt1 = PurchaseOrderManager.GetPurchaseOrderItemsDetails(dt.Rows[0]["ID"].ToString());
            dgPVDetailsDtl.DataSource = dt1;
            Session["purdtl"] = dt1;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            tabVch.Visible = true;
           // dgPOrderMst.Visible = false;
            PVI_UP.Update();
            PVIesms_UP.Update();
            txtGRNO.Enabled = txtPO.Enabled = txtGRNODate.Enabled = ddlSupplier.Enabled =txtPODate.Enabled= false;
        }
    }
    protected void ddlPaymentMethord_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlPaymentMethord.SelectedValue == "C")
        { 
            VisiblePayment(false,false,false,false,false,false,false,false);
            lblAmount.Text = "Cash Amount ";
            ddlChequeStatus.SelectedIndex = -1;
        }
        else if (ddlPaymentMethord.SelectedValue == "A")
        {
            VisiblePayment(false, false, false, false, false, false, false, false);
            lblAmount.Text = "Advance Amount ";
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
    }
    public void VisiblePayment(bool lblBank,bool Bank,bool lblChkNo,bool ChkNo,bool lblChkDate,bool chkdate,bool lblChkStatus,bool chkStatus)
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
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = PVReturnManager.GetShowPVMasterInfo(txtSearchGrNO.Text);
        if (dt.Rows.Count > 0)
        {
            txtID.Text = dt.Rows[0]["ID"].ToString();
            //txtID.Text = dgPVMst.SelectedRow.Cells[7].Text;
            // btnFind_Click(sender, e);

        }
        else
        {
            txtID.Text = "";
            txtSearchGrNO.Text = "";
        }
    }
    protected void btnFind_Click(object sender, EventArgs e)
    {

        PurchaseVoucherInfo purmst = PurchaseVoucherManager.GetPurchaseMst(txtID.Text);
        if (purmst != null)
        {
            txtChallanNo.Enabled = txtPO.Enabled = txtGRNODate.Enabled = txtChallanDate.Enabled = ddlSupplier.Enabled = txtRemarks.Enabled = true;
            //txtID.Text = dgPVMst.SelectedRow.Cells[7].Text;
            txtGRNO.Text = purmst.GoodsReceiveNo;
            txtGRNODate.Text = purmst.GoodsReceiveDate;
            txtPO.Text = purmst.PurchaseOrderNo;
            txtPODate.Text = purmst.PurchaseOrderDate;
            txtChallanNo.Text = purmst.ChallanNo;
            txtChallanDate.Text = purmst.ChallanDate;
            ddlSupplier.SelectedValue = purmst.Supplier;
            ViewState["Address"] = IdManager.GetShowSingleValueString("Address1", "ID", "Supplier", ddlSupplier.SelectedValue);
            ViewState["Phone"] = IdManager.GetShowSingleValueString("Phone", "ID", "Supplier", ddlSupplier.SelectedValue);
            Session["Supplier_COA"] = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Supplier", ddlSupplier.SelectedValue);
           
            txtRemarks.Text = purmst.Remarks;
            txtTotalAmount.Text = Convert.ToDouble(purmst.TotalAmount).ToString("N0");
            txtTotPayment.Text = Convert.ToDouble(purmst.TotalPayment).ToString("N0");
            ddlCarriagePerson.SelectedValue = purmst.CarriagePerson;
            txtCarriageCharge.Text = Convert.ToDouble(purmst.CarriageCharge).ToString("N0");
            ddlLaburePerson.SelectedValue = purmst.LaburePerson;
            txtLabureCharge.Text = Convert.ToDouble(purmst.LabureCharge).ToString("N0");
            txtOtherCharge.Text = Convert.ToDouble(purmst.OtherCharge).ToString("N0");
            ddlPaymentMethord.SelectedValue = purmst.PaymentMethord.Trim();
            txtDue.Text = Convert.ToDouble(purmst.Due).ToString("N0");
            ddlChequeStatus.SelectedValue = purmst.ChkStatus;
            if (purmst.PartyID != "0" || purmst.PartyID !="")
            {
                if (purmst.PartyID != "1")
                {
                    ddlParty.SelectedValue = purmst.PartyID;
                    ViewState["AddressParty"] = IdManager.GetShowSingleValueString("Address", "ID", "PartyInfo", ddlParty.SelectedValue);
                    ViewState["PhoneParty"] = IdManager.GetShowSingleValueString("Phone", "ID", "PartyInfo", ddlParty.SelectedValue);
                    Session["Party_COA"] = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "PartyInfo", ddlParty.SelectedValue);
                }
            }
            txtShiftmentNo.Text = purmst.ShiftmentNO;
            lblShiftmentID.Text = purmst.ShiftmentID;
            if (purmst.PaymentMethord.Trim() != "C")
            {
                VisiblePayment(true, true, true, true, true, true, true, true);
                ddlBank.SelectedValue = purmst.BankId;
                txtChequeNo.Text = purmst.ChequeNo;
                txtChequeDate.Text = purmst.ChequeDate;
                txtChequeAmount.Text = Convert.ToDouble(purmst.ChequeAmount).ToString("N0");
            }
            else
            {
                VisiblePayment(false, false, false, false, false, false, false, false);
            }
            DataTable dt = PurchaseVoucherManager.GetPurchaseItemsDetails(txtID.Text);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                dgPVDetailsDtl.DataSource = dt;
                Session["purdtl"] = dt;
                ViewState["OldStock"] = dt;
                dgPVDetailsDtl.DataBind();
                ShowFooterTotal();
            }
            tabVch.Visible = true;
            PanelHistory.Visible = btnNew.Visible = false;
            PVI_UP.Update();
            PVIesms_UP.Update();
            UPPaymentMtd.Update();
        }
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (ddlPaymentMethord.SelectedValue == "C")
        {
            CashReport();
        }
        else
        {
            BankReport();
        }
    }

    //*************************  Check Popup  *******************//


    protected void btnClientSave_Click(object sender, EventArgs e)
    {
       
        if (txtvalue.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Supplier Name..!!');", true);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "LoadModalDiv()", true);
            return;           
        }
        if (ddlType.SelectedValue == "S")
        {
            string IdGlCoa = "";
            Supplier sup = new Supplier();
            sup.ComName = ""; sup.SupAddr1 = ""; sup.SupName = txtvalue.Text;
            sup.SupAddr2 = ""; sup.Designation = ""; sup.City = "";
            sup.SupMobile = txtMobile.Text; sup.State = ""; sup.SupPhone = "";
            sup.PostCode = ""; sup.Fax = ""; sup.Country = "";
            sup.Email = txtEmail.Text; sup.SupGroup = "3"; sup.Active = "True";
            sup.SupCode = IdManager.GetNextID("supplier", "Code").ToString().PadLeft(7, '0');
            sup.LoginBy = Session["user"].ToString();
            IdGlCoa = IdManager.getAutoIdWithParameter("402", "GL_SEG_COA", "SEG_COA_CODE", "4020000", "0000", "4");
            sup.GlCoa = IdGlCoa;
            SupplierManager.CreateSupplier(sup);
            //Gl_COA(IdGlCoa);
            string queryLoc = "select '' ID,'' ContactName  union select ID ,ContactName from Supplier order by 1";
            util.PopulationDropDownList(ddlSupplier, "Supplier", queryLoc, "ContactName", "ID");
        }
        
        txtvalue.Text = txtMobile.Text = txtEmail.Text = "";
        ddlType.SelectedIndex = -1;
    }
    private void CashReport()
    {
        string filename = "PV_" + txtGRNO.Text;
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        Document document = new Document(PageSize.LEGAL.Rotate(), 50f, 50f, 40f, 40f);
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
        gif.ScalePercent(50f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        
        dth.AddCell(cell);

       
        string StatusChk = "";
        cell = new PdfPCell(new Phrase("  add: " + Session["add1"] + Session["add12"], FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);

        cell = new PdfPCell(new Phrase(ddlSupplier.SelectedItem.Text, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        if (ddlChequeStatus.SelectedValue == "P") { StatusChk = "PENDING CHEQUE STATEMENT"; }
        else if (ddlChequeStatus.SelectedValue == "A") { StatusChk = "HONOURED CHEQUE STATEMENT"; } else { StatusChk = "DISHONOURED CHEQUE STATEMENT"; }
        cell = new PdfPCell(new Phrase("CASH STATEMENT", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
       
        dth.AddCell(cell);
        
        cell = new PdfPCell(new Phrase(" Mobile Number : " + ViewState["Phone"], FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
       
        dth.AddCell(cell);
        document.Add(dth);

        float[] vn = new float[2] { 10, 90 };
        PdfPTable VNO = new PdfPTable(vn);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(FormatHeaderPhrase("Voucher No: "));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 0;
        cell.BorderWidth = 0f;       
        VNO.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(txtGRNO.Text));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 0;
        cell.BorderWidth = 0f;      
        VNO.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 0;
        cell.BorderWidth = 0f;
        VNO.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 0;
        cell.BorderWidth = 0f;
        VNO.AddCell(cell);

        document.Add(VNO);

       

        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        dtempty.AddCell(cell);
         document.Add(dtempty);

        float[] widthdtl = new float[10] { 45, 20, 45, 30, 20, 20, 23, 22, 20,  20 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Particulars Of Goods"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Qnty"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Rate"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 3;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);        

        //cell = new PdfPCell(FormatHeaderPhrase("Shipment No"));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //cell.Rowspan = 2;
        //cell.PaddingTop = 10;
        //pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Status"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Supplier Rate"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Additional (%)"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Payment Rate"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);       

        DataTable dt = (DataTable)Session["purdtl"];
        //DataRow dr1 = dt.NewRow();
        //dt.Rows.Add(dr1);
        int Serial = 1;
        decimal totQty = 0;
        decimal tot = 0;
        foreach (DataRow dr in dt.Rows)
        {
            if (dr["ID"].ToString() != "" && dr["item_desc"].ToString() != "")
            {
                cell = new PdfPCell(FormatPhrase(Serial.ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                //string GRN = "";
                //if (Serial == 1)
                //{ GRN = txtGRNODate.Text; }
                cell = new PdfPCell(FormatPhrase(txtGRNODate.Text));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);


                cell = new PdfPCell(FormatPhrase(dr["item_desc"].ToString()));
                cell.HorizontalAlignment = 0;
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

                decimal total = Convert.ToDecimal(dr["item_rate"]) * Convert.ToDecimal(dr["qnty"]);
                decimal totAdd = (total) + ((total * Convert.ToDecimal(dr["Additional"])) / 100);
                decimal totaddPer = ((total * Convert.ToDecimal(dr["Additional"])) / 100);

                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_rate"]).ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Additional"]).ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_rate"]).ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(totAdd.ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);                
                

               // cell = new PdfPCell(FormatPhrase(txtSiftment.Text));
                //cell.HorizontalAlignment = 2;
                //cell.VerticalAlignment = 1;
                //cell.BorderColor = BaseColor.LIGHT_GRAY;
                //pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(ddlChequeStatus.SelectedItem.Text));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                //tot += Convert.ToDecimal(dr["Total"]);
                totQty += Convert.ToDecimal(dr["qnty"]);

                Serial++;
            }
        }

        cell = new PdfPCell(FormatPhrase("Total Qty."));
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(totQty.ToString("N0")));
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        //cell.Colspan = 3;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(""));
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Border = 0;
        cell.Colspan = 5;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 9;
        pdtdtl.AddCell(cell);
        document.Add(pdtdtl);

        PdfPTable pdt1 = new PdfPTable(1);
        cell.FixedHeight = 10f;
        cell.Border = 0;
        pdt1.AddCell(cell);


        cell = SignatureFormat(document, cell);       

        document.Close();
        Response.Flush();
        Response.End();
    }

    private void BankReport()
    {
        string filename = "PV_" + txtGRNO.Text;
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        Document document = new Document(PageSize.LEGAL.Rotate(), 50f, 50f, 40f, 40f);
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
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        string StatusChk = "";
        if (ddlChequeStatus.SelectedValue == "P") { StatusChk = "PENDING CHEQUE STATEMENT"; }
        else if (ddlChequeStatus.SelectedValue == "A") { StatusChk = "HONOURED CHEQUE STATEMENT"; } else { StatusChk = "DISHONOURED CHEQUE STATEMENT"; }
        cell = new PdfPCell(new Phrase(StatusChk, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(ddlSupplier.SelectedItem.Text + "  add: " + ViewState["Address"], FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(" Mobile Number : " + ViewState["Phone"], FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] widthdtl = new float[15] { 15, 20, 45, 30, 20, 20, 23, 22, 20, 20, 20, 20, 20, 20, 20 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Particulars Of Goods"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Qnty"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Rate"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 3;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Cheque Status"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);

        //cell = new PdfPCell(FormatHeaderPhrase("Shipment No"));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //cell.Rowspan = 2;
        //cell.PaddingTop = 10;
        //pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Status"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Supplier Rate"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Additional (%)"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Payment Rate"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Issue Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Cheque No"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Payment Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Bank"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        DataTable dt = (DataTable)Session["purdtl"];
        //DataRow dr1 = dt.NewRow();
        //dt.Rows.Add(dr1);
        int Serial = 1;
        decimal totQty = 0;
        decimal tot = 0;
        foreach (DataRow dr in dt.Rows)
        {
            if (dr["ID"].ToString() != "" && dr["item_desc"].ToString() != "")
            {
                cell = new PdfPCell(FormatPhrase(Serial.ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                //string GRN = "";
                //if (Serial == 1)
                //{ GRN = txtGRNODate.Text; }
                cell = new PdfPCell(FormatPhrase(txtGRNODate.Text));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);


                cell = new PdfPCell(FormatPhrase(dr["item_desc"].ToString()));
                cell.HorizontalAlignment = 0;
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

                decimal total = Convert.ToDecimal(dr["item_rate"]) * Convert.ToDecimal(dr["qnty"]);
                decimal totAdd = (total) + ((total * Convert.ToDecimal(dr["Additional"])) / 100);
                decimal totaddPer = ((total * Convert.ToDecimal(dr["Additional"])) / 100);

                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_rate"]).ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Additional"]).ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase((totaddPer).ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(totAdd.ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(txtGRNODate.Text));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(txtChequeNo.Text));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(txtChequeDate.Text));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(ddlBank.SelectedItem.Text));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

            //    cell = new PdfPCell(FormatPhrase(txtSiftment.Text));
                //cell.HorizontalAlignment = 2;
                //cell.VerticalAlignment = 1;
                //cell.BorderColor = BaseColor.LIGHT_GRAY;
                //pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(ddlChequeStatus.SelectedItem.Text));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                //tot += Convert.ToDecimal(dr["Total"]);
                //totQty += Convert.ToDecimal(dr["qnty"]);

                Serial++;
            }
        }

        cell = new PdfPCell(FormatPhrase(""));
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 9;
        pdtdtl.AddCell(cell);
        document.Add(pdtdtl);

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
   
    
    //protected void txtShiftmentNo_TextChanged(object sender, EventArgs e)
    //{
    //    DataTable dt = ShiftmentAssignManager.GetShowShiftmentAssignOnSearch(txtShiftmentNo.Text);
    //    if (dt.Rows.Count > 0)
    //    {
    //        txtShiftmentNo.Text = dt.Rows[0]["ShiftmentNO"].ToString();            
    //        lblShiftmentID.Text = dt.Rows[0]["ID"].ToString();
    //        txtRemarks.Focus();
    //    }
    //}

    protected void txtSearchSuplier_TextChanged(object sender, EventArgs e)
    {
        // int ID = IdManager.GetShowSingleValueInt("ID", " Supplier where Upper(Code+'-'+ContactName+'-'+Mobile)=upper('" + txtSearchSuplier.Text + "')");
        DataTable Dt = IdManager.GetShowDataTable("Select ID,Code,ContactName,Mobile from Supplier where Upper(Code+'-'+ContactName+'-'+Mobile)=upper('" + txtSearchSuplier.Text + "')");
        if (Dt.Rows.Count > 0)
        {
            lblSupplierID.Text = Dt.Rows[0]["ID"].ToString();
            txtSearchSuplier.Text = Dt.Rows[0]["ContactName"].ToString();
        }
        else
        {
            lblSupplierID.Text = "";
            txtSearchSuplier.Text = "";
        }
        PVI_UP.Update();
        PVIesms_UP.Update();
    }
    protected void lbSearch_Click(object sender, EventArgs e)
    {
        DataTable dt = PurchaseVoucherManager.GetPurchaseVoucherHistory(txtID.Text, txtSearchChallanNo.Text, txtFromDate.Text, txtToDAte.Text, lblSupplierID.Text);
        if (dt.Rows.Count > 0)
        {
            dgPVMst.DataSource = dt;
            Session["PvMst"] = dt;
            dgPVMst.DataBind();
        }

        else
        {
            dgPVMst.DataSource = null;
            Session["PvMst"] = null;
            dgPVMst.DataBind();
        }
        PVI_UP.Update();
        PVIesms_UP.Update();
        UPPaymentMtd.Update();
    }
    protected void lbClear_Click(object sender, EventArgs e)
    {
        txtID.Text = txtSearchChallanNo.Text = txtFromDate.Text = txtToDAte.Text = txtSearchGrNO.Text = txtSearchSuplier.Text = lblSupplierID.Text = string.Empty;
        DataTable dt = PurchaseVoucherManager.GetShowPurchaseMst();
        dgPVMst.DataSource = dt;
        Session["PvMst"] = dt;
        dgPVMst.DataBind();

        PVI_UP.Update();
        PVIesms_UP.Update();
        UPPaymentMtd.Update();
    }
}