using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
//using RunPower;
using System.Data;
using System.Configuration;
using autouniv;

public partial class DatabaseBackupUI : System.Web.UI.Page
{
    public static Permis per;
    SqlConnection con = new SqlConnection(DataManager.OraConnString());
    protected void Page_Load(object sender, EventArgs e)
    {       
        try
        {
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('" + ViewState.ViewStateID + "');", true);
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
                Response.Redirect("Home.aspx?sid=sam");
            }
        }
        catch
        {
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('"+ex.Message+"!!');", true);
            Response.Redirect("Default.aspx?sid=sam");
        }
        if (!IsPostBack)
        {

        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        SqlConnection con = new SqlConnection(DataManager.OraConnString());
        SqlCommand sqlcmd = new SqlCommand();
        try
        {
            string Drive = ConfigurationManager.AppSettings["BackupDriver"];
            string backupDIR = "" + Drive + ":\\BackUpDataBase";
            if (!System.IO.Directory.Exists(backupDIR))
            {
                System.IO.Directory.CreateDirectory(backupDIR);
            }

            con.Open();
            sqlcmd = new SqlCommand("backup database " + con.Database + " to disk='" + backupDIR + "\\" + con.Database + DateTime.Now.ToString("dd-MM-yyyy(hh_mmtt)") + ".Bak'", con);
            sqlcmd.ExecuteNonQuery();
            con.Close();
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Backup successfully..!!');", true);

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('"+ex+"');", true);

        }
        
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {

    }
}