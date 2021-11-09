using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using OldColor;
using iTextSharp.text.pdf.draw;
using autouniv;
using System.Data.SqlClient;

public partial class rptProductin : System.Web.UI.Page
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
               // Response.Redirect("Home.aspx?sid=sam");
            }
        }

        catch
        {
           // Response.Redirect("Default.aspx?sid=sam");
        } 
        if (!IsPostBack)
        {
            lblFromDate.Visible = true;
            lblToDate.Visible = true;
            txtfromdate.Visible = true;
            txtToDate.Visible = true;
            lblUnitPriceFrom.Visible = false;
            lblUnitePriceTo.Visible = false;
            txtUnitePriceFrom.Visible = false;
            txtUnitePriceTo.Visible = false;
            ddlReportType.Visible = false;

        }
    }
    protected void ddlReportType_SelectedIndexChanged(object sender, EventArgs e)
    {
       
        

       
        if (ddlReportType.SelectedValue == "RD")
        {
            lblFromDate.Text = "From Date";
            lblToDate.Text = "To";
            txtfromdate.Visible = true;
            txtToDate.Visible = true;
            lblFromDate.Visible = true;
            lblToDate.Visible = true;
            lblUnitPriceFrom.Visible = false;
            lblUnitePriceTo.Visible = false;
            txtUnitePriceFrom.Visible = false;
            txtUnitePriceTo.Visible = false;
        }

        if (ddlReportType.SelectedValue == "ALL")
        {
            lblFromDate.Text = "From Date";
            lblToDate.Text = "To";
            lblFromDate.Visible = false;
            lblToDate.Visible = false;
            txtfromdate.Visible = false;
            txtToDate.Visible = false;
            lblUnitPriceFrom.Visible = false;
            lblUnitePriceTo.Visible = false;
            txtUnitePriceFrom.Visible = false;
            txtUnitePriceTo.Visible = false;
        }

    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {


        string per = "";


        if (txtfromdate.Visible == true)
        {
            if (string.IsNullOrEmpty(txtfromdate.Text))
            {
                per = "Y";
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Select Receive From Date !!');", true);
            }
        }

        if (txtToDate.Visible == true)
        {
            if (string.IsNullOrEmpty(txtToDate.Text))
            {
                per = "Y";
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Select Receive To Date !!');", true);
            }
        }


        if (per != "Y")
        {
            DataTable dt = rptProductionReport.GetProductionInformation("", txtProductionCode.Text, txtUnitePriceFrom.Text, txtUnitePriceTo.Text, txtBatchNo.Text, "", txtBrandName.Text, txtfromdate.Text, txtToDate.Text, "RD");
            CreatePDFReport(dt);


        }
    }
    private readonly itextFormatHeaderPhrase _itextFormatHeaderPhrase = new itextFormatHeaderPhrase();
    private void CreatePDFReport(DataTable dt)
    {

        {
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename=Production.pdf");
            Document document = new Document();
            document = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
            //pdfPage page = new pdfPage();
            //writer.PageEvent = page;
            document.Open();
            string logoo = "";
            if (Session["book"] != null)
            {
                logoo = Session["book"].ToString();
            }
            byte[] logo = GlBookManager.GetGlLogo(logoo);
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(20f);
            float[] titwidth = new float[2] { 10, 200 };
            PdfPCell cell;

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

            document.Add(dth);
            //****************************************

            PdfPTable dth2 = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;
            cell = new PdfPCell();
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;            
            cell.BorderWidth = 0f;
            dth2.AddCell(cell);
            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader14_BOLD("Production Information"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 15f;
            dth2.AddCell(cell);
            //**********************************************
            cell = new PdfPCell();
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            
            cell.BorderWidth = 0f;
            dth2.AddCell(cell);
            cell =
                new PdfPCell(_itextFormatHeaderPhrase.FormatHeader14_BOLD(" Total Production : "+dt.Rows.Count));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 15f;
            dth2.AddCell(cell);
            //**********************************************
            cell = new PdfPCell();
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
           
            cell.BorderWidth = 0f;
            dth2.AddCell(cell);
            cell =
                new PdfPCell();
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 15f;
            dth2.AddCell(cell);

            document.Add(dth2);


            float[] titwidthDtl = new float[7] { 10, 40, 30, 40,40,40,30 };
            PdfPTable dtl = new PdfPTable(titwidthDtl);
            dtl.WidthPercentage = 100;
            dtl.HeaderRows = 1;
            ///DataTable ddtDays = IdManager.GetShowDataTable("Select * from Days");

            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("SL"));
            // cell.Border = 1;
            cell.VerticalAlignment = 0;
            cell.HorizontalAlignment = 1;
            dtl.AddCell(cell);
            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("Product Name"));
            // cell.Border = 1;
            cell.VerticalAlignment = 0;
            cell.HorizontalAlignment = 1;
            dtl.AddCell(cell);

            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("Brand Name"));
            // cell.Border = 1;
            cell.VerticalAlignment = 0;
            cell.HorizontalAlignment = 1;
            dtl.AddCell(cell);

            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("Product Code"));
            // cell.Border = 1;
            cell.VerticalAlignment = 0;
            cell.HorizontalAlignment = 1;
            dtl.AddCell(cell);
            //cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("Expire Date"));
            //// cell.Border = 1;
            //cell.VerticalAlignment = 0;
            //cell.HorizontalAlignment = 1;
            //dtl.AddCell(cell);

            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("receive Date"));
            // cell.Border = 1;
            cell.VerticalAlignment = 0;
            cell.HorizontalAlignment = 1;
            dtl.AddCell(cell);

            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("Create Date"));
            // cell.Border = 1;
            cell.VerticalAlignment = 0;
            cell.HorizontalAlignment = 1;
            dtl.AddCell(cell);

            cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader10_BOLD("Quantity"));
            // cell.Border = 1;
            cell.VerticalAlignment = 0;
            cell.HorizontalAlignment = 1;
            dtl.AddCell(cell);
            int SL = 1;
           
                    foreach (DataRow dr in dt.Rows)
                    {
                        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(SL.ToString())) ;
                        //cell.Border = 1;
                        cell.VerticalAlignment = 0;
                        cell.HorizontalAlignment = 1;
                        dtl.AddCell(cell);
                        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["ItemName"].ToString()));
                        //cell.Border = 1;
                        cell.VerticalAlignment = 1;
                        cell.HorizontalAlignment = 0;
                        dtl.AddCell(cell);
                        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["BrandName"].ToString()));
                        // cell.Border = 1;
                        cell.VerticalAlignment = 0;
                        cell.HorizontalAlignment = 1;
                        dtl.AddCell(cell);
                        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["Code"].ToString()));
                        // cell.Border = 1;
                        cell.VerticalAlignment = 1;
                        cell.HorizontalAlignment = 0;
                        dtl.AddCell(cell);

                        //cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["ExpireDate"].ToString()));
                        //// cell.Border = 1;
                        //cell.VerticalAlignment = 1;
                        //cell.HorizontalAlignment = 0;
                        //dtl.AddCell(cell);

                        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["ReceivedDate"].ToString()));
                        // cell.Border = 1;
                        cell.VerticalAlignment = 1;
                        cell.HorizontalAlignment = 0;
                        dtl.AddCell(cell);

                        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["CreatedDate"].ToString()));
                        // cell.Border = 1;
                        cell.VerticalAlignment = 1;
                        cell.HorizontalAlignment = 0;
                        dtl.AddCell(cell);

                        cell = new PdfPCell(_itextFormatHeaderPhrase.FormatHeader8_NORMAL(dr["Quantity"].ToString()));
                        // cell.Border = 1;
                        cell.VerticalAlignment = 1;
                        cell.HorizontalAlignment = 1;
                        dtl.AddCell(cell);

                        SL++;
                    }
                
            
            document.Add(dtl);

            document.Close();
            Response.Flush();
            Response.End();
        }
    }

    
    protected void Clear_Click(object sender, EventArgs e)
    {

    }
}