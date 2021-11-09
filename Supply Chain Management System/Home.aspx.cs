using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using autouniv;
using OldColor;
using System.Net;
using System.Globalization;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.draw;
using System.IO;

public partial class Home : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null)
        {
            if (Session.SessionID != "" | Session.SessionID != null)
            {
                clsSession ses = clsSessionManager.getSession(Session.SessionID);
                if (ses != null)
                {
                    Session["user"] = ses.UserId;  Session["wnote"] = "Bonjour Mr. " + UsersManager.getUserName(ses.UserId);
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
        if (!IsPostBack)
        {
            try
            {
                if (int.Parse(Session["userlevel"].ToString()) > 0)
                {
                    pnlTask.Visible = true;                                    
                }
                else
                {
                    pnlTask.Visible = true;
                   
                }
                Panel2.Visible = false;
                txtSearchItemITS.Text = "";
                ((Label)Page.Master.FindControl("lblLogin")).Text = Session["wnote"].ToString();
                ((LinkButton)Page.Master.FindControl("lbLogout")).Visible = true;
                //var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
                //var response = myHttpWebRequest.GetResponse();
                //string todaysDates = response.Headers["date"];
              //// var clientZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
              // CultureInfo enUS = new CultureInfo("bn-BD"); 
                //DateTime date = DateTime.ParseExact(todaysDates,
                //                           "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                //                           CultureInfo.InvariantCulture.DateTimeFormat,
                //                           DateTimeStyles.AssumeUniversal);
                //string zoneId = "Bangladesh Standard Time";
                //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
                DateTime serverTime = DateTime.Now;
                DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "Bangladesh Standard Time");
                //DateTime date = TimeZoneInfo.ConvertTimeFromUtc(date, tzi);
                Session["date"] = _localTime;
                lblTime.Text = _localTime.ToString("hh:mm:ss tt");

                DataTable dt = PurchaseVoucherManager.GetShowREportInformation("", "", "", "", "", "", "", "", "1", "ITSAll");
                if (dt.Rows.Count>0)
                {
                    lblpurQty.Text = dt.Rows[0]["Data6"].ToString();
                    lblCnAmn.Text = dt.Rows[0]["Data3"].ToString();
                   lblCnQty.Text = dt.Rows[0]["Data7"].ToString();
                    //lblDmAmn.Text = dt.Rows[0]["Data4"].ToString();
                   //lblDmQty.Text = dt.Rows[0]["Data8"].ToString();
                    lblpurAmn.Text = dt.Rows[0]["Data1"].ToString();
                    lblpurReQty.Text = dt.Rows[0]["Data11"].ToString();
                    lblpurReAmn.Text = dt.Rows[0]["Data2"].ToString();

                    lblDmQty.Text = dt.Rows[0]["Data22"].ToString();
                    lblDmAmn.Text = dt.Rows[0]["Data32"].ToString();

                    lblsalesqty.Text = dt.Rows[0]["Data23"].ToString();
                    lblsaleamn.Text = dt.Rows[0]["Data33"].ToString();

                    lblStAmn.Text = dt.Rows[0]["Data12"].ToString();
                    lblStQty.Text = dt.Rows[0]["Data10"].ToString();
                    
                }

              
            }
            catch
            {
                Session["user"] = "";
                Session["pass"] = "";
                pnlTask.Visible = false;
                Response.Redirect("Default.aspx?sid=sam");
            }
            txtCpUserName.Text = Session["user"].ToString();
            lblTranStatus.Visible = false;

            txtSearchItemITS.Text = "";
            //pnlChangePass.Visible = false;            
        } 
    }
    
    protected void lbChangePassword_click(object sender, EventArgs e)
    {
        if (txtCpNewPass.Text != txtCpConfPass.Text)
        {
            lblTranStatus.Text = "New Password & Confirm Password are not same!!";
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
            ModalPopupExtenderLogin.Show();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "LoadModalDiv();", true);
        }
        else if (txtCpCurPass.Text == String.Empty)
        {
            lblTranStatus.Text = "Please provide current password!!";
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
            ModalPopupExtenderLogin.Show();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "LoadModalDiv();", true);
        }
        else if (txtCpNewPass.Text == String.Empty | txtCpConfPass.Text == String.Empty)
        {
            lblTranStatus.Text = "New password cannot be null!!";
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
            ModalPopupExtenderLogin.Show();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "LoadModalDiv();", true);
        }
        else if (txtCpNewPass.Text != String.Empty & txtCpConfPass.Text != String.Empty)
        {
            Users usr = UsersManager.getUser(txtCpUserName.Text.ToString().ToUpper());
            if (usr != null & usr.Password == txtCpCurPass.Text)
            {
                usr.LoginBy = Session["userID"].ToString();
                usr.Password = txtCpNewPass.Text;
                UsersManager.GetResetPassword(usr);
                lblTranStatus.Text = "Password has changed!!";
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Green;
                txtCpCurPass.Text = "";
                txtCpNewPass.Text = "";
                txtCpConfPass.Text = "";
            }
            else
            {
                lblTranStatus.Text = "Old password is not correct!!";
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
                ModalPopupExtenderLogin.Show();
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "LoadModalDiv();", true);
            }
        }
    }
    protected void lbCancel_Click(object sender, EventArgs e)
    {
       //pnlChangePass.Visible = false;
    }
    protected void lbChangePass_Click(object sender, EventArgs e)
    {
        pnlTask.Visible = true;
       
        pnlChangePass.Visible = true;
        txtCpUserName.Text = Session["user"].ToString();
        lblTranStatus.Visible = false;
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {

        //var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
        //var response = myHttpWebRequest.GetResponse();
        //string todaysDates = response.Headers["date"];

        //DateTime date = DateTime.ParseExact(todaysDates,
        //                           "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
        //                           CultureInfo.InvariantCulture.DateTimeFormat,
        //                           DateTimeStyles.AssumeUniversal);
        //string a = Session["date"].ToString();
        DateTime date = Convert.ToDateTime( Session["date"]);

        lblTime.Text = date.ToString("hh:mm:ss tt");
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (txtSearchItemITS.Text != "")
        {
            DataTable dt = ConsumptionManager.GetStatusBYItemStyleNo(txtSearchItemITS.Text, "", "");

            ViewState["ItemStatus"]=dt;
            if (dt.Rows.Count > 0)
            {
                DataTable dt2 = ConsumptionManager.GetItemInfobyStyleNo(txtSearchItemITS.Text);
                if (dt2.Rows.Count > 0)
                {

                    //imgEmp.ImageUrl = "data:image;base64," + Convert.ToBase64String((byte[])dt2.Rows[0]["Images"]); ;
                    lblItemName.Text = dt2.Rows[0]["Name"].ToString();
                    lblUOM.Text = dt2.Rows[0]["UOM"].ToString();
                    lblUnitPrice.Text = dt2.Rows[0]["UnitPrice"].ToString();
                  
                    lblSupplier.Text = dt2.Rows[0]["ContactName"].ToString();
                   // lblDesign.Text = dt2.Rows[0]["DesighNo"].ToString();
                    lblStyleNO.Text = dt2.Rows[0]["StyleNo"].ToString();
                  
                    //lblCategory.Text = dt2.Rows[0]["Category"].ToString();
                    //lblSubCategory.Text = dt2.Rows[0]["SubCategory"].ToString();
                }
                GridView1.DataSource = dt;
                GridView1.DataBind();
                Panel2.Visible = true;
            }
            else
            {
                GridView1.DataSource = null;
                GridView1.DataBind();
                Panel2.Visible = false;
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Data Not Found.!!');", true);
            }
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)ViewState["ItemStatus"];
        if (dt.Rows.Count > 0)
        {
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition",
                "attachment; filename='ItemStatus'.pdf");
            Document document = new Document(PageSize.A6, 10f, 10f, 10f, 10f);
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
            gif.ScalePercent(11f);

            float[] titwidth = new float[2] { 10, 200 };
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(gif);
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;

            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;

            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);
            
                cell =
                    new PdfPCell(new Phrase("ItemStatus",
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

            PdfPTable dtempty = new PdfPTable(1);
            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            dtempty.AddCell(cell);
            document.Add(dtempty);


            float[] widt = new float[6] { 27, 1, 33, 30, 1, 30 };
            PdfPTable dtl = new PdfPTable(widt);
            dtl.WidthPercentage = 100;


                    

            cell = new PdfPCell(FormatHeaderPhrase("Item Name:"));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(lblItemName.Text));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Colspan = 4;
            cell.Border = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Unite Price:"));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(lblUnitPrice.Text));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("UOM:"));
            cell.HorizontalAlignment = 2;
            cell.Border = 0;
            cell.VerticalAlignment = 1;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(lblUOM.Text));
            cell.HorizontalAlignment = 0;
            cell.Border = 0;
            cell.Border=0;
            cell.VerticalAlignment = 1;
            dtl.AddCell(cell);


           

            cell = new PdfPCell(FormatHeaderPhrase("StyleNo:"));
            cell.HorizontalAlignment = 2;
            cell.Border = 0;
            cell.VerticalAlignment = 1;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(lblStyleNO.Text));
            cell.HorizontalAlignment = 0;
            cell.Border = 0;
            cell.Border = 0;
            cell.VerticalAlignment = 1;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Supplier:"));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            dtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(lblSupplier.Text));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            dtl.AddCell(cell);

           


            cell = new PdfPCell(FormatPhrase(""));
            //cell.BorderWidth = 0f;
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            cell.Colspan = 6;
           
            dtl.AddCell(cell);

            document.Add(dtl);

            float[] widthdtl = new float[8] { 8, 30, 22, 19,27,19,17,18 };
            PdfPTable pdtdtl = new PdfPTable(widthdtl);
            pdtdtl.WidthPercentage = 100;
            int Serial = 1;
            
            
            cell = new PdfPCell(FormatHeaderPhrase("SL"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Description"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Date"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Purchase"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Consumption"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Damages"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Pur.Ret"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Stocks"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            
           

            decimal purQty = 0,CnQty=0,dmQty=0,PrQty=0,Stock=0;
            decimal tot = 0;
            foreach (DataRow dr in dt.Rows)
            {
                cell = new PdfPCell(FormatPhrase(Serial.ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                Serial++;

                cell = new PdfPCell(FormatPhrase(dr["Description"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Date"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["PurQty"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["CnQty"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["DmQty"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["PRQty"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Stock"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                purQty += Convert.ToDecimal(dr["PurQty"].ToString());
                CnQty += Convert.ToDecimal(dr["CnQty"].ToString());
                dmQty += Convert.ToDecimal(dr["DmQty"].ToString());
                PrQty += Convert.ToDecimal(dr["PRQty"].ToString());
                Stock = Convert.ToDecimal(dr["Stock"].ToString()); 
            }

            cell = new PdfPCell(FormatHeaderPhrase("Total"));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 3;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(purQty.ToString("N0")));
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatHeaderPhrase(CnQty.ToString("N0")));
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatHeaderPhrase(dmQty.ToString("N0")));
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatHeaderPhrase(PrQty.ToString("N0")));
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatHeaderPhrase(Stock.ToString("N0")));
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
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
            cell.Colspan = 8;
            pdtdtl.AddCell(cell);

            document.Add(pdtdtl);



            document.Close();
            Response.Flush();
            Response.End();
        }
    }

    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD));
    }

}