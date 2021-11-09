using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using autouniv;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text.html.simpleparser;
using System.Text;
using OldColor;
using System.Diagnostics;

public partial class frmBercode : System.Web.UI.Page
{
    public static Permis per;
    //public static ReportDocument rpt;
    //public static decimal priceDr = 0;
    //public static decimal priceCr = 0;
    private int dtlRecordNum = 0;
    private BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();
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
            ViewState["ID"] = "";
            Session["purdtl"] = null;
            getEmptyDtl();
        }
    }
    private static DataTable dtColor = new DataTable();
    private static DataTable dtSize = new DataTable();
    private void getEmptyDtl()
    {
        dgPODetailsDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ID", typeof(string));
        dtDtlGrid.Columns.Add("item_code", typeof(string));
        dtDtlGrid.Columns.Add("item_desc", typeof(string));     
        dtDtlGrid.Columns.Add("item_rate", typeof(string));
        dtDtlGrid.Columns.Add("qnty", typeof(string));
        dtDtlGrid.Columns.Add("StkQty", typeof(string));
        dtDtlGrid.Columns.Add("Color", typeof(string));
        dtDtlGrid.Columns.Add("Size", typeof(string));
        dtDtlGrid.Columns.Add("ColorName", typeof(string));
        dtDtlGrid.Columns.Add("SizeName", typeof(string));
        dtDtlGrid.Columns.Add("Tax", typeof(string));
        dtDtlGrid.Columns.Add("Description", typeof(string));
        dtDtlGrid.Columns.Add("SecurityCode", typeof(string));
        dtDtlGrid.Columns.Add("StyleNo", typeof(string));
        dtDtlGrid.Columns.Add("Category", typeof(string));
        dtDtlGrid.Columns.Add("BranchSalePice", typeof(string));
        DataRow dr = dtDtlGrid.NewRow();
        dtDtlGrid.Rows.Add(dr);
        dgPODetailsDtl.DataSource = dtDtlGrid;
        Session["purdtl"] = dtDtlGrid;
        dgPODetailsDtl.DataBind();
    }
    public DataTable Color()
    {
        string ItemsID = "";
        dtColor = IdManager.GetShowDataTable("SELECT t1.[ColorID],t2.ColorName  FROM [ItemColor] t1 inner join ColorInfo t2 on t2.ID=t1.ColorID where t1.[DeleteBy] IS NULL and t1.ItemID='" + ViewState["ID"].ToString() + "'");
        DataRow dr = dtColor.NewRow();
        dtColor.Rows.InsertAt(dr, 0);
        return dtColor;
    }
    public DataTable Size()
    {
        string ItemsID = "";
        dtSize =
            IdManager.GetShowDataTable(
                "SELECT t1.SizeID,t2.SizeName  FROM ItemSize t1 inner join SizeInfo t2 on t2.ID=t1.SizeID where t1.[DeleteBy] IS NULL and t1.ItemID='" +
                ViewState["ID"].ToString() + "'");
        DataRow dr = dtSize.NewRow();
        dtSize.Rows.InsertAt(dr, 0);
        return dtSize;
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {


        

        string filename = "Bercode-" + Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        //Document document = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
        Document document = new Document();
        float pheight = (float) 68;
        float pwidth = (float) 120;
        document = new Document(new iTextSharp.text.Rectangle( pheight,pwidth).Rotate(), 4f, 2f, 0f, 0f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        PdfPCell cell;


        //float pheight = (float)(1 / 2.54) * 72;
        //float pwidth = (float)(1.5 / 2.54) * 72;
        //document = new Document(new iTextSharp.text.Rectangle(pwidth, pheight).Rotate(), 10f, 10f, 10f, 10f);

        float[] widthdtl = new float[2] { 2,90 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;


        barcode.Alignment = BarcodeLib.AlignmentPositions.CENTER;
        int W = 445;
        int H = 120;

        BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;
        barcode.IncludeLabel = false;
        barcode.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
        barcode.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
        DataTable dt = (DataTable)Session["purdtl"];
        ViewState["Desc"] = null;
        ViewState["SecurityCode"] = null;
        //ViewState["ColorName"] = null;
        string Desc = "", SecurityCode = "";
        
        foreach (DataRow dr in dt.Rows)
        {
            decimal tot = Convert.ToDecimal(dr["qnty"].ToString());
            if (tot<=0)
            {
                
                Response.Clear();

                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Empty Bercode Quentity ..!!');", true);
                return;
            }
            if (ViewState["Desc"] == null)
            {
                Desc = IdManager.GetShowSingleValueString("description", "ID", "Item", dr["ID"].ToString());
            }
            //if (ViewState["SecurityCode"] == null)
            //{
            //    SecurityCode = IdManager.GetShowSingleValueString("SecurityCode", "ID", "Item", dr["ID"].ToString());
            //}
            //if (ViewState["ColorName"] == null)
            //{
            //    ColorName = IdManager.GetShowSingleValueString("ColorName", "t1.[ItemID]", "[ItemColor] t1 inner join Item t2 on t2.ID=t1.ItemID inner join ColorInfo t3 on t3.ID=t1.ColorID", dr["ID"].ToString());
            //}
            if (dr["ID"].ToString() != "")
            {
                System.Drawing.Image generatedBarcode = barcode.Encode(type, dr["item_code"].ToString(), System.Drawing.Color.Black, System.Drawing.Color.White, W, H);
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                generatedBarcode.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                byte[] logo = stream.ToArray();
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
                gif.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                gif.ScalePercent(26f); 

               
                for (int i = 0; i < Convert.ToInt32(tot); i = i + 1)
                {

                   // string gg = Session["ShotName"].ToString();
                    var subTable = new PdfPTable(new float[2] { 40,60 });
                    //subTable.AddCell(
                    //    new PdfPCell(
                    //        FormatPhrase("Image"))
                    //    {
                    //        //Colspan = 2,
                    //        Border = 0,
                    //        HorizontalAlignment = 0
                    //    });
                    //subTable.AddCell(
                    //    new PdfPCell(
                    //        FormatPhrase(dr["SizeName"].ToString()))
                    //    {
                    //       // Colspan = 2,
                    //        Border = 0,
                    //        HorizontalAlignment = 0
                    //    });
                    //subTable.AddCell(
                    //    new PdfPCell(
                    //        FormatPhrase(Desc+" "+SecurityCode))
                    //    {
                    //        Colspan = 2,
                    //        Border = 0,
                    //        HorizontalAlignment = 0
                    //    });
                    //subTable.AddCell(
                    //    new PdfPCell(
                    //        FormatPhrase(dr["ColorName"].ToString()))
                    //    {
                    //        Colspan = 2,
                    //        Border = 0,
                    //        HorizontalAlignment = 0
                    //    });
                    
                    subTable.AddCell(
                    new PdfPCell(new Phrase(dr["item_desc"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD)))
                     {
                         Colspan = 2,
                         Border = 0,
                         HorizontalAlignment = 0
                     });
                    //subTable.AddCell(
                    //    new PdfPCell(
                    //        FormatPhrase(dr["item_desc"].ToString()))
                    //    {
                    //        Colspan = 2,
                    //        Border = 0,
                    //        HorizontalAlignment = 1
                    //    });
                    
                    subTable.AddCell(new PdfPCell(gif) { Colspan = 2, Border = 0, HorizontalAlignment = 0 });

                    cell = new PdfPCell(new Phrase(dr["StyleNo"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD)));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 1;
                    cell.BorderWidth = 0f;
                    subTable.AddCell(cell);
                    //subTable.AddCell(
                    //    new PdfPCell(
                    //        FormatPhrase(dr["StyleNo"].ToString()))
                    //    {
                    //        Colspan = 1,
                    //        Border = 0,
                            
                    //        HorizontalAlignment = 0
                    //    });
                    cell = new PdfPCell(new Phrase(" BDT = " + dr["BranchSalePice"].ToString() + " /=", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD)));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 1;
                    cell.BorderWidth = 0f;
                    subTable.AddCell(cell);
                    //subTable.AddCell(
                    //    new PdfPCell(FormatPhrase("BDT = " + dr["item_rate"].ToString() + " /="))
                    //    {
                    //        Colspan = 1,
                    //        Border = 0,
                    //        HorizontalAlignment = 0
                    //    });


                    pdtdtl.AddCell(new PdfPCell(new Phrase(""))
                        
                         {
                         Colspan = 1,
                         Border = 0,
                         HorizontalAlignment = 0
                     });

                    PdfPCell cellBody = new PdfPCell(subTable);
                    cellBody.BorderWidth = 0; //<--- This is what sets the border for the nested table
                    pdtdtl.AddCell(cellBody);

                    //if (i + 2 <= Convert.ToInt32(tot))
                    //{
                    //    subTable = new PdfPTable(new float[2] {60, 20});
                    //    subTable.AddCell(
                    //        new PdfPCell(
                    //            FormatPhrase(Session["ShotName"].ToString() + " - " +
                    //                         Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK"))
                    //        {
                    //            Colspan = 2,
                    //            Border = 0,
                    //            HorizontalAlignment = 1
                    //        });
                    //    subTable.AddCell(new PdfPCell(gif) {Colspan = 2, Border = 0, HorizontalAlignment = 1});
                    //    subTable.AddCell(
                    //        new PdfPCell(FormatPhrase(dr["item_code"].ToString() + "-" + dr["item_desc"].ToString()))
                    //        {
                    //            Colspan = 2,
                    //            Border = 0,
                    //            HorizontalAlignment = 1
                    //        });
                    //    cellBody = new PdfPCell(subTable);
                    //    cellBody.BorderWidth = 0; //<--- This is what sets the border for the nested table
                    //    pdtdtl.AddCell(cellBody);
                    //}
                    //else
                    //{
                    //    cell = new PdfPCell(FormatPhrase(""));
                    //    cell.Border = 0;
                    //    pdtdtl.AddCell(cell);
                    //}
                    //if (i + 3 <= Convert.ToInt32(tot))
                    //{
                    //    subTable = new PdfPTable(new float[2] {60, 20});
                    //    subTable.AddCell(
                    //        new PdfPCell(
                    //            FormatPhrase(Session["ShotName"].ToString() + " - " +
                    //                         Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK"))
                    //        {
                    //            Colspan = 2,
                    //            Border = 0,
                    //            HorizontalAlignment = 1
                    //        });
                    //    subTable.AddCell(new PdfPCell(gif) {Colspan = 2, Border = 0, HorizontalAlignment = 1});
                    //    subTable.AddCell(
                    //        new PdfPCell(FormatPhrase(dr["item_code"].ToString() + "-" + dr["item_desc"].ToString()))
                    //        {
                    //            Colspan = 2,
                    //            Border = 0,
                    //            HorizontalAlignment = 1
                    //        });
                    //    subTable.HorizontalAlignment = 1;
                    //    cellBody = new PdfPCell(subTable);
                    //    cellBody.BorderWidth = 0; //<--- This is what sets the border for the nested table
                    //    pdtdtl.AddCell(cellBody);
                    //}
                    //else
                    //{
                    //    cell = new PdfPCell(FormatPhrase(""));
                    //    cell.Border = 0;
                    //    pdtdtl.AddCell(cell);
                    //}

                    //cell = new PdfPCell(FormatPhrase(""));
                    //cell.Border = 0;
                    //cell.Colspan = 3;
                    //cell.FixedHeight = 15f;
                    //pdtdtl.AddCell(cell);
                }

            }
        }
        document.Add(pdtdtl);
        document.Close();
        Response.Flush();
        Response.End();
    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
    protected void txtItemDesc_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dtdtl = (DataTable)Session["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        DataTable dt = ItemManager.GetItemsBercodeNew(((TextBox)gvr.FindControl("txtItemDesc")).Text, Session["user"].ToString());
        if (dt.Rows.Count > 0)
        {
            //DataRow[] rows = dtdtl.Select("ID = " + ((DataRow)dt.Rows[0])["ID"].ToString() + " ");
            //// DataRow drr = dtdtl.AsEnumerable().SingleOrDefault(r => r.Field<int?>("ItemsID") ==Convert.ToInt32(((DataRow)dt.Rows[0])["ItemsID"].ToString()));
            //if (rows != null)
            //{
            //    if (rows.Length > 0)
            //    {
            //        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('This items already added...!!!');", true);
            //        ((TextBox)gvr.FindControl("txtItemDesc")).Text = "";
            //        ((TextBox)gvr.FindControl("txtItemDesc")).Focus();
            //        return;
            //    }
            //}
            dtdtl.Rows.Remove(dr);
            dr = dtdtl.NewRow();
            dr["ID"] = ((DataRow)dt.Rows[0])["ID"].ToString();
            ViewState["ID"] = ((DataRow)dt.Rows[0])["ID"].ToString();
            dr["item_desc"] = ((DataRow)dt.Rows[0])["item_desc"].ToString();
            dr["item_code"] = ((DataRow)dt.Rows[0])["Code"].ToString();          
            dr["item_rate"] = ((DataRow)dt.Rows[0])["UnitPrice"].ToString();
           // dr["Tax"] = ((DataRow)dt.Rows[0])["Rate"].ToString();
            dr["StkQty"] = ((DataRow)dt.Rows[0])["Quantity"].ToString();
            dr["qnty"] = "0";
            //dr["Description"] = ((DataRow)dt.Rows[0])["Description"].ToString();
            dr["SecurityCode"] = ((DataRow)dt.Rows[0])["SecurityCode"].ToString();
            dr["StyleNo"] = ((DataRow)dt.Rows[0])["StyleNo"].ToString();
            dr["BranchSalePice"] = ((DataRow)dt.Rows[0])["BranchSalesPrice"].ToString();
            
            dr["Color"] = "";
            dr["Size"] = "";
            dr["ColorName"] = "";
            dr["SizeName"] = "";
            dr["Category"] = "";

            dtdtl.Rows.InsertAt(dr, gvr.DataItemIndex);
          
            //Color();
            //Size();
        }
        dgPODetailsDtl.DataSource = dtdtl;
        dgPODetailsDtl.DataBind();
        //ShowFooterTotal();
        ((TextBox)dgPODetailsDtl.Rows[dgPODetailsDtl.Rows.Count - 1].FindControl("txtQnty")).Focus();
    }
    protected void dgPurDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.Footer)
            {                
                //e.Row.Cells[8].Attributes.Add("style", "display:none");
                e.Row.Cells[3].Attributes.Add("style", "display:none");
                //e.Row.Cells[3].Attributes.Add("style", "display:none");
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
            dtDtlGrid.Rows.RemoveAt(dgPODetailsDtl.Rows[e.RowIndex].DataItemIndex);
            if (dtDtlGrid.Rows.Count > 0)
            {
                string found = "";
                foreach (DataRow drf in dtDtlGrid.Rows)
                {
                    if (drf["ID"].ToString() == "" && drf["item_desc"].ToString() == "")
                    {
                        found = "Y";
                    }
                }
                if (found == "")
                {
                    DataRow dr = dtDtlGrid.NewRow();
                    dtDtlGrid.Rows.Add(dr);
                }
                dgPODetailsDtl.DataSource = dtDtlGrid;
                dgPODetailsDtl.DataBind();
            }
            else
            {
                getEmptyDtl();
            }           
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Your session is over. Try it again!!');", true);
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
            //dr["Color"] = ((DropDownList)gvr.FindControl("ddlColor")).SelectedValue;
            //dr["Size"] = ((DropDownList)gvr.FindControl("ddlSize")).SelectedValue;
            //dr["ColorName"] = ((DropDownList)gvr.FindControl("ddlColor")).SelectedItem.Text;
            //dr["SizeName"] = ((DropDownList)gvr.FindControl("ddlSize")).SelectedItem.Text;
            dr["Category"] = IdManager.GetShowSingleValueString("t2.Name as Category", "t1.ID", "Item t1  inner join Category t2 on t1.CategoryID=t2.ID", dr["ID"].ToString());
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
        //if (found == "")
        //{
        //    DataRow drd = dt.NewRow();
        //    dt.Rows.Add(drd);
        //}
        dgPODetailsDtl.DataSource = dt;
        dgPODetailsDtl.DataBind();      
        ((TextBox)dgPODetailsDtl.Rows[dgPODetailsDtl.Rows.Count - 1].FindControl("txtItemDesc")).Focus();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }

    private void Bercode()
    {
        string filename = "Bercode";
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        Document document = new Document(PageSize.A4, 50f, 50f, 40f, 40f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        PdfPCell cell;

        float[] widthdtl = new float[3] { 30, 30, 30 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;


        barcode.Alignment = BarcodeLib.AlignmentPositions.CENTER;
        int W = 550;
        int H = 160;

        BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;
        barcode.IncludeLabel = false;
        barcode.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
        barcode.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
        DataTable dt = (DataTable)Session["purdtl"];

        foreach (DataRow dr in dt.Rows)
        {
            if (dr["ID"].ToString() != "")
            {
                decimal tot = Convert.ToDecimal(dr["qnty"].ToString());
                for (int i = 0; i < Convert.ToInt32(tot); i = i + 3)
                {

                    System.Drawing.Image generatedBarcode = barcode.Encode(type, dr["item_code"].ToString(), System.Drawing.Color.Black, System.Drawing.Color.White, W, H);
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    generatedBarcode.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    byte[] logo = stream.ToArray();
                    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
                    gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                    gif.ScalePercent(20f);
                    //new PdfPCell(FormatPhrase(""));
                    var subTable = new PdfPTable(new float[2] { 60, 20 });
                    //subTable.AddCell(new PdfPCell(FormatPhrase("Netsoft Solution Ltd.")) { Border = 0, HorizontalAlignment = 1 });
                    //subTable.AddCell(new PdfPCell(FormatPhrase(dr["Tax"].ToString())) { Border = 0, HorizontalAlignment = 0 });
                    //subTable.AddCell(new PdfPCell(gif) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                    //subTable.AddCell(new PdfPCell(FormatPhrase(dr["item_desc"].ToString())) { Border = 0, HorizontalAlignment = 1 });
                    //subTable.AddCell(new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Border = 0, HorizontalAlignment = 0 });                             
                    ////pdtdtl.AddCell(subTable);
                    subTable.AddCell(new PdfPCell(FormatPhrase(Session["org"].ToString().Substring(0, 5) + "    " + Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                    // subTable.AddCell(new PdfPCell(FormatPhrase()) { Border = 0, HorizontalAlignment = 0 });
                    subTable.AddCell(new PdfPCell(gif) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                    subTable.AddCell(new PdfPCell(FormatPhrase(dr["item_code"].ToString() + "-" + dr["item_desc"].ToString())) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });

                    PdfPCell cellBody = new PdfPCell(subTable);
                    cellBody.BorderWidth = 0; //<--- This is what sets the border for the nested table
                    pdtdtl.AddCell(cellBody);

                    if (i + 2 <= Convert.ToInt32(tot))
                    {
                        subTable = new PdfPTable(new float[2] { 60, 20 });
                        subTable.AddCell(new PdfPCell(FormatPhrase(Session["org"].ToString().Substring(0, 5) + "    " + Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        //subTable.AddCell(new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Border = 0, HorizontalAlignment = 0 });
                        subTable.AddCell(new PdfPCell(gif) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        subTable.AddCell(new PdfPCell(FormatPhrase(dr["item_code"].ToString() + "-" + dr["item_desc"].ToString())) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        cellBody = new PdfPCell(subTable);
                        cellBody.BorderWidth = 0; //<--- This is what sets the border for the nested table
                        pdtdtl.AddCell(cellBody);
                    }
                    else
                    {
                        cell = new PdfPCell(FormatPhrase(""));
                        cell.Border = 0;
                        pdtdtl.AddCell(cell);
                    }
                    if (i + 3 <= Convert.ToInt32(tot))
                    {
                        subTable = new PdfPTable(new float[2] { 60, 20 });
                        subTable.AddCell(new PdfPCell(FormatPhrase(Session["org"].ToString().Substring(0, 5) + "    " + Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        // subTable.AddCell(new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Border = 0, HorizontalAlignment = 0 });
                        subTable.AddCell(new PdfPCell(gif) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        subTable.AddCell(new PdfPCell(FormatPhrase(dr["item_code"].ToString() + "-" + dr["item_desc"].ToString())) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        subTable.HorizontalAlignment = 1;
                        cellBody = new PdfPCell(subTable);
                        cellBody.BorderWidth = 0; //<--- This is what sets the border for the nested table
                        pdtdtl.AddCell(cellBody);
                    }
                    else
                    {
                        cell = new PdfPCell(FormatPhrase(""));
                        cell.Border = 0;
                        pdtdtl.AddCell(cell);
                    }

                    cell = new PdfPCell(FormatPhrase(""));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.FixedHeight = 15f;
                    pdtdtl.AddCell(cell);
                }

            }
        }
        document.Add(pdtdtl);
        document.Close();
        Response.Flush();
        Response.End();
    }

    private void getDataTable()
    {
        dgPODetailsDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("Code", typeof(string));
        dtDtlGrid.Columns.Add("ItemName", typeof(string));
        dtDtlGrid.Columns.Add("ItemSize", typeof(string));
        dtDtlGrid.Columns.Add("ItemColor", typeof(string));
        dtDtlGrid.Columns.Add("SalePrice", typeof(string));
        dtDtlGrid.Columns.Add("Quantity", typeof(string));
        dtDtlGrid.Columns.Add("Description", typeof(string));
        dtDtlGrid.Columns.Add("SecurityCode", typeof(string));
        dtDtlGrid.Columns.Add("Category", typeof(string));
        dtDtlGrid.Columns.Add("SubCategory", typeof(string));
        dtDtlGrid.Columns.Add("Barcode", typeof (byte[]));
        dtDtlGrid.Columns.Add("Image", typeof(byte[]));
        DataRow dr = dtDtlGrid.NewRow();
        dtDtlGrid.Rows.Add(dr);
        dgPODetailsDtl.DataSource = dtDtlGrid;
        Session["Barcode"] = dtDtlGrid;
        dgPODetailsDtl.DataBind();
    }
    
    private byte[] GetBytesFromImage(String imageFile)
    {
        MemoryStream ms = new MemoryStream();
        System.Drawing.Image img = System.Drawing.Image.FromFile(imageFile);
        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

        return ms.ToArray();
    }
}