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
using autouniv;
using OldColor;
using ClosedXML.Excel;
using System.IO;

public partial class StockItemsDetails : System.Web.UI.Page
{
    private static Permis per;
    protected void Page_Load(object sender, EventArgs e)
    {
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
            DataTable dt = ClsItemDetailsManager.GetShowItemsDetails(txtName.Text,rdbItemType.SelectedValue);
            dgItems.DataSource = dt;
            ViewState["STK"] = dt;
            dgItems.DataBind();

            
            DataTable dtBranch = ClsItemDetailsManager.GetBranchInfo();
            util.PopulationDropDownList(ddlItemStockType, "Name", "ID", dtBranch);

            ShowFooterTotal();
        }
    }
    private void ShowFooterTotal()
    {
        try
        {
            if (dgItems.Rows.Count > 0)
            {
                GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
                TableCell cell;
                DataTable dtdtl = (DataTable)ViewState["STK"];
                double totQty = 0, totPrice = 0;
                foreach (DataRow dr in dtdtl.Rows)
                {
                    if (!string.IsNullOrEmpty(dr["ClosingStock"].ToString()))
                    {
                        totQty += Convert.ToDouble(dr["ClosingStock"]);
                    }
                }
                cell = new TableCell();
                cell.Text = "Total";
                cell.ColumnSpan = 5;
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = totQty.ToString("N0");
                cell.HorizontalAlign = HorizontalAlign.Center;
                row.Cells.Add(cell);
                row.Font.Bold = true;

                //cell = new TableCell();
                //cell.Text = "";
                //cell.HorizontalAlign = HorizontalAlign.Right;
                //row.Cells.Add(cell);
                //cell.ColumnSpan = 2;
                //row.Font.Bold = true;
                dgItems.Controls[0].Controls.Add(row);
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
    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect("StockItemsDetails.aspx?mno=5.24");
    }
    //protected void txtName_TextChanged(object sender, EventArgs e)
    //{
    //    if(ddlItemStockType.SelectedValue !="")
    //    {
    //        DataTable dt = ClsItemDetailsManager.GetShowBranchWiseItemsDetaile(txtName.Text, ddlItemStockType.SelectedValue,rdbItemType.SelectedValue);
    //        dgItems.DataSource = dt;
    //        ViewState["STK"] = dt;
    //        dgItems.DataBind();
    //        ShowFooterTotal();
    //        txtName.Focus();
    //    }
    //    else
    //    {
    //        DataTable dt = ClsItemDetailsManager.GetShowItemsDetails(txtName.Text,ddlItemStockType.SelectedValue);
    //        dgItems.DataSource = dt;
    //        ViewState["STK"] = dt;
    //        dgItems.DataBind();
    //        ShowFooterTotal();
    //        txtName.Focus();
    //    }
        
    //}
    protected void dgItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgItems.DataSource = ViewState["STK"];
        dgItems.PageIndex = e.NewPageIndex;
        dgItems.DataBind();
    }
    protected void dgItems_SelectedIndexChanged(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "newWindow", "window.open('frmImageView.aspx?ID=" + dgItems.SelectedRow.Cells[9].Text + " &ItemsName=" + dgItems.SelectedRow.Cells[1].Text + "','_blank','status=1,toolbar=0,menubar=0,location=1,top=250,left=250px,width=500px,height=250px,directories=no,status=no, linemenubar=no,scrollbars=no,resizable=no ,modal=yes');", true); 
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (rbReportType.SelectedValue.Equals("P"))
        {
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename='Stock-Items'.pdf");
            Document document = new Document(PageSize.A4.Rotate(), 50f, 50f, 40f, 40f);
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();

            PdfPCell cell;
            byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(20f);

            float[] titwidth = new float[2] {10, 200};
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(gif);
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(Session["org"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(Session["add1"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(Session["add2"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase("Total Items Stock",
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 30f;
            dth.AddCell(cell);
            document.Add(dth);
            LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
            document.Add(line);

            float[] widthdtl = new float[9] {10, 50, 20, 20, 20, 20, 20, 20, 20};
            PdfPTable pdtdtl = new PdfPTable(widthdtl);
            pdtdtl.WidthPercentage = 100;

            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 15f;
            cell.Border = 0;
            cell.Colspan = 9;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Serial"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.PaddingTop = 12;
            cell.Rowspan = 2;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Items Name & Code"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.PaddingTop = 12;
            cell.FixedHeight = 20f;
            cell.Rowspan = 2;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Catagory"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.PaddingTop = 12;
            cell.FixedHeight = 20f;
            cell.Rowspan = 2;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Sub Catagory"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.PaddingTop = 12;
            cell.FixedHeight = 20f;
            cell.Rowspan = 2;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Unit Price"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.PaddingTop = 12;
            cell.Rowspan = 2;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Opening Stock"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;

            cell.Colspan = 2;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);



            cell = new PdfPCell(FormatHeaderPhrase("Closing Stock"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;

            cell.Colspan = 2;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatHeaderPhrase("Qnty"));
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

            cell = new PdfPCell(FormatHeaderPhrase("Qnty"));
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


            int Serial = 1;
            decimal totOPStk = 0;
            decimal totOpAmt = 0;
            decimal totCloseStk = 0;
            decimal totCloseAmt = 0;
            DataTable dtdtl = (DataTable) ViewState["STK"];
            foreach (DataRow dr in dtdtl.Rows)
            {
                cell = new PdfPCell(FormatPhrase(Serial.ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 20f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                Serial++;
                cell = new PdfPCell(FormatPhrase(dr["Items"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Catagory"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["SubCat"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["UnitPrice"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["OpeningStock"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;

                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["OpeningAmount"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["ClosingStock"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;

                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["ClosingAmount"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                totOPStk += Convert.ToDecimal(dr["OpeningStock"]);
                totOpAmt += Convert.ToDecimal(dr["OpeningAmount"]);
                totCloseStk += Convert.ToDecimal(dr["ClosingStock"]);
                totCloseAmt += Convert.ToDecimal(dr["ClosingAmount"]);
            }
            cell = new PdfPCell(FormatHeaderPhrase("Total"));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 5;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(totOPStk.ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(totOpAmt.ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(totCloseStk.ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(totCloseAmt.ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            document.Add(pdtdtl);
            document.Close();
            Response.Flush();
            Response.End();
        }
        if(rbReportType.SelectedValue.Equals("E"))
        {
            DataTable dtdtl = (DataTable)ViewState["STK"];

            string filename = "Iotal_Items_Stock-" + Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dtdtl, "Iotal_Items_Stock");
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }

    protected void dgItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[5].Attributes.Add("style", "display:none");
                e.Row.Cells[6].Attributes.Add("style", "display:none");
                e.Row.Cells[8].Attributes.Add("style", "display:none");
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
    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgItems.DataSource = ViewState["ddt"];
        dgItems.PageIndex = e.NewPageIndex;
        dgItems.DataBind();
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {              
                e.Row.Cells[8].Attributes.Add("style", "display:none");
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
   
    protected void Button2_Click(object sender, EventArgs e)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename='Stock-Items'.pdf");
        Document document = new Document(PageSize.A4.Rotate(), 50f, 50f, 40f, 40f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(20f);

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
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;       
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;        
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase("Total Items Stock", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;        
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        float[] widthdtl = new float[8] { 10, 30, 40, 20, 20, 20, 20, 20 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 15f;
        cell.Border = 0;
        cell.Colspan = 8;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        //cell.PaddingTop = 12;  
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Party Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.PaddingTop = 12;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Code & Items Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.PaddingTop = 12;
        cell.FixedHeight = 20f;        
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Catagory"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.PaddingTop = 12;
        cell.FixedHeight = 20f;        
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Sub Catagory"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
       // cell.PaddingTop = 12;
        cell.FixedHeight = 20f;        
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Unit Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
       // cell.PaddingTop = 12;        
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Opening Stock"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;        
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Closing Stock"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;     
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);  

        int Serial = 1;
        decimal totOPStk = 0; decimal totOpAmt = 0; decimal totCloseStk = 0; decimal totCloseAmt = 0;
        DataTable dtdtl = (DataTable)ViewState["ddt"];
        foreach (DataRow dr in dtdtl.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;
            cell = new PdfPCell(FormatPhrase(dr["PartyName"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["Items"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Catagory"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["SubCat"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["UnitPrice"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["OpeningStock"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatPhrase(dr["Quantity"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);           

            totOPStk += Convert.ToDecimal(dr["OpeningStock"]);
            totCloseStk += Convert.ToDecimal(dr["Quantity"]);            
        }
        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 6;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(totOPStk.ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);        
        cell = new PdfPCell(FormatHeaderPhrase(totCloseStk.ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);  
        document.Add(pdtdtl);
        document.Close();
        Response.Flush();
        Response.End();
    }

   
 

    protected void ddlItemStockType_SelectedIndexChanged1(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ddlItemStockType.SelectedValue))
        {
            DataTable dt = ClsItemDetailsManager.GetShowBranchWiseItemsDetaile(txtName.Text, ddlItemStockType.SelectedValue, rdbItemType.SelectedValue);
            dgItems.DataSource = dt;
            ViewState["STK"] = dt;
            dgItems.DataBind();
        }
        else
        {

            DataTable dt2 = ClsItemDetailsManager.GetShowItemsDetails(txtName.Text, rdbItemType.SelectedValue);
            dgItems.DataSource = dt2;
            ViewState["STK"] = dt2;
            dgItems.DataBind();
        }
    }

    protected void txtName_TextChanged(object sender, EventArgs e)
    {
       ddlItemStockType_SelectedIndexChanged1(sender,e);
    }
    protected void rdbItemType_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dt2 = ClsItemDetailsManager.GetShowItemsDetails(txtName.Text, rdbItemType.SelectedValue);
        dgItems.DataSource = dt2;
        ViewState["STK"] = dt2;
        dgItems.DataBind();
    }
}