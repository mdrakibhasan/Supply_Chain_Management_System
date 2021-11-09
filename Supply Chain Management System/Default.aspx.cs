using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using autouniv;
using System.IO;
using System.Configuration;
using System.Net.Mail;
using System.Net;

public partial class _Default : System.Web.UI.Page
{
    public int userLvl = 0;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["sid"] != null)
        {
            string RepType = Request.QueryString["sid"].ToString();
            if (RepType != "sam")
            {
                Response.Redirect("~/Default.aspx?sid=sam");
                //ClientScript.RegisterStartupScript(this.GetType(), "ale", "closeWindowNoPrompt();", true);
            }
        }
        else
        {
            Response.Redirect("~/Default.aspx?sid=sam");
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "closeWindowNoPrompt();", true);
        }
        if (!Page.IsPostBack)
        {
            //ddlBook.Items.Clear();
            //string queryBook = "select 'AMB' book_name, '' book_desc from dual  union select '*' book_name, 'New GL' book_desc from dual union select book_name,book_name book_desc from gl_set_of_books where book_status='A' order by 2 desc";
            
            //string queryBook = "select 'AMB' book_name, '' book_desc   union select '*' book_name, 'New GL' book_desc  union select book_name,book_name book_desc from gl_set_of_books where book_status='A' order by 2 desc";

            //util.PopulationDropDownList(ddlBook, "level", queryBook, "book_desc", "book_name");
            //ddlBook.SelectedValue = "AMB";
            ddlBook.Visible = false;
            //lbClose.Attributes.Add("onclick", "opener=self;window.close()");
            System.Type oType = System.Type.GetTypeFromProgID("InternetExplorer.Application");
            txtEmail.Visible = false;
            txtUserName.Enabled = txtPassword.Visible = true;
            txtUserName.Text = txtPassword.Text = txtEmail.Text = string.Empty;
            txtEmail.Enabled = false;
            LoginBtn.Text = "Login";
            txtUserName.Focus();
        }
    }
    bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }  
   
    protected void lblForget_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtUserName.Text))
        {
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Input User ID .!!!!');", true);
            return;
        }
        txtPassword.Text = txtEmail.Text = string.Empty;
        if (lblForget.Text.Equals("Forgot Password?"))
        {
            txtPassword.Visible =txtUserName.Enabled= false;
            txtEmail.Visible = true;
            lblForget.Text = "Go to login page?";
            LoginBtn.Text = "Send";
            Users usr = UsersManager.getUser(txtUserName.Text);
            if (usr != null)
            {
                txtEmail.Text = usr.Email;
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Not Find your E-mail address.contract your software provider.!!!!');", true);
                return;
            }
        }
        else
        {
            lblForget.Text = "Forgot Password?";
            txtPassword.Visible = txtUserName.Enabled = true;
            txtEmail.Visible = false;
            LoginBtn.Text = "Login";
            
             txtPassword.Text = txtEmail.Text = string.Empty;
        }
    }
    private void Email(string Subject, string Body, string Mail)
    {
        string EmailID = ConfigurationManager.AppSettings["Email"];
        string Password = ConfigurationManager.AppSettings["PassWord"];
        MemoryStream memoryStream = new MemoryStream();
        byte[] bytes = memoryStream.ToArray();
        using (MailMessage mm = new MailMessage(EmailID, Mail))
        {
            mm.Subject = Subject;
            //mm.Body = "Your Id = " + dt.Rows[0]["UserID"].ToString() + " & Password = " + dt.Rows[0]["Password"].ToString() + " are Verified";
            mm.Body = Body;
            // mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "143"));
            mm.IsBodyHtml = true;
            mm.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential(EmailID, Password);
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            smtp.Send(mm);
        }
    }
    protected void LoginBtn_Click1(object sender, EventArgs e)
    {
        System.Threading.Thread.Sleep(3000);
        if (LoginBtn.Text.Equals("Send"))
        {
            if (IsValidEmail(txtEmail.Text) == false)
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Incorrect E-mail Address..!!!!');", true);
            }

            lblForget.Text = "Forgot Password?";
            txtPassword.Visible = true;
            txtEmail.Visible = false;
            LoginBtn.Text = "Login";
            string NewPass = "";
            Users usr = UsersManager.getUser(txtUserName.Text.ToUpper());
            if (usr != null)
            {
                usr.LoginBy = usr.ID;
                int Val = DateTime.Now.Month + DateTime.Now.Year + DateTime.Now.Day + DateTime.Now.Minute + DateTime.Now.Second;
                usr.Password = txtUserName.Text + "@" + Convert.ToInt32(((Val - 10) * 2)).ToString();
                NewPass = txtUserName.Text + "@" + Convert.ToInt32(((Val - 10) * 2)).ToString();
                UsersManager.GetResetPassword(usr);
                Email("Password Recovery On Time :" + System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), "New password : " + NewPass, txtEmail.Text);
            }
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('New Password Send your email.!!');", true);
        }
        else
        {
            //if (ddlBook.SelectedValue != "")
            //{
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select ID,password,user_grp,description,EMP_NO,BranchID from [utl_userinfo] where upper(user_name)=upper('" + txtUserName.Text.Trim() + "') and status='A'";
            conn.Open();
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())

                    if (txtPassword.Text != "" && txtPassword.Text.Trim() == dReader["password"].ToString())
                    {
                        Session["user"] = txtUserName.Text;
                        Session["pass"] = txtPassword.Text;
                        Session["userID"] = dReader["ID"].ToString();
                        Session["EMP_NO"] = dReader["EMP_NO"].ToString();
                        Session["EMPNO"] = dReader["EMP_NO"].ToString();
                        userLvl = int.Parse(dReader["user_grp"].ToString());
                        Session["userlevel"] = userLvl.ToString();
                        Session["book"] = "AMB";
                        Session["Branch"] = dReader["BranchID"].ToString();
                        ViewState["user_grp"] = dReader["user_grp"].ToString();
                        string wnote = dReader["description"].ToString();
                        Session["wnote"] = wnote;
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText =
                            "Select book_desc,company_address1,company_address2,separator_type,ShortName from [gl_set_of_books] where book_name='" +
                            Session["book"].ToString() + "' ";
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
                                Session["ShotName"] = dReader["ShortName"].ToString();
                            }
                        }
                        clsSession ses = clsSessionManager.getLoginSession(txtUserName.Text.ToUpper(), "");
                        if (ses == null)
                        {
                            ses = new clsSession();
                            ses.UserId = txtUserName.Text.ToUpper();
                            ses.SessionTime = System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                            ses.SessionId = Session.SessionID;

                            ses.Mac = Server.HtmlEncode(Request.UserHostAddress);
                            clsSessionManager.CreateSession(ses);
                        }
                        if (chkRemarks.Checked)
                        {
                            Response.Cookies["useridLeb"].Value = txtUserName.Text;
                            Response.Cookies["pwd"].Value = txtPassword.Text;
                            Response.Cookies["useridLeb"].Expires = DateTime.Now.AddDays(15);
                            Response.Cookies["pwd"].Expires = DateTime.Now.AddDays(15);
                        }
                        else
                        {
                            Response.Cookies["useridLeb"].Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies["pwd"].Expires = DateTime.Now.AddDays(-1);

                        }

                        if (ViewState["user_grp"].ToString() == "11")
                        {
                            Response.Redirect("~/HomeLibrary.aspx");
                        }
                        else
                        {
                            Response.Redirect("~/Home.aspx");
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                            "alert('Incorrect Password....!!!!');", true);
                    }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Incorrect User ID ...!!!!');", true);
                Session["user"] = "";
                Session["pass"] = "";
                txtUserName.Focus();
            }
            //}
            //else
            //{
            //    Session["user"] = "";
            //    Session["pass"] = "";
            //    txtUserName.Focus();
            //}
        }
    }
}