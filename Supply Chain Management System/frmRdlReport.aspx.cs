using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Microsoft.Reporting.WebForms;
using autouniv;
using OldColor;

public partial class frmRdlReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlCatagory.DataSource = MajorCategoryManager.GetMajorCats();
            ddlCatagory.DataTextField = "mjr_desc";
            ddlCatagory.DataValueField = "mjr_code";
            ddlCatagory.DataBind();
            ddlCatagory.Items.Insert(0, "");
            ddlSupplierpur.Items.Clear();
            string queryLoc = "select '' ID,'' ContactName  union select ID ,ContactName from Supplier order by 1";
            util.PopulationDropDownList(ddlSupplierpur, "CostType", queryLoc, "ContactName", "ID");
            pnlPurchase.Visible = true;
            pnlOrderReceived.Visible = false;
            pnlITS.Visible = false;
            pnlStock.Visible = false;
        }
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        //------------------------------------- Stock Report -------------------------
        if (rdbReportType.SelectedValue == "Stk")
        {

            DataTable dt = PurchaseVoucherManager.GetShowREportInformation("", "", "", lblItemIDStk.Text, "", ddlCatagory.SelectedValue, ddlSubCatagory.SelectedValue, "", "1", "Stk");


            if (dt.Rows.Count > 0)
            {
                DataTable dt2 = IdManager.GetShowDataTable("Select  * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                Panel1.Visible = true;

                // ReportParameter s2=new ReportParameter("ReportDate",);
                string RptType = "";

                rptOthersreportviewer.ProcessingMode = ProcessingMode.Local;
                rptOthersreportviewer.LocalReport.ReportPath = Server.MapPath("Report/rptItemStockReport.rdlc");
                rptOthersreportviewer.LocalReport.DataSources.Clear();
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportType", ""));
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy")));
                //if (txtStartDategoods.Text != "")
                //{
                //    string date = "Date : " + txtStartDategoods.Text;
                //    if (txtEndDateGoods.Text != "")
                //    {
                //        date = date + " To :" + txtEndDateGoods.Text;
                //    }
                //    rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("Date", date));
                //}
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("CommonTable", dt));
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                rptOthersreportviewer.LocalReport.Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }
        //------------------------------------- Profit Report -------------------------
        if (rdbReportType.SelectedValue == "PL")
        {

            DataTable dt = PurchaseVoucherManager.PlGetShowREportInformation(txtFromDateITS.Text, txtToDateITS.Text, "", lblItemIDItemStatus.Text, "", "", "", "", "1", "PL");


            if (dt.Rows.Count > 0)
            {
                DataTable dt2 = IdManager.GetShowDataTable("Select  * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                Panel1.Visible = true;

                // ReportParameter s2=new ReportParameter("ReportDate",);
                string RptType = "";

                rptOthersreportviewer.ProcessingMode = ProcessingMode.Local;
                rptOthersreportviewer.LocalReport.ReportPath = Server.MapPath("Report/rptProfitandLoss.rdlc");
                rptOthersreportviewer.LocalReport.DataSources.Clear();
               // rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy")));
                //if (txtStartDategoods.Text != "")
                //{
                //    string date = "Date : " + txtStartDategoods.Text;
                //    if (txtEndDateGoods.Text != "")
                //    {
                //        date = date + " To :" + txtEndDateGoods.Text;
                //    }
                //    rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("Date", date));
                //}
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("CommonTable", dt));
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                rptOthersreportviewer.LocalReport.Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }
        //------------------------------------- Stock Report With Picture -------------------------

        if (rdbReportType.SelectedValue == "StkPic")
        {

            DataTable dt = PurchaseVoucherManager.GetShowREportInformation("", "", "", lblItemIDStk.Text, "", ddlCatagory.SelectedValue, ddlSubCatagory.SelectedValue, "", "1", "Stk");


            if (dt.Rows.Count > 0)
            {
                DataTable dt2 = IdManager.GetShowDataTable("Select  * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                Panel1.Visible = true;

                // ReportParameter s2=new ReportParameter("ReportDate",);
                string RptType = "";

                rptOthersreportviewer.ProcessingMode = ProcessingMode.Local;
                rptOthersreportviewer.LocalReport.ReportPath = Server.MapPath("Report/rptItemStockWithItem.rdlc");
                rptOthersreportviewer.LocalReport.DataSources.Clear();
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportType", ""));
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy")));
                //if (txtStartDategoods.Text != "")
                //{
                //    string date = "Date : " + txtStartDategoods.Text;
                //    if (txtEndDateGoods.Text != "")
                //    {
                //        date = date + " To :" + txtEndDateGoods.Text;
                //    }
                //    rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("Date", date));
                //}
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("CommonTable", dt));
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                rptOthersreportviewer.LocalReport.Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }

            //------------------------------------- Consumption Report -------------------------

        else if (rdbReportType.SelectedValue == "Cn")
        {

            DataTable dt = PurchaseVoucherManager.GetShowREportInformation(txtFromDateORC.Text, txtToDateORC.Text, "", lblItemIDORC.Text, "", "", "", "", "1", "Cn");


            if (dt.Rows.Count > 0)
            {
                DataTable dt2 = IdManager.GetShowDataTable("Select  * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                Panel1.Visible = true;

                // ReportParameter s2=new ReportParameter("ReportDate",);
                string RptType = "";

                rptOthersreportviewer.ProcessingMode = ProcessingMode.Local;
                rptOthersreportviewer.LocalReport.ReportPath = Server.MapPath("Report/rptConsumptionReport.rdlc");
                rptOthersreportviewer.LocalReport.DataSources.Clear();
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportType", ""));
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy")));
                //if (txtStartDategoods.Text != "")
                //{
                //    string date = "Date : " + txtStartDategoods.Text;
                //    if (txtEndDateGoods.Text != "")
                //    {
                //        date = date + " To :" + txtEndDateGoods.Text;
                //    }
                //    rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("Date", date));
                //}
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("CommonTable", dt));
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                rptOthersreportviewer.LocalReport.Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }

        else if (rdbReportType.SelectedValue == "SL")
        {

            DataTable dt = PurchaseVoucherManager.GetShowREportInformation(txtFromDateORC.Text, txtToDateORC.Text, "", lblItemIDORC.Text, "", "", "", "", "1", "SL");


            if (dt.Rows.Count > 0)
            {
                DataTable dt2 = IdManager.GetShowDataTable("Select  * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                Panel1.Visible = true;

                // ReportParameter s2=new ReportParameter("ReportDate",);
                string RptType = "";

                rptOthersreportviewer.ProcessingMode = ProcessingMode.Local;
                rptOthersreportviewer.LocalReport.ReportPath = Server.MapPath("Report/rptSalesReport.rdlc");
                rptOthersreportviewer.LocalReport.DataSources.Clear();
                //if (txtStartDategoods.Text != "")
                //{
                //    string date = "Date : " + txtStartDategoods.Text;
                //    if (txtEndDateGoods.Text != "")
                //    {
                //        date = date + " To :" + txtEndDateGoods.Text;
                //    }
                //    rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("Date", date));
                //}
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("CommonTable", dt));
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                rptOthersreportviewer.LocalReport.Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }

        else if (rdbReportType.SelectedValue == "SLD")
        {

            DataTable dt = PurchaseVoucherManager.GetShowREportInformation(txtFromDateORC.Text, txtToDateORC.Text, "", lblItemIDORC.Text, "", "", "", "", "2", "SL");


            if (dt.Rows.Count > 0)
            {
                DataTable dt2 = IdManager.GetShowDataTable("Select  * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                Panel1.Visible = true;

                // ReportParameter s2=new ReportParameter("ReportDate",);
                string RptType = "";

                rptOthersreportviewer.ProcessingMode = ProcessingMode.Local;
                rptOthersreportviewer.LocalReport.ReportPath = Server.MapPath("Report/rptsalesdetails.rdlc");
                rptOthersreportviewer.LocalReport.DataSources.Clear();
                //if (txtStartDategoods.Text != "")
                //{
                //    string date = "Date : " + txtStartDategoods.Text;
                //    if (txtEndDateGoods.Text != "")
                //    {
                //        date = date + " To :" + txtEndDateGoods.Text;
                //    }
                //    rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("Date", date));
                //}
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("CommonTable", dt));
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                rptOthersreportviewer.LocalReport.Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }

            //------------------------------------- Purchase Report -------------------------
        else if (rdbReportType.SelectedValue == "pur")
        {

            DataTable dt = PurchaseVoucherManager.GetShowREportInformation(txtDatefromPur.Text, txtDateToPur.Text, ddlSupplierpur.SelectedValue, lblItemIDPurchase.Text, "", "", "", "", "1", "pur");


            if (dt.Rows.Count > 0)
            {
                DataTable dt2 = IdManager.GetShowDataTable("Select  * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                Panel1.Visible = true;

                // ReportParameter s2=new ReportParameter("ReportDate",);
                string RptType = "";

                rptOthersreportviewer.ProcessingMode = ProcessingMode.Local;
                rptOthersreportviewer.LocalReport.ReportPath = Server.MapPath("Report/rptPurchaseReport.rdlc");
                rptOthersreportviewer.LocalReport.DataSources.Clear();
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportType", ""));
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy")));
                //if (txtStartDategoods.Text != "")
                //{
                //    string date = "Date : " + txtStartDategoods.Text;
                //    if (txtEndDateGoods.Text != "")
                //    {
                //        date = date + " To :" + txtEndDateGoods.Text;
                //    }
                //    rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("Date", date));
                //}
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("CommonTable", dt));
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                rptOthersreportviewer.LocalReport.Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }
        //------------------------------------- Purchase Return Report -------------------------
        else if (rdbReportType.SelectedValue == "PReR")
        {

            DataTable dt = PurchaseVoucherManager.GetShowREportInformation(txtDatefromPur.Text, txtDateToPur.Text, ddlSupplierpur.SelectedValue, lblItemsIDPur.Text, "", "", "", "", "1", "PReR");


            if (dt.Rows.Count > 0)
            {

                DataTable dt2 = IdManager.GetShowDataTable("Select  * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                Panel1.Visible = true;

                // ReportParameter s2=new ReportParameter("ReportDate",);
                string RptType = "";

                rptOthersreportviewer.ProcessingMode = ProcessingMode.Local;
                rptOthersreportviewer.LocalReport.ReportPath = Server.MapPath("Report/rptPurchaseReturnReport.rdlc");
                rptOthersreportviewer.LocalReport.DataSources.Clear();
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportType", ""));
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy")));
                //if (txtStartDategoods.Text != "")
                //{
                //    string date = "Date : " + txtStartDategoods.Text;
                //    if (txtEndDateGoods.Text != "")
                //    {
                //        date = date + " To :" + txtEndDateGoods.Text;
                //    }
                //    rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("Date", date));
                //}
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("CommonTable", dt));
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                rptOthersreportviewer.LocalReport.Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }
        //------------------------------------- Damages Stock Report -------------------------
        else if (rdbReportType.SelectedValue == "DStk")
        {

            DataTable dt = PurchaseVoucherManager.GetShowREportInformation("", "", "", lblItemIDStk.Text, "", ddlCatagory.SelectedValue, ddlSubCatagory.SelectedValue, "", "1", "DStk");

            if (dt.Rows.Count > 0)
            {
                DataTable dt2 = IdManager.GetShowDataTable("Select  * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                Panel1.Visible = true;

                // ReportParameter s2=new ReportParameter("ReportDate",);
                string RptType = "";

                rptOthersreportviewer.ProcessingMode = ProcessingMode.Local;
                rptOthersreportviewer.LocalReport.ReportPath = Server.MapPath("Report/rptDemageStock.rdlc");
                rptOthersreportviewer.LocalReport.DataSources.Clear();
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportType", ""));
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy")));
                //if (txtStartDategoods.Text != "")
                //{
                //    string date = "Date : " + txtStartDategoods.Text;
                //    if (txtEndDateGoods.Text != "")
                //    {
                //        date = date + " To :" + txtEndDateGoods.Text;
                //    }
                //    rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("Date", date));
                //}
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("CommonTable", dt));
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                rptOthersreportviewer.LocalReport.Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }
        //------------------------------------- Damages ReceivedS Report -------------------------
        else if (rdbReportType.SelectedValue == "DRec")
        {

            DataTable dt = PurchaseVoucherManager.GetShowREportInformation(txtFromDateORC.Text, txtToDateORC.Text, "", lblItemIDORC.Text, "", "", "", "", "1", "DRec");



            if (dt.Rows.Count > 0)
            {
                DataTable dt2 = IdManager.GetShowDataTable("Select  * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                Panel1.Visible = true;

                // ReportParameter s2=new ReportParameter("ReportDate",);
                string RptType = "";

                rptOthersreportviewer.ProcessingMode = ProcessingMode.Local;
                rptOthersreportviewer.LocalReport.ReportPath = Server.MapPath("Report/rptDemagesReceivedInformation.rdlc");
                rptOthersreportviewer.LocalReport.DataSources.Clear();
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportType", ""));
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy")));
                //if (txtStartDategoods.Text != "")
                //{
                //    string date = "Date : " + txtStartDategoods.Text;
                //    if (txtEndDateGoods.Text != "")
                //    {
                //        date = date + " To :" + txtEndDateGoods.Text;
                //    }
                //    rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("Date", date));
                //}
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("CommonTable", dt));
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                rptOthersreportviewer.LocalReport.Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }
        //------------------------------------- Item Status Report -------------------------
        else if (rdbReportType.SelectedValue == "ITS")
        {

            DataTable dt = PurchaseVoucherManager.GetShowREportInformation(txtFromDateITS.Text, txtToDateITS.Text, "", lblItemIDItemStatus.Text, "", "", "", "", "1", "ITS");



            if (dt.Rows.Count > 0)
            {
                DataTable dt2 = IdManager.GetShowDataTable("Select  * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                Panel1.Visible = true;

                // ReportParameter s2=new ReportParameter("ReportDate",);
                string RptType = "";

                rptOthersreportviewer.ProcessingMode = ProcessingMode.Local;
                rptOthersreportviewer.LocalReport.ReportPath = Server.MapPath("Report/rptItemStatusReport.rdlc");
                rptOthersreportviewer.LocalReport.DataSources.Clear();
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportType", ""));
                rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy")));
                //if (txtStartDategoods.Text != "")
                //{
                //    string date = "Date : " + txtStartDategoods.Text;
                //    if (txtEndDateGoods.Text != "")
                //    {
                //        date = date + " To :" + txtEndDateGoods.Text;
                //    }
                //    rptOthersreportviewer.LocalReport.SetParameters(new ReportParameter("Date", date));
                //}
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("CommonTable", dt));
                rptOthersreportviewer.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                rptOthersreportviewer.LocalReport.Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }

    }
    protected void txtSearchItemPur_TextChanged(object sender, EventArgs e)
    {
        DataTable dtitem = ConsumptionManager.GetItemInformation(txtSearchItemPur.Text);
        if (dtitem.Rows.Count > 0)
        {
            lblItemIDPurchase.Text = dtitem.Rows[0]["ID"].ToString();
            txtSearchItemPur.Text = dtitem.Rows[0]["Name"].ToString();

        }
        else if (txtSearchItemPur.Text == "")
        {
            txtSearchItemPur.Text = lblItemIDPurchase.Text = "";
        }

    }
    protected void txtSearchItemORC_TextChanged(object sender, EventArgs e)
    {
        DataTable dtitem = ConsumptionManager.GetItemInformation(txtSearchItemORC.Text);
        if (dtitem.Rows.Count > 0)
        {
            
            lblItemIDORC.Text = dtitem.Rows[0]["ID"].ToString();
            txtSearchItemORC.Text = dtitem.Rows[0]["Name"].ToString();

        }
        else if (txtSearchItemORC.Text == "")
        {
            txtSearchItemORC.Text = lblItemIDORC.Text = "";
        }

    }
    protected void rdbReportType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rdbReportType.SelectedValue == "pur" || rdbReportType.SelectedValue == "PReR")
        {
            pnlPurchase.Visible = true;
            pnlITS.Visible = false;
            pnlOrderReceived.Visible = false;
            pnlStock.Visible = false;
        }
        else if (rdbReportType.SelectedValue == "Cn" || rdbReportType.SelectedValue == "DRec" || rdbReportType.SelectedValue == "IST")
        {
            pnlPurchase.Visible = false;
            pnlOrderReceived.Visible = true;
            pnlStock.Visible = false;
            pnlITS.Visible = false;
        }
        else if (rdbReportType.SelectedValue == "SL" || rdbReportType.SelectedValue == "SLD" )
        {
            pnlPurchase.Visible = false;
            pnlOrderReceived.Visible = true;
            pnlStock.Visible = false;
            pnlITS.Visible = false;
        }

        else if (rdbReportType.SelectedValue == "Stk" || rdbReportType.SelectedValue == "StkPic" || rdbReportType.SelectedValue == "DStk" )
        {
            pnlPurchase.Visible = false;
            pnlITS.Visible = false;
            pnlOrderReceived.Visible = false;
            pnlStock.Visible = true;
        }
        else if (rdbReportType.SelectedValue == "ITS" || rdbReportType.SelectedValue == "PL")
        {
            pnlPurchase.Visible = false;
            pnlITS.Visible = true;
            pnlOrderReceived.Visible = false;
            pnlStock.Visible = false;
        }
    }
    protected void ddlCatagory_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlSubCatagory.DataSource = SubMajorCategoryManager.GetSubMajorCategories(ddlCatagory.SelectedValue);
        ddlSubCatagory.DataTextField = "Name";
        ddlSubCatagory.DataValueField = "ID";
        ddlSubCatagory.DataBind();

        ddlSubCatagory.Items.Insert(0, "");
    }
    protected void btnInventoryClear_Click(object sender, EventArgs e)
    {
        lblItemsIDPur.Text = lblItemIDStk.Text = lblItemIDPurchase.Text = lblItemIDORC.Text = txtDatefromPur.Text = txtDateToPur.Text = txtFromDateORC.Text = txtSearchItemORC.Text = txtSearchItemPur.Text = txtSearchItemStk.Text = txtToDateORC.Text = "";
        ddlCatagory.SelectedIndex = ddlSubCatagory.SelectedIndex = ddlSupplierpur.SelectedIndex = -1;
    }
    protected void txtSearchItemStk_TextChanged(object sender, EventArgs e)
    {
        DataTable dtitem = ConsumptionManager.GetItemInformation(txtSearchItemStk.Text);
        if (dtitem.Rows.Count > 0)
        {

            lblItemIDStk.Text = dtitem.Rows[0]["ID"].ToString();
            txtSearchItemStk.Text = dtitem.Rows[0]["Name"].ToString();

        }
        else if (txtSearchItemStk.Text=="")
        {
            txtSearchItemStk.Text = lblItemIDStk.Text = "";
        }
    }
    protected void txtSearchItemITS_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = ItemManager.GetStockItems(txtSearchItemITS.Text);
        if (dt.Rows.Count > 0)
        {
            lblItemIDItemStatus.Text = dt.Rows[0]["ID"].ToString();
            txtSearchItemITS.Text = dt.Rows[0]["item_code"].ToString() + '-' + dt.Rows[0]["item_desc"].ToString();
        }
        else if (txtSearchItemITS.Text == "")
        {
            txtSearchItemITS.Text = lblItemIDItemStatus.Text= "";
        }

    }
}