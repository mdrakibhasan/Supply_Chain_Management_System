using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for Users
/// </summary>
public class Users
{
    public string UserName;
    public string Password;
    public string Description;
    public string UserGrp;
    public string Status;
    public string EmpNo, BranchID, Email, ID, Type;

    public Users()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Users(DataRow dr)
    {
        if (dr["Type"].ToString() != String.Empty)
        {
            this.Type = dr["Type"].ToString();
        }
        if (dr["ID"].ToString() != String.Empty)
        {
            this.ID = dr["ID"].ToString();
        }
        if (dr["user_name"].ToString() != String.Empty)
        {
            this.UserName = dr["user_name"].ToString();
        }
        if (dr["password"].ToString() != String.Empty)
        {
            this.Password = dr["password"].ToString();
        }
        if (dr["description"].ToString() != String.Empty)
        {
            this.Description = dr["description"].ToString();
        }
        if (dr["user_grp"].ToString() != String.Empty)
        {
            this.UserGrp = dr["user_grp"].ToString();
        }
        if (dr["status"].ToString() != String.Empty)
        {
            this.Status = dr["status"].ToString();
        }
        if (dr["emp_no"].ToString() != String.Empty)
        {
            this.EmpNo = dr["emp_no"].ToString();
        }
        if (dr["BranchID"].ToString() != String.Empty)
        {
            this.BranchID = dr["BranchID"].ToString();
        }
        if (dr["Email"].ToString() != String.Empty)
        {
            this.Email = dr["Email"].ToString();
        }
    }

    public string GroupID { get; set; }

    public string LoginBy { get; set; }
}
