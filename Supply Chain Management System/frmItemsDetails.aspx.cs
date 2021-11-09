using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class frmItemsDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DataTable dt = ClsItemDetailsManager.GetShowItemsInfo("");
            dgItems.DataSource = dt;
            Session["DT"] = dt;
            dgItems.DataBind();
        }
    }

    protected void dgItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
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
    protected void dgItems_SelectedIndexChanged(object sender, EventArgs e)
    {
        string Id;
        Id = dgItems.SelectedRow.Cells[2].Text.Trim();
        ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>SubmitToParent('" + Id + "');</script>");
    }
    protected void dgItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgItems.DataSource = Session["DT"];
        dgItems.PageIndex = e.NewPageIndex;       
        dgItems.DataBind();
    }
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = ClsItemDetailsManager.GetShowItemsInfo(txtSearch.Text);
        dgItems.DataSource = dt;
        Session["DT"] = dt;
        dgItems.DataBind();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        DataTable dt = ClsItemDetailsManager.GetShowItemsInfo("");
        dgItems.DataSource = dt;
        Session["DT"] = dt;
        dgItems.DataBind();
    }
}